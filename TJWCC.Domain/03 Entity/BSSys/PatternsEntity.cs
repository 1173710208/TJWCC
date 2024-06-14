using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BSSys
{
    public class PatternsEntity : IEntity<PatternsEntity>
    {
        public int RecordID { get; set; }
        public Nullable<float> Physical_Hours { get; set; }
        public Nullable<float> Physical_Multiplier { get; set; }
        public Nullable<int> Pattern_DefinitionID { get; set; }
    }
}
