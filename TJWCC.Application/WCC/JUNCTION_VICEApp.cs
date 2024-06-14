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

    public class JUNCTION_VICEApp
    {
        private IJUNCTION_VICERepository service = new JUNCTION_VICERepository();

        public List<JUNCTION_VICEEntity> GetList(string[] id)
        {
            return service.IQueryable().Where(i => id.Contains(i.OBJECTID)).ToList();
        }

        /// <summary>
        /// 获取所有调度依据数据
        /// </summary>
        /// <returns></returns>
        public List<JUNCTION_VICEEntity> GetAllList()
        {
            var reslut = service.IQueryable().ToList();
            return reslut;
        }
    }
}
