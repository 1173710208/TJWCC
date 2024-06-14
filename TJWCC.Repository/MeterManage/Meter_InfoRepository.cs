using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Data;
using TJWCC.Domain.Entity.MeterManage;
using TJWCC.Domain.Entity.DataDisplay;
using TJWCC.Domain.IRepository.MeterManage; 

namespace TJWCC.Repository.MeterManage
{
    public class Meter_InfoRepository : RepositoryBase<METER_INFOEntity>, IMeter_InfoRespository
    {
    }
}
