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
using Newtonsoft.Json.Linq;

namespace TJWCC.Application.BSSys
{

    public class BS_SCADA_TAG_CURRENTApp
    {
        private IBS_SCADA_TAG_CURRENTRepository service = new BS_SCADA_TAG_CURRENTRepository();
        private IBS_SCADA_TAG_INFORepository bstiservice = new BS_SCADA_TAG_INFORepository();
        private IBSM_Meter_InfoRepository bmiservice = new BSM_Meter_InfoRepository();

        /// <summary>
        /// 获取站点数据平均值(例如：水厂出口平均压力)
        /// </summary>
        ///<param name="stationId"></param>
        ///<param name="ValueTypeId">数据类别ID</param>
        /// <returns></returns>
        public decimal? GetStationAvgValue(string stationId, string valueTypeId)
        {
            return service.IQueryable().Where(i => i.GYStationId == stationId && i.Tag_key == valueTypeId && i.Tag_value > 0.01m).Average(i => i.Tag_value);
        }
        public decimal? GetAvgValue(string stationId, string valueTypeId)
        {
            return service.IQueryable().Where(i => i.GYStationId == stationId && i.Tag_key == valueTypeId).Average(i => i.Tag_value);
        }

        /// <summary>
        /// 获取原水泵站数据合计(例如：泵站出口流量合计)
        /// </summary>
        ///<param name="stationId"></param>
        ///<param name="ValueTypeId">数据类别ID</param>
        /// <returns></returns>
        public decimal? GetStationSumValue(string stationId, string valueTypeId)
        {
            return service.IQueryable().Where(i => i.GYStationId == stationId && i.Tag_key == valueTypeId).Sum(i => i.Tag_value);
        }

        /// <summary>
        /// 获取单个实时数据实体（例如：泵站的前池水位等）
        /// </summary>
        ///<param name="stationId"></param>
        ///<param name="ValueTypeId">数据类别ID</param>
        /// <returns></returns>
        public BS_SCADA_TAG_CURRENTEntity GetStationData(string stationId, string valueTypeId, int number)
        {
            var result = service.IQueryable().Where(i => i.GYStationId == stationId && i.Tag_key == valueTypeId && i.Number == number).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// 获取实时数据实体List（例如：流量、压力等）
        /// </summary>
        ///<param name="ValueTypeId">数据类别ID</param>
        /// <returns></returns>
        public List<BS_SCADA_TAG_CURRENTEntity> GetStationDataList(string valueTypeId)
        {
            var result = service.IQueryable().Where(i => i.Tag_key == valueTypeId).OrderBy(i => i.OrderNum).ToList();
            return result;
        }

        /// <summary>
        /// 获取原水泵站水库液位
        /// </summary>
        ///<param name="stationId"></param>
        /// <returns></returns>
        public BS_SCADA_TAG_CURRENTEntity GetReservoirLevel(string stationId)
        {
            var result = service.IQueryable().Where(i => i.GYStationId == stationId && i.Tag_key == "93").FirstOrDefault();
            return result;
        }

        /// <summary>
        /// 获取SCADA
        /// </summary>
        /// <returns></returns>
        public List<BS_SCADA_TAG_CURRENTEntity> GetList()
        {
            return service.IQueryable().Where(i => i.Tag_key != null).ToList();
        }

        /// <summary>
        /// 获取SCADA
        /// </summary>
        /// <returns></returns>
        public BS_SCADA_TAG_CURRENTEntity GetList(string gyStationId, string tagKey,int number)
        {

            return service.IQueryable().Where(i => i.GYStationId.Equals(gyStationId) & i.Tag_key == tagKey & i.Number == number).OrderByDescending(i => i.Save_date).FirstOrDefault();
        }

        /// <summary>
        /// 获取实时数据
        /// </summary>
        /// <param name="pumpStationId">泵站（水厂）ID</param>
        /// <returns></returns>
        public List<BS_SCADA_TAG_CURRENTEntity> GetList(string pumpStationId)
        {
            return service.IQueryable().Where(i => i.GYStationId == pumpStationId).ToList();
        }

        /// <summary>
        /// 分页查询SCADA数据记录
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="Station_key"></param>
        /// <returns></returns>
        public List<BS_SCADA_TAG_CURRENTEntity> GetList(Pagination pagination, string Station_key)
        {
            var expression = ExtLinq.True<BS_SCADA_TAG_CURRENTEntity>();
            if (!string.IsNullOrEmpty(Station_key))
            {
                expression = expression.And(t => t.Station_key.Equals(Station_key));
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
        public void SubmitForm(BS_SCADA_TAG_CURRENTEntity moduleEntity, string id)
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

        public string GetsTagKeys()
        {
            JArray result = new JArray();
            JArray name = new JArray();
            JArray unit = new JArray();
            string sTagKeys = "";
            var bstcs = service.IQueryable().Where(i => i.Selected == 1).ToArray();
            foreach (var bstc in bstcs)
            {
                string tmp = bstc.GYStationId;
                string tagKey = bstc.Tag_key.ToString();
                unit.Add(bstiservice.FindEntity(i => i.Tag_key.Equals(tagKey)).Units);
                if (string.IsNullOrWhiteSpace(bstc.ValueName))
                    name.Add(bmiservice.FindEntity(i => i.Station_Key == tmp).Meter_Name);
                else
                    name.Add(bstc.ValueName);
                if (sTagKeys.Length == 0)
                {
                    sTagKeys = bstc.GYStationId + "_" + bstc.Tag_key + "_" + bstc.Number;
                }
                else
                {
                    sTagKeys += "," + bstc.GYStationId + "_" + bstc.Tag_key + "_" + bstc.Number;
                }
            }
            result.Add(sTagKeys);
            result.Add(name);
            result.Add(unit);
            return result.ToJson();
        }
    }
}
