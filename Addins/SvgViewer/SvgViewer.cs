using System.IO;
using System.Windows.Forms;
using Base;
using Svg;
using System.Threading.Tasks;

namespace SvgViewer
{
	public class DisplayBinding : IDisplayBinding
	{
		public IViewContent OpenFile(string fileName)
		{
			string extension = Path.GetExtension(fileName).ToLowerInvariant();
			switch (extension)
			{
				case ".svg": return new SvgViewContent(fileName);
			}

			return null;
		}
	}
	
	public class SvgViewContent : FileViewContent
	{
		PictureBox textBox = new PictureBox();
		
		public SvgViewContent()
		{
			
		}

		public SvgViewContent(string fileName) : this()
		{
			Load(fileName);

			this.FileName = fileName;
		}

		private async void Load(string fileName)
		{
			await Task.Run(() =>
			{
				SvgDocument svgDoc = SvgDocument.Open(fileName);
				textBox.Image = svgDoc.Draw();
			});
		}

		public override Control Control => textBox;
	}
}