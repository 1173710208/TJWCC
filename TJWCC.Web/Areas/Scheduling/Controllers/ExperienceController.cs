using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TJWCC.Application.BSSys;
using TJWCC.Application.WCC;
using TJWCC.Code;
using TJWCC.Domain.Entity.BSSys;
using TJWCC.Domain.Entity.WCC;
using TJWCC.Domain.ViewModel;

namespace TJWCC.Web.Areas.Scheduling.Controllers
{
    public class ExperienceController : ControllerBase
    {
        private CC_DispatchPlanApp dpApp = new CC_DispatchPlanApp();
        private BSM_Meter_InfoApp bmiApp = new BSM_Meter_InfoApp();
        private GY_PumpStationInfoApp pumpSApp = new GY_PumpStationInfoApp();
        private DistrictClassApp DistrictClass = new DistrictClassApp();
        // 调度经验库: Scheduling/Experience

        public ActionResult GetFactoryList()
        {
            List<GetDataType_Result> list = new List<GetDataType_Result>();
            var sds = pumpSApp.GetPStationList(2).Where(i => i.SStationTypeId == 1);
            foreach (var sd in sds)
            {
                GetDataType_Result rdr = new GetDataType_Result()
                {
                    Name = sd.StationName,
                    Value = sd.StationId.ToString()
                };
                list.Add(rdr);
            }
            return Content(list.ToJson());
        }
        /// <summary>
        /// 根据条件获取调度日志数据
        /// </summary>
        ///<param name="typeId">水厂类型</param> 
        /// <returns></returns>

        public ActionResult GetDispatchPlan(DateTime? sDate, DateTime? eDate, int? dtype, string pressList)
        {
            List<CC_DispatchPlanEntity> msList = dpApp.GetList(sDate, eDate, dtype, pressList);
            return Content(msList.Select(i=>i.ID).ToJson());
        }
        
        public ActionResult GetPressList()
        {
            List<GetDataType_Result> list = new List<GetDataType_Result>();
            var sds = bmiApp.GetListByMType(2).Where(i=>!i.MeasureAreaId.Equals("17")).OrderBy(i=>i.Meter_Name).ToList();
            foreach (var sd in sds)
            {
                GetDataType_Result rdr = new GetDataType_Result()
                {
                    Name = sd.Meter_Name,
                    Value = sd.Station_Key
                };
                list.Add(rdr);
            }
            return Content(list.ToJson());
        }
        public ActionResult GetDMAList()
        {
            List<DistrictClassEntity> dist = DistrictClass.GetList().OrderBy(item => item.ID).ToList();
            List<GetDataType_Result> list = new List<GetDataType_Result>();
            for (int i = 0; i < dist.Count; i++)
            {
                string sdistid = Convert.ToString(dist[i].ID);
                GetDataType_Result rdr = new GetDataType_Result()
                {
                    Name = dist[i].District,
                    Value = sdistid
                };
                list.Add(rdr);
            }
            return Content(list.ToJson());
        }
        /// <summary>
        /// 获取日常调度方案中调度数据
        /// </summary>
        /// <returns></returns>
        
        public ActionResult GetData(int? planId)
        {
            if (planId == null) return Content(new JArray().ToJson());
            JArray msList = dpApp.GetList(planId);
            return Content(msList.ToJson());
        }

    }
}