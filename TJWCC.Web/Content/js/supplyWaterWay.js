var supplyWaterChart;
function supplyWaterWayClick() {//供水路径单击事件
    //$("#fixed_list>li").removeAttr("id");
    //$(".nav-title").removeAttr("id");
    //$(".auxiliary_null").html("");
    if (isSupplyWaterWay) {
        recoverAll();
        hideAndClear();
    } else {
        recoverAll();
        hideAndClear();
        // 改变按钮选中颜色
        $("#nav-serve").find(".nav-title").attr("id", "check-nav");
        isSupplyWaterWay = true;//是否供水路径标识
        setValueForDiv("followMouseInfo", "双击地图计算供水路径")
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
var isNodePontShow = true;
function supplyWaterWay(pt, elementId, elementTypeId) {
    $.ajax({
        url: 'SupplyWaterWay',
        type: 'post',
        data: {
            elementId: elementId,
            nodeType: elementTypeId,
            swwType: true,
        },
        //beforeSend: function () {
        //    $.messager.progress({text: '正在计算中...'});
        //},
        success: function (data, response, status) {
            if (data.length > 0) {
                var isChangShow = false;
                var PipeNoArr_pipeno = data[0]; //供水路径途径管道集合
                var PipeNoArr_node = data[1];//供水路径途径元素集合(不包括含初始元素)
                var PipeNoArr_nodetable = data[2];//供水路径途径元素对应数据表集合
                //if (isSupplyWaterWay) {
                //    var NodeElevResult = data[3];//途径元素的高程数据
                //    var NodeHGLResult = data[4];//途径元素的水力坡度数据
                    //NodeElevResult[0] = Number((NodeElevResult[0] - 26).toFixed(2));//修正水厂自由水压
                    //NodeHGLResult[0] = Number((NodeHGLResult[0] + 6).toFixed(2));//修正水厂自由水压
                //}
                var pipeNum = data[5];//途径管道数量
                var findnode = data[6];//水源ID
                sourceName = data[7];//水源label名
                var findtype = data[8];//水源类型
                var maxPress = data[9]+0.1;//最大压力
                var minPress = data[10] - 0.1;//最小压力
                polage = data[11];
                var pipeLength = 0.0;
                let strde = "Result_Source_7 ='" + sourceName + "' AND Result_Age_7<" + polage;
                PollutionAge = new esri.layers.FeatureLayer(ArcGISServer + "PollutionAge/MapServer/0", {
                    id: "水质超标范围",
                    outFields: ["Result_Age_7","Result_Source_7"],
                    infoTemplate: infoTemplatePoint,
                    definitionExpression: strde,
                    visible: false
                });//水质超标范围
                map.addLayer(PollutionAge);
                var supplyWaterExtent = new esri.geometry.Extent({ "xmin": pt.x, "ymin": pt.y, "xmax": pt.x, "ymax": pt.y, "spatialReference": map.spatialReference });//地图显示的新区域
                if (PipeNoArr_pipeno.length > 0) {//管道高亮处理
                    var queryTask1 = new esri.tasks.QueryTask(pipeMapLayer);
                    var query1 = new esri.tasks.Query();
                    query1.where = "ElementId in (" + PipeNoArr_pipeno.join(',') + ")";
                    query1.outSpatialReference = map.spatialReference;
                    query1.returnGeometry = true;
                    query1.outFields = ["ElementId","ElementTypeId", "GISID", "Label", "Physical_PipeDiameter", "Physical_PipeMaterialID", "Physical_Length", "Physical_HazenWilliamsC", "Physical_InstallationYear"
                        , "Physical_Address", "Physical_ZoneID", "Physical_DistrictID", "Physical_DMAID"];
                    queryTask1.execute(query1, function (featureSet) {
                        var infoTemplate1 = new esri.InfoTemplate("管道信息",
                            "<div class=\"display-div\"><div>元素Id:${ElementId}</div>" +
                            "<div>GIS编号:${GISID}</div>" +
                            "<div>标识:${Label}</div>" +
                            "<div>管径:${Physical_PipeDiameter}" + PDiameterUnit + "</div>" +
                            "<div>管材:${Physical_PipeMaterialID}</div>" +
                            "<div>管长:${Physical_Length}" + PLengthUnit + "</div>" +
                            "<div>海曾威廉系数:${Physical_HazenWilliamsC}</div>" +
                            "<div>安装年代:${Physical_InstallationYear}</div>" +
                            "<div>地址:${Physical_Address}</div>" +
                            "<div>供水公司:${Physical_ZoneID}</div>" +
                            "<div>营销公司:${Physical_DistrictID}</div></div>"
                            //+"<div>计量分区:${Physical_DMAID}</div>"
                        );
                        //dojo.connect(graphicslayer, "onClick", function (evt) {
                        //    if (evt.graphic.attributes["ElementTypeId"] == 69) {
                        //        evt.graphic.attributes["Physical_Length"] = Number(evt.graphic.attributes["Physical_Length"]).toFixed(2);
                        //    }
                        //    var tmpIa = evt.graphic.attributes["Is_Active"] == 0 ? "停用" : "启用";
                        //    if (tmpIa != null)
                        //        evt.graphic.attributes["Is_Active"] = tmpIa;
                        //    var tmpId = zoneClass.getValue(evt.graphic.attributes["Physical_ZoneID"]);
                        //    if (tmpId != null)
                        //        evt.graphic.attributes["Physical_ZoneID"] = tmpId;
                        //    var tmpId = DMAClass.getValue(evt.graphic.attributes["Physical_DMAID"])
                        //    if (tmpId != null)
                        //        evt.graphic.attributes["Physical_DMAID"] = tmpId;
                        //    var tmpId = districtClass.getValue(evt.graphic.attributes["Physical_DistrictID"]);
                        //    if (tmpId != null)
                        //        evt.graphic.attributes["Physical_DistrictID"] = tmpId;
                        //    var tmpId = MaterialClass.getValue(evt.graphic.attributes["Physical_PipeMaterialID"]);
                        //    if (tmpId != null)
                        //        evt.graphic.attributes["Physical_PipeMaterialID"] = tmpId;
                        //    map.infoWindow.resize(260, 275);
                        //});
                        for (var i = 0; i < featureSet.features.length; i++) {
                            isChangShow = true;
                            graphicslayer.add(featureSet.features[i].setSymbol(outline).setInfoTemplate(infoTemplate1));
                            pipeLength += featureSet.features[i].attributes.Physical_Length;
                            var xmin = featureSet.features[i].geometry.cache._extent.xmin;
                            var xmax = featureSet.features[i].geometry.cache._extent.xmax;
                            var ymin = featureSet.features[i].geometry.cache._extent.ymin;
                            var ymax = featureSet.features[i].geometry.cache._extent.ymax;
                            if (xmin < supplyWaterExtent.xmin) supplyWaterExtent.xmin = xmin;
                            if (xmax > supplyWaterExtent.xmax) supplyWaterExtent.xmax = xmax;
                            if (ymin < supplyWaterExtent.ymin) supplyWaterExtent.ymin = ymin;
                            if (ymax > supplyWaterExtent.ymax) supplyWaterExtent.ymax = ymax;
                        }
                    });
                }
                if (PipeNoArr_node.length > 0) {//阀门高亮处理
                    var queryTask1 = new esri.tasks.QueryTask(valveMapLayer);
                    var query1 = new esri.tasks.Query();
                    query1.where = "ElementId in (" + PipeNoArr_node.join(',') + ")";
                    query1.outSpatialReference = map.spatialReference;
                    query1.returnGeometry = true;
                    query1.outFields = ["ElementId", "GISID", "Label", "Physical_Depth", "Physical_Diameter", "Physical_Address", "Physical_ZoneID", "Physical_DistrictID", "Physical_DMAID"];
                    queryTask1.execute(query1, function (featureSet) {
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
                        for (var i = 0; i < featureSet.features.length; i++) {
                            isChangShow = true;
                            graphicslayer.add(featureSet.features[i].setSymbol(BigPolygonSymbol).setInfoTemplate(infoTemplate2));
                            var xmin = featureSet.features[i].geometry.x;
                            var xmax = featureSet.features[i].geometry.x;
                            var ymin = featureSet.features[i].geometry.y;
                            var ymax = featureSet.features[i].geometry.y;
                            if (xmin < supplyWaterExtent.xmin) supplyWaterExtent.xmin = xmin;
                            if (xmax > supplyWaterExtent.xmax) supplyWaterExtent.xmax = xmax;
                            if (ymin < supplyWaterExtent.ymin) supplyWaterExtent.ymin = ymin;
                            if (ymax > supplyWaterExtent.ymax) supplyWaterExtent.ymax = ymax;
                        }
                    });
                }
                if (PipeNoArr_node.length > 0) {//节点高亮处理
                    var queryTask1 = new esri.tasks.QueryTask(nodeMapLayer);
                    var query1 = new esri.tasks.Query();
                    query1.where = "ElementId in (" + PipeNoArr_node.join(',') + ")";
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
                            if (xmin < supplyWaterExtent.xmin) supplyWaterExtent.xmin = xmin;
                            if (xmax > supplyWaterExtent.xmax) supplyWaterExtent.xmax = xmax;
                            if (ymin < supplyWaterExtent.ymin) supplyWaterExtent.ymin = ymin;
                            if (ymax > supplyWaterExtent.ymax) supplyWaterExtent.ymax = ymax;
                        }
                    });
                }
                if (findtype == 68)
                    var queryTask1 = new esri.tasks.QueryTask(pumpMapLayer);
                else
                    var queryTask1 = new esri.tasks.QueryTask(reservoirMapLayer);
                var query1 = new esri.tasks.Query();
                query1.where = "ElementId = " + findnode;
                query1.outSpatialReference = map.spatialReference;
                query1.returnGeometry = true;
                queryTask1.execute(query1, function (featureSet) {//气泡显示源头信息
                    $('#waterSource').html('水源:' + sourceName);
                    PinTextSymbol.setText(sourceName);
                    PinTextSymbol.setColor(new dojo.Color([255, 255, 255]));
                    PinTextSymbol.setOffset(10, 20);
                    if (featureSet.features.length > 0) {
                        if (findtype == 68)
                            graphicslayer.add(new esri.Graphic(featureSet.features[0].geometry, PumpPolygonSymbol));
                        else
                            graphicslayer.add(new esri.Graphic(featureSet.features[0].geometry, ReserviorPolygonSymbol));
                        graphicslayer.add(new esri.Graphic(featureSet.features[0].geometry, BlackinfoPictureMSymbol));
                        graphicslayer.add(new esri.Graphic(featureSet.features[0].geometry, PinTextSymbol));
                    }
                });
                //$.messager.alert('计算完成！', '管道总长度：' + pipeLength, 'info');
            } else {
                hideAndClear();
                $.modalAlert("系统提示", '未知错误导致失败，请重试！');
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            hideAndClear();
            $.modalAlert("系统提示", '无数据或数据解析错误！');
            //TODO
        }
    });
}
function nodePointIn(nodeId) {
    tbaGraphicslayer.clear();
    var PolygonSymbol1 = new esri.symbol.SimpleMarkerSymbol().setSize(12).setColor(new dojo.Color([0, 0, 255]));
    var queryTask1 = new esri.tasks.QueryTask(nodeMapLayer);
    var query1 = new esri.tasks.Query();
    query1.where = "ElementId = " + nodeId;
    query1.outSpatialReference = map.spatialReference;
    query1.returnGeometry = true;
    //query1.outFields = ["CITY_NAME"];
    queryTask1.execute(query1, function (featureSet) {
        if (featureSet.features.length > 0) {
            tbaGraphicslayer.add(new esri.Graphic(featureSet.features[0].geometry, PolygonSymbol1));
            var centerPoint = featureSet.features[0].geometry;
            var cPoint = new esri.geometry.Point();
            cPoint.x = centerPoint.x;
            cPoint.y = centerPoint.y;
            cPoint.spatialReference = map.spatialReference;
            map.centerAndZoom(cPoint, 7);
        }
        isNodePontShow = true;
    });
    var queryTask1 = new esri.tasks.QueryTask(valveMapLayer);
    var query1 = new esri.tasks.Query();
    query1.where = "ElementId = " + nodeId;
    query1.outSpatialReference = map.spatialReference;
    query1.returnGeometry = true;
    //query1.outFields = ["CITY_NAME"];
    queryTask1.execute(query1, function (featureSet) {
        if (featureSet.features.length > 0) {
            tbaGraphicslayer.add(new esri.Graphic(featureSet.features[0].geometry, PolygonSymbol1));
            var centerPoint = featureSet.features[0].geometry;
            var cPoint = new esri.geometry.Point();
            cPoint.x = centerPoint.x;
            cPoint.y = centerPoint.y;
            cPoint.spatialReference = map.spatialReference;
            map.centerAndZoom(cPoint, 7);
        }
        isNodePontShow = true;
    });
}
function nodePointOut() {
    tbaGraphicslayer.clear();
}
