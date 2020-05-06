var MemberGlobal = {
    init: function () {
        this.profile();
        this.eduHistories();
        this.workHistories();
        this.certificates();
        this.formEventsRegister();
    },
    profile: function () {
        showItemLoading("#MemberSubContainer");
        setTimeout(function () {
            var url_tk = $("#CsProfileTk").val();
            $('#MemberSubContainer').load("/Member/MyProfilePartial?read_only=true&" + url_tk, function (response, status, xhr) {
                hideItemLoading("#MemberSubContainer");
                $("#MemberSubContainer").find("h3").removeClass("pl30");
                $.validator.unobtrusive.parse("#frmCs");
            });
        }, 300);
    },
    eduHistories: function () {
        showItemLoading("#EduHistories");
        setTimeout(function () {
            var url_tk = $("#CsEduTk").val();
            $('#EduHistories').load("/Cs/EduHistories?" + url_tk, function (response, status, xhr) {
                hideItemLoading("#EduHistories");
            });
        }, 300);
    },
    workHistories: function () {
        showItemLoading("#WorkHistories");
        setTimeout(function () {
            var url_tk = $("#CsWorkTk").val();
            $('#WorkHistories').load("/Cs/WorkHistories?" + url_tk, function (response, status, xhr) {
                hideItemLoading("#WorkHistories");
            });
        }, 300);
    },
    certificates: function () {
        showItemLoading("#Certificates");
        setTimeout(function () {
            var url_tk = $("#CsCerTk").val();
            $('#Certificates').load("/Cs/Certificates?" + url_tk, function (response, status, xhr) {
                hideItemLoading("#Certificates");
            });
        }, 300);
    },
    formEventsRegister: function () {
        $(".datepicker").datepicker({
            format: "dd-mm-yyyy"
        });

        $(".datepicker").mask("99-99-9999");

        $("body").on("click", ".dl-cs-edu", function () {
            $(this).closest(".edu-history-item").remove();
        });

        $("body").on("click", ".dl-cs-work", function () {
            $(this).closest(".work-history-item").remove();
        });

        $("body").on("click", ".dl-cs-cer", function () {
            $(this).closest(".certificate-item").remove();
        });

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
                params['url'] = '/Cs/UpdateCs';
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
    }
};

function InitFormElementEvent() {
    $('.select2').select2({
        width: '100%'
    });

    $(".datepicker").mask("99-99-9999");
    $(".datepicker").datepicker({
        format: "dd-mm-yyyy",
        showButtonPanel: true,
        closeText: LanguageDic['BT_DELETE']
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

$(function () {
    MemberGlobal.init();
});