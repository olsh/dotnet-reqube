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
using Xunit;

namespace ReQube.Tests
{
    public class SonarConverterTest
    {
        [Fact]
        public void SonarConverter_WithInvalidProjectPath_ShouldFail()
        {
            var sonarConverter = GetSonarConverter(new TestReportGenerator());

            var options = new Options
            {
                Input = "Resources/ReSharperTestReport.xml",
                Project = "ReQube.Test1"
            };

            var exception = Assert.Throws<FileNotFoundException>(() => sonarConverter.Convert(options));
            Assert.Equal("Project not found.", exception.Message);
            Assert.Equal("ReQube.Test1", exception.FileName);
        }

        [Fact]
        public void SonarConverter_WithValidProjectPathButNoIssues_ShouldBeSkipped()
        {
            var sonarConverter = GetSonarConverter(new TestReportGenerator());

            var options = new Options
            {
                Input = "Resources/ReSharperTestReport.xml",
                Project = "Resources/ReSharperTest/Test1/Test1.csproj"
            };

            var mockLogger = new Mock<ILogger>();
            sonarConverter.Logger = mockLogger.Object;

            sonarConverter.Convert(options);

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

            var sonarConverter = GetSonarConverter(
                new TestReportGenerator { SonarReports = new List<ISonarReport> { new SonarRoslynReport() } });

            var options = new Options
            {
                Input = "Resources/ReSharperTestReport.xml",
                Directory = "tmp",
                Output = "report.json",
                Project = "Resources/ReSharperTest/Test1/Test1.csproj"
            };

            sonarConverter.Convert(options);

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
                });

            var exception = Assert.Throws<ArgumentException>(() => sonarConverter.Convert(options));
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
                });

            var exception = Assert.Throws<ArgumentException>(() => sonarConverter.Convert(options));
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
                });

            sonarConverter.Convert(options);

            foreach (var reportFile in reportFiles)
            {
                Assert.True(File.Exists(reportFile), $"Report file {reportFile} is not written.");
            }
        }

        private SonarConverter GetSonarConverter(ISonarReportGenerator sonarReportGenerator)
        {
            var sonarConverter = new SonarConverter();
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
