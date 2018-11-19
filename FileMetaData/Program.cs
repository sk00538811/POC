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
            List<DocumentData> metadata = GetMetaDataList();
            string loop = "n";
            FileInfo fi = null;
            do
            {filepath = string.Empty;
                Console.Clear();
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("1 - files metadata on file");
                Console.WriteLine("2 - Read file metadata");
                Console.WriteLine("3 - Read file metadata after url download");
                Console.WriteLine("4 - Fetch the SFDC 18 Dig Id(SalesForceID) using the OnyxCustomerId value");
                Console.WriteLine("5 - DocumentSecurity Metadata/Apply API");
                Console.WriteLine("6 - DocumentSecurity Metadata/Read API");
                Console.WriteLine("7 - Microsoft.Office.Interop dll Convert doc,xls and ppt file to docx, xlsx, and pptx");
                Console.WriteLine("8 - opensource Convert doc,xls and ppt file to docx, xlsx, and pptx");
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
                            filepath =  Console.ReadLine();
                            Console.Write("\nFile Path:{0} ", filepath);
                            fi = new FileInfo(filepath);
                            if (!string.IsNullOrEmpty(filepath) && fi.Exists)
                            {
                            string outputfilename =Path.Combine(fi.DirectoryName, "Output_"+fi.Name) ;
                                metadata = GetMetaDataList();
                                Console.WriteLine(string.Format("\n File Applymetadata  Start time:{0}, files folder", DateTime.Now.ToString("o"), fi.FullName));
                                switch (fi.Extension.ToUpper())
                                {
                                    case ".DOC":
                                    case ".XLS":
                                    case ".PPT":
                                        //ApplyMetaDataInComments(fi.FullName, metadata);
                                        ApplyMetaData(fi.FullName, metadata);
                                        break;
                                    case ".PDF":
                                        ApplyMetaDataInPDF(fi.FullName, outputfilename, metadata);
                                        Console.WriteLine("Please check output file: {0} ", outputfilename);
                                        break;
                                }
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
                            Console.Write("\nEnter file path to read metadata: ");
                            filepath =  Console.ReadLine();
                            Console.Write("\nFile Path:{0} ", filepath);
                            fi = new FileInfo(filepath);
                            if (!string.IsNullOrEmpty(filepath) && fi.Exists)
                            {
                                Console.WriteLine(string.Format("\nRead File Metadata Start time:{0}, files folder", DateTime.Now.ToString("o"), fi.FullName));
                                switch (fi.Extension.ToUpper())
                                {
                                    case ".DOC":
                                    case ".XLS":
                                    case ".PPT":
                                        // metadata = ReadMetaDataInComments(fi.FullName);
                                        metadata = ReadMetaData(fi.FullName);
                                        break;
                                    case ".PDF":
                                        metadata = ReadMetaDataFromPDF(fi.FullName, metadata);
                                        break;
                                }
                                Console.WriteLine(string.Format("\nRead File Metadata End time:{0}, files folder", DateTime.Now.ToString("o"), fi.FullName));
                                Console.WriteLine("-----------------output------------------------");
                                Console.WriteLine("---Key----\t|\t------Value---- ");
                                foreach (DocumentData md in metadata)
                                {
                                    Console.WriteLine("  {0}\t|\t{1} ", md.Key, md.Text);
                                }
                            }
                            else { Console.WriteLine("File path you have given not exists..."); }

                            Console.WriteLine("Press Enter key to return main menu...");
                            Console.ReadLine();
                        }
                        break;
                    case "3":
                        {
                            Console.Write("\nEnter file url to read metadata: ");
                          string  url =  Console.ReadLine();
                           //  url = "http://dev-s3c-webpub.s3.amazonaws.com/dev/bcs-test/Catalog-IR/20181011/w1.doc";//Answers_workbook.pdf
                            filepath = DownloadFileFromServer(url, "ttt");
                            Console.Write("\nFile Path:{0} ", filepath);
                            fi = new FileInfo(filepath);
                            if (!string.IsNullOrEmpty(filepath) && fi.Exists)
                            {
                                Console.WriteLine(string.Format("\nRead File Metadata Start time:{0}, files folder", DateTime.Now.ToString("o"), fi.FullName));
                                switch (fi.Extension.ToUpper())
                                {
                                    case ".DOC":
                                    case ".XLS":
                                    case ".PPT":
                                        //  metadata = ReadMetaDataInComments(fi.FullName);
                                        metadata = ReadMetaData(fi.FullName);
                                        break;
                                    case ".PDF":
                                        metadata = ReadMetaDataFromPDF(fi.FullName, metadata);
                                        break;
                                }

                                Console.WriteLine(string.Format("\nRead File Metadata End time:{0}, files folder", DateTime.Now.ToString("o"), fi.FullName));
                                Console.WriteLine("-----------------output------------------------");
                                Console.WriteLine("---Key----\t|\t------Value---- ");
                                foreach (DocumentData md in metadata)
                                {
                                    Console.WriteLine("  {0}\t|\t{1} ", md.Key, md.Text);
                                }
                            }
                            else { Console.WriteLine("File path you have given not exists..."); }

                            Console.WriteLine("Press Enter key to return main menu...");
                            Console.ReadLine();
                        }
                        break;
                    case "4":
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
                    case "5":
                        {    //"http://dev-s3c-webpub.s3.amazonaws.com/dev/bcs-test/Catalog-IR/20181011/Answers_workbook.pdf";
                            // "http://dev-s3c-webpub.s3.amazonaws.com/dev/bcs-test/Catalog-IR/20181013/a7648575-62bc-4ed2-8151-1d69ffd69a4a.zip";
                            //"http://dev-s3c-webpub.s3.amazonaws.com/dev/bcs-test/Catalog-IR/20181011/221dc991-ae8c-4a10-8108-1c95a0723fdf.zip";
                            string sourceFileUrl = Console.ReadLine();// "http://dev-s3c-webpub.s3.amazonaws.com/dev/bcs-test/Catalog-IR/20181011/w.docx";
                            string api_baseAddressUrl = "http://local.macmillanlearning.com/DocumentSecurityService/";
                            string api_requestUri = "Metadata/Apply";
                            MetadataParamBatch oMetadataParamBatch = new MetadataParamBatch();
                            oMetadataParamBatch.UserRAId = "1212";
                            List<MetadataParam> lstMetaDataParam = new List<MetadataParam>();
                            lstMetaDataParam.Add(new MetadataParam { documentType = DocumentType.Word, InputFileName = sourceFileUrl, ResourceType = "IR", ResourceName = "test" });
                            oMetadataParamBatch.MetadataParams = lstMetaDataParam;
                            List<DocumentData> metadataCollection = new List<DocumentData>(){
                                new DocumentData{Key="CopyRight", Text="2018" },
                                new DocumentData{Key="CompanyName" , Text="Macmillan Learning"},
                                new DocumentData{Key="CreatedOn", Text="16-Nov-2018" },
                                new DocumentData{Key="RequestId" , Text="11111"},
                                new DocumentData{Key="RequestDate" , Text="16-Nov-2018"} };

                            oMetadataParamBatch.MetadataCollection = metadataCollection;

                            Console.WriteLine(sourceFileUrl);
                            Console.WriteLine("");
                             string strresult = Webapicall_ApplyMetaData(api_baseAddressUrl, api_requestUri, oMetadataParamBatch);
                            Console.WriteLine("-----------------output------------------------");
                            Console.WriteLine(strresult);

                            Console.WriteLine("Press Enter key to return main menu...");
                            Console.ReadLine();
                        }
                        break;
                    case "6":
                        {    //"http://dev-s3c-webpub.s3.amazonaws.com/dev/bcs-test/Catalog-IR/20181011/Answers_workbook.pdf";
                            // "http://dev-s3c-webpub.s3.amazonaws.com/dev/bcs-test/Catalog-IR/20181013/a7648575-62bc-4ed2-8151-1d69ffd69a4a.zip";
                            //"http://dev-s3c-webpub.s3.amazonaws.com/dev/bcs-test/Catalog-IR/20181011/221dc991-ae8c-4a10-8108-1c95a0723fdf.zip";
                            string sourceFileUrl = Console.ReadLine(); //"http://dev-s3c-webpub.s3.amazonaws.com/dev/bcs-test/Catalog-IR/20181011/w.docx";
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
                            string strresult = Webapicall_ReadMetaData(api_baseAddressUrl, api_requestUri, parameters);
                            Console.WriteLine("-----------------output------------------------");
                            Console.WriteLine(strresult);

                            Console.WriteLine("Press Enter key to return main menu...");
                            Console.ReadLine();
                        }
                        break;
                    case "7":
                        {
                            Console.Write("\nEnter file path of doc,xls or ppt: ");
                            filepath = Console.ReadLine();
                              fi = new FileInfo(filepath);
                            switch (fi.Extension.ToUpper())
                            {
                                case ".DOC":
                                    { filepath= TransformNewOfficeDocument.DOCToDOCX(fi.FullName); }
                                    break;
                                case ".XLS":
                                    {  filepath=TransformNewOfficeDocument.XLSToXLSX(fi.FullName);}
                                    break;
                                case ".PPT":
                                    { filepath= TransformNewOfficeDocument.PPTToPPTX(fi.FullName);}
                                    break;
                            }
                            Console.WriteLine("-----------------output------------------------");
                            Console.WriteLine(filepath);

                            Console.WriteLine("Press Enter key to return main menu...");
                            Console.ReadLine();
                        }break;
                    case "8":
                        {
                            Console.Write("\nEnter file path of doc,xls or ppt: ");
                            filepath = Console.ReadLine();
                              fi = new FileInfo(filepath);
                            switch (fi.Extension.ToUpper())
                            {
                                case ".DOC":
                                    { filepath= TransformNewOfficeDocument_OpenSource.DOCToDOCX(fi.FullName); }
                                    break;
                                case ".XLS":
                                    {  filepath=TransformNewOfficeDocument.XLSToXLSX(fi.FullName);}
                                    break;
                                case ".PPT":
                                    { filepath= TransformNewOfficeDocument.PPTToPPTX(fi.FullName);}
                                    break;
                            }
                            Console.WriteLine("-----------------output------------------------");
                            Console.WriteLine(filepath);

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
        private static string Webapicall_ApplyMetaData(string baseAddressUrl, string requestUri, MetadataParamBatch param)
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
        private static string Webapicall_ReadMetaData(string baseAddressUrl, string requestUri, MetadataParamWithDocumnetData param)
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

        public static void ApplyMetaData(string filepath, List<DocumentData> lstmetadata)
        {
            bool ispropfound = false;
            OleDocumentProperties myFile = new DSOFile.OleDocumentProperties();
            myFile.Open(filepath, false, DSOFile.dsoFileOpenOptions.dsoOptionDefault);
            foreach (var metadata in lstmetadata)
            {
                ispropfound = false;
                foreach (DSOFile.CustomProperty property in myFile.CustomProperties)
                {
                    if (property.Name.ToUpper() == metadata.Key.ToUpper())
                    { ispropfound = true; break; }

                }
                if (!ispropfound)//add property
                {
                    object objValue = metadata.Text;
                    myFile.CustomProperties.Add(metadata.Key, ref objValue);
                }
            }
            myFile.Save();
            myFile.Close(true);


        }
        public static List<DocumentData> ReadMetaData(string filepath)
        {
            List<DocumentData> lstmetadata = new List<DocumentData>();

            OleDocumentProperties myFile = new DSOFile.OleDocumentProperties();
            myFile.Open(filepath, true, DSOFile.dsoFileOpenOptions.dsoOptionDefault);

            foreach (DSOFile.CustomProperty property in myFile.CustomProperties)
            {
                lstmetadata.Add(new DocumentData { Key = property.Name, Text = Convert.ToString(property.get_Value()) });
            }


            myFile.Close();

            return lstmetadata;
        }
        public static MetadataResult ApplyMetaDataInPDF(string filepath, string outputfilename, List<DocumentData> lstmetadata)
        {

            iTextSharpUtility oITextsharpUtility = new iTextSharpUtility();
            MetadataResult rsltMetadataResult = oITextsharpUtility.ApplyMetaData(filepath, outputfilename, lstmetadata);
            return rsltMetadataResult;

        }
        public static List<DocumentData> ReadMetaDataFromPDF(string filepath, List<DocumentData> lstmetadata)
        {

            List<string> metadataCollection = new List<string>();
            foreach (DocumentData dd in lstmetadata)
            {
                metadataCollection.Add(dd.Key);
            }
            iTextSharpUtility oITextsharpUtility = new iTextSharpUtility();
            MetadataResult rsltMetadataResult = oITextsharpUtility.ReadMetaData(filepath, metadataCollection);
            lstmetadata = rsltMetadataResult.MetaData;
            return lstmetadata;
        }

        /// <summary>
        /// Description: its works for DOC,PPT,XLS
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static void ApplyMetaDataInComments(string filepath, List<DocumentData> lstmetadata)
        {

            OleDocumentProperties myFile = new DSOFile.OleDocumentProperties();
            myFile.Open(filepath, false, DSOFile.dsoFileOpenOptions.dsoOptionDefault);
            /* 
               bool ispropfound = false;
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
            }*/
            myFile.SummaryProperties.Comments = JsonConvert.SerializeObject(lstmetadata);
            myFile.SummaryProperties.Title = JsonConvert.SerializeObject(lstmetadata);
            myFile.Save();
            myFile.Close(true);


        }
        /// <summary>
        /// Description: its works for DOC,PPT,XLS
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static List<DocumentData> ReadMetaDataInComments(string filepath)
        {
            List<DocumentData> lstmetadata = new List<DocumentData>();

            OleDocumentProperties myFile = new DSOFile.OleDocumentProperties();

            myFile.Open(filepath, true, DSOFile.dsoFileOpenOptions.dsoOptionOpenReadOnlyIfNoWriteAccess);

            /* foreach (DSOFile.CustomProperty property in myFile.CustomProperties)
             {
                 lstmetadata.Add(new DocumentData { Text = property.Name, Value = Convert.ToString(property.get_Value()) });
             }
             */
            string data = "";
            if (!string.IsNullOrEmpty(myFile.SummaryProperties.Comments))
                data = myFile.SummaryProperties.Comments;
            if (!string.IsNullOrEmpty(myFile.SummaryProperties.Title))
                data = myFile.SummaryProperties.Title;
            lstmetadata = JsonConvert.DeserializeObject<List<DocumentData>>(data);
            myFile.Close();


            return lstmetadata;
        }
        public static List<DocumentData> GetMetaDataList()
        {
            List<DocumentData> metadata = new List<DocumentData>();
            metadata.Add(new DocumentData { Key = "CopyRight", Text = "2018" });
            metadata.Add(new DocumentData { Key = "CompanyName", Text = "Macmillan Learning" });
            metadata.Add(new DocumentData { Key = "CreatedOn", Text = "14-oct-2018" });
            metadata.Add(new DocumentData { Key = "RequestId", Text = "1232" });
            metadata.Add(new DocumentData { Key = "RequestDate", Text = "14-nov-2018" });
            return metadata;
        }
        public static string DownloadFileFromServer(string url, string userId)
        {
            string TempFilePath = @"C:\DocumentSecurity\Logs\";
            string fileName = string.Empty;

            try
            {
                fileName = url.Split('/').LastOrDefault();

                using (WebClient wc = new WebClient())
                {
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        fileName = TempFilePath + userId + "\\" + fileName;
                        if (!Directory.Exists(TempFilePath + userId))
                        {
                            Directory.CreateDirectory(TempFilePath + userId);
                        }

                        if (File.Exists(fileName))
                        {
                            File.Delete(fileName);

                        }
                        wc.DownloadFile(url, fileName);
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

            return fileName;
        }

    }
}
