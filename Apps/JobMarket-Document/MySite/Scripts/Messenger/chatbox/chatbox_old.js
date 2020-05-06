
function ScrollToBottom(element) {
    element.scrollTop(element[0].scrollHeight);
}

function OpenConversation(id) {
    var boxChat = $("#private_box_" + id);
    if (boxChat.length > 0) {
        boxChat.removeClass('minimize');
        boxChat.find(".msg2send").focus();

        return false;
    } 
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Messenger/OpenConversation';
    params['requestType'] = 'POST';
    params['data'] = { receiver: id };
    params['dataType'] = "json";

    params['successCallbackFunction'] = function (data) {
        if (data) {
            if (!data.success) {
                bootbox.showmessage({
                    message: data.message
                }, data.success);

                return false;
            } else {
                CreateChatBox(id, data.html);                                 
            }
        }
    };
    doAjax(params);
}

function GetConversation(receiver) {
    var pageNum = $("#private_box_" + receiver).data("current");
    if (pageNum == null || pageNum == 0 || pageNum == 'undefined') {
        pageNum = 1;
        $("#private_box_" + receiver).attr("data-current", pageNum);
    }

    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Messenger/GetCurrentConversation';
    params['requestType'] = 'POST';
    params['data'] = { receiver: receiver };
    params['dataType'] = "json";

    params['successCallbackFunction'] = function (data) {
        if (data) {
            if (!data.success) {
                bootbox.showmessage({
                    message: data.message
                }, data.success);

                return false;
            } else {
                var box = $("#private_box_" + receiver).find(".ui-chat-box-content");
                var msgList = box.find(".ui-chat-msglist");
                msgList.html(data.html);
                box.find(".msgloader").addClass("hidden");

                ScrollToBottom(msgList.parent());

                $(msgList).scroll(function () {
                    if (msgList[0].scrollTop == 0) {
                        
                        LoadMoreMessages(receiver);
                    }
                });

                $("#private_box_" + receiver).find(".msg2send").focus();
            }
        }
    };
    doAjax(params);
}

function LoadMoreMessages(userId) {
    if ($("#private_box_" + userId).data("finished") == true) {        
        return false;
    }
    var box = $("#private_box_" + userId).find(".ui-chat-box-content");
    var msgList = box.find(".ui-chat-msglist");
    box.find(".msgloader").removeClass("hidden");

    var currentPage = parseInt($("#private_box_" + userId).data("current"));
    currentPage = currentPage + 1;
    $("#private_box_" + userId).data("current", currentPage);

    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Messenger/LoadMoreMessages';
    params['requestType'] = 'POST';
    params['data'] = { page: $("#private_box_" + userId).data("current"), receiver: userId };
    params['dataType'] = "json";

    params['successCallbackFunction'] = function (data) {
        if (data) {
            if (!data.success) {
                bootbox.showmessage({
                    message: data.message
                }, data.success);

                return false;
            } else {
                if (data.html) {        
                    msgList.scrollTop(50);
                    msgList.prepend(data.html);
                } else {
                    $("#private_box_" + userId).attr("data-finished", true);
                }
            }
        }
       
        box.find(".msgloader").addClass("hidden");
    };
    doAjax(params);
}


function CreateChatBox(id, html) {
    var typingSent = false;
    $("#ui-chat-container").append(html);
    $("#private_box_" + id).find(".msg2send").attr("data-id", id);

    initGetTextInContentEditale();
    $(document).on('keypress', ".msg2send", function (e) {
        var target = $(this);
        if (e.which == 13) {
            TypingMessageDone($("#CurrentUser").val(), target.data("id"), target);
            e.preventDefault();
            typingSent = false;
            messengerHub.server.messageTypingDone($("#CurrentUser").val(), target.data("id"));
        } else {
            if (target.html() != '') {
                if (typingSent == false) {
                    messengerHub.server.userIsTyping($("#CurrentUser").val(), target.data("id"));
                    typingSent = true;
                }
            }

            setTimeout(function () {
                typingSent = false;
            }, 5000);
        }
    });

    //Get messages
    GetConversation(id);
}

function SendMessageToUser(fromId, toId, message) {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Messenger/Send';
    params['requestType'] = 'POST';
    params['data'] = { receiver: toId, message: message };
    params['dataType'] = "json";

    params['successCallbackFunction'] = function (data) {
        if (data) {
            if (!data.success) {
                bootbox.showmessage({
                    message: data.message
                }, data.success);
            } else {
                if (data.msgItem) {
                    messengerHub.server.sendPrivateMessage(fromId, toId, data.msgItem);
                }
            }            
        }
    };
    doAjax(params);
}

function TypingMessageDone(fromId, toId, target) {
    var msg = target.html();
    if (msg.length > 0) {
        target.empty();   
        target.focus();

        //Begin send message
        SendMessageToUser(fromId, toId, msg);
    }
}

function RegisterBoxButtonsEvents() {
    //$(".chat-header-button").click(function (ev) {
    //    ev.stopImmediatePropagation();
    //    var action = $(this).data("control");
    //    if (action == "fvideo")
    //        FacetimeVideoCall();
    //    else if (action == "earphone")
    //        VoiceCall();
    //    else if (action == "attachment")
    //        Attachment(this);
    //    else if (action == "remove") {
    //        var parentBox = $(this).closest(".popup-box-on");
    //        var toId = parentBox.find("div[contenteditable]").data("id");
    //        close_popup(toId);
    //        parentBox.remove();
    //    }
           
    //});

    //$(".attach-control").click(function (ev) {
    //    ev.stopImmediatePropagation();
    //    var action = $(this).data("control");
    //    if (action == "fvideo")
    //        FacetimeAttach();
    //});

}

function FacetimeVideoCall() {
    alert('Video call');
}

function VoiceCall() {
    alert('Voice call');
}

function FacetimeAttach() {
    alert('Video call attachment');
}

function Attachment(selector) {
    var container = $(selector).parent();
    var myBox = container.parents().eq(2);
    if (myBox.hasClass("normal")) {
        $(".gurdeepoushan").removeClass("open");
        if (container.hasClass("open"))
            container.removeClass("open");
        else
            container.addClass("open");
    }
}

function DoChatBoxEvents(selector) {
    //For show/hide
    ShowHideChatBox(selector);

    //Bla bla bla
}

function ShowHideChatBox(selector) {
    var boxParent = selector.closest(".popup-box-on");
    var myParent = selector.parent();
    var chatboxContent = myParent.find(".chat_box_wrapper");
    var chatboxInputArea = myParent.find(".chat_submit_box");
    var chatHeader = myParent.find(".popup-head");

    var defaultHeight = $("#chatbox_temp").height();
    var afterMinizeHeight = chatHeader.height();

    if (myParent.hasClass('mini')) {
        myParent.removeClass('mini').addClass('normal');
        myParent.height(400);
        boxParent.width(284);
        boxParent.find(".mini-hidden").removeClass("hidden");
        chatboxContent.show();
        chatboxInputArea.show();
    }
    else {
        myParent.removeClass('normal').addClass('mini');
        chatboxContent.hide();
        chatboxInputArea.hide();
        myParent.height(afterMinizeHeight);
        boxParent.width(205);
        boxParent.find(".mini-hidden").addClass("hidden");
    }
}
