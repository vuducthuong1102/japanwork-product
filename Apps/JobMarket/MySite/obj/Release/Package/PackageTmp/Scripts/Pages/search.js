var _window = $(window);
var _hasNoData = false;

$(function () {
    $.getScript("/Scripts/Pages/job-search-events.js").done(function (script, textStatus) {
        SearchJobBox.init();

        setTimeout(function () {
            $("#SearchJobBox").show();
        }, 500);

        $.getScript("/Scripts/Widgets/modal-filter-city.js").done(function (script, textStatus) { });
        $.getScript("/Scripts/Widgets/modal-filter-station.js").done(function (script, textStatus) { });
    });

    GetSearchResults();
    //$(window).on('scroll', throttle(PagingByScroll, 500));
});

var treeIndustry = $('#treeIndustry').tree({
    primaryKey: 'id',
    uiLibrary: 'bootstrap',
    dataSource: '/Master/GetListIndustriesAsTreeData?CheckedItems=' + $("#sub_industry_ids").val(),
    checkboxes: true
    //dataBound: function (e) {
    //    treeIndustry.expandAll();
    //}
});

var treeField = $('#treeField').tree({
    primaryKey: 'id',
    uiLibrary: 'bootstrap',
    dataSource: '/Master/GetListFieldsAsTreeData?CheckedItems=' + $("#sub_field_ids").val(),
    checkboxes: true,
    //dataBound: function (e) {
    //    treeField.expandAll();
    //}
});

$("body").on("click", "#treeIndustry .list-group-item", function (ev) {
    var id = $(this).data("id");
    var node = treeIndustry.getNodeById(id);
    treeIndustry.expand(node);
});

$("body").on("click", "#treeField .list-group-item", function (ev) {
    var id = $(this).data("id");
    var node = treeField.getNodeById(id);
    treeField.expand(node);
});

function RemoveOverLay(container) {
    setTimeout(function () {
        $(".list-overlay").remove();
    }, 500);
}

function afterGetSearchResults(data) {
    var container = $("#JobsList");
    
    $('html, body').animate({
        scrollTop: $("#JobsListContainer").offset().top - 70
    }, 500);

    container.html(data.html);
    $("#JobSearchCounter").html(numberWithCommas(data.total));  

    $(".job-searching-state").addClass("hidden");
    $(".job-searching-state-done").removeClass("hidden");

    $("img.lazy").myLazyLoad();
}

$("body").on("click", "#treeIndustry input[type='checkbox']", function () {
    var arrCheckedChildrens = [];
    $(".sub-industry").each(function () {
        var subId = $(this).data("id");
        var firstChild = $(this).closest("div[data-role='wrapper']").find("input[type='checkbox']").first();
        if (firstChild.is(":checked")) {
            arrCheckedChildrens.push(subId);
        }
    });

    $("#sub_industry_ids").val(arrCheckedChildrens);

    FilterChangeDetected();
});

$("body").on("click", "#treeField input[type='checkbox']", function () {
    var arrCheckedChildrens = [];
    $(".sub-field").each(function () {
        var subId = $(this).data("id");
        var firstChild = $(this).closest("div[data-role='wrapper']").find("input[type='checkbox']").first();
        if (firstChild.is(":checked")) {
            arrCheckedChildrens.push(subId);
        }
    });

    $("#sub_field_ids").val(arrCheckedChildrens);

    FilterChangeDetected();
});

function GetSearchResults() {
    var data = $("#frmJobSearch").serialize();

    history.pushState(null, "", "/Job/Search?" + data);
    data += '&__RequestVerificationToken=' + $('input[name = "__RequestVerificationToken"]').val();

    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Job/ShowAllResults';
    params['requestType'] = 'POST';
    params['data'] = data;
    params['dataType'] = "json";

    params['successCallbackFunction'] = afterGetSearchResults;
    doAjax(params);
}

$(".searching-element").change(function () {
    var currentControl = $(this).data("control");
    $("#" + currentControl).val($(this).val());

    FilterChangeDetected();
});

function JobSearchNext(pageNum) {
    $("#CurrentPage").val(pageNum);

    GetSearchResults();
}

$("body").on("click", ".btn-clean-filter", function () {
    $(this).parent().closest(".tags-bar").find(".tag-filter-item").remove();
    $(this).closest(".action-tags").addClass("hidden");
});

$('input[type=radio][name=filter-salary-month]').change(function () {
    ChangeFilterSalary();

    FilterChangeDetected();
});

$('input[type=radio][name=filter-salary-hour]').change(function () {
    ChangeFilterSalary();

    FilterChangeDetected();
});

$("body").on("click", ".btn-apply-filter-search", function () {
    GetSearchResults();
    var thisBox = $(".filterFixBottom");
    //thisBox.removeClass("show");
    thisBox.fadeOut();
});

function FilterChangeDetected() {
    var thisBox = $(".filterFixBottom");   

    $(".close-filter-icon").removeClass("fa-chevron-circle-up");
    $(".close-filter-icon").addClass("fa-chevron-circle-down");

    $('.filterFixBottom').removeClass('is_closed');
    $('.filterFixBottom').animate({ 'bottom': '0px' });

    var sb = 0;
    if ($('.filterApplyTempBox').length > 0) {
        sb = $('.filterApplyTempBox').offset().top + $('.filterApplyTempBox').height();
    }
    var wb = $(window).scrollTop() + $(window).height();
    if (wb > sb) {
        $('.filterFixBottom').fadeOut();
    } else {
        if (!$('.filterFixBottom').hasClass('show')) {
            $('.filterFixBottom').addClass("show");
        } 
    }

    AutoDisplayFilterSearchBottom();
}

function setFilterSearchBottom() {
    var dw = $(window).width();
    if (dw > 1200) {
        l = Math.ceil((dw - 1200) / 2);
    } else {
        l = 0;
        if ($(window).scrollLeft() > 0) {
            l = -($(window).scrollLeft());
        }
    }

    if (l > 0)
        l = l - 10;
    else
        l = 40;
    $('.filterFixBottom').css('left', l + 'px');
    $('.filterFixBottom').css('width', '' + $("#FilterLeftCol").width());
}

function AutoDisplayFilterSearchBottom() {
    if ($('.filterApplyTempBox').length > 0) {
        var sb = $('.filterApplyTempBox').offset().top + $('.filterApplyTempBox').height();
    }
    var wb = $(window).scrollTop() + $(window).height();
    if (wb > sb) {
        $('.filterFixBottom').fadeOut();
    } else {
        if ($('.filterFixBottom').hasClass('show')) {
            $('.filterFixBottom').slideDown();
        }
    }
}

$(window).scroll(function () {
    AutoDisplayFilterSearchBottom();
});

$(window).scroll(function () {
    AutoDisplayFilterSearchBottom();
});

$(window).scroll(function () {
    setFilterSearchBottom();
});

$(window).resize(function () {
    setFilterSearchBottom();
});

function toogleSlideFilterBox() {
    if ($(".close-filter-icon").hasClass("fa-chevron-circle-up")) {
        $(".close-filter-icon").removeClass("fa-chevron-circle-up");
        $(".close-filter-icon").addClass("fa-chevron-circle-down");
    } else {
        $(".close-filter-icon").addClass("fa-chevron-circle-up");
        $(".close-filter-icon").removeClass("fa-chevron-circle-down");
    }

    if ($('.filterFixBottom').hasClass('is_closed')) {
        $('.filterFixBottom').removeClass('is_closed');
        $('.filterFixBottom').animate({ 'bottom': '0px' });
    } else {
        $('.filterFixBottom').addClass('is_closed');
        $('.filterFixBottom').animate({ 'bottom': '-55px' });
    }
}

$(function () {
    setFilterSearchBottom();
    $('.filterFixBottom .btn-close-filter').click(function () {
        toogleSlideFilterBox();
    });
});
