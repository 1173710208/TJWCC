using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Domain.Entity.DataDisplay;
using TJWCC.Domain.IRepository.DataDisplay;
using TJWCC.Repository.DataDisplay;
using TJWCC.Code;
using TJWCC.Domain.Entity.Public;
using TJWCC.Domain.IRepository.SystemManage;
using TJWCC.Repository.SystemManage;
using TJWCC.Application.SystemManage;

namespace TJWCC.Application.DataDisplay
{
    public class Dis_WaterApp
    {

        private IDis_WaterRepository service = new Dis_WaterRepository();
        private OrganizeApp appOrg = new OrganizeApp();

        /// <summary>
        /// 获取一级水司用水总量
        /// </summary>
        /// <returns></returns>
        public double GetAllWater()
        {
            //return service.IQueryable().Sum(i => i.AMOUNT) / 10000.0;
            return service.IQueryable().Sum(i => i.AMOUNT);
        }

        /// <summary>
        /// 获取首页24小时总用水量曲线，包含一级水司总用水量和二级公司总用水量
        /// </summary>
        /// <returns></returns>
        public string GetHourWaterIndex()
        {
            var query = service.IQueryable();
            string companyName = appOrg.GetList("0")[0].FULLNAME;

            ///一级水司总用水量
            var TJWaterResult = (from q in query
                                 group q by q.HOUR into d
                                 select new
                                 {
                                     Hour = d.Key,
                                     //Water = (d.Sum(q => q.AMOUNT) / 10000.0)
                                     Water = (d.Sum(q => q.AMOUNT))
                                 }).OrderBy(i => i.Hour).ToList();
            //二级分公司用水量
            var EJWaterResult = (from q in query
                                 group q by new { q.COMPANYID, q.CNAME, q.HOUR } into d
                                 select new
                                 {
                                     CompanyID=d.Key.COMPANYID,
                                     CName = d.Key.CNAME,
                                     Hour = d.Key.HOUR,
                                     //Water = (d.Sum(q => q.AMOUNT) / 10000.0)
                                     Water = (d.Sum(q => q.AMOUNT))
                                 }).OrderBy(i => new { i.CompanyID, i.Hour }).ToList();
            //二级公司获取系列（图例）
            var seriesResult = (from s in EJWaterResult
                                group s by s.CName into d
                                select new { CName = d.Key }).ToList();

            ChartDataEntity chartdata = new ChartDataEntity();
            //chartdata.Title = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd") + companyName + "NB-IoT水表用水对比";
            chartdata.Title = companyName + "NB水表用水对比";
            ChartSeries TJseries = new ChartSeries();
            TJseries.Name =  "NB-IoT水表总用水量";
            //绑定一级水司总用水信息
            foreach (var item in TJWaterResult)
            {
                TJseries.Data.Add(Convert.ToDouble(item.Water.ToString("N2")));
            }
            chartdata.Series.Add(TJseries);



            //绑定二级公司用水信息
            foreach (var seriesItem in seriesResult)
            {
                ChartSeries series = new ChartSeries();
                series.Name = seriesItem.CName;
                foreach (var item in EJWaterResult.Where(i => i.CName == seriesItem.CName).ToList())
                {
                    series.Data.Add(Convert.ToDouble(item.Water.ToString("N2")));
                }
                chartdata.Series.Add(series);

            }
            return chartdata.ToJson();
        }

        /// <summary>
        /// 获取一级水司名称
        /// </summary>
        /// <returns></returns>
        public string GetCompanyName()
        {
            return appOrg.GetList("0")[0].FULLNAME;
        }

        /// <summary>
        /// 获取一级水司24小时每小时用水量
        /// </summary>
        /// <returns></returns>
        public string GetHourWaterTJ()
        {
            var query = service.IQueryable();
            string companyName = appOrg.GetList("0")[0].FULLNAME;
            var result = (from q in query
                          group q by q.HOUR into d
                          select new
                          {
                              Hour = d.Key,
                              //Water = (d.Sum(q => q.AMOUNT) / 10000)
                              Water = (d.Sum(q => q.AMOUNT))
                          }).OrderBy(i => i.Hour).ToList();


            ChartDataEntity chartdata = new ChartDataEntity();
            //chartdata.Title = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd") + companyName + "NB水表总用水趋势";
            chartdata.Title = companyName + "NB水表总用水趋势";
            ChartSeries series = new ChartSeries();
            series.Name = companyName + "总用水";
            foreach (var item in result)
            {
                series.Data.Add(item.Water);
            }
            chartdata.Series.Add(series);
            return chartdata.ToJson();
        }

        /// <summary>
        /// 获取二级公司24小时每小时用水量
        /// </summary>
        /// <returns></returns>
        public string GetHourWaterEJGS()
        {
            var query = service.IQueryable();
            //分组后的用水数据
            var WaterResult = (from q in query
                               group q by new { q.COMPANYID, q.CNAME, q.HOUR } into d
                               select new
                               {
                                   CompanyID = d.Key.COMPANYID,
                                   CName = d.Key.CNAME,
                                   Hour = d.Key.HOUR,
                                   //Water = (d.Sum(q => q.AMOUNT) / 10000)
                                   Water = (d.Sum(q => q.AMOUNT))
                               }).OrderBy(i => new { i.CompanyID, i.Hour }).ToList();
            //获取系列（图例）
            var seriesResult = (from s in WaterResult
                                group s by s.CName into d
                                select new { CName = d.Key }).ToList();

            ChartDataEntity chartdata = new ChartDataEntity();
            chartdata.Title = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd") + " 二级公司NB-IoT水表用水趋势";

            foreach (var seriesItem in seriesResult)
            {
                ChartSeries series = new ChartSeries();
                series.Name = seriesItem.CName;
                foreach (var item in WaterResult.Where(i => i.CName == seriesItem.CName).ToList())
                {
                    series.Data.Add(item.Water);
                }
                chartdata.Series.Add(series);
            }
            return chartdata.ToJson();
        }

        /// <summary>
        /// 获取指定二级分公司总用水量
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public double GetEJAllWater(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                return service.IQueryable().Where(i => i.CNAME.Equals(name)).OrderBy(i=>i.COMPANYID).Sum(i => i.AMOUNT) / 10000.0;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 获取三级公司24小时每小时用水量
        /// </summary>
        /// <returns></returns>
        public string GetHourWaterSJGS()
        {
            var query = service.IQueryable();
            //分组后的用水数据
            var WaterResult = (from q in query
                               group q by new { q.BUSINESSID, q.BNAME, q.HOUR } into d
                               select new
                               {
                                   BUSINESSID=d.Key.BUSINESSID,
                                   BName = d.Key.BNAME,
                                   Hour = d.Key.HOUR,
                                   //Water = (d.Sum(q => q.AMOUNT) / 10000)
                                   Water = (d.Sum(q => q.AMOUNT))
                               }).OrderBy(i => new { i.BUSINESSID, i.Hour }).ToList();
            //获取系列（图例）
            var seriesResult = (from s in WaterResult
                                group s by s.BName into d
                                select new { BName = d.Key }).ToList();

            ChartDataEntity chartdata = new ChartDataEntity();
            chartdata.Title = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd") + " 三级公司NB-IoT水表用水趋势";

            foreach (var seriesItem in seriesResult)
            {
                ChartSeries series = new ChartSeries();
                series.Name = seriesItem.BName;
                foreach (var item in WaterResult.Where(i => i.BName == seriesItem.BName).ToList())
                {
                    series.Data.Add(item.Water);
                }
                chartdata.Series.Add(series);
            }
            return chartdata.ToJson();
        }

        /// <summary>
        /// 获取指定三级分公司总用水量
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public double GetSJAllWater(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                return service.IQueryable().Where(i => i.BNAME.Equals(name)).Sum(i => i.AMOUNT) / 10000.0;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 获取饼图二级分公司和三级分公司用水占比
        /// 二级子公司包含三级子公司饼图
        /// </summary>
        /// <returns></returns>
        public string GetWaterRation()
        {
            var query = service.IQueryable();

            ///获取二级子公司名称
            var EJresult = (from q in query
                            group q by new {q.COMPANYID, q.CNAME } into d
                            select new
                            {
                                CompanyID=d.Key.COMPANYID,
                                CName = d.Key.CNAME,
                            }).OrderBy(i=>i.CompanyID).ToList();


            //根据二级子公司获取三级子公司的名称和用水量
            var SJresult = (from q in query
                            group q by new { q.COMPANYID, q.CNAME, q.BUSINESSID, q.BNAME } into d
                            select new
                            {
                                CompanyID=d.Key.COMPANYID,
                                CName = d.Key.CNAME,
                                BusinessID=d.Key.BUSINESSID,
                                BName = d.Key.BNAME,
                                //Value = d.Sum(q => q.AMOUNT) / 10000.0
                                Value = d.Sum(q => q.AMOUNT)
                            }).OrderBy(i=>new {i.CompanyID,i.BusinessID }).ToList();


            //添加数据到图表对象  
            List<ChartDataChildEntity> listChartData = new List<ChartDataChildEntity>();
            foreach (var item in EJresult)
            {

                ChartDataChildEntity chartdata = new ChartDataChildEntity();
                chartdata.Name = item.CName;

                foreach (var itemChild in SJresult.Where(i => i.CName == item.CName).OrderBy(i =>i.BusinessID))
                {
                    ChartDataChildValueEntity valuesEntity = new ChartDataChildValueEntity();
                    valuesEntity.Name = itemChild.BName;
                    valuesEntity.Value = itemChild.Value;
                    chartdata.Children.Add(valuesEntity);
                }
                listChartData.Add(chartdata);
            }


            return listChartData.ToJson();


        }

        /// <summary>
        /// 根据二级公司获取所属各三级公司的用水量
        /// </summary>
        /// <param name="cname"></param>
        /// <returns></returns>
        public string GetBWaterByCname(string cname)
        {
            ///不为空的二级公司名称
            if (!string.IsNullOrWhiteSpace(cname))
            {
                var query = service.IQueryable().Where(d => d.CNAME == cname);
                //分组后的用水数据
                var WaterResult = (from q in query
                                   group q by new { q.BUSINESSID, q.BNAME} into d
                                   select new
                                   {
                                       BUSINESSID = d.Key.BUSINESSID,
                                       BName = d.Key.BNAME,
                                       //Water = (d.Sum(q => q.AMOUNT) / 10000)
                                       Water = d.Sum(q => q.AMOUNT)
                                   }).OrderBy(i =>i.BUSINESSID).ToList();

                ////获取系列（图例）
                //var seriesResult = (from s in WaterResult
                //                    group s by s.BName into d
                //                    select new { BName = d.Key }).ToList();

                ChartDataEntity chartdata = new ChartDataEntity();
                chartdata.Title = " 各所属营销公司用水量";
                //chartdata.Title = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd") + " 所属营销分公司用水趋势";

                foreach (var item in WaterResult)
                {
                    ChartSeries series = new ChartSeries();
                    series.Name = item.BName;
                    series.Data.Add(item.Water);
                    chartdata.Series.Add(series);
                }
                return chartdata.ToJson();
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 根据二级公司获取所属三级公司每小时的用水量
        /// </summary>
        /// <param name="cname"></param>
        /// <returns></returns>
        public string GetBHourWaterByCname(string cname)
        {
            ///不为空的二级公司名称
            if (!string.IsNullOrWhiteSpace(cname))
            {
                var query = service.IQueryable().Where(d => d.CNAME == cname);
                //分组后的用水数据
                var WaterResult = (from q in query
                                   group q by new { q.BUSINESSID, q.BNAME, q.HOUR } into d
                                   select new
                                   {
                                       BUSINESSID = d.Key.BUSINESSID,
                                       BName = d.Key.BNAME,
                                       Hour = d.Key.HOUR,
                                       //Water = (d.Sum(q => q.AMOUNT) / 10000)
                                       Water = (d.Sum(q => q.AMOUNT))
                                   }).OrderBy(i => new { i.BUSINESSID, i.Hour }).ToList();
                //获取系列（图例）
                var seriesResult = (from s in WaterResult
                                    group s by s.BName into d
                                    select new { BName = d.Key }).ToList();

                ChartDataEntity chartdata = new ChartDataEntity();
                chartdata.Title = " 所属营销分公司用水趋势";
                //chartdata.Title = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd") + " 所属营销分公司用水趋势";

                foreach (var seriesItem in seriesResult)
                {
                    ChartSeries series = new ChartSeries();
                    series.Name = seriesItem.BName;
                    foreach (var item in WaterResult.Where(i => i.BName == seriesItem.BName).ToList())
                    {
                        series.Data.Add(item.Water);
                    }
                    chartdata.Series.Add(series);
                }
                return chartdata.ToJson();
            }
            else
            {
                return "";
            }
        }

    }
}
