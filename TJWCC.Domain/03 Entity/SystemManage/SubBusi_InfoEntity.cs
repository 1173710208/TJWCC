using System;

namespace TJWCC.Domain.Entity.SystemManage
{
    public class SubBusi_InfoEntity : IEntity<SubBusi_InfoEntity>
    {
        public int SUBBUSIID { get; set; }
        public string SNAME { get; set; }
        public string ADDRESS { get; set; }
        public string CONTACT { get; set; }
        public string MOBILE { get; set; }
        public int BUSINESSID { get; set; }
        public string REMARK { get; set; }
    }
}
