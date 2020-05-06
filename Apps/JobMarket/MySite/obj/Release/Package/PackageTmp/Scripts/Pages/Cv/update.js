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
            var url_tk = $("#CvProfileTk").val();
            $('#MemberSubContainer').load("/Member/MyProfilePartial?read_only=true&" + url_tk, function (response, status, xhr) {
                hideItemLoading("#MemberSubContainer");
                $("#MemberSubContainer").find("h3").removeClass("pl30");
                $.validator.unobtrusive.parse("#frmCv");
            });
        }, 300);
    },
    eduHistories: function () {
        showItemLoading("#EduHistories");
        setTimeout(function () {
            var url_tk = $("#CvEduTk").val();
            $('#EduHistories').load("/Cv/EduHistories?" + url_tk, function (response, status, xhr) {
                hideItemLoading("#EduHistories");
            });
        }, 300);
    },
    workHistories: function () {
        showItemLoading("#WorkHistories");
        setTimeout(function () {
            var url_tk = $("#CvWorkTk").val();
            $('#WorkHistories').load("/Cv/WorkHistories?" + url_tk, function (response, status, xhr) {
                hideItemLoading("#WorkHistories");
            });
        }, 300);
    },
    certificates: function () {
        showItemLoading("#Certificates");
        setTimeout(function () {
            var url_tk = $("#CvCerTk").val();
            $('#Certificates').load("/Cv/Certificates?" + url_tk, function (response, status, xhr) {
                hideItemLoading("#Certificates");
            });
        }, 300);
    },
    formEventsRegister: function () {
        $(".datepicker").datepicker({
            format: "dd-mm-yyyy"
        });

        $(".datepicker").mask("99-99-9999");

        $("body").on("click", ".dl-cv-edu", function () {
            $(this).closest(".edu-history-item").remove();
        });

        $("body").on("click", ".dl-cv-work", function () {
            $(this).closest(".work-history-item").remove();
        });

        $("body").on("click", ".dl-cv-cer", function () {
            $(this).closest(".certificate-item").remove();
        });

        $("#frmCv").submit(function () {           
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

            if ($("#frmCv").valid()) {
                var data = new FormData(this);
                var params = $.extend({}, doAjax_params_default);
                params['url'] = '/Cv/UpdateCv';
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
    }
};

$(function () {
    MemberGlobal.init();
});