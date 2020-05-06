$.getScript("/Scripts/Post/commentreply.js").done(function (script, textStatus) {
    $.getScript("/Scripts/Post/commentreplynoti.js");
    $.getScript("/Scripts/Post/comment.js").done(function (script, textStatus) {
        $.getScript("/Scripts/Post/commentnoti.js");
        $.getScript("/Scripts/Post/post.js").done(function (script, textStatus) {
            //InitAll();
        });
    });
});


//------------ Auto resize text area----------------
var measurer = $('<span>', {
    style: "display:inline-block;word-break:break-word;visibility:hidden;white-space:pre-wrap;"
}).appendTo('body');


$(function () {    
    // open add feed form
    $('body').on('click', '.feed-top-actions', function () {
        ShowFeedActions();
    });
    // close add feed form
    $('body').on('click', '.close-post a', function () {
        ConfirmFirst(QuitWithoutSaving, LanguageDic['LB_CONFIRM_QUIT']);
    });

    // Select add-location in list option
    $('body').on('click', '.add-location', function () {
        $('.add-feed .check-in-list').show();
        $('.add-feed .check-in-list .search-location input').focus();
    });

    // Select days in list
    $('body').on('click', '.add-feed .option .days .dropdown-menu a', function () {
        var value = $(this).text();
        $(this).closest('.days').find('.days-values').text(value).val(value);
    });

    $('body').on('click', '.modify-post', function () {
        var id = $(this).data("id");
        ModifyPost(id);
    });

    $("body").on('click', "#btnCreatePost", function () {
        if ($("#FrmAddPost").valid()) {
            AddPostValidation();
        }
    });

    $('body').on('click', '.tag-close', function (ev) {
        ev.preventDefault();
        ev.stopPropagation();
        $(this).parent().parent().remove();
    });

    $("body").on('click', '.icon-delete_photo', function () {
        var currentDelList = $("#RemovingImages").val();
        if (!$(this).parent().hasClass("item-uploaded")) {
            var img = $(this).data('idimg');

            //imgs = cleanArray(imgs, img);
            // xóa ảnh ra khỏi mảng
            for (var j = 0; j < imgs.length; j++) {
                if (imgs[j].key == img) {
                    imgs = jQuery.grep(imgs, function (item) { return item != imgs[j]; });
                }
            }
        } else {
            var imageId = $(this).data("id");
            var delImgs = [];
            if (currentDelList != "") {
                delImgs = currentDelList.split(",");
            }
            if (imageId != 0) {
                delImgs.push(imageId);
            }

            var delList = delImgs.join(",");
            $("#RemovingImages").val(delList);
        }

        //return false;
        $(this).parent(".item").remove();
        $(".list-img .content").css('width', $(".list-img .content").width() - 115);

        checkHasImage();
    });

    $("body").on('click', '.imgs-upload', function () {
        $('#imgs-upload').click();
    });
});

function HideFeedActions() {
    $('.add-feed-bg').fadeOut();
    $('.add-feed').removeClass('focus');
    if ($(".feed-actions").hasClass("nodisplay")) {
        $(".feed-actions").addClass("hidden");
    }
}

function ShowFeedActions() {
    $('.add-feed-bg').fadeIn();
    $('.add-feed').addClass('focus');
}

function FeedActionsReset() {
    $("#FrmAddPost")[0].reset();
    $("#UploadedImages").html("");
    measurer.html("");
    $("#Description").val("");
    $("#divdescription").html("");
    $("#SelectedLocation").html("");
    $(".days > .days-values").html(LanguageDic["LB_DAYS"]);
    $("#SearchLocation").val("");
    $(".check-in-list").hide();
    $(".list-img").hide();
    $("#Locations").val("");
    $(".tag-item").remove();
    $("#Title").val("");
    $("#PostId").val("");
    $("#Title").parent().find('.count span').text(0);
    $(".feed-actions").removeClass("update");
    $("#RemovingImages").val("");
    EmptyUploadFile();
}

function QuitWithoutSaving() {
    HideFeedActions();
    FeedActionsReset();
}

// Count text when typing title
function countChar(el, maxlength) {
    var len = el.value.length;
    if (len > maxlength) {
        el.value = el.value.substring(0, maxlength);
    } else {
        $(el).parent().find('.count span').text(len);
    }
};

$('textarea.autofit').on({
    input: function () {
        var text = $(this).val();
        if ($(this).attr("preventEnter") == undefined) {
            text = text.replace(/[\n]/g, "<br>&#8203;");
        }
        measurer.html(text);
        updateTextAreaSize($(this));
    },
    focus: function () {
        initMeasurerFor($(this));
    },
    keypress: function (e) {
        if (e.which == 13 && $(this).attr("preventEnter") != undefined) {
            e.preventDefault();
        }
    }
});


function initMeasurerFor(textarea) {
    if (!textarea[0].originalOverflowY) {
        textarea[0].originalOverflowY = textarea.css("overflow-y");
    }
    var maxWidth = textarea.css("max-width");
    measurer.text(textarea.text())
        .css("max-width", maxWidth == "none" ? textarea.width() + "px" : maxWidth)
        .css('font', textarea.css('font'))
        .css('overflow-y', textarea.css('overflow-y'))
        .css("max-height", textarea.css("max-height"))
        .css("min-height", textarea.css("min-height"))
        .css("min-width", textarea.css("min-width"))
        .css("padding", textarea.css("padding"))
        .css("border", textarea.css("border"))
        .css("box-sizing", textarea.css("box-sizing"))
}
function updateTextAreaSize(textarea) {
    textarea.height(measurer.height());
    var w = measurer.width();
    if (textarea[0].originalOverflowY == "auto") {
        var mw = textarea.css("max-width");
        if (mw != "none") {
            if (w == parseInt(mw)) {
                textarea.css("overflow-y", "auto");
            } else {
                textarea.css("overflow-y", "hidden");
            }
        }
    }
    textarea.width(w + 2);
}

function cleanArray(actual, index) {
    var newArray = new Array();
    console.log(index);
    for (var i = 0; i < actual.length; i++) {
        if (i != index) {
            newArray.push(actual[i]);
        }
    }
    console.log(newArray);
    return newArray;
}

var imgs = [];

if (window.File && window.FileList && window.FileReader) {
    EventOnChangeUploadFile();
} else {
    console.log("Your browser doesn't support to File API")
}   

function EventOnChangeUploadFile() {
    $("#imgs-upload").on("change", function (e) {
        var files = e.target.files,
            filesLength = files.length;

        if (files[0]) {
            if (files[0]['size']) {
                if (files[0]['size'] / 1024 / 1024 >= _MaxFileSizeUpload) { // anh qua 2 MB
                    bootbox.showmessage({
                        message: LanguageDic["COMMON_ERROR_MAXFILEUPLOAD"].replace("{0}", _MaxFileSizeUpload)
                    }, false);
                    return false;
                }
            }
        }

        var idImg = 0;
        if (imgs.length > 0)
            idImg = imgs[imgs.length - 1].key + 1;
        for (var i = 0; i < filesLength; i++) {
            if (files[i]['size'] / 1024 / 1024 >= _MaxFileSizeUpload) { // anh qua 2 MB
                continue;
            }

            (function (file) {
                idImg += i;
                var f = files[i]
                var fileReader = new FileReader();
                var number = 1 + Math.floor(Math.random() * 10000);
                var itemId = number + "_" + file.name;

                fileReader.onload = (function (e) {
                    $('<div class="item"><span class="imageThumb"><img src="' + e.target.result + '" title="' + file.name + '"/></span><i class="icon-delete_photo i1 ie-2" data-idimg="' + itemId + '"></i></div>').insertBefore(".list-img .content .imgs-upload");
                });
                fileReader.readAsDataURL(f);
                imgs.push({
                    key: itemId,
                    value: f
                });
            })(files[i]);
        }
        checkHasImage();
        // console.log(imgs);
    });     
}

function checkHasImage() {
    setTimeout(function () {
        $(".list-img .content").css('width', $(".list-img .content .item").length * 115);
        if ($('.list-img .content .item').length > 1) {
            $('.list-img').show();
        }
        else {
            $('.list-img').hide();
            $("#imgs-upload").val('');
        }
    }, 500)
}

function Days(day) {
    $("#Days").val(day);
}

function EmptyUploadFile() {
    $(".list-img > .content > .item").not(":last").html("");
    $("#imgs-upload").value = "";
}

$("#imgs-upload").click(function (event) {
    var target = event.target || event.srcElement;
    if (target.value.length == 0) {
        //
    } else {
        //numFiles = target.files.length;
    }
});

$("#imgs-upload").on("change", function (event) {
    var target = event.target || event.srcElement;
    if (target.value.length == 0) {
        //EmptyUploadFile();
        return false;
    }
});

$("#imgs-upload").on("blur", function (event) {
    var target = event.target || event.srcElement;
    if (target.value.length == 0) {
        //EmptyUploadFile();
        return false;
    }
});

function RemoveOverLay(container) {
    //$('.lazy').lazy();
    setTimeout(function () {
        container.parent().find(".post-overlay").remove();
        container.parent().find(".widget-overlay").remove();
        container.removeClass("hidden").fadeIn();
    }, 500);

    $("a.post-viewmore").click(function (e) {
        e.stopPropagation();
    });
}


function afterGetRecentPosts(data) {
    var container = $(".recent-post");
    if (data) {
        if (data.success) {
            container.html(data.html);
            if (data.html !== "")
                container.fadeIn().removeClass("hidden");
            else
                container.addClass("hidden");
        }
        container.parent().find(".widget-overlay").remove();
    }
}

function GetRecentPosts() {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Home/GetRecentPosts';
    params['requestType'] = 'GET';
    params['data'] = {};
    params['dataType'] = "json";

    params['successCallbackFunction'] = afterGetRecentPosts;
    doAjax(params);
}

function afterGetTraveller(data) {
    var container = $(".top-travel");
    if (data) {
        if (data.success) {
            container.html(data.html);
            if (data.html !== "")
                container.fadeIn().removeClass("hidden");
            else
                container.addClass("hidden");
        }
        container.parent().find(".widget-overlay").remove();
    }
}

function GetTopTralveller() {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Home/GetTopTraveller';
    params['requestType'] = 'GET';
    params['data'] = {};
    params['dataType'] = "json";

    params['successCallbackFunction'] = afterGetTraveller;
    doAjax(params);
}

function afterGetNewsFeedPosts(data) {
    var container = $("#ListPosts");
    if (data !== "" && data) {
        $("#ListPosts").append(data);
        //setTimeout(function () {
        //    //InitLikePost();
        //    //InitPopupDetail();
        //    //InitDeletePost();
        //}, 500);
    } else {
        _hasNoData = true;
    }

    RemoveOverLay(container);
}

function GetNewsFeedPosts() {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Home/GetNewsFeedPosts';
    params['requestType'] = 'GET';
    params['data'] = { page: _currentPage };
    params['dataType'] = "html";

    params['successCallbackFunction'] = afterGetNewsFeedPosts;
    doAjax(params);
}

function follow(e) {
    var userid = $(e).data("user");
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/UserAction/Follow';
    params['requestType'] = 'POST';
    params['data'] = { userid: userid };
    params['dataType'] = "json";
    params['successCallbackFunction'] = afterFollowAction;
    doAjax(params);
}

function afterFollowAction(data) {
    if (data.success == false) {
        bootbox.showmessage({
            message: data.message
        }, false);
        //bootbox.alert(data.message);
    }
    else {
        GetTopTralveller();
    }
}

function AddPostValidation() {
    var des = $("#divdescription").html();
    var title = $("#Title").val();
    if (!title || title === '') {
        bootbox.showmessage({
            message: LanguageDic["VAL_TITLE_NULL"]
        }, false);
        return false;
    }

    if (!des || des === '') {
        bootbox.showmessage({
            message: LanguageDic["VAL_DESCRIPTION_NULL"]
        }, false);
        return false;
    }

    var totalPlace = 0;
    $("#FeedLocations .tag-item").each(function () {
        totalPlace++;
    });

    if (totalPlace <= 0) {
        bootbox.showmessage({
            message: LanguageDic["VAL_PLACE_NULL"]
        }, false);

        $(".add-location").click();
        $("#SearchLocation").focus();
        return false;
    }

    var totalImg = 0;
    var needToUpload = 0;
    $(".list-img .content .item").each(function () {
        if (!$(this).hasClass("item-uploaded")) {
            needToUpload++;
        }
        totalImg++;
    });

    if (totalImg <= 1) {
        bootbox.showmessage({
            message: LanguageDic["COMMON_ERROR_NO_IMAGE"]
        }, false);
        return false;
    } else {      
        //UploadFiles();
        $('#btnCreatePost').button('loading');
        if (needToUpload > 1) {
            UpdateProgress(0);
            UpoadFilesNew();
        } else {
            $("#FrmAddPost").submit();
        }
    }     
}

function afterUploadSuccess(data) {
    if (data) {
        if ((data.Code != 1)) {
            if (data.Msg)
                bootbox.showmessage({
                    message: data.Msg
                }, false);
            else {
                if (data.html)
                    bootbox.showmessage({
                        message: data.html
                    }, false);
            }
        }

        if (data.Data) {
            var uploadedContainer = $("#UploadedImages");
            for (var i = 0; i < data.Data.length; i++) {
                uploadedContainer.append("<input type='text' name='Images' class='hdImages' value='" + data.Data[i] + "'>");
            }
        }
    }

    $("#FrmAddPost").submit();
}

function afterAddPostSuccess(data) {
    $('#btnCreatePost').button('reset');
    if (data) {
        if (!data.success) {
            if (data.code == 103) {
                if (data.html) {
                    ConfirmFirst(NeedToLogout, data.html);
                }
            }

            if (data.message) {
                bootbox.showmessage({
                    message: data.message
                }, false);
            }
            return false;
        }
        var oldItem = $("#PostId").val();

        if ($(".new-feeds-detail").length > 0) {
            return location.reload();
        }

        if (oldItem > 0) {
            $(".post-item-" + oldItem).replaceWith(data.html);
        } else {
            $("#ListPosts").prepend(data.html);
        }       
        HideFeedActions();
        FeedActionsReset();
        UpdateProgress(0);

        imgs = [];
    }
}

function UploadFiles() {
    var params = $.extend({}, doAjax_params_default);

    var data = new FormData();
    //$.each($("#imgs-upload")[0].files, function (i, file) {
    //    data.append('file_' + i, file);
    //});

    for (var i = 0; i < imgs.length; i++) {
        data.append("file_" + i, imgs[i].value);
    }

    // Add the uploaded image content to the form data collection
    data.append("UserId", $("#UserId").val());

    params['url'] = _HaloSocialApi + "/api/post/uploadfiles";
    params['requestType'] = 'POST';
    params['data'] = data;
    params['contentType'] = false;
    params['processData'] = false;


    params['successCallbackFunction'] = afterUploadSuccess;
    doAjax(params);
}

function UpoadFilesNew() {
    var formData = new FormData();

    for (var i = 0; i < imgs.length; i++) {
        formData.append("file_" + i, imgs[i].value);
    }

    formData.append("UserId", $("#UserId").val());

    $.ajax({
        type: 'POST',
        url: _CdnServer + "/api/upload/postimages",
        data: formData,
        xhr: function () {
            var myXhr = $.ajaxSettings.xhr();
            $(".upload-progress").removeClass("hidden");
            if (myXhr.upload) {
                myXhr.upload.addEventListener('progress', progress, false);
            }
            return myXhr;
        },
        cache: false,
        contentType: false,
        processData: false,
        success: function (data) {
            afterUploadSuccess(data);
        },

        error: function (data) {
            console.log('Error');
            console.log(data);
        }
    });
}

function UpdateProgress(percent) {
    var progressBarCurrent = $(".upload-progress .progress-bar");
    progressBarCurrent.attr("aria-valuenow", percent);
    progressBarCurrent.css("width", percent + "%");
    progressBarCurrent.html(percent + "%");

    if (percent <= 0) {
        $(".upload-progress").addClass("hidden");
    } else {
        $(".upload-progress").removeClass("hidden");
    }
}

function progress(e) {
    if (e.lengthComputable) {
        var max = e.total;
        var current = e.loaded;        
        var Percentage = Math.ceil((current * 100) / max);

        UpdateProgress(Percentage);

        if (Percentage >= 100) {
            //process completed  
            UpdateProgress(100);
        }        
    }
}

function ModifyPost(id) {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Post/GetToModify';
    params['requestType'] = 'POST';
    params['data'] = { id : id };
    params['dataType'] = "json";

    params['successCallbackFunction'] = afterClickModifyPost;
    doAjax(params);
}

function afterClickModifyPost(data) {
    if (data) {
        if (data.success) {
            if (data.htmlReturn.length > 0) {
                $(".feed-actions").html(data.htmlReturn);               
                ShowFeedActions();
                $(".feed-actions").addClass("update");
                $(".feed-actions").removeClass("hidden");   

                initLocationAutoComplete();

                EventOnChangeUploadFile();

                checkHasImage();

                $(".add-location").click();
                $("#SearchLocation").focus();

                // Count text when typing title
                var titleLength = $("#Title").val().length;
                if (titleLength > 80) {
                    $("#Title").value = $("#Title").value.substring(0, maxlength);
                } else {
                    $("#Title").parent().find('.count span').text(titleLength);
                }

                InitSavePostEvent();
                initContenteditable();
            }
        } else {
            if (data.message) {
                bootbox.showmessage({
                    message: data.message
                }, false);
            }
        }
    }
}


function InitSavePostEvent() {
    $("#FrmAddPost").on("submit", function (ev) {
        $("#Description").val($("#divdescription").html());

        ev.preventDefault();
        var placeid = "";
        $(".place-item").each(function () {
            if (placeid == "") {
                placeid = $(this).data("id");
            }
            else {
                placeid += "," + $(this).data("id");
            }
        });

        $("#Locations").val(placeid);

        var params = $.extend({}, doAjax_params_default);
        params['url'] = $(this).attr('action');
        params['requestType'] = $(this).attr('method');
        params['data'] = $(this).serialize();
        params['context'] = $("#btnCreatePost");
        params['dataType'] = "json";

        params['successCallbackFunction'] = afterAddPostSuccess;
        doAjax(params);
    });
}
$.getScript("/Scripts/Common/weather.js");


