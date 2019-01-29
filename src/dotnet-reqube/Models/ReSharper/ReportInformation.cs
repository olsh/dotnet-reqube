using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace ReQube.Models.ReSharper
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class ReportInformation
    {
        private string _solutionField;

        public string Solution
        {
            get => _solutionField;
            // ReSharper disable once UnusedMember.Global
            set => _solutionField = value;
        }
    }
}
