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
        private string _idField;

        private string _severityField;

        [XmlAttribute]
        public string Id
        {
            get => _idField;
            // ReSharper disable once UnusedMember.Global
            set => _idField = value;
        }

        [XmlAttribute]
        public string Severity
        {
            get => _severityField;
            // ReSharper disable once UnusedMember.Global
            set => _severityField = value;
        }
    }
}
