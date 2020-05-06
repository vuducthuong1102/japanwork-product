$.fn.exists = function () {
    return this.length !== 0;
}

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

    //make sure that url ends with '/'
    if (!url.endsWith("/")) {
        url = url + "/";
    }

    if (context) {
        var me = $(context);

        if (me.data('requestRunning')) {
            return false;
        }

        me.data('requestRunning', true);
    }

    //data.push({ __RequestVerificationToken: $('input[name = "__RequestVerificationToken"]').val() });
    data.__RequestVerificationToken = $('input[name = "__RequestVerificationToken"]').val();

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

$("body").on("click", ".reset-btn", function () {
    var thisForm = $(this).closest("form");
    thisForm.find(".m-input").val("");
    thisForm.find(".selectpicker").val(0);
    thisForm.find(".selectpicker").each(function () {
        $(this).val($(this).find("option:first").val());
        if ($(this).hasClass("select-trigger")) {
            $(this).find('option').not(':first').remove();
        }

        $(this).selectpicker("refresh");
    });

    thisForm.find(".m-select2").each(function () {
        $(this).val('').trigger('change');
    });

    thisForm.find('input:checkbox').each(function () {
        $(this).removeAttr('checked');
    });
});

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

$(document).ready(function () {
    DynamicRealTime();

    //ReplaceErrorImage();
});

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

function changeLocalTimezone(format = "") {
    $(".local-time").each(function () {
        var strTime = $(this).data("time");
        var date = new Date(strTime);
        $(this).html(date.toLocaleString());
    });
}    