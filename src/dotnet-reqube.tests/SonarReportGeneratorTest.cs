using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReQube.Models;
using ReQube.Models.ReSharper;
using ReQube.Utils;
using System;
using System.IO;
using System.Xml.Serialization;
using Xunit;
using Xunit.Abstractions;

namespace ReQube.Tests
{
    public class SonarReportGeneratorTest
    {
        private static readonly XmlSerializer ReSharperXmlSerializer = new XmlSerializer(typeof(Report));
        private static readonly JsonSerializerSettings JsonSerializerSettings
            = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        private readonly ITestOutputHelper _output;

        public SonarReportGeneratorTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(SonarOutputFormat.Roslyn, "Resources/ExpectedOutputs/SonarRoslynReport.json")]
        [InlineData(SonarOutputFormat.Generic, "Resources/ExpectedOutputs/SonarGenericReport.json")]
        public void SonarReportGenerator_WithValidInput_ShouldProduceCorrectReport(
            SonarOutputFormat outputFormat, 
            string expectedResultFile)
        {
            Report reSharperReport;

            using (var reader = new StreamReader("Resources/ReSharperTestReport.xml"))
            {
                reSharperReport = (Report)ReSharperXmlSerializer.Deserialize(reader);
            }

            SonarConverter.ConvertToAbsolutePaths(reSharperReport);

            var reportGeneratorFactory = new SonarReportGeneratorFactory();
            var reportGenerator = reportGeneratorFactory.GetGenerator(outputFormat);

            // JArray.FromObject is not used, because serializer settings cannot be provided
            var actualSonarReports = JArray.Parse(
                JsonConvert.SerializeObject(reportGenerator.Generate(reSharperReport), JsonSerializerSettings));

            FixReportPaths(actualSonarReports, outputFormat);

            var convertSourceRootFunc = outputFormat == SonarOutputFormat.Roslyn 
                ? (Func<string, string>) FileUtils.FilePathToFileUrl : x => x.Replace("\\", "/");

            var sourceRoot = Directory.GetCurrentDirectory();
            var expectedSonarReportsAsString =
                File.ReadAllText(expectedResultFile)
                .Replace("{sourceRoot}", convertSourceRootFunc(sourceRoot));

            var expectedSonarReports = JArray.Parse(expectedSonarReportsAsString);

            try
            {
                Assert.True(JToken.DeepEquals(expectedSonarReports, actualSonarReports));
            }
            catch
            {
                _output.WriteLine($"Expected sonar reports:\n{expectedSonarReports}, actual: {actualSonarReports}.");
                throw;
            }
        }

        private static void FixReportPaths(JArray sonarReports, SonarOutputFormat outputFormat)
        {
            if (outputFormat == SonarOutputFormat.Roslyn)
            {
                return;
            }

            var primaryLocations = sonarReports.SelectTokens("[*].issues[*].primaryLocation");

            foreach (JToken primaryLocation in primaryLocations)
            {
                primaryLocation["filePath"] = primaryLocation.Value<string>("filePath").Replace("\\", "/");
            }
        }
    }
}
