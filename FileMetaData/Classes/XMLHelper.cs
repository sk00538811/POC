using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace FileMetaData.Classes
{
   public class XMLHelper
    {
          /// <summary>
        /// Description: convert xml string to spaecific object
        ///  XMLHelper hlpr = new XMLHelper();
        ///  string path = string.Empty;
        ///  string xmlInputData = string.Empty;
        ///
        ///  // EXAMPLE 1
        ///  path = Directory.GetCurrentDirectory() + @"\Customer.xml";
        ///  xmlInputData = File.ReadAllText(path);
        ///  Customer customer = hlpr.XMLDeserialize<Customer>(xmlInputData);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
      public T XMLDeserialize<T>(string input) where T : class
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));

            using (StringReader sr = new StringReader(input))
            {
                return (T)ser.Deserialize(sr);
            }
        }
        /// <summary>
        /// Description: Convert object to xml string
        ///  XMLHelper hlpr = new XMLHelper();
        ///  string xmlOutputData = string.Empty;
        ///
        ///  // EXAMPLE 1
        ///  Customer customer = new customer{ Name="Surendra",Age=37};
        ///  xmlOutputData = hlpr.Serialize<Customer>(customer)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ObjectToSerialize"></param>
        /// <returns></returns>
        public string SerializeObjectToXML<T>(T ObjectToSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(ObjectToSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, ObjectToSerialize);
                return textWriter.ToString();
            }
        }
    }
}
