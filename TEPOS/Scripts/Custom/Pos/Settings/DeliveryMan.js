var dTable = null;
var _DeliveryManId = null;

$(document).ready(function () {
    Manager.GetDataForTable(0);
    Manager.LoadDeliveryManClassDDL();
    Manager.LoadDeliveryManRegionDDL();
    Manager.LoadDeliveryManNearestCityDDL();
    Manager.LoadAllBranch();

    //Datatable serial
    dTable.on('order.dt search.dt', function () {
        dTable.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = dTableManager.IndexColumn(i + 1);
        });
    }).draw();


    $("#addCityOrNearestZone").click(function() {
        $("#btnSaveCityOrNearestZone").removeClass('dN');
        $("#btnEditCityOrNearestZone").addClass('dN');
        $("#frmModalCityOrNearestZone").modal("show");
        $("#PosCityOrNearestZone_Name").val('');
    });
    $("#editaddCityOrNearestZone").click(function () {
        if ($("#PosCityOrNearestZoneId").val() > 0) {
            $("#btnEditCityOrNearestZone").removeClass('dN');
            $("#btnSaveCityOrNearestZone").addClass('dN');
            $("#frmModalCityOrNearestZone").modal("show");
            $("#PosCityOrNearestZone_Name").val($("#PosCityOrNearestZoneId").find("option:selected").text());
        } else {
            Message.Warning("Select a City Or Nearest Zone");
        }
    });

    $("#btnSaveCityOrNearestZone").click(function () {
        Manager.SaveCityZone();
    });
    $("#btnEditCityOrNearestZone").click(function () {
        Manager.UpdateCityZone();
    });

    $("#addDeliveryManRegion").click(function () {
        $("#btnSaveRegion").removeClass('dN');
        $("#btnEditRegion").addClass('dN');
        $("#frmModalPosRegion").modal("show");
        $("#PosCityOrNearestZone_Name").val('');
    });
    $("#editDeliveryManRegion").click(function () {
        if ($("#PosRegionId").val() > 0) {
            $("#btnEditRegion").removeClass('dN');
            $("#btnSaveRegion").addClass('dN');
            $("#frmModalPosRegion").modal("show");
            $("#PosRegion_Name").val($("#PosRegionId").find("option:selected").text());
        } else {
            Message.Warning("Select a Region for edit");
        }
    });

    $("#btnSaveRegion").click(function () {
        Manager.SaveRegion();
    });
    $("#btnEditRegion").click(function () {
        Manager.UpdateRegion();
    });

    $("#addDeliveryManClass").click(function () {
        $("#btnSaveDeliveryManClass").removeClass('dN');
        $("#btnEditDeliveryManClass").addClass('dN');
        $("#frmModalDeliveryManClass").modal("show");
        $("#PosDeliveryManClass_Name").val('');
    });
    $("#editDeliveryManClass").click(function () {
        if ($("#PosDeliveryManClassId").val() > 0) {
            $("#btnEditDeliveryManClass").removeClass('dN');
            $("#btnSaveDeliveryManClass").addClass('dN');
            $("#frmModalDeliveryManClass").modal("show");
            $("#PosDeliveryManClass_Name").val($("#PosDeliveryManClassId").find("option:selected").text());
        } else {
            Message.Warning("Select a DeliveryMan class for edit");
        }
    });

    $("#btnSaveDeliveryManClass").click(function () {
        Manager.SaveDeliveryManClass();
    });
    $("#btnEditDeliveryManClass").click(function () {
        Manager.UpdateDeliveryManClass();
    });
    
    $("#IsPosBranchDeliveryMan").click(function () {
        if ($(this).prop("checked") == true) {
            $("#IsDefaultPosDeliveryMan").prop("checked", false);
        }
    });
    $("#IsDefaultPosDeliveryMan").click(function () {
        if ($(this).prop("checked") == true) {
            $("#IsPosBranchDeliveryMan").prop("checked", false);
        }
    });

});

$(document).on('click', '.dTableEdit', function () {
    JsManager.StartProcessBar();
    var $item = $(this).parents('tr');
    var row = dTable.row($item).data();

    _DeliveryManId = row.Id;
    $("#PosDeliveryManClassId").val(row.PosDeliveryManClassId);
    $("#PosDeliveryManClassId").trigger("chosen:updated");
    $("#Phone").val(row.Phone);
    $("#DeliveryManName_en").val(row.DeliveryManName_en);
    $("#PosBranchId").val(row.PosBranchId);
    $("#DeliveryManName_ar").val(row.DeliveryManName_ar);
    $("#AdditionalPhone").val(row.AdditionalPhone);
    $("#Address").val(row.Address);
    //$("#DeliveryManCode").val(row.DeliveryManCode);
    $("#NationalID").val(row.NationalID);
    $("#PosRegionId").val(row.PosRegionId);
    $("#PosCityOrNearestZoneId").val(row.PosCityOrNearestZoneId);
    $("#IsPointAllowable").prop('checked', row.IsPointAllowable);
    $("#IsDueAllowable").prop('checked', row.IsDueAllowable);
    $("#IsDefaultPosDeliveryMan").prop('checked', row.IsDefaultPosDeliveryMan);
    $("#IsPosBranchDeliveryMan").prop("checked",row.IsPosBranchDeliveryMan);

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

$(document).on('click', '.dTableDelete', function () {
    var $item = $(this).parents('tr');
    var row = dTable.row($item).data();
    Manager.Delete(row.Id);
});


var Manager = {
        UpdateDeliveryManClass: function () {
            var obj = [];
            var id = $("#PosDeliveryManClassId").val();
        obj.push({ 'DeliveryMan Class': $("#PosDeliveryManClass_Name").val() });
        if (JsManager.validate(obj)) {
            if (Message.Prompt()) {
                var jsonParam = $('#frmPostDeliveryManClass').serialize() + "&id=" + id;
                var serviceURL = "/PosSetting/UpdateDeliveryManClass/";
                JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
            }

            function onSuccess(jsonData) {
                if (jsonData == "0") {
                    Message.Error("update");
                } else {
                    Message.Success("update");
                    Manager.LoadDeliveryManClassDDL();
                    $("#PosDeliveryManClassId").val(id);
                    $("#frmModalDeliveryManClass").modal("hide");
                }
            }

            function onFailed(xhr, status, err) {
                Message.Exception(xhr);
            }
        }

    },
    SaveDeliveryManClass: function () {
        var obj = [];
        obj.push({ 'DeliveryMan Class': $("#PosDeliveryManClass_Name").val() });
        if (JsManager.validate(obj)) {
            if (Message.Prompt()) {
                var jsonParam = $('#frmPostDeliveryManClass').serialize();
                var serviceURL = "/PosSetting/SaveDeliveryManClass/";
                JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
            }

            function onSuccess(jsonData) {
                if (jsonData == "0") {
                    Message.Error("save");
                } else {
                    Message.Success("save");
                    Manager.LoadDeliveryManClassDDL();
                    $("#PosDeliveryManClassId").val(jsonData);
                    $("#frmModalDeliveryManClass").modal("hide");
                }
            }

            function onFailed(xhr, status, err) {
                Message.Exception(xhr);
            }
        }

    },
    SaveRegion: function () {
        var obj = [];
        obj.push({ 'Region': $("#PosRegion_Name").val() });
        if (JsManager.validate(obj)) {
            if (Message.Prompt()) {
                var jsonParam = $('#frmPostPosRegion').serialize();
                var serviceURL = "/PosSetting/SaveRegion/";
                JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
            }

            function onSuccess(jsonData) {
                if (jsonData == "0") {
                    Message.Error("save");
                } else {
                    Message.Success("save");
                    Manager.LoadDeliveryManRegionDDL();
                    $("#PosRegionId").val(jsonData);
                    $("#frmModalPosRegion").modal("hide");
                }
            }

            function onFailed(xhr, status, err) {
                Message.Exception(xhr);
            }
        }

    },
    UpdateRegion: function () {
        var obj = [];
        var id = $("#PosRegionId").val();
        obj.push({ 'Region': $("#PosRegion_Name").val() });
        if (JsManager.validate(obj)) {
            if (Message.Prompt()) {
                var jsonParam = $('#frmPostPosRegion').serialize() + "&id=" + id;
                var serviceURL = "/PosSetting/UpdateRegion/";
                JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
            }

            function onSuccess(jsonData) {
                if (jsonData == "0") {
                    Message.Error("update");
                } else {
                    Message.Success("update");
                    Manager.LoadDeliveryManRegionDDL();
                    $("#PosRegionId").val(id);
                    $("#frmModalPosRegion").modal("hide");
                }
            }

            function onFailed(xhr, status, err) {
                Message.Exception(xhr);
            }
        }

    },
    SaveCityZone: function () {
        var obj = [];
        obj.push({ 'City/Zone': $("#PosCityOrNearestZone_Name").val() });
            if (JsManager.validate(obj)) {
                if (Message.Prompt()) {
                    var jsonParam = $('#frmPostCityOrNearestZone').serialize();
                    var serviceURL = "/PosSetting/SaveDeliveryManCityOrZone/";
                    JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
                }

                function onSuccess(jsonData) {
                    if (jsonData == "0") {
                        Message.Error("save");
                    } else {
                        Message.Success("save");
                        Manager.LoadDeliveryManNearestCityDDL();
                        $("#PosCityOrNearestZoneId").val(jsonData);
                        $("#frmModalCityOrNearestZone").modal("hide");
                    }
                }

                function onFailed(xhr, status, err) {
                    Message.Exception(xhr);
                }
            }
        
    },
    UpdateCityZone: function () {
        var obj = [];
        var cityOrZoneId = $("#PosCityOrNearestZoneId").val();
        obj.push({ 'City/Zone': $("#PosCityOrNearestZone_Name").val() });
        if (JsManager.validate(obj)) {
            if (Message.Prompt()) {
                var jsonParam = $('#frmPostCityOrNearestZone').serialize() + "&id=" + cityOrZoneId;
                var serviceURL = "/PosSetting/UpdateDeliveryManCityOrZone/";
                JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
            }

            function onSuccess(jsonData) {
                if (jsonData == "0") {
                    Message.Error("update");
                } else {
                    Message.Success("update");
                    Manager.LoadDeliveryManNearestCityDDL();
                    $("#PosCityOrNearestZoneId").val(cityOrZoneId);
                    $("#frmModalCityOrNearestZone").modal("hide");
                }
            }

            function onFailed(xhr, status, err) {
                Message.Exception(xhr);
            }
        }

    },
    Save: function() {
        var obj = [];
        obj.push({ 'DeliveryMan class': $("#PosDeliveryManClassId").val() });
        obj.push({ 'Phone no': $("#Phone").val() });
        obj.push({ 'Region': $("#PosRegionId").val() });
        obj.push({ 'Nearest city or zone': $("#PosCityOrNearestZoneId").val() });

        if (_DeliveryManId != null) {
            Message.Exist('Data already exists! Try new one.');
        } else {
            if (JsManager.validate(obj)) {
                if (Message.Prompt()) {
                    var jsonParam = $('#DeliveryManForm').serialize();
                    var serviceURL = "/PosSetting/SaveDeliveryMan/";
                    JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
                }

                function onSuccess(jsonData) {
                    if (jsonData == "0") {
                        Message.Error("save");
                    } else if (jsonData == "PhoneNoExists") {
                        Message.Warning("Phone already exists.");
                    } else {
                        Message.Success('DeliveryMan(' + jsonData.DeliveryManNo + ') saved successfully.');
                        Manager.FormReset();
                        Manager.GetDataForTable(1);
                    }
                }

                function onFailed(xhr, status, err) {
                    Message.Exception(xhr);
                }
            }
        }
    },

    Update: function() {
        if (_DeliveryManId == null) {
            Message.Warning('Please select a row to update.');
        } else {
            var obj = [];
            obj.push({ 'DeliveryMan class': $("#PosDeliveryManClassId").val() });
            obj.push({ 'Phone no': $("#Phone").val() });
            //obj.push({ 'Region': $("#PosRegionId").val() });
            obj.push({ 'Nearest city or zone': $("#PosCityOrNearestZoneId").val() });

            if (JsManager.validate(obj)) {
                if (Message.Prompt()) {
                    var jsonParam = $('#DeliveryManForm').serialize() + '&id=' + _DeliveryManId;
                    var serviceURL = "/PosSetting/UpdateDeliveryMan/";
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
        }
    },

    Delete: function(DeliveryManId) {
        if (Message.Prompt()) {
            var jsonParam = { DeliveryManId: DeliveryManId };
            var serviceURL = "/PosSetting/DeleteDeliveryMan/";
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

    LoadDeliveryManClassDDL: function() {
        var jsonParam = "";
        var serviceURL = "/PosDropdownSetting/GetDeliveryManClasses/";
        JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            JsManager.PopulateCombo('#PosDeliveryManClassId', jsonData, 'Select Class',0);
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

    LoadDeliveryManRegionDDL: function() {
        var jsonParam = "";
        var serviceURL = "/PosDropdownSetting/GetDeliveryManRegions/";
        JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            JsManager.PopulateCombo('#PosRegionId', jsonData, 'Select Region',0);
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },
    LoadAllBranch: function() {
        var jsonParam = "";
        var serviceURL = "/PosDropDown/GetBranch/";
        JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            JsManager.PopulateCombo('#PosBranchId', jsonData);
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

    LoadDeliveryManNearestCityDDL: function() {
        var jsonParam = "";
        var serviceURL = "/PosDropdownSetting/GetDeliveryManNearestCity/";
        JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            JsManager.PopulateCombo('#PosCityOrNearestZoneId', jsonData, 'Select Nearest City',0);
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

    GetDataForTable: function(ref) {
        var jsonParam = '';
        var serviceURL = "/PosSetting/GetDeliveryMans/";
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
                dom: "<'row'<'col-md-6'B><'col-md-3'l><'col-md-3'f>>" + "<'row'<'col-md-12'tr>>" + "<'row'<'col-md-5'i><'col-md-7 mt-7'p>>",
                initComplete: function() {
                    dTableManager.Border("#dTable", 350);
                    $('#tableElement_length').css({ 'float': 'right' });
                },
                buttons: [
                    {
                        text: '<i class="far fa-file-pdf"></i> PDF',
                        className: 'btn btn-sm',
                        extend: 'pdfHtml5',
                        exportOptions: {
                            columns: [2, 3, 4, 5, 6, 7]
                        },
                        title: 'DeliveryMan List'
                    },
                    {
                        text: '<i class="fas fa-print"></i> Print',
                        className: 'btn btn-sm',
                        extend: 'print',
                        exportOptions: {
                            columns: [2, 3, 4, 5, 6, 7]
                        },
                        title: 'DeliveryMan List'
                    },
                    {
                        text: '<i class="far fa-file-excel"></i> Excel',
                        className: 'btn btn-sm',
                        extend: 'excelHtml5',
                        exportOptions: {
                            columns: [2, 3, 4, 5, 6, 7]
                        },
                        title: 'DeliveryMan List'
                    }
                ],
                scrollY: "350px",
                scrollX: true,
                scrollCollapse: true,
                lengthMenu: [[50, 100, 500, -1], [50, 100, 500, "All"]],
                columnDefs: [
                    { visible: false, targets: [] },
                    { "className": "dt-center", "targets": [0, 8] }
                ],
                columns: [
                    {
                        data: '',
                        name: 'SL',
                        orderable: false,
                        title: '#SL',
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
                            return EventManager.DataTableCommonButton();
                        }
                    },
                   
                    { data: 'DeliveryManName_en', name: 'DeliveryManName_en', title: 'Delivery Man Name_en', width: 100 },
                    { data: 'DeliveryManName_ar', name: 'DeliveryManName_ar', title: 'Delivery Man Name_ar', width: 100 },
                    { data: 'Address', name: 'Address', title: 'Address Line1', width: 170 },
                    { data: 'NationalID', name: 'NationalID', title: 'NationalID', width: 170 },
                    //{ data: 'DeliveryManCode', name: 'DeliveryManCode', title: 'Delivery Man Code', width: 170 },
                    //{ data: 'RegionName', name: 'RegionName', title: 'Region', width: 70 },
                    { data: 'Phone', name: 'Phone', title: 'Phone', width: 70 },
                    //{ data: 'ClassName', name: 'ClassName', title: 'Class', align: 'center', width: 60 },
                    { data: 'Branch', name: 'Branch', title: 'Branch', align: 'center', width: 120 },
                    {
                        data: 'IsPointAllowable',
                        name: 'IsPointAllowable',
                        title: 'Point',
                        width: 45,
                        render: function(data, type, row) {
                            var text = 'Yes';
                            if (data == false) {
                                text = 'No';
                            }
                            return text;
                        }
                    },
                    {
                        data: 'IsDueAllowable',
                        name: 'IsDueAllowable',
                        title: 'Due',
                        width: 45,
                        render: function(data, type, row) {
                            var text = 'Yes';
                            if (data == false) {
                                text = 'No';
                            }
                            return text;
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

    },

    FormReset: function() {
        _DeliveryManId = null;
        $('#DeliveryManForm')[0].reset();
        $('#PosDeliveryManClassId').trigger("chosen:update");
        $('#PosRegionId').trigger("chosen:update");
        $('#PosCityOrNearestZoneId').trigger("chosen:update");
    },


}