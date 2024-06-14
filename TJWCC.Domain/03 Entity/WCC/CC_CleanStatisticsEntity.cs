using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.WCC
{
    public class CC_CleanStatisticsEntity : IEntity<CC_CleanStatisticsEntity>
    {
        public int ID { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime CountTime { get; set; }
        public int Totality { get; set; }
        public int Clean { get; set; }
        public int Normal { get; set; }
        public decimal NormalRate { get; set; }
        public int Type { get; set; }
        public string TypeName { get; set; }
        public int Cycle { get; set; }
        public string Remark { get; set; }
    }
}
