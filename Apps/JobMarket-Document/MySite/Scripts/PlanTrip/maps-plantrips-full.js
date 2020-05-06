var listMarkers = [];
var listPolyLine = [];
var map, poly;
var directionsService = new google.maps.DirectionsService();
var directionsDisplay = new google.maps.DirectionsRenderer();

function initMaps(json) {
    var zoom = 14;
    var mapCenter = new google.maps.LatLng(json.data[0].lat, json.data[0].lng);
    map = new google.maps.Map(document.getElementById('mapindex'), {
        zoom: zoom,
        center: mapCenter,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        clickableIcons: false
    });
    google.maps.event.addListener(map, 'zoom_changed', function () {
        var zoomLevel = map.getZoom();
        if (14 > zoomLevel) {
            $(".maps-marker").addClass("off");
        }
        else {
            $(".maps-marker").removeClass("off");
        }
    });
    poly = new google.maps.Polyline({
        geodesic: true,
        strokeColor: '#456aaf',
        strokeOpacity: 0.8,
        strokeWeight: 3,
        map: map,
        path: []
    });
    directionsDisplay.setMap(map);
    for (var i = 0; i < json.data.length; i++) {
        var infobox = new CustomMarker({
            position: mapCenter,
            map: map,
            position: new google.maps.LatLng(json.data[i].lat, json.data[i].lng),
            draggable: false,
            flat: true,
            visible: false,
            content: generateInfobox(json.data[i]),
            data: json.data[i]
        });
        var marker = new CustomMarker({
            map: map,
            position: new google.maps.LatLng(json.data[i].lat, json.data[i].lng),
            draggable: false,
            flat: true,
            anchor: CustomMarkerPosition.MIDDLE,
            content: GetMarkerDataHtml(json.data[i]),
            data: json.data[i]
        });
        listMarkers.push({
            key: json.data[i].id,
            marker: marker,
            infobox: infobox
        })
        google.maps.event.addListener(marker, 'click', function (marker, i) {
            var id = $(this)[0].data.id;
            hideAllInfoboxOther(id);
            var infobox = getInfoboxById(id);
            infobox.setVisible(!infobox.getVisible());
            showTripPost(infobox.data.tripPost, $('.plantrip-maps .left-col .maps .trips-post'));            
        });
        // show luon cacs bai viet ve dia diem dau tien
        if (i == 0)
            showTripPost(json.data[i].tripPost, $('.plantrip-maps .left-col .maps .trips-post'));
    }
}

function GetMarkerDataHtml(data) {
    var groupCode = 'all';
    if (data.group_code)
        groupCode = data.group_code;

    var styleTxt = '';
    if (data.img)
        styleTxt = 'background: url(' + data.img + ') no-repeat center center;background-size: contain;';
    var html = '<div style="' + styleTxt +'" class="maps-marker ' + data.category + ' ' + '" data-place="' + data.id + '" data-groupcode="'+ data.group_code +'"><p><span>' + data.title + '</span></p></div>';

    return html;
}

function showTripPost(data, el) {
    if (data) {
        $(el).hide();
        var html = '<div class="content">';
        for (var i = 0; i < data.length; i++) {
            html += '<div class="item"><a href="' + data[i].url + '"><p>' + data[i].title + '</p><p>' + data[i].subTitle + '</p><p>' + data[i].time + '</p></a></div>';
        }
        html += '</div>';
        $(el).html(html);
        $(el).find('.content').slick();
        setTimeout(function () { $(el).show(); }, 100);
    }
}
function getInfoboxById(id) {
    for (var i = 0; i < listMarkers.length; i++) {
        if (listMarkers[i].key == id) {
            return listMarkers[i].infobox;
        }
    }
}
function generateInfobox(data) {
    var html = '<div class="maps-infobox plantrip"><div class="checkbox add-sign"><input type="checkbox" class="select-func"  id="add_' + data.id + '" data-type="add" data-id="' + data.id + '" /><label for="add_' + data.id + '"> ' + LanguageDic['LB_ADD_SIGN_PLAN'] + '</label></div><div class="checkbox start-sign"><input type="checkbox" class="select-func"  id="start_' + data.id + '" data-type="start" data-id="' + data.id + '" /><label for="start_' + data.id + '">' + LanguageDic['LB_START_PLAN'] + '</label></div><div class="checkbox remove-sign-dln"><input type="checkbox" class="select-func"  id="remove_' + data.id + '" data-type="remove" data-id="' + data.id + '" /><label for="remove_' + data.id + '">' + LanguageDic['LB_REMOVE_SIGN_PLAN'] + '</label></div></div >'
    return html;
}
function getMarkerById(id) {
    for (var i = 0; i < listMarkers.length; i++) {
        if (listMarkers[i].key == id) {
            return listMarkers[i].marker;
        }
    }
}
function hideAllInfoboxOther(id) {
    for (var i = 0; i < listMarkers.length; i++) {
        if (listMarkers[i].key != id) {
            listMarkers[i].infobox.setVisible(false);
        }
    }
}
function selectFunction(el) {
    var id = $(el).data('id');
    var infobox = getInfoboxById(id);
    infobox.setVisible(!infobox.getVisible());
    var type = $(el).data('type');
    var data = infobox.data;
    if (type == 'add') {
        if (!checkExist(listPolyLine, id)) {
            listPolyLine.push({ id: id, value: new google.maps.LatLng(data.lat, data.lng), data: data });
            infobox.setContent(infobox.content.replace('add-sign', 'add-sign-dln').replace('remove-sign-dln', 'remove-sign'));
        }
    }
    else if (type == 'start') {
        if (!checkExist(listPolyLine, id))
            listPolyLine.unshift({ id: id, value: new google.maps.LatLng(data.lat, data.lng), data: data });
        else
            listPolyLine = moveToTop(listPolyLine, id)
        infobox.setContent(infobox.content.replace('add-sign-dln', 'add-sign').replace('add-sign', 'add-sign-dln').replace('remove-sign-dln', 'remove-sign'));
    }
    else if (type == 'remove') {
        listPolyLine = $.grep(listPolyLine, function (item, i) {
            return (item.id !== id);
        });
        var marker = getMarkerById(id);
        marker.setContent(GetMarkerDataHtml(marker.data));
        infobox.setContent(infobox.content.replace('add-sign-dln', 'add-sign').replace('remove-sign', 'remove-sign-dln'));
    }
    poly.setPath(getListLatlng(listPolyLine));
    if (listPolyLine.length > 0) {
        showInfoPlan(listPolyLine, $('.plantrip-content .contents'));
    }
}
function getListLatlng(array) {
    var arr = [];
    for (var i = 0; i < array.length; i++) {
        arr.push(array[i].value);
    }
    return arr;
}
function showInfoPlan(array, el) {
    var html = '';
    var arraybind = [];

    html += '<p class="text-bold start-plan place-item">' + LanguageDic['LB_START_PLAN'] + ': <span class="asi">' + array[0].data.title + '</span></p>';
    for (var i = 0; i < array.length; i++) {
        var data = array[i].data;
        var itemDetalHtml = '<span class="place-item-detail" data-id="' + data.id + '" data-lat="' + data.lat + '" data-long="' + data.lng + '" data-title="' + data.title + '"></span>';
        var startclass = "item-start";
        var classNumber = '';
        if (i != 0) {
            itemDetalHtml = '<span class="place-item-detail" data-id="' + data.id + '" data-lat="' + data.lat + '" data-long="' + data.lng + '" data-title="' + data.title + '"><span class="asi">' + data.title + '</span></span>';
            startclass = "";
            classNumber = '<i class="no">' + (i + 1) + '</i>';
        }

        if (i == array.length - 1) {
            if (i != 0) {
                html += '<p class="item-end place-item end-plan"><span class="text-bold">' + LanguageDic['LB_END_PLAN'] + ':</span> ' + itemDetalHtml + '</p>';
            }
        }
        else {
            var datanext = array[i + 1].data;
            var classid = data.id + '-' + datanext.id;
            var value = $("." + classid).text();
            if (value == "" || value == "undefined") {
                value = $("." + datanext.id + "-" + data.id).text();
                if (value == "" || value == "undefined") {
                    arraybind.push(array[i]);
                    arraybind.push(array[i + 1]);
                    value = "";
                }
            }
            html += '<p class="item  place-item ' + startclass + '" data-index="' + i + '">' + itemDetalHtml + classNumber + '<i class="distance ' + classid + '">' + value + '</i></p>';
        }

        var marker = getMarkerById(array[i].id);
        marker.setContent(GetMarkerDataHtml(data));
    }
    $(el).html(html);

    if (arraybind.length > 0) {
        updateDistance(arraybind, el);
    }
}

function GetDistance(id1, id2, request) {
    var distance = 0;
    directionsService.route(request, function (response, status) {
        if (status == google.maps.DirectionsStatus.OK) {
            var distance = 0;
            var myroute = response.routes[0];
            for (var j = 0; j < myroute.legs.length; j++) {
                distance += myroute.legs[j].distance.value;
            }
            var distance = (distance / 1000).toFixed(1);
            $('.' + id1 + "-" + id2).text(distance + ' km ');

        } else {
            console.log('Failed: GetDistance');
        }
    });
}

function updateDistance(array, el) {
    if (array.length > 1) {
        for (var id = 0; id < array.length - 1; id++) {
            var currentIndex = id;
            var request = {
                origin: new google.maps.LatLng(array[id].data.lat, array[id].data.lng),
                destination: new google.maps.LatLng(array[id + 1].data.lat, array[id + 1].data.lng),
                travelMode: google.maps.DirectionsTravelMode.WALKING  //BICYCLING //DRIVING //WALKING 
            };

            GetDistance(array[id].data.id, array[id + 1].data.id, request);
        }
    }
}
function moveToTop(array, id) {
    var firstItem = array[0];
    for (var i = 0; i < array.length; i++) {
        if (array[i].id == id) {
            array[0] = array[i];
            array[i] = firstItem;
        }
    }
    return array;
}
function checkExist(array, id) {
    for (var i = 0; i < array.length; i++) {
        if (array[i].id == id)
            return true;
    }
    return false;
}
function getPlantrip() {
    var arr = [];
    for (var i = 0; i < listPolyLine.length; i++) {
        arr.push(listPolyLine[i].data);
    }
    return arr;
}
function resetAllPlantrip() {
    listPolyLine = [];
    map = [];
    poly = [];
    listMarkers = [];
}
function filterByCategory(category) {
    //alert(123);
    //for (var i = 0; i < listMarkers.length; i++) {
    //    var marker = listMarkers[i].marker;
    //    if (category == 'all')
    //        marker.setVisible(true);
    //    else {
    //        var categoryMarker = marker.data.category;
    //        if (category == categoryMarker)
    //            marker.setVisible(true);
    //        else
    //            marker.setVisible(false);
    //    }
    //}
    if (category == 'all') {
        $(".maps-marker").show();
    } else {
        $(".maps-marker").hide();
        $(".maps-marker").each(function () {
            if ($(this).data("groupcode") == category) {
                $(this).show();
            }
        });
    }    
}
function restorePlantrip(json) {
    for (var i = 0; i < json.length; i++) {
        listPolyLine.push({ id: json[i].id, value: new google.maps.LatLng(json[i].lat, json[i].lng), data: json[i] });
    }
    poly.setPath(getListLatlng(listPolyLine));
    showInfoPlan(listPolyLine, $('.plantrip-content .contents'));
}