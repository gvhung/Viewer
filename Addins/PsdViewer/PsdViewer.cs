using System.IO;
using System.Windows.Forms;
using Base;
using Photoshop;
using System.Threading.Tasks;

namespace PsdViewer
{
	public class DisplayBinding : IDisplayBinding
	{
		public IViewContent OpenFile(string fileName)
		{
			string extension = Path.GetExtension(fileName).ToLowerInvariant();
			switch (extension)
			{
				case ".psd": return new ImageViewContent(fileName);
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
			loadPSD(fileName);

			this.FileName = fileName;
		}

		private async void loadPSD(string fileName)
		{
			await Task.Run(() =>
			{
				var psd = new PsdFile(fileName);
				textBox.Image = psd.CompositImage;
			});
		}

		public override Control Control => textBox;
	}
}