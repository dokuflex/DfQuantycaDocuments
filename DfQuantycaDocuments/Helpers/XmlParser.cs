using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace DfQuantycaDocuments.Helpers
{
    public static class XmlParser
    {
        /// <summary>
        /// Convierte un XML a un tipo de objeto pasado
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns>Objeto construido</returns>
        public static T toObject<T>(string xml) where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            // xml = xml.Replace("encoding=\"utf-16\"", ""); // Vamos a quitarle el utf-16 a ver
            MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            return (T)serializer.Deserialize(memStream);
        }

        /// <summary>
        /// Obtiene el XML correspondiente de un objeto.
        /// </summary>
        /// <param name="obj">Objeto a convertir en string de xml</param>
        /// <returns>string</returns>
        public static string toXml(object obj, string ns = "")
        {
            XmlWriterSettings xmlWriterSettings = new System.Xml.XmlWriterSettings() { Encoding = UTF8Encoding.UTF8 };

            string xml;
            XmlSerializer xsSubmit;
            if (ns == "") xsSubmit = new XmlSerializer(obj.GetType());
            else xsSubmit = new XmlSerializer(obj.GetType(), ns);

            var nss = new XmlSerializerNamespaces();
            nss.Add("", "");

            using (StringWriter sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww, new System.Xml.XmlWriterSettings() { Encoding = Encoding.UTF8 }))
                {
                    xsSubmit.Serialize(writer, obj, nss);
                    xml = sww.ToString(); // Your XML
                }
            }

            xml = xml.Replace("encoding=\"utf-16\"", ""); // Vamos a quitarle el utf-16 a ver

            return xml;
        }
    }
}