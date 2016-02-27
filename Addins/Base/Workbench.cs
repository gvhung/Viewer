using System;
using System.ComponentModel;
using System.Windows.Forms;
using ICSharpCode.Core;
using System.Windows.Threading;

namespace Base
{
	public sealed class Workbench : Form
	{
		static Workbench instance;

		public static Workbench Instance => instance;

		public static void InitializeWorkbench(string[] args)
		{
			instance = new Workbench();

			if(args.Length == 1)
			{
				IViewContent content = DisplayBindingManager.CreateViewContent(args[0]);
				if (content != null)
				{
					instance.ShowContent(content);
				}
			}
		}
		
		MenuStrip menu;
		private TabControl tabControl1;
		Panel contentPanel;
		
		private Workbench()
		{
			InitializeComponent();

			// restore form location from last session
			FormLocationHelper.Apply(this, "StartupFormPosition", true);
			
			contentPanel = new Panel();
			contentPanel.Dock = DockStyle.Fill;
			this.Controls.Add(contentPanel);
			
			menu = new MenuStrip();
			MenuService.AddItemsToMenu(menu.Items, this, "/Workbench/MainMenu");

			this.Controls.Add(menu);
			
			// Start with an empty text file
			//ShowContent(new TextViewContent());
			
			// Use the Idle event to update the status of menu and toolbar items.
			Application.Idle += OnApplicationIdle;
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				Application.Idle -= OnApplicationIdle;
			}
			base.Dispose(disposing);
		}
		
		void OnApplicationIdle(object sender, EventArgs e)
		{
			// Use the Idle event to update the status of menu and toolbar.
			// Depending on your application and the number of menu items with complex conditions,
			// you might want to update the status less frequently.
			UpdateMenuItemStatus();
		}
		
		/// <summary>Update Enabled/Visible state of items in the main menu based on conditions</summary>
		void UpdateMenuItemStatus()
		{
			foreach (ToolStripItem item in menu.Items) {
				if (item is IStatusUpdate)
					(item as IStatusUpdate).UpdateStatus();
			}
		}
		
		/// <summary>The active view content</summary>
		IViewContent viewContent;
		
		public IViewContent ActiveViewContent {
			get {
				return viewContent;
			}
		}
		
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			if (!e.Cancel) {
				e.Cancel = !CloseCurrentContent();
			}
		}
		
		public bool CloseCurrentContent()
		{
			IViewContent content = viewContent;
			if (content != null) {
				if (!content.Close()) {
					return false;
				}
				viewContent = null;
				content.TitleChanged -= OnTitleChanged;
				OnTitleChanged(content, EventArgs.Empty);
				foreach (Control ctl in contentPanel.Controls) {
					ctl.Dispose();
				}
				contentPanel.Controls.Clear();
			}
			return true;
		}
		
		public void ShowContent(IViewContent content)
		{
			var page = new TabPage(content.Title);
			if (viewContent != null)
				throw new InvalidOperationException("There is still another content opened.");
			viewContent = content;
			Control ctl = content.Control;
			ctl.Dock = DockStyle.Fill;
			page.Controls.Add(ctl);
			ctl.Focus();

		    tabControl1.TabPages.Add(page);
			
			tabControl1.SelectTab(page);
			
			content.TitleChanged += OnTitleChanged;
			OnTitleChanged(content, EventArgs.Empty);
		}
		
		void OnTitleChanged(object sender, EventArgs e)
		{
			if (viewContent != null) {
				this.Text = viewContent.Title + " - Viewer";
			} else {
				this.Text = "Viewer";
			}
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Workbench));
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(873, 348);
			this.tabControl1.TabIndex = 1;
			// 
			// Workbench
			// 
			this.AllowDrop = true;
			this.ClientSize = new System.Drawing.Size(873, 348);
			this.Controls.Add(this.tabControl1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Workbench";
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Workbench_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Workbench_DragEnter);
			this.ResumeLayout(false);

		}

		private void Workbench_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
		}

		private void Workbench_DragDrop(object sender, DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			foreach (string file in files)
			{
				IViewContent content = DisplayBindingManager.CreateViewContent(file);
				if (content != null)
				{
					instance.ShowContent(content);
				}
			}
		}
	}
}