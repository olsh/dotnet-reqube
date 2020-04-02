using ReQube.Utils;
using System.Xml;
using System.Xml.Linq;
using Xunit;

namespace ReQube.Tests.Utils
{
    public class XmlUtilsTest
    {
        [Fact]
        public void RequiredElement_ForValidName_ShouldReturnCorrectXElement()
        {
            var root = new XElement("root", new XElement("child", "test"));
            Assert.Equal("test", root.RequiredElement("child").Value);
        }

        [Fact]
        public void RequiredElement_ForInvalidName_ShouldThrowXmlException()
        {
            var root = new XElement("root", "test");
            var ex = Assert.Throws<XmlException>(() => root.RequiredElement("child").Value);
            Assert.Equal("Invalid xml. Required element child is not found.", ex.Message);
        }
    }
}
