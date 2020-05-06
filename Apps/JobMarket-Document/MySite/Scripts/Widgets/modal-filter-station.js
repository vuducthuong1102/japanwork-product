function St_ShowListRegion() {
    var toggleClassName = "hidden-sm-down";
    $(".st-area-region").removeClass(toggleClassName);
    $(".st-area-prefecture").addClass(toggleClassName);
    $(".st-area-train-line").addClass(toggleClassName);
}

function St_ShowListPrefecture() {
    var toggleClassName = "hidden-sm-down";
    $(".st-area-region").addClass(toggleClassName);
    $(".st-area-prefecture").removeClass(toggleClassName);
    $(".st-area-train-line").addClass(toggleClassName);
}

function St_ShowListTrainLine() {
    var toggleClassName = "hidden-sm-down";
    $(".st-area-region").addClass(toggleClassName);
    $(".st-area-prefecture").addClass(toggleClassName);
    $(".st-area-train-line").removeClass(toggleClassName);
}

$("body").on("click", ".st-choose-region", function () {
    St_ShowListRegion();
});

$("body").on("click", ".st-choose-prefecture", function () {
    St_ShowListPrefecture();
});

$("body").on("click", ".st-choose-station", function () {
    St_ShowListTrainLine();
});

$("#btnClearStationFilter").click(function () {
    $(".st-region-flag").addClass("hidden");
    $(".st-prefecture-flag").addClass("hidden");
    $(".st-train-flag").addClass("hidden");
    $(".train-by-prefecture-cbx").prop("checked", false);
    $(".sub-item-cbx-station").prop("checked", false);

    $("#station_ids").val("");
    $("#st_prefecture_ids").val("");
    $("#st_region_ids").val("");
});

$("#btnAllowSelectedStation").click(function () {
    var strStationIds = $("#station_ids").val();
    if (strStationIds == null || strStationIds == "") {
        $("#btnSelectStation").find("span").html(LanguageDic["LB_SELECT_STATION"]);
    } else {
        var arr = strStationIds.split(",");
        if (arr.length > 0) {
            $("#btnSelectStation").find("span").html(arr.length + " " + LanguageDic["LB_STATIONS"]);
        }
    }
});

function St_PrefHasSelectedStation(prefecture_id) {
    var counter = 0;
    $("#st_filter_train_lines_" + prefecture_id).find(".sub-item-cbx-station").each(function () {
        if ($(this).is(":checked")) {
            counter++;
        }
    });

    return counter;
}

function St_TrainHasSelectedStation(prefecture_id, train_id) {
    var counter = 0;
    $("#train_by_prefecture_" + prefecture_id + "_" + train_id).find(".sub-item-cbx-station").each(function () {
        if ($(this).is(":checked")) {
            counter++;
        }
    });

    return counter;
}

function St_SetPrefectureSelecting(prefecture_id, train_id) {
    var prefectureElem = $(".st-prefecture-flag-" + prefecture_id);
    var trainLineElem = $("#train_by_prefecture_" + prefecture_id + "_" + train_id);
    if (St_PrefHasSelectedStation(prefecture_id) > 0) {
        prefectureElem.removeClass("hidden");
    }
    else {
        prefectureElem.addClass("hidden");
    }

    if (St_TrainHasSelectedStation(prefecture_id, train_id) > 0) {
        trainLineElem.find(".st-train-flag").removeClass("hidden");
    } else {
        trainLineElem.find(".st-train-flag").addClass("hidden");
    }

    var listStationIds = ConvertStringToListArray($("#station_ids").val());
    var listPrefectureIds = ConvertStringToListArray($("#st_prefecture_ids").val());
    $(".sub-item-cbx-station").each(function () {
        var stationId = parseInt($(this).val());
        if ($(this).is(":checked")) {
            if (!listStationIds.includes(stationId)) {
                listStationIds.push(stationId);
            }
        } else {
            RemoveItemFromArray(listStationIds, stationId);
        }
    });

    $(".st-prefecture-flag").each(function () {
        var prefectureId = parseInt($(this).data("id"));
        if (!$(this).hasClass("hidden")) {
            if (!listPrefectureIds.includes(prefectureId)) {
                listPrefectureIds.push(prefectureId);
            }
        } else {
            RemoveItemFromArray(listPrefectureIds, prefectureId);
        }
    });

    $("#st_prefecture_ids").val(listPrefectureIds);
    $("#station_ids").val(listStationIds);

    St_SetRegionSelecting(prefecture_id);
}

function St_SetRegionSelecting(prefecture_id) {
    var listRegionIds = ConvertStringToListArray($("#st_region_ids").val());
    var prefectureElem = $(".st-prefecture-flag-" + prefecture_id);
    var region_id = prefectureElem.data("region");
    var selectedPrefectCounter = 0;
    var regionElem = $(".st-region-flag-" + region_id);
    $("#st_filter_prefectures_" + region_id).find(".st-prefecture-flag").each(function () {
        if (!$(this).hasClass("hidden")) {
            selectedPrefectCounter++;
        }
    });

    if (selectedPrefectCounter > 0) {
        regionElem.removeClass("hidden");
    } else {
        regionElem.addClass("hidden");
    }

    $(".st-region-flag").each(function () {
        var currentRegionId = parseInt($(this).data("id"));
        if (!$(this).hasClass("hidden")) {
            if (!listRegionIds.includes(currentRegionId)) {
                listRegionIds.push(currentRegionId);
            }
        } else {
            RemoveItemFromArray(listRegionIds, currentRegionId);
        }
    });

    $("#st_region_ids").val(listRegionIds);
}

$(function () {
    $("body").on("click", ".st-item-control", function () {
        var id = $(this).data("id");
        var prefecture_id = $(this).data("prefecture");
        $("#train_by_prefecture_" + prefecture_id + "_" + id).find(".item-station").toggleClass("hidden");
        $(this).find("i").toggleClass("la-chevron-up");
    });

    $("body").on("click", ".train-by-prefecture-cbx", function () {
        var id = $(this).data("id");
        var prefecture_id = $(this).data("prefecture");
        var currentContainer = $("#train_by_prefecture_" + prefecture_id + "_" + id);
        if ($(this).is(":checked")) {
            currentContainer.find(".sub-item-cbx-station").prop("checked", true);
        } else {
            currentContainer.find(".sub-item-cbx-station").prop("checked", false);
        }

        St_SetPrefectureSelecting(prefecture_id, id);
    });

    $("body").on("click", ".sub-item-cbx-station", function () {
        var subCheckboxes = $(this).closest(".item-station").find(".sub-item-cbx-station");
        var totalSubCbx = subCheckboxes.length;
        var checkedTotal = 0;
        var prefecture_id = $(this).data("prefecture");
        var train_id = $(this).data("train");
        subCheckboxes.each(function () {
            if ($(this).is(":checked")) {
                checkedTotal++;
            }
        });

        if (checkedTotal == totalSubCbx) {
            $(this).closest(".sub-item").find(".train-by-prefecture-cbx").prop("checked", true);
        } else {
            $(this).closest(".sub-item").find(".train-by-prefecture-cbx").prop("checked", false);
        }

        St_SetPrefectureSelecting(prefecture_id, train_id);
    });

    $("body").on("click", ".st-region-select", function () {
        $(".st-region-select").removeClass("active");
        var id = $(this).data("id");
        $(this).toggleClass("active");

        $(".st-prefectures-default").addClass("hidden");
        var prefecturesContainer = $("#st_filter_prefectures_" + id);

        $(".filter-train-lines").addClass("hidden");
        $(".train-lines-default").removeClass("hidden");

        if (prefecturesContainer.length > 0) {
            $(".st-filter-prefectures").addClass("hidden");
            prefecturesContainer.removeClass("hidden");

            St_ShowListPrefecture();
        } else {
            St_GetPrefecturesByRegion(id);
        }
    });

    $("body").on("click", ".st-prefecture-select", function () {
        $(".st-prefecture-select").removeClass("active");
        var id = $(this).data("id");
        $(this).toggleClass("active");

        $(".train-lines-default").addClass("hidden");
        var container = $("#st_filter_train_lines_" + id);

        if (container.length > 0) {
            $(".filter-train-lines").addClass("hidden");
            container.removeClass("hidden");

            St_ShowListTrainLine();
        } else {
            St_GetTrainLinesByPrefecture(id);
        }
    });

    function St_GetPrefecturesByRegion(id) {
        var params = $.extend({}, doAjax_params_default);
        params['url'] = '/Master/St_FilterGetPrefecturesByRegion';
        params['requestType'] = 'POST';
        params['data'] = { id: id, st_prefecture_ids: $("#st_prefecture_ids").val() };
        params['showloading'] = true;
        params['dataType'] = "html";

        params['successCallbackFunction'] = function (result) {

            $(".st-filter-prefectures").addClass("hidden");
            $("#st_filterPrefecturesList").append(result);

            St_ShowListPrefecture();
        };
        doAjax(params);
    }

    function St_RemarkSelectedTrainLines() {
        $(".train-line-item").each(function () {
            var subCheckboxes = $(this).find(".sub-item-cbx-station");
            var totalSubCbx = subCheckboxes.length;
            var checkedTotal = 0;
            var train_id = 0;
            var prefecture_id = 0;
            subCheckboxes.each(function () {
                train_id = $(this).data("train");
                prefecture_id = $(this).data("prefecture");
                if ($(this).is(":checked")) {
                    checkedTotal++;
                }
            });

            if (checkedTotal == totalSubCbx) {
                $("#train_line_" + prefecture_id + "_" + train_id).prop("checked", true);
            } else {
                $("#train_line_" + prefecture_id + "_" + train_id).prop("checked", false);
            }
        });
    }

    function St_GetTrainLinesByPrefecture(id) {
        var params = $.extend({}, doAjax_params_default);
        params['url'] = '/Master/St_FilterGetTrainLinesByPrefecture';
        params['requestType'] = 'POST';
        params['data'] = { id: id, station_ids: $("#station_ids").val() };
        params['showloading'] = true;
        params['dataType'] = "html";

        params['successCallbackFunction'] = function (result) {
            $(".filter-train-lines").addClass("hidden");
            $("#filterTrainLinesList").append(result);

            St_RemarkSelectedTrainLines();

            St_ShowListTrainLine();
        };
        doAjax(params);
    }
})