$(function () {
    // Declare a proxy to reference the hub.
    var messengerHub = $.connection.messengerHub;
    $.connection.hub.url = _myCloudServer + "/signalr/hubs/";
    $.connection.hub.start().done(function () {
        console.log('Connected to MyCloud server successfully');
        registerGateHubEvents(messengerHub);
    }).fail(function (error) {
        console.log("Cannot connect to MyCloud server because: " + error);
    });

    //Register all client methods
    registerGateHubClientMethods(messengerHub);
});

function registerGateHubEvents(messengerHub) {
    messengerHub.server.connect(_currentUserInfo);
}

function registerGateHubClientMethods(messengerHub) {
    // Calls when user successfully logged in
    messengerHub.client.onConnected = function (connectionId) {
        //console.log("Your current ConnectionId: " + connectionId);
    }

    // On New User Connected
    messengerHub.client.onNewUserConnected = function (id, name) {
        //console.log(name + " connected to MyCloud. ConnectionId: " + id);
    }

    // On User Disconnected
    messengerHub.client.onUserDisconnected = function (id, userName) {

    }

    // On New User Connected
    messengerHub.client.updateNotification = function () {
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

}