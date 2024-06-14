using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BSSys
{
    public class MaterialClassEntity : IEntity<MaterialClassEntity>
    {
        public int ID { get; set; }
        public string Material { get; set; }
    }
}
