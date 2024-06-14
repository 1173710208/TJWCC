using TJWCC.Code;
using TJWCC.Domain.Entity.SystemManage;
using TJWCC.Domain.IRepository.SystemManage;
using TJWCC.Repository.SystemManage;
using System.Collections.Generic;
using System.Linq;

namespace TJWCC.Application.SystemManage
{
    public class DutyApp
    {
        private IRoleRepository service = new RoleRepository();

        public List<RoleEntity> GetList(string keyword = "")
        {
            var expression = ExtLinq.True<RoleEntity>();
            if (!string.IsNullOrEmpty(keyword))
            {
                expression = expression.And(t => t.FULLNAME.Contains(keyword));
                expression = expression.Or(t => t.ENCODE.Contains(keyword));
            }
            expression = expression.And(t => t.CATEGORY == 2);
            return service.IQueryable(expression).OrderBy(t => t.SORTCODE).ToList();
        }
        public RoleEntity GetForm(string keyValue)
        {
            return service.FindEntity(keyValue);
        }
        public void DeleteForm(string keyValue)
        {
            service.Delete(t => t.ID == keyValue);
        }
        public void SubmitForm(RoleEntity roleEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                roleEntity.Modify(keyValue);
                service.Update(roleEntity);
            }
            else
            {
                roleEntity.Create();
                roleEntity.CATEGORY = 2;
                service.Insert(roleEntity);
            }
        }
    }
}
