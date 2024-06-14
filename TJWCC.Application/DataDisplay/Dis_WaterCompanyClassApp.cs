using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Domain.Entity.Public;
using TJWCC.Domain.Entity.DataDisplay;
using TJWCC.Domain.IRepository.DataDisplay;
using TJWCC.Repository.DataDisplay;
using TJWCC.Code;

namespace TJWCC.Application.DataDisplay
{
    public class Dis_WaterCompanyClassApp
    {
        private IDis_WaterCompanyClassRepository service = new Dis_WaterCompanyClassRepository();




        /// <summary>
        ///获取总的安装表数量
        /// </summary>
        /// <returns></returns>
        public int GetAllInstallAmount()
        {
            return service.IQueryable().Sum(i => i.INSTALLAMOUNT);
        }

        /// <summary>
        /// 总的在线数量
        /// </summary>
        /// <returns></returns>
        public int GetOnlineAmount()
        {
            return service.IQueryable().Sum(i => i.ONLINEAMOUNT); ;
        }
        /// <summary>
        /// 获取水务公司总的离线表数量
        /// </summary>
        /// <returns></returns>
        public int GetOffLineAmount()
        {
            return GetAllInstallAmount() - GetOnlineAmount();
        }

        /// <summary>
        ///获取二级公司总的安装表数量
        /// </summary>
        /// <returns></returns>
        public int GetAllInstallAmountByCName(string cname)
        {
            return service.IQueryable().Where(i => i.CNAME == cname).Sum(i => i.INSTALLAMOUNT);
        }

        /// <summary>
        /// 二级公司总的在线数量
        /// </summary>
        /// <returns></returns>
        public int GetOnlineAmountByCName(string cname)
        {
            return service.IQueryable().Where(i => i.CNAME == cname).Sum(i => i.ONLINEAMOUNT); ;
        }
        /// <summary>
        /// 获取二级公司总的离线表数量
        /// </summary>
        /// <returns></returns>
        public int GetOfflineAmountByCName(string cname)
        {
            return GetAllInstallAmountByCName(cname) - GetOnlineAmountByCName(cname);
        }


        /// <summary>
        /// 总上线率
        /// </summary>
        /// <returns></returns>
        public double GetOnlineRate()
        {
            double InstallAmount = service.IQueryable().Sum(i => i.INSTALLAMOUNT);
            double OnlineAmount = service.IQueryable().Sum(i => i.ONLINEAMOUNT);

            return (OnlineAmount / InstallAmount) * 100;
        }

        /// <summary>
        /// 二级分公司的在线率
        /// </summary>
        /// <returns></returns>
        public string GetBranchCompanyOnlineRate()
        {
            var query = service.IQueryable();

            //二级分公司上线数量
            var OnLineResult = (from q in query
                                group q by new { q.COMPANYID, q.CNAME } into d
                                select new
                                {
                                    CompanyID=d.Key.COMPANYID,
                                    CName = d.Key.CNAME,
                                    OnLine = (d.Sum(q => q.ONLINEAMOUNT)),
                                    Install = (d.Sum(q => q.INSTALLAMOUNT))
                                }).OrderBy(i => new { i.CompanyID }).ToList();


            OnlineRateEntity chartData = new OnlineRateEntity();
            chartData.Title = "二级水司在线率统计";


            foreach (var item in OnLineResult)
            {
                OnLineChart onLineChart = new OnLineChart();
                onLineChart.Name = item.CName;
                onLineChart.OnLine = item.OnLine;
                onLineChart.Install = item.Install;

                chartData.Series.Add(onLineChart);
            }

            return chartData.ToJson();
        }

        ///// <summary>
        ///// 按照不同品牌显示在线率
        ///// </summary>
        ///// <returns></returns>
        //public string GetRadarChartData()
        //{
        //    ///查询所有品牌的分类的安装数量和在线数量
        //    var query = service.IQueryable().ToList();

        //    var result = from q in query
        //                 group q by q.MNAME into d
        //                 select new
        //                 {
        //                     name = d.Key,
        //                     Max = d.Sum(q => q.INSTALLAMOUNT),
        //                     Value = d.Sum(q => q.ONLINEAMOUNT)
        //                 };

        //    RadarChartDataEntity radarChartData = new RadarChartDataEntity();
        //    foreach (var item in result)
        //    {
        //        RadarChartChildEntity entity = new RadarChartChildEntity();
        //        entity.Name = item.name;
        //        entity.Max = item.Max;
        //        radarChartData.Indicator.Add(entity);
        //        radarChartData.Value.Add(item.Value);
        //    }
        //    return radarChartData.ToJson();

        //    return null;

        //}

        /// <summary>
        /// 获取三级公司的在线率
        /// </summary>
        public string GetSubCompanyOnlineRate()
        {
            var query = service.IQueryable();

            //三级级分公司上线数量
            //var OnLineResult = (from q in query
            //                    group q by new { q.BNAME } into d
            //                    select new
            //                    {
            //                        Bname = d.Key.BNAME,
            //                        OnLine = (d.Sum(q => q.ONLINEAMOUNT)),
            //                        Install = (d.Sum(q => q.INSTALLAMOUNT))
            //                    }).OrderBy(i => new { i.Bname }).ToList();
            var OnLineResult = service.IQueryable().OrderBy(i=>i.ID).ToList();

            OnlineRateEntity chartData = new OnlineRateEntity();
            chartData.Title = "三级水司在线率统计";


            foreach (var item in OnLineResult)
            {
                OnLineChart onLineChart = new OnLineChart();
                onLineChart.Name = item.BNAME;
                onLineChart.OnLine = item.ONLINEAMOUNT;
                onLineChart.Install = item.INSTALLAMOUNT;

                chartData.Series.Add(onLineChart);
            }

            return chartData.ToJson();
        }

        /// <summary>
        /// 根据二级公司获取对应三级公司的安装数量和在线数量
        /// </summary>
        /// <returns></returns>
        public string GetInstallAndOnlineAmountByCname(string cName)
        {
            var query = service.IQueryable(); 
            var result = (from q in query
                          where q.CNAME == cName
                          select new
                          {
                              q.BNAME,
                              q.BUSINESSID,
                              q.INSTALLAMOUNT,
                              q.ONLINEAMOUNT
                          }).OrderBy(i => i.BUSINESSID);

            return result.ToJson();
        }

        /// <summary>
        /// 根据各二级公司获取对应三级公司的安装数量和在线数量（LIST）
        /// </summary>
        /// <returns></returns>
        public string GetInstallAndOnlineAmount()
        {
            var query = service.IQueryable();
            var cnameResult = query.GroupBy(i => new { i.COMPANYID, i.CNAME }).OrderBy(i => i.Key.COMPANYID).ToList();

            List<OnlineRateEntity> chartDataList =new List<OnlineRateEntity>();
            foreach (var item in cnameResult)
            {
                OnlineRateEntity chartData = new OnlineRateEntity();
                chartData.Title = item.Key.CNAME;
                var twoGradeResult = (from q in query
                              where q.CNAME == item.Key.CNAME
                              select new
                              {
                                  q.BNAME,
                                  q.BUSINESSID,
                                  q.INSTALLAMOUNT,
                                  q.ONLINEAMOUNT
                              }).OrderBy(i => i.BUSINESSID).ToList();

                foreach (var threeGradeItem in twoGradeResult)
                {
                    OnLineChart onLineChart = new OnLineChart();
                    onLineChart.Name = threeGradeItem.BNAME;
                    onLineChart.OnLine = threeGradeItem.ONLINEAMOUNT;
                    onLineChart.Install = threeGradeItem.INSTALLAMOUNT;

                    chartData.Series.Add(onLineChart);
                }
                chartDataList.Add(chartData);
            }
            return chartDataList.ToJson();
        }

    }
}
