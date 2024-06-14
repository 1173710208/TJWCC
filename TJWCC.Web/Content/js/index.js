var storage, fail, uid, warningCount = 0;
var data = $.request("data");
console.log(data);
switch (data) {
    case "1":
        $("#iframe123456789").attr("src", "/DataSynthetical/CurrentData/Index");
        $("#iframe123456789").attr("data-id", "/DataSynthetical/CurrentData/Index");
        $("#tab123456789").attr("data-id", "/DataSynthetical/CurrentData/Index");
        $("#tab123456789").html("实时监测数据");
        break;
    case "2":
        $("#iframe123456789").attr("src", "/Scheduling/Daily/Index");
        $("#iframe123456789").attr("data-id", "/Scheduling/Daily/Index");
        $("#tab123456789").attr("data-id", "/Scheduling/Daily/Index");
        $("#tab123456789").html("日常调度方案");
        break;
    case "3":
        $("#iframe123456789").attr("src", "/Elaborate/Optimize/Index");
        $("#iframe123456789").attr("data-id", "/Elaborate/Optimize/Index");
        $("#tab123456789").attr("data-id", "/Elaborate/Optimize/Index");
        $("#tab123456789").html("泵站优化调度");
        break;
    case "4":
        $("#iframe123456789").attr("src", "/SystemManage/Alarm/Index?data=1");
        $("#iframe123456789").attr("data-id", "/SystemManage/Alarm/Index");
        $("#tab123456789").attr("data-id", "/SystemManage/Alarm/Index");
        $("#tab123456789").html("报警管理");
        break;
    default:
        $("#iframe123456789").attr("src", "/DataSynthetical/CurrentData/Index");
        $("#iframe123456789").attr("data-id", "/DataSynthetical/CurrentData/Index");
        $("#tab123456789").attr("data-id", "/DataSynthetical/CurrentData/Index");
        $("#tab123456789").html("实时监测数据");
        break;
}
try {
    uid = new Date;
    (storage = window.localStorage).setItem(uid, uid);
    fail = storage.getItem(uid) != uid; storage.removeItem(uid);
    fail && (storage = false);
}
catch (e) { }
if (storage) {
    var usedSkin = localStorage.getItem('config-skin');
    if (usedSkin != '' && usedSkin != null) {
        document.body.className = usedSkin;
    }
    else {
        document.body.className = 'theme-blue-light';
        localStorage.setItem('config-skin', "theme-blue-light");
    }
}
else {
    document.body.className = 'theme-blue-light';
}
$(function () {
    if (storage) {
        try {
            var usedSkin = localStorage.getItem('config-skin');
            if (usedSkin != '') {
                $('#skin-colors .skin-changer').removeClass('active'); $('#skin-colors .skin-changer[data-skin="' + usedSkin + '"]').addClass('active');
            }
        }
        catch (e) { console.log(e); }
    }
})
$.fn.removeClassPrefix = function (prefix) {
    this.each(function (i, el) {
        var classes = el.className.split(" ").filter(function (c) {
            return c.lastIndexOf(prefix, 0) !== 0;
        });
        el.className = classes.join(" ");
    });
    return this;
};
$(function ($) {
    $('#config-tool-cog').on('click', function () { $('#config-tool').toggleClass('closed'); });
    $('#config-fixed-header').on('change', function () {
        var fixedHeader = '';
        if ($(this).is(':checked')) {
            $('body').addClass('fixed-header'); fixedHeader = 'fixed-header';
        }
        else {
            $('body').removeClass('fixed-header');
            if ($('#config-fixed-sidebar').is(':checked')) {
                $('#config-fixed-sidebar').prop('checked', false);
                $('#config-fixed-sidebar').trigger('change'); location.reload();
            }
        }
    });
    $('#skin-colors .skin-changer').on('click', function () {
        $('body').removeClassPrefix('theme-');
        $('body').addClass($(this).data('skin'));
        $('#skin-colors .skin-changer').removeClass('active');
        $(this).addClass('active');
        writeStorage(storage, 'config-skin', $(this).data('skin'));
    });
    function writeStorage(storage, key, value) {
        if (storage) {
            try {
                localStorage.setItem(key, value);
            }
            catch (e) { console.log(e); }
        }
    }
});
$(function ($) {
    $("#content-wrapper").find('.mainContent').height($(window).height() - 99);
    $(window).resize(function (e) {
        $("#content-wrapper").find('.mainContent').height($(window).height() - 99);
    });
    $('#sidebar-nav,#nav-col-submenu').on('click', '.dropdown-toggle', function (e) {
        e.preventDefault();
        var $item = $(this).parent();
        if (!$item.hasClass('open')) {
            $item.parent().find('.open .submenu').slideUp('fast');
            $item.parent().find('.open').toggleClass('open');
        }
        $item.toggleClass('open');
        if ($item.hasClass('open')) {
            $item.children('.submenu').slideDown('fast', function () {
                var _height1 = $(window).height() - 92 - $item.position().top;
                var _height2 = $item.find('ul.submenu').height() + 10;
                var _height3 = _height2 > _height1 ? _height1 : _height2;
                $item.find('ul.submenu').css({
                    overflow: "auto",
                    //height: _height3
                })
            });
        }
        else {
            $item.children('.submenu').slideUp('fast');
        }
    });
    GetLoadNav();
    $('body').on('mouseenter', '#page-wrapper.nav-small #sidebar-nav .dropdown-toggle', function (e) {
        if ($(document).width() >= 992) {
            var $item = $(this).parent();
            if ($('body').hasClass('fixed-leftmenu')) {
                var topPosition = $item.position().top;

                if ((topPosition + 4 * $(this).outerHeight()) >= $(window).height()) {
                    topPosition -= 6 * $(this).outerHeight();
                }
                $('#nav-col-submenu').html($item.children('.submenu').clone());
                $('#nav-col-submenu > .submenu').css({ 'top': topPosition });
            }

            $item.addClass('open');
            $item.children('.submenu').slideDown('fast');
        }
    });
    $('body').on('mouseleave', '#page-wrapper.nav-small #sidebar-nav > .nav-pills > li', function (e) {
        if ($(document).width() >= 992) {
            var $item = $(this);
            if ($item.hasClass('open')) {
                $item.find('.open .submenu').slideUp('fast');
                $item.find('.open').removeClass('open');
                $item.children('.submenu').slideUp('fast');
            }
            $item.removeClass('open');
        }
    });
    $('body').on('mouseenter', '#page-wrapper.nav-small #sidebar-nav a:not(.dropdown-toggle)', function (e) {
        if ($('body').hasClass('fixed-leftmenu')) {
            $('#nav-col-submenu').html('');
        }
    });
    $('body').on('mouseleave', '#page-wrapper.nav-small #nav-col', function (e) {
        if ($('body').hasClass('fixed-leftmenu')) {
            $('#nav-col-submenu').html('');
        }
    });
    $('body').find('#make-small-nav').click(function (e) {
        $('#page-wrapper').toggleClass('nav-small');
    });
    $('body').find('.mobile-search').click(function (e) {
        e.preventDefault();
        $('.mobile-search').addClass('active');
        $('.mobile-search form input.form-control').focus();
    });
    $(document).mouseup(function (e) {
        var container = $('.mobile-search');
        if (!container.is(e.target) && container.has(e.target).length === 0) // ... nor a descendant of the container
        {
            container.removeClass('active');
        }
    });
    $(window).load(function () {
        window.setTimeout(function () {
            $('#ajax-loader').fadeOut();
        }, 300);
    });
    setInterval(function () {
        $.ajax({
            type: "GET",
            url: "/SystemManage/Alarm/WarnCount",
            dataType: "JSON",
            success: function (adata) {
                console.log(adata)
                if (Number(adata) > 99)
                    $("#homeWarn").html("99+");
                else
                    $("#homeWarn").html(adata);
            }
        });
        $.ajax({
            type: "GET",
            url: "/SystemManage/Alarm/WarnNewInfo",
            dataType: "JSON",
            success: function (adata) {
                console.log(adata)
                if (adata.length > 1)
                    $.modalAlert(adata[0], adata[1], "warning");
            }
        });
    }, 300000); //定时刷新
});
var _html = "";
function GetLoadNav() {
    var data = top.clients.authorizeMenu;
    _html = "";
    $.each(data, function (i) {
        var row = data[i];
        if (row.TARGET == "expand") {
            _html += '<li>';
            _html += '<a data-id="' + row.ID + '" href="#" class="dropdown-toggle"><i class="' + row.ICON + '"></i><span>' + row.FULLNAME + '</span><i class="fa fa-angle-right drop-icon"></i></a>';
            var childNodes = row.ChildNodes;
            if (childNodes.length > 0) {
                _html += '<ul class="submenu" style="background-color: #173567">';
                GetLoadChildNav(childNodes)
                _html += '</ul>';
            }
            _html += '</li>';
        }
    });
    $("#sidebar-nav ul").prepend(_html);
}
function GetLoadChildNav(data) {
    $.each(data, function (i) {
        var row = data[i];
        if (row.TARGET == "expand") {
            _html += '<li>';
            _html += '<a data-id="' + row.ID + '" href="#" class="dropdown-toggle"><i class="' + row.ICON + '"></i><span>' + row.FULLNAME + '</span><i class="fa fa-angle-right drop-icon"></i></a>';
            var childNodes = row.ChildNodes;
            if (childNodes.length > 0) {
                _html += '<ul class="submenu" style="background-color: #173567">';
                GetLoadChildNav(childNodes)
                _html += '</ul>';
            }
            _html += '</li>';
        } else {
            _html += '<li>';
            _html += '<a class="menuItem" data-id="' + row.ID + '" href="' + row.URLADDRESS + '" data-index="' + row.SORTCODE + '">' + row.FULLNAME + '</a>';
            _html += '</li>';
        }
    });
}
function closecard() {// 关闭弹出层
    $("#WarningInfo").css('display','none');
}
function toWarningList() {
    var flag = true;
    let dataUrl = '/SystemManage/Alarm/Index';
    $('.menuTab').each(function () {
        if ($(this).data('id') == dataUrl) {
            if (!$(this).hasClass('active')) {
                $(this).addClass('active').siblings('.menuTab').removeClass('active');
                $.TJWCCtab.scrollToTab(this);
                $('.mainContent .TJWCC_iframe').each(function () {
                    if ($(this).data('id') == dataUrl) {
                        $(this).show().siblings('.TJWCC_iframe').hide();
                        return false;
                    }
                });
            }
            flag = false;
            return false;
        }
    });
    if (flag) {
        var str = '<a href="javascript:;" class="active menuTab" data-id="/SystemManage/Alarm/Index">报警管理<i class="fa fa-remove"></i></a>';
        $('.menuTab').removeClass('active');
        var str1 = '<iframe class="TJWCC_iframe" id="iframe00031e68-b027-400f-b666-dcb381421f4d" name="iframe00031e68-b027-400f-b666-dcb381421f4d"  width="100%" height="100%" src="/SystemManage/Alarm/Index?data=1" frameborder="0" data-id="/SystemManage/Alarm/Index" seamless></iframe>';
        $('.mainContent').find('iframe.TJWCC_iframe').hide();
        $('.mainContent').append(str1);
        $.loading(true);
        $('.mainContent iframe:visible').load(function () {
            $.loading(false);
        });
        $('.menuTabs .page-tabs-content').append(str);
        $.TJWCCtab.scrollToTab($('.menuTab.active'));
    }
}