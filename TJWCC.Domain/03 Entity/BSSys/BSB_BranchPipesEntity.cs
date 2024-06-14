using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BSSys
{
    public class BSB_BranchPipesEntity : IEntity<BSB_BranchPipesEntity>
    {
        public long ElementId { get; set; }
        public int MainBranch { get; set; }
        public string Remark { get; set; }
    }
}
