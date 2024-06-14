using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.WCC
{
    public class GY_PumpStationInfoEntity : IEntity<GY_PumpStationInfoEntity>
    {
        public int ID { get; set; }
        public int StationId { get; set; }
        public string StationName { get; set; }
        public int? StationTypeId { get; set; }
        public int? SStationTypeId { get; set; }
        public int? PumpTotal0 { get; set; }
        public decimal? DesignFlow0 { get; set; }
        public decimal? PumpLift0 { get; set; }
        public int? Power0 { get; set; }
        public int? RotateSpeed0 { get; set; }
        public string Remark0 { get; set; }
        public string Remarka0 { get; set; }
        public int? PumpTotal1 { get; set; }
        public decimal? DesignFlow1 { get; set; }
        public decimal? PumpLift1 { get; set; }
        public int? Power1 { get; set; }
        public int? RotateSpeed1 { get; set; }
        public string Remark1 { get; set; }
        public string Remarka1 { get; set; }
        public int? PumpTotal2 { get; set; }
        public decimal? DesignFlow2 { get; set; }
        public decimal? PumpLift2 { get; set; }
        public int? Power2 { get; set; }
        public int? RotateSpeed2 { get; set; }
        public string Remark2 { get; set; }
        public string Remarka2 { get; set; }
        public string PName0 { get; set; }
        public string PName1 { get; set; }
        public string PName2 { get; set; }
        public string PName3 { get; set; }
        public string PName4 { get; set; }
        public string PName5 { get; set; }
        public string PName6 { get; set; }
        public string PName7 { get; set; }
        public string PName8 { get; set; }
        public string PName9 { get; set; }
        public string PName10 { get; set; }
        public string PName11 { get; set; }
        public string PName12 { get; set; }
        public string PName13 { get; set; }
        public string PName14 { get; set; }
        public decimal? Angle0 { get; set; }
        public decimal? Angle1 { get; set; }
        public decimal? Angle2 { get; set; }
        public decimal? Angle3 { get; set; }
        public decimal? Angle4 { get; set; }
        public decimal? Angle5 { get; set; }
        public decimal? Angle6 { get; set; }
        public decimal? Angle7 { get; set; }
        public decimal? Angle8 { get; set; }
        public decimal? Angle9 { get; set; }
        public decimal? Angle10 { get; set; }
        public decimal? Angle11 { get; set; }
        public decimal? Angle12 { get; set; }
        public decimal? Angle13 { get; set; }
        public decimal? Angle14 { get; set; }
        public decimal? LevelMax { get; set; }
        public decimal? LevelMin { get; set; }
        public string Remark { get; set; }
    }
}
