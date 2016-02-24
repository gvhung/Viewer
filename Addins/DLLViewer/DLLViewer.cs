using Base;
using DLLViewer.Properties;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System;
using System.Collections.Generic;

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
            var il = new ImageList();
            il.Images.Add(Resources.enums);
            il.Images.Add(Resources.properties);
            il.Images.Add(Resources.methods);
            il.Images.Add(Resources.events);
            il.Images.Add(Resources.namespaces);

            textBox.ImageList = il;
            textBox.StateImageList = il;

            var namespaces = GetNamespaces(assembly);

            foreach (var ns in namespaces)
            {
                var namesp = new TreeNode(ns.Key);
                namesp.ImageIndex = 4;
                namesp.SelectedImageIndex = 4;

                foreach (var type in ns.Value)
                {
                    var n = new TreeNode(type.Name);

                    var props = new TreeNode("Properties");
                    props.ImageIndex = 1;
                    props.SelectedImageIndex = 1;
                    foreach (var prop in type.GetProperties())
                    {
                        var mn = new TreeNode(prop.Name);
                        mn.ImageIndex = 1;
                        mn.SelectedImageIndex = 1;

                        props.Nodes.Add(mn);
                    }
                    n.Nodes.Add(props);

                    var events = new TreeNode("Events");
                    events.ImageIndex = 3;
                    events.SelectedImageIndex = 3;
                    foreach (var prop in type.GetEvents())
                    {
                        var mn = new TreeNode(prop.Name);
                        mn.ImageIndex = 3;
                        mn.SelectedImageIndex = 3;

                        events.Nodes.Add(mn);
                    }
                    n.Nodes.Add(events);

                    var methods = new TreeNode("Methods");
                    methods.ImageIndex = 2;
                    methods.SelectedImageIndex = 2;
                    foreach (var method in type.GetMethods())
                    {
                        var mn = new TreeNode(method.Name);
                        mn.ImageIndex = 2;
                        mn.SelectedImageIndex = 2;

                        methods.Nodes.Add(mn);
                    }
                    n.Nodes.Add(methods);

                    namesp.Nodes.Add(n);
                    root.Nodes.Add(namesp);
                }
                

                textBox.Nodes.Add(root);
            }
        }

        private Dictionary<string, List<Type>> GetNamespaces(Assembly assembly)
        {
            var res = new Dictionary<string, List<Type>>();

            foreach (var t in assembly.GetTypes())
            {
                if (t.Namespace != null)
                {
                    if (!res.ContainsKey(t.Namespace))
                    {
                        res.Add(t.Namespace, new List<Type>() { t });
                    }
                    else
                    {
                        res[t.Namespace].Add(t);
                    }
                }
            }

            return res;
        }

        public override Control Control => textBox;
    }
}