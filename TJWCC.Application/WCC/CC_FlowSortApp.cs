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

    public class CC_FlowSortApp
    {
        private ICC_FlowSortRepository service = new CC_FlowSortRepository();

        /// <summary>
        /// 根据条件获取水量数据
        /// </summary>
        /// <param name="Type">数据分类1：小时；2：天；3：周；4：月；5：季；6：年；7：GDP；8：人口</param>
        /// <returns></returns>
        public List<CC_FlowSortEntity> GetList(DateTime? SaveDate)
        {
            var expression = ExtLinq.True<CC_FlowSortEntity>();
            if (SaveDate != null)
                expression = expression.And(i => i.Save_date == SaveDate);
            return service.IQueryable(expression).ToList();
        }

        /// <summary>
        /// 获取人口数据量和GDP
        /// </summary>
        /// <param name="Type">1：工作日2：非工作日</param>
        /// <returns></returns>
        public List<CC_FlowSortEntity> GetDPYearList()
        {
            string sql = "SELECT * FROM CC_FlowSort WHERE Save_date>='" + DateTime.Now.AddYears(-1).Year + "-01-01 00:00:00' AND (Type=7 OR Type=8)";
            return service.FindList(sql);
        }
        public DateTime GetMaxDate()
        {
            return service.IQueryable().Where(i=>i.Type==1).Max(i=>i.Save_date).Value;
        }

        /// <summary>
        /// 根据id删除水量数据记录
        /// </summary>
        /// <returns></returns>
        public List<CC_FlowSortEntity> GetAllPressTargetList()
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
        public void SubmitForm(CC_FlowSortEntity moduleEntity, string id)
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
