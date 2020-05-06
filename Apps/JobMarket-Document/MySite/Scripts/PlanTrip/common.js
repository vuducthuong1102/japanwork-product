// dữ liệu mẫu
var _placesByProvince = jQuery.parseJSON('{"data":[{"id":"20092","title":"Thanh Miện","rate":"3.5","category":"eat","urlViewListPost":"http://localhost:55460/ListPost","lat":"20.7821204","lng":"106.2203815","urlViewDestination":"http://localhost:55460/Destination","urlCreatPlanTrip":"http://localhost:55460/Destination","img":"http://45.32.99.190:7171/Content/ShowImage?url=/Trips/24/11-05-2018/abc.jpg","tripPost":[{"title":"Ghềnh đá đĩa","subTitle":"Ghềnh đá đĩa - Phú Yên","time":"11/06/2018 3:46:30 CH","url":"http://localhost:1991"},{"title":"Post02 - Hang Dơi, Mộc Châu, Sơn La","subTitle":"Sub text title","time":"2:40 AM - 4 Apr 2018","url":"http://haloplan.com/"}]}]}');
var quitModal = false; // user chọn close popup
var editable = false;

$('#myModal').on('shown.bs.modal', function () {
    //quitModal = false;
    //initMaps(_placesByProvince);
});

// khi tắt modal sẽ hỏi có quit hay save ko
$('#myModal').on('hide.bs.modal', function (event) {
    // nếu đang chọn dở hành trình thì hỏi xem có tắt hay ko
    if (!quitModal & getPlantrip().length > 0) {
        $('.name-trip.box-alert').hide();
        $('.quit-trip.box-alert').fadeIn();
        event.preventDefault();
        event.stopPropagation();
    }
    else { // khi chon tat modal thi reset lai
        resetAllPlantrip();
        $('.plantrip-content .contents').html('');
    }
});
// click btn save plantrip - trả mảng json theo thứ tự đi
$('body').on('click', '#save-plan', function () {
    $('.name-trip.box-alert').fadeIn();
    console.log(getPlantrip());
})
// Tắt form điền tên plan trip
$('body').on('click', '.name-trip.box-alert .btn', function () {
    $('.name-trip.box-alert').fadeOut();
})
//
$('body').on('click', '.quit-trip.box-alert .cancel', function () {
    $('.quit-trip.box-alert').fadeOut();
    quitModal = true;
    $('#myModal').modal('hide');
})
$('body').on('click', '.quit-trip.box-alert .save', function () {
    $('.quit-trip.box-alert').hide();
    $('.name-trip.box-alert').fadeIn();
})
// lọc theo category
$('body').on('click', '.plantrip-maps .category li>a', function () {
    filterByCategory($(this).data('category'));
    $(this).closest('.category').find('.active').removeClass('active');
    $(this).closest('li').addClass('active');
})
// chọn các chức năng trong box info trên bản đồ
$('body').on('click', '.maps-infobox .select-func', function () {
    selectFunction($(this));
})

$('body').on('click', '.btn-confirm-save', function () {
    BeginSavePlan();
});

$('body').on('click', '.btn-save', function () {
    SavePlan();
    if ($(this).data("reload") == true) {
        setTimeout(function () {
            location.reload();
        }, 2000);
    }    
});

function BeginSavePlan() {
    var totalPlaces = 0;
    $(".place-item").each(function () {
        totalPlaces++;
    });
    if (totalPlaces <= 1) {
        bootbox.showmessage({
            message: LanguageDic["LB_PLANTRIP_MINIMUM"]
        }, false);

        return false;
    }

    $('.name-trip.box-alert').fadeIn();
}

function SavePlan() {
    var planArr = [];
    $(".place-item").each(function () {
        var item = $(this);

        var detail = item.find(".place-item-detail");
        var id = detail.data("id");
        var distance = item.find(".distance").html();

        var placeData = {
            id: id,
            distance: distance,
            isstart: item.hasClass("item-start"),
            isend: item.hasClass("item-end")
        };

        planArr.push(placeData);
    });

    if (planArr.length <= 0) {
        bootbox.showmessage({
            message: LanguageDic["LB_PLANTRIP_NULL"]
        }, false);
        return false;
    }

    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/PlanTrip/SavePlan';
    params['requestType'] = 'POST';
    params['data'] = { planTripName: $("#PlanTripName").val().trim(), planTrips: planArr, currentPlace: $("#CurrentPlace").val() };
    params['context'] = $(this);
    params['dataType'] = "json";

    params['successCallbackFunction'] = function (result) {
        if (result) {
            if (result.success) {
                quitModal = true;
                $('#myModal').modal("hide");
                resetAllPlantrip();
                $("#PlanTripName").val("");
                $('.plantrip-content .contents').html('');
            }

            bootbox.showmessage({
                message: result.message
            }, result.success);
        }
    };

    doAjax(params);
}
