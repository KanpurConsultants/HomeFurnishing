angular.module('BusinessSessionLine', ['ui.grid', 'ui.grid.resizeColumns', 'ui.grid.cellNav', 'ui.grid.edit', 'ui.select'])
  .controller('MainCtrl', MainCtrl)
  .directive('uiSelectWrap', uiSelectWrap);

MainCtrl.$inject = ['$scope', '$http', 'uiGridConstants'];
function MainCtrl($scope, $http, uiGridConstants) {
    var ProductList = [];
    var ProductList_New = null;

    $scope.gridOptions = {
        onRegisterApi: function (gridApi) {
            $scope.gridApi = gridApi;
            gridApi.edit.on.afterCellEdit($scope, function (rowEntity, colDef, newValue, oldValue) {
                if (newValue != oldValue) {
                    $scope.Post(colDef.name, rowEntity.BusinessSessionLineId, rowEntity.BusinessSessionId, 
                        rowEntity.SiteId, rowEntity.DivisionId, newValue);
                }
            });
        },

        enableHorizontalScrollbar: uiGridConstants.scrollbars.ALWAYS,
        //rowHeight: 38,
        enableFiltering: true,
        columnDefs: [
          { name: 'BusinessSessionLineId', width: 50, visible: false },
          { name: 'BusinessSessionId', width: 50, visible: false },
          { name: 'SiteId', width: 50, visible: false },
          { name: 'DivisionId', width: 50, visible: false },
          { name: 'BusinessSessionName', displayName: "Business Session", width: 220, cellClass: 'cell-text ', headerCellClass: 'header-text', enableCellEdit: false },
          { name: 'SiteName', displayName: "Site", width: 220, cellClass: 'cell-text ', headerCellClass: 'header-text', enableCellEdit: false },
          { name: 'DivisionName', displayName: "Division", width: 220, cellClass: 'cell-text ', headerCellClass: 'header-text', enableCellEdit: false },
          { name: 'OpeningStockValue', displayName: "Opening Stock Value", width: 200, cellClass: 'text-right cell-text ', headerCellClass: 'text-right header-text', enableCellEdit: true, visible: false },
          { name: 'ClosingStockValue', displayName: "Closing Stock Value", width: 200, cellClass: 'text-right cell-text ', headerCellClass: 'text-right header-text', enableCellEdit: true },
        ]
    };


    $scope.Post = function (ColumnName, BusinessSessionLineId, BusinessSessionId, SiteId, DivisionId, newValue) {
        $.ajax({
            cache: false,
            url: "/BusinessSessionLine/Post/",
            data: {
                ColumnName : ColumnName, BusinessSessionLineId: BusinessSessionLineId, BusinessSessionId: BusinessSessionId,
                SiteId: SiteId, DivisionId: DivisionId, newValue: newValue
            },
            success: function (data) {
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert('Failed to retrive calculation footer' + thrownError);
            },
        });
    };

    angular.element(document).ready(function () {
        $scope.BindData();
    });


    $scope.BindData = function () {
        $.ajax({
            async: false,
            cache: false,
            type: "POST",
            url: '/BusinessSessionLine/FillRecords',
            success: function (result) {
                $scope.gridOptions.data = result.Data;
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert('Failed to retrieve product details.' + thrownError);
            }
        });
    }
}

uiSelectWrap.$inject = ['$document', 'uiGridEditConstants'];
function uiSelectWrap($document, uiGridEditConstants) {
    return function link($scope, $elm, $attr) {
        $document.on('click', docClick);
        $document.on('keydown', docClick);

        function docClick(evt) {
            if ($(evt.target).closest('.ui-select-container').size() === 0) {
                $scope.$emit(uiGridEditConstants.events.END_CELL_EDIT);
                $document.off('click', docClick);
            }
        }
    };
}

