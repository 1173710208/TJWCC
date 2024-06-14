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

    public class GY_PumpStationInfoApp
    {
        private IGY_PumpStationInfoRepository service = new GY_PumpStationInfoRepository();

        /// <summary>
        /// 获取原水泵站数据
        /// </summary>
        ///<param name="stationTypeId">泵站一级分类</param>
        ///<param name="sStationTypeId">泵站二级分类</param>
        /// <returns></returns>
        public List<GY_PumpStationInfoEntity> GetSStationList(int stationTypeId, int sStationTypeId)
        {
            if (sStationTypeId == 1)
            {
                var reslut = service.IQueryable().Where(i => i.StationTypeId == stationTypeId && i.SStationTypeId == sStationTypeId).ToList();
                return reslut;
            }
            else
            {
                var reslut = service.IQueryable().Where(i => i.StationTypeId == stationTypeId && (i.SStationTypeId == sStationTypeId  || i.SStationTypeId == 4)).ToList();
                return reslut;
            }
        }
        public List<GY_PumpStationInfoEntity> GetPStationList(int stationTypeId)
        {
            var reslut = service.IQueryable().Where(i => i.StationTypeId == stationTypeId).ToList();
            return reslut;
        }

        /// <summary>
        /// 分页查询SCADA数据记录
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public List<GY_PumpStationInfoEntity> GetList(Pagination pagination, int type)
        {
            var expression = ExtLinq.True<GY_PumpStationInfoEntity>();
            if (!string.IsNullOrEmpty(type.ToString()))
            {
                expression = expression.And(t => t.StationTypeId.Equals(type));
            }
            return service.FindList(expression, pagination).ToList();

        }

        /// <summary>
        /// 获取所有泵站（水厂）数据
        /// </summary>
        /// <returns></returns>
        public List<GY_PumpStationInfoEntity> GetAllStationList()
        {
            var reslut = service.IQueryable().OrderBy(i=>i.Remarka0).ToList();
            return reslut;
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
        public void SubmitForm(GY_PumpStationInfoEntity moduleEntity, string id)
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

        public string GetSingleRecord(int stationId)
        {
            var result = service.FindEntity(t => t.StationId == stationId);
            return JsonConvert.SerializeObject(result);
        }
    }
}
