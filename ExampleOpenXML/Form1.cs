
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ExampleOpenXML
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            cmbSheets.Items.Clear();
            cmbSheets.Items.Add(ComboboxItem.DefaultItem);
            cmbSheets.SelectedIndex =0;
        }
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtfilePath.Text = ofd.FileName;
                //Save the uploaded Excel file.
                string filePath = txtfilePath.Text;
                DataSet ds = Common.OleDBParse(filePath);//SpreadsheetDocument.Open(ofd.FileName, false).GetDataTableFromSpreadSheet();
                // Get the DataTableCollection through the Tables property.
                DataTableCollection tables = ds.Tables;
                cmbSheets.Items.Clear();
                cmbSheets.Items.Add(ComboboxItem.DefaultItem);
                // Get the index of the table named "Authors", if it exists.
                foreach (DataTable dt in tables)
                {
                    cmbSheets.Items.Add(new ComboboxItem() { Text = dt.TableName, Value = tables.IndexOf(dt.TableName) });
                }
                cmbSheets.SelectedIndex = 0;
                dgvData.Tag = ds;  
            }
        }
        private void cmbSheets_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindDataToGrid();

        }
        private void BindDataToGrid()
        {
            ComboboxItem cbi=(cmbSheets.SelectedItem as ComboboxItem);
            DataSet ds = (DataSet)dgvData.Tag;
            if(cbi!=null && Convert.ToInt32( cbi.Value)>-1)
            dgvData.DataSource = ds.Tables[Convert.ToInt32( cbi.Value) ];

        }

       


        //private string GetValue(SpreadsheetDocument doc, Cell cell)
        //{
        //    string value = cell.CellValue==null? null: cell.CellValue.InnerText;
        //    if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
        //    {
        //        return doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
        //    }
        //    return value;
        //}
    }
}
