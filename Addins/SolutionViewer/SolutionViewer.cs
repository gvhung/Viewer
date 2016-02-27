using System.IO;
using System.Windows.Forms;
using Base;
using LinqToVisualStudioSolution;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

namespace SolutionViewer
{
	public class DisplayBinding : IDisplayBinding
	{
		public IViewContent OpenFile(string fileName)
		{
			if (Path.GetExtension(fileName).ToLowerInvariant() == ".sln") {
				return new RichTextViewContent(fileName);
			}
			return null;
		}
	}
	
	public class RichTextViewContent : FileViewContent
	{
		TreeView textBox = new TreeView();
		
		public RichTextViewContent()
		{
			
		}

		public RichTextViewContent(string fileName) : this()
		{
			this.FileName = fileName;

			var sln = new Solution(fileName);
			var root = new TreeNode(sln.SolutionPath);

			var projects = (from p in sln select p);

			foreach (var pp in projects)
			{
				var pn = new TreeNode(pp.ProjectName);
				var refs = new TreeNode("References");

				foreach (var pi in pp)
				{
					if(pi.Name == pi.Name.Namespace + "Reference")
					{
						refs.Nodes.Add(new TreeNode(new List<XAttribute>(pi.Attributes("Include"))[0].Value));
					}
				}

				pn.Nodes.Add(refs);

				
				root.Nodes.Add(pn);
			}

			root.Expand();
			textBox.Nodes.Add(root);
		}

		public override Control Control => textBox;
	}
}