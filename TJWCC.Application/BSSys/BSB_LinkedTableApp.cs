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

    public class BSB_LinkedTableApp
    {
        private IBSB_LinkedTableRepository service = new BSB_LinkedTableRepository();

        /// <summary>
        /// 获取SCADA
        /// </summary>
        /// <returns></returns>
        public List<BSB_LinkedTableEntity> GetList()
        {
            return service.IQueryable().ToList();
        }
        public List<BSB_LinkedTableEntity> GetList(int[] id)
        {
            return service.IQueryable().Where(i => id.Contains((int)i.ElementId)).ToList();
        }

        ///// <summary>
        ///// 分页查询SCADA数据记录
        ///// </summary>
        ///// <param name="pagination"></param>
        ///// <param name="Station_key"></param>
        ///// <returns></returns>
        //public List<BSB_LinkedTableEntity> GetList(Pagination pagination, int Station_key)
        //{
        //    var expression = ExtLinq.True<BSB_LinkedTableEntity>();
        //    if (!string.IsNullOrEmpty(Station_key.ToString()))
        //    {
        //        expression = expression.And(t => t.Station_key.Equals(Station_key));
        //    }
        //    return service.FindList(expression, pagination).ToList();

        //}


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
        public void SubmitForm(BSB_LinkedTableEntity moduleEntity, string id)
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
