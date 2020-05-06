$(".selectpicker").selectpicker();


$("#frmChoosenProduct").on("submit", function () {
    ChoosenProductSearch();
    return false;
});

function ChoosenProductSearchNext(pageNum) {
    $("#CurrentPage").val(pageNum)
    ChoosenProductSearch();
}

function ChoosenProductSearch() {
    var selMulti = $.map($("#ddlProperty option:selected"), function (el, i) {
        return $(el).val();
    });
    $("#PropertyList").val(selMulti.join(","));

    var frmData = $("#frmChoosenProduct").serialize();
    var params = $.extend({}, doAjax_params_default);
    params['url'] = "/Product/ChoosenProductSearch";
    params['requestType'] = "POST";
    params['data'] = frmData;
    params['dataType'] = "html";

    params['successCallbackFunction'] = function (data) {
        $("#ProductChoosenList").html(data);
    };

    doAjax(params);
}

function AllowChoosenProduct() {
    if ($("#CallbackFunction").val() != "") {
        eval($("#CallbackFunction").val() + "()");
    } else {
        alert("Please define CallbackFunction");
    }
}

$("#PropertyCategoryId").change(function () {
    $("#ddlProperty").html("");
    var params = $.extend({}, doAjax_params_default);
    params['url'] = "/Product/ChoosenProperty";
    params['requestType'] = "POST";
    params['data'] = { PropertyCategoryId: $(this).val() };
    params['dataType'] = "html";

    params['successCallbackFunction'] = function (data) {
        $("#ddlProperty").html(data);
        $("#ddlProperty").selectpicker('refresh');
    };

    doAjax(params);
});