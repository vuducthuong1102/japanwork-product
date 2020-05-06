$('.top_download_success_document_popup').css('display','none');

$('.top_download_success_document_button_wrap button').on('click', function(){
  $(this).next().toggle();
});

$(document).click(function(event) {
  if(!$(event.target).closest('[class^="top_download_success_document_"]').length) {
    $('.top_download_success_document_popup').css('display','none');
  }
});

$('.download_validation').css('display','none');

$('.top_download_success_document_popup').children("input[type='submit']").on('click', function() {
  const mailAddress = $('.top_download_success_document_popup').children("input[type='text']").val();
  const regexp = /^[A-Za-z0-9]+[\w-\.]+@[\w\.-]+\.\w{2,}$/;

  if (regexp.test(mailAddress)) {
    return true;
  }

  const errorMessage = mailAddress === '' ?
    'メールアドレスが未入力です。' : 'メールアドレスの形式が正しくありません。';

  $('.download_validation').css('display','');
  $('.download_validation').text(errorMessage);
  return false;
});

$('.top_download_success_document_popup').children("input[type='text']").on('click', function(event) {
  $('.download_validation').css('display','none');
});

if (getParam('result') === 'success') {
  window.confirm('入力されたE-mailアドレスにメールを送信いたしました。\nメールを確認して、ダウンロードしてください。');
}

function getParam(name, url) {
  if (!url) url = window.location.href;
  name = name.replace(/[\[\]]/g, "\\$&");
  var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
      results = regex.exec(url);
  if (!results) return null;
  if (!results[2]) return '';
  return decodeURIComponent(results[2].replace(/\+/g, " "));
}
