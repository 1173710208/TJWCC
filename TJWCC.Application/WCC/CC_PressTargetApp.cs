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

    public class CC_PressTargetApp
    {
        private ICC_PressTargetRepository service = new CC_PressTargetRepository();

        /// <summary>
        /// 获取日常调度方案中调度依据数据
        /// </summary>
        /// <param name="time"></param>
        /// <param name="dType"></param>
        /// <returns></returns>
        public List<CC_PressTargetEntity> GetList(TimeSpan? time, int? dType)
        {
            var expression = ExtLinq.True<CC_PressTargetEntity>();
            expression = expression.And(i => i.Selected == true);
            if (dType != null)
                expression = expression.And(i => i.Type == dType);
            if (time != null)
            {
                expression = expression.And(i => i.PlanTime == time);
            }
            return service.IQueryable(expression).ToList();

        }

        /// <summary>
        /// 获取所有调度依据数据
        /// </summary>
        /// <returns></returns>
        public List<CC_PressTargetEntity> GetAllPressTargetList()
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
        public void SubmitForm(CC_PressTargetEntity moduleEntity, string id)
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
