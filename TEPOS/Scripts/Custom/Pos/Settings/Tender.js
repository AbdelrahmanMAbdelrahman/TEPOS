var dTable = null;
var _Id = 0;
$(document).ready(function () {
    Manager.GetDataForTable(0);
    Manager.LoadTenderTypeDDL();
    dTableManager.dTableSerialNumber(dTable);


    $("#addTenderType").click(function () {
        $("#PosTenderType_Name").val('');
        $("#btnSaveTenderType").removeClass('dN');
        $("#btnEditTenderType").addClass('dN');
        $("#frmModalTenderType").modal("show");
    });

    $("#editTenderType").click(function () {
        if ($("#PosTenderTypeId").val() == 0) {
            Message.Warning("Select an tender type for edit.");
        } else {
            $("#btnSaveTenderType").addClass('dN');
            $("#btnEditTenderType").removeClass('dN');
            $("#frmModalTenderType").modal("show");
            $("#PosTenderType_Name").val($("#PosTenderTypeId").find("option:selected").text());
        }
    });

    $("#btnSaveTenderType").click(function() {
        Manager.SaveTenderType();
    });
    $("#btnEditTenderType").click(function() {
        Manager.UpdateTenderType();
    });
});

$(document).on('click', '.dTableEdit', function () {
    JsManager.StartProcessBar();
    var $item = $(this).parents('tr');
    var row = dTable.row($item).data();

    _Id = row.Id;
    $("#PosTenderTypeId").val(row.PosTenderTypeId);
    $("#PosTenderTypeId").trigger("chosen:update");
    $("#PosTender_Type").val(row.Type);
    $("#Name").val(row.Name);
    $("#Order").val(row.Order);

    JsManager.EndProcessBar();
    
});

$(document).on('click', '#btnSave', function () {
    Manager.Save();
});

$(document).on('click', '#btnEdit', function () {
    Manager.Update();
});

$(document).on('click', '.dTableDelete', function () {
    var $item = $(this).parents('tr');
    var row = dTable.row($item).data();
    Manager.Delete(row.Id);
});

$(document).on('click', '#btnClear', function () {
    Manager.FormReset();
});


var Manager = {

    FormReset: function () {
        _Id = 0;
        $("#tenderForm")[0].reset();
    },

    LoadTenderTypeDDL: function () {
        var jsonParam = "";
        var serviceURL = "/PosDropdownSetting/GetTenders/";
        JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);
        function onSuccess(jsonData) {
            JsManager.PopulateCombo('#PosTenderTypeId', jsonData, 'Select Type',0);
        }
        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

    SaveTenderType: function () {
        var obj = [];
        obj.push({ 'Type Name': $("#TenderType_Name").val() });
        if (JsManager.validate(obj)) {
            if (Message.Prompt()) {
                var jsonParam = $('#frmPostTenderType').serialize();
                var serviceURL = "/PosSetting/SaveTenderType/";
                JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
            }
            function onSuccess(jsonData) {
                if (jsonData == "0") {
                    Message.Error("save");
                } else {
                    Message.Success("save");
                  Manager.LoadTenderTypeDDL();
                    $("PosTenderTypeId").val(jsonData);
                    $("#frmModalTenderType").modal('hide');
                }
            }

            function onFailed(xhr, status, err) {
                Message.Exception(xhr);
            }
        }
    },
    UpdateTenderType: function () {
        var obj = [];
        var tenderTypeId = $("#PosTenderTypeId").val();
        obj.push({ 'Type Name': $("#TenderType_Name").val() });
        if (JsManager.validate(obj)) {
            if (Message.Prompt()) {
                var jsonParam = $('#frmPostTenderType').serialize() + '&Id=' + tenderTypeId;
                var serviceURL = "/PosSetting/UpdateTenderType/";
                JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
            }
            function onSuccess(jsonData) {
                if (jsonData == "0") {
                    Message.Error("update");
                } else {
                    Manager.LoadTenderTypeDDL();
                    Message.Success("update");
                    $("#PosTenderTypeId").val(tenderTypeId);
                    $("#frmModalTenderType").modal('hide');
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
        obj.push({ 'Order': $("#Order").val() });
        if (JsManager.validate(obj)) {
            if (Message.Prompt()) {
                var jsonParam = $('#tenderForm').serialize();
                var serviceURL = "/PosSetting/SaveTender/";
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

    Update: function () {
        var obj = [];
        obj.push({ 'Name': $("#Name").val() });
        obj.push({ 'Order': $("#Order").val() });
        if (JsManager.validate(obj)) {
            if (Message.Prompt()) {
                var jsonParam = $('#tenderForm').serialize() + '&id=' + _Id;
                var serviceURL = "/PosSetting/UpdateTender/";
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

    Delete: function (tenderId) {
        if (Message.Prompt()) {
            var jsonParam = { tenderId: tenderId };
            var serviceURL = "/PosSetting/DeleteTender/";
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

    GetDataForTable: function (ref) {
        var jsonParam = '';
        var serviceURL = "/PosSetting/GetTenders/";
        JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);
        function onSuccess(jsonData) {
            Manager.LoadDataTable(jsonData, ref);
        }
        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

    LoadDataTable: function(userdata, isRef) {
        if (isRef == "0") {
            dTable = $('#dTable').DataTable({
                dom: "<'row'<'col-md-4'B><'col-md-3'l><'col-md-5'f>>" + "<'row'<'col-md-12'tr>>" + "<'row'<'col-md-5'i><'col-md-7 mt-7'p>>",
                initComplete: function() {
                    dTableManager.Border("#dTable", 350);
                    $('#tableElement_length').css({ 'float': 'right' });
                },
                buttons: [
                    {
                        text: '<i class="fas fa-print"></i> Print',
                        className: 'btn btn-sm',
                        extend: 'print',
                        exportOptions: {
                            columns: [2, 3, 4]
                        },
                        title: 'Payment method List'
                    },
                    {
                        text: '<i class="far fa-file-excel"></i> Excel',
                        className: 'btn btn-sm',
                        extend: 'excelHtml5',
                        exportOptions: {
                            columns: [2, 3, 4]
                        },
                        title: 'Payment method List'
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
                        title: '#',
                        width: 7,
                        render: function(data, type, row, meta) {
                            return '';
                        }
                    },
                    {
                        name: 'Option',
                        title: 'Option',
                        width: 80,
                        render: function(data, type, row) {
                            var icon = EventManager.DataTableCommonButton();
                            if (row.IsEditable == false) {
                                icon = '<div class="text-center"><span style="cursor:not-allowed" class="btn btn-warning btn-xs" title="Can not be edited or deleted"><i class="glyphicon glyphicon-ok-sign"></i> Fixed</span></div>';
                            }
                            return icon;
                        }
                    },
                    { data: 'TenderName', name: 'TenderName', title: 'Payment Type', align: 'center', width: 300 },
                    { data: 'Name', name: 'Name', title: 'Payment method', width: 420 },
                    { data: 'Order', name: 'Order', title: 'Order', width: 150 },
                    {
                        data: 'Type',name: 'Type',title: 'Type',width: 150,
                        render: function(data, type, row) {
                            var rtr = "";
                            switch (data) {
                            case "CR":
                                rtr = "Cash";
                                break;
                            case "DI":
                                rtr = "Discount";
                                break;
                            default:
                                rtr = "Default";
                            }
                            return rtr;
                        }
                    }
                ],
                fixedColumns: true,
                data: userdata,
                rowsGroup: null
            });

        } else {
            dTable.clear().rows.add(userdata).draw();

        }

    }

}