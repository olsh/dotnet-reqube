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

        private string _messageField;

        private string _typeIdField;

        [XmlAttribute]
        public string File
        {
            get => _fileField;
            // ReSharper disable once UnusedMember.Global
            set => _fileField = value;
        }

        [XmlAttribute]
        public ushort Line
        {
            get => _lineField;
            // ReSharper disable once UnusedMember.Global
            set => _lineField = value;
        }

        [XmlAttribute]
        public string Message
        {
            get => _messageField;
            // ReSharper disable once UnusedMember.Global
            set => _messageField = value;
        }

        [XmlAttribute]
        public string TypeId
        {
            get => _typeIdField;
            // ReSharper disable once UnusedMember.Global
            set => _typeIdField = value;
        }
    }
}
