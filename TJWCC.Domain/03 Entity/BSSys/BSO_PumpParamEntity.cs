using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BSSys
{
    public class BSO_PumpParamEntity : IEntity<BSO_PumpParamEntity>
    {
        public long PumpId { get; set; }
        public string PumpName { get; set; }
        public Nullable<long> ElementId { get; set; }
        public long StationId { get; set; }
        public int LocalID { get; set; }
        public Nullable<int> LogicID { get; set; }
        public int TankID { get; set; }
        public string PumpModel { get; set; }
        public int IsActive { get; set; }
        public Nullable<decimal> S0 { get; set; }
        public Nullable<decimal> H0 { get; set; }
        public Nullable<decimal> a2 { get; set; }
        public Nullable<decimal> a1 { get; set; }
        public Nullable<decimal> a0 { get; set; }
        public Nullable<decimal> b2 { get; set; }
        public Nullable<decimal> b1 { get; set; }
        public Nullable<decimal> b0 { get; set; }
        public string PumpType { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string CreaterId { get; set; }
        public Nullable<decimal> BasePress { get; set; }
        public Nullable<decimal> BaseFlow { get; set; }
        public Nullable<decimal> BasePower { get; set; }
        public Nullable<decimal> RotateSpeed { get; set; }
        public string Remark { get; set; }
    }
}
