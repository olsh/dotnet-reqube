using ReQube.Utils;
using System.IO;
using Xunit;

namespace ReQube.Tests.Utils
{
    public class FileUtilsTest
    {
        [Fact]
        public void FilePathToFileUrl_WithNonUNCWinPath_ConvertsToValidUrl()
        {
            var url = FileUtils.FilePathToFileUrl("C:\\test\\fileA7+/:.-_~ &.xml");
            Assert.Equal("file:///C:/test/fileA7+/:.-_~%20%26.xml", url);
        }

        [Fact]
        public void FilePathToFileUrl_WithUNCWinPath_ConvertsToValidUrl()
        {
            var url = FileUtils.FilePathToFileUrl("\\test\\fileA7+/:.-_~ &.xml");
            Assert.Equal("file:////test/fileA7+/:.-_~%20%26.xml", url);
        }

        [Fact]
        public void FilePathToFileUrl_WithNonRootPath_ConvertsToValidUrl()
        {            
            var urlFromRelativePath = FileUtils.FilePathToFileUrl("tmp/test.xml");
            var urlFromAbsolutePath = FileUtils.FilePathToFileUrl(Path.GetFullPath("tmp/test.xml"));
            Assert.Equal(urlFromAbsolutePath, urlFromRelativePath);
        }

        [Fact]
        public void FilePathToFileUrl_WithRootUnixPath_ConvertsToValidUrl()
        {
            var url = FileUtils.FilePathToFileUrl("/tmp/test.xml");
            Assert.Equal("file:////tmp/test.xml", url);
        }

        [Fact]
        public void FindLineOffset_WithCorrectGlobalOffset_ShouldConvertToLocalOffset()
        {
            var content =
                "namespace Test\r\n" +
                "{\n" +
                "  public class Program\r\n"  +
                "  {\n" +
                "    public static void Main(string[] args)\n" +
                "    {\n" +
                "      System.out.println(\"TEST\");\n" +
                "    }\n" +
                "  }\n" +
                "}";

            var (startColumn, endColumn) = FileUtils.FindLineOffset(content, 104, 114);
            Assert.Equal(9, startColumn);
            Assert.Equal(19, endColumn);
        }
    }
}
