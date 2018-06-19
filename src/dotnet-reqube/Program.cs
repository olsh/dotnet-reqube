using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

                var sonarQubeReport = Map(report);

                Console.WriteLine("Writing output file {0}", opts.Output);
                File.WriteAllText(opts.Output, JsonConvert.SerializeObject(sonarQubeReport));
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

        private static void HandleParseError(IEnumerable<Error> errs)
        {
        }

        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(opts => Convert(opts))
                .WithNotParsed(errs => HandleParseError(errs));
        }

        private static SonarQubeReport Map(Report report)
        {
            var reportIssueTypes = report.IssueTypes.ToDictionary(t => t.Id, type => type);

            var sonarQubeReport = new SonarQubeReport { Issues = new LinkedList<Issue>() };

            foreach (var project in report.Issues)
            {
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

                    var offsets = issue.Offset.Split('-');
                    int startColumn = 0;
                    if (offsets.Length > 0 && int.TryParse(offsets[0], out int start))
                    {
                        startColumn = start;
                    }

                    int endColumn = 0;
                    if (offsets.Length > 0 && int.TryParse(offsets[0], out int end))
                    {
                        endColumn = end;
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
                                                             FilePath = issue.File,
                                                             Message = issueType.Description,
                                                             TextRange =
                                                                 new TextRange
                                                                     {
                                                                         StartLine = issue.Line,
                                                                         StartColumn = startColumn,
                                                                         EndColumn = endColumn
                                                                     }
                                                         }
                                             };

                    sonarQubeReport.Issues.Add(sonarQubeIssue);
                }
            }

            return sonarQubeReport;
        }
    }
}
