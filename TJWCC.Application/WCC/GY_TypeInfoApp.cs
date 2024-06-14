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

    public class GY_TypeInfoApp
    {
        private IGY_TypeInfoRepository service = new GY_TypeInfoRepository();

        /// <summary>
        /// 获取气象数据
        /// </summary>
        ///<param name="Type"></param>
        /// <returns></returns>
        public List<GY_TypeInfoEntity> GetList(int type)
        {
            string strSql = "";
            if (type==1)
            {
                strSql = "select * from GY_TypeInfo where type=1 and LEFT(convert(NVARCHAR(20), preDate, 20),13) = LEFT(convert(NVARCHAR(20), getdate(), 20),13) order by type, PreDate";
            }else
            {
                strSql = "select * from GY_TypeInfo where (type=1 and LEFT(convert(NVARCHAR(20), preDate, 20),13) = LEFT(convert(NVARCHAR(20), getdate(), 20),13)) or (type=2 and LEFT(convert(NVARCHAR(20), SaveDate, 20),10) = LEFT(convert(NVARCHAR(20), getdate(), 20),10)) order by type, PreDate ";
            }
            return service.FindList(strSql).ToList();
        }

        /// <summary>
        /// 分页查询SCADA数据记录
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public List<GY_TypeInfoEntity> GetList(Pagination pagination, int typeId)
        {
            var expression = ExtLinq.True<GY_TypeInfoEntity>();
            if (!string.IsNullOrEmpty(typeId.ToString()))
            {
                expression = expression.And(t => t.TypeId.Equals(typeId));
            }
            return service.FindList(expression, pagination).ToList();

        }


        /// <summary>
        /// 根据id删除SCADA数据记录
        /// </summary>
        /// <param name="keyValue"></param>
        public void DeleteForm(int id)
        {
            service.Delete(t => t.TypeId == id);
        }

        /// <summary>
        /// 添加或更新用户
        /// </summary>
        /// <param name="moduleEntity">数据实体信息</param>
        /// <param name="userID">用户编号</param>
        public void SubmitForm(GY_TypeInfoEntity moduleEntity, string id)
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
            var result = service.FindEntity(t => t.TypeId == tmp);
            return JsonConvert.SerializeObject(result);
        }
    }
}
