using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Configuration;
using System.IO;
using System.Data.SqlClient;
using System.Data;

namespace FileInfoUpdateDB
{
    /// <summary>
    /// Console application for extracting the zip files and to write the zip contents in the database
    /// </summary>
    class Program
    {
        //Zip folder path reading from the config file
        static string FilesFolderPath = string.Empty;

        //Datatable defined to store the zipcontent to update in the db
        private static DataTable dtContent = null;

        private static string logFile = string.Empty;


        /// <summary>
        /// Application starting point for the console application
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string strlog = "";
            try
            {
                FilesFolderPath = ConfigurationManager.AppSettings.Get("FilesFolderPath");
                logFile = ConfigurationManager.AppSettings.Get("LogFleName");

                //Create object of FileInfoEntity class that have all possible property and method for file information
                FileInfoEntity obj = new FileInfoEntity();
                File.WriteAllText(logFile, string.Empty);
                  strlog = string.Format("----------Execution start Data :  {0} , time : {1}----------------------------------------- " + Environment.NewLine, DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString());
                File.AppendAllText(logFile, strlog);  Console.Write(strlog);
                strlog = "Start colleting file information from  Dir:" + FilesFolderPath + Environment.NewLine;
                File.AppendAllText(logFile, strlog);  Console.Write(strlog);
                
                //collect all file information
                List<FileInfoEntity.FileInfoDataEntity> lstFileinfo = obj.GetDirectoriesFileInfo(FilesFolderPath);
                strlog = "All files info are colleted  from  Dir:" + FilesFolderPath + Environment.NewLine;
                File.AppendAllText(logFile,strlog); Console.Write(strlog);
                strlog = "List of file information is filling in dataset table:" + Environment.NewLine;
                File.AppendAllText(logFile, strlog); Console.Write(strlog);

                //fill entity list into dataset
                DataSet dbtables = obj.FillEntityInDataSet(lstFileinfo);

                strlog = "Now Dataset table have all list of file information. and we are try to save in databse." + Environment.NewLine;
                File.AppendAllText(logFile, strlog); Console.Write(strlog);
                //save dataset table data in database
                bool flag = obj.SaveFileInfoInDB(dbtables);
                if (flag) {
                    strlog = "\nFileInfoEntity.SaveFileInfoInDB(dbtables) execute successfully. all record is save in database." + Environment.NewLine;
                    File.AppendAllText(logFile, strlog); Console.Write(strlog);
                }
                else {
                    strlog = "\nFileInfoEntity.SaveFileInfoInDB(dbtables) unable to insert record.   " + Environment.NewLine;
                    File.AppendAllText(logFile, strlog); Console.Write(strlog);
                }
            }
            catch (Exception ex)
            {
                //Log the processing file details in the ProcessLog.log file
                strlog = "\nError:" + ex.Message + Environment.NewLine ;
                File.AppendAllText(logFile, strlog); Console.Write(strlog);
            }
            finally
            {
                strlog = string.Format("----------Execution end Data :  {0} , time : {1}----------------------------------------- " + Environment.NewLine, DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString());
                File.AppendAllText(logFile, strlog); Console.Write(strlog);
            }
            Console.WriteLine("Please entry any key to exist...");
            Console.ReadKey();

        }
        #region old code
        /*
        static void Main(string[] args)
        {     
             FilesFolderPath = ConfigurationManager.AppSettings.Get("FilesFolderPath");
             //Getting the table schema from database table
             dtContent = GetTableSchema();

             //Retrieving the zip files from the path mentioned
             GetDirectories(FilesFolderPath);

             //Inserting the zipcontents to Table
             BulkInsert(dtContent);
         }
         
       

        /// <summary>
        /// Recursive method to extract zip files from the folder and subfolder
        /// </summary>
        /// <param name="directoryPath">Path of a zip folder</param>
        /// <returns></returns>
        private static string GetDirectories(string directoryPath)
        {
            //Fetching the files from the folder
            foreach (string file in Directory.GetFiles(directoryPath))
            {
                //Checking for the zip file
                if (new FileInfo(file).Extension.ToLower().Equals(".zip"))
                {
                    try
                    {
                        //Log the processing file details in the ProcessLog.log file
                        File.AppendAllText(logFile, file + " Processing Started" + Environment.NewLine);

                        //Looping through the the contents of the zip file
                        foreach (ZipArchiveEntry zipEntry in ZipFile.Open(file, ZipArchiveMode.Read).Entries)
                        {

                            if (zipEntry != null)
                            {
                                //Retrieving the file details such as file size , modifed date, and  file name
                                FileInfo zipFileInfo = new FileInfo(file);
                                long zipFileSize = zipFileInfo.Length;
                                DateTime zipFileCreatedDate = zipFileInfo.LastWriteTime;
                                string zipName = zipFileInfo.Name;

                                //populating the zip contents details in to the datatable
                                if (dtContent != null)
                                {
                                    DataRow dtRow = dtContent.NewRow();

                                    // Retrieving the zip content file details such as file size , modifed date, and  file name
                                    long contentSize = zipEntry.CompressedLength;
                                    string zipEntryFilename = zipEntry.FullName;

                                    string zipContentsFileType = string.Empty;//zipContents.Substring(zipContents.LastIndexOf('.') + 1);

                                    //checking the zip file content is a folder or a file
                                    bool isFolder = zipEntryFilename.EndsWith("/");

                                    if (!isFolder)
                                    {
                                        //Getting the zipcontent file extension
                                        zipContentsFileType = zipEntryFilename.Substring(zipEntryFilename.LastIndexOf('.') + 1);
                                    }

                                    if (zipEntryFilename.Length > 500)
                                    {
                                        zipEntryFilename = zipEntryFilename.Substring(zipEntryFilename.Length - 500, 500);
                                    }

                                    //Adding each zip entries to datarow
                                    dtRow["ZipFullPath"] = file;
                                    dtRow["ZipName"] = zipName;
                                    dtRow["ZipFileLastModifed"] = zipFileCreatedDate;
                                    dtRow["ZipFileSize"] = zipFileSize;

                                    dtRow["ZipContents"] = zipEntryFilename;
                                    dtRow["ZipContentsFileType"] = zipContentsFileType;
                                    dtRow["IsFolder"] = isFolder;


                                    dtRow["ZipContentsLastModifed"] = zipEntry.LastWriteTime.DateTime;
                                    dtRow["ZipContentSize"] = contentSize;

                                    dtContent.Rows.Add(dtRow);

                                }


                            }


                        }
                    }
                    catch (Exception ex)
                    {
                        //Log the error detaails in the processlog  file
                        File.AppendAllText(logFile, "Error Processing File: " + file + ".  Error Message: " + ex.Message + Environment.NewLine);
                    }

                    File.AppendAllText(logFile, file + " Processing Ended" + Environment.NewLine);

                }

            }

            //Recursive call to get the zip file from sub folders
            string[] subDirectories = Directory.GetDirectories(directoryPath);

            foreach (string dir in subDirectories)
            {
                GetDirectories(dir);
            }

            return directoryPath;
        }

        /// <summary>
        /// Method used to insert the zip entries populated in the datatable to db
        /// </summary>
        /// <param name="contentTable"></param>
        private static void BulkInsert(DataTable contentTable)
        {
            //Connection string retrieving from the webconfig file
            string conString = ConfigurationManager.ConnectionStrings["AppControllerDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(conString))
            {
                conn.Open();

                //Inserting the zipcontent to db in a single db call
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                {
                    bulkCopy.DestinationTableName = "dbo.tblZipFileContent_rm";

                    // Write from the source to the destination.
                    bulkCopy.WriteToServer(contentTable);

                }


            }

        }

        /// <summary>
        /// Method used to retrieved the table schema from sql table tblZipFileContent
        /// </summary>
        /// <returns></returns>
        private static DataTable GetTableSchema()
        {
            string conString = ConfigurationManager.ConnectionStrings["AppControllerDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(conString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    string cmdText = "select  id,ZipFullPath,ZipName,ZipFileSize, ZipFileLastModifed,ZipContents,ZipContentsFileType,IsFolder,ZipContentSize,ZipContentsLastModifed from [dbo].[tblZipFileContent_rm]";

                    cmd.CommandText = cmdText;
                    cmd.Connection = conn;
                    SqlDataAdapter a = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    return a.FillSchema(dt, SchemaType.Mapped);

                }
            }

        }
 */
        #endregion
    }

    }




