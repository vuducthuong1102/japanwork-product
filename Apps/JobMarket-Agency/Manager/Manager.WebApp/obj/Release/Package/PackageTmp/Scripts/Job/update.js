//$(".m-select2").select2({
//});

$(".select-employment-type").change(function () {
    var calculate = $('option:selected', this).data('calculate');
    $("#SalaryBy").html(calculate);
});

function InitStationPicker() {
    $('.station-picker').selectpicker({
        actionsBox: true,
        selectAllText: LanguageDic["LB_SELECT_ALL"],
        deselectAllText: LanguageDic["LB_DESELECT_ALL"]
    }).ajaxSelectPicker({
        ajax: {
            // data source
            url: '/Master/GetSuggestionStations',
            // ajax type
            type: 'POST',
            // data type
            dataType: 'json',
            // Use "{{{q}}}" as a placeholder and Ajax Bootstrap Select will
            // automatically replace it with the value of the search query.
            data: {
                query: '{{{q}}}',
                __RequestVerificationToken: $('input[name = "__RequestVerificationToken"]').val(),
                place_id: 0
            }
        },
        locale: {
            emptyTitle: LanguageDic["LB_KEYWORD_SEARCH"],
            statusInitialized: "",
            statusSearching: LanguageDic["LB_SEARCHING"],
            statusNoResults: LanguageDic["LB_NO_RESULTS_FOUND"]
        },
        // function to preprocess JSON data
        preprocessData: function (data) {
            var result = data.data;
            var i, l = result.length, array = [];
            if (l) {
                for (i = 0; i < l; i++) {
                    array.push($.extend(true, result[i], {
                        text: result[i].furigana + " (" + result[i].station + ")",
                        value: result[i].id
                        //data: {
                        //    subtext: data[i].station
                        //}
                    }));
                }
            }
            return array;
        }

    });
    //$('.select-station').selectpicker();
}

$(".select-employment-type").change(function () {
    var employment_type = $(this).val();
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Master/GetFieldsByEmploymentType';
    params['requestType'] = 'POST';
    params['data'] = { employment_type: employment_type };
    params['showloading'] = false;
    params['dataType'] = "html";

    params['successCallbackFunction'] = function (result) {
        $(".select-job-field").html(result);
        $(".select-job-field .selectpicker").selectpicker();
    };
    doAjax(params);
});

$('.select-tag').select2({
    placeholder: LanguageDic['LB_KEYWORD_SEARCH'],
    width: '100%',
    language: {
        searching: function () {
            return LanguageDic['LB_SEARCHING'];
        }
    },
    ajax: {
        url: '/Master/GetSuggestionTags',
        type: 'POST',
        quietMillis: 1000,
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
                results: data.data.map(function (item) {
                    return {
                        id: item.id,
                        text: item.tag
                    };
                })
                //pagination: {
                //    // If there are 10 matches, there's at least another page
                //    more: data.matches.length === 10
                //}
            };
        },
        cache: true
    }
});

var AreaRepeater = {
    init: function () {

        $(".select-country").change(function () {
            var counter = parseInt($(this).attr("counter"));
            counter++;
            $(this).attr("counter", counter);

            var id = parseInt($(this).val());
            var rptItem = $(this).parent().closest(".repeater-item");
            var region = rptItem.find(".select-region").first();
            var prefecture = rptItem.find(".select-prefecture").first();
            var city = rptItem.find(".select-city").first();

            if (id !== _defaultCountry) {
                //!$("#DefaultRegion").hasClass("region-loaded") 
                var params = $.extend({}, doAjax_params_default);
                params['url'] = '/Master/GetRegionsByCountry';
                params['requestType'] = 'POST';
                params['data'] = { id: id };
                params['showloading'] = false;
                params['dataType'] = "html";

                params['successCallbackFunction'] = function (result) {
                    region.html(result);
                    prefecture.find('option').not(':first').remove();
                    city.find('option').not(':first').remove();

                    if (counter === 1) {
                        region.val(region.data("current"));
                        region.change();
                    }
                };
                doAjax(params);
            } else {
                if (!$("#DefaultRegion").hasClass("region-loaded")) {
                    var params1 = $.extend({}, doAjax_params_default);
                    params1['url'] = '/Master/GetRegionsByCountry';
                    params1['requestType'] = 'POST';
                    params1['data'] = { id: id };
                    params1['showloading'] = false;
                    params1['dataType'] = "html";

                    params1['successCallbackFunction'] = function (result) {
                        $("#DefaultRegion").html(result);
                        region.html($("#DefaultRegion").html());
                        prefecture.find('option').not(':first').remove();
                        city.find('option').not(':first').remove();

                        $("#DefaultRegion").addClass("region-loaded");

                        if (counter === 1) {
                            region.val(region.data("current"));
                            region.change();
                        }
                    };
                    doAjax(params1);
                } else {

                    region.html($("#DefaultRegion").html());
                    prefecture.find('option').not(':first').remove();
                    city.find('option').not(':first').remove();
                    if (counter === 1) {

                        region.val(region.data("current"))
                        region.change();
                    }
                }
            }
        });

        $(".select-region").change(function () {
            var counter = parseInt($(this).attr("counter"));
            counter++;

            $(this).attr("counter", counter);

            var id = parseInt($(this).val());
            var rptItem = $(this).parent().closest(".repeater-item");
            var prefecture = rptItem.find(".select-prefecture").first();
            var city = rptItem.find(".select-city").first();

            if (id > 0) {
                var params = $.extend({}, doAjax_params_default);
                params['url'] = '/Master/GetPrefecturesByRegion';
                params['requestType'] = 'POST';
                params['data'] = { id: id };
                params['showloading'] = false;
                params['dataType'] = "html";

                params['successCallbackFunction'] = function (result) {
                    prefecture.html(result);
                    city.find('option').not(':first').remove();

                    if (counter === 1) {
                        prefecture.val(prefecture.data("current"));
                        prefecture.change();
                    }
                };
                doAjax(params);
            } else {
                prefecture.find('option').not(':first').remove();
                city.find('option').not(':first').remove();
            }
        });

        $(".select-prefecture").change(function () {
            var counter = parseInt($(this).attr("counter"));
            counter++;

            $(this).attr("counter", counter);

            var id = parseInt($(this).val());
            var rptItem = $(this).parent().closest(".repeater-item");
            var city = rptItem.find("select.select-city").first();
            var line = rptItem.find(".select-train-line").first();
            var line_id = rptItem.find(".train-line-selected").first();
            if (id > 0) {
                var params = $.extend({}, doAjax_params_default);
                params['url'] = '/Master/GetCitiesByPrefecture';
                params['requestType'] = 'POST';
                params['data'] = { id: id };
                params['showloading'] = false;
                params['dataType'] = "html";

                params['successCallbackFunction'] = function (result) {
                    city.html(result);
                    if (counter === 1) {
                        city.val(city.data("current"));
                        city.change();
                        //line_id.val(line_id.data("current"));
                        //TrainLineChange(line);
                    }
                    city.selectpicker("refresh");
                };

                doAjax(params);
            } else {
                city.find('option').not(':first').remove();
                city.selectpicker("refresh");
            }
        });

        //$(".select-city").change(function () {
        //    var counter = parseInt($(this).attr("counter"));
        //    counter++;

        //    $(this).attr("counter", counter);

        //    var id = parseInt($(this).val());
        //    var rptItem = $(this).parent().closest(".repeater-item");
        //    var line = rptItem.find(".select-train-line").first();
        //    var station = rptItem.find(".select-station").first();

        //    if (id > 0) {
        //        var params = $.extend({}, doAjax_params_default);
        //        params['url'] = '/Master/GetSuggestionTrainLineByCitys';
        //        params['requestType'] = 'POST';
        //        params['data'] = { id: id };
        //        params['showloading'] = false;
        //        params['dataType'] = "html";

        //        params['successCallbackFunction'] = function (result) {
        //            line.html(result);
        //            station.find('option').not(':first').remove();

        //            if (counter === 1) {
        //                line.val(line.data("current"));
        //                line.change();
        //            }
        //        };

        //        doAjax(params);
        //    } else {
        //        line.find('option').not(':first').remove();
        //        station.find('option').not(':first').remove();
        //    }
        //});
        $(document).on("typeahead:selected", ".select-train-line", function (evt, item) {
            $(this).closest(".m-typeahead").find(".train-line-selected").val(item.id);            
            TrainLineChange(this);
        });
        $(document).on("change", ".select-train-line", function () {            
            if ($(this).val() == "") {
                $(this).closest(".m-typeahead").find(".train-line-selected").val(0);
            }
            //TrainLineChange(this);
        })

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

        //$(".select-city").change(function () {
        //    var counter = parseInt($(this).attr("counter"));
        //    counter++;

        //    $(this).attr("counter", counter);

        //    var id = parseInt($(this).val());
        //    var rptItem = $(this).parent().closest(".repeater-item");
        //    var station = rptItem.find("select.select-station").first();

        //    if (id > 0) {
        //        var params = $.extend({}, doAjax_params_default);
        //        params['url'] = '/Master/GetSuggestionStations';
        //        params['requestType'] = 'POST';
        //        params['data'] = { city_id: id };
        //        params['showloading'] = false;
        //        params['dataType'] = "html";

        //        params['successCallbackFunction'] = function (result) {
        //            station.html(result);
        //            var selectedStations = station.data("current") + '';

        //            if (counter === 1) {
        //                if (selectedStations != "0" && selectedStations.indexOf(",") >= 0) {
        //                    $.each(selectedStations.split(","), function (i, e) {
        //                        station.find("option[value='" + e + "']").prop("selected", true);
        //                    });
        //                }
        //                else {
        //                    station.val(selectedStations);
        //                }
        //            }
        //            station.selectpicker('refresh');
        //        };

        //        doAjax(params);
        //    } else {
        //        station.find('option').not(':first').remove();
        //    }
        //});

        InitStationPicker();
    }
};

function RepeaterUpdateCounter() {
    var total = 0;
    $(".add-idx").each(function () {
        total++;
        $(this).html(total);
    });
}

var Summernote = {
    init: function () {
        $(".summernote").summernote({ height: 150 });
        $(".note-group-select-from-files").remove();
    }
};

var FormRepeater = {
    init: function () {
        $(".Addresses").repeater({
            initEmpty: !1, defaultValues: { "text-input": "foo" },
            show: function () {
                $(this).slideDown();
                if ($(this).hasClass("new-rpt")) {
                    AreaRepeater.init();

                    var country = $(this).find(".select-country");
                    country.val(_defaultCountry);
                    country.attr("counter", 1);

                    $(this).find(".select-region").val("0").attr("counter", 1);
                    $(this).find(".select-prefecture").val("0").attr("counter", 1);
                    $(this).find(".select-city").val("0").attr("counter", 1);
                    $(this).find(".train-line-selected").val("0")
                    $(this).find(".select-train-line").attr("counter", 1);
                    $(this).find(".select-station").val("0").attr("counter", 1);
                    $(this).find(".hdCurrentAddId").val("0");
                    country.change();
                }

                $(".repeater-item").removeClass("new-rpt");
                RepeaterUpdateCounter();

                InitStationPicker();
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

//$("#btnSaveJob").click(function () {
//    $("#frmUpdateJob").submit();
//});

$("#frmUpdateJob").submit(function () {

    var data = new FormData(this);
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Job/Edit';
    params['requestType'] = 'POST';
    params['data'] = data;
    params['processData'] = false;
    params['contentType'] = false;
    params['dataType'] = "json";
    params['context'] = "#frmUpdateJob";

    params['successCallbackFunction'] = function (result) {
        CatchAjaxResponseWithNotif(result);
        if (result.success && result.message) {
            window.location.href = $("#hdfDetailLink").val();
        }
    };
    doAjax(params);

    return false;
});

$("#frmUpdateLanguage").submit(function () {

    var data = new FormData(this);
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Job/Language';
    params['requestType'] = 'POST';
    params['data'] = data;
    params['processData'] = false;
    params['contentType'] = false;
    params['dataType'] = "json";
    params['context'] = "#frmUpdateLanguage";

    params['successCallbackFunction'] = function (result) {
        CatchAjaxResponseWithNotif(result);
        if (result.success && result.message) {
            window.location.href = $("#hdfDetailLink").val();
        }
    };
    doAjax(params);

    return false;
});

function TrainLineChange(event) {
    var counter = parseInt($(event).attr("counter"));
    counter++;

    $(event).attr("counter", counter);

    var id = parseInt($(event).closest(".m-typeahead").find(".train-line-selected").val());

    var rptItem = $(event).parent().closest(".repeater-item");
    var station = rptItem.find("select.select-station").first();

    if (id > 0) {
        station.html("");
        var params = $.extend({}, doAjax_params_default);
        params['url'] = '/Master/GetSuggestionStationByTrainLines';
        params['requestType'] = 'POST';
        params['data'] = { line_id: id };
        params['showloading'] = true;
        params['dataType'] = "html";

        params['successCallbackFunction'] = function (result) {
            station.html(result);
            //var selectedStations = station.data("current") + '';
            //alert(selectedStations);
            station.val(station.data("current"));
            station.attr("selected", "selected");
            //if (counter === 1) {
            //    station.val(selectedStations);

            //}
            station.selectpicker('val', station.data("current"));
            station.selectpicker("refresh");
        };

        doAjax(params);
    } else {
        station.find('option').not(':first').remove();
        station.selectpicker("refresh");
    }
}

function inItStation() {
    $(".repeater-item").each(function () {
        var line = $(this).find(".select-train-line").first();
        var line_id = $(this).find(".train-line-selected").first();
        line_id.val(line_id.data("current"));
        TrainLineChange(line);
    })
}
$(function () {
    //Summernote.init();
    FormRepeater.init();
    AreaRepeater.init();
    $('select.station-picker').each(function () {
        var value = $(this).attr("data-current");
        $(this).val(value);
        $(this).selectpicker('refresh');
    });


    $(".repeater-item").removeClass("new-rpt");

    $(".timepicker").timepicker({
        defaultTime: "08:00",
        minuteStep: 5,
        showSeconds: !1,
        showMeridian: !1
    });

    $(".timepicker-end").timepicker({
        defaultTime: "18:00",
        minuteStep: 5,
        showSeconds: !1,
        showMeridian: !1
    });

    $(".summernote").removeClass("hidden");

    $(".select-country").change();

    function changeLange(value) {
        $("#language_code").val(value);
    }

    inItStation();
    //$(".selectpicker").selectpicker();
});