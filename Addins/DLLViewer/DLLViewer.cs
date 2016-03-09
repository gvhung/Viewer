using Base;
using DLLViewer.Properties;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Collections;

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
            root.ImageIndex = 5;
            root.SelectedImageIndex = 5;

            var il = new ImageList();
            il.Images.Add(Resources.enums);
            il.Images.Add(Resources.properties);
            il.Images.Add(Resources.methods);
            il.Images.Add(Resources.events);
            il.Images.Add(Resources.namespaces);
            il.Images.Add(Resources.assembly);

            textBox.ImageList = il;
            textBox.StateImageList = il;

            var namespaces = GetNamespaces(assembly);

            var res = new TreeNode("Ressources");

            var refs = new TreeNode("References");
            refs.ImageIndex = 5;
            refs.SelectedImageIndex = 5;

            root.Nodes.Add(res);
            root.Nodes.Add(refs);

            foreach (var resource in assembly.GetManifestResourceNames())
            {
                var n = new TreeNode(resource);

                foreach (DictionaryEntry name in new ResourceReader(assembly.GetManifestResourceStream(resource)))
                {
                    n.Nodes.Add(new TreeNode(name.Key.ToString()));
                }

                res.Nodes.Add(n);
            }

            foreach (var re in assembly.GetReferencedAssemblies())
            {
                var tn = new TreeNode(re.Name);
                tn.ImageIndex = 5;
                tn.SelectedImageIndex = 5;

                refs.Nodes.Add(tn);
            }

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
                    if (props.Nodes.Count > 0)
                    {
                        n.Nodes.Add(props);
                    }

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
                    if (events.Nodes.Count > 0)
                    {
                        n.Nodes.Add(events);
                    }

                    var methods = new TreeNode("Methods");
                    methods.ImageIndex = 2;
                    methods.SelectedImageIndex = 2;
                    foreach (var method in GetMethods(type))
                    {
                        var mn = new TreeNode(method.Name);
                        mn.ImageIndex = 2;
                        mn.SelectedImageIndex = 2;

                        if (method.Name.StartsWith("set_") || method.Name.StartsWith("get")) continue;
                        if (method.Name.StartsWith("add_") || method.Name.StartsWith("remove_")) continue;

                        methods.Nodes.Add(mn);
                    }

                    if (methods.Nodes.Count > 0)
                    {
                        n.Nodes.Add(methods);
                    }

                    namesp.Nodes.Add(n);

                }
                root.Nodes.Add(namesp);

            }

            root.Expand();

            textBox.Nodes.Add(root);
            this.FileName = fileName;
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
        private MethodInfo[] GetMethods(Type t)
        {
            var d = new Dictionary<string, MethodInfo>();

            foreach (var method in t.GetMethods())
            {
                try
                {
                    d.Add(method.Name, method);
                } catch { }
            }

            return d.Values.ToArray();
        }

        public override Control Control => textBox;
    }
}