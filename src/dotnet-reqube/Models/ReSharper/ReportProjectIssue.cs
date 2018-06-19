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
        private string _fileField;

        private ushort _lineField;

        private bool _lineFieldSpecified;

        private string _messageField;

        private string _offsetField;

        private string _typeIdField;

        [XmlAttribute]
        public string File
        {
            get => _fileField;
            set => _fileField = value;
        }

        [XmlAttribute]
        public ushort Line
        {
            get => _lineField;
            set => _lineField = value;
        }

        [XmlIgnore]
        public bool LineSpecified
        {
            get => _lineFieldSpecified;
            set => _lineFieldSpecified = value;
        }

        [XmlAttribute]
        public string Message
        {
            get => _messageField;
            set => _messageField = value;
        }

        [XmlAttribute]
        public string Offset
        {
            get => _offsetField;
            set => _offsetField = value;
        }

        [XmlAttribute]
        public string TypeId
        {
            get => _typeIdField;
            set => _typeIdField = value;
        }
    }
}
