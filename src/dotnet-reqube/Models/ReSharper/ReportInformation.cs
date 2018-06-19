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
        private ReportInformationInspectionScope _inspectionScopeField;

        private string _solutionField;

        public ReportInformationInspectionScope InspectionScope
        {
            get => _inspectionScopeField;
            set => _inspectionScopeField = value;
        }

        public string Solution
        {
            get => _solutionField;
            set => _solutionField = value;
        }
    }
}
