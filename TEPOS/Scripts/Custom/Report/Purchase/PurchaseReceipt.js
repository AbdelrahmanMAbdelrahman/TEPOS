$(document).ready(function () {

    Manager.DatePicker();

    Manager.LoadBranchDDL();
    Manager.loadSupplier();
   // Manager.loadProduct();  
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

    loadSupplier: function () {
        var jsonParam = '';
        var serviceURL = "/PosDropDown/GetSupplier/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            JsManager.PopulateCombo('#PosSupplierId', jsonData, 'Empty', '');
        }

        function onFailed(error) {
            window.alert(error.statusText);
        }
    },

    //loadProduct: function () {
    //    var jsonParam = '';
    //    var serviceURL = "/PosDropDown/GetProducts/";
    //    JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

    //    function onSuccess(jsonData) {
    //        JsManager.PopulateCombo('#ProductId', jsonData, 'Select Product', '');
    //    }

    //    function onFailed(error) {
    //        window.alert(error.statusText);
    //    }
    //},

    PreviewReport: function () {
        var dateFrom = $('#DateFrom').val();
        var dateTo = $('#DateTo').val();
        var branchId = !$('#Branch').val() ? 0 : $('#Branch').val();
        var PosSupplierId = !$('#PosSupplierId').val() ? 0 : $('#PosSupplierId').val();
       // var ProductId = !$('#ProductId').val() ? 0 : $('#ProductId').val(); 
        var InvoiceNo = $('#InvoiceNo').val(); 
        var ProductName = $('#ProductName').val(); 

        if (dateFrom == "") {
            Message.Warning('Date From is required');
        } else if (dateTo == "") {
            Message.Warning('Date To is required');
        } else {
            var url = location.protocol + '//' + location.host +
                '/Report/Pos/Aspx/Purchase/PurchaseReceipt.aspx?dateFrom=' + dateFrom +
                '&dateTo=' + dateTo +
                '&branchId=' + branchId +
                '&PosSupplierId=' + PosSupplierId +
                //'&ProductId=' + ProductId +
                '&ProductName=' + ProductName +
                '&InvoiceNo=' + InvoiceNo
                + '&PosInvoiceType=' + $('#PosInvoiceType').val();
            window.open(url, '_blank');
        }
    }
};
