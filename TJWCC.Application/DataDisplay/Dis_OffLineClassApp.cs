using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Code;
using TJWCC.Domain.Entity.DataDisplay;
using TJWCC.Domain.Entity.Public;
using TJWCC.Domain.IRepository.DataDisplay;
using TJWCC.Repository.DataDisplay;

namespace TJWCC.Application.DataDisplay
{
   
    public class Dis_OffLineClassApp
    {
        
        private IDis_OffLineClassRepository service = new Dis_OffLineClassRepository();


        /// <summary>
        /// 离线天数区间对比
        /// </summary>
        /// <returns></returns>
        public string GetOffLineInfo()
        {

            var query = service.IQueryable();

            //离线天数
            var OffLineResult = (from q in query
                                group q by new {q.OFFLINEID, q.OFFLINENAME } into d
                                select new
                                {
                                    offlineID = d.Key.OFFLINEID,
                                    offlinename = d.Key.OFFLINENAME,
                                    amount = (d.Sum(q => q.AMOUNT))

                                }).OrderBy(i => new { i.offlineID }).ToList();


            OffLineDayEntity offlineInfo = new OffLineDayEntity();
            offlineInfo.Title = "离线天数区间对比";
            
            foreach (var item in OffLineResult)
            {
                OffLineChart offLineChart = new OffLineChart();
                offLineChart.Name = item.offlinename;
                offLineChart.Value = item.amount;

                offlineInfo.Series.Add(offLineChart);
            }

            return offlineInfo.ToJson();
        }

    }
}
