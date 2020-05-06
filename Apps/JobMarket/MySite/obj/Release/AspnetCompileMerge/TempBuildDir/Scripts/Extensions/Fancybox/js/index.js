// Create template for zoom button
//$.fancybox.defaults.btnTpl.zoom = '<button class="fancybox-button fancybox-zoom"><div class="zoom"><span class="zoom-inner"></span></div></button>';

// Choose what buttons to display by default
$.fancybox.defaults.buttons = [
  'slideShow',
  'fullScreen',
  'thumbs',
  //'zoom',
  'close'
];


$( '[data-fancybox]' ).fancybox({
  onInit : function( instance ) {

    // Make zoom icon clickable
    instance.$refs.toolbar.find('.fancybox-zoom').on('click', function() {
      if ( instance.isScaledDown() ) {
        instance.scaleToActual();

      } else {
        instance.scaleToFit();
      }
      
    });
  }
});

//$(function () {
//    console.log('%c' + LanguageDic['LB_WARING_STOP'] + '!', 'font-size: 50px; font-weight: bold; color: #F00; text-shadow: 1px 1px #000, -1px -1px #000, -1px 1px #000, 1px -1px #000');
//    var warn = LanguageDic['LB_WARNING_CONSOLE'];
//    console.log("%c" + warn, "font: 20px sans-serif; color: #3c3c3c;");
//})