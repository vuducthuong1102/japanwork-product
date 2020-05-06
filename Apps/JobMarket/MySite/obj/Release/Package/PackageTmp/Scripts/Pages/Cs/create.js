var MemberGlobalObj = function () { };
MemberGlobalObj.prototype.getProfile = function () {
    showItemLoading("#MemberSubContainer");
    setTimeout(function () {
        $('#MemberSubContainer').load("/Member/MyProfilePartial?read_only=true&exclude_ct_add=1", function (response, status, xhr) {
            hideItemLoading("#MemberSubContainer");
            $("#MemberSubContainer").find("h3").removeClass("pl30");
            $.validator.unobtrusive.parse("#frmCs");
        });
    }, 300);
}

MemberGlobalObj.prototype.getEduHistories = function () {
    showItemLoading("#EduHistories");
    setTimeout(function () {
        $('#EduHistories').load("/Cs/DefaultEduHistories", function (response, status, xhr) {
            hideItemLoading("#EduHistories");
        });
    }, 300);
}

MemberGlobalObj.prototype.getWorkHistories = function () {
    showItemLoading("#WorkHistories");
    setTimeout(function () {
        $('#WorkHistories').load("/Cs/DefaultWorkHistories", function (response, status, xhr) {
            hideItemLoading("#WorkHistories");
        });
    }, 300);
}

MemberGlobalObj.prototype.getCertificates = function () {
    showItemLoading("#Certificates");
    setTimeout(function () {
        $('#Certificates').load("/Cs/DefaultCertificates", function (response, status, xhr) {
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

$("#frmCs").submit(function () {
    $(this).validate({
        rules: {
            fullname: {
                required: true,
                minlength: 5
            }
        }
    });
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

    if ($("#frmCs").valid()) {
        var data = new FormData(this);
        var params = $.extend({}, doAjax_params_default);
        params['url'] = '/Cs/CreateCs';
        params['requestType'] = 'POST';
        params['data'] = data;
        params['processData'] = false;
        params['contentType'] = false;
        params['dataType'] = "json";
        params['context'] = "#frmCs";

        params['successCallbackFunction'] = function (result) {
            CatchAjaxResponseWithNotif(result);
        };
        doAjax(params);
    }
    return false;
});

$("body").on("click", ".dl-cs-edu", function () {
    $(this).closest(".edu-history-item").remove();
});

$("body").on("click", ".dl-cs-work", function () {
    $(this).closest(".work-history-item").remove();
});

$("body").on("click", ".dl-cs-cer", function () {
    $(this).closest(".certificate-item").remove();
});

function InitFormElementEvent() {
    $('.select2').select2({
        width: '100%'
    });

    $(".datepicker").mask("99-99-9999");
    $(".datepicker").datepicker({
        format: "dd-mm-yyyy"
    });
}

function RepeaterUpdateCounter() {
    var total = 0;
    $(".work-detail-idx").each(function () {
        total++;
        $(this).html(total);
    });
}

var FormEvents = {
    repeaterBinding: function () {
        $(".Details").repeater({
            initEmpty: !1, defaultValues: { "text-input": "foo" },
            show: function () {
                $(this).slideDown();

                $(".repeater-item").removeClass("new-rpt");
                RepeaterUpdateCounter();

                InitFormElementEvent();
            },
            hide: function (e) {
                $(this).slideUp(e);

                setTimeout(function () {
                    RepeaterUpdateCounter();
                }, 1000);
            }
        });
    },
    eventBinding: function () {
        InitFormElementEvent();
    }
};

$("body").on("click", ".up-cs-work-default", function () {
    var idx = $(this).data("idx");
    var json_obj = $("#work_history_idx_" + idx).val();

    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Cs/UpdateDefaultCsWorkHistory';
    params['requestType'] = 'POST';
    params['data'] = { json_obj: json_obj };
    params['dataType'] = "html";

    params['successCallbackFunction'] = function (result) {
        if (result) {
            $("#myModalContent").html(result);
            $("#myModal").modal("show");

            $("#frmUpdateCsWorkHistory").attr("action", "/Cs/ConfirmUpdateDefaultCsWorkHistory");
            $("#frmUpdateCsWorkHistory").addClass("new-work-frm");
        }
    };
    doAjax(params);
});

$(function () {
    var globalMember = new MemberGlobalObj();
    globalMember.init();
}); 