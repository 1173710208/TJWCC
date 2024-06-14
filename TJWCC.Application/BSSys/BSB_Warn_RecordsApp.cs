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
using System.Linq.Expressions;
using TJWCC.Data;
using System.Text.RegularExpressions;

namespace TJWCC.Application.BSSys
{

    public class BSB_Warn_RecordsApp
    {
        TJWCCDbContext dbcontext = new TJWCCDbContext();
        private IBSB_Warn_RecordsRepository service = new BSB_Warn_RecordsRepository();
        private BS_SCADA_TAG_CURRENTApp bstcapp = new BS_SCADA_TAG_CURRENTApp();

        /// <summary>
        /// 获取SCADA
        /// </summary>
        /// <returns></returns>
        public List<BSB_Warn_RecordsEntity> GetList()
        {
            return service.IQueryable().ToList();
        }

        public List<BSB_Warn_RecordsEntity> GetSuddLow(int? aId)
        {
            var expression = ExtLinq.True<BSB_Warn_RecordsEntity>();
            expression = expression.And(i => i.WarnType == 1);
            if (aId != null) expression = expression.And(i => i.id == aId);
            return service.IQueryable(expression).ToList();
        }
        /// <summary>
        /// 分页查询SCADA数据记录
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<BSB_Warn_RecordsEntity> GetList(Pagination pagination, string keyword, DateTime? sDate, DateTime? eDate)
        {
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            if (string.IsNullOrWhiteSpace(keyword))
            {
                var tmpdt = service.IQueryable().Max(item => item.StartTime);
                if (tmpdt != null)
                {
                    dt = tmpdt.Value.AddHours(1);
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
                }
                if (sDate == null)
                {
                    sDate = dt.AddYears(-1);
                }
                if (eDate == null)
                {
                    eDate = dt;
                }
            }
            else
            {
                string tkey = keyword.Split(',')[0].Split('_')[2].ToString();
                var tmpdt = service.IQueryable().Where(i => i.TagKey.Equals(tkey)).Max(item => item.StartTime);
                if (tmpdt != null)
                {
                    dt = tmpdt.Value.AddHours(1);
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
                }
                if (sDate == null)
                {
                    sDate = dt.AddYears(-1);
                }
                if (eDate == null)
                {
                    eDate = dt;
                }
            }
            string where = "WHERE StartTime>='" + sDate + "' AND StartTime<='" + eDate + "'";//流量计数据查询条件
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var skeys = keyword.Split(',');
                for (var i = 0; i < skeys.Length; i++)
                {
                    if (skeys[i].Length > 0)
                    {
                        var sskey = skeys[i].Split('_');
                        if (i == 0)
                        {
                            if (skeys.Length == 1)
                                where += " AND (StationKey ='" + sskey[1] + "' AND TagKey=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                            else
                                where += " AND ((StationKey ='" + sskey[1] + "' AND TagKey=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                        }
                        else if (i == skeys.Length - 1)
                        {
                            where += " OR (StationKey ='" + sskey[1] + "' AND TagKey=" + sskey[2] + " AND Number=" + sskey[3] + "))";
                        }
                        else
                        {
                            where += " OR (StationKey ='" + sskey[1] + "' AND TagKey=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                        }
                    }
                }
            }
            string sql = "SELECT * FROM BSB_Warn_Records " + where;
            bool isAsc = pagination.sord.ToLower() == "asc" ? true : false;
            string[] _order = pagination.sidx.Split(',');
            MethodCallExpression resultExp = null;
            dbcontext.Database.CommandTimeout = int.MaxValue;
            var tempData = dbcontext.Database.SqlQuery<BSB_Warn_RecordsEntity>(sql).AsQueryable();
            foreach (string item in _order)
            {
                string _orderPart = item;
                _orderPart = Regex.Replace(_orderPart, @"\s+", " ");
                string[] _orderArry = _orderPart.Split(' ');
                string _orderField = _orderArry[0];
                bool sort = isAsc;
                if (_orderArry.Length == 2)
                {
                    isAsc = _orderArry[1].ToUpper() == "ASC" ? true : false;
                }
                var parameter = Expression.Parameter(typeof(BSB_Warn_RecordsEntity), "t");
                var property = typeof(BSB_Warn_RecordsEntity).GetProperty(_orderField);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                resultExp = Expression.Call(typeof(Queryable), isAsc ? "OrderBy" : "OrderByDescending", new Type[] { typeof(BSB_Warn_RecordsEntity), property.PropertyType }, tempData.Expression, Expression.Quote(orderByExp));
            }
            tempData = tempData.Provider.CreateQuery<BSB_Warn_RecordsEntity>(resultExp);
            pagination.records = tempData.Count();
            tempData = tempData.Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).AsQueryable();
            return tempData.ToList();
        }

        /// <summary>
        /// 根据id删除SCADA数据记录
        /// </summary>
        /// <param name="keyValue"></param>
        public void DeleteForm(int id)
        {
            service.Delete(t => t.id == id);
        }

        /// <summary>
        /// 添加或更新用户
        /// </summary>
        /// <param name="moduleEntity">数据实体信息</param>
        /// <param name="userID">用户编号</param>
        public void SubmitForm(BSB_Warn_RecordsEntity moduleEntity, string id)
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
            var result = service.FindEntity(t => t.id == tmp);
            return JsonConvert.SerializeObject(result);
        }
    }
}
