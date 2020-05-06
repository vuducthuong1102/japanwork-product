$(function () {

});

var MyCloudGlobal = {
    
};

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
}

function registerGateHubClientMethods(commonGateHub) {
    // On User Disconnected
    commonGateHub.client.onUserDisconnected = function (id, userName) {

    };

    // On received new notif
    commonGateHub.client.updateNotification = function (notifId) {
        AuthenticatedGlobal.getNewNotification(notifId);

        AuthenticatedGlobal.counterUpNotification();
    };  
}