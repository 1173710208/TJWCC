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
    public class MeterType_InfoApp
    {
        private IMeterType_InfoRespository service = new MeterType_InfoRepository();
        public List<METERTYPE_INFOEntity> GetList()
        {
            return service.IQueryable().ToList();
        }

        /// <summary>
        /// 分页查询水表类型信息，也可以根据水表类型名称进行条件筛选查询
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="makerName"></param>
        /// <returns></returns>
        public List<METERTYPE_INFOEntity> GetList(Pagination pagination, string meterTypeName)
        {
            var expression = ExtLinq.True<METERTYPE_INFOEntity>();
            if (!string.IsNullOrEmpty(meterTypeName))
            {
                expression = expression.And(t => t.TYPENAME.Contains(meterTypeName));
            }
            return service.FindList(expression, pagination).ToList();

        }
        /// <summary>
        /// 根据TypeId获取水表类型
        /// </summary>
        /// <param name="TypeId">TypeId</param>
        /// <returns></returns>
        public METERTYPE_INFOEntity GetForm(int TypeId)
        {
            return service.FindEntity(TypeId);
        }


        /// <summary>
        /// 根据ID删除水表类型
        /// </summary>
        /// <param name="keyValue"></param>
        public void DeleteForm(int TypeId)
        {
            service.Delete(t => t.TYPEID == TypeId);
        }
        /// <summary>
        /// 添加水表类型
        /// </summary>
        /// <param name="moduleEntity">水表类型实体信息</param>
        /// <param name="TypeId">水表类型编号</param>
        public void SubmitForm(METERTYPE_INFOEntity moduleEntity, string TypeId)
        {
            if (!string.IsNullOrEmpty(TypeId))
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
