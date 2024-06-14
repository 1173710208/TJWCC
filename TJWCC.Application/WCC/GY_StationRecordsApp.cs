using TJWCC.Code;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Repository.WCC;
using TJWCC.Domain.IRepository.WCC;
using TJWCC.Domain.Entity.WCC;

namespace TJWCC.Application.WCC
{

    public class GY_StationRecordsApp
    {
        private IGY_StationRecordsRepository service = new GY_StationRecordsRepository();

        /// <summary>
        /// 获取原水泵站流量
        /// </summary>
        ///<param name="stationId"></param>
        /// <returns></returns>
        public decimal GetStationFlow(int stationId)
        {
            return service.IQueryable().Where(i => i.StationId == stationId).Where(i => i.TypeId == 2).Sum(i => i.Value1).ToDecimal();
        }

        /// <summary>
        /// 获取原水泵站前池液位
        /// </summary>
        ///<param name="stationId"></param>
        /// <returns></returns>
        public GY_StationRecordsEntity GetStationLevel(int stationId)
        {
            var result = service.IQueryable().Where(i => i.StationId == stationId && i.TypeId == 20).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// 分页查询SCADA数据记录
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public List<GY_StationRecordsEntity> GetList(Pagination pagination, int typeId)
        {
            var expression = ExtLinq.True<GY_StationRecordsEntity>();
            if (!string.IsNullOrEmpty(typeId.ToString()))
            {
                expression = expression.And(t => t.TypeId.Equals(typeId));
            }
            return service.FindList(expression, pagination).ToList();

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
        public void SubmitForm(GY_StationRecordsEntity moduleEntity, string id)
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
