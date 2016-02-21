using Base;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms.Integration;

namespace MediaViewer
{
	public class DisplayBinding : IDisplayBinding
	{
		public IViewContent OpenFile(string fileName)
		{
			string extension = Path.GetExtension(fileName).ToLowerInvariant();
			switch (extension)
			{
				case ".mp3": return new MediaViewContent(fileName);
				case ".avi": return new MediaViewContent(fileName);
			}

			return null;
		}
	}
	
	public class MediaViewContent : FileViewContent
	{
		private System.Windows.Forms.Control textbox = new System.Windows.Forms.Control();

		public MediaViewContent()
		{
			
		}
		
		public MediaViewContent(string fileName) : this()
		{
			Load(fileName);
		}

		private void Load(string fileName)
		{
			this.FileName = fileName;
		}

		public override System.Windows.Forms.Control Control {
			get {
				if (FileName.EndsWith(".mp3") | FileName.EndsWith(".ogg"))
				{
					textbox = new AudioControl(FileName);
				}
				else if (FileName.EndsWith(".avi") | FileName.EndsWith(".mp4"))
				{
					textbox = new VideoControl(FileName);
				}

				return textbox;
			}
		}
	}
}