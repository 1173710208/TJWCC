using System;

namespace TJWCC.Domain.Entity.SystemManage
{
    public class AreaEntity : IEntity<AreaEntity>, ICreationAudited, IDeleteAudited, IModificationAudited
    {
        public string ID { get; set; }
        public string PARENTID { get; set; }
        public int? LAYERS { get; set; }
        public string ENCODE { get; set; }
        public string FULLNAME { get; set; }
        public string SIMPLESPELLING { get; set; }
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
