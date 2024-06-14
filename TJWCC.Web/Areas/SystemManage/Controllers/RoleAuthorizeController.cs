using TJWCC.Application.SystemManage;
using TJWCC.Code;
using TJWCC.Domain.Entity.SystemManage;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace TJWCC.Web.Areas.SystemManage.Controllers
{
    public class RoleAuthorizeController : ControllerBase
    {
        private RoleAuthorizeApp roleAuthorizeApp = new RoleAuthorizeApp();
        private ModuleApp moduleApp = new ModuleApp();
        private ModuleButtonApp moduleButtonApp = new ModuleButtonApp();

        public ActionResult GetPermissionTree(string roleId)
        {
            var moduledata = moduleApp.GetList();
            var buttondata = moduleButtonApp.GetList();
            var authorizedata = new List<RoleAuthorizeEntity>();
            if (!string.IsNullOrEmpty(roleId))
            {
                authorizedata = roleAuthorizeApp.GetList(roleId);
            }
            var treeList = new List<TreeViewModel>();
            foreach (ModuleEntity item in moduledata)
            {
                TreeViewModel tree = new TreeViewModel();
                bool hasChildren = moduledata.Count(t => t.PARENTID == item.ID) == 0 ? false : true;
                tree.id = item.ID;
                tree.text = item.FULLNAME;
                tree.value = item.ENCODE;
                tree.parentId = item.PARENTID;
                tree.isexpand = true;
                tree.complete = true;
                tree.showcheck = true;
                tree.checkstate = authorizedata.Count(t => t.ITEMID == item.ID);
                tree.hasChildren = true;
                tree.img = item.ICON == "" ? "" : item.ICON;
                treeList.Add(tree);
            }
            foreach (ModuleButtonEntity item in buttondata)
            {
                TreeViewModel tree = new TreeViewModel();
                bool hasChildren = buttondata.Count(t => t.PARENTID == item.ID) == 0 ? false : true;
                tree.id = item.ID;
                tree.text = item.FULLNAME;
                tree.value = item.ENCODE;
                tree.parentId = item.PARENTID == "0" ? item.MODULEID : item.PARENTID;
                tree.isexpand = true;
                tree.complete = true;
                tree.showcheck = true;
                tree.checkstate = authorizedata.Count(t => t.ITEMID == item.ID);
                tree.hasChildren = hasChildren;
                tree.img = item.ICON == "" ? "" : item.ICON;
                treeList.Add(tree);
            }
            return Content(treeList.TreeViewJson());
        }
    }
}
