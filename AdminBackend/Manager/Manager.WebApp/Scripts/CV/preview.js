var MemberGlobal = {
    init: function () {
        this.eduHistories();
        this.workHistories();
        this.certificates();
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
    mailBoxHistories: function () {
        //showItemLoading("#MailBoxHistories");
        //setTimeout(function () {
        //    var url_tk = $("#CvMailTk").val();

        //    $('#MailBoxHistories').load(url_tk, function (response, status, xhr) {
        //        hideItemLoading("#MailBoxHistories");
        //    });
        //}, 300);
        JobSeekerMailBox(1);
    }
};

$("body").on("click", ".MailBoxTab", function () {
    if (!$(this).hasClass("loaded")) {
        MemberGlobal.mailBoxHistories();
        $(this).addClass("loaded");
    }
});

function JobSeekerMailBox(pageIndex) {
    $("#MailBoxHistories").html("");
    showItemLoading("#MailBoxHistories"); 

    $("#CurrentMailBoxPageIdx").val(pageIndex);

    var frmData = $("#MailBoxFrm").serialize();
    
    var params = $.extend({}, doAjax_params_default);
    params['url'] = "/MyEmail/JobSeekerMailBox";
    params['requestType'] = "POST";
    params['data'] = frmData;
    params['dataType'] = "html";
    params['showloading'] = false;

    params['successCallbackFunction'] = function (data) {
        hideItemLoading("#MailBoxHistories");
        setTimeout(function () {
            $("#MailBoxHistories").html(data);
        }, 300);
        
    };

    doAjax(params);
}

$(document).on("click", "#btn-view", function () {
    $('#myCvDetailModal').modal('show');
});

$('#btn-print').click(function () {
    var divToPrint = document.getElementById('printContent');
    var newWin = window.open('', 'Print-Window');
    newWin.document.open();
    newWin.document.write('<html><body onload="window.print()">' + divToPrint.innerHTML + '</body></html>');
    newWin.document.close();
    setTimeout(function () { newWin.close(); }, 10);
});

$(function () {
    MemberGlobal.init();
});