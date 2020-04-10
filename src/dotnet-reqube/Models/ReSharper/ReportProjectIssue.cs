using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace ReQube.Models.ReSharper
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class ReportProjectIssue
    {
        [XmlAttribute]
        public string File { get; set; }

        [XmlAttribute]
        public ushort Line { get; set; }

        [XmlAttribute]
        public string Message { get; set; }

        [XmlAttribute]
        public string TypeId { get; set; }

        [XmlAttribute]
        public string Offset { get; set; }
    }
}
