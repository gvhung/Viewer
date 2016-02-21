using System;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace Base
{
	/// <summary>
	/// Base ViewContent for files.
	/// </summary>
	public abstract class FileViewContent : IViewContent
	{
		public abstract Control Control {
			get;
		}
		
		string fileName;
		public event EventHandler FileNameChanged;
		
		public string FileName {
			get {
				return fileName;
			}
			set {
				if (fileName != value) {
					fileName = value;
					ChangeTitleToFileName();
					if (FileNameChanged != null) {
						FileNameChanged(this, EventArgs.Empty);
					}
				}
			}
		}
		
		protected virtual void ChangeTitleToFileName()
		{
			this.Title = System.IO.Path.GetFileName(this.FileName);
		}
		
		string title = "Untitled";
		public event EventHandler TitleChanged;
		
		public string Title {
			get {
				return title;
			}
			set {
				if (title != value) {
					title = value;
					
					if (TitleChanged != null) {
						TitleChanged(this, EventArgs.Empty);
					}
				}
			}
		}
		
		bool dirty;
		public event EventHandler DirtyChanged;
		
		public static string GetFileFilter(string addInTreePath)
		{
			StringBuilder b = new StringBuilder();
			b.Append("All known file types|");
			foreach (string filter in AddInTree.BuildItems(addInTreePath, null, true)) {
				b.Append(filter.Substring(filter.IndexOf('|') + 1));
				b.Append(';');
			}
			foreach (string filter in AddInTree.BuildItems(addInTreePath, null, true)) {
				b.Append('|');
				b.Append(filter);
			}
			b.Append("|All files|*.*");
			return b.ToString();
		}
		
		public virtual bool Close()
		{
			return true;
		}
	}
}