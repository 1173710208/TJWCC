using System;

namespace TJWCC.Domain.Entity.SystemManage
{
    public class RoleEntity : IEntity<RoleEntity>, ICreationAudited, IDeleteAudited, IModificationAudited
    {
        public string ID { get; set; }
        public string ORGANIZEID { get; set; }
        public int? CATEGORY { get; set; }
        public string ENCODE { get; set; }
        public string FULLNAME { get; set; }
        public string TYPE { get; set; }
        public bool? ALLOWEDIT { get; set; }
        public bool? ALLOWDELETE { get; set; }
        public int? SORTCODE { get; set; }
        public bool? DELETEMARK { get; set; }
        public bool? ENABLEDMARK { get; set; }
        public string DESCRIPTION { get; set; }
        public DateTime? CREATORTIME { get; set; }
        public string CREATORUSERID { get; set; }
        public DateTime? LASTMODIFYTIME { get; set; }
        public string LASTMODIFYUSERID { get; set; }
        public DateTime? DELETETIME { get; set; }
        public string DELETEUSERID { get; set; }
    }
}
