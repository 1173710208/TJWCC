using DGSWDC.Code.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGSWDC.Code
{
    public class ExcelHelper
    {
        private List<DataTable> tables;
        private string[] titles;
        private string[] sheetNames;
        public string FilePath { get; set; }
        public string[] Titles
        {
            get
            {
                if (titles == null) { titles = new string[] { }; }
                return titles;
            }
            set { titles = value; }
        }
        public string[] SheetNames
        {
            get
            {
                if (sheetNames == null) { sheetNames = new string[] { }; }
                return sheetNames;
            }
            set { sheetNames = value; }
        }
        public List<DataTable> Tables
        {
            get
            {
                if (tables == null) { tables = new List<DataTable>(); }
                return tables;
            }
            set { tables = value; }
        }
        public void AddTables(DataTable table)
        {
            if (tables == null) { tables = new List<DataTable>(); }
            tables.Add(table);
        }
        public void AddTables<T>(List<T> table)
        {
            if (tables == null) { tables = new List<DataTable>(); }
            tables.Add(new DataTableHelper().ToDataTable(table));
        }
        public void CreateExcel()
        {
            new NPOIExcel().ToExcel(tables.ToArray(), titles, sheetNames, FilePath);
        }
        public void CreateStatistics()
        {
            new NPOIExcel().ToStatistics(tables.ToArray(), titles, sheetNames, FilePath);
        }
        public void CreateAnalyse()
        {
            new NPOIExcel().ToAnalyse(tables.ToArray(), titles, sheetNames, FilePath);
        }
    }
}
