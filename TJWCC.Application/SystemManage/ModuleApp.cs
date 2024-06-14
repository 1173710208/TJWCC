using TJWCC.Code;
using TJWCC.Domain.Entity.SystemManage;
using TJWCC.Domain.IRepository.SystemManage;
using TJWCC.Repository.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TJWCC.Application.SystemManage
{
    public class ModuleApp
    {
        private IModuleRepository service = new ModuleRepository();

        public List<ModuleEntity> GetList()
        { 
            return service.IQueryable().OrderBy(t => t.SORTCODE).ToList();
        }
        public ModuleEntity GetForm(string keyValue)
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
        public void SubmitForm(ModuleEntity moduleEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                moduleEntity.Modify(keyValue);
                service.Update(moduleEntity);
            }
            else
            {
                moduleEntity.Create();
                service.Insert(moduleEntity);
            }
        }
    }
}
