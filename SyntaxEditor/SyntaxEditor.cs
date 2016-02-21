using Base;
using FastColoredTextBoxNS;
using System.IO;
using System.Windows.Forms;

namespace SyntaxEditor
{
	public class DisplayBinding : IDisplayBinding
	{
		public IViewContent OpenFile(string fileName)
		{
			string extension = Path.GetExtension(fileName).ToLowerInvariant();
			switch (extension)
			{
				case ".cs": return new SyntaxViewContent(fileName, extension);
				case ".xml": return new SyntaxViewContent(fileName, extension);
				case ".html": return new SyntaxViewContent(fileName, extension);
				case ".vb": return new SyntaxViewContent(fileName, extension);
				case ".lua": return new SyntaxViewContent(fileName, extension);
				case ".php": return new SyntaxViewContent(fileName, extension);
				case ".js": return new SyntaxViewContent(fileName, extension);
				case ".sql": return new SyntaxViewContent(fileName, extension);
			}

			return null;
		}
	}
	
	public class SyntaxViewContent : FileViewContent
	{
		private string extension;
		FastColoredTextBox textBox = new FastColoredTextBox();
		
		public SyntaxViewContent()
		{
			textBox.ReadOnly = true;
			textBox.AcceptsTab = true;
			textBox.Refresh();
		}
		
		public SyntaxViewContent(string fileName) : this()
		{
			textBox.OpenFile(fileName);
			this.FileName = fileName;
		}

		public SyntaxViewContent(string fileName, string extension) : this(fileName)
		{
			this.extension = extension;

			switch (this.extension)
			{
				case ".cs":
					textBox.Language = Language.CSharp;
					break;
				case ".js":
					textBox.Language = Language.JS;
					break;
				case ".html":
					textBox.Language = Language.HTML;
					break;
				case ".lua":
					textBox.Language = Language.Lua;
					break;
				case ".php":
					textBox.Language = Language.PHP;
					break;
				case ".sql":
					textBox.Language = Language.SQL;
					break;
				case ".vb":
					textBox.Language = Language.VB;
					break;
				case ".xml":
					textBox.Language = Language.XML;
					break;
			}
			
		}

		public override Control Control => textBox;
	}
}
