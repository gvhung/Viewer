using Base;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace XmlViewer
{
    public class DisplayBinding : IDisplayBinding
    {
        public IViewContent OpenFile(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();
            switch (extension)
            {
                case ".xml": return new XmlViewContent(fileName);
            }

            return null;
        }
    }

    public class XmlViewContent : FileViewContent
    {
        TreeView textBox = new TreeView();

        public XmlViewContent()
        {

        }

        public XmlViewContent(string fileName) : this()
        {
            Load(fileName);
        }

        private async void Load(string fileName)
        {
            XmlDocument dom = new XmlDocument();
            dom.Load(fileName);
            
            // SECTION 2. Initialize the TreeView control.
            textBox.Nodes.Clear();
            textBox.Nodes.Add(new TreeNode(dom.DocumentElement.Name));
            TreeNode tNode = new TreeNode();
            tNode = textBox.Nodes[0];

            // SECTION 3. Populate the TreeView with the DOM nodes.
            AddNode(dom.DocumentElement, tNode);

            textBox.ExpandAll();
            this.FileName = fileName;
        }

        private void AddNode(XmlNode inXmlNode, TreeNode inTreeNode)
        {
            XmlNode xNode;
            TreeNode tNode;
            XmlNodeList nodeList;
            int i;

            // Loop through the XML nodes until the leaf is reached.
            // Add the nodes to the TreeView during the looping process.
            if (inXmlNode.HasChildNodes)
            {
                nodeList = inXmlNode.ChildNodes;
                for (i = 0; i <= nodeList.Count - 1; i++)
                {
                    xNode = inXmlNode.ChildNodes[i];
                    inTreeNode.Nodes.Add(new TreeNode(xNode.Name));
                    tNode = inTreeNode.Nodes[i];
                    AddNode(xNode, tNode);
                }
            }
            else
            {
                // Here you need to pull the data from the XmlNode based on the
                // type of node, whether attribute values are required, and so forth.
                inTreeNode.Text = (inXmlNode.OuterXml).Trim();
            }
        }

        public override Control Control => textBox;
    }
}