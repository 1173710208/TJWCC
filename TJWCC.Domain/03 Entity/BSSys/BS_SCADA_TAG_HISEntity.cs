using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BSSys
{
    public class BS_SCADA_TAG_HISEntity : IEntity<BS_SCADA_TAG_HISEntity>
    {
        public long ID { get; set; }
        public decimal? Tag_value_ss { get; set; }
        public decimal? Tag_value_zx { get; set; }
        public decimal? Tag_value_fx { get; set; }
        public decimal? Tag_value { get; set; }
        public DateTime? Save_date { get; set; }
        public string Station_key { get; set; }
        public string Tag_key { get; set; }
        public decimal? CleanedValue { get; set; }
        public decimal? ModifiedValue { get; set; }
        public int? Number { get; set; }
    }
}
