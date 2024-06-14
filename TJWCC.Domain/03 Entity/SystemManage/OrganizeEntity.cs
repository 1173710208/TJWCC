using System;

namespace TJWCC.Domain.Entity.SystemManage
{
    public class OrganizeEntity : IEntity<OrganizeEntity>, ICreationAudited, IDeleteAudited, IModificationAudited
    {
        public string ID { get; set; }
        public string PARENTID { get; set; }
        public int? LAYERS { get; set; }
        public string ENCODE { get; set; }
        public string FULLNAME { get; set; }
        public string SHORTNAME { get; set; }
        public string CATEGORYID { get; set; }
        public string MANAGERID { get; set; }
        public string TELEPHONE { get; set; }
        public string MOBILEPHONE { get; set; }
        public string WECHAT { get; set; }
        public string FAX { get; set; }
        public string EMAIL { get; set; }
        public string AREAID { get; set; }
        public string ADDRESS { get; set; }
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
