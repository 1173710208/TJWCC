using TJWCC.Code;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Repository.BSSys;
using TJWCC.Domain.IRepository.BSSys;
using TJWCC.Domain.Entity.BSSys;
using System.Collections;
using System.Drawing;
using TJWCC.Application.WCC;

namespace TJWCC.Application.BSSys
{

    public class PointCloseValveApp
    {
        PIPEApp PIPE = new PIPEApp();
        TCVApp TCV = new TCVApp();
        JUNCTIONApp JUNCTION = new JUNCTIONApp();
        RESERVOIRApp RESERVOIR = new RESERVOIRApp();
        RESULT_JUNCTIONApp RESULT_JUNCTION = new RESULT_JUNCTIONApp();
        RESULT_PIPEApp RESULT_PIPE = new RESULT_PIPEApp();
        BSB_LinkedTableApp BSB_LinkedTable = new BSB_LinkedTableApp();
        MaterialClassApp MaterialClass = new MaterialClassApp();
        DistrictClassApp DistrictClass = new DistrictClassApp();
        DMAClassApp DMAClass = new DMAClassApp();
        DMAAreaClassApp DMAAreaClass = new DMAAreaClassApp();
        ZoneClassApp ZoneClass = new ZoneClassApp();
        BSB_BranchPipesApp BSB_BranchPipes = new BSB_BranchPipesApp();
        JUNCTION_VICEApp JUNCTION_VICE = new JUNCTION_VICEApp();
        //List<BSB_BranchPipes> bsbBranchPipes = new List<BSB_BranchPipes>();
        //List<TCV> valve = new List<TCV>();
        //List<PIPE> pipe = new List<PIPE>();
        //List<JUNCTION> junction = new List<JUNCTION>();
        //private int mintStart;
        private bool isBranchNet = false;//是否选中枝状管网标识
        private ArrayList _foundNodes = new ArrayList();
        private ArrayList _foundPipes = new ArrayList();
        private ArrayList _foundNodeType = new ArrayList();
        private ArrayList _foundValves = new ArrayList();
        private ArrayList influence_user = new ArrayList();
        private ArrayList titleArr = new ArrayList();

        private ArrayList canNotBeClosedValves = new ArrayList();
        //元素类型
        public readonly int TYPE_JUNCTION = 55;
        public readonly int TYPE_TCV = 61;
        public readonly int TYPE_HYDRANT = 54;
        public readonly int TYPE_TANK = 52;
        public readonly int TYPE_PUMP = 68;
        public readonly int TYPE_PIPE = 69;
        public readonly int TYPE_RESERVIOR = 56;
        public readonly int TYPE_CHECKVALVE = 309;

        private List<PointF> nodes;
        /// <summary>
        /// 计时类
        /// </summary>
        public class StopWatch
        {
            private int mintStart;

            /// <summary>
            /// 启动计时
            /// </summary>
            public void start()
            {
                mintStart = Environment.TickCount;
            }

            /// <summary>
            /// 获取运行时间
            /// </summary>
            /// <returns>已运行时间</returns>
            public long elapsed()
            {
                return Environment.TickCount - mintStart;
            }
        }

        /// <summary>
        /// 得到与某节点连接的所有管道和另一端的节点
        /// </summary>
        /// <param name="nodeno">要判断的节点ID</param>
        /// <param name="linkPipes">连接的管道列表</param>
        /// <param name="linkNodes">另一端节点ID列表</param>
        /// <param name="linkType">另一端节点类型</param>
        /// <returns></returns>
        private int GetLinkPipesAndNodes(int nodeno, ref ArrayList linkPipes, ref ArrayList linkNodes, ref ArrayList linkType)
        {
            linkPipes.Clear();
            linkNodes.Clear();
            linkType.Clear();

            List<PIPEEntity> wnwPipe = PIPE.GetStartList(nodeno);
            for (int i = 0; i < wnwPipe.Count; i++)
            {
                linkPipes.Add((int)wnwPipe[i].ElementId);//管道ID
                linkNodes.Add((int)wnwPipe[i].EndNodeID);//另一端节点ID
                linkType.Add(wnwPipe[i].EndNodeType);//另一端元素类型
            }
            wnwPipe = PIPE.GetEndList(nodeno);
            for (int j = 0; j < wnwPipe.Count; j++)
            {
                linkPipes.Add((int)wnwPipe[j].ElementId);//管道ID
                linkNodes.Add((int)wnwPipe[j].StartNodeID);//另一端节点ID
                linkType.Add(wnwPipe[j].StartNodeType);//另一端元素类型
            }
            return linkPipes.Count;
        }

        /// <summary>
        /// 查找一组节点所需关闭的下一级管道和节点列表
        /// </summary>
        /// <param name="closeNodes">传入参数，需要判断关闭的节点列表</param>
        /// <param name="nextLevelNodes">返回参数，下一级需要关闭的节点列表</param>
        /// <returns>返回是否已经找到全部需要关闭的阀门</returns>
        private bool CloseNodeLevel(ArrayList closeNodes, ref ArrayList nextLevelNodes)
        {
            bool allclosed = true;
            nextLevelNodes.Clear();
            //遍历需要关闭的节点
            for (int i = 0; i < closeNodes.Count; i++)
            {
                ArrayList linkPipes = new ArrayList();
                ArrayList linkNodes = new ArrayList();
                ArrayList linkType = new ArrayList();
                //找到与该节点连接的所有管道和节点列表
                int nodecount = GetLinkPipesAndNodes((int)closeNodes[i], ref linkPipes, ref linkNodes, ref linkType);
                for (int k = 0; k < linkNodes.Count; k++)
                {
                    //如果该管道已判断过了，即已关闭了，则忽略该管道
                    if (_foundPipes.IndexOf(linkPipes[k]) >= 0) continue;

                    _foundNodes.Add(linkNodes[k]);
                    _foundPipes.Add(linkPipes[k]);
                    _foundNodeType.Add(linkType[k]);
                    int tmplinkNodes = (int)linkNodes[k];
                    Boolean thisClosed = false;
                    if ((int)linkType[k] == TYPE_TCV)//判断找到的元素是否阀门
                    {
                        //BSB_BranchPipes tmpbsbBranchPipes = bsbBranchPipes.Where(item => item.ElementId == tmplinkNodes).FirstOrDefault();
                        //TCV tmpValve = valve.Where(item => item.ElementId == tmplinkNodes).FirstOrDefault();
                        var tmpbsbBranchPipes = BSB_BranchPipes.GetList(tmplinkNodes).FirstOrDefault();
                        var tmpValve = TCV.GetList(tmplinkNodes).FirstOrDefault();
                        if (tmpValve != null)
                        {
                            //如果关闭区域是枝状管网，则遇到阀门是枝状管网入口时该元素为该分支的终点，否则遇到阀门是普通阀门时该元素为该分支的终点
                            if (isBranchNet) thisClosed = tmpbsbBranchPipes.MainBranch == 1;
                            else
                            {
                                if (tmpbsbBranchPipes == null) thisClosed = true;
                                else thisClosed = false;
                            }
                            int isActive = tmpValve.Is_Active == null ? 0 : (int)tmpValve.Is_Active;
                            //阀门是正常状态而且是可以关闭的，不是水池，则该元素为该分支的终点
                            thisClosed = (thisClosed && isActive == 1 && canNotBeClosedValves.IndexOf(linkNodes[k]) < 0) || ((int)linkType[k] == TYPE_RESERVIOR);
                        }
                    }
                    //如果不是终点，则将该节点添加到下一轮需要判断的节点列表中
                    if (!thisClosed && nextLevelNodes.IndexOf(linkNodes[k]) < 0) nextLevelNodes.Add(linkNodes[k]);
                    //判断是否所有分支都是终点了，如是整个判断就结束了
                    allclosed = allclosed && thisClosed;
                }
            }

            return allclosed;
        }

        /// <summary>
        /// 搜索所有可能需要关闭管道和节点组件ID，关闭的阀门在节点数组中，未排除枝状管或失灵的阀门
        /// 结果保存在
        /// ClosedNodes(关闭的节点，含阀门）
        /// closedNodeTypes(关闭节点的类型)
        /// ClosedPipes（关闭的管道）
        /// </summary>
        /// <param name="element">需要关闭的点组件的ElementID</param>
        private void CloseNode(int element)
        {
            //传入CloseNodeLevel方法的参数是需要判断的节点ElementID数组，尽管开始时只有一个点
            ArrayList nodelist = new ArrayList();
            nodelist.Add(element);
            Boolean allClosed = false;
            //逐级循环直到找到全部阀门
            do
            {
                ArrayList nextLevelList = new ArrayList();
                //传入需判断的关闭节点列表，返回该列表下一级需要关闭的节点列表nextLevelList
                allClosed = CloseNodeLevel(nodelist, ref nextLevelList);
                nodelist.Clear();
                nodelist = (ArrayList)nextLevelList.Clone();
            } while (!allClosed);
        }

        /// <summary>
        /// 关闭一个节点的搜索主过程
        /// </summary>
        /// <param name="elementID">关闭的节点ID</param>
        /// <param name="elementType">关闭的节点类型</param>
        /// <param name="_canNotBeclosedValves">无法关闭阀门ID列表</param>
        /// <returns>返回ASObject类型结果，包括FoundNodes（节点数组），FoundNodeTypes（节点类型数组），FoundPipes（管道数组）</returns>
        public ArrayList IsolateNode(int elementID, int elementType, ArrayList _canNotBeclosedValves = null)
        {
            StopWatch sw = new StopWatch();
            sw.start();

            long t1 = 0, t2 = 0, t3 = 0, t4 = 0, t5 = 0, t6 = 0;

            t1 = sw.elapsed();
            //     SBGlobal.Global.LoadPipeAnalysisData();
            t2 = sw.elapsed();

            _foundNodes.Clear();
            _foundNodeType.Clear();
            _foundPipes.Clear();
            canNotBeClosedValves.Clear();

            //BSB_BranchPipes tmpbsbBranchPipes = bsbBranchPipes.Where(item => item.ElementId == elementID).FirstOrDefault();
            var tmpbsbBranchPipes = BSB_BranchPipes.GetList(elementID).FirstOrDefault();
            switch (elementType)//判断关闭的节点是否在枝状管网内
            {
                case 55:
                    if (tmpbsbBranchPipes != null) isBranchNet = true;
                    _foundNodes.Add(elementID);
                    _foundNodeType.Add(55);
                    break;
                case 61:
                    if (tmpbsbBranchPipes != null) isBranchNet = tmpbsbBranchPipes.MainBranch == 0;
                    _foundNodes.Add(elementID);
                    _foundNodeType.Add(61);
                    break;
            }

            if (_canNotBeclosedValves != null)
            {
                for (int i = 0; i < _canNotBeclosedValves.Count; i++)
                {
                    canNotBeClosedValves.Add(_canNotBeclosedValves[i]);
                    long tmpeId = (long)_canNotBeclosedValves[i];
                    tmpbsbBranchPipes = BSB_BranchPipes.GetList(tmpeId).FirstOrDefault();
                    if (tmpbsbBranchPipes != null) isBranchNet = false;
                }
            }
            CloseNode(elementID);
            t3 = sw.elapsed();
            ProcessBranchPipes(elementID);
            t4 = sw.elapsed();
            influence();
            t5 = sw.elapsed();
            title();
            t6 = sw.elapsed();
            PointF tmpPoint = new PointF();
            GetNodesByAngle(out tmpPoint);

            ArrayList result = new ArrayList();
            result.Add(_foundNodes);//受影响节点
            result.Add(_foundNodeType);//受影响节点对应数据表名
            result.Add(_foundPipes);//受影响管道
            result.Add(_foundValves);//关键阀门
            result.Add(influence_user);//受影响用户信息
            result.Add(titleArr);//受影响情况的基本信息
            result.Add(_canNotBeclosedValves);//不能关闭阀门
            result.Add(SortedNodes);//凸包点集

            return result;
        }

        /// <summary>
        /// 关闭一个管道的搜索主过程
        /// </summary>
        /// <param name="elementID">关闭的管道ID</param>
        /// <param name="_canNotBeclosedValves">无法关闭阀门ID列表</param>
        /// <returns>返回ASObject类型结果，包括FoundNodes（节点数组），FoundNodeTypes（节点类型数组），FoundPipes（管道数组）</returns>
        public ArrayList IsolatePipe(int elementID, ArrayList _canNotBeclosedValves = null)
        {
            //       SBGlobal.Global.LoadPipeAnalysisData();
            _foundNodes.Clear();
            _foundNodeType.Clear();
            _foundPipes.Clear();

            //BSB_BranchPipes tmpbsbBranchPipes = bsbBranchPipes.Where(item => item.ElementId == elementID).FirstOrDefault();
            var tmpbsbBranchPipes = BSB_BranchPipes.GetList(elementID).FirstOrDefault();
            if (tmpbsbBranchPipes != null) isBranchNet = tmpbsbBranchPipes.MainBranch == 0;

            //PIPE wnwPipe = pipe.Where(item => item.ElementId == elementID && item.Is_Active == 1).FirstOrDefault();//调取有效的elementID数据
            var wnwPipe = PIPE.GetList(elementID).FirstOrDefault();//调取有效的elementID数据
            if (wnwPipe != null)
            {
                canNotBeClosedValves.Clear();

                if (_canNotBeclosedValves != null)
                {
                    for (int i = 0; i < _canNotBeclosedValves.Count; i++)
                    {
                        canNotBeClosedValves.Add(_canNotBeclosedValves[i]);
                    }
                }
                int tmpStartNodeID = (int)wnwPipe.StartNodeID;
                _foundNodes.Add(tmpStartNodeID);
                _foundNodeType.Add(wnwPipe.StartNodeType);
                tmpbsbBranchPipes = BSB_BranchPipes.GetList(tmpStartNodeID).FirstOrDefault();
                if (wnwPipe.StartNodeType == TYPE_TCV)
                {
                    bool isClose = true;
                    var tmpValve = TCV.GetList(tmpStartNodeID).FirstOrDefault();
                    if (tmpValve != null)
                    {
                        //如果关闭区域是枝状管网，则遇到阀门是枝状管网入口时该元素为该分支的终点，否则遇到阀门是普通阀门时该元素为该分支的终点
                        if (isBranchNet) isClose = tmpbsbBranchPipes.MainBranch == 1;
                        else
                        {
                            if (tmpbsbBranchPipes == null) isClose = true;
                            else isClose = false;
                        }
                        int isActive = tmpValve.Is_Active == null ? 0 : (int)tmpValve.Is_Active;
                        //阀门是正常状态而且是可以关闭的，不是水池，则该元素为该分支的终点
                        isClose = (isClose && isActive == 1 && canNotBeClosedValves.IndexOf(tmpStartNodeID) < 0) || wnwPipe.StartNodeType == TYPE_RESERVIOR;
                    }
                    if (isClose)
                    {
                    }
                    else CloseNode(tmpStartNodeID);
                }
                else
                {
                    CloseNode(tmpStartNodeID);
                }
                int tmpEndNodeID = (int)wnwPipe.EndNodeID;
                _foundNodes.Add(tmpEndNodeID);
                _foundNodeType.Add(wnwPipe.EndNodeType);
                tmpbsbBranchPipes = BSB_BranchPipes.GetList(tmpEndNodeID).FirstOrDefault();
                if (wnwPipe.EndNodeType == TYPE_TCV)
                {
                    bool isClose = true;
                    var tmpValve = TCV.GetList(tmpEndNodeID).FirstOrDefault();
                    if (tmpValve != null)
                    {
                        //如果关闭区域是枝状管网，则遇到阀门是枝状管网入口时该元素为该分支的终点，否则遇到阀门是普通阀门时该元素为该分支的终点
                        if (isBranchNet) isClose = tmpbsbBranchPipes.MainBranch == 1;
                        else
                        {
                            if (tmpbsbBranchPipes == null) isClose = true;
                            else isClose = false;
                        }
                        int isActive = tmpValve.Is_Active == null ? 0 : (int)tmpValve.Is_Active;
                        //阀门是正常状态而且是可以关闭的，不是水池，则该元素为该分支的终点
                        isClose = (isClose && isActive == 1 && canNotBeClosedValves.IndexOf(tmpEndNodeID) < 0) || (wnwPipe.EndNodeType == TYPE_RESERVIOR);
                    }
                    if (isClose)
                    {
                    }
                    else CloseNode(tmpEndNodeID);
                }
                else
                {
                    CloseNode(tmpEndNodeID);
                }
                ProcessBranchPipes(elementID);
            }
            PointF tmpPoint = new PointF();
            influence();
            title();
            GetNodesByAngle(out tmpPoint);

            ArrayList result = new ArrayList();
            result.Add(_foundNodes);//受影响节点
            result.Add(_foundNodeType);//受影响节点对应数据表名
            result.Add(_foundPipes);//受影响管道
            result.Add(_foundValves);//关键阀门
            result.Add(influence_user);//受影响用户信息
            result.Add(titleArr);//受影响情况的基本信息
            result.Add(_canNotBeclosedValves);//不能关闭阀门
            result.Add(SortedNodes);//凸包点集

            return result;
        }

        /// <summary>
        /// 遍历关闭结果数组，找出真正需要关闭的阀门（去掉失灵阀门）
        /// </summary>
        public void ProcessBranchPipes(int closedElementID)
        {
            _foundValves.Clear();
            for (int i = 0; i < _foundNodeType.Count; i++)
            {
                if ((int)_foundNodeType[i] == TYPE_TCV)
                {
                    int tmpfoundNodes = (int)_foundNodes[i];
                    //TCV tmpValve = valve.Where(item => item.ElementId == tmpfoundNodes).FirstOrDefault();
                    var tmpValve = TCV.GetList(tmpfoundNodes).FirstOrDefault();
                    if (tmpValve != null)
                    {
                        if (isBranchNet)
                        {
                            //判断阀门是有效的
                            if (tmpValve.Is_Active == 1 && canNotBeClosedValves.IndexOf(tmpfoundNodes) < 0) _foundValves.Add(tmpfoundNodes);
                        }
                        else
                        {
                            var tmpbsbBranchPipes = BSB_BranchPipes.GetList(tmpfoundNodes).FirstOrDefault();
                            if (tmpValve.Is_Active == 1 && canNotBeClosedValves.IndexOf(tmpfoundNodes) < 0 && tmpbsbBranchPipes == null) _foundValves.Add(tmpfoundNodes);
                        }
                    }
                }
            }

        }

        /// <summary>
        /// 根据关闭的元素获取显示信息
        /// </summary>
        public void influence()
        {
            int[] foundPipes = (int[])_foundPipes.ToArray(typeof(int)); //受影响管道arraylist数据转成数组数据
            int[] foundValves = (int[])_foundValves.ToArray(typeof(int));//关键阀门arraylist数据转成数组数据
            int[] foundJunctions = (int[])_foundNodes.ToArray(typeof(int));//受影响节点arraylist数据转成数组数据
            ArrayList arr = new ArrayList();
            nodes = new List<PointF>();
            arr.Add("影响的用户");
            arr.Add("关闭的管道");
            arr.Add("关闭的阀门");
            arr.Add("节点");

            for (int j = 0; j < arr.Count; j++)
            {
                ArrayList asob = new ArrayList();
                asob.Add(arr[j].ToString());//label
                //int no = 1;
                switch (arr[j].ToString())
                {
                    #region 影响的用户
                    case "影响的用户":
                        {
                            //sql = "SELECT [USERNAME],[CALIBER],[ADDR] ,[X] ,[Y] FROM [sambo].[dbo].[linkedtable] where ";
                            var bsBusinessToPipe = (from bsblt in BSB_LinkedTable.GetList(foundJunctions)
                                                    select new
                                                    {
                                                        bsblt.ElementId,
                                                        bsblt.WaterMeter_ID,
                                                        bsblt.CALIBER,
                                                        Address = bsblt.Address == null ? "无" : bsblt.Address,
                                                        bsblt.USERNAME,
                                                        bsblt.GISID,
                                                        bsblt.Label,
                                                        bsblt.OBJECTID,
                                                    }).ToList();
                            asob.Add(bsBusinessToPipe.Count);//count
                            asob.Add(bsBusinessToPipe.ToJson());
                            var jv= JUNCTION_VICE.GetList(bsBusinessToPipe.Select(i=>i.OBJECTID.ToString()).ToArray());
                            if (jv != null)
                                for (int i = 0; i < jv.Count; i++)
                                {
                                    PointF tmpnodes = new PointF();
                                    tmpnodes.X = Convert.ToSingle(jv[i].Shape.XCoordinate);
                                    tmpnodes.Y = Convert.ToSingle(jv[i].Shape.YCoordinate);
                                    //nodes.Add(tmpnodes);
                                }
                            break;
                        }
                    #endregion
                    #region 关闭的管道
                    case "关闭的管道":
                        {
                            //foundPipes = "SELECT [Physical_Address],[Physical_PipeDiameter],[Physical_DistrictID],[ElementId] FROM [sde].[dbo].[PIPE] where ";
                            //List<PIPE> tmpPipe = pipe.Where(item => foundPipes.Contains((long)item.ElementId)).ToList();
                            var tmpPipe = (from pipe in PIPE.GetList(foundPipes)
                                           select new
                                           {
                                               pipe.ElementId,
                                               GISID = pipe.GISID == null ? "无" : pipe.GISID,
                                               pipe.Label,
                                               pipe.StartNodeLabel,
                                               StartNodeType = pipe.StartNodeType == 55 ? "节点" : pipe.StartNodeType == 61 ? "阀门" : pipe.StartNodeType == 56 ? "水库" : pipe.StartNodeType == 54 ? "消火栓" : pipe.StartNodeType == 68 ? "泵" : "无",
                                               pipe.EndNodeLabel,
                                               EndNodeType = pipe.EndNodeType == 55 ? "节点" : pipe.EndNodeType == 61 ? "阀门" : pipe.EndNodeType == 56 ? "水库" : pipe.EndNodeType == 54 ? "消火栓" : pipe.EndNodeType == 68 ? "泵" : "无",
                                               Physical_PipeDiameter = pipe.Physical_PipeDiameter == null ? "无" : pipe.Physical_PipeDiameter.ToString(),
                                               Physical_PipeMaterialID = pipe.Physical_PipeMaterialID == null ? "无" : MaterialClass.GetMaterial(pipe.Physical_PipeMaterialID.Value),
                                               Physical_Length = pipe.Physical_Length == null ? "无" : pipe.Physical_Length.ToString(),
                                               pipe.Physical_InstallationYear,
                                               Physical_Address = pipe.Physical_Address == null ? "无" : pipe.Physical_Address,
                                               Physical_ZoneID = pipe.Physical_ZoneID == null ? "无" : ZoneClass.GetZone(pipe.Physical_ZoneID.Value),
                                               Physical_DistrictID = pipe.Physical_DistrictID == null ? "无" : DistrictClass.GetDistrict(pipe.Physical_DistrictID.Value),
                                               Physical_DMAID = pipe.Physical_DMAID == null ? "无" : DMAClass.GetDMA(pipe.Physical_DMAID.Value),
                                               Physical_DMAAreaID = pipe.Physical_DMAAreaID == null ? "无" : DMAAreaClass.GetDMAArea(pipe.Physical_DMAAreaID.Value),
                                               Is_Active = pipe.Is_Active == 1 ? "启用" : "停用",
                                           }).ToList();
                            asob.Add(tmpPipe.Count);//count
                            asob.Add(tmpPipe.ToJson());
                            break;
                        }
                    #endregion
                    #region 关闭的阀门
                    case "关闭的阀门":
                        {
                            //foundPipes = "SELECT [Physical_Address],[Physical_Diameter],[Physical_DistrictID],[ElementId] FROM [sde].[dbo].[TCV] where ";
                            //List<TCV> tmpValve = valve.Where(item => foundValves.Contains((long)item.ElementId)).ToList();
                            var tmpValve = (from tcv in TCV.GetList(foundValves)
                                            select new
                                            {
                                                tcv.ElementId,
                                                GISID = tcv.GISID == null ? "无" : tcv.GISID,
                                                tcv.Label,
                                                Physical_Elevation = tcv.Physical_Elevation == null ? "无" : tcv.Physical_Elevation.ToString(),
                                                Physical_Depth = tcv.Physical_Depth == null ? "无" : tcv.Physical_Depth.ToString(),
                                                //tcv.Physical_Type,
                                                Physical_Diameter = tcv.Physical_Diameter == null ? "无" : tcv.Physical_Diameter.ToString(),
                                                Physical_Address = tcv.Physical_Address == null ? "无" : tcv.Physical_Address,
                                                Physical_ZoneID = tcv.Physical_ZoneID == null ? "无" : ZoneClass.GetZone(tcv.Physical_ZoneID.Value),
                                                Physical_DistrictID = tcv.Physical_DistrictID == null ? "无" : DistrictClass.GetDistrict(tcv.Physical_DistrictID.Value),
                                                Physical_DMAID = tcv.Physical_DMAID == null ? "无" : DMAClass.GetDMA(tcv.Physical_DMAID.Value),
                                                Physical_DMAAreaID = tcv.Physical_DMAAreaID == null ? "无" : DMAAreaClass.GetDMAArea(tcv.Physical_DMAAreaID.Value),
                                                tcv.Physical_InstallationYear,
                                                Is_Active = tcv.Is_Active == 1 ? "启用" : "停用",
                                                tcv.SHAPE
                                            }).ToList();
                            if (tmpValve != null)
                                for (int i = 0; i < tmpValve.Count; i++)
                                {
                                    PointF tmpnodes = new PointF();
                                    tmpnodes.X = Convert.ToSingle(tmpValve[i].SHAPE.XCoordinate);
                                    tmpnodes.Y = Convert.ToSingle(tmpValve[i].SHAPE.YCoordinate);
                                    nodes.Add(tmpnodes);
                                }
                            asob.Add(tmpValve.Count);//count
                            asob.Add(tmpValve.ToJson());
                            break;
                        }
                    #endregion
                    #region 节点
                    case "节点":
                        {
                            //foundPipes = "SELECT [Physical_Address],[Physical_DistrictID],[ElementId] FROM [sde].[dbo].[JUNCTION] where ";
                            //List<JUNCTION> tmpJunction = junction.Where(item => foundJunctions.Contains((long)item.ElementId)).ToList();
                            var tmpJunction = JUNCTION.GetList(foundJunctions);
                            if (tmpJunction != null)
                                for (int i = 0; i < tmpJunction.Count; i++)
                                {
                                    PointF tmpnodes = new PointF();
                                    tmpnodes.X = Convert.ToSingle(tmpJunction[i].SHAPE.XCoordinate);
                                    tmpnodes.Y = Convert.ToSingle(tmpJunction[i].SHAPE.YCoordinate);
                                    nodes.Add(tmpnodes);
                                }
                            asob.Add(tmpJunction.Count);//count
                            asob.Add(tmpJunction.ToJson());
                            break;
                        }

                        #endregion
                }
                influence_user.Add(asob);
            }
        }

        /// <summary>
        /// 生成爆管分析计算结果标题
        /// </summary>
        public void title()
        {
            int[] foundPipes = (int[])_foundPipes.ToArray(typeof(int)); //受影响管道arraylist数据转成数组数据
            int[] foundJunctions = (int[])_foundNodes.ToArray(typeof(int));//受影响节点arraylist数据转成数组数据
            double length = 0;//受影响管道长度
            double NodesDemand = 0;//受影响需水量
            double volume = 0;
            int _hour = 24;

            //sql = "SELECT [Physical_Length],[Physical_PipeDiameter] FROM [sde].[dbo].[RESULT_PIPE] where ";
            //List<PIPE> tmpPipe = pipe.Where(item => foundPipes.Contains((long)item.ElementId)).ToList();
            var tmpPipe = PIPE.GetList(foundPipes);
            for (int k = 0; k < tmpPipe.Count; k++)
            {
                double tmpPhysical_Length = tmpPipe[k].Physical_Length == null ? 0 : Convert.ToDouble(tmpPipe[k].Physical_Length);
                double tmpPhysical_Diameter = tmpPipe[k].Physical_PipeDiameter == null ? 0 : Convert.ToDouble(tmpPipe[k].Physical_PipeDiameter);
                length += tmpPhysical_Length;
                volume += (tmpPhysical_Diameter / 2000) * (tmpPhysical_Diameter / 2000) * Math.PI * tmpPhysical_Length;
            }

            //sql = "SELECT [Result_Demand_" + _hour + "] FROM [sde].[dbo].[RESULT_JUNCTION] where ";
            var resultJunction = RESULT_JUNCTION.GetList(foundJunctions);
            for (int k = 0; k < resultJunction.Count; k++)
            {
                switch (_hour)
                {
                    case 0:
                        NodesDemand += resultJunction[k].Result_Demand_0 == null ? 0 : Convert.ToDouble(resultJunction[k].Result_Demand_0);
                        break;
                    case 1:
                        NodesDemand += resultJunction[k].Result_Demand_1 == null ? 0 : Convert.ToDouble(resultJunction[k].Result_Demand_1);
                        break;
                    case 2:
                        NodesDemand += resultJunction[k].Result_Demand_2 == null ? 0 : Convert.ToDouble(resultJunction[k].Result_Demand_2);
                        break;
                    case 3:
                        NodesDemand += resultJunction[k].Result_Demand_3 == null ? 0 : Convert.ToDouble(resultJunction[k].Result_Demand_3);
                        break;
                    case 4:
                        NodesDemand += resultJunction[k].Result_Demand_4 == null ? 0 : Convert.ToDouble(resultJunction[k].Result_Demand_4);
                        break;
                    case 5:
                        NodesDemand += resultJunction[k].Result_Demand_5 == null ? 0 : Convert.ToDouble(resultJunction[k].Result_Demand_5);
                        break;
                    case 6:
                        NodesDemand += resultJunction[k].Result_Demand_6 == null ? 0 : Convert.ToDouble(resultJunction[k].Result_Demand_6);
                        break;
                    case 7:
                        NodesDemand += resultJunction[k].Result_Demand_7 == null ? 0 : Convert.ToDouble(resultJunction[k].Result_Demand_7);
                        break;
                    case 8:
                        NodesDemand += resultJunction[k].Result_Demand_8 == null ? 0 : Convert.ToDouble(resultJunction[k].Result_Demand_8);
                        break;
                    case 9:
                        NodesDemand += resultJunction[k].Result_Demand_9 == null ? 0 : Convert.ToDouble(resultJunction[k].Result_Demand_9);
                        break;
                    case 10:
                        NodesDemand += resultJunction[k].Result_Demand_10 == null ? 0 : Convert.ToDouble(resultJunction[k].Result_Demand_10);
                        break;
                    case 11:
                        NodesDemand += resultJunction[k].Result_Demand_11 == null ? 0 : Convert.ToDouble(resultJunction[k].Result_Demand_11);
                        break;
                    case 12:
                        NodesDemand += resultJunction[k].Result_Demand_12 == null ? 0 : Convert.ToDouble(resultJunction[k].Result_Demand_12);
                        break;
                    case 13:
                        NodesDemand += resultJunction[k].Result_Demand_13 == null ? 0 : Convert.ToDouble(resultJunction[k].Result_Demand_13);
                        break;
                    case 14:
                        NodesDemand += resultJunction[k].Result_Demand_14 == null ? 0 : Convert.ToDouble(resultJunction[k].Result_Demand_14);
                        break;
                    case 15:
                        NodesDemand += resultJunction[k].Result_Demand_15 == null ? 0 : Convert.ToDouble(resultJunction[k].Result_Demand_15);
                        break;
                    case 16:
                        NodesDemand += resultJunction[k].Result_Demand_16 == null ? 0 : Convert.ToDouble(resultJunction[k].Result_Demand_16);
                        break;
                    case 17:
                        NodesDemand += resultJunction[k].Result_Demand_17 == null ? 0 : Convert.ToDouble(resultJunction[k].Result_Demand_17);
                        break;
                    case 18:
                        NodesDemand += resultJunction[k].Result_Demand_18 == null ? 0 : Convert.ToDouble(resultJunction[k].Result_Demand_18);
                        break;
                    case 19:
                        NodesDemand += resultJunction[k].Result_Demand_19 == null ? 0 : Convert.ToDouble(resultJunction[k].Result_Demand_19);
                        break;
                    case 20:
                        NodesDemand += resultJunction[k].Result_Demand_20 == null ? 0 : Convert.ToDouble(resultJunction[k].Result_Demand_20);
                        break;
                    case 21:
                        NodesDemand += resultJunction[k].Result_Demand_21 == null ? 0 : Convert.ToDouble(resultJunction[k].Result_Demand_21);
                        break;
                    case 22:
                        NodesDemand += resultJunction[k].Result_Demand_22 == null ? 0 : Convert.ToDouble(resultJunction[k].Result_Demand_22);
                        break;
                    case 23:
                        NodesDemand += resultJunction[k].Result_Demand_23 == null ? 0 : Convert.ToDouble(resultJunction[k].Result_Demand_23);
                        break;
                    case 24:
                        NodesDemand += resultJunction[k].Result_Demand_24 == null ? 0 : Convert.ToDouble(resultJunction[k].Result_Demand_24);
                        break;
                }
            }

            titleArr.Add(Math.Round(length, 2));
            titleArr.Add(Math.Round(volume, 2));
            titleArr.Add(Math.Round(NodesDemand, 2));
        }


        public PointF[] sor_nodes;
        private double DistanceOfNodes(PointF p0, PointF p1)
        {
            if (p0.IsEmpty || p1.IsEmpty)
                return 0.0;
            return Math.Sqrt((p1.X - p0.X) * (p1.X - p0.X) + (p1.Y - p0.Y) * (p1.Y - p0.Y));
        }
        public void GetNodesByAngle(out PointF p0)
        {
            LinkedList<PointF> list_node = new LinkedList<PointF>();
            p0 = GetMinYPoint();
            LinkedListNode<PointF> node = new LinkedListNode<PointF>(nodes[0]);
            list_node.AddFirst(node);
            for (int i = 1; i < nodes.Count; i++)
            {
                int direct = IsClockDirection(p0, node.Value, nodes[i]);
                if (direct == 1)
                {
                    list_node.AddLast(nodes[i]);
                    node = list_node.Last;
                    //node.Value = nodes[i]; 

                }
                else if (direct == -10)
                {
                    list_node.Last.Value = nodes[i];
                    //node = list_node.Last 
                    //node.Value = nodes[i]; 
                }
                else if (direct == 10)
                    continue;
                else if (direct == -1)
                {
                    LinkedListNode<PointF> temp = node.Previous;
                    while (temp != null && IsClockDirection(p0, temp.Value, nodes[i]) == -1)
                    {
                        temp = temp.Previous;
                    }
                    if (temp == null)
                    {
                        list_node.AddFirst(nodes[i]);
                        continue;
                    }
                    if (IsClockDirection(p0, temp.Value, nodes[i]) == -10)
                        temp.Value = nodes[i];
                    else if (IsClockDirection(p0, temp.Value, nodes[i]) == 10)
                        continue;
                    else
                        list_node.AddAfter(temp, nodes[i]);
                }
            }
            sor_nodes = list_node.ToArray();
            SortedNodes = new Stack<PointF>();
            SortedNodes.Push(p0);
            SortedNodes.Push(sor_nodes[0]);
            SortedNodes.Push(sor_nodes[1]);
            for (int i = 2; i < sor_nodes.Length; i++)
            {

                PointF p2 = sor_nodes[i];
                PointF p1 = SortedNodes.Pop();
                PointF p0_sec = SortedNodes.Pop();
                SortedNodes.Push(p0_sec);
                SortedNodes.Push(p1);

                if (IsClockDirection1(p0_sec, p1, p2) == 1)
                {
                    SortedNodes.Push(p2);
                    continue;
                }
                while (IsClockDirection1(p0_sec, p1, p2) != 1)
                {
                    SortedNodes.Pop();
                    p1 = SortedNodes.Pop();
                    p0_sec = SortedNodes.Pop();
                    SortedNodes.Push(p0_sec);
                    SortedNodes.Push(p1);
                }
                SortedNodes.Push(p2);

            }

        }
        private int IsClockDirection1(PointF p0, PointF p1, PointF p2)
        {
            PointF p0_p1 = new PointF(p1.X - p0.X, p1.Y - p0.Y);
            PointF p0_p2 = new PointF(p2.X - p0.X, p2.Y - p0.Y);
            return (p0_p1.X * p0_p2.Y - p0_p2.X * p0_p1.Y) > 0 ? 1 : -1;
        }
        private PointF GetMinYPoint()
        {
            PointF succNode;
            float miny = nodes.Min(r => r.Y);
            IEnumerable<PointF> pminYs = nodes.Where(r => r.Y == miny);
            PointF[] ps = pminYs.ToArray();
            if (pminYs.Count() > 1)
            {
                succNode = pminYs.Single(r => r.X == pminYs.Min(t => t.X));
                nodes.Remove(succNode);
                return succNode;
            }
            else
            {
                nodes.Remove(ps[0]);
                return ps[0];
            }

        }
        private int IsClockDirection(PointF p0, PointF p1, PointF p2)
        {
            PointF p0_p1 = new PointF(p1.X - p0.X, p1.Y - p0.Y);
            PointF p0_p2 = new PointF(p2.X - p0.X, p2.Y - p0.Y);
            if ((p0_p1.X * p0_p2.Y - p0_p2.X * p0_p1.Y) != 0)
                return (p0_p1.X * p0_p2.Y - p0_p2.X * p0_p1.Y) > 0 ? 1 : -1;
            else
                return DistanceOfNodes(p0, p1) > DistanceOfNodes(p0, p2) ? 10 : -10;

        }
        public Stack<PointF> SortedNodes { get; private set; }


        /////////////////////////////////////////////////////////////////////////////////////////供水路径////////////////////////////////////////////////////////////////
        /// <summary>
        /// 查询elementID节点的供水路径
        /// </summary>
        /// <param name="elementID">需查询的节点ID</param>
        /// <param name="curTime">时间点</param>
        /// <returns>返回ASObject类型结果，包括FoundNodes（节点数组），FoundNodeTypes（节点类型数组），FoundPipes（管道数组）</returns>
        public ArrayList GetSupplyPath(int elementID, int curTime)
        {
            ArrayList result = new ArrayList();
            ArrayList pipeResult = new ArrayList();
            ArrayList nodeResult = new ArrayList();
            ArrayList nodeTypeResult = new ArrayList();

            nodeResult.Add(elementID);
            //nodeTypeResult.Add(pipe.Where(item => item.StartNodeID == elementID && item.Is_Active == 1).FirstOrDefault().StartNodeType);
            nodeTypeResult.Add(PIPE.GetList(elementID).FirstOrDefault().StartNodeType);

            //如第一个点即为水库，则立即结束搜索
            var tmpReservoir = RESERVOIR.GetList(elementID).FirstOrDefault();
            if (tmpReservoir != null) return result;

            ArrayList linkPipes = new ArrayList();
            ArrayList linkNodes = new ArrayList();
            ArrayList linkTypes = new ArrayList();

            int searchID = (int)elementID;
            Boolean finished = false;

            do
            {
                linkNodes.Clear();
                linkPipes.Clear();
                linkTypes.Clear();
                //找出该节点连接的所有管道和节点
                int nodecount = GetLinkPipesAndNodes(searchID, ref linkPipes, ref linkNodes, ref linkTypes);

                double maxFlow = -double.MaxValue;
                int maxPos = -1;
                //找出上游管道流量最大的管道
                for (int i = 0; i < linkPipes.Count; i++)
                {
                    int tmplinkPipes = (int)linkPipes[i];
                    double flow = 0;
                    //PIPE tmpPipe = pipe.Where(item => item.ElementId == tmplinkPipes && item.Is_Active == 1).FirstOrDefault();
                    var tmpPipe = PIPE.GetList(tmplinkPipes).FirstOrDefault();
                    var rePipe = RESULT_PIPE.GetList(tmplinkPipes).FirstOrDefault();
                    if (tmpPipe != null && rePipe != null)
                    {
                        switch (curTime)
                        {
                            case 0:
                                flow = rePipe.Result_Flow_0 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_0);
                                break;
                            case 1:
                                flow = rePipe.Result_Flow_1 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_1);
                                break;
                            case 2:
                                flow = rePipe.Result_Flow_2 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_2);
                                break;
                            case 3:
                                flow = rePipe.Result_Flow_3 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_3);
                                break;
                            case 4:
                                flow = rePipe.Result_Flow_4 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_4);
                                break;
                            case 5:
                                flow = rePipe.Result_Flow_5 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_5);
                                break;
                            case 6:
                                flow = rePipe.Result_Flow_6 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_6);
                                break;
                            case 7:
                                flow = rePipe.Result_Flow_7 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_7);
                                break;
                            case 8:
                                flow = rePipe.Result_Flow_8 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_8);
                                break;
                            case 9:
                                flow = rePipe.Result_Flow_9 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_9);
                                break;
                            case 10:
                                flow = rePipe.Result_Flow_10 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_10);
                                break;
                            case 11:
                                flow = rePipe.Result_Flow_11 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_11);
                                break;
                            case 12:
                                flow = rePipe.Result_Flow_12 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_12);
                                break;
                            case 13:
                                flow = rePipe.Result_Flow_13 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_13);
                                break;
                            case 14:
                                flow = rePipe.Result_Flow_14 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_14);
                                break;
                            case 15:
                                flow = rePipe.Result_Flow_15 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_15);
                                break;
                            case 16:
                                flow = rePipe.Result_Flow_16 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_16);
                                break;
                            case 17:
                                flow = rePipe.Result_Flow_17 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_17);
                                break;
                            case 18:
                                flow = rePipe.Result_Flow_18 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_18);
                                break;
                            case 19:
                                flow = rePipe.Result_Flow_19 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_19);
                                break;
                            case 20:
                                flow = rePipe.Result_Flow_20 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_20);
                                break;
                            case 21:
                                flow = rePipe.Result_Flow_21 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_21);
                                break;
                            case 22:
                                flow = rePipe.Result_Flow_22 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_22);
                                break;
                            case 23:
                                flow = rePipe.Result_Flow_23 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_23);
                                break;
                        }
                        //流向该节点的管道
                        if ((tmpPipe.StartNodeID == searchID && flow < 0) || (tmpPipe.EndNodeID == searchID && flow > 0))
                        {
                            if (Math.Abs(flow) > maxFlow)
                            {
                                maxFlow = Math.Abs(flow);
                                maxPos = i;
                            }
                        }
                    }
                }

                //如找到上游最大流量管道
                if (maxPos >= 0)
                {
                    //设置继续搜索的节点
                    searchID = (int)linkNodes[maxPos];

                    //记录本次搜索的结果
                    nodeResult.Add((int)linkNodes[maxPos]);
                    nodeTypeResult.Add((int)linkTypes[maxPos]);
                    pipeResult.Add((int)linkPipes[maxPos]);

                    //如果搜索到水库，则搜索完成
                    tmpReservoir = RESERVOIR.GetList(elementID).FirstOrDefault();
                    if (tmpReservoir != null) finished = true;
                }
                else finished = true;

            } while (!finished);

            result.Add(pipeResult);//PipeResult
            result.Add(nodeResult);//NodeResult
            result.Add(nodeTypeResult);//NodeTypeResult
            return result;
        }

        /// <summary>
        /// 搜索下一级的供水节点列表，该方法为GetSupplyArea的子方法
        /// </summary>
        /// <param name="searchNodes">需搜索的节点列表</param>
        /// <param name="nextLevelNodes">下一级的搜索节点列表</param>
        /// <returns>是否无积蓄搜索的节点，是否结束搜索</returns>
        private Boolean GetSupplyAreaLevel(ArrayList searchNodes, ref ArrayList nextLevelNodes, int curTime)
        {
            nextLevelNodes.Clear();
            //遍历需要搜索的节点
            for (int i = 0; i < searchNodes.Count; i++)
            {
                ArrayList linkPipes = new ArrayList();
                ArrayList linkNodes = new ArrayList();
                ArrayList linkTables = new ArrayList();
                //找到与该节点连接的所有管道和节点列表
                int nodecount = GetLinkPipesAndNodes((int)searchNodes[i], ref linkPipes, ref linkNodes, ref linkTables);
                for (int k = 0; k < linkPipes.Count; k++)
                {
                    int tmplinkPipes = (int)linkPipes[k];
                    double flow = 0;
                    //PIPE tmpPipe = pipe.Where(item => item.ElementId == tmplinkPipes && item.Is_Active == 1).FirstOrDefault();
                    var tmpPipe = PIPE.GetList(tmplinkPipes).FirstOrDefault();
                    var rePipe = RESULT_PIPE.GetList(tmplinkPipes).FirstOrDefault();
                    if (tmpPipe != null && rePipe != null)
                    {
                        switch (curTime)
                        {
                            case 0:
                                flow = rePipe.Result_Flow_0 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_0);
                                break;
                            case 1:
                                flow = rePipe.Result_Flow_1 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_1);
                                break;
                            case 2:
                                flow = rePipe.Result_Flow_2 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_2);
                                break;
                            case 3:
                                flow = rePipe.Result_Flow_3 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_3);
                                break;
                            case 4:
                                flow = rePipe.Result_Flow_4 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_4);
                                break;
                            case 5:
                                flow = rePipe.Result_Flow_5 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_5);
                                break;
                            case 6:
                                flow = rePipe.Result_Flow_6 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_6);
                                break;
                            case 7:
                                flow = rePipe.Result_Flow_7 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_7);
                                break;
                            case 8:
                                flow = rePipe.Result_Flow_8 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_8);
                                break;
                            case 9:
                                flow = rePipe.Result_Flow_9 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_9);
                                break;
                            case 10:
                                flow = rePipe.Result_Flow_10 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_10);
                                break;
                            case 11:
                                flow = rePipe.Result_Flow_11 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_11);
                                break;
                            case 12:
                                flow = rePipe.Result_Flow_12 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_12);
                                break;
                            case 13:
                                flow = rePipe.Result_Flow_13 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_13);
                                break;
                            case 14:
                                flow = rePipe.Result_Flow_14 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_14);
                                break;
                            case 15:
                                flow = rePipe.Result_Flow_15 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_15);
                                break;
                            case 16:
                                flow = rePipe.Result_Flow_16 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_16);
                                break;
                            case 17:
                                flow = rePipe.Result_Flow_17 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_17);
                                break;
                            case 18:
                                flow = rePipe.Result_Flow_18 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_18);
                                break;
                            case 19:
                                flow = rePipe.Result_Flow_19 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_19);
                                break;
                            case 20:
                                flow = rePipe.Result_Flow_20 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_20);
                                break;
                            case 21:
                                flow = rePipe.Result_Flow_21 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_21);
                                break;
                            case 22:
                                flow = rePipe.Result_Flow_22 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_22);
                                break;
                            case 23:
                                flow = rePipe.Result_Flow_23 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_23);
                                break;
                            case 24:
                                flow = rePipe.Result_Flow_24 == null ? 0 : Convert.ToDouble(rePipe.Result_Flow_24);
                                break;
                        }
                        if ((tmpPipe.StartNodeID == (int)searchNodes[i] && flow > 0) || (tmpPipe.EndNodeID == (int)searchNodes[i] && flow < 0))
                        {
                            if (_foundNodes.IndexOf(linkNodes[k]) < 0) nextLevelNodes.Add(linkNodes[k]);
                            if (_foundPipes.IndexOf(linkPipes[k]) < 0) _foundPipes.Add((int)linkPipes[k]);
                            if (_foundNodes.IndexOf(linkNodes[k]) < 0) _foundNodes.Add((int)linkNodes[k]);
                            if (_foundNodeType.IndexOf(linkNodes[k]) < 0) _foundNodeType.Add((string)linkTables[k]);
                        }
                    }
                }
            }
            return nextLevelNodes.Count <= 0;
        }

        /// <summary>
        /// 搜索某个节点的供水范围
        /// </summary>
        /// <param name="elementID">节点ElementID</param>
        /// <returns>返回ASObject类型的搜索结果，包括FoundNodes（节点数组），FoundNodeTypes（节点类型数组），FoundPipes（管道数组）</returns>
        public ArrayList GetSupplyArea(int elementID, int curTime)
        {
            _foundNodes.Clear();
            _foundPipes.Clear();
            _foundNodeType.Clear();

            //传入CloseNodeLevel方法的参数是需要判断的节点ElementID数组，尽管开始时只有一个点
            ArrayList nodelist = new ArrayList();
            nodelist.Add(elementID);
            Boolean finished = false;
            //逐级循环直到找到全部管道
            do
            {
                ArrayList nextLevelList = new ArrayList();
                //传入需判断的节点列表，返回该列表下一级需要搜索的节点列表nextLevelList
                finished = GetSupplyAreaLevel(nodelist, ref nextLevelList, curTime);
                nodelist.Clear();
                nodelist = (ArrayList)nextLevelList.Clone();
            } while (!finished);

            ArrayList result = new ArrayList();
            result.Add(_foundNodes);//FoundNodes
            result.Add(_foundPipes);//FoundPipes
            result.Add(_foundNodeType);//FoundNodeTypes

            return result;
        }
    }
}
