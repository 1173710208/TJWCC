using TJWCC.Code;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Repository.BSSys;
using TJWCC.Domain.IRepository.BSSys;
using TJWCC.Domain.Entity.BSSys;
using TJWCC.Data;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;
using DGSWDC.Code;

namespace TJWCC.Application.BSSys
{

    public class JUNCTIONApp
    {
        private IJUNCTIONRepository service = new JUNCTIONRepository();

        /// <summary>
        /// 获取SCADA
        /// </summary>
        /// <returns></returns>
        public List<JUNCTIONEntity> GetList()
        {
            return service.IQueryable().ToList();
        }
        public List<JUNCTIONEntity> GetList(int[] id)
        {
            return service.IQueryable().Where(i => id.Contains((int)i.ElementId)).ToList();
        }

        public string GetDownload(string keyword)
        {
            string sql = "SELECT MAX(Physical_PipeDiameter) Physical_PipeDiameter,StartNodeID NodeID INTO #startNodes FROM PIPE WHERE StartNodeID " +
                "in(" + keyword + ") GROUP BY StartNodeID;SELECT MAX(Physical_PipeDiameter) Physical_PipeDiameter,EndNodeID NodeID INTO #endNodes FROM PIPE " +
                "WHERE EndNodeID in(" + keyword + ") GROUP BY EndNodeID;SELECT Label '名称',Physical_Address '地址','DN'+ CONVERT(VARCHAR(10),CONVERT(INT,Physical_PipeDiameter)) '管径'," +
                "District '所属公司',Physical_Elevation '高程' FROM JUNCTION J,DistrictClass d ,(SELECT MAX(Physical_PipeDiameter) Physical_PipeDiameter,NodeID FROM " +
                "(SELECT * FROM #startNodes UNION ALL SELECT * FROM #endNodes) a GROUP BY NodeID) x WHERE j.Physical_DistrictID=d.ID AND j.ElementId=x.NodeID AND ElementId in(" + keyword + ")";
            TJWCCDbContext dbcontext = new TJWCCDbContext();
            DbCommand cmd = dbcontext.Database.Connection.CreateCommand();
            cmd.CommandTimeout = int.MaxValue;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;
            //转成sqlcommand正常查询语句。
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd as SqlCommand;
            DataTable tmp1 = new DataTable();
            da.Fill(tmp1);
            var excel = new ExcelHelper
            {
                FilePath = "/file/新增监测点" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx",
                SheetNames = new string[] { "新增监测点" },
                Titles = new string[] { "新增监测点" }
            };
            excel.AddTables(tmp1);
            excel.CreateExcel();
            return excel.FilePath;

        }
        /// <summary>
        /// 分页查询SCADA数据记录
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<JUNCTIONEntity> GetList(Pagination pagination, int id)
        {
            var expression = ExtLinq.True<JUNCTIONEntity>();
            if (!string.IsNullOrEmpty(id.ToString()))
            {
                expression = expression.And(t => t.OBJECTID.Equals(id));
            }
            return service.FindList(expression, pagination).ToList();

        }


        /// <summary>
        /// 根据id删除SCADA数据记录
        /// </summary>
        /// <param name="keyValue"></param>
        public void DeleteForm(int id)
        {
            service.Delete(t => t.OBJECTID == id);
        }

        /// <summary>
        /// 添加或更新用户
        /// </summary>
        /// <param name="moduleEntity">数据实体信息</param>
        /// <param name="userID">用户编号</param>
        public void SubmitForm(JUNCTIONEntity moduleEntity, string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                service.Update(moduleEntity);
            }
            else
            {
                service.Insert(moduleEntity);
            }
        }

        public string GetSingleFeeRecord(string id)
        {
            decimal tmp = 0;
            try
            {
                tmp = Convert.ToDecimal(id);
            }
            catch { }
            var result = service.FindEntity(t => t.OBJECTID == tmp);
            return JsonConvert.SerializeObject(result);
        }
    }
}
