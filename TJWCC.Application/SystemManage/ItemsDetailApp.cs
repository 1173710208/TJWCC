using TJWCC.Code;
using TJWCC.Domain.Entity.SystemManage;
using TJWCC.Domain.IRepository.SystemManage;
using TJWCC.Repository.SystemManage;
using System.Collections.Generic;
using System.Linq;

namespace TJWCC.Application.SystemManage
{
    public class ItemsDetailApp
    {
        private IItemsDetailRepository service = new ItemsDetailRepository();

        public List<ItemsDetailEntity> GetList(string itemId = "", string keyword = "")
        {
            var expression = ExtLinq.True<ItemsDetailEntity>();
            if (!string.IsNullOrEmpty(itemId))
            {
                expression = expression.And(t => t.ITEMID == itemId);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                expression = expression.And(t => t.ITEMNAME.Contains(keyword));
                expression = expression.Or(t => t.ITEMCODE.Contains(keyword));
            }
            return service.IQueryable(expression).OrderBy(t => t.SORTCODE).ToList();
        }
        public List<ItemsDetailEntity> GetItemList(string enCode)
        {
            return service.GetItemList(enCode);
        }
        public ItemsDetailEntity GetForm(string keyValue)
        {
            return service.FindEntity(keyValue);
        }
        public void DeleteForm(string keyValue)
        {
            service.Delete(t => t.ID == keyValue);
        }
        public void SubmitForm(ItemsDetailEntity itemsDetailEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                itemsDetailEntity.Modify(keyValue);
                service.Update(itemsDetailEntity);
            }
            else
            {
                itemsDetailEntity.Create();
                service.Insert(itemsDetailEntity);
            }
        }
    }
}
