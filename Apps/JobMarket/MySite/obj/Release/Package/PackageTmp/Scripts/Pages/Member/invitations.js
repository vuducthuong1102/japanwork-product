function GetInvitations() {
    showItemLoading("#Invitations");
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Member/GetInvitations';
    params['requestType'] = 'POST';
    params['showloading'] = false;
    params['data'] = { page_index: $("#CurrentPage").val() };
    params['dataType'] = "json";
    params['successCallbackFunction'] = function (result) {
        if (result) {
            if (result.html) {
                $("#Invitations").html(result.html);
            }
        }

        hideItemLoading("#Invitations");
    };
    doAjax(params);
}

function reload() {
    window.location.reload();
}
function InvitationSearchNext(pageNum) {
    $("#CurrentPage").val(pageNum);

    GetInvitations();
}

$("body").on("click", ".invite-msg", function () {
    var id = $(this).data("id");
    var htmlNote = $("#invite-note-" + id).html();

    $("#myModalNoteText").html(htmlNote);
    $("#myModalNote").modal("show");
});

$(function () {
    setTimeout(function () {
        GetInvitations();
    }, 500);
});