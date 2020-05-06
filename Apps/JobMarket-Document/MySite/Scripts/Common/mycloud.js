$(function () {
    //setTimeout(function () {

    //}, 2000);


});

// Declare a proxy to reference the hub.
var commonGateHub = $.connection.commonGateHub;
$.connection.hub.url = _myCloudServer + "/signalr/hubs/";
$.connection.hub.start().done(function () {
    console.log('Connected to MyCloud server successfully');
    registerGateHubEvents(commonGateHub);
}).fail(function (error) {
    console.log("Cannot connect to MyCloud server because: " + error);
});

//Register all client methods
registerGateHubClientMethods(commonGateHub);

function registerGateHubEvents(commonGateHub) {
    commonGateHub.server.connect(_currentUserInfo);

    $("#testbutton").click(function () {
        commonGateHub.server.testMessage("Demo message ok");
    });
}

function registerGateHubClientMethods(commonGateHub) {
    // Calls when user successfully logged in
    commonGateHub.client.onConnected = function (connectionId) {
        //console.log("Your current ConnectionId: " + connectionId);
    }

    // On New User Connected
    commonGateHub.client.onNewUserConnected = function (id, name) {
        //console.log(name + " connected to MyCloud. ConnectionId: " + id);
    }

    // On User Disconnected
    commonGateHub.client.onUserDisconnected = function (id, userName) {

    }

    // On received new notif
    commonGateHub.client.updateNotification = function (notifInfo) {
        var counter = $(".count-noti");
        var currentVal = parseInt(counter.html().replace("+", ""));

        $(".box-noti").removeClass("loaded");
        if (counter.hasClass("hidden")) {
            currentVal = 1;
            counter.html(currentVal);
            counter.removeClass("hidden");
            return false;
        } else {
            if (currentVal <= 99) {
                currentVal++;
            } else {
                currentVal = 1;
            }
            counter.html(currentVal);
        }
    }

    //On received new friend notif 
    commonGateHub.client.updateFriendNotif = function (notifInfo) {
        var counter = $(".count-friend-noti");
        var count = counter.data("count");

        $(".request-friend-list").removeClass("loaded");
        RenderFriend(notifInfo.UserId, notifInfo.ActorId, notifInfo.ObjectId, notifInfo.ActionType);
        if (notifInfo.ActionType != 'NOT_ACCEPT_FRIEND' && notifInfo.ActionType != 'REMOVE_FRIEND') {
            if (notifInfo.ActionType == 'REMOVE_NOTIF') {
                if (count > 0) {
                    count--;
                }
            }
            else {
                count++;
            }
            UpdateNotifFriend(count);
        }
    }

    // On receivedMessage
    commonGateHub.client.receivedMessage = function (message) {
        //alert(message);
    }

    function UpdateNotifFriend(count) {
        var target = $(".count-friend-noti");
        target.data("count", count);
        target.html(count);
        if (count > 0) {
            if (count > 99) {
                target.html("99+");
            }
            target.removeClass("hidden");
        }
        else {
            target.addClass("hidden");
        }
    }

    function RenderFriend(UserId,ActorId, RequestId, ActionType) {
        if ($(".friend-render").length) {
            var params = $.extend({}, doAjax_params_default);
            params['url'] = '/Friend/RenderFriend';
            params['requestType'] = 'GET';
            params['data'] = { RequestId: RequestId, UserId: UserId, ActorId: ActorId, ActionType: ActionType };
            params['dataType'] = "json";

            params['successCallbackFunction'] = afterRenderFriend;
            doAjax(params);
        }
    }

    function afterRenderFriend(result) {
        if (result) {
            if (result.success) {
                $(".friend-render").html(result.data);
            }
        }
    }
}