using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TJWCC.Domain.Entity.Public
{
    public class RadarChartDataEntity
    {
        private List<RadarChartChildEntity> indicator;
        public List<RadarChartChildEntity> Indicator
        {
            get
            {
                if (indicator == null) { indicator = new List<RadarChartChildEntity>(); }
                return indicator;
            }
            set { indicator = value; }
        }
        private List<double> value;
        public List<double> Value
        {
            get
            {
                if (value == null) { value = new List<double>(); }
                return value;
            }
            set
            {
                this.value = value;
            }
        }
    }

    public class RadarChartChildEntity
    {
        public string Name { get; set; }
        public double Max { get; set; }
    }
}