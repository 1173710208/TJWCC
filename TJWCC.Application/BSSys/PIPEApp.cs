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

    public class PIPEApp
    {
        private IPIPERepository service = new PIPERepository();

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public List<PIPEEntity> GetList()
        {
            return service.IQueryable().ToList();
        }
        public List<PIPEEntity> GetList(int[] foundPipes)
        {
            return service.IQueryable().Where(i=>foundPipes.Contains((int)i.ElementId)).ToList();
        }
        public List<PIPEEntity> GetList(int NodeID)
        {
            return service.IQueryable().Where(i => i.ElementId == NodeID && i.Is_Active == 1).ToList();
        }
        public List<PIPEEntity> GetStartList(int NodeID)
        {
            return service.IQueryable().Where(i => i.StartNodeID == NodeID).ToList();
        }
        public List<PIPEEntity> GetEndList(int NodeID)
        {
            return service.IQueryable().Where(i => i.EndNodeID == NodeID).ToList();
        }


        public string GetDownload(string keyword)
        {
            string sql = "SELECT Label '名称',Physical_Address '地址','DN'+ CONVERT(VARCHAR(10),CONVERT(INT,Physical_PipeDiameter)) '管径',District '所属公司',Material '管材' " +
                "FROM PIPE p,MaterialClass m,DistrictClass d WHERE p.Physical_DistrictID=d.ID AND p.Physical_PipeMaterialID=m.ID AND ElementId in(" + keyword + ")";
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
        public List<PIPEEntity> GetList(Pagination pagination, int id)
        {
            var expression = ExtLinq.True<PIPEEntity>();
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
        public void SubmitForm(PIPEEntity moduleEntity, string id)
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
