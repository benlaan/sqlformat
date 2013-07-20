using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Laan.AddIns.Ssms.Actions
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
            get { return _name; }
            set
            {
                if (_name == value)
                    return;
                
                _name = value;

                Notify("Name");
            }
        }

        [XmlElement("code")]
        public string Code
        {
            get { return _code; }
            set
            {
                if (_code == value)
                    return;

                _code = value;

                Notify("Code");
            }
        }

        [XmlElement("description")]
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description == value)
                    return;

                _description = value;
                
                Notify("Description");
            }
        }

        [XmlElement("body")]
        public XmlCDataSection CDataBody
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                return doc.CreateCDataSection(Body);
            }
            set
            {
                Body = value.Value.TrimStart();
            }
        }

        [XmlIgnore]
        public string Body
        {
            get
            {
                return _body;
            }
            set
            {
                if (_body == value)
                    return;

                _body = value;

                Notify("Body");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
