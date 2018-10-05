using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSOFile;
using System.IO;
using System.Net;
using FileMetaData.Classes;

namespace FileMetaData
{
    class Program
    {
        static void Main(string[] args)
        {

            string filepath = string.Empty; //@"D:\Metadata\Test\testfile\universe10e_ch03.ppt";
            List<MetaData> metadata = GetMetaDataList();
            string loop = "n";

            do
            {
                Console.Clear();
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("1 - files metadata on file");
                Console.WriteLine("2 - Read file metadata");
                Console.WriteLine("3 - Fetch the SFDC 18 Dig Id(SalesForceID) using the OnyxCustomerId value");
                Console.WriteLine("0 - Exist");
                Console.WriteLine("-----------------------------------");
                Console.Write("Enter your choice: ");
                ConsoleKeyInfo input = Console.ReadKey();
                Console.WriteLine("");
                string filesfolder = @"C:\DocumentSecurity\Logs\test";
                string zipfilepath = string.Empty;
                switch (input.KeyChar.ToString())
                {
                    case "1":
                        Console.Write("\nEnter file path to apply metadata: ");
                        filepath = Console.ReadLine();
                        if (!string.IsNullOrEmpty(filepath) && new FileInfo(filepath).Exists)
                        {
                            metadata = GetMetaDataList();
                            Console.WriteLine(string.Format("\n File Applymetadata  Start time:{0}, files folder", DateTime.Now.ToString("o"), filepath));
                            ApplyMetaData(filepath, metadata);
                            Console.WriteLine(string.Format("File Applymetadata End time:{0} , file path:{1}", DateTime.Now.ToString("o"), filepath));
                            Console.WriteLine("-----------------output------------------------");
                            Console.WriteLine("Please check file: {0} ", filepath);

                        }
                        else { Console.WriteLine("File path you have given not exists..."); }
                        Console.WriteLine("Press Enter key to return main menu...");
                        Console.ReadLine();
                        break;

                    case "2":
                        {
                            Console.Write("\nEnter file path to apply metadata: ");
                            filepath = Console.ReadLine();
                            if (!string.IsNullOrEmpty(filepath) && new FileInfo(filepath).Exists)
                            {
                                Console.WriteLine(string.Format("\nRead File Metadata Start time:{0}, files folder", DateTime.Now.ToString("o"), filepath));
                                metadata = ReadMetaData(filepath);
                                Console.WriteLine(string.Format("\nRead File Metadata End time:{0}, files folder", DateTime.Now.ToString("o"), filepath));
                                Console.WriteLine("-----------------output------------------------");
                                Console.WriteLine("---Key----\t|\t------Value---- ");
                                foreach (MetaData md in metadata)
                                {
                                    Console.WriteLine("  {0}\t|\t{1} ", md.Text, md.Value);
                                }
                            }
                            else { Console.WriteLine("File path you have given not exists..."); }

                            Console.WriteLine("Press Enter key to return main menu...");
                            Console.ReadLine();
                        }
                        break;

                    case "3":
                        {
                            Console.Write("\nEnter OnyxCustomerId: ");
                            string OnyxCustomerId = Console.ReadLine();//24073460
                            Console.WriteLine(string.Format("\nCall service to fetch SalesForceID Start time:{0}", DateTime.Now.ToString("o")));

                            string SalesForceID = GetUserSalesforceIdByOnyxCustomerID(OnyxCustomerId);
                            Console.WriteLine(string.Format("Call service to fetch SalesForceID End time:{0} ", DateTime.Now.ToString("o")));

                            Console.WriteLine("-----------------output------------------------");
                            Console.WriteLine(SalesForceID);

                            Console.WriteLine("Press Enter key to return main menu...");
                            Console.ReadLine();
                        }
                        break;
                    case "0":
                        Console.Write("\nAre you sure you want to exit.(y/n)?");
                        input = Console.ReadKey();
                        loop = input.KeyChar.ToString();
                        break;
                    default:
                        return;

                }
            } while (loop != "y");
            //
        }

        private static string GetUserSalesforceIdByOnyxCustomerID(string OnyxCustomerID)
        {
            string UserSalesforceId = string.Empty;

            FileMetaData.Classes.CustomerLegacy.Individual oIndividual  = CustomerLegacyByOnyxCustomerID(OnyxCustomerID);
            if (oIndividual != null)
                UserSalesforceId =oIndividual.AssignedId;
            return UserSalesforceId;
        }
        public static FileMetaData.Classes.CustomerLegacy.Individual CustomerLegacyByOnyxCustomerID(string OnyxCustomerID)
        {
            FileMetaData.Classes.CustomerLegacy.Individual oIndividual = null;
            string UserSalesforceId = string.Empty;
            string serviceUrlForUserSalesforceId = "http://services-uat.macmillan-learning.com/services/GetCustomerLegacy";
            string paramTemplate = "<parameters><secondaryId>{0}</secondaryId><objectType>individual</objectType></parameters>";
            try
            {
                //Assemble the URL string.Format(paramTemplate, OnyxCustomerID);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serviceUrlForUserSalesforceId);// "application /x-www-form-urlencoded";
                byte[] bytes;
                bytes = System.Text.Encoding.ASCII.GetBytes(string.Format(paramTemplate, OnyxCustomerID));
                request.Method = "POST";
                request.ContentType = "text/xml; encoding='utf-8'";
                request.ContentLength = bytes.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                HttpWebResponse response;
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = response.GetResponseStream();
                  string  serviceXMLResponse = new StreamReader(responseStream).ReadToEnd();
                    XMLHelper hlpr = new XMLHelper();
                    oIndividual = hlpr.XMLDeserialize<CustomerLegacy.Individual>(serviceXMLResponse);
                }

            }
            catch (Exception ex)
            { oIndividual = null; }

            return oIndividual;
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
