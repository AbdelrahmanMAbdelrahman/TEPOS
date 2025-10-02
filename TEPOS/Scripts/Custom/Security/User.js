var _UsrId = null;
var dTable = null;

$(document).ready(function () {
    User.GetDataList(0);
   dTableManager.dTableSerialNumber(dTable);

    $("#btnSave").click(function () {
        User.Save();
    });

    $("#btnEdit").click(function () {
        User.Edit();
    });

    $("#btnAddUser").click(function () {
        $('#frmModal').modal('show');
        User.FormClear();
        $("#btnEdit").hide();
        $("#btnSave").show();

    });
});

$(document).on('click', '.dTableEdit', function () {
    var rowData = dTable.row($(this).parent()).data();
    var status = rowData['Status'];
    _UsrId = rowData.Id;
    $('#logName').val(rowData.LoginName);
    $('#UserCode').val(rowData.UserCode);
    $('#email').val(rowData.Email);
    $('#TerminalId').val(rowData.TerminalId);
    if (status == 1) {
        $('#stsActive').prop('checked', true);
    }
    else {
        $('#stsInactive').prop('checked', true);
    }
    $("#btnEdit").show();
    $("#btnSave").hide();
    $("#frmModal").modal('show');
});

$(document).on('click', '.dTableDelete', function () {
    var userId = dTable.row($(this).parent()).data().Id;
    User.Delete(userId);
});

var User = {
    Save: function() {
        var obj = [];
        obj.push({ 'Login Name': $("#logName").val() });
        obj.push({ 'Password': $("#logPass").val() });
        obj.push({ 'Confirm Password': $("#logconfirmPass").val() });
        obj.push({ 'Terminal ID': $("#TerminalId").val() });
        obj.push({ 'User Code': $("#UserCode").val() });
        if (JsManager.validate(obj)) {
            if (Message.Prompt()) {
                var form = $("#UsrPostId");
                var form2 = form.serialize();
                var jsonParam = form2;
                var serviceURL = "/Security/InsertUser/";
                JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
            }
        }

        function onSuccess(jsonData) {
            if (jsonData == "0") {
                Message.Error("save");
            } else {
                Message.Success("save");
                User.GetDataList(1);
                User.FormClear();
            }
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }

    },

    Edit: function() {
        if (_UsrId === null) {
            Message.Warning("Please Properly Select Record");
        } else {
            var obj = [];
            obj.push({ 'Login Name': $("#logName").val() });
            obj.push({ 'Password': $("#logPass").val() });
            obj.push({ 'Confirm Password': $("#logconfirmPass").val() });
            obj.push({ 'Terminal ID': $("#TerminalId").val() });
            obj.push({ 'User Code': $("#UserCode").val() });
            if (JsManager.validate(obj)) {
                if (Message.Prompt()) {
                    var jsonParam = $("#UsrPostId").serialize() + '&Id=' + _UsrId; 
                    var serviceUrl = "/Security/UpdateUser/";
                    JsManager.SendJson(serviceUrl, jsonParam, onSuccess, onFailed);
                }
            }
        }

        function onSuccess(jsonData) {
            if (jsonData == "0") {
                Message.Error("update");
            } else {
                Message.Success("update");
                User.GetDataList(1);
                User.FormClear();
                $("#frmModal").modal('hide');
            }
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

    Delete: function(userId) {

        if (Message.Prompt()) {
            var jsonParam = { Id: userId };
            var serviceURL = "/Security/DeleteUser/";
            JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
        }

        function onSuccess(jsonData) {
            if (jsonData == "0") {
                Message.Error("delete");
            } else {
                Message.Success("delete");
                User.GetDataList(1);
                User.FormClear();
            }
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },


    GetDataList: function (refresh) {
        var jsonParam = '';
        var serviceURL = "/Security/GetUsers";
        JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            User.LoadDataTable(jsonData, refresh);
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
                            columns: [2, 3, 4, 5, 6]
                        },
                        title: 'User List'
                    },
                    {
                        text: '<i class="fas fa-print"></i> Print',
                        className: 'btn btn-sm',
                        extend: 'print',
                        exportOptions: {
                            columns: [2, 3, 4, 5, 6]
                        },
                        title: 'User List'
                    },
                    {
                        text: '<i class="far fa-file-excel"></i> Excel',
                        className: 'btn btn-sm',
                        extend: 'excelHtml5',
                        exportOptions: {
                            columns: [2, 3, 4, 5, 6]
                        },
                        title: 'User List'
                    }
                ],

                scrollY: "350px",
                scrollX: true,
                scrollCollapse: true,
                lengthMenu: [[50, 100, 500, -1], [50, 100, 500, "All"]],
                columnDefs: [
                    { visible: false, targets: [] },
                    { "className": "dt-center", "targets": [3] }
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
                        data: 'LoginName',
                        name: 'LoginName',
                        title: 'Login Name'
                    },
                    {
                        data: 'UserCode',
                        name: 'UserCode',
                        title: 'User Code'
                    },
                    {
                        data: 'Email',
                        name: 'Email',
                        title: 'Email'
                    },
                    {
                        data: 'Company',
                        name: 'Company',
                        title: 'Company Name'
                    }, {
                        data: 'TerminalId',
                        name: 'TerminalId',
                        title: 'TerminalId'
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
    },

    FormClear: function() {
        $('#UsrPostId').trigger("reset");
        document.getElementById("stsActiv").checked = true;
        _UsrId = null;
    }
}


