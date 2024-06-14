using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System.Data;
using System.IO;
using System.Web;

namespace DGSWDC.Code.Excel
{
    public class NPOIExcel
    {
        public string MapPathFile(string FileName)
        {
            return HttpContext.Current.Server.MapPath(FileName);
        }

        /// <summary>
        /// 导出到Excel
        /// </summary>
        /// <param name="table"></param>
        /// <param name="title"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public bool ToExcel(DataTable[] table, string[] title, string[] sheetName, string filePath)
        {
            FileStream fs = new FileStream(MapPathFile(filePath), FileMode.OpenOrCreate, FileAccess.ReadWrite);
            IWorkbook workBook = new XSSFWorkbook();
            for (int t = 0; t < table.Length; t++)
            {
                sheetName[t] = string.IsNullOrEmpty(sheetName[t]) ? "sheet1" : sheetName[t];
                ISheet sheet = workBook.CreateSheet(sheetName[t]);

                //处理表格标题
                IRow row = sheet.CreateRow(0);
                row.CreateCell(0).SetCellValue(title[t]);
                if (table[t].Columns.Count > 0)
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, table[t].Columns.Count - 1));
                else
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 0));
                row.Height = 500;

                ICellStyle cellStyle = workBook.CreateCellStyle();
                IFont font = workBook.CreateFont();
                font.FontName = "微软雅黑";
                font.FontHeightInPoints = 17;
                cellStyle.SetFont(font);
                cellStyle.VerticalAlignment = VerticalAlignment.Center;
                cellStyle.Alignment = HorizontalAlignment.Center;
                row.Cells[0].CellStyle = cellStyle;

                //处理表格列头
                row = sheet.CreateRow(1);
                for (int i = 0; i < table[t].Columns.Count; i++)
                {
                    row.CreateCell(i).SetCellValue(table[t].Columns[i].ColumnName);
                    row.Height = 350;
                    sheet.AutoSizeColumn(i);
                }

                //处理数据内容
                for (int i = 0; i < table[t].Rows.Count; i++)
                {
                    row = sheet.CreateRow(2 + i);
                    row.Height = 250;
                    for (int j = 0; j < table[t].Columns.Count; j++)
                    {
                        row.CreateCell(j).SetCellValue(table[t].Rows[i][j].ToString());
                        sheet.SetColumnWidth(j, 256 * 15);
                    }
                }
            }
            //写入数据流
            workBook.Write(fs);
            //fs.Flush();
            fs.Close();

            return true;
        }
        public bool ToStatistics(DataTable[] table, string[] title, string[] sheetName, string filePath)
        {
            FileStream fs = new FileStream(MapPathFile(filePath), FileMode.OpenOrCreate, FileAccess.ReadWrite);
            IWorkbook workBook = new XSSFWorkbook();
            for (int t = 0; t < table.Length; t++)
            {
                sheetName[t] = string.IsNullOrEmpty(sheetName[t]) ? "sheet1" : sheetName[t];
                ISheet sheet = workBook.CreateSheet(sheetName[t]);

                //处理表格标题
                IRow row = sheet.CreateRow(0);
                row.CreateCell(0).SetCellValue(title[t]);
                if (table[t].Columns.Count > 0)
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, table[t].Columns.Count - 1));
                else
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 0));
                row.Height = 500;

                ICellStyle cellStyle = workBook.CreateCellStyle();
                IFont font = workBook.CreateFont();
                font.FontName = "微软雅黑";
                font.FontHeightInPoints = 17;
                cellStyle.SetFont(font);
                cellStyle.VerticalAlignment = VerticalAlignment.Center;
                cellStyle.Alignment = HorizontalAlignment.Center;
                row.Cells[0].CellStyle = cellStyle;

                cellStyle = workBook.CreateCellStyle();
                font = workBook.CreateFont();
                font.FontName = "微软雅黑";
                font.FontHeightInPoints = 12;
                cellStyle.SetFont(font);
                cellStyle.VerticalAlignment = VerticalAlignment.Center;
                cellStyle.Alignment = HorizontalAlignment.Center;
                row = sheet.CreateRow(1);
                row.CreateCell(0).SetCellValue("日期");
                sheet.AddMergedRegion(new CellRangeAddress(1, 2, 0, 0));
                row.Cells[0].CellStyle = cellStyle;
                row.CreateCell(1).SetCellValue("供水量(m³)");
                sheet.AddMergedRegion(new CellRangeAddress(1, 1, 1, 11));
                row.Cells[1].CellStyle = cellStyle;
                row.CreateCell(12).SetCellValue("总计");
                sheet.AddMergedRegion(new CellRangeAddress(1, 2, 12, 12));
                row.Cells[2].CellStyle = cellStyle;
                row.CreateCell(13).SetCellValue("平均出厂压力(MPa)");
                sheet.AddMergedRegion(new CellRangeAddress(1, 1, 13, 21));
                row.Cells[3].CellStyle = cellStyle;
                row.CreateCell(22).SetCellValue("大港水务(m³)");
                sheet.AddMergedRegion(new CellRangeAddress(1, 1, 22, 24));
                row.Cells[4].CellStyle = cellStyle;
                row.CreateCell(25).SetCellValue("大港水务(m³)");
                sheet.AddMergedRegion(new CellRangeAddress(1, 1, 25, 25));
                row.Cells[5].CellStyle = cellStyle;
                row = sheet.CreateRow(2);
                row.RowStyle = cellStyle;
                row.Height = 350;
                row.CreateCell(1).SetCellValue("芥园");
                row.CreateCell(2).SetCellValue("凌庄");
                row.CreateCell(3).SetCellValue("新开河");
                row.CreateCell(4).SetCellValue("津滨");
                row.CreateCell(5).SetCellValue("市区合计");
                row.CreateCell(6).SetCellValue("新村");
                row.CreateCell(7).SetCellValue("新河");
                row.CreateCell(8).SetCellValue("新区");
                row.CreateCell(9).SetCellValue("大港油田");
                row.CreateCell(10).SetCellValue("津港水厂");
                row.CreateCell(11).SetCellValue("滨海水务合计");
                row.CreateCell(13).SetCellValue("芥园");
                row.CreateCell(14).SetCellValue("凌庄");
                row.CreateCell(15).SetCellValue("新开河");
                row.CreateCell(16).SetCellValue("津滨");
                row.CreateCell(17).SetCellValue("新村");
                row.CreateCell(18).SetCellValue("新河");
                row.CreateCell(19).SetCellValue("新区");
                row.CreateCell(20).SetCellValue("滨海");
                row.CreateCell(21).SetCellValue("津港");
                row.CreateCell(22).SetCellValue("津滨来水");
                row.CreateCell(23).SetCellValue("大港供水站(滦)");
                row.CreateCell(24).SetCellValue("小计");
                row.CreateCell(25).SetCellValue("进水量");
                //处理表格列头
                //row = sheet.CreateRow(3);
                //for (int i = 0; i < table[t].Columns.Count; i++)
                //{
                //    row.CreateCell(i).SetCellValue(table[t].Columns[i].ColumnName);
                //    row.Height = 350;
                //    sheet.AutoSizeColumn(i);
                //}

                //处理数据内容
                for (int i = 0; i < table[t].Rows.Count; i++)
                {
                    row = sheet.CreateRow(3 + i);
                    row.Height = 250;
                    for (int j = 0; j < table[t].Columns.Count; j++)
                    {
                        row.CreateCell(j).SetCellValue(table[t].Rows[i][j].ToString());
                        sheet.SetColumnWidth(j, 256 * 15);
                    }
                }
            }
            //写入数据流
            workBook.Write(fs);
            //fs.Flush();
            fs.Close();

            return true;
        }
        public bool ToAnalyse(DataTable[] table, string[] title, string[] sheetName, string filePath)
        {
            FileStream fs = new FileStream(MapPathFile(filePath), FileMode.OpenOrCreate, FileAccess.ReadWrite);
            IWorkbook workBook = new XSSFWorkbook();
            for (int t = 0; t < table.Length; t++)
            {
                sheetName[t] = string.IsNullOrEmpty(sheetName[t]) ? "sheet1" : sheetName[t];
                ISheet sheet = workBook.CreateSheet(sheetName[t]);

                //处理表格标题
                IRow row = sheet.CreateRow(0);
                row.CreateCell(0).SetCellValue(title[t]);
                if (table[t].Columns.Count > 0)
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, table[t].Columns.Count - 1));
                else
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 0));
                row.Height = 500;

                ICellStyle cellStyle = workBook.CreateCellStyle();
                IFont font = workBook.CreateFont();
                font.FontName = "微软雅黑";
                font.FontHeightInPoints = 17;
                cellStyle.SetFont(font);
                cellStyle.VerticalAlignment = VerticalAlignment.Center;
                cellStyle.Alignment = HorizontalAlignment.Center;
                row.Cells[0].CellStyle = cellStyle;

                cellStyle = workBook.CreateCellStyle();
                font = workBook.CreateFont();
                font.FontName = "微软雅黑";
                font.FontHeightInPoints = 12;
                cellStyle.SetFont(font);
                cellStyle.VerticalAlignment = VerticalAlignment.Center;
                cellStyle.Alignment = HorizontalAlignment.Center;
                row = sheet.CreateRow(1);
                row.CreateCell(0).SetCellValue("日期");
                sheet.AddMergedRegion(new CellRangeAddress(1, 2, 0, 0));
                row.Cells[0].CellStyle = cellStyle;
                row.CreateCell(1).SetCellValue("芥园(m³)");
                sheet.AddMergedRegion(new CellRangeAddress(1, 1, 1, 3));
                row.Cells[1].CellStyle = cellStyle;
                row.CreateCell(4).SetCellValue("凌庄(m³)");
                sheet.AddMergedRegion(new CellRangeAddress(1, 1, 4, 6));
                row.Cells[2].CellStyle = cellStyle;
                row.CreateCell(7).SetCellValue("新开河(m³)");
                sheet.AddMergedRegion(new CellRangeAddress(1, 1, 7, 9));
                row.Cells[3].CellStyle = cellStyle;
                row.CreateCell(10).SetCellValue("津滨(m³)");
                sheet.AddMergedRegion(new CellRangeAddress(1, 1, 10, 12));
                row.Cells[4].CellStyle = cellStyle;
                row.CreateCell(13).SetCellValue("新村(m³)");
                sheet.AddMergedRegion(new CellRangeAddress(1, 1, 13, 15));
                row.Cells[5].CellStyle = cellStyle;
                row.CreateCell(16).SetCellValue("新河(m³)");
                sheet.AddMergedRegion(new CellRangeAddress(1, 1, 16, 18));
                row.Cells[6].CellStyle = cellStyle;
                row.CreateCell(19).SetCellValue("新区(m³)");
                sheet.AddMergedRegion(new CellRangeAddress(1, 1, 19, 21));
                row.Cells[7].CellStyle = cellStyle;
                row.CreateCell(22).SetCellValue("大港油田(m³)");
                sheet.AddMergedRegion(new CellRangeAddress(1, 1, 22, 24));
                row.Cells[8].CellStyle = cellStyle;
                row.CreateCell(25).SetCellValue("津港水厂(m³)");
                sheet.AddMergedRegion(new CellRangeAddress(1, 1, 25, 27));
                row.Cells[9].CellStyle = cellStyle;
                row.CreateCell(28).SetCellValue("市区四水厂(m³)");
                sheet.AddMergedRegion(new CellRangeAddress(1, 1, 28, 30));
                row.Cells[10].CellStyle = cellStyle;
                row.CreateCell(31).SetCellValue("滨海五水厂(m³)");
                sheet.AddMergedRegion(new CellRangeAddress(1, 1, 31, 33));
                row.Cells[11].CellStyle = cellStyle;
                row.CreateCell(34).SetCellValue("九大水厂(m³)");
                sheet.AddMergedRegion(new CellRangeAddress(1, 1, 34, 36));
                row.Cells[12].CellStyle = cellStyle;
                row = sheet.CreateRow(2);
                row.RowStyle = cellStyle;
                row.Height = 350;
                row.CreateCell(1).SetCellValue("同比");
                row.CreateCell(2).SetCellValue("水量");
                row.CreateCell(3).SetCellValue("变化");
                row.CreateCell(4).SetCellValue("同比");
                row.CreateCell(5).SetCellValue("水量");
                row.CreateCell(6).SetCellValue("变化");
                row.CreateCell(7).SetCellValue("同比");
                row.CreateCell(8).SetCellValue("水量");
                row.CreateCell(9).SetCellValue("变化");
                row.CreateCell(10).SetCellValue("同比");
                row.CreateCell(11).SetCellValue("水量");
                row.CreateCell(12).SetCellValue("变化");
                row.CreateCell(13).SetCellValue("同比");
                row.CreateCell(14).SetCellValue("水量");
                row.CreateCell(15).SetCellValue("变化");
                row.CreateCell(16).SetCellValue("同比");
                row.CreateCell(17).SetCellValue("水量");
                row.CreateCell(18).SetCellValue("变化");
                row.CreateCell(19).SetCellValue("同比");
                row.CreateCell(20).SetCellValue("水量");
                row.CreateCell(21).SetCellValue("变化");
                row.CreateCell(22).SetCellValue("同比");
                row.CreateCell(23).SetCellValue("水量");
                row.CreateCell(24).SetCellValue("变化");
                row.CreateCell(25).SetCellValue("同比");
                row.CreateCell(26).SetCellValue("水量");
                row.CreateCell(27).SetCellValue("变化");
                row.CreateCell(28).SetCellValue("同比");
                row.CreateCell(29).SetCellValue("水量");
                row.CreateCell(30).SetCellValue("变化");
                row.CreateCell(31).SetCellValue("同比");
                row.CreateCell(32).SetCellValue("水量");
                row.CreateCell(33).SetCellValue("变化");
                row.CreateCell(34).SetCellValue("同比");
                row.CreateCell(35).SetCellValue("水量");
                row.CreateCell(36).SetCellValue("变化");
                //处理表格列头
                //row = sheet.CreateRow(1);
                //for (int i = 0; i < table[t].Columns.Count; i++)
                //{
                //    row.CreateCell(i).SetCellValue(table[t].Columns[i].ColumnName);
                //    row.Height = 350;
                //    sheet.AutoSizeColumn(i);
                //}

                //处理数据内容
                for (int i = 0; i < table[t].Rows.Count; i++)
                {
                    row = sheet.CreateRow(3 + i);
                    row.Height = 250;
                    for (int j = 0; j < table[t].Columns.Count; j++)
                    {
                        row.CreateCell(j).SetCellValue(table[t].Rows[i][j].ToString());
                        sheet.SetColumnWidth(j, 256 * 15);
                    }
                }
            }
            //写入数据流
            workBook.Write(fs);
            //fs.Flush();
            fs.Close();

            return true;
        }
    }
}
