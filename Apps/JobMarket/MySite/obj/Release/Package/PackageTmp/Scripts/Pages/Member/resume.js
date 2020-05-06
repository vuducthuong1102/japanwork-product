function GetWorkHistories() {
    showItemLoading("#WorkHistories");
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Member/WorkHistories';
    params['requestType'] = 'GET';
    params['showloading'] = false;
    params['data'] = { };
    params['dataType'] = "html";
    params['successCallbackFunction'] = function (result) {
        if (result) {
            if (result) {
                $("#WorkHistories").html(result);
            }
        }

        setTimeout(function () {
            hideItemLoading("#WorkHistories");
        }, 1000);
    };
    doAjax(params);
}

function GetEduHistories() {
    showItemLoading("#EduHistories");
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Member/EduHistories';
    params['requestType'] = 'GET';
    params['showloading'] = false;
    params['data'] = {};
    params['dataType'] = "html";
    params['successCallbackFunction'] = function (result) {
        if (result) {
            if (result) {
                $("#EduHistories").html(result);
            }
        }

        setTimeout(function () {
            hideItemLoading("#EduHistories");
        }, 1000);
    };
    doAjax(params);
}

function GetCertificates() {
    showItemLoading("#Certificates");
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Member/Certificates';
    params['requestType'] = 'GET';
    params['showloading'] = false;
    params['data'] = {};
    params['dataType'] = "html";
    params['successCallbackFunction'] = function (result) {
        if (result) {
            if (result) {
                $("#Certificates").html(result);
            }
        }

        setTimeout(function () {
            hideItemLoading("#Certificates");
        }, 1000);
    };
    doAjax(params);
}

$(function () {
    setTimeout(function () {
        GetEduHistories();

        GetWorkHistories();

        GetCertificates();
    }, 500);
});