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

    public class CC_WetherRecordApp
    {
        private ICC_WetherRecordRepository service = new CC_WetherRecordRepository();

        /// <summary>
        /// 获取气象数据
        /// </summary>
        ///<param name="Type"></param>
        /// <returns></returns>
        public List<CC_WetherRecordEntity> GetList(int type)
        {
            string strSql = "";
            var PreDate = service.IQueryable().Where(i => i.Type == 1).Max(i => i.PreDate).Value;
            var nowDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH") + ":00:00");
            if (PreDate > nowDate) PreDate = nowDate;
            string strDate = PreDate.ToString("yyyy-MM-dd HH:mm:ss");
            string strDate1 = PreDate.AddDays(2).ToString("yyyy-MM-dd HH:mm:ss");
            //string strDate = "2021-11-24 11:00:00";
            //string strDate1 = "2021-11-26 11:00:00";
            if (type==1)
            {
                strSql = "select * from CC_WetherRecord where type=1 and LEFT(convert(NVARCHAR(20), preDate, 20),13) = LEFT('" + strDate + "',13) order by type, PreDate";
                //strSql = "select * from CC_WetherRecord where type=1 and LEFT(convert(NVARCHAR(20), preDate, 20),13) = LEFT(convert(NVARCHAR(20), getdate(), 20),13) order by type, PreDate";
            }
            else
            {
                strSql = "select * from CC_WetherRecord where (type=1 and LEFT(convert(NVARCHAR(20), preDate, 20),13) = LEFT('" + strDate + "',13)) or (type=2 and LEFT(convert(NVARCHAR(20), PreDate, 20),10) >= LEFT('" + strDate + "',10) and LEFT(convert(NVARCHAR(20), PreDate, 20),10) <= LEFT('" + strDate1 + "',10) ) order by type, PreDate ";
                //strSql = "select * from CC_WetherRecord where (type=1 and LEFT(convert(NVARCHAR(20), preDate, 20),13) = LEFT(convert(NVARCHAR(20), getdate(), 20),13)) or (type=2 and LEFT(convert(NVARCHAR(20), SaveDate, 20),10) = LEFT(convert(NVARCHAR(20), getdate(), 20),10)) order by type, PreDate ";
            }
            return service.FindList(strSql).ToList();
        }

        /// <summary>
        /// 分页查询SCADA数据记录
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public List<CC_WetherRecordEntity> GetList(Pagination pagination, int type)
        {
            var expression = ExtLinq.True<CC_WetherRecordEntity>();
            if (!string.IsNullOrEmpty(type.ToString()))
            {
                expression = expression.And(t => t.Type.Equals(type));
            }
            return service.FindList(expression, pagination).ToList();

        }


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
        public void SubmitForm(CC_WetherRecordEntity moduleEntity, string id)
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
