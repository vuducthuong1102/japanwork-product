function RemoveOverLay(container) {
    //$('.lazy').lazy();
    setTimeout(function () {
        container.parent().find(".post-overlay").remove();
        container.parent().find(".widget-overlay").remove();
        container.removeClass("hidden").fadeIn();
    }, 500);

    $("a.post-viewmore").click(function (e) {
        e.stopPropagation();
    });
}