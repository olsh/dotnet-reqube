using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace ReQube.Models.ReSharper
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class ReportProject
    {
        [XmlElement("Issue")]
        public ReportProjectIssue[] Issue { get; set; }

        [XmlAttribute]
        public string Name { get; set; }
    }
}
