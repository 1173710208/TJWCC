using TJWCC.Code;
using TJWCC.Domain.Entity.SystemSecurity;
using TJWCC.Domain.IRepository.SystemSecurity;
using TJWCC.Repository.SystemSecurity;
using System;
using System.Collections.Generic;

namespace TJWCC.Application.SystemSecurity
{
    public class LogApp
    {
        private ILogRepository service = new LogRepository();

        public List<LogEntity> GetList(Pagination pagination, string queryJson)
        {
            var expression = ExtLinq.True<LogEntity>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["keyword"].IsEmpty())
            {
                string keyword = queryParam["keyword"].ToString();
                expression = expression.And(t => t.ACCOUNT.Contains(keyword));
            }
            if (!queryParam["timeType"].IsEmpty())
            {
                string timeType = queryParam["timeType"].ToString();
                DateTime startTime = DateTime.Now.ToString("yyyy-MM-dd").ToDate();
                DateTime endTime = DateTime.Now.ToString("yyyy-MM-dd").ToDate().AddDays(1);
                switch (timeType)
                {
                    case "1":
                        break;
                    case "2":
                        startTime = DateTime.Now.AddDays(-7);
                        break;
                    case "3":
                        startTime = DateTime.Now.AddMonths(-1);
                        break;
                    case "4":
                        startTime = DateTime.Now.AddMonths(-3);
                        break;
                    default:
                        break;
                }
                expression = expression.And(t => t.CURRDATE >= startTime && t.CURRDATE <= endTime);
            }
            return service.FindList(expression, pagination);
        }
        public void RemoveLog(string keepTime)
        {
            DateTime operateTime = DateTime.Now;
            if (keepTime == "7")            //保留近一周
            {
                operateTime = DateTime.Now.AddDays(-7);
            }
            else if (keepTime == "1")       //保留近一个月
            {
                operateTime = DateTime.Now.AddMonths(-1);
            }
            else if (keepTime == "3")       //保留近三个月
            {
                operateTime = DateTime.Now.AddMonths(-3);
            }
            var expression = ExtLinq.True<LogEntity>();
            expression = expression.And(t => t.CURRDATE <= operateTime);
            service.Delete(expression);
        }
        public void WriteDbLog(bool result, string resultLog)
        {
            LogEntity logEntity = new LogEntity();
            logEntity.ID = Common.GuId();
            logEntity.CURRDATE = Convert.ToDateTime(DateTime.Now.ToDateTimeString());
            logEntity.ACCOUNT = OperatorProvider.Provider.GetCurrent().UserCode;
            logEntity.NICKNAME = OperatorProvider.Provider.GetCurrent().UserName;
            logEntity.IPADDRESS = Net.Ip;
            logEntity.IPADDRESSNAME = Net.GetLocation(logEntity.IPADDRESS);
            logEntity.RESULT = result;
            logEntity.DESCRIPTION = resultLog;
            logEntity.Create();
            service.Insert(logEntity);
        }
        public void WriteDbLog(LogEntity logEntity)
        {
            logEntity.ID = Common.GuId();
            logEntity.CURRDATE = Convert.ToDateTime(DateTime.Now.ToDateTimeString());
            logEntity.IPADDRESS = "";
            logEntity.IPADDRESSNAME = Net.GetLocation(logEntity.IPADDRESS);
            logEntity.Create();
            service.Insert(logEntity);
        }
    }
}
