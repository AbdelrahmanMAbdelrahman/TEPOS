var dTable = null;
var ugId = null;

$(document).ready(function () {

    Manager.GetDataForTable(0);
    dTableManager.dTableSerialNumber(dTable);
    Manager.LoadUnitGroupDetailDDL();
    Manager.LoadUnitGroupMasterDDL();

    $("#addUomGroup").click(function () {
        $("#btnSaveUomGroup").removeClass('dN');
        $("#btnEditUomGroup").addClass('dN');
        $("#frmModalUomGroup").modal("show");
        $("#UomGroup_Name").val('');
    });
    $("#editUomGroup").click(function () {
        if ($("#PosUomGroupId").val() > 0) {
            $("#btnEditUomGroup").removeClass('dN');
            $("#btnSaveUomGroup").addClass('dN');
            $("#frmModalUomGroup").modal("show");
            $("#UomGroup_Name").val($("#PosUomGroupId").find("option:selected").text());
        } else {
            Message.Warning("Select an Uom group for edit");
        }
        
    });

    $("#btnSaveUomGroup").click(function() {
        Manager.SaveUnitGroup();
    });
    $("#btnEditUomGroup").click(function () {
        Manager.UpdateUnitGroup();
    });

});

$(document).on('click', '#btnSave', function () {
    Manager.Save();
});

$(document).on('click', '.dTableEdit', function () {
    JsManager.StartProcessBar();
    var $item = $(this).parents('tr');
    var row = dTable.row($item).data();

    ugId = row.Id;
    $("#PosUomGroupId").val(row.PosUomGroupId);
    $("#PosUomMasterId").val(row.PosUomMasterId);
    $("#ConversionFactor").val(row.ConversionFactor);

    JsManager.EndProcessBar();
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
    SaveUnitGroup: function () {
        var obj = [];
        obj.push({ 'Uom Group': $("#UomGroup_Name").val() });
        if (JsManager.validate(obj)) {
            if (Message.Prompt()) {
                var jsonParam = $('#frmUomGroupPost').serialize();
                var serviceURL = "/PosSetting/SaveUnitGroup/";
                JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
            }
            function onSuccess(jsonData) {
                if (jsonData == "0") {
                    Message.Error("save");
                } else {
                    Message.Success("save");
                    Manager.LoadUnitGroupDetailDDL();
                    $("#PosUomGroupId").val(jsonData);
                    $("#frmModalUomGroup").modal("hide");
                }
            }

            function onFailed(xhr, status, err) {
                Message.Exception(xhr);
            }
        }
    },
    UpdateUnitGroup: function () {
        var obj = [];
        obj.push({ 'Unit group': $("#PosUomGroupId").val() });
        var uomGroupId = $("#PosUomGroupId").val();
        if (JsManager.validate(obj)) {
            if (Message.Prompt()) {
                var jsonParam = $('#frmUomGroupPost').serialize() + '&id=' + uomGroupId;
                var serviceURL = "/PosSetting/UpdateUnitGroup/";
                JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
            }
            function onSuccess(jsonData) {
                if (jsonData == "0") {
                    Message.Error("update");
                } else {
                    Message.Success("update");
                    Manager.LoadUnitGroupDetailDDL();
                    $("#PosUomGroupId").val(uomGroupId);
                    $("#frmModalUomGroup").modal("hide");
                }
            }

            function onFailed(xhr, status, err) {
                Message.Exception(xhr);
            }
        }
    },
    Save: function () {
        var obj = [];
        obj.push({ 'PosUomGroup': $("#PosUomGroupId").val() });
        obj.push({ 'PosUomMaster': $("#PosUomMasterId").val() });
        obj.push({ 'ConversionFactor': $("#ConversionFactor").val() });
        if (JsManager.validate(obj)) {
            if (Message.Prompt()) {
                var jsonParam = $('#unitConversionForm').serialize();
                var serviceURL = "/PosSetting/SaveUnitGroupDetails/";
                JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
            }
            function onSuccess(jsonData) {
                if (jsonData == "0") {
                    Message.Error("save");
                } else {
                    Message.Success("save");
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
        obj.push({ 'Unit group': $("#PosUomGroupId").val() });
        obj.push({ 'Unit master ': $("#PosUomMasterId").val() });
        obj.push({ 'Conversion factor': $("#ConversionFactor").val() });
        if (JsManager.validate(obj)) {
            if (Message.Prompt()) {
                var jsonParam = $('#unitConversionForm').serialize() + '&id=' + ugId;
                var serviceURL = "/PosSetting/UpdateUnitGroupDetails/";
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

    Delete: function (uGroupId) {
        if (Message.Prompt()) {
            var jsonParam = { ugId: uGroupId };
            var serviceURL = "/PosSetting/DeleteUnitGroupDetails/";
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

    LoadUnitGroupDetailDDL: function () {
        var jsonParam = "";
        var serviceURL = "/PosDropdownSetting/GetUnitGroupDetails/";
        JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);
        function onSuccess(jsonData) {
            JsManager.PopulateCombo('#PosUomGroupId', jsonData, 'Select Unit Group',0);
        }
        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

    LoadUnitGroupMasterDDL: function () {
        var jsonParam = "";
        var serviceURL = "/PosDropdownSetting/GetUnitGroupMasters/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
        function onSuccess(jsonData) {
            JsManager.PopulateCombo('#PosUomMasterId', jsonData, 'Select Unit Group Master',0);
        }
        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

    GetDataForTable: function (ref) {
        var jsonParam = '';
        var serviceURL = "/PosSetting/GetUnitGroupDetails/";
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
                    initComplete: function() {
                        dTableManager.Border("#dTable", 250);
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
                            title: 'Unit Conversion List'
                        },
                        {
                            text: '<i class="far fa-file-excel"></i> Excel',
                            className: 'btn btn-sm',
                            extend: 'excelHtml5',
                            exportOptions: {
                                columns: [2, 3, 4]
                            },
                            title: 'Unit Conversion List'
                        }
                    ],
                    scrollY: "250px",
                    scrollX: true,
                    scrollCollapse: true,
                    lengthMenu: [[100, 250, 500, -1], [100, 250, 500, "All"]],
                    columnDefs: [
                        { visible: false, targets: [] },
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
                        title: 'Option',
                        width: 80,
                        render: function(data, type, row) {
                            return EventManager.DataTableCommonButton();
                        }
                    },
                    { data: 'GroupName', name: 'GroupName', title: 'Unit Group', align: 'center', width: 250 },
                    { data: 'UnitMasterCode', name: 'UnitMasterCode', title: 'Unit Master Code', width: 250 },
                    { data: 'ConversionFactor', name: 'ConversionFactor', title: 'Conversion Factor', width: 150 }, 
                ],
                fixedColumns: true,
                data: userdata,
                rowsGroup: null
            });

        } else {
            dTable.clear().rows.add(userdata).draw();

        }

    },

    FormReset: function () {
        ugId = null;
        $("#unitConversionForm")[0].reset();
    },
}