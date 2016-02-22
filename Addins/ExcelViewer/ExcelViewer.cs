using Base;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExcelViewer
{
	public class DisplayBinding : IDisplayBinding
	{
		public IViewContent OpenFile(string fileName)
		{
			string extension = Path.GetExtension(fileName).ToLowerInvariant();
			switch (extension)
			{
				case ".xlsx": return new DocumentViewContent(fileName);
			}

			return null;
		}
	}
	
	public class DocumentViewContent : FileViewContent
	{
		unvell.ReoGrid.ReoGridControl textBox = new unvell.ReoGrid.ReoGridControl();
		
		public DocumentViewContent()
		{
			
		}
		
		public DocumentViewContent(string fileName) : this()
		{
			Load(fileName);
		}

		private async void Load(string fileName)
		{
			textBox.Readonly = true;
			textBox.Load(fileName);

			this.FileName = fileName;
		}

		public override Control Control => textBox;
	}
}