using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Laan.AddIns.Ssms.VsExtension.Utils
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Return XML representation of object, encoding with <see cref="Encoding.UTF8"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="omitXmlDeclaration"></param>
        /// <returns></returns>
        public static string ToXml(this object entity, bool omitXmlDeclaration = false)
        {
            return ToXml(entity, Encoding.UTF8, omitXmlDeclaration);
        }

        /// <summary>
        /// Return XML representation of object
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="encoding">Text encoding to use</param>
        /// <param name="omitXmlDeclaration">true if XML declaration should be ommitted</param>
        /// <returns></returns>
        public static string ToXml(this object entity, Encoding encoding, bool omitXmlDeclaration)
        {
            var writerSettings = new XmlWriterSettings
            {
                Encoding = encoding,
                OmitXmlDeclaration = omitXmlDeclaration,
                Indent = true,
            };

            using (var stream = new MemoryStream())
            {
                using (var xmlWriter = XmlWriter.Create(stream, writerSettings))
                {
                    var serializer = new XmlSerializer(entity.GetType());
                    var namespaces = new XmlSerializerNamespaces();
                    namespaces.Add("", "");
                    serializer.Serialize(xmlWriter, entity, namespaces);

                    xmlWriter.Flush();
                }

                stream.Position = 0;

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
        public static T FromXml<T>(this string xmlString) where T : class
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                object deserializedObject;
                using (var reader = new StringReader(xmlString))
                {
                    deserializedObject = serializer.Deserialize(reader);
                }
                return (T)Convert.ChangeType(deserializedObject, typeof(T));
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return null;
            }
        }

        public static T FromXml<T>(this byte[] data, Encoding encoding) where T : class
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                object deserializedObject;

                using (var stream = new MemoryStream(data))
                {
                    deserializedObject = serializer.Deserialize(stream);
                }
                return (T)Convert.ChangeType(deserializedObject, typeof(T));
            }
            catch
            {
                return null;
            }
        }
    }
}
