var loadedGoogleMap = function(){
  function initialize() {
    var myLatlng = new google.maps.LatLng(35.665909, 139.746793); //マーカーの位置（＝所在地）
    var mapOptions = {
      center: myLatlng,
      zoom: 16,
    };
    var map = new google.maps.Map(document.getElementById('company_access_map'),mapOptions);
    var marker = new google.maps.Marker({
      position: myLatlng,
      map: map,
      title: '株式会社ブレイン・ラボ',
      icon: 'https://www.matchingood.co.jp/resource/img/icon/bl_map_maker.png'
    });
  }
  google.maps.event.addDomListener(window, 'load', initialize);
};
