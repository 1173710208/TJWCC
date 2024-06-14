using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BSSys
{
    public class BSOT_SysInfoEntity : IEntity<BSOT_SysInfoEntity>
    {
        public int GradeNum { get; set; }
        public string CompanyName { get; set; }
        public string ArcGISAPI { get; set; }
        public string ArcGISServer { get; set; }
        public bool IsRealtime { get; set; }
    }
}
