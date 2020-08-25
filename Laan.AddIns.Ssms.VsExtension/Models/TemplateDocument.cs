using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml.Serialization;

using Laan.AddIns.Ssms.VsExtension.Commands;
using Laan.AddIns.Ssms.VsExtension.Utils;

namespace Laan.AddIns.Ssms.VsExtension.Models
{
    [XmlRoot("templateDocument")]
    public class TemplateDocument
    {
        private static string _templatePath;

        private static string LoadDefaultResourceFile(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        static TemplateDocument()
        {
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                path = Path.Combine(path, @"Laan Sql Tools\\Templates");
                _templatePath = Path.Combine(path, "config.xml");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                if (!File.Exists(_templatePath))
                {
                    var xml = LoadDefaultResourceFile(@"Laan.AddIns.Ssms.VsExtension.Templates.default.xml");
                    File.WriteAllText(_templatePath, xml);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Laan SSMS AddIn", ex.ToString(), EventLogEntryType.Error);
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            }
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
                var doc = File.ReadAllText(_templatePath).FromXml<TemplateDocument>();

                return doc?.Templates ?? new List<Template>();
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

                SqlInsertTemplateAction.Templates = templates;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Laan SSMS AddIn", ex.ToString(), EventLogEntryType.Error);
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        [XmlArray("templates"), XmlArrayItem("template")]
        public List<Template> Templates { get; set; }
    }
}
