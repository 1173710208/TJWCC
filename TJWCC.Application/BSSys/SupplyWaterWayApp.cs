using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TJWCC.Application.WCC;
using TJWCC.Data;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Application.BSSys
{
    public class SupplyWaterWayApp
    {
        TJWCCDbContext db = new TJWCCDbContext();
        PIPEApp PIPE = new PIPEApp();
        PUMPApp PUMP = new PUMPApp();
        TCVApp TCV = new TCVApp();
        JUNCTIONApp JUNCTION = new JUNCTIONApp();
        RESERVOIRApp RESERVOIR = new RESERVOIRApp();
        RESULT_JUNCTIONApp RESULT_JUNCTION = new RESULT_JUNCTIONApp();
        RESULT_PIPEApp RESULT_PIPE = new RESULT_PIPEApp();
        RESULT_HYDRANTApp RESULT_HYDRANT = new RESULT_HYDRANTApp();
        RESULT_TCVApp RESULT_TCV = new RESULT_TCVApp();
        BSB_LinkedTableApp BSB_LinkedTable = new BSB_LinkedTableApp();
        MaterialClassApp MaterialClass = new MaterialClassApp();
        DistrictClassApp DistrictClass = new DistrictClassApp();
        DMAClassApp DMAClass = new DMAClassApp();
        DMAAreaClassApp DMAAreaClass = new DMAAreaClassApp();
        ZoneClassApp ZoneClass = new ZoneClassApp();
        BSB_BranchPipesApp BSB_BranchPipes = new BSB_BranchPipesApp();
        JUNCTION_VICEApp JUNCTION_VICE = new JUNCTION_VICEApp();
        BSB_SupplyWayApp BSB_SupplyWay = new BSB_SupplyWayApp();
        public ArrayList stationnode = new ArrayList();
        //List<TCV> valve = new List<TCV>();
        //List<PIPE> pipe = new List<PIPE>();
        //List<JUNCTION> junction = new List<JUNCTION>();

        public int lastpipeno = 0;
        public int m_nStationSum, donesum;
        //private int curTime = 1;

        public readonly int TYPE_JUNCTION = 55;
        public readonly int TYPE_TCV = 61;
        public readonly int TYPE_HYDRANT = 54;
        public readonly int TYPE_TANK = 52;
        public readonly int TYPE_PUMP = 68;
        public readonly int TYPE_PIPE = 69;
        public readonly int TYPE_RESERVIOR = 56;
        public readonly int TYPE_CHECKVALVE = 309;

        public SupplyWaterWayApp()
        {
            //valve = db.TCV.ToList();
            //pipe = db.PIPE.ToList();
            //junction = db.JUNCTION.ToList();
        }

        /// <summary>
        /// 获取水源个数
        /// </summary>
        private void getSourceInfo()
        {
            m_nStationSum = 0;

            List<PUMPEntity> tmpPump = PUMP.GetList();
            for (int i = 0; i < tmpPump.Count; i++)
            {
                ArrayList tmpresult = new ArrayList();
                tmpresult.Add((int)tmpPump[i].ElementId);//泵ID
                tmpresult.Add(tmpPump[i].ElementTypeId);//type
                tmpresult.Add(tmpPump[i].Label);
                stationnode.Add(tmpresult);
            }
            m_nStationSum += tmpPump.Count;

            List<RESERVOIREntity> tmpReservoir = RESERVOIR.GetList();
            for (int i = 0; i < tmpReservoir.Count; i++)
            {
                ArrayList tmpresult = new ArrayList();
                tmpresult.Add((int)tmpReservoir[i].ElementId);//水库ID
                tmpresult.Add(tmpReservoir[i].ElementTypeId);//type
                tmpresult.Add(tmpReservoir[i].Label);
                stationnode.Add(tmpresult);
            }
            m_nStationSum += tmpReservoir.Count;
        }

        /// <summary>
        /// 得到与某节点连接的所有管道和另一端的节点
        /// </summary>
        /// <param name="nodeno">要判断的节点ID</param>
        /// <param name="result">记录管道另一端的所有元素</param>
        /// <returns>所有元素个数</returns>
        private int GetLinkPipesAndNodes(int nodeno, ref ArrayList result)
        {
            result.Clear();

            int m_searchFromNode, m_searchToNode;
            int i = 0, j = 0;
            result = new ArrayList();
            m_searchFromNode = m_searchToNode = nodeno;

            //List<PIPE> tmpPipe = pipe.Where(item => item.StartNodeID == m_searchFromNode).ToList();
            List<PIPEEntity> tmpPipe = PIPE.GetStartList(m_searchFromNode).ToList();
            for (i = 0; i < tmpPipe.Count; i++)
            {
                int tmpId = (int)tmpPipe[i].EndNodeID;
                if (tmpId != nodeno)
                {
                    ArrayList tmpresult = new ArrayList();
                    tmpresult.Add((int)tmpPipe[i].ElementId);//管道ID
                    tmpresult.Add((int)tmpPipe[i].EndNodeID);//另一端节点ID
                    tmpresult.Add(tmpPipe[i].EndNodeType);//另一端元素类型
                    result.Add(tmpresult);
                }
            }
            //tmpPipe = pipe.Where(item => item.EndNodeID == m_searchToNode).ToList();
            tmpPipe = PIPE.GetEndList(m_searchToNode).ToList();
            for (j = 0; j < tmpPipe.Count; j++)
            {
                int tmpId = (int)tmpPipe[j].StartNodeID;
                if (tmpId != nodeno)
                {
                    ArrayList tmpresult = new ArrayList();
                    tmpresult.Add((int)tmpPipe[j].ElementId);//管道ID
                    tmpresult.Add((int)tmpPipe[j].StartNodeID);//另一端节点ID
                    tmpresult.Add(tmpPipe[j].StartNodeType);//另一端元素类型
                    result.Add(tmpresult);
                }
            }
            return i + j;
        }

        /// <summary>
        /// 查找管道流量和上游的连接元素ID
        /// </summary>
        /// <param name="pipeid">管道ID</param>
        /// <param name="curTime">流量时间点(0-23)</param>
        /// <returns>管道流量和上游的连接元素ID</returns>
        private ArrayList Pipe_Q(int pipeid, int curTime)
        {
            ArrayList result = new ArrayList();
            double Result_Flow = 0;
            bool isNullFlow = false;
            int fromnode = -1;
            int fromtype = 0;

            //List<PIPE> tmpPipe = pipe.Where(item => item.ElementId == pipeid).ToList();
            PIPEEntity tmpPipe = PIPE.GetList(pipeid).FirstOrDefault();
            RESULT_PIPEEntity resultPipe = RESULT_PIPE.GetList(pipeid).FirstOrDefault();
            if (tmpPipe != null && resultPipe != null)
            {
                switch (curTime)
                {
                    case 0:
                        if (resultPipe.Result_Flow_0 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe.Result_Flow_0.ToString());
                        break;
                    case 1:
                        if (resultPipe.Result_Flow_1 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe.Result_Flow_1.ToString());
                        break;
                    case 2:
                        if (resultPipe.Result_Flow_2 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe.Result_Flow_2.ToString());
                        break;
                    case 3:
                        if (resultPipe.Result_Flow_3 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe.Result_Flow_3.ToString());
                        break;
                    case 4:
                        if (resultPipe.Result_Flow_4 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe.Result_Flow_4.ToString());
                        break;
                    case 5:
                        if (resultPipe.Result_Flow_5 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe.Result_Flow_5.ToString());
                        break;
                    case 6:
                        if (resultPipe.Result_Flow_6 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe.Result_Flow_6.ToString());
                        break;
                    case 7:
                        if (resultPipe.Result_Flow_7 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe.Result_Flow_7.ToString());
                        break;
                    case 8:
                        if (resultPipe.Result_Flow_8 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe.Result_Flow_8.ToString());
                        break;
                    case 9:
                        if (resultPipe.Result_Flow_9 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe.Result_Flow_9.ToString());
                        break;
                    case 10:
                        if (resultPipe.Result_Flow_10 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe.Result_Flow_10.ToString());
                        break;
                    case 11:
                        if (resultPipe.Result_Flow_11 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe.Result_Flow_11.ToString());
                        break;
                    case 12:
                        if (resultPipe.Result_Flow_12 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe.Result_Flow_12.ToString());
                        break;
                    case 13:
                        if (resultPipe.Result_Flow_13 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe.Result_Flow_13.ToString());
                        break;
                    case 14:
                        if (resultPipe.Result_Flow_14 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe.Result_Flow_14.ToString());
                        break;
                    case 15:
                        if (resultPipe.Result_Flow_15 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe.Result_Flow_15.ToString());
                        break;
                    case 16:
                        if (resultPipe.Result_Flow_16 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe.Result_Flow_16.ToString());
                        break;
                    case 17:
                        if (resultPipe.Result_Flow_17 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe.Result_Flow_17.ToString());
                        break;
                    case 18:
                        if (resultPipe.Result_Flow_18 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe.Result_Flow_18.ToString());
                        break;
                    case 19:
                        if (resultPipe.Result_Flow_19 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe.Result_Flow_19.ToString());
                        break;
                    case 20:
                        if (resultPipe.Result_Flow_20 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe.Result_Flow_20.ToString());
                        break;
                    case 21:
                        if (resultPipe.Result_Flow_21 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe.Result_Flow_21.ToString());
                        break;
                    case 22:
                        if (resultPipe.Result_Flow_22 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe.Result_Flow_22.ToString());
                        break;
                    case 23:
                        if (resultPipe.Result_Flow_23 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe.Result_Flow_23.ToString());
                        break;
                    case 24:
                        if (resultPipe.Result_Flow_24 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe.Result_Flow_24.ToString());
                        break;
                }
                if (isNullFlow)
                {
                    fromtype = tmpPipe.StartNodeType == null ? 0 : (int)tmpPipe.StartNodeType;
                    if (fromtype == TYPE_RESERVIOR || fromtype == TYPE_PUMP || fromtype == TYPE_TANK)
                    {
                        fromnode = tmpPipe.StartNodeID == null ? 0 : tmpPipe.StartNodeID.Value;
                    }
                    else
                    {
                        fromnode = tmpPipe.EndNodeID == null ? 0 : (int)tmpPipe.EndNodeID;
                        fromtype = tmpPipe.EndNodeType == null ? 0 : (int)tmpPipe.EndNodeType;
                    }
                    result.Add(0);
                }
                else
                {
                    if (Result_Flow > 0)
                    {
                        fromnode = tmpPipe.StartNodeID == null ? 0 : (int)tmpPipe.StartNodeID;
                        fromtype = tmpPipe.StartNodeType == null ? 0 : (int)tmpPipe.StartNodeType;
                        result.Add(Result_Flow);
                    }
                    else if (Result_Flow < 0)
                    {
                        fromnode = tmpPipe.EndNodeID == null ? 0 : (int)tmpPipe.EndNodeID;
                        fromtype = tmpPipe.EndNodeType == null ? 0 : (int)tmpPipe.EndNodeType;
                        result.Add(Math.Abs(Result_Flow));
                    }
                    else
                    {
                        fromtype = tmpPipe.StartNodeType == null ? 0 : (int)tmpPipe.StartNodeType;
                        if (fromtype == TYPE_RESERVIOR || fromtype == TYPE_PUMP || fromtype == TYPE_TANK)
                        {
                            fromnode = tmpPipe.StartNodeID == null ? 0 : (int)tmpPipe.StartNodeID;
                        }
                        else
                        {
                            fromnode = tmpPipe.EndNodeID == null ? 0 : (int)tmpPipe.EndNodeID;
                            fromtype = tmpPipe.EndNodeType == null ? 0 : (int)tmpPipe.EndNodeType;
                        }
                        result.Add(Result_Flow);
                    }
                }
            }

            result.Add(fromnode);
            result.Add(fromtype);

            return result;
        }

        /// <summary>
        /// 查找下一个节点
        /// </summary>
        /// <param name="node">元素ID</param>
        /// <param name="type">元素表名</param>
        /// <param name="findresult">查找结果</param>
        /// <param name="curTime">数据时间点</param>
        /// <returns>是否源头元素</returns>
        public bool NextNode(int node, int type, ref ArrayList findresult, int curTime)
        {
            int maxnode, fromnode = 0, maxfromnode = 0, maxpipe;
            int fromtype = 0, maxformtype = 0, maxnodetype;
            double maxq;
            double Result_Flow = 0;

            bool last = true;

            ArrayList wnwPipe = new ArrayList();
            int links = GetLinkPipesAndNodes(node, ref wnwPipe);

            maxq = double.MinValue;//初始化本次最大流量
            maxpipe = 0;
            maxnode = node;//初始化本次流量最大管道元素ID
            maxnodetype = type;
            bool found = false;

            for (int i = 0; i < links; i++)
            {
                ArrayList resultq = Pipe_Q((int)(wnwPipe[i] as ArrayList)[0], curTime);//根据管道ID查找管道流量和上游连接元素
                if (resultq.Count > 0)
                {
                    Result_Flow = Convert.ToDouble(resultq[0]);
                    fromnode = (int)resultq[1];
                    fromtype = (int)resultq[2];
                }

                if ((Result_Flow > 0 && Result_Flow > maxq) && (node != fromnode || type != fromtype) && ((int)(wnwPipe[i] as ArrayList)[0] != lastpipeno))
                {
                    last = false;
                    maxq = Result_Flow;
                    lastpipeno = maxpipe = (int)(wnwPipe[i] as ArrayList)[0];//全局流量最大和本次流量最大管道ID
                    maxnode = (int)(wnwPipe[i] as ArrayList)[1];//管道连接元素ID
                    maxnodetype = (int)(wnwPipe[i] as ArrayList)[2];//管道连接元素类型

                    maxfromnode = fromnode;
                    maxformtype = fromtype;

                    found = true;
                }
            }

            if (found)
            {
                findresult.Add(maxnode);
                findresult.Add(maxnodetype);
                findresult.Add(maxpipe);
                findresult.Add(maxfromnode);
                findresult.Add(maxformtype);
            }
            else return true;
            return last;
        }

        /// <summary>
        /// 查询供水路径
        /// </summary>
        /// <param name="node">元素ID</param>
        /// <param name="type">元素表名</param>
        /// <param name="swwType">是否供水路径数据计算</param>
        /// <param name="curTime">时间</param>
        /// <returns>供水路径</returns>
        public ArrayList FindSupplyway(int node, int type, bool swwType, int curTime)
        {
            //getSourceInfo();//记录所有水源

            ArrayList result = new ArrayList();
            ArrayList PipeNoArr_pipeno = new ArrayList();//供水路径途径管道集合
            ArrayList PipeNoArr_node = new ArrayList();//供水路径途径元素集合（不包括含初始元素）
            ArrayList PipeNoArr_nodetype = new ArrayList();//供水路径途径元素类型集合
            ArrayList NodeResult = new ArrayList();//供水路径途径元素类型集合
            int findnode, findtype;
            bool finish;
            lastpipeno = 0;
            ArrayList NArr = new ArrayList();
            ArrayList UArr = new ArrayList();
            string sourceName = "";
            if (type == TYPE_PIPE)//如果选择的是管道元素，则要进行预处理来确定从管道的哪端进行供水路径计算
            {
                PIPEEntity tmpNode = PIPE.GetList(node).FirstOrDefault();
                int tmpNodeST = tmpNode.StartNodeType.Value;
                if(tmpNodeST != TYPE_PUMP && tmpNodeST != TYPE_TANK && tmpNodeST != TYPE_RESERVIOR)
                {
                    ArrayList foundresult = new ArrayList();
                    finish = NextNode(tmpNode.StartNodeID.Value, tmpNodeST, ref foundresult, curTime);//查找curTime时刻当前元素上游元素

                    findnode = (int)foundresult[0];
                    findtype = (int)foundresult[1];
                    if(findnode== tmpNode.EndNodeID)//如果找到的元素就是上游元素，就对本端进行供水路径计算
                    {
                        node = tmpNode.StartNodeID.Value;
                        type = tmpNodeST;
                    }
                    else
                    {
                        PipeNoArr_pipeno.Add(node);
                        node = tmpNode.EndNodeID.Value;
                        type = tmpNode.EndNodeType.Value;
                    }
                }
                lastpipeno = 0;
            }
            if (type == TYPE_PUMP || type == TYPE_TANK || type == TYPE_RESERVIOR)//首先查看初始元素是否是水源
            {

                result.Add(PipeNoArr_pipeno); //供水路径途径管道集合
                result.Add(PipeNoArr_node);//供水路径途径元素集合（不包括含初始元素）
                result.Add(PipeNoArr_nodetype);//供水路径途径元素类型集合
                result.Add(NodeResult);
                result.Add(NodeResult);
                result.Add(0);//pipesum途径管道数量
                result.Add(0);//findnode水源ID
                RESERVOIREntity reservoir = RESERVOIR.GetList(node).FirstOrDefault();
                if (reservoir != null)
                    result.Add(RESERVOIR.GetList(node).FirstOrDefault().Label);//水源label
                result.Add(type);//findtype水源类型
                result.Add(NodeResult);//最大值
                result.Add(NodeResult);//最小值
                result.Add(NodeResult);//水龄
                return result;
            }

            //检查是否已预存供水路径
            BSB_SupplyWayEntity bsbSupplyWay = BSB_SupplyWay.GetList(node, type).FirstOrDefault();
            if (bsbSupplyWay != null)
            {
                string[] tmpPipeNoArr = bsbSupplyWay.SupplyWayNode.Split(',');
                PipeNoArr_pipeno = new ArrayList(bsbSupplyWay.SupplyWayPipe.Split(','));
                PipeNoArr_pipeno.Reverse();//倒序计算结果使水源在前
                for (int i = tmpPipeNoArr.Count()-1; i >= 0; i--)
                {
                    string[] tmp = tmpPipeNoArr[i].Split('|');
                    PipeNoArr_node.Add(tmp[0]);
                    PipeNoArr_nodetype.Add(tmp[1]);
                }
                if(swwType)
                    NodeResult = GetNodesResult(PipeNoArr_node, PipeNoArr_nodetype, curTime);
                else
                {
                    NodeResult.Add(0);
                    NodeResult.Add(0);
                    NodeResult.Add(0);
                    NodeResult.Add(0);
                    NodeResult.Add(0);
                }
                findnode = Convert.ToInt32(PipeNoArr_node[0]);
                findtype = Convert.ToInt32(PipeNoArr_nodetype[0]);
                result.Add(PipeNoArr_pipeno); //供水路径途径管道集合
                result.Add(PipeNoArr_node);//供水路径途径元素集合（不包括含初始元素）
                result.Add(PipeNoArr_nodetype);//供水路径途径元素类型集合
                result.Add(NodeResult[0]);//途径元素的高程数据
                result.Add(NodeResult[1]);//途径元素的水力坡度数据
                result.Add(PipeNoArr_pipeno.Count);//pipesum途径管道数量
                result.Add(findnode);//findnode水源ID
                if (findtype == TYPE_PUMP)
                {
                    PUMPEntity pump = PUMP.GetList(findnode).FirstOrDefault();
                    if (pump != null)
                        sourceName = pump.Label;
                }
                if (findtype == TYPE_RESERVIOR)
                {
                    RESERVOIREntity reservoir = RESERVOIR.GetList(findnode).FirstOrDefault();
                    if (reservoir != null)
                        sourceName = reservoir.Label;
                }
                result.Add(sourceName);//水源label名称
                result.Add(findtype);//水源类型
                result.Add(NodeResult[2]);//最大值
                result.Add(NodeResult[3]);//最小值
                result.Add(NodeResult[4]);//水龄
                return result;
            }
            int n = 0;
            findnode = node;
            findtype = type;
            NArr.Add(node);
            UArr.Add(type);

            bool bCircle = false;

            do
            {
                int tmpnode = findnode;
                int tmptype = findtype;
                ArrayList foundresult = new ArrayList();
                finish = NextNode(tmpnode, tmptype, ref foundresult, curTime);//查找curTime时刻当前元素上游元素

                //if (finish && foundresult.Count < 1) return result; //判断是否查找完成或者是否找到当前元素上游元素

                if (foundresult.Count > 0)
                {
                    findnode = (int)foundresult[0];
                    findtype = (int)foundresult[1];

                    for (int i = 0; i < n; i++) bCircle = bCircle || ((int)NArr[i] == findnode && (int)UArr[i] == findtype);

                    PipeNoArr_pipeno.Add(foundresult[2].ToString());
                    PipeNoArr_node.Add(foundresult[3].ToString());
                    PipeNoArr_nodetype.Add(foundresult[4].ToString());
                    n++;
                }
                //if (finish || bCircle) break;

                NArr.Add(findnode);
                UArr.Add(findtype);

                if (findtype == TYPE_PUMP || findtype == TYPE_TANK || findtype == TYPE_RESERVIOR)
                {
                    finish = true;
                    if (findtype == TYPE_PUMP)
                    {
                        PUMPEntity pump = PUMP.GetList(findnode).FirstOrDefault();
                        if (pump != null)
                            sourceName = pump.Label;
                    }
                    if (findtype == TYPE_RESERVIOR)
                    {
                        RESERVOIREntity reservoir = RESERVOIR.GetList(findnode).FirstOrDefault();
                        if (reservoir != null)
                            sourceName = reservoir.Label;
                    }
                }
            } while (!finish && !bCircle);
            PipeNoArr_pipeno.Reverse();//倒序计算结果使水源在前
            PipeNoArr_node.Reverse();//倒序计算结果使水源在前
            PipeNoArr_nodetype.Reverse();//倒序计算结果使水源在前
            if (swwType)
                NodeResult = GetNodesResult(PipeNoArr_node, PipeNoArr_nodetype, curTime);
            else
            {
                NodeResult.Add(0);
                NodeResult.Add(0);
                NodeResult.Add(0);
                NodeResult.Add(0);
                NodeResult.Add(0);
            }

            result.Add(PipeNoArr_pipeno); //供水路径途径管道集合
            result.Add(PipeNoArr_node);//供水路径途径元素集合（不包括含初始元素）
            result.Add(PipeNoArr_nodetype);//供水路径途径元素类型集合
            result.Add(NodeResult[0]);//途径元素的高程数据
            result.Add(NodeResult[1]);//途径元素的水力坡度数据
            result.Add(n);//途径管道数量
            result.Add(findnode);//水源ID
            result.Add(sourceName);//水源label名称
            result.Add(findtype);//水源类型
            result.Add(NodeResult[2]);//最大值
            result.Add(NodeResult[3]);//最小值
            result.Add(NodeResult[4]);//水龄

            return result;
        }

        /// <summary>
        /// 获取节点数据
        /// </summary>
        /// <param name="nodeIndexs">路径节点列表</param>
        /// <param name="nodeTypes">路径节点类型</param>
        /// <param name="curTime">数据时间点</param>
        /// <returns>节点数据</returns>
        private ArrayList GetNodesResult(ArrayList nodeIndexs, ArrayList nodeTypes, int curTime)
        {
            ArrayList result = new ArrayList();
            ArrayList PressArr = new ArrayList();//压力数据
            ArrayList HGLArr = new ArrayList();//总压力数据
            decimal maxValue = decimal.MinValue;//最大值
            decimal minValue = decimal.MaxValue;//最小值
            string[] tmpNodeIndexs = (string[])nodeIndexs.ToArray(typeof(string));
            string[] tmpNodeTypes = (string[])nodeTypes.ToArray(typeof(string));
            var tmpNodeIds = tmpNodeIndexs.Select(s => int.Parse(s)).ToArray();
            //List<JUNCTION> tmpJunction = junction.Where(item => item.ElementId == ElementId).ToList();
            //List<JUNCTION> tmpJunction = db.JUNCTION.Where(item => tmpNodeIds.Contains(item.ElementId.Value)).ToList();
            List<RESULT_JUNCTIONEntity> resJunction = RESULT_JUNCTION.GetList(tmpNodeIds).ToList();
            //List<HYDRANT> tmpHydrant = db.HYDRANT.Where(item => tmpNodeIds.Contains(item.ElementId.Value)).ToList();
            List<RESULT_HYDRANTEntity> resHydrant = RESULT_HYDRANT.GetList(tmpNodeIds).ToList();
            //List<TCV> tmpValve = db.TCV.Where(item => tmpNodeIds.Contains(item.ElementId.Value)).ToList();
            List<RESULT_TCVEntity> resValve = RESULT_TCV.GetList(tmpNodeIds).ToList();
            List<RESERVOIREntity> tmpReservoir = RESERVOIR.GetList(tmpNodeIds).ToList();
            for (int i = 0; i < nodeIndexs.Count; i++)
            {
                int ElementId = Convert.ToInt32(nodeIndexs[i]);
                if (tmpReservoir != null)
                {
                    for (int x = 0; x < tmpReservoir.Count; x++)
                    {
                        if (ElementId == tmpReservoir[x].ElementId)
                        {
                            double tmpElevArr = Convert.ToDouble(tmpReservoir[x].Physical_Elevation.Value);
                            //DateTime? tmpdt = BSOT_CalDefaultInfo.Select(item => item.CurrentDate).FirstOrDefault();
                            //int tmpHour = tmpdt == null ? DateTime.Now.Hour : tmpdt.Value.Hour;
                            //int tmpPattern = tmpReservoir[x].Physical_HGLPattern.Value;
                            //var tmpHGL = (from p in db.Patterns
                            //              where p.Pattern_DefinitionID == tmpPattern && p.Physical_Hours == 24
                            //              select new
                            //              {
                            //                  HGL = p.Physical_Multiplier.Value * tmpElevArr
                            //              }).FirstOrDefault();
                            //decimal press = db.Database.SqlQuery<decimal>("SELECT AVG(Tag_value)*100 Tag_value FROM [dbo].[BS_SCADA_TAG_CURRENT] WHERE Station_Key in(SELECT Station_Key FROM BSM_Meter_Info WHERE Station_Unit='"
                            //    + ElementId + "' AND Tag_key='2' AND Tag_value>0.1) GROUP BY Tag_key").FirstOrDefault();
                            //decimal press = Convert.ToDecimal(tmpHGL.HGL);
                            ////tmpElevArr += press;
                            //if (press > maxValue) maxValue = press;
                            //if (press < minValue) minValue = press;
                            //if (tmpElevArr > maxValue) maxValue = tmpElevArr;
                            //if (tmpElevArr < minValue) minValue = tmpElevArr;
                            //PressArr.Add(Math.Round(Convert.ToDouble(press), 2));//自由水压数据
                            //HGLArr.Add(Math.Round(Convert.ToDouble(press), 2));
                        }
                    }
                }
                for (int x = 0; x < resJunction.Count; x++)
                {
                    if (ElementId == resJunction[x].ElementId)
                    {
                        decimal tmppress = resJunction[x].Result_Pressure_24 == null ? 0 : resJunction[x].Result_Pressure_24.Value * 100;
                        decimal tmphyd = resJunction[x].Result_HydraulicGrade_24 == null ? 0 : resJunction[x].Result_HydraulicGrade_24.Value * 100;
                        if (tmppress > maxValue) maxValue = tmppress;
                        if (tmppress < minValue) minValue = tmppress;
                        if (tmphyd > maxValue) maxValue = tmphyd;
                        if (tmphyd < minValue) minValue = tmphyd;
                        PressArr.Add(Math.Round(Convert.ToDouble(tmppress), 2));//压力数据
                        HGLArr.Add(Math.Round(Convert.ToDouble(tmphyd), 2));
                    }

                }

                for (int x = 0; x < resHydrant.Count; x++)
                {
                    if (ElementId == resHydrant[x].ElementId)
                    {
                        decimal tmppress = resHydrant[x].Result_Pressure_24 == null ? 0 : resHydrant[x].Result_Pressure_24.Value * 100;
                        decimal tmphyd = resHydrant[x].Result_HydraulicGrade_24 == null ? 0 : resHydrant[x].Result_HydraulicGrade_24.Value * 100;
                        if (tmppress > maxValue) maxValue = tmppress;
                        if (tmppress < minValue) minValue = tmppress;
                        if (tmphyd > maxValue) maxValue = tmphyd;
                        if (tmphyd < minValue) minValue = tmphyd;
                        PressArr.Add(Math.Round(Convert.ToDouble(tmppress), 2));//压力数据
                        HGLArr.Add(Math.Round(Convert.ToDouble(tmphyd), 2));
                    }
                }
                for (int x = 0; x < resValve.Count; x++)
                {
                    if (ElementId == resValve[x].ElementId)
                    {
                        decimal tmpFrom = resValve[x].Result_FromPressure_24 == null ? 0 : (decimal)resValve[x].Result_FromPressure_24 * 100;
                        decimal tmpTo = resValve[x].Result_ToPressure_24 == null ? 0 : (decimal)resValve[x].Result_ToPressure_24 * 100;
                        if ((tmpFrom + tmpTo) / 2 > maxValue) maxValue = (tmpFrom + tmpTo) / 2;
                        if ((tmpFrom + tmpTo) / 2 < minValue) minValue = (tmpFrom + tmpTo) / 2;
                        PressArr.Add(Math.Round((tmpFrom + tmpTo) / 2, 2));//压力数据
                        tmpFrom = resValve[x].Result_FromHead_24 == null ? 0 : (decimal)resValve[x].Result_FromHead_24 * 100;
                        tmpTo = resValve[x].Result_ToHead_24 == null ? 0 : (decimal)resValve[x].Result_ToHead_24 * 100;
                        if ((tmpFrom + tmpTo) / 2 > maxValue) maxValue = (tmpFrom + tmpTo) / 2;
                        if ((tmpFrom + tmpTo) / 2 < minValue) minValue = (tmpFrom + tmpTo) / 2;
                        HGLArr.Add(Math.Round((tmpFrom + tmpTo) / 2, 2));
                    }
                }
            }
            decimal age = db.Database.SqlQuery<decimal>("SELECT max(Result_Age_7) FROM RESULT_JUNCTION_TJWCC WHERE ElementId IN(" + string.Join(",", tmpNodeIndexs) + ")").FirstOrDefault();
            result.Add(PressArr);//压力数据
            result.Add(HGLArr);//总压力数据
            result.Add(maxValue);//最大值
            result.Add(minValue);//最小值
            result.Add(age);//水龄

            return result;
        }
    }
}