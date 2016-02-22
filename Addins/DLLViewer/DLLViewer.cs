using Base;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace DllViewer
{
	public class DisplayBinding : IDisplayBinding
	{
		public IViewContent OpenFile(string fileName)
		{
			string extension = Path.GetExtension(fileName).ToLowerInvariant();
			switch (extension)
			{
				case ".dll": return new DLLViewContent(fileName);
			}

			return null;
		}
	}
	
	public class DLLViewContent : FileViewContent
	{
		TreeView textBox = new TreeView();
		
		public DLLViewContent()
		{
			
		}
		
		public DLLViewContent(string fileName) : this()
		{
			var assembly = Assembly.LoadFile(fileName);
			var root = new TreeNode(assembly.GetName().Name);

			foreach (var type in assembly.GetTypes())
			{
				var n = new TreeNode(type.Name);

				foreach (var method in type.GetMethods())
				{
					var mn = new TreeNode(method.Name);

					n.Nodes.Add(mn);
				}

				root.Nodes.Add(n);
			}

			textBox.Nodes.Add(root);
		}

		public override Control Control => textBox;
	}
}