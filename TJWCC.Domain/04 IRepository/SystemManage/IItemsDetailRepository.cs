using TJWCC.Data;
using TJWCC.Domain.Entity.SystemManage;
using System.Collections.Generic;

namespace TJWCC.Domain.IRepository.SystemManage
{
    public interface IItemsDetailRepository : IRepositoryBase<ItemsDetailEntity>
    {
        List<ItemsDetailEntity> GetItemList(string enCode);
    }
}
