using System.IO;
using System.Windows.Forms;
using Base;

namespace RichTextEditor
{
	public class DisplayBinding : IDisplayBinding
	{
		public IViewContent OpenFile(string fileName)
		{
			if (Path.GetExtension(fileName).ToLowerInvariant() == ".rtf") {
				return new RichTextViewContent(fileName);
			}
			return null;
		}
	}
	
	public class RichTextViewContent : FileViewContent
	{
		RichTextBox textBox = new RichTextBox();
		
		public RichTextViewContent()
		{
			textBox.RichTextShortcutsEnabled = false;
			textBox.AcceptsTab = true;
			textBox.ScrollBars = RichTextBoxScrollBars.Both;
			textBox.ReadOnly = true;
		}
		
		public RichTextViewContent(string fileName) : this()
		{
			textBox.LoadFile(fileName);
			this.FileName = fileName;
		}

		public override Control Control => textBox;
	}
}