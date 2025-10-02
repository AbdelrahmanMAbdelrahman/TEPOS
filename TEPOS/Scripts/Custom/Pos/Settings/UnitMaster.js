var dTable = null;
var ugId = null;

$(document).ready(function () {

    Manager.GetDataForTable(0);
    dTableManager.dTableSerialNumber(dTable);
 

});

$(document).on('click', '#btnSave', function () {
    Manager.Save();
});

$(document).on('click', '.dTableEdit', function () {
    var $item = $(this).parents('tr');
    var row = dTable.row($item).data();

    ugId = row.Id;
    $("#UomCode").val(row.UomCode);
    $("#UomDescription").val(row.UomDescription);
    $("#IsBaseUom").prop("checked", row.IsBaseUom);
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
   
    Save: function () {
        var obj = [];
        obj.push({ 'Uom Code': $("#UomCode").val() });
        obj.push({ 'Uom Description ': $("#UomDescription ").val() });
        if (JsManager.validate(obj)) {
            if (Message.Prompt()) {
                var jsonParam = $('#frmPost').serialize();
                var serviceURL = "/PosSetting/SaveUnitMaster/";
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
        obj.push({ 'Uom Code': $("#UomCode").val() });
        obj.push({ 'Uom Description ': $("#UomDescription ").val() });
        if (JsManager.validate(obj)) {
            if (Message.Prompt()) {
                var jsonParam = $('#frmPost').serialize() + '&Id=' + ugId;
                var serviceURL = "/PosSetting/UpdateUnitMaster/";
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
            var jsonParam = { id: uGroupId };
            var serviceURL = "/PosSetting/DeleteUnitMaster/";
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
        var serviceURL = "/PosSetting/GetUnitMaster/";
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
                            text: '<i class="far fa-file-pdf"></i> PDF',
                            className: 'btn btn-sm',
                            extend: 'pdfHtml5',
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
                        width:80,
                        render: function(data, type, row) {
                            return EventManager.DataTableCommonButton();
                        }
                    },
                    { data: 'UomCode', name: 'UomCode', title: 'Uom Code', align: 'center'},
                    { data: 'UomDescription', name: 'UomDescription', title: 'Uom Description' },
                        {
                            data: 'IsBaseUom', name: 'IsBaseUom', title: 'Base Unit',
                            render: function (date, type, row) {
                                if (row["IsBaseUom"] === true) {
                                    return "Yes";
                                } else {
                                    return "No";
                                }
                            }
                        },
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
        $("#frmPost")[0].reset();
    },
}