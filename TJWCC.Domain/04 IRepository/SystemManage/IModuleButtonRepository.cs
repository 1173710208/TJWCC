using TJWCC.Data;
using TJWCC.Domain.Entity.SystemManage;
using System.Collections.Generic;

namespace TJWCC.Domain.IRepository.SystemManage
{
    public interface IModuleButtonRepository : IRepositoryBase<ModuleButtonEntity>
    {
        void SubmitCloneButton(List<ModuleButtonEntity> entitys);
    }
}
