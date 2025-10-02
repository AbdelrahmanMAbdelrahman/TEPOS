
$(document).ready(function () {

    Manager.LoadBranchDDL();
    $('#btnPreview').click(function () {
        Manager.PreviewReport();
    });

    $('#dateFrom').datetimepicker({
        timepicker: false,
        format: dtpDTFomat,
        onShow: function (ct) {
            this.setOptions({
                maxDate: JsManager.MDYToDashDMY($('#dateTo').val()) ? JsManager.MDYToDashDMY($('#dateTo').val()) : false

            });
        }
    });

    $('#dateTo').datetimepicker({
        timepicker: false,
        format: dtpDTFomat,
        onShow: function (ct) {
            this.setOptions({
                minDate: JsManager.MDYToDashDMY($('#dateFrom').val()) ? JsManager.MDYToDashDMY($('#dateFrom').val()) : false

            });
        }

    });
});

var Manager = {
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
   


    PreviewReport: function() {

        window.open(location.protocol + '//' + location.host + '/Report/Pos/Aspx/Sales/TotalSalesSummary.aspx?' +
            'branchId=' + ($('#cmbBranch').val() == null ? "" : $('#cmbBranch').val()) +
            '&dateFrom=' + $('#dateFrom').val() +
            '&dateTo=' + $("#dateTo").val() 
            ,'_blank');
    }
}