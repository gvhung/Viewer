using Base;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XmlViewer
{
	public class DisplayBinding : IDisplayBinding
	{
		public IViewContent OpenFile(string fileName)
		{
			string extension = Path.GetExtension(fileName).ToLowerInvariant();
			switch (extension)
			{
				case ".xml": return new XmlViewContent(fileName);
			}

			return null;
		}
	}
	
	public class XmlViewContent : FileViewContent
	{
		DataGrid textBox = new DataGrid();
		
		public XmlViewContent()
		{
			
		}
		
		public XmlViewContent(string fileName) : this()
		{
			Load(fileName);
		}

		private async void Load(string fileName)
		{
			var ds = new DataSet();

			await Task.Run(() =>
			{
				textBox.ReadOnly = true;
				
				ds.ReadXml(fileName);
			});

			textBox.DataSource = ds;

			this.FileName = fileName;
		}

		public override Control Control => textBox;
	}
}