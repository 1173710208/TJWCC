using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BSSys
{
    public class RESERVOIREntity : IEntity<RESERVOIREntity>
    {
        public int OBJECTID { get; set; }
        public Nullable<int> ElementTypeId { get; set; }
        public Nullable<int> ElementId { get; set; }
        public string GISID { get; set; }
        public string Label { get; set; }
        public Nullable<decimal> Physical_Elevation { get; set; }
        public Nullable<int> Physical_HGLPattern { get; set; }
        public Nullable<int> Is_Active { get; set; }
        public System.Data.Entity.Spatial.DbGeometry SHAPE { get; set; }
    }
}
