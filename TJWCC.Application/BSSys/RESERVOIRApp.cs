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

    public class RESERVOIRApp
    {
        private IRESERVOIRRepository service = new RESERVOIRRepository();

        /// <summary>
        /// 获取SCADA
        /// </summary>
        /// <returns></returns>
        public List<RESERVOIREntity> GetList()
        {
            return service.IQueryable().Where(item => item.Is_Active == 1).ToList();
        }
        public List<RESERVOIREntity> GetList(int NodeID)
        {
            return service.IQueryable().Where(i => i.ElementId == NodeID && i.Is_Active == 1).ToList();
        }
        public List<RESERVOIREntity> GetList(int[] id)
        {
            return service.IQueryable().Where(i => id.Contains((int)i.ElementId)).ToList();
        }

        /// <summary>
        /// 分页查询SCADA数据记录
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<RESERVOIREntity> GetList(Pagination pagination, int id)
        {
            var expression = ExtLinq.True<RESERVOIREntity>();
            if (!string.IsNullOrEmpty(id.ToString()))
            {
                expression = expression.And(t => t.OBJECTID.Equals(id));
            }
            return service.FindList(expression, pagination).ToList();

        }


        /// <summary>
        /// 根据id删除SCADA数据记录
        /// </summary>
        /// <param name="keyValue"></param>
        public void DeleteForm(int id)
        {
            service.Delete(t => t.OBJECTID == id);
        }

        /// <summary>
        /// 添加或更新用户
        /// </summary>
        /// <param name="moduleEntity">数据实体信息</param>
        /// <param name="userID">用户编号</param>
        public void SubmitForm(RESERVOIREntity moduleEntity, string id)
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
            var result = service.FindEntity(t => t.OBJECTID == tmp);
            return JsonConvert.SerializeObject(result);
        }
    }
}
