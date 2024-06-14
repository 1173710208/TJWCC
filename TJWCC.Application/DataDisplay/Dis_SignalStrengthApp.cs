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
    public class Dis_SignalStrengthApp
    {
        private IDis_SignalStrengthRepository service = new Dis_SignalStrengthRepository();

        public DIS_SIGNALSTRENGTHEntity GetEntity()
        {
            return null;
        }


        /// <summary>
        /// 获取信号强度对比
        /// </summary>
        /// <returns></returns>
        public string GetSignalStreng()
        {

            var query = service.IQueryable();

            //信号强度分类
            var signalResult = (from q in query
                                group q by new { q.ID, q.SIGNALNAME } into d
                                select new
                                {
                                    id=d.Key.ID,
                                    signalname = d.Key.SIGNALNAME,
                                    amount = (d.Sum(q => q.AMOUNT))
                                }).OrderBy(i => new { i.id }).ToList();


            SignalStrengthEntity signalEntity = new SignalStrengthEntity();
            signalEntity.Title = "信号强度对比";


            foreach (var item in signalResult)
            {
                SignalChart alarmChart = new SignalChart();
                alarmChart.Name = item.signalname;
                alarmChart.Value = item.amount;

                signalEntity.Series.Add(alarmChart);
            }

            return signalEntity.ToJson();
        }
    }
}
