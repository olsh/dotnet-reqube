using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

using CommandLine;

using Newtonsoft.Json;

using ReQube.Models;
using ReQube.Models.ReSharper;
using ReQube.Models.SonarQube;

namespace ReQube
{
    internal class Program
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        private static void Convert(Options opts)
        {
            StreamReader reader = null;
            try
            {
                var serializer = new XmlSerializer(typeof(Report));

                Console.WriteLine("Reading input file {0}", opts.Input);
                reader = new StreamReader(opts.Input);
                var report = (Report)serializer.Deserialize(reader);
                reader.Dispose();

                var sonarQubeReports = Map(report);

                // We need to write dummy report because SonarQube MSBuild reads a report from the root
                WriteReport(string.IsNullOrEmpty(opts.Directory) ? opts.Output : Path.Combine(opts.Directory, opts.Output), new SonarQubeReport());

                foreach (var sonarQubeReport in sonarQubeReports)
                {
                    var projectDirectory = !string.IsNullOrEmpty(opts.Directory)
                                               ? Path.Combine(opts.Directory, sonarQubeReport.ProjectName)
                                               : sonarQubeReport.ProjectName;

                    if (!Directory.Exists(projectDirectory))
                    {
                        Directory.CreateDirectory(projectDirectory);
                    }

                    var filePath = Path.Combine(projectDirectory, opts.Output);

                    WriteReport(filePath, sonarQubeReport);
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

        private static void WriteReport(string filePath, SonarQubeReport sonarQubeReport)
        {
            Console.WriteLine("Writing output files {0}", filePath);

            File.WriteAllText(filePath, JsonConvert.SerializeObject(sonarQubeReport, JsonSerializerSettings));
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
        }

        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(opts => Convert(opts))
                .WithNotParsed(errs => HandleParseError(errs));
        }

        private static IEnumerable<SonarQubeReport> Map(Report report)
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
                                                             Message = issueType.Description,
                                                             TextRange =
                                                                 new TextRange
                                                                     {
                                                                         // SonarQube lines are not zero-based
                                                                         StartLine = issue.Line + 1
                                                                     }
                                                         }
                                             };

                    sonarQubeReport.Issues.Add(sonarQubeIssue);
                }

                sonarQubeReports.Add(sonarQubeReport);
            }

            return sonarQubeReports;
        }
    }
}
