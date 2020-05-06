//大きいサイズの導入事例_見出し
var $setElmCaseBigTitle = $('.top_case_main_case_content_wrap h4');
var cutFigureCaseBigTitle = '42'; // カットする文字数
var afterText = '…'; // 文字カット後に表示するテキスト


$setElmCaseBigTitle.each(function(){
  var textLength = $(this).text().trim().length;
  var textTrim = $(this).text().trim().substr(0,(cutFigureCaseBigTitle))

  if(cutFigureCaseBigTitle < textLength) {
    $(this).html(textTrim + afterText).css({visibility:'visible'});
  } else if(cutFigureCaseBigTitle >= textLength) {
    $(this).css({visibility:'visible'});
  }
});

//大きいサイズの導入事例_内容
var $setElmCaseBigText = $('.case_main_case_text');
var cutFigureCaseBigText = '72'; // カットする文字数
var afterText = '…'; // 文字カット後に表示するテキスト

$setElmCaseBigText.each(function(){
  var textLength = $(this).text().trim().length;
  var textTrim = $(this).text().trim().substr(0,(cutFigureCaseBigText))

  if(cutFigureCaseBigText < textLength) {
    $(this).html(textTrim + afterText).css({visibility:'visible'});
  } else if(cutFigureCaseBigText >= textLength) {
    $(this).css({visibility:'visible'});
  }
});

//小さいサイズの導入事例_見出し
var $setElmCaseSmallTitle = $('.case_good_point p');
var cutFigureCaseSmallTitle = '40'; // カットする文字数
var afterText = '…'; // 文字カット後に表示するテキスト

$setElmCaseSmallTitle.each(function(){
  var textLength = $(this).text().trim().length;
  var textTrim = $(this).text().trim().substr(0,(cutFigureCaseSmallTitle))

  if(cutFigureCaseSmallTitle < textLength) {
    $(this).html(textTrim + afterText).css({visibility:'visible'});
  } else if(cutFigureCaseSmallTitle >= textLength) {
    $(this).css({visibility:'visible'});
  }
});

//導入事例一覧 リードコピー
var $setElmCaseSmallTitle = $('.case_list_title');
var cutFigureCaseSmallTitle = '40'; // カットする文字数
var afterText = '…'; // 文字カット後に表示するテキスト


$setElmCaseSmallTitle.each(function(){
  var textLength = $(this).text().trim().length;
  var textTrim = $(this).text().trim().substr(0,(cutFigureCaseSmallTitle))

  if(cutFigureCaseSmallTitle < textLength) {
    $(this).html(textTrim + afterText).css({visibility:'visible'});
  } else if(cutFigureCaseSmallTitle >= textLength) {
    $(this).css({visibility:'visible'});
  }
});

//導入事例一覧 リードサマリー
var $setElmCaseSmallTitle = $('.case_list_description');
var cutFigureCaseSmallTitle = '52'; // カットする文字数
var afterText = '…'; // 文字カット後に表示するテキスト

$setElmCaseSmallTitle.each(function(){
  var textLength = $(this).text().trim().length;
  var textTrim = $(this).text().trim().substr(0,(cutFigureCaseSmallTitle))

  if(cutFigureCaseSmallTitle < textLength) {
    $(this).html(textTrim + afterText).css({visibility:'visible'});
  } else if(cutFigureCaseSmallTitle >= textLength) {
    $(this).css({visibility:'visible'});
  }
});

//導入事例一覧 サブコンテンツ リードコピー
var $setElmCaseSmallTitle = $('.case_list_title_small');
var cutFigureCaseSmallTitle = '40'; // カットする文字数
var afterText = '…'; // 文字カット後に表示するテキスト

$setElmCaseSmallTitle.each(function(){
  var textLength = $(this).text().trim().length;
  var textTrim = $(this).text().trim().substr(0,(cutFigureCaseSmallTitle))

  if(cutFigureCaseSmallTitle < textLength) {
    $(this).html(textTrim + afterText).css({visibility:'visible'});
  } else if(cutFigureCaseSmallTitle >= textLength) {
    $(this).css({visibility:'visible'});
  }
});

//導入事例一覧 サブコンテンツ リードサマリー
var $setElmCaseSmallTitle = $('.case_list_description_small');
var cutFigureCaseSmallTitle = '52'; // カットする文字数
var afterText = '…'; // 文字カット後に表示するテキスト

$setElmCaseSmallTitle.each(function(){
  var textLength = $(this).text().trim().length;
  var textTrim = $(this).text().trim().substr(0,(cutFigureCaseSmallTitle))

  if(cutFigureCaseSmallTitle < textLength) {
    $(this).html(textTrim + afterText).css({visibility:'visible'});
  } else if(cutFigureCaseSmallTitle >= textLength) {
    $(this).css({visibility:'visible'});
  }
});

//導入事例詳細 サブコンテンツ リードコピー
var $setElmCaseSmallTitle = $('.case_detail_sub_title p');
var cutFigureCaseSmallTitle = '42'; // カットする文字数
var afterText = '…'; // 文字カット後に表示するテキスト

$setElmCaseSmallTitle.each(function(){
  var textLength = $(this).text().trim().length;
  var textTrim = $(this).text().trim().substr(0,(cutFigureCaseSmallTitle))

  if(cutFigureCaseSmallTitle < textLength) {
    $(this).html(textTrim + afterText).css({visibility:'visible'});
  } else if(cutFigureCaseSmallTitle >= textLength) {
    $(this).css({visibility:'visible'});
  }
});
