/*
 * TAGS: Parse, Project File, cproj
 * 
 */ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.IO;

namespace LinqToVisualStudioSolution
{

    public enum OutputType
    {
        NotSet,
        EXE,
        WinEXE,
        Library
    }

    /// <summary>
    /// Represents a project file with all available properties.
    /// </summary>
    public class Project : List<PElement>
    {
        /// <summary>
        /// Keep a list of errors in processing instead of stopping the flow.
        /// </summary>
        public List<string> Errors { get; private set; }

        XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";
        XElement projectXml;

		private string projectName;
        public string ProjectName { get { return projectName; } private set { projectName = value; } }
		
		private string projectPath;
        public string ProjectPath { get { return projectPath; } private set { projectPath = value; } }
		
		private string guid;
        public string Guid { get { return guid; } private set { guid = value; } }
		

        public List<Reference> References { get; private set; }
        public List<CodeFile> CodeFiles { get; private set; }
        public List<string> Imports { get; private set; }
        public List<string> Comments { get; private set; }
        
		private string configuration;
        public string Configuration { get { return configuration; } private set { configuration = value; } }
		
		private string platform;
        public string Platform { get { return platform; } private set { platform = value; } }
		
		private Version productVersion;
        public Version ProductVersion { get { return productVersion; } private set { productVersion = value; } }
		
		private string schemaVersion;
        public string SchemaVersion { get { return schemaVersion; } private set { schemaVersion = value; } }
		
		private OutputType outputType;
        public OutputType OutputType { get { return outputType; } private set { outputType = value; } }
		
		private string appDesignerFolder;
        public string AppDesignerFolder { get { return appDesignerFolder; } private set { appDesignerFolder = value; } }
		
		private string rootNamespace;
        public string RootNamespace { get { return rootNamespace; } private set { rootNamespace = value; } }
		
		private string assemblyName;
        public string AssemblyName { get { return assemblyName; } private set { assemblyName = value; } }
		
		private string targetFrameworkVersion;
        public string TargetFrameworkVersion { get { return targetFrameworkVersion; } private set { targetFrameworkVersion = value; } }
		
		private int? fileAlignment;
        public int? FileAlignment {  get { return fileAlignment; }  private  set { fileAlignment = value; } }
		

        // We will only worry about the active PropertyGroup

		private bool? debugSymbols;
        public bool? DebugSymbols { get { return debugSymbols; } private set { debugSymbols = value; } }
		
		private string debugType;
        public string DebugType { get { return debugType; } private set { debugType = value; } }
		
		private bool? optimize;
        public bool? Optimize { get { return optimize; } private set { optimize = value; } }
		
		private string outputPath;
        public string OutputPath { get { return outputPath; } private set { outputPath = value; } }
		
		private string outputPathFull;
        public string OutputPathFull { get { return outputPathFull; } private set { outputPathFull = value; } }
		
		private string outputPathToAssemblyFull;
        public string OutputPathToAssemblyFull { get { return outputPathToAssemblyFull; } private set { outputPathToAssemblyFull = value; } }
		
		private string defineConstants;
        public string DefineConstants { get { return defineConstants; } private set { defineConstants = value; } }
		
		private string errorReport;
        public string ErrorReport { get { return errorReport; } private set { errorReport = value; } }
		
		private int? warningLevel;
        public int? WarningLevel {  get { return warningLevel; } private  set { warningLevel = value; } }
		

        /// <summary>
        /// Initializes a dictionary of references and a list of the code files in the project.
        /// </summary>
        /// <param name="projectFilename">Full path to the project to load.</param>
        public Project(string guid, string projectName, string projectFilename)
	    {
            // These properties will come from the solution file.

            this.ProjectName = projectName;
            this.ProjectPath = projectFilename;
            this.Guid = guid;


            this.References = new List<Reference>();
            this.CodeFiles = new List<CodeFile>();
            this.Imports = new List<string>();
            this.Comments = new List<string>();

            this.Errors = new List<string>();

            if (!File.Exists(projectFilename))
            {
                projectXml = new XElement(ns + "Project");
                this.Errors.Add("WARNING:" + projectFilename + " does not exist.  Please check the reference.");
                return;
            }


            try
            {
                projectXml = XElement.Load(projectFilename);
            }
            catch (Exception)
            {
                // We cannot read this project for some reason.
                projectXml = new XElement(ns + "Project");
                this.Errors.Add("Cannot read: " + projectFilename);
                return;
            }

            FillPElements(projectXml);

            // Retrieve a list of all references with their paths if specified.
            
            var references = from r in ItemGroups.Elements("Reference")
                    select r;

            foreach (var reference in references)
            {
                this.References.Add(
                    new Reference(reference.Attribute("Include"),
                        reference.Element("SpecificVersion") == null ? null : (bool?)(reference.Element("SpecificVersion").Value == bool.TrueString),
                        reference.Element("RequiredTargetFramework") == null ? null : reference.Element("RequiredTargetFramework").Value,
                        reference.Element("HintPath") == null ? null : reference.Element("HintPath").Value));
            
            }

            // Retrieve a list of all code files to be compiled.

            var codefiles = from c in ItemGroups.Elements("Compile")
                            select c;

            Environment.CurrentDirectory = Path.GetDirectoryName(projectFilename);

            foreach (var codefile in codefiles)
            {
                // Add the required Include attribute.
                this.CodeFiles.Add(new CodeFile(Path.GetFullPath(codefile.Attribute("Include")), 
                    codefile.Element("Visible") != null ? codefile.Element("Visible").Value == bool.TrueString : true));

                // Handle the optional Exclude attribute.
                if (codefile.Attribute("Exclude") != null)
                {
                    this.CodeFiles.Add(new CodeFile(Path.GetFullPath(codefile.Attribute("Exclude")), false, false));
                }
            }

            // Retrieve a list of all imports but do not try to evaluate the paths.

            var import = projectXml.Element(ns + "Import");

            if (import != null)
            {
                this.Imports.Add(import.Attribute("Project").Value);
            }

            // Configuration

            GetProperty("Configuration", null, out this.configuration);

            
            // Platform

            GetProperty("Platform", null, out this.platform);

            // ProductVersion

            IEnumerable<PElement> pvs = PropertyGroups.Elements("ProductVersion");
            if (pvs.Count() > 0)
            {
                try
                {
                    this.productVersion = new Version(pvs.First().Value);
                }
                catch (ArgumentException)
                {
                    this.Errors.Add("ProductVersion format is incorrect:" + pvs.First().Value);
                }
            }

            // SchemaVersion

            GetProperty("SchemaVersion", null, out this.schemaVersion);

            // OutputType

            GetProperty("OutputType", null, out this.outputType);

            // AppDesignerFolder

            GetProperty("AppDesignFolder", null, out this.appDesignerFolder);
			
            // RootNamespace

            GetProperty("RootNamespace", null, out this.rootNamespace);
			
            // AssemblyName

            GetProperty("AssemblyName", null, out this.assemblyName);
			
            // TargetFrameworkVersion

            GetProperty("TargetFrameworkVersion", null, out this.targetFrameworkVersion);
			
            // FileAlignment

            GetProperty("FileAlignment", null, out this.fileAlignment);
			
            // DebugSymbols TODO: Make the following properties that of the active PropertyGroup, not just the first one.

            GetProperty("DebugSymbols", this.Configuration, out this.debugSymbols);
						
            // DebugType

            GetProperty("DebugType", this.Configuration, out this.debugType);
			
            // Optimize

            GetProperty("Optimize", this.Configuration, out this.optimize);
			
            // OutputPath

            GetProperty("OutputPath", this.Configuration, out this.outputPath);

            // OutputPathFull is a calculation from OutputPath to get the full path.

            if (this.OutputPath != null)
            {
                this.OutputPathFull = Path.GetFullPath(this.OutputPath);

                // OutputPathToAssemblyFull is a calculation from OutputPathFull, AssemblyName, and OutputType

                this.OutputPathToAssemblyFull = Path.Combine(this.OutputPathFull, this.AssemblyName);
                switch (OutputType)
                {
                    case OutputType.NotSet:
                        break;
                    case OutputType.EXE:
                        this.OutputPathToAssemblyFull += ".exe";
                        break;
                    case OutputType.WinEXE:
                        this.OutputPathToAssemblyFull += ".exe";
                        break;
                    case OutputType.Library:
                        this.OutputPathToAssemblyFull += ".dll";
                        break;
                    default:
                        break;
                }
            }
            
			
            // DefineConstants

            GetProperty("DefineConstants", this.Configuration, out this.defineConstants);
			
            // ErrorReport

            GetProperty("ErrorReport", this.Configuration, out this.errorReport);
			
            // WarningLevel

            GetProperty("WarningLevel", this.Configuration, out this.warningLevel);


	    }

        public PElement this[string name]
        {
            get
            {
                foreach (var item in this)
                {
                    if (item.Name.LocalName == name)
                    {
                        return item;
                    }
                }
                return null;
            }            
        }

        public PElement Element(string name)
        {
            return this[name];
        }

        public IEnumerable<PElement> Elements(string name)
        {
            return from e in this
                   where e.Name.LocalName == name
                   select e;
        }

        /// <summary>
        /// Just a wrapper of Elements("PropertyGroup") for convenience.
        /// </summary>
        public IEnumerable<PElement> PropertyGroups {
            get
            {
                return Elements("PropertyGroup");
            }
        }

        /// <summary>
        /// Just a wrapper of Elements("ItemGroup") for convenience.
        /// </summary>
        public IEnumerable<PElement> ItemGroups
        {
            get
            {
                return Elements("ItemGroup");
            }
        }

        

        private void FillPElements(XElement parentElement)
        {
            foreach (var node in parentElement.Nodes())
            {
                System.Diagnostics.Debug.WriteLine(node.NodeType);
                switch (node.NodeType)
                {
                    case System.Xml.XmlNodeType.Comment:
                        this.Comments.Add(((XComment)node).Value);
                        break;
                    case System.Xml.XmlNodeType.Element:
                        this.Add(new PElement((XElement)node));
                        FillPElements((XElement)node);
                        break;
                    default:
                        break;
                }
            }            
        }
               
        private void GetProperty(string propertyName, string configuration, out int? iProperty)
        {
            object objTemp;
            GetProperty(propertyName, configuration, out objTemp, typeof(int?));
            iProperty = (int?)objTemp;
        }

        private void GetProperty(string propertyName, string configuration, out bool? bProperty)
        {
            object objTemp;
            GetProperty(propertyName, configuration, out objTemp, typeof(bool?));
            bProperty = (bool?)objTemp;
        }

        private void GetProperty(string propertyName, string configuration, out string sProperty)
        {
            object objTemp;
            GetProperty(propertyName, configuration, out objTemp, typeof(string));
            sProperty = (string)objTemp;
        }

        private void GetProperty(string propertyName, string configuration, out OutputType oProperty)
        {
            object objTemp;
            GetProperty(propertyName, configuration, out objTemp, typeof(OutputType));
            if (objTemp == null)
            {
                oProperty = OutputType.NotSet;
            }
            else
            {
                oProperty = (OutputType)objTemp;
            }
        }

        private void GetProperty(string propertyName, string configuration, out object objProperty, Type propertyType) {
            var props = from p in PropertyGroups.Elements(propertyName)
                               select p;
            
            if (props.Count() > 0)
            {                

                if (propertyType == typeof(bool?))
                {
                    objProperty = props.First().Value == bool.TrueString;
                }
                else if (propertyType == typeof(int?))
                {
                    int temp = -1;
                    int.TryParse(props.First().Value, out temp);
                    objProperty = temp;
                }
                else if (propertyType == typeof(string))
                {
                    objProperty = props.First().Value;
                }
                else if (propertyType == typeof(LinqToVisualStudioSolution.OutputType))
                {
                    try
                    {
                        objProperty = (LinqToVisualStudioSolution.OutputType)Enum.Parse(typeof(LinqToVisualStudioSolution.OutputType), props.First().Value, true);
                    }
                    catch (ArgumentException)
                    {
                        objProperty = OutputType.NotSet;
                    }
                }
                else
                {
                    objProperty = null;
                }
            }
            else
            {
                objProperty = null;
            }
            
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var err in this.Errors)
            {
                sb.Append("Error: ");
                sb.AppendLine(err);
            }

            sb.Append("\t");
            sb.Append(this.ProjectName);
            sb.AppendLine(":");
            sb.Append("\t\t");
            sb.AppendFormat("Path:{0}\n", this.ProjectPath);
            sb.Append("\t\t");
            sb.AppendFormat("Guid:{0}\n", this.Guid);
			sb.Append("\t\t");
			sb.AppendFormat("Configuration:{0}\n", this.Configuration);
			sb.Append("\t\t");
			sb.AppendFormat("Platform:{0}\n", this.Platform);
			sb.Append("\t\t");
			sb.AppendFormat("ProductVersion:{0}\n", this.ProductVersion);
			sb.Append("\t\t");
			sb.AppendFormat("SchemaVersion :{0}\n", this.SchemaVersion );
			sb.Append("\t\t");
			sb.AppendFormat("OutputType:{0}\n", Enum.GetName(typeof(LinqToVisualStudioSolution.OutputType), this.OutputType));
			sb.Append("\t\t");
			sb.AppendFormat("AppDesignerFolder:{0}\n", this.AppDesignerFolder);
			sb.Append("\t\t");
			sb.AppendFormat("RootNamespace:{0}\n", this.RootNamespace);
			sb.Append("\t\t");
			sb.AppendFormat("AssemblyName:{0}\n", this.AssemblyName);
			sb.Append("\t\t");
			sb.AppendFormat("TargetFrameworkVersion:{0}\n", this.TargetFrameworkVersion);
			sb.Append("\t\t");
			sb.AppendFormat("FileAlignment:{0}\n", this.FileAlignment);
			sb.Append("\t\t");
			sb.AppendFormat("DebugSymbols:{0}\n", this.DebugSymbols);
			sb.Append("\t\t");
			sb.AppendFormat("DebugType:{0}\n", this.DebugType);
			sb.Append("\t\t");
			sb.AppendFormat("Optimize:{0}\n", this.Optimize);
			sb.Append("\t\t");
			sb.AppendFormat("OutputPath:{0}\n", this.OutputPath);
            sb.Append("\t\t");
            sb.AppendFormat("OutputPathFull:{0}\n", this.OutputPathFull);
            sb.Append("\t\t");
            sb.AppendFormat("OutputPathToAssemblyFull:{0}\n", this.OutputPathToAssemblyFull);
            sb.Append("\t\t");
			sb.AppendFormat("DefineConstants:{0}\n", this.DefineConstants);
			sb.Append("\t\t");
			sb.AppendFormat("ErrorReport:{0}\n", this.ErrorReport);
			sb.Append("\t\t");
			sb.AppendFormat("WarningLevel:{0}\n", this.WarningLevel);
            sb.Append("\t\t");
            sb.Append("Code Files:\n");
            foreach (var codefile in this.CodeFiles)
            {
                sb.AppendFormat("\t\t\t{0}\n", codefile);
            }
            sb.Append("\t\t");
            sb.Append("References:\n");

            foreach (var reference in this.References)
            {
                sb.AppendFormat("\t\t\t{0}\n", reference.ToString());
            }

            sb.Append("\t\t");
            sb.Append("Imports:\n");

            foreach (var import in this.Imports)
            {
                sb.AppendFormat("\t\t\t{0}\n", import);
            }

            return sb.ToString();
        }
            
    }
}
