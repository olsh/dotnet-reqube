using System.Xml;
using System.Xml.Linq;

namespace ReQube.Utils
{
    public static class XmlUtils
    {
        public static XElement RequiredElement(this XElement xElement, XName name)
        {
            return xElement.Element(name) 
                ?? throw new XmlException($"Invalid xml. Required element {name} is not found.");
        }
    }
}
