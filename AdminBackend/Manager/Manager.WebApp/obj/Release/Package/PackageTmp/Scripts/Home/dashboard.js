var DashboardGlobal = {
    getStatisticsByYear: function () {
        GetAndDrawChartData();
    },
    getStatisticsByMonth: function () {
        GetAndDrawWeekChartData();
    }
};

$(function () {
    setTimeout(function () {
        DashboardGlobal.getStatisticsByYear();
    }, 2000);   

    setTimeout(function () {
        DashboardGlobal.getStatisticsByMonth();
    }, 1000);    
});

function GetAndDrawChartData() {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = 'Home/DashBoardStatistics';
    params['requestType'] = 'POST';
    params['showloading'] = false;
    params['data'] = {};
    params['dataType'] = "json";
    params['successCallbackFunction'] = function (result) {
        if (result) {
            //Chart by year
            DrawYearChart(result);
        }
    };
    doAjax(params);
}

function GetAndDrawWeekChartData() {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = 'Home/DashBoardStatisticsByWeek';
    params['requestType'] = 'POST';
    params['showloading'] = false;
    params['data'] = {};
    params['dataType'] = "json";
    params['successCallbackFunction'] = function (result) {
        if (result) {
            //Chart by week
            DrawWeekLineChart(result);
        }
    };
    doAjax(params);
}

function DrawYearChart(rootData) {
    var MONTHS = [
        LanguageDic["LB_MONTH_1"],
        LanguageDic["LB_MONTH_2"],
        LanguageDic["LB_MONTH_3"],
        LanguageDic["LB_MONTH_4"],
        LanguageDic["LB_MONTH_5"],
        LanguageDic["LB_MONTH_6"],
        LanguageDic["LB_MONTH_7"],
        LanguageDic["LB_MONTH_8"],
        LanguageDic["LB_MONTH_9"],
        LanguageDic["LB_MONTH_10"],
        LanguageDic["LB_MONTH_11"],
        LanguageDic["LB_MONTH_12"]
    ];

    var color = Chart.helpers.color;
    Chart.defaults.global.animation.duration = 3000;
    //var note = LanguageDic['LB_UPDATE'] + ': UPDATED_AT, ' + LanguageDic['LB_UPDATE_NEXT_TIME'] + ': UPDATE_NEXT';
    //note = note.replace('UPDATED_AT', rootData.UpdatedTime).replace('UPDATE_NEXT', rootData.NextUpdate);
    var note = LanguageDic['LB_DATA_DAILY_UPDATE_AUTO'];

    var barChartData = {
        labels: MONTHS,
        datasets: [{
            label: LanguageDic['LB_JOB_STATUS_PUBLIC'],
            backgroundColor: color(window.chartColors.blue).alpha(0.5).rgbString(),
            borderColor: window.chartColors.blue,
            borderWidth: 1,
            data: rootData.InYearData.successData
        }
            , {
            label: LanguageDic['LB_JOB_STATUS_PROCESSING'],
            backgroundColor: color(window.chartColors.orange).alpha(0.5).rgbString(),
            borderColor: window.chartColors.orange,
            borderWidth: 1,
            data: rootData.InYearData.processingData
        }
            , {
            label: LanguageDic['LB_JOB_STATUS_UNPROCESSED'],
            backgroundColor: color(window.chartColors.red).alpha(0.5).rgbString(),
            borderColor: window.chartColors.red,
            borderWidth: 1,
            data: rootData.InYearData.failedData,
            //hidden: true,
        }
        ]

    };
    var ctx = document.getElementById("canvas").getContext("2d");

    $("#YearStatisticsChart").fadeIn(1000);

    window.myBar = new Chart(ctx, {
        type: 'bar',
        data: barChartData,
        options: {
            responsive: true,
            maintainAspectRatio: false,
            legend: {
                position: 'top',
            },
            title: {
                display: true,
                text: LanguageDic['LB_STATISTICS_JOB_BY_MONTH']
            },
            scales: {
                xAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        padding: 20,
                        fontStyle: "bold",
                        fontColor: "#ff1818",
                        labelString: note
                    }
                }],
                yAxes: [{
                    ticks: {
                        beginAtZero: true,
                        //userCallback: function (label, index, labels) {
                        //    // when the floored value is the same as the value we have a whole number
                        //    if (Math.floor(label) === label) {
                        //        return label;
                        //    }

                        //},
                        //stepSize: 1,
                    },
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: LanguageDic['LB_JOB_COUNT']
                    }
                }]
            }
        }
    });
}

function DrawWeekLineChart(rootData) {
    var DAYS = [
        LanguageDic['LB_DATE_MONDAY'],
        LanguageDic['LB_DATE_TUESDAY'],
        LanguageDic['LB_DATE_WEDNESDAY'],
        LanguageDic['LB_DATE_THUSDAY'],
        LanguageDic['LB_DATE_FRIDAY'],
        LanguageDic['LB_DATE_SATURDAY'],
        LanguageDic['LB_DATE_SUNDAY']
    ];

    //var progress = document.getElementById('animationProgress');
    var color = Chart.helpers.color;
    Chart.defaults.global.animation.duration = 3000;
    //var note = LanguageDic['LB_UPDATE'] + ': UPDATED_AT, ' + LanguageDic['LB_UPDATE_NEXT_TIME'] + ': UPDATE_NEXT';
    //note = note.replace('UPDATED_AT', rootData.UpdatedTime).replace('UPDATE_NEXT', rootData.NextUpdate);
    var note = rootData.FromDateStr + " - " + rootData.ToDateStr;

    var lineChartData = {
        labels: DAYS,
        datasets: [
        {
            label: LanguageDic['LB_JOB_STATUS_PUBLIC'],
            backgroundColor: 'rgba(0, 0, 0, 0)',
            borderColor: window.chartColors.blue,
            borderWidth: 2,
            data: rootData.InWeekData.successData
        },
        {
            label: LanguageDic['LB_JOB_STATUS_PROCESSING'],
            backgroundColor: 'rgba(0, 0, 0, 0)',
            borderColor: window.chartColors.orange,
            borderWidth: 2,
            data: rootData.InWeekData.processingData
        },
        {
            label: LanguageDic['LB_JOB_STATUS_UNPROCESSED'],
            backgroundColor: 'rgba(0, 0, 0, 0)',
            borderColor: window.chartColors.red,
            borderWidth: 2,
            data: rootData.InWeekData.failedData
            //hidden: true,
        }
        ]

    };
    var ctx = document.getElementById("WeekStatisticsChartCanvas").getContext("2d");

    $("#WeekStatisticsChart").fadeIn(1000);

    window.myBar = new Chart(ctx, {
        type: 'line',
        data: lineChartData,
        options: {
            responsive: true,
            maintainAspectRatio: false,
            legend: {
                position: 'top'
            },
            title: {
                display: true,
                text: LanguageDic['LB_STATISTICS_JOB_IN_WEEK']
            },
            scales: {
                xAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        padding: 20,
                        fontStyle: "bold",
                        fontColor: "#ff1818",
                        labelString: note
                    }
                }],
                yAxes: [{
                    ticks: {
                        beginAtZero: true,
                        //userCallback: function (label, index, labels) {
                        //    // when the floored value is the same as the value we have a whole number
                        //    if (Math.floor(label) === label) {
                        //        return label;
                        //    }

                        //},
                        //stepSize: 1
                    },
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: LanguageDic['LB_JOB_COUNT']
                    }
                }]
            },
            //animation: {
            //    duration: 1000,
            //    onProgress: function (animation) {
            //        progress.value = animation.currentStep / animation.numSteps;
            //    },
            //    onComplete: function () {
            //        window.setTimeout(function () {
            //            progress.value = 0;
            //        }, 1000);

            //        progress.className = "hidden";
            //    }
            //}
        }
    });
}