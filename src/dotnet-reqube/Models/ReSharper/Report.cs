using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace ReQube.Models.ReSharper
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class Report
    {
        public ReportInformation Information { get; set; }

        [XmlArrayItem("Project", IsNullable = false)]
        public ReportProject[] Issues { get; set; }

        [XmlArrayItem("IssueType", IsNullable = false)]
        public ReportIssueType[] IssueTypes { get; set; }
    }
}
