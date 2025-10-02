var _userId = null;
var data = null;
var dTable = null;

$(document).ready(function () {

    Manager.GetDataForTable(0);

    dTable.on('order.dt search.dt', function () {
        dTable.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = '<div align="center">' + (i + 1) + '<div>';
        });
    }).draw();

    $("#btnSave").click(function () {
        Manager.Save();
    });

    $("#btnEdit").click(function () {
        Manager.Update(data);

    });

    Manager.LoadToUser();
    Manager.LoadToOffice();

    $("#btnAddNew").click(function () {
        $('#frmModal').modal('show');
        Manager.Reset();
        $("#btnEdit").hide();
        $("#btnSave").show();
    });
});

var Manager = {

    LoadToUser: function () {
        var jsonParam = '';
        var serviceURL = "/PosDropDown/GetUser/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
        function onSuccess(jsonData) {
            var objProgram = jsonData;
            JsManager.PopulateCombo('#cmbUser', objProgram, 'Select User',0);
            $("#cmbUser").chosen();
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
    },

    LoadToOffice: function () {
        var jsonParam = '';
        var serviceURL = "/PosDropDown/GetBranch/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
        function onSuccess(jsonData) {
            var objProgram = jsonData;
            JsManager.PopulateCombo('#cmbOffice', objProgram, '');
            $("#cmbOffice").chosen();
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
    },

    Save: function () {
        if ($("#cmbUser").val() == "") {
            Message.Warning("Please select user");
        }
        else if (!$('#cmbOffice').val()) {
            Message.Warning("At least one office");
        }
        else if (Message.Prompt()) {
            var serviceURL = '/Security/InsertUserOffice/';
            var jsonParam = $('#resourceDpt').serialize() + "&offices=" + $("#cmbOffice").val() + "&userId=" + $("#cmbUser").val();
            JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

            function onSuccess(jsonData) {
                if (jsonData == "0") {
                    Message.Error("save");
                } else {
                    Message.Success("save");
                    Manager.GetDataForTable(1);
                    Manager.Reset();
                }
            }

            function onFailed(xhr, status, err) {
                Message.Exception(xhr);
            }
        }
    },

    Update: function (rowData) {
        if ($("#cmbUser").val() == "0" || $("#cmbUser").val() == "") {
            Message.Warning("Please select a row to update");
        }
        else if (!$('#cmbOffice').val()) {
            Message.Warning("At least one office");
        }
        else {
            if (Message.Prompt()) {

                var existingOfc = [];
                $.each(rowData.Offices, function (index, value) {
                    existingOfc.push(value.OfficeId);
                });

                var jsonParam = $("#resourceDpt").serialize() + "&existingOfc=" + existingOfc +"&updatedOfc=" + $('#cmbOffice').val() + "&existingUserId=" + rowData.UserId;
                var serviceUrl = '/Security/UpdateUserOffice/';
                JsManager.SendJson(serviceUrl, jsonParam, onSuccess, onFailed);
            }
            function onSuccess(jsonData) {
                if (jsonData == "0") {
                    Message.Error("update");
                }

                else {
                    Message.Success("update");
                    Manager.GetDataForTable(1);
                    Manager.Reset();
                    $("#frmModal").modal('hide');
                }
            }

            function onFailed(xhr, status, err) {
                Message.Exception(xhr);
            }

        }
    },

    Delete: function () {
        if (_userId == null) {
            Message.Warning("Select a row to delete");
        }
        else {
            if (Message.Prompt()) {
                var jsonParam = { userId: _userId };
                var serviceURL = "/Security/DeleteUserOffice/";
                JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
            }
        }

        function onSuccess(jsonData) {
            if (jsonData == "0") {
                Message.Error("delete");
            }

            else {
                Message.Success("delete");
                Manager.Reset();
                Manager.GetDataForTable(1);
            }
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

    Reset: function () {
        data = null;
        _userId = null;
        $('#cmbUser option').attr('disabled', false);
        $("#resourceDpt")[0].reset();
        $("#cmbUser").trigger("chosen:updated");
        $("#cmbOffice").trigger("chosen:updated");
        $("#cmbDepartment").trigger("chosen:updated");
    },

    GetDataForTable: function (refresh) {
        var jsonParam = '';
        var serviceURL = "/Security/GetUserOffice/";
        JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);
        function onSuccess(jsonData) {
            Manager.LoadDataTable(jsonData, refresh);
        }
        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

    LoadDataTable: function (data, refresh) {
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
                            columns: [2,3]
                        },
                        title: 'User Branch List'
                    },
                        {
                            text: '<i class="fas fa-print"></i> Print',
                            className: 'btn btn-sm',
                            extend: 'print',
                            exportOptions: {
                                columns: [2, 3]
                            },
                            title: 'User Branch List'
                        },

                        {
                            text: '<i class="far fa-file-excel"></i> Excel',
                            className: 'btn btn-sm',
                            extend: 'excelHtml5',
                            exportOptions: {
                                columns: [2, 3]
                            },
                            title: 'User Branch List'
                        }
                ],

                scrollY: "350px",
                scrollX: true,
                scrollCollapse: true,
                lengthMenu: [[50, 100, 500, -1], [50, 100, 500, "All"]],
                columnDefs: [
                    { visible: false, targets: [] },
                    { "className": "dt-center", "targets": [] }
                ],
                columns: [

                    {
                        data: null,
                        name: '',
                        'orderable': false,
                        'searchable': false,
                        title: '',
                        width: 8,
                        render: function () {
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
                        title: 'User Name'
                    },
                    {
                        data: 'Offices',
                        name: 'Offices',
                        title: 'Office',
                        render: function (data, type, row) {
                            var rtr = '';
                            $.each(row.Offices, function (index, value) {
                                rtr += value.OfficName + ", ";
                            });
                            return rtr.substr(0, rtr.length - 2);
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

$(document).on('click', '.dTableEdit', function () {
    Manager.Reset();

    var rowData = dTable.row($(this).parent()).data();
    $('#cmbUser').val(rowData.UserId);
    $('#cmbUser option:not(:selected)').attr('disabled', true);
    $('#cmbUser').trigger("chosen:updated");

    JsManager.StartProcessBar();

    setTimeout(function () {
        $.each(rowData.Offices, function (index, value) {
            $('#cmbOffice').find('option[value="' + value.OfficeId + '"]').prop('selected', true);
        });
        $('#cmbOffice').trigger("chosen:updated");
        JsManager.EndProcessBar();
    }, 500);

    data = rowData;

    $("#btnEdit").show();
    $("#btnSave").hide();
    $("#frmModal").modal('show');
});

$(document).on('click', '.dTableDelete', function () {
    _userId = dTable.row($(this).parent()).data().UserId;
    Manager.Delete();
});
