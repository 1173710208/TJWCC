using System;

namespace TJWCC.Domain.Entity.SystemManage
{
    public class UserEntity : IEntity<UserEntity>, ICreationAudited, IDeleteAudited, IModificationAudited
    {
        public string ID { get; set; }
        public string ACCOUNT { get; set; }
        public string REALNAME { get; set; }
        public string NICKNAME { get; set; }
        public string HEADICON { get; set; }
        public bool? GENDER { get; set; }
        public DateTime? BIRTHDAY { get; set; }
        public string MOBILEPHONE { get; set; }
        public string EMAIL { get; set; }
        public string WECHAT { get; set; }
        public string MANAGERID { get; set; }
        public int? SECURITYLEVEL { get; set; }
        public string SIGNATURE { get; set; }
        public string ORGANIZEID { get; set; }
        public string DEPARTMENTID { get; set; }
        public string ROLEID { get; set; }
        public string DUTYID { get; set; }
        public bool? ISADMINISTRATOR { get; set; }
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
