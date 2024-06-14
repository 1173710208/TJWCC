using TJWCC.Application.SystemManage;
using TJWCC.Code;
using TJWCC.Domain.Entity.SystemManage;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TJWCC.Domain.ViewModel;

namespace TJWCC.Web.Areas.SystemManage.Controllers
{
    public class SysDicController : ControllerBase
    {
        private SYS_DICApp app = new SYS_DICApp();


        /// <summary>
        /// 获取气象数据
        /// </summary>
        ///<param name="typeid"></param> 
        /// <returns></returns>
        
        public ActionResult GetItemList(int typeid)
        {
            return Content(app.GetItemList(typeid).ToJson());
        }
        
        public ActionResult Get5ItemList(int? itemId)
        {
            List<GetDataType_Result> list = new List<GetDataType_Result>();
            if (itemId == null)
            {
                var sds = app.Get5ItemList(0);
                foreach (var sd in sds)
                {
                    GetDataType_Result rdr = new GetDataType_Result()
                    {
                        Name = sd.ItemName,
                        Value = sd.ID.ToString()
                    };
                    list.Add(rdr);
                }
                return Content(list.ToJson());
            }
            else
            {
                var sds = app.Get5ItemList(itemId);
                foreach (var sd in sds)
                {
                    GetDataType_Result rdr = new GetDataType_Result()
                    {
                        Name = sd.ItemName,
                        Value = sd.ID.ToString()
                    };
                    list.Add(rdr);
                }
                return Content(list.ToJson());
            }
        }
        /// <summary>
        /// 三级分类树形结构
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTreeSelectJson()
        {
            var data = app.GetItemList(5);
            var treeList = new List<TreeSelectModel>();
            foreach (SYS_DICEntity item in data)
            {
                TreeSelectModel treeModel = new TreeSelectModel();
                treeModel.id = item.ID.ToString();
                treeModel.text = item.ItemName;
                treeModel.parentId = item.ItemID.ToString();
                treeModel.data = item;
                treeList.Add(treeModel);
            }
            return Content(treeList.TreeSelectJson());
        }
    }
}
