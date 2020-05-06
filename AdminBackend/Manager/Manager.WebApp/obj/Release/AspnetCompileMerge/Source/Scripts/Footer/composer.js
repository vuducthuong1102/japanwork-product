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
    extended_valid_elements: "i[class|name|id],span",
    valid_children: "+body[style], +style[type]",
    apply_source_formatting: false,     
    verify_html : false,
    convert_urls: false,
    //browser_spellcheck: true,  
    //paste_as_text: true,  
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


//tinymce.init({
//    selector: '#T1',
//    language: 'ru',
//    schema: "html5",
//    entity_encoding: "raw",
//    statusbar: false,
//    paste_data_images: true,
//    theme: 'modern',
//    content_css: "/include/css/css.css",
//    media_strict: false,
//    image_advtab: true,
//    force_br_newlines: true,
//    force_p_newlines: false,
//    remove_linebreaks: false,
//    forced_root_block: false,
//    remove_trailing_nbsp: false,
//    cleanup_on_startup: false,
//    trim_span_elements: false,
//    cleanup: false,
//    convert_urls: false,
//    valid_elements: '*[*],',
//    extended_valid_elements: '*[*]',
//    valid_children: '*[*]',
//    verify_html: false,
//    apply_source_formatting: false,
//    plugins: [
//        'advlist autolink lists link image charmap print preview hr anchor pagebreak',
//        'searchreplace wordcount visualblocks visualchars code fullscreen',
//        'insertdatetime media nonbreaking save table contextmenu directionality',
//        'emoticons template paste textcolor colorpicker textpattern imagetools codesample toc help fullpage'
//    ],
//    toolbar1: 'undo redo | insert | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image',
//    toolbar2: 'print preview media | forecolor backcolor emoticons | codesample help fullpage code',
//    code_dialog_width: 1200,
//    code_dialog_height: 800,
//    width: '1200',
//    height: 800,
//    templates: [
//        { title: 'Test template 1', content: 'Test 1' },
//        { title: 'Test template 2', content: 'Test 2' }
//    ]
//});