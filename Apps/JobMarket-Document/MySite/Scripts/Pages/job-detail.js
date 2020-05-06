$("body").on("click", ".btn-save-job-detail", function () {
    var job_id = $(this).data("id");
    throttle(SaveJobDetail(job_id), 1000);
});

function SaveJobDetail(id) {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Job/SaveJob';
    params['requestType'] = 'POST';
    params['showloading'] = false;
    params['data'] = { job_id: id };
    params['dataType'] = "json";
    params['successCallbackFunction'] = function (result) {
        if (result.success && result.message) {
            $.showSuccessMessage(LanguageDic['LB_NOTIFICATION'], result.message, function () { location.reload(); });
        }
    };
    doAjax(params);
}

function GetRecent() {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Job/GetRecent';
    params['requestType'] = 'POST';
    params['showloading'] = false;
    params['data'] = { id: $("#CurrentItemId").val(), company_id: $("#CompanyId").val() };
    params['dataType'] = "json";
    params['successCallbackFunction'] = function (result) {
        if (result) {
            if (result.html) {
                $("#RecentJobs").html(result.html);

                $("#RecentJobs").parent().removeClass("hidden");
            }
        }
    };
    doAjax(params);
}

$(function () {
    setTimeout(function () {
        GetRecent();
    }, 500);
});