using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace ReQube.Models.ReSharper
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class ReportInformationInspectionScope
    {
        private string _elementField;

        public string Element
        {
            get => _elementField;
            set => _elementField = value;
        }
    }
}
