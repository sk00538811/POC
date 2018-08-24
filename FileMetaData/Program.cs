using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSOFile;
namespace FileMetaData
{
    class Program
    {
        static void Main(string[] args)
        {
            List<MetaData> metadata = GetMetaDataList();
            string filepath = @"D:\Metadata\Test\testfile\test.txt";
            // ApplyMetaData(filepath, metadata);
             metadata = ReadMetaData(filepath);
            
        }
        public static void ApplyMetaData(string filepath, List<MetaData> lstmetadata)
        {
            bool ispropfound = false;
            OleDocumentProperties myFile = new DSOFile.OleDocumentProperties();
            myFile.Open(filepath, false, DSOFile.dsoFileOpenOptions.dsoOptionDefault);
            foreach (var metadata in lstmetadata)
            {
                ispropfound = false;
                foreach (DSOFile.CustomProperty property in myFile.CustomProperties)
                {
                    if (property.Name.ToUpper() == metadata.Text.ToUpper())
                    { ispropfound = true; break; }

                }
                if (!ispropfound)//add property
                {
                    object objValue = metadata.Value;
                    myFile.CustomProperties.Add(metadata.Text, ref objValue);
                }
            }
            myFile.Save();
            myFile.Close(true);


        }
        public static List<MetaData> ReadMetaData(string filepath)
        {
            List<MetaData> lstmetadata = new List<MetaData>();

            OleDocumentProperties myFile = new DSOFile.OleDocumentProperties();
            myFile.Open(filepath, false, DSOFile.dsoFileOpenOptions.dsoOptionDefault);

            foreach (DSOFile.CustomProperty property in myFile.CustomProperties)
            {
                lstmetadata.Add(new MetaData { Text = property.Name, Value = Convert.ToString(property.get_Value()) });
            }


            myFile.Save();
            myFile.Close(true);

            return lstmetadata;
        }
        public static List<MetaData> GetMetaDataList()
        {
            List<MetaData> metadata = new List<MetaData>();
            metadata.Add(new MetaData { Text = "abc", Value = "123" });
            metadata.Add(new MetaData { Text = "abc1", Value = "1231" });
            metadata.Add(new MetaData { Text = "abc2", Value = "1232" });
            metadata.Add(new MetaData { Text = "abc3", Value = "1233" });
            metadata.Add(new MetaData { Text = "abc4", Value = "1234" });
            return metadata;
        }
        public class MetaData
        {
            public string Text { get; set; }
            public string Value { get; set; }
        }
    }
}
