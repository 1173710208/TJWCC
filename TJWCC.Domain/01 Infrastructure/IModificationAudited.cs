using System;

namespace TJWCC.Domain
{
    public interface IModificationAudited
    {
        string ID { get; set; }
        string LASTMODIFYUSERID { get; set; }
        DateTime? LASTMODIFYTIME { get; set; }
    }
}