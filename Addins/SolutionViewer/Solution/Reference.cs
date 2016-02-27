using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToVisualStudioSolution
{

    public static class IEnumerableExtension
    {
        public static bool Contains(this IEnumerable<Reference> references, string reference)
        {
            foreach (var item in references)
            {
                if (item.Include.Contains(reference) || (item.HintPath != null && item.HintPath.Contains(reference)))
                {
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// Represents a Reference node of and ItemGroup in a cproj file.
    /// </summary>
    public class Reference
    {

        public Reference(string include, bool? specificVersion, string requiredTargetFramework, string hintPath)
        {
            this.Include = include;
            if (include != null && include.IndexOf(',') > 0)
            {
                this.IncludeAssemblyOnly = include.Split(',')[0];
            }
            this.SpecificVersion = specificVersion;
            this.RequiredTargetFramework = requiredTargetFramework;
            this.HintPath = hintPath;
        }

        public string Include { get; private set; }

        /// <summary>
        /// If the Include attribute includes Version, Culture, etc. infomation, this ignores that and only returns the assembly name.
        /// </summary>
        public string IncludeAssemblyOnly { get; private set; }

        public bool? SpecificVersion { get; private set; }

        public string RequiredTargetFramework { get; private set; }

        public string HintPath { get; private set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(this.Include);

            return sb.ToString();
        }
    }
}
