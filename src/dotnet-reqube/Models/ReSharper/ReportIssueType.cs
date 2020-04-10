using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace ReQube.Models.ReSharper
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class ReportIssueType
    {
        [XmlAttribute]
        public string Id { get; set; }

        [XmlAttribute]
        public string Severity { get; set; }

        [XmlAttribute]
        public string Description { get; set; }

        [XmlAttribute]
        public string WikiUrl { get; set; }
    }
}
