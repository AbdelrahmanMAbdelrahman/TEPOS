
var dTable = null;
var _customerId = null;

$(document).ready(function () {
    Manager.GetDataForTable(0);
    Manager.LoadCustomerClassDDL();
    Manager.LoadCustomerRegionDDL();
    Manager.LoadCustomerNearestCityDDL();
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

    $("#addCustomerRegion").click(function () {
        $("#btnSaveRegion").removeClass('dN');
        $("#btnEditRegion").addClass('dN');
        $("#frmModalPosRegion").modal("show");
        $("#PosCityOrNearestZone_Name").val('');
    });
    $("#editCustomerRegion").click(function () {
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

    $("#addCustomerClass").click(function () {
        $("#btnSaveCustomerClass").removeClass('dN');
        $("#btnEditCustomerClass").addClass('dN');
        $("#frmModalCustomerClass").modal("show");
        $("#PosCustomerClass_Name").val('');
    });
    $("#editCustomerClass").click(function () {
        if ($("#PosCustomerClassId").val() > 0) {
            $("#btnEditCustomerClass").removeClass('dN');
            $("#btnSaveCustomerClass").addClass('dN');
            $("#frmModalCustomerClass").modal("show");
            $("#PosCustomerClass_Name").val($("#PosCustomerClassId").find("option:selected").text());
        } else {
            Message.Warning("Select a customer class for edit");
        }
    });

    $("#btnSaveCustomerClass").click(function () {
        Manager.SaveCustomerClass();
    });
    $("#btnEditCustomerClass").click(function () {
        Manager.UpdateCustomerClass();
    });
    
    $("#IsPosBranchCustomer").click(function () {
        if ($(this).prop("checked") == true) {
            $("#IsDefaultPosCustomer").prop("checked", false);
        }
    });
    $("#IsDefaultPosCustomer").click(function () {
        if ($(this).prop("checked") == true) {
            $("#IsPosBranchCustomer").prop("checked", false);
        }
    });

});

$(document).on('click', '.dTableEdit', function () {
    JsManager.StartProcessBar();
    var $item = $(this).parents('tr');
    var row = dTable.row($item).data();

    _customerId = row.Id;
    $("#PosCustomerClassId").val(row.PosCustomerClassId);
    $("#PosCustomerClassId").trigger("chosen:updated");
    $("#Phone").val(row.Phone);
    $("#FirstName").val(row.FirstName);
    $("#PosBranchId").val(row.PosBranchId);
    $("#LastName").val(row.LastName);
    $("#AdditionalPhone").val(row.AdditionalPhone);
    $("#Address").val(row.Address);
    $("#Address2").val(row.Address2);
    $("#PosRegionId").val(row.PosRegionId);
    $("#PosCityOrNearestZoneId").val(row.PosCityOrNearestZoneId);
    $("#IsPointAllowable").prop('checked', row.IsPointAllowable);
    $("#IsDueAllowable").prop('checked', row.IsDueAllowable);
    $("#IsDefaultPosCustomer").prop('checked', row.IsDefaultPosCustomer);
    $("#IsPosBranchCustomer").prop("checked",row.IsPosBranchCustomer);

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
        UpdateCustomerClass: function () {
            var obj = [];
            var id = $("#PosCustomerClassId").val();
        obj.push({ 'Customer Class': $("#PosCustomerClass_Name").val() });
        if (JsManager.validate(obj)) {
            if (Message.Prompt()) {
                var jsonParam = $('#frmPostCustomerClass').serialize() + "&id=" + id;
                var serviceURL = "/PosSetting/UpdateCustomerClass/";
                JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
            }

            function onSuccess(jsonData) {
                if (jsonData == "0") {
                    Message.Error("update");
                } else {
                    Message.Success("update");
                    Manager.LoadCustomerClassDDL();
                    $("#PosCustomerClassId").val(id);
                    $("#frmModalCustomerClass").modal("hide");
                }
            }

            function onFailed(xhr, status, err) {
                Message.Exception(xhr);
            }
        }

    },
    SaveCustomerClass: function () {
        var obj = [];
        obj.push({ 'Customer Class': $("#PosCustomerClass_Name").val() });
        if (JsManager.validate(obj)) {
            if (Message.Prompt()) {
                var jsonParam = $('#frmPostCustomerClass').serialize();
                var serviceURL = "/PosSetting/SaveCustomerClass/";
                JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
            }

            function onSuccess(jsonData) {
                if (jsonData == "0") {
                    Message.Error("save");
                } else {
                    Message.Success("save");
                    Manager.LoadCustomerClassDDL();
                    $("#PosCustomerClassId").val(jsonData);
                    $("#frmModalCustomerClass").modal("hide");
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
                    Manager.LoadCustomerRegionDDL();
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
                    Manager.LoadCustomerRegionDDL();
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
                    var serviceURL = "/PosSetting/SaveCustomerCityOrZone/";
                    JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
                }

                function onSuccess(jsonData) {
                    if (jsonData == "0") {
                        Message.Error("save");
                    } else {
                        Message.Success("save");
                        Manager.LoadCustomerNearestCityDDL();
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
                var serviceURL = "/PosSetting/UpdateCustomerCityOrZone/";
                JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
            }

            function onSuccess(jsonData) {
                if (jsonData == "0") {
                    Message.Error("update");
                } else {
                    Message.Success("update");
                    Manager.LoadCustomerNearestCityDDL();
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
        obj.push({ 'Customer class': $("#PosCustomerClassId").val() });
        obj.push({ 'Phone no': $("#Phone").val() });
        obj.push({ 'Region': $("#PosRegionId").val() });
        obj.push({ 'Nearest city or zone': $("#PosCityOrNearestZoneId").val() });

        if (_customerId != null) {
            Message.Exist('Data already exists! Try new one.');
        } else {
            if (JsManager.validate(obj)) {
                if (Message.Prompt()) {
                    var jsonParam = $('#customerForm').serialize();
                    var serviceURL = "/PosSetting/SaveCustomer/";
                    JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
                }

                function onSuccess(jsonData) {
                    if (jsonData == "0") {
                        Message.Error("save");
                    } else if (jsonData == "PhoneNoExists") {
                        Message.Warning("Phone already exists.");
                    } else {
                        Message.Success('Customer(' + jsonData.CustomerNo + ') saved successfully.');
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
        if (_customerId == null) {
            Message.Warning('Please select a row to update.');
        } else {
            var obj = [];
            obj.push({ 'Customer class': $("#PosCustomerClassId").val() });
            obj.push({ 'Phone no': $("#Phone").val() });
            obj.push({ 'Region': $("#PosRegionId").val() });
            obj.push({ 'Nearest city or zone': $("#PosCityOrNearestZoneId").val() });

            if (JsManager.validate(obj)) {
                if (Message.Prompt()) {
                    var jsonParam = $('#customerForm').serialize() + '&id=' + _customerId;
                    var serviceURL = "/PosSetting/UpdateCustomer/";
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

    Delete: function(customerId) {
        if (Message.Prompt()) {
            var jsonParam = { customerId: customerId };
            var serviceURL = "/PosSetting/DeleteCustomer/";
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

    LoadCustomerClassDDL: function() {
        var jsonParam = "";
        var serviceURL = "/PosDropdownSetting/GetCustomerClasses/";
        JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            JsManager.PopulateCombo('#PosCustomerClassId', jsonData, 'Select Class',0);
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

    LoadCustomerRegionDDL: function() {
        var jsonParam = "";
        var serviceURL = "/PosDropdownSetting/GetCustomerRegions/";
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
        var serviceURL = "/PosDropdownSetting/GetAllBranchList/";
        JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            JsManager.PopulateCombo('#PosBranchId', jsonData);
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

    LoadCustomerNearestCityDDL: function() {
        var jsonParam = "";
        var serviceURL = "/PosDropdownSetting/GetCustomerNearestCity/";
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
        var serviceURL = "/PosSetting/GetCustomers/";
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
                            columns: [2, 3, 4, 5, 6, 7, 8, 9, 10]
                        },
                        title: 'Customer List'
                    },
                    {
                        text: '<i class="fas fa-print"></i> Print',
                        className: 'btn btn-sm',
                        extend: 'print',
                        exportOptions: {
                            columns: [2, 3, 4, 5, 6, 7, 8, 9, 10]
                        },
                        title: 'Customer List'
                    },
                    {
                        text: '<i class="far fa-file-excel"></i> Excel',
                        className: 'btn btn-sm',
                        extend: 'excelHtml5',
                        exportOptions: {
                            columns: [2, 3, 4, 5, 6, 7, 8, 9, 10]
                        },
                        title: 'Customer List'
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
                    { data: 'CustomerNo', name: 'CustomerNo', title: 'Cust. No', width: 70 },
                    {
                        data: 'IsDefaultPosCustomer', name: 'IsDefaultPosCustomer', title: 'Def. Cust.', width: 70,
                        render: function (data, type, row) {
                            if (row["IsDefaultPosCustomer"] == true)
                                return "Yes";
                                return "Regular Customer";
                            }
                    },
                    {
                        data: 'IsPosBranchCustomer', name: 'IsPosBranchCustomer', title: 'Pos Cust.', width: 70,
                        render: function(data, type, row) {
                            if (row["IsPosBranchCustomer"] == true)
                                return "Pos Customer";
                            return "Normal Customer";
                        }
                    },
                    { data: 'FirstName', name: 'FirstName', title: 'First Name', width: 100 },
                    { data: 'LastName', name: 'LastName', title: 'Last Name', width: 100 },
                    { data: 'Address', name: 'Address', title: 'Address Line1', width: 170 },
                    { data: 'RegionName', name: 'RegionName', title: 'Region', width: 70 },
                    { data: 'Phone', name: 'Phone', title: 'Phone', width: 70 },
                    { data: 'ClassName', name: 'ClassName', title: 'Class', align: 'center', width: 60 },
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
        _customerId = null;
        $('#customerForm')[0].reset();
        $('#PosCustomerClassId').trigger("chosen:update");
        $('#PosRegionId').trigger("chosen:update");
        $('#PosCityOrNearestZoneId').trigger("chosen:update");
    },


}