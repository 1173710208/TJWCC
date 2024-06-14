using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Code.MigraDoc
{
    public class Tables
    {
        public static void DemonstrateSimpleTable(Document document, double[] with, DataTable items)
        {
            var table = new Table();
            table.Borders.Width = 0.75;

            var column = table.AddColumn(Unit.FromCentimeter(with[0]));
            column.Format.Alignment = ParagraphAlignment.Center;

            table.AddColumn(Unit.FromCentimeter(with[1]));
            for (int i = 2; i < items.Columns.Count; i++)
            {
                table.AddColumn(Unit.FromCentimeter(with[2]));
            }

            var row = table.AddRow();
            row.Shading.Color = Colors.AliceBlue;
            var cell = new Cell();

            for (int i = 0; i < items.Columns.Count; i++)
            {
                cell = row.Cells[i];
                cell.AddParagraph(items.Columns[i].ColumnName);
            }

            for (int i = 0; i < items.Rows.Count; i++)
            {
                row = table.AddRow();
                for (int j = 0; j < items.Columns.Count; j++)
                {
                    cell = row.Cells[j];
                    cell.AddParagraph(items.Rows[i][j].ToString());
                }
            }

            //table.SetEdge(0, 0, 2, 3, Edge.Box, BorderStyle.Single, 1.5, Colors.Black);

            document.LastSection.Add(table);
        }

        public static void DemonstrateSimpleTable(Document document, double[] with, string[] itemNames, JArray items)
        {

            var table = new Table();
            table.Borders.Width = 0.75;

            var column = table.AddColumn(Unit.FromCentimeter(with[0]));
            column.Format.Alignment = ParagraphAlignment.Center;
            if (itemNames.Length > 1)
                table.AddColumn(Unit.FromCentimeter(with[1]));
            for (int i = 2; i < itemNames.Length; i++)
            {
                if (i < with.Length)
                    table.AddColumn(Unit.FromCentimeter(with[i]));
                else
                    table.AddColumn(Unit.FromCentimeter(with[with.Length - 1]));
            }

            var row = table.AddRow();
            row.Shading.Color = Colors.AliceBlue;
            var cell = new Cell();

            for (int i = 0; i < itemNames.Length; i++)
            {
                cell = row.Cells[i];
                cell.AddParagraph(itemNames[i]);
            }

            foreach (JArray item in items)
            {
                row = table.AddRow();
                for (int i = 0; i < itemNames.Length; i++)
                {
                    if (i < item.Count)
                    {
                        cell = row.Cells[i];
                        cell.AddParagraph(item[i].ToString());
                    }
                }
            }

            //table.SetEdge(0, 0, 2, 3, Edge.Box, BorderStyle.Single, 1.5, Colors.Black);

            document.LastSection.Add(table);
        }

    }
}
