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
            console.error("Failed to load branches");
        }
    },




    PreviewReport: function () {
        var dateFrom = $('#DateFrom').val();
        var dateTo = $('#DateTo').val();
        var branchId = !$('#Branch').val() ? 0 : $('#Branch').val();
        var ProductName = $('#ProductName').val();

        if (dateFrom == "") {
            Message.Warning('Date From is required');
        } else if (dateTo == "") {
            Message.Warning('Date To is required');
        } else {
            var url = location.protocol + '//' + location.host +
                '/Report/Pos/Aspx/ItemMovement/ItemMovement.aspx?dateFrom=' + dateFrom +
                '&dateTo=' + dateTo +
                '&branchId=' + branchId +
                '&ProductName=' + ProductName ;

            window.open(url, '_blank');
        }
    }
};
