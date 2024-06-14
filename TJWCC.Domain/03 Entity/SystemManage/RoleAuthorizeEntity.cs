using System;

namespace TJWCC.Domain.Entity.SystemManage
{
    public class RoleAuthorizeEntity : IEntity<RoleAuthorizeEntity>, ICreationAudited
    {
        public string ID { get; set; }
        public int? ITEMTYPE { get; set; }
        public string ITEMID { get; set; }
        public int? OBJECTTYPE { get; set; }
        public string OBJECTID { get; set; }
        public int? SORTCODE { get; set; }
        public DateTime? CREATORTIME { get; set; }
        public string CREATORUSERID { get; set; }
    }
}
