function Ct_ShowListRegion() {
    var toggleClassName = "hidden-sm-down";
    $(".ct-area-region").removeClass(toggleClassName);
    $(".ct-area-prefecture").addClass(toggleClassName);
    $(".ct-area-city").addClass(toggleClassName);
}

function Ct_ShowListPrefecture() {
    var toggleClassName = "hidden-sm-down";
    $(".ct-area-region").addClass(toggleClassName);
    $(".ct-area-prefecture").removeClass(toggleClassName);
    $(".ct-area-city").addClass(toggleClassName);
}

function Ct_ShowListCity() {
    var toggleClassName = "hidden-sm-down";
    $(".ct-area-region").addClass(toggleClassName);
    $(".ct-area-prefecture").addClass(toggleClassName);
    $(".ct-area-city").removeClass(toggleClassName);
}

$("body").on("click", ".ct-choose-region", function () {
    Ct_ShowListRegion();
});

$("body").on("click", ".ct-choose-prefecture", function () {
    Ct_ShowListPrefecture();
});

$("body").on("click", ".ct-choose-city", function () {
    Ct_ShowListCity();
});

$("#btnClearCityFilter").click(function () {
    $(".ct-region-flag").addClass("hidden");
    $(".ct-prefecture-flag").addClass("hidden");
    $(".city-cbx-all").prop("checked", false);
    $(".city-by-prefecture-cbx").prop("checked", false);

    $("#city_ids").val("");
    $("#ct_prefecture_ids").val("");
    $("#ct_region_ids").val("");
});

$("#btnAllowSelectedCity").click(function () {
    var strCityIds = $("#city_ids").val();
    if (strCityIds == null || strCityIds == "") {
        $("#btnSelectCity").find("span").html(LanguageDic["LB_SELECT_PLACE"]);
    } else {
        var arr = strCityIds.split(",");
        if (arr.length > 0) {
            $("#btnSelectCity").find("span").html(arr.length + " " + LanguageDic["LB_PLACES"]);
        }
    }
});

function Ct_HasSeletedCity(id) {
    var counter = 0;
    $("#ct_filter_cities_" + id).find(".city-by-prefecture-cbx").each(function () {
        if ($(this).is(":checked")) {
            counter++;
        }
    });

    return counter;
}

function Ct_SetPrefectureSelecting(id) {
    var prefectureElem = $(".ct-prefecture-flag-" + id);
    if (Ct_HasSeletedCity(id) > 0) {
        prefectureElem.removeClass("hidden");
    }
    else {
        prefectureElem.addClass("hidden");
    }

    var listPrefectureIds = ConvertStringToListArray($("#ct_prefecture_ids").val());
    var listCityIds = ConvertStringToListArray($("#city_ids").val());

    $(".city-by-prefecture-cbx").each(function () {
        var cityId = parseInt($(this).val());
        if ($(this).is(":checked")) {
            if (!listCityIds.includes(cityId)) {
                listCityIds.push(cityId);
            }
        } else {
            RemoveItemFromArray(listCityIds, cityId);
        }
    });

    $(".ct-prefecture-flag").each(function () {
        var prefectureId = parseInt($(this).data("id"));
        if (!$(this).hasClass("hidden")) {
            if (!listPrefectureIds.includes(prefectureId)) {
                listPrefectureIds.push(prefectureId);
            }
        } else {
            RemoveItemFromArray(listPrefectureIds, prefectureId);
        }
    });

    $("#city_ids").val(listCityIds);
    $("#ct_prefecture_ids").val(listPrefectureIds);

    Ct_SetRegionSelecting(id);
}

function Ct_SetRegionSelecting(prefecture_id) {
    var listRegionIds = ConvertStringToListArray($("#ct_region_ids").val());
    var prefectureElem = $(".ct-prefecture-flag-" + prefecture_id);
    var region_id = prefectureElem.data("region");
    var selectedPrefectCounter = 0;
    var regionElem = $(".ct-region-flag-" + region_id);
    $("#ct_filter_prefectures_" + region_id).find(".ct-prefecture-flag").each(function () {
        if (!$(this).hasClass("hidden")) {
            selectedPrefectCounter++;
        }
    });

    if (selectedPrefectCounter > 0) {
        regionElem.removeClass("hidden");
    } else {
        regionElem.addClass("hidden");
    }

    $(".ct-region-flag").each(function () {
        var currentRegionId = parseInt($(this).data("id"));
        if (!$(this).hasClass("hidden")) {
            if (!listRegionIds.includes(currentRegionId)) {
                listRegionIds.push(currentRegionId);
            }
        } else {
            RemoveItemFromArray(listRegionIds, currentRegionId);
        }
    });

    $("#ct_region_ids").val(listRegionIds);
}

$(function () {
    $("body").on("click", ".city-cbx-all", function () {
        var prefecture_id = $(this).val();
        var currentContainer = $("#ct_filter_cities_" + prefecture_id);
        if ($(this).is(":checked")) {
            currentContainer.find(".city-by-prefecture-cbx").prop("checked", true);
        } else {
            currentContainer.find(".city-by-prefecture-cbx").prop("checked", false);
        }

        Ct_SetPrefectureSelecting(prefecture_id);
    });

    $("body").on("click", ".city-by-prefecture-cbx", function () {
        var prefecture_id = $(this).data("prefecture");
        var subCheckboxes = $(this).closest(".filter-cities").find(".city-by-prefecture-cbx");
        var totalSubCbx = subCheckboxes.length;
        var checkedTotal = 0;
        subCheckboxes.each(function () {
            if ($(this).is(":checked")) {
                checkedTotal++;
            }
        });

        if (checkedTotal == totalSubCbx) {
            $("#city_all_" + prefecture_id).prop("checked", true);
        } else {
            $("#city_all_" + prefecture_id).prop("checked", false);
        }

        Ct_SetPrefectureSelecting(prefecture_id);
    });

    $("body").on("click", ".ct-region-select", function () {
        $(".ct-region-select").removeClass("active");
        var id = $(this).data("id");
        $(this).toggleClass("active");

        $(".ct-prefectures-default").addClass("hidden");
        var prefecturesContainer = $("#ct_filter_prefectures_" + id);

        $(".filter-cities").addClass("hidden");
        $(".cities-default").removeClass("hidden");

        if (prefecturesContainer.length > 0) {
            $(".ct-filter-prefectures").addClass("hidden");
            prefecturesContainer.removeClass("hidden");

            Ct_ShowListPrefecture();
        } else {
            Ct_GetPrefecturesByRegion(id);
        }
    });

    $("body").on("click", ".ct-prefecture-select", function () {
        $(".ct-prefecture-select").removeClass("active");
        var id = $(this).data("id");
        $(this).toggleClass("active");

        $(".cities-default").addClass("hidden");
        var container = $("#ct_filter_cities_" + id);
        if (container.length > 0) {
            $(".filter-cities").addClass("hidden");
            container.removeClass("hidden");

            Ct_ShowListCity();
        } else {
            Ct_GetCitiesByPrefecture(id);
        }
    });

    function Ct_RemarkSelectedCities() {
        $(".filter-cities").each(function () {
            var subCheckboxes = $(this).find(".city-by-prefecture-cbx");
            var totalSubCbx = subCheckboxes.length;
            var checkedTotal = 0;
            var prefecture_id = 0;
            subCheckboxes.each(function () {
                prefecture_id = $(this).data("prefecture");
                if ($(this).is(":checked")) {
                    checkedTotal++;
                }
            });

            if (checkedTotal == totalSubCbx) {
                $("#city_all_" + prefecture_id).prop("checked", true);
            } else {
                $("#city_all_" + prefecture_id).prop("checked", false);
            }
        });
    }

    function Ct_GetPrefecturesByRegion(id) {
        var params = $.extend({}, doAjax_params_default);
        params['url'] = '/Master/Ct_FilterGetPrefecturesByRegion';
        params['requestType'] = 'POST';
        params['data'] = { id: id, ct_prefecture_ids: $("#ct_prefecture_ids").val() };
        params['showloading'] = true;
        params['dataType'] = "html";

        params['successCallbackFunction'] = function (result) {
            $(".ct-filter-prefectures").addClass("hidden");
            $("#ct_filterPrefecturesList").append(result);

            Ct_ShowListPrefecture();
        };
        doAjax(params);
    }

    function Ct_GetCitiesByPrefecture(id) {
        var params = $.extend({}, doAjax_params_default);
        params['url'] = '/Master/Ct_FilterGetCitiesByPrefecture';
        params['requestType'] = 'POST';
        params['data'] = { id: id, city_ids: $("#city_ids").val() };
        params['showloading'] = true;
        params['dataType'] = "html";

        params['successCallbackFunction'] = function (result) {
            $(".filter-cities").addClass("hidden");
            $("#ct_filterCitiesList").append(result);

            Ct_RemarkSelectedCities();

            Ct_ShowListCity();
        };
        doAjax(params);
    }
})