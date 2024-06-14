using TJWCC.Domain.Entity.SystemManage;
using TJWCC.Domain.IRepository.SystemManage;
using TJWCC.Repository.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;
using TJWCC.Code;

namespace TJWCC.Application.SystemManage
{
    public class Business_InfoApp
    {
        private IBusiness_InfoRepository service = new Business_InfoRepository();

        public List<Business_InfoEntity> GetList()
        {
            return service.IQueryable().ToList();
        }

        //通过二级公司ID，查找三级公司
        public List<Business_InfoEntity> GetList(int keyword)
        {
            var expression = ExtLinq.True<Business_InfoEntity>();
            expression = expression.And(t => t.COMPANYID == keyword);
            return service.IQueryable(expression).OrderBy(t => t.BUSINESSID).ToList();
        }

        /// <summary>
        /// 分页查询营销公司信息，也可以根据营销公司名称进行条件筛选查询
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="bName"></param>
        /// <returns></returns>
        public List<Business_InfoEntity> GetList(Pagination pagination, string bName)
        {
            var expression = ExtLinq.True<Business_InfoEntity>();
            if (!string.IsNullOrEmpty(bName))
            {
                expression = expression.And(t => t.BNAME.Contains(bName));
            }
            return service.FindList(expression, pagination).ToList();
        }

        public Business_InfoEntity GetForm(int bid)
        {
            return service.FindEntity(bid);
        }
        /// <summary>
        /// 删除二级公司信息
        /// </summary>
        /// <param name="keyValue"></param>
        public void DeleteForm(int bid)
        {
            service.Delete(t => t.BUSINESSID == bid);

        }
        /// <summary>
        /// 编辑二级公司
        /// </summary>
        /// <param name="BusinessEntity"></param>
        /// <param name="keyValue"></param>
        public void SubmitForm(Business_InfoEntity BusinessEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                service.Update(BusinessEntity);
            }
            else
            {
                service.Insert(BusinessEntity);
            }
        }
    }
}
