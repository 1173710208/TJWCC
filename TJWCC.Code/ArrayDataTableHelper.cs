using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;

namespace TJWCC.Code
{
    /// <summary>
    ///ArrayDataTable 的摘要说明
    /// </summary>
    public class ArrayDataTableHelper
    {
        public static int PressUnitConv = 10;//单位换算
        public static string PressUnit = "公斤";
        public static string mPressUnit = "公斤";
        public static string ClUnit = "mg/L";
        public static string BgaUnit = "个/ml";
        public static string TurbUnit = "NTU";
        public static string FlowUnit = "m³/h";
        public static string TceUnit = "kw";
        /// <summary>  
        /// 把一个一维数组转换为DataTable  
        /// </summary>  
        /// <param name="ColumnName">列名</param>  
        /// <param name="Array">一维数组</param>  
        /// <returns>返回DataTable</returns>  
        public DataTable ArrayToTable(String ColumnName, Double[] Array)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(ColumnName, typeof(String));

            for (int i = 0; i < Array.Length; i++)
            {
                DataRow dr = dt.NewRow();
                dr[ColumnName] = Array[i].ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        /// <summary>  
        /// 把一个一维数组转换为DataTable  
        /// </summary>  
        /// <param name="ColumnName">列名</param>  
        /// <param name="Array">一维数组</param>  
        /// <returns>返回DataTable</returns>  
        public DataTable ArrayToTable(Double[] Array)
        {
            DataTable dt = new DataTable();

            for (int i = 0; i < Array.Length; i++)
            {
                DataRow dr = dt.NewRow();
                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>  
        /// 把一个DataTable转换为小数二维数组  
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns>返回二维数组</returns>
        public Double[,] TableToDouArray(DataTable dt)
        {
            Double[,] arrayA = new Double[dt.Rows.Count, dt.Columns.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    DataRow dr2 = dt.Rows[i];
                    if (!(dr2[j] is DBNull))
                        arrayA[i, j] = Convert.ToDouble(dr2[j]);
                }
            }
            return arrayA;
        }

        /// <summary>  
        /// 把一个DataTable转换为字符串二维数组  
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns>返回二维数组</returns>
        public String[,] TableToStrArray(DataTable dt)
        {
            String[,] arrayA = new String[dt.Rows.Count, dt.Columns.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    DataRow dr2 = dt.Rows[i];
                    if (!(dr2[j] is DBNull))
                        arrayA[i, j] = Convert.ToString(dr2[j]);
                }
            }
            return arrayA;
        }

        /// <summary>  
        /// 一个M行N列的二维数组转换为DataTable  
        /// </summary>  
        /// <param name="ColumnNames">一维数组，代表列名，不能有重复值</param>  
        /// <param name="Arrays">M行N列的二维数组</param>  
        /// <returns>返回DataTable</returns>  
        public DataTable ArrayToTable(String[] ColumnNames, Double[,] Arrays)
        {
            DataTable dt = new DataTable();

            foreach (String ColumnName in ColumnNames)
            {
                dt.Columns.Add(ColumnName, typeof(String));
            }

            for (int i1 = 0; i1 < Arrays.GetLength(0); i1++)
            {
                DataRow dr = dt.NewRow();
                for (int i = 0; i < ColumnNames.Length; i++)
                {
                    dr[i] = Arrays[i1, i];
                }
                dt.Rows.Add(dr);
            }

            return dt;

        }


        /// <summary>  
        /// 一个M行N列的二维数组转换为DataTable  
        /// </summary>  
        /// <param name="Arrays">M行N列的二维数组</param>  
        /// <returns>返回DataTable</returns>  
        public DataTable ArrayToTable(Double[,] Arrays, String name)
        {
            DataTable dt = new DataTable();
            dt.TableName = name;
            int a = Arrays.GetLength(0);
            for (int i = 0; i < Arrays.GetLength(1); i++)
            {
                dt.Columns.Add("col" + i.ToString(), typeof(String));
            }

            for (int i1 = 0; i1 < Arrays.GetLength(0); i1++)
            {
                DataRow dr = dt.NewRow();
                for (int i = 0; i < Arrays.GetLength(1); i++)
                {
                    dr[i] = Arrays[i1, i];
                }
                dt.Rows.Add(dr);
            }

            return dt;

        }

        /// <summary>
        /// 将两个列不同的DataTable合并成一个新的DataTable
        /// </summary>
        /// <param name="dt1">表1</param>
        /// <param name="dt2">表2</param>
        /// <param name="DTName">合并后新的表名</param>
        /// <returns></returns>
        public DataTable UniteDataTable(DataTable dt1, DataTable dt2, string DTName)
        {
            DataTable dt3 = dt1.Clone();
            for (int i = 0; i < dt2.Columns.Count; i++)
            {
                dt3.Columns.Add(dt2.Columns[i].ColumnName);
            }
            object[] obj = new object[dt3.Columns.Count];

            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                dt1.Rows[i].ItemArray.CopyTo(obj, 0);
                if (!(dt2.Rows[i][0] is DBNull))
                    obj[dt3.Columns.Count - 1] = dt2.Rows[i][0];
                dt3.Rows.Add(obj);
            }

            dt3.TableName = DTName; //设置DT的名字
            return dt3;
        }
    }
}