var cnbValves = "";
var nodeId;
var nodeType0;
var minFlowPT = null;//冒水动画显示坐标位置
var isTubeBurst = false;
var influence_user0;
var influence_user1;
var influence_user2;
$(function () {
    $("#resultDivBoxCalBut").click(function () {//爆管分析计算结果重新计算按钮单击事件
        document.getElementById("resultDivBox").style.display = "none";
        hideAndClear();//清除地图上图形
        pollutionAnalysis(minFlowPT, nodeId, nodeType0, cnbValves);
        $.loading('正在计算中...');
    });
    $("#resultDivBoxBackBut").click(function () {//爆管分析计算结果返回按钮单击事件
        $('#resultDivBoxBackBut').css("display", "none");//隐藏返回按钮
        $('#resultinfluenceuserDiv').css("display", "none");//隐藏影响用户信息显示
        $('#resultclosePipesDiv').css("display", "none");//隐藏影响管道信息显示
        $('#resultcloseValvesDiv').css("display", "none");//隐藏关键阀门信息显示
        $('#sDivUserResult').css("display", "none");//隐藏影响用户标题
        $('#sDivPipesResult').css("display", "none");//隐藏影响管道标题
        $('#sDivValvesResult').css("display", "none");//隐藏关键阀门标题
        $('#resultDiv').css("display", "");//显示爆管分析结果列表
    });
    $("#resultinfluenceuserBut").click(function () {//爆管分析计算结果影响用户按钮单击事件
        $('#resultDiv').css("display", "none");//隐藏爆管分析结果列表
        $('#resultDivBoxBackBut').css("display", "");//显示返回按钮
        $('#resultinfluenceuserDiv').css("display", "");//显示影响用户信息
        $('#sDivUserResult').css("display", "");//显示影响用户标题
    });
    $("#resultclosePipesBut").click(function () {//爆管分析计算结果影响管道按钮单击事件
        $('#resultDiv').css("display", "none");//隐藏爆管分析结果列表
        $('#resultDivBoxBackBut').css("display", "");//显示返回按钮
        $('#resultclosePipesDiv').css("display", "");//显示影响管道信息
        $('#sDivPipesResult').css("display", "");//显示影响管道标题
    });
    $("#resultcloseValvesBut").click(function () {//爆管分析计算结果关键阀门按钮单击事件
        $('#resultDiv').css("display", "none");//隐藏爆管分析结果列表
        $('#resultDivBoxBackBut').css("display", "");//显示返回按钮
        $('#resultcloseValvesDiv').css("display", "");//显示关键阀门信息
        $('#sDivValvesResult').css("display", "");//显示关键阀门标题
    });
});
//设置页面div显示内容
function setValueForDiv(id, content) {
    var element = document.getElementById(id);
    try {
        //element.innerHTML = unescape(content);
        element.innerHTML = decodeURI(content);
    } catch (e) {
        return;
    }

    if (!element.innerHTML) {
        try {
            element.innerHTML = decodeURI(content);
        } catch (e) { }
    }
    //content = "";
}

function pollutionAnalysisClick(th) {//
    if (isTubeBurst) {
        recoverAll();
        hideAndClear();
        th.classList.remove('active');
        $('#but_mid2').addClass('disabled');
        $("#but_mid2").attr('disabled', true);
        $('#showPA').addClass('disabled');
        $("#showPA").attr('disabled', true);
        $('#pipeFlowBut').addClass('disabled');
        $("#pipeFlowBut").attr('disabled', true);
        $('#createPlanBut').addClass('disabled');
        $("#createPlanBut").attr('disabled', true);
        $('#createPolOrderBut').addClass('disabled');
        $("#createPolOrderBut").attr('disabled', true);
        $('#showPAge').addClass('disabled');
        $("#showPAge").attr('disabled', true);
        $("#right62_data").html("");
        $("#right622_data").html("");//影响用户个数
        $(".right732_data").html("");//关闭阀门信息
        $(".right522_data").html("");//影响用户信息
        isTubeBurst = false;//是否爆管分析标识
        if (isShowPA) {
            $('#showPA').removeClass('active');
            isShowPA = false;
            PollutionAfter.hide();
            $("#legendDivSoto").css("display", "none");
            tiled2.show();
            layerModelBaseMapMin.hide();
        }
        if (isPipeFlow) {
            recoverAll();
            hideAndClear();
            var _tmpdiv = $("#map_layers").nextAll();
            if (_tmpdiv.length > 0) {//隐藏水流方向动画效果
                $("#map_layers").nextAll().css("display", "none");
                //myPipeFlowChart.dispose();
                //overlay = null;
                //reservoirInfo();
            }
            $('#pipeFlowBut').removeClass('active');
            isPipeFlow = false;//是否水流方向标识
        }
    }
    else {
        recoverAll();
        hideAndClear();
        th.classList.add('active');
        // 改变按钮选中颜色
        isTubeBurst = true;//是否爆管分析标识
        setValueForDiv("followMouseInfo", "双击地图选择事件点")
        $('#map').mousemove(function (e) {
            $('#followMouseInfo').show().css({
                "top": e.pageY + 5,
                "left": e.pageX + 15
            });
        });
        $('#map').mouseleave(function () {
            $('#followMouseInfo').hide();
        });
    }
}

function canNotBeclosedValves(ValveId, thisDiv) {
    if (cnbValves.indexOf(ValveId) < 0) {
        if (cnbValves.length == 0) {
            cnbValves += ValveId;
        } else {
            cnbValves += "," + ValveId;
        }
        thisDiv.style.backgroundColor = "rgb(149,184,231)";
        $('#resultDivBoxCalBut').css("display", "");//显示重新计算按钮
        var queryTask1 = new esri.tasks.QueryTask(valveMapLayer);
        var query1 = new esri.tasks.Query();
        query1.where = "ElementId = " + ValveId;
        query1.outSpatialReference = map.spatialReference;
        query1.returnGeometry = true;
        queryTask1.execute(query1, function (featureSet) {
            if (featureSet.features.length > 0) {
                graphicslayer.add(new esri.Graphic(featureSet.features[0].geometry, YellowBigPolygonSymbol));
            }
        });
    } else {
        if (cnbValves.indexOf(ValveId) > 0) {
            var tmpstr = "," + ValveId;
            cnbValves = cnbValves.replace(tmpstr, "");
        } else {
            if (cnbValves.indexOf(",") < 0) {
                cnbValves = "";
                $('#resultDivBoxCalBut').css("display", "none");//隐藏重新计算按钮
            } else {
                var tmpstr = ValveId + ",";
                cnbValves = cnbValves.replace(tmpstr, "");
            }
        }
        thisDiv.style.backgroundColor = "";
        var queryTask1 = new esri.tasks.QueryTask(valveMapLayer);
        var query1 = new esri.tasks.Query();
        query1.where = "ElementId = " + ValveId;
        query1.outSpatialReference = map.spatialReference;
        query1.returnGeometry = true;
        queryTask1.execute(query1, function (featureSet) {
            if (featureSet.features.length > 0) {
                for (var i = 0; i < graphicslayer.graphics.length; i++) {
                    var url1 = graphicslayer.graphics[i].symbol.url;
                    var url2 = YellowBigPolygonSymbol.url;
                    var geometry1 = graphicslayer.graphics[i].geometry.x == featureSet.features[0].geometry.x;
                    var geometry2 = graphicslayer.graphics[i].geometry.y == featureSet.features[0].geometry.y;
                    if (url1 == url2 && geometry1 && geometry2)
                        graphicslayer.remove(graphicslayer.graphics[i]);
                }
            }
        });
    }
}
function valvePointShow(valveId) {
    if (tbaGraphicslayer.graphics.length == 0) {
        var queryTask1 = new esri.tasks.QueryTask(valveMapLayer);
        var query1 = new esri.tasks.Query();
        query1.where = "ElementId = " + valveId;
        query1.outSpatialReference = map.spatialReference;
        query1.returnGeometry = true;
        //query1.outFields = ["CITY_NAME"];
        queryTask1.execute(query1, function (featureSet) {
            if (featureSet.features.length > 0) {
                tbaGraphicslayer.clear();
                tbaGraphicslayer.add(new esri.Graphic(featureSet.features[0].geometry, ReBigPolygonSymbol));
            }
        });
    }
}
function valvePointHide() {
    tbaGraphicslayer.clear();
}
function pollutionAnalysis(pt, elementId, elementTypeId, canNotBeclosedValves) {
    //alert(pt + ","+elementId+","+elementTypeId+","+canNotBeclosedValves);
    $('#resultDivBoxCalBut').css("display", "none");//隐藏重新计算按钮
    $('#sDivUserResult').css("display", "none");//隐藏影响用户标题
    $('#sDivPipesResult').css("display", "none");//隐藏影响管道标题
    $('#sDivValvesResult').css("display", "none");//隐藏关键阀门标题
    nodeId = elementId;
    //修改为局部变量
    var nodeType = elementTypeId;
    nodeType0 = nodeType;
    trtu_content = "";
    valveList1 = "";
    valveList2 = "";
    var trcp_content = "";
    var trcv_content = "";
    $.ajax({
        url: 'PollutionAnalysis',
        type: 'post',
        data: {
            elementId: nodeId,
            nodeType: nodeType,
            cnbValves: canNotBeclosedValves,
        },
        //beforeSend: function () {
        //    $.loading(true, '正在计算中...');
        //},
        success: function (data, response, status) {
            
            if (data.length > 0) {
                var isChangShow = false;//是否更改显示区域
                var _foundNodes = data[0];//受影响节点
                var _foundNodeTable = data[1];//受影响节点对应数据表名
                var _foundPipes = data[2];//受影响管道
                var _foundValves = data[3];//关键阀门
                var influence_user = data[4];//受影响用户信息
                if (influence_user[0][2].length>0)
                    influence_user0 = JSON.parse(influence_user[0][2]);//影响的用户信息
                if (influence_user[1][2].length > 0)
                    influence_user1 = JSON.parse(influence_user[1][2]);//关闭的管道信息
                if (influence_user[2][2].length > 0)
                    influence_user2 = JSON.parse(influence_user[2][2]);//关闭的阀门信息
                var titleArr = data[5];//受影响情况的基本信息
                var _cnbValves = data[6];//不能关闭阀门
                var _foundMeter = data[8];//地表
                let areamj = (influence_user[0][1] * 400.0 / 1000000.0).toFixed(2);
                right622_data = "<tr class='right6221'><td class='right62211'>" + influence_user[0][1] + "</td ><td class='right62211'>" + areamj + "</td></tr>";//影响用户信息
                $("#right1").html("关阀后受影响用户明细表(以地表计,共" + influence_user[0][1] + "块)");
                var titlecontent = "<div class=\"tubeBurst-title\">停水管道长度 <br /><span>" + titleArr[0] + " " + PLengthUnit + "</span><br />停水管道容积 <br /><span>" + titleArr[1] + " m³</span><br />停水用户水量 <br /><span>" + titleArr[2] + " " + FlowUnit + "</span></div>";
                setValueForDiv("resulttitleDiv", titlecontent);
                valveList1 = influence_user[0][1] + "," + areamj + "|" + titleArr[1];
                $("#right821").val(titleArr[1]);
                //setValueForDiv("resultinfluenceuserBut", rtu_content);
                var rcp_content = "<div class=\"tubeBurst-index\"><div class=\"tubeBurst-index-no\">" + influence_user[1][1] + "</div><div class=\"tubeBurst-index-info\">影响的管道</div></div>";
                setValueForDiv("resultclosePipesBut", rcp_content);
                var rcv_content = "<div class=\"tubeBurst-index\"><div class=\"tubeBurst-index-no\">" + influence_user[2][1] + "</div><div class=\"tubeBurst-index-info\">关闭的阀门</div></div>";
                setValueForDiv("resultcloseValvesBut", rcv_content);
                var sdur_content = "<div class=\"tubeBurst-stitle\"><div class=\"tubeBurst-stitle-no\">" + influence_user[0][1] + "</div><div class=\"tubeBurst-stitle-info\">影响的用户<div id=\"tbamm1\" style=\"float: right;height:30px;width:30px\"><a href=\"#\"><img src=\"Content/myImage/menu-button.png\" /></a><div id=\"tbmm1\" style=\"display:none;top: 25px;right: 0px;\" class=\"menu\"><div class=\"menu-item\" onclick=\"\"><div class=\"menu-text\">打印</div></div><div class=\"menu-sep\"></div><div class=\"menu-item\" onclick=\"tbExportFiles(1,1)\"><div class=\"menu-text\">导出CSV文件</div></div><div class=\"menu-item\" onclick=\"tbExportFiles(1,2)\"><div class=\"menu-text\">导出Excel文件</div></div></div></div></div>";
                setValueForDiv("sDivUserResult", sdur_content);
                var sdpr_content = "<div class=\"tubeBurst-stitle\"><div class=\"tubeBurst-stitle-no\">" + influence_user[1][1] + "</div><div class=\"tubeBurst-stitle-info\">影响的管道<div id=\"tbamm2\" style=\"float: right;height:30px;width:30px\"><a href=\"#\"><img src=\"Content/myImage/menu-button.png\" /></a><div id=\"tbmm2\" style=\"display:none;top: 25px;right: 0px;\" class=\"menu\"><div class=\"menu-item\" onclick=\"\"><div class=\"menu-text\">打印</div></div><div class=\"menu-sep\"></div><div class=\"menu-item\" onclick=\"tbExportFiles(2,1)\"><div class=\"menu-text\">导出CSV文件</div></div><div class=\"menu-item\" onclick=\"tbExportFiles(2,2)\"><div class=\"menu-text\">导出Excel文件</div></div></div></div></div>";
                setValueForDiv("sDivPipesResult", sdpr_content);
                var sdvr_content = "<div class=\"tubeBurst-stitle\"><div class=\"tubeBurst-stitle-no\">" + influence_user[2][1] + "</div><div class=\"tubeBurst-stitle-info\">关闭的阀门<div id=\"tbamm3\" style=\"float: right;height:30px;width:30px\"><a href=\"#\"><img src=\"Content/myImage/menu-button.png\" /></a><div id=\"tbmm3\" style=\"display:none;top: 25px;right: 0px;\" class=\"menu\"><div class=\"menu-item\" onclick=\"\"><div class=\"menu-text\">打印</div></div><div class=\"menu-sep\"></div><div class=\"menu-item\" onclick=\"tbExportFiles(3,1)\"><div class=\"menu-text\">导出CSV文件</div></div><div class=\"menu-item\" onclick=\"tbExportFiles(3,2)\"><div class=\"menu-text\">导出Excel文件</div></div></div></div></div>";
                setValueForDiv("sDivValvesResult", sdvr_content);
                $('#tbamm1').mouseenter(function (e) {
                    $('#tbmm1').css({ 'display': 'block' });
                });
                $('#tbamm1').mouseleave(function () {
                    $('#tbmm1').css({ 'display': 'none' });
                });
                $('#tbamm2').mouseenter(function (e) {
                    $('#tbmm2').css({ 'display': 'block' });
                });
                $('#tbamm2').mouseleave(function () {
                    $('#tbmm2').css({ 'display': 'none' });
                });
                $('#tbamm3').mouseenter(function (e) {
                    $('#tbmm3').css({ 'display': 'block' });
                });
                $('#tbamm3').mouseleave(function () {
                    $('#tbmm3').css({ 'display': 'none' });
                });
                for (var i0 = 0; i0 < influence_user0.length; i0++) {
                    var temp0 = influence_user0[i0];
                    trtu_content += "<tr class='right5221'><td class='right52211'>" + (i0 + 1) + "</td>" +
                        "<td class='right52211'>" + temp0.USERNAME + "</td>" +
                        "<td class='right52211'>" + temp0.Address + "</td>" +
                        "<td class='right52211'>" + temp0.Label + "</td>" +
                        "</tr>";
                    valveList2 += temp0.USERNAME + "," + temp0.Address + "," + temp0.Label + ";";
                }
                for (var i1 = 0; i1 < influence_user1.length; i1++) {
                    var temp1 = influence_user1[i1];
                    trcp_content += "<div class=\"tubeBurst-pipes-index\"><div class=\"tubeBurst-pipes-index-no\">" + (i1 + 1)
                        + "</div><div class=\"tubeBurst-pipes-index-info\"><div class=\"tubeBurst-pipes-index-info1\">"
                        + "地址：" + temp1.Physical_Address + "</div><div class=\"tubeBurst-pipes-index-info2\">"
                        + "GIS编号：" + temp1.GISID + "</div><div class=\"tubeBurst-pipes-index-info3\">"
                        + "管径：" + Number(temp1.Physical_PipeDiameter).toFixed(0) + PDiameterUnit + "</div><div class=\"tubeBurst-pipes-index-info4\">"
                        + "分区：" + temp1.Physical_ZoneID + "," + temp1.Physical_DistrictID + "," + temp1.Physical_DMAID + "," + temp1.Physical_DMAAreaID + "</div></div></div>";
                }
                for (var i2 = 0; i2 < influence_user2.length; i2++) {
                    var temp2 = influence_user2[i2];
                    trcv_content += "<tr class='right7321' ondblclick='drawPointsToMap(this,\"" + temp2.ElementId + "\")' \"><td class='right73211'>" + (i2 + 1)
                        + "</td><td class='right73211'>"
                        + temp2.Physical_Address + "</td><td class='right73211'>"
                        + temp2.GISID + "</td><td class='right73211'>"
                        + Number(temp2.Physical_Diameter).toFixed(0) + "</td></tr>";
                    valveList += temp2.ElementId + "," + temp2.Physical_Address + "," + temp2.GISID + "," + Number(temp2.Physical_Diameter).toFixed(0) + ";";//存储找到的阀门
                }
                if (minFlowPT) {
                    if (minFlowPT != null) {
                        var graphic = new esri.Graphic(minFlowPT, pictureMarkerSymbol);//显示漏水动画
                        graphicslayer.add(graphic);
                    } else {//报警监测点高亮处理
                        var queryTask1 = new esri.tasks.QueryTask(nodeMapLayer);
                        var query1 = new esri.tasks.Query();
                        query1.where = "ElementId = " + nodeId;
                        query1.outSpatialReference = map.spatialReference;
                        query1.returnGeometry = true;
                        //query1.outFields = ["CITY_NAME"];
                        queryTask1.execute(query1, function (featureSet) {
                            if (featureSet.features.length > 0) {
                                graphicslayer.add(new esri.Graphic(featureSet.features[0].geometry, RedGlowPolygonSymbol));
                                var centerPoint = featureSet.features[0].geometry;
                                var cPoint = new esri.geometry.Point();
                                cPoint.x = centerPoint.x;
                                cPoint.y = centerPoint.y;
                                cPoint.spatialReference = map.spatialReference;
                                map.centerAndZoom(cPoint, 7);
                            }
                        });
                    }
                }
                //设置线的样式
                //const symbol = new esri.symbol.SimpleFillSymbol(esri.symbol.SimpleFillSymbol.STYLE_SOLID, new esri.symbol.SimpleLineSymbol(esri.symbol.SimpleLineSymbol.STYLE_SOLID, new esri.Color([152, 106, 180]), 2), new esri.Color([152, 106, 180, 0.25]));
                ////创建线对象
                //const polygon = new esri.geometry.Polygon(map.spatialReference);
                //var points = [];
                //for (var x = 0; x < data[7].length; x++) {
                //    points.push([data[7][x].X, data[7][x].Y]);
                //}

                //polygon.addRing(points);
                //var graphic = new esri.Graphic(polygon, symbol);
                //graphicslayer.add(graphic);
                //----------------------
                var res = [];
                var tubeBurstExtent = new esri.geometry.Extent({ "xmin": pt.x, "ymin": pt.y, "xmax": pt.x, "ymax": pt.y, "spatialReference": map.spatialReference });//地图显示的新区域
                for (var i = 0; i < _foundPipes.length; i++) {//管道高亮处理
                    if (minFlowPT == null) {
                        var queryTask = new esri.tasks.QueryTask(resultPipeMapLayer);
                        var query = new esri.tasks.Query();
                        query.where = "ElementId = " + _foundPipes[i];
                        query.spatialRelationship = esri.tasks.Query.SPATIAL_REL_ENVELOPEINTERSECTS;
                        query.outSpatialReference = map.spatialReference;
                        query.returnGeometry = true;
                        query.outFields = ["Result_Flow_24"];
                        queryTask.execute(query, function (featureSet) {
                            if (featureSet.features.length == 0) {
                                $.messager.alert('获取失败！', '无此分区范围信息！', 'warning');
                                $.loading();
                                return;
                            }
                            var _tmpdiv = $("#map_layers").nextAll();
                            if (_tmpdiv.length > 0) {//显示水流方向动画效果
                                _tmpdiv.css("display", "block");
                                $("#map_layers").next().css({ "height": $("#map_root").height() + "px", "width": $("#map_root").width() + "px" });
                                //myPipeFlowChart.dispose();
                                //overlay = null;
                                //reservoirInfo();
                            }
                            layerPipeFlow0.hide();//隐藏水流方向图层
                            //legendDivClose();//关闭图例显示
                            featureSet.features.forEach(function (item, j) {
                                var tmprFlow = item.attributes.Result_Flow_24;
                                var tmpPaths = item.geometry.paths[0];
                                if (tmprFlow > 0) {
                                    for (var i = 0; i < tmpPaths.length - 1; i++) {
                                        res.push([{
                                            coord: tmpPaths[i]
                                        }, {
                                            coord: tmpPaths[i + 1]
                                        }]);
                                    }
                                } else {
                                    for (var i = 0; i < tmpPaths.length - 1; i++) {
                                        res.push([{
                                            coord: tmpPaths[i + 1]
                                        }, {
                                            coord: tmpPaths[i]
                                        }]);
                                    }
                                }
                            });
                        });
                    }
                    var queryTask1 = new esri.tasks.QueryTask(pipeMapLayer);
                    var query1 = new esri.tasks.Query();
                    query1.where = "ElementId = " + _foundPipes[i];
                    query1.outSpatialReference = map.spatialReference;
                    query1.returnGeometry = true;
                    query1.outFields = ["*"];
                    //queryTask1.execute(query1, pipeTemp);
                    queryTask1.execute(query1, function (featureSet) {
                        var infoTemplate1 = new esri.InfoTemplate("管道信息",
                            "<div class=\"display-div\"><div>元素Id:${ElementId}</div>" +
                            "<div>GIS编号:${GISID}</div>" +
                            "<div>标识:${Label}</div>" +
                            "<div>管径:${Physical_PipeDiameter}" + PDiameterUnit + "</div>" +
                            "<div>管长:${Physical_Length}" + PLengthUnit + "</div>" +
                            "<div>海曾威廉系数:${Physical_HazenWilliamsC}</div>" +
                            "<div>安装年代:${Physical_InstallationYear}</div>" +
                            "<div>地址:${Physical_Address}</div>" +
                            "<div>供水公司:${Physical_ZoneID}</div>" +
                            "<div>营销公司:${Physical_DistrictID}</div></div>"
                            //+"<div>计量分区:${Physical_DMAID}</div>"
                            );
                        dojo.connect(graphicslayer, "onClick", function (evt) {
                            //if (evt.graphic.attributes["ElementTypeId"] == 69) {
                            //    evt.graphic.attributes["Physical_Length"] = Number(evt.graphic.attributes["Physical_Length"]).toFixed(2);
                            //}
                            var tmpIa = evt.graphic.attributes["Is_Active"] == 0 ? "停用" : "启用";
                            if (tmpIa != null)
                                evt.graphic.attributes["Is_Active"] = tmpIa;
                            var tmpId = zoneClass.getValue(evt.graphic.attributes["Physical_ZoneID"]);
                            if (tmpId != null)
                                evt.graphic.attributes["Physical_ZoneID"] = tmpId;
                            var tmpId = DMAClass.getValue(evt.graphic.attributes["Physical_DMAID"])
                            if (tmpId != null)
                                evt.graphic.attributes["Physical_DMAID"] = tmpId;
                            var tmpId = districtClass.getValue(evt.graphic.attributes["Physical_DistrictID"]);
                            if (tmpId != null)
                                evt.graphic.attributes["Physical_DistrictID"] = tmpId;
                        });
                        map.infoWindow.resize(260, 275);
                        //map.infoWindow.resize(250, 220);
                        //dojo.forEach(featureSet.features, function (feature) {
                        //    graphicslayer.add(feature.setSymbol(outline).setInfoTemplate(infoTemplate1));
                        //});
                        //if (featureSet.features.length > 0) {
                        //    isChangShow = true;
                        //}
                        if (featureSet.features.length > 0) {
                            isChangShow = true;
                            graphicslayer.add(featureSet.features[0].setSymbol(outline).setInfoTemplate(infoTemplate1));
                            var xmin = featureSet.features[0].geometry.cache._extent.xmin;
                            var xmax = featureSet.features[0].geometry.cache._extent.xmax;
                            var ymin = featureSet.features[0].geometry.cache._extent.ymin;
                            var ymax = featureSet.features[0].geometry.cache._extent.ymax;
                            if (xmin < tubeBurstExtent.xmin) tubeBurstExtent.xmin = xmin;
                            if (xmax > tubeBurstExtent.xmax) tubeBurstExtent.xmax = xmax;
                            if (ymin < tubeBurstExtent.ymin) tubeBurstExtent.ymin = ymin;
                            if (ymax > tubeBurstExtent.ymax) tubeBurstExtent.ymax = ymax;
                        }
                    });
                }
                if (_foundNodes.length > 0) {//节点高亮处理
                    var queryTask1 = new esri.tasks.QueryTask(nodeMapLayer);
                    var query1 = new esri.tasks.Query();
                    query1.where = "ElementId in (" + _foundNodes.join(',') + ")";
                    query1.outSpatialReference = map.spatialReference;
                    query1.returnGeometry = true;
                    //query1.outFields = ["*"];
                    query1.outFields = ["ElementId", "GISID", "Label", "Physical_Depth", "Physical_Elevation", "Physical_Address", "Physical_ZoneID", "Physical_DistrictID", "Physical_DMAID"];
                    queryTask1.execute(query1, function (featureSet) {
                        var infoTemplate3 = new esri.InfoTemplate("节点信息",
                            "<div class=\"display-div\"><div>元素Id : ${ElementId}</div>" +
                            "<div>GIS编号:${GISID}</div>" +
                            "<div>标识:${Label}</div>" +
                            "<div>埋深:${Physical_Depth}</div>" +
                            "<div>高程:${Physical_Elevation}</div>" +
                            "<div>地址:${Physical_Address}</div>" +
                            "<div>供水公司:${Physical_ZoneID}</div>" +
                            "<div>营销公司:${Physical_DistrictID}</div></div>"
                            //+"<div>计量分区:${Physical_DMAID}</div>"
                        );
                        for (var i = 0; i < featureSet.features.length; i++) {
                            isChangShow = true;
                            graphicslayer.add(featureSet.features[i].setSymbol(SmallPolygonSymbol).setInfoTemplate(infoTemplate3));
                            var xmin = featureSet.features[i].geometry.x;
                            var xmax = featureSet.features[i].geometry.x;
                            var ymin = featureSet.features[i].geometry.y;
                            var ymax = featureSet.features[i].geometry.y;
                            if (xmin < tubeBurstExtent.xmin) tubeBurstExtent.xmin = xmin;
                            if (xmax > tubeBurstExtent.xmax) tubeBurstExtent.xmax = xmax;
                            if (ymin < tubeBurstExtent.ymin) tubeBurstExtent.ymin = ymin;
                            if (ymax > tubeBurstExtent.ymax) tubeBurstExtent.ymax = ymax;
                        }
                    });
                }
                if (_foundMeter.length > 0) {//地表高亮处理
                    var queryTask1 = new esri.tasks.QueryTask(meter_DServerLayer);
                    var query1 = new esri.tasks.Query();
                    query1.where = "ELEMENTID in (" + _foundMeter.join(',') + ")";
                    query1.outSpatialReference = map.spatialReference;
                    query1.returnGeometry = true;
                    query1.outFields = ["*"];
                    //query1.outFields = ["ElementId", "GISID", "Label", "Physical_Depth", "Physical_Elevation", "Physical_Address", "Physical_ZoneID", "Physical_DistrictID", "Physical_DMAID"];
                    queryTask1.execute(query1, function (featureSet) {
                        //var infoTemplate3 = new esri.InfoTemplate("节点信息",
                        //    "<div class=\"display-div\"><div>元素Id : ${ElementId}</div>" +
                        //    "<div>GIS编号:${GISID}</div>" +
                        //    "<div>标识:${Label}</div>" +
                        //    "<div>埋深:${Physical_Depth}</div>" +
                        //    "<div>高程:${Physical_Elevation}</div>" +
                        //    "<div>地址:${Physical_Address}</div>" +
                        //    "<div>供水公司:${Physical_ZoneID}</div>" +
                        //    "<div>营销公司:${Physical_DistrictID}</div></div>"
                        //    //+"<div>计量分区:${Physical_DMAID}</div>"
                        //);
                        for (var i = 0; i < featureSet.features.length; i++) {
                            isChangShow = true;
                            graphicslayer.add(featureSet.features[i].setSymbol(YellowBigPolygonSymbol));
                            var xmin = featureSet.features[i].geometry.x;
                            var xmax = featureSet.features[i].geometry.x;
                            var ymin = featureSet.features[i].geometry.y;
                            var ymax = featureSet.features[i].geometry.y;
                            if (xmin < tubeBurstExtent.xmin) tubeBurstExtent.xmin = xmin;
                            if (xmax > tubeBurstExtent.xmax) tubeBurstExtent.xmax = xmax;
                            if (ymin < tubeBurstExtent.ymin) tubeBurstExtent.ymin = ymin;
                            if (ymax > tubeBurstExtent.ymax) tubeBurstExtent.ymax = ymax;
                        }
                    });
                }
                for (var i = 0; i < _foundValves.length; i++) {//阀门高亮处理
                    var str = "ElementId = " + _foundValves[i];
                    var queryTask1 = new esri.tasks.QueryTask(valveMapLayer);
                    var query1 = new esri.tasks.Query();
                    query1.where = str;
                    query1.outSpatialReference = map.spatialReference;
                    query1.returnGeometry = true;
                    query1.outFields = ["*"];
                    queryTask1.execute(query1, function(featureSet) {
                        var infoTemplate2 = new esri.InfoTemplate("阀门信息",
                            "<div class=\"display-div\"><div>元素Id:${ElementId}</div>" +
                            "<div>GIS编号:${GISID}</div>" +
                            "<div>标识:${Label}</div>" +
                            "<div>埋深:${Physical_Depth}" + JDepthUnit + "</div>" +
                            "<div>口径:${Physical_Diameter}" + PDiameterUnit + "</div>" +
                            "<div>地址:${Physical_Address}</div>" +
                            "<div>供水公司:${Physical_ZoneID}</div>" +
                            "<div>营销公司:${Physical_DistrictID}</div></div>"
                            //+"<div>计量分区:${Physical_DMAID}</div>"
                            );
                        if (featureSet.features.length > 0) {
                            isChangShow = true;
                            graphicslayer.add(featureSet.features[0].setSymbol(RedBigPolygonSymbol).setInfoTemplate(infoTemplate2));
                            var xmin = featureSet.features[0].geometry.x;
                            var xmax = featureSet.features[0].geometry.x;
                            var ymin = featureSet.features[0].geometry.y;
                            var ymax = featureSet.features[0].geometry.y;
                            if (xmin < tubeBurstExtent.xmin) tubeBurstExtent.xmin = xmin;
                            if (xmax > tubeBurstExtent.xmax) tubeBurstExtent.xmax = xmax;
                            if (ymin < tubeBurstExtent.ymin) tubeBurstExtent.ymin = ymin;
                            if (ymax > tubeBurstExtent.ymax) tubeBurstExtent.ymax = ymax;
                        }
                    });
                }
                for (var i = 0; i < _cnbValves.length; i++) {//无法关闭阀门高亮处理
                    var queryTask1 = new esri.tasks.QueryTask(valveMapLayer);
                    var query1 = new esri.tasks.Query();
                    query1.where = "ElementId = " + _cnbValves[i];
                    query1.outSpatialReference = map.spatialReference;
                    query1.returnGeometry = true;
                    //query1.outFields = ["CITY_NAME"];
                    queryTask1.execute(query1, function (featureSet) {
                        var infoTemplate3 = new esri.InfoTemplate("阀门信息",
                            "<div class=\"display-div\"><div>元素Id:${ElementId}</div>" +
                            "<div>GIS编号:${GISID}</div>" +
                            "<div>标识:${Label}</div>" +
                            "<div>埋深:${Physical_Depth}" + JDepthUnit + "</div>" +
                            "<div>口径:${Physical_Diameter}" + PDiameterUnit + "</div>" +
                            "<div>地址:${Physical_Address}</div>" +
                            "<div>供水公司:${Physical_ZoneID}</div>" +
                            "<div>营销公司:${Physical_DistrictID}</div></div>"
                            //+"<div>计量分区:${Physical_DMAID}</div></div>"
                            );
                        if (featureSet.features.length > 0) {
                            isChangShow = true;
                            //graphicslayer.add(new esri.Graphic(featureSet.features[0].geometry, YellowBigPolygonSymbol));
                            graphicslayer.add(featureSet.features[0].setSymbol(YellowBigPolygonSymbol).setInfoTemplate(infoTemplate3));
                            var xmin = featureSet.features[0].geometry.x;
                            var xmax = featureSet.features[0].geometry.x;
                            var ymin = featureSet.features[0].geometry.y;
                            var ymax = featureSet.features[0].geometry.y;
                            if (xmin < tubeBurstExtent.xmin) tubeBurstExtent.xmin = xmin;
                            if (xmax > tubeBurstExtent.xmax) tubeBurstExtent.xmax = xmax;
                            if (ymin < tubeBurstExtent.ymin) tubeBurstExtent.ymin = ymin;
                            if (ymax > tubeBurstExtent.ymax) tubeBurstExtent.ymax = ymax;
                        }
                    });
                }
                if (_foundValves.length > 0) {
                    for (var i = 0; i < 1; i++) {//控制计算顺序
                        var queryTask1 = new esri.tasks.QueryTask(valveMapLayer);
                        var query1 = new esri.tasks.Query();
                        query1.where = "ElementId = " + _foundValves[0];
                        query1.outSpatialReference = map.spatialReference;
                        query1.returnGeometry = true;
                        queryTask1.execute(query1, function (featureSet) {
                            //tubeBurstExtent.xmin += 3;
                            //tubeBurstExtent.xmax += 3;
                            //tubeBurstExtent.ymin += 3;
                            //tubeBurstExtent.ymax += 3;
                            var w_h = map.width / map.height;
                            var theight = (tubeBurstExtent.xmax - tubeBurstExtent.xmin) / w_h;
                            var twidth = (tubeBurstExtent.ymax - tubeBurstExtent.ymin) * w_h;
                            var wheight = (tubeBurstExtent.ymax - tubeBurstExtent.ymin);
                            var wwidth = (tubeBurstExtent.xmax - tubeBurstExtent.xmin);
                            if (theight > wheight) {//根据显示区域长宽比调整缩放位置
                                tubeBurstExtent.ymax = tubeBurstExtent.ymax + (theight - wheight / 2) + 230 / w_h;
                                tubeBurstExtent.ymin = tubeBurstExtent.ymin - (theight - wheight / 2);
                            } else {
                                tubeBurstExtent.xmax = tubeBurstExtent.xmax + (twidth - wwidth / 2) + 230;
                                tubeBurstExtent.xmin = tubeBurstExtent.xmin - (twidth - wwidth / 2);
                            }
                            if (isChangShow) {
                                map.setExtent(tubeBurstExtent, false);
                            }
                            var color = ['#a6c84c', '#ffa022', '#46bee9', '#FF4500'];
                            var series = [];
                            series.push({
                                type: 'lines',
                                effect: {
                                    show: true,
                                    period: 3,
                                    color: color[0],
                                    symbolSize: 9
                                },
                                lineStyle: {
                                    normal: {
                                        color: color[1],
                                        width: 1.5,
                                    }
                                },
                                data: res
                            },
                                {
                                    type: 'scatter',
                                    coordinateSystem: 'geo'
                                });

                            option = {
                                //        backgroundColor: '#404a59',
                                geo: {
                                    roam: false,
                                },
                                animation: false,
                                series: series
                            };
                            // 使用刚指定的配置项和数据显示图表。
                            myPipeFlowChart.resize();
                            myPipeFlowChart.setOption(option);
                        });
                    }
                }
            } else {
                $.loading();
                $.modalAlert("系统提示", '未知错误导致失败，请重试！');
            }
        },
        complete: function () {
            //$("#right622_data").html("<tr class='right6221'><td class='right62211'>" + trtu_content +"</td ><td class='right62211'>02.26</td></tr>");//影响用户信息
            setValueForDiv("resultclosePipesDiv", trcp_content);//影响管道信息
            right732_data = trcv_content;//关闭阀门信息
            //$(".right732_data").html(trcv_content);//关闭阀门信息
            $('#resultDivBoxBackBut').css("display", "none");//隐藏返回按钮
            $('#resultinfluenceuserDiv').css("display", "none");//隐藏影响用户信息显示
            $('#resultclosePipesDiv').css("display", "none");//隐藏影响管道信息显示
            $('#resultcloseValvesDiv').css("display", "none");//隐藏关键阀门信息显示
            $('#resultDiv').css("display", "");//显示爆管分析结果列表
            $('#showPA').removeClass('disabled');
            $("#showPA").attr('disabled', false);
            $('#pipeFlowBut').removeClass('disabled');
            $("#pipeFlowBut").attr('disabled', false);
            $('#createPlanBut').removeClass('disabled');
            $("#createPlanBut").attr('disabled', false);
            $('#showPAge').removeClass('disabled');
            $("#showPAge").attr('disabled', false);
            $.loading();
        },
       error: function (XMLHttpRequest, textStatus, errorThrown) {
            //$.messager.progress('close');
            //$.modalAlert("系统提示", '无数据或数据解析错误！');
        }
    });
}
function drawPointsToMap(node, val) {
    let arr = val.split(",")
    var title = "阀门信息";
    var picAddr = "timg_blue";
    var colors = color1;
    var unit = PressUnit;
    var query = new esri.tasks.Query();
    var queryTask = new esri.tasks.QueryTask(valveMapLayer);
    query.where = "ElementId = " + arr[0];
    query.outSpatialReference = map.spatialReference;
    query.returnGeometry = true;
    query.outFields = ["*"];
    queryTask.execute(query, function (featureSet) {
        if (featureSet.features.length == 0) {
            $.modalMsg("未找到该元素！", "warning");
            //$.messager.progress('close');
            return;
        }

        var centerPoint = featureSet.features[0].geometry;
        var text = featureSet.features[0].attributes;

        //开始画，画成能够在页面上显示数字的形式。
        var point = new esri.geometry.Point(centerPoint.x, centerPoint.y, map.spatialReference);
        // 定义自变量
        let smsLength = ((arr[0] + '阀门').length * 14) + 5;//背景图片宽
        let smsLevel = smsLength * 50 / 180;    //图片水平偏移
        let textLevel = smsLength * 49 / 180;       //文字水平偏移
        var textSymbol1 = new esri.symbol.TextSymbol((arr[0] + '阀门'), font1, colors).setOffset(textLevel, 15.5556);
        // var textSymbol2 = new esri.symbol.TextSymbol(value.toString(), font1, colors).setOffset(47, 20);
        let val2 = (text.Physical_Status == 0 ? "开启" : text.Physical_Status == 0 ? "开启" : "关闭")
        var sms = new esri.symbol.PictureMarkerSymbol("/Areas/DataDisplay/Content/img/" + picAddr + ".png", smsLength, 32).setOffset(smsLevel, 15);
        var infoTemplate = new esri.InfoTemplate();
        infoTemplate.setTitle("<div>" + title + "</div>");
        infoTemplate.setContent("<div>元素Id:" + text.ElementId + "</div>" +
            "<div>GIS编号:" + text.GISID + "</div>" +
            //"<div>标识:${Label}</div>" +
            "<div>埋深:" + text.Physical_Depth + "" + JDepthUnit + "</div>" +
            "<div>口径:" + text.Physical_Diameter + "" + PDiameterUnit + "</div>" +
            "<div>地址:" + text.Physical_Address + "</div>" +
            "<div>供水公司:" + zoneClass.getValue(text.Physical_ZoneID) + "</div>" +
            "<div>营销公司:" + districtClass.getValue(text.Physical_DistrictID) + "</div>" +
            //"<div>计量分区:${Physical_DMAID}</div>" +
            "<div>状态:" + val2 + "</div>");
        map.infoWindow.resize(240, 190);
        var graphic = new esri.Graphic(point, SmallQualitySymbol);
        var graphic1 = new esri.Graphic(point, textSymbol1);
        // var graphic2 = new esri.Graphic(point, textSymbol2);
        var graphic3 = new esri.Graphic(point, sms);
        graphicslayer.add(graphic3.setInfoTemplate(infoTemplate));
        graphicslayer.add(graphic1.setInfoTemplate(infoTemplate));
        graphicslayer.add(graphic);
        // graphicslayer.add(graphic2.setInfoTemplate(infoTemplate));
        map.centerAndZoom(point, 7);
        //$.messager.progress('close');
    });

}

function tbExportFiles(s1,s2){
    switch (s1) {
        case 1:
            var data = JSON.stringify(influence_user0);
            if (data == '')
                return;
            data = data.replace(/ElementId/g, '元素ID').replace(/WaterMeter_ID/g, '水表编号').replace(/CALIBER/g, '表径').replace(/Address/g, '地址').replace(/USERNAME/g, '用户')
                .replace(/GISID/g, 'GIS编号').replace(/Label/g, '所属标识').replace(/null/g, '"无"');
            switch (s2) {
                case 1:
                    JSONToCSVConvertor(data, '影响用户', true);
                    break;
                case 2:
                    JSONToEXLConvertor(data, '影响用户', true);
                    break;
            }
            break;
        case 2:
            var data = JSON.stringify(influence_user1);
            if (data == '')
                return;
            data = data.replace(/ElementId/g, '元素ID').replace(/GISID/g, 'GIS编号').replace(/Label/g, '标识').replace(/StartNodeLabel/g, '起始端标识').replace(/StartNodeType/g, '起始端类型')
                .replace(/EndNodeLabel/g, '终止端标识').replace(/EndNodeType/g, '终止端类型').replace(/Physical_PipeDiameter/g, '管径').replace(/Physical_PipeMaterialID/g, '材质')
                .replace(/Physical_Length/g, '管长').replace(/Physical_InstallationYear/g, '竣工时间').replace(/Physical_Address/g, '地址').replace(/Physical_ZoneID/g, '所属公司')
                .replace(/Physical_DistrictID/g, '所属营业公司').replace(/Physical_DMAID/g, '所属分区').replace(/Physical_DMAAreaID/g, 'DMA分区').replace(/Is_Active/g, '状态').replace(/null/g, '"无"');
            switch (s2) {
                case 1:
                    JSONToCSVConvertor(data, '影响管道', true);
                    break;
                case 2:
                    JSONToEXLConvertor(data, '影响管道', true);
                    break;
            }
            break;
        case 3:
            var data = JSON.stringify(influence_user2);
            if (data == '')
                return;
            data = data.replace(/ElementId/g, '元素ID').replace(/GISID/g, 'GIS编号').replace(/Label/g, '标识').replace(/Physical_Elevation/g, '高程').replace(/Physical_Depth/g, '埋深')
                .replace(/Physical_Diameter/g, '口径').replace(/Physical_Address/g, '地址').replace(/Physical_ZoneID/g, '所属公司').replace(/Physical_DistrictID/g, '所属营业公司')
                .replace(/Physical_DMAID/g, '所属分区').replace(/Physical_DMAAreaID/g, 'DMA分区').replace(/Physical_InstallationYear/g, '竣工时间').replace(/Is_Active/g, '状态').replace(/null/g, '"无"');
            switch (s2) {
                case 1:
                    JSONToCSVConvertor(data, '影响阀门', true);
                    break;
                case 2:
                    JSONToEXLConvertor(data, '影响阀门', true);
                    break;
            }
            break;
    }

}
