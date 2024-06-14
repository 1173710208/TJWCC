using System;

namespace TJWCC.Domain.Entity.SystemSecurity
{
    public class LogEntity : IEntity<LogEntity>, ICreationAudited
    {
        public string ID { get; set; }
        public DateTime? CURRDATE { get; set; }
        public string ACCOUNT { get; set; }
        public string NICKNAME { get; set; }
        public string TYPE { get; set; }
        public string IPADDRESS { get; set; }
        public string IPADDRESSNAME { get; set; }
        public string MODULEID { get; set; }
        public string MODULENAME { get; set; }
        public bool? RESULT { get; set; }
        public string DESCRIPTION { get; set; }
        public DateTime? CREATORTIME { get; set; }
        public string CREATORUSERID { get; set; }
    }
}
