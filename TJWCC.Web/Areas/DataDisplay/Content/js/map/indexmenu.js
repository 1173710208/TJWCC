$(function () {
    $("#homeExtent").on('contextmenu', function (e) {
		e.preventDefault();
		homeExtent = map.extent;
		//$.messager.alert('提示', '已保存当前显示区域！', 'info');
    });
	//$('#menubox').menu({
	//	onShow : function () {
	//		alert('显示时触发！');
	//	},
	//	onHide : function () {
	//		alert('隐藏时触发！');
	//	},
	//	onClick : function (item) {
	//		alert(item.text);
	//	}
	//});
});