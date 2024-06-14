using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Domain.IRepository.DataDisplay;
using TJWCC.Domain.Entity.DataDisplay;
using TJWCC.Repository.DataDisplay;
using TJWCC.Domain.Entity.Public;
using TJWCC.Code;

namespace TJWCC.Application.DataDisplay
{
    public class Dis_MakerClassApp
    {
        private IDis_MakerClassRepository service = new Dis_MakerClassRepository();

        public DIS_MAKERCLASSEntity GetEntity()
        {
            return service.FindEntity(i => i.ID == 2);
        }
        /// <summary>
        /// 按照不同品牌显示在线率
        /// </summary>
        /// <returns></returns>
        public string GetRadarChartData()
        {
            ///查询所有品牌的分类的安装数量和在线数量
            var query = service.IQueryable().ToList();

            var result = from q in query
                         group q by q.MNAME into d
                         select new
                         {
                             name = d.Key,
                             Max = d.Sum(q => q.INSTALLAMOUNT),
                             Value = d.Sum(q => q.ONLINEAMOUNT)
                         };

            RadarChartDataEntity radarChartData = new RadarChartDataEntity();
            foreach (var item in result)
            {
                RadarChartChildEntity entity = new RadarChartChildEntity();
                entity.Name = item.name;
                entity.Max = item.Max;
                radarChartData.Indicator.Add(entity);
                radarChartData.Value.Add(item.Value);
            }
            return radarChartData.ToJson();


        }
    }
}
