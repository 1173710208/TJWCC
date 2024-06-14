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

    public class BS_SCADA_TAG_INFOApp
    {
        private IBS_SCADA_TAG_INFORepository service = new BS_SCADA_TAG_INFORepository();

        /// <summary>
        /// 获取SCADA
        /// </summary>
        /// <returns></returns>
        public List<BS_SCADA_TAG_INFOEntity> GetList()
        {
            return service.IQueryable().ToList();
        }
        public BS_SCADA_TAG_INFOEntity GetEntity(string tagKey)
        {
            return service.FindEntity(i=>i.Tag_key.Equals(tagKey));
        }

        /// <summary>
        /// 分页查询SCADA数据记录
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="Station_key"></param>
        /// <returns></returns>
        //public List<BS_SCADA_TAG_INFOEntity> GetList(Pagination pagination, int Station_key)
        //{
        //    var expression = ExtLinq.True<BS_SCADA_TAG_INFOEntity>();
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
        public void SubmitForm(BS_SCADA_TAG_INFOEntity moduleEntity, string id)
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
