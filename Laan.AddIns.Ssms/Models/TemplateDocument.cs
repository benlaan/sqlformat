using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace Laan.AddIns.Ssms.Actions
{
    [XmlRoot("templateDocument")]
    public class TemplateDocument
    {
        private static string _templatePath;

        static TemplateDocument()
        {
            _templatePath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 
                @"Templates\default.xml"
            );
        }

        /// <summary>
        /// Initializes a new instance of the TemplateDocument class.
        /// </summary>
        public TemplateDocument()
        {
            Templates = new List<Template>();
        }

        public static List<Template> Load()
        {
            try
            {
                var doc = System.IO.File
                    .ReadAllText(_templatePath)
                    .FromXml<TemplateDocument>();

                return doc != null ? doc.Templates : new List<Template>();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
                return new List<Template>();
            }
        }

        public static void Save(List<Template> templates)
        {
            try
            {
                TemplateDocument doc = new TemplateDocument();
                doc.Templates.AddRange(templates);
                File.WriteAllText(_templatePath, doc.ToXml());

                SqlTemplating.Templates = templates;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        [XmlArray("templates"), XmlArrayItem("template")]
        public List<Template> Templates { get; set; }
    }
}
