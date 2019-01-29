using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace ReQube.Models.ReSharper
{
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class Report
    {
        private ReportInformation _informationField;

        private ReportProject[] _issuesField;

        private ReportIssueType[] _issueTypesField;

        public ReportInformation Information
        {
            get => _informationField;
            // ReSharper disable once UnusedMember.Global
            set => _informationField = value;
        }

        [XmlArrayItem("Project", IsNullable = false)]
        public ReportProject[] Issues
        {
            get => _issuesField;
            // ReSharper disable once UnusedMember.Global
            set => _issuesField = value;
        }

        [XmlArrayItem("IssueType", IsNullable = false)]
        public ReportIssueType[] IssueTypes
        {
            get => _issueTypesField;
            set => _issueTypesField = value;
        }
    }
}
