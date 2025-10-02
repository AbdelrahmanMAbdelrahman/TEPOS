var dTableTender = null;
var dTableBilling = null;
var dTableFreeItems = null;
var dTableWarranty = null;
var invoiceNumber = 0;
var isItemForStockTransfer = false;
var dtWarrantyRef = 0;
var prdFindBySystem = 0;


$(document).ready(function () {
    Manager.LoadAllBranch();
    Manager.GetTenderDetails(0);
    Manager.GetBillingDetails(0);
    Manager.GetFreeItemsDetails(0);
    dTableManager.dTableSerialNumber(dTableFreeItems);
    dTableManager.dTableSerialNumber(dTableBilling);
    $("#btnHoldInvoice").prop('disabled', false);

    Manager.LoadHoldInvoice();
    Manager.GetCustomer();
    Manager.GetDeliveryMan();
    var selecttDeliveryMan = document.getElementById("txtDeliveryMan");

    selecttDeliveryMan.disabled = true;
    // Add this to the $(document).ready section
    $(document).on('change', '.dtBillingDiscount', function () {
        // Trigger the function to recalculate totals for all rows
        Manager.BillingTotalCalculation(0); // Ensure totals across all rows are recalculated
    });

    $(document).on('change', '.dtBillingSd', function () {
        // Trigger the function to recalculate totals for all rows
        Manager.BillingTotalCalculation(0); // Ensure totals across all rows are recalculated
    });



    $("#txtItemOrBarcode").focus();

    if ($("#btnSaveInvoiceStockTrns").length > 0) {
        isItemForStockTransfer = true;
    }

    //#region Billing

    $("#txtInvoiceFind").keydown(function (e) {
        if (e.keyCode == 13) {
            if ($("#btnFindInvoiceStockTrns").length > 0) {
                //get stock transfer invoice
                Manager.GetInvoice(5, $(this).val());
            } else {
                Manager.GetInvoice(0, $(this).val());
            }
        }
    });

    $("#btnFindInvoice").click(function () {
        dTableTender.rows().every(function (rowIdx, tableLoop, rowLoop) {
            $("#row-" + rowIdx + "-tenderAmount").val("0.00");
        });
        Manager.GetInvoice(0, $("#txtInvoiceFind").val());
    });

    $("#btnFindInvoiceStockTrns").click(function () {
        //get stock transfer invoice
        dTableTender.rows().every(function (rowIdx, tableLoop, rowLoop) {
            $("#row-" + rowIdx + "-tenderAmount").val("0.00");
        });
        Manager.GetInvoice(5, $("#txtInvoiceFind").val());
    });

    $("#btnRecallInvoice").click(function () {
        Manager.GetInvoice(0, $("#cmbHoldInvoice").val());
    });


    $("#txtItemOrBarcode").keydown(function (key) {
        if (key.keyCode == 13) {
            if ($(this).val() == "" || $(this).val() == "0") {
                $(".txtCashReceiptTender").focus();
                $(".txtCashReceiptTender").select();
            } else {
                Manager.AddBillingItem($(this).val());
            }
        }
    });

    $("#txtScientificName").keydown(function (key) {
        if (key.keyCode == 13) {
            if ($(this).val() == "" || $(this).val() == "0") {
                $(".txtCashReceiptTender").focus();
                $(".txtCashReceiptTender").select();
            } else {
                Manager.AddBillingItem($(this).val());
            }
        }
    });
    $("#txtProductName").keydown(function (key) {
        if (key.keyCode == 13) {
            if ($(this).val() == "" || $(this).val() == "0") {
                $(".txtCashReceiptTender").focus();
                $(".txtCashReceiptTender").select();
            } else {
                Manager.AddBillingItem($(this).val());
            }
        }
    });
    $(".txtCashReceiptTender").keydown(function (key) {
        if (key.keyCode == 13) {
            if ($("#btnSaveInvoiceStockTrns").length > 0) {
                $("#btnSaveInvoiceStockTrns").trigger("click");
            } else {
                $("#btnSaveInvoice").trigger("click");
            }
        }
    });

    $(document).on("keyup", ".dtBillingChangeableInput", function (key) {
        var val = parseFloat($(this).val());
        if (!val) {
            $(this).val('0.00');
            $(this).select();
        }

        if (!key.ctrlKey && !key.altKey) {
            Manager.getScheme();
            Manager.BillingTotalCalculation(0);
            $(this).focus();
        }

    });


    $(document).on('keydown', '.dtBillingInputs', function (key) {
        var inputs = $(this).parents('tr').find('.dtBillingInputs');
        var idx = inputs.index(this);
        var $item = $(this);
        Manager.dtBillingEventManager(key, inputs, idx, $item);
    });

    $(document).on('keyup', '.dtBillingUnitPrice', function (key) {
        if (!key.ctrlKey && !key.altKey) {
            Manager.getScheme();
            Manager.BillingTotalCalculation(0);
        }
    });

    $(document).on('change', '.dtBillingDropdown', function (key) {
        Manager.getScheme();
        Manager.BillingTotalCalculation(0);
    });

    ///////////
    //$(document).on('change', '#PosBranchId',function () {
    //    debugger;
    //    Manager.GetCustomer();
    //    $("#txtCustomer").trigger("chosen:updated");
    //});

    //////////////////


    $(document).on('click', '.dtBillingInputs', function () {
        $(this).select();
    });

    $("#btnSaveInvoice").click(function () {
        Manager.insertData(1);
    });

    $("#btnSaveInvoiceStockTrns").click(function () {
        Manager.insertData(5);
    });

    $("#btnReprintInv").click(function () {
        var invoice = $("#txtLastInvoiceNo").text();
        if ($("#txtInvoiceFind").val() != "0000") {
            invoice = $("#txtInvoiceFind").val();
        }
        if (invoice == "" || invoice == "0") {
            Message.Warning("Last Invoice remove from billing");
        } else {
            Manager.PrintBill(invoice);
        }
    });

    $("#btnPrintLastInv").click(function () {
        var serviceURL = "/Pos/GetLastInvoiceNo/";
        JsManager.SendJson(serviceURL, '', onSuccess, onFailed);
        function onSuccess(jsonData) {
            Manager.PrintBill(jsonData);
        }
        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    });

    $("#btnCancel").click(function () {
        if ($("#btnSaveInvoiceStockTrns").length > 0) {
            //cancel stock transfer stock
            Manager.CancelInvoice(5);
        } else {
            Manager.CancelInvoice(0);
        }
    });

    $("#btnHoldInvoice").click(function () {
        Manager.insertData(3);
        Manager.LoadHoldInvoice();
    });

    $("#btnClear").click(function () {
        Manager.resetForm();
        $("#txtInvoiceFind").val('');
    });

    //#endregion Billing

    $("#btnAddWarranty").click(function () {
        if (($("#txtLastInvoiceNo").text().length < 4 && $("#txtInvoiceFind").val().length < 4)) {
            Message.Warning("Please find the invoice for warranty issue.");
        } else {
            var invoiceNo = $("#txtInvoiceFind").val().length < 4 ? $("#txtLastInvoiceNo").text() : $("#txtInvoiceFind").val();
            $("#hInvoiceNo").text(invoiceNo);
            var promise1 = new Promise(function (resolve, reject) {
                $("#modalAddWarranty").modal('show');
                resolve(1);
            });

            promise1.then(function (value) {
                if (value == 1) {
                    setTimeout(function () {
                        Manager.GetInvoiceItemForAddWarranty(dtWarrantyRef, invoiceNo);
                        dtWarrantyRef = 1;
                    }, 500);
                }
            });
        }
    });

    $("#btnWarrantySave").click(function () {
        Manager.InsertOrUpdateWarrantyData();
    });
    $("#btnPrintInvoiceWithWarrantyCard").click(function () {
        Manager.BillPrintWithWarranty($("#hInvoiceNo").text());
    });

    $(".otherDiscountCls").click(function () {
        $(this).select();
    });
    $(".otherDiscountCls").keyup(function () {
        Manager.BillingTotalCalculation(0, $(this));
        if ($(this).val() == "0.00") {
            $(this).select();
        }
    });
    $(".otherDiscountCls").change(function () {

    });
});

//#region warranty 

$(document).on('keydown', '.dtWarrantyInputs', function (key) {
    var inputs = $(this).parents('tr').find('.dtWarrantyInputs');
    var idx = inputs.index(this);
    var $item = $(this);
    Manager.dtWarrantyEventManager(key, inputs, idx, $item);
});

//#endregion warranty


//#region dt Tender event

$(document).on('keydown', '.dtTenderInputs', function (key) {
    var inputs = $(this).parents('tr').find('.dtTenderInputs');
    var idx = inputs.index(this);
    var $item = $(this);
    Manager.dtTenderEventManager(key, inputs, idx, $item);
});

$(document).on('keyup', '.dtTenderAmount', function (key) {
    var $item = $(this);
    Manager.changeInputBackgroundOfTender($item);
    Manager.dtTenderCalculation();
});

$(document).on('click', '.dtTenderInputs', function () {
    $(this).select();
});

//#endregion dt Tender event

function DeleteBillingItemRow(obj) {
    var dtTr = $(obj).parents('tr');
    dTableBilling.row(dtTr).remove().draw();
    Manager.getScheme();
    Manager.BillingTotalCalculation(0);
}

var Manager = {

    //#region dropdown
    GetCustomer: function () {
        //debugger;
        var posInvoiceType = 0;
        if ($("#btnFindInvoiceStockTrns").length > 0) {
            //get stock transfer invoice
            posInvoiceType = 5;
        } else {
            posInvoiceType = 1;
        }
        var serviceUrl = '/PosDropDown/GetCustomerCallCenter/';
        //var serviceUrl = '/PosDropDown/GetCustomer/';
        //var PosBranchId = $("#PosBranchId").val();
        //var jsonParam = { posInvoiceType: posInvoiceType, PosBranchId: PosBranchId  };
        var jsonParam = { posInvoiceType: posInvoiceType };
        JsManager.SendJsonAsyncON(serviceUrl, jsonParam, onSuccess, onFailed);

        function onSuccess(response) {
            JsManager.PopulateCombo('#txtCustomer', response);
            $("#txtCustomer").chosen("width:84%");
            $("#txtCustomer").trigger("chosen:updated");
        }

        function onFailed() {
        }
    },
    LoadAllBranch: function () {
        var jsonParam = "";
        var serviceURL = "/PosDropdownSetting/GetBranchList/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            JsManager.PopulateCombo('#PosBranchId', jsonData, 'Please Select Branch', 0);
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },
    GetDeliveryMan: function () {
        var posInvoiceType = 0;
        if ($("#btnFindInvoiceStockTrns").length > 0) {
            //get stock transfer invoice
            posInvoiceType = 5;
        } else {
            posInvoiceType = 1;
        }
        var serviceUrl = '/PosDropDown/GetDeliveryMan/';
        var jsonParam = { posInvoiceType: posInvoiceType };
        JsManager.SendJsonAsyncON(serviceUrl, jsonParam, onSuccess, onFailed);

        function onSuccess(response) {
            JsManager.PopulateCombo('#txtDeliveryMan', response, 'Empty Delivery Man', 0);
            $("#txtDeliveryMan").chosen("width:84%");
        }

        function onFailed() {
        }
    },


    //#endRegion dropdown

    resetForm: function () {
        invoiceNumber = 0;
        JsManager.StartProcessBar();
        dTableFreeItems.clear().draw();
        dTableBilling.clear().draw();
        $("#cmbHoldInvoice").val(0);
        dTableTender.rows().every(function (rowIdx, tableLoop, rowLoop) {
            $("#row-" + rowIdx + "-tenderAmount").val("0.00");
            $("#row-" + rowIdx + "-tenderRemarks").val('');
        });
        $("#txtMrpTotal").val("0.00");
        $("#txtSd").val("0.00");
        $("#txtVat").val("0.00");
        $("#txtDiscount").val("0.00");
        $("#txtTotal").val("0.00");
        $("#txtInvoiceFind").attr('readonly', false);
        $("#txtItemOrBarcode").focus();
        $("#spnBillingDate").text(customToDay());
        $("#txtInvoiceRemarks").val('');
        $("#txtNumOfItem").text('00');
        $("#spnInvoiceQty").text('00.00');
        $("#txtOtherDiscountPer").val('00');
        $("#txtOtherDiscountAmt").val('0.00');
        $("#txtNotes").val('');
        $("#txtCustomer").val('');
        $("#txtUserCode").val('');
        $("#txtDeliveryMan").val('');
        Manager.GetCustomer();
        Manager.GetDeliveryMan();
        JsManager.EndProcessBar();
        setTimeout(function () {
            $("#ui_notifIt").slideUp(500);
        }, 1000);

    },

    //#region insert/update/getBilling

    LoadHoldInvoice: function () {
        var jsonParam = { invoiceType: 3 };
        var serviceURL = "/Pos/GetInvoiceByTypeCallCenter/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            $("#cmbHoldInvoice").empty();
            JsManager.PopulateCombo('#cmbHoldInvoice', jsonData, 'Hold Inv.', 0);
        }

        function onFailed(error) {
            window.alert(error.statusText);
        }
    },
    GetInvoice: function (posInvoiceType, invNumber) {

        if (invNumber == "") {
            Message.Warning("Please input or select the invoice no.");
        } else {
            var jsonParam = {
                invoiceNo: invNumber,
                posInvoiceType: posInvoiceType
            };
            var serviceURL = "/Pos/GetInvoiceCallCenter/";
            JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

            function onSuccess(jsonData) {
                if (jsonData.toString().length > 0) {
                    Manager.resetForm();
                    $("#txtInvoiceFind").attr('readonly', true);
                    invoiceNumber = invNumber;
                    $.each(jsonData.SalesInviceProduct, function (i, v) {
                        v.Vat = v.VatPar;
                        v.Sd = v.SdPar;
                        Manager.AddNewBillingItemRow(v);
                    });

                    //for free item table
                    Manager.dTableFreeItemsAddNewRow(jsonData.SalesInvoiceFreeProduct);
                    //for general field
                    $("#txtMrpTotal").val(jsonData.MrpTotal.toFixed(2));
                    $("#txtSd").val(jsonData.SdTotal.toFixed(2));
                    $("#txtVat").val(jsonData.VatTotal.toFixed(2));
                    $("#txtDiscount").val(jsonData.Discount.toFixed(2));
                    $("#txtOtherDiscountAmt").val(jsonData.OtherDiscount.toFixed(2));
                    $("#txtOtherDiscountPer").val(((jsonData.OtherDiscount / jsonData.MrpTotal) * 100).toFixed(2));
                    $("#txtTotal").val(jsonData.TotalAmt.toFixed(2));


                    $("#txtDeliveryMan").val(jsonData.PosDeliveryManId);
                    $("#txtDeliveryMan").trigger("chosen:updated");
                    $("#PosBranchId").val(jsonData.PosBranchId);
                    $("#PosBranchId").trigger("chosen:updated");
                    $("#txtInvoiceRemarks").val(jsonData.Remarks);
                    //$("#txtUserCode").val(jsonData.UserCode);
                    $("#txtNotes").val(jsonData.Notes);
                    $("#spnBillingDate").text(JsManager.ChangeDateFormat(jsonData.InvDate, 0));
                    // Manager.GetCustomer();
                    $("#txtCustomer").trigger("chosen:updated");
                    $("#txtCustomer").val(jsonData.CustomerNo);
                    $("#txtCustomer").trigger("chosen:updated");

                    //for tender table populate
                    dTableTender.rows().every(function (rowIdx, tableLoop, rowLoop) {
                        $.each(jsonData.SalesInvoiceTender, function (i, v) {
                            if (dTableTender.cell(rowIdx, 0).data() == v.PosTenderId) {
                                $("#row-" + rowIdx + "-tenderAmount").val(v.TenderAmount.toFixed(2));
                                $("#row-" + rowIdx + "-tenderRemarks").val(v.Remarks);
                                Manager.changeInputBackgroundOfTender($("#row-" + rowIdx + "-tenderAmount"));
                            }
                        });
                    });
                } else {
                    Message.Warning("Invoice not found!");
                }
            }

            function onFailed(xhr, status, err) {
                Message.Exception(xhr);
            }
        }
    },
    CancelInvoice: function (posInvoiceType) {
        if ($("#txtInvoiceFind").val() == "") {
            Message.Warning("At first find the invoice, after then you press the cancel invoice button.");
        } else {
            if (Message.Prompt("Do you want to cancel this invoice?")) {
                var jsonParam = {
                    invoiceNo: $("#txtInvoiceFind").val(),
                    posInvoiceType: posInvoiceType

                };
                var serviceURL = "/Pos/CancelInvoice/";
                JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

                function onSuccess(jsonData) {
                    if (jsonData > 0) {
                        Message.Success("cancel");
                        Manager.resetForm();
                        $("#txtInvoiceFind").val('');
                        Manager.LoadHoldInvoice();
                    } else if (jsonData == "-0001") {
                        Message.Warning("Invalide Invoice No.");
                    } else {
                        Message.Error("cancel");
                    }
                }

                function onFailed(xhr, status, err) {
                    Message.Exception(xhr);
                }
            }
        }
    },
    insertData: function (invoiceType) {
        if (dTableBilling.rows().count() > 0) {
            if (Message.Prompt("Do you want to save this invoice?")) {
                $("#btnHoldInvoice").prop('disabled', true);
                var objInvoice = new Object();
                var posSalesInvoiceProduct = [];
                var posSalesInvoiceFreeProduct = [];
                var posSalesInvoiceTender = [];

                objInvoice.Remarks = $("#txtInvoiceRemarks").val();
                objInvoice.InvDate = customToDay();
                objInvoice.InvoiceNumber = invoiceNumber;
                objInvoice.PosInvoiceType = invoiceType;
                objInvoice.Discount = $("#txtDiscount").val();
                objInvoice.OtherDiscount = $("#txtOtherDiscountAmt").val();
                objInvoice.PosCustomerId = $("#txtCustomer").val() == "" ? 0 : $("#txtCustomer").val();
                objInvoice.PosDeliveryManId = $("#txtDeliveryMan").val() == "" ? 0 : $("#txtDeliveryMan").val();
                objInvoice.PosBranchId = $("#PosBranchId").val() == "" ? 0 : $("#PosBranchId").val();
                //objInvoice.UserCode = $("#txtUserCode").val();
                objInvoice.Notes = $("#txtNotes").val();
                $.each($('#dTableBilling tbody tr'), function (rowIdx, val) {
                    var objPosSalesInvoiceProduct = new Object();
                    objPosSalesInvoiceProduct.PosProductId = dTableBilling.cell(rowIdx, 1).data();
                    objPosSalesInvoiceProduct.PosProductBatchId = $(val).find(".dTableBatchName").attr('data-batchid');
                    var qty = $(val).find(".dtBillingQty").val();
                    var converFac = $(val).find(".dTablePosUomMasterId").val();
                    objPosSalesInvoiceProduct.Qty = parseFloat(qty == "" ? 0 : qty) * parseFloat(converFac == "" ? 0 : converFac);
                    objPosSalesInvoiceProduct.Vat = $(val).find(".dtBillingVat").val();
                    objPosSalesInvoiceProduct.SchDiscount = $(val).find(".dtBillingDiscount").val();
                    objPosSalesInvoiceProduct.OtherDiscount = $(val).find(".dtBillingOtherDiscount").val();
                    objPosSalesInvoiceProduct.Sd = $(val).find(".dtBillingSd").val();
                    objPosSalesInvoiceProduct.Rate = $(val).find(".dtBillingUnitPrice").val();
                    posSalesInvoiceProduct.push(objPosSalesInvoiceProduct);

                });
                objInvoice.PosSalesInvoiceProduct = posSalesInvoiceProduct;

                if (dTableFreeItems.rows().count() > 0) {
                    dTableFreeItems.rows().every(function (rowIdx, tableLoop, rowLoop) {
                        var objPosSalesInvoiceFreeProduct = new Object();
                        objPosSalesInvoiceFreeProduct.PosProductId = dTableFreeItems.cell(rowIdx, 1).data();
                        objPosSalesInvoiceFreeProduct.PosProductBatchId = dTableFreeItems.cell(rowIdx, 2).data();
                        var freeQty = $("#row-" + rowIdx + "-FreeQty").val();
                        freeQty = freeQty == "" ? 0 : freeQty;
                        objPosSalesInvoiceFreeProduct.Qty = freeQty;
                        var freeMqty = $("#row-" + rowIdx + "-ManualQty").val();
                        freeMqty = freeMqty == "" ? 0 : freeMqty;
                        objPosSalesInvoiceFreeProduct.ManualQty = freeMqty;
                        posSalesInvoiceFreeProduct.push(objPosSalesInvoiceFreeProduct);
                    });
                }

                objInvoice.PosSalesInvoiceFreeProduct = posSalesInvoiceFreeProduct;

                if (dTableTender.rows().count() > 0) {
                    dTableTender.rows().every(function (rowIdx, tableLoop, rowLoop) {
                        var tendarType = $("#row-" + rowIdx + "-tenderName").data("tendertype");
                        if (tendarType == "NA" || tendarType == "CR" || tendarType == "DI") {
                            var tendAmt = $("#row-" + rowIdx + "-tenderAmount").val();
                            if (tendAmt != null && tendAmt !== "" && parseFloat(tendAmt) > 0) {
                                var objPosSalesInvoiceTender = new Object();
                                objPosSalesInvoiceTender.PosTenderId = dTableTender.cell(rowIdx, 0).data();
                                objPosSalesInvoiceTender.TenderAmount = tendAmt;
                                objPosSalesInvoiceTender.Remarks = $("#row-" + rowIdx + "-tenderRemarks").val();
                                posSalesInvoiceTender.push(objPosSalesInvoiceTender);
                            }
                        }
                    });
                }
                objInvoice.PosSalesInvoiceTender = posSalesInvoiceTender;


                var jsonParam = {
                    invoice: JSON.stringify(objInvoice)
                };
                //debugger;
                //jsonParam = JSON.stringify(jsonParam);
                var serviceURL = "/Pos/InsertOrUpdateBillingCallCenter/";
                JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

                function onSuccess(jsonData) {
                    $("#btnHoldInvoice").prop('disabled', false);
                    if (jsonData.rtrCode == 0) {
                        Message.Error("save");
                    } else if (jsonData.rtrCode == "-00002") {
                        Message.Warning("Customer not found! Set or select the customer. Invoice No <b style='color:red;'>" + jsonData.LastInv + "</b>");
                    } else if (jsonData.rtrCode == "-00003") {
                        Message.Warning("Product/Item are not allow to add the Invoice No <b style='color:red;'>" + jsonData.LastInv + "</b>");
                    } else if (jsonData.rtrCode == "-000033") {
                        Message.Warning("Free Product/Item are not allow to add. Invoice No <b style='color:red;'>" + jsonData.LastInv + "</b>");
                    } else if (jsonData.rtrCode == "-00004") {
                        Message.Warning("Credit note generation failed. Invoice No <b style='color:red;'>" + jsonData.LastInv + "</b>");
                    } else if (jsonData.rtrCode == "00004") {
                        Message.Success("Successfully generated credit note. Invoice No is <b style='color:red;'>" + jsonData.LastInv + "</b>");
                        Manager.PrintBill(jsonData.LastInv);
                        Manager.resetForm();
                        $("#txtInvoiceFind").val('');
                    } else if (jsonData.rtrCode == "-00005") {
                        Message.Warning("No data found for this Invoice No <b style='color:red;'>" + jsonData.LastInv + "</b>");
                    } else if (jsonData.rtrCode == "00006") {
                        Message.Success("Successfully Inserted Bill. Invoice No <b style='color:red;'>" + jsonData.LastInv + "</b>");
                        Manager.PrintBill(jsonData.LastInv);
                        Manager.resetForm();
                    } else if (jsonData.rtrCode == "-00006") {
                        Message.Warning("Failed To Create Bill. Invoice No <b style='color:red;'>" + jsonData.LastInv + "</b>");
                    }

                    $("#txtLastInvoiceNo").text(jsonData.LastInv);

                }

                function onFailed(xhr, status, err) {

                    Message.Exception(xhr);
                    $("#btnHoldInvoice").prop('disabled', false);
                }
            }

        } else {
            Message.Warning("Minimum one item/prduct add to Invoice");
        }
    },
    PrintBill: function (invoiceNo) {

        if (Message.Prompt("Do you want to print this Invoice?  " + invoiceNo)) {
            JsManager.StartProcessBar("Printing......");
            var billPrintType = 2;
            if ($("#btnSaveInvoiceStockTrns").length > 0) {
                billPrintType = 1;
            }
            var jsonParam = {
                invoiceNo: invoiceNo,
                billingType: billPrintType
            };
            var serviceURL = "/Pos/GenerateBillHTML/";
            JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);

            function onSuccess(jsonData) {

                if ($("#PrintByA4Page").prop("checked") && jsonData.BillprintMessage.BillPrintTemplateId != 1) {
                    BillPrintByA4Page(jsonData);
                } else {
                    if (jsonData.BillprintMessage.Status == 1) {
                        if (jsonData.BillprintMessage.BillPrintTemplateId == 1) {
                            BillPrintOfStockTransfer(jsonData);
                        } else if (jsonData.BillprintMessage.BillPrintTemplateId == 2) {
                            BillPrintByA4Page(jsonData);
                        } else if (jsonData.BillprintMessage.BillPrintTemplateId == 3) {
                            PosBillPrintByPosPrinter(jsonData);
                        } else if (jsonData.BillprintMessage.BillPrintTemplateId == 4) {
                            PosBillPrintByPosPrinterWithBatchOrSize(jsonData);
                        } else {
                            Message.Warning("Template Not Found!");
                        }
                    } else if (jsonData.BillprintMessage.Status == "-1") {
                        //default pos page print
                        PosBillPrintByPosPrinter(jsonData);
                    } else {
                        Message.Warning(jsonData.BillprintMessage.Status);
                    }
                }
                JsManager.EndProcessBar();
            }

            function onFailed(xhr, status, err) {
                Message.Exception(xhr);
            }
        }

    },

    BillPrintWithWarranty: function (invoiceNo) {

        if (Message.Prompt("Do you want to print this Invoice?  " + invoiceNo)) {
            JsManager.StartProcessBar("Printing......");
            var billPrintType = 2;
            if ($("#btnSaveInvoiceStockTrns").length > 0) {
                billPrintType = 1;
            }
            var jsonParam = {
                invoiceNo: invoiceNo,
                billingType: billPrintType
            };
            var serviceURL = "/Pos/GenerateBillHTML/";
            JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);

            function onSuccess(jsonData) {
                BillPrintByA4Page(jsonData);

            }

            function onFailed(xhr, status, err) {
                Message.Exception(xhr);
            }
            JsManager.EndProcessBar();
        }

    },

    //endregion insert/update/getBilling


    //#region free item code
    dtFreeItemsEventManager: function (key, inputs, idx, $item) {
        if (key.keyCode == 13) {
            key.preventDefault();
            var enterInputs = $item.parents('tr').next().find('.dtFreeItemInputs');
            enterInputs[idx].focus();
            enterInputs[idx].select();
        } else if (key.keyCode == 38) {
            key.preventDefault();
            var uPArrowInputs = $item.parents('tr').prev().find('.dtFreeItemInputs');
            uPArrowInputs[idx].focus();
            uPArrowInputs[idx].select();
        } else if (key.keyCode == 40) {
            key.preventDefault();
            var downArrowInputs = $item.parents('tr').next().find('.dtFreeItemInputs');
            downArrowInputs[idx].focus();
            downArrowInputs[idx].select();
        }
    },
    dTableFreeItemsAddNewRow: function ($data) {
        dTableFreeItems.clear().draw();
        $.each($data, function (i, v) {
            dTableFreeItems.row.add({
                PosProductId: v.PosProductId,
                PosProductBatchId: v.PosProductBatchId,
                ProductCode: v.Code,
                Name: v.Name,
                BatchName: v.Batch,
                Qty: v.Qty,
                ManualQty: typeof (v.ManualQty) == 'undefined' ? 0 : v.ManualQty
            }).draw();
        });
    },
    GetFreeItemsDetails: function (ref) {
        var billObj = { PosProductId: '', PosProductBatchId: '', ProductCode: '', Name: '', BatchName: '', Qty: '', ManualQty: '0' };
        Manager.LoadDataTabledTableFreeItems(billObj, ref);
    },
    LoadDataTabledTableFreeItems: function (userdata, isRef) {
        if (isRef == "0") {
            dTableFreeItems = $('#dTableFreeItems').DataTable({
                dom: '<"tableToolbar">rt',
                initComplete: function () {
                    dTableManager.Border("#dTableFreeItems", 130);
                },
                buttons: [
                ],
                scrollY: "130px",
                scrollX: true,
                scrollCollapse: true,
                lengthMenu: [[-1], ["All"]],
                columnDefs: [
                    { visible: false, targets: [1, 2] },
                    { className: "dt-head-right", targets: [0, 1] }
                ],
                columns: [
                    {
                        name: '',
                        orderable: false,
                        width: 7,
                        title: '#',
                        render: function () {
                            return '';
                        }
                    },
                    {
                        data: 'PosProductId',
                        name: 'PosProductId',
                        orderable: false,
                        title: 'PosProductId'
                    },
                    {
                        data: 'PosProductBatchId',
                        name: 'PosProductBatchId',
                        orderable: false,
                        title: 'PosProductBatchId'
                    },
                    {
                        data: 'ProductCode',
                        name: 'ProductCode',
                        orderable: false,
                        title: 'Item Code',
                        width: 140,
                        render: function (data, type, row, meta) {
                            return "<input type='text' id='row-" + meta.row + "-ProductCode' name='row-" + meta.row + "-ProductCode' value='" + data + "' class='form-control input-sm font-size-12px font-weight readOnlyInputBackground' readonly='readonly'/>";
                        }
                    },
                    {
                        data: 'Name',
                        name: 'name',
                        orderable: false,
                        title: 'Item Name',
                        width: 290,
                        render: function (data, type, row, meta) {
                            return "<input type='text' id='row-" + meta.row + "-Name' name='row-" + meta.row + "-Name' value='" + data + "' class='form-control input-sm font-size-12px font-weight readOnlyInputBackground' readonly='readonly'/>";
                        }
                    },
                    {
                        data: 'BatchName',
                        name: 'BatchName',
                        title: 'Batch/Size *',
                        orderable: false,
                        width: 170,
                        render: function (data, type, row, meta) {
                            return "<input type='text' id='row-" + meta.row + "-BatchName' name='row-" + meta.row + "-BatchName' value='" + data + "' class='form-control input-sm font-size-12px font-weight readOnlyInputBackground' readonly='readonly'/>";
                        }
                    },
                    {
                        data: 'Qty',
                        name: 'Qty',
                        orderable: false,
                        title: 'Qty',
                        width: 140,
                        render: function (data, type, row, meta) {
                            return "<input type='text' id='row-" + meta.row + "-FreeQty' name='row-" + meta.row + "-FreeQty' value='" + data + "' class='form-control input-sm font-size-12px font-weight readOnlyInputBackground text-center' readonly='readonly'/>";
                        }
                    },
                    {
                        data: 'ManualQty',
                        name: 'ManualQty',
                        orderable: false,
                        title: 'Manual Qty',
                        width: 140,
                        render: function (data, type, row, meta) {
                            return "<input type='text' id='row-" + meta.row + "-ManualQty' name='row-" + meta.row + "-ManualQty' value='" + data + "' class='dtFreeItemInputs form-control input-sm font-size-12px font-weight text-center'/>";
                        }
                    }
                ],
                fixedColumns: true,
                data: userdata,
                rowsGroup: null,
                ordering: false
            });

        } else {
            dTableFreeItems.clear().rows.add(userdata).draw();

        }
    },
    //#endregion dt free item code


    //#region Billing code
    getScheme: function () {
        var billing = [];
        $.each($('#dTableBilling tbody tr'), function (rowIdx, val) {
            var obj = new Object();
            obj.PosProductId = dTableBilling.cell(rowIdx, 1).data();
            //obj.PosProductBatchId = $(val).find(".dTableBatchName").data('batchid_' + rowIdx); abdo
            obj.PosProductBatchId = $(val).find(".dTableBatchName").attr('data-batchid'); //abdo
            var qty = $(val).find(".dtBillingQty").val();
            var converFac = $(val).find(".dTablePosUomMasterId").val();
            obj.Qty = parseFloat(qty == "" ? 0 : qty) * parseFloat(converFac == "" ? 0 : converFac);
            //var amount = $(val).find(".dtBillingGrossTotal").val(); abdo
            //abdo
            var fqty = parseFloat(qty == "" ? 0 : qty) * parseFloat(converFac == "" ? 0 : converFac);
            var uamount = $(val).find(".dtBillingUnitPrice").val();
            var amount = parseFloat(fqty == "" ? 0 : fqty) * parseFloat(uamount == "" ? 0 : uamount);
            // Abdo
            obj.Amount = parseFloat(amount == "" ? 0 : amount);
            billing.push(obj);
        });
        var jsonParam = {
            date: customToDay(),
            customerId: $("#txtCustomer").val(),
            DeliveryManId: $("#txtDeliveryMan").val(),
            branchId: $("#PosBranchId").val(),
            billingItem: JSON.stringify(billing)
        };
        var serviceURL = "/Pos/GetBillingScheme/";
        JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            if (jsonData == "customerNotFound") {
                Message.Warning("Customer Not Found!");
            } else {
                Manager.applyScheme(jsonData);
            }
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },
    applyScheme: function (data) {
        $.each($('#dTableBilling tbody tr'), function (rowIdx, val) {
            //  $(val).find(".dtBillingDiscount").val(0.00);
            if (data.SingleSchemeDiscount.length > 0) {
                $.each(data.SingleSchemeDiscount, function (i, v) {
                    // if (dTableBilling.cell(rowIdx, 1).data() == v.PosProductId && $(val).find(".dTableBatchName").data('batchid_' + rowIdx) == v.PosProductBatchId) { abdo
                    if (dTableBilling.cell(rowIdx, 1).data() == v.PosProductId && $(val).find(".dTableBatchName").attr('data-batchid') == v.PosProductBatchId) { //abdo                        $(val).find(".dtBillingDiscount").val(v.DiscountAmount.toFixed(2));
                        $(val).find(".dtBillingDiscount").val(v.DiscountAmount.toFixed(2));
                        //data.SingleSchemeDiscount.splice(i);
                    }
                });
            }
        });

        Manager.dTableFreeItemsAddNewRow(data.SchemeFreeProduct);
        Manager.BillingTotalCalculation(data.CombiDiscount);
    },
    dtBillingEventManager: function (key, inputs, idx, $item) {

        if (key.keyCode == 13) {
            key.preventDefault();
            $("#txtItemOrBarcode").focus();
            $("#txtItemOrBarcode").val('');
            $("#txtScientificName").val('');
            $("#txtProductName").val('');
            //var enterInputs = $item.parents('tr').next().find('.dtBillingInputs');
            //enterInputs[idx].focus();
            //enterInputs[idx].select();
        } else if (key.keyCode == 37) {
            inputs[idx - 1].focus();
            inputs[idx - 1].select();
        } else if (key.keyCode == 38) {
            key.preventDefault();
            var uPArrowInputs = $item.parents('tr').prev().find('.dtBillingInputs');
            uPArrowInputs[idx].focus();
            uPArrowInputs[idx].select();
        } else if (key.keyCode == 39) {
            inputs[idx + 1].focus();
            inputs[idx + 1].select();
        } else if (key.keyCode == 40) {
            key.preventDefault();
            var downArrowInputs = $item.parents('tr').next().find('.dtBillingInputs');
            downArrowInputs[idx].focus();
            downArrowInputs[idx].select();
        }
    },
    AddNewBillingItemRow: function (jsonData) {

        var conversionFactor = jsonData.UomDetails.filter(w => w.IsBaseUom == true)[0].ConversionFactor;
        var billQty = parseFloat(jsonData.Qty);

        var salesPriceIncOrExcVat = $("#spnSalesPriceIncExcVat").data("salespriceincexcvat");
        var grossTotal = ((billQty * parseFloat(conversionFactor)) * parseFloat(jsonData.UnitPrice));
        var sdAmt = (((grossTotal - jsonData.OtherDiscount) * parseFloat(jsonData.Sd)) / 100);
        var vatAmt = 0;
        var netAmt = 0;
        //including vat
        if (salesPriceIncOrExcVat === 1) {
            var vatPar = parseFloat(jsonData.Vat);
            vatAmt = (((grossTotal - jsonData.OtherDiscount - jsonData.Discount) / (vatPar + 100)) * vatPar);
            netAmt = ((grossTotal + sdAmt) - parseFloat(jsonData.Discount + jsonData.OtherDiscount));
        }
        //Excluding vat
        else if (salesPriceIncOrExcVat === 2) {
            vatAmt = (((grossTotal - jsonData.OtherDiscount - jsonData.Discount) * parseFloat(jsonData.Vat)) / 100);
            netAmt = ((grossTotal + vatAmt + sdAmt) - parseFloat(jsonData.Discount + jsonData.OtherDiscount));
        } else {
            Message.Warning("Please reload and try again.");
            return;
        }

        dTableBilling.row.add({
            ProductId: jsonData.PosProductId,
            VatPar: jsonData.Vat.toFixed(2),
            SdPar: jsonData.Sd.toFixed(2),
            ParUnitDiscount: jsonData.Discount.toFixed(2),
            ProductCode: jsonData.ProductCode,
            Name: jsonData.Name,
            PosUomMasterId: jsonData.UomDetails,
            BatchName: jsonData.BatchName,
            PosProductBatchId: jsonData.PosProductBatchId,
            UnitPrice: jsonData.UnitPrice.toFixed(2),
            Stock: jsonData.Stock,
            Qty: billQty,
            GrossTotal: grossTotal,
            Vat: vatAmt.toFixed(2),
            Sd: sdAmt.toFixed(2),
            //Discount: jsonData.Discount.toFixed(2),
            Discount: ((jsonData.Discount.toFixed(2) / (jsonData.UnitPrice.toFixed(2) * billQty)) * 100).toFixed(2),
            OtherDiscount: jsonData.OtherDiscount.toFixed(2),
            NetAmount: netAmt.toFixed(2),
            IsPriceChangeable: jsonData.IsPriceChangeable
        }).draw();

    },

    BillingTotalCalculation: function (combiSchemeDiscount, otherDiscountObj) {
        var mrpTotal = 0.00;
        var vatTotal = 0.00;
        var sdTotal = 0.00;
        var discountTotal = 0.00;
        var otherDiscountTotal = 0.00;
        var subTotal = 0.00;
        var totalInvQty = 0;
        combiSchemeDiscount = parseFloat(combiSchemeDiscount != "" ? combiSchemeDiscount : 0);
        var salesPriceIncOrExcVat = $("#spnSalesPriceIncExcVat").data("salespriceincexcvat");

        //other discount
        if (otherDiscountTotal < 1) {
            var otherDiscountValue = parseFloat($(otherDiscountObj).val());
            if (!otherDiscountValue) {
                $(".otherDiscountCls").val('0.00');
            } else {
                var mrpValueTotal = parseFloat($("#txtMrpTotal").val());
                if ($(otherDiscountObj).attr('id') == "txtOtherDiscountPer") {
                    $("#txtOtherDiscountAmt").val(((mrpValueTotal * otherDiscountValue) / 100).toFixed(2));
                } else {
                    $("#txtOtherDiscountPer").val(((otherDiscountValue / mrpValueTotal) * 100).toFixed(2));
                }
            }
        }

        var dTableRowCount = dTableBilling.rows().count();
        if (dTableRowCount > 0) {
            $.each($('#dTableBilling tbody tr'), function (rowIdx, val) {
                var conversionFactor = $(val).find(".dTablePosUomMasterId").val();
                var tmpQty = $(val).find(".dtBillingQty").val();
                if (!tmpQty || parseFloat(tmpQty) <= 0) {
                    $(val).find(".dtBillingOtherDiscount").val(0.00);
                }
                totalInvQty += parseFloat(tmpQty);
                var newQty = (parseFloat(tmpQty) * parseFloat(conversionFactor));

                var unitPrice = $(val).find(".dtBillingUnitPrice").val();
                var loopMrpTotal = parseFloat((unitPrice * newQty).toFixed(2));
                mrpTotal += loopMrpTotal;


                var singleSchemeDiscount = $(val).find(".dtBillingDiscount").val();
                singleSchemeDiscount = parseFloat(singleSchemeDiscount != "" ? singleSchemeDiscount : 0);
                var loopDiscountTotal = (parseFloat(singleSchemeDiscount.toFixed(2)) / 100) * loopMrpTotal;
                discountTotal += loopDiscountTotal;

                //Mohamed Adel
                //when place other discount in payment section
                //var otherDiscountCount = parseFloat($("#txtOtherDiscountAmt").val()) / dTableRowCount;
                var otherDiscountCount = ((parseFloat($("#txtOtherDiscountPer").val()) * unitPrice) / 100) * newQty;

                if (otherDiscountCount > 0) {
                    $(val).find(".dtBillingOtherDiscount").val(otherDiscountCount.toFixed(2));
                }

                var loopVatableAmt = 0;
                var loopOtherDiscount = parseFloat($(val).find(".dtBillingOtherDiscount").val());
                otherDiscountTotal += loopOtherDiscount;
                //loopVatableAmt = (loopOtherDiscount ? (loopMrpTotal - (loopOtherDiscount + loopDiscountTotal)) : loopMrpTotal);
                loopVatableAmt = loopMrpTotal - (loopOtherDiscount + loopDiscountTotal); //adel

                //var sdPar = dTableBilling.rows(rowIdx).data()[0].SdPar;
                var sdPar = $(val).find(".dtBillingSd").val();
                var loopSdTotal = parseFloat((parseFloat(sdPar)).toFixed(2));
                sdTotal += loopSdTotal;

                var loopVatTotal = 0;
                var loopSubTotal = 0;
                var vatPar = $(val).find(".dtBillingVatPar").val();

                //including vat
                if (salesPriceIncOrExcVat === 1) {
                    loopVatTotal = parseFloat(((loopVatableAmt / (100 + parseFloat(vatPar))) * parseFloat(vatPar)).toFixed(2));
                    vatTotal += loopVatTotal;

                    loopSubTotal = parseFloat(((loopMrpTotal + loopSdTotal) - (loopDiscountTotal + loopOtherDiscount)).toFixed(2));
                    subTotal += loopSubTotal;
                }
                //excluding vat    
                else if (salesPriceIncOrExcVat === 2) {
                    loopVatTotal = parseFloat(((parseFloat(vatPar) * loopVatableAmt) / 100).toFixed(2));
                    vatTotal += loopVatTotal;

                    loopSubTotal = parseFloat(((loopMrpTotal + loopVatTotal + loopSdTotal) - (loopDiscountTotal + loopOtherDiscount)).toFixed(2));
                    subTotal += loopSubTotal;
                } else {
                    Message.Warning("Please reload and try again.");
                    return;
                }



                $(val).find(".dtBillingGrossTotal").val(loopMrpTotal.toFixed(2));
                $(val).find(".dtBillingVat").val(loopVatTotal.toFixed(2));
                $(val).find(".dtBillingSd").val(loopSdTotal.toFixed(2));
                $(val).find(".dtBillingDiscount").val(singleSchemeDiscount.toFixed(2));
                $(val).find(".dtBillingNetAmt").val(loopSubTotal.toFixed(2));
            });
        }


        //end other discount
        $("#txtMrpTotal").val(mrpTotal.toFixed(2));
        $("#txtSd").val(sdTotal.toFixed(2));
        $("#txtVat").val(vatTotal.toFixed(2));
        $("#txtDiscount").val((discountTotal + combiSchemeDiscount).toFixed(2));
        $("#txtTotal").val(subTotal.toFixed(2));
        $("#txtNumOfItem").text(dTableBilling.rows().count());
        $("#spnInvoiceQty").text(totalInvQty.toFixed(2));
        if (!$(otherDiscountObj).val()) {
            $("#txtOtherDiscountAmt").val(otherDiscountTotal.toFixed(2));
        }


        var cnd1 = false, cnd2 = false;
        dTableTender.rows().every(function (rowIdx, tableLoop, rowLoop) {
            var tenderType = $("#row-" + rowIdx + "-tenderName").data("tendertype");
            if (tenderType == 'R') {
                $("#row-" + rowIdx + "-tenderAmount").val((Math.round(subTotal) - subTotal).toFixed(2));
                Manager.changeInputBackgroundOfTender($("#row-" + rowIdx + "-tenderAmount"));
                cnd1 = true;
            } else if (tenderType == 'PA') {
                $("#row-" + rowIdx + "-tenderAmount").val(Math.round(subTotal).toFixed(2));
                // $("#row-" + 1 + "-tenderAmount").val(Math.round(subTotal).toFixed(2));

                Manager.changeInputBackgroundOfTender($("#row-" + rowIdx + "-tenderAmount"));
                cnd2 = true;
            } else if (tenderType == 'DA') {
                $("#row-" + rowIdx + "-tenderAmount").val(Math.round(subTotal).toFixed(2));
                Manager.changeInputBackgroundOfTender($("#row-" + rowIdx + "-tenderAmount"));
            } else {
                $("#row-" + rowIdx + "-tenderAmount").val('0.00');
            }
            if (cnd1 && cnd2) {
                return;
            }
        });

    },
    AddBillingItem: function (prdCode) {
        JsManager.StartProcessBar("Adding Item ....");
        var PosBranch = $("#PosBranchId").val();
        var jsonParam = { prdCodeOrBarCode: prdCode, isItemForStockTransfer: isItemForStockTransfer, PosBranchId: PosBranch};
        var serviceURL = "/Pos/GetBilingItem/";
        JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);
        function onSuccess(jsonData) {
            //debugger;
            var PosBranchId = $("#PosBranchId").val();
            if (jsonData == "") {
                Message.Warning("Product/Item Not Found!");
            } else if (/*jsonData.BatchCount > 1 ||*/ jsonData.Stock <= 0 || jsonData.PosBranchId != parseInt(PosBranchId)) {
                $("#txtItemOrBarcodeACC").prop("checked", true);
                $("#txtItemOrBarcodeACC").trigger({ type: 'keypress', which: 39, keyCode: 39 });
                $("#txtScientificNameACC").prop("checked", true);
                $("#txtScientificNameACC").trigger({ type: 'keypress', which: 39, keyCode: 39 });
                $("#txtProductNameACC").prop("checked", true);
                $("#txtProductNameACC").trigger({ type: 'keypress', which: 39, keyCode: 39 });
                prdFindBySystem = 1;
            } else {

                var foundExistingItem = 0;
                if (dTableBilling.rows().count() > 0) {
                    $.each($('#dTableBilling tbody tr'), function (rowIdx, val) {
                        if (jsonData.PosProductId == dTableBilling.rows(rowIdx).data()[0].ProductId && jsonData.PosProductBatchId == $(val).find(".dTableBatchName").attr('data-batchid')) {
                            //found same biling product and only increment item qty

                            foundExistingItem = 1;
                            var tmpQty = $(val).find(".dtBillingQty").val();
                            var newQty = (parseFloat(tmpQty) + parseFloat(jsonData.Qty));
                            $(val).find(".dtBillingQty").val(newQty);

                            if ($("#chkEnableQtyChangeOption").prop("checked")) {
                                $(val).find(".dtBillingQty").focus();
                                $(val).find(".dtBillingQty").select();
                            } else {
                                $("#txtItemOrBarcode").focus();
                                $("#txtItemOrBarcode").val('');
                                $("#txtScientificName").val('');
                                $("#txtProductName").val('');
                            }
                            JsManager.EndProcessBar();
                            return;
                        }
                    });
                }
                if (foundExistingItem == 0) {
                    Manager.AddNewBillingItemRow(jsonData);
                    if ($("#chkEnableQtyChangeOption").prop("checked")) {
                        $("#row-" + (dTableBilling.rows().count() - 1) + "-Qty").focus();
                        $("#row-" + (dTableBilling.rows().count() - 1) + "-Qty").select();
                    } else {
                        $("#txtItemOrBarcode").focus();
                        $("#txtItemOrBarcode").val('');
                        $("#txtScientificName").val('');
                        $("#txtProductName").val('');
                    }
                }
                Manager.BillingTotalCalculation(0);
                Manager.getScheme();
                $("#txtItemOrBarcode").focus();

            }
            JsManager.EndProcessBar();
        }

        function onFailed(xhr, status, err) {
            JsManager.EndProcessBar();
            Message.Exception(xhr);
        }
    },
    GetBillingDetails: function (ref) {
        var billObj = { ProductId: '', VatPar: '0.00', SdPar: '0.00', ParUnitDiscount: '0.00', ProductCode: '', Name: '', PosUomMasterId: '', BatchName: '', UnitPrice: '0.00', Stock: '', Qty: '', GrossTotal: '', Vat: '', Sd: '0.00', Discount: '0.00', NetAmount: '0.00', PosProductBatchId: 0, IsPriceChangeable: false };
        Manager.LoadDataTableBilling(billObj, ref);
    },
    LoadDataTableBilling: function (userdata, isRef) {
        if (isRef == "0") {
            dTableBilling = $('#dTableBilling').DataTable({
                dom: '<"tableToolbar">rt',
                initComplete: function () {
                    dTableManager.Border("#dTableBilling", 320);
                },
                buttons: [
                ],
                scrollY: "350px",
                scrollX: true,
                scrollCollapse: true,
                lengthMenu: [[-1], ["All"]],
                columnDefs: [
                    { visible: false, targets: [1, 2, 3] },
                    { className: "dt-head-right", targets: [0, 1] }
                ],
                columns: [
                    {
                        name: '',
                        orderable: false,
                        title: '#',
                        render: function () {
                            return '';
                        }
                    },
                    {
                        data: 'ProductId',
                        name: 'ProductId',
                        orderable: false,
                        title: 'ProductId'
                    },
                    {
                        data: 'SdPar',
                        name: 'SdPar',
                        orderable: false,
                        title: 'SD(%)'
                    },
                    {
                        data: 'ParUnitDiscount',
                        name: 'ParUnitDiscount',
                        orderable: false,
                        title: 'ParUnitDiscount'
                    },
                    {
                        data: 'ProductCode',
                        name: 'ProductCode',
                        orderable: false,
                        title: 'Code',
                        width: 80,
                        render: function (data, type, row, meta) {
                            return "<input type='text' id='row-" + meta.row + "-ProductCode' name='row-" + meta.row + "-ProductCode' value='" + data + "' class='form-control input-sm font-size-12px font-weight readOnlyInputBackground dtBillingInputs minWidth80' readonly='readonly'/>";
                        }
                    },
                    {
                        data: 'Name',
                        name: 'name',
                        orderable: false,
                        title: 'Item Name',
                        width: 190,
                        render: function (data, type, row, meta) {
                            return "<input type='text' id='row-" + meta.row + "-ItemName' name='row-" + meta.row + "-ItemName' value='" + data + "' class='form-control input-sm font-size-12px font-weight readOnlyInputBackground dtBillingInputs minWidth190' readonly='readonly'/>";
                        }
                    },
                    {
                        data: 'PosUomMasterId',
                        name: 'PosUomMasterId',
                        title: 'Pack....',
                        width: 70,
                        orderable: false,
                        render: function (data, type, row, meta) {
                            var uomList = data.filter(w => w.IsBaseUom == true)[0];
                            var uomOption = '<option data-isbaseuom=' + uomList.IsBaseUom + ' value=' + uomList.ConversionFactor + '>' + uomList.UomCode + '</option>';;
                            $.each(data.filter(w => w.IsBaseUom == false), function (i, v) {
                                uomOption += '<option data-isbaseuom=' + v.IsBaseUom + ' value=' + v.ConversionFactor + '>' + v.UomCode + '</option>';
                            });
                            return "<select id='row-" + meta.row + "-PosUomMasterId' name='row-" + meta.row + "-PosUomMasterId'  class='dtBillingDropdown form-control input-sm dTablePosUomMasterId readOnlyInputBackground dtBillingInputs minWidth70'>" + uomOption + "</select>";
                        }
                    },
                    {
                        data: 'BatchName',
                        name: 'BatchName',
                        title: 'Batch/Size *',
                        orderable: false,
                        width: 80,
                        render: function (data, type, row, meta) {
                            return "<input data-batchid=" + row.PosProductBatchId + " type='text' id='row-" + meta.row + "-BatchName' name='row-" + meta.row + "-BatchName' value=" + data + "  class='dtBillingDropdown form-control input-sm dTableBatchName readOnlyInputBackground dtBillingInputs minWidth80' />";
                        }
                    },
                    {
                        data: 'UnitPrice',
                        name: 'UnitPrice',
                        orderable: false,
                        title: 'Unit Price',
                        width: 60,
                        render: function (data, type, row, meta) {
                            return "<input type='number' id='row-" + meta.row + "-UnitPrice' name='row-" + meta.row + "-UnitPrice' value='" + data + "' class='minWidth60 form-control input-sm font-size-12px font-weight text-right " + (row.IsPriceChangeable ? "" : "readOnlyInputBackground") + " dtBillingInputs dtBillingUnitPrice' " + (row.IsPriceChangeable ? "" : "readonly") + "/>";
                        }
                    },
                    {
                        data: 'Stock',
                        name: 'Stock',
                        orderable: false,
                        title: 'Stock',
                        width: 50,
                        render: function (data, type, row, meta) {
                            return "<input type='number' id='row-" + meta.row + "-Stock' name='row-" + meta.row + "-Stock' value='" + data + "' class='form-control input-sm font-size-12px font-weight text-center readOnlyInputBackground dtBillingInputs dtBillingStockQty minWidth50' readonly='readonly'/>";
                        }
                    },
                    {
                        data: 'Qty',
                        name: 'Qty',
                        orderable: false,
                        title: 'Qty',
                        width: 50,
                        render: function (data, type, row, meta) {
                            return "<input type='number' id='row-" + meta.row + "-Qty' name='row-" + meta.row + "-Qty' value='" + data + "' class='dtBillingChangeableInput form-control text-center input-sm font-size-12px font-weight dtBillingInputs dtBillingQty minWidth50'/>";
                        }
                    },
                    {
                        data: 'GrossTotal',
                        name: 'GrossTotal',
                        orderable: false,
                        title: 'Gross Total',
                        width: 70,
                        render: function (data, type, row, meta) {
                            return "<input type='number' id='row-" + meta.row + "-GrossTotal' name='row-" + meta.row + "-GrossTotal' value='" + data + "' class='form-control input-sm font-size-12px font-weight text-right readOnlyInputBackground dtBillingInputs dtBillingGrossTotal minWidth70' readonly='readonly'/>";
                        }
                    },
                    {
                        data: 'Discount',
                        name: 'Discount',
                        orderable: false,
                        title: 'Sch. (Dis)',
                        width: 60,
                        render: function (data, type, row, meta) {
                            return "<input type='number' id='row-" + meta.row + "-Discount' name='row-" + meta.row + "-Discount' value='" + data + "' class='form-control input-sm font-size-12px font-weight text-right dtBillingInputs dtBillingDiscount minWidth60'/>";
                        }
                    },
                    {
                        data: 'OtherDiscount',
                        name: 'OtherDiscount',
                        orderable: false,
                        title: 'Other (Dis)',
                        width: 70,
                        render: function (data, type, row, meta) {
                            return "<input type='number' id='row-" + meta.row + "-OtherDiscount' name='row-" + meta.row + "-OtherDiscount' value='" + data + "' class='dtBillingChangeableInput form-control input-sm font-size-12px font-weight text-right dtBillingInputs dtBillingOtherDiscount minWidth70' value='0.00'/>";
                        }
                    },
                    {
                        data: 'VatPar',
                        name: 'VatPar',
                        orderable: false,
                        width: 40,
                        title: 'Vat(%)',
                        render: function (data, type, row, meta) {
                            return "<input type='number' id='row-" + meta.row + "-VatPar' name='row-" + meta.row + "-VatPar' value='" + data + "' class='form-control input-sm font-size-12px font-weight text-right readOnlyInputBackground dtBillingInputs dtBillingVatPar minWidth40' readonly='readonly'/>";
                        }
                    },
                    {
                        data: 'Vat',
                        name: 'Vat',
                        orderable: false,
                        title: 'Vat',
                        width: 50,
                        render: function (data, type, row, meta) {
                            return "<input type='number' id='row-" + meta.row + "-Vat' name='row-" + meta.row + "-Vat' value='" + data + "' class='form-control input-sm font-size-12px font-weight text-right readOnlyInputBackground dtBillingInputs dtBillingVat minWidth50' readonly='readonly'/>";
                        }
                    },
                    {
                        data: 'Sd',
                        name: 'Sd',
                        orderable: false,
                        title: 'SD',
                        width: 40,
                        render: function (data, type, row, meta) {
                            return "<input type='number' id='row-" + meta.row + "-Sd' name='row-" + meta.row + "-Sd' value='" + data + "' class='form-control input-sm font-size-12px font-weight text-right dtBillingInputs dtBillingSd minWidth40' />";
                        }
                    },
                    {
                        data: 'NetAmountNetAmount',
                        name: 'NetAmount',
                        orderable: false,
                        title: 'Net Amount',
                        width: 80,
                        render: function (data, type, row, meta) {
                            return "<input type='number' id='row-" + meta.row + "-NetAmount' name='row-" + meta.row + "-NetAmount' value='" + data + "' class='form-control input-sm font-size-12px font-weight text-right readOnlyInputBackground dtBillingInputs dtBillingNetAmt minWidth80' readonly='readonly'/>";
                        }
                    },
                    {
                        name: '',
                        orderable: false,
                        title: 'Option',
                        width: 0,
                        render: function (data, type, row, meta) {
                            return "<button class='btn-danger' style='border: 2px solid #d9534f;width:100%;' onclick='DeleteBillingItemRow(this);'>Remove</button>";
                        }
                    }
                ],
                fixedColumns: true,
                data: userdata,
                rowsGroup: null,
                ordering: false
            });

        } else {
            dTableBilling.clear().rows.add(userdata).draw();

        }
    },
    //#endregion dt tender code


    //#region tender code
    changeInputBackgroundOfTender: function ($item) {
        if (parseFloat($item.val()) > 0 || parseFloat($item.val()) < 0) {
            $item.attr('style', 'background-color: #000 !important;color:#fff;');
        } else {
            $item.css({ 'background-color': '#fff', 'color': '#000' });
            $item.val('0.00');
            $item.select();
        }
    },
    dtTenderCalculation: function () {
        var sumOfColumn = ["CR", 'NA', 'DI'];
        var sumOfPaidAmt = 0;
        var paidableAmount = 0;
        dTableTender.rows().every(function (rowIdx, tableLoop, rowLoop) {
            var tenderType = $("#row-" + rowIdx + "-tenderName").data("tendertype");
            if (sumOfColumn.includes(tenderType)) {
                sumOfPaidAmt += parseFloat($("#row-" + rowIdx + "-tenderAmount").val());
            } else if (tenderType == "PA") {
                paidableAmount = parseFloat($("#row-" + rowIdx + "-tenderAmount").val());
            } else if (tenderType == 'CA') {
                $("#row-" + rowIdx + "-tenderAmount").val((sumOfPaidAmt - paidableAmount).toFixed(2));
            } else if (tenderType == 'DA') {
                var dueAmt = paidableAmount - sumOfPaidAmt;
                if (dueAmt < 1) {
                    dueAmt = 0;
                }
                $("#row-" + rowIdx + "-tenderAmount").val(dueAmt.toFixed(2));
            }
        });
    },
    dtTenderEventManager: function (key, inputs, idx, $item) {
        if (key.keyCode == 13) {
            key.preventDefault();
            var enterInputs = $item.parents('tr').next().find('.dtTenderInputs');
            enterInputs[idx].focus();
            enterInputs[idx].select();
        } else if (key.keyCode == 37) {
            inputs[idx - 1].focus();
            inputs[idx - 1].select();
        } else if (key.keyCode == 38) {
            key.preventDefault();
            var uPArrowInputs = $item.parents('tr').prev().find('.dtTenderInputs');
            uPArrowInputs[idx].focus();
            uPArrowInputs[idx].select();
        } else if (key.keyCode == 39) {
            inputs[idx + 1].focus();
            inputs[idx + 1].select();
        } else if (key.keyCode == 40) {
            key.preventDefault();
            var downArrowInputs = $item.parents('tr').next().find('.dtTenderInputs');
            downArrowInputs[idx].focus();
            downArrowInputs[idx].select();
        }
    },
    GetTenderDetails: function (ref) {
        var jsonParam = '';
        var serviceURL = "/Pos/GetTenderList/";
        JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            Manager.LoadDataTableTender(jsonData, ref);
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },
    LoadDataTableTender: function (userdata, isRef) {
        if (isRef == "0") {
            dTableTender = $('#dTableTender').DataTable({
                dom: '<"tableToolbar">rt',
                buttons: [
                ],
                scrollY: "200px",
                scrollX: true,
                scrollCollapse: true,
                lengthMenu: [[-1], ["All"]],
                columnDefs: [
                    { visible: false, targets: [0] },
                    { className: "dt-head-right", targets: [0, 1, 2] }
                ],
                "order": [],
                columns: [
                    {
                        data: 'PosTenderId',
                        name: 'PosTenderId',
                        orderable: false,
                        title: 'PosTenderId'
                    },
                    {
                        data: 'Name',
                        name: 'Name',
                        orderable: false,
                        title: 'P. Details',
                        render: function (data, type, row, meta) {
                            return "<input type='text' data-tendertype=" + row.Type + " id='row-" + meta.row + "-tenderName' name='row-" + meta.row + "-tenderName' value='" + data + "' class='form-control input-sm font-size-12px font-weight readOnlyInputBackground minWidth180' readonly='readonly'style='padding-left:5px !important;' />";
                        }
                    },
                    {
                        data: '',
                        name: 'Amount',
                        title: 'Amount',
                        orderable: false,
                        render: function (data, type, row, meta) {
                            var readProperty = '';
                            var cRclass = "";
                            if (row.Type == "CR") {
                                cRclass = "txtCashReceiptTender";
                            }
                            var readPrpList = ['R', 'PA', 'DA', 'CA'];
                            if (readPrpList.includes(row.Type)) {
                                readProperty = 'readonly="readonly"';
                            } else {
                                readProperty = '';
                            }
                            return "<input type='number' " + readProperty + " id='row-" + meta.row + "-tenderAmount' name='row-" + meta.row + "-tenderAmount' value=0.00 class='minWidth110 form-control input-sm dtInputTextAlign dtTenderInputs dtTenderAmount font-size-14px font-weight " + cRclass + "'placeholder='0.00' min='0' style='padding-right:5px !important;'/>";
                        }
                    },
                    {
                        data: '',
                        name: 'Remarks',
                        title: 'Remarks',
                        width: 60,
                        orderable: false,
                        render: function (data, type, row, meta) {
                            var readProperty = '';
                            var readPrpList = ['R', 'PA', 'DA', 'CA'];
                            if (readPrpList.includes(row.Type)) {
                                readProperty = 'readonly="readonly"';
                            } else {
                                readProperty = '';
                            }
                            return "<input type='text' " + readProperty + " id='row-" + meta.row + "-tenderRemarks' name='row-" + meta.row + "-tenderRemarks' class='minWidth200 form-control input-sm dtTenderInputs font-size-14px font-weight'  maxlength='499' style='padding-right:5px !important;'/>";
                        }
                    }
                ],
                fixedColumns: true,
                data: userdata,
                rowsGroup: null
            });

        } else {
            dTableTender.clear().rows.add(userdata).draw();

        }
    },
    //#endregion dt tender code


    //#region warranty

    GetInvoiceItemForAddWarranty: function (ref, invoiceNo) {
        var jsonParam = { invoiceNo: invoiceNo };
        var serviceUrl = "/Pos/GetWarrantyProduct/";
        JsManager.SendJsonAsyncON(serviceUrl, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            Manager.LoadDataTableWarranty(jsonData, ref);
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },
    LoadDataTableWarranty: function (userdata, isRef) {
        if (isRef == "0") {
            dTableWarranty = $('#dTableWarranty').DataTable({
                dom: '<"tableToolbar">rt',
                buttons: [],
                initComplete: function () {
                    $('#dTableWarranty').parent().css({
                        'minHeight': '300px',
                        'borderLeft': '1px solid #dddddd',
                        'borderRight': '1px solid #dddddd',
                        'background': '#fff'
                    });

                },
                scrollY: "300px",
                scrollX: true,
                scrollCollapse: true,
                lengthMenu: [[-1], ["All"]],
                columnDefs: [
                    { visible: false, targets: [1] },
                    { className: "dt-head-right", targets: [0, 1, 2] }
                ],
                "order": [],
                columns: [
                    {
                        name: '',
                        orderable: false,
                        width: 7,
                        title: '#',
                        render: function (data, type, row, meta) {
                            return dTableManager.IndexColumn(meta.row + 1);
                        }
                    },
                    {
                        data: 'PosSalesInvoiceProductId',
                        name: 'PosSalesInvoiceProductId',
                        orderable: false,
                        title: 'PosSalesInvoiceProductId'
                    },
                    {
                        data: 'Code',
                        name: 'Code',
                        orderable: false,
                        title: 'Code',
                        render: function (data, type, row, meta) {
                            return "<input type='text' id='row-" + meta.row + "-Code' name='row-" + meta.row + "-Code' value='" + data + "' class='dtWarrantyInputs minWidth100 form-control input-sm font-size-12px font-weight readOnlyInputBackground' readonly='readonly'/>";
                        }
                    },
                    {
                        data: 'ProductName',
                        name: 'ProductName',
                        orderable: false,
                        title: 'Product Name',
                        render: function (data, type, row, meta) {
                            return "<input type='text' id='row-" + meta.row + "-ProductName' name='row-" + meta.row + "-ProductName' value='" + data + "' class='dtWarrantyInputs minWidth190 form-control input-sm font-size-12px font-weight readOnlyInputBackground' readonly='readonly'/>";
                        }

                    },
                    {
                        data: 'BatchName',
                        name: 'BatchName',
                        orderable: false,
                        title: 'Batch/Size',
                        render: function (data, type, row, meta) {
                            return "<input type='text' id='row-" + meta.row + "-BatchName' name='row-" + meta.row + "-BatchName' value='" + data + "' class='dtWarrantyInputs minWidth90 form-control input-sm font-size-12px font-weight readOnlyInputBackground' readonly='readonly'/>";
                        }

                    },
                    {
                        data: 'Qty',
                        name: 'Qty',
                        orderable: false,
                        title: 'Qty',
                        render: function (data, type, row, meta) {
                            return "<input type='text' id='row-" + meta.row + "-Qty' name='row-" + meta.row + "-Qty' value='" + data + "' class='dtWarrantyInputs text-center minWidth70 form-control input-sm font-size-12px font-weight readOnlyInputBackground' readonly='readonly'/>";
                        }

                    },
                    {
                        data: 'SerialOrImeiNo',
                        name: 'SerialOrImeiNo',
                        orderable: false,
                        title: 'Serial / IMEI',
                        render: function (data, type, row, meta) {
                            return "<input type='text' id='row-" + meta.row + "-SerialOrImeiNo' name='row-" + meta.row + "-SerialOrImeiNo' value='" + (data === 'null' ? "" : data) + "' class='dtWarrantyInputs dtWarrantySerialOrImeiNo text-center form-control input-sm font-size-12px font-weight minWidth120'/>";
                        }
                    },
                    {
                        data: 'WarrantyPeriod',
                        name: 'WarrantyPeriod',
                        orderable: false,
                        title: 'W. Period',
                        render: function (data, type, row, meta) {
                            return "<input type='text' id='row-" + meta.row + "-WarrantyPeriod' name='row-" + meta.row + "-WarrantyPeriod' value='" + data + "' class='dtWarrantyInputs dtWarrantyPeriod text-center form-control input-sm font-size-12px font-weight minWidth80'/>";
                        }
                    },
                    {
                        data: 'WarrantyType',
                        name: 'WarrantyType',
                        orderable: false,
                        title: 'W. Type',
                        render: function (data, type, row, meta) {
                            var dropDownData = '<select class="dtWarrantyInputs dtWarrantyType form-control input-sm font-size-12px font-weight minWidth120 id="row-' + meta.row + '-WarrantyType" name="row-' + meta.row + '-WarrantyType">' +
                                '<option ' + (data === 0 ? "selected" : "") + ' value="0">No Warranty</option>' +
                                '<option ' + (data === 1 ? "selected" : "") + ' value="1">Year Warranty</option>' +
                                '<option ' + (data === 2 ? "selected" : "") + ' value="2">Month Warranty</option>' +
                                '<option ' + (data === 3 ? "selected" : "") + ' value="3">Days Warranty</option>' +
                                '</select>';
                            return dropDownData;
                        }
                    },
                    {
                        data: 'Remarks',
                        name: 'Remarks',
                        orderable: false,
                        title: 'Remarks',
                        render: function (data, type, row, meta) {
                            return "<input type='text' id='row-" + meta.row + "-WRemarks' name='row-" + meta.row + "-WRemarks' value='" + (data == null ? "" : data) + "' class='dtWarrantyInputs dtWarrantyRemarks form-control input-sm font-size-12px font-weight minWidth190'/>";
                        }
                    }
                ],
                fixedColumns: false,
                data: userdata,
                rowsGroup: null,
                ordering: false
            });

        } else {
            dTableWarranty.clear().rows.add(userdata).draw();

        }
    },
    dtWarrantyEventManager: function (key, inputs, idx, $item) {
        if (key.keyCode == 13) {
            key.preventDefault();
            inputs[idx + 1].focus();
            inputs[idx + 1].select();
        } else if (key.keyCode == 37) {
            key.preventDefault();
            inputs[idx - 1].focus();
            inputs[idx - 1].select();
        } else if (key.keyCode == 38) {
            key.preventDefault();
            var uPArrowInputs = $item.parents('tr').prev().find('.dtWarrantyInputs');
            uPArrowInputs[idx].focus();
            uPArrowInputs[idx].select();
        } else if (key.keyCode == 39) {
            key.preventDefault();
            inputs[idx + 1].focus();
            inputs[idx + 1].select();
        } else if (key.keyCode == 40) {
            key.preventDefault();
            var downArrowInputs = $item.parents('tr').next().find('.dtWarrantyInputs');
            downArrowInputs[idx].focus();
            downArrowInputs[idx].select();
        }
    },
    InsertOrUpdateWarrantyData: function () {
        if (dTableWarranty.rows().count() > 0) {
            if (Message.Prompt("Do you want to warranty issue?")) {
                var posSalesInvoiceWarranty = [];
                $.each($('#dTableWarranty tbody tr'), function (rowIdx, val) {
                    var objPosSalesInvoiceWarranty = new Object();
                    objPosSalesInvoiceWarranty.PosSalesInvoiceProductId = dTableWarranty.cell(rowIdx, 1).data();
                    objPosSalesInvoiceWarranty.SerialOrImeiNo = $(val).find(".dtWarrantySerialOrImeiNo").val();
                    objPosSalesInvoiceWarranty.WarrantyPeriod = $(val).find(".dtWarrantyPeriod").val();
                    objPosSalesInvoiceWarranty.WarrantyType = $(val).find(".dtWarrantyType").val();
                    objPosSalesInvoiceWarranty.Remarks = $(val).find(".dtWarrantyRemarks").val();

                    posSalesInvoiceWarranty.push(objPosSalesInvoiceWarranty);

                });

                var jsonParam = {
                    salesInvoiceWarranty: posSalesInvoiceWarranty
                };
                jsonParam = JSON.stringify(jsonParam);
                var serviceUrl = "/Pos/InsertOrUpdateWarrantyIssue/";
                JsManager.SendJson(serviceUrl, jsonParam, onSuccess, onFailed);

                function onSuccess(jsonData) {
                    if (jsonData == 0) {
                        Message.Error("save");
                    } else if (jsonData == "-4") {
                        Message.Warning("Failed to save or update.");
                    }
                    else if (jsonData == "1") {
                        Message.Success("Successfully Saved with updated.");
                    } else if (jsonData == "2") {
                        Message.Success("Successfully Saved.");
                    } else if (jsonData == "3") {
                        Message.Success("Successfully Updated.");
                    }

                }

                function onFailed(xhr, status, err) {
                    Message.Exception(xhr);
                }
            }

        } else {
            Message.Warning("Product / Item not found to add warranty issue.");
        }
    },

    //#endregion warranty

}



var productInfos = null;
var prdFindBySystem = 0;
var isSearchOn = 0;
var productCodeId = "";

//$(function () {
//    fetch("/pos/GetProductInfoCallCenter").then(response => response.json().then(jsonData => {
//        productInfos = jsonData;
//    }));
    $(document).on("keyup", ".dTableProductCode, #txtItemOrBarcode", function (itemKey) {
        if (itemKey.key == "F2") {
            $("#txtProductName").val('');
            $("#txtProductNameAC").html("");
            $("#txtProductNameAC").css("display", "none !important");
            $("#txtScientificName").val('');
            $("#txtScientificNameAC").html("");
            $("#txtScientificNameAC").css("display", "none !important");
            var elInput = this;
            var elWrap = document.querySelector("#txtItemOrBarcodeAC");
            var searchCloseDiv = '<div id="divCloseSearch" style="position: absolute;z-index: 100;right: -1px;top: -30px;font-size: 20px;color: red;cursor: pointer;height: 29px;background: #efecea;width: 32px;border-top: 2px solid #dcdad7;border-left: 2px solid #dcdad7;border-right: 2px solid #dcdad7;border-radius: 4px;padding-top: 0px;padding-left: 4px;"><i class="fas fa-window-close"></i></div>';
            var elCheckBox = document.querySelector("#txtItemOrBarcodeACC");
            if (itemKey.key == "F4") {
                if (!elCheckBox.checked) {
                    $("#txtItemOrBarcodeACC").prop('checked', true);
                }
                else {
                    $("#txtItemOrBarcodeACC").prop('checked', false);
                }

                return;
            } else if (itemKey.key == "F8") {
                if (!document.querySelector("#chkEnableQtyChangeOption").checked) {
                    $("#chkEnableQtyChangeOption").prop('checked', true);
                }
                else {
                    $("#chkEnableQtyChangeOption").prop('checked', false);
                }

                return;
            }

            if (itemKey.key == "ArrowDown") {
                if (elWrap.childElementCount > 0) {
                    let elFirst = document.querySelector(".txtItemOrBarcodeACI");
                    elFirst.setAttribute("class", "txtItemOrBarcodeACI active");
                    elFirst.parentNode.parentNode.setAttribute("class", "txtItemOrBarcodeACTR active");
                    elFirst.focus();
                }
                return;
            }
            $(elWrap).css("display", "none");
            elWrap.innerHTML = '<table class="table table-search table-sm table-bordered" style="min-width:700px;background: #f7f7f7;margin-bottom:7px;">' +
                '<thead class="theadPrdSrcTbl">' +
                '<tr style="margin:0;padding:0;">' +
                '<th>Product Name</th>' +
                '<th>Code</th>' +
                // '<th>Bar Code</th>' +
                '<th>Batch/Size</th>' +
                '<th>Stock</th>' +
                '<th>Rate</th>' +
                '</tr>' +
                '</thead>' +
                '<tbody id="txtItemOrBarcodeACTR">' +
                '</tbody>' +
                '</table>' +
                '</div>';
           

            let elWrapTbody = document.querySelector("#txtItemOrBarcodeACTR");
            if (elCheckBox.checked && elInput.value.length > 0) {
                  var matchs = false;
                    ////////////
                var PosBranchId = $("#PosBranchId").val();
                if (PosBranchId != 0) {
                    var serviceUrl = '/Pos/GetProductInfoCallCenter/';
                    var jsonParam = { PosBranchId: PosBranchId, Parcode: elInput.value, Name: elInput.value, ScientificName: '' };
                    JsManager.SendJsonAsyncON(serviceUrl, jsonParam, onSuccess, onFailed);

                    function onSuccess(jsonData) {
                        // debugger;
                        // JsManager.PopulateCombo('#txtCodeOrBarcode', response);
                        var productInfos = new Object();
                        productInfos = jsonData;
                        //debugger;
                        productInfos.forEach(function (pItem, ind) {
                            //if ((
                            //    pItem.ProductCode.search(serXp) != -1 ||
                            //    // pItem.ProductName.search(serXp) != -1 ||
                            //    pItem.ProductBarCode.search(serXp) != -1
                            //    //|| pItem.BatchName.search(serXp) != -1
                            //)
                            //    && (pItem.PosBranchId == PosBranchId)
                            //) {
                            if (pItem.Stock > 0) {
                                matchs = true;
                                let elFound = document.createElement("tr");
                                elFound.innerHTML = '<td class="txtItemOrBarcodeACITD"><button class="txtItemOrBarcodeACI" value="' + pItem.ProductBarCode + ";" + pItem.BatchName + '">' + pItem.ProductName + '</button></td>' +
                                    '<td>' + pItem.ProductCode + '</td>' +
                                    //  '<td>' + pItem.ProductBarCode + '</td>' +
                                    '<td>' + pItem.BatchName + '</td>' +
                                    '<td>' + pItem.Stock + '</td>' +

                                    '<td style="text-align:right;">' + pItem.SellingRate + '/-</td>';

                                $(elWrap).find("#divCloseSearch").remove();
                                $(elWrap).append(searchCloseDiv);
                                elWrapTbody.appendChild(elFound);

                            }
                            // }

                        });

                        if (matchs) {
                            $(elWrap).css("display", "inline-table");
                        }
                    }
                    function onFailed() {
                    }
                }
                else {
                    Message.Error("Please Select Branch");
                }
            }
        }
    });


    ////
$(document).on("keyup", ".dTableProductCode, #txtScientificName", function (itemKey) {
    if (itemKey.key == "F2") {
        $("#txtProductName").val('');
        $("#txtProductNameAC").html("");
        $("#txtProductNameAC").css("display", "none !important");
        $("#txtItemOrBarcode").val('');
        $("#txtItemOrBarcodeAC").html("");
        $("#txtItemOrBarcodeAC").css("display", "none !important");

        var elInput = this;
        var elWrap = document.querySelector("#txtScientificNameAC");
        var searchCloseDiv = '<div id="divCloseSearchScientificName" style="position: absolute;z-index: 100;right: -1px;top: -30px;font-size: 20px;color: red;cursor: pointer;height: 29px;background: #efecea;width: 32px;border-top: 2px solid #dcdad7;border-left: 2px solid #dcdad7;border-right: 2px solid #dcdad7;border-radius: 4px;padding-top: 0px;padding-left: 4px;"><i class="fas fa-window-close"></i></div>';
        var elCheckBox = document.querySelector("#txtScientificNameACC");
        if (itemKey.key == "F4") {
            if (!elCheckBox.checked) {
                $("#txtScientificNameACC").prop('checked', true);
            }
            else {
                $("#txtScientificNameACC").prop('checked', false);
            }

            return;
        } else if (itemKey.key == "F8") {
            if (!document.querySelector("#chkEnableQtyScientificName").checked) {
                $("#chkEnableQtyScientificName").prop('checked', true);
            }
            else {
                $("#chkEnableQtyScientificName").prop('checked', false);
            }

            return;
        }

        if (itemKey.key == "ArrowDown") {
            if (elWrap.childElementCount > 0) {
                let elFirst = document.querySelector(".txtScientificNameACI");
                elFirst.setAttribute("class", "txtScientificNameACI active");
                elFirst.parentNode.parentNode.setAttribute("class", "txtScientificNameACTR active");
                elFirst.focus();
            }
            return;
        }
        $(elWrap).css("display", "none");
        elWrap.innerHTML = '<table class="table table-search table-sm table-bordered" style="min-width:700px;background: #f7f7f7;margin-bottom:7px;">' +
            '<thead class="theadPrdSrcTbl">' +
            '<tr style="margin:0;padding:0;">' +
            '<th>Scientific Name</th>' +
            '<th>Code</th>' +
            '<th>Branch Name</th>' +
           // '<th>Batch/Size</th>' +
            '<th>Stock</th>' +
           // '<th>Rate</th>' +
            '</tr>' +
            '</thead>' +
            '<tbody id="txtScientificNameACTR">' +
            '</tbody>' +
            '</table>' +
            '</div>';
        let elWrapTbody = document.querySelector("#txtScientificNameACTR");
        if (elCheckBox.checked && elInput.value.length > 0) {
            var matchs = false;
            ////////////
            //var PosBranchId = $("#PosBranchId").val();
            var serviceUrl = '/Pos/GetProductInfoCallCenter/';
            var jsonParam = { PosBranchId: 0, Parcode: '', Name: '', ScientificName:elInput.value };
            JsManager.SendJsonAsyncON(serviceUrl, jsonParam, onSuccess, onFailed);

            function onSuccess(jsonData) {
                // debugger;
                // JsManager.PopulateCombo('#txtCodeOrBarcode', response);
                var productInfos = new Object();
                productInfos = jsonData;
                //debugger;
                productInfos.forEach(function (pItem, ind) {
                    //if ((
                    //    pItem.ProductCode.search(serXp) != -1 ||
                    //    // pItem.ProductName.search(serXp) != -1 ||
                    //    pItem.ProductBarCode.search(serXp) != -1
                    //    //|| pItem.BatchName.search(serXp) != -1
                    //)
                    //    && (pItem.PosBranchId == PosBranchId)
                    //) {
                    matchs = true;
                    let elFound = document.createElement("tr");
                    elFound.innerHTML = '<td class="txtScientificNameACITD"><button class="txtScientificNameACI" value="' + pItem.ProductCode + ";" + /*pItem.BatchName +*/ '">' + pItem.PosProductScientificName + '</button></td>' +
                        '<td>' + pItem.ProductCode + '</td>' +
                        '<td>' + pItem.BranchName + '</td>' +
                        // '<td>' + pItem.BatchName + '</td>' +
                        '<td>' + pItem.Stock + '</td>';
                       // +
                      //  '<td style="text-align:right;">' + pItem.SellingRate + '/-</td>';
                    $(elWrap).find("#divCloseSearchScientificName").remove();
                    $(elWrap).append(searchCloseDiv);
                    elWrapTbody.appendChild(elFound);
                });

                if (matchs) {
                    $(elWrap).css("display", "inline-table");
                }
            }
            function onFailed() {
            }
            
        }
    }
    });
    ////
    $(document).on("keyup", ".dTableProductCode, #txtProductName", function (itemKey) {
        if (itemKey.key == "F2") {
            $("#txtItemOrBarcode").val('');
            $("#txtItemOrBarcodeAC").html("");
            $("#txtItemOrBarcodeAC").css("display", "none !important");
            $("#txtScientificName").val('');
            $("#txtScientificNameAC").html("");
            $("#txtScientificNameAC").css("display", "none !important");

        var elInput = this;
        var elWrap = document.querySelector("#txtProductNameAC");
        var searchCloseDiv = '<div id="divCloseSearchProductName" style="position: absolute;z-index: 100;right: -1px;top: -30px;font-size: 20px;color: red;cursor: pointer;height: 29px;background: #efecea;width: 32px;border-top: 2px solid #dcdad7;border-left: 2px solid #dcdad7;border-right: 2px solid #dcdad7;border-radius: 4px;padding-top: 0px;padding-left: 4px;"><i class="fas fa-window-close"></i></div>';
        var elCheckBox = document.querySelector("#txtProductNameACC");
        if (itemKey.key == "F4") {
            if (!elCheckBox.checked) {
                $("#txtProductNameACC").prop('checked', true);
            }
            else {
                $("#txtProductNameACC").prop('checked', false);
            }

            return;
        } else if (itemKey.key == "F8") {
            if (!document.querySelector("#chkEnableQtyProductName").checked) {
                $("#chkEnableQtyProductName").prop('checked', true);
            }
            else {
                $("#chkEnableQtyProductName").prop('checked', false);
            }

            return;
        }

        if (itemKey.key == "ArrowDown") {
            if (elWrap.childElementCount > 0) {
                let elFirst = document.querySelector(".txtProductNameACI");
                elFirst.setAttribute("class", "txtProductNameACI active");
                elFirst.parentNode.parentNode.setAttribute("class", "txtProductNameACTR active");
                elFirst.focus();
            }
            return;
        }
        $(elWrap).css("display", "none");
        elWrap.innerHTML = '<table class="table table-search table-sm table-bordered" style="min-width:700px;background: #f7f7f7;margin-bottom:7px;">' +
            '<thead class="theadPrdSrcTbl">' +
            '<tr style="margin:0;padding:0;">' +
            '<th>Product Name</th>' +
            '<th>Code</th>' +
            '<th>Branch Name</th>' +
            //'<th>Batch/Size</th>' +
            '<th>Stock</th>' +
           // '<th>Rate</th>' +
            '</tr>' +
            '</thead>' +
            '<tbody id="txtProductNameACTR">' +
            '</tbody>' +
            '</table>' +
            '</div>';
        let elWrapTbody = document.querySelector("#txtProductNameACTR");
        if (elCheckBox.checked && elInput.value.length > 0) {
            var matchs = false;
            ////////////
            //var PosBranchId = $("#PosBranchId").val();
            var serviceUrl = '/Pos/GetProductInfoCallCenter/';
            var jsonParam = { PosBranchId: 0, Parcode: '', Name: elInput.value, ScientificName:''};
            JsManager.SendJsonAsyncON(serviceUrl, jsonParam, onSuccess, onFailed);

            function onSuccess(jsonData) {
                // debugger;
                // JsManager.PopulateCombo('#txtCodeOrBarcode', response);
                var productInfos = new Object();
                productInfos = jsonData;
                //debugger;
                productInfos.forEach(function (pItem, ind) {
                    //if ((
                    //    pItem.ProductCode.search(serXp) != -1 ||
                    //    // pItem.ProductName.search(serXp) != -1 ||
                    //    pItem.ProductBarCode.search(serXp) != -1
                    //    //|| pItem.BatchName.search(serXp) != -1
                    //)
                    //    && (pItem.PosBranchId == PosBranchId)
                    //) {
                    matchs = true;
                    let elFound = document.createElement("tr");
                    elFound.innerHTML = '<td class="txtProductNameACITD"><button class="txtProductNameACI" value="' + pItem.ProductCode + ";" + /*pItem.BatchName +*/ '">' + pItem.ProductName + '</button></td>' +
                        '<td>' + pItem.ProductCode + '</td>' +
                        '<td>' + pItem.BranchName + '</td>' +
                        //  '<td>' + pItem.BatchName + '</td>' +
                        '<td>' + pItem.Stock + '</td>';
                        //+
                    //    '<td style="text-align:right;">' + pItem.SellingRate + '/-</td>';
                    $(elWrap).find("#divCloseSearchProductName").remove();
                    $(elWrap).append(searchCloseDiv);
                    elWrapTbody.appendChild(elFound);
                });

                if (matchs) {
                    $(elWrap).css("display", "inline-table");
                }
            }
            function onFailed() {
            }  
            }
        }
    });

$(document).on('keydown', function (e) {

    if (e.target && (e.target.getAttribute("class") == 'txtItemOrBarcodeACI' || e.target.getAttribute("class") == 'txtItemOrBarcodeACI active')) {
        e.preventDefault();
        e.stopPropagation();

        if (e.key == "ArrowUp") {
            if (e.target.parentNode.parentNode.previousSibling && e.target.parentNode.parentNode.previousSibling.querySelector("button")) {
                let preVN = e.target.parentNode.parentNode.previousSibling.querySelector("button");
                e.target.setAttribute("class", "txtItemOrBarcodeACI");
                e.target.parentNode.parentNode.setAttribute("class", "");
                preVN.setAttribute("class", "txtItemOrBarcodeACI active");
                preVN.parentNode.parentNode.setAttribute("class", "txtItemOrBarcodeACTR active");
                preVN.focus();
            } else {
                e.target.parentNode.parentNode.setAttribute("class", "");
                e.target.setAttribute("class", "txtItemOrBarcodeACI");
                document.querySelector("#txtItemOrBarcode").focus();
            }
        } else if (e.key == "ArrowDown") {
            if (e.target.parentNode.parentNode.nextSibling && e.target.parentNode.parentNode.nextSibling.querySelector("button")) {
                let nextVN = e.target.parentNode.parentNode.nextSibling.querySelector("button");
                e.target.parentNode.parentNode.setAttribute("class", "");
                e.target.setAttribute("class", "txtItemOrBarcodeACI");
                nextVN.setAttribute("class", "txtItemOrBarcodeACI active");
                nextVN.parentNode.parentNode.setAttribute("class", "txtItemOrBarcodeACTR active");
                nextVN.focus();
            }
        } else if (e.key == "Enter") {
            document.querySelector("#txtItemOrBarcode").value = e.target.value;
            document.querySelector("#txtItemOrBarcode").focus();
            if (prdFindBySystem === 1) {
                $("#txtItemOrBarcodeACC").prop("checked", false);
                prdFindBySystem = 0;
            }
            document.querySelector("#txtItemOrBarcodeAC").innerHTML = "";
            document.querySelector("#txtItemOrBarcodeAC").style.display = "None !important";
            Manager.AddBillingItem(e.target.value);
        } else if (e.key == "Escape") {
            $("#txtItemOrBarcode").val('');
            $("#txtItemOrBarcode").focus();
            $("#txtItemOrBarcodeACC").prop("checked", false);
            document.querySelector("#txtItemOrBarcodeAC").innerHTML = "";
            document.querySelector("#txtItemOrBarcodeAC").style.display = "None !important";
        }
    }
    if (e.key == "F12") {
        $("#txtItemOrBarcode").val('');
        $("#txtItemOrBarcode").focus();
    } else if (e.key == "F7") {
        $(".txtCashReceiptTender").focus();
        $(".txtCashReceiptTender").select();
    } else if (e.ctrlKey && e.keyCode == 46) {
        //delete billing item
        event.preventDefault();
        var dtTr = $("#" + document.activeElement.id).parents('tr');
        dTableBilling.row(dtTr).remove().draw();
        Manager.getScheme();
        Manager.BillingTotalCalculation(0);
    } else if (e.ctrlKey && e.keyCode == 'S'.charCodeAt(0)) {
        //save invoice 
        e.preventDefault();
        if ($("#btnSaveInvoiceStockTrns").length > 0) {
            $("#btnSaveInvoiceStockTrns").trigger("click");
        } else {
            $("#btnSaveInvoice").trigger("click");
        }
        e.preventDefault();
    } else if (e.ctrlKey && e.keyCode == 67) {
        //cancel invoice 
        e.preventDefault();
        $("#btnCancel").trigger("click");
    } else if (e.ctrlKey && e.keyCode == 82) {
        //re print invoice 
        e.preventDefault();
        $("#btnReprintInv").trigger("click");
    } else if (e.ctrlKey && e.keyCode == 80) {
        //print last invoice 
        e.preventDefault();
        $("#btnPrintLastInv").trigger("click");
    } else if (e.keyCode == 123) {
        e.preventDefault();
        return false;
    } else if (e.ctrlKey && e.shiftKey && e.keyCode == 'I'.charCodeAt(0)) {
        e.preventDefault();
        return false;
    } else if (e.ctrlKey && e.shiftKey && e.keyCode == 'C'.charCodeAt(0)) {
        e.preventDefault();
        return false;
    } else if (e.ctrlKey && e.shiftKey && e.keyCode == 'J'.charCodeAt(0)) {
        e.preventDefault();
        return false;
    } else if (e.ctrlKey && e.keyCode == 'U'.charCodeAt(0)) {
        e.preventDefault();
        return false;
    }

});

$(document).on('click', function (e) {
    if (e.target && (e.target.getAttribute("class") == 'txtItemOrBarcodeACI' || e.target.getAttribute("class") == 'txtItemOrBarcodeACI active')) {
        e.preventDefault();
        e.stopPropagation();

        document.querySelector("#txtItemOrBarcode").value = e.target.value;
        document.querySelector("#txtItemOrBarcode").focus();
        if (prdFindBySystem === 1) {
            $("#txtItemOrBarcodeACC").prop("checked", false);
            prdFindBySystem = 0;
        }
        document.querySelector("#txtItemOrBarcodeAC").innerHTML = "";
        document.querySelector("#txtItemOrBarcodeAC").style.display = "None !important";
        Manager.AddBillingItem(e.target.value);

    }
});

//$(document).on('keydown', function (e) {

//    if (e.target && (e.target.getAttribute("class") == 'txtScientificNameACI' || e.target.getAttribute("class") == 'txtScientificNameACI active')) {
//        e.preventDefault();
//        e.stopPropagation();

//        if (e.key == "ArrowUp") {
//            if (e.target.parentNode.parentNode.previousSibling && e.target.parentNode.parentNode.previousSibling.querySelector("button")) {
//                let preVN = e.target.parentNode.parentNode.previousSibling.querySelector("button");
//                e.target.setAttribute("class", "txtScientificNameACI");
//                e.target.parentNode.parentNode.setAttribute("class", "");
//                preVN.setAttribute("class", "txtScientificNameACI active");
//                preVN.parentNode.parentNode.setAttribute("class", "txtScientificNameACTR active");
//                preVN.focus();
//            } else {
//                e.target.parentNode.parentNode.setAttribute("class", "");
//                e.target.setAttribute("class", "txtScientificNameACI");
//                document.querySelector("#txtScientificName").focus();
//            }
//        } else if (e.key == "ArrowDown") {
//            if (e.target.parentNode.parentNode.nextSibling && e.target.parentNode.parentNode.nextSibling.querySelector("button")) {
//                let nextVN = e.target.parentNode.parentNode.nextSibling.querySelector("button");
//                e.target.parentNode.parentNode.setAttribute("class", "");
//                e.target.setAttribute("class", "txtScientificNameACI");
//                nextVN.setAttribute("class", "txtScientificNameACI active");
//                nextVN.parentNode.parentNode.setAttribute("class", "txtScientificNameACTR active");
//                nextVN.focus();
//            }
//        } else if (e.key == "Enter") {
//            document.querySelector("#txtScientificName").value = e.target.value;
//            document.querySelector("#txtScientificName").focus();
//            if (prdFindBySystem === 1) {
//                $("#txtScientificNameACC").prop("checked", false);
//                prdFindBySystem = 0;
//            }
//            document.querySelector("#txtScientificNameAC").innerHTML = "";
//            document.querySelector("#txtScientificNameAC").style.display = "None !important";
//            Manager.AddBillingItem(e.target.value);
//        } else if (e.key == "Escape") {
//            $("#txtScientificName").val('');
//            $("#txtScientificName").focus();
//            $("#txtScientificNameACC").prop("checked", false);
//            document.querySelector("#txtScientificNameAC").innerHTML = "";
//            document.querySelector("#txtScientificNameAC").style.display = "None !important";
//        }
//    }
//    if (e.key == "F12") {
//        $("#txtScientificName").val('');
//        $("#txtScientificName").focus();
//    } else if (e.key == "F7") {
//        $(".txtCashReceiptTender").focus();
//        $(".txtCashReceiptTender").select();
//    } else if (e.ctrlKey && e.keyCode == 46) {
//        //delete billing item
//        event.preventDefault();
//        var dtTr = $("#" + document.activeElement.id).parents('tr');
//        dTableBilling.row(dtTr).remove().draw();
//        Manager.getScheme();
//        Manager.BillingTotalCalculation(0);
//    } else if (e.ctrlKey && e.keyCode == 'S'.charCodeAt(0)) {
//        //save invoice 
//        e.preventDefault();
//        if ($("#btnSaveInvoiceStockTrns").length > 0) {
//            $("#btnSaveInvoiceStockTrns").trigger("click");
//        } else {
//            $("#btnSaveInvoice").trigger("click");
//        }
//        e.preventDefault();
//    } else if (e.ctrlKey && e.keyCode == 67) {
//        //cancel invoice 
//        e.preventDefault();
//        $("#btnCancel").trigger("click");
//    } else if (e.ctrlKey && e.keyCode == 82) {
//        //re print invoice 
//        e.preventDefault();
//        $("#btnReprintInv").trigger("click");
//    } else if (e.ctrlKey && e.keyCode == 80) {
//        //print last invoice 
//        e.preventDefault();
//        $("#btnPrintLastInv").trigger("click");
//    } else if (e.keyCode == 123) {
//        e.preventDefault();
//        return false;
//    } else if (e.ctrlKey && e.shiftKey && e.keyCode == 'I'.charCodeAt(0)) {
//        e.preventDefault();
//        return false;
//    } else if (e.ctrlKey && e.shiftKey && e.keyCode == 'C'.charCodeAt(0)) {
//        e.preventDefault();
//        return false;
//    } else if (e.ctrlKey && e.shiftKey && e.keyCode == 'J'.charCodeAt(0)) {
//        e.preventDefault();
//        return false;
//    } else if (e.ctrlKey && e.keyCode == 'U'.charCodeAt(0)) {
//        e.preventDefault();
//        return false;
//    }

//});

//$(document).on('click', function (e) {
//    if (e.target && (e.target.getAttribute("class") == 'txtScientificNameACI' || e.target.getAttribute("class") == 'txtScientificNameACI active')) {
//        e.preventDefault();
//        e.stopPropagation();

//        document.querySelector("#txtScientificName").value = e.target.value;
//        document.querySelector("#txtScientificName").focus();
//        if (prdFindBySystem === 1) {
//            $("#txtScientificNameACC").prop("checked", false);
//            prdFindBySystem = 0;
//        }
//        document.querySelector("#txtScientificNameAC").innerHTML = "";
//        document.querySelector("#txtScientificNameAC").style.display = "None !important";
//        Manager.AddBillingItem(e.target.value);

//    }
//});
//$(document).on('keydown', function (e) {

//    if (e.target && (e.target.getAttribute("class") == 'txtProductNameACI' || e.target.getAttribute("class") == 'txtProductNameACI active')) {
//        e.preventDefault();
//        e.stopPropagation();

//        if (e.key == "ArrowUp") {
//            if (e.target.parentNode.parentNode.previousSibling && e.target.parentNode.parentNode.previousSibling.querySelector("button")) {
//                let preVN = e.target.parentNode.parentNode.previousSibling.querySelector("button");
//                e.target.setAttribute("class", "txtProductNameACI");
//                e.target.parentNode.parentNode.setAttribute("class", "");
//                preVN.setAttribute("class", "txtProductNameACI active");
//                preVN.parentNode.parentNode.setAttribute("class", "txtProductNameACTR active");
//                preVN.focus();
//            } else {
//                e.target.parentNode.parentNode.setAttribute("class", "");
//                e.target.setAttribute("class", "txtProductNameACI");
//                document.querySelector("#txtProductName").focus();
//            }
//        } else if (e.key == "ArrowDown") {
//            if (e.target.parentNode.parentNode.nextSibling && e.target.parentNode.parentNode.nextSibling.querySelector("button")) {
//                let nextVN = e.target.parentNode.parentNode.nextSibling.querySelector("button");
//                e.target.parentNode.parentNode.setAttribute("class", "");
//                e.target.setAttribute("class", "txtProductNameACI");
//                nextVN.setAttribute("class", "txtProductNameACI active");
//                nextVN.parentNode.parentNode.setAttribute("class", "txtProductNameACTR active");
//                nextVN.focus();
//            }
//        } else if (e.key == "Enter") {
//            document.querySelector("#txtProductName").value = e.target.value;
//            document.querySelector("#txtProductName").focus();
//            if (prdFindBySystem === 1) {
//                $("#txtProductNameACC").prop("checked", false);
//                prdFindBySystem = 0;
//            }
//            document.querySelector("#txtProductNameAC").innerHTML = "";
//            document.querySelector("#txtProductNameAC").style.display = "None !important";
//            Manager.AddBillingItem(e.target.value);
//        } else if (e.key == "Escape") {
//            $("#txtProductName").val('');
//            $("#txtProductName").focus();
//            $("#txtProductNameACC").prop("checked", false);
//            document.querySelector("#txtProductNameAC").innerHTML = "";
//            document.querySelector("#txtProductNameAC").style.display = "None !important";
//        }
//    }
//    if (e.key == "F12") {
//        $("#txtProductName").val('');
//        $("#txtProductName").focus();
//    } else if (e.key == "F7") {
//        $(".txtCashReceiptTender").focus();
//        $(".txtCashReceiptTender").select();
//    } else if (e.ctrlKey && e.keyCode == 46) {
//        //delete billing item
//        event.preventDefault();
//        var dtTr = $("#" + document.activeElement.id).parents('tr');
//        dTableBilling.row(dtTr).remove().draw();
//        Manager.getScheme();
//        Manager.BillingTotalCalculation(0);
//    } else if (e.ctrlKey && e.keyCode == 'S'.charCodeAt(0)) {
//        //save invoice 
//        e.preventDefault();
//        if ($("#btnSaveInvoiceStockTrns").length > 0) {
//            $("#btnSaveInvoiceStockTrns").trigger("click");
//        } else {
//            $("#btnSaveInvoice").trigger("click");
//        }
//        e.preventDefault();
//    } else if (e.ctrlKey && e.keyCode == 67) {
//        //cancel invoice 
//        e.preventDefault();
//        $("#btnCancel").trigger("click");
//    } else if (e.ctrlKey && e.keyCode == 82) {
//        //re print invoice 
//        e.preventDefault();
//        $("#btnReprintInv").trigger("click");
//    } else if (e.ctrlKey && e.keyCode == 80) {
//        //print last invoice 
//        e.preventDefault();
//        $("#btnPrintLastInv").trigger("click");
//    } else if (e.keyCode == 123) {
//        e.preventDefault();
//        return false;
//    } else if (e.ctrlKey && e.shiftKey && e.keyCode == 'I'.charCodeAt(0)) {
//        e.preventDefault();
//        return false;
//    } else if (e.ctrlKey && e.shiftKey && e.keyCode == 'C'.charCodeAt(0)) {
//        e.preventDefault();
//        return false;
//    } else if (e.ctrlKey && e.shiftKey && e.keyCode == 'J'.charCodeAt(0)) {
//        e.preventDefault();
//        return false;
//    } else if (e.ctrlKey && e.keyCode == 'U'.charCodeAt(0)) {
//        e.preventDefault();
//        return false;
//    }

//});

//$(document).on('click', function (e) {
//    if (e.target && (e.target.getAttribute("class") == 'txtProductNameACI' || e.target.getAttribute("class") == 'txtProductNameACI active')) {
//        e.preventDefault();
//        e.stopPropagation();

//        document.querySelector("#txtProductName").value = e.target.value;
//        document.querySelector("#txtProductName").focus();
//        if (prdFindBySystem === 1) {
//            $("#txtProductNameACC").prop("checked", false);
//            prdFindBySystem = 0;
//        }
//        document.querySelector("#txtProductNameAC").innerHTML = "";
//        document.querySelector("#txtProductNameAC").style.display = "None !important";
//        Manager.AddBillingItem(e.target.value);

//    }
//});


$(document).on("click", "#divCloseSearch", function () {
    $("#txtItemOrBarcode").val('');
    $("#txtItemOrBarcode").focus();
    $("#txtItemOrBarcodeAC").html("");
    $("#txtItemOrBarcodeAC").css("display", "none !important");

});

$(document).on("click", "#divCloseSearchProductName", function () {
    $("#txtProductName").val('');
    $("#txtProductName").focus();

    $("#txtProductNameAC").html("");
    $("#txtProductNameAC").css("display", "none !important");
});
$(document).on("click", "#divCloseSearchScientificName", function () {
    $("#txtScientificName").val('');
    $("#txtScientificName").focus();
    $("#txtScientificNameAC").html("");
    $("#txtScientificNameAC").css("display", "none !important");
});