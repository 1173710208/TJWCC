using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TJWCC.Data;
using TJWCC.Code;
using TJWCC.Application.BSSys;
using System.Collections;
using TJWCC.Domain.Entity.BSSys;
using Newtonsoft.Json.Linq;


namespace TJWCC.Web.Areas.Elaborate.Controllers
{
    public class ValueCheckController : ControllerBase
    {
        TJWCCDbContext dbcontext = new TJWCCDbContext();
        private TCVApp tcvApp = new TCVApp();
        private RESULT_HYDRANTApp rhApp = new RESULT_HYDRANTApp();
        private RESULT_TCVApp rtApp = new RESULT_TCVApp();
        private RESULT_JUNCTIONApp rjApp = new RESULT_JUNCTIONApp();
        private RESULT_PIPEApp rpApp = new RESULT_PIPEApp();
        private ZoneClassApp ZoneClass = new ZoneClassApp();
        private DistrictClassApp DistrictClass = new DistrictClassApp();
        private DMAAreaClassApp DMAAreaClass = new DMAAreaClassApp();
        private DMAClassApp DMAClass = new DMAClassApp();
        private MaterialClassApp MaterialClass = new MaterialClassApp();
        private BSB_ValveAreaApp bvaApp = new BSB_ValveAreaApp();

        // 阀门状态核准: Elaborate/ValueCheck/ValveCheckCal
        public ActionResult ValveCheckCal(string valve_ids, string valve_status)
        {
            //string temp = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            //string BAK_modelFileName = "BAK_rumen_16page.inp";  //模型备份文件名称
            //string modelFileName = "D:\\sambo\\inp.inp";          //模型文件名称
            //string reportFileName = "D:\\sambo\\moni.rpt";         //报告文件名称
            string modelFileName = "D:\\sambo\\rumen_16page.inp";          //模型文件名称
            string reportFileName = "D:\\sambo\\rumen_16page.rpt";         //报告文件名称

            int errcode = 0;
            EpanetEngine ce = new EpanetEngine();

            //value_status = "1,1";
            ////value_ids = "462786,222215";
            //value_ids = "230605,222215";
            var valveStatus = valve_status.Split(',');                  //阀门状态数组; 0:关闭；1：开启
            var valveIds = valve_ids.Split(',');                        //阀门ID数组
            decimal[] oriPress = new decimal[valveStatus.Count()];      //阀门改变状态前的压力表的压力值数组
            decimal[] meterPress = new decimal[valveStatus.Count()];    //阀门对应的压力表的检测压力值数组
            decimal[] calPress = new decimal[valveStatus.Count()];      //阀门对应的计算压力值数组
            int[] meterIds = new int[valveStatus.Count()];              //阀门对应的压力表的ID数组
            int[] checkResults = new int[valveStatus.Count()];          //阀门核准结果数组，0：正确；1：状态不对

            int k = 0;
            long t = 0;
            float tempFloat = 0f;
            int index = 0;
            int index2 = 0;
            string strerror=string.Empty;

            //获取阀门ID和阀门对应的压力表的检测压力值
            foreach (var vlaue in valveIds)
            {
                string strSQL = "SELECT Pointid FROM BSB_ValveArea WHERE Valveid=" + valveIds[k];
                meterIds[k] = dbcontext.Database.SqlQuery<int>(strSQL).FirstOrDefault();
                strSQL = "SELECT c.Tag_value FROM BSM_Meter_Info i, BS_SCADA_TAG_CURRENT c WHERE i.Station_Key=c.Station_key and i.Meter_Type=2 and i.ElementID=" + meterIds[k];
                meterPress[k] = dbcontext.Database.SqlQuery<decimal>(strSQL).FirstOrDefault();
                k++;
            }

            errcode = ce.CENopen(modelFileName, reportFileName, "");
            errcode = ce.CENopenH();

            //meterIds[0] = 147210;
            //valveIds[0] = "226403";

            //获取阀门状态改变前的压力
            ce.CENinitH(0);
            ce.CENrunH(ref t);
            for (int i = 0; i < meterIds.Count(); i++)
            {
                ce.CENgetnodeindex(meterIds[i].ToString(), ref index);
                ce.CENgetnodevalue(index, EpanetEngine.EN_PRESSURE, ref tempFloat);
                oriPress[i] = tempFloat.ToDecimal() / 100;
            }

            //获取阀门状态改变后的检测点压力
            //float tempBool = 0f;
            ce.CENinitH(0);
            for (int i = 0; i < meterIds.Count(); i++)
            {
                //初始化水力特性

                //设置当前执行的参数
                //ce.XkENgetlinkindex(valves[i]["veid"].ToString(), ref index);
                //ce.CENsetlinkvalue(index, CallEpaneth.EN_STATUS, Convert.ToBoolean(valves[i]["vstatus"]) ? 1 : 0);//设置阀门状态

                errcode = ce.CENgetlinkindex(valveIds[i], ref index2);
                if (index2 == 0)
                {
                    strerror = "阀门或压力检测点的ElementId不存在！";
                    continue;
                }
                //ce.CENgetlinkvalue(index2, EpanetEngine.EN_STATUS, ref tempBool);
                ce.CENsetlinkvalue(index2, EpanetEngine.EN_STATUS, !valveStatus[i].ToBool() ? 1 : 0);
                //ce.CENgetlinkvalue(index2, EpanetEngine.EN_STATUS, ref tempBool);

                //单一时段运行 
                ce.CENrunH(ref t);
                //检索结果
                errcode = ce.CENgetnodeindex(meterIds[i].ToString(), ref index);
                if (index2 == 0)
                {
                    strerror = "阀门或压力检测点的ElementId不存在！";
                    continue;
                }
                ce.CENgetnodevalue(index, EpanetEngine.EN_PRESSURE, ref tempFloat);
                calPress[i] = tempFloat.ToDecimal()/100;
                ce.CENsetlinkvalue(index2, EpanetEngine.EN_STATUS, valveStatus[i].ToBool() ? 1 : 0);
                //ce.CENgetlinkvalue(index2, EpanetEngine.EN_STATUS, ref tempBool);

                //判段阀门状态是否与实际一致，0：一致；1：不一致
                if (Math.Abs(meterPress[i] - oriPress[i]) > Math.Abs(meterPress[i] - calPress[i]))
                {
                    checkResults[i] = 1;
                }
                else
                {
                    checkResults[i] = 0;
                }
            }
            errcode = ce.CENcloseH();
            errcode = ce.CENclose();

            string strResult = string.Empty;
            JArray result = new JArray();
            if (strerror.Count() == 0)
            {
                strResult = string.Join(",", checkResults);
            }
            else
            {
                strResult = "-1," + strerror;
            }
            result.Add(strResult);

            return Content(result.ToJson());
        }

        public string MapQuery(int ElementId, int layerId)
        {
            switch (layerId)
            {
                case 1://水库
                    decimal Result_Pressure = dbcontext.Database.SqlQuery<decimal>("SELECT AVG(Tag_value) Tag_value FROM [dbo].[BS_SCADA_TAG_CURRENT] WHERE Station_Key in(SELECT Station_Key FROM BSM_Meter_Info WHERE Station_Unit='"
                        + ElementId + "' AND Tag_key='2' AND Tag_value>0.1) GROUP BY Tag_key").FirstOrDefault();
                    var elementsr = new { Result_Pressure };
                    return elementsr.ToJson();
                case 2://消火栓
                    var tmph = rhApp.GetList(ElementId);
                    var elementsh = (from m in tmph
                                     select new
                                     {
                                         Result_HydraulicGrade = m.Result_HydraulicGrade_24 == null ? "0" : m.Result_HydraulicGrade_24.ToString(),
                                         Result_Pressure = m.Result_Pressure_24 == null ? "0" : m.Result_Pressure_24.ToString(),
                                     }).FirstOrDefault();
                    return elementsh.ToJson();
                //break;
                case 3://阀门
                    var tmpt = rtApp.GetList(ElementId);
                    var elementst = (from m in tmpt
                                     select new
                                     {
                                         Result_FromHead = m.Result_FromHead_24 == null ? "0" : m.Result_FromHead_24.ToString(),
                                         Result_FromPressure = m.Result_FromPressure_24 == null ? "0" : m.Result_FromPressure_24.ToString(),
                                         Result_ToHead = m.Result_ToHead_24 == null ? "0" : m.Result_ToHead_24.ToString(),
                                         Result_ToPressure = m.Result_ToPressure_24 == null ? "0" : m.Result_ToPressure_24.ToString(),
                                     }).FirstOrDefault();
                    return elementst.ToJson();
                //break;
                case 4://节点
                    var tmpj = rjApp.GetList(ElementId);
                    var elementsj = (from m in tmpj
                                     select new
                                     {
                                         Result_HydraulicGrade = m.Result_HydraulicGrade_24 == null ? "0" : m.Result_HydraulicGrade_24.ToString(),
                                         Result_Pressure = m.Result_Pressure_24 == null ? "0" : m.Result_Pressure_24.ToString(),
                                         Result_Demand = m.Result_Demand_24 == null ? "0" : m.Result_Demand_24.ToString(),
                                         Result_Cl = m.Result_Cl_24 == null ? "0" : m.Result_Cl_24.ToString(),
                                         Result_Age = m.Result_Age_24 == null ? "0" : m.Result_Age_24.ToString(),
                                         Result_Source = m.Result_Source_24,
                                     }).FirstOrDefault();
                    return elementsj.ToJson();
                //break;
                case 5://管道
                    var tmpp = rpApp.GetList(ElementId);
                    var elementsp = (from m in tmpp
                                     select new
                                     {
                                         Result_Flow = m.Result_Flow_24 == null ? "0" : m.Result_Flow_24.ToString(),
                                         Result_Headloss = m.Result_Headloss_24 == null ? "0" : m.Result_Headloss_24.ToString(),
                                         Result_Velocity = m.Result_Velocity_24 == null ? "0" : m.Result_Velocity_24.ToString(),
                                     }).FirstOrDefault();
                    return elementsp.ToJson();
                    //break;
            }
            return null;
        }
        public ActionResult CheckValve(string ids)
        {
            JArray result = new JArray();
            var valves = ids.Split(',');
            foreach (var valve in valves)
            {
                var va = tcvApp.GetList(Convert.ToInt32(valve)).FirstOrDefault();
                var pointid = bvaApp.GetById(Convert.ToInt32(valve));
                JArray tmp = new JArray();
                tmp.Add(va.ElementId);
                tmp.Add(va.Physical_Address);
                tmp.Add(va.Physical_Status == 2 ? "关闭<br>" : "开启<br>" + va.GISID);
                tmp.Add(va.Physical_Diameter.ToInt());
                result.Add(tmp);
            }
            return Content(result.ToJson());
        }
        public ActionResult CreateValveOrder(string ids)
        {
            TimeSpan time = new TimeSpan(DateTime.Now.Hour, 0, 0);
            return Success("下发成功！");
        }
        public ActionResult GetAreaList()
        {
            JArray result = new JArray();
            JArray name = new JArray();
            JArray area = new JArray();
            var areas = dbcontext.Database.SqlQuery<AreaView>("SELECT COUNT(*) Tc,Physical_DistrictID,CONVERT(int,Physical_Diameter) Jc,0.0 Length,0 Pc,'' District FROM [dbo].[TCV] WHERE Is_Active=1 AND " +
                "Physical_DistrictID IS NOT NULL GROUP BY Physical_DistrictID,Physical_Diameter ORDER BY Physical_DistrictID,Physical_Diameter").ToList();
            var areavs = dbcontext.Database.SqlQuery<AreaView>("SELECT x.*,y.District FROM(SELECT Tc,Jc,Pc,Length,c.Physical_DistrictID FROM(SELECT COUNT(*) tc,Physical_DistrictID " +
                "FROM [dbo].[TCV] WHERE Is_Active=1 AND Physical_DistrictID IS NOT NULL GROUP BY Physical_DistrictID) a LEFT JOIN (SELECT COUNT(*) jc,Physical_DistrictID FROM " +
                "[dbo].[JUNCTION] WHERE Is_Active=1 AND Physical_DistrictID IS NOT NULL GROUP BY Physical_DistrictID) b ON a.Physical_DistrictID=b.Physical_DistrictID LEFT JOIN " +
                "(SELECT COUNT(*) pc,CONVERT(DECIMAL(15,2),SUM(Physical_Length)) length,Physical_DistrictID FROM [dbo].[PIPE] WHERE Is_Active=1 AND Physical_DistrictID IS NOT NULL GROUP BY Physical_DistrictID)c " +
                "ON b.Physical_DistrictID=c.Physical_DistrictID) x,DistrictClass y WHERE Physical_DistrictID=ID ORDER BY Physical_DistrictID").ToList();
            foreach(var areav in areavs)
            {
                name.Add(areav.District);
                JArray tmp = new JArray();
                var tmpas = areas.Where(i => i.Physical_DistrictID == areav.Physical_DistrictID).OrderBy(i=>i.Length).ToList();
                tmp.Add(JArray.FromObject(tmpas));
                tmp.Add(JObject.FromObject(areav));
                area.Add(tmp);
            }
            result.Add(name);
            result.Add(area);
            return Content(result.ToJson());
        }
        public ActionResult GetAreaByIdList(int areaId,int diam)
        {
            JArray result = new JArray();
            var tCVs = dbcontext.Database.SqlQuery<TCVEntity>("SELECT * FROM [dbo].[TCV] WHERE Is_Active=1 AND Physical_DistrictID=" + areaId + " AND Physical_Diameter=" + diam).ToList();
            result.Add(JArray.FromObject(tCVs));
            return Content(result.ToJson());
        }
        public JsonResult GetDMAList()
        {
            int GradeNum = 3;//session中获取当前管网分区等级个数
            //List<DMAClass> dma = db.DMAClass.OrderBy(item => item.DistrictID).ThenBy(item => item.ID).ToList();//按双字段排序
            List<DistrictClassEntity> dist = DistrictClass.GetList().OrderBy(item => item.ID).ToList();
            ArrayList result = new ArrayList();
            ArrayList tmpdist = new ArrayList();
            ArrayList tmpdma = new ArrayList();
            ArrayList tmpdmaa = new ArrayList();
            for (int i = 0; i < dist.Count; i++)
            {
                int tmpdistid = dist[i].ID;
                string sdistid = Convert.ToString(dist[i].ID);
                bool isDistNext = false;
                ArrayList tmp = new ArrayList();
                if (GradeNum > 2)
                {
                    List<DMAClassEntity> dma = DMAClass.GetList(tmpdistid).OrderBy(item => item.ID).ToList();
                    for (int j = 0; j < dma.Count; j++)
                    {
                        isDistNext = true;
                        int tmpdmaid = dma[j].ID;
                        string sdmaid = Convert.ToString(dma[j].ID);
                        bool isDmaNext = false;
                        if (GradeNum > 3)
                        {
                            List<DMAAreaClassEntity> dmaa = DMAAreaClass.GetList(tmpdmaid).OrderBy(item => item.ID).ToList();
                            for (int x = 0; x < dmaa.Count; x++)
                            {
                                isDmaNext = true;
                                int tmpdmaaid = dmaa[x].ID;
                                string sdmaaid = Convert.ToString(dmaa[x].ID);
                                tmp = new ArrayList();
                                tmp.Add(tmpdmaaid);
                                tmp.Add(dmaa[x].DMAArea);
                                tmp.Add(dmaa[x].DMAID);
                                tmp.Add(false);//是否有下级菜单
                                tmpdmaa.Add(tmp);
                            }
                        }
                        tmp = new ArrayList();
                        tmp.Add(tmpdmaid);
                        tmp.Add(dma[j].DMA);
                        tmp.Add(dma[j].DistrictID);
                        tmp.Add(isDmaNext);//是否有下级菜单
                        tmpdma.Add(tmp);
                    }
                }
                tmp = new ArrayList();
                tmp.Add(tmpdistid);
                tmp.Add(dist[i].District);
                tmp.Add(dist[i].ZoneID);
                tmp.Add(isDistNext);//是否有下级菜单
                tmpdist.Add(tmp);
            }
            var tmpZone = ZoneClass.GetList();
            ArrayList tmpzone = new ArrayList();
            for (int i = 0; i < tmpZone.Count; i++)
            {
                ArrayList tmp = new ArrayList();
                tmp.Add(tmpZone[i].ID);
                tmp.Add(tmpZone[i].Zone);
                tmpzone.Add(tmp);
            }
            result.Add(tmpzone);
            result.Add(tmpdist);
            if (GradeNum > 2) result.Add(tmpdma);
            if (GradeNum > 3) result.Add(tmpdmaa);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPipeMaterialList()
        {
            //List<DMAClass> dma = db.DMAClass.OrderBy(item => item.DistrictID).ThenBy(item => item.ID).ToList();//按双字段排序
            List<MaterialClassEntity> dist = MaterialClass.GetList().OrderBy(item => item.ID).ToList();
            ArrayList result = new ArrayList();
            for (int i = 0; i < dist.Count; i++)
            {
                int tmpdistid = dist[i].ID;
                bool isDistNext = false;
                ArrayList tmp = new ArrayList();
                tmp.Add(tmpdistid);
                tmp.Add(dist[i].Material);
                tmp.Add(isDistNext);//是否有下级菜单
                result.Add(tmp);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        class AreaView
        {
            public int Tc { get; set; }
            public int Jc { get; set; }
            public int Pc { get; set; }
            public decimal Length { get; set; }
            public string District { get; set; }
            public int Physical_DistrictID { get; set; }

        }
    }
}