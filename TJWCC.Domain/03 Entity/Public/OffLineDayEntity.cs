using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.Public
{
    public class OffLineDayEntity
    {
        /// <summary>
        /// 图表名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 图表数据
        /// </summary>
        private List<OffLineChart> series;
        public List<OffLineChart> Series
        {
            get
            {
                if (series == null) { series = new List<OffLineChart>(); }
                return series;
            }
            set { series = value; }
        }
    }

    /// <summary>
    /// 图表数据
    /// </summary>
    public class OffLineChart
    {
        public string Name { get; set; }
        public int Value { get; set; }

    }
}
