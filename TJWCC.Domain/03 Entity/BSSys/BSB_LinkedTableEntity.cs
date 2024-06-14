using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BSSys
{
    public class BSB_LinkedTableEntity : IEntity<BSB_LinkedTableEntity>
    {
        public string WaterMeter_ID { get; set; }
        public string CALIBER { get; set; }
        public string Address { get; set; }
        public string USERNAME { get; set; }
        public Nullable<decimal> X { get; set; }
        public Nullable<decimal> Y { get; set; }
        public Nullable<int> OBJECTID { get; set; }
        public Nullable<int> ElementType { get; set; }
        public Nullable<int> ElementId { get; set; }
        public string GISID { get; set; }
        public string Label { get; set; }
        public Nullable<int> StartNodeID { get; set; }
        public Nullable<int> EndNodeID { get; set; }
        public string StartNodeLabel { get; set; }
        public string EndNodeLabel { get; set; }
        public Nullable<int> StartNodeType { get; set; }
        public Nullable<int> EndNodeType { get; set; }
        public Nullable<int> Is_Active { get; set; }
        public string Remark { get; set; }
        public int ID { get; set; }
    }
}
