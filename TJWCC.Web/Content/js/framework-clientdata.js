var clients = [];
$(function () {
    clients = $.clientsInit();
})
$.clientsInit = function () {
    var dataJson = {
        dataItems: [],
        organize: [],
        shiftItems: [],
        dispatchType: [],
        statusItems: [],
        role: [],
        duty: [],
        user: [],
        areaTree:[],
        authorizeMenu: [],
        authorizeButton: []
    };
    var init = function () {
        $.ajax({
            url: "/ClientsData/GetClientsDataJson",
            type: "get",
            dataType: "json",
            async: false,
            success: function (data) { 
                dataJson.dataItems = data.dataItems;
                dataJson.organize = data.organize;
                dataJson.shiftItems = data.shiftItems;
                dataJson.dispatchType = data.dispatchType;
                dataJson.statusItems = data.statusItems;
                dataJson.role = data.role;
                dataJson.duty = data.duty;
                dataJson.areaTree = data.areaTree;
                dataJson.authorizeMenu = eval(data.authorizeMenu);
                dataJson.authorizeButton = data.authorizeButton;
            }
        });
    }
    init();
    return dataJson;
}