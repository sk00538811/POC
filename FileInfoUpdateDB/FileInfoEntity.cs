using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileInfoUpdateDB
{
    public class FileInfoEntity
    {
        public string DirectoryFullPath { get; set; }
        public string LogFileFullPath { get; set; }
        public List<FileInfoDataEntity> oEntities { get; set; }
        public DataSet GetTableStructure()
        {
            DataSet dbTables = new DataSet("data");
            DataTable dt = new DataTable("TT_FileInfoDetails");
            dt.Columns.Add("FileInfoDetailsId", Type.GetType("System.Int64"));
            dt.Columns.Add("FullFilePath", Type.GetType("System.String"));
            dt.Columns.Add("FileName", Type.GetType("System.String"));
            dt.Columns.Add("FileExtension", Type.GetType("System.String"));
            dt.Columns.Add("FileSize", Type.GetType("System.Int64"));
            dt.Columns.Add("FileLastModifiedDate", Type.GetType("System.DateTime"));
            dt.Columns.Add("InUse", Type.GetType("System.Boolean"));
            dt.AcceptChanges();
            dbTables.Tables.Add(dt);
            dbTables.AcceptChanges();

            return dbTables;
        }
        public DataSet FillEntityInDataSet(List<FileInfoDataEntity> oEntities_)
        {
            oEntities = oEntities_ ?? oEntities;
            DataSet dbTables = GetTableStructure();
            DataTable dt = dbTables.Tables["TT_FileInfoDetails"];
            foreach (FileInfoDataEntity oEntity in oEntities)
            {
                DataRow dr = dt.NewRow();
                dr["FileInfoDetailsId"] = oEntity.FileInfoDetailsId ?? 0;
                dr["FullFilePath"] = oEntity.FullFilePath ?? "";
                dr["FileName"] = oEntity.FileName ?? "";
                dr["FileExtension"] = oEntity.FileExtension ?? "";
                dr["FileSize"] = oEntity.FileSize ?? 0;
                if (oEntity.FileLastModifiedDate != null)
                    dr["FileLastModifiedDate"] = oEntity.FileLastModifiedDate;
                dr["InUse"] = oEntity.InUse ?? false;
                dt.Rows.Add(dr);

            }
            dt.AcceptChanges();
            return dbTables;
        }
        public bool SaveFileInfoInDB(DataSet dbTables)
        {
            int cnt = 0;
            string logFile = ConfigurationManager.AppSettings.Get("LogFleName");
            File.AppendAllText(logFile, "Start inserting in database table: total record: " + dbTables.Tables["TT_FileInfoDetails"].Rows.Count.ToString() + Environment.NewLine);
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["AppControllerDB"].ConnectionString))
            {

                using (SqlCommand cmd = new SqlCommand("[dbo].[uspInsertFileInfo]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter("@TT_FileInfoDetails", dbTables.Tables["TT_FileInfoDetails"]);
                    param.SqlDbType = SqlDbType.Structured;
                    param.TypeName = "TT_FileInfoDetails";
                    cmd.Parameters.Add(param);

                    try
                    {
                        con.Open();
                        cnt = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(logFile, "Unable to insert file information in database. Detail:" + ex.Message);
                    }
                    finally
                    {
                        con.Close();
                        File.AppendAllText(logFile, "End file information inserting in database table: total record: " + cnt.ToString() + Environment.NewLine);

                    }
                }

            }
            return cnt > 0;
        }
        public List<FileInfoDataEntity> GetDirectoriesFileInfo(string directoryfullPath = null)
        {
            string strlog = "";
            if (directoryfullPath != null)
                DirectoryFullPath = directoryfullPath;
            oEntities = oEntities ?? new List<FileInfoDataEntity>();
            DataTable dtContent = new DataTable();
            string logFile = ConfigurationManager.AppSettings.Get("LogFleName");
            List<FileInfoDataEntity> oEnities = new List<FileInfoDataEntity>();
            string[] files = Directory.GetFiles(DirectoryFullPath, "*.*", SearchOption.AllDirectories);
            strlog = "Total files in Dir/Sub Dir is: " + files.Length.ToString() + Environment.NewLine;
            File.AppendAllText(logFile, strlog); Console.Write(strlog);
            strlog = "FileInfo Processing Started" + Environment.NewLine;
            File.AppendAllText(logFile, strlog); Console.Write(strlog);
            strlog = "---------------------------------------------------------------" + Environment.NewLine;
             File.AppendAllText(logFile, strlog); Console.Write(strlog);
            //Fetching the files from the folder
            int i = 1;
            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                FileInfoDataEntity oEnity = new FileInfoDataEntity();
                if (fi.Length > 0) //some time file with 0 size, we will ignore such file
                {
                    try
                    {  /*
                            FullFilePath
                            FileName
                            FileExtension
                            FileSize
                            FileLastModifiedDate
                            InUse
                            */
                        oEnity.FullFilePath = file;
                        oEnity.FileName = fi.Name;
                        oEnity.FileExtension = fi.Extension;
                        oEnity.FileSize = fi.Length;
                        oEnity.FileLastModifiedDate = fi.LastWriteTime;

                        oEnities.Add(oEnity);
                          
                        File.AppendAllText(logFile, string.Format("  {0}. File Name: {1}\t Full Path: {2} " + Environment.NewLine, i, fi.Name, fi.DirectoryName));
                        Console.Write(string.Format( "  {0}. File Name: {1}\t Full Path: {2} "  + Environment.NewLine,i, fi.Name, fi.DirectoryName ) );
                    }
                    catch (Exception ex)
                    {
                        //Log the error detaails in the processlog  file
                        strlog = "Error Processing File: " + file + ".  Error Message: " + ex.Message + Environment.NewLine  ;
                        File.AppendAllText(logFile, strlog); Console.Write(strlog);
                    }
                }
                i++;
            }
            strlog = "---------------------------------------------------------------" + Environment.NewLine;
            File.AppendAllText(logFile, strlog); Console.Write(strlog);

            strlog = "FileInfo Processing Ended" + Environment.NewLine ;
            File.AppendAllText(logFile, strlog); Console.Write(strlog);
            return oEnities;
        }

        public class FileInfoDataEntity
        {
            #region property
            public Int64? FileInfoDetailsId { get; set; }
            public string FullFilePath { get; set; }
            public string FileName { get; set; }
            public string FileExtension { get; set; }
            public Int64? FileSize { get; set; }
            public DateTime? FileLastModifiedDate { get; set; }
            public bool? InUse { get; set; }
            #endregion
        }
    }
}
