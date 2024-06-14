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

    public class BSO_PumpParamApp
    {
        private IBSO_PumpParamRepository service = new BSO_PumpParamRepository();

        /// <summary>
        /// 获取SCADA
        /// </summary>
        /// <returns></returns>
        public List<BSO_PumpParamEntity> GetList()
        {
            return service.IQueryable().ToList();
        }

        /// <summary>
        /// 分页查询SCADA数据记录
        /// </summary>
        /// <param name="psid"></param>
        /// <returns></returns>
        public List<BSO_PumpParamEntity> GetList(int? psid)
        {
            var expression = ExtLinq.True<BSO_PumpParamEntity>();
            if (psid != null)
            {
                expression = expression.And(t => t.StationId == psid);
            }
            return service.IQueryable(expression).OrderBy(i => i.LocalID).ToList();

        }
        public List<BSO_PumpParamEntity> GetListByStation(int? psid)
        {
            var expression = ExtLinq.True<BSO_PumpParamEntity>();
            if (psid != null)
            {
                expression = expression.And(t => t.StationId == psid);
            }
            return service.IQueryable(expression).ToList();

        }

        public BSO_PumpParamEntity GetPump(int? psid)
        {
            var expression = ExtLinq.True<BSO_PumpParamEntity>();
            if (psid != null)
            {
                expression = expression.And(t => t.PumpId == psid);
            }
            return service.FindEntity(expression);

        }


        /// <summary>
        /// 根据id删除SCADA数据记录
        /// </summary>
        /// <param name="keyValue"></param>
        public void DeleteForm(int id)
        {
            service.Delete(t => t.PumpId == id);
        }

        /// <summary>
        /// 添加或更新用户
        /// </summary>
        /// <param name="moduleEntity">数据实体信息</param>
        /// <param name="userID">用户编号</param>
        public void SubmitForm(BSO_PumpParamEntity moduleEntity, long? id)
        {
            if (id != null)
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
            var result = service.FindEntity(t => t.PumpId == tmp);
            return JsonConvert.SerializeObject(result);
        }
    }
}
