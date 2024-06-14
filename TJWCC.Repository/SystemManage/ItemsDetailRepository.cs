using TJWCC.Data;
using TJWCC.Domain.Entity.SystemManage;
using TJWCC.Domain.IRepository.SystemManage;
using TJWCC.Repository.SystemManage;
using System.Collections.Generic;
using System.Data.Common;
using Oracle.ManagedDataAccess.Client;
using System.Text;
using TJWCC.Code;

namespace TJWCC.Repository.SystemManage
{
    public class ItemsDetailRepository : RepositoryBase<ItemsDetailEntity>, IItemsDetailRepository
    {
        private IItemsRepository service = new ItemsRepository();
        public List<ItemsDetailEntity> GetItemList(string enCode)
        {
            ItemsEntity itemsEntity = service.FindEntity(i => i.ENCODE == enCode);
            var expression = ExtLinq.True<ItemsDetailEntity>().And(t => t.ITEMID == itemsEntity.ID && t.ENABLEDMARK == true);
            return this.FindList(expression);
        }
    }
}
