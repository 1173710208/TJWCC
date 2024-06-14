using System;

namespace TJWCC.Domain.Entity.SystemManage
{
    public class Business_InfoEntity : IEntity<Business_InfoEntity>
    {
        public int BUSINESSID { get; set; }
        public string BNAME { get; set; }
        public string ADDRESS { get; set; }
        public string CONTACT { get; set; }
        public string MOBILE { get; set; }
        public int COMPANYID { get; set; }
        public string REMARK { get; set; }
    }
}
