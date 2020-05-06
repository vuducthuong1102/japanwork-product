$.getScript("/Scripts/Post/commentreply.js").done(function (script, textStatus) {
    $.getScript("/Scripts/Post/comment.js").done(function (script, textStatus) {
    });
});
$.getScript("/Scripts/Common/weather.js")


$(function () {
    GetListPosted();
    GetCounter();
});
var _window = $(window);
function SetPageCounter(container, counter, hasData) {
    var currentVal = parseInt(counter.val());
    if (hasData) {
        if (currentVal != 0) {
            currentVal++;
        }
        counter.val(currentVal);
    } else {
        container.addClass("done");
    }
}

function MarkPanelIsLoaded(panel) {
    panel.addClass("loaded");
}

function RemoveOverLay(container) {
    //$('.lazy').lazy();
    setTimeout(function () {
        container.parent().find(".post-overlay").remove();
        container.removeClass("hidden").fadeIn();
    }, 500);

    $("a.post-viewmore").click(function (e) {
        e.stopPropagation();
    });
}

function afterGetListPosted(data) {
    var container = $("#PF_Posted");
    var counter = $("#PostedCounter");
    if (data) {
        if (data.success) {
            if (data.html !== "") {
                container.append(data.html);
                //InitAll();
                //InitDeletePost();
                SetPageCounter(container, counter, true);
            }
        } else {
            SetPageCounter(container, counter, false);
        }
    }
    RemoveOverLay(container);
    MarkPanelIsLoaded($("#trip-post-profile"));
}

function GetListPosted() {
    var params = $.extend({}, doAjax_params_default);
    var counterControl = $("#PostedCounter");
    params['url'] = '/Account/GetListPosted';
    params['requestType'] = 'POST';
    params['data'] = { page: counterControl.val(), OwnerId: $("#ProfileId").val() };
    params['dataType'] = "json";

    params['successCallbackFunction'] = afterGetListPosted;
    doAjax(params);
}

function afterGetUploadedImages(data) {
    var container = $("#PF_UploadedImages");
    var counter = $("#UploadedImagesCounter");
    if (data) {
        if (data.success) {
            if (data.html !== "") {
                container.append(data.html);
                SetPageCounter(container, counter, true);
            }
        } else {
            SetPageCounter(container, counter, false);
        }
        container.parent().find(".widget-overlay").remove();
    }

    RemoveOverLay(container);

    MarkPanelIsLoaded($("#photo-profile"));
}

function GetUploadedImages() {
    var params = $.extend({}, doAjax_params_default);
    var counterControl = $("#UploadedImagesCounter");
    params['url'] = '/Account/GetUploadedImages';
    params['requestType'] = 'POST';
    params['data'] = { page: counterControl.val(), OwnerId: $("#ProfileId").val() };
    params['dataType'] = "json";

    params['successCallbackFunction'] = afterGetUploadedImages;
    doAjax(params);
}

function afterGetListSaved(data) {
    var container = $("#PF_Saved");
    var counter = $("#SavedCounter");
    if (data) {
        if (data.success) {
            if (data.html !== "") {
                container.append(data.html);
                //InitAll();
                SetPageCounter(container, counter, true);
            }
        } else {
            SetPageCounter(container, counter, false);
        }
        container.parent().find(".widget-overlay").remove();
    }
    RemoveOverLay(container);
    MarkPanelIsLoaded($("#save-profile"));
}

function GetListSaved() {
    var params = $.extend({}, doAjax_params_default);
    var counterControl = $("#SavedCounter");
    params['url'] = '/Account/GetListSaved';
    params['requestType'] = 'POST';
    params['data'] = { page: counterControl.val(), OwnerId: $("#ProfileId").val() };
    params['dataType'] = "json";

    params['successCallbackFunction'] = afterGetListSaved;
    doAjax(params);
}


function afterChangePwd(data) {
    if (data) {
        bootbox.showmessage({
            message: data.message
        }, data.success);
        //bootbox.alert(data.message);
        //Hide modal
        $('#change-password').modal('hide');
        $("#FrmChangePwd")[0].reset();
    }
}

function ChangePassword() {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Account/ChangePassword';
    params['requestType'] = 'POST';
    params['data'] = $("#FrmChangePwd").serialize();
    params['dataType'] = "json";

    params['successCallbackFunction'] = afterChangePwd;
    doAjax(params);
}

$(function () {
    // Each time the user scrolls
    _window.scroll(function () {
        // End of the document reached?
        if ($(document).height() - _window.height() == _window.scrollTop()) {
            if ($("#trip-post-profile").hasClass("active"))
                if (!$("#PF_Posted").hasClass("done") && $("#PF_Posted >div.item").length>0) {
                    GetListPosted();
                }
                   
            if ($("#photo-profile").hasClass("active"))
                if (!$("#PF_UploadedImages").hasClass("done"))
                    GetUploadedImages();       
            if ($("#save-profile").hasClass("active"))
                if (!$("#PF_Saved").hasClass("done"))
                    GetListSaved();       
        }
    });

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target);
        var targetId = $(e.target).attr("href") // activated tab
        if ($(targetId).hasClass("loaded")) {
            return false;
        }
        else
        {
            if (targetId === "#trip-post-profile") {
                GetListPosted();
            }
            else if (targetId === "#photo-profile") {
                GetUploadedImages();
            }
            else if (targetId === "#save-profile") {
                GetListSaved();
            }
        }
    });

    $("#FrmChangePwd").submit(function () {
        if ($(this).valid()) {
            ChangePassword();
        }       
        return false;
    });

});

function afterGetCounter(data) {
    if (data) {
        if (data.success) {
            $("#FollowersCount").html(data.data.FollowerCount);
            $("#FollowingsCount").html(data.data.FollowingCount);
            $("#PostsCount").html(data.data.PostCount);
            $("#PhotosCount").html(data.data.PhotoCount);
            $("#SavedCount").html(data.data.SavedPostCount);
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
