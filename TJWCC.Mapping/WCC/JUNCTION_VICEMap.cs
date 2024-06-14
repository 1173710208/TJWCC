using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.WCC;

namespace TJWCC.Mapping.WCC
{
    public class JUNCTION_VICEMap : EntityTypeConfiguration<JUNCTION_VICEEntity>
    {
        public JUNCTION_VICEMap()
        {
            this.ToTable("JUNCTION_VICE");
            this.HasKey(t => t.OBJECTID_1);
        }
    }
}
