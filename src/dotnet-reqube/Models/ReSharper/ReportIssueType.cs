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
        private string _categoryField;

        private string _categoryIdField;

        private string _descriptionField;

        private string _globalField;

        private string _idField;

        private string _severityField;

        private string _subCategoryField;

        private string _wikiUrlField;

        [XmlAttribute]
        public string Category
        {
            get => _categoryField;
            set => _categoryField = value;
        }

        [XmlAttribute]
        public string CategoryId
        {
            get => _categoryIdField;
            set => _categoryIdField = value;
        }

        [XmlAttribute]
        public string Description
        {
            get => _descriptionField;
            set => _descriptionField = value;
        }

        [XmlAttribute]
        public string Global
        {
            get => _globalField;
            set => _globalField = value;
        }

        [XmlAttribute]
        public string Id
        {
            get => _idField;
            set => _idField = value;
        }

        [XmlAttribute]
        public string Severity
        {
            get => _severityField;
            set => _severityField = value;
        }

        [XmlAttribute]
        public string SubCategory
        {
            get => _subCategoryField;
            set => _subCategoryField = value;
        }

        [XmlAttribute]
        public string WikiUrl
        {
            get => _wikiUrlField;
            set => _wikiUrlField = value;
        }
    }
}
