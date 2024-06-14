using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.Public
{
    public class ChartDataEntity
    {
        /// <summary>
        /// 图表名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 图表数据
        /// </summary>
        private List<ChartSeries> series;
        public List<ChartSeries> Series
        {
            get
            {
                if (series == null) { series = new List<ChartSeries>(); }
                return series;
            }
            set { series = value; }
        }


    }
    /// <summary>
    /// 图表数据
    /// </summary>
    public class ChartSeries
    {
        public string Name { get; set; }
        private List<double> data;
        public List<double> Data
        {
            get
            {
                if (data == null) { data = new List<double>(); }
                return data;
            }
            set { data = value; }
        }
    }
}
