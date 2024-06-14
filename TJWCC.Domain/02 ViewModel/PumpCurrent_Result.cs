using System;
namespace TJWCC.Domain.ViewModel
{
    
    public partial class PumpCurrent_Result
    {
        public long PumpId { get; set; }
        public string PumpName { get; set; }
        public DateTime? DataTime { get; set; }
        public decimal? PumpLift { get; set; }
        public decimal? Power { get; set; }
        public decimal? Press { get; set; }
        public decimal? Vacuum { get; set; }
        public decimal? Value1 { get; set; }
        public decimal? Value2 { get; set; }
        public decimal? Frequency { get; set; }
        public decimal? RotateSpeed { get; set; }
        public decimal? Flow { get; set; }
        public decimal? Efficiency { get; set; }
        public bool? IsOpen { get; set; }
        public decimal? SpeedRatio { get; set; }
        public int? LocalID { get; set; }
        public bool? IsChange { get; set; }
        public int? IsActive { get; set; }
    }
}
