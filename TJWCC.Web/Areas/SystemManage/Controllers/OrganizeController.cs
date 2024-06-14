using TJWCC.Application.SystemManage;
using TJWCC.Code;
using TJWCC.Domain.Entity.SystemManage;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace TJWCC.Web.Areas.SystemManage.Controllers
{
    public class OrganizeController : ControllerBase
    {
        private OrganizeApp organizeApp = new OrganizeApp();

        
        [HandlerAjaxOnly]
        public ActionResult GetTreeSelectJson()
        {
            var data = organizeApp.GetList();
            var treeList = new List<TreeSelectModel>();
            foreach (OrganizeEntity item in data)
            {
                TreeSelectModel treeModel = new TreeSelectModel();
                treeModel.id = item.ID;
                treeModel.text = item.FULLNAME;
                treeModel.parentId = item.PARENTID;
                treeModel.data = item;
                treeList.Add(treeModel);
            }
            return Content(treeList.TreeSelectJson());
        }
        
        [HandlerAjaxOnly]
        public ActionResult GetTreeJson()
        {
            var data = organizeApp.GetList();
            var treeList = new List<TreeViewModel>();
            foreach (OrganizeEntity item in data)
            {
                TreeViewModel tree = new TreeViewModel();
                bool hasChildren = data.Count(t => t.PARENTID == item.ID) == 0 ? false : true;
                tree.id = item.ID;
                tree.text = item.FULLNAME;
                tree.value = item.ENCODE;
                tree.parentId = item.PARENTID;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = hasChildren;
                treeList.Add(tree);
            }
            return Content(treeList.TreeViewJson());
        }
        
        [HandlerAjaxOnly]
        public ActionResult GetTreeGridJson(string keyword)
        {
            var data = organizeApp.GetList();
            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.TreeWhere(t => t.FULLNAME.Contains(keyword));
            }
            var treeList = new List<TreeGridModel>();
            foreach (OrganizeEntity item in data)
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
            var data = organizeApp.GetForm(keyValue);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(OrganizeEntity organizeEntity, string keyValue)
        {
            organizeApp.SubmitForm(organizeEntity, keyValue);
            return Success("操作成功。");
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            organizeApp.DeleteForm(keyValue);
            return Success("删除成功。");
        }
    }
}
