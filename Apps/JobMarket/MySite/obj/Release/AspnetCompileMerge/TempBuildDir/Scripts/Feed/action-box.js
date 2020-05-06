function AddNewPlace(placeData) {
    $.post("/home/addplace", { inputModel: JSON.stringify(placeData) })
        .done(function (result) {
            if (result) {
                if (result.success) {
                    setTimeout(function () {
                        $(".place-item").each(function () {
                            if ($(this).data("place_id") === placeData.place_id) {
                                $(this).attr("data-id", result.data);
                            }
                        });
                    }, 500);
                }
            }
        });
}

function generateLocation(place) {
    var model = { place_id: place.place_id, name: place.name, formatted_address: place.formatted_address, url: place.url };
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Render/RenderLocation/';
    params['requestType'] = 'POST';
    params['data'] = { model: model };
    params['successCallbackFunction'] = function (data) {
        if (data) {
            if (data.success) {
                $("#FeedLocations").append(data.html);
                AddNewPlace(place);
            }
        }
    };

    doAjax(params);
}

function initLocationAutoComplete() {
    setTimeout(function () {
        // Create the search box and link it to the UI element.
        var input = document.getElementById('SearchLocation');
        var searchBox = new google.maps.places.SearchBox(input);
        searchBox.addListener('places_changed', function () {
            var places = searchBox.getPlaces();
            if (places.length == 0) {
                return;
            }
            var lat = places[0].geometry.location.lat();
            var lng = places[0].geometry.location.lng();
            $("#FeedLocations").children().show();

            var text = $("#SearchLocation").val();

            var isDuplicated = false;
            $(".place-item").each(function () {
                var placeid = $(this).data("place_id");
                if (placeid === places[0].place_id) {
                    isDuplicated = true;
                }
            });

            if (!isDuplicated) {
                generateLocation(places[0]);
            }
            //console.log(places[0]);

            $("#SearchLocation").val("");
        });
    }, 1000);
}
