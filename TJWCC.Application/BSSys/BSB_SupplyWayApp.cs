using TJWCC.Code;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Repository.BSSys;
using TJWCC.Domain.IRepository.BSSys;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Application.BSSys
{

    public class BSB_SupplyWayApp
    {
        private IBSB_SupplyWayRepository service = new BSB_SupplyWayRepository();

        /// <summary>
        /// 获取SCADA
        /// </summary>
        /// <returns></returns>
        public List<BSB_SupplyWayEntity> GetList()
        {
            return service.IQueryable().ToList();
        }

        public List<BSB_SupplyWayEntity> GetList(int elementId, int type)
        {
            var expression = ExtLinq.True<BSB_SupplyWayEntity>();
            expression = expression.And(t => t.ElementId == elementId);
            expression = expression.And(t => t.ElementType == type);
            return service.IQueryable(expression).ToList();
        }

        /// <summary>
        /// 分页查询SCADA数据记录
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="elementId"></param>
        /// <returns></returns>
        public List<BSB_SupplyWayEntity> GetList(Pagination pagination, int elementId)
        {
            var expression = ExtLinq.True<BSB_SupplyWayEntity>();
            if (!string.IsNullOrEmpty(elementId.ToString()))
            {
                expression = expression.And(t => t.ElementId.Equals(elementId));
            }
            return service.FindList(expression, pagination).ToList();

        }


        /// <summary>
        /// 根据id删除SCADA数据记录
        /// </summary>
        /// <param name="keyValue"></param>
        public void DeleteForm(int id)
        {
            service.Delete(t => t.ElementId == id);
        }

        /// <summary>
        /// 添加或更新用户
        /// </summary>
        /// <param name="moduleEntity">数据实体信息</param>
        /// <param name="userID">用户编号</param>
        public void SubmitForm(BSB_SupplyWayEntity moduleEntity, string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                service.Update(moduleEntity);
            }
            else
            {
                service.Insert(moduleEntity);
            }
        }

        public string GetSingleFeeRecord(string id)
        {
            decimal tmp = 0;
            try
            {
                tmp = Convert.ToDecimal(id);
            }
            catch { }
            var result = service.FindEntity(t => t.ElementId == tmp);
            return JsonConvert.SerializeObject(result);
        }
    }
}
