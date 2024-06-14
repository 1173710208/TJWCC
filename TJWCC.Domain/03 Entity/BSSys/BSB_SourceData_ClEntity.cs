using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BSSys
{
    public class BSB_SourceData_ClEntity : IEntity<BSB_SourceData_ClEntity>
    {
        public long ID { get; set; }
        public Nullable<long> ElementID { get; set; }
        public Nullable<decimal> Cl { get; set; }
        public Nullable<int> Hour { get; set; }
        public string SourceName { get; set; }
    }
}
