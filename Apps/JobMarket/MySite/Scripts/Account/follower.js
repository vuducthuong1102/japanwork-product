var _window = $(window);
var _currentPage = 1;
var _hasNoData = false;

$(function () {
    $(".infomation span.follower").addClass("text-blue").removeClass("text-grey-03");

    GetCounter();

    GetFollowersList();

    // Each time the user scrolls
    _window.scroll(function () {
        // End of the document reached?
        if ($(document).height() - _window.height() == _window.scrollTop()) {
            _currentPage++;

            if (!_hasNoData)
                GetFollowersList();
        }
    });
});

function afterGetCounter(data) {
    if (data) {
        if (data.success) {
            $("#FollowersCount").html(data.data.FollowerCount);
            $("#FollowingsCount").html(data.data.FollowingCount);
            $("#PostsCount").html(data.data.PostCount);
            $("#PhotosCount").html(data.data.PhotoCount);
            $("#LikesCount").html(data.data.LikePostCount);
        }
    }
}

function GetCounter() {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Account/GetCounter';
    params['requestType'] = 'POST';
    params['data'] = { userId: $("#ProfileId").val() };
    params['dataType'] = "json";

    params['successCallbackFunction'] = afterGetCounter;
    doAjax(params);
}

function follow(e) {
    var userid = $(e).data("user");
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/UserAction/Follow';
    params['requestType'] = 'POST';
    params['data'] = { userid: userid };
    params['dataType'] = "json";
    params['successCallbackFunction'] = afterfollow;

    $(e).fadeOut();
    doAjax(params);
}

function afterfollow(data) {
    GetCounter();
}

function afterGetFollowersList(data) {
    var container = $("#FollowersList");
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
    $(".follower-overlay").remove();
}

function GetFollowersList() {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Account/GetListFollowers';
    params['requestType'] = 'POST';
    params['data'] = { page: _currentPage, ownerId: $("#ProfileId").val() };
    params['dataType'] = "json";

    params['successCallbackFunction'] = afterGetFollowersList;
    doAjax(params);
}

