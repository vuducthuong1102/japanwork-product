var ProfileGlobal = {
    init: function () {
        this.eduHistories();
        this.workHistories();
        this.certificates();
    },
    workHistories: function () {
        showItemLoading("#WorkHistories");
        var params = $.extend({}, doAjax_params_default);
        var url_tk = $("#WorkHistoryTk").val();
        params['url'] = url_tk;
        params['requestType'] = 'GET';
        params['showloading'] = false;
        params['data'] = {};
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
    },
    eduHistories: function () {
        showItemLoading("#EduHistories");
        var params = $.extend({}, doAjax_params_default);
        var url_tk = $("#EduHistoryTk").val();
        params['url'] = '' + url_tk;
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
    },
    certificates: function () {
        showItemLoading("#Certificates");
        var params = $.extend({}, doAjax_params_default);
        var url_tk = $("#CertificateTk").val();
        params['url'] = url_tk;
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
};

$(function () {
    ProfileGlobal.init();
});