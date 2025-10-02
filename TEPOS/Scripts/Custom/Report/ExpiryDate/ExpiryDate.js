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
            JsManager.PopulateCombo('#cmbProductCategory', objProgram, '  All Category', 0);
            //$("#cmbProductCategory").chosen({ width: '97%' });
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
    },
    LoadBranchDDL: function () {
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
    LoadStockTypeDDL: function () {
        var serviceUrl = '/PosDropDown/GetStockType/';
        var jsonParam = '';
        JsManager.SendJson(serviceUrl, jsonParam, onSuccess, onFailed);

        function onSuccess(response) {
            JsManager.PopulateCombo('#cmbstockType', response, 'All Location', 0);
        }

        function onFailed() {
        }

    },

    PreviewReport: function () {


        window.open(location.protocol + '//' + location.host + '/Report/Pos/Aspx/ExpiryDate/ExpiryDate.aspx?' +
            'userOffice=' + ($('#cmbBranch').val() == null ? "" : $('#cmbBranch').val()) +
            '&stockType=' + $('#cmbstockType').val() +
            '&stockValueAaPar=' + $("#cmbstockValueAs").val() +
            '&productCategoryId=' + $('#cmbProductCategory').val() ,
            '_blank');
    }


}