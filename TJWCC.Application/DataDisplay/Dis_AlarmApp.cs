using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Repository.DataDisplay;
using TJWCC.Domain.IRepository.DataDisplay;
using TJWCC.Domain.Entity.DataDisplay;
using TJWCC.Domain.Entity.Public;
using TJWCC.Code;

namespace TJWCC.Application.DataDisplay
{
    public class Dis_AlarmApp
    {
        private IDis_AlarmRepository service = new Dis_AlarmRepository();


        /// <summary>
        /// 获取二级报警数量
        /// </summary>
        /// <returns></returns>
        public string GetSubAlarmType()
        {
            var query = service.IQueryable();

            ///查询报警类型
            var alarmCName = (from q in query
                              group q by new {q.COMPANYID, q.CNAME } into d
                              select new
                              {
                                  CompanyID=d.Key.COMPANYID,
                                  TypeName = d.Key.CNAME
                              }).OrderBy(i=>i.CompanyID).ToList();

            //报警分类数量
            var alarmResult = (from q in query
                               group q by new { q.COMPANYID, q.CNAME, q.ATYPEID, q.ATYPENAME } into d
                               select new
                               {
                                   CompanyID=d.Key.COMPANYID,
                                   cnName = d.Key.CNAME,
                                   AtypeID = d.Key.ATYPEID,
                                   alarmType = d.Key.ATYPENAME,
                                   alarms = (d.Sum(q => q.ALARMS))

                               }).OrderBy(i => new { i.CompanyID,i.AtypeID }).ToList();


            AlarmTypeEntity alarmTpeEntity = new AlarmTypeEntity();
            alarmTpeEntity.Title = "二级公司报警对比";


            foreach (var typeItem in alarmCName)
            {
                AlarmTypeLegend entity = new AlarmTypeLegend();
                entity.Name = typeItem.TypeName;

                foreach (var item in alarmResult.Where(i => i.cnName == typeItem.TypeName))
                {
                    AlarmTypeChart alarmChart = new AlarmTypeChart();
                    alarmChart.Name = item.alarmType;
                    alarmChart.Value = item.alarms;
                    entity.AlarmTypeCharts.Add(alarmChart);
                }
                alarmTpeEntity.Series.Add(entity);
            }
            return alarmTpeEntity.ToJson();

        }


        /// <summary>
        /// 获取公司报警分类
        /// </summary>
        /// <returns></returns>
        public string GetAlarmType()
        {

            var query = service.IQueryable();

            //报警分类
            var alarmResult = (from q in query
                               group q by new { q.ATYPENAME, q.ATYPEID } into d
                               select new
                               {
                                   AtypeID=d.Key.ATYPEID,
                                   AtypeName = d.Key.ATYPENAME,
                                   alarms = (d.Sum(q => q.ALARMS))

                               }).OrderBy(i => new { i.AtypeID }).ToList();


            //图例
            //二级公司获取系列（图例）
            //var seriesResult = (from s in alarmResult
            //                    group s by s.alarmTypeName into d
            //                    select new { alarmTypeName = d.Key }).ToList();



            AlarmType1Entity alarmTypeEntity = new AlarmType1Entity();
            alarmTypeEntity.Title = "NB水表报警对比";


            foreach (var item in alarmResult)
            {
                AlarmType1Chart alarmChart = new AlarmType1Chart();
                alarmChart.Name = item.AtypeName;
                alarmChart.Value = item.alarms;

                alarmTypeEntity.Series.Add(alarmChart);
            }

            return alarmTypeEntity.ToJson();

        }

        /// <summary>
        /// 总报警条数
        /// </summary>
        /// <returns></returns>
        public int GetAllAlarmCount()
        {
            return service.IQueryable().Sum(i => i.ALARMS);

        }

        /// <summary>
        /// 根据二级公司名称获取该公司的总报警数量
        /// </summary>
        /// <param name="cname"></param>
        /// <returns></returns>
        public int GetAllAlarmCountByCName(string cname)
        {
            return service.IQueryable().Where(i => i.CNAME == cname).Sum(i => i.ALARMS);
        }


        /// <summary>
        /// 获取指定二级分公司的三级公司报警数量
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetSubCompanyThreeLevelAlarm(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                var query = service.IQueryable();

                var reslut = service.IQueryable().Where(i => i.CNAME.Equals(name)).ToList();
                var alarmResult = (from q in reslut
                                   group q by new { q.BUSINESSID, q.BNAME } into d
                                   select new
                                   {
                                       BusinessID = d.Key.BUSINESSID,
                                       BName = d.Key.BNAME,
                                       alarms = (d.Sum(q => q.ALARMS))

                                   }).OrderBy(i=>i.BusinessID).ToList();

                //var reslut = service.IQueryable().Where(i => i.CNAME.Equals(name)).GroupBy(i => i.BUSINESSID, i => i.BNAME).ToList();

                AlarmType1Entity alarmEntity = new AlarmType1Entity();
                alarmEntity.Title = name;

                foreach (var item in alarmResult)
                {
                    AlarmType1Chart dataChart = new AlarmType1Chart();
                    dataChart.Name = item.BName;
                    dataChart.Value = item.alarms;
                    alarmEntity.Series.Add(dataChart);
                }

                string str = alarmEntity.ToJson();
                return str;

            }
            else
            {
                return "";
            }
        }


    }
}
