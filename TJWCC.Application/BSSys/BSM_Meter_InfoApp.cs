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
using TJWCC.Domain.ViewModel;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace TJWCC.Application.BSSys
{

    public class BSM_Meter_InfoApp
    {
        private IBSM_Meter_InfoRepository service = new BSM_Meter_InfoRepository();
        TJWCCDbContext dbcontext = new TJWCCDbContext();

        /// <summary>
        /// 获取表具信息
        /// </summary>
        /// <returns></returns>
        public List<BSM_Meter_InfoEntity> GetList()
        {
            return service.IQueryable().ToList();
        }
        public List<BSM_Meter_InfoEntity> GetList(Pagination pagination, string keyword)
        {
            string where = "";
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var skeys = keyword.Split(',');
                for (var i = 0; i < skeys.Length; i++)
                {
                    var sskey = skeys[i].Split('_');
                    if (i == 0)
                    {
                        if (skeys.Length == 1)
                            where += " AND (Station_Key ='" + sskey[1] + "' AND Meter_Type=" + sskey[2] + " AND DistrictAreaId=" + sskey[3] + ")";
                        else
                            where += " AND ((Station_Key ='" + sskey[1] + "' AND Meter_Type=" + sskey[2] + " AND DistrictAreaId=" + sskey[3] + ")";
                    }
                    else if (i == skeys.Length - 1)
                    {
                        where += " OR (Station_Key ='" + sskey[1] + "' AND Meter_Type=" + sskey[2] + " AND DistrictAreaId=" + sskey[3] + "))";
                    }
                    else
                    {
                        where += " OR (Station_Key ='" + sskey[1] + "' AND Meter_Type=" + sskey[2] + " AND DistrictAreaId=" + sskey[3] + ")";
                    }
                }
            }
            string sql = "SELECT * FROM BSM_Meter_Info WHERE 1=1" + where;
            bool isAsc = pagination.sord.ToLower() == "asc" ? true : false;
            string[] _order = pagination.sidx.Split(',');
            MethodCallExpression resultExp = null;
            dbcontext.Database.CommandTimeout = int.MaxValue;
            var tempData = dbcontext.Database.SqlQuery<BSM_Meter_InfoEntity>(sql).AsQueryable();
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
                var parameter = Expression.Parameter(typeof(BSM_Meter_InfoEntity), "t");
                var property = typeof(BSM_Meter_InfoEntity).GetProperty(_orderField);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                resultExp = Expression.Call(typeof(Queryable), isAsc ? "OrderBy" : "OrderByDescending", new Type[] { typeof(BSM_Meter_InfoEntity), property.PropertyType }, tempData.Expression, Expression.Quote(orderByExp));
            }
            tempData = tempData.Provider.CreateQuery<BSM_Meter_InfoEntity>(resultExp);
            pagination.records = tempData.Count();
            tempData = tempData.Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).AsQueryable();
            return tempData.ToList();
        }
        public List<BSM_Meter_InfoEntity> GetList(string keyword)
        {
            var expression = ExtLinq.True<BSM_Meter_InfoEntity>();
            //expression = expression.And(i => i.Display == 1);
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "'" + keyword.Replace(",", "','") + "'";
                expression = expression.And(i => keyword.Contains("'" + i.Station_Key + "'"));
            }
            return service.IQueryable(expression).ToList();
        }


        public BSM_Meter_InfoEntity GetForm(long keyValue)
        {
            return service.FindEntity(keyValue);
        }
        /// <summary>
        /// 根据表具类型获取表具信息
        /// </summary>
        /// <returns></returns>
        public List<BSM_Meter_InfoEntity> GetListByMType(int type)
        {
            return service.IQueryable().Where(i => i.Meter_Type == type).ToList();
        }
        public List<GetPoint_Result> GetPointList(int type)
        {
            string sql = "SELECT a.Meter_Name,a.Station_Unit,a.Measure_Grade,b.Physical_Address,b.Physical_Elevation,a.ElementID FROM BSM_Meter_Info a,JUNCTION b WHERE a.ElementID=b.ElementID AND Meter_Type=" + type +
                " AND a.ElementID IS not NULL AND Display=1";
            return dbcontext.Database.SqlQuery<GetPoint_Result>(sql).ToList();
        }
        public List<BSM_Meter_InfoEntity> GetList(int? area, int? type, int? sPoint, int? sWater)
        {
            string name = "";
            string where = "";
            if (sPoint != null)
            {
                if (sPoint == 3)
                    where += " AND sWater ='" + sPoint + "'";
                else
                    where += " AND sPoint ='" + sPoint + "'";
            }
            if (sWater != null)
            {
                switch (sWater)
                {
                    case 10: name = "芥园水厂"; break;
                    case 11: name = "凌庄水厂"; break;
                    case 12: name = "新开河水厂"; break;
                    case 13: name = "津滨水厂"; break;
                }
                where += " AND ElementID IN(SELECT ElementID FROM RESULT_JUNCTION WHERE Result_Source_24='" + name + "')";
            }
            if (type == null)
            {
                var sql = "SELECT DISTINCT m.* FROM BSM_Meter_Info m,(SELECT ID FROM (SELECT '" + area + "' ID UNION ALL SELECT CONVERT(VARCHAR(5),ID) FROM SYS_DIC WHERE TypeID=5 AND ItemID=" + area +
                    " UNION ALL SELECT CONVERT(VARCHAR(5),ID) FROM SYS_DIC WHERE TypeID=5 AND ItemID in(SELECT ID FROM SYS_DIC WHERE TypeID=5 AND ItemID=" + area + ")) a) b " +
                    "WHERE MeasureAreaId LIKE'%'+b.ID+'%'" + where;
                return service.FindList(sql).OrderBy(i => i.Meter_Name).ToList();
            }
            else
            {
                var sql = "SELECT DISTINCT m.* FROM BSM_Meter_Info m,(SELECT ID FROM (SELECT '" + area + "' ID UNION ALL SELECT CONVERT(VARCHAR(5),ID) FROM SYS_DIC WHERE TypeID=5 AND ItemID=" + area +
                    " UNION ALL SELECT CONVERT(VARCHAR(5),ID) FROM SYS_DIC WHERE TypeID=5 AND ItemID in(SELECT ID FROM SYS_DIC WHERE TypeID=5 AND ItemID=" + area + ")) a) b " +
                    "WHERE MeasureAreaId LIKE'%'+b.ID+'%' AND Meter_Type =" + type + where;
                return service.FindList(sql).OrderBy(i => i.Meter_Name).ToList();
            }
        }
        public List<BSM_Meter_InfoEntity> GetList(int? area, int? type)
        {
            if (type == null)
            {
                var sql = "SELECT DISTINCT * FROM BSM_Meter_Info WHERE ID IN(SELECT MAX(m.ID) ID FROM BSM_Meter_Info m,(SELECT ID FROM (SELECT '" + area + "' ID UNION ALL SELECT CONVERT(VARCHAR(5),ID) " +
                    "FROM SYS_DIC WHERE TypeID=5 AND ItemID=" + area + " UNION ALL SELECT CONVERT(VARCHAR(5),ID) FROM SYS_DIC WHERE TypeID=5 AND ItemID in(SELECT ID FROM SYS_DIC WHERE TypeID=5 " +
                    "AND ItemID=" + area + ")) a) b WHERE MeasureAreaId LIKE'%'+b.ID+'%' GROUP BY Meter_Type)";
                return service.FindList(sql).OrderBy(i => i.Meter_Name).ToList();
            }
            else
            {
                var sql = "SELECT DISTINCT * FROM BSM_Meter_Info WHERE ID IN(SELECT MAX(m.ID) ID FROM BSM_Meter_Info m,(SELECT ID FROM (SELECT '" + area + "' ID UNION ALL SELECT CONVERT(VARCHAR(5),ID) " +
                    "FROM SYS_DIC WHERE TypeID=5 AND ItemID=" + area + " UNION ALL SELECT CONVERT(VARCHAR(5),ID) FROM SYS_DIC WHERE TypeID=5 AND ItemID in(SELECT ID FROM SYS_DIC WHERE TypeID=5 " +
                    "AND ItemID=" + area + ")) a) b WHERE MeasureAreaId LIKE'%'+b.ID+'%' AND Meter_Type =" + type+ " GROUP BY Meter_Type)";
                return service.FindList(sql).OrderBy(i => i.Meter_Name).ToList();
            }
        }
        public List<BSM_Meter_InfoEntity> GetList(int? area1, int? area2, int? area3, int? type, int? sPoint, int? sWater)
        {
            if (type == null)
            {
                var sql = "SELECT DISTINCT * FROM BSM_Meter_Info m,(SELECT CONVERT(VARCHAR(5),ID) ID FROM SYS_DIC WHERE TypeID=5 AND ID=" + area3 + " AND ID IN(SELECT ID FROM SYS_DIC WHERE TypeID=5 " +
                    "AND ItemID=" + area2 + ") AND ID IN(SELECT ID FROM SYS_DIC WHERE TypeID=5 AND ItemID in(SELECT ID FROM SYS_DIC WHERE TypeID=5 AND ItemID=" + area1 + "))) a " +
                    "WHERE Display=1 AND MeasureAreaId LIKE'%'+a.ID+'%'";
                return service.FindList(sql).ToList();
            }
            else
            {
                var sql = "SELECT DISTINCT * FROM BSM_Meter_Info m,(SELECT CONVERT(VARCHAR(5),ID) ID FROM SYS_DIC WHERE TypeID=5 AND ID=" + area3 + " AND ID IN(SELECT ID FROM SYS_DIC WHERE TypeID=5 " +
                    "AND ItemID=" + area2 + ") AND ID IN(SELECT ID FROM SYS_DIC WHERE TypeID=5 AND ItemID in(SELECT ID FROM SYS_DIC WHERE TypeID=5 AND ItemID=" + area1 + "))) a " +
                    "WHERE Display=1 AND MeasureAreaId LIKE'%'+a.ID+'%' AND Meter_Type =" + type;
                return service.FindList(sql).ToList();
            }
        }

        /// <summary>
        /// 分页查询表具信息
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<BSM_Meter_InfoEntity> GetList(Pagination pagination, int id)
        {
            var expression = ExtLinq.True<BSM_Meter_InfoEntity>();
            if (!string.IsNullOrEmpty(id.ToString()))
            {
                expression = expression.And(t => t.ID.Equals(id));
            }
            return service.FindList(expression, pagination).ToList();

        }

        public BSM_Meter_InfoEntity GetEntity(string stationKey)
        {
            return service.FindEntity(i => i.Station_Key == stationKey);

        }

        /// <summary>
        /// 根据id删除表具信息
        /// </summary>
        /// <param name="keyValue"></param>
        public void DeleteForm(int id)
        {
            service.Delete(t => t.ID == id);
        }

        /// <summary>
        /// 添加或更新表具信息
        /// </summary>
        /// <param name="moduleEntity">数据实体信息</param>
        /// <param name="userID">用户编号</param>
        public void SubmitForm(BSM_Meter_InfoEntity moduleEntity, string id)
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
                tmp = Convert.ToInt64(id);
            }
            catch { }
            var result = service.FindEntity(t => t.ID == tmp);
            return JsonConvert.SerializeObject(result);
        }
    }
}
