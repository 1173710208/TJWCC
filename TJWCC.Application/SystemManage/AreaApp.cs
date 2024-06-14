using TJWCC.Domain.Entity.SystemManage;
using TJWCC.Domain.IRepository.SystemManage;
using TJWCC.Repository.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TJWCC.Application.SystemManage
{
    public class AreaApp
    {
        private IAreaRepository service = new AreaRepository();

        public List<AreaEntity> GetList()
        {
            return service.IQueryable().ToList();
        }
        public AreaEntity GetForm(string keyValue)
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
        public void SubmitForm(AreaEntity areaEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                areaEntity.Modify(keyValue);
                service.Update(areaEntity);
            }
            else
            {
                areaEntity.Create();
                service.Insert(areaEntity);
            }
        }
    }
}
