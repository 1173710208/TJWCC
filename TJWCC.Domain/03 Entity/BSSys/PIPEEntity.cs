using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BSSys
{
    public class PIPEEntity : IEntity<PIPEEntity>
    {
        public int OBJECTID { get; set; }
        public Nullable<int> ElementTypeId { get; set; }
        public Nullable<int> ElementId { get; set; }
        public string GISID { get; set; }
        public string Label { get; set; }
        public Nullable<int> StartNodeID { get; set; }
        public Nullable<int> EndNodeID { get; set; }
        public string StartNodeLabel { get; set; }
        public string EndNodeLabel { get; set; }
        public Nullable<int> StartNodeType { get; set; }
        public Nullable<int> EndNodeType { get; set; }
        public Nullable<short> IsStartNodeDownstream { get; set; }
        public Nullable<short> IsEndNodeDownstream { get; set; }
        public Nullable<decimal> Physical_PipeDiameter { get; set; }
        public Nullable<int> Physical_PipeMaterialID { get; set; }
        public Nullable<decimal> Physical_PipeMannings { get; set; }
        public Nullable<decimal> Physical_HazenWilliamsC { get; set; }
        public Nullable<decimal> Physical_DarcyWeisbachE { get; set; }
        public Nullable<decimal> Physical_Length { get; set; }
        public Nullable<short> Physical_IsUserDefinedLength { get; set; }
        public Nullable<decimal> Physical_UserDefinedLength { get; set; }
        public Nullable<short> Physical_HasCheckValve { get; set; }
        public Nullable<decimal> Physical_CompositeMinorLoss { get; set; }
        public Nullable<short> Physical_SpecifyLocalMinorLoss { get; set; }
        public Nullable<decimal> Physical_MinorLossCoefficient { get; set; }
        public string Physical_InstallationYear { get; set; }
        public string Physical_Address { get; set; }
        public Nullable<int> Physical_ZoneID { get; set; }
        public Nullable<int> Physical_DistrictID { get; set; }
        public Nullable<int> Physical_DMAID { get; set; }
        public Nullable<int> Physical_DMAAreaID { get; set; }
        public Nullable<int> Is_Active { get; set; }
        public Nullable<int> PipeStatus { get; set; }
        public System.Data.Entity.Spatial.DbGeometry SHAPE { get; set; }
        public string PressPoint { get; set; }
        public decimal SHAPE_STLength__ { get; set; }
    }
}
