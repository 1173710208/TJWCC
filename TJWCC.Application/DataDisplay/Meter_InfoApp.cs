using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Domain.Entity.DataDisplay;
using TJWCC.Domain.IRepository.DataDisplay;
using TJWCC.Repository.DataDisplay;


namespace TJWCC.Application.DataDisplay
{

    public class Meter_InfoApp
    {
        private IMeter_InfoRepository service = new Meter_InfoRepository();
        //private IUser_InfoRepository uservice = new User_InfoRepository();
        private IDis_WaterCompanyClassRepository wservice = new Dis_WaterCompanyClassRepository();
        private IDis_AlarmRepository aservice = new Dis_AlarmRepository();

        public string GetSingleMeter(string Mid)
        {
            var results = new { result = new METER_INFOEntity(), install = 0, online = 0, inp = "0%", alarm = 0, bid = 0 };

            long tmp = 0;
            try
            {
                tmp = Convert.ToInt64(Mid);
            }
            catch { }
            var mresult = service.FindEntity(t => t.IMEI == Mid || t.NBMETERID == tmp);
            if (mresult != null)
            {
                int? bid = mresult.BUSINESSID;
                if (bid != null)
                {
                    var wresult = wservice.FindEntity(t => t.BUSINESSID == bid);
                    var aresult = aservice.FindEntity(t => t.BUSINESSID == bid);
                    results = new { result = mresult, install = wresult.INSTALLAMOUNT, online = wresult.ONLINEAMOUNT, inp = (Convert.ToDouble(wresult.ONLINEAMOUNT) / Convert.ToDouble(wresult.INSTALLAMOUNT) * 100).ToString("N2") + "%", alarm = aresult.ALARMS, bid = wresult.BUSINESSID };
                }
                else
                {
                    results = new { result = mresult, install = 0, online = 0, inp = "0%", alarm = 0, bid = 0 };
                }
            }
            return JsonConvert.SerializeObject(results);
        }
        public string GetMeterList()
        {
            var query = service.IQueryable();
            var mlist = (from q in query
                              select new
                              {
                                  imei = q.IMEI
                              }).ToList();
           return JsonConvert.SerializeObject(mlist);
        }
        public METER_INFOEntity GetForm(decimal keyValue)
        {
            return service.FindEntity(keyValue);
        }
        public void SubmitForm(METER_INFOEntity meter_infoEntity)
        {
            service.Update(meter_infoEntity);
        }
    }
}
