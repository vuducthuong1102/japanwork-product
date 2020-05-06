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