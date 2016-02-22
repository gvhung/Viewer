using Base;
using System.IO;
using System.Windows.Forms;

namespace IniViewer
{
	public class DisplayBinding : IDisplayBinding
	{
		public IViewContent OpenFile(string fileName)
		{
			string extension = Path.GetExtension(fileName).ToLowerInvariant();
			switch (extension)
			{
				case ".ini": return new IniViewContent(fileName);
			}

			return null;
		}
	}
	
	public class IniViewContent : FileViewContent
	{
		TreeView textBox = new TreeView();
		
		public IniViewContent()
		{
			
		}
		
		public IniViewContent(string fileName) : this()
		{
			var f = new IniParser.FileIniDataParser();
			var data = f.ReadFile(fileName);

			var root = new TreeNode(fileName);

			foreach (var section in data.Sections)
			{
				var sn = new TreeNode(section.SectionName);

				foreach (var key in section.Keys)
				{
					var kn = new TreeNode(key.KeyName);
					var vn = new TreeNode(key.Value);

					kn.Nodes.Add(vn);
					sn.Nodes.Add(kn);
				}

				root.Nodes.Add(sn);
			}

			textBox.Nodes.Add(root);

			this.FileName = fileName;
		}

		public override Control Control => textBox;
	}
}