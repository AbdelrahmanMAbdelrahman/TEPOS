var dTablePurchaseReceipt = null;
var stockTypeDropdownValue = null;

$(document).ready(function () {
    Manager.GetPurchaseReceipt(0);
    Manager.loadSupplier();
    Manager.loadBranch();
    Manager.loadStockType();
    Manager.addNewDTableRow();
    Manager.LoadHoldPurchaseInvoice();
    globalThis.InvoiceThatFind = false;
    $("#btnSave").prop("disabled", true);

    $("#Inv_OtherDiscount").keyup(function () {
        Manager.netAmtCalculation();
        $("#btnSave").prop("disabled", true);
    });

    $("#Inv_OtherCharges").keyup(function () {
        Manager.netAmtCalculation();
        $("#btnSave").prop("disabled", true);
    });
    //recall button
    $("#btnRecallInvoice").click(function () {
        Manager.FetchPurchaseReceipt($("#holdInvDropdown").val());

    });
    $("#btnRefresh").click(function () {
        $("#btnSave").prop("disabled", false);
        Manager.netAmtCalculation();
    });

    $("#btnCancel").click(function () {
        $("#btnSave").prop("disabled", true);
        Manager.resetForm();

    });
    $(document).on('click', '.delete-row', function () {
        debugger;
        var rowIdx = $(this).data('row');

        Manager.deletePurchaseReceipt(rowIdx);
    });
    $(document).on('click', '#deleteSelected', function () {
        debugger;
        const selectedCheckboxes = document.querySelectorAll(
            ".invoiceCheckbox:checked"
        );

        const selectedIds = Array.from(selectedCheckboxes).map(
            (checkbox) => checkbox.value
        );
        if (globalThis.InvoiceThatFind == true) {
            Manager.deletePurchaseReceipt(selectedIds);

        }
        else {
            Message.Error("Invoice not found to delete items");
        }
    });
    $(document).on('change', '.invoiceCheckbox', function () {
        // Initialize total amount
        let totalAmount = 0;

        // Get all checked checkboxes
        $('.invoiceCheckbox:checked').each(function () {
            const amount = parseFloat($(this).data('amount') || 0);
            totalAmount -= amount; // Always subtract the amount
        });

        // Update the text box with the new total (always negative)
        $('#Inv_TotalAmount').val(totalAmount.toFixed(2));
    });



    $("#btnSave").click(function () {
        Manager.savePurchaseReceipt(1);
    });
    $("#btnHoldInvoice").click(function () {
        Manager.savePurchaseReceipt(3);
    });
    $("#btnFind").click(function () {
        Manager.BranchTransfarStockReceive();
    });

    if ($("#CompanyInvNo").val() != "") {
        setTimeout(function () {
            Manager.BranchTransfarStockReceive();
        }, 500);
    }
    $("#btnCancelInvoice").click(function () {
        debugger;
        if ($("#btnSaveInvoiceStockTrns").length > 0) {
            //cancel stock transfer stock
            // Manager.cancelInvoiceForSave();
            Manager.CancelInvoice(5);
        } else {
            //Manager.cancelInvoiceForSave();
            Manager.CancelInvoice(0);
        }
    });
    $("#btnSearchInvoice").click(function () {
        const invNumber = $("#InvReferenceNo").val().trim();

        if (invNumber === "") {
            Message.Warning("Please input or select the invoice no.");
        } else {

            Manager.FetchPurchaseReceipt(invNumber);
        }
    });


});




$(document).on("click", ".dtBtnAddNewRow", function () {
    var inputs = $(this).parents('tr').find('.dtInputs');
    if ($(inputs[4]).val() != "" && $(inputs[0]).val() != "" && $(inputs[4]).val() != "0" && $(inputs[2]).val() != null && $(inputs[3]).val() != null && $(inputs[5]).val() != null) {
        Manager.addNewDTableRow();
        $item = $(this);
        var nextInput = $item.parents('tr').next().find('.dtInputs');
        nextInput[0].focus();
    } else {
        inputs[0].focus();
    }
});

$(document).on('click', '.dtBtnRemoveRow', function () {
    dTablePurchaseReceipt.row($(this).parents('tr')).remove().draw();
});

//Adel 17/8/2024
$(document).on('change', '.dTableBatchName', function () {
    var inputs = $(this).parents('tr').find('.dtInputs');
    $(inputs[8]).val($(inputs[2]).find(':selected').data('SellingRate'));
    $(inputs[9]).val($(inputs[2]).find(':selected').data('purchaserate'));
    $(inputs[10]).val($(inputs[2]).find(':selected').data('dTableDateTo'));

});


$(document).on('change', '.dtInputs', function (key) {
    var inputs = $(this).parents('tr').find('.dtInputs');
    Manager.calculateProductAmount(inputs);
});

$(document).on('keyup', '.dtInputs', function (key) {
    var inputs = $(this).parents('tr').find('.dtInputs');
    Manager.calculateProductAmount(inputs);
});

$(document).on('keydown', '.dtInputs', function (key) {
    var inputs = $(this).parents('tr').find('.dtInputs');
    var idx = inputs.index(this);
    var $item = $(this);
    Manager.dTableEventManager(key, inputs, idx, $item);
    Manager.calculateProductAmount(inputs);
});



$(document).on('click', '.dtInputs', function () {
    $(this).focus();
    $(this).select();
});
$(document).on("change", ".dTableDateTo", function () {
    const inputValue = $(this).val();
    const formattedDate = Manager.formatDate(inputValue);

    if (formattedDate) {
        $(this).val(formattedDate);
    } else {
        $(this).val('');
        alert("Please enter a valid date in the format ddMMyyyy.");
    }
});




var Manager = {

    BranchTransfarStockReceive() {
        var jsonParam = { invoiceNo: $("#CompanyInvNo").val() };
        var serviceURL = "/PosStock/GetStockTransfarInvoiceItem/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            if (jsonData.Status == 1 && jsonData.Result.toString().length > 0) {
                dTablePurchaseReceipt.rows().remove();

                $.each(jsonData.Result.ProductList, function (i, v) {
                    if (i >= 0) {
                        Manager.addNewDTableRow();
                    }

                    var inputs = $($("#dTablePurchaseReceipt").find('tr')[i + 1]).find('.dtInputs');

                    Manager.loadProductBatch(v.Code, inputs);
                    Manager.loadUomMaster(v.Code, inputs);
                    $(inputs[0]).val(v.Code);
                    $(inputs[1]).val(v.Name);
                    $(inputs[2]).val(v.PosProductBatchId);
                    $(inputs[4]).val(v.Qty);
                    $(inputs[5]).html(stockTypeDropdownValue);
                    $(inputs[6]).val(((v.SchDiscount / (v.Qty * v.PurchaseRate)) * 100).toFixed(2));
                    /* $(inputs[8]).val(v.PurchaseRate);*/
                    $(inputs[8]).val(v.SellingRate);
                    $(inputs[9]).val(v.PurchaseRate);
                    $(inputs[10]).val(v.DateTo);
                    // $(inputs[11]).val(v.PosProductHasExpire);

                    Manager.calculateProductAmount(inputs);
                });
                var lessDis = jsonData.Result.Invoice.Discount.toFixed(2);
                $("#Inv_OtherDiscount").val(lessDis);
                $("#NetPayable").val(jsonData.Result.Invoice.ReceivedAmount);
                $("#InvDate").val(JsManager.DMYToMDY(jsonData.Result.Invoice.InvDate));
                Manager.netAmtCalculation();
                $('.dTableQty').prop('readonly', true);
                $('.dTableProductCode ').prop('readonly', true);


            } else if (jsonData.Status == "InvoiceReceived") {
                Message.Warning("The invoice already received.");
            } else if (jsonData.Status == "InvalidInvoice") {
                Message.Warning("Invoice not found!");
            }
        }

        function onFailed(error) {
            inputs[0].focus();
            inputs[0].select();
            $(inputs[0]).val('');
            //  window.alert(error.statusText);
        }

    },


    resetForm: function () {
        dTablePurchaseReceipt.clear().row.add({
            'ProductCode': '',
            'ProductName': '',
            'BatchName': '',
            'PosUomMasterId': '',
            'Qty': '',
            'PosStockTypeId': '',
            'DiscountPer': '0.00',
            'Discount': '0.00',
            'SellingRate': '0.00',
            'PurchaseRate': '0.00',
            'DateTo': '',
            'Amount': '0.00',
            'PurchaseTax': '0.00'
        }).draw();
        $("#CompanyInvNo").val('');
        $("#InvReferenceNo").val('');
        $("#NetPayable").val('');
        $("#Inv_TotalAmount").val(0);
        $("#Inv_OtherCharges").val(0);
        $("#Inv_NetValue").val(0);
        $("#Inv_LessDiscount").val(0);
        $("#Inv_OtherDiscount").val(0);
        $("#Inv_PurchaseTax").val(0);

        $("#holdInvDropdown").val(0);
    },
    deletePurchaseReceipt: function (row) {
        var obj = [];
        globalThis.yourGlobalVariable = false;
        obj.push({ 'Invoice No': $("#InvReferenceNo").val() });
        obj.push({ 'Company Invoice No': $("#CompanyInvNo").val() });
        obj.push({ 'Invoice Date': $("#InvDate").val() });
        obj.push({ 'Inv. Receiv eDate': $("#InvReceiveDate").val() });
        // obj.push({ 'Net Payable': $("#NetPayable").val() });
        if (parseFloat($("#Inv_TotalAmount").val()) > 1) {
            Message.Warning("Select minimum one product.");
            return;
        }
        if (JsManager.validate(obj)) {
            var posStockDetail = [];
            var objStock = new Object();
            objStock.InvReferenceNo = $("#InvReferenceNo").val() + "1";
            objStock.CompanyInvNo = $("#CompanyInvNo").val() + "1";
            objStock.PosSupplierId = $("#PosSupplierId").val();
            objStock.PosBranchId = $("#PosBranchId").val();
            objStock.InvDate = $("#InvDate").val();
            objStock.InvReceiveDate = $("#InvReceiveDate").val();
            objStock.NetPayable = $("#NetPayable").val();
            objStock.PosInvoiceType = 4;
            if ($("#NetPayable").val() == '') {
                objStock.NetPayable = 0;
            }
            objStock.OtherDiscount = $("#Inv_OtherDiscount").val();
            objStock.OtherCharges = $("#Inv_OtherCharges").val();
            objStock.Remarks = $("#Inv_Remarks").val();

            let isValid = true;
            let missingExpireDates = [];
            $.each($('#dTablePurchaseReceipt tbody tr'), function (selectedIds, val) {
                debugger;
                if (row.includes(selectedIds.toString())) {
                    var objStockDetails = new Object();
                    var BatchDetailsObj = new Object();
                    var qty = parseFloat($(val).find(".dTableQty ").val()) * parseFloat($(val).find(".dTablePosUomMasterId").find(':selected').data('conversion'));
                    const dateToInput = $(val).find(".dTableDateTo").val();
                    const isExpireChecked = $(val).find(".dtHasExpireCheckbox").prop("checked");
                    if (isExpireChecked && !dateToInput) {
                        const productName = $(val).find(".dTableProductCode").val();
                        missingExpireDates.push(productName || `Row ${rowIdx + 1}`);
                        isValid = false;
                        return false;
                    }
                    if (!isNaN(qty) && qty > 0) {
                        var jsonParam = { productCode: $(val).find(".dTableProductCode").val() };
                        var serviceURL = "/PosStock/GetBatchByProductCode/";
                        JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccessbatch, onFailedbatch);

                        function onSuccessbatch(jsonData) {

                            var cbmOptions = "";
                            $.each(jsonData, function () {
                                BatchDetailsObj.Id = this.Id;
                                cbmOptions += '<option data-purchaserate=' + this.PurchaseRate + ' value=\"' + this.Id + '\">' + this.Name + '</option>';
                                cbmOptions += '<option data-SellingRate=' + this.SellingRate + ' value=\"' + this.Id + '\">' + this.Name + '</option>';

                            });


                        }

                        function onFailedbatch(error) {
                            window.alert(error.statusText);
                        }
                        var jsonParam = { productCode: $(val).find(".dTableProductCode").val() };
                        var serviceURL = "/PosDropDown/GetProductUom/";
                        JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccessProductUom, onFailedProductUom);

                        function onSuccessProductUom(jsonData) {
                            var cbmOptions = "";
                            $.each(jsonData, function () {
                                cbmOptions += '<option data-uomgroupid="' + this.PosUomGroupId + '" data-conversion="' + this.ConversionFactor + '" value="' + this.Id + '">' + this.Name + '</option>';
                            });
                            var isBasedUom = jsonData.filter(w => w.IsBaseUom == true)[0];
                            //$(inputs[3]).html(cbmOptions);
                            //$(inputs[3]).val(isBasedUom.Id);
                        }

                        function onFailedProductUom(error) {
                            window.alert(error.statusText);
                        }

                        var jsonParam = '';
                        var serviceURL = "/PosDropDown/GetStockType/";
                        JsManager.SendJson(serviceURL, jsonParam, onSuccessStockType, onFailedStockType);

                        function onSuccessStockType(jsonData) {
                            var cbmOptions = "";
                            $.each(jsonData, function () {
                                cbmOptions += '<option value=\"' + this.Id + '\">' + this.Name + '</option>';
                            });
                            $('.dTablePosStockTypeId').html(cbmOptions);
                            stockTypeDropdownValue = cbmOptions;
                        }

                        function onFailedStockType(error) {
                            window.alert(error.statusText);
                        }

                        objStockDetails.ProductCode = $(val).find(".dTableProductCode").val();
                        //BatchDetailsObj.Id = $(val).find(".dTableBatchName").val();
                        BatchDetailsObj.SellingRate = $(val).find(".dTableSellingRate").val();
                        BatchDetailsObj.PurchaseRate = $(val).find(".dTablePurchaseRate").val();
                        BatchDetailsObj.DateTo = $(val).find(".dTableDateTo").val() || null;
                        BatchDetailsObj.ProductCode = $(val).find(".dTableProductCode").val();
                        //Adel 17/8/2024
                        var jsonParam = { BatchObj: JSON.stringify(BatchDetailsObj) };
                        var serviceURL = "/PosStock/DetectBatchId/";
                        JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);
                        function onSuccess(jsonData) {
                            objStockDetails.PosProductBatchId = jsonData;
                        }
                        function onFailed(error) {
                            window.alert(error.statusText);
                            yourGlobalVariable = true;
                            return;
                        }
                        //Adel
                        if (yourGlobalVariable == false) {
                            objStockDetails.Qty = -qty;
                            objStockDetails.PosStockTypeId = $(val).find(".dTablePosStockTypeId").val();
                            objStockDetails.Discount = -Math.abs(parseFloat($(val).find(".dTableDiscount").val() || 0));
                            objStockDetails.PurchaseTax = -Math.abs(parseFloat($(val).find(".dTablePurchaseTax").val() || 0));
                            objStockDetails.PurchaseRate = $(val).find(".dTablePurchaseRate").val();
                            posStockDetail.push(objStockDetails);
                        }
                    }
                }
            });
            if (!isValid) {
                const missingProducts = missingExpireDates.join(", ");
                Message.Warning(`Expire Date is required for the following products: ${missingProducts}. Please enter valid dates.`);
                return;
            }
            if (yourGlobalVariable == false) {
                objStock.PosStockDetail = posStockDetail;
                if (Message.Prompt()) {
                    var jsonParam = { vmStock: JSON.stringify(objStock) }
                    var serviceURL = "/PosStock/InsertPurchaseReceipt/";
                    JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
                }
                function onSuccess(jsonData) {
                    if (jsonData == "0") {
                        Message.Error("save");
                    } else {
                        Message.Success("save");
                        Manager.resetForm();
                        //$('.dTableQty').prop('readonly', true);
                        // $('.dTableProductCode ').prop('readonly', true);
                    }
                }

                function onFailed(xhr, status, err) {
                    Message.Exception(xhr);
                }
            }
            else {
                Message.Error("save");
            }
        }
    },
    formatDate: function (input) {
        if (!input || typeof input !== 'string') {
            return null;
        }

        if (input.length === 8) {

            const day = input.substring(0, 2);
            const month = input.substring(2, 4);
            const year = input.substring(4, 8);
            return `${day}/${month}/${year}`;
        } else if (input.length === 6) {

            const month = input.substring(0, 2);
            const year = input.substring(2, 6)
            return `01/${month}/${year}`;
        }

        return null;
    },



    savePurchaseReceipt: function (invoiceType) {
        debugger;
        var obj = [];
        globalThis.yourGlobalVariable = false;
        obj.push({ 'Invoice No': $("#InvReferenceNo").val() });
        obj.push({ 'Company Invoice No': $("#CompanyInvNo").val() });
        obj.push({ 'Invoice Date': $("#InvDate").val() });
        obj.push({ 'Inv. Receiv eDate': $("#InvReceiveDate").val() });
        // obj.push({ 'Net Payable': $("#NetPayable").val() });
        if (parseFloat($("#Inv_TotalAmount").val()) < 1) {
            Message.Warning("Select minimum one product. or Amount Must be Postive");
            return;
        }
        if (JsManager.validate(obj)) {
            var posStockDetail = [];
            var objStock = new Object();
            objStock.InvReferenceNo = $("#InvReferenceNo").val();
            objStock.CompanyInvNo = $("#CompanyInvNo").val();
            objStock.PosSupplierId = $("#PosSupplierId").val();
            objStock.PosBranchId = $("#PosBranchId").val();
            objStock.InvDate = $("#InvDate").val();
            objStock.InvReceiveDate = $("#InvReceiveDate").val();
            objStock.NetPayable = $("#NetPayable").val();
            objStock.PosInvoiceType = invoiceType;
            if ($("#NetPayable").val() == '') {
                objStock.NetPayable = 0;
            }
            objStock.OtherDiscount = $("#Inv_OtherDiscount").val();
            objStock.OtherCharges = $("#Inv_OtherCharges").val();
            objStock.Remarks = $("#Inv_Remarks").val();
            let isValid = true;
            let missingExpireDates = [];
            let isValidNegativeValue = true;

            $.each($('#dTablePurchaseReceipt tbody tr'), function (rowIdx, val) {
                debugger;
                var objStockDetails = new Object();
                var BatchDetailsObj = new Object();
                var ErrorMessage = '';
                if (parseFloat($(val).find(".dTableQty ").val()) <= 0) {

                    ErrorMessage = ErrorMessage + "qty"
                }
                if (parseFloat($(val).find(".dTableSellingRate ").val()) <= 0) {
                    ErrorMessage = ErrorMessage + " and Selling Rate";
                }
                if (parseFloat($(val).find(".dTablePurchaseRate ").val()) <= 0) {
                    ErrorMessage = ErrorMessage + " and Purchase Rate";
                }
                if (parseFloat($(val).find(".dTableDiscount ").val()) < 0) {
                    ErrorMessage = ErrorMessage + " and Discount";

                }
                if (ErrorMessage != '') {
                    Message.Error(ErrorMessage + " must be postive value for product" + $(val).find(".dTableProductCode").val());
                    isValidNegativeValue = false;
                    return false;
                }
                var qty = parseFloat($(val).find(".dTableQty ").val()) * parseFloat($(val).find(".dTablePosUomMasterId").find(':selected').data('conversion'));

                const dateToInput = $(val).find(".dTableDateTo").val();
                const isExpireChecked = $(val).find(".dtHasExpireCheckbox").prop("checked");
                if (isExpireChecked && !dateToInput) {
                    const productName = $(val).find(".dTableProductCode").val();
                    missingExpireDates.push(productName || `Row ${rowIdx + 1}`);
                    isValid = false;
                    return false;
                }

                if (!isNaN(qty) && qty > 0) {

                    objStockDetails.ProductCode = $(val).find(".dTableProductCode").val();
                    BatchDetailsObj.Id = $(val).find(".dTableBatchName").val();
                    BatchDetailsObj.SellingRate = $(val).find(".dTableSellingRate").val();
                    BatchDetailsObj.PurchaseRate = $(val).find(".dTablePurchaseRate").val();
                    BatchDetailsObj.DateTo = $(val).find(".dTableDateTo").val();
                    BatchDetailsObj.ProductCode = $(val).find(".dTableProductCode").val();
                    //Adel 17/8/2024
                    var jsonParam = { BatchObj: JSON.stringify(BatchDetailsObj) };
                    var serviceURL = "/PosStock/DetectBatchId/";
                    JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);
                    function onSuccess(jsonData) {
                        objStockDetails.PosProductBatchId = jsonData;
                    }
                    function onFailed(error) {
                        window.alert(error.statusText);
                        yourGlobalVariable = true;
                        return;
                    }
                    //Adel
                    if (yourGlobalVariable == false) {
                        objStockDetails.Qty = qty;
                        objStockDetails.PosStockTypeId = $(val).find(".dTablePosStockTypeId").val();
                        objStockDetails.Discount = $(val).find(".dTableDiscount").val();
                        objStockDetails.PurchaseTax = $(val).find(".dTablePurchaseTax").val();
                        objStockDetails.PurchaseRate = $(val).find(".dTablePurchaseRate").val();
                        posStockDetail.push(objStockDetails);
                    }
                }
            });
            if (!isValid) {
                const missingProducts = missingExpireDates.join(", ");
                Message.Warning(`Expire Date is required for the following products: ${missingProducts}. Please enter valid dates.`);
                return;
            }

            if (!isValidNegativeValue) {
                return;
            }

            if (yourGlobalVariable == false) {
                objStock.PosStockDetail = posStockDetail;
                if (Message.Prompt()) {
                    var jsonParam = { vmStock: JSON.stringify(objStock) }
                    var serviceURL = "/PosStock/InsertPurchaseReceipt/";
                    JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
                }
                function onSuccess(jsonData) {
                    if (jsonData == "0") {
                        Message.Error("save");
                    } else {
                        Message.Success("save");
                        Manager.resetForm();
                        Manager.LoadHoldPurchaseInvoice();
                        //$('.dTableQty').prop('readonly', true);
                        // $('.dTableProductCode ').prop('readonly', true);
                    }
                }

                function onFailed(xhr, status, err) {
                    Message.Exception(xhr);
                }
            }
            else {
                Message.Error("save");
            }
        }
    },






    netAmtCalculation: function () {
        var productAmount = 0;
        var productDiscount = 0;
        var productPurchaseTax = 0;
        var productOtherDiscount = 0;
        var productOtherCharges = 0;
        $.each($('#dTablePurchaseReceipt tbody tr'), function (rowIdx, val) {
            var tmpAmt = parseFloat($(val).find(".dTablePurchaseAmount").val());
            if (!isNaN(tmpAmt)) {
                productAmount += tmpAmt;
            } else {
                productAmount += 0;
            }
            var tmpPurc = parseFloat($(val).find(".dTablePurchaseTax").val());
            if (!isNaN(tmpPurc)) {
                productPurchaseTax += tmpPurc;
            } else {
                productPurchaseTax += 0;
            }
            var tmpDisc = parseFloat($(val).find(".dTableDiscount").val());
            if (!isNaN(tmpDisc)) {
                productDiscount += tmpDisc;
            } else {
                productDiscount += 0;
            }
        });
        $("#Inv_LessDiscount").val(productDiscount);
        $("#Inv_PurchaseTax").val(productPurchaseTax);
        $("#Inv_TotalAmount").val(productAmount);
        var othrDisc = parseFloat($("#Inv_OtherDiscount").val());
        if (!isNaN(othrDisc)) {
            productOtherDiscount += othrDisc;
        } else {
            productOtherDiscount += 0;
        }
        var otherCharge = parseFloat($("#Inv_OtherCharges").val());
        if (!isNaN(otherCharge)) {
            productOtherCharges += otherCharge;
        } else {
            productOtherCharges += 0;
        }
        $("#Inv_NetValue").val((productAmount + productPurchaseTax + productOtherCharges) - productOtherDiscount);

    },
    calculateProductAmount: function (inputs) {
        $("#btnSave").prop("disabled", true);
        //$(inputs[8] ).val($(inputs[2]).find(':selected').data('purchaserate')); // Commented by Adel
        var pSellingRate = parseFloat($(inputs[8]).val());
        var pDiscAmt = parseFloat($(inputs[7]).val());
        var pDiscPer = parseFloat($(inputs[6]).val());
        $(inputs[6]).on('change', function () {
            if (pDiscPer != 0 && pSellingRate != 0) {
                var pRate = pSellingRate - ((pDiscPer * pSellingRate) / 100);
                $(inputs[9]).val(pRate);
            }

        });
        $(inputs[8]).on('change', function () {
            if (pDiscPer != 0 && pSellingRate != 0) {
                var pRate = pSellingRate - ((pDiscPer * pSellingRate) / 100);
                $(inputs[9]).val(pRate);
            }

        });
        var pRate = parseFloat($(inputs[9]).val());
        var pQty = parseFloat($(inputs[4]).val());
        var conversionFactor = parseFloat($(inputs[3]).find(':selected').data('conversion'));
        var amt = (conversionFactor * pQty) * pRate;
        var discAmt = 0;
        var calByDisPer = 0;
        $(inputs[7]).val(discAmt);
        if (!isNaN(pDiscPer)) {
            // discAmt = (amt * pDiscPer) / 100;
            discAmt = (pSellingRate * pDiscPer * pQty) / 100;
            $(inputs[7]).val(discAmt);
            calByDisPer = 1;
            // $(inputs[11]).val(amt - discAmt);
            $(inputs[12]).val(amt);
        }
        // $(inputs[11]).val(amt - discAmt);
        $(inputs[12]).val(amt);
        Manager.netAmtCalculation();
    },


    GetProductDetailsForPurchaseReceipt: function (pCode, inputs) {
        var jsonParam = { productCode: pCode };
        var serviceURL = "/PosStock/GetProductDetailsForPurchaseReceipt/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            Manager.loadProductBatch(jsonData.Code, inputs);
            Manager.loadUomMaster(jsonData.Code, inputs);
            $(inputs[5]).html(stockTypeDropdownValue);
            $(inputs[0]).val(jsonData.Code);
            $(inputs[1]).val(jsonData.Name);
            $(inputs[2]).val(jsonData.PosProductBatchId);
            $(inputs[3]).val(jsonData.PosUomMasterId);
            $(inputs[8]).val(jsonData.SellingRate);
            $(inputs[9]).val(jsonData.PurchaseRate);
            $(inputs[10]).val(jsonData.DateTo);
            if (jsonData.PosProductHasExpire) {
                $(inputs[11]).attr('checked', 'checked');
            }
        }

        function onFailed(error) {
            inputs[0].focus();
            inputs[0].select();
            $(inputs[0]).val('');
        }
    },
    loadProductBatch: function (pCode, inputs) {
        var jsonParam = { productCode: pCode };
        var serviceURL = "/PosStock/GetBatchByProductCode/";
        JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {

            var cbmOptions = "";
            $.each(jsonData, function () {
                cbmOptions += '<option data-purchaserate=' + this.PurchaseRate + ' value=\"' + this.Id + '\">' + this.Name + '</option>';
                cbmOptions += '<option data-SellingRate=' + this.SellingRate + ' value=\"' + this.Id + '\">' + this.Name + '</option>';

            });
            $(inputs[2]).html(cbmOptions);

        }

        function onFailed(error) {
            window.alert(error.statusText);
        }
    },
    loadUomMaster: function (pCode, inputs) {
        var jsonParam = { productCode: pCode };
        var serviceURL = "/PosDropDown/GetProductUom/";
        JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            var cbmOptions = "";
            $.each(jsonData, function () {
                cbmOptions += '<option data-uomgroupid="' + this.PosUomGroupId + '" data-conversion="' + this.ConversionFactor + '" value="' + this.Id + '">' + this.Name + '</option>';
            });
            var isBasedUom = jsonData.filter(w => w.IsBaseUom == true)[0];
            $(inputs[3]).html(cbmOptions);
            $(inputs[3]).val(isBasedUom.Id);
        }

        function onFailed(error) {
            window.alert(error.statusText);
        }
    },
    loadStockType: function () {
        var jsonParam = '';
        var serviceURL = "/PosDropDown/GetStockType/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            var cbmOptions = "";
            $.each(jsonData, function () {
                cbmOptions += '<option value=\"' + this.Id + '\">' + this.Name + '</option>';
            });
            $('.dTablePosStockTypeId').html(cbmOptions);
            stockTypeDropdownValue = cbmOptions;
        }

        function onFailed(error) {
            window.alert(error.statusText);
        }
    },
    loadBranch: function () {
        var jsonParam = '';
        var serviceURL = "/PosDropDown/GetBranch/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            var objProgram = jsonData;
            JsManager.PopulateCombo('#PosBranchId', objProgram);
        }

        function onFailed(error) {
            window.alert(error.statusText);
        }
    },



    loadSupplier: function () {
        var jsonParam = '';
        var serviceURL = "/PosDropDown/GetSupplier/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            var objProgram = jsonData;
            JsManager.PopulateCombo('#PosSupplierId', objProgram, 'Empty', '');
        }

        function onFailed(error) {
            window.alert(error.statusText);
        }
    },
    CancelInvoice: function (posInvoiceType) {
        debugger;
        if ($("#InvReferenceNo").val() == "") {
            Message.Warning("At first find the invoice, after then you press the cancel invoice button.");
        } else {
            if (Message.Prompt("Do you want to cancel this invoice?")) {
                var jsonParam = {
                    invoiceNo: $("#InvReferenceNo").val(),
                    posInvoiceType: posInvoiceType

                };
                var serviceURL = "/Pos/CancelPurchaseInvoice/";
                JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

                function onSuccess(jsonData) {
                    if (jsonData > 0) {
                        Message.Success("cancel");
                        Manager.resetForm();
                        $("#InvReferenceNo").val('');
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

    //GetInvoice: function (posInvoiceType, invNumber) {
    //    const jsonParam = {
    //        invoiceNo: invNumber,
    //        posInvoiceType: posInvoiceType
    //    };
    //    const serviceURL = "/PosStock/GetPurchaseReceipt/ ";

    //    JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

    //    function onSuccess(jsonData) {
    //        console.log(jsonData);
    //        try {
    //            if (jsonData && jsonData.SalesInviceProduct && jsonData.SalesInviceProduct.length > 0) {
    //                const tableBody = $("#invoiceTableBody");
    //                tableBody.empty();

    //                jsonData.SalesInviceProduct.forEach(product => {
    //                    const row = `
    //                        <tr>
    //                            <td>${product.ProductName || 'N/A'}</td>
    //                            <td>${product.Quantity || 0}</td>
    //                            <td>${product.Price.toFixed(2) || 0.00}</td>
    //                            <td>${product.VatPar || 0}</td>
    //                            <td>${product.SdPar || 0}</td>
    //                            <td>${(product.Price * product.Quantity).toFixed(2)}</td>
    //                        </tr>
    //                    `;
    //                    tableBody.append(row);
    //                });
    //            } else {
    //                Message.Warning("Invoice not found!");
    //            }
    //        } catch (err) {
    //            console.error("Parsing error:", err);
    //            Message.Exception("Invalid response format.");
    //        }
    //    }

    //    function onFailed(xhr) {
    //        console.error("Response Text:", xhr.responseText); // Log the server's response
    //        Message.Exception("An error occurred while processing your request.");
    //    }
    //}
    // ,
    LoadHoldPurchaseInvoice: function () {
        var jsonParam = { invoiceType: 3 };
        var serviceURL = "/PosStock/GetInvoiceByType/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            $("#holdInvDropdown").empty();
            JsManager.PopulateCombo('#holdInvDropdown', jsonData, 'Hold Inv.', 0);
        }

        function onFailed(error) {
            window.alert(error.statusText);
        }
    },
    FetchPurchaseReceipt: function (invNumber) {

        var jsonParam = { invReferenceNo: invNumber };
        var serviceURL = "/PosStock/GetPurchaseReceiptByinvNo/";
        JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            debugger;
            if (jsonData.length == 0) {
                Message.Warning("Invoice not found!");
                return;
            }
            let firstArray = jsonData[0];
            $("#CompanyInvNo").val(firstArray.CompanyInvNo);
            $("#InvReferenceNo").val(firstArray.InvReferenceNo);
            $("#InvDate").val(JsManager.ChangeDateFormat(firstArray.InvDate, 0));
            $("#InvReceiveDate").val(JsManager.ChangeDateFormat(firstArray.InvReceiveDate, 0));
            $("#Inv_TotalAmount").val(firstArray.Amount);

            //  Manager.loadSupplier();
            //Manager.loadBranch();
            Manager.loadStockType();

            $("#PosSupplierId").val(firstArray.PosSupplierId).trigger('change');

            //$("#PosSupplierId").trigger("chosen:updated");
            Manager.LoadDataTablePurchaseReceipt(jsonData);
            // Manager.addNewDTableRow();
            Manager.netAmtCalculation();
            // sara 22-1-2025
            // Load Batch/Size * Pack * for each row
            $('#dTablePurchaseReceipt tbody tr').each(function () {
                var pCode = $(this).find('.dTableProductCode').val();
                var inputs = $(this).find('.dtInputs');
                Manager.loadProductBatch(pCode, inputs);
                Manager.loadUomMaster(pCode, inputs);
            });

            globalThis.InvoiceThatFind = true;
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },



    addNewDTableRow: function () {
        dTablePurchaseReceipt.row.add({
            'ProductCode': '',
            'ProductName': '',
            'BatchName': '',
            'PosUomMasterId': '',
            'Qty': '',
            'PosStockTypeId': '',
            'DiscountPer': '0.00',
            'Discount': '0.00',
            'SellingRate': '0.00',
            'PurchaseRate': '0.00',
            'DateTo': '',
            'Amount': '0.00',
            'PurchaseTax': '0.00'
        }).draw();
    },




    GetPurchaseReceipt: function (ref) {
        var jsonParam = { companyInvNo: 0 };
        var serviceURL = "/PosStock/GetPurchaseReceipt/";
        JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            Manager.LoadDataTablePurchaseReceipt(jsonData, ref);
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

    LoadDataTablePurchaseReceipt: function (userdata, isRef) {

        if (isRef == "0") {
            const style = document.createElement('style');
            style.innerHTML = `
    .invoiceCheckbox {
        position: relative;
        width: 20px;
        height: 20px;
        border: 2px solid #F44336; 
        border-radius: 4px;
        cursor: pointer;
        transition: all 0.3s ease;
    }

    .invoiceCheckbox:hover {
        border-color: #D32F2F; 
    }

    .invoiceCheckbox:checked {
        background-color: #F44336;
        border-color: #F44336;
    }

    .invoiceCheckbox:checked::before {
        content: '🗑️';
        position: absolute;
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
        color: white;
        font-size: 14px;
    }

    .invoiceCheckbox:focus {
        outline: none;
        box-shadow: 0 0 5px rgba(244, 67, 54, 0.6); 
    }
    .readonly-checkbox {
    pointer-events: none; 
    opacity: 1; 
    background-color: #f5f5f5; 
    border-color: #007bff; 
    color: #007bff; 
}

.readonly-checkbox:checked {
    background-color: #007bff; 
    border-color: #0056b3; 
    color: #fff;
}

.readonly-checkbox:disabled {
    cursor: not-allowed; 
}

    `;
            document.head.appendChild(style);

            dTablePurchaseReceipt = $('#dTablePurchaseReceipt').DataTable({
                dom: 'B<"tableToolbar">rt',
                initComplete: function () {
                    dTableManager.Border("#dTablePurchaseReceipt", 300);
                },
                buttons: [
                ],
                scrollY: "300px",
                scrollX: true,
                scrollCollapse: true,
                lengthMenu: [[-1], ["All"]],
                columnDefs: [
                    { visible: false, targets: [] },
                ],
                columns: [
                    {
                        data: '',
                        name: 'SL',
                        orderable: false,
                        title: '',
                        width: 10,
                        render: function (data, type, row, meta) {
                            return '<div class="dataTableSerialNumberDiv">' + (meta.row + 1) + '</div>';
                        }
                    },
                    {
                        data: 'ProductCode',
                        name: 'ProductCode',
                        orderable: false,
                        title: 'Code *....',
                        align: 'left',
                        width: 90,
                        render: function (data, type, row, meta) {
                            return "<input type='text' id='row-" + meta.row + "-ProductCode' name='row-" + meta.row + "-ProductCode'  value='" + data + "' class='form-control input-sm dTableProductCode dtInputs' placeholder='Product Code'/>";
                        }
                    },
                    {
                        data: 'ProductName',
                        name: 'ProductName',
                        title: 'Product Name',
                        align: 'left',
                        width: 250,
                        orderable: false,
                        render: function (data, type, row, meta) {
                            return "<input type='text' id='row-" + meta.row + "-ProductName' name='row-" + meta.row + "-ProductName' value='" + data + "' class='form-control dtInputBackground input-sm dTableProductName dtInputs' placeholder='Product Name' readonly='true'/>";
                        }
                    },
                    {
                        data: 'BatchName',
                        name: 'BatchName',
                        title: 'Batch/Size *',
                        orderable: false,
                        width: 90,
                        render: function (data, type, row, meta) {
                            return "<select id='row-" + meta.row + "-BatchName' name='row-" + meta.row + "-BatchName'  value='" + data + "' class='form-control input-sm dTableBatchName dtInputs'></select>";
                        }
                    },
                    {
                        data: 'PosUomMasterId',
                        name: 'PosUomMasterId',
                        title: 'Pack *',
                        width: 80,
                        orderable: false,
                        render: function (data, type, row, meta) {
                            return "<select id='row-" + meta.row + "-PosUomMasterId' name='row-" + meta.row + "-PosUomMasterId' value='" + data + "' class='form-control input-sm dTablePosUomMasterId dtInputs'></select>";
                        }
                    },
                    {
                        data: 'Qty',
                        name: 'Qty',
                        title: 'Received Qty *',
                        width: 100,
                        orderable: false,
                        render: function (data, type, row, meta) {
                            return "<input type='number' id='row-" + meta.row + "-Qty' name='row-" + meta.row + "-Qty' value='" + data + "' class='form-control input-sm dtInputTextAlign dTableQty dtInputs' placeholder='Qty' min='0'/>";
                        }
                    },
                    {
                        data: 'PosStockTypeId',
                        name: 'PosStockTypeId',
                        title: 'Stock Type *',
                        width: 80,
                        orderable: false,
                        render: function (data, type, row, meta) {
                            return "<select id='row-" + meta.row + "-PosStockTypeId' name='row-" + meta.row + "-PosStockTypeId' value='" + data + "' class='form-control input-sm dTablePosStockTypeId dtInputs'></select>";
                        }
                    },
                    {
                        data: 'DiscountPer',
                        name: 'DiscountPer',
                        title: 'Discount %',
                        width: 70,
                        orderable: false,
                        render: function (data, type, row, meta) {
                            return "<input type='number' id='row-" + meta.row + "-DiscountPer' name='row-" + meta.row + "-DiscountPer'  value='" + data + "' class='form-control input-sm dtInputTextAlign dTableDiscountPer dtInputs' placeholder='Discount %'  min='0'/>";
                        }
                    },
                    {
                        data: 'Discount',
                        name: 'Discount',
                        title: 'Total Discount Amt.',
                        width: 100,
                        orderable: false,
                        render: function (data, type, row, meta) {
                            return "<input type='number' id='row-" + meta.row + "-Discount' name='row-" + meta.row + "-Discount'  value='" + data + "' class='form-control input-sm dtInputTextAlign dTableDiscount dtInputs' placeholder='Discount Amount'  min='0'/>";
                        }
                    },
                    {
                        data: 'SellingRate',
                        name: 'SellingRate',
                        title: 'Selling Rate',
                        width: 100,
                        orderable: false,
                        render: function (data, type, row, meta) {
                            return "<input type='number' id='row-" + meta.row + "-SellingRate' name='row-" + meta.row + "-SellingRate'  value='" + data + "' class='form-control input-sm dtInputTextAlign dtInputBackground dTableSellingRate dtInputs' placeholder='SellingRate'   min='0'/>";
                        }
                    },
                    {
                        data: 'PurchaseRate',
                        name: 'PurchaseRate',
                        title: 'Purchase Rate',
                        width: 100,
                        orderable: false,
                        render: function (data, type, row, meta) {
                            return "<input type='number' id='row-" + meta.row + "-PurchaseRate' name='row-" + meta.row + "-PurchaseRate'  value='" + data + "' class='form-control input-sm dtInputTextAlign dtInputBackground dTablePurchaseRate dtInputs' placeholder='PurchaseRate'   min='0'/>";
                        }
                    },

                    {
                        data: 'DateTo',
                        name: 'DateTo',
                        title: 'Expire Date',
                        width: 150,
                        orderable: false,
                        render: function (data, type, row, meta) {

                            let FormattedDate = "";

                            if (data && data.includes("Date")) {
                                FormattedDate = JsManager.ChangeDateFormat(data, 0);
                            } else {
                                FormattedDate = data || '';
                            }


                            return `
            <input type="text"
                   id="row-${meta.row}-DateTo"
                   name="row-${meta.row}-DateTo"
                   value="${FormattedDate}"
                   class="form-control input-sm dtInputTextAlign dtInputBackground dTableDateTo dtInputs"
                   placeholder="Enter date as ddMMyyyy"
                   min="0" />
        `;
                        }
                    },

                    {
                        data: 'PosProductHasExpire',
                        name: 'PosProductHasExpire',
                        title: 'Has Expire',
                        width: 50,
                        orderable: false,
                        render: function (data, type, row, meta) {
                            const isChecked = data ? 'checked' : '';
                            return `
                            <input type="checkbox" 
                                   id="row-${meta.row}-PosProductHasExpire" 
                                   name="row-${meta.row}-PosProductHasExpire" 
                                   
                                     class="form-control input-sm dtHasExpireCheckbox dtInputs"
                                   ${isChecked}  disabled />
                        `;
                        }
                    },

                    {
                        data: 'Amount',
                        name: 'Amount',
                        title: 'Amount',
                        width: 100,
                        orderable: false,
                        render: function (data, type, row, meta) {
                            return "<input type='number' id='row-" + meta.row + "-Amount' name='row-" + meta.row + "-Amount'  value='" + data + "' class='form-control input-sm dtInputTextAlign dtInputBackground dTablePurchaseAmount dtInputs' placeholder='Amount' readonly='true' min='0'/>";
                        }
                    },
                    {
                        data: 'PurchaseTax',
                        name: 'PurchaseTax',
                        title: 'Purchase Tax',
                        width: 100,
                        orderable: false,
                        render: function (data, type, row, meta) {
                            return "<input type='number' id='row-" + meta.row + "-PurchaseTax' name='row-" + meta.row + "-PurchaseTax' value='" + data + "' class='form-control input-sm dtInputTextAlign dTablePurchaseTax dtInputs' placeholder='Purchase Tax'  min='0'/>";
                        }
                    },
                    {
                        name: 'Option',
                        title: 'Option',
                        width: 20,
                        orderable: false,
                        render: function (data, type, row) {
                            return "<div style='min-width:90px;'><input type='button' value='Ad.' class='dtBtnAddNewRow btn-blue' style='float:left;' /><input type='button' value='Rv.' class='dtBtnRemoveRow btn-danger' style='float:left;'/></div>";
                        }
                    },
                    {

                        name: 'Delete',
                        title: 'Delete',
                        width: 50,
                        orderable: false,
                        render: function (data, type, row, meta) {
                            // console.log(row);
                            // console.log(meta.row);
                            return `    <input
                       
                                    type="checkbox"
                                    name="domids[]"
                                    class="  invoiceCheckbox "
                                    value="${meta.row}"
                                        data-amount="${row.Amount}" 
                                />`
                                ;
                        },


                    }
                ],
                fixedColumns: true,
                data: userdata,
                rowsGroup: null
            });

        } else {
            dTablePurchaseReceipt.clear().rows.add(userdata).draw();

        }

    },
    dTableEventManager: function (key, inputs, idx, $item) {
        //for F4 product hot search
        if (key.keyCode == 115 && idx == 0) {
        } else if (key.keyCode == 13) {
            if (idx == 0) {
                if ($(inputs[0]).val() != "" && $(inputs[0]).val() != "0") {
                    x = $(inputs[1]).val();
                    Manager.GetProductDetailsForPurchaseReceipt($(inputs[0]).val(), inputs);
                    inputs[idx + 4].focus();
                    inputs[idx + 4].select();
                } else {
                    inputs[0].focus();
                    inputs[0].select();
                }
            } else if (idx == 6) {
                if ($(inputs[4]).val() != "" && $(inputs[0]).val() != "" && $(inputs[4]).val() != "0" && $(inputs[2]).val() != null && $(inputs[3]).val() != null && $(inputs[5]).val() != null) {
                    Manager.addNewDTableRow();
                    var nextInput = $item.parents('tr').next().find('.dtInputs');
                    nextInput[0].focus();
                } else {
                    inputs[0].focus();
                    inputs[0].select();
                }
            } else if (idx == 10) {
                if ($(inputs[4]).val() != "" && $(inputs[0]).val() != "" && $(inputs[4]).val() != "0" && $(inputs[2]).val() != null && $(inputs[3]).val() != null && $(inputs[5]).val() != null) {
                    Manager.addNewDTableRow();
                    var nextInput2 = $item.parents('tr').next().find('.dtInputs');
                    nextInput2[0].focus();
                } else {
                    inputs[0].focus();
                    inputs[0].select();
                }
            } else {
                inputs[idx + 1].focus();
                if (!$(inputs[idx + 1]).is('select')) {
                    inputs[idx + 1].select();
                }

            }
        } else if (key.keyCode == 37 && idx != 0) {
            inputs[idx - 1].focus();
        } else if (key.keyCode == 38) {
            key.preventDefault();
            var uPArrowInputs = $item.parents('tr').prev().find('.dtInputs');
            if (uPArrowInputs[idx] != undefined)
                uPArrowInputs[idx].focus();
        } else if (key.keyCode == 39) {
            inputs[idx + 1].focus();
        } else if (key.keyCode == 40) {
            key.preventDefault();
            var downArrowInputs = $item.parents('tr').next().find('.dtInputs');
            if (downArrowInputs[idx] != undefined)
                downArrowInputs[idx].focus();
        }
    }


}



var productInfos = null;
var prdFindBySystem = 0;
var isSearchOn = 0;
var productCodeId = "";

//$(function () {
//fetch("/PosStock/GetProductInfoForPurchasReceipt").then(response => response.json().then(jsonData => {
//    productInfos = jsonData;
//}));

//});

$(document).on("keydown", ".dTableProductCode, #txtCodeOrBarcode", function (e) {
    if (e.key == "F2") {
        var elInput = this;
        var elWrap = document.querySelector("#txtItemOrBarcodeAC");
        if (e.key == "F4") {
            if (isSearchOn == 0) {
                isSearchOn = 1;
                $("#chkSearchEnable").prop("checked", true);
            } else {
                isSearchOn = 0;
                $("#chkSearchEnable").prop("checked", false);
            }
            return;
        }
        if ($("#chkSearchEnable").prop("checked")) { isSearchOn = 1; } else { isSearchOn = 0; }

        var ignoreKey = [13, 37, 38, 39, 40];
        if (!ignoreKey.find(a => a == e.keyCode)) {
            if (isSearchOn == 1) {
                $("#txtCodeOrBarcode").focus();
                $("#txtCodeOrBarcode").val(elInput.value);
            }

            elWrap.innerHTML = '<table class="table table-sm table-bordered table-search" style="min-width:700px;background: #f7f7f7;margin-bottom:7px;">' +
                '<thead class="theadPrdSrcTbl">' +
                '<tr style="margin:0;padding:0;">' +
                '<th>Product</th>' +
                '<th>Product Code</th>' +
                '<th>Product Bar Code</th>' +
                '<th>Batch/Size</th>' +
                '<th>PurchaseRate</th>' +
                '<th>SellingRate</th>' +
                '</tr>' +
                '</thead>' +
                '<tbody id="txtItemOrBarcodeACTR">' +
                '</tbody>' +
                '</table>';

            if (isSearchOn && elInput.value.length > 0) {
                var matchs = false;
                ////////////
                debugger;
                if (elInput.value.search("%") == -1) {
                    var serviceUrl = '/PosStock/GetProductInfoForPurchasReceiptByItem/';
                }
                else {
                    var serviceUrl = '/PosStock/GetProductInfoForPurchasReceipt/';
                }
                var jsonParam = { Parcode: elInput.value, Name: elInput.value };
                JsManager.SendJsonAsyncON(serviceUrl, jsonParam, onSuccess, onFailed);

                function onSuccess(jsonData) {
                    // debugger;
                    // JsManager.PopulateCombo('#txtCodeOrBarcode', response);
                    var productInfos = new Object();
                    productInfos = jsonData;
                    var str = "";
                    var result = str.concat(".*", elInput.value.toUpperCase(), ".*");
                    var serXp = result.replace(/%/g, '.*');
                    const serXI = new RegExp(serXp);
                    productInfos.forEach(function (pItem, ind) {
                        //debugger;
                        //var serXp = new RegExp('^' + elInput.value, "i");
                        //var serXI = new RegExp(elInput.value, "i");
                        //if (
                        //    pItem.Code.search(serXp) != -1 ||
                        //    pItem.BarCode.search(serXI) != -1 ||
                        //    pItem.Name.search(serXp) != -1 ||
                        //    pItem.BatchName.search(serXp) != -1
                        //) {

                        if (
                            pItem.Code.match(serXI) ||
                            pItem.BarCode.match(serXI) ||
                            pItem.Name.toUpperCase().match(serXI) ||
                            pItem.BatchName.toUpperCase().match(serXI)
                        ) {
                            matchs = true;
                            var elFound = '<tr><td class="txtItemOrBarcodeACITD"><button class="txtItemOrBarcodeACI" value="' + pItem.Code + '">' + pItem.Name + '</button></td>' +
                                '<td>' + pItem.Code + '</td>' +
                                '<td>' + pItem.BarCode + '</td>' +
                                '<td>' + pItem.BatchName + '</td>' +
                                '<td style="text-align:right;">' + pItem.PurchaseRate + '/</td>' +
                                '<td style="text-align:right;">' + pItem.SellingRate + '/-</td></tr>';
                            $(elWrap).find("#txtItemOrBarcodeACTR").append(elFound);

                        }
                    });
                }
                function onFailed() {
                }
                ///////////


                if (matchs) {
                    $(elWrap).css("display", "contents");
                    $("#divSearchOn").show();
                    $("#txtCodeOrBarcode").focus();
                }
            }
        }
    }
});

$(document).on('keydown', "#divSearchOn", function (e) {
    if (e.target && (e.target.getAttribute("class") == 'form-control input-sm txtItemOrBarcodeACI' || e.target.getAttribute("class") == 'txtItemOrBarcodeACI active')) {
        e.stopPropagation();
        if (e.key == "ArrowUp") {
            if (e.target.parentNode.parentNode.previousSibling && e.target.parentNode.parentNode.previousSibling.querySelector("button")) {
                var preVN = e.target.parentNode.parentNode.previousSibling.querySelector("button");
                e.target.parentNode.parentNode.setAttribute("class", "");
                preVN.setAttribute("class", "txtItemOrBarcodeACI active");
                preVN.parentNode.parentNode.setAttribute("class", "txtItemOrBarcodeACTR active");
                preVN.focus();
                e.target.setAttribute("class", "txtItemOrBarcodeACI");
            }
            else {
                e.target.parentNode.parentNode.setAttribute("class", "");
                e.target.setAttribute("class", "txtItemOrBarcodeACI");
                $("#txtCodeOrBarcode").focus();
            }
        } else if (e.key == "ArrowDown") {
            if (e.target.parentNode.parentNode.nextSibling && e.target.parentNode.parentNode.nextElementSibling.querySelector("button")) {
                var nextVN = e.target.parentNode.parentNode.nextElementSibling.querySelector("button");

                var prvTr = nextVN.parentNode.parentNode.previousSibling;
                if (prvTr != null) {
                    prvTr.setAttribute("class", "txtItemOrBarcodeACI");
                    prvTr.querySelector('button').setAttribute("class", "txtItemOrBarcodeACI");
                }
                nextVN.setAttribute("class", "txtItemOrBarcodeACI active");
                nextVN.parentNode.parentNode.setAttribute("class", "txtItemOrBarcodeACTR active");
                nextVN.focus();
            }
        }
        else if (e.key == "Enter") {
            $("#txtCodeOrBarcode").val('');
            $("#txtItemOrBarcodeAC").html("");
            $("#divSearchOn").hide();
            $("#" + productCodeId).val(e.target.value);
            $("#" + productCodeId).focus();

        }
    }
    if (e.key == "Escape") {
        $("#txtCodeOrBarcode").val('');
        $("#txtCodeOrBarcode").focus();
        $("#txtItemOrBarcodeAC").html("");
        $("#divSearchOn").hide();
        isSearchOn = 0;
    }
    else if (e.key == "F2") {
        $("#txtCodeOrBarcode").val('');
        $("#txtCodeOrBarcode").focus();
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
        $("#txtItemOrBarcodeAC").html("");
        $("#divSearchOn").hide();
        $("#" + productCodeId).val(e.target.value);
        $("#" + productCodeId).focus();
    }
});

$(document).on("focus", ".dTableProductCode", function () {
    productCodeId = this.id;
});

$(document).on("click", "#divCloseSearch", function () {

    $("#txtCodeOrBarcode").val('');
    $("#txtCodeOrBarcode").focus();
    $("#txtItemOrBarcodeAC").html("");
    $("#divSearchOn").hide();
});
