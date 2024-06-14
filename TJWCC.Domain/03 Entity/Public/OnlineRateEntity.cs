using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.Public
{
    public class OnlineRateEntity
    {
        /// <summary>
        /// 图表名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 图表数据
        /// </summary>
        private List<OnLineChart> series;
        public List<OnLineChart> Series
        {
            get
            {
                if (series == null) { series = new List<OnLineChart>(); }
                return series;
            }
            set { series = value; }
        }


    }

    /// <summary>
    /// 图表数据
    /// </summary>
    public class OnLineChart
    {
        public string Name { get; set; }
        public int OnLine { get; set; }
        public int Install { get; set; }

    }
}
