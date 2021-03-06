﻿using System.IO;
using System.Windows.Forms;
using Base;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

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
				case ".gif": return new ImageViewContent(fileName);
				case ".ico": return new ImageViewContent(fileName);
				case ".tiff": return new ImageViewContent(fileName);
				case ".wmf": return new ImageViewContent(fileName);
				case ".emf": return new ImageViewContent(fileName);
				case ".bmp": return new ImageViewContent(fileName);
			}

			return null;
		}
	}
	
	public class ImageViewContent : FileViewContent
	{
		Cyotek.Windows.Forms.ImageBox textBox = new Cyotek.Windows.Forms.ImageBox();
		
		public ImageViewContent()
		{
			
		}

		public ImageViewContent(string fileName) : this()
		{
			textBox.Image = Image.FromFile(fileName);

			this.FileName = fileName;
		}

		public override Control Control => textBox;
	}
}