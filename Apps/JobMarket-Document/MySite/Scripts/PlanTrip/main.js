var _window = $(window);
var _currentPage = 1;
var _hasNoData = false;

$(function () {     

    GetPlacesList(_currentPage);

    // Each time the user scrolls
    _window.scroll(function () {
        // End of the document reached?
        if ($(document).height() - _window.height() == _window.scrollTop()) {
            _currentPage++;

            if (!_hasNoData)
                GetPlacesList(_currentPage);
        }
    });

    $("#frmPlanPlaceSearch").submit(function () {
        var container = $("#PlacesList");
        container.html("");
        _currentPage = 1;
        GetPlacesList(_currentPage);  
        return false;
    });

    $("body").on('click', '.btn-create-plan', function () {
        $(".trips-post").html("");
        var params = $.extend({}, doAjax_params_default);
        var currentPlace = $(this).data('id');
        params['url'] = '/PlanTrip/GetPlacesByParent';
        params['requestType'] = 'POST';
        params['data'] = { placeId: currentPlace};
        params['context'] = $(this);
        params['dataType'] = "json";

        params['successCallbackFunction'] = function (result) {
            if (result) {
                if (result.success) {
                    if (result.data) {           

                        $('#myModal').modal("show");
                        $("#CurrentPlace").val(currentPlace);                        
                        setTimeout(function () {
                            quitModal = false;
                            initMaps(result.data);                                 
                        }, 500)                       
                    }
                    //console.log(result);
                    if (result.data.groups_html) {
                        $("#PlaceTypeGroups").html(result.data.groups_html);
                        $("#AllTypeGroups").html("(" + result.data.totalplaces + ")");
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
});

function afterGetPlacesList(data) {
    var container = $("#PlacesList");
    if (data !== "" && data) {
        if (data.success) {
            if (data.html) {             
                container.append(data.html);
            }
        }
        else {
            _hasNoData = true;
        }
    }
    
    container.removeClass("hidden");
    $(".plantrip-overlay").remove();
}

function GetPlacesList() {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/PlanTrip/GetPlacesByPage';
    params['requestType'] = 'POST';
    params['data'] = { page: _currentPage, keyword: $("#txtSearchPlace").val() };
    params['dataType'] = "json";

    params['successCallbackFunction'] = afterGetPlacesList;
    doAjax(params);
}


