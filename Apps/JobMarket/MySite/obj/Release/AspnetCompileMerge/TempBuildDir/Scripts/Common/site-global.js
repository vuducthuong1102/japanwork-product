$('.my-profiles-sec > span').on('click', function (ev) {   
    $('.profile-sidebar').toggleClass('active');
});

$("body").on("click", ".btn-save-job", function () {
    var job_id = $(this).data("id");
    var iconElem = $(this).find("i").first();

    iconElem.toggleClass("la-heart-o");
    iconElem.toggleClass("text-danger");

    if (iconElem.hasClass("la-heart-o")) {
        throttle(SaveJob(job_id, true), 1000);
    } else {
        throttle(SaveJob(job_id), 1000);
    }
});

function SaveJob(id, isUnSave = false) {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Job/SaveJob';
    if (isUnSave) {
        params['url'] = '/Job/UnSaveJob';
    }

    params['requestType'] = 'POST';
    params['showloading'] = false;
    params['data'] = { job_id: id };
    params['dataType'] = "json";
    params['successCallbackFunction'] = function (result) {

    };
    doAjax(params);
}

function PreviewImageFromBrowseDialog(input) {
    var previewContainer = $(input).data("preview");
    var currentPreviewContainer = $("#" + previewContainer);
    var previewImg = currentPreviewContainer.children(".thumbImg").first();
    var fileUploadIcon = currentPreviewContainer.children(".file-upload-icon").first();
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            //if (previewImg.length <= 0) {
            //    previewImg.attr(e.target.result);
            //    currentPreviewContainer.html('<img src="' + e.target.result + '" class="img-res thumbImg img-thum-preview" width="100" height="50" style="float:left;margin-right:10px;" />');
            //} else {
            //    previewImg.attr('src', e.target.result);
            //}
            previewImg.attr("src", e.target.result);
            previewImg.removeClass("hidden");
            fileUploadIcon.addClass("hidden");
        };

        reader.readAsDataURL(input.files[0]);
    } else {
        if (previewImg) {
            previewImg.addClass("hidden");
            fileUploadIcon.removeClass("hidden");
        }            
    }
}

$(".file-upload-storage").change(function () {
    PreviewImageFromBrowseDialog(this);
});

$("body").on("click", ".file-upload-btn", function () {
    $(this).parent().find(".file-upload-storage").click();
});

//$("body").on("click", ".member-left-menu-item", function () {
//    var currentTabName = $(this).data("name");
//    var currentActionLink = $(this).data("href");

//    console.log($('#MemberSubContainer').data("current"));
//    console.log(currentTabName);
//    if ($('#MemberSubContainer').data("current") !== currentTabName){
//        var params = $.extend({}, doAjax_params_default);
//        params['url'] = currentActionLink;
//        params['requestType'] = 'POST';
//        params['showloading'] = true;
//        params['data'] = {};
//        params['dataType'] = "html";
//        params['successCallbackFunction'] = function (result) {
//            if (result) {
//                if (result.html) {
//                    $('#MemberSubContainer').html(result.html);
//                }
//            }
//        };
//        doAjax(params);

//        $(".member-left-menu-item").removeClass("active");
//        $(this).toggleClass("active");      
//    }   

//    $('#MemberSubContainer').attr("data-current", currentTabName);

//    return false;
//});

var AuthenticatedGlobal = {
    getNotifications: function (pageNum = 0) {
        //var notifPage = parseInt($("#NotifPageNum").val());
        if (pageNum <= 0) {
            pageNum = 1;
        }
        showItemLoading(".box-noti");

        var params = $.extend({}, doAjax_params_default);
        params['url'] = '/AuthedGlobal/GetNotification';
        params['requestType'] = 'POST';
        params['data'] = { page: pageNum };
        params['dataType'] = "json";
        params['showloading'] = false;

        params['successCallbackFunction'] = function (data) {
            var container = $(".box-noti");
            if (data !== "" && data) {
                if (data.success) {
                    if (data.html) {
                        container.html(data.html);
                        container.addClass("loaded");
                       // $(".notif-dropdown").find("dropdown").addClass("open");
                    }
                }
                else {
                    _hasNoData = true;
                }
            }

            hideItemLoading(".box-noti");
        };
        doAjax(params);
    },
    getNotifCounter: function () {
        var params = $.extend({}, doAjax_params_default);
        params['url'] = '/AuthedGlobal/GetNotifCount';
        params['requestType'] = 'POST';
        params['data'] = {};
        params['dataType'] = "json";
        params['showloading'] = false;

        params['successCallbackFunction'] = function (data) {
            if (data) {
                if (data.success) {
                    var notifCount = data.data;
                    if (notifCount > 0) {
                        if (notifCount <= 99) {
                            $(".count-noti-str").html(notifCount);
                        } else {
                            $(".count-noti-str").html("99+");
                        }
                        $(".count-noti-str").removeClass("hidden");
                    }
                }
            }
        };
        doAjax(params);
    },
    getNewNotification: function (id) {
        var params = $.extend({}, doAjax_params_default);
        params['url'] = '/Member/GetNewNotification';
        params['requestType'] = 'POST';
        params['data'] = { id: id };
        params['showloading'] = false;
        params['dataType'] = "html";

        params['successCallbackFunction'] = function (result) {
            $("body").append(result);
        };

        doAjax(params);
    },
    counterUpNotification: function () {
        var counter = $(".count-noti-str");
        var currentVal = parseInt(counter.html().replace("+", ""));

        $(".box-noti").removeClass("loaded");
        if (counter.hasClass("hidden")) {
            currentVal = 1;
            counter.html(currentVal);
            counter.removeClass("hidden");
            return false;
        } else {
            if (currentVal <= 99) {
                currentVal++;
            }

            if (currentVal <= 99)
                counter.html(currentVal);
            else
                counter.html("99+");
        }
    }
};

//$('#notifDropdown').on('show.bs.dropdown', function () {
//    alert(123);
//    var container = $(".box-noti");
//    if (!container.hasClass("loaded")) {
//        AuthenticatedGlobal.getNotifications();
//    } else {
//        alert(234);
//    }

//    $(".dropdown-menu").toggleClass("show");
//    $(".count-noti-str").addClass("hidden");
//});

$(function () {
    $(".notif-icon").parent().on('show.bs.dropdown', function () {
        var container = $(".box-noti");
        if (!container.hasClass("loaded")) {
            AuthenticatedGlobal.getNotifications();
        }

        $(".count-noti-str").addClass("hidden");
    });
});
//$("body").on("click", ".notif-icon", function (e) {
//    e.stopPropagation();
//    e.preventDefault();
//    var container = $(".box-noti");
//    if (!container.hasClass("loaded")) {       
//        AuthenticatedGlobal.getNotifications();
//    } else {
//        alert(234);
//    }

//    $(".dropdown-menu").toggleClass("show");
//    $(".count-noti-str").addClass("hidden");

//    $('.wishlist-dropdown').fadeIn();
//});