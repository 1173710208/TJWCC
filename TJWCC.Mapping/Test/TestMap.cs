using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.Test;

namespace TJWCC.Mapping.Test
{
    public class TestMap : EntityTypeConfiguration<TestEntity>
    {
        public TestMap()
        {
            this.ToTable("TEST");
            this.HasKey(t => t.ID);
        }
    }
}
