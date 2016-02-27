using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace LinqToVisualStudioSolution
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<PElement> Elements(this IEnumerable<PElement> elements, string name)
        {
            foreach (var item in elements)
            {
                foreach (var element in item.Elements())
                {
                    if (element.Name.LocalName == name)
                    {
                        yield return element;
                    }
                }
            }
        }

        public static IEnumerable<PElement> Elements(this IEnumerable<PElement> elements)
        {
            foreach (var item in elements)
            {
                foreach (var element in item.Elements())
                {
                    yield return element;
                }
            }
        }
    }

    public class NullableDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        new public TValue this[TKey key]
        {
            get
            {
                if (this.Keys.Contains(key))
                {
                    return base[key];
                }
                return default(TValue);
            }
            set
            {
                base[key] = value;
            }
        }
    }

    public class PElement : XElement
    {

        XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";

        new public PElement Element(XName name)
        {
            if (name.Namespace.NamespaceName == string.Empty)
            {
                name = XName.Get(name.LocalName, ns.NamespaceName);
            }
            XElement baseElement = base.Element(name);
            if (baseElement == null)
            {
                return null;
            }
            return new PElement(baseElement);
        }

        new public IEnumerable<PElement> Elements()
        {
            foreach (var item in base.Elements())
            {
                yield return new PElement(item);
            }
        }

        private NullableDictionary<string, string> attributes = new NullableDictionary<string, string>();

        new public IEnumerable<string> Attributes()
        {
            foreach (XAttribute att in base.Attributes())
            {
                yield return att.Value;
            }
        }


        public string Attribute(string name)
        {
            return this.attributes[name];
        }

        public PElement(XElement other):base(other)
        {
            
            foreach (XAttribute item in other.Attributes())
            {
                this.attributes.Add(item.Name.LocalName, item.Value);
            }
        }

        public PElement(XName name):base(name)
        {

        }

        public PElement(XStreamingElement other):base(other)
        {

        }

        public PElement(XName name, object content):base(name, content)
        {

        }

        public PElement(XName name, params object[] content):base(name, content)
        {

        }

        

        
    }
}
