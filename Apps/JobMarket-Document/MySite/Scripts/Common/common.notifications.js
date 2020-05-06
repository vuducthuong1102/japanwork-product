(function ($) {

    $.showErrorMessage = function (title, text, afterCloseEvent) {
        //swal({
        //    title: "<div class='text-danger'>" + title + "</div>",
        //    //text: text,
        //    html: "<div class='text-danger'>" + text + "</div>",
        //    type: "error",
        //    confirmButtonText: "OK",
        //    reverseButtons: !0
        //}).then(function (e) {
        //    if (e.value) {
        //        if (afterCloseEvent && (typeof afterCloseEvent == "function")) {
        //            afterCloseEvent();
        //        }
        //    }

        //});

        Swal.fire({
            title: "<div class='sw-alert-title text-danger'>" + title + "</div>",
            html: "<div class='sw-alert-content text-danger'>" + text + "</div>",
            type: 'error',
            confirmButtonText: "OK",
            showCancelButton: false,
            confirmButtonColor: '#17a2b8'
            //cancelButtonColor: '#d33'
        }).then((result) => {
            if (result.value) {
                if (afterCloseEvent && (typeof afterCloseEvent === "function")) {
                    afterCloseEvent();
                } else {
                    eval(afterCloseEvent);
                }
            }
        });

        return false;
    };


    $.showSuccessMessage = function (title, text, afterCloseEvent) {        
        Swal.fire({
            title: "<div class='sw-alert-title'>" + title + "</div>",
            html: "<div class='sw-alert-content'>" + text + "</div>",
            type: 'success',
            confirmButtonText: "OK",
            showCancelButton: false,
            reverseButtons: !0,
            confirmButtonColor: '#17a2b8',
            cancelButtonColor: '#d33'
        }).then((result) => {
            if (result.value) {
                if (afterCloseEvent && (typeof afterCloseEvent === "function")) {
                    afterCloseEvent();
                }
                else {
                    eval(afterCloseEvent);
                }
            }
        });

        return false;
    };
})(jQuery);