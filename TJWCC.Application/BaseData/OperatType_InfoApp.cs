using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Code;
using TJWCC.Domain.Entity.BaseData;
using TJWCC.Domain.IRepository.BaseData;
using TJWCC.Repository.BaseData;

namespace TJWCC.Application.BaseData
{
    public class OperatType_InfoApp
    {
        private IOperatType_InfoRespository service = new OperatType_InfoRepository();

        /// <summary>
        /// 查找查询指令类型全部信息
        /// </summary>
        /// <returns></returns>
        public List<OperatType_InfoEntity> GetList()
        {
            return service.IQueryable().ToList();
        }

        public List<OperatType_InfoEntity> GetList(string keyword)
        {
            var expression = ExtLinq.True<OperatType_InfoEntity>();
            expression = expression.And(t => t.REMARK.Contains(keyword));
            return service.IQueryable(expression).OrderBy(t => t.OTYPEID).ToList();
        }

        /// <summary>
        /// 根据ID删除查询指令类型
        /// </summary>
        /// <param name="keyValue"></param>
        public void DeleteForm(int oTypeID)
        {
            service.Delete(t => t.OTYPEID == oTypeID);
        }

        /// <summary>
        /// 添加查询指令类型信息
        /// </summary>
        /// <param name="moduleEntity">查询指令类型实体信息</param>
        /// <param name="CommID">查询指令类型编号</param>
        public void SubmitForm(OperatType_InfoEntity moduleEntity, string oTypeID)
        {
            if (!string.IsNullOrEmpty(oTypeID))
            {
                service.Update(moduleEntity);
            }
            else
            {
                service.Insert(moduleEntity);
            }
        }

    }
}
