/*
 * TAGS:  Parse, Solution File, sln
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace LinqToVisualStudioSolution
{
    public class Solution : List<Project>
    {
        /// <summary>
        /// Keep a list of errors instead of halting execution flow.
        /// </summary>
        public List<string> Errors { get; private set; }

        public string SolutionPath { get; private set; }
        //public List<Project> Projects { get; private set; }

        /// <summary>
        /// Creates a List of Project objects from the projects within the solution.
        /// Limitations: If any projects withing the solution are not C# projects, they will be skipped.
        /// </summary>
        /// <param name="solutionPath">Path to the solution to load.</param>
        public Solution(string solutionPath) : base()
        {
            this.Errors = new List<string>();

            this.SolutionPath = solutionPath;
            //this.Projects = new List<Project>();

            string[] solutionAllLines = File.ReadAllLines(solutionPath);

            // Only bother with the lines that specify what projects are in the solution.

            var solutionProjectLines = from p in solutionAllLines
                                       where p.StartsWith("Project")
                                       select p;
            
            // Fill the List


            foreach (var projectLine in solutionProjectLines)
            {
                Environment.CurrentDirectory = Path.GetDirectoryName(solutionPath);

                string pathToProject = Regex.Match(projectLine, @", ""(.+\...proj)").Groups[1].Value;
                string guid = Regex.Match(projectLine, @", ""(\{.+\})").Groups[1].Value;
                string projectName = Regex.Match(projectLine, @"= \""(\w+)\""").Groups[1].Value;

                if (pathToProject.Length == 0)
                {
                    this.Errors.Add(solutionPath + " contains a project without a valid path:" + projectName);
                    continue;
                }

                if (!pathToProject.StartsWith("http:"))
                {
                    pathToProject = Path.GetFullPath(pathToProject);
                }

                Project pr = new Project(
                    guid,
                    projectName,
                    pathToProject);

                this.Add(pr);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.SolutionPath);
            foreach (var err in this.Errors)
            {
                sb.Append("Error:");
                sb.AppendLine(err);
            }
            sb.AppendLine(":");
            foreach (var project in this)
            {
                sb.AppendLine(project.ToString());
            }

            return sb.ToString();
        }
    }
}
