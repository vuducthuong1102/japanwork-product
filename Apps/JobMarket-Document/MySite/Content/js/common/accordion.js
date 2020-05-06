$(".accordion_trigger").on("click", function() {
  $(this).next().slideToggle();
  $(this).toggleClass("active");
});
