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
        private ReportProjectIssue[] _issueField;

        private string _nameField;

        [XmlElement("Issue")]
        public ReportProjectIssue[] Issue
        {
            get => _issueField;
            set => _issueField = value;
        }

        [XmlAttribute]
        public string Name
        {
            get => _nameField;
            set => _nameField = value;
        }
    }
}
