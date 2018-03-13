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
              fillColor: "rgba(210, 214, 222, 1)",
              strokeColor: "rgba(210, 214, 222, 1)",
              pointColor: "rgba(210, 214, 222, 1)",
              pointStrokeColor: "#c1c7d1",
              pointHighlightFill: "#fff",
              pointHighlightStroke: "rgba(210, 214, 222, 1)",
              data: data_SaleChart
          }
        ]
    };

    var VehicleSalebarChartData = VehicleSalesChartData;
    VehicleSalebarChartData.datasets[0].fillColor = "#00c0ef";
    VehicleSalebarChartData.datasets[0].strokeColor = "#00c0ef";
    VehicleSalebarChartData.datasets[0].pointColor = "#00c0ef";
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
    SpareSalebarChartData.datasets[0].fillColor = "#00c0ef";
    SpareSalebarChartData.datasets[0].strokeColor = "#00c0ef";
    SpareSalebarChartData.datasets[0].pointColor = "#00c0ef";
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

//----------------------Set Single Value------------------------------------------------

function SetSingleValue(functionname, Div_Id) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: '/DashBoardAuto/' + functionname,
        success: function (result) {
            $(Div_Id).text(result.Data[0].Value);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Failed to retrieve product details.' + thrownError);
        }
    });
}

//-----------------------------End Single Value Function---------------------------------------------------------

//----------------------Start For Readymande Table Design-------------------------------


function DesignTable(functionname, Head_Caption, Value_Caption, Div_Id) {
    var TableHTML = '<div class="box-body" style="overflow-y:auto; height: 400px;"> ' +
                                ' <table class="table table-bordered"> '
    TableHTML = TableHTML + '<tr> ' +
                                '<th style="width: 200px">' + Head_Caption + '</th> ' +
                                '<th style="width: 100px">' + Value_Caption + '</th> ' +
                            '</tr>'

    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: '/DashBoardAuto/' + functionname,
        success: function (result) {
            result.Data.forEach(function (value) {
                TableHTML = TableHTML + '<tr> ' +
                        ' <td style="width: 200px">' + value.Head + '</td> ' +
                        ' <td style="width: 100px">' + value.Value + '</td> ' +
                        ' </tr>'
            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Failed to retrieve product details.' + thrownError);
        }
    });
    TableHTML = TableHTML + '</table></div>'
    console.log(TableHTML);
    console.log(Div_Id);
    $(Div_Id).html(TableHTML)
}
//----------------------End For Readymande Table Design-------------------------------




$(document).ready(function () {
    SetSingleValue('GetVehicleSale', '#VehicleSaleAmount')
    SetSingleValue('GetVehicleProfit', '#VehicleProfitAmount')
    SetSingleValue('GetDebtors', '#DebtorsAmount')
    SetSingleValue('GetBankBalance', '#BankBalanceAmount')

    SetSingleValue('GetVehicleStock', '#VehicleStockAmount')
    SetSingleValue('GetExpense', '#ExpenseAmount')
    SetSingleValue('GetCreditors', '#CreditorsAmount')
    SetSingleValue('GetCashBalance', '#CashBalanceAmount')

    DesignTable('GetVehicleSaleDetailFinancierWise', 'Financier', 'Amount', '#VehicleSaleDetailFinancierWise');
    DesignTable('GetVehicleSaleDetailSalesManWise', 'Sales Man', 'Amount', '#VehicleSaleDetailSalesManWise');
    DesignTable('GetVehicleSaleDetailProductTypeWise', 'Type', 'Amount', '#VehicleSaleDetailProductTypeWise');

    DesignTable('GetVehicleProfitDetail', 'Type', 'Amount', '#VehicleProfitDetailTable');
    DesignTable('GetDebtorsDetail', 'Group', 'Amount', '#DebtorsDetailTable');
    DesignTable('GetBankBalanceDetail', 'Bank Account', 'Amount', '#BankBalanceDetailTable');

    DesignTable('GetVehicleStockDetail', 'Type', 'Amount', '#VehicleStockDetailTable');
    DesignTable('GetExpenseDetail', 'Group', 'Amount', '#ExpenseDetailTable');
    DesignTable('GetCreditorsDetail', 'Group', 'Amount', '#CreditorsDetailTable');
    DesignTable('GetCashBalanceDetail', 'Group', 'Amount', '#CashBalanceDetailTable');
});

