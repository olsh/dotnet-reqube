using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

using Onion.SolutionParser.Parser;
using Onion.SolutionParser.Parser.Model;
using ReQube.Logging;
using ReQube.Models;
using ReQube.Models.ReSharper;
using ReQube.Models.SonarQube;
using ReQube.Models.SonarQube.Generic;
using Serilog;
using Constants = ReQube.Models.Constants;

namespace ReQube
{
    public class SonarConverter : ISonarConverter
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings
            = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        public SonarReportGeneratorFactory SonarReportGeneratorFactory { get; set; } 
            = new SonarReportGeneratorFactory();

        internal ILogger Logger { private get; set; } = LoggerFactory.GetLogger();

        public void Convert(Options options)
        {
            var serializer = new XmlSerializer(typeof(Report));

            Logger.Information("Reading input file {0}...", options.Input);

            Report report;

            using (var reader = new StreamReader(options.Input))
            {
                report = (Report)serializer.Deserialize(reader);
            }

            ConvertToAbsolutePaths(report);

            if (string.IsNullOrEmpty(options.Project))
            {
                ConvertSolution(report, options);
            }
            else
            {
                ConvertProject(report, options);
            }
        }

        internal static void ConvertToAbsolutePaths(Report reSharperReport)
        {
            if (!Path.IsPathFullyQualified(reSharperReport.Information.Solution))
            {
                // if the path is not fully qualified, it is assumed to be relative to the current dir,
                // preferrably ReSharper's inspectcode should be run with the "-a" option, to generate absolute paths
                reSharperReport.Information.Solution = Path.GetFullPath(reSharperReport.Information.Solution);
            }

            var solutionDir = Path.GetDirectoryName(reSharperReport.Information.Solution);

            foreach (var project in reSharperReport.Issues)
            {
                foreach (var issue in project.Issue)
                {
                    if (!Path.IsPathFullyQualified(issue.File))
                    {
                        // if file paths are not fully qualified, they are relative to the solution directory
                        issue.File = Path.Combine(solutionDir, issue.File);
                    }
                }
            }
        }

        private void ConvertSolution(Report report, Options options)
        {
            if (Path.IsPathRooted(options.Output))
            {
                throw new ArgumentException("Absolute paths are not allowed with -output, when converting a sln.");
            }

            var solution = SolutionParser.Parse(report.Information.Solution);
            ValidateSolution(solution);

            var baseDir = string.IsNullOrEmpty(options.Directory) 
                ? Path.GetDirectoryName(report.Information.Solution) : options.Directory;
            var sonarReports = SonarReportGeneratorFactory.GetGenerator(options.OutputFormat).Generate(report);

            if (options.OutputFormat == SonarOutputFormat.Generic)
            {
                // We need to write dummy report because SonarQube MSBuild reads a report from the root
                WriteReport(
                    GetOutputPath(baseDir, options.Output, string.Empty),
                    SonarGenericReport.Empty);
            }

            foreach (var sonarReport in sonarReports)
            {
                var filePath = GetOutputPath(
                    baseDir,
                    Path.Combine(GetFolderProject(solution, sonarReport.ProjectName), options.Output),
                    sonarReport.ProjectName);
                WriteReport(filePath, sonarReport);
            }

            TryWriteMissingReports(solution, baseDir, options, sonarReports);
        }

        private void ConvertProject(Report report, Options options)
        {
            var projectFileInfo = new FileInfo(options.Project);

            if (!projectFileInfo.Exists)
            {
                throw new FileNotFoundException("Project not found.", options.Project);
            }

            var projectName = projectFileInfo.Name;
            var projectNameWithoutExtension = Path.GetFileNameWithoutExtension(projectName);
            var sonarReports = SonarReportGeneratorFactory.GetGenerator(options.OutputFormat).Generate(report);

            var projectToWrite = sonarReports.FirstOrDefault(
                r => string.Equals(projectName, projectFileInfo.Name, StringComparison.OrdinalIgnoreCase));

            if (projectToWrite == null)
            {
                Logger.Information("Project " + options.Project + " contains no issues.");
            }
            else
            {
                var outputFile = string.IsNullOrEmpty(options.Directory)
                    ? GetOutputPath(projectFileInfo.DirectoryName, options.Output, projectNameWithoutExtension)
                    : GetOutputPath(options.Directory, options.Output, projectNameWithoutExtension);

                WriteReport(outputFile, projectToWrite);
            }
        }

        private string GetFolderProject(ISolution solution, string projectName)
        {
            var path = solution.Projects
                .Where(x => x.Name == projectName && x.TypeGuid != Constants.ProjectTypeGuids["Solution Folder"])
                .Select(x => x.Path)
                .FirstOrDefault();

            return path != null ? Path.GetDirectoryName(path) : projectName;
        }

        private string GetOutputPath(string baseDir, string filePath, string projectName)
        {
            return (string.IsNullOrEmpty(baseDir) ? filePath : Path.Combine(baseDir, filePath))
                .Replace("<ProjectName>", projectName);
        }

        private void WriteReport(string filePath, ISonarReport sonarReport)
        {
            Logger.Information("Writing output files {0}", filePath);

            var projectDirectory = new FileInfo(filePath).Directory;

            if (projectDirectory == null)
            {
                throw new ArgumentException("Invalid file path.", nameof(filePath));
            }

            if (!projectDirectory.Exists)
            {
                projectDirectory.Create();
            }

            File.WriteAllText(filePath, JsonConvert.SerializeObject(sonarReport, JsonSerializerSettings));
        }

        private void TryWriteMissingReports(
            ISolution solution, string baseDir, Options options, IList<ISonarReport> sonarReports)
        {
            if (options.OutputFormat == SonarOutputFormat.Roslyn)
            {
                // for Roslyn format, there is no need to write empty reports
                return;
            }

            foreach (var project in solution.Projects)
            {
                // We should skip solution directories
                if (!project.Path.EndsWith(".csproj"))
                {
                    continue;
                }

                if (sonarReports.Any(
                    r => string.Equals(r.ProjectName, project.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                var projectDir = Path.GetDirectoryName(project.Path);
                var reportPath = GetOutputPath(
                    baseDir, Path.Combine(projectDir, options.Output), project.Name);
                WriteReport(reportPath, SonarGenericReport.Empty);
            }
        }
        
        private void ValidateSolution(ISolution solution)
        {
            foreach (var project in solution.Projects)
            {
                if (Path.IsPathRooted(project.Path))
                {
                    throw new ArgumentException("Solution cannot contain absolute project paths.");
                }
            }
        }
    }
}
