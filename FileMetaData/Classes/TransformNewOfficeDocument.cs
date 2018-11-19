using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSWord = Microsoft.Office.Interop.Word;
using MSExcel = Microsoft.Office.Interop.Excel;
using MSPowerPoint = Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;
namespace FileMetaData.Classes
{
    public class TransformNewOfficeDocument
    {
        public static string DOCToDOCX(string filepath)
        {
            string outputfilepath = string.Empty;
            FileInfo fi = new FileInfo(filepath);
            if (fi.Exists)
            {
                MSWord.Application wordApp = new MSWord.Application() { Visible = false };
                MSWord.Document doc = wordApp.Documents.Open(fi.FullName);
                outputfilepath = fi.FullName.Replace(fi.Extension, ".docx");
                doc.SaveAs2(outputfilepath, MSWord.WdSaveFormat.wdFormatDocumentDefault);
                doc.Close();
                wordApp.Quit();
            }
            return outputfilepath;
        }
        public static string XLSToXLSX(string filepath)
        {
            string outputfilepath = string.Empty;
            FileInfo fi = new FileInfo(filepath);
            if (fi.Exists)
            {
                MSExcel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                excelApp.Visible = false;
                MSExcel.Workbook eWorkbook = excelApp.Workbooks.Open(fi.FullName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                outputfilepath = fi.FullName.Replace(fi.Extension, ".xlsx");
                eWorkbook.SaveAs(outputfilepath, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                eWorkbook.Close(false, Type.Missing, Type.Missing); excelApp.Quit();
            }
            return outputfilepath;
        }
        public static string PPTToPPTX(string filepath)
        {
            string outputfilepath = string.Empty;
            FileInfo fi = new FileInfo(filepath);
            if (fi.Exists)
            { 
                MSPowerPoint.Application app = new MSPowerPoint.Application();
                object missing = Type.Missing;
                MSPowerPoint.Presentation pptx = app.Presentations.Open(fi.FullName, MsoTriState.msoTrue, MsoTriState.msoTrue, MsoTriState.msoTrue);
                outputfilepath = fi.FullName.Replace(fi.Extension, ".pptx");
                pptx.SaveAs(outputfilepath, MSPowerPoint.PpSaveAsFileType.ppSaveAsDefault);
                pptx.Close();
                app.Quit();
            }
            return outputfilepath;
        }
    }
}
