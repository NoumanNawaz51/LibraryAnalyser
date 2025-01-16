using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Xml.Linq;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Concurrent;

namespace LibraryAnalyser

{
    public class ProjectDependencyAnalyzer
    {

        public static void AnalyzeProject(string projectPath)
        {

            try
            {
                var projectFile = XDocument.Load(projectPath);
                var ns = projectFile.Root?.GetDefaultNamespace();

                Console.WriteLine($"Project: {Path.GetFileNameWithoutExtension(projectPath)}");
                Console.WriteLine("----------------------------------------");
                var projName = Path.GetFileNameWithoutExtension(projectPath);

            var references = projectFile.Descendants(ns + "Reference")
                    .Select(x => new
                    {
                        Include = x.Attribute("Include")?.Value,
                        HintPath = x.Element(ns + "HintPath")?.Value,
                        //SpecificVersion = x.Element(ns + "SpecificVersion")?.Value
                    })
                    .ToList();

                if (references.Any())
                {
                    foreach (var reference in references)
                    {
                        string type = reference.Include.Contains("System") ? "System"
               : reference.HintPath?.Contains("packages") == true ? "Nuget"
               : "Thirdparty";

                        string outputLine = $"Project={projName}, Library={reference.Include}, HintPath={reference.HintPath ?? ""}, Type={type}";
                        Console.WriteLine(outputLine);
                    }
                }


                var projectRefs = projectFile.Descendants(ns + "ProjectReference")
               .Select(x => new
               {
                   Include = x.Attribute("Include")?.Value,
                   Name = x.Element(ns + "Name")?.Value,
                   HintPath = x.Element(ns + "HintPath")?.Value
               });
                Console.WriteLine("--------------------------------------");
                foreach (var proj in projectRefs)
                {
                    string type = proj.HintPath?.Contains("packages") == true ? "Nuget" : "Project";

                    string outputLine = $"Project={proj.Name}, Library={(!string.IsNullOrEmpty(proj.Name) ? proj.Name : Path.GetFileNameWithoutExtension(proj.Include))}, HintPath={proj.HintPath ?? ""}, Type={type}";
                    Console.WriteLine(outputLine);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error analyzing project {projectPath}: {ex.Message}");
            }

        }

        public static void AnalyzeSolution(string solutionDirectory)
        {
            try
            {
                // Normalize the path
                solutionDirectory = Path.GetFullPath(solutionDirectory);

                if (!Directory.Exists(solutionDirectory))
                {
                    Console.WriteLine($"Directory not found: {solutionDirectory}");
                    return;
                }

                // Use EnumerateFiles instead of GetFiles for better memory usage with large directories
                var projectFiles = Directory.EnumerateFiles(solutionDirectory, "*.csproj", SearchOption.AllDirectories);
                
                  bool foundProjects = false;

                foreach (var projFile in projectFiles)
                {
                    Console.WriteLine("\n************************************************************************************************************************");

                    try
                    {
                        foundProjects = true;
                        Console.WriteLine($"\nAnalyzing: {projFile}");
                        string projectName = Path.GetFileNameWithoutExtension(projFile);

                         AnalyzeProject(projFile);
                        //Console.WriteLine("\n-------------------");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing project {projFile}: {ex.Message}");
                    }

                    Console.WriteLine("\n************************************************************************************************************************");

                }

                if (!foundProjects)
                {
                    Console.WriteLine("No .csproj files found in the specified directory and its subdirectories.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error analyzing solution directory: {ex.Message}");
            }
        }
    }
}
