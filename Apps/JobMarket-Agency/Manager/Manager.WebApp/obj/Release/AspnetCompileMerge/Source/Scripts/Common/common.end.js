$(document).on("click", ".btn", function () {
    if (!$(this).hasClass("btn-error-permission")) {
        if ($(this).hasClass("btn-loading")) {
            return false;
        }
        if ($(this).attr("type") == "submit") {
            if ($(".field-validation-error").length == 0) {
                $(this).addClass("btn-loading").removeClass("btn-outline-info");
            }
        }
    }
})
setInterval(function () {
    if ($(".field-validation-error").length > 0) {
        $(".btn-loading").attr("type", "submit");
        $(".btn-loading").removeClass("btn-loading").addClass("btn-outline-info");
    }
    else {
        if ($(".btn-loading").length > 0) {
            $(this).attr("type", "button");
        }
    }
},1000)