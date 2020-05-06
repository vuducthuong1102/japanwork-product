var MemberGlobalObj = function () { };
MemberGlobalObj.prototype.getProfile = function () {
    showItemLoading("#MemberSubContainer");
    setTimeout(function () {
        $('#MemberSubContainer').load("/Member/MyProfilePartial", function (response, status, xhr) {
            hideItemLoading("#MemberSubContainer");
            $("#MemberSubContainer").find("h3").removeClass("pl30");
            $.validator.unobtrusive.parse("#frmCv");
        });
    }, 300);
}

MemberGlobalObj.prototype.getEduHistories = function () {
    showItemLoading("#EduHistories");
    setTimeout(function () {
        $('#EduHistories').load("/Cv/DefaultEduHistories", function (response, status, xhr) {
            hideItemLoading("#EduHistories");
        });
    }, 300);
}

MemberGlobalObj.prototype.getWorkHistories = function () {
    showItemLoading("#WorkHistories");
    setTimeout(function () {
        $('#WorkHistories').load("/Cv/DefaultWorkHistories", function (response, status, xhr) {
            hideItemLoading("#WorkHistories");
        });
    }, 300);
}

MemberGlobalObj.prototype.getCertificates = function () {
    showItemLoading("#Certificates");
    setTimeout(function () {
        $('#Certificates').load("/Cv/DefaultCertificates", function (response, status, xhr) {
            hideItemLoading("#Certificates");
        });
    }, 300);
}

MemberGlobalObj.prototype.init = function () {
    this.getProfile();
    this.getEduHistories();
    this.getWorkHistories();
    this.getCertificates();

    $(".datepicker").datepicker({
        format: "dd-mm-yyyy"
    });

    $(".datepicker").mask("99-99-9999");
}

$("#frmCv").submit(function () {
    var eduCounter = 0;
    $(".edu-history-item").each(function () {
        $(this).find(".edu-prop").each(function () {
            var propName = $(this).data("prop");
            $(this).attr("name", "EduHistories[" + eduCounter + "]." + propName);
        });
        eduCounter++;
    });

    var workCounter = 0;
    $(".work-history-item").each(function () {
        $(this).find(".work-prop").each(function () {
            var propName = $(this).data("prop");
            $(this).attr("name", "WorkHistories[" + workCounter + "]." + propName);
        });
        workCounter++;
    });

    var cerCounter = 0;
    $(".certificate-item").each(function () {
        $(this).find(".cer-prop").each(function () {
            var propName = $(this).data("prop");
            $(this).attr("name", "Certificates[" + cerCounter + "]." + propName);
        });
        cerCounter++;
    });

    if ($("#frmCv").valid()) {
        var data = new FormData(this);
        var params = $.extend({}, doAjax_params_default);
        params['url'] = '/Cv/CreateCv';
        params['requestType'] = 'POST';
        params['data'] = data;
        params['processData'] = false;
        params['contentType'] = false;
        params['dataType'] = "json";
        params['context'] = "#frmCv";

        params['successCallbackFunction'] = function (result) {
            CatchAjaxResponseWithNotif(result);
        };
        doAjax(params);
    }
    return false;
});

$("body").on("click", ".dl-cv-edu", function () {
    $(this).closest(".edu-history-item").remove();
});

$("body").on("click", ".dl-cv-work", function () {
    $(this).closest(".work-history-item").remove();
});

$("body").on("click", ".dl-cv-cer", function () {
    $(this).closest(".certificate-item").remove();
});

$(function () {
    var globalMember = new MemberGlobalObj();
    globalMember.init();
}); 