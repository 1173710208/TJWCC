using TJWCC.Code;
using TJWCC.Domain.Entity.SystemManage;
using TJWCC.Domain.IRepository.SystemManage;
using TJWCC.Domain.ViewModel;
using TJWCC.Repository.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TJWCC.Application.SystemManage
{
    public class SYS_DICApp
    {
        private ISYS_DICRepository service = new SYS_DICRepository();

        public List<SYS_DICEntity> GetList(int id)
        {
            return service.IQueryable(t => t.ID == id).ToList();
        }
        /// <summary>
        /// 根据类别ID获取数据
        /// </summary>
        /// <param typeId="TypeID"></param>
        /// <returns></returns>
        public List<SYS_DICEntity> GetItemList(int typeId)
        {
            return service.IQueryable(t => t.TypeID == typeId).ToList();
        }
        public List<SYS_DICEntity> Get5ItemList(int? itemId)
        {
            return service.IQueryable(t => t.TypeID == 5 && t.ItemID == itemId).ToList();
        }
    }
}
