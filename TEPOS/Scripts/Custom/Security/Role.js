var _roleId = null;
var dTable = null;

$(document).ready(function () {
    Role.GetDataList(0);

    dTableManager.dTableSerialNumber(dTable);

    $("#stsActive").prop("checked", true);

    $("#btnSave").click(function () {
        Role.Save();
    });

    $("#btnEdit").click(function () {
        Role.Edit();
    });

    $("#btnAddRole").click(function () {
        $('#frmModal').modal('show');
        Role.FormClear();
        $("#btnEdit").hide();
        $("#btnSave").show();

    });
});

$(document).on('click', '.dTableEdit', function () {
    var rowData = dTable.row($(this).parent()).data();
    var status = rowData['Status'];

    _roleId = rowData.Id;
    $('#Name').val(rowData.Name);

    if (status == 1) {
        $('#stsActive').prop('checked', true);
    }
    else {
        $('#stsInactive').prop('checked', true);
    }
    $("#btnSave").hide();
    $("#btnEdit").show();
    $("#frmModal").modal('show');
});

$(document).on('click', '.dTableDelete', function () {
    var roleId = dTable.row($(this).parent()).data().Id;
    Role.Delete(roleId);
});


var Role = {
    GetDataList: function(refresh) {
        var jsonParam = '';
        var serviceURL = "/Security/GetRole/";
        JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            Role.LoadDataTable(jsonData, refresh);
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

    Save: function() {
        var obj = [];
        obj.push({ 'Role': $("#Name").val() });
        if (JsManager.validate(obj)) {
            if (Message.Prompt()) {
                var jsonParam = $("#fmrPost").serialize();
                var serviceURL = "/Security/InsertRole/";
                JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
            }
        }

        function onSuccess(jsonData) {
            if (jsonData == "0") {
                Message.Error("save");
            } else {
                Message.Success("save");
                Role.GetDataList(1);
                Role.FormClear();
            }
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

   Edit: function() {
       var obj = [];
       obj.push({ 'Role': $("#Name").val() });
       if (JsManager.validate(obj)) {
            if (Message.Prompt()) {
                var jsonParam = $("#fmrPost").serialize() + '&Id=' + _roleId;
                var serviceURL = "/Security/UpdateRole/";
                JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
            }
        }

        function onSuccess(jsonData) {
            if (jsonData == "0") {
                Message.Error("update");
            } else {
                Message.Success("update");
                Role.GetDataList(1);
                Role.FormClear();
                $("#frmModal").modal('hide');
            }
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

    Delete: function(roleId) {
        if (roleId == null) {
            Message.Warning("Please Properly Select Record");
        } else {
            if (Message.Prompt()) {
                var jsonParam = { Id: roleId };
                var serviceURL = "/Security/DeleteRole/";
                JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
            }
        }

        function onSuccess(jsonData) {
            if (jsonData == "0") {
                Message.Error("delete");
            } else {
                Message.Success("delete");
                Role.GetDataList(1);
                Role.FormClear();

            }
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

    FormClear: function() {
        $('#fmrPost').trigger("reset");
        _roleId = null;
        $("#stsActive").prop("checked", true);
    },

    LoadDataTable: function(data, refresh) {
        if (refresh == "0") {
            dTable = $('#tableElement').DataTable({
                dom: "<'row'<'col-md-6'B><'col-md-3'l><'col-md-3'f>>" + "<'row'<'col-md-12'tr>>" + "<'row'<'col-md-5'i><'col-md-7 mt-7'p>>",
                initComplete: function () {
                    dTableManager.Border("#tableElement", 350);
                   $('#tableElement_length').css({ 'float': 'right' });
                },
                buttons: [
                    {
                        text: '<i class="far fa-file-pdf"></i> PDF',
                        className: 'btn btn-sm',
                        extend: 'pdfHtml5',
                        exportOptions: {
                            columns: [ 2,3]
                        },
                        title: 'Role List'
                    },
                    {
                        text: '<i class="fas fa-print"></i> Print',
                        className: 'btn btn-sm',
                        extend: 'print',
                        exportOptions: {
                            columns: [2, 3]
                        },
                        title: 'Role List'
                    },
                    {
                        text: '<i class="far fa-file-excel"></i> Excel',
                        className: 'btn btn-sm',
                        extend: 'excelHtml5',
                        exportOptions: {
                            columns: [2, 3]
                        },
                        title: 'Role List'
                    }
                ],

                scrollY: "350px",
                scrollX: true,
                scrollCollapse: true,
                lengthMenu: [[50, 100, 500, -1], [50, 100, 500, "All"]],
                columnDefs: [
                    { visible: false, targets: [] },
                    { "className": "dt-center", "targets": [2] }
                ],
                columns: [
                    {
                        data: null,
                        name: '',
                        'orderable': false,
                        'searchable': false,
                        title: '',
                        width: 8,
                        render: function() {
                            return '';
                        }
                    },
                    {
                        name: 'Option',
                        title: 'Option',
                        width: 80,
                        render: function(data, type, row) {
                            return EventManager.DataTableCommonButton();
                        }
                    },
                    {
                        data: 'Name',
                        name: 'Name',
                        title: 'Role'
                    },
                    {
                        data: 'Status',
                        name: 'Status',
                        title: 'Status',
                        render: function(data, type, row) {
                            var status = data ? "Active" : "Inactive";
                            return status;
                        }
                    }
                ],
                fixedColumns: true,
                data: data
            });
        } else {
            dTable.clear().rows.add(data).draw();
        }
    }
}




