using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BSSys
{
    public class BSO_Station_RecordsEntity : IEntity<BSO_Station_RecordsEntity>
    {
        public long ID { get; set; }
        public long StationId { get; set; }
        public Nullable<int> PressureAlarm { get; set; }
        public Nullable<int> EfficAlarm { get; set; }
        public Nullable<decimal> RealPower { get; set; }
        public Nullable<decimal> OptPower { get; set; }
        public Nullable<int> OpenClose1 { get; set; }
        public Nullable<int> OpenClose2 { get; set; }
        public Nullable<int> OpenClose3 { get; set; }
        public Nullable<int> OpenClose4 { get; set; }
        public Nullable<int> OpenClose5 { get; set; }
        public Nullable<int> OpenClose6 { get; set; }
        public Nullable<int> OpenClose7 { get; set; }
        public Nullable<int> OpenClose8 { get; set; }
        public Nullable<int> OpenClose9 { get; set; }
        public Nullable<int> OpenClose10 { get; set; }
        public Nullable<int> OpenClose11 { get; set; }
        public Nullable<int> OpenClose12 { get; set; }
        public Nullable<int> OpenClose13 { get; set; }
        public Nullable<int> OpenClose14 { get; set; }
        public Nullable<int> OpenClose15 { get; set; }
        public Nullable<int> OpenClose16 { get; set; }
        public Nullable<decimal> Pressure1 { get; set; }
        public Nullable<decimal> Pressure2 { get; set; }
        public Nullable<decimal> Pressure3 { get; set; }
        public Nullable<decimal> Pressure4 { get; set; }
        public Nullable<decimal> Pressure5 { get; set; }
        public Nullable<decimal> Pressure6 { get; set; }
        public Nullable<decimal> Pressure7 { get; set; }
        public Nullable<decimal> Pressure8 { get; set; }
        public Nullable<decimal> Pressure9 { get; set; }
        public Nullable<decimal> Pressure10 { get; set; }
        public Nullable<decimal> Pressure11 { get; set; }
        public Nullable<decimal> Pressure12 { get; set; }
        public Nullable<decimal> Pressure13 { get; set; }
        public Nullable<decimal> Pressure14 { get; set; }
        public Nullable<decimal> Pressure15 { get; set; }
        public Nullable<decimal> Pressure16 { get; set; }
        public Nullable<decimal> Flow1 { get; set; }
        public Nullable<decimal> Flow2 { get; set; }
        public Nullable<decimal> Flow3 { get; set; }
        public Nullable<decimal> Flow4 { get; set; }
        public Nullable<decimal> Flow5 { get; set; }
        public Nullable<decimal> Flow6 { get; set; }
        public Nullable<decimal> Flow7 { get; set; }
        public Nullable<decimal> Flow8 { get; set; }
        public Nullable<decimal> Flow9 { get; set; }
        public Nullable<decimal> Flow10 { get; set; }
        public Nullable<decimal> Flow11 { get; set; }
        public Nullable<decimal> Flow12 { get; set; }
        public Nullable<decimal> Flow13 { get; set; }
        public Nullable<decimal> Flow14 { get; set; }
        public Nullable<decimal> Flow15 { get; set; }
        public Nullable<decimal> Flow16 { get; set; }
        public Nullable<decimal> TankLevel1 { get; set; }
        public Nullable<decimal> TankLevel2 { get; set; }
        public Nullable<decimal> TankLevel3 { get; set; }
        public Nullable<decimal> TankLevel4 { get; set; }
        public Nullable<decimal> TankLevel5 { get; set; }
        public Nullable<decimal> TankLevel6 { get; set; }
        public Nullable<decimal> TankLevel7 { get; set; }
        public Nullable<decimal> TankLevel8 { get; set; }
        public Nullable<decimal> TankLevel9 { get; set; }
        public Nullable<decimal> TankLevel10 { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string CreaterId { get; set; }
        public string Remark { get; set; }
    }
}
