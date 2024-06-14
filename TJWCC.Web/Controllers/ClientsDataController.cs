using TJWCC.Application.SystemManage;
using TJWCC.Code;
using TJWCC.Domain.Entity.SystemManage;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace TJWCC.Web.Controllers
{
    [HandlerLogin]
    public class ClientsDataController : Controller
    {
        
        [HandlerAjaxOnly]
        public ActionResult GetClientsDataJson()
        {
            var data = new
            {
                dataItems = this.GetDataItemList(),
                organize = this.GetOrganizeList(),
                shiftItems = this.GetShiftList(),
                dispatchType = this.GetDicItemList(6),
                statusItems = this.GetDicItemList(7),
                role = this.GetRoleList(),
                duty = this.GetDutyList(),
                user = "",
                areaTree = this.GetAreaTreeList(),
                authorizeMenu = this.GetMenuList(),
                authorizeButton = this.GetMenuButtonList(),
            };
            return Content(data.ToJson());
        }
        private object GetDataItemList()
        {
            var itemdata = new ItemsDetailApp().GetList();
            Dictionary<string, object> dictionaryItem = new Dictionary<string, object>();
            foreach (var item in new ItemsApp().GetList())
            {
                var dataItemList = itemdata.FindAll(t => t.ITEMID.Equals(item.ID));
                Dictionary<string, string> dictionaryItemList = new Dictionary<string, string>();
                foreach (var itemList in dataItemList)
                {
                    dictionaryItemList.Add(itemList.ITEMCODE, itemList.ITEMNAME);
                }
                dictionaryItem.Add(item.ENCODE, dictionaryItemList);
            }
            return dictionaryItem;
        }
        private object GetOrganizeList()
        {
            OrganizeApp organizeApp = new OrganizeApp();
            var data = organizeApp.GetList();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (OrganizeEntity item in data)
            {
                var fieldItem = new
                {
                    encode = item.ENCODE,
                    fullname = item.FULLNAME
                };
                dictionary.Add(item.ID, fieldItem);
            }
            return dictionary;
        }
        private object GetAreaTreeList()
        {
            SYS_DICApp sysDicApp = new SYS_DICApp();
            var data = sysDicApp.GetItemList(5);
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (SYS_DICEntity item in data)
            {
                dictionary.Add(item.ID.ToString(), item.ItemName);
            }
            return dictionary;
        }
        private object GetDicItemList(int typeId)
        {
            SYS_DICApp sysDicApp = new SYS_DICApp();
            var data = sysDicApp.GetItemList(typeId);
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (SYS_DICEntity item in data)
            {
                dictionary.Add(item.ItemID.ToString(), item.ItemName);
            }
            return dictionary;
        }
        private object GetRoleList()
        {
            RoleApp roleApp = new RoleApp();
            var data = roleApp.GetList();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (RoleEntity item in data)
            {
                var fieldItem = new
                {
                    encode = item.ENCODE,
                    fullname = item.FULLNAME
                };
                dictionary.Add(item.ID, fieldItem);
            }
            return dictionary;
        }
        private object GetDutyList()
        {
            DutyApp dutyApp = new DutyApp();
            var data = dutyApp.GetList();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (RoleEntity item in data)
            {
                var fieldItem = new
                {
                    encode = item.ENCODE,
                    fullname = item.FULLNAME
                };
                dictionary.Add(item.ID, fieldItem);
            }
            return dictionary;
        }
        private object GetShiftList()
        {
            DutyApp dutyApp = new DutyApp();
            var data = dutyApp.GetList();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (RoleEntity item in data)
            {
                dictionary.Add(item.ENCODE, item.FULLNAME);
            }
            return dictionary;
        }
        private object GetMenuList()
        {
            var roleId = OperatorProvider.Provider.GetCurrent().RoleId; 
            return ToMenuJson(new RoleAuthorizeApp().GetMenuList(roleId), "0");
        }
        private string ToMenuJson(List<ModuleEntity> data, string parentId)
        {
            StringBuilder sbJson = new StringBuilder();
            sbJson.Append("[");
            List<ModuleEntity> entitys = data.FindAll(t => t.PARENTID == parentId);
            if (entitys.Count > 0)
            {
                foreach (var item in entitys)
                {
                    string strJson = item.ToJson();
                    strJson = strJson.Insert(strJson.Length - 1, ",\"ChildNodes\":" + ToMenuJson(data, item.ID) + "");
                    sbJson.Append(strJson + ",");
                }
                sbJson = sbJson.Remove(sbJson.Length - 1, 1);
            }
            sbJson.Append("]");
            return sbJson.ToString();
        }
        private object GetMenuButtonList()
        {
            var roleId = OperatorProvider.Provider.GetCurrent().RoleId;
            var data = new RoleAuthorizeApp().GetButtonList(roleId);
            var dataModuleId = data.Distinct(new ExtList<ModuleButtonEntity>("MODULEID"));
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (ModuleButtonEntity item in dataModuleId)
            {
                var buttonList = data.Where(t => t.MODULEID.Equals(item.MODULEID));
                dictionary.Add(item.MODULEID, buttonList);
            }
            return dictionary;
        }
    }
}
