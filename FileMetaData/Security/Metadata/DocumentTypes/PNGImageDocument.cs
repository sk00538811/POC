using DocumentSecurity.BusinessObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
namespace FileMetaData.Security.Metadata.DocumentTypes
{
    public class PNGImageDocument  
    {
        /// <summary>
        /// Method not implemented
        /// </summary>
        /// <param name="inputFileName"></param>
        /// <param name="metaDataCollection"></param>
        /// <returns></returns>
        public MetadataResult ApplyMetaData(string inputFileName, List<DocumentData> metaDataCollection)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method to apply meta data in the png file
        /// </summary>
        /// <param name="inputFileName">input file name</param>
        /// <param name="outputFileName">output file name</param>
        /// <param name="metaDataCollection">Metadata collection</param>
        /// <returns></returns>
        public MetadataResult ApplyMetaData(string inputFileName, string outputFileName, List<DocumentData> metaDataCollection)
        {
            MetadataResult retDataCol = new MetadataResult();
            if (metaDataCollection == null)
            {
                retDataCol.Status = "Error";
                retDataCol.Message = "There has to be at least 1 metadata field that needs to be applied to the assest";
            }
            try
            {
                using (FileStream fileStream = File.Open(inputFileName, FileMode.Open))
                {
                    //Creating the bitmat decoder from the input file streaam
                    BitmapDecoder decoder = PngBitmapDecoder.Create(fileStream, BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.Default);

                    BitmapMetadata md = new BitmapMetadata("png");

                    //Creating the frame with the meta data
                    BitmapFrame frame = BitmapFrame.Create(decoder.Frames[0], decoder.Frames[0].Thumbnail, md, decoder.Frames[0].ColorContexts);

                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    // BitmapMetadata md = new BitmapMetadata("png");
                    encoder.Frames.Add(frame);

                    int ctr = 0;

                    foreach (DocumentData documentData in metaDataCollection)
                    {
                        if (ctr == 0)
                        {

                            //Adding the meta data
                            // md.SetQuery(@"/Text/" + key, metaDataCollection[key]);
                            md.SetQuery("/iTXt/Keyword", documentData.Key.ToCharArray()); // need to convert using ToCharArray as internal representation is based on the LPSTR C type
                            md.SetQuery("/iTXt/TextEntry", documentData.Text);
                        }
                        else
                        {

                            md.SetQuery("/[" + ctr + "]iTXt/" + "Keyword", documentData.Key.ToCharArray()); // need to convert using ToCharArray as internal representation is based on the LPSTR C type
                            md.SetQuery("/[" + ctr + "]iTXt/" + "TextEntry", documentData.Text);
                        }
                        ctr++;
                    }

                    // encoder.Frames.Add(frame);

                    //after assigning the meta data saving to another file
                    using (FileStream of = File.Open(outputFileName, FileMode.Create, FileAccess.Write))
                    {
                        encoder.Save(of);
                        of.Close();
                    }
                }

                retDataCol.Status = "Success";
                retDataCol.Message = "Metadata applied successfully.";
            }
            catch (Exception ex)
            {
                retDataCol.Status = "Error";
                retDataCol.Message = ex.ToString();
                //Logger.LogError("PNGImageDocument", "ApplyMetaData", inputFileName, outputFileName, ex);
            }

            return retDataCol;
        }


        /// <summary>
        /// Method for reading the meta data from the input file
        /// </summary>
        /// <param name="fileName">input file name</param>
        /// <param name="metaDataCollection">Empty</param>
        /// <returns>Contains with metadata key and value</returns>
        public MetadataResult ReadMetaData(string fileName, IList<string> metaDataCollection)
        {
            if (metaDataCollection == null)
                return null;

            MetadataResult retDataCol = new MetadataResult();

            Dictionary<string, string> metaDataCol = new Dictionary<string, string>();
            try
            {
                using (Stream pngStream = new System.IO.FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    PngBitmapDecoder pngDecoder = new PngBitmapDecoder(pngStream, BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.Default);

                    GetMetaDataItems(pngDecoder.Frames[0].Metadata as BitmapMetadata, ref metaDataCol, "");
                    retDataCol.MetaData = (from metadata in metaDataCol
                                           select new DocumentData
                                           {
                                               Key = metadata.Key,
                                               Text = metadata.Value
                                           }).Where(k => metaDataCollection.Contains(k.Key)).ToList();
                }

                retDataCol.Status = "Success";
                retDataCol.Message = "Metadata read successfully.";
            }
            catch (Exception ex)
            {
                retDataCol.Status = "Error";
                retDataCol.Message = ex.ToString();
                //Logger.LogError("PNGImageDocument", "ReadMetaData", fileName, string.Empty, ex);
            }

            return retDataCol;
        }

        /// <summary>
        /// To clear the metadata from file
        /// </summary>
        /// <param name="inputFileName"></param>
        /// <param name="outputFileName"></param>
        /// <returns></returns>
        public MetadataResult ClearMetaData(string inputFileName, string outputFileName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the metadata item recursively
        /// </summary>
        /// <param name="bitmapMetadata"></param>
        /// <param name="itemMap"></param>
        /// <param name="filter"></param>
        /// <param name="query"></param>
        private static void GetMetaDataItems(BitmapMetadata bitmapMetadata, ref Dictionary<string, string> itemMap, string filter = null, string query = null)
        {
            if (query == null)
                query = string.Empty;

            if (bitmapMetadata != null)
            {
                var key = string.Empty;

                foreach (string relativeQuery in bitmapMetadata)
                {
                    var fullQuery = query + relativeQuery;
                    // GetQuery returns an object: either a string or child metadata
                    // If a string then it is one of 4 values: ["Keyword", "Translated", "Compression", "Language Tag", "TextEntry"]
                    // We want the Keyword and the subsequent TextEntry items, the tags are a sequence in the order specified above
                    var metadata = bitmapMetadata.GetQuery(relativeQuery);
                    var innerBitmapMetadata = metadata as BitmapMetadata;

                    if (innerBitmapMetadata == null)
                        AddToMap(ref key, fullQuery, metadata.ToString(), ref itemMap, filter);    // Not a metadata structure so it is data - therefore check and Add to map
                    else
                        GetMetaDataItems(innerBitmapMetadata, ref itemMap, filter, fullQuery);      // Recursive call
                }
            }
        }

        /// <summary>
        /// Add the metadata in to the itemMap dictiory object
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fullQuery"></param>
        /// <param name="metadata"></param>
        /// <param name="itemMap"></param>
        /// <param name="filter"></param>
        private static void AddToMap(ref string key, string fullQuery, string metadata, ref Dictionary<string, string> itemMap, string filter)
        {
            if (metadata != null)
            {
                if (!fullQuery.Contains("Translated"))
                {
                    if ((filter == null) || ((fullQuery.Contains(filter))))
                    {
                        //retrieving the key
                        if (fullQuery.Contains("Keyword"))
                            key = metadata;
                        //retrieving the value
                        if (fullQuery.Contains("TextEntry") && (key != null))
                            itemMap[key] = metadata.ToString();
                    }
                }
            }
        }
    }


}
