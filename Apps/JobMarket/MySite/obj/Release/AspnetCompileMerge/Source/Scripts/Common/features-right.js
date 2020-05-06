$.getScript("/Scripts/Common/weather.js");

$.getScript("/Scripts/Post/post-events.js").done(function (script, textStatus) {
    $(function () {

        GetRecentPosts();

        GetTopTralveller();

        GetFeedCounter();

        //window.onscroll = function () { StickyMyContent() };
    });
});
