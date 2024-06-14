using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BSSys
{
    public class BSB_ValveAreaEntity : IEntity<BSB_ValveAreaEntity>
    {
        public int Valveid { get; set; }
        public string Name { get; set; }
        public int Pointid { get; set; }
    }
}
