using System;

namespace TJWCC.Domain.Entity.SystemManage
{
    public class ItemsDetailEntity : IEntity<ItemsDetailEntity>, ICreationAudited, IDeleteAudited, IModificationAudited
    {
        public string ID { get; set; }
        public string ITEMID { get; set; }
        public string PARENTID { get; set; }
        public string ITEMCODE { get; set; }
        public string ITEMNAME { get; set; }
        public string SIMPLESPELLING { get; set; }
        public bool? ISDEFAULT { get; set; }
        public int? LAYERS { get; set; }
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
