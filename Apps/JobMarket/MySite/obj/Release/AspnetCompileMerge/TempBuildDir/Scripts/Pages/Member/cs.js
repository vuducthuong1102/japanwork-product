function GetMyCSs() {
    showItemLoading("#MyCSs");
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Member/MyCSs';
    params['requestType'] = 'POST';
    params['showloading'] = false;
    params['data'] = { page_index: $("#CurrentPage").val() };
    params['dataType'] = "json";
    params['successCallbackFunction'] = function (result) {
        if (result) {
            $("#MyCSs").html(result.html);
        }

        hideItemLoading("#MyCSs");
    };
    doAjax(params);
}

function CvSearchNext(pageNum) {
    $("#CurrentPage").val(pageNum);

    GetMyCSs();
}

$(function () {
    setTimeout(function () {
        GetMyCSs();
    }, 500);
});