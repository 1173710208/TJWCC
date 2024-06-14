(function ($) {
    $.TJWCCMain = {
        CurDate: function () {
            var d = new Date();
            //获取年份
            var year = d.getFullYear();
            //获取月份，返回值是0-11，因此需要加1 
            var mouth = d.getMonth() + 1;
            //获取日期
            var day = d.getDate();
            //获取小时，三元表达式，判断是否小于10，小于10就在前面加0（字符串拼接），例如：08
            var hour = d.getHours() < 10 ? '0' + d.getHours() : d.getHours();
            //获取分钟
            var minutes = d.getMinutes() < 10 ? '0' + d.getMinutes() : d.getMinutes();
            //获取秒数
            var second = d.getSeconds() < 10 ? '0' + d.getSeconds() : d.getSeconds();

            var curTime = year + '年' + mouth + '月' + day + '日';
            $("#time").text(curTime);
            $("#time-Hour1").text(hour.toString().substring(0, 1));
            $("#time-Hour2").text(hour.toString().substring(1, 2));
            $("#time-Minute1").text(minutes.toString().substring(0, 1));
            $("#time-Minute2").text(minutes.toString().substring(1, 2));
            $("#time-Second1").text(second.toString().substring(0, 1));
            $("#time-Second2").text(second.toString().substring(1, 2));
        },
        SelectedTab: function () {
            $(".tabNav .tabNavMenu").click(function () {
                $(".tabNav .tabNavMenu").removeClass("selected");
                $(this).addClass("selected");
                //$(".AreaMap").hide();
                //$(".pro").hide();
                //if ($(".selected").attr("id") == "menutj") {
                //    $("#tjmap").show();
                //    $("#tjpro").show();
                //} else if ($(".selected").attr("id") == "menuwly") {
                //    $("#wlymap").show();
                //    $("#wlypro").show();
                //} else if ($(".selected").attr("id") == "menubh") {
                //    $("#bhmap").show();
                //    $("#bhpro").show();
                //}
                GetSecondaryWater($(this).text());
            });
        },
        //绘制地图高亮显示部分
        DrawMap: function (dom) {
            $(dom).mouseover(function () {
                var canvers = document.getElementById($(dom).attr("data")); 
                var context = canvers.getContext("2d");
                context.globalAlpha = 0.2;
                context.beginPath();
                var strs = new Array(); //定义一数组
                strs = $(dom).attr("coords").split(",");
                var i1, i2;
                for (var i = 0; i < strs.length; i++) {
                    if (i % 2 == 0) {
                        i1 = strs[i];
                    }
                    if (i % 2 == 1) {
                        i2 = strs[i];
                        if (i == 1) {
                            context.moveTo(i1, i2);
                        }
                        else {
                            context.lineTo(i1, i2);
                        }
                    }
                }
                context.fillStyle = "#c0c0c0";
                context.fill();

                context.closePath(); //闭合
            });
            $(dom).mouseout(function () {
                var canvers = document.getElementById($(dom).attr("data"));
                var context = canvers.getContext("2d");
                context.globalAlpha = 0.2;
                context.clearRect(0, 0, 700, 450);
            });
        },
        //绑定4张地图信息
        BindMap: function () {

            $("#AreaMap area").each(function () {
                $.TJWCCMain.DrawMap(this);
            });

            $("#tjAreaMap area").each(function () {
                $.TJWCCMain.DrawMap(this);
            });

            $("#wlyAreaMap area").each(function () {
                $.TJWCCMain.DrawMap(this);
            });

            $("#bhAreaMap area").each(function () {
                $.TJWCCMain.DrawMap(this);
            });
        },
        Init: function () {
            //定时更新时间
            window.setInterval($.TJWCCMain.CurDate, 1000);
            $.TJWCCMain.SelectedTab();
            $.TJWCCMain.BindMap();
        }
    };
    $(function () {
        $.TJWCCMain.Init();
    })
})(jQuery);