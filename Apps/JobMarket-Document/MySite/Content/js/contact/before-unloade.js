$(window).ready(function(){
  $('input, textarea, select').on('keyup change', function(){
    $(window).on('beforeunload', function() {
      return "行った変更が保存されない可能性があります。";
    });
  })

  $('button[type=submit]').on('click', function(e) {
    $(window).off('beforeunload');
  });
});
