using System;

namespace TJWCC.Domain.Entity.SystemManage
{
    public class SYS_DICEntity : IEntity<SYS_DICEntity>
    {
        public long ID { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public int TypeID { get; set; }
        public string TypeName { get; set; }
        public string ItemKey { get; set; }
    }
}
