using Moq;
using Moq.Protected;
using ReQube.Models;
using System;
using Xunit;

namespace ReQube.Tests
{
    public class ReQubeProgramTest
    {
        [Fact]
        public void Program_WithInvalidOptions_ExitsWithStatus1()
        {
            var sonarConverterMock = new Mock<ISonarConverter>();
            var programMock = new Mock<Program> { CallBase = true };
            programMock.Protected().Setup<ISonarConverter>(
                "GetSonarConverter", ItExpr.IsAny<Options>()).Returns(sonarConverterMock.Object);
            programMock.Protected().Setup("Exit", ItExpr.Is<int>(c => c == 1));
            programMock.Object.Run(new[] { "-i r#" });
            programMock.Protected().Verify("Exit", Times.Once(), ItExpr.Is<int>(c => c == 1));
            sonarConverterMock.Verify(c => c.Convert(), Times.Never());
        }

        [Fact]
        public void Program_ThrowingException_PropagatesTheException()
        {
            var sonarConverterMock = new Mock<ISonarConverter>();
            var programMock = new Mock<Program> { CallBase = true };
            programMock.Protected().Setup<ISonarConverter>(
                "GetSonarConverter", ItExpr.IsAny<Options>()).Returns(sonarConverterMock.Object);
            programMock.Protected().Setup("Exit", ItExpr.IsAny<int>());
            sonarConverterMock.Setup(c => c.Convert()).Throws<Exception>();            
            Assert.Throws<Exception>(() => programMock.Object.Run(new[] { "-i", "input", "-o", "output" }));
            programMock.Protected().Verify("Exit", Times.Never(), ItExpr.IsAny<int>());
        }

        [Fact]
        public void Program_WithHelpOption_ExitsWithStatus0()
        {
            var sonarConverterMock = new Mock<ISonarConverter>();
            var programMock = new Mock<Program> { CallBase = true };
            programMock.Protected().Setup<ISonarConverter>(
                "GetSonarConverter", ItExpr.IsAny<Options>()).Returns(sonarConverterMock.Object);
            programMock.Protected().Setup("Exit", ItExpr.IsAny<int>());
            programMock.Object.Run(new[] { "--help" });
            programMock.Protected().Verify("Exit", Times.Never(), ItExpr.IsAny<int>());
            sonarConverterMock.Verify(c => c.Convert(), Times.Never());
        }

        [Fact]
        public void Program_WithValidOptions_InvokesConvert()
        {
            var sonarConverterMock = new Mock<ISonarConverter>();
            var programMock = new Mock<Program> { CallBase = true };

            programMock.Protected().Setup<ISonarConverter>(
                "GetSonarConverter", ItExpr.IsAny<Options>()).Returns(sonarConverterMock.Object);
            programMock.Protected().Setup("Exit", ItExpr.IsAny<int>());
            programMock.Object.Run(new[] {
                "-i", 
                "input.xml", 
                "-o", 
                "sonar.json", 
                "-d", 
                "./repo/test-sln", 
                "-p", 
                "./repo/test-sln/proj1.csproj", 
                "-f", "Roslyn",
                "--exclude-rules", "rule1, rule2"});
            programMock.Protected().Verify("Exit", Times.Never(), ItExpr.IsAny<int>());
            programMock.Protected().Verify(
                "GetSonarConverter",
                Times.Once(),
                ItExpr.Is<Options>(o =>
                    o.Input == "input.xml"
                    && o.Output == "sonar.json"
                    && o.Directory == "./repo/test-sln"
                    && o.Project == "./repo/test-sln/proj1.csproj"
                    && o.OutputFormat == SonarOutputFormat.Roslyn
                    && o.ExcludedRules == "rule1, rule2"));
        }
    }
}
