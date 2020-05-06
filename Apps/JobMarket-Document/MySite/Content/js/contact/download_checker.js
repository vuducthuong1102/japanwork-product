$(function() {
  $(".contact_item_content").change(function() {
    const method_length = $(".method_item").find('input[type="checkbox"]:checked').length;
    const radio_length = $(".download_form_radio").find('input[type="radio"]:checked').length;
    const policy_checked = $("#privacy_policy:checked").val();
    if (method_length && radio_length && policy_checked) {
      $(".contact_submit_button").addClass("button_green");
      $(".contact_submit_button").removeClass("button_disable");
    } else {
      $(".contact_submit_button").addClass("button_disable");
      $(".contact_submit_button").removeClass("button_green");
    }
  });
});
