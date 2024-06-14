using TJWCC.Code;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Repository.WCC;
using TJWCC.Domain.IRepository.WCC;
using TJWCC.Domain.Entity.WCC;

namespace TJWCC.Application.WCC
{

    public class CC_TheDateApp
    {
        private ICC_TheDateRepository service = new CC_TheDateRepository();

        /// <summary>
        /// 根据是否工作日获取日期数据
        /// </summary>
        /// <param name="Type">1：工作日2：非工作日</param>
        /// <returns></returns>
        public List<CC_TheDateEntity> GetList(int? Type)
        {
            var expression = ExtLinq.True<CC_TheDateEntity>();
            if (Type != null)
                expression = expression.And(i => i.Type == Type);
            return service.IQueryable(expression).ToList();

        }

        /// <summary>
        /// 根据日期获取是否工作日日期数据
        /// </summary>
        /// <param name="Type">1：工作日2：非工作日</param>
        /// <returns></returns>
        public CC_TheDateEntity GetTheDate(DateTime? time)
        {
            var expression = ExtLinq.True<CC_TheDateEntity>();
            if (time != null)
            {
                expression = expression.And(i => i.TheDate == time);
            }
            return service.IQueryable(expression).FirstOrDefault();

        }

        /// <summary>
        /// 获取所有调度依据数据
        /// </summary>
        /// <returns></returns>
        public List<CC_TheDateEntity> GetAllPressTargetList()
        {
            var reslut = service.IQueryable().ToList();
            return reslut;
        }

        /// <summary>
        /// 根据id删除调度依据数据记录
        /// </summary>
        /// <param name="keyValue"></param>
        public void DeleteForm(int id)
        {
            service.Delete(t => t.ID == id);
        }

        /// <summary>
        /// 添加或更新调度依据
        /// </summary>
        /// <param name="moduleEntity">数据实体信息</param>
        /// <param name="userID">用户编号</param>
        public void SubmitForm(CC_TheDateEntity moduleEntity, string id)
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
    }
}
