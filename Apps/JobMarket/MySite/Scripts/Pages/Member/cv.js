function GetMyCVs() {
    showItemLoading("#MyCVs");
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Member/MyCVs';
    params['requestType'] = 'POST';
    params['showloading'] = false;
    params['data'] = { page_index: $("#CurrentPage").val() };
    params['dataType'] = "json";
    params['successCallbackFunction'] = function (result) {
        if (result) {
            $("#MyCVs").html(result.html);
        }

        hideItemLoading("#MyCVs");
    };
    doAjax(params);
}

function CvSearchNext(pageNum) {
    $("#CurrentPage").val(pageNum);

    GetMyCVs();
}

$(function () {
    setTimeout(function () {
        GetMyCVs();
    }, 500);
});