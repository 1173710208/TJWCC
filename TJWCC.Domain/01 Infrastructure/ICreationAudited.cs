using System;

namespace TJWCC.Domain
{
    public interface ICreationAudited
    {
        string ID { get; set; }
        string CREATORUSERID { get; set; }
        DateTime? CREATORTIME { get; set; }
    }
}