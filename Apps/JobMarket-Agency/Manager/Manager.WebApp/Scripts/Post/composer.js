tinymce.init({
    selector: "#BodyContent", height: 450,
    plugins: [
        "advlist autolink link image lists charmap print preview hr anchor pagebreak code",
        "searchreplace wordcount visualblocks visualchars insertdatetime media nonbreaking",
        "table contextmenu directionality emoticons paste textcolor responsivefilemanager"
    ],
    toolbar1: "undo redo | sizeselect | fontselect |  fontsizeselect | bold italic underline | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | styleselect",
    toolbar2: "| responsivefilemanager | link unlink anchor | image media | forecolor backcolor  | print preview code ",
    image_advtab: true,
    //filemanager_crossdomain: true,
    //external_filemanager_path: _fileManagerUrl + "/",
    //external_plugins: { "filemanager": _fileManagerUrl + "/plugin.min.js" },
    external_filemanager_path: "/filemanager/",
    external_plugins: { "filemanager": "/filemanager/plugin.min.js" },
    filemanager_title: "File Manager",    
    entity_encoding: "raw",
    extended_valid_elements: "i[class|name|id]",
    valid_children: "+body[style], +style[type]",
    apply_source_formatting: false,       
});

//tinymce.init({
//    selector: "#BodyContent", height: 450,
//    theme: "silver",
//    height: 450,
//    plugins: [
//        "advlist autolink link image lists charmap print preview hr anchor pagebreak",
//        "searchreplace wordcount visualblocks visualchars insertdatetime media nonbreaking",
//        "table contextmenu directionality emoticons paste textcolor filemanager code responsivefilemanager"
//    ],
//    toolbar1: "undo redo | sizeselect | fontselect |  fontsizeselect | bold italic underline | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | styleselect",
//    toolbar2: "| responsivefilemanager | link unlink anchor | image media | forecolor backcolor  | print preview code ",
//    image_advtab: true,
//    external_filemanager_path: _fileManagerUrl + "/",
//    external_filemanager_path: "/filemanager/",
//    filemanager_title: "Responsive Filemanager",
//    external_plugins: {
//        "filemanager": "/filemanager/plugin.min.js",
//        "responsivefilemanager": "../../tinymce/plugins/responsivefilemanager/plugin.min.js",
//    },
//});

$("#Title").bind("input", function () {    
    $("#UrlFriendly").val(slug($('#Title').val()));
});