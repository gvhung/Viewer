// Copyright (c) 2005 Daniel Grunwald
// Licensed under the terms of the "BSD License", see doc/license.txt

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Base
{
	public class TextDisplayBinding : IDisplayBinding
	{
		public IViewContent OpenFile(string fileName)
		{
			return new TextViewContent(fileName);
		}
	}
	
	/// <summary>
	/// ViewContent showing a text file.
	/// </summary>
	public class TextViewContent : FileViewContent
	{
		TextBox textBox = new TextBox();
		
		public TextViewContent()
		{
			textBox.Multiline = true;
			textBox.AcceptsReturn = true;
			textBox.AcceptsTab = true;
			textBox.Font = new Font("Courier New", 10f);
			textBox.WordWrap = false;
			textBox.ScrollBars = ScrollBars.Both;
			textBox.ReadOnly = true;
		}
		
		public TextViewContent(string fileName) : this()
		{
			textBox.Text = File.ReadAllText(fileName);
			this.FileName = fileName;
		}

		public override Control Control => textBox;
	}
}