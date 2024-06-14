var resultdata;
function intervalClear() {
    dscFlag = 0;
    dszdFlag = 0;
    dsylFlag = 0;
    dsllFlag = 0;
    dsysFlag = 0;
}
function drawPointsToMap(id, biaohao, name, leixing, value, date, num, isMove) {
    switch (num) {
        case 1:
            var title = "压力监测数据";
            var picAddr = "timg_blue";
            var colors = color1;
            var unit = PressUnit;
            break;
        case 2:
            var title = "余氯监测数据";
            var picAddr = "timg_blue";
            var colors = color2;
            var unit = ClUnit;
            break;
        case 3:
            var title = "浊度监测数据";
            var picAddr = "timg_blue";
            var colors = color2;
            var unit = TurbUnit;
            break;
        case 4:
        case 41:
            var title = "流量监测数据";
            var picAddr = "timg_blue";
            var colors = color3;
            var unit = FlowUnit;
            break;
        case 5:
            var title = "事件预警信息";
            var picAddr = "timg_blue";
            var colors = color1;
            var unit = JElevationUnit;
            break;
        case 6:
            var title = "水质预警信息";
            var picAddr = "timg_blue";
            var colors = color2;
            var unit = TurbUnit;
            break;
        case 7:
            var title = "水质预警信息";
            var picAddr = "timg_blue";
            var colors = color2;
            var unit = ClUnit;
            break;
        default:
            var title = leixing+"信息";
            var picAddr = "timg_blue";
            var colors = color2;
            var unit = ClUnit;
            break;
    }
    if (biaohao == null) biaohao = "无";
    var query = new esri.tasks.Query();
    if (num == 41)
        var queryTask = new esri.tasks.QueryTask(sourceNodeMapLayer);
    else
        var queryTask = new esri.tasks.QueryTask(nodeMapLayer);
    query.where = "ElementId = " + id;
    query.outSpatialReference = map.spatialReference;
    query.returnGeometry = true;
    query.outFields = ["*"];
    queryTask.execute(query, function (featureSet) {
        if (featureSet.features.length == 0) {
            $.modalMsg("未找到该元素！", "warning");
            $.messager.progress('close');
            return;
        }
        var centerPoint = featureSet.features[0].geometry;
        //开始画，画成能够在页面上显示数字的形式。
        var point = new esri.geometry.Point(centerPoint.x, centerPoint.y, map.spatialReference);
        // 定义自变量
        let smsLength = (name.length * 14) + 5;//背景图片宽
        let smsLevel = smsLength * 50 / 180;    //图片水平偏移
        let textLevel = smsLength * 49 / 180;       //文字水平偏移
        var textSymbol1 = new esri.symbol.TextSymbol(name, font1, colors).setOffset(textLevel, 15.5556);
        // var textSymbol2 = new esri.symbol.TextSymbol(value.toString(), font1, colors).setOffset(47, 20);

        var sms = new esri.symbol.PictureMarkerSymbol("/Areas/DataDisplay/Content/img/" + picAddr + ".png", smsLength, 32).setOffset(smsLevel, 15);
        var infoTemplate = new esri.InfoTemplate();
        infoTemplate.setTitle("<div>" + title + "</div>");
        if (num == 5)
            infoTemplate.setContent("<div class=\"display-div\"><div>表号：" + biaohao + "</div><div>名称：" + name + "</div><div>压降值：" + value + " " + unit + "</div><div>时间：" + date + "</div><div>类型：" + leixing + "</div><div><a href=\"javascript:void(0)\" onclick=\"alarmSysteRes(" + id + ")\">事件模拟范围</a></div></div>");//"+id+","+leixing+"
        else if (num == 6 || num == 7)
            infoTemplate.setContent("<div class=\"display-div\"><div>表号：" + biaohao + "</div><div>名称：" + name + "</div><div>数值：" + value + " " + unit + "</div><div>时间：" + date + "</div><div>类型：" + leixing + "</div><div><a href=\"javascript:void(0)\" onclick=\"WarningSystemRes(" + id + ",1)\">防扩散关阀</a></div><div><a href=\"javascript:void(0)\" onclick=\"WarningSystemRes(" + id + ",2)\">溯源</a></div></div>");//"+id+","+leixing+"
        else
            infoTemplate.setContent("<div class=\"display-div\"><div>表号：" + biaohao + "</div><div>名称：" + name + "</div><div>数值：" + value + " " + unit + "</div><div>时间：" + date + "</div><div>类型：" + leixing + "</div></div>");//"+id+","+leixing+"
        map.infoWindow.resize(240, 190);
        switch (num){
            case 1:
            case 5:
                var graphic = new esri.Graphic(point, SmallPressSymbol);
                break;
            case 4:
                var graphic = new esri.Graphic(point, SmallFlowSymbol);
                break;
            case 2:
            case 3:
            case 6:
            case 7:
                var graphic = new esri.Graphic(point, SmallQualitySymbol);
                break;
            default:
                var graphic = new esri.Graphic(point, SmallPolygonSymbol);
                break;
        }
        var graphic1 = new esri.Graphic(point, textSymbol1);
        // var graphic2 = new esri.Graphic(point, textSymbol2);
        var graphic3 = new esri.Graphic(point, sms);
        graphicslayer.add(graphic3.setInfoTemplate(infoTemplate));
        graphicslayer.add(graphic1.setInfoTemplate(infoTemplate));
        graphicslayer.add(graphic);
        // graphicslayer.add(graphic2.setInfoTemplate(infoTemplate));
        if (isMove) {
            map.centerAndZoom(point, 7);
            $.messager.progress('close');
        }
    });
}
//余氯-------------------------------------------------------------------------------------------------------------------------
var dscFlag = 0;
function getClData(t) {
    $("#info").html(t.innerHTML);
    if (dscFlag == 0) {
        recoverAll();
        hideAndClear();
        tiled2.hide();
        layerModelBaseMapMin.show();
        //全图
        map.setExtent(tiled2.fullExtent, false);
        toAjax('/DataDisplay/DataQuery/GetClData', 2);
        dscFlag = 1;
    } else {
        recoverAll();
        hideAndClear();
    }
}
//浊度-------------------------------------------------------------------------------------------------------------------------
var dszdFlag = 0;
function getNTUData(t) {
    $("#info").html(t.innerHTML);
    if (dszdFlag == 0) {
        recoverAll();
        hideAndClear();
        tiled2.hide();
        layerModelBaseMapMin.show();
        //全图
        map.setExtent(tiled2.fullExtent, false);
        toAjax('/DataDisplay/DataQuery/GetTurbidityData', 3);
        dszdFlag = 1;
    } else {
        recoverAll();
        hideAndClear();
    }
}
//压力-------------------------------------------------------------------------------------------------------------------------
var dsylFlag = 0;
function getPressureData(t) {
    $("#info").html(t.innerHTML);
    //启动定时器
    if (dsylFlag == 0) {
        recoverAll();
        hideAndClear();
        tiled2.hide();
        layerModelBaseMapMin.show();
        //全图
        map.setExtent(tiled2.fullExtent, false);
        toAjax('/DataDisplay/DataQuery/GetPressureData', 1);
        dsylFlag = 1;
    } else {
        recoverAll();
        hideAndClear();
    }
}
//流量-------------------------------------------------------------------------------------------------------------------------
var dsllFlag = 0;
function getMeterData(t) {
    $("#info").html(t.innerHTML);
    //然后设置自己的轮询
    if (dsllFlag == 0) {
        recoverAll();
        hideAndClear();
        tiled2.hide();
        layerModelBaseMapMin.show();
        //全图
        map.setExtent(tiled2.fullExtent, false);
        toAjax('/DataDisplay/DataQuery/GetMeterData', 4);
        dsllFlag = 1;
    } else {
        recoverAll();
        hideAndClear();
    }
}
//源水流量-------------------------------------------------------------------------------------------------------------------------
var dsysFlag = 0;
function getSourceData(t) {
    $("#info").html(t.innerHTML);
    //然后设置自己的轮询
    if (dsysFlag == 0) {
        recoverAll();
        hideAndClear();
        tiled2.hide();
        layerModelBaseMapMin.show();
        //全图
        fullExtentClick();
        toAjax('/DataDisplay/DataQuery/GetSourceData', 41);
        dsysFlag = 1;
    } else {
        recoverAll();
        hideAndClear();
    }
}
//访问后台，获取监测数据
function toAjax(url, num) {
    //画图图层
    graphicslayer.clear();
    $.ajax({
        type: 'post',
        url: url,
        beforeSend: function (XMLHttpRequest) {
            //$.messager.progress({ text: '加载中...' });
        },
        success: function (data, textStatus) {
            //访问后台获取计量表数据和值。
            var jsonData = JSON.parse(data);
            //数据读取
            for (var i = 0; i < jsonData.length; i++) {
                if (num == 4 || num == 41) {
                    var tvalue = jsonData[i].Tag_value == null ? 0 : jsonData[i].Tag_value.toFixed(0);
                } else if (num == 2 || num == 3) {
                    var tvalue = jsonData[i].Tag_value == null ? 0 : jsonData[i].Tag_value.toFixed(2);
                } else {
                    var tvalue = jsonData[i].Tag_value == null ? 0 : jsonData[i].Tag_value.toFixed(PressPlace);
                }
                var sdate = jsonData[i].Save_date == null ? "" : jsonData[i].Save_date;
                drawPointsToMap(jsonData[i].ElementID, jsonData[i].WMeter_ID, jsonData[i].Meter_Name, jsonData[i].Explain, tvalue, sdate, num, false);
            }
        },
        complete: function (XMLHttpRequest, textStatus) {
            //$.messager.progress('close');
        },
        error: function () {
            //$.messager.progress('close');
        }
    });
}

$(function () {
    recoverAll = function () {
        //hideAndClear();//清除地图上图形
        //CLToolbartb.deactivate();//取消绘画
        isCLToolbartbL = false;//取消长度绘画状态
        isCLToolbartbA = false;//取消面积绘画状态
        //$("#mapSearchTop").css('height', "0px");//关闭其他按钮菜单
        $("#mapSearchTop").css('transform', "rotate(0deg)");//关闭其他按钮菜单动画
        $("#rtQuality").css("display", "none");
        $('#map').unbind('mousemove');//取消鼠标事件
        $('#map').unbind('mouseleave');//取消鼠标事件
        $('#followMouseInfo').hide();//隐藏鼠标提示信息
        let ss = $("#maptop > div:first > div");
        //$("#maptop > div:first > div").classList.remove('active');
        $("#mapSearchTop > div").css("background", "");//恢复实时数据下所有按钮颜色
        $("#mapSearchTop > div").css("color", "#000000");//恢复实时数据下所有按钮颜色
        isMapQuery = false;//是否图上查询标识
    }
});
//设置测量长度
function measutreLength() {
    hideAndClear();
    if (!isCLToolbartbL) {
        recoverAll();
        isCLToolbartbL = true;
        $("#measutreLength").css("background", "#4b77be");
        $("#measutreLength").css("color", "#ffffff");
        CLToolbartb.activate(esri.toolbars.Draw.POLYLINE);
    } else {
        recoverAll();
    }
}
//设置测量面积
function measutreArea() {
    hideAndClear();
    if (!isCLToolbartbA) {
        recoverAll();
        isCLToolbartbA = true;
        $("#measutreArea").css("background", "#4b77be");
        $("#measutreArea").css("color", "#ffffff");
        CLToolbartb.activate(esri.toolbars.Draw.POLYGON);
    } else {
        recoverAll();
    }
}

//全图
function fullExtentClick() {
    map.setExtent(FullExtent, false);
}

//定位初始化视图
function homeExtentClick() {
    map.setExtent(homeExtent, false);
}

//清除按钮 清除graphiclayer
function clearClick() {
    hideAndClear();
    recoverAll();
}

function tabsChange(title) {
    $('#tabs').tabs('select', title);
}
//清除 and 隐藏
function hideAndClear() {
    //layerMeasureArea.setVisibleLayers([0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14]);
   if (map.infoWindow.isShowing) map.infoWindow.hide();
    //画图图层
    graphicslayer.clear();
    dojo.disconnect(graphicslayer, "onClick");
    if (map.graphics.graphics.length > 0) map.graphics.clear();
    //管网图
    tiled2.show();
    layerModelBaseMapMin.hide();
    var layers = map.getLayersVisibleAtScale(map.getScale());
    for (var i = 0; i < layers.length; i++) {
        if (layers[i].id == "clusters") {
            map.removeLayer(layers[i]);//移除聚合效果图层
        }
    }
    //关闭实时显示的轮询
    intervalClear();
}
