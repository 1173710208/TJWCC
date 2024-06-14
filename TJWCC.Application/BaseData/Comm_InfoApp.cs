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
    public class Comm_InfoApp
    {
        private IComm_InfoRespository service = new Comm_InfoRepository();
        /// <summary>
        /// 查找运营商全部信息
        /// </summary>
        /// <returns></returns>
        public List<COMM_INFOEntity> GetList()
        {
            return service.IQueryable().ToList();
        }

        /// <summary>
        /// 分页查询运营商信息，也可以根据运营商名称进行条件筛选查询
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="commName"></param>
        /// <returns></returns>
        public List<COMM_INFOEntity> GetList(Pagination pagination, string commName)
        {
            var expression = ExtLinq.True<COMM_INFOEntity>();
            if (!string.IsNullOrEmpty(commName))
            {
                expression = expression.And(t => t.COMMNAME.Contains(commName));
            }
            return service.FindList(expression, pagination).ToList();
        }


        /// <summary>
        /// 根据CommID获取运营商信息
        /// </summary>
        /// <param name="keyValue">CommID</param>
        /// <returns></returns>
        public COMM_INFOEntity GetForm(int CommID)
        {
            return service.FindEntity(CommID);
        }


        /// <summary>
        /// 根据ID删除运营商
        /// </summary>
        /// <param name="keyValue"></param>
        public void DeleteForm(int CommID)
        {
            service.Delete(t => t.COMMID == CommID);
        }
        /// <summary>
        /// 添加运营商信息
        /// </summary>
        /// <param name="moduleEntity">运营商实体信息</param>
        /// <param name="CommID">运营商编号</param>
        public void SubmitForm(COMM_INFOEntity moduleEntity, string CommID)
        {
            if (!string.IsNullOrEmpty(CommID))
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
