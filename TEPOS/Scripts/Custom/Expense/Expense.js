var dTable = null;
var _ExpenseId = null;
var ExpenseManager = null;

$(document).ready(function () {
    
    $('#btnSave').on('click', function () {
        ExpenseManager.Save();
    });
   

    $('#btnEdit').on('click', function () {
        ExpenseManager.Update();
    });

    $('#btnClear').on('click', function () {
        ExpenseManager.FormReset();
    });

    $("#addExpenseType").click(function () {
        $("#btnSaveExpenseType").removeClass('dN');
        $("#btnEditExpenseType").addClass('dN');
        $("#frmModalExpenseType").modal("show");
        $("#ExpenseType_Name").val('');
    });

    $("#editExpenseType").click(function () {
        if ($("#expenseType").val()) {
            $("#btnEditExpenseType").removeClass('dN');
            $("#btnSaveExpenseType").addClass('dN');
            $("#frmModalExpenseType").modal("show");
            $("#ExpenseType_Name").val($("#expenseType").find("option:selected").text());
        } else {
            Message.Warning("Select an expense type to edit");
        }
    });

    $("#btnSaveExpenseType").click(function () {
        ExpenseManager.SaveExpenseType();
    });

    $("#btnEditExpenseType").click(function () {
        ExpenseManager.UpdateExpenseType();
    });
    
    ExpenseManager = {

        Save: function () {
            debugger;
           
            var amount = parseFloat($("#Amount").val());
            if (amount <= 0) {
                Message.Warning("Amount must be greater than 0");
                $("#Amount").focus();
                $("#Amount").select();
                return;
            }

            var obj = {
                'ExpenseType': $("#expenseType").val(),
                'Amount': amount,
                'Date': $("#Date").val(),
                'Notes': $("#Notes").val(),
                'PosBranchId': $("#PosBranchId").val()
            };
            
            console.log(obj);
                
            if (Message.Prompt()) {
                var jsonParam = { expenseObj: JSON.stringify(obj) };
                console.log(jsonParam);

                var serviceURL = "/Expense/SaveExpense/";

                JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
            }

            function onSuccess(jsonData) {
                console.log(jsonData);
                if (jsonData == "0") {
                    Message.Error("Failed to save expense.");
                } else if (jsonData == "exists") {
                    Message.Warning("Expense already exists for this date and type.");
                } else {
                    Message.Success('Expense (' + jsonData.Id + ') saved successfully.');
                    ExpenseManager.FormReset();
                    ExpenseManager.GetDataForTable();
                }
            }
              
            function onFailed(xhr, status, err) {
                if (xhr.status === 404) {
               //     Message.Error("The requested endpoint was not found. Please check your server configuration.");
                } else {
                    Message.Exception(xhr);
                }
            }
        },

        Update: function () {
            if (_ExpenseId == null) {
                Message.Warning('Please select a row to update.');
            } else {
                var amount = parseFloat($("#Amount").val());
                if (amount <= 0) {
                    Message.Warning("Amount must be greater than 0");
                    $("#Amount").focus();
                    $("#Amount").select();
                    return;
                }

                var obj = [];
                obj.push({ 'Expense Type': $("#expenseType").val() });
                obj.push({ 'Amount': amount });
                obj.push({ 'Date': $("#Date").val() });

                if (JsManager.validate(obj)) {
                    if (Message.Prompt()) {
                        var jsonParam = $('#expenseForm').serialize() + '&id=' + _ExpenseId;
                        var serviceURL = "/Expense/UpdateExpense/";
                        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
                    }

                    function onSuccess(jsonData) {
                        if (jsonData == "0") {
                            Message.Error("update");
                        } else {
                            Message.Success("update");
                            ExpenseManager.FormReset();
                            ExpenseManager.GetDataForTable(1);
                        }
                    }

                    function onFailed(xhr, status, err) {
                        if (xhr.status === 404) {
                      //      Message.Error("The requested endpoint was not found. Please check your server configuration.");
                        } else {
                            Message.Exception(xhr);
                        }
                    }
                }
            }
        },

        Delete: function (id) {
            if (Message.Prompt()) {
                debugger;
                var jsonParam = { expenseId: id };
                console.log(jsonParam);
                var serviceURL = "/Expense/DeleteExpense/";
                JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
            }

            function onSuccess(jsonData) {
                if (jsonData == "0") {
                    Message.Error("delete");
                } else {
                    Message.Success("delete");
                    ExpenseManager.GetDataForTable(1);
                }
            }

            function onFailed(xhr, status, err) {
                if (xhr.status === 404) {
                //    Message.Error("The requested endpoint was not found. Please check your server configuration.");
                } else {
                    Message.Exception(xhr);
                }
            }
        },
        LoadAllBranch: function () {
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

        GetDataForTable: function (refresh) {
            var jsonParam = '';
            var serviceURL = "/Expense/GetExpenseList/";
            JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed, 'GET');

            function onSuccess(jsonData) {
                ExpenseManager.LoadDataTable(jsonData, refresh);
            }

            function onFailed(xhr, status, err) {
                if (xhr.status === 404) {
              //      Message.Error("The requested endpoint was not found. Please check your server configuration.");
                } else {
                    Message.Exception(xhr);
                }
            }
        },

        LoadDataTable: function (userdata, refresh) {
            if (refresh == "0") {
                dTable = $('#expenseTable').DataTable({
                    dom: "<'row'<'col-md-6'B><'col-md-3'l><'col-md-3'f>>" + "<'row'<'col-md-12'tr>>" + "<'row'<'col-md-5'i><'col-md-7 mt-7'p>>",
                    initComplete: function () {
                        dTableManager.Border("#expenseTable", 350);
                        $('#tableElement_length').css({ 'float': 'right' });
                    },
                    buttons: [
                        {
                            text: '<i class="far fa-file-pdf"></i> PDF',
                            className: 'btn btn-sm',
                            extend: 'pdfHtml5',
                            exportOptions: {
                                columns: [1, 2, 3, 4, 5] // Add column 5 (Branch) here
                            },
                            title: 'Expense List'
                        },
                        {
                            text: '<i class="fas fa-print"></i> Print',
                            className: 'btn btn-sm',
                            extend: 'print',
                            exportOptions: {
                                columns: [1, 2, 3, 4, 5] // Add column 5 (Branch) here
                            },
                            title: 'Expense List'
                        },
                        {
                            text: '<i class="far fa-file-excel"></i> Excel',
                            className: 'btn btn-sm',
                            extend: 'excelHtml5',
                            exportOptions: {
                                columns: [1, 2, 3, 4, 5] // Add column 5 (Branch) here
                            },
                            title: 'Expense List'
                        }
                    ],
                    scrollY: "350px",
                    scrollX: true,
                    scrollCollapse: true,
                    lengthMenu: [[50, 100, 500, -1], [50, 100, 500, "All"]],
                    columnDefs: [
                        { visible: false, targets: [] },
                        { "className": "dt-center", "targets": [0, 6] }, // Add class for column 6 (Branch)
                        { "className": "dt-right", "targets": [2] }
                    ],
                    data: userdata,
                    columns: [
                        {
                            data: null,
                            render: function (data, type, row, meta) {
                                return meta.row + meta.settings._iDisplayStart + 1;
                            },
                            width: '5%'
                        },
                        {
                            data: 'ExpenseType',
                            width: '20%'
                        },
                        {
                            data: 'Amount',
                            render: function (data) {
                                return '<span class="text-end d-block">' + parseFloat(data).toFixed(2) + '</span>';
                            },
                            width: '15%'
                        },
                        {
                            data: 'Date',
                            render: function (data) {
                                try {
                                    return moment(data).format('YYYY-MM-DD');
                                } catch (e) {
                                    var date = new Date(data);
                                    return date.toISOString().split('T')[0];
                                }
                            },
                            width: '15%'
                        },
                        {
                            data: 'Notes',
                            width: '25%'
                        },
                        {
                            data: 'Branch',  // Add the Branch column here
                            width: '25%'     // Adjust width as needed
                        },
                        {
                            data: null,
                            width: '20%',
                            className: 'text-center',
                            render: function (data, type, row) {
                                return '<div class="btn-group" role="group">' +
                                    '<button class="btn btn-primary btn-sm" onclick="ExpenseManager.Edit(' + row.Id + ')" title="Edit">' +
                                    '<i class="fas fa-edit"></i></button> ' +
                                    '<button class="btn btn-danger btn-sm" onclick="ExpenseManager.Delete(' + row.Id + ')" title="Delete">' +
                                    '<i class="fas fa-trash"></i></button></div>';
                            }
                        }
                    ],
                    order: [[0, 'asc']],
                    pageLength: 50,
                    responsive: true,
                    language: {
                        search: "_INPUT_",
                        searchPlaceholder: "Search expenses...",
                        lengthMenu: "_MENU_ records per page",
                        paginate: {
                            first: '<i class="fas fa-angle-double-left"></i>',
                            previous: '<i class="fas fa-angle-left"></i>',
                            next: '<i class="fas fa-angle-right"></i>',
                            last: '<i class="fas fa-angle-double-right"></i>'
                        }
                    }
                });

                // Datatable serial
                dTable.on('order.dt search.dt', function () {
                    dTable.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                        cell.innerHTML = i + 1;
                    });
                }).draw();
            } else {
                dTable.clear().rows.add(userdata).draw();
            }
        },


        Edit: function (id) {
            _ExpenseId = id;
            var data = dTable.rows().data();
            var expense = data.filter(function (item) { return item.Id === id; })[0];

            $("#expenseType").val(expense.ExpenseType);
            $("#Amount").val(expense.Amount);
            $("#Date").val(moment(expense.Date).format('YYYY-MM-DD'));
            $("#Notes").val(expense.Notes);

            $("#btnSave").hide();
            $("#btnEdit").show();
        },
       
        FormReset: function () {
            _ExpenseId = null;
            $('#expenseForm')[0].reset();
            $("#btnSave").show();
            $("#btnEdit").hide();
        },

        SaveExpenseType: function () {
            var obj = {
                'Name': $("#ExpenseType_Name").val()
            };

            if (JsManager.validate(obj)) {
                if (Message.Prompt()) {
                    var jsonParam = { expenseType: JSON.stringify(obj) };
                    var serviceURL = "/Expense/SaveExpenseType/";
                    JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
                }

                function onSuccess(jsonData) {
                    if (jsonData == "0") {
                        Message.Error("save");
                    } else if (jsonData == "exists") {
                        Message.Warning("Expense type already exists");
                    } else {
                        Message.Success("save");
                        $("#frmModalExpenseType").modal("hide");
                        ExpenseManager.LoadExpenseTypeDDL();
                    }
                }

                function onFailed(xhr, status, err) {
                    if (xhr.status === 404) {
                       // Message.Error("The requested endpoint was not found. Please check your server configuration.");
                    } else {
                        Message.Exception(xhr);
                    }
                }
            }
        },

        UpdateExpenseType: function () {
            var obj = {
                'Name': $("#ExpenseType_Name").val(),
                'Id': $("#expenseType").val()
            };

            if (JsManager.validate(obj)) {
                if (Message.Prompt()) {
                    var jsonParam = { expenseType: JSON.stringify(obj) };
                    var serviceURL = "/Expense/UpdateExpenseType/";
                    JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
                }

                function onSuccess(jsonData) {
                    if (jsonData == "0") {
                        Message.Error("update");
                    } else if (jsonData == "exists") {
                        Message.Warning("Expense type already exists");
                    } else {
                        Message.Success("update");
                        $("#frmModalExpenseType").modal("hide");
                        ExpenseManager.LoadExpenseTypeDDL();
                    }
                }

                function onFailed(xhr, status, err) {
                    if (xhr.status === 404) {
                      //  Message.Error("The requested endpoint was not found. Please check your server configuration.");
                    } else {
                        Message.Exception(xhr);
                    }
                }
            }
        },

        LoadExpenseTypeDDL: function () {
            var jsonParam = '';
            var serviceURL = "/Expense/GetExpenseTypes/";
            JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed, 'GET');

            function onSuccess(jsonData) {
                JsManager.PopulateCombo('#expenseType', jsonData);
            }

            function onFailed(xhr, status, err) {
                if (xhr.status === 404) {
                //    Message.Error("The requested endpoint was not found. Please check your server configuration.");
                } else {
                    Message.Exception(xhr);
                }
            }
        }
    };

   
    ExpenseManager.GetDataForTable(0);
    ExpenseManager.LoadExpenseTypeDDL();
    ExpenseManager.LoadAllBranch();

});
