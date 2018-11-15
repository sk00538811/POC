using DocumentSecurity.BusinessObjects;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMetaData.Classes
{
  public  class iTextSharpUtility
    {
        public MetadataResult ApplyMetaData(string inputFileName, string outputFileName, List<DocumentData> metaDataCollection)
        {
            
            MetadataResult retDataCol = new MetadataResult();
            PdfReader reader = null;
            PdfStamper stamper = null;
            if (metaDataCollection == null)
            {
                retDataCol.Status = "Error";
                retDataCol.Message = "There has to be at least 1 metadata field that needs to be applied to the assest";
            }
            try
            {
                reader = new PdfReader(inputFileName);

                //for (int i = 1; i < reader.NumberOfPages; i++) //? Do we need metadata for all the pages ? I dont know yet!
                //{

                //getting the first page of the pdf document
                PdfDictionary pagedict = reader.GetPageN(1);

                foreach (DocumentData docData in metaDataCollection)
                {
                    //Applying the meta data in the first page
                    pagedict.Put(new PdfName(docData.Key), new PdfString(docData.Text));
                }
                //}

                //updating the meta data into the output file
                stamper = new PdfStamper(reader, new FileStream(outputFileName, FileMode.Create, FileAccess.Write, FileShare.None));
                stamper.SetFullCompression();
                stamper.Close();
                retDataCol.Status = "Success";
                retDataCol.Message = "Metadata applied successfully..";
                 
            }
            catch (Exception ex)
            {
                
                retDataCol.Status = "Error";
                retDataCol.Message = ex.ToString();
             }
            finally
            {
                if (reader != null) reader.Close();
            }

            return retDataCol;
        }
        public MetadataResult ReadMetaData(string fileName, IList<string> metaDataCollection)
        {
            MetadataResult retMetadataResult = new MetadataResult();
            if (metaDataCollection == null)
            {
                retMetadataResult.Status = "Success";
                retMetadataResult.Message = "No metadata found."; return retMetadataResult;
            }

            PdfReader reader = null;
            try
            {
                //Opening the pdf file
                reader = new PdfReader(fileName);
                //getting the first page
                PdfDictionary pagedict = reader.GetPageN(1);

                if (metaDataCollection != null)
                {
                    //loop throught the meta keys
                    foreach (string key in metaDataCollection)
                    {
                        //retrive the meta data based on the key

                        DocumentData docData = new DocumentData();
                        docData.Key = key;
                        docData.Text = pagedict.Get(new PdfName(key)).ToString();
                        if (retMetadataResult.MetaData == null)
                            retMetadataResult.MetaData = new List<DocumentData>();
                        retMetadataResult.MetaData.Add(docData);
                    }
                }

                if (retMetadataResult.MetaData.Count > 0)
                {
                    retMetadataResult.Status = "Success";
                    retMetadataResult.Message = "Metadata collected successfully.";
                }
                else
                {
                    retMetadataResult.Status = "Success";
                    retMetadataResult.Message = "No metadata found.";
                }
            }
            catch (Exception ex)
            {
                retMetadataResult.Status = "Error";
                retMetadataResult.Message = ex.Message;
             }
            finally
            {
                if (reader != null) reader.Close();
            }

            return retMetadataResult;
        }
    }
}
