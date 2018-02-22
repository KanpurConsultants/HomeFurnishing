var Salary = angular.module('Salary', ['ngTouch', 'ui.grid', 'ui.grid.resizeColumns', 'ui.grid.grouping', 'ui.grid.moveColumns',
    'ui.grid.selection', 'ui.grid.exporter', 'ui.grid.cellNav', 'ui.grid.pinning', 'ui.grid.edit']);

Salary.controller('MainCtrl', ['$scope', '$log', '$http', 'uiGridConstants', 'uiGridExporterConstants', 'uiGridExporterService',

  function ($scope, $log, $http, uiGridConstants, uiGridExporterConstants, uiGridExporterService) {

      
      //$scope.gridOptions = {};


      $scope.gridOptions = {
          enableHorizontalScrollbar: uiGridConstants.scrollbars.ALWAYS,
          enableFiltering: true,
          showColumnFooter: true,
          enableGridMenu: true,
          enableSelectAll: true,
          exporterMenuPdf: false,
          gridMenuShowHideColumns : false,
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



      $scope.Post = function (SaleEnquiryLineId, ProductName) {
          var i = 0;
          var Rows = [];          
          $scope.gridApi.core.getVisibleRows($scope.gridApi.grid).some(function (rowItem) {              
              Rows[i] = rowItem.entity;
              i++;
          });

          console.log(Rows);

          $.ajax({
              url: "/SalaryWizard/Post/",
              data: { SalaryDataList: Rows },
              type: "POST",
              success: function (data) {
                  //Do something:
                  window.location.href = data;

                  //window.open(data);
              }
          })
      };



      $scope.BindData = function ()
      {
          $.ajax({
              url: '/SalaryWizard/SalaryWizardFill/' + $(this).serialize(),
              type: "POST",
              data: $("#registerSubmit").serialize(),
              success: function (result) {
                  Lock = false;
                  if (result.Success == true) {
                      $scope.gridOptions.enableCellEditOnFocus = true;
                      $scope.gridOptions.columnDefs = new Array();
                      $scope.gridOptions.columnDefs.push({ field: 'EmployeeId', width: 100, visible: false });
                      $scope.gridOptions.columnDefs.push({ field: 'DocTypeId', width: 100, visible: false });
                      $scope.gridOptions.columnDefs.push({ field: 'DocDate', width: 100, visible: false });
                      $scope.gridOptions.columnDefs.push({ field: 'MonthDays', width: 100, visible: false });
                      $scope.gridOptions.columnDefs.push({ field: 'EmployeeName', width: 570, cellClass: 'cell-text ', headerCellClass: 'header-text', enableCellEdit: false });
                      $scope.gridOptions.columnDefs.push({ field: 'Days', width: 100, cellClass: 'text-right cell-text ', headerCellClass: 'text-right header-text', enableCellEdit: true });
                      $scope.gridOptions.columnDefs.push({ field: 'Additions', width: 150, cellClass: 'text-right cell-text ', headerCellClass: 'text-right header-text', enableCellEdit: true });
                      $scope.gridOptions.columnDefs.push({ field: 'Deductions', width: 150, cellClass: 'text-right cell-text ', headerCellClass: 'text-right header-text', enableCellEdit: true });
                      $scope.gridOptions.columnDefs.push({ field: 'LoanEMI', width: 150, cellClass: 'text-right cell-text ', headerCellClass: 'text-right header-text', enableCellEdit: true });
                      $scope.gridOptions.data = result.Data;
                      $scope.gridApi.grid.refresh();
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






