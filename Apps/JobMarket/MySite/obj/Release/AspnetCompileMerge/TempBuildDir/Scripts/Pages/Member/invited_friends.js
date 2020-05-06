function GetInvitedFriends() {
    showItemLoading("#InvitedFriends");
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Member/GetInvitedFriends';
    params['requestType'] = 'POST';
    params['showloading'] = false;
    params['data'] = { page_index: $("#CurrentPage").val() };
    params['dataType'] = "json";
    params['successCallbackFunction'] = function (result) {
        if (result) {
            if (result.html) {
                $("#InvitedFriends").html(result.html);
            }
        }

        hideItemLoading("#InvitedFriends");
    };
    doAjax(params);
}

function InvitedFriendSearchNext(pageNum) {
    $("#CurrentPage").val(pageNum);

    GetInvitedFriends();
}

$("body").on("click", ".invite-msg", function () {
    var id = $(this).data("id");
    var htmlNote = $("#invite-note-" + id).html();

    $("#myModalNoteText").html(htmlNote);
    $("#myModalNote").modal("show");
});

$(function () {
    setTimeout(function () {
        GetInvitedFriends();
    }, 500);
});