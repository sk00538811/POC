using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO; 
using NPOI.HSSF.UserModel;
namespace FileMetaData.Classes
{
    public class TransformNewOfficeDocument_OpenSource
    {
        public static string DOCToDOCX(string filepath)
        {
            string outputfilepath = string.Empty;
            /*
            XWPFDocument doc = null; 

            FileInfo fi = new FileInfo(filepath);
            if (fi.Exists)
            {
                using (FileStream fs = new FileStream(fi.FullName, FileMode.Open))
                {
                    doc = new XWPFDocument(fs);
                    outputfilepath = fi.FullName.Replace(fi.Extension, ".docx");
                    FileStream out1 = new FileStream(outputfilepath, FileMode.Create);
                    doc.Write(out1);
                    out1.Close();
                }
            }*/
            FileInfo fi = new FileInfo(filepath);
            if (fi.Exists)
            {
               Spire.Doc.Document   doc = new Spire.Doc.Document(); 
                    doc.LoadFromFile(fi.FullName);
                outputfilepath = fi.FullName.Replace(fi.Extension, ".docx");
                doc.SaveToFile(outputfilepath,  Spire.Doc.FileFormat.Docx);

            }
            return outputfilepath;
        }
        public static string XLSToXLSX(string filepath)
        {
            string outputfilepath = string.Empty;
           /* FileInfo fi = new FileInfo(filepath);
            if (fi.Exists)
            {
                // create a new excel document 
                Workbook book = new Workbook();
                //load a document from file 
                book.LoadFromFile("test.xls");
                //Save the file to the version you want 
                book.SaveToFile("test.xlsx", ExcelVersion.Version2007);

                eWorkbook.SaveAs(fi.FullName.Replace(fi.Extension, ".xlsx"), Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                eWorkbook.Close(false, Type.Missing, Type.Missing); excelApp.Quit();
            }*/
            return outputfilepath;
        }
        public static string PPTToPPTX(string filepath)
        {
          string outputfilepath = string.Empty;
            /*  FileInfo fi = new FileInfo(filepath);
            if (fi.Exists)
            {

                MSPowerPoint.Application app = new MSPowerPoint.Application();
                object missing = Type.Missing;
                MSPowerPoint.Presentation pptx = app.Presentations.Open(fi.FullName, MsoTriState.msoTrue
                , MsoTriState.msoTrue, MsoTriState.msoTrue);
                pptx.SaveAs(fi.FullName.Replace(fi.Extension, ".pptx")
                    , MSPowerPoint.PpSaveAsFileType.ppSaveAsDefault);
                pptx.Close();
                app.Quit();
            }*/
            return outputfilepath;
        }

    }
}
