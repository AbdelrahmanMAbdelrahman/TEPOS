
$(document).ready(function () {

    Manager.LoadBranchDDL();
    Manager.LoadStockTypeDDL();
    Manager.LoadCategoryDropdown();
    $('#btnPreview').click(function () {
        Manager.PreviewReport();
    });
});

var Manager = {
    LoadCategoryDropdown: function () {
        var jsonParam = '';
        var serviceURL = "/PosDropDown/GetCategories/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
        function onSuccess(jsonData) {
            var objProgram = jsonData;
            JsManager.PopulateCombo('#cmbProductCategory', objProgram, '  All Category',0);
            //$("#cmbProductCategory").chosen({ width: '97%' });
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
    },
    LoadBranchDDL: function() {
        var serviceUrl = '/PosDropDown/GetBranch/';
        var jsonParam = '';
        JsManager.SendJson(serviceUrl, jsonParam, onSuccess, onFailed);

        function onSuccess(response) {
            JsManager.PopulateCombo('#cmbBranch', response);
            $("#cmbBranch").chosen();
        }

        function onFailed() {
        }

    },
    LoadStockTypeDDL: function() {
        var serviceUrl = '/PosDropDown/GetStockType/';
        var jsonParam = '';
        JsManager.SendJson(serviceUrl, jsonParam, onSuccess, onFailed);

        function onSuccess(response) {
            JsManager.PopulateCombo('#cmbstockType', response, 'All Location',0);
        }

        function onFailed() {
        }

    },

    // mohamed labn 11/12/2024
    PreviewReport: function () {
        var minStockQuantity = $('#txtMinStockQuantity').val();
        var maxStockQuantity = 99999;

        minStockQuantity = (minStockQuantity === '' || minStockQuantity === null) ? maxStockQuantity : minStockQuantity;
 

        window.open(location.protocol + '//' + location.host + '/Report/Pos/Aspx/Stock/CurrentStock.aspx?' +
            'userOffice=' + ($('#cmbBranch').val() == null ? "" : $('#cmbBranch').val()) +
            '&stockType=' + $('#cmbstockType').val() +
            '&stockValueAaPar=' + $("#cmbstockValueAs").val() +
            '&itemStatus=' + $("#cmbitemStatus").val() +
            '&batchStatus=' + $("#cmbbatchStatus").val() +
            '&includeZeroStock=' + $("#chkIncludingZeroStock").is(':checked') +
            '&onlyZeroStock=' + $("#ckbOnlyZeroStock").is(':checked') +
            '&location=' + $('#cmbstockType option:selected').text() +
            '&productCategoryId=' + $('#cmbProductCategory').val() +
            '&minStockQuantity=' + minStockQuantity +
            '&withBatch=' + $('#chkWithBatch').is(':checked'), 
            '_blank');
    }


}