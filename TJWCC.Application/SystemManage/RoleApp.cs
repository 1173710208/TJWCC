using TJWCC.Code;
using TJWCC.Domain.Entity.SystemManage;
using TJWCC.Domain.IRepository.SystemManage;
using TJWCC.Repository.SystemManage;
using System.Collections.Generic;
using System.Linq;

namespace TJWCC.Application.SystemManage
{
    public class RoleApp
    {
        private IRoleRepository service = new RoleRepository();
        private ModuleApp moduleApp = new ModuleApp();
        private ModuleButtonApp moduleButtonApp = new ModuleButtonApp();

        public List<RoleEntity> GetList(string keyword = "")
        {
            var expression = ExtLinq.True<RoleEntity>();
            if (!string.IsNullOrEmpty(keyword))
            {
                expression = expression.And(t => t.FULLNAME.Contains(keyword));
                expression = expression.Or(t => t.ENCODE.Contains(keyword));
            }
            expression = expression.And(t => t.CATEGORY == 1);
            return service.IQueryable(expression).OrderBy(t => t.SORTCODE).ToList();
        }
        public RoleEntity GetShift(string dutyId)
        {
            var expression = ExtLinq.True<RoleEntity>();
            if (!string.IsNullOrEmpty(dutyId))
            {
                expression = expression.And(t => t.ID.Equals(dutyId));
            }
            return service.IQueryable(expression).FirstOrDefault();
        }
        public RoleEntity GetForm(string keyValue)
        {
            return service.FindEntity(keyValue);
        }
        public void DeleteForm(string keyValue)
        {
            service.DeleteForm(keyValue);
        }
        public void SubmitForm(RoleEntity roleEntity, string[] permissionIds, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                roleEntity.ID = keyValue;
            }
            else
            {
                roleEntity.ID = Common.GuId();
            }
            var moduledata = moduleApp.GetList();
            var buttondata = moduleButtonApp.GetList();
            List<RoleAuthorizeEntity> roleAuthorizeEntitys = new List<RoleAuthorizeEntity>();
            foreach (var itemId in permissionIds)
            {
                RoleAuthorizeEntity roleAuthorizeEntity = new RoleAuthorizeEntity();
                roleAuthorizeEntity.ID = Common.GuId();
                roleAuthorizeEntity.OBJECTTYPE = 1;
                roleAuthorizeEntity.OBJECTID = roleEntity.ID;
                roleAuthorizeEntity.ITEMID = itemId;
                if (moduledata.Find(t => t.ID == itemId) != null)
                {
                    roleAuthorizeEntity.ITEMTYPE = 1;
                }
                if (buttondata.Find(t => t.ID == itemId) != null)
                {
                    roleAuthorizeEntity.ITEMTYPE = 2;
                }
                roleAuthorizeEntitys.Add(roleAuthorizeEntity);
            }
            service.SubmitForm(roleEntity, roleAuthorizeEntitys, keyValue);
        }
    }
}
