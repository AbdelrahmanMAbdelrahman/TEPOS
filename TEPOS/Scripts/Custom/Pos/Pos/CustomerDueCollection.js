
var dTable = null;

$(document).ready(function () {

    Manager.LoadCustomerDropdown();

    $("#Customer").change(function () {
        var customerId = $(this).val();
        if (customerId && customerId != '0') {
            Manager.LoadInvoiceNumberDropdown(customerId);
        }
        else {
            $("#SalesInvoice").empty();
            $("#SalesInvoice").trigger('chosen:updated');
        }
    });

    $('#btnLoad').click(function () {
        var customerId = $("#Customer").val();
        var invoiceNumber = $("#SalesInvoice").val();

        if (customerId && customerId != '0') {
            Manager.GetCustomerDueInfo(customerId, invoiceNumber);
        }
        else {
            Message.Warning('Customer is required');
        }
    });

    $('#btnSave').click(function () {
        Manager.PayDueAmount();
    });
});


var Manager = {

    LoadCustomerDropdown: function () {
        var param = '';
        var serviceUrl = "/PosDropDown/GetCustomersHavingDue/";
        JsManager.SendJson(serviceUrl, param, onSuccess, onFailed);
        function onSuccess(response) {
            JsManager.PopulateCombo('#Customer', response, 'Select One');
            $("#Customer").chosen({ width: '97%' });
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
    },

    LoadInvoiceNumberDropdown: function (customerId) {
        var param = { customerId: customerId };
        var serviceUrl = "/PosDropDown/GetDueInvoiceNumbersByCustomer/";
        JsManager.SendJson(serviceUrl, param, onSuccess, onFailed);
        function onSuccess(response) {
            JsManager.PopulateCombo('#SalesInvoice', response, 'Select One');
            $("#SalesInvoice").chosen({ width: '97%' });
            $("#SalesInvoice").trigger('chosen:updated');
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
    },

    ResetForm: function () {
        $('#frmDueCollection').trigger('reset');
        $(".chosen-box").trigger("chosen:updated");
    },

    GetCustomerDueInfo: function (customerId, invoiceNumber) {
        var jsonParam = { customerId: customerId, invoiceNumber: invoiceNumber };
        var serviceUrl = "/Pos/GetCustomerDueInfo/";
        JsManager.SendJsonAsyncON(serviceUrl, jsonParam, onSuccess, onFailed);
        function onSuccess(response) {
            if (dTable == null) {
                Manager.LoadDueDataTable(response, 0);
            }
            else {
                Manager.LoadDueDataTable(response, 1);
            }
        }
        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

    LoadDueDataTable: function (data, isRef) {
        if (isRef == "0") {
            dTable = $('#tableElement').DataTable({
                dom: 'B<"tableToolbar">rt',
                initComplete: function () {
                    dTableManager.Border("#dTable", 350);
                },
                buttons: [
                ],
                "order": false,
                scrollY: "200px",
                scrollX: true,
                scrollCollapse: true,
                lengthMenu: [[-1], ["All"]],
                columnDefs: [
                    { visible: false, targets: [] },
                    { className: 'text-center', targets: [0, 1, 2] },
                    { className: 'text-right', targets: [3, 4, 5, 6] }
                ],
                columns: [
                {
                    data: '',
                    name: 'SL',
                    orderable: false,
                    title: '#SL',
                    render: function (data, type, row, meta) {
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                {
                    data: 'InvDate',
                    name: 'InvDate',
                    orderable: false,
                    title: 'Inv Date',
                    align: 'left',
                    render: function (data, type, row, meta) {
                        return JsManager.ChangeDateFormat(data, false);
                    }
                },
                {
                    data: 'InvoiceNumber',
                    name: 'InvoiceNumber',
                    title: 'Invoice No',
                    align: 'left',
                    orderable: false
                },
                {
                    data: 'DueAmount',
                    name: 'DueAmount',
                    title: 'Total Due',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        return data.toFixed(2);
                    }
                },
                {
                    data: 'TotalCollected',
                    name: 'TotalCollected',
                    title: 'Collected',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        return data.toFixed(2);
                    }
                },
                {
                    data: null,
                    name: 'CurrentDue',
                    title: 'Current Due',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        var currentDue = parseFloat(row.DueAmount) - parseFloat(row.TotalCollected);
                        return currentDue.toFixed(2);
                    }
                },
                {
                    data: null,
                    name: 'PaidAmount',
                    title: 'Payment',
                    orderable: false,
                    width: 60,
                    render: function (data, type, row, meta) {
                        return "<input style='text-align:right;border: 1px solid #6b6868  !important;' type='number' id='row-" + meta.row + "-PaidAmount' name='row-" + meta.row + "-PaidAmount' value='0.00' class='form-control input-sm paidAmountInRow' placeholder='amount'/>";
                    }
                }
                ],
                fixedColumns: true,
                data: data,
                rowsGroup: null,
                drawCallback: function () {
                    $('#btnSave').show();
                }
            });

        } else {
            dTable.clear().rows.add(data).draw();

        }

    },

    PayDueAmount: function () {
        var payments = [];
        dTable.rows().every(function () {
            var rowData = this.data();
            var row = $(this.node());

            var salesInvoiceId = rowData.SalesInvoiceId;
            var paidAmount = parseFloat(row.find('.paidAmountInRow').val());

            if (salesInvoiceId && paidAmount > 0) {
                var due = {
                    PosSalesInvoiceId: salesInvoiceId,
                    Amount: paidAmount
                };
                payments.push(due);
            }
        });

        if (payments.length === 0) {
            Message.Warning('No valid data found');
        }
        else {
            if (Message.Prompt()) {
                var serviceUrl = "/Pos/SaveCustomerDueCollection/";
                JsManager.SendJson(serviceUrl,{customerDueCollections: JSON.stringify(payments)}, onSuccess, onFailed);
                function onSuccess(response) {
                    if (response == "200") {
                        Message.Success("save");
                        $('#btnLoad').trigger('click');
                    }
                    else {
                        Message.Error("save");
                    }
                }
                function onFailed(error) {
                    window.alert(error.statusText);
                }
            }
        }
    }
}