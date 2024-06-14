using TJWCC.Data;
using TJWCC.Domain.Entity.SystemManage;
using TJWCC.Domain.IRepository.SystemManage;
using TJWCC.Repository.SystemManage;
using System.Collections.Generic;

namespace TJWCC.Repository.SystemManage
{
    public class ModuleButtonRepository : RepositoryBase<ModuleButtonEntity>, IModuleButtonRepository
    {
        public void SubmitCloneButton(List<ModuleButtonEntity> entitys)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                foreach (var item in entitys)
                {
                    db.Insert(item);
                }
                db.Commit();
            }
        }
    }
}
