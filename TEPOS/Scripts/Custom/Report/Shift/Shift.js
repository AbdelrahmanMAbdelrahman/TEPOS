
$(document).ready(function () {

    Manager.DatePicker();

    Manager.LoadBranchDDL();
    Manager.GetUser();
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
            JsManager.PopulateCombo('#Branch', response, "All",0);
        }

        function onFailed() {
        }

    },
    GetUser: function () {
        
        var serviceUrl = '/PosDropDown/GetUser/';
        var jsonParam = '';
        JsManager.SendJson(serviceUrl, jsonParam, onSuccess, onFailed);

        function onSuccess(response) {
            JsManager.PopulateCombo('#UserId', response, "All", 0);
        }

        function onFailed() {
        }
    },
 
    PreviewReport: function () {
        var dateFrom = $('#DateFrom').val();
        var dateTo = $('#DateTo').val();
        var branchId = !$('#Branch').val() ? 0 : $('#Branch').val();
        var UserId = !$('#UserId').val() ? 0 : $('#UserId').val();
        if (dateFrom == "") {
            Message.Warning('Date From is required');
        } else if (dateTo == "") {
            Message.Warning('Date To is required');
        } else {
            window.open(location.protocol + '//' + location.host + '/Report/Pos/Aspx/Shift/Shift.aspx?dateFrom=' +
                $('#DateFrom').val() +
                '&dateTo=' + $('#DateTo').val() +
                '&branchId=' + branchId +
                '&UserId=' + UserId +
                '&ShiftsOpen=' + $("#ShiftsOpen").is(':checked'),
                '_blank');
        }
    }
}