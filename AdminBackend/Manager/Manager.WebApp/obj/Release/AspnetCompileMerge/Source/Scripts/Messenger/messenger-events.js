if ($(".friends-list").length) {
    //new PerfectScrollbar('.friends-list');
}

$(function () {
    LoadMoreContacts();    

    $('.friends-list').on('scroll', function () {
        if ($(this).scrollTop() + $(this).innerHeight() >= $(this)[0].scrollHeight) {
            LoadMoreContacts();
        }
    });
});

$("body").on("click", ".cr-private-msg", function () {
    OpenConversation($(this),true);
});

$('body').on('click', '.ui-chat-box-close,.ui-minifer-close', function () {
    var currentBoxId = $(this).data('userid');
    $("#private_box_" + currentBoxId).addClass("hidden");
    $("#minifier-item-" + currentBoxId).addClass("hidden");

    //$("#private_box_" + currentBoxId).remove();
    //$("#minifier-item-" + currentBoxId).remove();
    close_popup(currentBoxId);

});

$('body').on('click', '.minifier-item', function () {
    OpenConversation($(this), true);
});

$('body').on('click', '.ui-chat-box-head', function (e) {
    if (e.target !== this)
        return;
    $(this).closest('.ui-chat-box').toggleClass('minimize');
    var boxMsg = $(this).closest('.ui-chat-box').find('.ui-chat-box-msg');
    var height = boxMsg[0].scrollHeight;
    boxMsg.scrollTop(height);
});
/*---------khi noi dung o nhap text - ui-chat-box-input thay doi chieu cao thì thay doi lại o  ui-chat-box-msg*/
var mresize = function (selector, callback) {
    [].forEach.call($(selector), function (el) {
        el.mr = [el.offsetWidth, el.offsetHeight];
        el.insertAdjacentHTML("beforeend", "<div class='mresize' style='position:absolute;width:auto;height:auto;top:0;right:0;bottom:0;left:0;margin:0;padding:0;overflow:hidden;visibility:hidden;z-index:-1'><iframe style='width:100%;height:0;border:0;visibility:visible;margin:0'></iframe><iframe style='width:0;height:100%;border:0;visibility:visible;margin:0'></iframe></div>");
        [].forEach.call(el.querySelectorAll(".mresize iframe"), function (frame) {
            (frame.contentWindow || frame).onresize = function () {
                if (el.mr[0] !== el.offsetWidth || el.mr[1] !== el.offsetHeight) {
                    if (callback) callback.call(el);
                    el.mr[0] = el.offsetWidth; el.mr[1] = el.offsetHeight;
                }
            }
        });
    });
}
var resize = new mresize($('.ui-chat-box-input'), function () {
    var height = $(this).outerHeight(true);
    var heightContent = $(this).closest('.ui-chat-box').find('.ui-chat-box-content').outerHeight(true);
    $(this).closest('.ui-chat-box').find('.ui-chat-box-msg').height(heightContent - 20 - height);
});

//this function can remove a array element.
Array.remove = function (array, from, to) {
    var rest = array.slice((to || from) + 1 || array.length);
    array.length = from < 0 ? array.length + from : from;
    return array.push.apply(array, rest);
};

//this variable represents the total number of popups can be displayed according to the viewport width
var total_popups = 0;
//arrays of popups ids
var popups = [];

//this is used to close a popup
function close_popup(id) {
    for (var iii = 0; iii < popups.length; iii++) {
        if (id === popups[iii]) {
            Array.remove(popups, iii);
            calculate_popups();

            return;
        }
    }
}

//creates markup for a new popup. Adds the id to popups array.
function register_popup(id, name) {

    for (var iii = 0; iii < popups.length; iii++) {
        //already registered. Bring it to front.
        if (id === popups[iii]) {
            Array.remove(popups, iii);

            popups.unshift(id);

            calculate_popups();

            active_popup(id);

            return;
        }
    }

    var element = OpenConversation(id);
}

//displays the popups. Displays based on the maximum number of popups that can be displayed on the current viewport width
function display_popups() {
    var iii = 0;
    for (iii; iii < total_popups; iii++) {
        if (popups[iii] !== undefined) {
            $("#private_box_" + popups[iii]).removeClass("hidden");
            $("#contact-" + popups[iii]).removeClass("minified");  

            if ($("#minifier-item-" + popups[iii]).length > 0) {
                $("#minifier-item-" + popups[iii]).remove();
                continue;
            }
        }
    }

    for (var jjj = iii; jjj < popups.length; jjj++) {      
        $("#private_box_" + popups[iii]).addClass("hidden");        
        $("#contact-" + popups[iii]).addClass("minified");       
        if ($("#minifier-item-" + popups[iii]).length <= 0) {
            $(".boxes-hiding").prepend("<li id='minifier-item-" + popups[iii] + "'>" + $(".contact-minifier-" + popups[iii]).html() + "</li>");
            continue;
        }
    }

    if ($(".boxes-hiding").is(':empty')) {
        $(".more-chat-box").addClass("hidden");
    } else {
        $(".more-chat-box").removeClass("hidden");
    }
}

//calculate the total number of popups suitable and then populate the toatal_popups variable.
function calculate_popups() {
    var width = window.innerWidth;
    if (width < 300) {
        total_popups = 0;
    }
    else {
        width = width - 93;
        //300 is width of a single popup box
        total_popups = parseInt(width / 300);
    }

    display_popups();    
}

//recalculate when window is loaded and also when window is resized.
window.addEventListener("resize", calculate_popups);
window.addEventListener("load", calculate_popups);

function afterGetFriendLists(data) {
    var container = $(".friends-list");
    if (data) {
        if (data.success) {
            container.html(data.html);
            if (data.html !== "")
                $(".contact").fadeIn().removeClass("hidden");
            else
                $(".contact").addClass("hidden");
        }
        $(".contact").parent().find(".widget-overlay").remove();
    }
}

function GetFriendLists() {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Messenger/GetFriendLists';
    params['requestType'] = 'GET';
    params['data'] = {};
    params['dataType'] = "json";

    params['successCallbackFunction'] = afterGetFriendLists;
    doAjax(params);
}

function ScrollToBottom(element) {
    element.scrollTop(element[0].scrollHeight);
}

function HideTheLastBox() {
    $(".ui-chat-box").each(function () {
        if (!$(this).hasClass("hidden")) {
            var lastItemId = $(this).data("userid");
            alert(lastItemId);
           

            return;
        }
    });
}

function OpenConversation(target, isRestore, userId = 0) {
    var id = userId;
    if (target) {
        id = target.data("userid");
    }

    var boxChat = $("#private_box_" + id);

    if (boxChat.length > 0) {
        boxChat.removeClass('minimize');
        boxChat.find(".msg2send").focus();

        if (isRestore) {
            MinifyChatBox(id);
        }

        return false;
    }

    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Messenger/OpenConversation';
    params['requestType'] = 'POST';
    params['data'] = { receiver: id };
    params['dataType'] = "json";
    params['context'] = target;
    params['showloading'] = false;

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
            calculate_popups();

            if (isRestore) {
                MinifyChatBox(id);
            }

        }
    };
    doAjax(params);
}

function MinifyChatBox(id) {
    var idfinal = 0;
    $(".ui-chat-box").each(function () {
        if (!$(this).hasClass("hidden")) {
            idfinal = $(this).data("userid");
        }
    });
    if (idfinal !== id && $("#private_box_" + id).hasClass("hidden")) {
        $("#private_box_" + idfinal).addClass("hidden");
        $("#private_box_" + id).removeClass("hidden");

        if ($("#minifier-item-" + idfinal).length <= 0) {
            $(".boxes-hiding").prepend("<li id='minifier-item-" + idfinal + "'>" + $(".contact-minifier-" + idfinal).html() + "</li>");
            $("#minifier-item-" + id).remove();
        } 
    }
      
}

function GetConversation(receiver) {
    var pageNum = $("#private_box_" + receiver).data("current");
    if (pageNum === null || pageNum === 0 || pageNum === 'undefined') {
        pageNum = 1;
        $("#private_box_" + receiver).attr("data-current", pageNum);
    }

    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Messenger/GetCurrentConversation';
    params['requestType'] = 'POST';
    params['data'] = { receiver: receiver };
    params['dataType'] = "json";
    params['showloading'] = false;

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

                $(msgList.parent()).scroll(function () {
                    if (msgList.parent()[0].scrollTop === 0) {

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
    var containerBox = $("#private_box_" + userId);

    if (containerBox.data("finished") === true) {
        return false;
    }

    var box = containerBox.find(".ui-chat-box-content");
    var msgList = box.find(".ui-chat-msglist");

    box.find(".msgloader").removeClass("hidden");    

    var pageCounter = $("#private_box_msg_idx_" + userId);
    var idxPage = parseInt(pageCounter.val());
    if (isNaN(idxPage))
        idxPage = 1;
    idxPage = idxPage + 1;

    pageCounter.val(idxPage);

    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Messenger/LoadMoreMessages';
    params['requestType'] = 'POST';
    params['data'] = { page: idxPage, receiver: userId };
    params['dataType'] = "json";
    params['showloading'] = false;

    params['successCallbackFunction'] = function (data) {
        if (data) {
            if (!data.success) {
                bootbox.showmessage({
                    message: data.message
                }, data.success);

                return false;
            } else {
                if (data.html) {
                    msgList.parent().scrollTop(20);
                    msgList.prepend(data.html);
                } else {
                    containerBox.attr("data-finished", true);
                }
            }
        }

        setTimeout(function () {
            box.find(".msgloader").addClass("hidden");
        }, 500);
    };
    doAjax(params);
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

function LoadMoreContacts() {
    var contactBox = $(".friends-list");
    if (contactBox.data("finished") === true) {
        return false;
    }

    var currentPage = parseInt(contactBox.data("current"));
    if (isNaN(currentPage)) {
        currentPage = 0;
    }
    currentPage = currentPage + 1;
    contactBox.data("current", currentPage);

    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Messenger/GetOperatorsList';
    params['requestType'] = 'POST';
    params['data'] = { page: contactBox.data("current") };
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
                    contactBox.append(data.html);
                } else {
                    contactBox.attr("data-finished", true);
                }
            }

            if (currentPage === 1) {
                if (data.html !== "")
                    $(".list-contact").fadeIn().removeClass("hidden");
                else
                    $(".list-contact").addClass("hidden");

                $(".list-contact").parent().find(".widget-overlay").remove();
            }
        }
    };
    
    doAjax(params);
}

