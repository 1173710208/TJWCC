using TJWCC.Application.SystemManage;
using TJWCC.Code;
using TJWCC.Domain.Entity.SystemManage;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace TJWCC.Web.Areas.SystemManage.Controllers
{
    public class ModuleButtonController : ControllerBase
    {
        private ModuleApp moduleApp = new ModuleApp();
        private ModuleButtonApp moduleButtonApp = new ModuleButtonApp();
        
        [HandlerAjaxOnly]
        public ActionResult GetTreeSelectJson(string moduleId)
        {
            var data = moduleButtonApp.GetList(moduleId);
            var treeList = new List<TreeSelectModel>();
            foreach (ModuleButtonEntity item in data)
            {
                TreeSelectModel treeModel = new TreeSelectModel();
                treeModel.id = item.ID;
                treeModel.text = item.FULLNAME;
                treeModel.parentId = item.PARENTID;
                treeList.Add(treeModel);
            }
            return Content(treeList.TreeSelectJson());
        }
        
        [HandlerAjaxOnly]
        public ActionResult GetTreeGridJson(string moduleId)
        {
            var data = moduleButtonApp.GetList(moduleId);
            var treeList = new List<TreeGridModel>();
            foreach (ModuleButtonEntity item in data)
            {
                TreeGridModel treeModel = new TreeGridModel();
                bool hasChildren = data.Count(t => t.PARENTID == item.ID) == 0 ? false : true;
                treeModel.id = item.ID;
                treeModel.isLeaf = hasChildren;
                treeModel.parentId = item.PARENTID;
                treeModel.expanded = hasChildren;
                treeModel.entityJson = item.ToJson();
                treeList.Add(treeModel);
            }
            return Content(treeList.TreeGridJson());
        }
        
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = moduleButtonApp.GetForm(keyValue);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(ModuleButtonEntity moduleButtonEntity, string keyValue)
        {
            moduleButtonApp.SubmitForm(moduleButtonEntity, keyValue);
            return Success("操作成功。");
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            moduleButtonApp.DeleteForm(keyValue);
            return Success("删除成功。");
        }
        
        public ActionResult CloneButton()
        {
            return View();
        }
        
        [HandlerAjaxOnly]
        public ActionResult GetCloneButtonTreeJson()
        {
            var moduledata = moduleApp.GetList();
            var buttondata = moduleButtonApp.GetList();
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
                tree.hasChildren = true;
                treeList.Add(tree);
            }
            foreach (ModuleButtonEntity item in buttondata)
            {
                TreeViewModel tree = new TreeViewModel();
                bool hasChildren = buttondata.Count(t => t.PARENTID == item.ID) == 0 ? false : true;
                tree.id = item.ID;
                tree.text = item.FULLNAME;
                tree.value = item.ENCODE;
                if (item.PARENTID == "0")
                {
                    tree.parentId = item.MODULEID;
                }
                else
                {
                    tree.parentId = item.PARENTID;
                }
                tree.isexpand = true;
                tree.complete = true;
                tree.showcheck = true;
                tree.hasChildren = hasChildren;
                if (item.ICON != "")
                {
                    tree.img = item.ICON;
                }
                treeList.Add(tree);
            }
            return Content(treeList.TreeViewJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult SubmitCloneButton(string moduleId, string Ids)
        {
            moduleButtonApp.SubmitCloneButton(moduleId, Ids);
            return Success("克隆成功。");
        }
    }
}
