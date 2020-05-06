var _window = $(window);
var _currentPage = 1;
var _hasNoData = false;

$(function () {
    $(document).on("click", "a.list-request-friend", function () {
        GetInviteByPage();
    })
    //GetNotifcations(_currentPage);
    // Each time the user scrolls
    _window.scroll(function () {
        // End of the document reached?
        if ($(document).height() - _window.height() == _window.scrollTop()) {
            _currentPage++;

            if (!_hasNoData)
                GetInviteByPage(_currentPage);
        }
    });
});

//function afterGetInviteByPage(data) {
//    var container = $("#ListFriendRequests");
//    if (data !== "" && data) {
//        if (data.success) {
//            if (data.html) {
//                container.append(data.html);
//            }
//        }
//        else {
//            _hasNoData = true;
//        }
//    }

//    container.removeClass("hidden");
//    $(".dividerr").not(":first").remove();
//    $(".friend-overlay").remove();
//}

//function GetInviteByPage() {
//    var countNotif = $(".count-friend-noti").data("count");
//    alert(countNotif);
//    var params = $.extend({}, doAjax_params_default);
//    params['url'] = '/Account/GetInviteByPage';
//    params['requestType'] = 'GET';
//    params['data'] = { page: _currentPage, countnotif: countNotif };
//    params['dataType'] = "json";

//    params['successCallbackFunction'] = afterGetInviteByPage;
//    doAjax(params);
//}


