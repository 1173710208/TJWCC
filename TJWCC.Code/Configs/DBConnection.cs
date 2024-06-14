using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Code
{
    public class DBConnection<T> where T : class, new()
    {
        public static bool Encrypt { get; set; }
        public DBConnection(bool encrypt)
        {
            Encrypt = encrypt;
        }
        public static string connectionString
        {
            get
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["TJWCCDbContext"].ConnectionString;
                if (Encrypt == true)
                {
                    return DESEncrypt.Decrypt(connection);
                }
                else
                {
                    return connection;
                }
            }
        }

        public static DataSet Query(string SQLString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter(SQLString, connection);
                    da.Fill(ds, "tablename");
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }

        #region DataTable转换成实体类

        /// <summary>
        /// 填充对象列表：用DataSet的第一个表填充实体类
        /// </summary>
        /// <param name="ds">DataSet</param>
        /// <returns></returns>
        //public static List<T> FillModel(DataSet ds)
        //{
        //    if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        return FillModel(ds.Tables[0]);
        //    }
        //}


        /// <summary>  
        /// 填充对象列表：用DataSet的第index个表填充实体类
        /// </summary>  
        public static List<T> FillModel(DataSet ds, int index)
        {
            if (ds == null || ds.Tables.Count <= index || ds.Tables[index].Rows.Count == 0)
            {
                return null;
            }
            else
            {
                return FillModel(ds.Tables[index]);
            }
        }


        /// <summary>  
        /// 填充对象列表：用DataTable填充实体类
        /// </summary>  
        public static List<T> FillModel(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            List<T> modelList = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                //T model = (T)Activator.CreateInstance(typeof(T));  
                T model = new T();
                for (int i = 0; i < dr.Table.Columns.Count; i++)
                {
                    PropertyInfo propertyInfo = model.GetType().GetProperty(dr.Table.Columns[i].ColumnName);
                    if (propertyInfo != null && dr[i] != DBNull.Value)
                        propertyInfo.SetValue(model, dr[i], null);
                }


                modelList.Add(model);
            }
            return modelList;
        }


            /// <summary>  
            /// 填充对象：用DataRow填充实体类
            /// </summary>  
            public T FillModel(DataRow dr)
            {
                if (dr == null)
                {
                    return default(T);
                }


                //T model = (T)Activator.CreateInstance(typeof(T));  
                T model = new T();


                for (int i = 0; i < dr.Table.Columns.Count; i++)
                {
                    PropertyInfo propertyInfo = model.GetType().GetProperty(dr.Table.Columns[i].ColumnName);
                    if (propertyInfo != null && dr[i] != DBNull.Value)
                        propertyInfo.SetValue(model, dr[i], null);
                }
                return model;
            }


            #endregion


            #region 实体类转换成DataTable


            /// <summary>
            /// 实体类转换成DataSet
            /// </summary>
            /// <param name="modelList">实体类列表</param>
            /// <returns></returns>
            public DataSet FillDataSet(List<T> modelList)
            {
                if (modelList == null || modelList.Count == 0)
                {
                    return null;
                }
                else
                {
                    DataSet ds = new DataSet();
                    ds.Tables.Add(FillDataTable(modelList));
                    return ds;
                }
            }


            /// <summary>
            /// 实体类转换成DataTable
            /// </summary>
            /// <param name="modelList">实体类列表</param>
            /// <returns></returns>
            public DataTable FillDataTable(List<T> modelList)
            {
                if (modelList == null || modelList.Count == 0)
                {
                    return null;
                }
                DataTable dt = CreateData(modelList[0]);


                foreach (T model in modelList)
                {
                    DataRow dataRow = dt.NewRow();
                    foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
                    {
                        dataRow[propertyInfo.Name] = propertyInfo.GetValue(model, null);
                    }
                    dt.Rows.Add(dataRow);
                }
                return dt;
            }


            /// <summary>
            /// 根据实体类得到表结构
            /// </summary>
            /// <param name="model">实体类</param>
            /// <returns></returns>
            private DataTable CreateData(T model)
            {
                DataTable dataTable = new DataTable(typeof(T).Name);
                foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
                {
                    dataTable.Columns.Add(new DataColumn(propertyInfo.Name, propertyInfo.PropertyType));
                }
                return dataTable;
            }


        #endregion
    }
}
