using System.IO;
using System.Windows.Forms;
using Base;
using Photoshop;

namespace ImageViewer
{
	public class DisplayBinding : IDisplayBinding
	{
		public IViewContent OpenFile(string fileName)
		{
			string extension = Path.GetExtension(fileName).ToLowerInvariant();
			switch (extension)
			{
				case ".psd": return new ImageViewContent(fileName);
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
			if (!fileName.EndsWith(".psd"))
			{
				textBox.Load(fileName);
			}
			else
			{
				var psd = new PsdFile(fileName);
				textBox.Image = psd.CompositImage;
			}
			this.FileName = fileName;
		}

		public override Control Control => textBox;
	}
}