using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace ReQube.Tests
{
    public class SonarMetaDataWriterTest
    {
        private static readonly string ProjectDir = Path.GetFullPath("tmp/projects");
        private static readonly string SonarDir = Path.GetFullPath("tmp/.sonarqube");

        private readonly ITestOutputHelper _output;

        public SonarMetaDataWriterTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void AddReSharperAnalysisPaths_ShouldModifyProjectInfoFilesCorrectly()
        {
            CreateSonarAndProjectStructure();

            var sonarMetaDataWriter = new SonarRoslynMetaDataWriter(SonarDir);
            var reportPathsByProject = new Dictionary<string, string>();

            for (int i = 0; i < 5; i++)
            {
                reportPathsByProject.Add($"test{i}", Path.Combine(ProjectDir, $"test{i}.ReSharper.RoslynCA.json"));
            }

            sonarMetaDataWriter.AddReSharperAnalysisPaths(reportPathsByProject);

            ValidateOutputs();
        }

        private void ValidateOutputs()
        {
            var projectInfos = new string[5];

            projectInfos[0] = $@"
                    <?xml version=""1.0"" encoding=""utf-8""?>
                    <ProjectInfo xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                                 xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
                                 xmlns=""http://www.sonarsource.com/msbuild/integration/2015/1"">
                      <ProjectLanguage>C#</ProjectLanguage>
                      <FullPath>{Path.Combine(ProjectDir, "test0.csproj")}</FullPath>
                      <IsExcluded>false</IsExcluded>
                      <AnalysisSettings></AnalysisSettings>
                      <Configuration>Release</Configuration>
                      <Platform>AnyCPU</Platform>
                      <TargetFramework>netcoreapp3.1</TargetFramework>
                    </ProjectInfo>
                ".Trim();

            projectInfos[1] = $@"
                    <?xml version=""1.0"" encoding=""utf-8""?>
                    <ProjectInfo xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                                 xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
                                 xmlns=""http://www.sonarsource.com/msbuild/integration/2015/1"">
                      <ProjectLanguage>C#</ProjectLanguage>
                      <FullPath>{Path.Combine(ProjectDir, "test1.csproj")}</FullPath>
                      <IsExcluded>false</IsExcluded>
                      <AnalysisSettings>
                        <Property Name=""sonar.cs.roslyn.reportFilePath"">
                            /tmp/test1.json|{Path.Combine(ProjectDir, "test1.ReSharper.RoslynCA.json")}
                        </Property>
                        <Property Name=""sonar.cs.analyzer.projectOutPath"">{Path.Combine(SonarDir, "1")}</Property>
                      </AnalysisSettings>
                      <Configuration>Release</Configuration>
                      <Platform>AnyCPU</Platform>
                      <TargetFramework>netcoreapp3.1</TargetFramework>
                    </ProjectInfo>
                ".Trim();

            projectInfos[2] = $@"
                    <?xml version=""1.0"" encoding=""utf-8""?>
                    <ProjectInfo xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                                 xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
                                 xmlns=""http://www.sonarsource.com/msbuild/integration/2015/1"">
                      <ProjectLanguage>C#</ProjectLanguage>
                      <FullPath>{Path.Combine(ProjectDir, "test2.csproj")}</FullPath>
                      <IsExcluded>false</IsExcluded>
                      <AnalysisSettings>
                        <Property Name=""sonar.cs.roslyn.reportFilePath"">
                            /tmp/test1.json|/tmp/test2.json|{Path.Combine(ProjectDir, "test2.ReSharper.RoslynCA.json")}
                        </Property>
                        <Property Name=""sonar.cs.analyzer.projectOutPath"">{Path.Combine(SonarDir, "2")}</Property>
                      </AnalysisSettings>
                      <Configuration>Release</Configuration>
                      <Platform>AnyCPU</Platform>
                      <TargetFramework>netcoreapp3.1</TargetFramework>
                    </ProjectInfo>
                ".Trim();

            projectInfos[3] = $@"
                    <?xml version=""1.0"" encoding=""utf-8""?>
                    <ProjectInfo xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                                 xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
                                 xmlns=""http://www.sonarsource.com/msbuild/integration/2015/1"">
                      <ProjectLanguage>C#</ProjectLanguage>
                      <FullPath>{Path.Combine(ProjectDir, "test3.csproj")}</FullPath>
                      <IsExcluded>false</IsExcluded>
                      <AnalysisSettings>
                        <Property Name=""sonar.cs.roslyn.reportFilePath"">
                            {Path.Combine(ProjectDir, "test3.ReSharper.RoslynCA.json")}
                        </Property>
                        <Property Name=""sonar.cs.analyzer.projectOutPath"">{Path.Combine(SonarDir, "3")}</Property>
                      </AnalysisSettings>
                      <Configuration>Release</Configuration>
                      <Platform>AnyCPU</Platform>
                      <TargetFramework>netcoreapp3.1</TargetFramework>
                    </ProjectInfo>
                ".Trim();

            projectInfos[4] = $@"
                    <?xml version=""1.0"" encoding=""utf-8""?>
                    <ProjectInfo xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                                 xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
                                 xmlns=""http://www.sonarsource.com/msbuild/integration/2015/1"">
                      <ProjectLanguage>VB</ProjectLanguage>
                      <FullPath>{Path.Combine(ProjectDir, "test4.csproj")}</FullPath>
                      <IsExcluded>false</IsExcluded>
                      <AnalysisSettings>
                        <Property Name=""sonar.vbnet.roslyn.reportFilePath"">
                            {Path.Combine(ProjectDir, "test4.ReSharper.RoslynCA.json")}
                        </Property>
                        <Property Name=""sonar.vbnet.analyzer.projectOutPath"">{Path.Combine(SonarDir, "4")}</Property>
                      </AnalysisSettings>
                      <Configuration>Release</Configuration>
                      <Platform>AnyCPU</Platform>
                      <TargetFramework>netcoreapp3.1</TargetFramework>
                    </ProjectInfo>
                ".Trim();

            for (var i = 0; i < projectInfos.Length; i++)
            {
                XElement expected = null;
                XElement actual = null;

                try
                {
                    expected = XElement.Parse(NormalizeXml(projectInfos[i]));
                    actual = XElement.Load(Path.Combine(SonarDir, i.ToString(), "ProjectInfo.xml"));

                    AssertEqualProjectInfoXml(expected, actual);
                }
                catch (AssertActualExpectedException)
                {
                    _output.WriteLine($"Project {i}, expected XML: {expected}, actual XML: {actual}.");
                    throw;
                }
            }
        }

        private void CreateSonarAndProjectStructure()
        {
            if (Directory.Exists(ProjectDir))
            {
                Directory.Delete(ProjectDir, true);
            }

            if (Directory.Exists(SonarDir))
            {
                Directory.Delete(SonarDir, true);
            }

            Directory.CreateDirectory(SonarDir);
            Directory.CreateDirectory(ProjectDir);

            var projectInfoTemplate = $@"
                    <?xml version=""1.0"" encoding=""utf-8""?>
                    <ProjectInfo xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                                 xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                                 xmlns=""http://www.sonarsource.com/msbuild/integration/2015/1"">
                      <ProjectLanguage>C#</ProjectLanguage>
                      <FullPath>{"{0}"}</FullPath>
                      <IsExcluded>false</IsExcluded>
                      <AnalysisSettings>{"{1}"}</AnalysisSettings>
                      <Configuration>Release</Configuration>
                      <Platform>AnyCPU</Platform>
                      <TargetFramework>netcoreapp3.1</TargetFramework>
                    </ProjectInfo>
                ".Trim();

            var project0 = Path.Combine(ProjectDir, "test0.csproj");
            var analysisSettings0 = "";

            var projectInfo0 = NormalizeXml(string.Format(projectInfoTemplate, project0, analysisSettings0));

            Directory.CreateDirectory(Path.Combine(SonarDir, "0"));
            File.WriteAllText(Path.Combine(SonarDir, "0", "ProjectInfo.xml"), projectInfo0);

            var project1 = Path.Combine(ProjectDir, "test1.csproj");
            var report1 = Path.Combine(ProjectDir, "test1.ReSharper.RoslynCA.json");
            var analysisSettings1 = $@"
                    <Property Name=""sonar.cs.roslyn.reportFilePath"">
                        /tmp/test1.json|{Path.Combine(ProjectDir, "test1.ReSharper.RoslynCA.json")}
                    </Property>
                    <Property Name=""sonar.cs.analyzer.projectOutPath"">dummy string</Property>
                ";

            var projectInfo1 = NormalizeXml(string.Format(projectInfoTemplate, project1, analysisSettings1));

            Directory.CreateDirectory(Path.Combine(SonarDir, "1"));
            File.WriteAllText(Path.Combine(SonarDir, "1", "ProjectInfo.xml"), projectInfo1);
            File.WriteAllText(report1, string.Empty);

            var project2 = Path.Combine(ProjectDir, "test2.csproj");
            var report2 = Path.Combine(ProjectDir, "test2.ReSharper.RoslynCA.json");
            var analysisSettings2 = @"
                    <Property Name=""sonar.cs.roslyn.reportFilePath"">
                        /tmp/test1.json|/tmp/test2.json
                    </Property>
                ";

            var projectInfo2 = NormalizeXml(string.Format(projectInfoTemplate, project2, analysisSettings2));

            Directory.CreateDirectory(Path.Combine(SonarDir, "2"));
            File.WriteAllText(Path.Combine(SonarDir, "2", "ProjectInfo.xml"), projectInfo2);
            File.WriteAllText(report2, string.Empty);

            var project3 = Path.Combine(ProjectDir, "test3.csproj");
            var report3 = Path.Combine(ProjectDir, "test3.ReSharper.RoslynCA.json");
            var analysisSettings3 = @"
                    <Property Name=""sonar.cs.roslyn.reportFilePath"">
                        
                    </Property>
                ";

            Directory.CreateDirectory(Path.Combine(SonarDir, "3"));
            var projectInfo3 = NormalizeXml(string.Format(projectInfoTemplate, project3, analysisSettings3));

            File.WriteAllText(Path.Combine(SonarDir, "3", "ProjectInfo.xml"), projectInfo3);
            File.WriteAllText(report3, string.Empty);

            var project4 = Path.Combine(ProjectDir, "test4.csproj");
            var report4 = Path.Combine(ProjectDir, "test4.ReSharper.RoslynCA.json");
            var analysisSettings4 = "";

            var projectInfo4 = NormalizeXml(string.Format(projectInfoTemplate, project4, analysisSettings4));

            Directory.CreateDirectory(Path.Combine(SonarDir, "4"));
            File.WriteAllText(Path.Combine(SonarDir, "4", "ProjectInfo.xml"), projectInfo4.Replace("C#", "VB"));
            File.WriteAllText(report4, string.Empty);
        }

        private static string NormalizeXml(string xml)
        {
            return string.Join(
                string.Empty, 
                xml.Split('\n')
                .Select(line => {
                    var newLine = line.Trim().Trim('\r');
                    if (newLine.EndsWith("\""))
                    {
                        newLine += " ";
                    }

                    return newLine;
                }));
        }

        private void AssertEqualProjectInfoXml(XElement expectedXml, XElement actualXml)
        {
            var expectedReportFileProperty = GetReportFilePath(expectedXml) ?? new XElement("dummy");
            var actualReportFileProperty = GetReportFilePath(actualXml) ?? new XElement("dummy");

            // order does not matter
            Assert.Equal(
                expectedReportFileProperty.Value.Split('|').ToHashSet(),
                actualReportFileProperty.Value.Split('|').ToHashSet());

            expectedReportFileProperty.Value = string.Empty;
            actualReportFileProperty.Value = string.Empty;

            Assert.True(XNode.DeepEquals(expectedXml, actualXml));
        }

        private XElement GetReportFilePath(XElement xml)
        {
            var ns = xml.GetDefaultNamespace();
            return 
                xml
                .Element(ns + "AnalysisSettings")
                ?.Elements()
                .FirstOrDefault(x => x.Attribute("Name")?.Value.Contains("roslyn.reportFilePath") ?? false);
        }
    }
}
