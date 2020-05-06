var _window = $(window);
var _currentPage = 1;
var _hasNoData = false;

$.getScript("/Scripts/Post/post-events.js").done(function (script, textStatus) {
    $(function () {

        GetNewsFeedPosts(_currentPage);
        
        // Each time the user scrolls
        _window.scroll(function () {
            // End of the document reached?
            PagingByScroll();
        });

        $(document.body).on('touchmove', PagingByScroll); // for mobile

        InitSavePostEvent();     

    });
});

function PagingByScroll() {
    if ($(document).height() - _window.height() == _window.scrollTop()) {
        _currentPage++;

        if (!_hasNoData)
            GetNewsFeedPosts(_currentPage);
    }
}


