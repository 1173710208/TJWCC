using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BSSys
{
    public class PUMPEntity : IEntity<PUMPEntity>
    {
        public int OBJECTID { get; set; }
        public Nullable<int> ElementTypeId { get; set; }
        public Nullable<int> ElementId { get; set; }
        public string GISID { get; set; }
        public string Label { get; set; }
        public Nullable<decimal> Physical_Elevation { get; set; }
        public Nullable<int> Physical_Definition { get; set; }
        public Nullable<decimal> InitialSettings_SpeedFactor { get; set; }
        public Nullable<int> InitialSettings_PumpStatus { get; set; }
        public Nullable<short> Physical_IsVariableSpeedPump { get; set; }
        public Nullable<int> Physical_SpeedPattern { get; set; }
        public Nullable<decimal> DesignDischarge { get; set; }
        public Nullable<decimal> DesignHead { get; set; }
        public Nullable<decimal> MotorEfficiency { get; set; }
        public Nullable<int> PumpDefinitionType { get; set; }
        public Nullable<int> PumpEfficiencyType { get; set; }
        public Nullable<decimal> ShutoffHead { get; set; }
        public Nullable<decimal> MaxOperatingDischarge { get; set; }
        public string ControlNode { get; set; }
        public Nullable<decimal> ControlPressure { get; set; }
        public Nullable<decimal> ControlMaxSpeedFactor { get; set; }
        public Nullable<int> VSPType { get; set; }
        public Nullable<int> Is_Active { get; set; }
        public System.Data.Entity.Spatial.DbGeometry SHAPE { get; set; }
    }
}
