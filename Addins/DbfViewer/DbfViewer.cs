using Base;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace DbfViewer
{
	public class DisplayBinding : IDisplayBinding
	{
		public IViewContent OpenFile(string fileName)
		{
			string extension = Path.GetExtension(fileName).ToLowerInvariant();
			switch (extension)
			{
				case ".dbf": return new DbfViewContent(fileName);
			}

			return null;
		}
	}
	
	public class DbfViewContent : FileViewContent
	{
		DataGrid textBox = new DataGrid();
		
		public DbfViewContent()
		{
			
		}
		
		public DbfViewContent(string fileName) : this()
		{
			DataTable dt = ParseDBF.ReadDBF(fileName);
			textBox.DataSource = dt;

			textBox.ReadOnly = true;
		}

		public override Control Control => textBox;
	}
}