using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Mapping.BSSys
{
    public class BSB_BranchPipesMap : EntityTypeConfiguration<BSB_BranchPipesEntity>
    {
        public BSB_BranchPipesMap()
        {
            this.ToTable("BSB_BranchPipes");
            this.HasKey(t => t.ElementId);
        }
    }
}
