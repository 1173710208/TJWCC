using TJWCC.Data;
using TJWCC.Domain.Entity.SystemManage;
using TJWCC.Domain.IRepository.SystemManage;
using TJWCC.Repository.SystemManage;
using System.Collections.Generic;

namespace TJWCC.Repository.SystemManage
{
    public class RoleRepository : RepositoryBase<RoleEntity>, IRoleRepository
    {
        public void DeleteForm(string keyValue)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                db.Delete<RoleEntity>(t => t.ID == keyValue);
                db.Delete<RoleAuthorizeEntity>(t => t.OBJECTID == keyValue);
                db.Commit();
            }
        }
        public void SubmitForm(RoleEntity roleEntity, List<RoleAuthorizeEntity> roleAuthorizeEntitys, string keyValue)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                if (!string.IsNullOrEmpty(keyValue))
                {
                    db.Update(roleEntity);
                }
                else
                {
                    roleEntity.CATEGORY = 1;
                    db.Insert(roleEntity);
                }
                db.Delete<RoleAuthorizeEntity>(t => t.OBJECTID == roleEntity.ID);
                db.Insert(roleAuthorizeEntitys);
                db.Commit();
            }
        }
    }
}
