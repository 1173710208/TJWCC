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

namespace TJWCC.Application.BSSys
{

    public class BSO_Station_CurrentApp
    {
        private IBSO_Station_CurrentRepository service = new BSO_Station_CurrentRepository();
        TJWCCDbContext dbcontext = new TJWCCDbContext();

        /// <summary>
        /// 获取SCADA
        /// </summary>
        /// <returns></returns>
        public List<BSO_Station_CurrentEntity> GetList()
        {
            return service.IQueryable().ToList();
        }

        /// <summary>
        /// 分页查询SCADA数据记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<BSO_Station_CurrentEntity> GetList(int? id)
        {
            var expression = ExtLinq.True<BSO_Station_CurrentEntity>();
            if (id != null)
            {
                expression = expression.And(t => t.StationId == id);
            }
            return service.IQueryable(expression).ToList();

        }
        public decimal GetPress(int? id)
        {
            string sql = "SELECT AVG(NewDate) Press from [dbo].[BSO_Station_Current] unpivot (NewDate for DateVal in (Pressure1,Pressure2,Pressure3,Pressure4,Pressure5,Pressure6)) as u " +
                "WHERE NewDate>0 AND NewDate IS NOT NULL AND StationId =" + id.Value ;
            var press = dbcontext.Database.SqlQuery<decimal>(sql).FirstOrDefault();
            return press;

        }

        public decimal GetFlow(int? id)
        {
            string sql = "SELECT (CASE WHEN Flow1 IS NULL THEN 0 ELSE Flow1 END)+(CASE WHEN Flow2 IS NULL THEN 0 ELSE Flow2 END)+(CASE WHEN Flow3 IS NULL THEN 0 ELSE Flow3 END)+" +
                "(CASE WHEN Flow4 IS NULL THEN 0 ELSE Flow4 END)+(CASE WHEN Flow5 IS NULL THEN 0 ELSE Flow5 END)+(CASE WHEN Flow6 IS NULL THEN 0 ELSE Flow6 END) Flow " +
                "FROM BSO_Station_Current WHERE StationId =" + id.Value ;
            var flow = dbcontext.Database.SqlQuery<decimal>(sql).FirstOrDefault();
            return flow;

        }

        /// <summary>
        /// 根据id删除SCADA数据记录
        /// </summary>
        /// <param name="keyValue"></param>
        public void DeleteForm(int id)
        {
            service.Delete(t => t.ID == id);
        }

        /// <summary>
        /// 添加或更新用户
        /// </summary>
        /// <param name="moduleEntity">数据实体信息</param>
        /// <param name="userID">用户编号</param>
        public void SubmitForm(BSO_Station_CurrentEntity moduleEntity, string id)
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
            var result = service.FindEntity(t => t.ID == tmp);
            return JsonConvert.SerializeObject(result);
        }
    }
}
