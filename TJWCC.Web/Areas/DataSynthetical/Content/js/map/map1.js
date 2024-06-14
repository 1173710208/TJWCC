var map;
//信息显示窗口 dialog
var homeExtent;
var FullExtent
var dialog;
var CLToolbartb;//add by huangxin 20180312测量工具
var isFirstRun = true;
var isCLToolbartbL;//add by huangxin 20180427长度测量工具状态
var isCLToolbartbA;//add by huangxin 20180427面积测量工具状态
var geometryService;//add by huangxin 20180312几何地图服务
var measuregeometry;//add by huangxin 20180312测量时临时存放绘画图形
var graphicslayer;
var clusterLayer;
var legendDisplay;
var recoverAll;//add by huangxin 20171227清除按钮和显示状态全局函数
var outline;//定义面的边界线符号
var BigPolygonSymbol;//定义面符号
var BluePinPictureMSymbol;//蓝色气泡标记
var GreenPinPictureMSymbol;//绿色气泡标记
var BlueCPictureMSymbol;//聚合蓝色气泡标记
var GreenCPictureMSymbol;//聚合绿色气泡标记
var RedCPictureMSymbol;//聚合红色气泡标记
var BlackinfoPictureMSymbol;//气泡标记
var PinTextSymbol;//气泡标记
var RedBigPolygonSymbol;
var RedGlowPolygonSymbol;
var YellowBigPolygonSymbol;
var SmallPolygonSymbol;
var DBSAreaLaye;
var message="";
//图层
var layerMeasureArea;
//管网详图
var font1;//这个是字体
var color1;//这个是颜色
var color2;//这个是颜色
var color3;//这个是颜色
var tickInterval = 0;
var tickPixelInterval = 100;
var ChartXCount1 = 9//图表显示X轴数据个数
var ChartXCount2 = 15//图表显示X轴数据个数
var ChartXCount3 = 2//图表显示X轴数据个数
var ChartXCount4 = 7//图表显示X轴数据个数
var graphicslayerR = [];
//引入部分dojo包
dojo.require("esri.map");
//引入基础类包，具体看官方API
require([
    "esri/basemaps",
    "esri/map",
    "esri/Color",
    "esri/toolbars/draw",
    "esri/dijit/Scalebar",
    "esri/dijit/Legend",
    "esri/dijit/HomeButton",
    "esri/dijit/OverviewMap",
    "esri/lang",
    "esri/layers/GraphicsLayer",
    "esri/layers/ArcGISTiledMapServiceLayer",
    "esri/layers/ArcGISDynamicMapServiceLayer",
    "esri/geometry/Point",
    "esri/geometry/Extent",
    "esri/tasks/query",
    "esri/tasks/FindTask",
    "esri/tasks/GeometryService",
    "esri/tasks/QueryTask",
    "esri/tasks/IdentifyTask",
    "esri/tasks/FindParameters",
    "esri/symbols/Font",
    "esri/symbols/TextSymbol",
    "esri/symbols/SimpleLineSymbol",
    "esri/symbols/SimpleFillSymbol",
    "esri/symbols/SimpleMarkerSymbol",
    "esri/symbols/PictureMarkerSymbol",
    "esri/graphic",
    "esri/InfoTemplate",
    "dojo/on",
    "dojo/dom",
    "dojo/dom-style",
    "dojo/query",
    "dojo/colors",
    "dojo/_base/array",
    "dojo/number",
    "dojo/parser",
    "dijit/TooltipDialog",
    "dijit/popup",
    "esri/tasks/LengthsParameters",
    "esri/tasks/AreasAndLengthsParameters",
    "dojo/domReady!"
], function (
    esriBasemaps,
    Map,
    Color,
    Draw,
    Scalebar,
    Legend,
    HomeButton,
    OverviewMap,
    esriLang,
    GraphicsLayer,
    ArcGISTiledMapServiceLayer,
    ArcGISDynamicMapServiceLayer,
    Point,
    Extent,
    query,
    FindTask,
    GeometryService,
    QueryTask,
    IdentifyTask,
    FindParameters,
    Font,
    TextSymbol,
    SimpleLineSymbol,
    SimpleFillSymbol,
    SimpleMarkerSymbol,
    PictureMarkerSymbol,
    Graphic,
    InfoTemplate,
    on,
    dom,
    domStyle,
    Query,
    Color,
    arrayUtils,
    number,
    parser,
    TooltipDialog,
    dijitPopup,
    LengthsParameters,
    AreasAndLengthsParameters) {
    //font1
    font1 = new Font("13px", Font.STYLE_NORMAL, Font.VARIANT_NORMAL, Font.WEIGHT_BOLDER);
    color1 = new Color([0, 0, 0]);
    color2 = new Color([0, 0, 0]);
    color3 = new Color([0, 0, 0]);

    parser.parse();
    var initExtent = new esri.geometry.Extent({ "xmin": 13006295.421917431, "ymin": 4733993.264721222, "xmax": 13092134.204681568, "ymax": 4764568.076035252, "spatialReference": 102100 });
    esriBasemaps.delorme = {
        baseMapLayers: [
            //中国矢量地图服务
            {
                url: ArcGISServer +"/MapServer",
                id:"底图",
                opacity: 0.6
            }
        ],
        //缩略图
        //thumbnailUrl: "Imgs/shiliang.jpg",
        //title: "矢量图"
    };
    //初始化地图
    map = new Map("map", {
        basemap: "delorme",
        logo: false,
        extent: initExtent,
        fadeOnZoom: true,
        showLabels: true,
        isDoubleClickZoom: true//双击放大    
    });
    
    //禁用键盘缩放平移
    map.disableKeyboardNavigation();
    //卫星底图
    //var toggle = new BasemapToggle({
    //    map: map,
    //    basemap: "satellite"
    //}, "BasemapToggle");
    //toggle.startup();
    //返回主视图
    var home = new HomeButton({
        map: map
    }, "HomeButton");
    home.startup();
    //定位
    //geoLocate = new LocateButton({
    //    map: map
    //}, "LocateButton");
    //geoLocate.startup();
    //鹰眼
    var overviewMapDijit = new OverviewMap({
        map: map,
        expandFactor: 70,
        attachTo: "bottom-left",
        visible: false
    });
    overviewMapDijit.startup();
    //比例尺
    var scalebar = new Scalebar({ map: map, attachTo: "bottom-left", scalebarUnit: "metric" });
    //显示坐标点
    dojo.connect(map, "onLoad", function () {
        //dojo.connect(map, "onMouseMove", showCoordinates);
        //dojo.connect(map, "onMouseDrag", showCoordinates);
        initCLToolbar();//初始化测量工具
    });
   
    //1.定义面的边界线符号
    outline = new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([0, 0, 255]), 3);
    //2.定义面符号
    BigPolygonSymbol = new PictureMarkerSymbol('Content/myImage/valve.png', 16, 16);
    RedBigPolygonSymbol = new PictureMarkerSymbol('Content/myImage/valves.png', 30, 34).setOffset(0, 17);
    RedGlowPolygonSymbol = new PictureMarkerSymbol('../../Areas/DataDisplay/Content/img/gif/Red_glow.gif', 25, 25);
    YellowBigPolygonSymbol = new PictureMarkerSymbol('Content/myImage/traffic-cone.png', 34, 36).setOffset(0, 5);
    SmallPolygonSymbol = new PictureMarkerSymbol('Content/myImage/junction.png', 16, 16);
    BlackinfoPictureMSymbol = new PictureMarkerSymbol("Content/myImage/blackinfowin.png", 150, 48).setOffset(0, 24);
    BluePinPictureMSymbol = new PictureMarkerSymbol("Content/myImage/BluePin1LargeB.png", 64, 64).setOffset(0, 32);
    GreenPinPictureMSymbol = new PictureMarkerSymbol("Content/myImage/GreenPin1LargeB.png", 64, 64).setOffset(0, 32);
    BlueCPictureMSymbol = new PictureMarkerSymbol("Content/myImage/BluePin1LargeB.png", 32, 32).setOffset(0, 15);//聚合蓝色气泡标记
    GreenCPictureMSymbol = new PictureMarkerSymbol("Content/myImage/GreenPin1LargeB.png", 64, 64).setOffset(0, 32);//聚合绿色气泡标记
    RedCPictureMSymbol = new PictureMarkerSymbol("Content/myImage/RedPin1LargeB.png", 72, 72).setOffset(0, 32);//聚合红色气泡标记
    PinTextSymbol = new TextSymbol('', new Font("13px", Font.STYLE_NORMAL, Font.VARIANT_NORMAL, Font.WEIGHT_BOLDER), new dojo.Color([0, 0, 0])).setOffset(0, 25);//显示文字
    //管网图
    var infoTemplatePoint = new InfoTemplate("元素信息", "${*}");
    //map.addLayers([pipe, point, valve, hydrant, pool, pump]);
    //geometryService = GeometryService("http://" + ArcGISServer.split('/')[2] + "/arcgis/rest/services/Utilities/Geometry/GeometryServer");//几何地图服务
    geometryService = GeometryService("http://39.98.190.133:6080/arcgis/rest/services/Utilities/Geometry/GeometryServer");//几何地图服务
    layerMeasureArea = new ArcGISDynamicMapServiceLayer("http://39.98.190.133:6080/arcgis/rest/services/BSTJ/MeasureInfo/MapServer", { id: "分区", visible: true, opacity: 0.6 });//分区
    map.addLayer(layerMeasureArea);
    DBSAreaLaye = "http://39.98.190.133:6080/arcgis/rest/services/BSTJ/MeasureInfo/MapServer/";
 //图层按钮单击事件
    //graphicslayer
    graphicslayer = new GraphicsLayer();//画图图层
    map.addLayer(graphicslayer);
    //地图单击处理
    map.on("update-start", showLoading);//add by huangxin 20180508地图加载时显示loading图标
    map.on("update-end", hideLoading);//add by huangxin 20180508地图加载完隐藏loading图标
    function showLoading() {//地图加载时显示loading图标
        $('#loadingImg').css('left', $("#map").width() / 2 - 42 + 'px');
        $('#loadingImg').css('top', $("#map").height() / 2 - 40 + 'px');
        if(rtcdi == 0)
            esri.show(dojo.byId("loadingImg"));   // 显示图片  
    }

    function hideLoading() {//地图加载完成后隐藏loading图标
        esri.hide(dojo.byId("loadingImg"));
        if (isFirstRun) {
            isFirstRun = false;
            homeExtent = map.extent;
            FullExtent = new esri.geometry.Extent({ "xmin": 13006295.421917431, "ymin": 4733993.264721222, "xmax": 13092134.204681568, "ymax": 4764568.076035252, "spatialReference": map.spatialReference });//地图显示的新区域;
            $(".ovwButton.ovwController.ovwShow").click(function () {//鹰眼显示单击事件
                var tmpdisplay = $(".ovwContainer").css("display");
                if (tmpdisplay == "none")
                    $("#legendDivSoto").css("padding", "70px");
                else
                    $("#legendDivSoto").css("bottom", ($(".ovwContainer").height()+3) + "px");
            });
        }
    }
    function initCLToolbar() {//测量工具初始化
        CLToolbartb = new Draw(map);
        CLToolbartb.on("draw-complete", doMeasure);
        //tb.activate(Draw.POLYGON);  
    }
    
    function doMeasure(geometry) {//量算  
        geometry = geometry.geometry;
        dojo.disconnect(map.graphics, "onClick");
        //更加类型设置显示样式  
        measuregeometry = geometry;
        CLToolbartb.deactivate();
        switch (geometry.type) {
            case "polyline":
                var symbol = new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([152, 106, 180]), 2);
                break;
            case "polygon":
                var symbol = new SimpleFillSymbol(SimpleFillSymbol.STYLE_SOLID, new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([152, 106, 180]), 2), new Color([152, 106, 180, 0.25]));
                break;
        }
        //设置样式  
        var graphic = new Graphic(geometry, symbol);
        //清除上一次的画图内容  
        map.graphics.clear();
        map.graphics.add(graphic);
        //进行投影转换，完成后调用projectComplete  
        MeasureGeometry(geometry);
    }

    function MeasureGeometry(geometry) {//投影转换完成后调用方法  
        if (geometry.type == "polyline") {//如果为线类型就进行lengths距离测算  
            var lengthParams = new esri.tasks.LengthsParameters();
            lengthParams.polylines = [geometry];
            lengthParams.lengthUnit = esri.tasks.GeometryService.UNIT_METER;
            lengthParams.geodesic = true;
            //lengthParams.polylines[0].spatialReference = new esri.SpatialReference(4509);
            lengthParams.polylines[0].spatialReference = map.spatialReference;
            geometryService.lengths(lengthParams);
            dojo.connect(geometryService, "onLengthsComplete", outputDistance);
        }
        else if (geometry.type == "polygon") {//如果为面类型需要先进行simplify操作在进行面积测算  
            //$.messager.progress({ text: '正在计算中...' });
            var areasAndLengthParams = new esri.tasks.AreasAndLengthsParameters();
            areasAndLengthParams.lengthUnit = esri.tasks.GeometryService.UNIT_METER;
            areasAndLengthParams.areaUnit = esri.tasks.GeometryService.UNIT_SQUARE_METERS;
            //this.outSR = new esri.SpatialReference({ wkid: 4509 });
            this.outSR = map.spatialReference;
            geometryService.project([geometry], this.outSR, function (geometry) {
                geometryService.simplify(geometry, function (simplifiedGeometries) {
                    areasAndLengthParams.polygons = simplifiedGeometries;
                    //areasAndLengthParams.polygons[0].spatialReference = new esri.SpatialReference(4509);
                    areasAndLengthParams.polygons[0].spatialReference = map.spatialReference;
                    geometryService.areasAndLengths(areasAndLengthParams);
                });
            });
            dojo.connect(geometryService, "onAreasAndLengthsComplete", outputAreaAndLength);
        }
    }

    function outputDistance(result) {//显示测量距离  
        var CurX = measuregeometry.paths[0][measuregeometry.paths[0].length - 1][0];
        var CurY = measuregeometry.paths[0][measuregeometry.paths[0].length - 1][1];
        var CurPos = new esri.geometry.Point(CurX, CurY, map.spatialReference);
        map.infoWindow.setTitle("距离测量");
        map.infoWindow.setContent(" 测量长度：<strong>" + parseInt(String(result.lengths[0])) + "m</strong>");
        map.infoWindow.show(CurPos);
        map.infoWindow.resize(245, 58);
        $("#measutreLength").css("background", "");
        $("#measutreLength").css("color", "#000000");
        isCLToolbartbL = false;
    }

    function outputAreaAndLength(result) {//显示测量面积  
        var CurX = (measuregeometry.cache._extent.xmax + measuregeometry.cache._extent.xmin) / 2;
        var CurY = (measuregeometry.cache._extent.ymax + measuregeometry.cache._extent.ymin) / 2
        var CurPos = new esri.geometry.Point(CurX, CurY, map.spatialReference);
        //if (isCLToolbartbA)
        //    $.messager.progress('close');
        var aalContent = "<div class=\"display-div\"><div>面积：" + parseInt(String(result.areas[0])) + "㎡ </div><div>周长：" + parseInt(String(result.lengths[0])) + "m</div></div>";
        map.infoWindow.setTitle("面积测量");
        map.infoWindow.setContent(aalContent);
        map.infoWindow.show(CurPos);
        map.infoWindow.resize(195, 120);
        $("#measutreArea").css("background", "");
        $("#measutreArea").css("color", "#000000");
        isCLToolbartbA = false;
    }
        
    var flag = 0;
    map.infoWindow.resize(245, 125);
    dialog = new TooltipDialog({
        id: "tooltipDialog",
        style: "position: absolute; width: 250px; font: normal normal normal 10pt Helvetica;z-index:100"
    });
    dialog.startup();
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
//显示分区
function showArea(id) {
    layerMeasureArea.setVisibleLayers([id - 1]);
    layerMeasureArea.show();
    var queryTask = new esri.tasks.QueryTask(DBSAreaLaye + (id - 1));
    var query = new esri.tasks.Query();
    query.outSpatialReference = map.spatialReference;
    query.where = "ID = " + id;
    //query.where = "Physical_DistrictID = " + mess.id;
    query.returnGeometry = true;
    query.outFields = ["*"];
    queryTask.execute(query, function (featureSet) {
        if (featureSet.features.length == 0) {
            $.messager.alert('获取失败！', '无此分区范围信息！', 'warning');
            return;
        }
        var centerPoint = featureSet.features[0].geometry.getExtent().getCenter();
        cPoint = new esri.geometry.Point();
        cPoint.x = centerPoint.x;
        cPoint.y = centerPoint.y;
        cPoint.spatialReference = map.spatialReference;
        //map.centerAndZoom(cPoint, 2);
        //var AreaExtent = DBSFeature2.fullExtent;
        var AreaExtent = featureSet.features[0].geometry.getExtent();
        var w_h = map.width / map.height;
        var theight = (AreaExtent.xmax - AreaExtent.xmin) / w_h;
        var twidth = (AreaExtent.ymax - AreaExtent.ymin) * w_h;
        var wheight = (AreaExtent.ymax - AreaExtent.ymin);
        var wwidth = (AreaExtent.xmax - AreaExtent.xmin);
        if (theight > wheight) {//根据显示区域长宽比调整缩放位置
            AreaExtent.ymax = AreaExtent.ymax + (theight - wheight / 2);
            AreaExtent.ymin = AreaExtent.ymin - (theight - wheight / 2);
        } else {
            AreaExtent.xmax = AreaExtent.xmax + (twidth - wwidth / 2);
            AreaExtent.xmin = AreaExtent.xmin - (twidth - wwidth / 2);
        }
        map.setExtent(AreaExtent, false);
    });
}
