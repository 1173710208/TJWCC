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

    public class BSO_PumpOptimal_RecordsApp
    {
        private IBSO_PumpOptimal_RecordsRepository service = new BSO_PumpOptimal_RecordsRepository();

        /// <summary>
        /// 获取SCADA
        /// </summary>
        /// <returns></returns>
        public List<BSO_PumpOptimal_RecordsEntity> GetList()
        {
            return service.IQueryable().ToList();
        }
        public List<BSO_PumpOptimal_RecordsEntity> GetNew(int stId)
        {
            var expression = ExtLinq.True<BSO_PumpOptimal_RecordsEntity>();
            expression = expression.And(t => t.StationId == stId);
            //expression = expression.And(t => t.LocalId == locId);
            return service.IQueryable(expression).OrderByDescending(i => i.ID).ToList();
        }

        /// <summary>
        /// 分页查询SCADA数据记录
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<BSO_PumpOptimal_RecordsEntity> GetList(Pagination pagination, int id)
        {
            var expression = ExtLinq.True<BSO_PumpOptimal_RecordsEntity>();
            if (!string.IsNullOrEmpty(id.ToString()))
            {
                expression = expression.And(t => t.ID.Equals(id));
            }
            return service.FindList(expression, pagination).ToList();

        }


        /// <summary>
        /// 根据id删除SCADA数据记录
        /// </summary>
        /// <param name="keyValue"></param>
        public void DeleteForm(int id)
        {
            service.Delete(t => t.ID == id);
        }

        /// <summary>
        /// 添加或更新用户
        /// </summary>
        /// <param name="moduleEntity">数据实体信息</param>
        /// <param name="userID">用户编号</param>
        public void SubmitForm(BSO_PumpOptimal_RecordsEntity moduleEntity, string id)
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
            var result = service.FindEntity(t => t.ID == tmp);
            return JsonConvert.SerializeObject(result);
        }
    }
}
