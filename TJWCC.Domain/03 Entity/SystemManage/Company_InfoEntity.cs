using System;

namespace TJWCC.Domain.Entity.SystemManage
{
    public class Company_InfoEntity : IEntity<Company_InfoEntity>
    {
        public int COMPANYID { get; set; }
        public string CNAME { get; set; }
        public string ADDRESS { get; set; }
        public string CONTACT { get; set; }
        public string MOBILE { get; set; }
        public string REMARK { get; set; }
    }
}
