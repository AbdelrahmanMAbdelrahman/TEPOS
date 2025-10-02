
$(document).ready(function () {

    Manager.DatePicker();
    Manager.LoadBranchDDL();
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
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
    },
    DatePicker: function() {
        $('#DateFrom').datetimepicker({
            timepicker: false,
            format: dtpDTFomat
        });

        $('#DateTo').datetimepicker({
            timepicker: false,
            format: dtpDTFomat
        });
    },



    LoadBranchDDL: function() {
        var serviceUrl = '/PosDropDown/GetBranch/';
        var jsonParam = '';
        JsManager.SendJson(serviceUrl, jsonParam, onSuccess, onFailed);

        function onSuccess(response) {
            JsManager.PopulateCombo('#Branch', response, "All",0);
        }

        function onFailed() {
        }

    },

    PreviewReport: function() {
        var dateFrom = $('#DateFrom').val();
        var dateTo = $('#DateTo').val();
        var ProductName = $('#ProductName').val();
        var branchId = !$('#Branch').val() ? 0 : $('#Branch').val();
        if (dateFrom == "") {
            Message.Warning('Date From is required');
        } else if (dateTo == "") {
            Message.Warning('Date To is required');
        } else {
            window.open(location.protocol + '//' + location.host + '/Report/Pos/Aspx/Sales/ItemWiseSales.aspx?' +
                'dateFrom=' + $('#DateFrom').val() +
                '&dateTo=' + $('#DateTo').val() +
                '&ProductName=' + ProductName +
                '&branchId=' + branchId +
                '&productCaregoryId=' + $("#cmbProductCategory").val(), '_blank');
        }
    }
}