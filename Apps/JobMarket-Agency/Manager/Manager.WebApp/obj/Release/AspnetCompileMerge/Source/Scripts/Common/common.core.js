$.fn.exists = function () {
    return this.length !== 0;
};

String.prototype.endsWith = function (suffix) {
    return this.indexOf(suffix, this.length - suffix.length) !== -1;
};

var doAjax_params_default = {
    'showloading': true,
    'url': null,
    'requestType': "GET",
    'contentType': 'application/x-www-form-urlencoded; charset=UTF-8',
    'dataType': 'json',
    'processData': true,
    'async': true,
    'data': {},
    'beforeSendCallbackFunction': null,
    'successCallbackFunction': null,
    'completeCallbackFunction': null,
    'errorCallBackFunction': null,
    'context': null
};

$(document).on("click", function (e) {
    if ($(e.target).closest(".tsb-form").length === 0) {
        SearchOptionClose();
    }
});

$(document).on("click", ".select2-search__field", function (e) {
    e.stopPropagation();
});

//$(document).on("click", ".btn-disabled", function (ev) {
//    ev.preventDefault();
//    //$(this).prev().prop("disabled", true);
//    //alert(12);
//    //setTimeout(function () {
//    //    $(this).prev().prop("disabled", false);
//    //}, 300)
//})

$(document).on("click", ".btn-search", function (e) {
    e.stopPropagation();
    $(".search-advance").addClass("hidden");
    $(".tsb-form").removeClass("show");
});

$(document).on("click", ".input-search", function (ev) {
    ev.stopPropagation();
    SearchOptionOpen();
    UpdateSelect($(this).closest("form"));
});

$(document).on("click", ".btn-error-permision", function (e) {
    $.showErrorMessage(LanguageDic['LB_NOTIFICATION'], LanguageDic['ERROR_ACTION_DENNIED'], null);
    return false;
});

$(document).on("click", ".btn-error-permission", function (e) {
    $.showErrorMessage(LanguageDic['LB_NOTIFICATION'], LanguageDic['ERROR_ACTION_DENNIED'], null);
    return false;
});

$(document).on("click", ".btn-error-incorrect-user", function (e) {
    $.showErrorMessage(LanguageDic['LB_NOTIFICATION'], LanguageDic['ERROR_INCORRECT_USER'], null);
    return false;
});

$(document).on("click", ".btn-process-end", function (e) {
    $.showErrorMessage(LanguageDic['LB_NOTIFICATION'], LanguageDic['ERROR_PROCESS_END'], null);
    return false;
});

$(document).on("click", ".btn-no-permission-to-use", function (e) {
    $.showErrorMessage(LanguageDic['LB_NOTIFICATION'], LanguageDic['ERROR_ACTION_DENNIED'], null);
    return false;
});

$(document).on("click", ".btn-error-permision-edit", function (e) {
    $.showErrorMessage(LanguageDic['LB_NOTIFICATION'], LanguageDic['ERROR_ACTION_DENNIED_EDIT'], null);
    return false;
});

function SearchOptionClose() {
    if ($(".search-advance").hasClass("show")) {
        $(".search-advance").addClass("hidden");
        $(".tsb-form").removeClass("show");
    }
}

setDefaultPage();

function setDefaultPage() {
    var searchOpt = $(".search-option").html();
    if (searchOpt) {
        if (searchOpt.length !== 33) {
            $(".input-search").attr("placeholder", $(".search-option").find(".title-search").val());
        }
    }
}

function SearchOptionOpen() {
    $(".search-advance").addClass("show");
    $(".tsb-form").addClass("show");
    $(".search-advance").removeClass("hidden");
}

function doAjax(doAjax_params) {

    var url = doAjax_params['url'];
    var showloading = doAjax_params['showloading'];
    var requestType = doAjax_params['requestType'];
    var contentType = doAjax_params['contentType'];
    var dataType = doAjax_params['dataType'];
    var data = doAjax_params['data'];
    var context = doAjax_params['context'];
    var processData = doAjax_params['processData'];
    var async = doAjax_params['async'];
    var beforeSendCallbackFunction = doAjax_params['beforeSendCallbackFunction'];
    var successCallbackFunction = doAjax_params['successCallbackFunction'];
    var completeCallbackFunction = doAjax_params['completeCallbackFunction'];
    var errorCallBackFunction = doAjax_params['errorCallBackFunction'];

    if (context) {
        var me = $(context);

        if (me.data('requestRunning')) {
            return false;
        }

        me.data('requestRunning', true);
    }

    //data.push({ __RequestVerificationToken: $('input[name = "__RequestVerificationToken"]').val() });
    if (requestType === "POST" || requestType === "post") {
        //make sure that url ends with '/'
        if (!url.endsWith("/")) {
            url = url + "/";
        }

        data.__RequestVerificationToken = $('input[name = "__RequestVerificationToken"]').val();
    }

    $.ajax({
        url: url,
        crossDomain: true,
        async: async,
        type: requestType,
        contentType: contentType,
        dataType: dataType,
        processData: processData,
        data: data,
        context: context,
        headers: { 'X-Requested-With': 'XMLHttpRequest' },
        beforeSend: function (jqXHR, settings) {
            if (showloading) {
                showLoading();
            }

            if (typeof beforeSendCallbackFunction === "function") {
                if (context)
                    beforeSendCallbackFunction(context);
                else
                    beforeSendCallbackFunction();
            }
        },
        success: function (data, textStatus, jqXHR) {
            if (data.Error) {
                if (data.clientcallback)
                    eval(data.clientcallback);

                return false;
            }

            if (typeof successCallbackFunction === "function") {
                successCallbackFunction(data);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (typeof errorCallBackFunction === "function") {
                errorCallBackFunction(errorThrown);
            }

        },
        complete: function (jqXHR, textStatus) {
            hideLoading();
            if (typeof completeCallbackFunction === "function") {
                completeCallbackFunction();
            }

            if (context)
                me.data('requestRunning', false);
        }
    });
}
$(".input-off-enter").keypress(function (e) {
    e.stopPropagation();
    if ($(this).hasClass("input-off-enter")) {
        return false;
    }
});

$("body").on("change", ".change-type", function (e) {
    e.stopPropagation();
    var thisForm = $(this).closest("form");
    var type = $(this).val();

    if (type == "0") {
        thisForm.find(".check-status").removeClass("hidden");
    }
    else {
        thisForm.find(".check-status").addClass("hidden");
    }
    thisForm.attr("action", $(this).find("option:selected").attr("data-action"));
});

function ResetFormSearch(thisForm) {
    $(".input-search").val("");
    thisForm.find(".m-input").val("");
    thisForm.find(".number").val(0);
    thisForm.find(".selectpicker-search").each(function () {
        $(this).val($(this).find("option:first").val());
        $(this).selectpicker("refresh");
    });

    thisForm.find(".m-select2-search").each(function () {
        var val = $(this).find("option:first").val();
        $(this).val(val);
    });
}

$("body").on("click", ".reset-btn", function (e) {
    e.stopPropagation();
    var thisForm = $(this).closest("form");
    thisForm.find("input[name=type]").each(function () {
        if ($(this).is(":checked") && !$(this).hasClass("checked")) {
            $(this).prop('checked', false);
        }
        if (!$(this).is(":checked") && $(this).hasClass("checked")) {
            $(this).prop('checked', true);
        }
    })
    thisForm.find('input:checkbox').each(function () {
        $(this).prop('checked', false);
    });
    if ($(".search-option").html().length == 33) {
        //$(".search-advance").html("");
        $("input[name=type]").each(function () {
            if ($(this).is(":checked")) {
                RenderFormSearch($(this));
            }
        })
        //$("input[name=Keyword]").addClass("input-off-enter");
        //thisForm.removeAttr('action');
        //$(".search-advance").addClass("hidden-search");
    }
    else {
        $(".input-search").removeClass("input-off-enter");
        $(".search-advance").html($(".search-option").html());
        $(".input-search").attr("placeholder", $(".search-option").find(".title-search").val());
        UpdateSelect(thisForm)
    }
    $(".input-search").val("");
    thisForm.find(".m-input").val("");
    thisForm.find(".number").val(0);
    thisForm.find(".selectpicker-search").each(function () {
        $(this).val($(this).find("option:first").val());
        $(this).selectpicker("refresh");
    });

    thisForm.find(".m-select2-search").each(function () {
        var val = $(this).find("option:first").val();
        $(this).val(val);

    });

    if (thisForm.find("select.select-employment-type").length > 0) {
        thisForm.find("select.select-employment-type").change();
    }
});

function RemoveItemFromArray(arr, item) {
    var index = arr.indexOf(item);
    if (index > -1) {
        arr.splice(index, 1);
    }
}

function ConvertStringToListArray(str, seperaterChar = ",") {
    if (str === null || str === "" || str === "NaN") {
        return [];
    }

    var arr = str.split(seperaterChar).map(function (item) {
        return parseInt(item, 10);
    });

    return arr;
}


const numberWithCommas = (x, seperateChar = ".") => {
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, seperateChar);
}

function FeatureCommingSoon() {
    bootbox.alert(LanguageDic["LB_FEATURE_COMMING"]);
}


function AutoSetTime() {
    $(".livetimestamp").each(function () {
        SettingElement(this);
    });
    $(".livetimestamp-title").each(function () {
        SettingElementTitle(this);
    });
}

function NeedToLogin() {
    window.location.href = "/webauth/login";
}

$("body").on("click", ".btn-back", function () {
    if ((1 < history.length) && document.referrer) {
        history.back();
    } else {
        if ($(this).data("back")) {
            window.location.href = $(this).data("back");
        } else {
            window.location.href = "/";
        }
    }
});

function NeedToLogout() {
    window.location.href = "/webauth/logout";
}

(function ($) {
    $.fn.changeElementType = function (newType) {
        var attrs = {};

        $.each(this[0].attributes, function (idx, attr) {
            attrs[attr.nodeName] = attr.nodeValue;
        });

        this.replaceWith(function () {
            return $("<" + newType + "/>", attrs).append($(this).contents());
        });
    }
})(jQuery);

function InitNumberFormat(seperateChar = ".") {
    $(".number-format").bind("input", function () {
        // format number
        $(this).val(function (index, value) {
            return value
                .replace(/\D/g, "")
                .replace(/\B(?=(\d{3})+(?!\d))/g, ",")
                ;
        });

        var val = $(this).val().toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,');

        var currentVal = $(this).val().replace(",", "");
        //currentVal = $(this).val().replace(".", "");
        var newValue = $(this).val().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,');
        $(this).val(newValue);

        //return $(this).val().toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,')
    });

}

function searching(keyword) {
    var url = window.location.href;

    if (url.indexOf("/search/") > -1) {
        var index = url.indexOf("=");
        history.pushState({}, document.title, url.substring(0, index + 1) + keyword);
        reSearch();
    }
    else {
        window.location.href = "/search/top/q=" + keyword;
    }
}

function GetDateTimeNowByFormat() {
    var mydate = new Date();
    year = mydate.getFullYear();
    month = mydate.getMonth();

    months = new Array('01', '02', '03', '04', '05', '06', '07', '08', '09', '10', '11', '12');
    d = mydate.getDate();
    day = mydate.getDay();
    days = new Array('Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday');
    h = mydate.getHours();

    var strMonth = "";
    var strDay = "";
    if (h < 10) {
        h = "0" + h;
    }
    m = mydate.getMinutes();
    if (m < 10) {
        m = "0" + m;
    }

    if (month < 10) {
        strMonth = "0" + month;
    }

    if (day < 10) {
        strDay = "0" + day;
    }

    s = mydate.getSeconds();
    if (s < 10) {
        s = "0" + s;
    }

    result = '' + year + months[month] + d + h + m + s;

    return result;
}

function DynamicRealTime() {
    var mydate = new Date();
    year = mydate.getFullYear();
    month = mydate.getMonth();

    months = new Array('01', '02', '03', '04', '05', '06', '07', '08', '09', '10', '11', '12');
    d = mydate.getDate();
    day = mydate.getDay();
    days = new Array('Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday');
    h = mydate.getHours();

    var strMonth = "";
    var strDay = "";
    if (h < 10) {
        h = "0" + h;
    }
    m = mydate.getMinutes();
    if (m < 10) {
        m = "0" + m;
    }

    if (month < 10) {
        strMonth = "0" + month;
    }

    if (day < 10) {
        strDay = "0" + day;
    }

    s = mydate.getSeconds();
    if (s < 10) {
        s = "0" + s;
    }

    result = '' + year + '/' + months[month] + '/' + d + ' ' + h + ':' + m + ':' + s;
    $('.dynamic-timer').html(result);
    setTimeout('DynamicRealTime();', '1000');
    return true;
}


function ReplaceErrorImage() {
    $("img").on("error", function () {
        $(this).attr("src", "/Content/images/no-image.png");
    });
}

function initContenteditable() {
    var editors = document.querySelectorAll("div[contenteditable]");
    if (editors.length > 0) {
        for (var i = 0; i < editors.length; i++) {
            editors[i].addEventListener("paste", function (e) {
                e.preventDefault();
                var text = e.clipboardData.getData("text/plain");
                document.execCommand("insertText", true, text);
                if ($(this).find("div").length > 0) {
                    $(this).find("div").changeElementType("span");
                }
                $(this).html($(this).html().replace(/<span><\/span>/g, "<br>"));
                $(this).find("span").each(function () {
                    if ($(this).text().length == 1) {
                        $(this).replaceWith("<br>");
                    }
                    else {
                        $(this).after("<br>");
                    }
                });
            });
        }
    }
}

function initGetTextInContentEditale() {

    var editors = document.querySelectorAll("div[contenteditable].msg2send");
    if (editors.length > 0) {
        for (var i = 0; i < editors.length; i++) {
            editors[i].addEventListener("paste", function (e) {
                e.preventDefault();
                var target = this;
                var text = e.clipboardData.getData("text/plain");
                document.execCommand("insertHTML", false, text);
                if ($(this).find("div").length > 0) {
                    $(this).find("div").changeElementType("span");
                }
                $(this).html($(this).html().replace(/<span><\/span>/g, "<br>"));
                $(this).find("span").each(function () {
                    if ($(this).text().length == 1) {
                        $(this).replaceWith("<br>");
                    }
                    else {
                        $(this).after("<br>");
                    }
                });

                placeCaretAtEnd(target);
            });
        }
    }
}

function placeCaretAtEnd(el) {
    el.focus();
    if (typeof window.getSelection != "undefined"
        && typeof document.createRange != "undefined") {
        var range = document.createRange();
        range.selectNodeContents(el);
        range.collapse(false);
        var sel = window.getSelection();
        sel.removeAllRanges();
        sel.addRange(range);
    } else if (typeof document.body.createTextRange != "undefined") {
        var textRange = document.body.createTextRange();
        textRange.moveToElementText(el);
        textRange.collapse(false);
        textRange.select();
    }
    $(el).parent().scrollTop($(el).parent()[0].scrollHeight);
}


var confirmButtons = {
    cancel: {
        label: '<i class="fa fa-remove"></i> ' + LanguageDic["BT_CANCEL"],
        className: 'btn-secondary button-cancel'
    },
    confirm: {
        label: '<i class="fa fa-check"></i> ' + LanguageDic["BT_ALLOW"],
        className: 'btn-info'
    }
};

function ConfirmFirst(callback, message, data) {
    if (!message)
        message = LanguageDic["LB_CONFIRM"];
    bootbox.confirm({
        message: message,
        buttons: confirmButtons,
        callback: function (confirmed) {
            if (confirmed)
                callback(data);
        }
    });
    return false;
}

function ConfirmDynamic(title, message) {
    if (!message)
        message = LanguageDic["LB_CONFIRM"];
    bootbox.confirm({
        title: title,
        message: message,
        buttons: confirmButtons,
        callback: function (confirmed) {
            if (confirmed) {
                return true;
            }
            else {
                return false;
            }
        }
    });
}

bootbox.showmessage = function (options, isSuccess, clientcallback) {
    // Set the defaults
    var className = (!isSuccess) ? 'text-danger' : 'text-info';
    var title = (!isSuccess) ? LanguageDic['LB_ERROR'] : LanguageDic['LB_NOTIFICATION'];
    var defaults = {
        className: className,
        title: title,
        message: 'An Error has occurred. Please contact your system administrator.',
        closeButton: false,
        buttons: {
            "OK": function () {
                if (clientcallback) {
                    clientcallback();
                }
            }
        }
    };

    // Extend the defaults with any passed in options
    var settings = $.extend(defaults, options);
    var iconHtml = (!isSuccess) ? '<i class="fa fa-warning"></i>' : '<i class="fa fa-check"></i>';
    // Build and show the dialog
    bootbox.dialog({
        className: settings.className,
        title: settings.title,
        message: iconHtml + ' ' + settings.message,
        closeButton: settings.closeButton,
        buttons: settings.buttons
    });
};

function ScrollToElement(el) {
    el.addClass("focuss");
    $('html, body').animate({
        scrollTop: (el.offset().top)
    }, 500);

    setTimeout(function () {
        el.removeClass("focuss");
    }, 3000);
}

function DoNoThing() {

}

function ShowMsg(msg, isSuccess) {
    if (isSuccess)
        toastr["success"](msg, 'Thông báo');
    else
        toastr["error"](msg, 'Thông báo');
}

function formatErrorMessage(jqXHR, exception) {

    if (jqXHR.status === 0) {
        return ('Not connected.\nPlease verify your network connection.');
    } else if (jqXHR.status == 404) {
        return ('The requested page not found. [404].\n' + jqXHR.responseText);
    } else if (jqXHR.status == 500) {
        var resultObj = $.parseJSON(jqXHR.responseText);
        return ('' + resultObj.message);
    } else if (exception === 'parsererror') {
        return ('Requested JSON parse failed.');
    } else if (exception === 'timeout') {
        return ('Time out error.');
    } else if (exception === 'abort') {
        return ('Ajax request aborted.');
    } else {
        return ('Uncaught Error.\n' + jqXHR.responseText);
    }
}

function ShowMyModalAgain(modalId = "myModal") {
    var myModal = $("#" + modalId);
    myModal.addClass("re-show");
    myModal.modal("toggle");
}

function showLoading() {
    $('.common-loading').show();
}
function hideLoading() {
    $('.common-loading').fadeOut(1000);
}

function urlfriendly(str) {
    //Tiến hành thay thế , lọc bỏ dấu cho chuỗi
    for (var i = 1; i < VietnameseSigns.Length; i++) {
        for (var j = 0; j < VietnameseSigns[i].Length; j++)
            str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
    }

    //str = str.Replace(" ", "-").Replace(",", "-").Replace(".", "-").Replace("–","-").Replace("&","-").ToLower();
    //while (str.contains("--"))
    //{
    //    str = str.Replace("--", "-");
    //}


    str = URLFriendly(str);
    //console.log(str);
    if (str == "") {
        str = "invalid";
    }

    return str;
}

var VietnameseSigns = [

    "aAeEoOuUiIdDyY",

    "áàạảãâấầậẩẫăắằặẳẵ",

    "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

    "éèẹẻẽêếềệểễ",

    "ÉÈẸẺẼÊẾỀỆỂỄ",

    "óòọỏõôốồộổỗơớờợởỡ",

    "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

    "úùụủũưứừựửữ",

    "ÚÙỤỦŨƯỨỪỰỬỮ",

    "íìịỉĩ",

    "ÍÌỊỈĨ",

    "đ",

    "Đ",

    "ýỳỵỷỹ",

    "ÝỲỴỶỸ"

];

function URLFriendly(title) {
    if (title == null) return "";
    title = title.replace(".", " ");

    var maxlen = 80;
    var len = title.length;
    var prevdash = false;
    var sb = "";
    var c;

    for (var i = 0; i < len; i++) {
        if (i <= maxlen) {
            c = title[i];
            if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9')) {
                sb += c;
                prevdash = false;
            }
            else if (c >= 'A' && c <= 'Z') {
                // tricky way to convert to lowercase
                sb += c.toLowerCase();
                prevdash = false;
            }
            else if (c == ' ' || c == ',' || c == '.' || c == '/' ||
                c == '\\' || c == '-' || c == '_' || c == '=') {
                if (!prevdash && sb.length > 0) {
                    sb += '-';
                    prevdash = true;
                }
            }
            else {
                var prevlen = sb.length;
                sb += RemapInternationalCharToAscii(c);
                if (prevlen != sb.length) prevdash = false;
            }
        }
    }

    //console.log(sb);
    if (prevdash)
        return sb.Substring(0, sb.length - 1);
    else
        return sb;
}

function RemapInternationalCharToAscii(c) {
    var s = c.toLowerCase();
    if ("àáảạãâẩầấậẫăẳắằặẵäåą".indexOf(s) != -1) {
        return "a";
    }
    else if ("èéẹẻẽêểếễệềëę".indexOf(s) != -1) {
        return "e";
    }
    else if ("ìíĩỉịîïı".indexOf(s) != -1) {
        return "i";
    }
    else if ("òóỏọõôổộốồỗõöøőð".indexOf(s) != -1) {
        return "o";
    }
    else if ("ùúũủụûüŭů".indexOf(s) != -1) {
        return "u";
    }
    else if ("çćčĉ".indexOf(s) != -1) {
        return "c";
    }
    else if ("żźž".indexOf(s) != -1) {
        return "z";
    }
    else if ("śşšŝ".indexOf(s) != -1) {
        return "s";
    }
    else if ("ñń".indexOf(s) != -1) {
        return "n";
    }
    else if ("ýỳỵỷỹÿ".indexOf(s) != -1) {
        return "y";
    }
    else if ("ğĝ".indexOf(s) != -1) {
        return "g";
    }
    else if (s == 'ř') {
        return "r";
    }
    else if (s == 'ł') {
        return "l";
    }
    else if (s == 'đ') {
        return "d";
    }
    else if (s == 'ß') {
        return "ss";
    }
    else if (s == 'Þ') {
        return "th";
    }
    else if (s == 'ĥ') {
        return "h";
    }
    else if (s == 'ĵ') {
        return "j";
    }
    else {
        return "";
    }
}
function forceNumber(n, t) {
    var r = window.Event && t.which ? t.which : t.keyCode,
        i;
    return r == 9 ? !0 : r >= 48 && r <= 57 || r == 8 ? (i = n.value.replace(/,/g, ""), r == 8 && i.length != 0 && (i = i.substr(0, i.length - 1)), parseFloat(i) == 0 && (i = ""), !0) : (t.returnValue = !1, !1)
}

function removeSigns(str) {
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    str = str.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, "A");
    str = str.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, "E");
    str = str.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, "I");
    str = str.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, "O");
    str = str.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, "U");
    str = str.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, "Y");
    str = str.replace(/Đ/g, "D");
    return str;
}

var slug = function (str) {
    str = str.replace(/^\s+|\s+$/g, ''); // trim
    str = str.toLowerCase();

    str = removeSigns(str);
    // remove accents, swap ñ for n, etc
    var from = "ÁÄÂÀÃÅČÇĆĎÉĚËÈÊẼĔȆĞÍÌÎÏİŇÑÓÖÒÔÕØŘŔŠŞŤÚŮÜÙÛÝŸŽáäâàãåčçćďéěëèêẽĕȇğíìîïıňñóöòôõøðřŕšşťúůüùûýÿžþÞĐđßÆa·/_,:;";
    var to = "AAAAAACCCDEEEEEEEEGIIIIINNOOOOOORRSSTUUUUUYYZaaaaaacccdeeeeeeeegiiiiinnooooooorrsstuuuuuyyzbBDdBAa------";
    for (var i = 0, l = from.length; i < l; i++) {
        str = str.replace(new RegExp(from.charAt(i), 'g'), to.charAt(i));
    }

    str = str.replace(/[^a-z0-9 -]/g, '') // remove invalid chars
        .replace(/\s+/g, '-') // collapse whitespace and replace by -
        .replace(/-+/g, '-'); // collapse dashes

    return str;
};

function exportTableToCSV($table, filename) {
    var universalBOM = "\uFEFF";
    var $rows = $table.find('tr:has(td),tr:has(th)'),

        // Temporary delimiter characters unlikely to be typed by keyboard
        // This is to avoid accidentally splitting the actual contents
        tmpColDelim = String.fromCharCode(11), // vertical tab character
        tmpRowDelim = String.fromCharCode(0), // null character

        // actual delimiter characters for CSV format
        colDelim = '","',
        rowDelim = '"\r\n"',

        // Grab text from table into CSV formatted string
        csv = '"' + $rows.map(function (i, row) {
            var $row = jQuery(row), $cols = $row.find('td,th');

            return $cols.map(function (j, col) {

                var $col = jQuery(col), text = $col.text().trim();
                return text; // escape double quotes

            }).get().join(tmpColDelim);

        }).get().join(tmpRowDelim)
            .split(tmpRowDelim).join(rowDelim)
            .split(tmpColDelim).join(colDelim) + '"',



        // Data URI        
        csvData = 'data:application/csv;charset=utf-8,' + encodeURIComponent(universalBOM + csv);

    if (window.navigator.msSaveBlob) { // IE 10+
        //alert('IE' + csv);
        window.navigator.msSaveOrOpenBlob(new Blob([csv], { type: "text/plain;charset=utf-8;" }), filename);
    }
    else {
        jQuery(this).attr({ 'download': filename, 'href': csvData, 'target': '_blank' });
    }
}

// This must be a hyperlink
$("body").on("click", ".exportcsv", function (event) {
    var fileName = "";
    fileName = $(this).data("filename");

    if (fileName.length === 0) {
        fileName = "Export";
    }

    fileName = fileName + "_" + GetDateTimeNowByFormat();

    var tableId = "";
    tableId = $(this).data("tableid");

    if (tableId.length === 0) {
        tableId = "dataTable";
    }

    exportTableToCSV.apply(this, [$('#' + tableId), fileName + '.csv']);
});

function tableToExcel(id, sheetname, filename, title, info, footer, subtitle) {
    var tab_text = '\uFEFF';
    tab_text = tab_text + '<html xmlns:v="urn:schemas-microsoft-com:vml" xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40">';
    tab_text = tab_text + '<head>';
    tab_text = tab_text + '<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />';
    tab_text = tab_text + '<meta name="ProgId" content="Excel.Sheet" />';
    tab_text = tab_text + '<meta name="Generator" content="Microsoft Excel 11" />';
    tab_text = tab_text + '<title>Sample</title>';
    tab_text = tab_text + '<!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet>';
    tab_text = tab_text + '<x:Name>' + ((sheetname) ? sheetname : 'Sheet 1') + '</x:Name>';
    tab_text = tab_text + '<x:WorksheetOptions><x:Panes></x:Panes></x:WorksheetOptions></x:ExcelWorksheet>';
    tab_text = tab_text + '</x:ExcelWorksheets></x:ExcelWorkbook>';
    tab_text = tab_text + '</xml><![endif]--></head><body>';
    if (title != null && title != "") {
        tab_text += '           <span><b>CÔNG TY TNHH XXX</b></span><br>';
        tab_text += '           <span><b>30 Floor, Icon 4 Tower, 243A De La Thanh Str, Dong Da Dist, Hanoi, Vietnam</b></span><br>';
        tab_text += '           <span><b>Tax: 01237894321</b><span><br><br>';

        tab_text += '<center style="font-size: 150%;"><b>' + title + '</b></center><br>';
    }
    if (info != null && info != "") {
        tab_text += info + '<br>';
    }
    if (subtitle != null && subtitle != "") {
        tab_text += subtitle;
    }
    tab_text = tab_text + '<table border="1px">';
    var exportTable = $('#' + id).clone();
    exportTable.find('input').each(function (index, elem) { $(elem).remove(); });
    tab_text = tab_text + exportTable.html();
    tab_text = tab_text + '</table><br>';
    if (footer != null && footer != "") {
        tab_text += footer;
    }
    tab_text += '</body></html> ';

    var fileName = filename + '.xls';
    var blob = new Blob([tab_text], { type: "application/vnd.ms-excel" })
    window.saveAs(blob, fileName);
}

function PrintElem(elem) {
    var mywindow = window.open('', 'PRINT', 'height=400,width=600');

    mywindow.document.write('<html><head><title>' + document.title + '</title>');
    mywindow.document.write('</head><body >');
    mywindow.document.write('<h1>' + document.title + '</h1>');
    mywindow.document.write(document.getElementById(elem).innerHTML);
    mywindow.document.write('</body></html>');

    mywindow.document.close(); // necessary for IE >= 10
    mywindow.focus(); // necessary for IE >= 10*/

    mywindow.print();
    mywindow.close();

    return true;
}

function CatchAjaxResponseWithNotif(result) {
    var title = LanguageDic['LB_NOTIFICATION'];
    if (result && result.title) {
        title = result.title;
    }

    if (result.success && result.message) {
        $.showSuccessMessage(LanguageDic['LB_NOTIFICATION'], result.message, result.clientcallback);
    } else {
        $.showErrorMessage(LanguageDic['LB_NOTIFICATION'], result.message, result.clientcallback);
    }
}

function RedirectTo(url) {
    if (url.length > 0) {
        window.location.href = url;
    }
}

function showItemLoading(element) {
    if (element) {
        $(element).find('.common-loading-item').remove();
        var loadingHtml = $('.common-loading-item').wrap('<div/>').parent().html();
        $(element).prepend(loadingHtml);
        $(element).find('.common-loading-item').show();
    } else {
        $('.common-loading-item').show();
    }
}
function hideItemLoading(element) {
    if (element) {
        $(element).find('.common-loading-item').fadeOut(1000);
    } else {
        $('.common-loading-item').fadeOut(1000);
    }
}

var MySiteGlobal = {
    init: function () {
        this.versionVerify();

        GetResources();
    },
    bindEvents: function () {
        $("#btnViewSchedule").click(function () {
            if (!$("#scheduleModalContent").hasClass("loaded")) {
                var params = $.extend({}, doAjax_params_default);
                params['url'] = "/Schedule/ShowCalendar";
                params['requestType'] = 'GET';
                params['data'] = {};
                params['dataType'] = "html";

                params['successCallbackFunction'] = function (result) {
                    $('#scheduleModalContent').html(result).addClass("loaded");
                    $('#scheduleModal').modal("show");
                };

                doAjax(params);
            } else {
                $('#scheduleModal').modal("show");
            }
        });
    },
    bindAlertIcons: function () {
        setInterval(function () {
            $(".shake-icon .m-nav__link-icon").addClass("m-animate-shake"), $(".shake-icon .m-nav__link-badge").addClass("m-animate-blink");
        }, 3e3);

        setInterval(function () {
            $(".shake-icon .m-nav__link-icon").removeClass("m-animate-shake"), $(".shake-icon .m-nav__link-badge").removeClass("m-animate-blink");
        }, 6e3);
    },
    versionVerify: function () {
        if (localStorage.getItem("CurrentVersion") !== CurrentVersion) {
            localStorage.setItem("CurrentVersion", CurrentVersion);

            //Clear data
            localStorage.removeItem("LanguageDic_" + CurrentLang);
        }
    }
};

function GetResources() {
    var langKey = "LanguageDic_" + CurrentLang;
    var langRes = localStorage.getItem("LanguageDic_" + CurrentLang);

    if (langRes === null || langRes.length === 0) {
        var params = $.extend({}, doAjax_params_default);
        params['url'] = '/Master/GetResources';
        params['requestType'] = 'POST';
        params['showloading'] = false;
        params['data'] = {};
        params['dataType'] = "json";
        params['successCallbackFunction'] = function (result) {
            LanguageDic = JSON.parse(result);

            localStorage.setItem(langKey, result);
        };
        doAjax(params);
    } else {
        LanguageDic = JSON.parse(langRes);
    }
}

function PreviewImageFromBrowseDialog(input) {
    var previewContainer = $(input).data("preview");
    var currentPreviewContainer = $("#" + previewContainer);
    var previewImg = currentPreviewContainer.children(".thumbImg").first();
    var fileUploadIcon = currentPreviewContainer.children(".file-upload-icon").first();
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
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

$("body").on("click", ".customFieldRemove", function () {
    var ctn = $(this).closest(".customFieldGroup");
    ctn.slideUp();
    setTimeout(function () {
        ctn.remove();

        ReflectCustomField();
    }, 300);
});

function ReflectCustomField() {
    var count = 0;
    $(".customFieldGroup").each(function () {
        var inputCtrl = $(this).find(".customFieldInput");
        inputCtrl.each(function () {
            var propName = $(this).data("prop");
            $(this).attr("name", "metadata[" + count + "][" + propName + "]");
        });

        count++;
    });
}

$(document).ready(function () {
    //DynamicRealTime();
    MySiteGlobal.init();
    MySiteGlobal.bindEvents();
    MySiteGlobal.bindAlertIcons();

    $(".page-loading").addClass("hidden");

    $(".RemovalScripts").remove();
    setTimeout(function () {
        if ($(".input-off-enter").length > 0) {
            var target = $("input[data-controller=JobSeeker]");
            RenderFormSearch(target, false);
            $(".input-off-enter").removeClass("input-off-enter");
        }
    }, 200);
});

$(document).on('keyup', '.number-format', function (e) {
    // Get the value.
    var input = $(e.target).val();
    var rs = FormatNumber(input);
    $(e.target).val(rs);


});

$(document).on("change", "input[name=type]", function (e) {
    e.stopPropagation();
    var target = $(this);
    RenderFormSearch(target);
});
function RenderFormSearch(target) {
    var myController = target.attr("data-controller");
    var mydiv = target.attr("data-div");
    var thisForm = $(".search-advance").closest("form");
    //SearchOptionClose();
    if ($("." + mydiv).html().length == 0) {
        $(".search-advance").html("");
        var params = $.extend({}, doAjax_params_default);
        params['url'] = "/" + myController + "/SearchRender";
        params['requestType'] = "GET";
        params['data'] = null;
        params['dataType'] = "html";
        params['showloading'] = false;

        params['successCallbackFunction'] = function (data) {
            $(".search-advance").html(data);
            $("." + mydiv).html(data);
            thisForm.attr("action", "/" + myController + "/Index");
            if ($(".search-advance").hasClass("hidden-search")) {
                $(".search-advance").removeClass("hidden-search");
            }
            $(".input-search").removeClass("input-off-enter");
            $(".input-search").attr("placeholder", $("." + mydiv).find(".title-search").val());
            UpdateSelect(thisForm);
        };

        doAjax(params);
    }
    else {
        $(".search-advance").html($("." + mydiv).html());
        $(".search-advance").closest("form").attr("action", "/" + myController + "/Index");
        if ($(".search-advance").hasClass("hidden-search")) {
            $(".search-advance").removeClass("hidden-search");
        }
        $(".input-search").removeClass("input-off-enter");
        $(".input-search").attr("placeholder", $("." + mydiv).find(".title-search").val());
        //SearchOptionOpen();
        UpdateSelect(thisForm);
        ResetFormSearch(thisForm);
    }

}

function UpdateSelect(thisForm) {
    if (!thisForm.find(".selectpicker-search:first").hasClass("m-bootstrap-select")) {
        thisForm.find(".selectpicker-search").selectpicker();
        thisForm.find(".m-select2-search").selectpicker({
            width: '100%'
        });
    }
    var targetSelect = thisForm.find("select.select_sub_field");
    var lengthOption = targetSelect.find('option').length;    
    var btnDropdown = targetSelect.next();
    checkSubList(btnDropdown, lengthOption);
}
function checkSubList(btnDropdown, lengthOption) {    
    if (lengthOption > 1) {
        btnDropdown.removeClass("disabled");
        btnDropdown.attr("data-toggle", "dropdown");
        btnDropdown.attr("onclick", "");
        btnDropdown.attr("title", LanguageDic['LB_SELECT_SUB_FIELD']);
    }
    else {        
        setTimeout(function () {
            btnDropdown.addClass("disabled");
            btnDropdown.attr("data-toggle", "");
            btnDropdown.attr("onclick", "return emptyElement(); ");
            btnDropdown.attr("title", LanguageDic['ERROR_SELECT_SUB_FIELD_EMPTY']);
        }, 200)
    }
}
function emptyElement() {
    alert(LanguageDic['ERROR_SELECT_SUB_FIELD_EMPTY']);
    $(".select-employment-type").focus();
    return false;
}
function changeLocalTimezone(format = "") {
    $(".local-time").each(function () {
        var strTime = $(this).data("time");
        strTime = strTime + " UTC";
        var date = new Date(strTime);
        $(this).html(date.toLocaleString());
        $(this).removeClass("local-time");
    });
}

function FormatNumber(num) {
    var listNum = num.split('.');
    var number = listNum[0].replace(/\,/g, '');
    if (!$.isNumeric(number)) {
        number = number.substring(0, number.length - 1);
    }
    var result = "0";
    if (parseInt(number) > 0) {
        result = parseInt(number) + '';
    }
    var i = 0;
    var count = 0;
    while (i < number.length) {
        if (i % 3 == 0 && i > 0) {
            var index = number.length - i;
            result = result.insertAt(index, ",");
            count++;
        }
        i++;
    }
    if (listNum.length > 1) {
        return result + "." + listNum[1];
    }
    return result;

}

String.prototype.insertAt = function (index, string) {
    return this.substr(0, index) + string + this.substr(index);
}

$("body").on("click", ".legend-ctrl", function () {
    var fs = $(this).closest("fieldset");
    var ep = fs.find(".fieldset-expand");
    if (ep.length > 0) {
        ep.remove();
    } else {
        fs.append('<div class="fieldset-expand row"><div class="col-md-12"><a href="javascript:;" class="text-info"><b>...</b></a></div></div>');
    }

    fs.find(".fieldset-content").slideToggle();
    $(this).find("i").toggleClass("fa-angle-down");
});

$("body").on("click", ".fieldset-expand", function () {
    var fs = $(this).closest("fieldset");
    $(this).remove();
    fs.find(".fieldset-content").slideToggle();
    fs.find(".legend-ctrl").find("i").toggleClass("fa-angle-down");
});

function ClearSomeLocalStorage(startsWith) {
    var myLength = startsWith.length;

    Object.keys(localStorage)
        .forEach(function (key) {
            if (key.substring(0, myLength) == startsWith) {
                localStorage.removeItem(key);
            }
        });
}