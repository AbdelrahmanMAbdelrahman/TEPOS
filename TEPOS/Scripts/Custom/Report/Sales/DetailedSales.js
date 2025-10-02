
$(document).ready(function () {

    Manager.LoadBranchDDL();
    Manager.LoadCategoryDropdown();
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
   


    PreviewReport: function() {
        var ProductName = $('#ProductName').val();
        window.open(location.protocol + '//' + location.host + '/Report/Pos/Aspx/Sales/DetailedSales.aspx?' +
            'branchId=' + ($('#cmbBranch').val() == null ? "" : $('#cmbBranch').val()) +
            '&dateFrom=' + $('#dateFrom').val() +
            '&dateTo=' + $("#dateTo").val() +
            '&ProductName=' + ProductName +
            '&invoiceNo=' + $("#invoiceNo").val() +
            '&isRelatedAllInvoice=' + $("#ckbIsRelatedAllInvoice").is(':checked') +
            '&IsInsite=' + $("#ckbIsInsite").is(':checked') +
            '&IsDelivery=' + $("#ckbIsDelivery").is(':checked') +
            '&IsCallCenter=' + $("#ckbIsCallCenter").is(':checked') +
            '&productCaregoryId=' + $("#cmbProductCategory").val()
            + '&PosInvoiceType=' + $('#PosInvoiceType').val() ,

            '_blank');
    }
}