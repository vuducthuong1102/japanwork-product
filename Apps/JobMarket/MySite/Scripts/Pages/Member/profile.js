$(function () {
    $(".readonly").each(function () {
        $(this).prop("disabled", true);
        $(this).css("background-color", "#eee");
    });

    $('.select2').select2({
        width: '100%'
    });

    $(".file-upload-storage").change(function () {
        PreviewImageFromBrowseDialog(this);
    });

    setTimeout(function () {
        $("#Address_Country").val($("#address_country_id").val());
        $("#Address_Country").change();        
    }, 500);

    setTimeout(function () {
        $("#Address_Contact_Country").val($("#address_contact_country_id").val());
        $("#Address_Contact_Country").change();
    }, 500);

    //setTimeout(function () {
    //    $("#Address_Region").val($("#address_region_id").val());
    //    $("#Address_Region").change();
    //}, 500);

    //setTimeout(function () {
    //    $("#Address_Contact_Region").val($("#address_contact_region_id").val());
    //    $("#Address_Contact_Region").change();
    //}, 1000);

    $("#birthday").mask("99-99-9999");
    $(".datepicker").datepicker({
        format: "dd-mm-yyyy"       
    });

    //$('.Address_TrainLine').select2({
    //    placeholder: LanguageDic['LB_INPUT_TRAIN_LINE_NAME'],
    //    width: '100%',
    //    language: {
    //        searching: function () {
    //            return LanguageDic['LB_SEARCHING'];
    //        }
    //    },
    //    ajax: {
    //        url: '/Master/GetSuggestionTrainLines',
    //        type: 'POST',
    //        data: function (params) {
    //            var query = {
    //                query: params.term,
    //                page: params.page || 1,
    //                place_id: $("#" + $(this).data("control")).val(),
    //                __RequestVerificationToken: $('input[name = "__RequestVerificationToken"]').val()
    //            };

    //            // Query parameters will be ?search=[term]&page=[page]
    //            return query;
    //        },
    //        processResults: function (data, page) {
    //            return {
    //                results: data.data.map(function (item) {
    //                    return {
    //                        id: item.id,
    //                        text: item.train_line
    //                    };
    //                })
    //                //pagination: {
    //                //    // If there are 10 matches, there's at least another page
    //                //    more: data.matches.length === 10
    //                //}
    //            };
    //        },
    //        cache: true
    //    }
    //});

    //$('.Address_Station').select2({
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
    //                place_id: $("#" + $(this).data("control")).val(),
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
    //                        text: item.furigana + " (" + item.station + ")"
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

$("#same_address").click(function () {
    if ($(this).is(":checked")) {
        var country_id = $("#Address_Country").val();

        $("#Address_Contact_Country").html($("#Address_Country").html());
        $("#Address_Contact_Region").html($("#Address_Region").html());
        $("#Address_Contact_Prefecture").html($("#Address_Prefecture").html());
        $("#Address_Contact_City").html($("#Address_City").html());

        $("#Address_Contact_Postal_Code").val($("#Address_Postal_Code").val());
        $("#Address_Contact_Country").val(country_id);

        $("#Address_Contact_Region").val($("#Address_Region").val());
        $("#Address_Contact_Prefecture").val($("#Address_Prefecture").val());
        $("#Address_Contact_City").val($("#Address_City").val());

        $("#Address_Contact_Detail").val($("#Address_Detail").val());
        $("#Address_Contact_Furigana").val($("#Address_Furigana").val());
    }
});

$("#Address_Country").change(function () {
    var id = parseInt($(this).val());
    if (id !== 81) {
        $("#Address_City_Dropdown").addClass("hidden");
        $("#Address_TrainLine_Dropdown").addClass("hidden");
        $("#Address_Station_Dropdown").addClass("hidden");
    } else {
        $("#Address_City_Dropdown").removeClass("hidden");
        $("#Address_TrainLine_Dropdown").removeClass("hidden");
        $("#Address_Station_Dropdown").removeClass("hidden");
    }
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Master/GetRegionsByCountry';
    params['requestType'] = 'POST';
    params['data'] = { id: id };
    params['showloading'] = false;
    params['dataType'] = "html";

    params['successCallbackFunction'] = function (result) {
        $("#Address_Region").html(result);
        $("#Address_Prefecture").find('option').not(':first').remove();
        $("#Address_City").find('option').not(':first').remove();
        //$("#Address_TrainLine").find('option').remove();
        //$("#Address_Station").find('option').remove();

        $("#Address_Region").val($("#address_region_id").val());      
        $("#Address_Region").change();
    };
    doAjax(params);
});

$("#Address_Region").change(function () {
    var id = $(this).val();
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Master/GetPrefecturesByRegion';
    params['requestType'] = 'POST';
    params['data'] = { id: id };
    params['showloading'] = false;
    params['dataType'] = "html";

    params['successCallbackFunction'] = function (result) {
        $("#Address_Prefecture").html(result);
        $("#Address_City").find('option').not(':first').remove();
        //$("#Address_TrainLine").find('option').remove();
        //$("#Address_Station").find('option').remove();

        $("#Address_Prefecture").val($("#address_prefecture_id").val());
        $("#Address_Prefecture").change();
    };
    doAjax(params);
});

$("#Address_Prefecture").change(function () {
    var id = $(this).val();
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Master/GetCitiesByPrefecture';
    params['requestType'] = 'POST';
    params['data'] = { id: id };
    params['showloading'] = false;
    params['dataType'] = "html";

    params['successCallbackFunction'] = function (result) {
        $("#Address_City").html(result);
        //$("#Address_TrainLine").find('option').remove();
        //$("#Address_Station").find('option').remove();
        $("#Address_City").val($("#address_city_id").val());
        TrainLineChange();
    };

    doAjax(params);
});

//$("#Address_City").change(function () {
//    var id = $(this).val();
//    var params = $.extend({}, doAjax_params_default);
//    params['url'] = '/Master/GetSuggestionTrainLines';
//    params['requestType'] = 'POST';
//    params['data'] = { id: id };
//    params['showloading'] = false;
//    params['dataType'] = "html";

//    params['successCallbackFunction'] = function (result) {
//        $("#Address_TrainLine").html(result);
//        //$("#Address_TrainLine").find('option').remove();
//        //$("#Address_Station").find('option').remove();
//        $("#Address_TrainLine").val($("#address_train_line_id").val());
//        $("#Address_TrainLine").change();
//    };

//    doAjax(params);
//});

//$("#Address_TrainLine").change(function () {
//    var line_id = $(this).val();
//    var city_id = $("#Address_City").val();
//    var params = $.extend({}, doAjax_params_default);
//    params['url'] = '/Master/GetSuggestionStations';
//    params['requestType'] = 'POST';
//    params['data'] = { line_id: line_id, city_id: city_id };
//    params['showloading'] = false;
//    params['dataType'] = "html";

//    params['successCallbackFunction'] = function (result) {
//        $("#Address_Station").html(result);
//        //$("#Address_TrainLine").find('option').remove();
//        //$("#Address_Station").find('option').remove();
//        $("#Address_Station").val($("#address_station_id").val());
//        $("#Address_Station").change();
//    };

//    doAjax(params);
//});

$(".select-train-line").on('typeahead:selected', function (evt, item) {
    $(this).closest(".m-typeahead").find(".train-line-selected").val(item.id);
});
$(document).on("change", ".select-train-line", function () {
    if ($(this).val() == "") {
        $(this).closest(".m-typeahead").find(".train-line-selected").val(0);
    }
    TrainLineChange();
})

function TrainLineChange() {
    var id = parseInt($(".train-line-selected").val());
    var station = $("#Address_Station");

    if (id > 0) {
        var params = $.extend({}, doAjax_params_default);
        params['url'] = '/Master/GetSuggestionStationByTrainLines';
        params['requestType'] = 'POST';
        params['data'] = { line_id: id };
        params['showloading'] = false;
        params['dataType'] = "html";

        params['successCallbackFunction'] = function (result) {
            station.html(result);
            var selectedStations = station.data("current") + '';
            station.val(selectedStations);
            //if (counter === 1) {
            //    if (selectedStations != "0" && selectedStations.indexOf(",") >= 0) {
            //        $.each(selectedStations.split(","), function (i, e) {
            //            station.find("option[value='" + e + "']").prop("selected", true);
            //        });
            //    }
            //    else {
            //        if (station.find("option[value='" + selectedStations + "']").length > 0) {
            //            station.val(selectedStations);
            //        }
            //        else {
            //            station.val(0);
            //        }
            //    }
            //}
           // station.selectpicker('refresh');
        };

        doAjax(params);
    } else {
        station.find('option').not(':first').remove();
    }
}
$('.select-train-line').typeahead({
    highlight: true,
    minLength: 1,
    hint: false,
    delay: 2000,
},
    {
        source: function (query, process) {
            var rptItem = $(this).parent().closest(".repeater-item");
            var city_id = parseInt(rptItem.find("select.select-city").first().val());
            var params = $.extend({}, doAjax_params_default);
            params['url'] = '/Master/GetSuggestionTrainLineByCitys';
            params['requestType'] = 'POST';
            params['data'] = { query: query, city_id: city_id };
            params['dataType'] = "json";
            params['async'] = false;
            params['showloading'] = false;

            params['successCallbackFunction'] = function (result) {
                process(result.data);
            };
            doAjax(params);
        },
        display: 'furigana',
        templates: {
            suggestion: function (data) {
                var nameStr = data.furigana;
                return "<div class='ac-item' data-id='" + data.id + "'>" + nameStr + "</div>";
            }
        }
    });


$("#Address_Contact_Country").change(function () {
    var id = parseInt($(this).val());
    if (id !== 81) {
        $("#Address_Contact_City_Dropdown").addClass("hidden");
    } else {
        $("#Address_Contact_City_Dropdown").removeClass("hidden");
    }
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Master/GetRegionsByCountry';
    params['requestType'] = 'POST';
    params['data'] = { id: id };
    params['showloading'] = false;
    params['dataType'] = "html";

    params['successCallbackFunction'] = function (result) {
        $("#Address_Contact_Region").html(result);
        $("#Address_Contact_Prefecture").find('option').not(':first').remove();
        $("#Address_Contact_City").find('option').not(':first').remove();
        //$("#Address_TrainLine").find('option').remove();
        //$("#Address_Station").find('option').remove();

        $("#Address_Contact_Region").val($("#address_contact_region_id").val());
        $("#Address_Contact_Region").change();
    };
    doAjax(params);
});

$("#Address_Contact_Region").change(function () {
    var id = $(this).val();
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Master/GetPrefecturesByRegion';
    params['requestType'] = 'POST';
    params['data'] = { id: id };
    params['showloading'] = false;
    params['dataType'] = "html";

    params['successCallbackFunction'] = function (result) {
        $("#Address_Contact_Prefecture").html(result);
        $("#Address_Contact_City").find('option').not(':first').remove();

        $("#Address_Contact_Prefecture").val($("#address_contact_prefecture_id").val());
        $("#Address_Contact_Prefecture").change();        
    };
    doAjax(params);
});

$("#Address_Contact_Prefecture").change(function () {
    var id = $(this).val();
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Master/GetCitiesByPrefecture';
    params['requestType'] = 'POST';
    params['data'] = { id: id };
    params['showloading'] = false;
    params['dataType'] = "html";

    params['successCallbackFunction'] = function (result) {
        $("#Address_Contact_City").html(result);

        $("#Address_Contact_City").val($("#address_contact_city_id").val());
        $("#Address_Contact_City").change();
    };

    doAjax(params);
});

$("#frmProfile").submit(function () {
    if ($("#frmProfile").valid()) {
        var data = new FormData(this);
        var params = $.extend({}, doAjax_params_default);
        params['url'] = '/Member/UpdateProfile';
        params['requestType'] = 'POST';
        params['data'] = data;
        params['processData'] = false;
        params['contentType'] = false;
        params['dataType'] = "json";
        params['context'] = "#frmProfile";

        params['successCallbackFunction'] = function (result) {
            CatchAjaxResponseWithNotif(result);
        };
        doAjax(params);
    }
    return false;
});      