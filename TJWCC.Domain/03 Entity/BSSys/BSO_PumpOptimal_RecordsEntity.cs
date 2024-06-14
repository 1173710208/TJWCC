using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BSSys
{
    public class BSO_PumpOptimal_RecordsEntity : IEntity<BSO_PumpOptimal_RecordsEntity>
    {
        public long ID { get; set; }
        public long StationId { get; set; }
        public int LocalId { get; set; }
        public int? OpenClose { get; set; }
        public decimal? SpeedRate { get; set; }
        public decimal? Power { get; set; }
        public decimal? Efficiency { get; set; }
        public decimal? Flow { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreaterId { get; set; }
        public string Remark { get; set; }
    }
}
