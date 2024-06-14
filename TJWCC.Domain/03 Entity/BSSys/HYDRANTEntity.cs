using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BSSys
{
    public class HYDRANTEntity : IEntity<HYDRANTEntity>
    {
        public int OBJECTID { get; set; }
        public Nullable<int> ElementTypeId { get; set; }
        public Nullable<int> ElementId { get; set; }
        public string GISID { get; set; }
        public string Label { get; set; }
        public Nullable<decimal> Physical_Elevation { get; set; }
        public Nullable<decimal> Physical_Depth { get; set; }
        public string Physical_Address { get; set; }
        public Nullable<int> Physical_ZoneID { get; set; }
        public Nullable<int> Physical_DistrictID { get; set; }
        public Nullable<int> Physical_DMAID { get; set; }
        public Nullable<int> Physical_DMAAreaID { get; set; }
        public Nullable<int> Physical_Diameter { get; set; }
        public Nullable<int> Physical_Type { get; set; }
        public Nullable<int> Physical_Status { get; set; }
        public Nullable<decimal> Physical_Emitter { get; set; }
        public Nullable<int> Is_Active { get; set; }
        public System.Data.Entity.Spatial.DbGeometry SHAPE { get; set; }
    }
}
