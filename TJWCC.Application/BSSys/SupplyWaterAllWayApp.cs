using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using TJWCC.Application.WCC;
using TJWCC.Data;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Application.BSSys
{
    public class SupplyWaterAllWayApp
    {
        TJWCCDbContext db = new TJWCCDbContext();

        RESULT_PIPEApp RESULT_PIPE = new RESULT_PIPEApp();
        BSB_SupplyWayApp BSB_SupplyWay = new BSB_SupplyWayApp();
        public ArrayList donearr_nodeno = new ArrayList();
        public ArrayList donearr_nodeunit = new ArrayList();
        public ArrayList donearr_stationno = new ArrayList();
        public ArrayList donearr_stationunit = new ArrayList();
        List<PIPEEntity> pipe = new List<PIPEEntity>();
        List<PIPEEntity> tmpPipe_GLPAN = new List<PIPEEntity>();
        PIPEEntity tmpPipe_Q = new PIPEEntity();
        List<RESULT_PIPEEntity> resultPipe_Q = new List<RESULT_PIPEEntity>();

        public readonly int TYPE_JUNCTION = 55;
        public readonly int TYPE_TCV = 61;
        public readonly int TYPE_HYDRANT = 54;
        public readonly int TYPE_TANK = 52;
        public readonly int TYPE_PUMP = 68;
        public readonly int TYPE_PIPE = 69;
        public readonly int TYPE_RESERVIOR = 56;
        public readonly int TYPE_CHECKVALVE = 309;

        public SupplyWaterAllWayApp()
        {
            pipe = new PIPEApp().GetList();
        }

        /// <summary>
        /// 管道的另一端所有元素
        /// </summary>
        /// <param name="pnodeId">已搜索到元素ID</param>
        /// <param name="nodeId">管道一端元素ID</param>
        /// <param name="result">记录管道另一端的元素</param>
        /// <returns>所有元素个数</returns>
        public int GetLinkPipesAndNodes(int pnodeId, int nodeId, ref ArrayList result)
        {
            result.Clear();
            //result = new ArrayList();

            tmpPipe_GLPAN = pipe.Where(item => item.StartNodeID == nodeId).ToList();
            for (int i = 0; i < tmpPipe_GLPAN.Count; i++)
            {
                int tmpId = (int)tmpPipe_GLPAN[i].EndNodeID;
                if (tmpId != pnodeId)
                {
                    ArrayList tmpresult = new ArrayList();
                    tmpresult.Add((int)tmpPipe_GLPAN[i].ElementId);//管道ID
                    tmpresult.Add((int)tmpPipe_GLPAN[i].EndNodeID);
                    tmpresult.Add(tmpPipe_GLPAN[i].EndNodeType);
                    result.Add(tmpresult);
                }
            }
            tmpPipe_GLPAN = pipe.Where(item => item.EndNodeID == nodeId).ToList();
            for (int j = 0; j < tmpPipe_GLPAN.Count; j++)
            {
                int tmpId = (int)tmpPipe_GLPAN[j].StartNodeID;
                if (tmpId != pnodeId)
                {
                    ArrayList tmpresult = new ArrayList();
                    tmpresult.Add((int)tmpPipe_GLPAN[j].ElementId);//管道ID
                    tmpresult.Add((int)tmpPipe_GLPAN[j].StartNodeID);
                    tmpresult.Add(tmpPipe_GLPAN[j].StartNodeType);
                    result.Add(tmpresult);
                }
            }
            return result.Count;
        }

        /// <summary>
        /// 查找管道流量和上游的连接元素ID
        /// </summary>
        /// <param name="pipeid">管道ID</param>
        /// <param name="curTime">流量时间点(0-23)</param>
        /// <returns>管道流量和上游的连接元素ID</returns>
        public ArrayList Pipe_Q(int pipeid, int curTime)
        {
            ArrayList result = new ArrayList();
            double Result_Flow = 0;
            bool isNullFlow = false;
            int fromnode = -1;
            int fromtype = 0;

            tmpPipe_Q = pipe.Where(item => item.ElementId == pipeid).FirstOrDefault();
            resultPipe_Q = RESULT_PIPE.GetList(pipeid);
            if (tmpPipe_Q != null && resultPipe_Q.Count > 0)
            {
                switch (curTime)
                {
                    case 0:
                        if (resultPipe_Q[0].Result_Flow_0 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe_Q[0].Result_Flow_0.ToString());
                        break;
                    case 1:
                        if (resultPipe_Q[0].Result_Flow_1 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe_Q[0].Result_Flow_1.ToString());
                        break;
                    case 2:
                        if (resultPipe_Q[0].Result_Flow_2 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe_Q[0].Result_Flow_2.ToString());
                        break;
                    case 3:
                        if (resultPipe_Q[0].Result_Flow_3 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe_Q[0].Result_Flow_3.ToString());
                        break;
                    case 4:
                        if (resultPipe_Q[0].Result_Flow_4 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe_Q[0].Result_Flow_4.ToString());
                        break;
                    case 5:
                        if (resultPipe_Q[0].Result_Flow_5 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe_Q[0].Result_Flow_5.ToString());
                        break;
                    case 6:
                        if (resultPipe_Q[0].Result_Flow_6 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe_Q[0].Result_Flow_6.ToString());
                        break;
                    case 7:
                        if (resultPipe_Q[0].Result_Flow_7 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe_Q[0].Result_Flow_7.ToString());
                        break;
                    case 8:
                        if (resultPipe_Q[0].Result_Flow_8 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe_Q[0].Result_Flow_8.ToString());
                        break;
                    case 9:
                        if (resultPipe_Q[0].Result_Flow_9 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe_Q[0].Result_Flow_9.ToString());
                        break;
                    case 10:
                        if (resultPipe_Q[0].Result_Flow_10 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe_Q[0].Result_Flow_10.ToString());
                        break;
                    case 11:
                        if (resultPipe_Q[0].Result_Flow_11 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe_Q[0].Result_Flow_11.ToString());
                        break;
                    case 12:
                        if (resultPipe_Q[0].Result_Flow_12 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe_Q[0].Result_Flow_12.ToString());
                        break;
                    case 13:
                        if (resultPipe_Q[0].Result_Flow_13 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe_Q[0].Result_Flow_13.ToString());
                        break;
                    case 14:
                        if (resultPipe_Q[0].Result_Flow_14 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe_Q[0].Result_Flow_14.ToString());
                        break;
                    case 15:
                        if (resultPipe_Q[0].Result_Flow_15 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe_Q[0].Result_Flow_15.ToString());
                        break;
                    case 16:
                        if (resultPipe_Q[0].Result_Flow_16 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe_Q[0].Result_Flow_16.ToString());
                        break;
                    case 17:
                        if (resultPipe_Q[0].Result_Flow_17 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe_Q[0].Result_Flow_17.ToString());
                        break;
                    case 18:
                        if (resultPipe_Q[0].Result_Flow_18 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe_Q[0].Result_Flow_18.ToString());
                        break;
                    case 19:
                        if (resultPipe_Q[0].Result_Flow_19 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe_Q[0].Result_Flow_19.ToString());
                        break;
                    case 20:
                        if (resultPipe_Q[0].Result_Flow_20 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe_Q[0].Result_Flow_20.ToString());
                        break;
                    case 21:
                        if (resultPipe_Q[0].Result_Flow_21 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe_Q[0].Result_Flow_21.ToString());
                        break;
                    case 22:
                        if (resultPipe_Q[0].Result_Flow_22 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe_Q[0].Result_Flow_22.ToString());
                        break;
                    case 23:
                        if (resultPipe_Q[0].Result_Flow_23 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe_Q[0].Result_Flow_23.ToString());
                        break;
                    case 24:
                        if (resultPipe_Q[0].Result_Flow_24 == null) isNullFlow = true; else Result_Flow = double.Parse(resultPipe_Q[0].Result_Flow_24.ToString());
                        break;
                }
                if (isNullFlow)
                {
                    fromtype = tmpPipe_Q.StartNodeType == null ? 0 : (int)tmpPipe_Q.StartNodeType;
                    if (fromtype == TYPE_RESERVIOR || fromtype == TYPE_PUMP || fromtype == TYPE_TANK)
                    {
                        fromnode = tmpPipe_Q.StartNodeID == null ? 0 : (int)tmpPipe_Q.StartNodeID;
                    }
                    else
                    {
                        fromnode = tmpPipe_Q.EndNodeID == null ? 0 : (int)tmpPipe_Q.EndNodeID;
                        fromtype = tmpPipe_Q.EndNodeType == null ? 0 : (int)tmpPipe_Q.EndNodeType;
                    }
                    result.Add(0);
                }
                else
                {
                    if (Result_Flow >= 0)
                    {
                        fromnode = tmpPipe_Q.StartNodeID == null ? 0 : (int)tmpPipe_Q.StartNodeID;
                        fromtype = tmpPipe_Q.StartNodeType == null ? 0 : (int)tmpPipe_Q.StartNodeType;
                        result.Add(Result_Flow);
                    }
                    else if (Result_Flow < 0)
                    {
                        fromnode = tmpPipe_Q.EndNodeID == null ? 0 : (int)tmpPipe_Q.EndNodeID;
                        fromtype = tmpPipe_Q.EndNodeType == null ? 0 : (int)tmpPipe_Q.EndNodeType;
                        result.Add(Math.Abs(Result_Flow));
                    }
                    else
                    {
                        fromtype = tmpPipe_Q.StartNodeType == null ? 0 : (int)tmpPipe_Q.StartNodeType;
                        if (fromtype == TYPE_RESERVIOR || fromtype == TYPE_PUMP || fromtype == TYPE_TANK)
                        {
                            fromnode = tmpPipe_Q.StartNodeID == null ? 0 : (int)tmpPipe_Q.StartNodeID;
                        }
                        else
                        {
                            fromnode = tmpPipe_Q.EndNodeID == null ? 0 : (int)tmpPipe_Q.EndNodeID;
                            fromtype = tmpPipe_Q.EndNodeType == null ? 0 : (int)tmpPipe_Q.EndNodeType;
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
        public bool NextNode(int pnode, int node, int type, ref ArrayList findresult, int curTime)
        {
            int maxnode, fromnode = 0, maxpipe;
            int fromtype = 0, maxnodetype;
            double maxq;
            double Result_Flow = 0;

            bool last = true;

            ArrayList tmpPipe = new ArrayList();
            int links = GetLinkPipesAndNodes(pnode, node, ref tmpPipe);

            maxq = double.MinValue;//初始化本次最大流量
            maxpipe = 0;
            maxnode = node;//初始化本次流量最大管道元素ID
            maxnodetype = type;
            bool found = false;

            if (links > 0)
            {
                for (int i = 0; i < links; i++)
                {
                    ArrayList resultq = Pipe_Q((int)(tmpPipe[i] as ArrayList)[0], curTime);//根据管道ID查找管道流量和上游连接元素
                    if (resultq.Count > 2)
                    {
                        Result_Flow = Convert.ToDouble(resultq[0]);//管道流量
                        fromnode = (int)resultq[1];
                        fromtype = (int)resultq[2];
                    }

                    //if (fromnode == (long)(tmpPipe[i] as ArrayList)[1])
                    if (Result_Flow > maxq && (node != fromnode || type != fromtype))
                    {
                        last = false;
                        maxq = Result_Flow;
                        maxpipe = (int)(tmpPipe[i] as ArrayList)[0];//全局流量最大和本次流量最大管道ID
                        maxnode = (int)(tmpPipe[i] as ArrayList)[1];//管道连接元素ID
                        maxnodetype = (int)(tmpPipe[i] as ArrayList)[2];//管道连接元素类型
                        found = true;
                    }
                }
            }

            if (found)
            {
                findresult.Add(maxnode);
                findresult.Add(maxnodetype);
                findresult.Add(maxpipe);
            }
            else return true;
            return last;
        }

        /// <summary>
        /// 查询供水路径
        /// </summary>
        /// <param name="node">元素ID</param>
        /// <param name="type">元素表名</param>
        /// <param name="curTime">计算时刻</param>
        /// <returns>供水路径</returns>
        public void FindSupplyway(int node, int type, int curTime)
        {
            //db.Configuration.AutoDetectChangesEnabled = false;
            //db.Configuration.ValidateOnSaveEnabled = false;
            ArrayList PipeNoArr_pipeno = new ArrayList();//供水路径途径管道集合
            ArrayList PipeNoArr_nodes = new ArrayList();//需要更新元素集合（不包含水源）
            ArrayList PipeNoArr_node = new ArrayList();//供水路径途径元素和元素类型集合（不包括含初始元素）
            var sb = new StringBuilder();
            int findnode, pnodeId;
            int findtype;
            bool finish;
            bool isNew = true;//是否新添加
            ArrayList NArr = new ArrayList();
            ArrayList UArr = new ArrayList();

            bool IsUpdate = false;//是否需要更新

            BSB_SupplyWayEntity wnwSupplyWay = BSB_SupplyWay.GetList(node, type).FirstOrDefault();
            if (wnwSupplyWay == null)
            {
                IsUpdate = true;
                isNew = true;
            }
            else
            {
                isNew = false;
                IsUpdate = wnwSupplyWay.IsUpdate;
            }
            if (IsUpdate)
            {
                if (type == TYPE_RESERVIOR || type == TYPE_TANK || type == TYPE_PUMP)//首先查看初始元素是否是水源
                {
                    return;
                }

                findnode = node;
                pnodeId = node;
                findtype = type;
                NArr.Add(node);
                UArr.Add(type);

                bool bCircle = false;
                do
                {
                    int tmpnode = findnode;
                    int tmptype = findtype;
                    ArrayList foundresult = new ArrayList();
                    finish = NextNode(pnodeId, tmpnode, tmptype, ref foundresult, curTime);//查找curTime时刻当前元素上游元素

                    //if (finish && foundresult.Count < 1) return; //判断是否查找完成或者是否找到当前元素上游元素
                    if (foundresult.Count > 0)
                    {
                        findnode = (int)foundresult[0];
                        findtype = (int)foundresult[1];
                        pnodeId = tmpnode;

                        for (int i = 0; i < PipeNoArr_node.Count; i++) bCircle = bCircle || ((int)NArr[i] == findnode && (int)UArr[i] == findtype);

                        PipeNoArr_nodes.Add(tmpnode + "|" + tmptype);
                        PipeNoArr_pipeno.Add(Convert.ToString(foundresult[2]));
                        PipeNoArr_node.Add(findnode + "|" + findtype);//保存供水路径途径元素和元素类型
                    }
                    //if (finish || bCircle) break;

                    NArr.Add(findnode);
                    UArr.Add(findtype);

                    if (findtype != TYPE_JUNCTION && findtype != TYPE_TCV && findtype != TYPE_HYDRANT)
                    {
                        if (findtype == TYPE_RESERVIOR || findtype == TYPE_TANK || findtype == TYPE_PUMP)
                        {
                            finish = true;
                        }
                    }
                    string tmpPipeNoArrNode = "";//上游连接元素和元素类型
                    string tmpPipeNoArrPipeno = "";//上游连接管道
                    wnwSupplyWay = BSB_SupplyWay.GetList(tmpnode,tmptype).FirstOrDefault();
                    if (wnwSupplyWay != null)
                    {
                        if (!wnwSupplyWay.IsUpdate)//判断元素供水路径是否需要更新，如果已经更新过就当做供水路径结果
                        {
                            tmpPipeNoArrNode = wnwSupplyWay.SupplyWayNode;
                            tmpPipeNoArrPipeno = wnwSupplyWay.SupplyWayPipe;
                            finish = true;
                        }
                    }
                    if (PipeNoArr_node.Count > 1 && finish)
                    {
                        if (tmpPipeNoArrNode.Length > 0)
                        {
                            ArrayList tmpNode = new ArrayList(tmpPipeNoArrNode.Split(','));
                            tmpNode.RemoveAt(0);
                            PipeNoArr_node.AddRange(tmpNode);
                            ArrayList tmpPipe = new ArrayList(tmpPipeNoArrPipeno.Split(','));
                            tmpPipe.RemoveAt(0);
                            PipeNoArr_pipeno.AddRange(tmpPipe);
                            PipeNoArr_nodes.RemoveAt(PipeNoArr_nodes.Count - 1);
                        }
                        string tmpNodes = string.Join(",", (string[])PipeNoArr_node.ToArray(typeof(string)));
                        string tmpPipes = string.Join(",", (string[])PipeNoArr_pipeno.ToArray(typeof(string)));
                        for (int i = 0; i < PipeNoArr_nodes.Count; i++)
                        {
                            string[] tmpNodeType = PipeNoArr_nodes[i].ToString().Split('|');
                            long currentNode = Convert.ToInt64(tmpNodeType[0]);
                            if (!isNew)
                                sb.AppendFormat("UPDATE [dbo].[BSB_SupplyWay] SET [Is_Active] = 1, [IsUpdate] = {0},[SupplyWayNode]={1},[SupplyWayPipe]={2} WHERE [ElementId] = {3}", finish ? false : true, tmpNodes, tmpPipes, currentNode);
                            else
                                sb.AppendFormat("INSERT INTO [dbo].[BSB_SupplyWay] VALUES({0},{1},{2},{3},'{4}','{5}')",currentNode, Convert.ToInt32(tmpNodeType[1]),1, finish ? 0 : 1, tmpNodes, tmpPipes);
                            tmpNodes = tmpNodes.Substring(tmpNodes.IndexOf(',') + 1);
                            tmpPipes = tmpPipes.Substring(tmpPipes.IndexOf(',') + 1);
                        }
                    }
                } while (!finish && !bCircle);
            }
            if (sb.Length > 0) db.Database.ExecuteSqlCommand(sb.ToString());
            //db.Configuration.AutoDetectChangesEnabled = true;
            //db.Configuration.ValidateOnSaveEnabled = true;
        }
    }
}