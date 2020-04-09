using ReQube.Models;
using Xunit;

namespace ReQube.Tests
{
    public class SonarMetaDataFactoryTest
    {
        [Fact]
        public void GetMetaDataWriter_WithRoslynOutput_ShouldReturnCorrectTypeOfMetaDataWriter()
        {
            var metaDataWriter =
                new SonarMetaDataWriterFactory().GetMetaDataWriter(
                    new Options
                    {
                        SonarDirectory = ".sonarqube",
                        OutputFormat = SonarOutputFormat.Roslyn
                    });

            Assert.IsType<SonarRoslynMetaDataWriter>(metaDataWriter);
        }

        [Fact]
        public void GetMetaDataWriter_WithGenericOutput_ShouldReturnNull()
        {
            var metaDataWriter =
                new SonarMetaDataWriterFactory().GetMetaDataWriter(
                    new Options
                    {
                        SonarDirectory = ".sonarqube",
                        OutputFormat = SonarOutputFormat.Generic
                    });

            Assert.Null(metaDataWriter);
        }

        [Fact]
        public void GetMetaDataWriter_WithoutSonarDir_ShouldReturnNull()
        {
            var metaDataWriter =
                new SonarMetaDataWriterFactory().GetMetaDataWriter(
                    new Options
                    {
                        OutputFormat = SonarOutputFormat.Roslyn
                    });

            Assert.Null(metaDataWriter);
        }
    }
}
