
$(document).ready(function () {

    Manager.DatePicker();

    Manager.LoadBranchDDL();

    Manager.LoadCategoryDropdown();

    $('#btnPreview').click(function () {
        Manager.PreviewReport();
    });

    $('#Category').change(function() {
        var categoryId = $(this).val();
        if (categoryId && categoryId != '0') {
            Manager.LoadProductDropdown(categoryId);
        }
        else {
            $("#Product").empty();
            $("#Product").trigger('chosen:updated');
        }
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
            JsManager.PopulateCombo('#Branch', response);
            $('#Branch').chosen({
                placeholder_text_multiple:'  Select Branch',
                width:'100%'
            });
        }

        function onFailed() {
        }

    },

    PreviewReport: function () {
        var dateFrom = $('#DateFrom').val();
        var dateTo = $('#DateTo').val();
        var branch = !$('#Branch').val() ? '' : $('#Branch').val();
        var categoryId = !$('#Category').val() ? 0 : $('#Category').val();
        var productId = !$('#Product').val() ? 0 : $('#Product').val();

        if (dateFrom == "") {
            Message.Warning('Date From is required');
        } else if (dateTo == "") {
            Message.Warning('Date To is required');
        } else {
            window.open(location.protocol + '//' + location.host + '/Report/Pos/Aspx/Stock/ItemHistory.aspx?dateFrom=' + dateFrom + '&dateTo=' + dateTo + '&branchId=' + branch + '&categoryId=' + categoryId + '&productId=' + productId, '_blank');
        }
    },

    LoadCategoryDropdown: function () {
        var jsonParam = '';
        var serviceUrl = "/PosDropDown/GetCategories/";
        JsManager.SendJson(serviceUrl, jsonParam, onSuccess, onFailed);
        function onSuccess(jsonData) {
            var objProgram = jsonData;
            JsManager.PopulateCombo('#Category', objProgram, 'Select one',0);
            $("#Category").chosen({width:'100%'});
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
    },

    LoadProductDropdown: function (categoryId) {
        var jsonParam = { categoryId: categoryId };
        var serviceUrl = "/PosDropDown/GetProductByCategory/";
        JsManager.SendJson(serviceUrl, jsonParam, onSuccess, onFailed);
        function onSuccess(jsonData) {
            var objProgram = jsonData;
            JsManager.PopulateCombo('#Product', objProgram, 'Select one',0);
            $("#Product").chosen({ width: '100%' });
            $("#Product").trigger('chosen:updated');
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
    }

}