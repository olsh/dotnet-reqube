using JetBrains.Annotations;
using Moq;
using ReQube.Models;
using ReQube.Models.ReSharper;
using ReQube.Models.SonarQube;
using ReQube.Models.SonarQube.Roslyn;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Xunit;

namespace ReQube.Tests
{
    public class SonarConverterTest
    {
        private static readonly XmlSerializer ReSharperXmlSerializer = new XmlSerializer(typeof(Report));

        [Fact]
        public void SonarConverter_WithInvalidProjectPath_ShouldFail()
        {            
            var options = new Options
            {
                Input = "Resources/ReSharperTestReport.xml",
                Project = "ReQube.Test1"
            };

            var sonarConverter = GetSonarConverter(new TestReportGenerator(), options);

            var exception = Assert.Throws<FileNotFoundException>(() => sonarConverter.Convert());
            Assert.Equal("Project not found.", exception.Message);
            Assert.Equal("ReQube.Test1", exception.FileName);
        }

        [Fact]
        public void SonarConverter_WithValidProjectPathButNoIssues_ShouldBeSkipped()
        {            
            var options = new Options
            {
                Input = "Resources/ReSharperTestReport.xml",
                Project = "Resources/ReSharperTest/Test1/Test1.csproj"
            };

            var sonarConverter = GetSonarConverter(new TestReportGenerator(), options);

            var mockLogger = new Mock<ILogger>();
            sonarConverter.Logger = mockLogger.Object;

            sonarConverter.Convert();

            mockLogger.Verify(
                l => l.Information(
                    It.Is<string>(m => m == "Project Resources/ReSharperTest/Test1/Test1.csproj contains no issues.")), 
                Times.Once());
        }

        [Fact]
        public void SonarConverter_WithValidProjectPathAndOutputDir_ShouldBeWrittenCorrectly()
        {
            var reportFile = "tmp/report.json";

            if (File.Exists(reportFile))
            {
                File.Delete(reportFile);
            }

            var options = new Options
            {
                Input = "Resources/ReSharperTestReport.xml",
                Directory = "tmp",
                Output = "report.json",
                Project = "Resources/ReSharperTest/Test1/Test1.csproj"
            };

            var sonarConverter = GetSonarConverter(
                new TestReportGenerator { SonarReports = new List<ISonarReport> { new SonarRoslynReport() } },
                options);

            sonarConverter.Convert();

            Assert.True(File.Exists(reportFile));
        }

        [UsedImplicitly]
        public static IEnumerable<object[]> SonarConverterWithProjectPathData =>
            new List<object[]>
            {
                // project and full output path provided
                new object[] {
                    new[] { "tmp2/report.json" },
                    new Options
                    {
                        Input = "Resources/ReSharperTestReport.xml",
                        Directory = "tmp",
                        Output = Path.GetFullPath("tmp2/report.json"),
                        Project = "Resources/ReSharperTest/Test1/Test1.csproj"
                    }
                },
                // project and directory provided
                new object[] {
                    new[] { "tmp/report.json" },
                    new Options
                    {
                        Input = "Resources/ReSharperTestReport.xml",
                        Directory = "tmp",
                        Output = "report.json",
                        Project = "Resources/ReSharperTest/Test1/Test1.csproj"
                    }
                },

                // project provided but no directory; also <ProjectName> template
                new object[] {
                    new[] { "Resources/ReSharperTest/Test1/report.Test1.json" },
                    new Options
                    {
                        Input = "Resources/ReSharperTestReport.xml",
                        Output = "report.<ProjectName>.json",
                        Project = "Resources/ReSharperTest/Test1/Test1.csproj"
                    }
                },

                // solution analysis for generic format
                new object[] {
                    new[] {
                        "Resources/ReSharperTest/report.json",
                        "Resources/ReSharperTest/Test1/report.json",
                        "Resources/ReSharperTest/Test2/src/report.json"
                    },
                    new Options
                    {
                        Input = "Resources/ReSharperTestReport.xml",
                        Output = "report.json"
                    }
                },

                // solution analysis with dir
                new object[] {
                    new[] {
                        "tmp/report.json",
                        "tmp/Test1/report.json",
                        "tmp/Test2/src/report.json"
                    },
                    new Options
                    {
                        Directory = "tmp",
                        Input = "Resources/ReSharperTestReport.xml",
                        Output = "report.json"
                    }
                }
            };

        [Theory]
        [MemberData(nameof(SonarConverterWithProjectPathData))]
        public void SonarConverter_WithValidOptions_ShouldBeWrittenCorrectly(string[] reportFiles, Options options)
        {
            RunGenericSonarConverterTest(reportFiles, options);
        }

        [Fact]
        public void SonarConverter_WithAbsoluteOutPathForSln_ShouldFail()
        {
            var options = new Options
            {
                Input = "Resources/ReSharperTestReport.xml",
                Output = "/tmp/report.json"
            };

            var sonarConverter = GetSonarConverter(
                new TestReportGenerator
                {
                    SonarReports = new List<ISonarReport> { new SonarRoslynReport() { ProjectName = "Test1" } }
                },
                options);

            var exception = Assert.Throws<ArgumentException>(() => sonarConverter.Convert());
            Assert.Equal("Absolute paths are not allowed with -output, when converting a sln.", exception.Message);
        }

        [Fact]
        public void SonarConverter_WithAbsoluteProjectPathsInSln_ShouldFail()
        {
            var options = new Options
            {
                Input = "Resources/InvalidReSharperTestReport.xml",
                Output = "report.json"
            };

            var sonarConverter = GetSonarConverter(
                new TestReportGenerator
                {
                    SonarReports = new List<ISonarReport> { new SonarRoslynReport() { ProjectName = "Test1" } }
                },
                options);

            var exception = Assert.Throws<ArgumentException>(() => sonarConverter.Convert());
            Assert.Equal("Solution cannot contain absolute project paths.", exception.Message);
        }

        [Fact]
        public void SonarConverter_WithMissingReports_WriteEmptyFiles()
        {
            var reportFiles = new[] {
                "tmp/tmp2/report.json",
                "tmp/Test1/tmp2/report.json",
                "tmp/Test2/src/tmp2/report.json"
            };

            var options = new Options
            {
                Directory = "tmp",
                Input = "Resources/ReSharperTestReport.xml",
                Output = "tmp2/report.json"
            };

            RunGenericSonarConverterTest(reportFiles, options);

            Assert.False(IsEmptyGenericReport("tmp/Test1/tmp2/report.json"));
            Assert.True(IsEmptyGenericReport("tmp/Test2/src/tmp2/report.json"));
        }

        [UsedImplicitly]
        public static IEnumerable<object[]> ConvertToAbsolutePathsTestData =>
            new List<object[]>
            {
                // relative path of the solution
                new object[] { "test/example.sln", Path.GetFullPath("test") },
                new object[] { "example.sln", Path.GetFullPath(".") }
            };

        [Theory]
        [MemberData(nameof(ConvertToAbsolutePathsTestData))]
        public void ConvertToAbsolutePaths_ShouldConvertAllRelativePaths(string solutionFile, string solutionDir)
        {
            var report = new Report
            {
                Information = new ReportInformation
                {
                    Solution = solutionFile
                },
                Issues = new []
                {
                    new ReportProject
                    {
                        Issue = new []
                        {
                            new ReportProjectIssue
                            {
                                File = "path1"
                            },
                            new ReportProjectIssue
                            {
                                File = "path2"
                            }
                        }
                    },
                    new ReportProject
                    {
                        Issue = new []
                        {
                            new ReportProjectIssue
                            {
                                File = "path3"
                            }
                        }
                    }
                }
            };

            SonarConverter.ConvertToAbsolutePaths(report);

            Assert.Equal(Path.Combine(solutionDir, "path1"), report.Issues[0].Issue[0].File);
            Assert.Equal(Path.Combine(solutionDir, "path2"), report.Issues[0].Issue[1].File);
            Assert.Equal(Path.Combine(solutionDir, "path3"), report.Issues[1].Issue[0].File);
        }
        
        [Fact]
        public void RemoveExcludedRules_ShouldFilterRulesCorrectly()
        {
            var reportXml = @"
                <Report>  
                    <Issues>   
                        <Project Name=""Test1"">    
                            <Issue TypeId=""UnusedParameter.Global"" 
                                   Message=""Parameter 'countrygeoid' is used neither in this nor in overriding methods"" />     
                            <Issue TypeId=""UnusedParameter.Global"" 
                                   Message=""Parameter 'geocodeResolution' is never used""/>      
                            <Issue TypeId=""CheckNamespace"" />       
                            <Issue TypeId=""UnusedVariable"" />        
                        </Project>        
                        <Project Name=""Test2"">         
                           <Issue TypeId=""RedundantUsingDirective""
                                  Message=""Using directive is not required by the code and can be safely removed"" />          
                           <Issue TypeId=""UnusedType.Global"" />           
                        </Project>           
                    </Issues>
                </Report>";

            var reSharperReport = ParseReSharperReport(reportXml);

            // test exclude filters by type only
            SonarConverter.RemoveExcludedRules(
                reSharperReport, "RedundantUsingDirective|UnusedVariable|UnusedParameter.Global");

            AssertEqualIssueTypes(reSharperReport, new[] { "CheckNamespace" }, new[] { "UnusedType.Global" });

            // test message filtering
            reSharperReport = ParseReSharperReport(reportXml);            
            SonarConverter.RemoveExcludedRules(
                reSharperReport,
                "RedundantUsingDirective|UnusedParameter.Global##Parameter 'geocodeResolution'.*|UnusedVariable");

            AssertEqualIssueTypes(
                reSharperReport, new[] { "UnusedParameter.Global", "CheckNamespace" }, new[] { "UnusedType.Global" });
            Assert.Equal(
                "Parameter 'countrygeoid' is used neither in this nor in overriding methods",
                reSharperReport.Issues[0].Issue[0].Message);

            // test message filtering with partial match
            reSharperReport = ParseReSharperReport(reportXml);
            SonarConverter.RemoveExcludedRules(
                reSharperReport,
                "RedundantUsingDirective##directive|UnusedParameter.Global|UnusedVariable");

            AssertEqualIssueTypes(
                reSharperReport,
                new[] { "CheckNamespace" }, 
                new[] { "UnusedType.Global" });

            // test message filtering with full match
            reSharperReport = ParseReSharperReport(reportXml);
            SonarConverter.RemoveExcludedRules(
                reSharperReport,
                "RedundantUsingDirective##^directive$|UnusedParameter.Global|UnusedVariable");

            AssertEqualIssueTypes(
                reSharperReport,
                new[] { "CheckNamespace" },
                new[] { "RedundantUsingDirective", "UnusedType.Global" });

            // test message filtering overridden by type filtering
            reSharperReport = ParseReSharperReport(reportXml);
            SonarConverter.RemoveExcludedRules(
                reSharperReport,
                "RedundantUsingDirective##^directive$|RedundantUsingDirective|UnusedParameter.Global|UnusedVariable");

            AssertEqualIssueTypes(
                reSharperReport,
                new[] { "CheckNamespace" },
                new[] { "UnusedType.Global" });
        }

        [Fact]
        public void SonarConverter_ForRoslynOutput_ShouldCallAddReSharperAnalysisPathsWithCorrectArguments()
        {
            var options = new Options
            {
                Input = "Resources/ReSharperTestReport.xml",
                Output = "report.json",
                OutputFormat = SonarOutputFormat.Roslyn
            };

            var sonarConverter = GetSonarConverter(
                new TestReportGenerator { 
                    SonarReports = new List<ISonarReport> { 
                        new SonarRoslynReport
                        {
                            ProjectName = "Test1"
                        },
                        new SonarRoslynReport
                        {
                            ProjectName = "Test2"
                        }
                    } 
                },
                options);

            var metaDataWriterMock = new Mock<ISonarMetaDataWriter>();

            var metaDataWriterFactoryMock = new Mock<SonarMetaDataWriterFactory>();
            metaDataWriterFactoryMock.Setup(
                x => x.GetMetaDataWriter(It.IsAny<Options>())).Returns(metaDataWriterMock.Object);

            sonarConverter.SonarMetaDataWriterFactory = metaDataWriterFactoryMock.Object;

            sonarConverter.Convert();

            metaDataWriterMock.Verify(
                x => x.AddReSharperAnalysisPaths(
                    It.Is<IDictionary<string, string>>(
                        map => 
                            map["Test1"] == Path.GetFullPath("Resources/ReSharperTest/Test1/report.json") && 
                            map["Test2"] == Path.GetFullPath("Resources/ReSharperTest/Test2/src/report.json") &&
                            map.Count == 2
                        )), Times.Once);
        }
        
        private void RunGenericSonarConverterTest(string[] reportFiles, Options options)
        {
            foreach (var reportFile in reportFiles)
            {
                if (File.Exists(reportFile))
                {
                    File.Delete(reportFile);
                }
            }

            var sonarConverter = GetSonarConverter(
                new TestReportGenerator
                {
                    SonarReports = new List<ISonarReport> { new SonarRoslynReport() { ProjectName = "Test1" } }
                },
                options);

            sonarConverter.Convert();

            foreach (var reportFile in reportFiles)
            {
                Assert.True(File.Exists(reportFile), $"Report file {reportFile} is not written.");
            }
        }

        private SonarConverter GetSonarConverter(ISonarReportGenerator sonarReportGenerator, Options options)
        {
            var sonarConverter = new SonarConverter(options);
            var mockSonarReportGeneratorFactory = new Mock<SonarReportGeneratorFactory>();
            mockSonarReportGeneratorFactory.Setup(
                f => f.GetGenerator(It.IsAny<SonarOutputFormat>())).Returns(sonarReportGenerator);
            sonarConverter.SonarReportGeneratorFactory = mockSonarReportGeneratorFactory.Object;

            return sonarConverter;
        }

        private bool IsEmptyGenericReport(string file)
        {
            return File.ReadAllText(file) == "{\"issues\":[]}";
        }

        private Report ParseReSharperReport(string reportXml)
        {
            using var reader = new StringReader(reportXml);
            return (Report)ReSharperXmlSerializer.Deserialize(reader);
        }

        [AssertionMethod]
        private void AssertEqualIssueTypes(
            Report reSharperReport, string[] expectedTypesForTest1, string[] expectedTypesForTest2)
        {
            Assert.Equal(
                expectedTypesForTest1,
                reSharperReport.Issues[0].Issue.Select(x => x.TypeId).ToArray());

            Assert.Equal(
                expectedTypesForTest2,
                reSharperReport.Issues[1].Issue.Select(x => x.TypeId).ToArray());
        }
    }

    public class TestReportGenerator : ISonarReportGenerator
    {
        public List<ISonarReport> SonarReports { get; set; } = new List<ISonarReport>();

        public List<ISonarReport> Generate(Report reSharperReport)
        {
            return SonarReports;
        }
    }
}
