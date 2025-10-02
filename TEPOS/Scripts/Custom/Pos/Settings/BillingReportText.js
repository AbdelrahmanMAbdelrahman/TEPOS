var dTable = null;
var _billingReportTextId = 0;

$(document).ready(function () {

    Manager.LoadBranchDropdown();

    Manager.GetDataForTable(0);

    dTable.on('order.dt search.dt', function () {
        dTable.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = dTableManager.IndexColumn(i + 1);
        });
    }).draw();

});

$(document).on('click', '.dTableDelete', function () {
    var $item = $(this).parents('tr');
    var row = dTable.row($item).data();
    Manager.Delete(row.BillingReportTextId);
});

$(document).on('click', '.dTableEdit', function () {
    JsManager.StartProcessBar();
    var $item = $(this).parents('tr');
    var row = dTable.row($item).data();

    _billingReportTextId = row.BillingReportTextId;
    $("#PosBranchId").val(row.PosBranchId);
    $("#PosBranchId").trigger('chosen:updated');
    $("#Text").val(row.Text);

    JsManager.EndProcessBar();
});

$(document).on('click', '#btnSave', function () {
    Manager.Save();
});

$(document).on('click', '#btnEdit', function () {
    Manager.Update();
});

$(document).on('click', '#btnClear', function () {
    Manager.FormReset();
});

var Manager = {

    FormReset: function () {
        _billingReportTextId = 0;
        $("#BillingReportForm")[0].reset();
        $("#PosBranchId").trigger('chosen:updated');
    },

    LoadBranchDropdown: function () {
        var jsonParam = "";
        var serviceUrl = "/PosDropdownSetting/GetAllBranchList/";
        JsManager.SendJson(serviceUrl, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            JsManager.PopulateCombo('#PosBranchId', jsonData, 'Select Branch',0);
            $('#PosBranchId').chosen({width:'96%'});
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

    Delete: function (billingReportTextId) {
        if (Message.Prompt()) {
            var jsonParam = { billingReportTextId: billingReportTextId };
            var serviceUrl = "/PosSetting/DeleteBillingReportText/";
            JsManager.SendJson(serviceUrl, jsonParam, onSuccess, onFailed);
        }


        function onSuccess(jsonData) {
            if (jsonData == "0") {
                Message.Error("delete");
            } else {
                Message.Success("delete");
                Manager.FormReset();
                Manager.GetDataForTable(1);
            }
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

    Update: function () {
        var obj = [];
        obj.push({ 'Branch': $("#PosBranchId").val() });
        obj.push({ 'Text': $("#Text").val() });
        if (JsManager.validate(obj)) {
            if (Message.Prompt()) {
                var jsonParam = $('#BillingReportForm').serialize() + '&Id=' + _billingReportTextId;
                var serviceURL = "/PosSetting/UpdateBillingReportText/";
                JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
            }
            function onSuccess(jsonData) {
                if (jsonData == "0") {
                    Message.Error("update");
                } else {
                    Message.Success("update");
                    Manager.FormReset();
                    Manager.GetDataForTable(1);
                }
            }

            function onFailed(xhr, status, err) {
                Message.Exception(xhr);
            }
        }
    },

    Save: function () {
        var obj = [];
        obj.push({ 'Branch': $("#PosBranchId").val() });
        obj.push({ 'Text': $("#Text").val() });
        if (JsManager.validate(obj)) {
            if (Message.Prompt()) {
                var jsonParam = $('#BillingReportForm').serialize();
                var serviceURL = "/PosSetting/SaveBillingReportText/";
                JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
            }
            function onSuccess(jsonData) {
                if (jsonData == "0") {
                    Message.Error("save");
                } else {
                    Message.Success("save");
                    Manager.FormReset();
                    Manager.GetDataForTable(1);
                }
            }

            function onFailed(xhr, status, err) {
                Message.Exception(xhr);
            }
        }
    },

    GetDataForTable: function (ref) {
        var jsonParam = '';
        var serviceURL = "/PosSetting/GetAllBillingReportTexts/";
        JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            Manager.LoadDataTable(jsonData, ref);
        }
        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

    LoadDataTable: function (userdata, isRef) {
        if (isRef == "0") {
            dTable = $('#dTable').DataTable({
                dom: "<'row'<'col-md-4'B><'col-md-3'l><'col-md-5'f>>" + "<'row'<'col-md-12'tr>>" + "<'row'<'col-md-5'i><'col-md-7 mt-7'p>>",
                initComplete: function () {
                    dTableManager.Border("#dTable", 250);
                    $('#tableElement_length').css({ 'float': 'right' });
                },
                buttons: [
                   
                    {
                        text: '<i class="fas fa-print"></i> Print',
                        className: 'btn btn-sm',
                        extend: 'print',
                        exportOptions: {
                            columns: [2, 3]
                        },
                        title: 'Billing Report Text'
                    },
                    {
                        text: '<i class="far fa-file-excel"></i> Excel',
                        className: 'btn btn-sm',
                        extend: 'excelHtml5',
                        exportOptions: {
                            columns: [2, 3]
                        },
                        title: 'Billing Report Text'
                    }
                ],

                scrollY: 250,
                scrollX: true,
                scrollCollapse: true,
                lengthMenu: [[50, 100, 500, -1], [50, 100, 500, "All"]],
                columnDefs: [
                    { visible: false, targets: [] },
                    { "className": "dt-center", "targets": [] }
                ],
                columns: [
                    {
                        data: '',
                        name: 'SL',
                        orderable: false,
                        title: '#', width: 7,
                        render: function (data, type, row, meta) {
                            return '';
                        }
                    },
                    {
                        name: 'Option',
                        title: 'Option', width: 80,
                        render: function (data, type, row) {
                            return EventManager.DataTableCommonButton();
                        }
                    },
                    { data: 'BranchName', name: 'BranchName', title: 'BranchName' },
                    { data: 'Text', name: 'Text', title: 'Text' }
                ],
                fixedColumns: true,
                data: userdata,
                rowsGroup: null
            });

        } else {
            dTable.clear().rows.add(userdata).draw();

        }

    },
}