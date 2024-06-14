using TJWCC.Domain.Entity.SystemManage;
using TJWCC.Domain.IRepository.SystemManage;
using TJWCC.Repository.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;
using TJWCC.Code;

namespace TJWCC.Application.SystemManage
{
    public class SubBusi_InfoApp
    {
        private ISubBusi_InfoRepository service = new SubBusi_InfoRepository();

        public List<SubBusi_InfoEntity> GetList()
        {
            return service.IQueryable().ToList();
        }

        //通过三级公司ID，查找四级公司
        public List<SubBusi_InfoEntity> GetList(int keyword)
        {
            var expression = ExtLinq.True<SubBusi_InfoEntity>();
            expression = expression.And(t => t.BUSINESSID == keyword);
            return service.IQueryable(expression).OrderBy(t => t.SUBBUSIID).ToList();
        }

        /// <summary>
        /// 分页查询营销分公司信息，也可以根据营销分公司名称进行条件筛选查询
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="bName"></param>
        /// <returns></returns>
        public List<SubBusi_InfoEntity> GetList(Pagination pagination, string sName)
        {
            var expression = ExtLinq.True<SubBusi_InfoEntity>();
            if (!string.IsNullOrEmpty(sName))
            {
                expression = expression.And(t => t.SNAME.Contains(sName));
            }
            return service.FindList(expression, pagination).ToList();
        }

        public SubBusi_InfoEntity GetForm(int sid)
        {
            return service.FindEntity(sid);
        }
        /// <summary>
        /// 删除四级公司信息
        /// </summary>
        /// <param name="keyValue"></param>
        public void DeleteForm(int sid)
        {
            service.Delete(t => t.SUBBUSIID == sid);

        }
        /// <summary>
        /// 编辑四级公司
        /// </summary>
        /// <param name="SubBusiEntity"></param>
        /// <param name="keyValue"></param>
        public void SubmitForm(SubBusi_InfoEntity subBusiEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                service.Update(subBusiEntity);
            }
            else
            {
                service.Insert(subBusiEntity);
            }
        }
    }
}
