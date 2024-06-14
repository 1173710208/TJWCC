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
    public class Alarm_RAW_RecordApp
    {
        private IAlarm_RAW_RecordRespository service = new Alarm_RAW_RecordRepository();
        public List<ALARM_RAW_RECORDEntity> GetList()
        {
            return service.IQueryable().ToList();
        }

        /// <summary>
        /// 分页查询水表报警信息，也可以根据IMEI号码进行条件筛选查询
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="imei"></param>
        /// <returns></returns>
        public List<ALARM_RAW_RECORDEntity> GetList(Pagination pagination, string imei)
        {
            var expression = ExtLinq.True<ALARM_RAW_RECORDEntity>();
            if (!string.IsNullOrEmpty(imei))
            {
                expression = expression.And(t => t.IMEI.Contains(imei));
            }
            return service.FindList(expression, pagination).OrderBy(t => t.IMEI).OrderByDescending(t => t.SAVEDATE).ToList();
             
        }


    }
}
