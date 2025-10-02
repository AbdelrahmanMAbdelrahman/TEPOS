var _usrRoleId = null;
var dTable = "";

$(document).ready(function () {

    UserRole.GetDataList(0);
    dTableManager.dTableSerialNumber(dTable);

    UserRole.UserDropDown();
    UserRole.RoleDropDown();

    $("#stsActive").prop("checked", true);

    $("#btnSave").click(function () {
        UserRole.Save();
    });

    $("#btnEdit").click(function () {
        UserRole.Edit();
    });
    $("#btnAddNew").click(function () {
        $('#frmModal').modal('show');
        UserRole.FormClear();
        $("#btnEdit").hide();
        $("#btnSave").show();

    });
});

$(document).on('click', '.dTableEdit', function () {
    var rowData = dTable.row($(this).parent()).data();
    var status = rowData['Status'];

    _usrRoleId = rowData.Id;
    $('#SecUserId').val(rowData.UserId);
    $('#SecRoleId').val(rowData.RoleId);
    if (status == 1) {
        $('#stsActive').prop('checked', true);}
    else {
        $('#stsInactive').prop('checked', true);
    }

    $("#btnEdit").show();
    $("#btnSave").hide();
    $("#frmModal").modal('show');
});

$(document).on('click', '.dTableDelete', function () {
    var userId = dTable.row($(this).parent()).data().Id;
    UserRole.Delete(userId);
});




var UserRole = {
    UserDropDown: function() {
        var jsonParam = '';
        var serviceURL = "/PosDropDown/GetUser/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            var objProgram = jsonData;
            JsManager.PopulateCombo('#SecUserId', objProgram, 'Select User',0);

        }

        function onFailed(error) {
            window.alert(error.statusText);
        }
    },

    RoleDropDown: function() {
        var jsonParam = '';
        var serviceUrl = "/PosDropDown/GetRole/";
        JsManager.SendJson(serviceUrl, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            var objProgram = jsonData;
            JsManager.PopulateCombo('#SecRoleId', objProgram, 'Select Role',0);

        }

        function onFailed(error) {
            window.alert(error.statusText);
        }
    },

    Save: function() {
        var obj = [];
        obj.push({ 'User': $("#SecUserId").val() });
        obj.push({ 'Role': $("#SecRoleId").val() });
        if (JsManager.validate(obj)) {
            if (Message.Prompt()) {
                var jsonParam = $("#frmPost").serialize();
                var serviceUrl = "/Security/InsertUserRole/";
                JsManager.SendJson(serviceUrl, jsonParam, onSuccess, onFailed);
            }
        }

        function onSuccess(jsonData) {
            if (jsonData == "0") {
                Message.Error("save");
            } else {
                Message.Success("save");
                UserRole.GetDataList(1);
                UserRole.FormClear();
            }
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

    Edit: function() {
        if (_usrRoleId == null) {
            Message.Warning("Please Properly Select Record");
        } else {
            var obj = [];
            obj.push({ 'User': $("#SecUserId").val() });
            obj.push({ 'Role': $("#SecRoleId").val() });
            if (JsManager.validate(obj)) {
                if (Message.Prompt()) {
                    var jsonParam = $("#frmPost").serialize() + '&Id=' + _usrRoleId;
                    var serviceUrl = "/Security/UpdateUserRole/";
                    JsManager.SendJson(serviceUrl, jsonParam, onSuccess, onFailed);
                }
            }
        }

        function onSuccess(jsonData) {
            if (jsonData == "0") {
                Message.Error("update");
            } else {
                Message.Success("update");
                UserRole.GetDataList(1);
                UserRole.FormClear();
                $("#frmModal").modal('hide');
            }
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

    Delete: function(userId) {
        if (userId === "0") {
            Message.Warning("Please Properly Select Record");
        } else {
            if (Message.Prompt()) {
                var jsonParam = { Id: userId };
                var serviceUrl = "/Security/DeleteUserRole/";
                JsManager.SendJson(serviceUrl, jsonParam, onSuccess, onFailed);
            }
        }

        function onSuccess(jsonData) {
            if (jsonData == "0") {
                Message.Error("delete");
            } else {
                Message.Success("delete");
                UserRole.GetDataList(1);
                UserRole.FormClear();
            }
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

    FormClear: function() {
        $('#frmPost').trigger("reset");
        _usrRoleId = null;
        $("#stsActive").prop("checked", true);
    },

    GetDataList: function(refresh) {
        var jsonParam = '';
        var serviceUrl = "/Security/GetUserRole/";
        JsManager.SendJsonAsyncON(serviceUrl, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            UserRole.LoadDataTable(jsonData, refresh);
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

    LoadDataTable: function(data, refresh) {
        if (refresh == "0") {
            dTable = $('#tableElement').DataTable({
                dom: "<'row'<'col-md-6'B><'col-md-3'l><'col-md-3'f>>" + "<'row'<'col-md-12'tr>>" + "<'row'<'col-md-5'i><'col-md-7 mt-7'p>>",
                initComplete: function() {
                    dTableManager.Border("#tableElement", 350);
                    $('#tableElement_length').css({ 'float': 'right' });
                },
                buttons: [
                    
                    {
                        text: '<i class="far fa-file-pdf"></i> PDF',
                        className: 'btn btn-sm',
                        extend: 'pdfHtml5',
                        exportOptions: {
                            columns: [2, 3, 4]
                        },
                        title: 'User Role List'
                    },
                    {
                        text: '<i class="fas fa-print"></i> Print',
                        className: 'btn btn-sm',
                        extend: 'print',
                        exportOptions: {
                            columns: [2, 3, 4]
                        },
                        title: 'User Role List'
                    },
                    {
                        text: '<i class="far fa-file-excel"></i> Excel',
                        className: 'btn btn-sm',
                        extend: 'excelHtml5',
                        exportOptions: {
                            columns: [2, 3, 4]
                        },
                        title: 'User Role List'
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
                        data: 'UserName',
                        name: 'UserName',
                        title: 'User Name',
                        width: 400
                    },
                    {
                        data: 'RoleName',
                        name: 'RoleName',
                        title: 'Role Name',
                        width: 450
                    },
                    {
                        data: 'Status',
                        name: 'Status',
                        title: 'Status',
                        width: 50,
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

