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
    public class MeterMaker_InfoApp
    {
        private IMeterMaker_InfoRespository service = new MeterMaker_InfoRepository();
        /// <summary>
        /// 获取水表供应商全部信息
        /// </summary>
        /// <returns></returns>
        public List<METERMAKER_INFOEntity> GetList()
        {
            return service.IQueryable().ToList();
        }

        /// <summary>
        /// 分页查询水表供应商信息，也可以根据供应商名称进行条件筛选查询
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="makerName"></param>
        /// <returns></returns>
        public List<METERMAKER_INFOEntity> GetList(Pagination pagination, string makerName)
        {
            var expression = ExtLinq.True<METERMAKER_INFOEntity>();
            if (!string.IsNullOrEmpty(makerName))
            {
                expression = expression.And(t => t.MNAME.Contains(makerName));
            }
            return service.FindList(expression, pagination).ToList();

        }



        /// <summary>
        /// 根据MakerID获取供应商信息
        /// </summary>
        /// <param name="keyValue">MakerID</param>
        /// <returns></returns>
        public METERMAKER_INFOEntity GetForm(int MakerID)
        {
            return service.FindEntity(MakerID);
        }


        /// <summary>
        /// 根据ID删除供应商信息
        /// </summary>
        /// <param name="keyValue"></param>
        public void DeleteForm(int MakerID)
        {
            service.Delete(t => t.MAKERID == MakerID);
        }
        /// <summary>
        /// 添加供应商
        /// </summary>
        /// <param name="moduleEntity">供应商实体信息</param>
        /// <param name="MakerID">供应商编号</param>
        public void SubmitForm(METERMAKER_INFOEntity moduleEntity, string MakerID)
        {
            if (!string.IsNullOrEmpty(MakerID))
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
