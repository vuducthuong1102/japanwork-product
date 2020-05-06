function GetAppliedJobs() {
    showItemLoading("#AppliedJobs");
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Member/AppliedJobs';
    params['requestType'] = 'POST';
    params['showloading'] = false;
    params['data'] = { page_index: $("#CurrentPage").val() };
    params['dataType'] = "json";
    params['successCallbackFunction'] = function (result) {
        if (result) {
            if (result.html) {
                $("#AppliedJobs").html(result.html);
            }
        }

        hideItemLoading("#AppliedJobs");
    };
    doAjax(params);
}

function ApplicationSearchNext(pageNum) {
    $("#CurrentPage").val(pageNum);

    GetAppliedJobs();
}

$(function () {
    setTimeout(function () {
        GetAppliedJobs();
    }, 500);
});