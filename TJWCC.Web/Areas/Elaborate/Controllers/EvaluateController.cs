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
using System.Text;
using System.Data.Common;
using System.Data;

namespace TJWCC.Web.Areas.Elaborate.Controllers
{
    public class EvaluateController : ControllerBase
    {
        log4net.ILog loger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        TJWCCDbContext dbcontext = new TJWCCDbContext();

        public EvaluateController()
        {
            dbcontext.Database.CommandTimeout = 300;
        }

        // 运行评估: Elaborate/Evaluate

        //获取压力平均值指标
        public ActionResult GetAvgPressure()
        {
            //获取方案id
            int scenId = dbcontext.Database.SqlQuery<int>("SELECT SC_ID FROM SPF_WGSCEN WHERE SCEN_NAME='最高时'").FirstOrDefault();

            //获取节点压力及服务水平列表
            StringBuilder strBSql = new StringBuilder();
            //strBSql.Append(" select top 20 a.Node_Label Label, round(a.Pressure_VALUE,2) as  Pressure from SPF_JCWGNodePressure a ");
            //strBSql.Append(" where SCEN_ID=" + scenId);
            //strBSql.Append(" order by a.Node_Label");
            strBSql.Append(" SELECT TOP 100 Node_Label Label, Pressure_VALUE Pressure, ");
            //strBSql.Append("   CONVERT(numeric(12,2),round((case when y2=y1 then y2 when x2=x1 then 0 else (Pressure_VALUE-x1)*(y2-y1)/(x2-x1)+y1  end),2)) Service ");
            strBSql.Append("   round((case when y2=y1 then y2 when x2=x1 then 0 else (Pressure_VALUE-x1)*(y2-y1)/(x2-x1)+y1  end),2) Service ");
            strBSql.Append(" FROM SPF_JCWGNodePressure join SPF_dt_Fuction ");
            strBSql.Append(" 		ON ((Pressure_VALUE>x1 AND Pressure_VALUE<x2) OR (Pressure_VALUE=x1)) ");
            strBSql.Append(" 		AND SCEN_ID=" + scenId);
            strBSql.Append(" 		AND C_Name='" + JueCheZhiCiCanShu.FTechPressureAvg + "'");
            strBSql.Append(" ORDER BY service ");
            var nodePressureList = dbcontext.Database.SqlQuery<NodePressure>(strBSql.ToString()).ToList();

            //获取压力最大、最小和平均值
            string strSql = "select round(Max(Pressure_Value),2) MaxP, round(Min(Pressure_Value),2) MinP, round(Avg(Pressure_Value),2) AvgP from SPF_JCWGNodePressure where SCEN_ID=" + scenId;
            Maxminavg nodePressureMaxMin = dbcontext.Database.SqlQuery<Maxminavg>(strSql).FirstOrDefault();

            double minp = nodePressureMaxMin.MinP;
            double maxp = nodePressureMaxMin.MaxP;
            double interval = System.Math.Round((maxp - minp) / 4, 2);

            //获取压力分布的百分比
            List<HeadPercentage> headPList = new List<HeadPercentage>();
            HeadPercentage headper = new HeadPercentage();
            headper.head = minp;
            headper.percentage = 0;
            headPList.Add(headper);
            strSql = "select count(1) from SPF_JCWGNodePressure where SCEN_ID=" + scenId;
            int totalNodes = dbcontext.Database.SqlQuery<int>(strSql).FirstOrDefault();
            for (int i = 0; i < 4; i++)
            {
                HeadPercentage hp = new HeadPercentage();
                double head1 = minp + interval * i;
                double head2 = Math.Round(minp + interval * (i + 1), 2);
                if (i == 3) head2 = maxp;
                hp.head = head2;
                strSql = "select cast(100.00*count(1)/" + totalNodes + " as decimal(6,2)) from SPF_JCWGNodePressure where SCEN_ID=" + scenId + " and Pressure_Value>" + head1 + " and Pressure_Value<=" + head2;
                hp.percentage = dbcontext.Database.SqlQuery<decimal>(strSql).FirstOrDefault().ToDouble();
                headPList.Add(hp);
            }

            //最高工作压力、最低工作压力、标定曲线
            double Hmax = GetT_UserSet(JueCheZhiCiCanShu.strHmax);
            double Ha = GetT_UserSet(JueCheZhiCiCanShu.strHa);
            List<Dt_Function> dt_FunctionList = GetDt_Function(JueCheZhiCiCanShu.FTechPressureAvg);

            //获取服务水平
            strBSql = new StringBuilder();
            //strBSql.Append(" SELECT SUM(CONVERT(numeric(12,2), round((case when y2=y1 then y2 when x2=x1 then 0 else (Pressure_VALUE-x1)*(y2-y1)/(x2-x1)+y1  end),2)))/count(*) ");
            strBSql.Append(" SELECT SUM(round((case when y2=y1 then y2 when x2=x1 then 0 else (Pressure_VALUE-x1)*(y2-y1)/(x2-x1)+y1  end),2))/count(*) ");
            strBSql.Append(" FROM SPF_JCWGNodePressure, SPF_dt_Fuction ");
            strBSql.Append(" WHERE ((Pressure_VALUE>x1 AND Pressure_VALUE<x2) OR (Pressure_VALUE=x1)) ");
            strBSql.Append(" AND SCEN_ID=" + scenId);
            strBSql.Append(" AND C_Name='" + JueCheZhiCiCanShu.FTechPressureAvg + "' ");
            double avgService = Math.Round(dbcontext.Database.SqlQuery<double>(strBSql.ToString()).FirstOrDefault(), 2);

            //保存服务水平
            UserSet_Insert(JueCheZhiCiCanShu.TecpressureavgScore, avgService);

            //获取服务水平占比
            strBSql = new StringBuilder();
            strBSql.Append(" SELECT ROUND(SUM(CASE WHEN s.Service<=1 THEN 1.0 ELSE 0 END)/count(*), 2)*100 one, ROUND(SUM(CASE WHEN s.Service<=2 and s.Service>1 THEN 1.0 ELSE 0 END)/count(*), 2)*100 two,  ");
            strBSql.Append(" ROUND(SUM(CASE WHEN s.Service<=3 and s.Service>2 THEN 1.0 ELSE 0 END)/count(*), 2)*100 three, ROUND(SUM(CASE WHEN s.Service<=4 and s.Service>3 THEN 1.0 ELSE 0 END)/count(*), 2)*100 four  ");
            strBSql.Append(" FROM  ");
            strBSql.Append("  (SELECT round((case when y2=y1 then y2 when x2=x1 then 0 else (Pressure_VALUE-x1)*(y2-y1)/(x2-x1)+y1  end),2) Service ");
            strBSql.Append("   FROM SPF_JCWGNodePressure join SPF_dt_Fuction ");
            strBSql.Append(" 		ON ((Pressure_VALUE>x1 AND Pressure_VALUE<x2) OR (Pressure_VALUE=x1)) ");
            strBSql.Append(" 		AND SCEN_ID=" + scenId);
            strBSql.Append(" 		AND C_Name='" + JueCheZhiCiCanShu.FTechPressureAvg + "') s");
            var servicePercentage = dbcontext.Database.SqlQuery<ServicePercentage>(strBSql.ToString()).FirstOrDefault();

            JObject result = new JObject();

            //strResult = string.Join(",", "");

            result.Add("节点压力及服务水平列表", JArray.FromObject(nodePressureList));
            result.Add("压力最大、最小和平均值", nodePressureMaxMin.ToJson());
            result.Add("压力分布的百分比", JArray.FromObject(headPList));
            result.Add("最高工作压力", Hmax.ToString());
            result.Add("最低工作压力", Ha.ToString());
            result.Add("标定曲线", JArray.FromObject(dt_FunctionList));
            result.Add("最高工作压力、最低工作压力、标定曲线", JArray.FromObject(headPList));
            result.Add("TecpressureavgScore", avgService.ToString());
            result.Add("服务水平占比", servicePercentage.ToJson());

            return Content(result.ToJson());
        }

        //获取压力合格率指标
        public ActionResult GetRatePressure()
        {
            //获取方案id
            int scenId = dbcontext.Database.SqlQuery<int>("SELECT SC_ID FROM SPF_WGSCEN WHERE SCEN_NAME='最高时'").FirstOrDefault();

            //最高工作压力、最低工作压力
            double Hmax = GetT_UserSet(JueCheZhiCiCanShu.strHmax);
            double Ha = GetT_UserSet(JueCheZhiCiCanShu.strHa);

            //获取各节点的低压、适宜、高压需水量列表
            StringBuilder strBSql = new StringBuilder();
            strBSql.Append(" SELECT TOP 100 a.Node_Label Label, ");
            strBSql.Append(" case when a.Pressure_VALUE<" + Ha + " then Round(b.Demand_VALUE, 2) else 0 end  as LowDemand, ");
            strBSql.Append(" case when a.Pressure_VALUE>=" + Ha + " and a.Pressure_VALUE<=" + Hmax + "  then Round(b.Demand_VALUE, 2) else 0 end  as MidDemand, ");
            strBSql.Append(" case when a.Pressure_VALUE>" + Hmax + " then Round(b.Demand_VALUE, 2) else 0 end as HighDemand ");
            strBSql.Append(" FROM SPF_JCWGNodePressure a LEFT JOIN SPF_WGNodeDemand b ON a.SCEN_ID=b.SCEN_ID AND a.Node_Label=b.Node_ID ");
            strBSql.Append(" WHERE a.SCEN_ID=" + scenId);
            strBSql.Append(" ORDER BY a.Node_Label ");
            var nodeDemandList = dbcontext.Database.SqlQuery<NodeDemand>(strBSql.ToString()).ToList();

            //获取节点的低压、适宜、高压需水量总量
            strBSql = new StringBuilder();
            strBSql.Append(" SELECT Round(SUM(b.Demand_VALUE), 2) TotalDemand, ");
            strBSql.Append(" Round(SUM(case when a.Pressure_VALUE<" + Ha + " then b.Demand_VALUE else 0 end), 2) LowDemand, ");
            strBSql.Append(" Round(SUM(case when a.Pressure_VALUE>=" + Ha + " and a.Pressure_VALUE<=" + Hmax + "  then b.Demand_VALUE else 0 end), 2) MidDemand, ");
            strBSql.Append(" Round(SUM(case when a.Pressure_VALUE>" + Hmax + " then b.Demand_VALUE else 0 end), 2) HighDemand ");
            strBSql.Append(" FROM SPF_JCWGNodePressure a LEFT JOIN SPF_WGNodeDemand b ON a.SCEN_ID=b.SCEN_ID AND a.Node_Label=b.Node_ID ");
            strBSql.Append(" where a.SCEN_ID=" + scenId);
            var nodeTotalDemand = dbcontext.Database.SqlQuery<NodeTotalDemand>(strBSql.ToString()).FirstOrDefault();

            //节点的低压、适宜、高压的标定曲线
            List<Dt_Function> low_dt_FunctionList = GetDt_Function(JueCheZhiCiCanShu.FTechPressureLow);
            List<Dt_Function> mid_dt_FunctionList = GetDt_Function(JueCheZhiCiCanShu.FTechPressureMid);
            List<Dt_Function> high_dt_FunctionList = GetDt_Function(JueCheZhiCiCanShu.FTechPressureHigh);

            //根据标定曲线计算低压、适宜、高压的服务水平
            double lowService = Get_FunctionScore(JueCheZhiCiCanShu.FTechPressureLow, 100 * nodeTotalDemand.LowDemand / nodeTotalDemand.TotalDemand);
            double midService = Get_FunctionScore(JueCheZhiCiCanShu.FTechPressureMid, 100 * nodeTotalDemand.MidDemand / nodeTotalDemand.TotalDemand);
            double highService = Get_FunctionScore(JueCheZhiCiCanShu.FTechPressureHigh, 100 * nodeTotalDemand.HighDemand / nodeTotalDemand.TotalDemand);

            //获取低压、适宜、高压的权重
            double lowQuan = UserSet_Select(JueCheZhiCiCanShu.FTechPressureLowQuan);
            double midQuan = UserSet_Select(JueCheZhiCiCanShu.FTechPressureMidQuan);
            double highQuan = UserSet_Select(JueCheZhiCiCanShu.FTechPressureHighQuan);
            //获取压力合格率服务水平
            double rateService = UserSet_Select(JueCheZhiCiCanShu.TecpressureQualifiedRateScore);


            JObject result = new JObject();

            result.Add("节点的低压、适宜、高压需水量列表", JArray.FromObject(nodeDemandList));
            result.Add("节点的低压、适宜、高压需水量总量", nodeTotalDemand.ToJson());
            result.Add("低压的标定曲线", JArray.FromObject(low_dt_FunctionList));
            result.Add("适宜的标定曲线", JArray.FromObject(mid_dt_FunctionList));
            result.Add("高压的标定曲线", JArray.FromObject(high_dt_FunctionList));
            result.Add("低压服务水平", lowService.ToString());
            result.Add("适宜服务水平", midService.ToString());
            result.Add("高压服务水平", highService.ToString());
            result.Add("最高工作压力", Hmax.ToString());
            result.Add("最低工作压力", Ha.ToString());
            result.Add("FTechPressureLowQuan", lowQuan.ToString());
            result.Add("FTechPressureMidQuan", midQuan.ToString());
            result.Add("FTechPressureHighQuan", highQuan.ToString());
            result.Add("TecpressureQualifiedRateScore", rateService.ToString());

            return Content(result.ToJson());
        }

        //获取压力均衡性指标
        public ActionResult GetDiffPressure()
        {
            double Ha;              //最低工作压力Ha
            double S_Value;         //节点压力离优差
            double diffService;     //离优差服务水平
            double Smax_Value;      //允许最大离优差

            //获取方案id
            int scenId = dbcontext.Database.SqlQuery<int>("SELECT SC_ID FROM SPF_WGSCEN WHERE SCEN_NAME='最高时'").FirstOrDefault();

            //最低工作压力Ha、允许最大离优差
            Ha = GetT_UserSet(JueCheZhiCiCanShu.FTechPressureSYou);     //离优差H优值
            Smax_Value = GetT_UserSet(JueCheZhiCiCanShu.FTechPressureSValue);

            //节点压力离优差、计算压力离优差
            string strSQL = "SELECT SQRT(sum(Power(Pressure_VALUE-" + Ha + ",2))/COUNT(*)) FROM SPF_JCWGNodePressure where SCEN_ID=" + scenId;
            S_Value = Math.Round(dbcontext.Database.SqlQuery<double>(strSQL).FirstOrDefault(), 2);
            //S_Value = Math.Round(dbcontext.Database.SqlQuery<decimal>(strSQL).FirstOrDefault().ToDouble(), 2);

            //离优差服务水平
            diffService = Get_FunctionScore(JueCheZhiCiCanShu.FTechPressureFromQX, S_Value);

            //压力离优差的标定曲线
            List<Dt_Function> diff_dt_FunctionList = GetDt_Function(JueCheZhiCiCanShu.FTechPressureFromQX);

            //保存服务水平
            UserSet_Insert(JueCheZhiCiCanShu.TecpressureFromdiffScore, diffService);

            JObject result = new JObject();

            result.Add("TecpressureFromdiffScore", diffService.ToString());
            result.Add("压力均衡性标准平方差", S_Value.ToString());
            result.Add("最低工作压力Ha", Ha.ToString());
            result.Add("允许的最大均衡性", Smax_Value.ToString());
            result.Add("均衡性的标定曲线", JArray.FromObject(diff_dt_FunctionList));

            return Content(result.ToJson());
        }

        //获取管段经济流速指标数据
        public ActionResult GetPipeVelocity()
        {
            //获取方案id
            int scenId = dbcontext.Database.SqlQuery<int>("SELECT SC_ID FROM SPF_WGSCEN WHERE SCEN_NAME='最高时'").FirstOrDefault();

            //获取流速的最大、最小、平均值和标准差
            string strSql = "SELECT Round(Max(VELOCITY_VALUE), 2) MaxP, Round(Min(VELOCITY_VALUE), 2) MinP, Round(AVG(VELOCITY_VALUE), 2) AvgP FROM SPF_JCWGPIPE_VELOCITY WHERE SCEN_ID=" + scenId;
            Maxminavg pipeVelocityMaxMin = dbcontext.Database.SqlQuery<Maxminavg>(strSql).FirstOrDefault();

            //获取流速的标准差
            strSql = "SELECT Round(SQRT(SUM(POWER(VELOCITY_VALUE-" + pipeVelocityMaxMin.AvgP + ",2))/COUNT(*)), 2) FROM SPF_JCWGPIPE_VELOCITY WHERE SCEN_ID=" + scenId;
            double pipeVariance = Math.Round(dbcontext.Database.SqlQuery<double>(strSql).FirstOrDefault(), 2);

            double minp = pipeVelocityMaxMin.MinP;
            double maxp = pipeVelocityMaxMin.MaxP;
            double interval = Math.Round((maxp - minp) / 4, 2);

            //获取经济流速分布的百分比
            List<SpeedPercentage> speedPList = new List<SpeedPercentage>();
            SpeedPercentage velocity = new SpeedPercentage();
            velocity.speed = minp;
            velocity.percentage = 0;
            speedPList.Add(velocity);
            strSql = "select count(1) from SPF_JCWGPIPE_VELOCITY where SCEN_ID=" + scenId;
            int totalPipes = dbcontext.Database.SqlQuery<int>(strSql).FirstOrDefault();
            for (int i = 0; i < 4; i++)
            {
                SpeedPercentage sp = new SpeedPercentage();
                double head1 = minp + interval * i;
                double head2 = Math.Round(minp + interval * (i + 1), 2);
                if (i == 3) head2 = maxp;
                sp.speed = head2;
                strSql = "select cast(100.00*count(1)/" + totalPipes + " as decimal(6,2)) from SPF_JCWGPIPE_VELOCITY where SCEN_ID=" + scenId + " and VELOCITY_VALUE>=" + head1 + " and VELOCITY_VALUE<" + head2;
                sp.percentage = Math.Round(dbcontext.Database.SqlQuery<decimal>(strSql).FirstOrDefault().ToDouble(), 2);
                speedPList.Add(sp);
            }


            //获取各管线的信息
            StringBuilder strBSql = new StringBuilder();
            //strBSql.Append(" SELECT p.PIPE_Lable Lable,  CAST(p.PIPE_LENGTH as numeric(12,2)) Length,  CAST(p.PIPE_DIA as numeric(12,2)) as Diameter, ");
            strBSql.Append(" SELECT TOP 100 p.PIPE_Lable Label,  p.PIPE_LENGTH Length,  p.PIPE_DIA Diameter, ");
            strBSql.Append(" 	j.VELOCITY_VALUE Velocity, Round(0.1274*POWER(p.PIPE_DIA,0.3),2) EconomicSpeed, ");
            strBSql.Append(" 	Round(p.PIPE_LENGTH*PIPE_DIA*PIPE_DIA*PI()*(case when j.VELOCITY_VALUE < 0.1274*POWER(PIPE_DIA,0.3)*0.95 then 1 else 0 end)/4000000,2) LowVolume, ");
            strBSql.Append(" 	Round(p.PIPE_LENGTH*PIPE_DIA*PIPE_DIA*PI()*(case when j.VELOCITY_VALUE >= 0.1274*POWER(PIPE_DIA,0.3)*0.95  and j.VELOCITY_VALUE <= 0.1274*POWER(PIPE_DIA,0.3)*1.15 then 1  else 0 end)/4000000,2) MidVolume, ");
            strBSql.Append(" 	Round(p.PIPE_LENGTH*PIPE_DIA*PIPE_DIA*PI()*(case when j.VELOCITY_VALUE > 0.1274*POWER(PIPE_DIA,0.3)*1.15 then 1 else 0 end)/4000000,2) HighVolume ");
            strBSql.Append(" FROM SPF_JCWGPIPE_VELOCITY j, SPF_WGPIPE p ");
            strBSql.Append(" WHERE j.MODEL_ID = p.MODEL_ID and j.PIPE_ID=p.ID and j.SCEN_ID=" + scenId);
            strBSql.Append(" --ORDER BY p.PIPE_Lable desc ");
            var pipeInfoList = dbcontext.Database.SqlQuery<PipeInfo>(strBSql.ToString()).ToList();

            //获取各管线的低负荷、经济、高负荷管线容积的和
            strBSql = new StringBuilder();
            strBSql.Append(" SELECT Round(SUM(p.PIPE_LENGTH*PIPE_DIA*PIPE_DIA*PI()/4000000), 2) TotalVolume, ");
            strBSql.Append("   Round(SUM(p.PIPE_LENGTH*PIPE_DIA*PIPE_DIA*PI()*(case when j.VELOCITY_VALUE < 0.1274*POWER(PIPE_DIA,0.3)*0.95 then 1 else 0 end)/4000000), 2) LowVolume, ");
            strBSql.Append("   Round(SUM(p.PIPE_LENGTH*PIPE_DIA*PIPE_DIA*PI()*(case when j.VELOCITY_VALUE >= 0.1274*POWER(PIPE_DIA,0.3)*0.95  and j.VELOCITY_VALUE <= 0.1274*POWER(PIPE_DIA,0.3)*1.15 then 1  else 0 end)/4000000), 2) MidVolume, ");
            strBSql.Append("   Round(SUM(p.PIPE_LENGTH*PIPE_DIA*PIPE_DIA*PI()*(case when j.VELOCITY_VALUE > 0.1274*POWER(PIPE_DIA,0.3)*1.15 then 1 else 0 end)/4000000), 2) HighVolume ");
            strBSql.Append(" FROM SPF_JCWGPIPE_VELOCITY j, SPF_WGPIPE p ");
            strBSql.Append(" WHERE j.MODEL_ID = p.MODEL_ID and j.PIPE_ID=p.ID and j.SCEN_ID=" + scenId);
            var pipeToalVolume = dbcontext.Database.SqlQuery<PipeToalVolume>(strBSql.ToString()).FirstOrDefault();


            //管线的低负荷、经济、高负荷标定曲线
            List<Dt_Function> low_dt_FunctionList = GetDt_Function(JueCheZhiCiCanShu.FTechNicalPipeVelocityLow);
            List<Dt_Function> mid_dt_FunctionList = GetDt_Function(JueCheZhiCiCanShu.FTechNicalPipeVelocityMid);
            List<Dt_Function> high_dt_FunctionList = GetDt_Function(JueCheZhiCiCanShu.FTechNicalPipeVelocityHigh);

            //根据标定曲线计算低负荷、经济、高负荷的服务水平
            double lowService = Get_FunctionScore(JueCheZhiCiCanShu.FTechNicalPipeVelocityLow, 100 * pipeToalVolume.LowVolume / pipeToalVolume.TotalVolume);
            double midService = Get_FunctionScore(JueCheZhiCiCanShu.FTechNicalPipeVelocityMid, 100 * pipeToalVolume.MidVolume / pipeToalVolume.TotalVolume);
            double highService = Get_FunctionScore(JueCheZhiCiCanShu.FTechNicalPipeVelocityHigh, 100 * pipeToalVolume.HighVolume / pipeToalVolume.TotalVolume);

            //获取低负荷、经济、高负荷的权重
            double lowQuan = UserSet_Select(JueCheZhiCiCanShu.FTechNicalPipeVelocityLowQuan);
            double midQuan = UserSet_Select(JueCheZhiCiCanShu.FTechNicalPipeVelocityMidQuan);
            double highQuan = UserSet_Select(JueCheZhiCiCanShu.FTechNicalPipeVelocityHighQuan);

            //获取压力合格率服务水平
            double rateService = Math.Round((lowService * lowQuan + midService * midQuan + highService * highQuan), 2);
            UserSet_Insert(JueCheZhiCiCanShu.TechNicalPipeVelocityScore, rateService);

            JObject result = new JObject();

            result.Add("流速的最大、最小、平均值", pipeVelocityMaxMin.ToJson());
            result.Add("流速的标准差", pipeVariance.ToString());
            result.Add("经济流速分布的百分比", speedPList.ToJson());
            result.Add("各管线的信息列表", JArray.FromObject(pipeInfoList));
            result.Add("各管线的低负荷、经济、高负荷管线容积的和", pipeToalVolume.ToJson());
            result.Add("低负荷标定曲线", JArray.FromObject(low_dt_FunctionList));
            result.Add("经济标定曲线", JArray.FromObject(mid_dt_FunctionList));
            result.Add("高负荷标定曲线", JArray.FromObject(high_dt_FunctionList));
            result.Add("低负荷服务水平", lowService.ToString());
            result.Add("经济服务水平", midService.ToString());
            result.Add("高负荷服务水平", highService.ToString());
            result.Add("FTechNicalPipeVelocityLowQuan", lowQuan.ToString());
            result.Add("FTechNicalPipeVelocityMidQuan", midQuan.ToString());
            result.Add("FTechNicalPipeVelocityHighQuan", highQuan.ToString());
            result.Add("TechNicalPipeVelocityScore", rateService.ToString());

            return Content(result.ToJson());
        }

        //水力性能综合评价
        public ActionResult GetTechJSum()
        {
            //获取水力性能的压力平均值、压力合格率、压力均衡性和管段经济流速的权重
            double avgQuan = UserSet_Select(JueCheZhiCiCanShu.TecpressureavgQuanZhong);
            double rateQuan = UserSet_Select(JueCheZhiCiCanShu.TecpressureQualifiedRateQuanZhong);
            double diffQuan = UserSet_Select(JueCheZhiCiCanShu.TecpressureFromdiffQuanZhong);
            double speedQuan = UserSet_Select(JueCheZhiCiCanShu.TechNicalPipeVelocityQZ);

            //获取水力性能的服务水平
            double avgService = UserSet_Select(JueCheZhiCiCanShu.TecpressureavgScore);
            double rateService = UserSet_Select(JueCheZhiCiCanShu.TecpressureQualifiedRateScore);
            double diffService = UserSet_Select(JueCheZhiCiCanShu.TecpressureFromdiffScore);
            double speedService = UserSet_Select(JueCheZhiCiCanShu.TechNicalPipeVelocityScore);

            //计算水力性能服务水平
            double TecpressureService = Math.Round((avgService * avgQuan + rateService * rateQuan + diffService * diffQuan + speedService * speedQuan), 2);

            //保存服务水平
            UserSet_Insert(JueCheZhiCiCanShu.TecpressureScore, TecpressureService);

            JObject result = new JObject();

            //strResult = string.Join(",", "");

            result.Add("压力平均值服务水平", avgService.ToString());
            result.Add("压力合格率服务水平", rateService.ToString());
            result.Add("压力均衡性服务水平", diffService.ToString());
            result.Add("管段经济流速服务水平", speedService.ToString());
            result.Add("TecpressureavgQuanZhong", avgQuan.ToString());
            result.Add("TecpressureQualifiedRateQuanZhong", rateQuan.ToString());
            result.Add("TecpressureFromdiffQuanZhong", diffQuan.ToString());
            result.Add("TechNicalPipeVelocityQZ", speedQuan.ToString());
            result.Add("TecpressureScore", TecpressureService.ToString());

            return Content(result.ToJson());
        }

        //保存权重和服务水平
        public ActionResult SaveServiceAndQZ(string names, string values)
        {
            string[] nameList = names.Split(',');
            string[] valueList = values.Split(',');
            int row = 0;
            for(int i=0; i<nameList.Count(); i++)
            {
                row = row + UserSet_Insert(nameList[i], Math.Round(valueList[i].ToDouble(),2));
            }

            //JObject result = new JObject();
            if (row == nameList.Count())
            {
                return Success("保存成功！");
            }
            return Error("保存失败！");
        }

        //获取水质合格率指标数据
        public ActionResult GetInWaterQualityRate()
        {
            //标定曲线
            List<Dt_Function> dt_FunctionList = GetDt_Function(JueCheZhiCiCanShu.TechNicalWaterQualityRateBiaoDing);

            //获取保存的输入数据和服务水平
            double inputData = UserSet_Select(JueCheZhiCiCanShu.TechNicalWaterQualityRate);
            double service = UserSet_Select(JueCheZhiCiCanShu.TechNicalWaterQualityRateScore);

            JObject result = new JObject();

            result.Add("标定曲线", JArray.FromObject(dt_FunctionList));
            result.Add("TechNicalWaterQualityRate", inputData.ToString());
            result.Add("TechNicalWaterQualityRateScore", service.ToString());

            return Content(result.ToJson());
        }

        //获取节点水龄合格率指标
        public ActionResult GetAgeRate()
        {
            //获取方案id
            int scenId = dbcontext.Database.SqlQuery<int>("SELECT SC_ID FROM SPF_WGSCEN WHERE SCEN_NAME='水龄分析'").FirstOrDefault();

            //可允许的节点水龄、允许的最大节点水龄
            double Tmax = GetT_UserSet(JueCheZhiCiCanShu.TechNicalPipeBestWaterTimemax);
            double Ta = GetT_UserSet(JueCheZhiCiCanShu.TechNicalPipeBestWaterTimea);

            //获取各节点编号、水龄、服务水平信息列表
            StringBuilder strBSql = new StringBuilder();
            strBSql.Append(" SELECT TOP 100 Pipe_Label Label, ROUND(age_value,2) Age, ");
            strBSql.Append("	Round((case when y2=y1 then y2 when x2=x1 then 0 else (a.AGE_VALUE-x1)*(y2-y1)/(x2-x1)+y1  end),2) Service ");
            strBSql.Append(" FROM SPF_JCWGPIPE_AGE a, SPF_dt_Fuction b ");
            strBSql.Append(" WHERE ((a.AGE_VALUE>x1 and a.AGE_VALUE<x2) or (a.AGE_VALUE=x1)) ");
            strBSql.Append("	AND SCEN_ID=" + scenId);
            strBSql.Append("	AND b.C_Name='" + JueCheZhiCiCanShu.TechNicalPipeBestWaterTime + "'");
            strBSql.Append(" ORDER BY Pipe_Label ");
            List<NodeAge> nodeAgeList = dbcontext.Database.SqlQuery<NodeAge>(strBSql.ToString()).ToList();

            //获取节点水龄的最大、最小、平均及标准差
            strBSql = new StringBuilder();
            strBSql.Append(" SELECT MAX(AGE_VALUE) Max, Min(AGE_VALUE) Min, Round(avg(AGE_VALUE),2) Avg, Round(stdev(AGE_VALUE),2) Stdev ");
            strBSql.Append(" FROM SPF_JCWGPIPE_AGE where SCEN_ID=" + scenId);
            MaxminavgStdev maxminavgStdev = dbcontext.Database.SqlQuery<MaxminavgStdev>(strBSql.ToString()).FirstOrDefault();

            //节点水龄的标定曲线
            List<Dt_Function> dt_FunctionList = GetDt_Function(JueCheZhiCiCanShu.TechNicalPipeBestWaterTime);

            //根据标定曲线及需水量权重计算水龄的服务水平
            strBSql = new StringBuilder();
            strBSql.Append(" SELECT round(SUM(c.Demand_VALUE*(case when y2=y1 then y2 when x2=x1 then 0 else (a.AGE_VALUE-x1)*(y2-y1)/(x2-x1)+y1  end))/SUM(c.Demand_VALUE),2) Score ");
            strBSql.Append(" FROM SPF_JCWGPIPE_AGE a, SPF_dt_Fuction b, SPF_WGNodeDemand c ");
            strBSql.Append(" WHERE ((a.AGE_VALUE>x1 and a.AGE_VALUE<x2) or (a.AGE_VALUE=x1)) ");
            strBSql.Append(" 	AND a.SCEN_ID=" + scenId);
            strBSql.Append(" 	AND b.C_Name='" + JueCheZhiCiCanShu.TechNicalPipeBestWaterTime + "'");
            strBSql.Append(" 	AND a.Pipe_ID=c.Node_ID ");
            double ageService = dbcontext.Database.SqlQuery<double>(strBSql.ToString()).FirstOrDefault();
            UserSet_Insert(JueCheZhiCiCanShu.TechNicalPipeWaterTimeScore, ageService);

            //获取水龄分布的百分比
            double minp = maxminavgStdev.Min;
            double maxp = maxminavgStdev.Max;
            double interval = System.Math.Round((maxp - minp) / 4, 2);

            List<HeadPercentage> headPList = new List<HeadPercentage>();
            HeadPercentage headper = new HeadPercentage();
            headper.head = minp;
            headper.percentage = 0;
            headPList.Add(headper);
            string strSql = "select count(1) from SPF_JCWGPIPE_AGE where SCEN_ID=" + scenId;
            int totalNodes = dbcontext.Database.SqlQuery<int>(strSql).FirstOrDefault();
            for (int i = 0; i < 4; i++)
            {
                HeadPercentage hp = new HeadPercentage();
                double head1 = minp + interval * i;
                double head2 = Math.Round(minp + interval * (i + 1), 2);
                if (i == 3) head2 = maxp;
                hp.head = head2;
                strSql = "SELECT cast(100.00*count(1)/" + totalNodes + " as decimal(6,2)) FROM SPF_JCWGPIPE_AGE WHERE SCEN_ID=" + scenId + " and AGE_VALUE>" + head1 + " and AGE_VALUE<=" + head2;
                hp.percentage = dbcontext.Database.SqlQuery<decimal>(strSql).FirstOrDefault().ToDouble();
                headPList.Add(hp);
            }

            //获取节点水龄服务水平的最大、最小值
            strBSql = new StringBuilder();
            strBSql.Append(" SELECT MAX(s.Score) Max, Min(s.Score) Min ");
            strBSql.Append(" FROM ");
            strBSql.Append(" (SELECT Round((case when y2=y1 then y2 when x2=x1 then 0 else (a.AGE_VALUE-x1)*(y2-y1)/(x2-x1)+y1  end),2) Score ");
            strBSql.Append(" FROM SPF_JCWGPIPE_AGE a, SPF_dt_Fuction b ");
            strBSql.Append(" WHERE ((a.AGE_VALUE>x1 and a.AGE_VALUE<x2) or (a.AGE_VALUE=x1)) ");
            strBSql.Append("	AND SCEN_ID=" + scenId);
            strBSql.Append("	AND b.C_Name='" + JueCheZhiCiCanShu.TechNicalPipeBestWaterTime + "') s");
            MaxminavgStdev maxmin = dbcontext.Database.SqlQuery<MaxminavgStdev>(strBSql.ToString()).FirstOrDefault();

            //获取水龄服务水平分布的百分比
            //minp = maxmin.Min;
            //maxp = maxmin.Max;
            minp = 1;
            maxp = 4;
            interval = System.Math.Round((maxp - minp) / 3, 2);

            List<HeadPercentage> headPList1 = new List<HeadPercentage>();
            headper = new HeadPercentage();
            headper.head = minp;
            strBSql = new StringBuilder();
            strBSql.Append(" SELECT cast(100.00*count(1)/" + totalNodes + " as decimal(6,2)) ");
            strBSql.Append(" FROM ");
            strBSql.Append(" (SELECT Round((case when y2=y1 then y2 when x2=x1 then 0 else (a.AGE_VALUE-x1)*(y2-y1)/(x2-x1)+y1  end),2) Score ");
            strBSql.Append(" FROM SPF_JCWGPIPE_AGE a, SPF_dt_Fuction b ");
            strBSql.Append(" WHERE ((a.AGE_VALUE>x1 and a.AGE_VALUE<x2) or (a.AGE_VALUE=x1)) ");
            strBSql.Append("	AND SCEN_ID=" + scenId);
            strBSql.Append("	AND b.C_Name='" + JueCheZhiCiCanShu.TechNicalPipeBestWaterTime + "') s");
            strBSql.Append(" WHERE s.Score=" + minp);
            headper.percentage = Math.Round(dbcontext.Database.SqlQuery<decimal>(strBSql.ToString()).FirstOrDefault().ToDouble(), 2);

            headPList1.Add(headper);
            strSql = "select count(1) from SPF_JCWGPIPE_AGE where SCEN_ID=" + scenId;
            totalNodes = dbcontext.Database.SqlQuery<int>(strSql).FirstOrDefault();

            for (int i = 0; i < 3; i++)
            {
                HeadPercentage hp = new HeadPercentage();
                double head1 = minp + interval * i;
                double head2 = Math.Round(minp + interval * (i + 1), 2);
                if (i == 3) head2 = maxp;
                hp.head = head2;
                strBSql = new StringBuilder();
                strBSql.Append(" SELECT cast(100.00*count(1)/" + totalNodes + " as decimal(6,2)) ");
                strBSql.Append(" FROM ");
                strBSql.Append(" (SELECT Round((case when y2=y1 then y2 when x2=x1 then 0 else (a.AGE_VALUE-x1)*(y2-y1)/(x2-x1)+y1  end),2) Score ");
                strBSql.Append(" FROM SPF_JCWGPIPE_AGE a, SPF_dt_Fuction b ");
                strBSql.Append(" WHERE ((a.AGE_VALUE>x1 and a.AGE_VALUE<x2) or (a.AGE_VALUE=x1)) ");
                strBSql.Append("	AND SCEN_ID=" + scenId);
                strBSql.Append("	AND b.C_Name='" + JueCheZhiCiCanShu.TechNicalPipeBestWaterTime + "') s");
                strBSql.Append(" WHERE s.Score> " + head1 + " and s.Score<= " + head2);
                hp.percentage = dbcontext.Database.SqlQuery<decimal>(strBSql.ToString()).FirstOrDefault().ToDouble();
                headPList1.Add(hp);
            }

            JObject result = new JObject();

            result.Add("可允许的节点水龄", Ta.ToString());
            result.Add("允许的最大节点水龄", Tmax.ToString());
            result.Add("各节点编号、水龄、服务水平信息列表", JArray.FromObject(nodeAgeList));
            result.Add("节点水龄的最大、最小、平均及标准差", maxminavgStdev.ToJson());
            result.Add("节点水龄的标定曲线", JArray.FromObject(dt_FunctionList));
            result.Add("水龄分布的百分比", JArray.FromObject(headPList));
            result.Add("水龄服务水平分布的百分比", JArray.FromObject(headPList1));
            result.Add("TechNicalPipeWaterTimeScore", ageService.ToString());

            return Content(result.ToJson());
        }

        //水质性能综合评价
        public ActionResult GetTechQualitySum()
        {
            //获取水质性能的水质合格率、水龄合格率的权重
            double qualityQuan = UserSet_Select(JueCheZhiCiCanShu.TechNicalWaterQualityRateQZ);
            double ageQuan = UserSet_Select(JueCheZhiCiCanShu.TechNicalPipeWaterTimeQZ);

            //获取水质性能的服务水平
            double qualityService = UserSet_Select(JueCheZhiCiCanShu.TechNicalWaterQualityRateScore);
            double ageService = UserSet_Select(JueCheZhiCiCanShu.TechNicalPipeWaterTimeScore);

            //计算水质性能服务水平
            double qualitySumScore = Math.Round((qualityService * qualityQuan + ageService * ageQuan), 2);

            //保存服务水平
            UserSet_Insert(JueCheZhiCiCanShu.TechNicalWaterQualitySumScore, qualitySumScore);

            JObject result = new JObject();

            //strResult = string.Join(",", "");

            result.Add("水质合格率服务水平", qualityService.ToString());
            result.Add("水龄合格率服务水平", ageService.ToString());
            result.Add("TechNicalWaterQualityRateQZ", qualityQuan.ToString());
            result.Add("TechNicalPipeWaterTimeQZ", ageQuan.ToString());
            result.Add("TechNicalWaterQualitySumScore", qualitySumScore.ToString());

            return Content(result.ToJson());
        }

        //获取管网漏损率指标数据
        public ActionResult GetInLeakageRate()
        {
            //标定曲线
            List<Dt_Function> dt_FunctionList = GetDt_Function(JueCheZhiCiCanShu.TechNicalSupplyEfficiencyLeakageBiaoDing);

            //获取保存的输入数据和服务水平
            double inputData = UserSet_Select(JueCheZhiCiCanShu.TechNicalSupplyEfficiencyLeakage);
            double service = UserSet_Select(JueCheZhiCiCanShu.TechNicalSupplyEfficiencyLeakageScore);

            JObject result = new JObject();

            result.Add("管网漏损率标定曲线", JArray.FromObject(dt_FunctionList));
            result.Add("TechNicalSupplyEfficiencyLeakage", inputData.ToString());
            result.Add("TechNicalSupplyEfficiencyLeakageScore", service.ToString());

            return Content(result.ToJson());
        }
        
        //获取用水普及率指标数据
        public ActionResult GetInCoverageRate()
        {
            //标定曲线
            List<Dt_Function> dt_FunctionList = GetDt_Function(JueCheZhiCiCanShu.TechNicalSupplyEfficiencyCoverageBiaoDing);

            //获取保存的输入数据和服务水平
            double inputData = UserSet_Select(JueCheZhiCiCanShu.TechNicalSupplyEfficiencyCoverage);
            double service = UserSet_Select(JueCheZhiCiCanShu.TechNicalSupplyEfficiencyCoverageScore);

            JObject result = new JObject();

            result.Add("用水普及率标定曲线", JArray.FromObject(dt_FunctionList));
            result.Add("TechNicalSupplyEfficiencyCoverage", inputData.ToString());
            result.Add("TechNicalSupplyEfficiencyCoverageScore", service.ToString());

            return Content(result.ToJson());
        }

        //供水效率综合评价
        public ActionResult GetTechSupplySum()
        {
            //获取管网漏损率、用水普及率的权重
            double leakageQuan = UserSet_Select(JueCheZhiCiCanShu.TechNicalSupplyEfficiencyLeakageQZ);
            double coverageQuan = UserSet_Select(JueCheZhiCiCanShu.TechNicalSupplyEfficiencyCoverageQZ);

            //获取服务水平
            double leakageService = UserSet_Select(JueCheZhiCiCanShu.TechNicalSupplyEfficiencyLeakageScore);
            double coverageService = UserSet_Select(JueCheZhiCiCanShu.TechNicalSupplyEfficiencyCoverageScore);

            //计算供水效率服务水平
            double sumScore = Math.Round((leakageService * leakageQuan + coverageService * coverageQuan), 2);

            //保存服务水平
            UserSet_Insert(JueCheZhiCiCanShu.TechNicalSupplyEfficiencySumScore, sumScore);

            JObject result = new JObject();

            result.Add("管网漏损率服务水平", leakageService.ToString());
            result.Add("用水普及率服务水平", coverageService.ToString());
            result.Add("TechNicalSupplyEfficiencyLeakageQZ", leakageQuan.ToString());
            result.Add("TechNicalSupplyEfficiencyCoverageQZ", coverageQuan.ToString());
            result.Add("TechNicalSupplyEfficiencySumScore", sumScore.ToString());

            return Content(result.ToJson());
        }

        //技术性综合评价
        public ActionResult GetTechSum()
        {
            //获取水力性能、水质性能、供水效率的权重
            double Quan1 = UserSet_Select(JueCheZhiCiCanShu.TecpressureQuanZhong);
            double Quan2 = UserSet_Select(JueCheZhiCiCanShu.TechNicalWaterQualitySumQZ);
            double Quan3 = UserSet_Select(JueCheZhiCiCanShu.TechNicalSupplyEfficiencySumQZ);

            //获取服务水平
            double service1 = UserSet_Select(JueCheZhiCiCanShu.TecpressureScore);
            double service2 = UserSet_Select(JueCheZhiCiCanShu.TechNicalWaterQualitySumScore);
            double service3 = UserSet_Select(JueCheZhiCiCanShu.TechNicalSupplyEfficiencySumScore);

            //计算技术性服务水平
            double sumScore = Math.Round((service1 * Quan1 + service2 * Quan2 + service3 * Quan3), 2);

            //保存服务水平
            UserSet_Insert(JueCheZhiCiCanShu.TechScore, sumScore);

            JObject result = new JObject();

            //strResult = string.Join(",", "");

            result.Add("水力性能服务水平", service1.ToString());
            result.Add("水质性能服务水平", service2.ToString());
            result.Add("供水效率服务水平", service3.ToString());
            result.Add("TecpressureQuanZhong", Quan1.ToString());
            result.Add("TechNicalWaterQualitySumQZ", Quan2.ToString());
            result.Add("TechNicalSupplyEfficiencySumQZ", Quan3.ToString());
            result.Add("TechScore", sumScore.ToString());

            return Content(result.ToJson());
        }

        //获取基建费用数据
        public ActionResult GetFixedInvestment()
        {
            //获取各管段的Label、成本信息列表
            StringBuilder strBSql = new StringBuilder();
            strBSql.Append(" SELECT TOP 100 a.PIPE_Lable Label, cast(a.PIPE_LENGTH as decimal(12,2)) Length,  cast(a.PIPE_DIA as decimal(12,2)) Diameter, ");
            strBSql.Append("  cast((b.a+b.b*POWER(a.PIPE_DIA*0.001,b.k))*a.PIPE_LENGTH/10000 as decimal(16,4)) cost ");
            strBSql.Append(" FROM SPF_WGPIPE a, SPF_Pipe_SetM b ");
            strBSql.Append(" WHERE a.PIPE_MATERIALS=b.Pipe_M ");
            List<PipeCost> pipeCostList = dbcontext.Database.SqlQuery<PipeCost>(strBSql.ToString()).ToList();

            //获取各类管材的管材名称、成本信息列表
            strBSql = new StringBuilder();
            strBSql.Append(" SELECT a.PIPE_MATERIALS Pipe_Material, Round(SUM((b.a+b.b*POWER(a.PIPE_DIA*0.001,b.k))*a.PIPE_LENGTH/10000),2) cost ");
            strBSql.Append(" FROM SPF_WGPIPE a, SPF_Pipe_SetM b ");
            strBSql.Append(" WHERE a.PIPE_MATERIALS=b.Pipe_M ");
            strBSql.Append(" GROUP BY a.PIPE_MATERIALS ");
            List<PipeMCost> pipeMCostList = dbcontext.Database.SqlQuery<PipeMCost>(strBSql.ToString()).ToList();

            //标定曲线
            List<Dt_Function> dt_FunctionList = GetDt_Function(JueCheZhiCiCanShu.EconomicFixedInvestmentBiaoDing);

            //获取水厂投资(万元)、泵站投资(万元)、管网投资(万元)、其他投资(万元)、总投资(万元)
            double factory = UserSet_Select(JueCheZhiCiCanShu.EconomicFixedInvestmentFactory);
            double bengZhan = UserSet_Select(JueCheZhiCiCanShu.EconomicFixedInvestmentPump);
            double other = UserSet_Select(JueCheZhiCiCanShu.EconomicFixedInvestmentOther);
            strBSql = new StringBuilder();
            strBSql.Append(" SELECT Round(SUM((b.a+b.b*POWER(a.PIPE_DIA/1000,b.k))*a.PIPE_LENGTH)/10000,2) ");
            strBSql.Append(" FROM SPF_WGPIPE a, SPF_Pipe_SetM b ");
            strBSql.Append(" WHERE a.PIPE_MATERIALS=b.Pipe_M ");
            double pipe = dbcontext.Database.SqlQuery<double>(strBSql.ToString()).FirstOrDefault();
            double sum = Math.Round((factory + bengZhan + pipe + other),2);
            UserSet_Insert(JueCheZhiCiCanShu.EconomicFixedInvestmentSum, sum);

            //C优
            double Cyou = UserSet_Select(JueCheZhiCiCanShu.EconomicFixedInvestmentCYou);

            //C总/C优
            double costRate = Math.Round(sum/Cyou, 2);

            //根据标定曲线计算基建费用的服务水平
            double fixService = Get_FunctionScore(JueCheZhiCiCanShu.EconomicFixedInvestmentBiaoDing, costRate);
            UserSet_Insert(JueCheZhiCiCanShu.EconomicFixedInvestmentScore, fixService);


            JObject result = new JObject();

            result.Add("各管段的Label、成本信息列表", JArray.FromObject(pipeCostList));
            result.Add("各类管材的管材名称、成本信息列表", JArray.FromObject(pipeMCostList));
            result.Add("标定曲线", JArray.FromObject(dt_FunctionList));
            result.Add("EconomicFixedInvestmentFactory", factory.ToString());
            result.Add("EconomicFixedInvestmentPump", bengZhan.ToString());
            result.Add("管网投资", pipe.ToString());
            result.Add("EconomicFixedInvestmentOther", other.ToString());
            result.Add("总投资", sum.ToString());
            result.Add("C优", Cyou.ToString());
            result.Add("C总/C优", costRate.ToString());
            result.Add("EconomicFixedInvestmentScore", fixService.ToString());

            return Content(result.ToJson());
        }
        
        //获取运行费用数据
        public ActionResult GetRunCost()
        {
            //标定曲线
            List<Dt_Function> dt_FunctionList = GetDt_Function(JueCheZhiCiCanShu.EconomicEnergyConsumptionBiaoDing);

            //获取每天的供水量和电耗
            double water = UserSet_Select(JueCheZhiCiCanShu.EconomicEnergyConsumptionEwater);
            double power = UserSet_Select(JueCheZhiCiCanShu.EconomicEnergyConsumptionEpower);
            //供水千吨水电耗E
            double eTotal= Math.Round(power/water,2);
            UserSet_Insert(JueCheZhiCiCanShu.EconomicEnergyConsumptionE, eTotal);

            //C优
            double Eyou = UserSet_Select(JueCheZhiCiCanShu.EconomicEnergyConsumptionEYou);

            //C总/C优
            double costRate = Math.Round(eTotal / Eyou, 2);

            //根据标定曲线计算运行费用的服务水平
            double runCostScore = Get_FunctionScore(JueCheZhiCiCanShu.EconomicEnergyConsumptionBiaoDing, costRate);
            UserSet_Insert(JueCheZhiCiCanShu.EconomicEnergyConsumptionScore, runCostScore);

            JObject result = new JObject();

            result.Add("运行费用标定曲线", JArray.FromObject(dt_FunctionList));
            result.Add("EconomicEnergyConsumptionEwater", water.ToString());
            result.Add("EconomicEnergyConsumptionEpower", power.ToString());
            result.Add("供水电耗E(kWh/km3)", eTotal.ToString());
            result.Add("EconomicEnergyConsumptionEYou", Eyou.ToString());
            result.Add("C总/C优", costRate.ToString());
            result.Add("EconomicEnergyConsumptionScore", runCostScore.ToString());

            return Content(result.ToJson());
        }

        //经济性综合评价
        public ActionResult GetEconomicSum()
        {
            //获取基建费用、运行费用的权重
            double Quan1 = UserSet_Select(JueCheZhiCiCanShu.EconomicFixedInvestmentQuanZhong);
            double Quan2 = UserSet_Select(JueCheZhiCiCanShu.EconomicEnergyConsumptionQuanZhong);
            //double Quan3 = UserSet_Select(JueCheZhiCiCanShu.TechNicalSupplyEfficiencySumQZ);

            //获取服务水平
            double service1 = UserSet_Select(JueCheZhiCiCanShu.EconomicFixedInvestmentScore);
            double service2 = UserSet_Select(JueCheZhiCiCanShu.EconomicEnergyConsumptionScore);
            //double service3 = UserSet_Select(JueCheZhiCiCanShu.TechNicalSupplyEfficiencySumScore);

            //计算技术性服务水平
            double sumScore = Math.Round((service1 * Quan1 + service2 * Quan2), 2);

            //保存服务水平
            UserSet_Insert(JueCheZhiCiCanShu.EconomicScore, sumScore);

            JObject result = new JObject();

            //strResult = string.Join(",", "");

            result.Add("基建费用服务水平", service1.ToString());
            result.Add("运行费用服务水平", service2.ToString());
            result.Add("EconomicFixedInvestmentQuanZhong", Quan1.ToString());
            result.Add("EconomicEnergyConsumptionQuanZhong", Quan2.ToString());
            result.Add("EconomicScore", sumScore.ToString());

            return Content(result.ToJson());
        }

        //获取枯水年水量保证率指标数据
        public ActionResult GetInLowWaterRate()
        {
            //标定曲线
            List<Dt_Function> dt_FunctionList = GetDt_Function(JueCheZhiCiCanShu.SafeSupplyLowWaterRateBiaoDing);

            //获取保存的输入数据和服务水平
            double inputData = UserSet_Select(JueCheZhiCiCanShu.SafeSupplyLowWaterRate);
            double service = UserSet_Select(JueCheZhiCiCanShu.SafeSupplyLowWaterRateScore);

            JObject result = new JObject();

            result.Add("枯水年水量保证率标定曲线", JArray.FromObject(dt_FunctionList));
            result.Add("SafeSupplyLowWaterRate", inputData.ToString());
            result.Add("SafeSupplyLowWaterRateScore", service.ToString());

            return Content(result.ToJson());
        }

        //获取水源水质类别指标数据
        public ActionResult GetInSourceQuality()
        {
            //获取保存的输入数据和服务水平
            //double inputData = UserSet_Select(JueCheZhiCiCanShu.TechNicalWaterQualityRate);
            double service = UserSet_Select(JueCheZhiCiCanShu.SafeSupplySourceQualityScore);

            JObject result = new JObject();

            //result.Add("TechNicalWaterQualityRate", inputData.ToString());
            result.Add("SafeSupplySourceQualityScore", service.ToString());

            return Content(result.ToJson());
        }

        //获取水源备用比例指标数据
        public ActionResult GetInSourceStandby()
        {
            //获取保存的输入数据和服务水平
            //double inputData = UserSet_Select(JueCheZhiCiCanShu.TechNicalWaterQualityRate);
            double service = UserSet_Select(JueCheZhiCiCanShu.SafeSupplySourceStandbyScore);

            JObject result = new JObject();

            //result.Add("TechNicalWaterQualityRate", inputData.ToString());
            result.Add("SafeSupplySourceStandbyScore", service.ToString());

            return Content(result.ToJson());
        }

        //获取调蓄水量比率指标数据
        public ActionResult GetInAdjustRate()
        {
            //标定曲线
            List<Dt_Function> dt_FunctionList = GetDt_Function(JueCheZhiCiCanShu.SafeSupplyAdjustRateBiaoDing);

            //获取保存的输入数据和服务水平
            double inputData = UserSet_Select(JueCheZhiCiCanShu.SafeSupplyAdjustRate);
            double service = UserSet_Select(JueCheZhiCiCanShu.SafeSupplyAdjustRateScore);

            JObject result = new JObject();

            result.Add("调蓄水量比率标定曲线", JArray.FromObject(dt_FunctionList));
            result.Add("SafeSupplyAdjustRate", inputData.ToString());
            result.Add("SafeSupplyAdjustRateScore", service.ToString());

            return Content(result.ToJson());
        }
        
        //供给安全综合评价
        public ActionResult GetSupplySum()
        {
            //获取枯水年水量保证率、水源水质类别、水源备用比例、调蓄水量比率的权重
            double Quan1 = UserSet_Select(JueCheZhiCiCanShu.SafeSupplyLowWaterRateQZ);
            double Quan2 = UserSet_Select(JueCheZhiCiCanShu.SafeSupplySourceQualityQZ);
            double Quan3 = UserSet_Select(JueCheZhiCiCanShu.SafeSupplySourceStandbyQZ);
            double Quan4 = UserSet_Select(JueCheZhiCiCanShu.SafeSupplyAdjustRateQZ);

            //获取服务水平
            double Service1 = UserSet_Select(JueCheZhiCiCanShu.SafeSupplyLowWaterRateScore);
            double Service2 = UserSet_Select(JueCheZhiCiCanShu.SafeSupplySourceQualityScore);
            double Service3 = UserSet_Select(JueCheZhiCiCanShu.SafeSupplySourceStandbyScore);
            double Service4 = UserSet_Select(JueCheZhiCiCanShu.SafeSupplyAdjustRateScore);

            //计算供给安全服务水平
            double sumScore = Math.Round((Service1 * Quan1 + Service2 * Quan2 + Service3 * Quan3 + Service4 * Quan4), 2);

            //保存服务水平
            UserSet_Insert(JueCheZhiCiCanShu.SafeSupplySumScore, sumScore);

            JObject result = new JObject();

            result.Add("枯水年水量保证率服务水平", Service1.ToString());
            result.Add("水源水质类别服务水平", Service2.ToString());
            result.Add("水源备用比例服务水平", Service3.ToString());
            result.Add("调蓄水量比率服务水平", Service4.ToString());
            result.Add("SafeSupplyLowWaterRateQZ", Quan1.ToString());
            result.Add("SafeSupplySourceQualityQZ", Quan2.ToString());
            result.Add("SafeSupplySourceStandbyQZ", Quan3.ToString());
            result.Add("SafeSupplyAdjustRateQZ", Quan4.ToString());
            result.Add("SafeSupplySumScore", sumScore.ToString());

            return Content(result.ToJson());
        }

        //获取管网综合评估描述
        public ActionResult GetSumDescription()
        {
            //获取各指标项目的标志ItemLable
            //http://localhost:4921/Elaborate/Evaluate/GetSumDescription
            StringBuilder strBSql = new StringBuilder();
            strBSql.Append(" SELECT ID, ItemName, ItemLable, [Value], '' Description, '' Action, Type ");
            strBSql.Append(" FROM SPF_Description WHERE Type in (1,2)  ");
            List<SPFDescriptione> spfDescriptionList = dbcontext.Database.SqlQuery<SPFDescriptione>(strBSql.ToString()).ToList();

            foreach(SPFDescriptione item in spfDescriptionList)
            {
                double ServiceTemp = UserSet_Select(item.ItemLable);
                string strSql= " UPDATE SPF_Description SET [Value]= "  + ServiceTemp + " WHERE ID= " + item.ID ;
                dbcontext.Database.ExecuteSqlCommand(strSql);
            }

            strBSql.Clear();
            strBSql.Append(" SELECT ID, ItemName, ItemLable, [Value], ");
            strBSql.Append("    (case when [Value] >= 3 THEN Desc3 when [Value] >= 2 THEN Desc2 when [Value] >= 1 THEN Desc1 ELSE Desc0 END) Description, ");
            strBSql.Append("    (case when [Value] >= 3 THEN Action3 when [Value] >= 2 THEN Action2 when [Value] >= 1 THEN Action1 ELSE Action0 END) Action, Type ");
            strBSql.Append(" FROM SPF_Description WHERE Type in (1,2) ");
            List<SPFDescriptione> resultList = dbcontext.Database.SqlQuery<SPFDescriptione>(strBSql.ToString()).ToList();

            return Content(resultList.ToJson());
        }

        //获取管网爆管概率指标
        public ActionResult GetBreakProbability()
        {
            //获取方案id
            int scenId = dbcontext.Database.SqlQuery<int>("SELECT SC_ID FROM SPF_WGSCEN WHERE SCEN_NAME='最高时'").FirstOrDefault();

            //获取管网爆管概率信息列表
            StringBuilder strBSql = new StringBuilder();
            strBSql.Append(" SELECT TOP 60 pp.PIPE_Lable, pp.PIPE_MATERIALS, pp.PIPE_AGE, pp.underpress, pp.MaterialScore, ");
            strBSql.Append("       round((case when dt.y2=dt.y1 then dt.y2 when dt.x2=dt.x1 then 0  ");
            strBSql.Append("                   else (pp.PIPE_AGE-dt.x1)*(dt.y2-dt.y1)/(dt.x2-dt.x1)+dt.y1  end),2) ageScore, ");
            strBSql.Append("       round((case when dt1.y2=dt1.y1 then dt1.y2 when dt1.x2=dt1.x1 then 0  ");
            strBSql.Append("                   else (pp.underpress-dt1.x1)*(dt1.y2-dt1.y1)/(dt1.x2-dt1.x1)+dt1.y1  end),2) pressScore ");
            strBSql.Append(" FROM  ");
            strBSql.Append("    ((SELECT p.PIPE_Lable, p.PIPE_MATERIALS, p.PIPE_AGE, round((case when n1.Pressure_VALUE IS null and n2.Pressure_VALUE is null then 0  ");
            strBSql.Append("             when n1.Pressure_VALUE IS not null and n2.Pressure_VALUE is null then n1.Pressure_VALUE  ");
            strBSql.Append("             when n1.Pressure_VALUE IS null and n2.Pressure_VALUE is not null then n2.Pressure_VALUE ");
            strBSql.Append("             when n1.Pressure_VALUE > n2.Pressure_VALUE then n1.Pressure_VALUE ");
            strBSql.Append("             else n2.Pressure_VALUE ");
            strBSql.Append("             end)/(pb.UnderPressure),2) underpress, pb.ServiceScore MaterialScore    ");
            strBSql.Append("         FROM ((SPF_WGPIPE p  ");
            strBSql.Append("             left join SPF_JCWGNodePressure n1  ");
            strBSql.Append("             on p.NodeID_START=n1.Node_Label ");
            strBSql.Append("             and n1.SCEN_ID=" + scenId + ") ");
            strBSql.Append("             left join SPF_JCWGNodePressure n2  ");
            strBSql.Append("             on p.NodeID_END=n2.Node_Label  ");
            strBSql.Append("             and n2.SCEN_ID=" + scenId + ") ");
            strBSql.Append("             left join SPF_JCWGPipeBreakFactor pb ");
            strBSql.Append("             on p.PIPE_MATERIALS=pb.Material ) pp  ");
            strBSql.Append("   left join SPF_dt_Fuction dt ");
            strBSql.Append("   on ((pp.PIPE_AGE>dt.x1 and pp.PIPE_AGE<dt.x2) or (pp.PIPE_AGE=dt.x1))  ");
            strBSql.Append("         and  dt.C_Name='" + JueCheZhiCiCanShu.SafeSecurityBreakFactorPipeAge + "') ");
            strBSql.Append("   left join SPF_dt_Fuction dt1 ");
            strBSql.Append("   on ((pp.underpress>dt1.x1 and pp.underpress<dt1.x2) or (pp.underpress=dt1.x1))  ");
            strBSql.Append("         and  dt1.C_Name='" + JueCheZhiCiCanShu.SafeSecurityBreakFactorPress + "' ");
            strBSql.Append("   WHERE  pp.PIPE_MATERIALS IS NOT NULL ");
            List<PipeBreak> pipeBreakList = dbcontext.Database.SqlQuery<PipeBreak>(strBSql.ToString()).ToList();


            //获取管材、管龄、压力/承压的平均服务水平
            strBSql = new StringBuilder();
            strBSql.Append(" SELECT Round(AVG(pp.MaterialScore),2) avgmaterial, ");
            strBSql.Append("        Round(AVG((case when dt.y2=dt.y1 then dt.y2 when dt.x2=dt.x1 then 0  ");
            strBSql.Append("              else (pp.PIPE_AGE-dt.x1)*(dt.y2-dt.y1)/(dt.x2-dt.x1)+dt.y1  end)),2) avgage, ");
            strBSql.Append("        Round(AVG((case when dt1.y2=dt1.y1 then dt1.y2 when dt1.x2=dt1.x1 then 0  ");
            strBSql.Append("              else (pp.underpress-dt1.x1)*(dt1.y2-dt1.y1)/(dt1.x2-dt1.x1)+dt1.y1  end)),2) avgpress ");
            strBSql.Append(" FROM  ");
            strBSql.Append("    ((SELECT p.PIPE_Lable, p.PIPE_MATERIALS, p.PIPE_AGE, round((case when n1.Pressure_VALUE IS null and n2.Pressure_VALUE is null then 0  ");
            strBSql.Append("             when n1.Pressure_VALUE IS not null and n2.Pressure_VALUE is null then n1.Pressure_VALUE  ");
            strBSql.Append("             when n1.Pressure_VALUE IS null and n2.Pressure_VALUE is not null then n2.Pressure_VALUE ");
            strBSql.Append("             when n1.Pressure_VALUE > n2.Pressure_VALUE then n1.Pressure_VALUE ");
            strBSql.Append("             else n2.Pressure_VALUE ");
            strBSql.Append("             end)/(pb.UnderPressure),2) underpress, pb.ServiceScore MaterialScore    ");
            strBSql.Append("         FROM ((SPF_WGPIPE p  ");
            strBSql.Append("             left join SPF_JCWGNodePressure n1  ");
            strBSql.Append("             on p.NodeID_START=n1.Node_Label ");
            strBSql.Append("             and n1.SCEN_ID=" + scenId + ") ");
            strBSql.Append("             left join SPF_JCWGNodePressure n2  ");
            strBSql.Append("             on p.NodeID_END=n2.Node_Label  ");
            strBSql.Append("             and n2.SCEN_ID=" + scenId + ") ");
            strBSql.Append("             left join SPF_JCWGPipeBreakFactor pb ");
            strBSql.Append("             on p.PIPE_MATERIALS=pb.Material ) pp  ");
            strBSql.Append("   left join SPF_dt_Fuction dt ");
            strBSql.Append("   on ((pp.PIPE_AGE>dt.x1 and pp.PIPE_AGE<dt.x2) or (pp.PIPE_AGE=dt.x1))  ");
            strBSql.Append("         and  dt.C_Name='" + JueCheZhiCiCanShu.SafeSecurityBreakFactorPipeAge + "') "); 
            strBSql.Append("   left join SPF_dt_Fuction dt1 ");
            strBSql.Append("   on ((pp.underpress>dt1.x1 and pp.underpress<dt1.x2) or (pp.underpress=dt1.x1))  ");
            strBSql.Append("         and  dt1.C_Name='" + JueCheZhiCiCanShu.SafeSecurityBreakFactorPress + "' ");
            PipeBreakAvg pipeBreakAvg = dbcontext.Database.SqlQuery<PipeBreakAvg>(strBSql.ToString()).FirstOrDefault();

            //获取管材、管龄、压力/承压的服务水平
            UserSet_Insert(JueCheZhiCiCanShu.SafeSecurityBreakFactorMaterialScore, pipeBreakAvg.avgmaterial);
            UserSet_Insert(JueCheZhiCiCanShu.SafeSecurityBreakFactorPipeAgeScore, pipeBreakAvg.avgage);
            UserSet_Insert(JueCheZhiCiCanShu.SafeSecurityBreakFactorPressScore, pipeBreakAvg.avgpress);

            //管龄、压力/承压的标定曲线
            List<Dt_Function> dt_FunctionList1 = GetDt_Function(JueCheZhiCiCanShu.SafeSecurityBreakFactorPipeAge);
            List<Dt_Function> dt_FunctionList2 = GetDt_Function(JueCheZhiCiCanShu.SafeSecurityBreakFactorPress);

            //获取管材、管龄、压力/承压的权重
            double Quan1 = UserSet_Select(JueCheZhiCiCanShu.SafeSecurityBreakFactorMaterialQZ);
            double Quan2 = UserSet_Select(JueCheZhiCiCanShu.SafeSecurityBreakFactorPipeAgeQZ);
            double Quan3 = UserSet_Select(JueCheZhiCiCanShu.SafeSecurityBreakFactorPressQZ);

            //计算服务水平
            double Service = Math.Round((pipeBreakAvg.avgmaterial* Quan1 + pipeBreakAvg.avgage * Quan2 + pipeBreakAvg.avgpress * Quan3),2);
            //保存服务水平
            UserSet_Insert(JueCheZhiCiCanShu.SafeSecurityBreakProbabilityScore, Service);

            JObject result = new JObject();

            result.Add("管网爆管概率信息列表", JArray.FromObject(pipeBreakList));
            result.Add("管材、管龄、压力/承压的平均服务水平", pipeBreakAvg.ToJson());
            result.Add("管龄标定曲线", JArray.FromObject(dt_FunctionList1));
            result.Add("压力/承压标定曲线", JArray.FromObject(dt_FunctionList2));
            result.Add("SafeSecurityBreakFactorMaterialQZ", Quan1.ToString());
            result.Add("SafeSecurityBreakFactorPipeAgeQZ", Quan2.ToString());
            result.Add("SafeSecurityBreakFactorPressQZ", Quan3.ToString());
            result.Add("SafeSecurityBreakProbabilityScore", Service.ToString());

            return Content(result.ToJson());
        }

        //获取事件时节点流量保证率指标数据
        public ActionResult GetAccidentFlowRate()
        {
            //获取方案id
            int scenId = dbcontext.Database.SqlQuery<int>("SELECT SC_ID FROM SPF_WGSCEN WHERE SCEN_NAME='最高时'").FirstOrDefault();
            int scenId1 = dbcontext.Database.SqlQuery<int>("SELECT SC_ID FROM SPF_WGSCEN WHERE SCEN_NAME='事故分析'").FirstOrDefault();

            //获取事件时节点流量保证率信息列表
            StringBuilder strBSql = new StringBuilder();
            strBSql.Append(" SELECT TOP 100 d1.Node_ID Label, round(d1.Demand_VALUE,4) highDemand, round(d2.Demand_VALUE,4) accDemand, ");
            strBSql.Append("       round(case when d1.Demand_VALUE=0 then 1 else d2.Demand_VALUE/d1.Demand_VALUE end,2) ratio , ");
            strBSql.Append("       case when (case when d1.Demand_VALUE=0 then 1 else d2.Demand_VALUE/d1.Demand_VALUE end)<0.7 then 0 else 1 end flag ");
            strBSql.Append(" FROM SPF_WGNodeDemand d1 LEFT JOIN SPF_WGNodeDemand d2 ON d1.Node_ID =d2.Node_ID ");
            strBSql.Append(" WHERE d1.SCEN_ID =" + scenId + " and d2.SCEN_ID=" + scenId1 );
            strBSql.Append(" ORDER BY d1.Node_ID ");
            List<NodeAccident> nodeAccidentList = dbcontext.Database.SqlQuery<NodeAccident>(strBSql.ToString()).ToList();

            //获取事件时节点流量总数
            strBSql = new StringBuilder();
            strBSql.Append(" SELECT COUNT(1) total, SUM(case when (case when d1.Demand_VALUE=0 then 1 else d2.Demand_VALUE/d1.Demand_VALUE end)<0.7 then 0 else 1 end) totalFlag ");
            strBSql.Append(" FROM SPF_WGNodeDemand d1 LEFT JOIN SPF_WGNodeDemand d2 ON d1.Node_ID =d2.Node_ID ");
            strBSql.Append(" WHERE d1.SCEN_ID =" + scenId + " and d2.SCEN_ID=" + scenId1 );
            NodeAccidentTotal nodeAccidentTotal = dbcontext.Database.SqlQuery<NodeAccidentTotal>(strBSql.ToString()).FirstOrDefault();


            //标定曲线
            List<Dt_Function> dt_FunctionList = GetDt_Function(JueCheZhiCiCanShu.SafeSecurityAccidentFlowRateBiaoDing);


            //获取事件时节点流量保证率和服务水平
            double inputData = Math.Round(1.00* nodeAccidentTotal.totalFlag/ nodeAccidentTotal.total,3);
            double service = Get_FunctionScore(JueCheZhiCiCanShu.SafeSecurityAccidentFlowRateBiaoDing, inputData);
            //保存事件时节点流量保证率的服务水平
            UserSet_Insert(JueCheZhiCiCanShu.SafeSecurityAccidentFlowRateScore, service);

            JObject result = new JObject();

            result.Add("事件时节点流量保证率信息列表", JArray.FromObject(nodeAccidentList));
            result.Add("事件时节点流量保证率标定曲线", JArray.FromObject(dt_FunctionList));
            result.Add("事件时节点流量总数", nodeAccidentTotal.ToJson());
            result.Add("SafeSecurityAccidentFlowRate", inputData.ToString());
            result.Add("SafeSecurityAccidentFlowRateScore", service.ToString());

            return Content(result.ToJson());
        }

        //获取输配水管线备用指标
        public ActionResult GetInPipeStandby()
        {
            //标定曲线
            List<Dt_Function> dt_FunctionList = GetDt_Function(JueCheZhiCiCanShu.SafeSecurityPipeStandbyBiaoDing);

            //获取保存的输入数据和服务水平
            double inputData = UserSet_Select(JueCheZhiCiCanShu.SafeSecurityPipeStandby);
            double service = UserSet_Select(JueCheZhiCiCanShu.SafeSecurityPipeStandbyScore);

            JObject result = new JObject();

            result.Add("输配水管线备用标定曲线", JArray.FromObject(dt_FunctionList));
            result.Add("SafeSecurityPipeStandby", inputData.ToString());
            result.Add("SafeSecurityPipeStandbyScore", service.ToString());

            return Content(result.ToJson());
        }

        //管网保障综合评价
        public ActionResult GetSafeSecuritySum()
        {
            //获取管网爆管概率、事件时节点流量保证率、输配水管线备用的权重
            double Quan1 = UserSet_Select(JueCheZhiCiCanShu.SafeSecurityBreakProbabilityQZ);
            double Quan2 = UserSet_Select(JueCheZhiCiCanShu.SafeSecurityAccidentFlowRateQZ);
            double Quan3 = UserSet_Select(JueCheZhiCiCanShu.SafeSecurityPipeStandbyQZ);

            //获取服务水平
            double Service1 = UserSet_Select(JueCheZhiCiCanShu.SafeSecurityBreakProbabilityScore);
            double Service2 = UserSet_Select(JueCheZhiCiCanShu.SafeSecurityAccidentFlowRateScore);
            double Service3 = UserSet_Select(JueCheZhiCiCanShu.SafeSecurityPipeStandbyScore);

            //计算供给安全服务水平
            double sumScore = Math.Round((Service1 * Quan1 + Service2 * Quan2 + Service3 * Quan3), 2);

            //保存服务水平
            UserSet_Insert(JueCheZhiCiCanShu.SafeSecuritySumScore, sumScore);

            JObject result = new JObject();

            result.Add("管网爆管概率服务水平", Service1.ToString());
            result.Add("事件时节点流量保证率服务水平", Service2.ToString());
            result.Add("输配水管线备用服务水平", Service3.ToString());
            result.Add("SafeSecurityBreakProbabilityQZ", Quan1.ToString());
            result.Add("SafeSecurityAccidentFlowRateQZ", Quan2.ToString());
            result.Add("SafeSecurityPipeStandbyQZ", Quan3.ToString());
            result.Add("SafeSecuritySumScore", sumScore.ToString());

            return Content(result.ToJson());
        }

        //获取应急调度预案专家打分结果
        public ActionResult GetExpert()
        {
            //获取专家打分信息列表
            StringBuilder strBSql = new StringBuilder();
            strBSql.Append(" SELECT e.C_NAME name, s.Store score, s.S_ID id ");
            strBSql.Append(" FROM SPF_CExperts e, SPF_C_Expert_Score s  ");
            strBSql.Append(" WHERE e.E_ID=s.E_ID  ");
            //strBSql.Append(" ORDER BY  e.C_NAME  ");
            List<ExpertScore> expertScoreList = dbcontext.Database.SqlQuery<ExpertScore>(strBSql.ToString()).ToList();

            //获取专家打分信息汇总
            strBSql = new StringBuilder();
            strBSql.Append(" SELECT MAX(Store) max, MIN(Store) min, Round((CASE WHEN COUNT(1)>2 THEN (SUM(Store)-MAX(Store)-MIN(Store))/(COUNT(1)-2) ELSE SUM(Store)/COUNT(1) END),2) Score ");
            strBSql.Append(" FROM SPF_C_Expert_Score  ");
            ExpertScoreTotal expertScoreTotal = dbcontext.Database.SqlQuery<ExpertScoreTotal>(strBSql.ToString()).FirstOrDefault();
            UserSet_Insert(JueCheZhiCiCanShu.SafeAdminEmergencyPlanScore, expertScoreTotal.Score);

            JObject result = new JObject();

            result.Add("专家打分信息列表", JArray.FromObject(expertScoreList));
            result.Add("专家打分信息汇总", expertScoreTotal.ToJson());

            return Content(result.ToJson());
        }

        //获取应急调度预案专家打分--添加
        public ActionResult GetExpertAdd(string name, double score)
        {
            string eid= Guid.NewGuid().ToString();
            string sid= Guid.NewGuid().ToString();

            string strSQL = "INSERT INTO SPF_CExperts(E_ID, C_NAME) VALUES('" + eid + "', '" + name.Trim() + "')";
            string strSQL1 = "INSERT INTO SPF_C_Expert_Score(S_ID, E_ID, Store) VALUES('" + sid + "', '" + eid + "', " + Math.Round(score,2) + ")";
            int i = 0;
            try
            {
                i=i+dbcontext.Database.ExecuteSqlCommand(strSQL);
                i=i+dbcontext.Database.ExecuteSqlCommand(strSQL1);
            }
            catch (Exception e)
            { loger.Error(e.StackTrace); }

            if (i != 2) return Error("添加失败！");
            return Success("添加成功！");
        }

        //获取应急调度预案专家打分--修改
        public ActionResult GetExpertUpdate(string id, double score)
        {
            string strSQL = "UPDATE SPF_C_Expert_Score SET Store=" + Math.Round(score,2) + " WHERE S_ID='" + id.Trim() + "'";
            int i = 0;
            try
            {
                i = dbcontext.Database.ExecuteSqlCommand(strSQL);
            }
            catch (Exception e)
            { loger.Error(e.StackTrace); }

            if (i != 1) return Error("修改失败！");
            return Success("修改成功！");
        }

        //获取应急调度预案专家打分--删除
        public ActionResult GetExpertDel(string id)
        {
            string strSQL = "SELECT E_ID FROM SPF_C_Expert_Score WHERE S_ID='" + id.Trim() + "'";
            string eid = dbcontext.Database.SqlQuery<Guid>(strSQL).FirstOrDefault().ToString();
            int i = 0;
            try
            {
                i = dbcontext.Database.ExecuteSqlCommand("DELETE SPF_C_Expert_Score WHERE S_ID='" + id.Trim() + "'");
                i = i + dbcontext.Database.ExecuteSqlCommand("DELETE SPF_CExperts WHERE E_ID='" + eid + "'");
            }
            catch (Exception e)
            { loger.Error(e.StackTrace); }

            if (i != 2) return Error("删除失败！");
            return Success("删除成功！");
        }

        //安全性综合评价
        public ActionResult GetSafetySum()
        {
            //获取供给安全、管网保障、综合管理的权重
            double Quan1 = UserSet_Select(JueCheZhiCiCanShu.SafeSupplySumQZ);
            double Quan2 = UserSet_Select(JueCheZhiCiCanShu.SafeSecuritySumQZ);
            double Quan3 = UserSet_Select(JueCheZhiCiCanShu.SafeAdminEmergencyPlanQZ);

            //获取服务水平
            double Service1 = UserSet_Select(JueCheZhiCiCanShu.SafeSupplySumScore);
            double Service2 = UserSet_Select(JueCheZhiCiCanShu.SafeSecuritySumScore);
            double Service3 = UserSet_Select(JueCheZhiCiCanShu.SafeAdminEmergencyPlanScore);

            //计算供给安全服务水平
            double sumScore = Math.Round((Service1 * Quan1 + Service2 * Quan2 + Service3 * Quan3), 2);

            //保存服务水平
            UserSet_Insert(JueCheZhiCiCanShu.SafetyScore, sumScore);

            JObject result = new JObject();

            result.Add("供给安全服务水平", Service1.ToString());
            result.Add("管网保障服务水平", Service2.ToString());
            result.Add("综合管理服务水平", Service3.ToString());
            result.Add("SafeSupplySumQZ", Quan1.ToString());
            result.Add("SafeSecuritySumQZ", Quan2.ToString());
            result.Add("SafeAdminEmergencyPlanQZ", Quan3.ToString());
            result.Add("SafetyScore", sumScore.ToString());

            return Content(result.ToJson());
        }

        //系统综合评价
        public ActionResult GetAllSum()
        {
            //获取技术性、经济性、安全性的权重
            double Quan1 = UserSet_Select(JueCheZhiCiCanShu.TechScoreQuanZhong);
            double Quan2 = UserSet_Select(JueCheZhiCiCanShu.EconomicScoreQuanZhong);
            double Quan3 = UserSet_Select(JueCheZhiCiCanShu.SafetyScoreQuanZhong);

            //获取服务水平
            double Service1 = UserSet_Select(JueCheZhiCiCanShu.TechScore);
            double Service2 = UserSet_Select(JueCheZhiCiCanShu.EconomicScore);
            double Service3 = UserSet_Select(JueCheZhiCiCanShu.SafetyScore);

            //计算供给安全服务水平
            double sumScore = Math.Round((Service1 * Quan1 + Service2 * Quan2 + Service3 * Quan3), 2);
            //保存服务水平
            UserSet_Insert(JueCheZhiCiCanShu.AllScore, sumScore);

            //获取二级指标的数据，并绑定到数据集中
            //技术性-水力性能、水质性能、供水效率
            double TecpressureScore = UserSet_Select(JueCheZhiCiCanShu.TecpressureScore);
            double TechNicalWaterQualitySumScore = UserSet_Select(JueCheZhiCiCanShu.TechNicalWaterQualitySumScore);
            double TechNicalSupplyEfficiencySumScore = UserSet_Select(JueCheZhiCiCanShu.TechNicalSupplyEfficiencySumScore);
            double TecpressureQuanZhong = UserSet_Select(JueCheZhiCiCanShu.TecpressureQuanZhong);
            double TechNicalWaterQualitySumQZ = UserSet_Select(JueCheZhiCiCanShu.TechNicalWaterQualitySumQZ);
            double TechNicalSupplyEfficiencySumQZ = UserSet_Select(JueCheZhiCiCanShu.TechNicalSupplyEfficiencySumQZ);
            //经济性-基建费用、运行费用
            double EconomicFixedInvestmentScore = UserSet_Select(JueCheZhiCiCanShu.EconomicFixedInvestmentScore);
            double EconomicEnergyConsumptionScore = UserSet_Select(JueCheZhiCiCanShu.EconomicEnergyConsumptionScore);
            double EconomicFixedInvestmentQuanZhong = UserSet_Select(JueCheZhiCiCanShu.EconomicFixedInvestmentQuanZhong);
            double EconomicEnergyConsumptionQuanZhong = UserSet_Select(JueCheZhiCiCanShu.EconomicEnergyConsumptionQuanZhong);
            //安全性-供给安全
            double SafeSupplySumScore = UserSet_Select(JueCheZhiCiCanShu.SafeSupplySumScore);
            double SafeSupplySumQZ = UserSet_Select(JueCheZhiCiCanShu.SafeSupplySumQZ);
            //安全性-管网保障
            double SafeSecuritySumScore = UserSet_Select(JueCheZhiCiCanShu.SafeSecuritySumScore);
            double SafeSecuritySumQZ = UserSet_Select(JueCheZhiCiCanShu.SafeSecuritySumQZ);
            //安全性-综合管理
            double SafeAdminEmergencyPlanScore = UserSet_Select(JueCheZhiCiCanShu.SafeAdminEmergencyPlanScore);
            double SafeAdminEmergencyPlanQZ = UserSet_Select(JueCheZhiCiCanShu.SafeAdminEmergencyPlanQZ);

            TecpressureScore = Math.Round(TecpressureScore * TecpressureQuanZhong, 2);
            TechNicalWaterQualitySumScore= Math.Round(TechNicalWaterQualitySumScore * TechNicalWaterQualitySumQZ, 2);
            TechNicalSupplyEfficiencySumScore = Math.Round(TechNicalSupplyEfficiencySumScore * TechNicalSupplyEfficiencySumQZ, 2);
            EconomicFixedInvestmentScore = Math.Round(EconomicFixedInvestmentScore * EconomicFixedInvestmentQuanZhong, 2);
            EconomicEnergyConsumptionScore = Math.Round(EconomicEnergyConsumptionScore * EconomicEnergyConsumptionQuanZhong, 2);
            SafeSupplySumScore = Math.Round(SafeSupplySumScore * SafeSupplySumQZ, 2);
            SafeSecuritySumScore = Math.Round(SafeSecuritySumScore * SafeSecuritySumQZ, 2);
            SafeAdminEmergencyPlanScore = Math.Round(SafeAdminEmergencyPlanScore * SafeAdminEmergencyPlanQZ, 2);

            JObject result = new JObject();

            result.Add("水力性能", TecpressureScore.ToString());
            result.Add("水质性能", TechNicalWaterQualitySumScore.ToString());
            result.Add("供水效率", TechNicalSupplyEfficiencySumScore.ToString());
            result.Add("基建费用", EconomicFixedInvestmentScore.ToString());
            result.Add("运行费用", EconomicEnergyConsumptionScore.ToString());
            result.Add("供给安全", SafeSupplySumScore.ToString());
            result.Add("管网保障", SafeSecuritySumScore.ToString());
            result.Add("综合管理", SafeAdminEmergencyPlanScore.ToString());

            result.Add("技术性服务水平", Service1.ToString());
            result.Add("经济性服务水平", Service2.ToString());
            result.Add("安全性服务水平", Service3.ToString());
            result.Add("TechScoreQuanZhong", Quan1.ToString());
            result.Add("EconomicScoreQuanZhong", Quan2.ToString());
            result.Add("SafetyScoreQuanZhong", Quan3.ToString());
            result.Add("AllScore", sumScore.ToString());

            return Content(result.ToJson());
        }


        ////////////////////////////////////////////////////////////////////////////////////////

        //获取T_USERSET数据（获取权重或服务水平）
        double UserSet_Select(string t_name)
        {
            string strSQL = "SELECT T_VALUE FROM SPF_T_USERSET WHERE T_NAME='" + t_name + "'";
            return Math.Round(dbcontext.Database.SqlQuery<decimal>(strSQL).FirstOrDefault().ToDouble(), 2);
        }
        //写入T_USERSET（写入权重或服务水平）
        int UserSet_Insert(string t_name, double t_value)
        {
            string strSQL = "SELECT COUNT(1) FROM SPF_T_USERSET WHERE T_NAME='" + t_name + "'";
            int count = dbcontext.Database.SqlQuery<int>(strSQL).FirstOrDefault();
            if (count == 0)
            {
                strSQL = "INSERT INTO SPF_T_USERSET(T_ID, T_NAME, T_VALUE) VALUES('" + Guid.NewGuid() + "', '" + t_name + "', " + t_value + ")";
            }
            else
            {
                strSQL = "UPDATE SPF_T_USERSET SET T_VALUE=" + t_value + " WHERE T_NAME='" + t_name + "'";
            }
            int rows = 0;
            try
            {
                rows= dbcontext.Database.ExecuteSqlCommand(strSQL);
            }catch(Exception e)
            { loger.Error(e.StackTrace); }
             
            return rows;
        }

        //获取服务值
        double Get_FunctionScore(string c_name, double value)
        {
            StringBuilder strBSql = new StringBuilder();
            strBSql.Append(" SELECT case when y1=y2 then y1 when x2=x1 then 0 else ( " + value + " -x2)*(y2-y1)/(x2-x1)+y2 end ");
            strBSql.Append(" FROM  SPF_dt_Fuction ");
            strBSql.Append(" WHERE C_Name='" + c_name + "' ");
            strBSql.Append(" AND ((" + value + ">=x1 and " + value + "<x2) or (" + value + "<=x1 and " + value + ">x2) or ");
            strBSql.Append("   (" + value + ">x1 and " + value + "<=x2) or (" + value + "<x1 and " + value + ">=x2)) ");
            return Math.Round(dbcontext.Database.SqlQuery<double>(strBSql.ToString()).FirstOrDefault(), 2);
        }

        List<Dt_Function> GetDt_Function(string c_name)
        {
            string strSql = "SELECT x1, y1, x2, y2 FROM SPF_dt_Fuction WHERE C_Name='" + c_name + "' ORDER BY x1";
            return dbcontext.Database.SqlQuery<Dt_Function>(strSql).ToList();
        }

        List<Dt_Function> GetDt_FunctionSortByy1(string c_name)
        {
            string strSql = "SELECT x1, y1, x2, y2 FROM SPF_dt_Fuction WHERE C_Name='" + c_name + "' ORDER BY y1";
            return dbcontext.Database.SqlQuery<Dt_Function>(strSql).ToList();
        }

        double GetT_UserSet(string t_name)
        {
            string strSql = "SELECT T_VALUE FROM SPF_T_UserSet WHERE T_NAME='" + t_name + "'";
            return dbcontext.Database.SqlQuery<decimal>(strSql).FirstOrDefault().ToDouble();
        }

        class ServicePercentage
        {
            public decimal one { get; set; }
            public decimal two { get; set; }
            public decimal three { get; set; }
            public decimal four { get; set; }
        }

        class Dt_Function
        {
            public double x1 { get; set; }
            public double y1 { get; set; }
            public double x2 { get; set; }
            public double y2 { get; set; }
        }

        class PipeBreak
        {
            public string PIPE_Lable { get; set; }
            public string PIPE_MATERIALS { get; set; }
            public double PIPE_AGE { get; set; }
            public double underpress { get; set; }
            public double MaterialScore { get; set; }
            public double ageScore { get; set; }
            public double pressScore { get; set; }
        }

        class PipeBreakAvg
        {
            public double avgmaterial { get; set; }
            public double avgage { get; set; }
            public double avgpress { get; set; }
        }

        class NodeAccident
        {
            public int Label { get; set; }
            public double highDemand { get; set; }
            public double accDemand { get; set; }
            public double ratio { get; set; }
            public int flag { get; set; }
        }

        class NodeAccidentTotal
        {
            public int total { get; set; }
            public int totalFlag { get; set; }
        }

        class ExpertScore
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public double Score { get; set; }
        }

        class ExpertScoreTotal
        {
            public double Min { get; set; }
            public double Max { get; set; }
            public double Score { get; set; }
        }

        class NodePressure
        {
            public string Label { get; set; }
            public double Pressure { get; set; }
            public double Service { get; set; }
        }

        class NodeAge
        {
            public string Label { get; set; }
            public double Age { get; set; }
            public double Service { get; set; }
        }

        class NodeDemand
        {
            public string Label { get; set; }
            public double LowDemand { get; set; }
            public double MidDemand { get; set; }
            public double HighDemand { get; set; }
        }

        class NodeTotalDemand
        {
            public double TotalDemand { get; set; }
            public double LowDemand { get; set; }
            public double MidDemand { get; set; }
            public double HighDemand { get; set; }
        }

        class PipeInfo
        {
            public string Label { get; set; }
            public double Length { get; set; }
            public double Diameter { get; set; }
            public double Velocity { get; set; }
            public double EconomicSpeed { get; set; }
            public double LowVolume { get; set; }
            public double MidVolume { get; set; }
            public double HighVolume { get; set; }
        }
        class PipeCost
        {
            public string Label { get; set; }
            public decimal Length { get; set; }
            public decimal Diameter { get; set; }
            public decimal Cost { get; set; }
        }
        class PipeMCost
        {
            public string Pipe_Material { get; set; }
            public double Cost { get; set; }
        }

        class PipeToalVolume
        {
            public double TotalVolume { get; set; }
            public double LowVolume { get; set; }
            public double MidVolume { get; set; }
            public double HighVolume { get; set; }
        }

        class Maxminavg
        {
            public double MaxP { get; set; }
            public double MinP { get; set; }
            public double AvgP { get; set; }
        }

        class MaxminavgStdev
        {
            public double Max { get; set; }
            public double Min { get; set; }
            public double Avg { get; set; }
            public double Stdev { get; set; }
        }

        class HeadPercentage
        {
            public double head { get; set; }
            public double percentage { get; set; }
        }

        class SpeedPercentage
        {
            public double speed { get; set; }
            public double percentage { get; set; }
        }

        class SPFDescriptione
        {
            public int ID { get; set; }
            public string ItemName { get; set; }
            public string ItemLable { get; set; }
            public decimal Value { get; set; }
            public string Description { get; set; }
            public string Action { get; set; }
            public int Type { get; set; }
        }

    }
}