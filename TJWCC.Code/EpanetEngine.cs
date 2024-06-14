//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Runtime.InteropServices;
//using System.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Runtime.InteropServices;

namespace TJWCC.Code
{
    public class EpanetEngine
    {
        #region 节点参数代号包含以下常量：
        /// <summary>
        /// 节点标高
        /// </summary>
        public const char EN_ELEVATION = (char)0;
        /// <summary>
        /// 节点基本需水量
        /// </summary>
        public const char EN_BASEDEMAND = (char)1;
        /// <summary>
        /// 节点需水量模式索引
        /// </summary>
        public const char EN_PATTERN = (char)2;
        /// <summary>
        /// 节点扩散器系数
        /// </summary>
        public const char EN_EMITTER = (char)3;
        /// <summary>
        /// 节点初始水质
        /// </summary>
        public const char EN_INITQUAL = (char)4;
        /// <summary>
        /// 节点源头水质
        /// </summary>
        public const char EN_SOURCEQUAL = (char)5;
        /// <summary>
        /// 节点源头模式索引
        /// </summary>
        public const char EN_SOURCEPAT = (char)6;
        /// <summary>
        /// 节点源头类型
        /// </summary>
        public const char EN_SOURCETYPE = (char)7;
        /// <summary>
        /// 节点水池中初始水位
        /// </summary>
        public const char EN_TANKLEVEL = (char)8;
        /// <summary>
        /// 节点实际需水量
        /// </summary>
        public const char EN_DEMAND = (char)9;
        /// <summary>
        /// 节点水力水头
        /// </summary>
        public const char EN_HEAD = (char)10;
        /// <summary>
        /// 节点压强
        /// </summary>
        public const char EN_PRESSURE = (char)11;
        /// <summary>
        /// 节点实际水质
        /// </summary>
        public const char EN_QUALITY = (char)12;
        /// <summary>
        /// 节点每分钟化学成分源头的质量流量
        /// </summary>
        public const char EN_SOURCEMASS = (char)13;
        /// <summary>
        /// 节点漏失流量
        /// </summary>
        public const char EN_EMITTERFLOW = (char)14;
        #endregion

        #region 管段参数代码包含以下常量：
        /// <summary>
        /// 管段直径
        /// </summary>
        public const char EN_DIAMETER = (char)0;
        /// <summary>
        /// 管段长度
        /// </summary>
        public const char EN_LENGTH = (char)1;
        /// <summary>
        /// 管段粗糙系数
        /// </summary>
        public const char EN_ROUGHNESS = (char)2;
        /// <summary>
        /// 管段局部损失系数
        /// </summary>
        public const char EN_MINORLOSS = (char)3;
        /// <summary>
        /// 管段初始管段状态(0 = 关闭, 1 = 开启)
        /// </summary>
        public const char EN_INITSTATUS = (char)4;
        /// <summary>
        /// 管段初始管道粗糙度、初始水泵转速、初始阀门设置
        /// </summary>
        public const char EN_INITSETTING = (char)5;
        /// <summary>
        /// 管段主流反应系数
        /// </summary>
        public const char EN_KBULK = (char)6;
        /// <summary>
        /// 管段管壁反应系数
        /// </summary>
        public const char EN_KWALL = (char)7;
        /// <summary>
        /// 管段流量
        /// </summary>
        public const char EN_FLOW = (char)8;
        /// <summary>
        /// 管段流速
        /// </summary>
        public const char EN_VELOCITY = (char)9;
        /// <summary>
        /// 管段水头损失
        /// </summary>
        public const char EN_HEADLOSS = (char)10;
        /// <summary>
        /// 管段实际管段状态(0 = 关闭, 1 = 开启)
        /// </summary>
        public const char EN_STATUS = (char)11;
        /// <summary>
        /// 管段管道粗糙系数、实际水泵转速、实际阀门设置
        /// </summary>
        public const char EN_SETTING = (char)12;
        /// <summary>
        /// 管段消耗能量，以千瓦计
        /// </summary>
        public const char EN_ENERGY = (char)13;
        /// <summary>
        /// 管段效率
        /// </summary>
        public const char EN_EFFICIENCY = (char)14;
        #endregion

        #region 组件编号包含以下：
        /// <summary>
        /// 节点
        /// </summary>
        public const char EN_NODECOUNT = (char)0;
        /// <summary>
        /// 水库和水池节点
        /// </summary>
        public const char EN_TANKCOUNT = (char)1;
        /// <summary>
        /// 管段
        /// </summary>
        public const char EN_LINKCOUNT = (char)2;
        /// <summary>
        /// 时间模式
        /// </summary>
        public const char EN_PATCOUNT = (char)3;
        /// <summary>
        /// 曲线
        /// </summary>
        public const char EN_CURVECOUNT = (char)4;
        /// <summary>
        /// 简单控制
        /// </summary>
        public const char EN_CONTROLCOUNT = (char)5;
        #endregion

        #region 组件编号包含以下：
        /// <summary>
        /// 连接节点
        /// </summary>
        public const char EN_JUNCTION = (char)0;
        /// <summary>
        /// 水库节点
        /// </summary>
        public const char EN_RESERVOIR = (char)1;
        /// <summary>
        /// 水池节点
        /// </summary>
        public const char EN_TANK = (char)2;
        /// <summary>
        /// 
        /// </summary>
        public const char EN_VSPCN = (char)3;
        #endregion

        #region 管段类型代号包含了以下常量：
        /// <summary>
        /// 具有止回阀的管道
        /// </summary>
        public const char EN_CVPIPE = (char)0;
        /// <summary>
        /// 管道
        /// </summary>
        public const char EN_PIPE = (char)1;
        /// <summary>
        /// 水泵
        /// </summary>
        public const char EN_PUMP = (char)2;
        /// <summary>
        /// 减压阀
        /// </summary>
        public const char EN_PRV = (char)3;
        /// <summary>
        /// 稳压阀
        /// </summary>
        public const char EN_PSV = (char)4;
        /// <summary>
        /// 压力制动阀
        /// </summary>
        public const char EN_PBV = (char)5;
        /// <summary>
        /// 流量控制阀
        /// </summary>
        public const char EN_FCV = (char)6;
        /// <summary>
        /// 节流控制阀
        /// </summary>
        public const char EN_TCV = (char)7;
        /// <summary>
        /// 常规阀门
        /// </summary>
        public const char EN_GPV = (char)8;
        #endregion

        #region 保存结果到文件标志：
        /// <summary>
        /// 没有保存
        /// </summary>
        public const char EN_NOSAVE = (char)0;
        /// <summary>
        /// 保存
        /// </summary>
        public const char EN_SAVE = (char)1;
        /// <summary>
        /// 
        /// </summary>
        public const char EN_SCRATCH = (char)2;
        /// <summary>
        /// 重新初始化流标志
        /// </summary>
        public const char EN_INITFLOW = (char)10;   /* Re-initialize flows flag  */
        #endregion

        #region 管段状态：
        /// <summary>
        /// 关闭
        /// </summary>
        public const char EN_CLOSED = (char)0;
        /// <summary>
        /// 打开
        /// </summary>
        public const char EN_OPEN = (char)1;
        /// <summary>
        /// 活跃
        /// </summary>
        public const char EN_ACTIVE = (char)2;
        #endregion

        #region 流量范围代码如下：
        /// <summary>
        /// 立方英尺每秒
        /// </summary>
        public const char EN_CFS = (char)0;
        /// <summary>
        /// 加仑每分
        /// </summary>
        public const char EN_GPM = (char)1;
        /// <summary>
        /// 百万加仑每日
        /// </summary>
        public const char EN_MGD = (char)2;
        /// <summary>
        /// 英制mgd
        /// </summary>
        public const char EN_IMGD = (char)3;
        /// <summary>
        /// 英亩—英尺每日
        /// </summary>
        public const char EN_AFD = (char)4;
        /// <summary>
        /// 升每秒
        /// </summary>
        public const char EN_LPS = (char)5;
        /// <summary>
        /// 升每分
        /// </summary>
        public const char EN_LPM = (char)6;
        /// <summary>
        /// 百万升每日
        /// </summary>
        public const char EN_MLD = (char)7;
        /// <summary>
        /// 立方米每时
        /// </summary>
        public const char EN_CMH = (char)8;
        /// <summary>
        /// 立方米每日
        /// </summary>
        public const char EN_CMD = (char)9;
        /// <summary>
        /// 
        /// </summary>
        public const char EN_SI = (char)10;
        /// <summary>
        /// 
        /// </summary>
        public const char EN_US = (char)11;
        #endregion

        #region 压力单位类型
        /// <summary>
        /// PSI
        /// </summary>
        public const char EN_PSI = (char)0;
        /// <summary>
        /// KPA
        /// </summary>
        public const char EN_KPA = (char)1;
        /// <summary>
        /// 米
        /// </summary>
        public const char EN_M = (char)2;
        #endregion

        #region 水头损失类型
        /// <summary>
        /// Hazen-WilliamsC因子  管道摩阻
        /// </summary>
        public const char EN_HW = (char)0;
        /// <summary>
        /// Darcy-Weisbach粗糙系数
        /// </summary>
        public const char EN_DW = (char)1;
        /// <summary>
        /// Chezy-Manning粗糙系数
        /// </summary>
        public const char EN_CM = (char)2;
        #endregion

        #region 水质分析代码如下：
        /// <summary>
        /// 不进行水质分析
        /// </summary>
        public const char EN_NONE = (char)0;
        /// <summary>
        /// 化学成分分析
        /// </summary>
        public const char EN_CHEM = (char)1;
        /// <summary>
        /// 水龄分析
        /// </summary>
        public const char EN_AGE = (char)2;
        /// <summary>
        /// 源头跟踪
        /// </summary>
        public const char EN_TRACE = (char)3;
        #endregion

        #region 源头类型利用以下常量标识：
        /// <summary>
        /// CONCEN源头：浓度随时间变化。表示任何进入节点的外部源头进流浓度。仅当节点具有负需水量时使用（水在该节点进入管网）
        /// </summary>
        public const char EN_CONCEN = (char)0;
        /// <summary>
        /// MASS类型源头的强度计量为质量流量每分。所有其它类型测试源头强度，以浓度单位计。
        /// </summary>
        public const char EN_MASS = (char)1;
        /// <summary>
        /// SETPOINT注入，固定了任何离开节点的水流浓度（只要进流来的浓度低于设置值)
        /// </summary>
        public const char EN_SETPOINT = (char)2;
        /// <summary>
        /// FLOWPACED注入，将固定浓度添加到节点进流浓度中
        /// </summary>
        public const char EN_FLOWPACED = (char)3;
        #endregion

        #region 分析
        /// <summary>
        /// STOP将在该点终止整个分析
        /// </summary>
        public const char EN_STOP = (char)0;
        /// <summary>
        /// CONTINUE将在公布警告消息的情况下继续分析。
        /// </summary>
        public const char EN_CONTINUE = (char)1;
        #endregion

        #region 泵类型
        public const char EN_CPOWER = (char)0;
        public const char EN_CSPEED = (char)1;
        public const char EN_CHEAD = (char)2;
        #endregion

        #region 在wtrg元素类型
        /// <summary>
        /// 管段
        /// </summary>
        public const char MO_PIPE = (char)69;
        /// <summary>
        /// 连接节点
        /// </summary>
        public const char MO_JUNCTION = (char)55;
        /// <summary>
        /// 水库节点
        /// </summary>
        public const char MO_RESERVIOR = (char)56;
        /// <summary>
        /// 水池节点
        /// </summary>
        public const char MO_TANK = (char)52;
        /// <summary>
        /// 水泵
        /// </summary>
        public const char MO_PUMP = (char)68;
        /// <summary>
        /// 消火栓
        /// </summary>
        public const char MO_HYDRANT = (char)54;
        /// <summary>
        /// PRV (减压阀)
        /// </summary>
        public const char MO_PRV = (char)64;
        /// <summary>
        /// PSV (稳压阀)
        /// </summary>
        public const char MO_PSV = (char)65;
        /// <summary>
        /// PBV (压力制动阀)
        /// </summary>
        public const char MO_PBV = (char)66;
        /// <summary>
        /// FCV (流量控制阀)
        /// </summary>
        public const char MO_FCV = (char)60;
        /// <summary>
        /// TCV (节流控制阀)
        /// </summary>
        public const char MO_TCV = (char)61;
        /// <summary>
        /// GPV (常规阀门)
        /// </summary>
        public const char MO_GPV = (char)62;
        /// <summary>
        /// CV，意味着管道包含了限制流向的止回阀
        /// </summary>
        public const char MO_CV = (char)309;
        #endregion

        #region 调用EPANETH的方法
        #region 运行完整的“命令行”模拟
        /// <summary>
        /// 执行完整的EPANETH模拟。
        /// </summary>
        /// <param name="f1">输入文件名</param>
        /// <param name="f2">输出报告文件名</param>
        /// <param name="f3">可选二进制输出文件名</param>
        /// <param name="vfunc">将字符串作为参数的用户使用函数的指针。</param>
        /// <returns>返回错误代号。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENepanet(string f1, string f2, string f3, IntPtr vfunc);
        #endregion

        #region 打开和关闭EPANETH工具箱系统
        /// <summary>
        /// 使用ENopen函数打开工具箱系统，同时打开EPANETH输入文件。
        /// </summary>
        /// <param name="inp">EPANETH输入文件名</param>
        /// <param name="out1">输出报告文件名</param>
        /// <param name="out2">可选二进制输出文件名。</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENopen(string inp, string out1, string out2);

        /// <summary>
        /// 关闭工具箱系统(包括所有正在处理的文件)。
        /// </summary>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENclose();
        #endregion

        #region 检索管网节点信息
        /// <summary>
        /// 检索具有指定ID的节点索引。
        /// </summary>
        /// <param name="id">节点ID标签</param>
        /// <param name="index">节点索引</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENgetnodeindex(string id, int* index);

        /// <summary>
        /// 检索具有指定索引的ID标签。
        /// </summary>
        /// <param name="index">节点索引</param>
        /// <param name="id">节点的ID标签</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENgetnodeid(int index, StringBuilder id);

        /// <summary>
        /// 检索指定节点的节点类型编号。
        /// </summary>
        /// <param name="index">节点索引</param>
        /// <param name="code">节点类型编号</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENgetnodetype(int index, int* code);

        /// <summary>
        /// 检索特定管段参数值。
        /// </summary>
        /// <param name="index">节点索引</param>
        /// <param name="code">参数代号</param>
        /// <param name="value">参数值</param>
        /// <returns>返回错误代号。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENgetnodevalue(int index, int code, float* value);
        #endregion

        #region 检索管网管段信息
        /// <summary>
        /// 检索具有特定ID的管段索引。
        /// </summary>
        /// <param name="id">管段ID标签</param>
        /// <param name="index">管段索引</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENgetlinkindex(String id, int* index);

        /// <summary>
        /// 检索具有特定指标的管段的ID标签。
        /// </summary>
        /// <param name="index">管段索引</param>
        /// <param name="id">管段的ID标签</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENgetlinkid(int index, StringBuilder id);

        /// <summary>
        /// 检索特定管段的类型编号。
        /// </summary>
        /// <param name="index">管段索引</param>
        /// <param name="code">管段类型代号</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENgetlinktype(int index, int* code);

        /// <summary>
        /// 检索指定管段端点节点的索引。
        /// </summary>
        /// <param name="index">管段索引</param>
        /// <param name="node1">管段起始节点索引</param>
        /// <param name="node2">管段终止节点索引</param>
        /// <returns>返回错误代号。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENgetlinknodes(int index, int* node1, int* node2);

        /// <summary>
        /// 检索指定管段参数值。
        /// </summary>
        /// <param name="index">管段索引</param>
        /// <param name="code">参数代号</param>
        /// <param name="value">参数值</param>
        /// <returns>返回错误代号。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENgetlinkvalue(int index, int code, float* value);
        #endregion

        #region 检索时间模式信息
        /// <summary>
        /// 检索特定时间模式的ID标签。
        /// </summary>
        /// <param name="index">模式索引</param>
        /// <param name="id">模式的ID标签</param>
        /// <returns>返回错误代号。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENgetpatternid(int index, StringBuilder id);

        /// <summary>
        /// 检索特定时间模式的索引。
        /// </summary>
        /// <param name="id">模式ID标签</param>
        /// <param name="index">模式索引</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENgetpatternindex(String id, int* index);

        /// <summary>
        /// 检索特定时间模式中的时段总数。
        /// </summary>
        /// <param name="index">模式索引</param>
        /// <param name="len">模式中时段总数</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENgetpatternlen(int index, int* len);

        /// <summary>
        /// 检索时间模式中特定时段的乘子。
        /// </summary>
        /// <param name="index">时间模式索引</param>
        /// <param name="period">时间模式范围内的时段</param>
        /// <param name="value">时段的乘子</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENgetpatternvalue(int index, int period, float* value);
        #endregion

        #region 检索其它管网信息
        /// <summary>
        /// 检索简单控制语句的参数。控制的索引在cindex中指定，以及剩余的参数返回控制参数。
        /// </summary>
        /// <param name="cindex">控制语句索引</param>
        /// <param name="ctype">控制类型代码</param>
        /// <param name="lindex">控制管段的索引</param>
        /// <param name="setting">控制设置数值</param>
        /// <param name="nindex">控制节点的索引</param>
        /// <param name="level">对于基于时间控制的控制行为（以秒计）的控制水位或者水位控制的压力或者控制时间的数值</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENgetcontrol(int cindex, int* ctype, int* lindex, float* setting, int* nindex, float* level);

        /// <summary>
        /// 检索指定类型的管网组件数量。
        /// </summary>
        /// <param name="countcode">组件编号</param>
        /// <param name="count">管网中countcode组件的数量</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENgetcount(int countcode, int* count);

        /// <summary>
        /// 检索表明使用单位的代码数，为了表达所有流量。
        /// </summary>
        /// <param name="unitscode">流量范围代码号的数值</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENgetflowunits(int* unitscode);

        /// <summary>
        /// 检索指定分析时间参数的数值。
        /// </summary>
        /// <param name="paramcode">时间参数编号</param>
        /// <param name="timevalue">时间参数值，以秒计</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENgettimeparam(int paramcode, long* timevalue);

        /// <summary>
        /// 检索调用水质分析的类型。
        /// </summary>
        /// <param name="qualcode">水质分析代码</param>
        /// <param name="tracenode">源头跟踪分子中跟踪的节点索引</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENgetqualtype(int* qualcode, int* tracenode);

        /// <summary>
        /// 检索特定分析选项的数值。
        /// </summary>
        /// <param name="optioncode">选项编号</param>
        /// <param name="value">选项值</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENgetoption(int optioncode, float* value);
        #endregion

        #region 设置新的管网参数值
        /// <summary>
        /// 设置特定简单控制语句的参数。
        /// </summary>
        /// <param name="cindex">控制语句索引</param>
        /// <param name="ctype">控制类型代码</param>
        /// <param name="lindex">被控制管段的索引</param>
        /// <param name="setting">控制设置的数值</param>
        /// <param name="nindex">控制节点的索引</param>
        /// <param name="level">对于水位控制的控制水位或者压力的数值，或者控制行为的时间（以秒计），对于基于时间的控制</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENsetcontrol(int cindex, int ctype, int lindex, float setting, int nindex, float level);

        /// <summary>
        /// 设置特定节点的参数值。
        /// </summary>
        /// <param name="index">节点索引</param>
        /// <param name="code">参数代码</param>
        /// <param name="value">参数值</param>
        /// <returns>返回错误代号。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENsetnodevalue(int index, int code, float value);

        /// <summary>
        /// 设置指定管段的参数值。
        /// </summary>
        /// <param name="index">管段索引</param>
        /// <param name="paramcode">参数代码</param>
        /// <param name="value"参数值></param>
        /// <returns>返回错误代号。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENsetlinkvalue(int index, int paramcode, float value);

        /// <summary>
        /// 设置指定时间模式的所有乘子。
        /// </summary>
        /// <param name="index">时间模式索引</param>
        /// <param name="factors">整个模式的乘子</param>
        /// <param name="nfactors">模式中因子总数</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENsetpattern(int index, float* factors, int nfactors);

        /// <summary>
        /// 设置时间模式内特定时段的乘子因子。
        /// </summary>
        /// <param name="index">时间模式索引</param>
        /// <param name="period">时间模式内的时段</param>
        /// <param name="value">该时段的乘子因子</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENsetpatternvalue(int index, int period, float value);

        /// <summary>
        /// 设置被调用水质分析的类型。
        /// </summary>
        /// <param name="qualcode">水质分析代码</param>
        /// <param name="chemname">被分析化学成分名称</param>
        /// <param name="chemunits">化学成分计量单位</param>
        /// <param name="tracenode">源头跟踪分析中被跟踪节点的ID</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENsetqualtype(int qualcode, string chemname, string chemunits, string tracenode);

        /// <summary>
        /// 设置时间参数值。
        /// </summary>
        /// <param name="paramcode">时间参数代</param>
        /// <param name="timevalue">时间参数值，以秒计</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENsettimeparam(int paramcode, int timevalue);

        /// <summary>
        /// 设置特定分析选项的数值。
        /// </summary>
        /// <param name="optioncode">选项编号</param>
        /// <param name="value">选项值</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENsetoption(int optioncode, float value);
        #endregion

        #region 保存和使用水力分析结果文件
        /// <summary>
        /// 将当前二进制水力文件的内容保存到一个文件。
        /// </summary>
        /// <param name="fname">水力结果应被保存的文件名。</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENsavehydfile(string fname);

        /// <summary>
        /// 将指定文件的内容作为当前二进制水力文件。
        /// </summary>
        /// <param name="fname">包含了当前管网水力分析结果的文件名。</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENusehydfile(string fname);
        #endregion

        #region 执行水力分析
        /// <summary>
        /// 执行一次完整的水力模拟，所有时段的结果写入二进制水力文件。
        /// </summary>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENsolveH();

        /// <summary>
        /// 打开水力分析系统。
        /// </summary>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENopenH();

        /// <summary>
        /// 在执行水力分析之前，初始化蓄水池水位、管段状态和设置，以及模拟钟表时间。
        /// </summary>
        /// <param name="mode">0-1标志，说明水力结果是否保存到水力文件。</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENinitH(int mode);

        /// <summary>
        /// 执行简单时段水力分析，检索当前模拟钟表时间t。
        /// </summary>
        /// <param name="time">当前模拟钟表时间，以秒计</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENrunH(long* time);

        /// <summary>
        /// 确定延时模拟下一次水力时间发生前的时间长度。
        /// </summary>
        /// <param name="tstep">时间(以秒计)，直到下一次水力时间发生；或者为0，如果在模拟时段的末端。</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENnextH(long* tstep);

        /// <summary>
        /// 关闭水力分析系统，释放所有分配内存。
        /// </summary>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENcloseH();
        #endregion

        #region 执行水质分析
        /// <summary>
        /// 执行完整的水质模拟，结果具有均匀报告间隔，写入EPANETH二进制输出文件。
        /// </summary>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENsolveQ();

        /// <summary>
        /// 打开水质分析系统。
        /// </summary>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENopenQ();

        /// <summary>
        /// 在执行水质分析之前，初始化水质和模拟钟表时间。
        /// </summary>
        /// <param name="mode">0-1标志，说明分析结果是否以均匀报告时段保存到EPANETH二进制输出文件。</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENinitQ(int mode);

        /// <summary>
        /// 在下一时段水质分析的开始，使水力和水质分析结果可用，其中时段的开始返回为t。
        /// </summary>
        /// <param name="time">当前模拟钟表时间，以秒计。</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENrunQ(long* time);

        /// <summary>
        /// 水质模拟前进到下一水力时段的开始。
        /// </summary>
        /// <param name="tstep">时刻(以秒计)，直到下一水力事件发生；或者为零，如果在模拟时段的末端。</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENnextQ(long* tstep);

        /// <summary>
        /// 水质模拟前进一个水质时间步长。保留整个模拟中的时间范围在tleft中。
        /// </summary>
        /// <param name="tleft">保留在整个模拟历时中的秒。</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENstepQ(long* tleft);

        /// <summary>
        /// 关闭水质分析系统，释放所有分配的内存。
        /// </summary>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENcloseQ();
        #endregion

        #region 运行完整的“命令行”模拟
        /// <summary>
        /// 将水力模拟结果从二进制水力文件转换到二进制输出文件，其中结果仅仅以均匀时间间隔进行报告。
        /// </summary>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENsaveH();

        /// <summary>
        /// 将所有当前管网输入数据写入到文件，利用EPANETH输入文件的格式。
        /// </summary>
        /// <param name="fname">数据保存的文件名。</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENsaveinpfile(string fname);

        /// <summary>
        /// 将模拟结果的格式化文本报告写入到报告文件。
        /// </summary>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENreport();

        /// <summary>
        /// 清除任何出现在EPANETH输入文件[REPORT]节中的报告格式命令，或者利用ENsetreport函数所公布的。
        /// </summary>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENresetreport();

        /// <summary>
        /// 公布报告格式命令。格式命令与EPANETH输入文件的[REPORT]节中使用的相同。
        /// </summary>
        /// <param name="command">报告格式命令文本。</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENsetreport(string command);

        /// <summary>
        /// 设置水力状态报告的水平。
        /// </summary>
        /// <param name="statuslevel">状态报告的水平</param>
        /// <returns>返回错误代码。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENsetstatusreport(int statuslevel);

        /// <summary>
        /// 检索与特定错误或者警告代码相关的信息文本。
        /// </summary>
        /// <param name="errcode">错误或者警告代码</param>
        /// <param name="errmsg">对应于errcode的错误或者警告信息的文本</param>
        /// <param name="nchar">errmsg可以拥有的最大字符数</param>
        /// <returns>返回错误代号。</returns>
        [DllImport("epanet.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int ENgeterror(int errcode, string errmsg, int nchar);
        #endregion
        #endregion

        #region 平台调用EPANETH的方法
        #region 运行完整的“命令行”模拟
        /// <summary>
        /// 执行完整的EPANETH模拟。
        /// </summary>
        /// <param name="f1">输入文件名</param>
        /// <param name="f2">输出报告文件名</param>
        /// <param name="f3">可选二进制输出文件名</param>
        /// <param name="vfunc">将字符串作为参数的用户使用函数的指针。</param>
        /// <returns>返回错误代号。</returns>
        public int CENepanet(string f1, string f2, string f3, IntPtr vfunc)
        {
            int i_Errcode = 0;
            unsafe
            {
                i_Errcode = ENepanet(f1, f2, f3, vfunc);
            }
            return i_Errcode;
        }
        #endregion

        #region 将模拟结果写入到报告文件
        /// <summary>
        /// 将模拟结果写入到报告文件
        /// </summary>
        /// <returns></returns>
        public int CENreport()
        {
            int errcode = 0;
            unsafe
            {
                errcode = ENreport();
            }
            return errcode;
        }
        #endregion

        #region 打开和关闭EPANETH工具箱系统
        /// <summary>
        /// 使用ENopen函数打开工具箱系统，同时打开EPANETH输入文件。
        /// </summary>
        /// <param name="inp">EPANETH输入文件名</param>
        /// <param name="out1">输出报告文件名</param>
        /// <param name="out2">可选二进制输出文件名。</param>
        /// <returns>返回错误代码。</returns>
        public int CENopen(string inp, string out1, string out2)
        {
            int errcode = 0;
            unsafe
            {
                 errcode = ENopen(inp, out1, out2);
            }
            return errcode;
        }

        /// <summary>
        /// 关闭工具箱系统(包括所有正在处理的文件)。
        /// </summary>
        /// <returns>返回错误代码。</returns>
        public int CENclose()
        {
            int errcode = 0;
            unsafe
            {
                errcode = ENclose();
            }
            return errcode;
        }
        #endregion

        #region 检索管网节点信息

        /// <summary>
        /// 检索调用水质分析的类型。
        /// </summary>
        /// <param name="qualcode">水质分析代码</param>
        /// <param name="tracenode">源头跟踪分子中跟踪的节点索引，非trace时为0</param>
        /// <returns>返回错误代码。</returns>
        public int CENgetqualtype(ref int qualcode, ref int tracenode)
        {
            int errcode = 0;
            int _qualcode = qualcode;
            int _tracenode = tracenode;
            unsafe
            {
                errcode = ENgetqualtype(&_qualcode, &_tracenode);
                qualcode = _qualcode;
                tracenode = _tracenode;
            }
            return errcode;
        }


        /// <summary>
        /// 检索具有指定ID的节点索引。
        /// </summary>
        /// <param name="id">节点ID标签</param>
        /// <param name="index">节点索引</param>
        /// <returns>返回错误代码。</returns>
        public int CENgetnodeindex(string id, ref int index)
        {
            int errcode = 0;
            int _index = index;
            unsafe
            {
                errcode = ENgetnodeindex(id, &_index);
                index = _index;
            }
            return errcode;
        }

        /// <summary>
        /// 检索具有指定索引的ID标签。
        /// </summary>
        /// <param name="index">节点索引</param>
        /// <param name="id">节点的ID标签</param>
        /// <returns>返回错误代码。</returns>
        public int CENgetnodeid(int index, ref string value)
        {
            int errcode = 0;
            StringBuilder _value = new StringBuilder();

            value = "";
            unsafe
            {
                errcode = ENgetnodeid(index, _value);
                value = _value.ToString();
            }

            return errcode;
        }

        /// <summary>
        /// 检索指定节点的节点类型编号。
        /// </summary>
        /// <param name="index">节点索引</param>
        /// <param name="code">节点类型编号</param>
        /// <returns>返回错误代码。</returns>
        public int CENgetnodetype(int index, ref int type)
        {
            int errcode = 0;
            int _type = type;
            unsafe
            {
                errcode = ENgetnodetype(index, &_type);
                type = _type;
            }

            return errcode;
        }

        /// <summary>
        /// 检索特定节点的code类型的参数值。
        /// </summary>
        /// <param name="index">节点索引</param>
        /// <param name="code">参数代号</param>
        /// <param name="value">参数值</param>
        /// <returns>返回错误代号。</returns>
        public int CENgetnodevalue(int index, int code, ref float value)
        {
            int errcode = 0;
            float _value = value;
            unsafe
            {
                errcode = ENgetnodevalue(index, code, &_value);
                value = _value;
            }

            return errcode;
        }
        #endregion

        #region 检索管网管段信息
        /// <summary>
        /// 检索具有特定ID的管段索引。
        /// </summary>
        /// <param name="id">管段ID标签</param>
        /// <param name="index">管段索引</param>
        /// <returns>返回错误代码。</returns>
        public int CENgetlinkindex(string id, ref int index)
        {
            int errcode = 0;
            int _index = index;
            unsafe
            {
                errcode = ENgetlinkindex(id, &_index);
                index = _index;
            }
            return errcode;
        }

        /// <summary>
        /// 检索具有特定指标的管段的ID标签。
        /// </summary>
        /// <param name="index">管段索引</param>
        /// <param name="id">管段的ID标签</param>
        /// <returns>返回错误代码。</returns>
        public int CENgetlinkid(int index, ref string value)
        {
            int errcode = 0;
            StringBuilder _value = new StringBuilder();

            value = "";
            unsafe
            {
                errcode = ENgetlinkid(index, _value);
                value = _value.ToString();
            }

            return errcode;
        }

        /// <summary>
        /// 检索特定管段的类型编号。
        /// </summary>
        /// <param name="index">管段索引</param>
        /// <param name="code">管段类型代号</param>
        /// <returns>返回错误代码。</returns>
        public int CENgetlinktype(int index, ref int type)
        {
            int errcode = 0;
            int _type = type;
            unsafe
            {
                errcode = ENgetlinktype(index, &_type);
                type = _type;
            }

            return errcode;
        }

        /// <summary>
        /// 检索指定管段端点节点的索引。
        /// </summary>
        /// <param name="index">管段索引</param>
        /// <param name="node1">管段起始节点索引</param>
        /// <param name="node2">管段终止节点索引</param>
        /// <returns>返回错误代号。</returns>
        public int CENgetlinknodes(int index, ref int from, ref int to)
        {
            int errcode = 0;
            int _from = from;
            int _to = to;
            unsafe
            {
                errcode = ENgetlinknodes(index, &_from, &_to);
                from = _from;
                to = _to;
            }

            return errcode;
        }

        /// <summary>
        /// 检索指定管段参数值。
        /// </summary>
        /// <param name="index">管段索引</param>
        /// <param name="code">参数代号</param>
        /// <param name="value">参数值</param>
        /// <returns>返回错误代号。</returns>
        public int CENgetlinkvalue(int index, int code, ref float value)
        {
            int errcode = 0;
            float _value = value;
            unsafe
            {
                errcode = ENgetlinkvalue(index, code, &_value);
                value = _value;
            }

            return errcode;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CENopenH()
        {
            int errcode = ENopenH();
            return errcode;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public int CENinitH(int mode)
        {
            int errcode = ENinitH(mode);
            return errcode;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CENcloseH()
        {
            int errcode = ENcloseH();
            return errcode;
        }

        /// <summary>
        /// 计算单时段的平差
        /// </summary>
        /// <param name="t">本次计算的其实是件（秒）</param>
        /// <returns></returns>
        public int CENrunH(ref long t)
        {
            int errcode = 0;
            long _t = t;

            unsafe
            {
                errcode = ENrunH(&_t);
                t = _t;
            }

            return errcode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tstep"></param>
        /// <returns></returns>
        public int CENnextH(ref long tstep)
        {
            int errcode = 0;
            long _t = tstep;

            unsafe
            {
                errcode = ENnextH(&_t);
                tstep = _t;
            }

            return errcode;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CENsolveH()
        {
            return ENsolveH();
        }
        //
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CENopenQ()
        {
            int errcode = ENopenQ();
            return errcode;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public int CENinitQ(int mode)
        {
            int errcode = ENinitQ(mode);
            return errcode;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CENcloseQ()
        {
            int errcode = ENcloseQ();
            return errcode;
        }

        /// <summary>
        /// 计算水质分析
        /// </summary>
        /// <param name="t">当前模拟时钟时间，既下一时段的开始（秒）</param>
        /// <returns></returns>
        public int CENrunQ(ref long t)
        {
            int errcode = 0;
            long _t = t;

            unsafe
            {
                errcode = ENrunQ(&_t);
                t = _t;
            }

            return errcode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tstep"></param>
        /// <returns></returns>
        public int CENnextQ(ref long tstep)
        {
            int errcode = 0;
            long _t = tstep;

            unsafe
            {
                errcode = ENnextQ(&_t);
                tstep = _t;
            }

            return errcode;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tleft"></param>
        /// <returns></returns>
        public int CENstepQ(ref long tleft)
        {
            int errcode = 0;
            long _t = tleft;

            unsafe
            {
                errcode = ENstepQ(&_t);
                tleft = _t;
            }

            return errcode;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CENsolveQ()
        {
            return ENsolveQ();
        }

        public int CENsetnodevalue(int index, int code, float value)
        {
            int errcode = 0;
            errcode = ENsetnodevalue(index, code, value);
            return errcode;
        }

        public int CENgetcount(int countcode, ref int count)
        {
            int errcode = 0;
            int _count = count;
            unsafe
            {
                errcode = ENgetcount(countcode, &_count);
                count = _count;
            }

            return errcode;
        }

        public int CENsetqualtype(int qualcode, string chemname, string chemunits, string tracenode)
        {
            int errcode = 0;
            unsafe
            {
                errcode = ENsetqualtype(qualcode, chemname, chemunits, tracenode);
            }
            return errcode;
        }

        public int CENsetlinkvalue(int index, int paramcode, float value)
        {
            int errcode = 0;
            unsafe
            {
                errcode = ENsetlinkvalue(index, paramcode, value);
            }
            return errcode;
        }

        public int CENsettimeparam(int paramcode, int timevalue)
        {
            int errcode = 0;
            unsafe
            {
                errcode = ENsettimeparam(paramcode, timevalue);
            }
            return errcode;
        }

        public int CENsetcontrol(int cindex, int ctype, int lindex, float setting, int nindex, float level)
        {
            int errcode = 0;
            errcode = ENsetcontrol(cindex, ctype, lindex, setting, nindex, level);
            return errcode;
        }
        #endregion
    }
}