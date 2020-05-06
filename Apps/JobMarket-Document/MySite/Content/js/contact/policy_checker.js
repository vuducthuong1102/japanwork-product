$(function() {
  $("#privacy_policy").change(function() {
    if ($(this).prop( "checked" )) {
      $(".contact_submit_button").addClass("button_green");
      $(".contact_submit_button").removeClass("button_disable");
    } else {
      $(".contact_submit_button").addClass("button_disable");
      $(".contact_submit_button").removeClass("button_green");
    }
  });
});
