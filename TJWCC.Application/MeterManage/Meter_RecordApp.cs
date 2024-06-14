using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Code;
using TJWCC.Domain.Entity.MeterManage;
using TJWCC.Domain.IRepository.MeterManage;
using TJWCC.Repository.MeterManage;

namespace TJWCC.Application.MeterManage
{
    public class Meter_RecordApp
    {
        private IMeter_RecordRespository service = new Meter_RecordRepository();
        public List<METER_RECORDEntity> GetList()
        {
            return service.IQueryable().OrderBy(t => t.IMEI).OrderByDescending(t => t.SAVEDATE).ToList();
        }

        /// <summary>
        /// 分页查询水表周期水量信息，也可以根据IMEI号码进行条件筛选查询
        /// </summary>
        /// <param name="pagination">分页信息</param>
        /// <param name="imei">IMEI号码</param>
        /// <returns></returns>
        public List<METER_RECORDEntity> GetList(Pagination pagination, string imei)
        {
            var expression = ExtLinq.True<METER_RECORDEntity>();
            if (!string.IsNullOrEmpty(imei))
            {
                expression = expression.And(t => t.IMEI.Contains(imei)); 
            }
            return service.FindList(expression, pagination).OrderBy(t => t.IMEI).OrderByDescending(t => t.SAVEDATE).ToList();
        }
    }
}
