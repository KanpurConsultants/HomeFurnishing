var app = angular.module('app', ['ngTouch', 'ui.grid', 'ui.grid.resizeColumns', 'ui.grid.grouping', 'ui.grid.moveColumns',
    'ui.grid.selection', 'ui.grid.exporter', 'ui.grid.cellNav', 'ui.grid.pinning']);

app.controller('MainCtrl', ['$scope', '$log', '$http', 'uiGridConstants', 'uiGridExporterConstants', 'uiGridExporterService',

  function ($scope, $log, $http, uiGridConstants, uiGridExporterConstants, uiGridExporterService) {

      
      //$scope.gridOptions = {};


      $scope.gridOptions = {
          enableHorizontalScrollbar: uiGridConstants.scrollbars.ALWAYS,
          enableFiltering: true,
          showColumnFooter: true,
          enableGridMenu: true,
          enableSelectAll: true,
          enableGridMenu: true,
          exporterMenuPdf: false,
          gridMenuShowHideColumns : true,
          exporterCsvLinkElement: angular.element(document.querySelectorAll(".custom-csv-link-location")),
          exporterCsvFilename: (($("#ReportTitle").val() != null && $("#ReportTitle").val()) != "" ? $("#ReportTitle").val() : $("#ReportHeader_ReportName").val()) + '.csv',
          exporterMenuCsv: false,
          gridMenuCustomItems: [{
              title: 'Export Data As CSV',
              order: 100,
              action: function ($event){
                  $scope.gridApi.exporter.csvExport('visible', 'visible');
              }
          },
          {
              title: 'Export Data As PDF',
              order: 100,
              action: function ($event) {
                  $scope.export();
              }
          }],


          onRegisterApi: function (gridApi) {
              $scope.gridApi = gridApi;
          }
      };


      $(document).keyup(function (e) {
          if (e.keyCode == 13) {
              if ($scope.gridApi.cellNav.getFocusedCell() != null) {
                  $scope.ShowDetail();
              }
          }
      });
      

      $scope.ShowDetail = function () {
          var rowCol = $scope.gridApi.cellNav.getFocusedCell();
          if (rowCol.row.entity.DocTypeId != null && rowCol.row.entity.DocId != null)
          {
              var DocTypeId = parseInt(rowCol.row.entity.DocTypeId);
              var DocId = parseInt(rowCol.row.entity.DocId);
              var Url = "/Redirect/RedirectToDocument?DocTypeId=" + DocTypeId + "&DocId=" + DocId + "&DocLineId=";
              window.open(Url, '_blank');
              return;
          }
      };


      function GetColumnWidth(results,j) {
          var ColWidth = 130;
          if (results.Data[0][j]["Value"] != null) {
              if ((results.Data[0][j]["Value"].length * 10).toString() != "NaN") {
                  ColWidth = results.Data[0][j]["Value"].length * 10;
              }
              else {
                  ColWidth = results.Data[0][j]["Key"].length * 10
              }
          }
          else {
              ColWidth = results.Data[0][j]["Key"].length * 10
          }

          if (ColWidth < 90)
              ColWidth = 90;

          if (ColWidth > 300)
              ColWidth = 300;

          return ColWidth;
      }


      $scope.BindData = function ()
      {
          $.ajax({
              url: '/GridReport/GridReportFill/' + $(this).serialize(),
              type: "POST",
              data: $("#registerSubmit").serialize(),
              success: function (result) {
                  Lock = false;
                  if (result.Success == true) {
                      var results = result;
                      if (results.Data.length > 0) {
                          var columnsIn = results.Data[results.Data.length - 1];
                          var j = 0;
                          var ColumnCount = 0;

                          $scope.gridOptions.columnDefs = new Array();

                          $.each(columnsIn, function (key, value) {
                              if (columnsIn[j]["Key"] != "SysParamType") {
                                  var ColWidth = GetColumnWidth(results, j);

                                  $scope.gridOptions.columnDefs.push({
                                      field: columnsIn[j]["Key"], aggregationType: columnsIn[j]["Value"],
                                      cellClass: (columnsIn[j]["Value"] == null ? 'cell-text' : 'text-right cell-text'),
                                      aggregationHideLabel: true,
                                      headerCellClass: (columnsIn[j]["Value"] == null ? 'header-text' : 'text-right header-text'),
                                      footerCellClass: (columnsIn[j]["Value"] == null ? '' : 'text-right '),
                                      width: ColWidth,
                                      enablePinning: true,
                                      visible: (columnsIn[j]["Key"] == "DocId" || columnsIn[j]["Key"] == "DocTypeId" ? false : true)
                                  });
                                }
                              ColumnCount++;
                              j++;
                              
                              
                          });


                          var rowDataSet = [];
                          var i = 0;
                          $.each(results.Data, function (key, value) {
                                var rowData = [];
                                var j = 0   
                                var columnsIn = results.Data[i];
                                if (columnsIn[ColumnCount - 1]["Value"] == null)
                                {
                                    $.each(columnsIn, function (key, value) {
                                        rowData[columnsIn[j]["Key"]] = columnsIn[j]["Value"];
                                        j++;
                                    });
                                }
                                rowDataSet[i] = rowData;
                                i++;
                          });

                          $scope.gridOptions.data = rowDataSet;

                          $scope.gridApi.grid.refresh();

                      }
                  }
                  else if (!result.Success) {
                      alert('Something went wrong');
                  }
              },
              error: function () {
                  Lock: false;
                  alert('Something went wrong');
              }
          });
      }
  }
]);






