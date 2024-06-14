using TJWCC.Domain.Entity.SystemManage;
using TJWCC.Domain.IRepository.SystemManage;
using TJWCC.Repository.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;
using TJWCC.Code;

namespace TJWCC.Application.SystemManage
{
    public class Company_InfoApp
    {
        private ICompany_InfoRepository service = new Company_InfoRepository();
        private IBusiness_InfoRepository businessService = new Business_InfoRepository();

        public List<Company_InfoEntity> GetList()
        {
            return service.IQueryable().ToList();
        }

        /// <summary>
        /// 分页查询二级公司信息，也可以根据二级公司名称进行条件筛选查询
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="cName"></param>
        /// <returns></returns>
        public List<Company_InfoEntity> GetList(Pagination pagination, string cName)
        {
            var expression = ExtLinq.True<Company_InfoEntity>();
            if (!string.IsNullOrEmpty(cName))
            {
                expression = expression.And(t => t.CNAME.Contains(cName));
            }
            return service.FindList(expression, pagination).ToList();
        }


        public Company_InfoEntity GetForm(int cID)
        {
            return service.FindEntity(cID);
        }
        public void DeleteForm(int cid)
        { 
            if (businessService.IQueryable().Count(t => t.COMPANYID.Equals(cid)) > 0)
            {
                throw new Exception("删除失败！操作的对象包含了下级数据。");
            }
            else
            {
                service.Delete(t => t.COMPANYID == cid);
            }
        }
        public void SubmitForm(Company_InfoEntity companyEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            { 
                service.Update(companyEntity);
            }
            else
            { 
                service.Insert(companyEntity);
            }
        }
    }
}
