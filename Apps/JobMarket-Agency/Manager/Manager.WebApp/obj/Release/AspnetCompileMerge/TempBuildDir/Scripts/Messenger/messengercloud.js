var messengerHub = $.connection.messengerHub;

$.connection.hub.url = _myCloudServer + "/signalr/hubs/";
$.connection.hub.start().done(function () {
    console.log('Connected to Messenger server successfully');
    registerMessengerEvents(messengerHub);


}).fail(function (error) {
    console.log("Cannot connect to MyCloud server because: " + error);
});

//Register all client methods
registerMessengerClientMethods();

function registerMessengerEvents(messengerHub) {   
    messengerHub.server.connect(_uInfo);
}

function registerMessengerClientMethods() {
    // On New User Connected
    messengerHub.client.newUserConnected = function (newUserInfo) {
        console.log(newUserInfo.ConnectionId);        
    };

    // On User Disconnected
    //messengerHub.client.onMessengerUserDisconnected = function (id, userInfo) {
    //    $(".online-user").each(function () {
    //        if ($(this).data("id") === userInfo.id) {
    //            $(this).remove();
    //        }
    //    });
    //};

    ////User offline
    //messengerHub.client.onMessengerUserOffline = function (userId) {
    //    var userItem = $("#contact-" + userId);
    //    if (userItem !== null) {
    //        userItem.find(".online").addClass("hidden");
    //        userItem.find(".online-text").html("");
    //        userItem.find(".online-text").addClass("livetimestamp-title");
    //        var ts = Math.round(new Date().getTime() / 1000);
    //        userItem.find(".online-text").attr("data-utime", ts);
    //        //$(".box-online-" + newUserInfo.Id).addClass("hidden");
    //    }
    //};

    //messengerHub.client.onMessengerUserReconnected = function (userId) {
    //    var userItem = $("#contact-" + userId);
    //    if (userItem !== null) {
    //        userItem.find(".online").removeClass("hidden");
    //        userItem.find(".online-text").html(LanguageDic["LB_ONLINE"]);
    //        userItem.find(".online-text").removeClass("livetimestamp-title");
    //        //$(".box-online-" + newUserInfo.Id).removeClass("hidden");
    //    }
    //};

    messengerHub.client.messageReceived = function (msgItem) {
        console.log(msgItem);
        AddNewMessageGuest(msgItem);
    };

    messengerHub.client.sendPrivateMessage = function (msgItem) {
        AddNewMessageMine(msgItem);
    };

    messengerHub.client.userIsTyping = function (fromUser) {
        if (fromUser !== null) {
            var currentWindow = $("#private_box_" + fromUser.id);
            if (currentWindow.length > 0) {
                if (currentWindow.data("t") === fromUser.t) {
                    var typingIcon = currentWindow.find(".user-typing");
                    typingIcon.removeClass("hidden");
                    //ScrollToBottom(currentWindow.find(".ui-chat-box-msg"));

                    setTimeout(function () {
                        typingIcon.addClass("hidden");
                    }, 5000);
                }                
            }
        }
    };

    messengerHub.client.messageTypingDone = function (fromUser) {
        if (fromUser !== null) {
            var currentWindow = $("#private_box_" + fromUser.id);
            if (currentWindow.length > 0) {
                var typingIcon = currentWindow.find(".user-typing");
                typingIcon.addClass("hidden");
            }
        }

    };
}

function CreateChatBox(id, html) {
    var typingSent = false;
    $("#ui-chat-container").append(html);

    popups.push(id);

    initGetTextInContentEditale();

    $(document).on('keypress', ".msg2send", function (e) {
        var target = $(this);
        var targetId = target.parent().data("userid");
        var t = $("#private_box_" + id).data("t");
        if (e.which === 13) {
            TypingMessageDone($("#CurrentUser").val(), targetId, target);
            e.preventDefault();
            typingSent = false;
            messengerHub.server.messageTypingDone(_uInfo, targetId, t);
        } else {
            if (target.html() !== '') {
                if (typingSent === false) {
                    messengerHub.server.userIsTyping(_uInfo, targetId, t);
                    typingSent = true;
                }
            }

            setTimeout(function () {
                typingSent = false;
            }, 5000);
        }
    });

    // Initializes and creates emoji set from sprite sheet
    window.emojiPicker = new EmojiPicker({
        emojiable_selector: '[data-emojiable=true]',
        assetsPath: '../Content/Extensions/emoji/lib/img/',
        popupButtonClasses: 'fa fa-smile-o'
    });
    // Finds all elements with `emojiable_selector` and converts them to rich emoji input fields
    // You may want to delay this step if you have dynamically created input fields that appear later in the loading process
    // It can be called as many times as necessary; previously converted input fields will not be converted again
    window.emojiPicker.discover();

    //Get messages
    GetConversation(id);
}

function SendMessageToUser(fromId, toId, message) {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Messenger/Send';
    params['requestType'] = 'POST';
    params['data'] = { receiver: toId, message: message };
    params['dataType'] = "json";
    params['showloading'] = false;

    params['successCallbackFunction'] = function (data) {
        if (data) {
            if (!data.success) {
                bootbox.showmessage({
                    message: data.message
                }, data.success);
            } else {
                if (data.msgItem) {
                    messengerHub.server.sendPrivateMessage(data.msgItem);
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

        SendMessageToUser(fromId, toId, msg);
    }
}

function AddNewMessageGuest(msgItem) {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Messenger/NewMessage';
    params['requestType'] = 'POST';
    params['data'] = { msgItem: msgItem };
    params['dataType'] = "json";
    params['showloading'] = false;

    params['successCallbackFunction'] = function (data) {
        if (data) {
            if (!data.success) {
                bootbox.showmessage({
                    message: data.message
                }, data.success);
            } else {
                if (data.html) {
                    var box = $("#private_box_" + msgItem.UserId).find(".ui-chat-box-content");
                    var msgList = box.find(".ui-chat-msglist");
                    if ($("#private_box_" + msgItem.UserId).length > 0) {
                        msgList.append(data.html);
                        ScrollToBottom(msgList.parent());
                    } else {
                        OpenConversation(null, true, msgItem.UserId);
                    }
                }
            }
        }
    };
    doAjax(params);
}

function AddNewMessageMine(msgItem) {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Messenger/NewMessage';
    params['requestType'] = 'POST';
    params['data'] = { msgItem: msgItem };
    params['dataType'] = "json";
    params['showloading'] = false;

    params['successCallbackFunction'] = function (data) {
        if (data) {
            if (!data.success) {
                bootbox.showmessage({
                    message: data.message
                }, data.success);
            } else {
                if (data.html) {
                    var box = $("#private_box_" + msgItem.TargetId).find(".ui-chat-box-content");
                    var msgList = box.find(".ui-chat-msglist");
                    if ($("#private_box_" + msgItem.TargetId).length > 0) {
                        msgList.append(data.html);
                        ScrollToBottom(msgList.parent());
                    } else {
                        OpenConversation(null, true, msgItem.TargetId);
                    }
                }
            }
        }
    };
    doAjax(params);
}