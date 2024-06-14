using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.Public
{


    public class AlarmType1Entity
    {
        /// <summary>
        /// 图表名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 图表数据
        /// </summary>
        private List<AlarmType1Chart> series;
        public List<AlarmType1Chart> Series
        {
            get
            {
                if (series == null) { series = new List<AlarmType1Chart>(); }
                return series;
            }
            set { series = value; }
        }
    }


    /// <summary>
    /// 图表数据
    /// </summary>
    public class AlarmType1Chart
    {
        public string Name { get; set; }
        public int Value { get; set; }

    }






    public class AlarmTypeEntity
    {
        /// <summary>
        /// 图表名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 图表数据
        /// </summary>
        private List<AlarmTypeLegend> series;
        public List<AlarmTypeLegend> Series
        {
            get
            {
                if (series == null) { series = new List<AlarmTypeLegend>(); }
                return series;
            }
            set { series = value; }
        }
    }

    public class AlarmTypeLegend
    {
        public string Name { get; set; }

        private List<AlarmTypeChart> alarmTypeCharts;
        public List<AlarmTypeChart> AlarmTypeCharts
        {
            get
            {
                if (alarmTypeCharts == null) { alarmTypeCharts = new List<AlarmTypeChart>(); }
                return alarmTypeCharts;
            }
            set { alarmTypeCharts = value; }
        }
    }

    /// <summary>
    /// 图表数据
    /// </summary>
    public class AlarmTypeChart
    {
        public string Name { get; set; }
        public int Value { get; set; }

    }
}
