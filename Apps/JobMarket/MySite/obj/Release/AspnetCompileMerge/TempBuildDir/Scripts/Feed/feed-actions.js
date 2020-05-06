import { Date } from "core-js/library/web/timers";

function AutoSetTime() {
    $(".livetimestamp").each(function () {
        SettingElement(this);
    });
}

function SettingElement(target) {
    var timestamp = $(target).data("utime");
    var start = new Date(timestamp * 1000);
    var currentDate = new Date.now();
    var minutes = Math.floor((currentDate - start) / 3600000);
    if (minutes < 71) {
        alert(1);
    }
}