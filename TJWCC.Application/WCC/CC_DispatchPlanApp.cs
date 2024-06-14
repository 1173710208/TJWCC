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
using System.Data.SqlClient;
using TJWCC.Data;
using System.Data.Common;
using System.Data;
using DGSWDC.Code;
using System.Collections;
using Newtonsoft.Json.Linq;
using TJWCC.Application.SystemManage;
using TJWCC.Domain.IRepository.BSSys;
using TJWCC.Repository.BSSys;
using TJWCC.Domain.ViewModel;

namespace TJWCC.Application.WCC
{

    public class CC_DispatchPlanApp
    {
        TJWCCDbContext dbcontext = new TJWCCDbContext();
        private ICC_DispatchPlanRepository service = new CC_DispatchPlanRepository();
        private IBS_SCADA_TAG_CURRENTRepository stcService = new BS_SCADA_TAG_CURRENTRepository();
        private ICC_FlowSortRepository fsService = new CC_FlowSortRepository();
        private IBSB_ForecastFlowRepository bffService = new BSB_ForecastFlowRepository();

        /// <summary>
        /// 分页查询调度日志数据
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="dType"></param>
        /// <returns></returns>
        public List<CC_DispatchPlanEntity> GetList(Pagination pagination, DateTime? sdDate, DateTime? edDate, DateTime? seDate, DateTime? eeDate, int? shift, string oper, decimal? devi, string keyword)
        {
            var expression = ExtLinq.True<CC_DispatchPlanEntity>();
            expression = expression.And(t => t.DispatchType == 2);
            if (sdDate != null || edDate != null)
            {
                if (sdDate == null && edDate != null)
                    expression = expression.And(i => i.IssueDate <= edDate);
                else if (sdDate != null && edDate == null)
                    expression = expression.And(i => i.IssueDate >= sdDate);
                else
                    expression = expression.And(i => i.IssueDate >= sdDate && i.IssueDate <= edDate);
            }
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                expression = expression.And(t => t.AccordingTo.Contains(keyword));
            }
            if (shift != null)
            {
                expression = expression.And(t => t.Shift == shift);
            }
            if (!string.IsNullOrWhiteSpace(oper))
            {
                expression = expression.And(t => t.Operator.Contains(oper));
            }
            if (devi != null)
            {
                expression = expression.And(t => t.Deviation == devi);
            }
            return service.FindList(expression, pagination).ToList();

        }
        public List<CC_DispatchPlanEntity> GetList(DateTime? sdDate, DateTime? edDate, int? dType, string pressList)
        {
            string where = "WHERE IsPlanBase=1";//流量计数据查询条件
            if (sdDate != null || edDate != null)
            {
                if (sdDate == null && edDate != null)
                {
                    where += " AND IssueDate<='" + edDate + "'";
                }
                else if (sdDate != null && edDate == null)
                {
                    where += " AND IssueDate>='" + sdDate + "'";
                }
                else
                {
                    where += " AND IssueDate>='" + sdDate + "' AND IssueDate<='" + edDate + "'";
                }
            }
            if (dType != null)
            {
                where += " AND DispatchType='" + dType + "'";
            }
            if (!string.IsNullOrWhiteSpace(pressList))
            {
                var ss = pressList.Split(',');
                if (dType == 7)
                    for (var i = 0; i < ss.Length; i++)
                    {
                        string tmp = "";
                        switch (ss[i])
                        {
                            case "10": tmp = "芥园水厂"; break;
                            case "11": tmp = "凌庄水厂"; break;
                            case "12": tmp = "新开河水厂"; break;
                            case "13": tmp = "津滨水厂"; break;
                        }
                        where += " AND(Status LIKE'" + tmp + ":%' OR Status LIKE'%;" + tmp + ":%')";
                    }
                else
                    for (var i = 0; i < ss.Length; i++)
                    {
                        where += " AND(AccordingTo LIKE'" + ss[i] + ",%' OR AccordingTo LIKE'%;" + ss[i] + ",%')";
                    }
            }
            var ccdp = dbcontext.Database.SqlQuery<CC_DispatchPlanEntity>("SELECT * FROM CC_DispatchPlan "+ where).ToList();
            return ccdp;
        }
        /// <summary>
        /// 分页查询调度指令数据
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="sdDate"></param>
        /// <param name="edDate"></param>
        /// <param name="seDate"></param>
        /// <param name="eeDate"></param>
        /// <returns></returns>
        public List<CC_DispatchPlanEntity> GetOrderList(Pagination pagination, DateTime? sdDate, DateTime? edDate, DateTime? seDate, DateTime? eeDate, int? dType, string status, int? isPlan)
        {
            var expression = ExtLinq.True<CC_DispatchPlanEntity>();
            if (sdDate != null || edDate != null)
            {
                if (sdDate == null && edDate != null)
                    expression = expression.And(i => i.IssueDate <= edDate);
                else if (sdDate != null && edDate == null)
                    expression = expression.And(i => i.IssueDate >= sdDate);
                else
                    expression = expression.And(i => i.IssueDate >= sdDate && i.IssueDate <= edDate);
            }
            if (seDate != null || eeDate != null)
            {
                if (seDate == null && eeDate != null)
                    expression = expression.And(i => i.ExecDate <= eeDate);
                else if (seDate != null && eeDate == null)
                    expression = expression.And(i => i.ExecDate >= seDate);
                else
                    expression = expression.And(i => i.ExecDate >= seDate && i.ExecDate <= eeDate);
            }
            if (dType != null)
            {
                expression = expression.And(t => t.DispatchType == dType);
            }
            if (!string.IsNullOrWhiteSpace(status))
            {
                expression = expression.And(t => t.Status.Contains(status));
            }
            if (isPlan != null)
            {
                expression = expression.And(t => t.IsPlanBase == isPlan);
            }
            var tmp = new int?[] { 1, 3, 7 };
            expression = expression.And(t => tmp.Contains(t.DispatchType));
            return service.FindList(expression, pagination).ToList();

        }
        public CC_DispatchPlanEntity GetLowPlan(string info,DateTime DataDate)
        {
            var dplan = service.FindList("SELECT * FROM CC_DispatchPlan WHERE DispatchType=6 AND DataDate='" + DataDate + "' AND AccordingTo LIKE'" + info + "%'").FirstOrDefault();
            return dplan;

        }
        /// <summary>
        /// 根据方案类型获取最新方案
        /// </summary>
        /// <param name="type">1:日常 3:水厂停减产 4:爆管应急 5:水质污染应急 6:压力突降应急 7:泵站优化</param>
        /// <returns></returns>
        public CC_DispatchPlanEntity GetNewPlan(int? type)
        {
            //var dutyId = OperatorProvider.Provider.GetCurrent().DutyId;//班次
            var dplan = service.FindList("SELECT * FROM [dbo].[CC_DispatchPlan] WHERE ID=(SELECT MAX(ID) FROM CC_DispatchPlan WHERE DispatchType=" + type + ")").FirstOrDefault();
            return dplan;

        }
        public CC_DispatchPlanEntity GetListById(int? dpid)
        {
            var expression = ExtLinq.True<CC_DispatchPlanEntity>();
            expression = expression.And(i => i.ID == dpid);
            return service.IQueryable(expression).FirstOrDefault();

        }
        public JArray GetRealtimeList()
        {
            DateTime nowDate = DateTime.Now;
            TimeSpan time = new TimeSpan(nowDate.Hour, 0, 0);
            int type = new CC_TheDateApp().GetTheDate(Convert.ToDateTime(DateTime.Now.ToDateString())).Type;//是否工作日
            JArray result = new JArray();
            string sqlRe = "SELECT ElementID,WMeter_ID,Meter_Name,Station_Unit,Geo_x,Geo_y,[Explain],(CASE WHEN Remark IS NULL THEN CONVERT(VARCHAR(10),Tag_value) ELSE Remark END) Remark," +
                "Tag_value,Save_date,(CASE WHEN Remark IS NULL THEN '' ELSE '1' END) Measure_Grade FROM(SELECT ElementID,WMeter_ID,Meter_Name,ltrim(Station_Unit) Station_Unit,Geo_x,Geo_y," +
                "[Explain],(SELECT CONVERT(VARCHAR(10),[value]) [value] FROM CC_PressTarget a WHERE a.Station_Key=WMeter_ID AND PlanTime='" + time + "' AND type="+ type + " AND Selected=1) " +
                "Remark,Tag_value,Save_date,Measure_Grade FROM (SELECT ElementID, WMeter_ID, Meter_Name, Station_Unit, Geo_x, Geo_y, [Explain],Station_Key,Meter_Type,Remark,Measure_Grade " +
                "FROM [dbo].[BSM_Meter_Info] WHERE Meter_Type =2 AND Display=1) bmi LEFT JOIN (SELECT (CASE WHEN Tag_value is null THEN 0 ELSE Tag_value END) Tag_value,Station_Key,Save_date," +
                "Tag_key FROM BS_SCADA_TAG_CURRENT where Tag_key =2) bth ON bmi.Station_Key = bth.Station_key and bmi.Meter_Type = bth.Tag_key) a ORDER BY Meter_Name";
            var pressRe = dbcontext.Database.SqlQuery<GetNewData_Result>(sqlRe).ToArray();
            sqlRe = "SELECT ElementID,WMeter_ID,Meter_Name,ltrim(Station_Unit) Station_Unit,Geo_x,Geo_y,[Explain],Remark,Tag_value,Save_date,Measure_Grade FROM " +
                "(SELECT ElementID, WMeter_ID, Meter_Name, Station_Unit, Geo_x, Geo_y, [Explain],Station_Key,Meter_Type,Remark,Measure_Grade FROM [dbo].[BSM_Meter_Info] " +
                "WHERE Meter_Type =1 AND Display=1) bmi LEFT JOIN (SELECT (CASE WHEN Tag_value is null THEN 0 ELSE Tag_value END) Tag_value,Station_Key,Save_date,Tag_key " +
                "FROM BS_SCADA_TAG_CURRENT where Tag_key =1) bth ON bmi.Station_Key = bth.Station_key and bmi.Meter_Type = bth.Tag_key ORDER BY Meter_Name";
            var flowRe = dbcontext.Database.SqlQuery<GetNewData_Result>(sqlRe).ToArray();
            sqlRe = "SELECT bmi.ElementID,bmi.WMeter_ID,bmi.Meter_Name,ltrim(bmi.Station_Unit) Station_Unit,Geo_x,Geo_y,[Explain],Remark,Tag_value,Save_date,Measure_Grade FROM " +
                "(SELECT ElementID, WMeter_ID, Meter_Name, Station_Unit, Geo_x, Geo_y, [Explain],Station_Key,Meter_Type,Remark,Measure_Grade FROM [dbo].[BSM_Meter_Info] " +
                "WHERE (Meter_Type =3 OR Meter_Type =4) AND Display=1) bmi LEFT JOIN (SELECT (CASE WHEN Tag_value is null THEN 0 ELSE Tag_value END) Tag_value,Station_Key,Save_date,Tag_key " +
                "FROM BS_SCADA_TAG_CURRENT WHERE Tag_key =3 OR Tag_key =4) bth ON bmi.Station_Key = bth.Station_key and bmi.Meter_Type = bth.Tag_key ORDER BY Meter_Name";
            var bsgndsRe = dbcontext.Database.SqlQuery<GetNewData_Result>(sqlRe).ToArray();
            var WMeterIDsRe = bsgndsRe.Select(item => item.WMeter_ID).Distinct().ToArray();
            var QualitysRe = new JArray();
            for (int i = 0; i < WMeterIDsRe.Count(); i++)
            {
                var tbsgnd = new JObject();
                var tbsgnds = bsgndsRe.Where(item => item.WMeter_ID == WMeterIDsRe[i]).ToArray();
                string[] Explains = new string[2];
                decimal?[] Tag_values = new decimal?[2];
                for (int x = 0; x < tbsgnds.Count(); x++)
                {
                    switch (tbsgnds[x].Explain)
                    {
                        case "浊度":
                            Explains[0] = "(" + ArrayDataTableHelper.TurbUnit + ")";
                            Tag_values[0] = tbsgnds[x].Tag_value;
                            break;
                        case "余氯":
                            Explains[1] = "(" + ArrayDataTableHelper.ClUnit + ")";
                            Tag_values[1] = tbsgnds[x].Tag_value;
                            break;
                    }
                }
                tbsgnd.Add("ElementID", tbsgnds[0].ElementID);
                tbsgnd.Add("WMeter_ID", tbsgnds[0].WMeter_ID);
                tbsgnd.Add("Meter_Name", tbsgnds[0].Meter_Name.Replace(tbsgnds[0].Explain, ""));
                tbsgnd.Add("Geo_x", tbsgnds[0].Geo_x);
                tbsgnd.Add("Geo_y", tbsgnds[0].Geo_y);
                tbsgnd.Add("Explain", JArray.FromObject(Explains));
                tbsgnd.Add("Save_date", tbsgnds[0].Save_date);
                tbsgnd.Add("Station_Unit", tbsgnds[0].Station_Unit);
                tbsgnd.Add("Tag_value", JArray.FromObject(Tag_values));
                QualitysRe.Add(tbsgnd);
            }
            JArray allFlow = new JArray();
            var fs = fsService.FindList("SELECT * FROM(SELECT 0 ID, MAX(Save_date) Save_date,0 Sort_key, SUM(Tag_value) Sort_value,1 Type,NULL Remark FROM BS_SCADA_TAG_CURRENT WHERE Station_key " +
                "in(SELECT Station_Key FROM [dbo].[BSM_Meter_Info] WHERE Meter_Type='9')) a WHERE Save_date IS NOT NULL AND ID IS NOT NULL");
            //var fs = fsService.FindList("SELECT * FROM(SELECT 0 ID, MAX(Save_date) Save_date,0 Sort_key, SUM(Tag_value) Sort_value,1 Type,NULL Remark FROM BS_SCADA_TAG_HIS WHERE Station_key " +
            //    "in(SELECT Station_Key FROM [dbo].[BSM_Meter_Info] WHERE Meter_Type='9') AND Save_date='" + nowDate.ToString("yyyy-MM-dd HH") + ":00:00') a WHERE ID IS NOT NULL");
            JArray fsj = new JArray();
            if (fs.Count > 0)
            {
                fsj.Add(fs[0].Save_date.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                fsj.Add(fs[0].Sort_value.ToDecimal(3));
            }
            else
            {
                fsj.Add(nowDate.ToString("yyyy-MM-dd HH") + ":00:00");
                fsj.Add(null);
            }
            allFlow.Add(fsj);
            var bff = bffService.FindList("SELECT * FROM [dbo].[BSB_ForecastFlow] WHERE MoreAlert=1 AND DMA=0 AND FlowTime='" + nowDate.AddHours(1).ToString("yyyy-MM-dd HH") + ":00:00'");
            JArray bffj = new JArray();
            if (bff.Count > 0)
            {
                bffj.Add(bff[0].FlowTime.ToString("yyyy-MM-dd HH:mm:ss"));
                bffj.Add(bff[0].ForecastFlow.ToDecimal(3));
            }
            else
            {
                bffj.Add(nowDate.AddHours(1).ToString("yyyy-MM-dd HH") + ":00:00");
                bffj.Add(null);
            }
            allFlow.Add(bffj);
            result.Add(JArray.FromObject(pressRe));//压力监测点数据
            result.Add(JArray.FromObject(flowRe));//流量监测点数据
            result.Add(JArray.FromObject(QualitysRe));//水质监测点数据
            result.Add(allFlow);//全市当前水量和预测水量
            return result;
        }
        public JArray GetList()
        {
            JArray result = new JArray();
            var dutyId = OperatorProvider.Provider.GetCurrent().DutyId;//班次
            var dplan = service.FindList("SELECT * FROM [dbo].[CC_DispatchPlan] WHERE ID=(SELECT MAX(ID) FROM CC_DispatchPlan WHERE Shift=" + dutyId + " AND DispatchType=1)").FirstOrDefault();
            JArray accordingTo = new JArray();
            var asss = string.IsNullOrWhiteSpace(dplan.AccordingTo) ? new string[] { } : dplan.AccordingTo.Split(';');
            foreach (var ass in asss)
            {
                JArray tmp = new JArray();
                tmp.Add(ass.Split(','));
                accordingTo.Add(tmp);
            }
            JArray dispatchOrder = new JArray();
            var dsss =string.IsNullOrWhiteSpace(dplan.DispatchOrder)?new string[] { } :dplan.DispatchOrder.Split(';');
            foreach (var dss in dsss)
            {
                JArray tmp = new JArray();
                tmp.Add(dss.Split(','));
                dispatchOrder.Add(tmp);
            }
            JArray allFlow = new JArray();
            //var fs = fsService.FindList("SELECT * FROM [dbo].[CC_FlowSort] WHERE Sort_key=0 AND Save_date='" + dplan.DataDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "' AND Type=1");
            var fs = fsService.FindList("SELECT * FROM(SELECT 0 ID, MAX(Save_date) Save_date,0 Sort_key, SUM(Tag_value) Sort_value,1 Type,NULL Remark FROM BS_SCADA_TAG_HIS WHERE Station_key " +
                "in(SELECT Station_Key FROM [dbo].[BSM_Meter_Info] WHERE Meter_Type='9') AND Save_date='" + dplan.DataDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "') a WHERE " +
                "Save_date IS NOT NULL AND ID IS NOT NULL");
            JArray fsj = new JArray();
            if (fs.Count > 0)
            {
                fsj.Add(fs[0].Save_date.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                fsj.Add(fs[0].Sort_value.ToDecimal(3));
            }
            else
            {
                fsj.Add(dplan.DataDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                fsj.Add(null);
            }
            allFlow.Add(fsj);
            var bff = bffService.FindList("SELECT * FROM [dbo].[BSB_ForecastFlow] WHERE MoreAlert=1 AND DMA=0 AND FlowTime='" + dplan.DataDate.Value.AddHours(1).ToString("yyyy-MM-dd HH") + ":00:00'");
            JArray bffj = new JArray();
            if (bff.Count > 0)
            {
                bffj.Add(bff[0].FlowTime.ToString("yyyy-MM-dd HH:mm:ss"));
                bffj.Add(bff[0].ForecastFlow.ToDecimal(3));
            }
            else
            {
                bffj.Add(dplan.DataDate.Value.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"));
                bffj.Add(null);
            }
            allFlow.Add(bffj); 
            result.Add(allFlow);//全市当前水量和预测水量
            result.Add(accordingTo);//调度依据压力监测点数据和执行效果
            result.Add(dispatchOrder);//调度方案和调度指令数据
            result.Add(dplan.DataDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));//监测点数据时间
            result.Add(dplan.ID.ToString());//调度方案ID
            result.Add(dplan.CreateDate == null ? "" : dplan.CreateDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));//调度方案生成时间
            result.Add(dplan.IssueDate == null ? "" : dplan.IssueDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));//调度方案下发时间
            result.Add(dplan.ExecDate == null ? "" : dplan.ExecDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));//调度方案执行时间
            result.Add(dplan.Deviation == null ? "" : dplan.Deviation.Value.ToString());//调度方案平均偏差率

            return result;
        }
        public JArray GetList(int? ID)
        {
            JArray result = new JArray();
            var dplan = service.FindList("SELECT * FROM [dbo].[CC_DispatchPlan] WHERE ID='" + ID + "'").FirstOrDefault();
            JArray accordingTo = new JArray();
            if (!string.IsNullOrWhiteSpace(dplan.AccordingTo))
            {
                var asss = dplan.AccordingTo.Split(';');
                foreach (var ass in asss)
                {
                    JArray tmp = new JArray();
                    tmp.Add(ass.Split(','));
                    accordingTo.Add(tmp);
                }
            }
            JArray dispatchOrder = new JArray();
            if (!string.IsNullOrWhiteSpace(dplan.DispatchOrder))
            {
                var dsss = dplan.DispatchOrder.Split(';');
                foreach (var dss in dsss)
                {
                    JArray tmp = new JArray();
                    tmp.Add(dss.Split(','));
                    dispatchOrder.Add(tmp);
                }
            }
            string sql = "SELECT * FROM(SELECT bmi.ElementID,bmi.WMeter_ID,bmi.Meter_Name,ltrim(bmi.Station_Unit) Station_Unit,Geo_x,Geo_y,[Explain],Remark,Tag_value,Save_date,Measure_Grade FROM " +
                "(SELECT ElementID,CONVERT(varchar(20),WMeter_ID) WMeter_ID,Meter_Name,Station_Unit,Geo_x,Geo_y,[Explain],Station_Key,Meter_Type,Remark,Measure_Grade FROM [dbo].[BSM_Meter_Info] " +
                "WHERE Meter_Type =2 AND Display=1) bmi LEFT JOIN (SELECT (CASE WHEN Tag_value is null THEN 0 ELSE Tag_value END) Tag_value,Station_Key,Save_date,Tag_key " +
                "FROM BS_SCADA_TAG_HIS WHERE ID in(SELECT MAX(id) ID FROM [dbo].[BS_SCADA_TAG_HIS] WHERE Save_date<='" + dplan.DataDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "' AND Tag_key='2' " +
                "GROUP BY Station_key) AND Save_date>CONVERT(Datetime,'" + dplan.DataDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "')-1.0/24.0) bth ON bmi.Station_Key = bth.Station_key AND " +
                "bmi.Meter_Type = bth.Tag_key) s WHERE Tag_value IS NOT NULL ORDER BY Meter_Name";
            var press = dbcontext.Database.SqlQuery<GetNewData_Result>(sql).ToArray();
            sql = "SELECT * FROM(SELECT bmi.ElementID,bmi.WMeter_ID,bmi.Meter_Name,ltrim(bmi.Station_Unit) Station_Unit,bmi.Geo_x,bmi.Geo_y,[Explain],Remark,Tag_value,Save_date,Measure_Grade FROM " +
                "(SELECT ElementID,CONVERT(varchar(20),WMeter_ID) WMeter_ID,Meter_Name,Station_Unit,Geo_x,Geo_y,[Explain],Station_Key,Meter_Type,Remark,Measure_Grade FROM [dbo].[BSM_Meter_Info] " +
                "WHERE Meter_Type =1 AND Display=1) bmi LEFT JOIN (SELECT (CASE WHEN Tag_value is null THEN 0 ELSE Tag_value END) Tag_value,Station_Key,Save_date,Tag_key " +
                "FROM BS_SCADA_TAG_HIS where Tag_key =1 AND Save_date='" + dplan.DataDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "') bth ON bmi.Station_Key = bth.Station_key " +
                "AND bmi.Meter_Type = bth.Tag_key) s WHERE Tag_value IS NOT NULL ORDER BY Meter_Name";
            var flow = dbcontext.Database.SqlQuery<GetNewData_Result>(sql).ToArray();
            sql = "SELECT * FROM(SELECT bmi.ElementID,bmi.WMeter_ID,bmi.Meter_Name,ltrim(bmi.Station_Unit) Station_Unit,bmi.Geo_x,bmi.Geo_y,[Explain],Remark,Tag_value,Save_date,Measure_Grade FROM " +
                "(SELECT ElementID,CONVERT(varchar(20),WMeter_ID) WMeter_ID,Meter_Name,Station_Unit,Geo_x,Geo_y,[Explain],Station_Key,Meter_Type,Remark,Measure_Grade FROM [dbo].[BSM_Meter_Info] " +
                "WHERE (Meter_Type =3 OR Meter_Type =4) AND Display=1) bmi LEFT JOIN (SELECT (CASE WHEN Tag_value is null THEN 0 ELSE Tag_value END) Tag_value,Station_Key,Save_date,Tag_key " +
                "FROM BS_SCADA_TAG_HIS WHERE ID in(SELECT MAX(id) ID FROM [dbo].[BS_SCADA_TAG_HIS] WHERE Save_date<='" + dplan.DataDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "' AND (Tag_key='3' " +
                " OR Tag_key='4') GROUP BY Station_key) AND Save_date>CONVERT(Datetime,'" + dplan.DataDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "')-1.0/24.0) bth ON bmi.Station_Key = bth.Station_key " +
                "AND bmi.Meter_Type = bth.Tag_key) s WHERE Tag_value IS NOT NULL ORDER BY Meter_Name";
            var bsgnds = dbcontext.Database.SqlQuery<GetNewData_Result>(sql).ToArray();
            var Qualitys = new JArray();
            if (bsgnds != null)
            {
                var ElementIDs = bsgnds.Where(i => i.ElementID != null).Select(item => item.ElementID.Value).Distinct().ToArray();
                for (int i = 0; i < ElementIDs.Count(); i++)
                {
                    var tbsgnd = new JObject();
                    var tbsgnds = bsgnds.Where(item => item.ElementID == ElementIDs[i]).ToArray();
                    string[] Explains = new string[2];
                    decimal?[] Tag_values = new decimal?[2];
                    for (int x = 0; x < tbsgnds.Count(); x++)
                    {
                        switch (tbsgnds[x].Explain)
                        {
                            case "浊度":
                                Explains[0] = "(" + ArrayDataTableHelper.TurbUnit + ")";
                                Tag_values[0] = tbsgnds[x].Tag_value;
                                break;
                            case "余氯":
                                Explains[1] = "(" + ArrayDataTableHelper.ClUnit + ")";
                                Tag_values[1] = tbsgnds[x].Tag_value;
                                break;
                        }
                    }
                    tbsgnd.Add("ElementID", tbsgnds[0].ElementID);
                    tbsgnd.Add("WMeter_ID", tbsgnds[0].WMeter_ID);
                    tbsgnd.Add("Meter_Name", tbsgnds[0].Meter_Name.Replace(tbsgnds[0].Explain, ""));
                    tbsgnd.Add("Geo_x", tbsgnds[0].Geo_x);
                    tbsgnd.Add("Geo_y", tbsgnds[0].Geo_y);
                    tbsgnd.Add("Explain", JArray.FromObject(Explains));
                    tbsgnd.Add("Save_date", tbsgnds[0].Save_date);
                    tbsgnd.Add("Station_Unit", tbsgnds[0].Station_Unit);
                    tbsgnd.Add("Tag_value", JArray.FromObject(Tag_values));
                    Qualitys.Add(tbsgnd);
                }
            }
            JArray allFlow = new JArray();
            if (dplan.DispatchType == 3)
            {
                var tmpe = dplan.Effect.Split(';');
                JArray fsj = new JArray();
                fsj.Add(tmpe[0].Split(',')[0]);
                fsj.Add(tmpe[0].Split(',')[1]);
                allFlow.Add(fsj);
                JArray bffj = new JArray();
                bffj.Add(tmpe[1].Split(',')[0]);
                bffj.Add(tmpe[1].Split(',')[1]);
                allFlow.Add(bffj);
            }
            else
            {
                //var fs = fsService.FindList("SELECT * FROM [dbo].[CC_FlowSort] WHERE Sort_key=0 AND Save_date='" + dplan.DataDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "' AND Type=1");
                var fs = fsService.FindList("SELECT * FROM(SELECT 0 ID, MAX(Save_date) Save_date,0 Sort_key, SUM(Tag_value) Sort_value,1 Type,NULL Remark FROM BS_SCADA_TAG_HIS WHERE Station_key " +
                    "in(SELECT Station_Key FROM [dbo].[BSM_Meter_Info] WHERE Meter_Type='9') AND Save_date='" + dplan.DataDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "') a WHERE " +
                    "Save_date IS NOT NULL AND ID IS NOT NULL");
                JArray fsj = new JArray();
                if (fs.Count > 0)
                {
                    fsj.Add(fs[0].Save_date.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                    fsj.Add(fs[0].Sort_value.ToDecimal(3));
                }
                allFlow.Add(fsj);
                var bff = bffService.FindList("SELECT * FROM [dbo].[BSB_ForecastFlow] WHERE MoreAlert=1 AND DMA=0 AND FlowTime='" + dplan.DataDate.Value.AddHours(1).ToString("yyyy-MM-dd HH") + ":00:00'");
                JArray bffj = new JArray();
                if (bff.Count > 0)
                {
                    bffj.Add(bff[0].FlowTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    bffj.Add(bff[0].ForecastFlow.ToDecimal(3));
                }
                allFlow.Add(bffj);
            }
            result.Add(JArray.FromObject(press));//压力监测点数据
            result.Add(JArray.FromObject(flow));//流量监测点数据
            result.Add(JArray.FromObject(Qualitys));//水质监测点数据
            result.Add(allFlow);//全市当前水量和预测水量
            result.Add(accordingTo);//调度依据压力监测点数据和执行效果
            result.Add(dispatchOrder);//调度方案和调度指令数据
            result.Add(dplan.DataDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));//监测点数据时间
            result.Add(dplan.ID.ToString());//调度方案ID
            result.Add(dplan.CreateDate == null ? "" : dplan.CreateDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));//调度方案生成时间
            result.Add(dplan.IssueDate == null ? "" : dplan.IssueDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));//调度方案下发时间
            result.Add(dplan.ExecDate == null ? "" : dplan.ExecDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));//调度方案执行时间
            result.Add(dplan.Deviation == null ? "" : dplan.Deviation.Value.ToString());//调度方案平均偏差率
            result.Add(dplan.Shift.Value == 1 ? "早班" : "晚班");//班次
            result.Add(dplan.Operator);//操作员
            result.Add(dplan.Status);//调令状态
            result.Add(dplan.Effect);
            if (dplan.LinkedMeters != null)
                result.Add(JArray.FromObject(dplan.LinkedMeters.Split(',')));//供水水厂

            return result;
        }

        /// <summary>
        /// 获取所有调度指令数据
        /// </summary>
        /// <returns></returns>
        public List<CC_DispatchPlanEntity> GetAllDispatchPlanList()
        {
            var reslut = service.IQueryable().ToList();
            return reslut;
        }

        /// <summary>
        /// 根据id删除调度指令数据记录
        /// </summary>
        /// <param name="keyValue"></param>
        public void DeleteForm(int id)
        {
            service.Delete(t => t.ID == id);
        }

        /// <summary>
        /// 添加或更新调度指令
        /// </summary>
        /// <param name="moduleEntity">数据实体信息</param>
        /// <param name="userID">用户编号</param>
        public void SubmitForm(CC_DispatchPlanEntity moduleEntity, int? id)
        {
            if (id != null)
            {
                service.Update(moduleEntity);
            }
            else
            {
                service.Insert(moduleEntity);
            }
        }
        public string GetDownload(DateTime? sdDate, DateTime? edDate, DateTime? seDate, DateTime? eeDate, int? shift, string oper, decimal? devi, int? status, int? isPlan, string keyword)
        {
            string where = "WHERE DispatchType=2";//数据查询条件
            if (sdDate != null || edDate != null)
            {
                if (sdDate == null && edDate != null)
                {
                    where += " AND IssueDate<='" + edDate + "'";
                }
                else if (sdDate != null && edDate == null)
                {
                    where += " AND IssueDate>='" + sdDate + "'";
                }
                else
                {
                    where += " AND IssueDate>='" + sdDate + "' AND IssueDate<='" + edDate + "'";
                }
            }
            if (seDate != null || eeDate != null)
            {
                if (seDate == null && eeDate != null)
                {
                    where += " AND ExecDate<='" + eeDate + "'";
                }
                else if (seDate != null && eeDate == null)
                {
                    where += " AND ExecDate>='" + seDate + "'";
                }
                else
                {
                    where += " AND ExecDate>='" + seDate + "' AND ExecDate<='" + eeDate + "'";
                }
            }
            if (shift != null)
            {
                where += " AND Shift='" + shift + "'";
            }
            if (!string.IsNullOrEmpty(oper))
            {
                where += " AND Operator LIKE'%" + oper + "%'";
            }
            if (devi != null)
            {
                where += " AND Deviation='" + devi + "'";
            }
            if (status != null)
            {
                where += " AND Status='" + status + "'";
            }
            if (isPlan != null)
            {
                where += " AND IsPlanBase=" + isPlan;
            }
            string sql = "SELECT IssueDate 日期时间,(SELECT FULLNAME FROM SYS_ROLE WHERE ENCODE=CONVERT(VARCHAR(5),Shift)) 班次,Operator 操作人员," +
                "Effect 类型,AccordingTo 值班记录 FROM CC_DispatchPlan " + where;
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
                FilePath = "/file/值班日志" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx",
                SheetNames = new string[] { "调度值班日志" },
                Titles = new string[] { "调度值班日志" }
            };
            excel.AddTables(tmp1);
            excel.CreateExcel();
            return excel.FilePath;

        }
        public string GetOrderDownload(DateTime? sdDate, DateTime? edDate, int? dType, DateTime? seDate, DateTime? eeDate,string status, int? isPlan)
        {
            string where = "WHERE DispatchType IN(1,3,7)";//流量计数据查询条件
            if (sdDate != null || edDate != null)
            {
                if (sdDate == null && edDate != null)
                {
                    where += " AND IssueDate<='" + edDate + "'";
                }
                else if (sdDate != null && edDate == null)
                {
                    where += " AND IssueDate>='" + sdDate + "'";
                }
                else
                {
                    where += " AND IssueDate>='" + sdDate + "' AND IssueDate<='" + edDate + "'";
                }
            }
            if (seDate != null || eeDate != null)
            {
                if (seDate == null && eeDate != null)
                {
                    where += " AND ExecDate<='" + eeDate + "'";
                }
                else if (seDate != null && eeDate == null)
                {
                    where += " AND ExecDate>='" + seDate + "'";
                }
                else
                {
                    where += " AND ExecDate>='" + seDate + "' AND ExecDate<='" + eeDate + "'";
                }
            }
            if (dType != null)
            {
                where += " AND DispatchType='" + dType + "'";
            }
            if (!string.IsNullOrWhiteSpace(status))
            {
                where += " AND Status LIKE'%" + status + "%'";
            }
            if (isPlan != null)
            {
                where += " AND IsPlanBase=" + isPlan;
            }
            string sql = "SELECT IssueDate 下发日期时间,ExecDate 执行日期时间,(SELECT FULLNAME FROM SYS_ROLE WHERE ENCODE=CONVERT(VARCHAR(5),Shift)) 班次,Operator 操作人员," +
                "(SELECT ItemName FROM SYS_DIC WHERE ItemID=DispatchType AND TypeID=6) 调度类型,DispatchOrder 调度指令,status 调令状态 FROM CC_DispatchPlan " + where;
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
                FilePath = "/file/调度指令" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx",
                SheetNames = new string[] { "调度指令" },
                Titles = new string[] { "调度指令" }
            };
            excel.AddTables(tmp1);
            excel.CreateExcel();
            return excel.FilePath;

        }
    }
}
