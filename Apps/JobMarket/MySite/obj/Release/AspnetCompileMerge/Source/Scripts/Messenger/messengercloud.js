$(function () {   
    // Declare a proxy to reference the hub.
    messengerHub = $.connection.messengerHub;
    $.connection.hub.url = _myCloudServer + "/signalr/hubs/";
    $.connection.hub.start().done(function () {
        console.log('Connected to Messenger server successfully');
        registerMessengerEvents(messengerHub);

    }).fail(function (error) {
        console.log("Cannot connect to MyCloud server because: " + error);
    });

    //Register all client methods
    registerMessengerClientMethods(messengerHub);    
});


function registerMessengerEvents(messengerHub) {   
    messengerHub.server.connect(_currentUserInfo);
}

function registerMessengerClientMethods(messengerHub) {
    // Calls when user successfully logged in
    //messengerHub.client.onMessengerConnected = function (allUsers) {
    //    //setScreen(true);
    //    for (i = 0; i < allUsers.length; i++) {
    //        AddUser(allUsers[i]);
    //    }
    //}

    // On New User Connected
    messengerHub.client.onMessengerNewUserConnected = function (id, newUserInfo) {       
        //AddUser(newUserInfo);
        var userItem = $("#contact-" + newUserInfo.UserId);
        if (userItem != null) {
            userItem.find(".online").removeClass("hidden");
            userItem.find(".online-text").html(LanguageDic["LB_ONLINE"]);
            userItem.find(".online-text").attr("data-utime", "");
            userItem.find(".online-text").removeClass("livetimestamp-title");
            //$(".box-online-" + newUserInfo.Id).removeClass("hidden");
        }
    }

    // On User Disconnected
    messengerHub.client.onMessengerUserDisconnected = function (id, userInfo) {
        $(".online-user").each(function () {
            if ($(this).data("id") == userInfo.UserId) {
                $(this).remove();
            }
        });
    }

    //User offline
    messengerHub.client.onMessengerUserOffline = function (userId) {
        var userItem = $("#contact-" + userId);
        if (userItem != null) {
            userItem.find(".online").addClass("hidden");
            userItem.find(".online-text").html("");
            userItem.find(".online-text").addClass("livetimestamp-title");
            var ts = Math.round(new Date().getTime() / 1000);
            userItem.find(".online-text").attr("data-utime", ts);
            //$(".box-online-" + newUserInfo.Id).addClass("hidden");
        }
    }

    messengerHub.client.onMessengerUserReconnected = function (userId) {
        var userItem = $("#contact-" + userId);
        if (userItem != null) {
            userItem.find(".online").removeClass("hidden");
            userItem.find(".online-text").html(LanguageDic["LB_ONLINE"]);
            userItem.find(".online-text").removeClass("livetimestamp-title");
            //$(".box-online-" + newUserInfo.Id).removeClass("hidden");
        }
    }

    messengerHub.client.messageReceived = function (msgItem) {
        AddNewMessageGuest(msgItem);
    }

    messengerHub.client.sendPrivateMessage = function (toUserId, msgItem) {        
        AddNewMessageMine(toUserId, msgItem);
    }

    messengerHub.client.userIsTyping = function (fromUser) {
        if (fromUser != null) {
            var currentWindow = $("#private_box_" + fromUser.UserId);           
            if (currentWindow.length > 0) {
                var typingIcon = currentWindow.find(".user-typing");
                typingIcon.removeClass("hidden");
                setTimeout(function () {
                    typingIcon.addClass("hidden");
                }, 5000);
            }
        }        
    }

    messengerHub.client.messageTypingDone = function (fromUser) {
        if (fromUser != null) {
            var currentWindow = $("#private_box_" + fromUser.UserId);
            if (currentWindow.length > 0) {
                var typingIcon = currentWindow.find(".user-typing");
                typingIcon.addClass("hidden");
            }
        }

    }
}

function AddNewMessageGuest(msgItem) {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Messenger/NewMessage';
    params['requestType'] = 'POST';
    params['data'] = { msgItem: msgItem };
    params['dataType'] = "json";

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
                        OpenConversation(null,true,msgItem.UserId);
                    }
                }
            }
        }
    };
    doAjax(params);
}

function AddNewMessageMine(toUserId, msgItem) {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Messenger/NewMessage';
    params['requestType'] = 'POST';
    params['data'] = { msgItem: msgItem };
    params['dataType'] = "json";

    params['successCallbackFunction'] = function (data) {
        if (data) {
            if (!data.success) {
                bootbox.showmessage({
                    message: data.message
                }, data.success);
            } else {
                if (data.html) {
                    var box = $("#private_box_" + toUserId).find(".ui-chat-box-content");
                    var msgList = box.find(".ui-chat-msglist");
                    if ($("#private_box_" + toUserId).length > 0) {
                        msgList.append(data.html);
                        ScrollToBottom(msgList.parent());
                    } else {
                        OpenConversation(null, true, toUserId);
                    }
                }
            }
        }
    };
    doAjax(params);
}