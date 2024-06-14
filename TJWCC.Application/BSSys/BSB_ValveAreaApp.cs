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

    public class BSB_ValveAreaApp
    {
        private IBSB_ValveAreaRepository service = new BSB_ValveAreaRepository();

        /// <summary>
        /// 获取SCADA
        /// </summary>
        /// <returns></returns>
        public List<BSB_ValveAreaEntity> GetList()
        {
            return service.IQueryable().ToList();
        }

        /// <summary>
        /// 分页查询SCADA数据记录
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<BSB_ValveAreaEntity> GetList(int? id)
        {
            var expression = ExtLinq.True<BSB_ValveAreaEntity>();
            if (id != null)
            {
                expression = expression.And(t => t.Pointid == id);
            }
            return service.IQueryable(expression).ToList();

        }
        public BSB_ValveAreaEntity GetById(int vid)
        {
            var expression = ExtLinq.True<BSB_ValveAreaEntity>();
                expression = expression.And(t => t.Valveid == vid);
            return service.IQueryable(expression).FirstOrDefault();
        }


        /// <summary>
        /// 根据id删除SCADA数据记录
        /// </summary>
        /// <param name="keyValue"></param>
        public void DeleteForm(int id)
        {
            service.Delete(t => t.Valveid == id);
        }

        /// <summary>
        /// 添加或更新用户
        /// </summary>
        /// <param name="moduleEntity">数据实体信息</param>
        /// <param name="userID">用户编号</param>
        public void SubmitForm(BSB_ValveAreaEntity moduleEntity, string id)
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
