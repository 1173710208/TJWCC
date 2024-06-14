using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.Public
{

    /// <summary>
    /// 大屏首页的原水泵站数据
    /// </summary>
    public class MainStation
    {
        public string StationName { get; set; }
        public decimal? Flow { get; set; }
        public decimal? Level { get; set; }
        public decimal? Press { get; set; }
        public string FlowId { get; set; }
        public string LevelId { get; set; }
        public string PressId { get; set; }

    }

    /// <summary>
    /// 大屏首页的水厂数据
    /// </summary>
    public class MainReservoir
    {
        public string StationName { get; set; }
        public decimal? Press { get; set; }
        public decimal? Flow { get; set; }
        public decimal? CL { get; set; }
        public decimal? NTU { get; set; }
        public decimal? SFlow { get; set; }
        public int? PumpCount { get; set; }
        public string FlowId { get; set; }
        public string PressId { get; set; }
        public string CLId { get; set; }
        public string NTUId { get; set; }
        public string SFlowId { get; set; }
    }

    /// <summary>
    /// 大屏首页的检测数据
    /// </summary>
    public class MainRealTime
    {
        public string Name { get; set; }
        public decimal? Value { get; set; }
        public string Station_Key { get; set; }
        public string Tag_key { get; set; }
    }

    /// <summary>
    /// 工艺二级菜单数据
    /// </summary>
    public class Main2ndItems
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// 工艺二级菜单数据
    /// </summary>
    public class PumpStationCurrent
    {
        public decimal? Tag_value { get; set; }
        public string Tag_key { get; set; }
        public int? Number { get; set; }
        public string ValueName { get; set; }
    }
}
