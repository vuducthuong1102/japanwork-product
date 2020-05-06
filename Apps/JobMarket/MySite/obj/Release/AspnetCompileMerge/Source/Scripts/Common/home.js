var HomeGlobal = {
    init: function () {
        GetFields();
        GetListHots();
    }
};


function GetFields() {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Master/GetFields';
    params['requestType'] = 'POST';
    params['showloading'] = false;
    params['data'] = {};
    params['dataType'] = "html";
    params['successCallbackFunction'] = function (result) {
        $("#category_list").html("");
        $("#category_list").html(result);
    };
    doAjax(params);
}




function GetListHots() {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Master/GetListHots';
    params['requestType'] = 'POST';
    params['showloading'] = false;
    params['data'] = {};
    params['dataType'] = "html";
    params['successCallbackFunction'] = function (result) {
        $("#list-hot-jobs").html("");
        $("#list-hot-jobs").html(result);

        $("img.lazy").myLazyLoad();
    };
    doAjax(params);
}

