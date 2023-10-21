using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Serialization;

namespace Laan.AddIns.Ssms.VsExtension.Models
{
    public class Template : INotifyPropertyChanged
    {
        private string _body;
        private string _code;
        private string _name;
        private string _description;

        [XmlElement("name")]
        public string Name
        {
            get => _name;
            set { Notify(ref _name, value); }
        }

        [XmlElement("code")]
        public string Code
        {
            get => _code;
            set { Notify(ref _code, value); }
        }

        [XmlElement("description")]
        public string Description
        {
            get => _description;
            set { Notify(ref _description, value); }
        }

        [XmlElement("body")]
        public XmlCDataSection CDataBody
        {
            get => new XmlDocument().CreateCDataSection(Body);
            set => Body = value.Value.TrimStart();
        }

        [XmlIgnore]
        public string Body
        {
            get => _body;
            set => Notify(ref _body, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify<T>(ref T field, T value, [CallerMemberName]string propertyName = "")
        {
            if (field != null && field.Equals(value))
                return;

            field = value;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Template Clone()
        {
            var clone = (Template)MemberwiseClone();
            clone.Name += " (Copy)";
            return clone;
        }
    }
}
