using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TJWCC.Application.DataDisplay;
using TJWCC.Code;
using TJWCC.Application.WCC;
using TJWCC.Domain.Entity.Public;
using TJWCC.Application.BSSys;
using TJWCC.Data;
using TJWCC.Application.SystemManage;
using TJWCC.Domain.ViewModel;
using TJWCC.Domain.Entity.WCC;
using TJWCC.Domain.Entity.SystemManage;
using TJWCC.Domain.Entity.BSSys;
using Newtonsoft.Json.Linq;
using System.Data.Common;
using System.Data;

namespace TJWCC.Web.Areas.DataDisplay.Controllers
{
    public class MainController : ControllerBase
    {
        log4net.ILog loger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //private Dis_WaterCompanyClassApp onlineApp = new Dis_WaterCompanyClassApp();
        //private Dis_WaterApp app = new Dis_WaterApp();
        //private Dis_SignalStrengthApp SignalApp = new Dis_SignalStrengthApp();
        //private Dis_AlarmApp alarmApp = new Dis_AlarmApp();
        //private Dis_MakerClassApp appMakerClassApp = new Dis_MakerClassApp();
        private SYS_DICApp sysDicApp = new SYS_DICApp();
        private BSM_Meter_InfoApp bmiApp = new BSM_Meter_InfoApp(); 

        private CC_WetherRecordApp wetherApp = new CC_WetherRecordApp();
        private GY_PumpStationInfoApp pumpSApp = new GY_PumpStationInfoApp();
        private BS_SCADA_TAG_CURRENTApp scadaCurrentApp = new BS_SCADA_TAG_CURRENTApp();
        private BS_SCADA_TAG_HISApp scadaHisApp = new BS_SCADA_TAG_HISApp();
        private BS_SCADA_TAG_INFOApp scadaInfoApp = new BS_SCADA_TAG_INFOApp();
        private TJWCCDbContext dbContext = new TJWCCDbContext();
        // 主页 DataDisplay/Main

        /// <summary>
        /// 根据工艺二级菜单数据,获取泵站的工艺数据
        /// </summary>
        ///<param name="stationId">工艺一级菜单类别</param> 
        /// <returns></returns>

        public ActionResult GetMainPumpCurrent(int stationId)
        {
            string strSQL = strSQL = "SELECT Tag_value, Tag_key, Number,ValueName  FROM BS_SCADA_TAG_CURRENT WHERE GYStationId='" + stationId + "' ORDER BY Tag_key, Number ";
            List<PumpStationCurrent> msList = dbContext.Database.SqlQuery<PumpStationCurrent>(strSQL).ToList();
            foreach (var msl in msList)
            {
                switch (msl.Tag_key)
                {
                    case "1":
                    case "8":
                    case "9":
                    case "83":
                    case "111":
                    case "118":
                    case "101":
                    case "106":
                    case "131":
                    case "204":
                    case "451":
                    case "454":
                    case "455":
                    case "94":
                        msl.Tag_value = msl.Tag_value.ToDecimal(0);
                        break;
                    case "2":
                    case "6":
                    case "105":
                    case "117":
                    case "203":
                    case "221":
                    case "223":
                    case "224":
                    case "225":
                    case "226":
                    case "522":
                        break;
                    default:
                        msl.Tag_value = msl.Tag_value.ToDecimal(2);
                        break;
                }
            }
            return Content(msList.ToJson());
        }

        /// <summary>
        /// 根据工艺二级菜单数据,获取泵站的静态数据
        /// </summary>
        ///<param name="stationId">工艺一级菜单类别</param> 
        /// <returns></returns>
        
        public ActionResult GetMainPumpStationInfo(int stationId)
        {
            return Content(pumpSApp.GetSingleRecord(stationId));
        }

        /// <summary>
        /// 根据工艺一级菜单获取工艺二级菜单数据
        /// </summary>
        ///<param name="typeId">工艺一级菜单类别</param> 
        /// <returns></returns>
        
        public ActionResult GetMainGY2ndItemList(int typeId)
        {
            string strSQL = "SELECT StationId Id, StationName Name FROM GY_PumpStationInfo WHERE StationTypeId=" + typeId + " ORDER BY SStationTypeId";
            List<Main2ndItems> msList = dbContext.Database.SqlQuery<Main2ndItems>(strSQL).ToList();

            return Content(msList.ToJson());
        }

        /// <summary>
        /// 根据监测点类别获取监测点数据（流量、压力、余氯和浊度）
        /// </summary>
        ///<param name="typeId">监测点类别</param> 
        /// <returns></returns>
        
        public ActionResult GetMainRealTimeData(int typeId)
        {
            string strSQL = "";
            if (typeId == 1 || typeId == 9)
            {
                strSQL = "SELECT ValueName Name, Tag_value Value,Tag_key,a.Station_Key FROM BS_SCADA_TAG_CURRENT a,BSM_Meter_Info b WHERE Tag_key in ('1','9') AND a.Station_key=b.Station_key AND Tag_key=Meter_Type ORDER BY OrderNum,CONVERT(int,Measure_Grade) DESC,a.ID";
            }else
            {
                strSQL = "SELECT ValueName Name, Tag_value Value,Tag_key,Station_Key FROM BS_SCADA_TAG_CURRENT WHERE Tag_key='" + typeId + "' ORDER BY OrderNum,ID";
            }

            List<MainRealTime> msList = dbContext.Database.SqlQuery<MainRealTime>(strSQL).ToList();

            return Content(msList.ToJson());
        }

        /// <summary>
        /// 根据监测点类别和监测点id置顶显示
        /// </summary>
        ///<param name="typeId">监测点类别</param> 
        /// <returns></returns>
        
        public ActionResult PointToTop(int typeId,string staKey)
        {
            string strSQL = "";
            TJWCCDbContext db = new TJWCCDbContext();
            try
            {
                db.Database.Connection.Open();
            }
            catch (Exception ex)
            {
                loger.Error(ex.Message);
                loger.Error(ex.Source + " |" + ex.StackTrace);
            }
            DbCommand cmd = db.Database.Connection.CreateCommand();
            cmd.CommandTimeout = int.MaxValue;
            cmd.CommandType = CommandType.Text;
            if (typeId == 1 || typeId == 9)
            {
                cmd.CommandText = "UPDATE [dbo].[BS_SCADA_TAG_CURRENT] SET OrderNum=OrderNum+1 WHERE Tag_key in('1','9')";
            }
            else
            {
                cmd.CommandText = "UPDATE [dbo].[BS_SCADA_TAG_CURRENT] SET OrderNum=OrderNum+1 WHERE Tag_key='" + typeId + "'";
            }
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                loger.Error(ex.Message);
                loger.Error(ex.Source + " |" + ex.StackTrace);
            }
            cmd.CommandText = "UPDATE [dbo].[BS_SCADA_TAG_CURRENT] SET OrderNum=1 WHERE Tag_key='" + typeId + "' AND Station_Key='" + staKey + "'";
            try
            {
                cmd.ExecuteNonQuery();
                db.Database.Connection.Close();
            }
            catch (Exception ex)
            {
                loger.Error(ex.Message);
                loger.Error(ex.Source + " |" + ex.StackTrace);
            }

            if (typeId == 1 || typeId == 9)
            {
                strSQL = "SELECT ValueName Name, Tag_value Value,Tag_key,a.Station_Key FROM BS_SCADA_TAG_CURRENT a,BSM_Meter_Info b WHERE Tag_key in ('1','9') AND a.Station_key=b.Station_key AND Tag_key=Meter_Type ORDER BY OrderNum,CONVERT(int,Measure_Grade) DESC,a.ID";
            }
            else
            {
                strSQL = "SELECT ValueName Name, Tag_value Value,Tag_key,Station_Key FROM BS_SCADA_TAG_CURRENT WHERE Tag_key='" + typeId + "' ORDER BY OrderNum,ID";
            }
            List<MainRealTime> msList = dbContext.Database.SqlQuery<MainRealTime>(strSQL).ToList();

            return Content(msList.ToJson());
        }

        /// <summary>
        /// 根据水厂类型获取大屏的水厂数据
        /// </summary>
        ///<param name="typeId">水厂类型</param> 
        /// <returns></returns>
        
        public ActionResult GetMainReservoir(int typeId)
        {
            List<MainReservoir> msList = new List<MainReservoir>();
            var pse = pumpSApp.GetSStationList(2, typeId);
            foreach (var item in pse)
            {
                MainReservoir ms = new MainReservoir();
                ms.StationName = item.StationName;
                ms.SFlow = scadaCurrentApp.GetStationSumValue(item.StationId.ToString(), "101")?.ToDecimal(3);     //101:水厂进厂流量
                ms.Press = scadaCurrentApp.GetStationAvgValue(item.StationId.ToString(), "117")?.ToDecimal(3);     //117:出口压力
                ms.PumpCount = scadaCurrentApp.GetStationSumValue(item.StationId.ToString(), "201")?.ToInt();     //开泵台数
                ms.Flow = scadaCurrentApp.GetStationSumValue(item.StationId.ToString(), "111")?.ToDecimal(3);      //111:水厂出口流量
                var cl = scadaCurrentApp.GetStationData(item.StationId.ToString(), "41", 1);//余氯
                ms.CL = cl?.Tag_value.ToDecimal(3);
                var ntu = scadaCurrentApp.GetStationData(item.StationId.ToString(), "42", 1);//浊度
                ms.NTU = ntu?.Tag_value.ToDecimal(3);
                msList.Add(ms);
            }
            return Content(msList.ToJson());
        }

        /// <summary>
        /// 根据原水泵站类型获取大屏的原水泵站数据
        /// </summary>
        ///<param name="typeId">原水泵站类型</param> 
        /// <returns></returns>
        
        public ActionResult GetMainStation(int typeId)
        {
            List<MainStation> msList = new List<MainStation>();
            var pse = pumpSApp.GetSStationList(1, typeId);
            foreach (var item in pse)
            {
                MainStation ms = new MainStation();
                if (typeId == 3)
                {
                    ms.StationName = item.Remark;
                    ms.Flow = scadaCurrentApp.GetStationAvgValue(item.StationId.ToString(), "93")?.ToDecimal(2);   //93：水库水位
                    ms.FlowId= item.StationId + "_93_1";
                }
                else
                {
                    var tmpss = scadaCurrentApp.GetStationSumValue(item.StationId.ToString(), "111")?.ToDecimal(2);
                    ms.StationName = item.StationName;
                    ms.Flow = (tmpss > 100m)? (tmpss / 3600m).ToDecimal(2) : tmpss;   //111:出口瞬时流量
                    ms.FlowId = item.StationId + "_111_1";
                    ms.Press = scadaCurrentApp.GetAvgValue(item.StationId.ToString(), "42")?.ToDecimal(3);  //42:出口浊度
                    ms.PressId = item.StationId + "_42_1";
                }

                // 91:前池水位，94：库容
                var level = typeId == 3 ? scadaCurrentApp.GetStationData(item.StationId.ToString(), "94", 1) : scadaCurrentApp.GetStationData(item.StationId.ToString(), "91", 1);
                var levelId = typeId == 3 ? (item.StationId + "_94_1") : (item.StationId + "_91_1");
                ms.Level = level?.Tag_value.ToDecimal(2);
                ms.LevelId = levelId;
                msList.Add(ms);
            }
            return Content(msList.ToJson());
        }

        public ActionResult UpdateMainStation(string MainSId, decimal TagValue)
        {
            List<MainStation> msList = new List<MainStation>();
            var pse = MainSId.Split('_');
            var sca = scadaCurrentApp.GetStationData(pse[0], pse[1], pse[2].ToInt());
            if (sca.Save_date< DateTime.Now.AddHours(-2)) sca.Tag_value = TagValue; else return Success("有新数无更新操作。");
            scadaCurrentApp.SubmitForm(sca, sca.ID.ToString());
            return Success("操作成功。");
        }
        /// <summary>
        /// 获取气象数据
        /// </summary>
        ///<param name="type"></param> 
        /// <returns></returns>
        
        public ActionResult GetWetherRecord(int type)
        {
            return Content(wetherApp.GetList(type).ToJson());
        }

        /// <summary>
        /// 获取历史趋势图初始表信息
        /// </summary>
        /// <returns></returns>
        
        public ActionResult GetsTagKeysData()
        {
            return Content(scadaCurrentApp.GetsTagKeys());
        }

        /// <summary>
        /// 获取历史趋势查询中一级下拉菜单列表
        /// </summary>
        /// <returns></returns>
        
        public ActionResult GetDataTypeList()
        {
            List<GetDataType_Result> list = new List<GetDataType_Result>();
            GetDataType_Result rdr1 = new GetDataType_Result()
            {
                Name = "原水流量",
                Value = "3_8"
            };
            list.Add(rdr1);
            var psis = pumpSApp.GetAllStationList();
            foreach (var psi in psis)
            {
                GetDataType_Result rdr = new GetDataType_Result()
                {
                    Name = psi.StationName,
                    Value = "0_" + psi.StationId
                };
                list.Add(rdr);
            }
            var sds = sysDicApp.GetItemList(3);
            foreach (var sd in sds)
            {
                GetDataType_Result rdr = new GetDataType_Result()
                {
                    Name = sd.ItemName,
                    Value = "3_"+ sd.ItemID
                };
                list.Add(rdr);
            }
            return Content(list.ToJson());
        }

        /// <summary>
        /// 获取历史趋势查询中二级下拉菜单列表
        /// </summary>
        /// <param name="dataType">一级菜单值</param>
        /// <returns></returns>
        
        public ActionResult GetWFTypeList(string dataType)
        {
            List<GetDataType_Result> list = new List<GetDataType_Result>();
            var dType = dataType.Split('_');
            switch (dType[0])
            {
                case "3":
                    var bmis = bmiApp.GetListByMType(Convert.ToInt32(dType[1])).OrderBy(i=>i.Meter_Name);
                    foreach (var bmi in bmis)
                    {
                        GetDataType_Result rdr = new GetDataType_Result()
                        {
                            Name = bmi.Meter_Name,
                            Value = bmi.Station_Key + "_" + bmi.Meter_Type + "_" + bmi.DistrictAreaId
                        };
                        list.Add(rdr);
                    }
                    break;
                default:
                    var bstcs = scadaCurrentApp.GetList(dType[1]).OrderBy(i => i.ValueName);
                    foreach (var bstc in bstcs)
                    {
                        GetDataType_Result rdr = new GetDataType_Result()
                        {
                            Name = bstc.ValueName,
                            Value = bstc.GYStationId + "_" + bstc.Tag_key + "_" + bstc.Number
                        };
                        list.Add(rdr);
                    }
                    break;
            }
            return Content(list.ToJson());
        }
        
        public ActionResult GetFactoryList()
        {
            List<GetDataType_Result> list = new List<GetDataType_Result>();
            var sds = pumpSApp.GetPStationList(2);
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
        
        public ActionResult GetEnergynData(int? stationKey, int? type)
        {
            DateTime nowDate = DateTime.Now;
            JArray name = JArray.FromObject(new string[] { "千吨水耗电量", "电量" });
            JArray result = new JArray();
            JArray data1 = new JArray();
            JArray data2 = new JArray();
            JArray time = new JArray();
            List<GetDataType_Result> list = new List<GetDataType_Result>();
            //var sds = bsthapp.GetList(areaId, cycle, startDate, endDate);
            //foreach (var sd in sds)
            //{
            //    GetDataType_Result rdr = new GetDataType_Result()
            //    {
            //        Name = sd.Explain,
            //        Value = sd.Meter_Type.ToString()
            //    };
            //    list.Add(rdr);
            //}
            for (int i = 0; i < 21; i++)
            {
                Random rd = new Random();
                int rd1 = rd.Next(330, 440);
                int rd2 = rd.Next(1500, 6050);
                System.Threading.Thread.Sleep(50);
                data1.Add(rd1);
                data2.Add(rd2);
                switch (type)
                {
                    case 1:
                        time.Add(nowDate.AddHours(-20 + i).ToDateString() + " " + nowDate.AddHours(-20 + i).Hour + ":00");
                        break;
                    case 2:
                        time.Add(nowDate.AddDays(-20 + i).ToDateString());
                        break;
                    case 3:
                        time.Add(nowDate.AddMonths(-20 + i).ToString("yyyy-MM"));
                        break;
                    case 4:
                        time.Add(nowDate.AddYears(-20 + i).ToString("yyyy"));
                        break;
                    default:
                        time.Add(nowDate.AddHours(-20 + i).ToDateString());
                        break;
                }
            }
            JArray tmp = new JArray();
            tmp.Add(data1);
            tmp.Add(data2);
            result.Add(tmp);
            result.Add(time);
            result.Add(name);
            return Content(result.ToJson());
        }

        /// <summary>
        /// 获取历史数据
        /// </summary>
        /// <returns></returns>
        
        public ActionResult GetHistoryData(string sTagKeys)
        {
            int stKey = -1;//分类判断依据
            JArray result = new JArray();
            JArray items1 = new JArray();
            JArray name1 = new JArray();
            JArray unit1 = new JArray();
            JArray items2 = new JArray();
            JArray name2 = new JArray();
            JArray unit2 = new JArray();
            var stationTagKeys = sTagKeys.Split(',');
            TJWCCDbContext dbcontext = new TJWCCDbContext();
            try
            {
                dbcontext.Database.Connection.Open();
            }
            catch (Exception ex)
            {
                loger.Error(ex.Message);
                loger.Error(ex.Source + " |" + ex.StackTrace);
            }
            DbCommand cmd = dbcontext.Database.Connection.CreateCommand();
            cmd.CommandTimeout = int.MaxValue;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "UPDATE [dbo].[BS_SCADA_TAG_CURRENT] SET Selected=0 WHERE Selected=1";
            try
            {
                cmd.ExecuteNonQuery();
                dbcontext.Database.Connection.Close();
            }
            catch (Exception ex)
            {
                loger.Error(ex.Message);
                loger.Error(ex.Source + " |" + ex.StackTrace);
            }
            dbcontext.Database.Connection.Close();
            //dbcontext.Database.SqlQuery<int>("UPDATE [dbo].[BS_SCADA_TAG_CURRENT] SET Selected=0 WHERE Selected=1");
            foreach (var stationTagKey in stationTagKeys)
            {
                var sTKey = stationTagKey.Split('_');
                var bstc = scadaCurrentApp.GetList(sTKey[0], sTKey[1], sTKey[2].ToInt());
                bstc.Selected = 1;
                scadaCurrentApp.SubmitForm(bstc, bstc.ID.ToString());
                if (stKey == -1) stKey = Convert.ToInt32(sTKey[1]);
                decimal max = decimal.MinValue;
                decimal min = decimal.MaxValue;
                JArray item1 = new JArray();
                JArray item2 = new JArray();
                DateTime endDate = bstc.Save_date.Value;
                DateTime startDate = endDate.AddDays(-30);
                var dataHiss = scadaHisApp.GetList(sTKey[0], sTKey[1], sTKey[2].ToInt(), startDate, endDate).OrderBy(i => i.Save_date).ToArray();
                foreach (var dataHis in dataHiss)
                {
                    JObject dataItems = new JObject();
                    JArray dataItem = new JArray();
                    dataItem.Add(dataHis.Save_date.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                    switch (sTKey[1])
                    {
                        case "1":
                        case "8":
                        case "9":
                        case "111":
                        case "94"://库容
                            dataItem.Add(dataHis.Tag_value.Value.ToString("F0"));
                            if (dataHis.Tag_value.Value > max) max = dataHis.Tag_value.Value;
                            if (dataHis.Tag_value.Value < min) min = dataHis.Tag_value.Value;
                            break;
                        case "2":
                            dataItem.Add(dataHis.Tag_value.Value.ToString("F3"));
                            max = 0.7m;
                            min = 0;
                            break;
                        case "4":
                        case "12":
                        case "42":
                        case "511":
                            dataItem.Add(dataHis.Tag_value.Value.ToString("F2"));
                            max = 20m;
                            min = 0;
                            break;
                        case "3":
                        case "11":
                        case "40":
                        case "41":
                            dataItem.Add(dataHis.Tag_value.Value.ToString("F2"));
                            max = 3m;
                            min = 0;
                            break;
                        case "14":
                        case "44":
                        case "501":
                            dataItem.Add(dataHis.Tag_value.Value.ToString("F2"));
                            max = 5m;
                            min = 0;
                            break;
                        case "91"://液位
                        case "92":
                        case "93":
                            max = 15;
                            min = 0;
                            dataItem.Add(dataHis.Tag_value.Value.ToString("F2"));
                            break;
                        default:
                            dataItem.Add(dataHis.Tag_value.Value.ToString("F2"));
                            if (dataHis.Tag_value.Value > max) max = dataHis.Tag_value.Value;
                            if (dataHis.Tag_value.Value < min) min = dataHis.Tag_value.Value;
                            break;
                    }
                    dataItems.Add("value", dataItem);
                    item1.Add(dataItems);
                }
                string tmp = bstc.Station_key;
                string tagKey = bstc.Tag_key.ToString();
                unit1.Add(scadaInfoApp.GetEntity(tagKey).Units);
                if (string.IsNullOrWhiteSpace(bstc.ValueName))
                    name1.Add(bmiApp.GetEntity(tmp).Meter_Name);
                else
                    name1.Add(bstc.ValueName);
                items1.Add(item1);
                items2.Add(max);
                name2.Add(min);
            }
            result.Add(items1);
            result.Add(name1);
            result.Add(unit1);
            result.Add(items2);
            result.Add(name2);
            result.Add(unit2);
            return Content(result.ToJson());
        }

    }
}