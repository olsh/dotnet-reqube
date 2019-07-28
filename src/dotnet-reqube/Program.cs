using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

using CommandLine;

using Newtonsoft.Json;

using Onion.SolutionParser.Parser;

using ReQube.Models;
using ReQube.Models.ReSharper;
using ReQube.Models.SonarQube;

namespace ReQube
{
    internal static class Program
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        private static void Convert(Options options)
        {
            StreamReader reader = null;
            try
            {
                var serializer = new XmlSerializer(typeof(Report));

                Console.WriteLine("Reading input file {0}", options.Input);
                reader = new StreamReader(options.Input);
                var report = (Report)serializer.Deserialize(reader);
                reader.Dispose();

                var sonarQubeReports = Map(report);

                // We need to write dummy report because SonarQube MSBuild reads a report from the root
                if (string.IsNullOrEmpty(options.Project))
                {
                    WriteReport(CombineOutputPath(options, options.Output), SonarQubeReport.Empty);

                    foreach (var sonarQubeReport in sonarQubeReports)
                    {
                        var filePath = CombineOutputPath(options, Path.Combine(sonarQubeReport.ProjectName, options.Output));

                        WriteReport(filePath, sonarQubeReport);
                    }

                    TryWriteMissingReports(report.Information.Solution, options, sonarQubeReports);
                }
                else
                {
                    var projectToWrite = sonarQubeReports.FirstOrDefault(r => string.Equals(r.ProjectName, options.Project, StringComparison.OrdinalIgnoreCase));
                    WriteReport(CombineOutputPath(options, options.Output), projectToWrite ?? SonarQubeReport.Empty);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
            finally
            {
                reader?.Dispose();
            }
        }

        private static string CombineOutputPath(Options options, string directory)
        {
            return string.IsNullOrEmpty(options.Directory) ? directory : Path.Combine(options.Directory, directory);
        }

        private static void WriteReport(string filePath, SonarQubeReport sonarQubeReport)
        {
            Console.WriteLine("Writing output files {0}", filePath);

            var projectDirectory = Path.GetDirectoryName(filePath);
            if (projectDirectory != null && !Directory.Exists(projectDirectory))
            {
                Directory.CreateDirectory(projectDirectory);
            }

            File.WriteAllText(filePath, JsonConvert.SerializeObject(sonarQubeReport, JsonSerializerSettings));
        }

        private static void HandleParseError()
        {
        }

        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Convert)
                .WithNotParsed(errs => HandleParseError());
        }

        private static List<SonarQubeReport> Map(Report report)
        {
            var reportIssueTypes = report.IssueTypes.ToDictionary(t => t.Id, type => type);

            var sonarQubeReports = new List<SonarQubeReport>();

            foreach (var project in report.Issues)
            {
                var sonarQubeReport = new SonarQubeReport { ProjectName = project.Name };
                var replaceFileNameRegex = new Regex($@"^{Regex.Escape(project.Name)}\\");
                foreach (var issue in project.Issue)
                {
                    if (!reportIssueTypes.TryGetValue(issue.TypeId, out ReportIssueType issueType))
                    {
                        Console.WriteLine("Unable to find issue type {0}.", issue.TypeId);

                        continue;
                    }

                    if (!Constants.ReSharperToSonarQubeSeverityMap.TryGetValue(issueType.Severity, out string sonarQubeSeverity))
                    {
                        Console.WriteLine("Unable to map ReSharper severity {0} to SonarQube", issueType.Severity);

                        continue;
                    }

                    var sonarQubeIssue = new Issue
                                             {
                                                 EngineId = Constants.EngineId,
                                                 RuleId = issue.TypeId,
                                                 Type = Constants.SonarQubeCodeSmellType,
                                                 Severity = sonarQubeSeverity,
                                                 PrimaryLocation =
                                                     new PrimaryLocation
                                                         {
                                                             FilePath = replaceFileNameRegex.Replace(issue.File, string.Empty),
                                                             Message = issue.Message,
                                                             TextRange =
                                                                 new TextRange
                                                                     {
                                                                         // For some reason, some issues doesn't have line, but actually they are on the first one
                                                                         StartLine = issue.Line > 0 ? issue.Line : 1
                                                                     }
                                                         }
                                             };

                    sonarQubeReport.Issues.Add(sonarQubeIssue);
                }

                sonarQubeReports.Add(sonarQubeReport);
            }

            return sonarQubeReports;
        }

        private static void TryWriteMissingReports(string solutionFile, Options options, List<SonarQubeReport> sonarQubeReports)
        {
            try
            {
                var solution = SolutionParser.Parse(solutionFile);
                foreach (var project in solution.Projects)
                {
                    // We should skip solution directories
                    if (!project.Path.EndsWith(".csproj"))
                    {
                        continue;
                    }

                    if (sonarQubeReports.Any(r => string.Equals(r.ProjectName, project.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    var projectPath = Path.GetDirectoryName(project.Path);
                    var reportPath = CombineOutputPath(options, Path.Combine(projectPath, options.Output));
                    WriteReport(reportPath, SonarQubeReport.Empty);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
