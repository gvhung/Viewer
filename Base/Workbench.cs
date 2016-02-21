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
		Panel contentPanel;
		
		private Workbench()
		{
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
			this.SuspendLayout();
			// 
			// Workbench
			// 
			this.ClientSize = new System.Drawing.Size(284, 261);
			this.Name = "Workbench";
			this.ShowIcon = false;
			this.ResumeLayout(false);

		}
	}
}