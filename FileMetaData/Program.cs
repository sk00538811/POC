using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSOFile;
using System.IO;
using System.Net;
using FileMetaData.Classes;
using System.Net.Http;
using Newtonsoft.Json;
using DocumentSecurity.BusinessObjects;

namespace FileMetaData
{
    class Program
    {
        static void Main(string[] args)
        {

            string filepath = string.Empty; //@"D:\Metadata\Test\testfile\universe10e_ch03.ppt";
            List<MetaData> metadata = GetMetaDataList();
            string loop = "n";
            FileInfo fi = null;
            do
            {
                Console.Clear();
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("1 - files metadata on file");
                Console.WriteLine("2 - Read file metadata");
                Console.WriteLine("3 - Fetch the SFDC 18 Dig Id(SalesForceID) using the OnyxCustomerId value");
                Console.WriteLine("4 - DocumentSecurity Metadata/Read API");
                Console.WriteLine("0 - Exist");
                Console.WriteLine("-----------------------------------");
                Console.Write("Enter your choice: ");
                ConsoleKeyInfo input = Console.ReadKey();
                Console.WriteLine("");
                string filesfolder = @"C:\DocumentSecurity\Logs\test\Instructor_Resource_PDFs";
                string zipfilepath = string.Empty;
                switch (input.KeyChar.ToString())
                {
                    case "1":
                        {
                            Console.Write("\nEnter file path to apply metadata: ");
                            filepath = @"C:\DocumentSecurity\Logs\test\Answers_workbook.pdf";// Console.ReadLine();
                            Console.Write("\nFile Path:{0} ", filepath);
                            fi = new FileInfo(filepath);
                            if (!string.IsNullOrEmpty(filepath) && fi.Exists)
                            {
                                metadata = GetMetaDataList();
                                Console.WriteLine(string.Format("\n File Applymetadata  Start time:{0}, files folder", DateTime.Now.ToString("o"), fi.FullName));
                                ApplyMetaData(fi.FullName, metadata);
                                Console.WriteLine(string.Format("File Applymetadata End time:{0} , file path:{1}", DateTime.Now.ToString("o"), fi.FullName));
                                Console.WriteLine("-----------------output------------------------");
                                Console.WriteLine("Please check file: {0} ", fi.FullName);

                            }
                            else { Console.WriteLine("File path you have given not exists..."); }
                            Console.WriteLine("Press Enter key to return main menu...");
                            Console.ReadLine();
                        }
                        break;

                    case "2":
                        {
                            Console.Write("\nEnter file path to apply metadata: ");
                            filepath = @"C:\Users\sk00538811\Downloads\Answers_workbook.pdf";// Console.ReadLine();
                            Console.Write("\nFile Path:{0} ", filepath);
                            fi = new FileInfo(filepath);
                            if (!string.IsNullOrEmpty(filepath) && fi.Exists) 
                                {
                                Console.WriteLine(string.Format("\nRead File Metadata Start time:{0}, files folder", DateTime.Now.ToString("o"), fi.FullName));
                                metadata = ReadMetaData(fi.FullName);
                                Console.WriteLine(string.Format("\nRead File Metadata End time:{0}, files folder", DateTime.Now.ToString("o"), fi.FullName));
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
                    case "4":
                        {    //"http://dev-s3c-webpub.s3.amazonaws.com/dev/bcs-test/Catalog-IR/20181011/Answers_workbook.pdf";
                            // "http://dev-s3c-webpub.s3.amazonaws.com/dev/bcs-test/Catalog-IR/20181013/a7648575-62bc-4ed2-8151-1d69ffd69a4a.zip";
                            //"http://dev-s3c-webpub.s3.amazonaws.com/dev/bcs-test/Catalog-IR/20181011/221dc991-ae8c-4a10-8108-1c95a0723fdf.zip";
                            string sourceFileUrl = "http://dev-s3c-webpub.s3.amazonaws.com/dev/bcs-test/Catalog-IR/20181013/a7648575-62bc-4ed2-8151-1d69ffd69a4a.zip";
                            string api_baseAddressUrl = "http://local.macmillanlearning.com/DocumentSecurityService/";
                            string api_requestUri = "Metadata/Read";
                            MetadataParamWithDocumnetData parameters = new MetadataParamWithDocumnetData();
                            parameters.documentType = DocumentType.Word;
                            List<DocumentData> metadataCollection = new List<DocumentData>(){
                                new DocumentData{Key="CopyRight" },
                                new DocumentData{Key="CompanyName" },
                                new DocumentData{Key="CreatedOn" },
                                new DocumentData{Key="RequestId" },
                                new DocumentData{Key="RequestDate" } };

                            parameters.MetadataCollection = metadataCollection;

                            Console.WriteLine(sourceFileUrl);
                            Console.WriteLine("");
                            parameters.OutputFileName = sourceFileUrl;
                            parameters.documentType = DocumentType.ZIP;
                            string strresult = Webapicall1(api_baseAddressUrl, api_requestUri, parameters);
                            Console.WriteLine("-----------------output------------------------");
                            Console.WriteLine(strresult);

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
        private static string Webapicall1(string baseAddressUrl, string requestUri, MetadataParamWithDocumnetData param)
        {
            string strresult = String.Empty;

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(baseAddressUrl);

                    // Create POST data and convert it to a byte array.
                    string postData = JsonConvert.SerializeObject(param);// IRDownloadFinalData
                    byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(postData);

                    // Set the ContentType property of the WebRequest.
                    var content = new StringContent(postData, Encoding.UTF8, "application/json");


                    httpClient.Timeout = new TimeSpan(0, 15, 0);
                    HttpResponseMessage httpResponseMessage = httpClient.PostAsync(requestUri, content).Result;
                    httpResponseMessage.Content.ReadAsStringAsync().Wait();


                    Task<string> response = httpResponseMessage.Content.ReadAsStringAsync();


                    MetadataResultList bsObj = JsonConvert.DeserializeObject<MetadataResultList>(response.Result.ToString());
                    strresult = JsonConvert.SerializeObject(bsObj);



                }
            }
            catch { }
            return strresult;
        }

        private static string GetUserSalesforceIdByOnyxCustomerID(string OnyxCustomerID)
        {
            string UserSalesforceId = string.Empty;

            FileMetaData.Classes.CustomerLegacy.Individual oIndividual = CustomerLegacyByOnyxCustomerID(OnyxCustomerID);
            if (oIndividual != null)
                UserSalesforceId = oIndividual.AssignedId;
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
                    string serviceXMLResponse = new StreamReader(responseStream).ReadToEnd();
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
            metadata.Add(new MetaData { Text = "CopyRight", Value = "2018" });
            metadata.Add(new MetaData { Text = "CompanyName", Value = "Macmillan Learning" });
            metadata.Add(new MetaData { Text = "CreatedOn", Value = "14-oct-2018" });
            metadata.Add(new MetaData { Text = "RequestId", Value = "1232" });
            metadata.Add(new MetaData { Text = "RequestDate", Value = "14-nov-2018" });
            return metadata;
        }
        public class MetaData
        {
            public string Text { get; set; }
            public string Value { get; set; }
        }
    }
}
