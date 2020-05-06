var _window = $(window);
var _currentPage = 1;
var _hasNoData = false;

$(function () {     

    GetNotifcations(_currentPage);

    // Each time the user scrolls
    _window.scroll(function () {
        // End of the document reached?
        if ($(document).height() - _window.height() == _window.scrollTop()) {
            _currentPage++;

            if (!_hasNoData)
                GetNotifcations(_currentPage);
        }
    });
});

function afterGetNotifcations(data) {
    var container = $("#ListNotifs");
    if (data !== "" && data) {
        if (data.success) {
            if (data.html) {
                container.append(data.html);
            }
        }
        else {
            _hasNoData = true;
        }
    }

    container.removeClass("hidden");
    $(".dividerr").not(":first").remove();
    $(".notif-overlay").remove();
}

function GetNotifcations() {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Account/GetNotificationsByPage';
    params['requestType'] = 'GET';
    params['data'] = { page: _currentPage };
    params['dataType'] = "json";

    params['successCallbackFunction'] = afterGetNotifcations;
    doAjax(params);
}


