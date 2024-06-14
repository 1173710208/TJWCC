using System;

namespace TJWCC.Domain
{
    public interface IDeleteAudited 
    {
        /// <summary>
        /// 逻辑删除标记
        /// </summary>
        bool? DELETEMARK { get; set; }

        /// <summary>
        /// 删除实体的用户
        /// </summary>
        string DELETEUSERID { get; set; }

        /// <summary>
        /// 删除实体时间
        /// </summary>
        DateTime? DELETETIME { get; set; } 
    }
}