using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace TJWCC.Code
{
    /// <summary>
    /// 决策支持中的相关参数
    /// </summary>
    public static class JueCheZhiCiCanShu
    {

       
        public static string wg351pressure = "wg351pressure";
        public static string wg351velotity = "wg351velotity";



        public static string wg352pressure = "wg352pressure";
        public static string wg352velotity = "wg352velotity";

        public static string chiaarea1 = "chiaarea1";
        public static string chiaarea2 = "chiaarea2";
        public static string chiaarea3= "chiaarea3";
        public static string chiaarea4 = "chiaarea4";
        public static string chiaarea5 = "chiaarea5";
        public static string chiaarea6 = "chiaarea6";
        public static string chiaarea7 = "chiaarea7"; 



        public static string CanShu3D = "CanShu3D";
 
        public static string MPressureAvg = "MPressureAvg";
        public static string MVelocityMax = "MVelocityMax";
        public static string MVelocityAvg = "MVelocityAvg";
        public static string MWaterTimeAvg = "MWaterTimeAvg";
       




        #region 决策支持参数
        //技术性

        //技术性综合分析
        public static string TechScore = "TechScore";

        /// <summary>
        /// 压力权重
        /// </summary>
        public static string TecpressureQuanZhong = "TecpressureQuanZhong";
       /// <summary>
       /// 流速权重
       /// </summary>
        public static string TechNicalPipeVelocityQuanZhong = "TechNicalPipeVelocityQuanZhong";
        /// <summary>
        /// 水龄权重
        /// </summary>
        public static string TechNicalPipeWaterTimeQuanZhong = "TechNicalPipeWaterTimeQuanZhong";



        //节点压力综合分析

       /// <summary>
       /// 压力的总得分
       /// </summary>
        public static string TecpressureScore = "TecpressureScore";


        /// <summary>
        /// 平均值的说明
        /// </summary>
        public static string TecpressureavgText = "TecpressureavgText";

        /// <summary>
        /// 平均值的权重
        /// </summary>
        public static string TecpressureavgQuanZhong = "TecpressureavgQuanZhong";
        /// <summary>
        /// 合格率的权重
        /// </summary>
        public static string TecpressureQualifiedRateQuanZhong = "TecpressureQualifiedRateQuanZhong";

        /// <summary>
        /// 离优差的权重
        /// </summary>
        public static string TecpressureFromdiffQuanZhong = "TecpressureFromdiffQuanZhong";

  
        //平均值

        /// <summary>
        /// 平均值等分
        /// </summary>
        public static string TecpressureavgScore = "TecpressureavgScore";
         
        /// <summary>
        /// 技术性-平均值-Ha
        /// </summary>
        public static string strHa = "PRESSURE_MIN_VALUE";

        /// <summary>
        ///技术性-平均值-Hmax
        /// </summary>
        public static string strHmax = "PRESSURE_MAX_VALUE";

        /// <summary>
        /// 技术性-标定曲线-平均值
        /// </summary>
        public static string FTechPressureAvg = "FTechPressureAvg";


        //合格率


        /// <summary>
        /// 合格率得分
        /// </summary>
        public static string TecpressureQualifiedRateScore = "TecpressureQualifiedRateScore";

        /// <summary>
        /// 技术性-标定曲线-合格率低压区
        /// </summary>
        public static string FTechPressureLow = "FTechPressureLow";

        /// <summary>
        /// 技术性-标定曲线-合格率中压区
        /// </summary>
        public static string FTechPressureMid = "FTechPressureMid";

        /// <summary>
        /// 技术性-标定曲线-合格率高压区
        /// </summary>
        public static string FTechPressureHigh = "FTechPressureHigh";

        /// <summary>
        /// 技术性-标定曲线-合格率低压权重
        /// </summary>
        public static string FTechPressureLowQuan = "FTechPressureLowQuan";

        /// <summary>
        /// 技术性-标定曲线-合格率中压权重
        /// </summary>
        public static string FTechPressureMidQuan = "FTechPressureMidQuan";

        /// <summary>
        /// 技术性-标定曲线-合格率高压权重
        /// </summary>
        public static string FTechPressureHighQuan = "FTechPressureHighQuan";

       




        //离优差

        /// <summary>
        /// 离优差得分
        /// </summary>
        public static string TecpressureFromdiffScore = "TecpressureFromdiffScore";
         


        /// <summary>
        /// 离优差H优值
        /// </summary>
        public static string FTechPressureSYou = "FTechPressureSYou";

        
        /// <summary>
        /// 离优差Smax值
        /// </summary>
        public static string FTechPressureSValue = "FTechPressureSValue";


        /// <summary>
        /// 技术性-标定曲线-离优差
        /// </summary>
        public static string FTechPressureFromQX = "FTechPressureFromQX";



        //管段流速


        /// <summary>
        /// 管段流速得分
        /// </summary>
        public static string TechNicalPipeVelocityScore = "TechNicalPipeVelocityScore";       

        /// <summary>
        /// 管段流速权重
        /// </summary>
        public static string TechNicalPipeVelocityQZ = "TechNicalPipeVelocityQZ";   
        
        /// <summary>
        /// 技术性-标定曲线-管段流速低压区
        /// </summary>
        public static string FTechNicalPipeVelocityLow = "FTechNicalPipeVelocityLow";

        /// <summary>
        /// 技术性-标定曲线-管段流速中压区
        /// </summary>
        public static string FTechNicalPipeVelocityMid = "FTechNicalPipeVelocityMid";

        /// <summary>
        /// 技术性-标定曲线-管段流速高压区
        /// </summary>
        public static string FTechNicalPipeVelocityHigh = "FTechNicalPipeVelocityHigh";

        /// <summary>
        /// 技术性-标定曲线-管段流速低压权重
        /// </summary>
        public static string FTechNicalPipeVelocityLowQuan = "FTechNicalPipeVelocityLowQuan";

        /// <summary>
        /// 技术性-标定曲线-管段流速中压权重
        /// </summary>
        public static string FTechNicalPipeVelocityMidQuan = "FTechNicalPipeVelocityMidQuan";

        /// <summary>
        /// 技术性-标定曲线-管段流速高压权重
        /// </summary>
        public static string FTechNicalPipeVelocityHighQuan = "FTechNicalPipeVelocityHighQuan";


        /// <summary>
        /// 水质性能
        /// </summary>

        //管网水质合格率
        public static string TechNicalWaterQualityRateScore = "TechNicalWaterQualityRateScore";
        public static string TechNicalWaterQualityRate = "TechNicalWaterQualityRate";
        public static string TechNicalWaterQualityRateQZ = "TechNicalWaterQualityRateQZ";


        //安全加氯量
        public static string TechNicalWaterQualityClScore = "TechNicalWaterQualityClScore";
        public static string TechNicalWaterQualityCl = "TechNicalWaterQualityCl";
        public static string TechNicalWaterQualityClQZ = "TechNicalWaterQualityClQZ";



        //水龄分析

        /// <summary>
        /// 水龄分析得分
        /// </summary>
        public static string TechNicalPipeWaterTimeScore = "TechNicalPipeWaterTimeScore";
        public static string TechNicalPipeWaterTimeQZ = "TechNicalPipeWaterTimeQZ";

        /// <summary>
        /// 水质性能综合得分
        /// </summary>
        public static string TechNicalWaterQualitySumScore = "TechNicalWaterQualitySumScore";
        public static string TechNicalWaterQualitySumQZ = "TechNicalWaterQualitySumQZ";

        /// <summary>
        /// 最佳水龄时间
        /// </summary>
        public static string TechNicalPipeBestWaterTime = "TechNicalPipeBestWaterTime";


        /// <summary>
        /// 供水效率
        /// </summary>

        //管网漏损率
        public static string TechNicalSupplyEfficiencyLeakageScore = "TechNicalSupplyEfficiencyLeakageScore";
        public static string TechNicalSupplyEfficiencyLeakage = "TechNicalSupplyEfficiencyLeakage";
        public static string TechNicalSupplyEfficiencyLeakageQZ = "TechNicalSupplyEfficiencyLeakageQZ";


        //供水覆盖率
        public static string TechNicalSupplyEfficiencyCoverageScore = "TechNicalSupplyEfficiencyCoverageScore";
        public static string TechNicalSupplyEfficiencyCoverage = "TechNicalSupplyEfficiencyCoverage";
        public static string TechNicalSupplyEfficiencyCoverageQZ = "TechNicalSupplyEfficiencyCoverageQZ";

        /// <summary>
        /// 供水效率综合得分
        /// </summary>
        public static string TechNicalSupplyEfficiencySumScore = "TechNicalSupplyEfficiencySumScore";
        public static string TechNicalSupplyEfficiencySumQZ = "TechNicalSupplyEfficiencySumQZ";




        /// <summary>
        /// 允许的最大节点水龄
        /// </summary>
        public static string TechNicalPipeBestWaterTimemax = "TechNicalPipeBestWaterTimemax";



        /// <summary>
        /// 可允许的节点水龄
        /// </summary>
        public static string TechNicalPipeBestWaterTimea = "TechNicalPipeBestWaterTimea";




        /// <summary>
        /// 技术性-标定曲线-水龄低压区
        /// </summary>
        public static string FTechNicalPipeWaterTimeLow = "FTechNicalPipeWaterTimeLow";

        /// <summary>
        /// 技术性-标定曲线-水龄中压区
        /// </summary>
        public static string FTechNicalPipeWaterTimeMid = "FTechNicalPipeWaterTimeMid";

        /// <summary>
        /// 技术性-标定曲线-水龄高压区
        /// </summary>
        public static string FTechNicalPipeWaterTimeHigh = "FTechNicalPipeWaterTimeHigh";

        /// <summary>
        /// 技术性-标定曲线-水龄低压权重
        /// </summary>
        public static string FTechNicalPipeWaterTimeLowQuan = "FTechNicalPipeWaterTimeLowQuan";

        /// <summary>
        /// 技术性-标定曲线-水龄中压权重
        /// </summary>
        public static string FTechNicalPipeWaterTimeMidQuan = "FTechNicalPipeWaterTimeMidQuan";

        /// <summary>
        /// 技术性-标定曲线-水龄高压权重
        /// </summary>
        public static string FTechNicalPipeWaterTimeHighQuan = "FTechNicalPipeWaterTimeHighQuan";

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 经济性总分
        /// </summary>
        public static string EconomicScore="EconomicScore"; 

        /// <summary>
        /// 经济性-固定投资-总分
        /// </summary>
        public static string EconomicFixedInvestmentScore = "EconomicFixedInvestmentScore";

        /// <summary>
        /// 经济性-固定投资权重
        /// </summary>
        public static string EconomicFixedInvestmentQuanZhong = "EconomicFixedInvestmentQuanZhong";
        /// <summary>
        /// 经济性-能耗分析权重
        /// </summary>
        public static string EconomicEnergyConsumptionQuanZhong = "EconomicEnergyConsumptionQuanZhong";
   


        #region 经济性-固定投资-
        /// <summary>
        /// 经济性-固定投资-水厂投资
        /// </summary>
        public static string EconomicFixedInvestmentFactory = "EconomicFixedInvestmentFactory";
         
        /// <summary>
        /// 经济性-固定投资-泵站投资
        /// </summary>
        public static string EconomicFixedInvestmentPump = "EconomicFixedInvestmentPump";

        /// <summary>
        /// 经济性-固定投资-管网投资
        /// </summary>
        public static string EconomicFixedInvestmentPipe = "EconomicFixedInvestmentPipe";

        /// <summary>
        /// 经济性-固定投资-其他投资
        /// </summary>
        public static string EconomicFixedInvestmentOther = "EconomicFixedInvestmentOther";


        /// <summary>
        /// 经济性-固定投资-总投资
        /// </summary>
        public static string EconomicFixedInvestmentSum = "EconomicFixedInvestmentSum";

        /// <summary>
        /// 经济性-固定投资-C优
        /// </summary>
        public static string EconomicFixedInvestmentCYou = "EconomicFixedInvestmentCYou";

        /// <summary>
        /// 经济性-固定投资-标定曲线函数
        /// </summary>
        public static string EconomicFixedInvestmentBiaoDing = "EconomicFixedInvestmentBiaoDing";
        #endregion


        /// <summary>
        /// 经济性-能耗分析-总分
        /// </summary>
        public static string EconomicEnergyConsumptionScore = "EconomicEnergyConsumptionScore";
        

        #region 经济性-能耗分析-
        /// <summary>
        /// 经济性-能耗分析-标定曲线函数
        /// </summary>
        public static string EconomicEnergyConsumptionBiaoDing = "EconomicEnergyConsumptionBiaoDing";



        /// <summary>
        /// 经济性-能耗分析-E
        /// </summary>
        public static string EconomicEnergyConsumptionE = "EconomicEnergyConsumptionE";

        /// <summary>
        /// 经济性-能耗分析-E优
        /// </summary>
        public static string EconomicEnergyConsumptionEYou = "EconomicEnergyConsumptionEYou";

        //每天的供水量（km3）
        public static string EconomicEnergyConsumptionEwater = "EconomicEnergyConsumptionEwater";
        //每天的电耗（kWh）
        public static string EconomicEnergyConsumptionEpower = "EconomicEnergyConsumptionEpower";

        #endregion



        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 安全性总分
        /// </summary>
        public static string SafetyScore = "SafetyScore";



        //安全性-供水安全
        public static string SafeSupplyScore = "SafeSupplyScore";
        public static string SafeSupplyQZ = "SafeSupplyQZ";
        //安全性-管网保障
        public static string SafeSecurityScore = "SafeSecurityScore";
        public static string SafeSecurityQZ = "SafeSecurityQZ";
        //安全性-综合管理
        public static string SafeAdminScore = "SafeAdminScore";
        public static string SafeAdminQZ = "SafeAdminQZ";
        //安全性-安全性综合评价
        public static string SafetySumScore = "SafetySumScore";
        public static string SafetySumQZ = "SafetySumQZ";
        //安全性-供水安全-枯水年水量保证率
        public static string SafeSupplyLowWaterRateScore = "SafeSupplyLowWaterRateScore";
        public static string SafeSupplyLowWaterRateQZ = "SafeSupplyLowWaterRateQZ";
        public static string SafeSupplyLowWaterRate = "SafeSupplyLowWaterRate";
        //安全性-供水安全-水源水质类别
        public static string SafeSupplySourceQualityScore = "SafeSupplySourceQualityScore";
        public static string SafeSupplySourceQualityQZ = "SafeSupplySourceQualityQZ";
        public static string SafeSupplySourceQuality = "SafeSupplySourceQuality";
        //安全性-供水安全-取水能力
        public static string SafeSupplyGetWaterScore = "SafeSupplyGetWaterScore";
        public static string SafeSupplyGetWaterQZ = "SafeSupplyGetWaterQZ";
        public static string SafeSupplyGetWater = "SafeSupplyGetWater";
        //安全性-供水安全-备用水源
        public static string SafeSupplySourceStandbyScore = "SafeSupplySourceStandbyScore";
        public static string SafeSupplySourceStandbyQZ = "SafeSupplySourceStandbyQZ";
        public static string SafeSupplySourceStandby = "SafeSupplySourceStandby";
        //安全性-供水安全-调蓄水量比率
        public static string SafeSupplyAdjustRateScore = "SafeSupplyAdjustRateScore";
        public static string SafeSupplyAdjustRateQZ = "SafeSupplyAdjustRateQZ";
        public static string SafeSupplyAdjustRate = "SafeSupplyAdjustRate";
        //安全性-供水安全-供水安全综合评价
        public static string SafeSupplySumScore = "SafeSupplySumScore";
        public static string SafeSupplySumQZ = "SafeSupplySumQZ";
        //安全性-管网保障-爆管影响率
        public static string SafeSecurityBreakInfluenceScore = "SafeSecurityBreakInfluenceScore";
        public static string SafeSecurityBreakInfluenceQZ = "SafeSecurityBreakInfluenceQZ";
        public static string SafeSecurityBreakInfluence = "SafeSecurityBreakInfluence";
        //add 20200820
        //安全性-管网保障-输配水管网爆管概率
        public static string SafeSecurityBreakProbabilityScore = "SafeSecurityBreakProbabilityScore";
        public static string SafeSecurityBreakProbabilityQZ = "SafeSecurityBreakProbabilityQZ";
        public static string SafeSecurityBreakProbability = "SafeSecurityBreakProbability";
        //add 20200827
        /// <summary>
        /// 影响管段爆管的三个影响因素管材、管龄、管段压力权重系数为0.3、0.3、0.4
        /// </summary>
        public static string SafeSecurityBreakFactorMaterial = "SafeSecurityBreakFactorMaterial";
        public static string SafeSecurityBreakFactorPipeAge = "SafeSecurityBreakFactorPipeAge";
        public static string SafeSecurityBreakFactorPress = "SafeSecurityBreakFactorPress";

        public static string SafeSecurityBreakFactorMaterialScore = "SafeSecurityBreakFactorMaterialScore";
        public static string SafeSecurityBreakFactorPipeAgeScore = "SafeSecurityBreakFactorPipeAgeScore";
        public static string SafeSecurityBreakFactorPressScore = "SafeSecurityBreakFactorPressScore";

        public static string SafeSecurityBreakFactorMaterialQZ = "SafeSecurityBreakFactorMaterialQZ";
        public static string SafeSecurityBreakFactorPipeAgeQZ = "SafeSecurityBreakFactorPipeAgeQZ";
        public static string SafeSecurityBreakFactorPressQZ = "SafeSecurityBreakFactorPressQZ";

        //安全性-管网保障-事件时节点流量保证率
        public static string SafeSecurityAccidentFlowRateScore = "SafeSecurityAccidentFlowRateScore";
        public static string SafeSecurityAccidentFlowRateQZ = "SafeSecurityAccidentFlowRateQZ";
        public static string SafeSecurityAccidentFlowRate = "SafeSecurityAccidentFlowRate";

        //安全性-管网保障-输配水管线备用
        public static string SafeSecurityPipeStandbyScore = "SafeSecurityPipeStandbyScore";
        public static string SafeSecurityPipeStandbyQZ = "SafeSecurityPipeStandbyQZ";
        public static string SafeSecurityPipeStandby = "SafeSecurityPipeStandby";
        //安全性-管网保障-管网保障综合评价
        public static string SafeSecuritySumScore = "SafeSecuritySumScore";
        public static string SafeSecuritySumQZ = "SafeSecuritySumQZ";
        //安全性-综合管理-应急调度预案
        public static string SafeAdminEmergencyPlanScore = "SafeAdminEmergencyPlanScore";
        public static string SafeAdminEmergencyPlanQZ = "SafeAdminEmergencyPlanQZ";
        public static string SafeAdminEmergencyPlan = "SafeAdminEmergencyPlan";




        /// <summary>
        /// 安全性-压力-权重
        /// </summary>
        public static string SafetyHydraulicPressureQuanZhong = "SafetyHydraulicPressureQuanZhong";


         /// <summary>
        /// 安全性-备用水源-权重
        /// </summary>
        public static string SafetyBackupWaterSourceQuanZhong = "SafetyBackupWaterSourceQuanZhong";


         /// <summary>
        /// 安全性-供需比-权重
        /// </summary>
        public static string SafetySupplyAndDemandThanQuanZhong = "SafetySupplyAndDemandThanQuanZhong";





        /// <summary>
        /// 安全性-压力-总分
        /// </summary>
        public static string SafetyHydraulicPressureScore = "SafetyHydraulicPressureScore";


         /// <summary>
        /// 安全性-备用水源-总分
        /// </summary>
        public static string SafetyBackupWaterSourceScore = "SafetyBackupWaterSourceScore";


         /// <summary>
        /// 安全性-供需比-总分
        /// </summary>
        public static string SafetySupplyAndDemandThanScore = "SafetySupplyAndDemandThanScore";





        /// <summary>
        /// 安全性-压力-Po
        /// </summary>
        public static string SafetyHydraulicPressurePo = "SafetyHydraulicPressurePo";


        /// <summary>
        /// 安全性-压力-标定曲线函数
        /// </summary>
        public static string SafetyHydraulicPressureBiaoDing = "SafetyHydraulicPressureBiaoDing";

        /// <summary>
        /// 安全性-压力-压力偏离
        /// </summary>
        public static string SafetyHydraulicPressureW = "SafetyHydraulicPressureW";



        /// <summary>
        /// 安全性-供需比-标定曲线函数
        /// </summary>
        public static string SafetySupplyAndDemandThanBiaoDing = "SafetySupplyAndDemandThanBiaoDing";

        /// <summary>
        /// 安全性-供需比-供水
        /// </summary>
        public static string SafetySupplyAndDemandThanFactoryDemand = "SafetySupplyAndDemandThanFactoryDemand";
        /// <summary>
        /// 安全性-供需比-需水
        /// </summary>
        public static string SafetySupplyAndDemandThanPipeDemand = "SafetySupplyAndDemandThanPipeDemand";
        /// <summary>
        /// 安全性-供需比-供需比
        /// </summary>
        public static string SafetySupplyAndDemandThanFactoryDivPipe = "SafetySupplyAndDemandThanFactoryDivPipe";


        //liuhz add
        #region 可持续性

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 可持续性总分
        /// </summary>
        public static string SustainabilityScore = "SustainabilityScore";

        //可持续性-能耗利用
        public static string SustainConsumptionScore = "SustainConsumptionScore";
        public static string SustainConsumptionQZ = "SustainConsumptionQZ";
        public static string SustainConsumption = "SustainConsumption";
        //可持续性-资源利用
        public static string SustainResourcesScore = "SustainResourcesScore";
        public static string SustainResourcesQZ = "SustainResourcesQZ";
        public static string SustainResources = "SustainResources";
        //可持续性-系统生态
        public static string SustainEcologicalScore = "SustainEcologicalScore";
        public static string SustainEcologicalQZ = "SustainEcologicalQZ";
        public static string SustainEcological = "SustainEcological";
        ////可持续性-能耗利用-单水电耗
        //public static string SustainConsumptionECScore = "SustainConsumptionECScore";
        //public static string SustainConsumptionECQZ = "SustainConsumptionECQZ";
        //public static string SustainConsumptionEC = "SustainConsumptionEC";
        //可持续性-能耗利用-全生命周期能耗（kWh/m3）
        public static string SustainConsumptionCycleECScore = "SustainConsumptionCycleECScore";
        public static string SustainConsumptionCycleECQZ = "SustainConsumptionCycleECQZ";
        public static string SustainConsumptionCycleEC = "SustainConsumptionCycleEC";

        public static string SustainConsumptionCycleECRecycleWater = "SustainConsumptionCycleECRecycleWater";
        public static string SustainConsumptionCycleECUnderWater = "SustainConsumptionCycleECUnderWater";
        public static string SustainConsumptionCycleECSurfaceWater = "SustainConsumptionCycleECSurfaceWater";
        public static string SustainConsumptionCycleECRecycleWaterFactor = "SustainConsumptionCycleECRecycleWaterFactor";
        public static string SustainConsumptionCycleECUnderWaterFactor = "SustainConsumptionCycleECUnderWaterFactor";
        public static string SustainConsumptionCycleECSurfaceWaterFactor = "SustainConsumptionCycleECSurfaceWaterFactor";
        public static string SustainConsumptionCycleECPipeService = "SustainConsumptionCycleECPipeService";
        public static string SustainConsumptionCycleECPumpEC = "SustainConsumptionCycleECPumpEC";  //关联泵站生产单水平均耗能：EconomicEnergyConsumptionE
        //可持续性-能耗利用-单水温室气体排放
        public static string SustainConsumptionGHGScore = "SustainConsumptionGHGScore";
        public static string SustainConsumptionGHGQZ = "SustainConsumptionGHGQZ";
        public static string SustainConsumptionGHG = "SustainConsumptionGHG";

        public static string SustainConsumptionGHGRecycleWaterFactor = "SustainConsumptionGHGRecycleWaterFactor";
        public static string SustainConsumptionGHGUnderWaterFactor = "SustainConsumptionGHGUnderWaterFactor";
        public static string SustainConsumptionGHGSurfaceWaterFactor = "SustainConsumptionGHGSurfaceWaterFactor";
        public static string SustainConsumptionGHGPump = "SustainConsumptionGHGPump";
        //可持续性-能耗利用-能耗利用综合评价
        public static string SustainConsumptionSumScore = "SustainConsumptionSumScore";
        public static string SustainConsumptionSumQZ = "SustainConsumptionSumQZ";
        public static string SustainConsumptionSum = "SustainConsumptionSum";
        //可持续性-资源利用-再生水利用率
        public static string SustainResourcesReclaimedWaterScore = "SustainResourcesReclaimedWaterScore";
        public static string SustainResourcesReclaimedWaterQZ = "SustainResourcesReclaimedWaterQZ";
        public static string SustainResourcesReclaimedWater = "SustainResourcesReclaimedWater";
        //可持续性-资源利用-雨水回收覆盖率
        public static string SustainResourcesRainRateScore = "SustainResourcesRainRateScore";
        public static string SustainResourcesRainRateQZ = "SustainResourcesRainRateQZ";
        public static string SustainResourcesRainRate = "SustainResourcesRainRate";
        //可持续性-资源利用-管网漏损率
        public static string SustainResourcesLeakageQZ = "SustainResourcesLeakageQZ";
        //可持续性-资源利用-原水供给效率/%
        public static string SustainResourcesRawWaterScore = "SustainResourcesRawWaterScore";
        public static string SustainResourcesRawWaterQZ = "SustainResourcesRawWaterQZ";
        public static string SustainResourcesRawWater = "SustainResourcesRawWater";
        //add 20200827
        //可持续性-资源利用-供水产销差/%
        public static string SustainResourcesFeeWater = "SustainResourcesFeeWater";
        //可持续性-资源利用-万元工业增加值用水量（m3/万元）
        public static string SustainResourcesAddedWaterScore = "SustainResourcesAddedWaterScore";
        public static string SustainResourcesAddedWaterQZ = "SustainResourcesAddedWaterQZ";
        public static string SustainResourcesAddedWater = "SustainResourcesAddedWater";
        //可持续性-资源利用-资源利用综合评价
        public static string SustainResourcesSumScore = "SustainResourcesSumScore";
        public static string SustainResourcesSumQZ = "SustainResourcesSumQZ";
        public static string SustainResourcesSum = "SustainResourcesSum";
        //可持续性-系统生态-地表水开发利用率(水源地超采率)
        public static string SustainEcologicalOverExploitedScore = "SustainEcologicalOverExploitedScore";
        public static string SustainEcologicalOverExploitedQZ = "SustainEcologicalOverExploitedQZ";
        public static string SustainEcologicalOverExploited = "SustainEcologicalOverExploited";
        //可持续性-系统生态-地下水开采系数
        public static string SustainEcologicalGroundWaterScore = "SustainEcologicalGroundWaterScore";
        public static string SustainEcologicalGroundWaterQZ = "SustainEcologicalGroundWaterQZ";
        public static string SustainEcologicalGroundWater = "SustainEcologicalGroundWater";         //地下水开采系数=地下水开采量/可开采资源量
        public static string SustainEcologicalGroundWater1 = "SustainEcologicalGroundWater1";       //地下水开采量
        public static string SustainEcologicalGroundWater2 = "SustainEcologicalGroundWater2";       //可开采资源量
        //可持续性-系统生态-系统生态综合评价
        public static string SustainEcologicalSumScore = "SustainEcologicalSumScore";
        public static string SustainEcologicalSumQZ = "SustainEcologicalSumQZ";
        public static string SustainEcologicalSum = "SustainEcologicalSum";


        ///// <summary>
        ///// 经济性-固定投资-总分
        ///// </summary>
        //public static string EconomicFixedInvestmentScore = "EconomicFixedInvestmentScore";



        ///// <summary>
        ///// 经济性-固定投资权重
        ///// </summary>
        //public static string EconomicFixedInvestmentQuanZhong = "EconomicFixedInvestmentQuanZhong";
        ///// <summary>
        ///// 经济性-能耗分析权重
        ///// </summary>
        //public static string EconomicEnergyConsumptionQuanZhong = "EconomicEnergyConsumptionQuanZhong";



        //#region 经济性-固定投资-
        ///// <summary>
        ///// 经济性-固定投资-水厂投资
        ///// </summary>
        //public static string EconomicFixedInvestmentFactory = "EconomicFixedInvestmentFactory";

        ///// <summary>
        ///// 经济性-固定投资-泵站投资
        ///// </summary>
        //public static string EconomicFixedInvestmentPump = "EconomicFixedInvestmentPump";

        ///// <summary>
        ///// 经济性-固定投资-管网投资
        ///// </summary>
        //public static string EconomicFixedInvestmentPipe = "EconomicFixedInvestmentPipe";

        ///// <summary>
        ///// 经济性-固定投资-其他投资
        ///// </summary>
        //public static string EconomicFixedInvestmentOther = "EconomicFixedInvestmentOther";


        ///// <summary>
        ///// 经济性-固定投资-总投资
        ///// </summary>
        //public static string EconomicFixedInvestmentSum = "EconomicFixedInvestmentSum";

        ///// <summary>
        ///// 经济性-固定投资-C优
        ///// </summary>
        //public static string EconomicFixedInvestmentCYou = "EconomicFixedInvestmentCYou";

        ///// <summary>
        ///// 经济性-固定投资-标定曲线函数
        ///// </summary>
        //public static string EconomicFixedInvestmentBiaoDing = "EconomicFixedInvestmentBiaoDing";
        //#endregion


        ///// <summary>
        ///// 经济性-能耗分析-总分
        ///// </summary>
        //public static string EconomicEnergyConsumptionScore = "EconomicEnergyConsumptionScore";


        //#region 经济性-能耗分析-
        ///// <summary>
        ///// 经济性-能耗分析-标定曲线函数
        ///// </summary>
        //public static string EconomicEnergyConsumptionBiaoDing = "EconomicEnergyConsumptionBiaoDing";



        ///// <summary>
        ///// 经济性-能耗分析-E
        ///// </summary>
        //public static string EconomicEnergyConsumptionE = "EconomicEnergyConsumptionE";

        ///// <summary>
        ///// 经济性-能耗分析-E优
        ///// </summary>
        //public static string EconomicEnergyConsumptionEYou = "EconomicEnergyConsumptionEYou";
        //#endregion
        #endregion





        /// <summary>
        /// 现状管网利用率设置
        /// </summary>
        public static string JueCheSetUpxzgwlyl = "JueCheSetUpxzgwlyl";

        /// <summary>
        /// 水厂负荷均匀性设置
        /// </summary>
        public static string JueCheSetUpscfhjyx = "JueCheSetUpscfhjyx";

        /// <summary>
        /// 现状管网总长度
        /// </summary>
        public static string xzgwzcd = "xzgwzcd";



        /// <summary>
        /// 现状管网利用率
        /// </summary>
        public static string xzgwlyl = "xzgwlyl";


        /// <summary>
        /// 专家打分
        /// </summary>
        public static string ZhuangJiaScore = "ZhuangJiaScore";

        /// <summary>
        /// 技术性性标定曲线函数
        /// </summary>
        //水质合格率类
        public static string TechNicalWaterQualityRateBiaoDing = "TechNicalWaterQualityRateBiaoDing";
        //安全加氯量类
        public static string TechNicalWaterQualityClBiaoDing = "TechNicalWaterQualityClBiaoDing";
        //管网漏损率类
        public static string TechNicalSupplyEfficiencyLeakageBiaoDing = "TechNicalSupplyEfficiencyLeakageBiaoDing";
        //供水覆盖率类
        public static string TechNicalSupplyEfficiencyCoverageBiaoDing = "TechNicalSupplyEfficiencyCoverageBiaoDing";
        /// <summary>
        /// 安全性性标定曲线函数
        /// </summary>
        //枯水年水量保证率面板类
        public static string SafeSupplyLowWaterRateBiaoDing = "SafeSupplyLowWaterRateBiaoDing";
        //取水能力面板类
        public static string SafeSupplyGetWaterBiaoDing = "SafeSupplyGetWaterBiaoDing";
        //调蓄水量比率面板类
        public static string SafeSupplyAdjustRateBiaoDing = "SafeSupplyAdjustRateBiaoDing";
        //爆管影响率面板类
        public static string SafeSecurityBreakInfluenceBiaoDing = "SafeSecurityBreakInfluenceBiaoDing";
        //事件时节点流量保证率面板类
        public static string SafeSecurityAccidentFlowRateBiaoDing = "SafeSecurityAccidentFlowRateBiaoDing";
        //输配水管线备用面板类
        public static string SafeSecurityPipeStandbyBiaoDing = "SafeSecurityPipeStandbyBiaoDing";
        /// <summary>
        /// 可持续性标定曲线函数
        /// </summary>
        ////单水电耗标定曲线
        //public static string EconomicEnergyConsumptionBiaoDing = "EconomicEnergyConsumptionBiaoDing";
        //全生命周期能耗（kWh/m3）标定曲线
        public static string SustainConsumptionCycleECBiaoDing = "SustainConsumptionCycleECBiaoDing";
        //单水温室气体排放标定曲线
        public static string SustainConsumptionGHGBiaoDing = "SustainConsumptionGHGBiaoDing";
        //再生水利用率%标定曲线
        public static string SustainResourcesReclaimedWaterBiaoDing = "SustainResourcesReclaimedWaterBiaoDing";
        //雨水回收覆盖率%标定曲线
        public static string SustainResourcesRainRateBiaoDing = "SustainResourcesRainRateBiaoDing";
        ////管网漏损率%标定曲线
        //public static string TechNicalSupplyEfficiencyLeakageBiaoDing = "TechNicalSupplyEfficiencyLeakageBiaoDing";
        //原水供给效率/%标定曲线
        public static string SustainResourcesRawWaterBiaoDing = "SustainResourcesRawWaterBiaoDing";
        //万元工业增加值用水量（m3/万元）标定曲线
        public static string SustainResourcesAddedWaterBiaoDing = "SustainResourcesAddedWaterBiaoDing";
        //地表水开发利用率标定曲线
        public static string SustainEcologicalOverExploitedBiaoDing = "SustainEcologicalOverExploitedBiaoDing";
        //地下水开采系数标定曲线
        public static string SustainEcologicalGroundWaterBiaoDing = "SustainEcologicalGroundWaterBiaoDing";



        public static string SustainabilityScoreQuanZhong = "SustainabilityScoreQuanZhong"; //可持续性权重
        public static string ZhuangJiaScoreQuanZhong = "ZhuangJiaScoreQuanZhong";           
        public static string TechScoreQuanZhong = "TechScoreQuanZhong";
        public static string EconomicScoreQuanZhong = "EconomicScoreQuanZhong";
        public static string SafetyScoreQuanZhong = "SafetyScoreQuanZhong";




        public static string AllScore = "AllScore";
        #endregion



        /// <summary>
        /// 根据标定曲线由x求服务水平score
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="x"></param>
        /// <param name="score"></param>
        public static void GetScore(DataSet ds, double x, ref double score)
        {

        }





        //最小XY值
        public static void geiMinXYvalue(double xx, ref double x1)
        {
            try
            {
                if (xx == 0) return;
                double b = 10/10;

                for (double i = 0.00001; i <= 0.00009; i += 0.00001)
                {
                    if (xx >= i * 10 && xx < (i + 0.00001) * 10)
                    {
                        //xx=0.13
                        x1 =  i * 10;
                        return;
                    }

                }

                for (double i = 0.0001; i <= 0.0009; i += 0.0001)
                {
                    if (xx >= i * 10 && xx < (i + 0.0001) * 10)
                    {
                        //xx=35 x1=4 i=3
                        x1 =  i * 10;
                        return;
                    }

                }

                for (double i = 0.001; i <= 0.009; i += 0.001)
                {
                    if (xx >= i * 10 && xx < (i + 0.001) * 10)
                    {
                        //xx=35 x1=4 i=3
                        x1 =  i * 10;
                        return;
                    }

                }


                for (double i = 0.01; i <= 0.09; i += 0.01)
                {
                    if (xx >= i * 10 && xx < (i + 0.01) * 10)
                    {
                        //xx=35.4 
                        x1 =  i * 10;
                        return;
                    }

                }


                for (double i = 0.1; i <= 0.9; i += 0.1)
                {
                    if (xx >= i * 10 && xx < (i + 0.1) * 10)
                    {
                        //xx=35 x1=4 i=3
                        x1 = i * 10;
                        return;
                    }

                }



                for (int i = 1; i <=9; i++)
                {
                    if (xx >=i*10 && xx < (i+1)*10)
                    {
                        //xx=35 x1=4 i=3
                        x1 = i * 10;
                        return;
                    }
                    
                }


                for (int i = 10; i <= 90; i+=10)
                {
                    if (xx >= i * 10 && xx < (i + 10) * 10)
                    {
                        //xx=35 x1=4 i=3
                        x1 = i * 10;
                        return;
                    }

                }


                for (int i = 100; i <= 900; i += 100)
                {
                    if (xx >= i * 10 && xx < (i + 100) * 10)
                    {
                        //xx=35 x1=4 i=3
                        x1 = i * 10;
                        return;
                    }

                }


                x1 = 0;




            }
            catch (Exception)
            {
                
               
            }
        }

      









        //正态分布图的X间隔
        public static void geiXvalue(double xx, ref double x1)
        {
            try
            {
                double b = 10/10;

                for (double i = 0.00001; i <= 0.0001; i += 0.00001)
                {
                    if (xx >= i * 10 && xx < (i + 0.00001) * 10)
                    {
                        //xx=35 x1=4 i=3
                        x1 = (i + 0.00001) *1 * b;
                        return;
                    }

                }

                for (double i = 0.0001; i <= 0.001; i += 0.0001)
                {
                    if (xx >= i * 10 && xx < (i + 0.0001) * 10)
                    {
                        //xx=35 x1=4 i=3
                        x1 = (i + 0.0001) * 1 * b;
                        return;
                    }

                }

                for (double i = 0.001; i <= 0.01; i += 0.001)
                {
                    if (xx >= i * 10 && xx < (i + 0.001) * 10)
                    {
                        //xx=35 x1=4 i=3
                        x1 = (i + 0.001) * 1 * b;
                        return;
                    }

                }


                for (double i = 0.01; i <= 0.1; i += 0.01)
                {
                    if (xx >= i * 10 && xx < (i + 0.01) * 10)
                    {
                        //xx=35 x1=4 i=3
                        x1 = (i + 0.01) *1 * b;
                        return;
                    }

                }


                for (double i = 0.1; i <= 1; i += 0.1)
                {
                    if (xx >= i * 10 && xx < (i + 0.1) * 10)
                    {
                        //xx=35 x1=4 i=3
                        x1 = (i + 0.1) * 1 * b; 
                        return;
                    }

                }



                for (int i = 1; i <=10; i++)
                {

                  
                    if (xx >= i * 10 && xx < (i + 1) * 10 )
                    {
                        if (xx == i * 10) x1 = i;
                        else
                        //xx=35 x1=4 i=3
                        x1 = (i + 1) * 1 * b;
                        return;
                    }




                }


                for (int i = 10; i <= 100; i+=10)
                {
                    if (xx >= i * 10 && xx < (i + 10) * 10)
                    {
                        //xx=35 x1=4 i=3
                        x1 = (i + 10) * 1 * b;
                        return;
                    }

                }


                for (int i = 100; i <= 1000; i += 100)
                {
                    if (xx >= i * 10 && xx < (i + 100) * 10)
                    {
                        //xx=35 x1=4 i=3
                        x1 = (i + 100) * 1 * b;
                        return;
                    }

                }


                x1 = 0;




            }
            catch (Exception)
            {
                
               
            }
        }

        

        
        }
    }

