using Base;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls.UI;

namespace PdfViewer
{
	public class DisplayBinding : IDisplayBinding
	{
		public IViewContent OpenFile(string fileName)
		{
			string extension = Path.GetExtension(fileName).ToLowerInvariant();
			switch (extension)
			{
				case ".pdf": return new PortableDocumentViewContent(fileName);
			}

			return null;
		}
	}
	
	public class PortableDocumentViewContent : FileViewContent
	{
		RadPdfViewer textBox = new RadPdfViewer();
		
		public PortableDocumentViewContent()
		{
			
		}
		
		public PortableDocumentViewContent(string fileName) : this()
		{
			this.FileName = fileName;
			
			Load(fileName);
		}

		private async void Load(string fileName)
		{
			await Task.Run(() =>
			{
				textBox.LoadDocument(fileName);

				this.FileName = fileName;
			});
		}

		public override Control Control => textBox;
	}
}