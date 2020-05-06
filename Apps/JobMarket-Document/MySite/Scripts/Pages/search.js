var _window = $(window);
var _hasNoData = false;

$(function () {
    $.getScript("/Scripts/Pages/job-search-events.js").done(function (script, textStatus) {
        SearchJobBox.init();

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
    //if (!$(this).hasClass("openned")) {
    //    treeIndustry.expand(node);
    //    $(this).addClass("openned");
    //}
    //else {
    //    treeIndustry.expand(node);
    //    $(this).removeClass("openned");
    //}
});

$("body").on("click", "#treeField .list-group-item", function (ev) {
    var id = $(this).data("id");
    var node = treeField.getNodeById(id);
    treeField.expand(node);
    //if (!$(this).hasClass("openned")) {
    //    treeIndustry.expand(node);
    //    $(this).addClass("openned");
    //}
    //else {
    //    treeIndustry.expand(node);
    //    $(this).removeClass("openned");
    //}
});

//function PagingByScroll() {
//    if ($(window).scrollTop() + window.innerHeight >= document.body.scrollHeight) {
//        _currentPage++;

//        if (!_hasNoData)
//            GetSearchResults(_currentPage);
//    }
//}

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

    throttle(GetSearchResults(), 1000);
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

    throttle(GetSearchResults(), 1000);
});

function GetSearchResults() {
    var data = $("#frmJobSearch").serialize();
    data += '&__RequestVerificationToken=' + $('input[name = "__RequestVerificationToken"]').val();

    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Job/ShowAllResults';
    params['requestType'] = 'POST';
    params['data'] = data;
    params['dataType'] = "json";

    params['successCallbackFunction'] = afterGetSearchResults;
    doAjax(params);
}

//$("#frmJobSearch").submit(function () {
//    $("#CurrentPage").val(1);
//    GetSearchResults();
//    return false;
//});

$(".searching-element").change(function () {
    var currentControl = $(this).data("control");
    $("#" + currentControl).val($(this).val());
    GetSearchResults();
    return false;
});

function JobSearchNext(pageNum) {
    $("#CurrentPage").val(pageNum);

    GetSearchResults();
}

$(".city-chosen").typeahead({
    highlight: true,
    minLength: 1,
    hint: false,
    delay: 2000
},
    {
        source: function (query, process) {
            var params = $.extend({}, doAjax_params_default);
            params['url'] = '/Master/GetSuggestionPlaces';
            params['requestType'] = 'POST';
            params['data'] = { query: query };
            params['dataType'] = "json";
            params['async'] = false;
            params['showloading'] = false;

            params['successCallbackFunction'] = function (result) {
                process(result.data);
            };
            doAjax(params);
        },
        updater: function (item) {
            return '';
        },
        display: function (data) {
            return data.furigana + " (" + data.name + ")";
        },
        limit: 10,
        templates: {
            suggestion: function (data) {
                var placeIcon = "la la-map-pin";
                if (data.type == "region") {
                    placeIcon = "la la-map";
                } else if (data.type == "prefecture") {
                    placeIcon = "la la-map-marker";
                }

                return "<div class='ac-item' data-id='" + data.id + "'><i class='" + placeIcon + "'></i> " + data.furigana + ' (' + data.name + ')' + "</div>";
            }
        }
    });

$(".city-chosen").on('typeahead:selected', function (evt, item) {
    var $myTextarea = $("#filter_cities");
    $myTextarea.append('<span class="tag-filter-item">' + item.furigana + '<i class="remove-city-tag">x</i></span>', ' ');
    $("#filter_cities").find(".action-tags").removeClass("hidden");

    $(".city-chosen").typeahead('val', '');
});

$("body").on("click", ".btn-clean-filter", function () {
    $(this).parent().closest(".tags-bar").find(".tag-filter-item").remove();
    $(this).closest(".action-tags").addClass("hidden");
});