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
            // ReSharper disable once UnusedMember.Global
            set => _issueField = value;
        }

        [XmlAttribute]
        public string Name
        {
            get => _nameField;
            // ReSharper disable once UnusedMember.Global
            set => _nameField = value;
        }
    }
}
