using System;
using System.ComponentModel;
using System.Windows.Forms;
using ICSharpCode.Core;

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
		private StatusStrip statusStrip1;
		private ToolStripStatusLabel toolStripStatusLabel1;
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

			StatusService.SetLabel(toolStripStatusLabel1);
			StatusService.SetStatus("");

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
			if (viewContent != null)
				throw new InvalidOperationException("There is still another content opened.");
			viewContent = content;
			Control ctl = content.Control;
			ctl.Dock = DockStyle.Fill;
			contentPanel.Controls.Add(ctl);
			ctl.Focus();
			
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
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.toolStripStatusLabel1});
			this.statusStrip1.Location = new System.Drawing.Point(0, 239);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(284, 22);
			this.statusStrip1.TabIndex = 0;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
			// 
			// Workbench
			// 
			this.ClientSize = new System.Drawing.Size(284, 261);
			this.Controls.Add(this.statusStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Workbench";
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
	}
}