using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Code;
using TJWCC.Domain.Entity.DataDisplay;
using TJWCC.Domain.IRepository.MeterManage;
using TJWCC.Repository.MeterManage;

namespace TJWCC.Application.MeterManage
{
    public class Meter_InfoApp
    {
        private IMeter_InfoRespository service = new Meter_InfoRepository();
        public List<METER_INFOEntity> GetList()
        {
            return service.IQueryable().ToList();
        }

        /// <summary>
        /// 分页查询水表档案信息，也可以根据IMEI号码进行条件筛选查询
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="imei"></param>
        /// <returns></returns>
        public List<METER_INFOEntity> GetList(Pagination pagination, string imei)
        {
            var expression = ExtLinq.True<METER_INFOEntity>();
            if (!string.IsNullOrEmpty(imei))
            {
                expression = expression.And(t => t.IMEI.Contains(imei));
            }
            //return service.FindList(expression, pagination).OrderBy(t => t.IMEI).OrderByDescending(t => t.CREATEDATE).ToList();
            return service.FindList(expression, pagination);
        }

        /// <summary>
        /// 添加水表信息
        /// </summary>
        /// <param name="moduleEntity">水表实体信息</param>
        /// <param name="MakerID">水表信息ID</param>
        public void SubmitForm(METER_INFOEntity moduleEntity, string MeterID)
        {
            if (!string.IsNullOrEmpty(MeterID))
            {
                service.Update(moduleEntity);
            }
            else
            {
                service.Insert(moduleEntity);
            }
        }

        public List<METER_INFOEntity> GetList(string keyword)
        {
            var expression = ExtLinq.True<METER_INFOEntity>();
            expression = expression.And(t => t.IMEI == keyword);
            return service.IQueryable(expression).ToList();
        }

        /// <summary>
        /// 根据IMEI删除供应商信息
        /// </summary>
        /// <param name="keyValue"></param>
        public void DeleteForm(string imei)
        {
            service.Delete(t => t.IMEI == imei);
        }
    }
}
