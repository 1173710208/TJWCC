using System;

namespace TJWCC.Domain.Entity.SystemManage
{
    public class ModuleButtonEntity : IEntity<ModuleButtonEntity>, ICreationAudited, IDeleteAudited, IModificationAudited
    {
        public string ID { get; set; }
        public string MODULEID { get; set; }
        public string PARENTID { get; set; }
        public int? LAYERS { get; set; }
        public string ENCODE { get; set; }
        public string FULLNAME { get; set; }
        public string ICON { get; set; }
        public int? LOCATION { get; set; }
        public string JSEVENT { get; set; }
        public string URLADDRESS { get; set; }
        public bool? SPLIT { get; set; }
        public bool? ISPUBLIC { get; set; }
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
