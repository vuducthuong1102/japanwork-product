var userAgent = navigator.userAgent;
if (userAgent.indexOf('iPhone') > 0 || userAgent.indexOf('Android') > 0 || userAgent.indexOf('iPad') > 0 || userAgent.indexOf('Windows Phone') > 0){
  $('.global_navi > li').on('click', function(){
    var get_hide_class = $(this).children('.header_hidden_navi_wrap').hasClass('hide_slide_menu');
    $('.header_hidden_navi_wrap').addClass('hide_slide_menu');
    if(get_hide_class){
      $(this).children('.header_hidden_navi_wrap').removeClass('hide_slide_menu');
    }
  });

  //SP メニュー外タップでサブメニューを閉じる
  $('.container_wrap').on('click', function(event){
    if(!$(event.target).closest('.global_navi').length){
      $('.global_navi > li').children('.header_hidden_navi_wrap').addClass('hide_slide_menu');
    }
  });
} else {
  //PC ホバーで開閉
  $('.global_navi > li').hover(function(){
    $(this).children('.header_hidden_navi_wrap').removeClass('hide_slide_menu');
  }, function (){
    $(this).children('.header_hidden_navi_wrap').addClass('hide_slide_menu');
  });
}

//PC SP スクロールでサブメニューを閉じる
$(window).scroll(function(){
  $('.global_navi > li').children('.header_hidden_navi_wrap').addClass('hide_slide_menu');
});
