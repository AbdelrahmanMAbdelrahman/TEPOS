var dTable = null;
var _branchId = 0;

$(document).ready(function () {

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
    Manager.Delete(row.BranchId);
});

$(document).on('click', '.dTableEdit', function () {
    JsManager.StartProcessBar();
    var $item = $(this).parents('tr');
    var row = dTable.row($item).data();

    _branchId = row.BranchId;
    $("#Name").val(row.Name);
    $("#Address").val(row.Address);
    $("#Phone").val(row.Phone);
    $("#Mobile").val(row.Mobile);
    $("#Remarks").val(row.Remarks);
    $("#Email").val(row.Email);
    $("#Fax").val(row.Fax);

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
        _branchId = 0;
        $("#branchForm")[0].reset();
    },

    Delete: function (branchId) {
        if (Message.Prompt()) {
            var jsonParam = { branchId: branchId };
            var serviceURL = "/PosSetting/DeleteBranch/";
            JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
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
        obj.push({ 'Name': $("#Name").val() });
        obj.push({ 'Address': $("#Address").val() });
        if (JsManager.validate(obj)) {
            if (Message.Prompt()) {
                var jsonParam = $('#branchForm').serialize() + '&Id=' + _branchId;
                var serviceURL = "/PosSetting/UpdateBranch/";
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
        obj.push({ 'Name': $("#Name").val() });
        obj.push({ 'Address': $("#Address").val() });
        if (JsManager.validate(obj)) {
            if (Message.Prompt()) {
                var jsonParam = $('#branchForm').serialize();
                var serviceURL = "/PosSetting/SaveBranch/";
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
        var serviceURL = "/PosSetting/GetAllBranches/";
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
                    dTableManager.Border("#dTable", 350);
                    $('#tableElement_length').css({ 'float': 'right' });

                },
                buttons: [
                    {
                        text: '<i class="fas fa-print"></i> Print',
                        className: 'btn btn-sm',
                        extend: 'print',
                        exportOptions: {
                            columns: [2, 3, 4, 5, 6, 7, 8]
                        },
                        title: 'Branch List'
                    },
                    {
                        text: '<i class="far fa-file-excel"></i> Excel',
                        className: 'btn btn-sm',
                        extend: 'excelHtml5',
                        exportOptions: {
                            columns: [2, 3, 4, 5, 6, 7, 8]
                        },
                        title: 'Branch List'
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
                        title: '#',width:7,
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
                    { data: 'Name', name: 'Name', title: 'Name', align: 'center', width: 120 },
                    { data: 'Address', name: 'Address', title: 'Address',width: 280},
                    { data: 'Phone', name: 'Phone', title: 'Phone', width: 80 },
                    { data: 'Mobile', name: 'Mobile', title: 'Mobile', width: 80 },
                    { data: 'Email', name: 'Email', title: 'Email', width: 100 },
                    { data: 'Fax', name: 'Fax', title: 'Fax', width: 100 },
                    { data: 'Remarks', name: 'Remarks', title: 'Remarks', width: 100 }
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