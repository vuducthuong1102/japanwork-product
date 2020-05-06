var AreaRepeater = {
    init: function () {
        $('.select-employment-id').change(function () {
            var employment_type = $(this).val();
            var params = $.extend({}, doAjax_params_default);
            params['url'] = '/Master/GetFieldsByEmploymentType';
            params['requestType'] = 'POST';
            params['data'] = { employment_type: employment_type };
            params['showloading'] = false;
            params['dataType'] = "html";

            params['successCallbackFunction'] = (result) => {
                var selectFields = $(this).closest('.repeater-item').find('select.select-fields');
                selectFields.html(result);
                selectFields.val('');
                selectFields.selectpicker("refresh");
            };
            doAjax(params);
        });
    }
};

function RepeaterUpdateCounter() {
    var total = 0;
    $(".wish-idx").each(function () {
        total++;
        $(this).html(total);
    });

    if (total > 2) {
        $('.btn-repeater-add').hide();
    } else {
        $('.btn-repeater-add').show();
    }
}

var FormRepeater = {
    init: function () {
        $(".WishModel").repeater({
            initEmpty: !1, defaultValues: { "text-input": "foo" },
            show: function () {
                $(this).slideDown();
                if ($(this).hasClass("new-rpt")) {
                    AreaRepeater.init();

                    $(this).find('.datepicker').datetimepicker({
                        format: "yyyy/mm/dd",
                        autoclose: !0, startView: 2, minView: 2
                    });

                    $(this).find('.select-employment-id').selectpicker();
                    $(this).find('.select-fields').selectpicker();
                    $(this).find('.select-work-place').selectpicker();

                    $(this).find("select.select-employment-id")
                        .val($(this).find("select.select-employment-id option:nth-child(2)").attr("value"))
                        .selectpicker("refresh");

                    $(this).find(".hdCurrentWishId").val("0");
                }

                $(".repeater-item").removeClass("new-rpt");
                RepeaterUpdateCounter();
            },
            hide: function (e) {
                $(this).slideUp(e);

                setTimeout(function () {
                    RepeaterUpdateCounter();
                }, 1000);
            }
        });
    }
};

FormRepeater.init();
AreaRepeater.init();
inItStation();

$(".repeater-item").removeClass("new-rpt");

$('.select-employment-id').selectpicker();
$('.select-fields').selectpicker();
$('.select-work-place').selectpicker();

$(function () {
    //$('.select2').select2({
    //    width: '100%'
    //});

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

    //$("#birthday").inputmask("99-99-9999", { autoUnmask: !0 });

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
        $("select.selectpicker").selectpicker("refresh");
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
    if (id > 0) {
        var params = $.extend({}, doAjax_params_default);
        params['url'] = '/Master/GetRegionsByCountry';
        params['requestType'] = 'POST';
        params['data'] = { id: id };
        params['showloading'] = false;
        params['dataType'] = "html";

        params['successCallbackFunction'] = function (result) {
            $("#Address_Region").html(result);

            if ($("#Address_Region option[value='" + $("#address_region_id").val() + "']").length > 0) {
                $("#Address_Region").val($("#address_region_id").val());
            } else {
                $("#Address_Region").val("0");
            }

            $("#Address_Region").change();
            $("#Address_Region").selectpicker('refresh');

        };
        doAjax(params);
    }
    $("#Address_Prefecture").find('option').not(':first').remove();
    $("#Address_City").find('option').not(':first').remove();
});

$("#Address_Region").change(function () {
    var id = $(this).val();
    if (id > 0) {
        var params = $.extend({}, doAjax_params_default);
        params['url'] = '/Master/GetPrefecturesByRegion';
        params['requestType'] = 'POST';
        params['data'] = { id: id };
        params['showloading'] = false;
        params['dataType'] = "html";

        params['successCallbackFunction'] = function (result) {
            $("#Address_Prefecture").html(result);

            if ($("#Address_Prefecture option[value='" + $("#address_prefecture_id").val() + "']").length > 0) {
                $("#Address_Prefecture").val($("#address_prefecture_id").val());
            } else {
                $("#Address_Prefecture").val("0");
            }
            $("#Address_Prefecture").change();
            $("#Address_Prefecture").selectpicker('refresh');

        };
        doAjax(params);
    }
    $("#Address_City").find('option').not(':first').remove();
    $("#Address_TrainLine").find('option').not(':first').remove();
});

$("#Address_Prefecture").change(function () {
    var id = $(this).val();
    if (id > 0) {
        var params = $.extend({}, doAjax_params_default);
        params['url'] = '/Master/GetCitiesByPrefecture';
        params['requestType'] = 'POST';
        params['data'] = { id: id };
        params['showloading'] = false;
        params['dataType'] = "html";

        params['successCallbackFunction'] = function (result) {
            $("#Address_City").html(result);

            if ($("#Address_City option[value='" + $("#address_city_id").val() + "']").length > 0) {
                $("#Address_City").val($("#address_city_id").val());
            } else {
                $("#Address_City").val("0");
            }

            $("#Address_City").change();
            $("#Address_City").selectpicker('refresh');

        };

        doAjax(params);
    }
    else {
        $("#Address_TrainLine").find('option').not(':first').remove();
    }
});

$("#Address_City").change(function () {
    var id = $(this).val();
    if (id > 0) {
        var params = $.extend({}, doAjax_params_default);
        params['url'] = '/Master/GetSuggestionTrainLineByCitys';
        params['requestType'] = 'POST';
        params['data'] = { id: id };
        params['showloading'] = false;
        params['dataType'] = "html";

        params['successCallbackFunction'] = function (result) {
            $("#Address_TrainLine").html(result);
            //$("#Address_TrainLine").find('option').not(':first').remove();

            $("#Address_TrainLine").val($("#address_train_line_id").val());
            $("#Address_TrainLine").change();
        };

        doAjax(params);
    }
});

//$("#Address_TrainLine").change(function () {
//    var id = $(this).val();
//    if (id > 0) {
//        var params = $.extend({}, doAjax_params_default);
//        params['url'] = '/Master/GetSuggestionStationByTrainLines';
//        params['requestType'] = 'POST';
//        params['data'] = { line_id: id };
//        params['showloading'] = false;
//        params['dataType'] = "html";

//        params['successCallbackFunction'] = function (result) {

//            $("#Address_Station").html(result);
//            //$("#Address_TrainLine").find('option').not(':first').remove();
//            //$("#Address_Station").find('option').not(':first').remove();
//            $("#Address_Station").val($("#address_station_id").val());
//            $("#Address_Station").change();
//            $("#Address_Station").selectpicker('refresh');

//        };

//        doAjax(params);
//    }
//});

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
        //$("#Address_TrainLine").find('option').not(':first').remove();
        //$("#Address_Station").find('option').not(':first').remove();

        if ($("#Address_Contact_Region option[value='" + $("#address_contact_region_id").val() + "']").length > 0) {
            $("#Address_Contact_Region").val($("#address_contact_region_id").val());
        } else {
            $("#Address_Contact_Region").val("0");
        }
        $("#Address_Contact_Region").change();
        $("#Address_Contact_Region").selectpicker('refresh');
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

        if ($("#Address_Contact_Prefecture option[value='" + $("#address_contact_prefecture_id").val() + "']").length > 0) {
            $("#Address_Contact_Prefecture").val($("#address_contact_prefecture_id").val());
        } else {
            $("#Address_Contact_Prefecture").val("0");
        }
        $("#Address_Contact_Prefecture").change();
        $("#Address_Contact_Prefecture").selectpicker('refresh');
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

        if ($("#Address_Contact_City option[value='" + $("#address_contact_city_id").val() + "']").length > 0) {
            $("#Address_Contact_City").val($("#address_contact_city_id").val());
        } else {
            $("#Address_Contact_City").val("0");
        }
        $("#Address_Contact_City").change();
        $("#Address_Contact_City").selectpicker('refresh');
    };

    doAjax(params);
});

$(document).on('typeahead:selected', ".select-train-line", function (evt, item) {
    $(this).closest(".m-typeahead").find(".train-line-selected").val(item.id);
    var id = parseInt($(".train-line-selected").val());    
    TrainLineChange(id);
});
$(document).on("change", ".select-train-line", function () {
    if ($(this).val() == "") {
        $(this).closest(".m-typeahead").find(".train-line-selected").val(0);
    }
})

function TrainLineChange(id) {
    //var id = parseInt($(event).closest(".m-typeahead").find(".train-line-selected").val());
    if (id > 0) {
        var params = $.extend({}, doAjax_params_default);
        params['url'] = '/Master/GetSuggestionStationByTrainLines';
        params['requestType'] = 'POST';
        params['data'] = { line_id: id };
        params['showloading'] = true;
        params['dataType'] = "html";

        params['successCallbackFunction'] = function (result) {            
            $("#Address_Station").html(result);
            var selectedStations = $("#Address_Station").data("current") + '';
            if (selectedStations != "0" && selectedStations.indexOf(",") >= 0) {
                $.each(selectedStations.split(","), function (i, e) {
                    $("#Address_Station").find("option[value='" + e + "']").prop("selected", true);
                });
            }
            else if (selectedStations != "") {
                if ($("#Address_Station").find("option[value='" + selectedStations + "']").length > 0) {
                    $("#Address_Station").val(selectedStations);
                }
                else {
                    $("#Address_Station").val(0);
                }
            }
            
            $("#Address_Station").selectpicker('refresh');
        };

        doAjax(params);
    } else {
        $("#Address_Station").find('option').not(':first').remove();
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
            //var city_id = parseInt($("#Address_City").val());
            var city_id = 0;
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
var isfocused = false;
//$('.select-train-line').on("focus", function (event) {
//    if ($(this).val() == "" && isfocused == false) {
//        isfocused = true;

//        var city_id = parseInt($("#Address_City").val());
//        var params = $.extend({}, doAjax_params_default);
//        params['url'] = '/Master/GetSuggestionTrainLineByCitys';
//        params['requestType'] = 'POST';
//        params['data'] = { query: "", city_id: city_id };
//        params['dataType'] = "json";
//        params['async'] = false;
//        params['showloading'] = false;

//        params['successCallbackFunction'] = function (result) {
//            if (result.data.length > 0) {
//                alert(1);
//                var contentHtml = "";
//                var maxlength = 5;
//                if (result.data.length < maxlength) maxlength = result.data.length;
//                for (var i = 0; i < maxlength; i++) {
//                    contentHtml += "<div class='ac-item' data-id='" + result.data[i].id + "'>" + result.data[i].furigana + "</div>";
//                }
//                $(".m-typeahead").find(".tt-menu").html("<div class='tt-dataset tt-dataset-0'>" + contentHtml + "</div> ");
//                $('.select-train-line').typeahead('open');
//            }
//        };
//        doAjax(params);
//    }
//});
$("#frmProfile").submit(function () {
    if ($("#frmProfile").valid()) {
        var data = new FormData(this);
        var params = $.extend({}, doAjax_params_default);
        params['url'] = '/JobSeeker/UpdateProfile';
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
function inItStation() {
    var line_id = $(".train-line-selected").val();
    TrainLineChange(parseInt(line_id));
}