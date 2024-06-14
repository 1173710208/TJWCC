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

    public class CC_CleanStatisticsApp
    {
        private ICC_CleanStatisticsRepository service = new CC_CleanStatisticsRepository();

        public List<CC_CleanStatisticsEntity> GetList(int? type, int? cycle, DateTime? sDate, DateTime? eDate)
        {
            var expression = ExtLinq.True<CC_CleanStatisticsEntity>();
            if (type != null)
                expression = expression.And(i => i.Type == type);
            if (cycle != null)
                expression = expression.And(i => i.Cycle == cycle);
            if (sDate != null || eDate != null)
            {
                if (sDate == null && eDate != null)
                    expression = expression.And(t => t.CountTime <= eDate);
                else if (sDate != null && eDate == null)
                    expression = expression.And(t => t.CountTime >= sDate);
                else
                    expression = expression.And(t => t.CountTime >= sDate && t.CountTime <= eDate);
            }
            else
                if (type != 0)
                return service.IQueryable(expression).OrderByDescending(i => i.CreateTime).ThenByDescending(i => i.ID).Take(60).ToList();
            return service.IQueryable(expression).ToList();

        }

        /// <summary>
        /// 获取所有调度依据数据
        /// </summary>
        /// <returns></returns>
        public List<CC_CleanStatisticsEntity> GetAllPressTargetList()
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
        public void SubmitForm(CC_CleanStatisticsEntity moduleEntity, string id)
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
