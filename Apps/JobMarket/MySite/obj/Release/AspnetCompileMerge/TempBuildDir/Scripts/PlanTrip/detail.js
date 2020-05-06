

var _window = $(window);
var _currentPage = 1;
var _hasNoData = false;

$(function () {
    GetCreatedPlans();

    GetSuggestedPlans(_currentPage);

    // Each time the user scrolls
    _window.scroll(function () {
        // End of the document reached?
        if ($(document).height() - _window.height() == _window.scrollTop()) {
            _currentPage++;

            if (!_hasNoData)
                GetSuggestedPlans(_currentPage);
        }
    });

    $("body").on('click', '.btn-create-plan', function () {
        $(".trips-post").html("");
        var params = $.extend({}, doAjax_params_default);
        editable = true;
        var currentPlace = $("#CurrentPlace").val();
        params['url'] = '/PlanTrip/GetPlacesByParent';
        params['requestType'] = 'POST';
        params['data'] = { placeId: currentPlace };
        params['context'] = $(this);
        params['dataType'] = "json";

        params['successCallbackFunction'] = function (result) {
            if (result) {
                if (result.success) {
                    if (result.data) {
                        $('#myModal').modal("show");
                        setTimeout(function () {
                            quitModal = false;
                            $(".btn-confirm-save").removeClass("hidden");
                            initMaps('mapdetail', result.data);

                            if (result.data.groups_html) {
                                $("#PlaceTypeGroups").html(result.data.groups_html);
                                $("#AllTypeGroups").html("(" + result.data.totalplaces + ")");
                            }
                        }, 500)

                    }
                }
            }
        };
        doAjax(params);
    });

    $("body").on('click', '.maps-marker', function () {
        var placeId = $(this).data("place");
        var containerId = "posts-place-" + placeId;
        if ($("." + containerId).length > 0) {
            $(".trips-post").html($("." + containerId).html());
            setTimeout(function () {
                $(".trips-post").find(".content").removeClass("hidden");
                $(".trips-post").find('.content').slick();                
            }, 500);

            return false;
        }

        var params = $.extend({}, doAjax_params_default);
        var currentPlace = $(this).data('id');
        params['url'] = '/PlanTrip/GetTopByPlace';
        params['requestType'] = 'POST';
        params['data'] = { placeId: placeId };
        params['context'] = $(this);
        params['dataType'] = "json";

        params['successCallbackFunction'] = function (result) {
            if (result) {
                if (result.success) {
                    if (result.html) {
                        $(".posts-place").append('<div class="' + containerId + '"></div>');
                        $("." + containerId).html(result.html);

                        $(".trips-post").html($("." + containerId).html());
                        setTimeout(function () {
                            $(".trips-post").find(".content").removeClass("hidden");
                            $(".trips-post").find('.content').slick();                           
                        }, 500);
                    }
                }
            }
        };
        doAjax(params);
    });

    $("body").on('click', '.plan-view', function () {
        var planId = $(this).data("id");
        var jsonData = JSON.parse($("#map_data_" + planId).val());
        quitModal = true;
        editable = false;

        //Draw the map
        $("#myModal").modal("show");

        listMarkers = [];
        initMaps("mapdetail", jsonData, 14);
        restorePlantrip(jsonData.data);

        //Show the map
        $(".btn-confirm-save").addClass("hidden");

    });
});

function afterGetSuggestedPlans(data) {
    var container = $("#SuggestedList");
    if (data !== "") {
        if (data.htmlReturn != "") {
            container.append(data.html);
            InitPlanDetail();
        }
        else {
            _hasNoData = true;
        }
    }

    UpdateSuggestedCounter();
    container.removeClass("hidden");
    container.find(".plantrip-overlay").remove();
}

function GetSuggestedPlans() {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/PlanTrip/GetSuggestedPlans';
    params['requestType'] = 'POST';
    params['data'] = { page: _currentPage, placeId: $("#CurrentPlace").val() };
    params['dataType'] = "json";

    params['successCallbackFunction'] = afterGetSuggestedPlans;
    doAjax(params);
}

function UpdateSuggestedCounter() {
    var container = $("#SuggestedCounter");
    var counter = 0;
    $("#SuggestedList").find(".item").each(function () {
        counter++;
    });

    if (counter > 0) {
        container.find(".total").html(counter);
        container.removeClass("hidden");
    } else {
        container.addClass("hidden");
    }
}

function afterGetCreatedPlans(data) {
    var container = $("#CreatedList");
    if (data !== "") {
        if (data.htmlReturn != "") {
            container.append(data.html);
            InitPlanDetail();
        }
        else {
            _hasNoData = true;
        }
    }

    UpdateCreatedCounter();
    container.removeClass("hidden");
    container.find(".plantrip-overlay").remove();
}

function UpdateCreatedCounter() {
    var container = $("#CreatedCounter");
    var counter = 0;
    $("#CreatedList").find(".item").each(function () {
        counter++;
    });

    if (counter > 0) {
        container.find(".total").html(counter);
        container.removeClass("hidden");
    } else {
        container.addClass("hidden");
    }
}

function GetCreatedPlans() {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/PlanTrip/GetCreatedPlans';
    params['requestType'] = 'POST';
    params['data'] = { page: _currentPage, placeId: $("#CurrentPlace").val() };
    params['dataType'] = "json";

    params['successCallbackFunction'] = afterGetCreatedPlans;
    doAjax(params);
}

//function InitMaps() {
//    var myOptions = {
//        zoom: 14,
//        center: new google.maps.LatLng(21.0431238, 105.8159647),
//        mapTypeId: google.maps.MapTypeId.ROADMAP
//    };

//    $(".maps").each(function () {
//        new google.maps.Map($(this), myOptions);
//    });
//}

function InitPlanDetail() {
    $(".Timeline").each(function () {
        var totalPlaces = 0;
        var remainPx = 0;
        var seperatorWidth = 0;
        var boxWidth = $(this).width();
        $(this).find(".mytrip").each(function () {
            totalPlaces++;
        });

        if (totalPlaces >= 6) {
            seperatorWidth = 120;
            $(this).css("width", seperatorWidth * (totalPlaces + 1));
            $(this).parent().addClass("pointer");
        } else {
            remainPx = boxWidth - 15;

            if (totalPlaces == 1) {
                seperatorWidth = remainPx / totalPlaces + 1;
            } else {
                seperatorWidth = remainPx / totalPlaces;
            }

        }  

        $(this).find(".trip-seperator").attr("width", seperatorWidth);

        var spwith = $(this).find(".trip-seperator").width();
        $(this).find(".trip-seperator").find("line").attr("x2", spwith);
        var exchange = 0;
        $(this).find(".mytripTime-even").each(function (e) {
            var elwith = $(this).innerWidth();
            if (elwith < 32) {
                exchange = 18;
            }
        });

        $(this).find(".mytripTime-even").each(function (e) {
            var elwith = $(this).innerWidth();
            $(this).css('margin-left', '-' + (((spwith + elwith + exchange) / 2)) + 'px');
        });
    });
}