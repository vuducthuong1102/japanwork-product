$("#JobSearchAdvanceBtn").click(function () {
    $("#JobSearchAdvance").slideToggle();
    $("#JobSearchAdvanceArrow").toggleClass("la-arrow-up");

    $('#Stations').select2({
        placeholder: LanguageDic['LB_INPUT_STATION_NAME'],
        width: '100%',
        language: {
            searching: function () {
                return LanguageDic['LB_SEARCHING'];
            }
        },
        ajax: {
            url: '/Master/GetSuggestionStations',
            type: 'POST',
            data: function (params) {
                var query = {
                    query: params.term,
                    page: params.page || 1,
                    __RequestVerificationToken: $('input[name = "__RequestVerificationToken"]').val()
                };

                // Query parameters will be ?search=[term]&page=[page]
                return query;
            },
            processResults: function (data, page) {
                return {
                    // Select2 requires an id, so we need to map the results and add an ID
                    // You could instead include an id in the tsv you add to soulheart ;)
                    results: data.data.map(function (item) {
                        return {
                            id: item.id,
                            text: item.furigana
                        };
                    }),
                    //pagination: {
                    //    // If there are 10 matches, there's at least another page
                    //    more: data.matches.length === 10
                    //}
                };
            },
            cache: true
        }        
    });
});


$("body").on("click", ".btn-clean-filter", function () {
    $(this).parent().closest(".tags-bar").find(".tag-filter-item").remove();
    $(this).closest(".action-tags").addClass("hidden");
});

$("body").on("click", ".remove-city-tag", function () {
    $(this).parent().fadeOut();
});

function St_GetListRegionsForFiltering() {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Master/St_FilterGetRegions';
    params['requestType'] = 'POST';
    params['data'] = { st_prefecture_ids: $("#st_prefecture_ids").val(), st_region_ids: $("#st_region_ids").val() };
    params['showloading'] = false;
    params['dataType'] = "html";

    params['successCallbackFunction'] = function (result) {
        $("#st_filterRegionsList").html(result);
    };
    doAjax(params);
}

function Ct_GetListRegionsForFiltering() {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Master/Ct_FilterGetRegions';
    params['requestType'] = 'POST';
    params['data'] = { ct_prefecture_ids: $("#ct_prefecture_ids").val(), ct_region_ids: $("#ct_region_ids").val() };
    params['showloading'] = false;
    params['dataType'] = "html";

    params['successCallbackFunction'] = function (result) {
        $("#ct_filterRegionsList").html(result);
    };
    doAjax(params);
}

$("#ddlEmploymentType").change(function () {
    var showTrains = $(this).find(':selected').data('showtrains');   
    if (showTrains === "True") {
        $("#btnFilterCity").addClass("hidden");
        $("#btnFilterStation").removeClass("hidden");
    }
    else {
        $("#btnFilterCity").removeClass("hidden");
        $("#btnFilterStation").addClass("hidden");
    }

    ChangeFilterSalary();
});

function ChangeFilterSalary() {
    var calculateBy = parseInt($("#ddlEmploymentType").find(':selected').data('calculate'));
    var currentMin = 0;
    var currentMax = 0;
    var salaryId = 0;

    if (calculateBy === 0) {
        //By month
        $("#ddlSalaryByHour_chosen").addClass("hidden");
        $("#ddlSalaryByMonth_chosen").removeClass("hidden");

        currentMin = parseInt($("#ddlSalaryByMonth").find(':selected').data('min'));
        currentMax = parseInt($("#ddlSalaryByMonth").find(':selected').data('max'));
        salaryId = parseInt($("#ddlSalaryByMonth").find(':selected').data('id'));
    }
    else {
        //By hour
        $("#ddlSalaryByHour_chosen").removeClass("hidden");
        $("#ddlSalaryByMonth_chosen").addClass("hidden");

        currentMin = parseInt($("#ddlSalaryByHour").find(':selected').data('min'));
        currentMax = parseInt($("#ddlSalaryByHour").find(':selected').data('max'));
        salaryId = parseInt($("#ddlSalaryByHour").find(':selected').data('id'));
    }

    $("#salary_min").val(currentMin);
    $("#salary_max").val(currentMax);
    $("#calculate_by").val(calculateBy);
    $("#salary_id").val(salaryId);
}

$("#ddlSalaryByMonth").change(function () {
    ChangeFilterSalary();
});

$("#ddlSalaryByHour").change(function () {
    ChangeFilterSalary();
});

var SearchJobBox = {
    init: function () {
        $("#ddlEmploymentType").change();

        setTimeout(function () {
            St_GetListRegionsForFiltering();
            Ct_GetListRegionsForFiltering();
        }, 500);
    }
}