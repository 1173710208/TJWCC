using TJWCC.Code;
using TJWCC.Domain.Entity.SystemManage;
using TJWCC.Domain.IRepository.SystemManage;
using TJWCC.Repository.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TJWCC.Application.SystemManage
{
    public class OrganizeApp
    {
        private IOrganizeRepository service = new OrganizeRepository();

        public List<OrganizeEntity> GetList()
        {
            return service.IQueryable().OrderBy(t => t.CREATORTIME).ToList();
        }

        /// <summary>
        /// 获取总公司的名称（PAEENTID=0）
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<OrganizeEntity> GetList(string keyword)
        {
            var expression = ExtLinq.True<OrganizeEntity>();
            expression = expression.And(t => t.PARENTID == keyword);
            return service.IQueryable(expression).ToList();
        }

        public OrganizeEntity GetForm(string keyValue)
        {
            return service.FindEntity(keyValue);
        }
        public void DeleteForm(string keyValue)
        {
            if (service.IQueryable().Count(t => t.PARENTID.Equals(keyValue)) > 0)
            {
                throw new Exception("删除失败！操作的对象包含了下级数据。");
            }
            else
            {
                service.Delete(t => t.ID == keyValue);
            }
        }
        public void SubmitForm(OrganizeEntity organizeEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                organizeEntity.Modify(keyValue);
                service.Update(organizeEntity);
            }
            else
            {
                organizeEntity.Create();
                service.Insert(organizeEntity);
            }
        }
    }
}
