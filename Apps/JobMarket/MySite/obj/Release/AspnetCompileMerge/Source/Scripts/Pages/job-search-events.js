$("#JobSearchAdvanceBtn").click(function () {
    $("#JobSearchAdvance").slideToggle();
    $("#JobSearchAdvanceArrow").toggleClass("la-arrow-up");

    //$('#Stations').select2({
    //    placeholder: LanguageDic['LB_INPUT_STATION_NAME'],
    //    width: '100%',
    //    language: {
    //        searching: function () {
    //            return LanguageDic['LB_SEARCHING'];
    //        }
    //    },
    //    ajax: {
    //        url: '/Master/GetSuggestionStations',
    //        type: 'POST',
    //        data: function (params) {
    //            var query = {
    //                query: params.term,
    //                page: params.page || 1,
    //                __RequestVerificationToken: $('input[name = "__RequestVerificationToken"]').val()
    //            };

    //            // Query parameters will be ?search=[term]&page=[page]
    //            return query;
    //        },
    //        processResults: function (data, page) {
    //            return {
    //                // Select2 requires an id, so we need to map the results and add an ID
    //                // You could instead include an id in the tsv you add to soulheart ;)
    //                results: data.data.map(function (item) {
    //                    return {
    //                        id: item.id,
    //                        text: item.furigana
    //                    };
    //                }),
    //                //pagination: {
    //                //    // If there are 10 matches, there's at least another page
    //                //    more: data.matches.length === 10
    //                //}
    //            };
    //        },
    //        cache: true
    //    }        
    //});
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

    var currentType = parseInt($(this).val());
    var selectedField = parseInt($("#ddlField").val());
    var flag = false;
    $("#ddlField").find("option").each(function () {
        var emp = parseInt($(this).data("employment"));
        if (emp === currentType) {
            flag = true;            
        }
    });

    if (flag) {
        $("#ddlField").find("option").each(function () {
            $(this).addClass("hidden");
            var emp = $(this).data("employment");
            if (emp > 0) {
                $(this).removeClass("hidden");                
            }
        });
    } else {
        $("#ddlField").find("option").each(function () {
            $(this).removeClass("hidden");
            var emp = $(this).data("employment");
            if (emp > 0) {
                $(this).addClass("hidden");
            }
        });
    }

    var matchedVal = false;
    $("#ddlField").find("option").each(function () {
        if (!$(this).hasClass("hidden")) {
            var val = parseInt($(this).val());
            if (val === selectedField) {
                matchedVal = true;
            }
        }
    });

    if (!matchedVal)
        selectedField = 0;

    $('#ddlField').val("" + selectedField);
    $('#ddlField').chosen().trigger("chosen:updated");
    $("#ddlField").change();

    ChangeFilterSalary();
});

function ChangeFilterSalary() {
    var calculateBy = parseInt($("#ddlEmploymentType").find(':selected').data('calculate'));
    var currentMin = 0;
    var currentMax = 0;
    var salaryId = 0;

    if (calculateBy === 0) {
        $("#ddlSalaryByHour").addClass("hidden");
        $("#ddlSalaryByMonth").removeClass("hidden");
        var currentItemMonth = $(".filter-salary-month:checked");
        if (currentItemMonth) {
            currentMin = parseInt(currentItemMonth.data('min'));
            currentMax = parseInt(currentItemMonth.data('max'));
            salaryId = parseInt(currentItemMonth.data('id'));
        }
    }
    else {       
        $("#ddlSalaryByHour").removeClass("hidden");
        $("#ddlSalaryByMonth").addClass("hidden");
        var currentItemHour = $(".filter-salary-hour:checked");
        if (currentItemHour) {
            currentMin = parseInt(currentItemHour.data('min'));
            currentMax = parseInt(currentItemHour.data('max'));
            salaryId = parseInt(currentItemHour.data('id'));
        }      
    }

    if (isNaN(currentMin))
        currentMin = 0;

    if (isNaN(currentMax))
        currentMax = 0;

    if (isNaN(salaryId))
        salaryId = 0;

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

$("#ddlField").change(function () {
    //var subFields = $(this).find(':selected').data('subfields');
    //var selectedFieldId = parseInt($("#ddlField :selected").val());
    var selectedSubFields = $("#sub_field_ids").val();
    var subFields = $("#ddlField :selected").data('subfields');    
    if (subFields === "") {
        $("#sub_field_ids").val("");

        return false;
    }
    var arrSelected = ConvertStringToListArray(selectedSubFields, ",");
    var arrSubFieldChilds = ConvertStringToListArray(subFields, ",");

    if (arrSubFieldChilds.length > 0) {
        if (arrSelected > 0) {
            arrSelected.concat(arrSubFieldChilds);
            $("#sub_field_ids").val(arrSelected);
        } else {
            $("#sub_field_ids").val(subFields);
        }
    } else {
        if (arrSelected > 0) {
            $("#sub_field_ids").val(arrSelected);
        }
    }
});

var SearchJobBox = {
    init: function () {
        $("#ddlEmploymentType").change();
       $("#ddlField").change();

        setTimeout(function () {
            St_GetListRegionsForFiltering();
            Ct_GetListRegionsForFiltering();
        }, 500);

        //$(".select2").chosen({ width: "100%" });
    }
}