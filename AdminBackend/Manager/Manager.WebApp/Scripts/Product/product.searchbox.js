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

$("#ddlProperty").select2({
    placeholder: "----" + LanguageDic['LB_SELECT_PROPERTY'] + "----"
});

$("#frmSearch").submit(function () {
    var selMulti = $.map($("#ddlProperty option:selected"), function (el, i) {
        return $(el).val();
    });
    $("#PropertyList").val(selMulti.join(","));
});