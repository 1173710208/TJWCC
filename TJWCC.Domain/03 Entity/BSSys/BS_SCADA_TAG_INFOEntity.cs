using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BSSys
{
    public class BS_SCADA_TAG_INFOEntity : IEntity<BS_SCADA_TAG_INFOEntity>
    {
        public long ID { get; set; }
        public string Tag_name { get; set; }
        public string Units { get; set; }
        public string Explain { get; set; }
        public string Tag_key { get; set; }
    }
}
