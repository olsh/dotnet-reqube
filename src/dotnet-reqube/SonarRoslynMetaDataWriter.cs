using ReQube.Logging;
using ReQube.Utils;
using Serilog;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ReQube
{
    public class SonarRoslynMetaDataWriter : ISonarMetaDataWriter
    {
        private readonly DirectoryInfo _sonarDirInfo;

        private ILogger Logger { get; } = LoggerFactory.GetLogger();

        public SonarRoslynMetaDataWriter(string sonarDir)
        {
            _sonarDirInfo = new DirectoryInfo(sonarDir);
        }

        public void AddReSharperAnalysisPaths(List<KeyValuePair<string, string>> reportPathsByProject)
        {
            var projectInfoFiles = _sonarDirInfo.GetFiles(
                "ProjectInfo.xml",
                new EnumerationOptions
                {
                    RecurseSubdirectories = true
                });

            foreach (var projectInfoFile in projectInfoFiles)
            {
                AddReSharperAnalysisPaths(projectInfoFile, reportPathsByProject);
            }
        }

        private void AddReSharperAnalysisPaths(
            FileInfo projectInfoFile, List<KeyValuePair<string, string>> reportPathsByProject)
        {
            var projectInfo = XElement.Load(projectInfoFile.FullName);
            var ns = projectInfo.GetDefaultNamespace();

            var projectPath = projectInfo.RequiredElement(ns + "FullPath").Value;
            var reSharperRoslynFile = reportPathsByProject
                .Single(x => x.Value == Path.GetFileNameWithoutExtension(projectPath)).Key;

            if (reSharperRoslynFile == null || !File.Exists(reSharperRoslynFile))
            {
                Logger.Information(
                    $"{reSharperRoslynFile} is not found, no changes to {projectInfoFile.FullName} are needed.");
                return;
            }

            var projectLanguage = projectInfo.RequiredElement(ns + "ProjectLanguage").Value;
            var sonarLanguage = projectLanguage == "C#" ? "cs" : "vbnet";
            var analysisSettings = projectInfo.RequiredElement(ns + "AnalysisSettings");
            var reportFilePathNameAttribute = $"sonar.{sonarLanguage}.roslyn.reportFilePath";
            var projectOutPathNameAttribute = $"sonar.{sonarLanguage}.analyzer.projectOutPath";

            var reportFilePathProperty =
                analysisSettings
                    .Elements()
                    .FirstOrDefault(p => p.Attribute("Name")?.Value == reportFilePathNameAttribute);

            if (reportFilePathProperty != null)
            {
                var reportFilePaths =
                    reportFilePathProperty.Value
                        .Split("|").Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToHashSet();
                reportFilePaths.Add(reSharperRoslynFile);
                reportFilePathProperty.Value = string.Join("|", reportFilePaths);
            } else
            {
                reportFilePathProperty = new XElement(ns + "Property", reSharperRoslynFile);
                reportFilePathProperty.SetAttributeValue("Name", reportFilePathNameAttribute);
                analysisSettings.Add(reportFilePathProperty);
            }

            var projectOutPathProperty =
                analysisSettings
                    .Elements()
                    .FirstOrDefault(p => p.Attribute("Name")?.Value == projectOutPathNameAttribute);

            if (projectOutPathProperty != null)
            {
                projectOutPathProperty.Value = projectInfoFile.DirectoryName ?? string.Empty;
            } else
            {
                projectOutPathProperty = new XElement(ns + "Property", projectInfoFile.DirectoryName);
                projectOutPathProperty.SetAttributeValue("Name", projectOutPathNameAttribute);
                analysisSettings.Add(projectOutPathProperty);
            }

            projectInfo.Save(projectInfoFile.FullName);

            Logger.Information(
                $"{reSharperRoslynFile} is successfully added to {projectInfoFile.FullName}.");
        }
    }
}