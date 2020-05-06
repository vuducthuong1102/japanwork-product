$(function(){
  $(".case_narrow_down_box").on("click", function() {
    $(this).toggleClass("active");
    $(".case_narrow_down").slideToggle("fast");
  });
});
