
$(document).ready(function () {

    Manager.DatePicker();
    Manager.LoadBranchDDL();

    $('#btnPreview').click(function () {
        Manager.PreviewReport();
    });
});

var Manager = {
    
    DatePicker: function () {
        $('#DateFrom').datetimepicker({
            timepicker: false,
            format: dtpDTFomat
        });

        $('#DateTo').datetimepicker({
            timepicker: false,
            format: dtpDTFomat
        });
    },

    LoadBranchDDL: function () {
        var serviceUrl = '/PosDropDown/GetBranch/';
        var jsonParam = '';
        JsManager.SendJson(serviceUrl, jsonParam, onSuccess, onFailed);

        function onSuccess(response) {
            JsManager.PopulateCombo('#Branch', response, "All", 0);
        }

        function onFailed() {
        }
    },

    LoadExpenseTypeDDL: function () {
        var jsonParam = '';
        var serviceURL = "/Expense/GetExpenseTypes/";
        JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed, 'GET');

        function onSuccess(jsonData) {
            JsManager.PopulateCombo('#expenseType', jsonData);
        }

        function onFailed(xhr, status, err) {
            if (xhr.status === 404) {
                //    Message.Error("The requested endpoint was not found. Please check your server configuration.");
            } else {
                Message.Exception(xhr);
            }
        }
    },

    PreviewReport: function () {
        var dateFrom = $('#DateFrom').val();
        var dateTo = $('#DateTo').val();
        var branchId = !$('#Branch').val() ? 0 : $('#Branch').val();
        var expenseType = !$('#expenseType').val() ? 0 : $('#expenseType').val();
        if (dateFrom == "") {
            Message.Warning('Date From is required');
        } else if (dateTo == "") {
            Message.Warning('Date To is required');
        } else {
            window.open(location.protocol + '//' + location.host + '/Report/Pos/Aspx/Expenses/Expenses.aspx?' +
                'dateFrom=' + $('#DateFrom').val() +
                '&dateTo=' + $('#DateTo').val() +
                '&expenseType=' + expenseType +
                '&branchId=' + branchId, '_blank');

        }
    }
}