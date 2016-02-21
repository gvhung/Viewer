using System.IO;
using System.Windows.Forms;
using Base;
using System.Threading.Tasks;

namespace ImageViewer
{
	public class DisplayBinding : IDisplayBinding
	{
		public IViewContent OpenFile(string fileName)
		{
			string extension = Path.GetExtension(fileName).ToLowerInvariant();
			switch (extension)
			{
				case ".png": return new ImageViewContent(fileName);
				case ".jpg": return new ImageViewContent(fileName);
			}

			return null;
		}
	}
	
	public class ImageViewContent : FileViewContent
	{
		PictureBox textBox = new PictureBox();
		
		public ImageViewContent()
		{
			
		}

		public ImageViewContent(string fileName) : this()
		{
			textBox.Load(fileName);

			this.FileName = fileName;
		}

		public override Control Control => textBox;
	}
}