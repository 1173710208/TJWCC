using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.Public
{

    public class SignalStrengthEntity
    {
        /// <summary>
        /// 图表名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 图表数据
        /// </summary>
        private List<SignalChart> series;
        public List<SignalChart> Series
        {
            get
            {
                if (series == null) { series = new List<SignalChart>(); }
                return series;
            }
            set { series = value; }
        }
    }

    /// <summary>
    /// 图表数据
    /// </summary>
    public class SignalChart
    {
        public string Name { get; set; }
        public int Value { get; set; }

    }
}
