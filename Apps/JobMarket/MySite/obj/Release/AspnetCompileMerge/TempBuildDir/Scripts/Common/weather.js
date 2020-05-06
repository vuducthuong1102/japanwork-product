var clock = 0;
var interval_msec = 1000;

$(document).ready(function () {
    GetWeather();

    //$("body").on('click', '.search-weather', function () {
    //    GetWeather();
    //});
});

// UpdateClock
function UpdateClock() {

    // clear timer
    clearTimeout(clock);

    var dt_now = new Date();
    var hh = dt_now.getHours();
    var mm = dt_now.getMinutes();
    var ss = dt_now.getSeconds();

    if (hh < 10) {
        hh = "0" + hh;
    }
    if (mm < 10) {
        mm = "0" + mm;
    }
    if (ss < 10) {
        ss = "0" + ss;
    }
    $("#myclock").html(hh + ":" + mm + ":" + ss);

    // set timer
    clock = setTimeout("UpdateClock()", interval_msec);

}

function afterGetWeather(data) {
    var container = $(".weather");
    if (data) {
        if (data.success) {
            container.html(data.html);
            if (data.html !== "")
                container.fadeIn().removeClass("hidden");
            else
                container.addClass("hidden");
            $("#weather-details").addClass($(container).data("collapse"));
        }
        container.parent().find(".widget-overlay").remove();

        var clock = 0;
        var interval_msec = 1000;

        // ready
        clock = setTimeout("UpdateClock()", interval_msec);       
    }
}

function GetWeather() {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Home/GetWeather';
    params['requestType'] = 'POST';
    params['data'] = { city: $("#txtCity").val() };
    params['dataType'] = "json";

    params['successCallbackFunction'] = afterGetWeather;
    doAjax(params);
}


