using Base;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls.RichTextBox;
using Telerik.WinControls.RichTextBox.FileFormats.OpenXml.Docx;

namespace WordViewer
{
	public class DisplayBinding : IDisplayBinding
	{
		public IViewContent OpenFile(string fileName)
		{
			string extension = Path.GetExtension(fileName).ToLowerInvariant();
			switch (extension)
			{
				case ".docx": return new DocumentViewContent(fileName);
			}

			return null;
		}
	}
	
	public class DocumentViewContent : FileViewContent
	{
		RadRichTextBox textBox = new RadRichTextBox();
		
		public DocumentViewContent()
		{
			
		}
		
		public DocumentViewContent(string fileName) : this()
		{
			Load(fileName);
		}

		private async void Load(string fileName)
		{
			await Task.Run(() =>
			{
				var doc = new DocxFormatProvider();
				textBox.Document = doc.Import(new FileStream(fileName, FileMode.Open));

				textBox.IsReadOnly = true;
				textBox.LayoutMode = Telerik.WinControls.RichTextBox.Model.DocumentLayoutMode.Paged;

				this.FileName = fileName;
			});
		}

		public override Control Control => textBox;
	}
}