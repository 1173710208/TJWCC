using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TJWCC.Domain.Entity.Public
{
    public class ChartDataChildEntity
    {
        public string Name { get; set; }
        private List<ChartDataChildValueEntity> children;
        public List<ChartDataChildValueEntity> Children
        {
            get
            {
                if (children == null) { children = new List<ChartDataChildValueEntity>(); }
                return children;
            }
            set { children = value; }
        }
    }

    public class ChartDataChildValueEntity
    {
        public string Name { get; set; }
        public double Value { get; set; }
    }
}