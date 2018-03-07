$(function () {

    'use strict';

    /* ChartJS
     * -------
     * Here we will create a few charts using ChartJS
     */



    //-------------
    //- Vehicle Sale PIE CHART -
    //-------------
    // Get context with jQuery - using jQuery's .get() method.
    var VehicleSalepieChartCanvas = $("#VehicleSalePieChart").get(0).getContext("2d");
    var VehicleSalepieChart = new Chart(VehicleSalepieChartCanvas);
    var VehicleSalePieChartDataArray = null
    GetVehicleSalePieChartData();

    function GetVehicleSalePieChartData() {
        $.ajax({
            async: false,
            cache: false,
            type: "POST",
            url: '/DashBoardAuto/GetVehicleSalePieChartData',
            success: function (result) {
                VehicleSalePieChartDataArray = result.Data;
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert('Failed to retrieve product details.' + thrownError);
            }
        });
    }


    var VehicleSalePieChartHint = '<ul class="chart-legend clearfix">'
    VehicleSalePieChartDataArray.forEach(function (value) {
        VehicleSalePieChartHint = VehicleSalePieChartHint + '<li><i class="fa fa-circle-o" style="color:' + value.color + '"></i> ' + value.label + '</li>'
    });
    VehicleSalePieChartHint = VehicleSalePieChartHint + '</ul>'

    $('#VehicleSalePieChartHint').html(VehicleSalePieChartHint)

    var VehicleSalePieChartOptions = {
        //Boolean - Whether we should show a stroke on each segment
        segmentShowStroke: true,
        //String - The colour of each segment stroke
        segmentStrokeColor: "#fff",
        //Number - The width of each segment stroke
        segmentStrokeWidth: 1,
        //Number - The percentage of the chart that we cut out of the middle
        percentageInnerCutout: 50, // This is 0 for Pie charts
        //Number - Amount of animation steps
        animationSteps: 100,
        //String - Animation easing effect
        animationEasing: "easeOutBounce",
        //Boolean - Whether we animate the rotation of the Doughnut
        animateRotate: true,
        //Boolean - Whether we animate scaling the Doughnut from the centre
        animateScale: false,
        //Boolean - whether to make the chart responsive to window resizing
        responsive: true,
        // Boolean - whether to maintain the starting aspect ratio or not when responsive, if set to false, will take up entire container
        maintainAspectRatio: false,
        //String - A legend template
        legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<segments.length; i++){%><li><span style=\"background-color:<%=segments[i].fillColor%>\"></span><%if(segments[i].label){%><%=segments[i].label%><%}%></li><%}%></ul>",
        //String - A tooltip template
        tooltipTemplate: "<%=value %> <%=label%>"
    };
    //Create pie or douhnut chart
    // You can switch between pie and douhnut using the method below.
    VehicleSalepieChart.Doughnut(VehicleSalePieChartDataArray, VehicleSalePieChartOptions);
    //-----------------
    //- END Vehicle Sale PIE CHART -
    //-----------------

    //---------------------------------------------------------------------------------------------------------
    //-------------
    //- Spare Sale PIE CHART -
    //-------------
    // Get context with jQuery - using jQuery's .get() method.
    var SpareSalepieChartCanvas = $("#SpareSalePieChart").get(0).getContext("2d");
    var SpareSalepieChart = new Chart(SpareSalepieChartCanvas);
    var SpareSalePieChartDataArray = null
    GetSpareSalePieChartData();

    function GetSpareSalePieChartData() {
        $.ajax({
            async: false,
            cache: false,
            type: "POST",
            url: '/DashBoardAuto/GetSpareSalePieChartData',
            success: function (result) {
                SpareSalePieChartDataArray = result.Data;
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert('Failed to retrieve product details.' + thrownError);
            }
        });
    }


    var SpareSalePieChartHint = '<ul class="chart-legend clearfix">'
    SpareSalePieChartDataArray.forEach(function (value) {
        SpareSalePieChartHint = SpareSalePieChartHint + '<li><i class="fa fa-circle-o" style="color:' + value.color + '"></i> ' + value.label + '</li>'
    });
    SpareSalePieChartHint = SpareSalePieChartHint + '</ul>'

    $('#SpareSalePieChartHint').html(SpareSalePieChartHint)

    var SpareSalePieChartOptions = {
        //Boolean - Whether we should show a stroke on each segment
        segmentShowStroke: true,
        //String - The colour of each segment stroke
        segmentStrokeColor: "#fff",
        //Number - The width of each segment stroke
        segmentStrokeWidth: 1,
        //Number - The percentage of the chart that we cut out of the middle
        percentageInnerCutout: 50, // This is 0 for Pie charts
        //Number - Amount of animation steps
        animationSteps: 100,
        //String - Animation easing effect
        animationEasing: "easeOutBounce",
        //Boolean - Whether we animate the rotation of the Doughnut
        animateRotate: true,
        //Boolean - Whether we animate scaling the Doughnut from the centre
        animateScale: false,
        //Boolean - whether to make the chart responsive to window resizing
        responsive: true,
        // Boolean - whether to maintain the starting aspect ratio or not when responsive, if set to false, will take up entire container
        maintainAspectRatio: false,
        //String - A legend template
        legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<segments.length; i++){%><li><span style=\"background-color:<%=segments[i].fillColor%>\"></span><%if(segments[i].label){%><%=segments[i].label%><%}%></li><%}%></ul>",
        //String - A tooltip template
        tooltipTemplate: "<%=value %> <%=label%>"
    };
    //Create pie or douhnut chart
    // You can switch between pie and douhnut using the method below.
    SpareSalepieChart.Doughnut(SpareSalePieChartDataArray, SpareSalePieChartOptions);
    //-----------------
    //- END Spare Sale PIE CHART -
    //-----------------


    //---------------------------------------------------------------------------------------------------------


    //-------------
    //-Vehicle Sale BAR CHART -
    //-------------
    var VehicleSalebarChartCanvas = $("#VehicleSaleBarChart").get(0).getContext("2d");
    var VehicleSalebarChart = new Chart(VehicleSalebarChartCanvas);
    

    var VehicleSaleChartDataArray = null
    GetVehicleSaleChartData();

    function GetVehicleSaleChartData() {
        $.ajax({
            async: false,
            cache: false,
            type: "POST",
            url: '/DashBoardAuto/GetVehicleSaleChartData',
            success: function (result) {
                VehicleSaleChartDataArray = result.Data;
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert('Failed to retrieve product details.' + thrownError);
            }
        });
    }

    var labels_SalesChart = [], data_SaleChart = []
    VehicleSaleChartDataArray.forEach(function (value) {
        labels_SalesChart.push(value.Month);
        data_SaleChart.push(value.Amount);
    });


    var VehicleSalesChartData = {
        labels: labels_SalesChart,
        datasets: [
          {
              label: "Amount",
              fillColor: "rgb(210, 214, 222)",
              strokeColor: "rgb(210, 214, 222)",
              pointColor: "rgb(210, 214, 222)",
              pointStrokeColor: "#c1c7d1",
              pointHighlightFill: "#fff",
              pointHighlightStroke: "rgb(220,220,220)",
              data: data_SaleChart
          }
        ]
    };

    var VehicleSalebarChartData = VehicleSalesChartData;
    VehicleSalebarChartData.datasets[0].fillColor = "#00a65a";
    VehicleSalebarChartData.datasets[0].strokeColor = "#00a65a";
    VehicleSalebarChartData.datasets[0].pointColor = "#00a65a";
    var VehicleSalebarChartOptions = {
        //Boolean - Whether the scale should start at zero, or an order of magnitude down from the lowest value
        scaleBeginAtZero: true,
        //Boolean - Whether grid lines are shown across the chart
        scaleShowGridLines: true,
        //String - Colour of the grid lines
        scaleGridLineColor: "rgba(0,0,0,.05)",
        //Number - Width of the grid lines
        scaleGridLineWidth: 1,
        //Boolean - Whether to show horizontal lines (except X axis)
        scaleShowHorizontalLines: true,
        //Boolean - Whether to show vertical lines (except Y axis)
        scaleShowVerticalLines: true,
        //Boolean - If there is a stroke on each bar
        barShowStroke: true,
        //Number - Pixel width of the bar stroke
        barStrokeWidth: 1,
        //Number - Spacing between each of the X value sets
        barValueSpacing: 5,
        //Number - Spacing between data sets within X values
        barDatasetSpacing: 1,
        //String - A legend template
        legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><li><span style=\"background-color:<%=datasets[i].fillColor%>\"></span><%if(datasets[i].label){%><%=datasets[i].label%><%}%></li><%}%></ul>",
        //Boolean - whether to make the chart responsive
        responsive: true,
        maintainAspectRatio: true
    };

    VehicleSalebarChartOptions.datasetFill = false;
    VehicleSalebarChart.Bar(VehicleSalesChartData, VehicleSalebarChartOptions);


    //-------------------------------------------------------------------------------------------------


    //-------------
    //-Spare Sale BAR CHART -
    //-------------
    var SpareSalebarChartCanvas = $("#SpareSaleBarChart").get(0).getContext("2d");
    var SpareSalebarChart = new Chart(SpareSalebarChartCanvas);


    var SpareSaleChartDataArray = null
    GetSpareSaleChartData();

    function GetSpareSaleChartData() {
        $.ajax({
            async: false,
            cache: false,
            type: "POST",
            url: '/DashBoardAuto/GetSpareSaleChartData',
            success: function (result) {
                SpareSaleChartDataArray = result.Data;
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert('Failed to retrieve product details.' + thrownError);
            }
        });
    }

    var labels_SalesChart = [], data_SaleChart = []
    SpareSaleChartDataArray.forEach(function (value) {
        labels_SalesChart.push(value.Month);
        data_SaleChart.push(value.Amount);
    });


    var SpareSalesChartData = {
        labels: labels_SalesChart,
        datasets: [
          {
              label: "Amount",
              fillColor: "rgb(210, 214, 222)",
              strokeColor: "rgb(210, 214, 222)",
              pointColor: "rgb(210, 214, 222)",
              pointStrokeColor: "#c1c7d1",
              pointHighlightFill: "#fff",
              pointHighlightStroke: "rgb(220,220,220)",
              data: data_SaleChart
          }
        ]
    };

    var SpareSalebarChartData = SpareSalesChartData;
    SpareSalebarChartData.datasets[0].fillColor = "#00a65a";
    SpareSalebarChartData.datasets[0].strokeColor = "#00a65a";
    SpareSalebarChartData.datasets[0].pointColor = "#00a65a";
    var SpareSalebarChartOptions = {
        //Boolean - Whether the scale should start at zero, or an order of magnitude down from the lowest value
        scaleBeginAtZero: true,
        //Boolean - Whether grid lines are shown across the chart
        scaleShowGridLines: true,
        //String - Colour of the grid lines
        scaleGridLineColor: "rgba(0,0,0,.05)",
        //Number - Width of the grid lines
        scaleGridLineWidth: 1,
        //Boolean - Whether to show horizontal lines (except X axis)
        scaleShowHorizontalLines: true,
        //Boolean - Whether to show vertical lines (except Y axis)
        scaleShowVerticalLines: true,
        //Boolean - If there is a stroke on each bar
        barShowStroke: true,
        //Number - Pixel width of the bar stroke
        barStrokeWidth: 1,
        //Number - Spacing between each of the X value sets
        barValueSpacing: 5,
        //Number - Spacing between data sets within X values
        barDatasetSpacing: 1,
        //String - A legend template
        legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><li><span style=\"background-color:<%=datasets[i].fillColor%>\"></span><%if(datasets[i].label){%><%=datasets[i].label%><%}%></li><%}%></ul>",
        //Boolean - whether to make the chart responsive
        responsive: true,
        maintainAspectRatio: true
    };

    SpareSalebarChartOptions.datasetFill = false;
    SpareSalebarChart.Bar(SpareSalesChartData, SpareSalebarChartOptions);


    //-------------------------------------------------------------------------------------------------

});



function GetVehicleSale()
{
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: '/DashBoardAuto/GetVehicleSale',
        success: function (result) {
            $('#VehicleSaleAmount').text(result.Data[0].SaleAmount);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Failed to retrieve product details.' + thrownError);
        }
    });
}

function GetVehicleProfit() {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: '/DashBoardAuto/GetVehicleProfit',
        success: function (result) {
            $('#VehicleProfitAmount').text(result.Data[0].ProfitAmount);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Failed to retrieve product details.' + thrownError);
        }
    });
}

function GetVehicleStock() {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: '/DashBoardAuto/GetVehicleStock',
        success: function (result) {
            $('#VehicleStockAmount').text(result.Data[0].StockAmount);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Failed to retrieve product details.' + thrownError);
        }
    });
}

function GetExpense() {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: '/DashBoardAuto/GetExpense',
        success: function (result) {
            $('#ExpenseAmount').text(result.Data[0].ExpenseAmount);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Failed to retrieve product details.' + thrownError);
        }
    });
}

function GetDebtors() {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: '/DashBoardAuto/GetDebtors',
        success: function (result) {
            $('#DebtorsAmount').text(result.Data[0].DebtorsAmount);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Failed to retrieve product details.' + thrownError);
        }
    });
}

function GetCreditors() {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: '/DashBoardAuto/GetCreditors',
        success: function (result) {
            $('#CreditorsAmount').text(result.Data[0].CreditorsAmount);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Failed to retrieve product details.' + thrownError);
        }
    });
}

function GetBankBalance() {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: '/DashBoardAuto/GetBankBalance',
        success: function (result) {
            $('#BankBalanceAmount').text(result.Data[0].BankBalanceAmount);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Failed to retrieve product details.' + thrownError);
        }
    });
}

function GetCashBalance() {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: '/DashBoardAuto/GetCashBalance',
        success: function (result) {
            $('#CashBalanceAmount').text(result.Data[0].CashBalanceAmount);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Failed to retrieve product details.' + thrownError);
        }
    });
}

var VehicleSalePieChartHint = '<div class="box-body" style="overflow-y:auto; height: 400px;"> ' +
                                ' <table class="table table-bordered"> '
VehicleSalePieChartHint = VehicleSalePieChartHint + '<tr> ' +
                                                        '<th style="width: 200px">Financier</th> ' +
                                                        '<th style="width: 100px">Amount</th> ' +
                                                    '</tr>'


function GetSpareSalePieChartData() {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: '/DashBoardAuto/GetVehicleSaleDetailFinancierWise',
        success: function (result) {
            result.Data.forEach(function (value) {
                //VehicleSalePieChartHint = VehicleSalePieChartHint + '<li><i class="fa fa-circle-o" style="color:' + value.color + '"></i> ' + value.label + '</li>'
                VehicleSalePieChartHint = VehicleSalePieChartHint + '<tr> ' +
                        ' <td style="width: 200px">' + value.FinancierName + '</td> ' +
                        ' <td style="width: 100px">' + value.Amount + '</td> ' +
                        ' </tr>'
            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Failed to retrieve product details.' + thrownError);
        }
    });
    VehicleSalePieChartHint = VehicleSalePieChartHint + '</table></div>'
    $('#VehicleSaleDetailFinancierWise').html(VehicleSalePieChartHint)
}




$(document).ready(function () {
    GetVehicleSale();
    GetVehicleProfit();
    GetVehicleStock();

    GetExpense();
    GetDebtors();
    GetCreditors();
    GetBankBalance();
    GetCashBalance();

    GetSpareSalePieChartData();
});

