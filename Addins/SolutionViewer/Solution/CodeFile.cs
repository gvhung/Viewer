using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToVisualStudioSolution
{

    /// <summary>
    /// Represents a Compile node in a project file.  Contains properties of a code file (e.g. a .cs file).
    /// </summary>
    public class CodeFile
    {
        public string Path { get; private set; }
        public bool Visible { get; private set; }

        /// <summary>
        /// If false, then it is an exclude setting.
        /// </summary>
        public bool Include { get; private set; }

        public CodeFile(string path, bool visible) : this(path, visible, true)
        {

        }

        public CodeFile(string path, bool visible, bool include)
        {
            this.Path = path;
            this.Visible = visible;
            this.Include = include;
        }

        public override string ToString()
        {
            return this.Path;
        }
    }
}
