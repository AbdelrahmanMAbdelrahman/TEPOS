var dTableStockAdjustment = null;
var stockTypeDropdownValue = null;

$(document).ready(function () {
    Manager.LoadDataTableStockAdjustment();
    Manager.loadSupplier();
    Manager.loadBranch();
    Manager.LoadStockType();
    Manager.addNewDTableRow();


    $("#btnSave").prop("disabled", true);

    dTableStockAdjustment.on('order.dt search.dt', function () {
        dTableStockAdjustment.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = dTableManager.IndexColumn(i + 1);
        });
    }).draw();

   
    

    $("#btnRefresh").click(function() {
        $("#btnSave").prop("disabled", false);
        Manager.netAmtCalculation();
    });

    $("#btnCancel").click(function () {
        $("#btnSave").prop("disabled", true);
        Manager.resetForm();

    });

    $("#btnSave").click(function() {
        Manager.SaveStockAdjustment();
    });

});




$(document).on("click", ".dtBtnAddNewRow", function () {
    var inputs = $(this).parents('tr').find('.dtInputs');
    if ($(inputs[4]).val() && $(inputs[0]).val() && $(inputs[4]).val()>0 && $(inputs[2]).val() != null && $(inputs[3]).val() && $(inputs[3]).val()>0) {
        Manager.addNewDTableRow();
        $item = $(this);
        var nextInput = $item.parents('tr').next().find('.dtInputs');
        nextInput[0].focus();
    } else {
        inputs[0].focus();
        inputs[0].select();
    }
});

$(document).on('click', '.dtBtnRemoveRow', function () {
    dTableStockAdjustment.row($(this).parents('tr')).remove().draw();
});


$(document).on('change', '.dtInputs', function (key) {
    var inputs = $(this).parents('tr').find('.dtInputs');
    Manager.calculateProductAmount(inputs);
});

$(document).on('change', '.dTableBatchName', function (key) {
    var inputs = $(this).parents('tr').find('.dtInputs');
    Manager.GetStockByLocationWise($(this).val(), inputs);

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




var Manager = {
    LoadStockType: function() {
        var jsonParam = '';
        var serviceUrl = "/PosDropDown/GetStockType/";
        JsManager.SendJson(serviceUrl, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            JsManager.PopulateCombo('#Location', jsonData);

        }

        function onFailed(error) {
            window.alert(error.statusText);
        }
    },

    SaveStockAdjustment: function() {
        var obj = [];
        obj.push({ 'Company Invoice No': $("#CompanyInvNo").val() });
        obj.push({ 'Invoice Date': $("#InvDate").val() });
        //if (parseFloat($("#Inv_TotalAmount").val()) < 1) {
        //    Message.Warning("Select minimum one product to transfer stock location.");
        //    return;
        //}
        if (JsManager.validate(obj)) {

            var stockAdjustmentDetails = [];
            var objStock = new Object();
            objStock.CompanyInvNo = $("#CompanyInvNo").val();
            objStock.PosSupplierId = $("#PosSupplierId").val();
            objStock.PosBranchId = $("#PosBranchId").val();
            objStock.Location = $("#Location").val();
            objStock.TranactionType = $("#tranactionType").val();
            objStock.InvDate = $("#InvDate").val();
            objStock.Remarks = $("#Inv_Remarks").val();
            var reqSts = 1;
            $.each($('#dTableStockAdjustment tbody tr'), function (rowIdx, val) {
                var objStockDetails = new Object();
                //2025 able user to make stock zero
                //if ($(val).find(".dTableQty").val() && parseFloat($(val).find(".dTableQty").val()) < 1) {
                 if ($(val).find(".dTableQty").val() && parseFloat($(val).find(".dTableQty").val()) < 0) {
                     //Message.Warning($(val).find(".dTableProductName").val() + " Qty is can't be empty or zero.");
                     Message.Warning($(val).find(".dTableProductName").val() + " Qty is can't negative.");
                    $(val).find(".dTableQty").focus();
                    $(val).find(".dTableQty").select();
                    reqSts = 0;
                    return;
                } else if (objStock.TranactionType === "5") {
                    var currentQty = parseFloat($(val).find(".dTableStockOnHand").val() || 0);
                    var adjustQty = parseFloat($(val).find(".dTableQty").val() || 0);
                    if (currentQty - adjustQty < 0) {
                        Message.Warning($(val).find(".dTableProductName").val() + " quantity must be less than current stock: " + currentQty);
                        $(val).find(".dTableQty").focus();
                        $(val).find(".dTableQty").select();
                        reqSts = 0;
                        return;
                    }
                //} else if (objStock.TranactionType === "4") {
                //    var currentQty = parseFloat($(val).find(".dTableStockOnHand").val() || 0);
                //    var adjustQty = parseFloat($(val).find(".dTableQty").val() || 0);
                //    if (adjustQty <= currentQty) {
                //        Message.Warning($(val).find(".dTableProductName").val() + " quantity must be greater than current stock: " + currentQty);
                //        $(val).find(".dTableQty").focus();
                //        $(val).find(".dTableQty").select();
                //        reqSts = 0;
                //        return;
                //    }
                }
                //2025
                if ($(val).find(".dTableQty").val() /*&& $(val).find(".dTableQty").val() > 0*/ && $(val).find(".dTableBatchName").val() && $(val).find(".dTableBatchName").val() != null) {
                    objStockDetails.ProductCode = $(val).find(".dTableProductCode").val();
                    objStockDetails.PosProductBatchId = $(val).find(".dTableBatchName").val();
                    objStockDetails.Qty = $(val).find(".dTableQty").val();
                    objStockDetails.StockOnHand = parseFloat($(val).find(".dTableStockOnHand").val() || 0);
                    stockAdjustmentDetails.push(objStockDetails);
                }
            });
            objStock.StockAdjustmentDetails = stockAdjustmentDetails;
            if (reqSts == 1) {
                if (Message.Prompt()) {
                    var jsonParam = { vmStockAdjustment: JSON.stringify(objStock) }
                    var serviceURL = "/PosStock/InsertStockAdjustment/";
                    JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
                }
            }


            function onSuccess(jsonData) {
                if (jsonData == "0") {
                    Message.Error("save");
                } else {
                    if (jsonData == "-1010") {
                        Message.Warning("Company Invoice Referance No can't be empty");
                    } else {
                        Message.Success("save");
                        Manager.resetForm();
                    }
                }
            }

            function onFailed(xhr, status, err) {
                Message.Exception(xhr);
            }
        }
    },

    GetStockByLocationWise: function(batchId, inputs) {
        var jsonParam = {
            batchId: batchId,
            officeId: $("#PosBranchId").val(),
            fromLocation: $("#Location").val()
        };
        var serviceURL = "/PosStock/GetStockByLocationWise/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            $(inputs[3]).val(jsonData);
        }

        function onFailed(error) {
        }
    },
    
    resetForm: function() {
        dTableStockAdjustment.clear().row.add({
            'ProductCode': '',
            'ProductName': '',
            'BatchName': '',
            'StockOnHand': '',
            'Qty': '',
            'PurchaseRate': '0.00',
            'SellingRate': '0.00',
            'Amount': '0.00'
        }).draw();
        $("#CompanyInvNo").val('');
        $("#InvReferenceNo").val('');
        $("#NetPayable").val('');
        $("#Inv_TotalAmount").val(0);
    },


    netAmtCalculation: function() {
        var productAmount = 0;
        $.each($('#dTableStockAdjustment tbody tr'), function(rowIdx, val) {
            var tmpAmt = parseFloat($(val).find(".dTablePurchaseAmount").val());
            if (!isNaN(tmpAmt)) {
                productAmount += tmpAmt;
            } else {
                productAmount += 0;
            }

        });
        $("#Inv_TotalAmount").val(productAmount);

    },
    calculateProductAmount: function(inputs) {
        $("#btnSave").prop("disabled", true);
        $(inputs[5]).val($(inputs[2]).find(':selected').data('purchaserate'));
        var pRate = parseFloat($(inputs[5]).val());
        var pQty = parseFloat($(inputs[4]).val());
        var amt = pQty * pRate;

        $(inputs[7]).val(amt.toFixed(2));
        Manager.netAmtCalculation();
    },


    GetProductDetailsForStockAdjustment: function(pCode, inputs) {

        var jsonParam = {
            productCode: pCode,
            officeId: $("#PosBranchId").val(),
            fromLocation: $("#Location").val()
        };
        var serviceURL = "/PosStock/GetProductDetailsForLocationTransfer/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            var foundSts = 0;
            var $tmpval;
            $.each($('#dTableStockAdjustment tbody tr'), function(rowIdx, val) {

                if ($(val).find(".dTableBatchName").val() == jsonData.PosProductBatchId) {
                    foundSts = 1;
                    $tmpval = val;
                }
                if ($(val).find(".dTableBatchName").val() == null && foundSts == 1) {
                    dTableStockAdjustment.row($(inputs[rowIdx]).parents('tr')).remove().draw();
                    $($tmpval).find(".dTableQty").focus();
                    $($tmpval).find(".dTableQty").select();
                    return;
                }


            });

            if (foundSts == 0) {
                Manager.loadProductBatch(jsonData.Code, inputs);
                $(inputs[0]).val(jsonData.Code);
                $(inputs[1]).val(jsonData.Name);
                $(inputs[2]).val(jsonData.PosProductBatchId);
                $(inputs[3]).val(jsonData.StockOnHand);
                $(inputs[5]).val(jsonData.PurchaseRate);
                $(inputs[6]).val(jsonData.SellingRate);
            }
        }

        function onFailed(error) {
            inputs[0].focus();
            inputs[0].select();
            $(inputs[0]).val('');
        }
    },
    loadProductBatch: function(pCode, inputs) {
        var jsonParam = { productCode: pCode };
        var serviceURL = "/PosStock/GetBatchByProductCode/";
        JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            var cbmOptions = "";
            $.each(jsonData, function() {
                cbmOptions += '<option data-purchaserate=' + this.PurchaseRate + ' value=\"' + this.Id + '\">' + this.Name + '</option>';
            });
            $(inputs[2]).html(cbmOptions);

        }

        function onFailed(error) {
            window.alert(error.statusText);
        }
    },

    loadBranch: function() {
        var jsonParam = '';
        var serviceURL = "/PosDropDown/GetBranch/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            var objProgram = jsonData;
            JsManager.PopulateCombo('#PosBranchId', objProgram);
            JsManager.PopulateCombo('#ToPosBranchId', objProgram);
        }

        function onFailed(error) {
            window.alert(error.statusText);
        }
    },


    loadSupplier: function() {
        var jsonParam = '';
        var serviceURL = "/PosDropDown/GetSupplier/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            var objProgram = jsonData;
            JsManager.PopulateCombo('#PosSupplierId', objProgram);
        }

        function onFailed(error) {
            window.alert(error.statusText);
        }
    },

    addNewDTableRow: function() {
        dTableStockAdjustment.row.add({
            'ProductCode': '',
            'ProductName': '',
            'BatchName': '',
            'StockOnHand': '',
            'Qty': '',
            'PurchaseRate': '0.00',
            'SellingRate': '0.00',
            'Amount': '0.00'
        }).draw();
    },


    LoadDataTableStockAdjustment: function() {
        var obj = {
            'ProductCode': '',
            'ProductName': '',
            'BatchName': '',
            'StockOnHand': '',
            'Qty': '',
            'PurchaseRate': '0.00',
            'SellingRate': '0.00',
            'Amount': '0.00'
        };
        dTableStockAdjustment = $('#dTableStockAdjustment').DataTable({
            dom: 'B<"tableToolbar">rt',
            initComplete: function() {
                dTableManager.Border("#dTableStockAdjustment", 350);
            },
            buttons: [
            ],
            scrollY: "250px",
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
                    title: '#',
                    render: function(data, type, row, meta) {
                        return '<div class="dataTableSerialNumberDiv">' + (meta.row + 1) + '</div>';
                    }
                },
                {
                    data: 'ProductCode',
                    name: 'ProductCode',
                    orderable: false,
                    title: 'Code *....',
                    align: 'left',
                    render: function(data, type, row, meta) {
                        return "<input type='text' id='row-" + meta.row + "-ProductCode' name='row-" + meta.row + "-ProductCode'  value='" + data + "' class='minWidth80 form-control input-sm dTableProductCode dtInputs' placeholder='Product Code'/>";
                    }
                },
                {
                    data: 'ProductName',
                    name: 'ProductName',
                    title: 'Product Name',
                    align: 'left',
                    minWidth: 300,
                    orderable: false,
                    render: function(data, type, row, meta) {
                        return "<input type='text' id='row-" + meta.row + "-ProductName' name='row-" + meta.row + "-ProductName' value='" + data + "' class='minWidth260 form-control dtInputBackground input-sm dTableProductName dtInputs' placeholder='Product Name' readonly='true'/>";
                    }
                },
                {
                    data: 'BatchName',
                    name: 'BatchName',
                    title: 'Batch/Size *',
                    orderable: false,
                    render: function(data, type, row, meta) {
                        return "<select id='row-" + meta.row + "-BatchName' name='row-" + meta.row + "-BatchName'  value='" + data + "' class='minWidth100 form-control input-sm dTableBatchName dtInputs'></select>";
                    }
                },
                {
                    data: 'StockOnHand',
                    name: 'StockOnHand',
                    title: 'Stock *',
                    orderable: false,
                    render: function(data, type, row, meta) {
                        return "<input type='text' id='row-" + meta.row + "-StockOnHand' name='row-" + meta.row + "-StockOnHand' value='" + data + "' class='minWidth80 form-control dtInputBackground input-sm dTableStockOnHand dtInputs' placeholder='Stock On Hand' readonly='true'/>";
                    }
                },
                {
                    data: 'Qty',
                    name: 'Qty',
                    title: 'Qty *',
                    width: 100,
                    orderable: false,
                    render: function(data, type, row, meta) {
                        return "<input type='number' id='row-" + meta.row + "-Qty' name='row-" + meta.row + "-Qty' value='" + data + "' class='minWidth100 form-control input-sm dtInputTextAlign dTableQty dtInputs' placeholder='Qty' min='0'/>";
                    }
                }, {
                    data: 'PurchaseRate',
                    name: 'PurchaseRate',
                    title: 'PurchaseRate',
                    width: 100,
                    orderable: false,
                    render: function(data, type, row, meta) {
                        return "<input type='number' id='row-" + meta.row + "-PurchaseRate' name='row-" + meta.row + "-PurchaseRate'  value='" + data + "' class='minWidth90 form-control input-sm dtInputTextAlign dtInputBackground dTablePurchaseRate dtInputs' placeholder='PurchaseRate' readonly='true'  min='0'/>";
                    }
                },
                {
                    data: 'SellingRate',
                    name: 'SellingRate',
                    title: 'SellingRate',
                    width: 100,
                    orderable: false,
                    render: function (data, type, row, meta) {
                        return "<input type='number' id='row-" + meta.row + "-SellingRate' name='row-" + meta.row + "-SellingRate'  value='" + data + "' class='minWidth90 form-control input-sm dtInputTextAlign dtInputBackground dTableSellingRate dtInputs' placeholder='SellingRate' readonly='true'  min='0'/>";
                    }
                },

                {
                    data: '',
                    name: 'Amount',
                    title: 'Amount',
                    width: 100,
                    orderable: false,
                    render: function(data, type, row, meta) {
                        return "<input type='number' id='row-" + meta.row + "-Amount' name='row-" + meta.row + "-Amount'  value='" + data + "' class='minWidth100 form-control input-sm dtInputTextAlign dtInputBackground dTablePurchaseAmount dtInputs' placeholder='Amount' readonly='true' min='0'/>";
                    }
                },
                {
                    name: 'Option',
                    title: 'Option',
                    width: 20,
                    orderable: false,
                    render: function(data, type, row) {
                        return "<div style='min-width:90px;'><input type='button' value='Ad.' class='dtBtnAddNewRow btn-blue' style='float:left;' /><input type='button' value='Rv.' class='dtBtnRemoveRow btn-danger' style='float:left;'/></div>";
                    }
                }
            ],
            fixedColumns: true,
            data: obj,
            rowsGroup: null,
            ordering:false
        });


    },
    dTableEventManager: function (key, inputs, idx, $item) {
        if (key.keyCode == 13) {
            if (idx == 0) {
                if ($(inputs[0]).val() != "" && $(inputs[0]).val() != "0") {
                    Manager.GetProductDetailsForStockAdjustment($(inputs[0]).val(), inputs);
                    inputs[idx + 4].focus();
                    inputs[idx + 4].select();
                } else {
                    inputs[0].focus();
                    inputs[0].select();
                }
            } else if (idx == 4) {
                if ($(inputs[4]).val() && $(inputs[0]).val() && $(inputs[4]).val() > 0 && $(inputs[2]).val() != null) {
                    Manager.addNewDTableRow();
                    var nextInput = $item.parents('tr').next().find('.dtInputs');
                    nextInput[0].focus();
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
            if (inputs[idx - 1] != undefined)
                inputs[idx - 1].focus();
        } else if (key.keyCode == 38) {
            key.preventDefault();
            var uPArrowInputs = $item.parents('tr').prev().find('.dtInputs');
            if (uPArrowInputs[idx] != undefined)
                uPArrowInputs[idx].focus();
        } else if (key.keyCode == 39) {
            if (inputs[idx + 1] != undefined)
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
//    fetch("/pos/GetProductInfo").then(response => response.json().then(jsonData => {
//        productInfos = jsonData;
//    }));
//});

//$(document).on("keyup", ".dTableProductCode, #txtCodeOrBarcode", function (e) {
//    var elInput = this;
//    var elWrap = document.querySelector("#txtItemOrBarcodeAC");
//    if (e.key == "F4") {
//        if (isSearchOn == 0) {
//            isSearchOn = 1;
//            $("#chkSearchEnable").prop("checked", true);
//        } else {
//            isSearchOn = 0;
//            $("#chkSearchEnable").prop("checked", false);
//        }
//        return;
//    }
//    if ($("#chkSearchEnable").prop("checked")) { isSearchOn = 1; } else { isSearchOn = 0; }

//    var ignoreKey = [13, 37, 38, 39, 40];
//    if (!ignoreKey.find(a=>a == e.keyCode)) {
//        if (isSearchOn == 1) {
//            $("#txtCodeOrBarcode").focus();
//            $("#txtCodeOrBarcode").val(elInput.value);
//        }

//        elWrap.innerHTML = '<table class="table table-sm table-bordered table-search" style="min-width:700px;background: #f7f7f7;margin-bottom:7px;">' +
//                            '<thead class="theadPrdSrcTbl">' +
//                                    '<tr style="margin:0;padding:0;">' +
//                                        '<th>Product</th>' +
//                                        '<th>Product Code</th>' +
//                                        '<th>Product Bar Code</th>' +
//                                        '<th>Batch/Size</th>' +
//                                        '<th>PurchaseRate</th>' +
//                                    '</tr>' +
//                                '</thead>' +
//                                '<tbody id="txtItemOrBarcodeACTR">' +
//                                '</tbody>' +
//                            '</table>';

//        if (isSearchOn && elInput.value.length > 0) {
//            var matchs = false;
//            productInfos.forEach(function (pItem, ind) {
//                var serXp = new RegExp('^' + elInput.value, "i");
//                var serXI = new RegExp(elInput.value, "i");
//                if (
//                        pItem.ProductCode.search(serXp) != -1 ||
//                        pItem.ProductBarCode.search(serXI) != -1 ||
//                        pItem.ProductName.search(serXp) != -1 ||
//                        pItem.BatchName.search(serXp) != -1
//                ) {
//                    matchs = true;
//                    var elFound = '<tr><td class="txtItemOrBarcodeACITD"><button class="txtItemOrBarcodeACI" value="' + pItem.ProductCode + '">' + pItem.ProductName + '</button></td>' +
//                        '<td>' + pItem.ProductCode + '</td>' +
//                        '<td>' + pItem.ProductBarCode + '</td>' +
//                        '<td>' + pItem.BatchName + '</td>' +
//                        '<td style="text-align:right;">' + pItem.SellingRate + '/-</td></tr>';
//                    $(elWrap).find("#txtItemOrBarcodeACTR").append(elFound);

//                }
//            });

//            if (matchs) {
//                $(elWrap).css("display", "contents");
//                $("#divSearchOn").show();
//                $("#txtCodeOrBarcode").focus();
//            }
//        }
//    }
//});

//mohmed Labn 8-12-2024

$(document).on("keyup", ".dTableProductCode, #txtCodeOrBarcode", function (e) {
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
                        //if (billin
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



//var productInfos = null;
//var currentEl = null;
//var loadSearch = false;
//$(function () {
//    fetch("/pos/GetProductInfo").then(response => response.json().then(jsonData => {
//        productInfos = jsonData;
//    }));
//});

//dOM.addEventListener('keyup', function (e) {
//    if (e.key == "F4") {
//        loadSearch = !loadSearch;
//    }
//    if (e.target && (e.target.getAttribute("class") && e.target.getAttribute("class").search("dTableProductCode") != -1)) {
//        currentEl = e.target;
//        var elInput = e.target;
//        var elWrap = dOM.querySelector("#txtItemOrBarcodeAC");

//        if (e.key == "ArrowDown") {
//            if (elWrap.childElementCount > 0) {
//                let elFirst = dOM.querySelector(".txtItemOrBarcodeACI");
//                elFirst.setAttribute("class", "txtItemOrBarcodeACI active");
//                elFirst.parentNode.parentNode.setAttribute("class", "txtItemOrBarcodeACTR");
//                elFirst.focus();
//            }
//            return;
//        }        

//        elWrap.style.display = "none";
//        elWrap.innerHTML = '<table class="table table-sm table-bordered" style="margin:0;padding:0;">' +
//                                '<thead class="theadPrdSrcTbl">' +
//                                    '<tr style="margin:0;padding:0;">' +
//                                        '<th>Product</th>' +
//                                        '<th>Product Code</th>' +
//                                        '<th>Product Bar Code</th>' +
//                                        '<th>Batch/Size</th>' +
//                                        '<th>Selling Rate</th>' +
//                                    '</tr>' +
//                                '</thead>' +
//                                '<tbody id="txtItemOrBarcodeACTR">' +
//                                '</tbody>' +
//                            '</table>';
//        let elWrapTbody = dOM.querySelector("#txtItemOrBarcodeACTR");

//        if (loadSearch && elInput.value.length > 0) {
//            //loadSearch = false;
//            let matchs = false;
//            productInfos.forEach(function (pItem, ind) {
//                let serXp = new RegExp(elInput.value, "i");
//                if (
//                    pItem.ProductCode.search(serXp) != -1 ||
//                    pItem.ProductName.search(serXp) != -1 ||
//                    pItem.ProductBarCode.search(serXp) != -1 ||
//                    pItem.BatchName.search(serXp) != -1
//                    ) {
//                    matchs = true;
//                    let elFound = dOM.createElement("tr");
//                    elFound.innerHTML = '<td class="txtItemOrBarcodeACITD"><button class="txtItemOrBarcodeACI" value="' + pItem.ProductCode + '">' + pItem.ProductName + '</button></td>' +
//                                        '<td>' + pItem.ProductCode + '</td>' +
//                                        '<td>' + pItem.ProductBarCode + '</td>' +
//                                        '<td>' + pItem.BatchName + '</td>' +
//                                        '<td style="text-align:right;">' + pItem.SellingRate + '/-</td>';
//                    elWrapTbody.appendChild(elFound);

//                    //if (dOM.querySelectorAll(".txtItemOrBarcodeACI").length == 1) {
//                    //    elFound.setAttribute("class", "txtItemOrBarcodeACTR");
//                    //    dOM.querySelector(".txtItemOrBarcodeACI").setAttribute("class", "txtItemOrBarcodeACI active");
//                    //}

//                }
//            });

//            if (matchs) {
//                elWrap.style.display = "initial";
//            }
//        }
//    }
//});

//dOM.addEventListener('keyup', function (e) {
//    if (e.target && (e.target.getAttribute("class") && e.target.getAttribute("class").search("txtItemOrBarcodeACI") != -1)) {
//        e.preventDefault();
//        e.stopPropagation();
//        if (e.key == "ArrowUp") {
//            if (e.target.parentNode.parentNode.previousSibling && e.target.parentNode.parentNode.previousSibling.querySelector("button")) {
//                let preVN = e.target.parentNode.parentNode.previousSibling.querySelector("button");
//                e.target.setAttribute("class", "txtItemOrBarcodeACI");
//                e.target.parentNode.parentNode.setAttribute("class", "");
//                preVN.setAttribute("class", "txtItemOrBarcodeACI active");
//                preVN.parentNode.parentNode.setAttribute("class", "txtItemOrBarcodeACTR");
//                preVN.focus();
//            } else {
//                e.target.parentNode.parentNode.setAttribute("class", "");
//                e.target.setAttribute("class", "txtItemOrBarcodeACI");
//                currentEl.focus();
//            }
//        } else if (e.key == "ArrowDown") {
//            if (e.target.parentNode.parentNode.nextSibling && e.target.parentNode.parentNode.nextSibling.querySelector("button")) {
//                let nextVN = e.target.parentNode.parentNode.nextSibling.querySelector("button");
//                e.target.parentNode.parentNode.setAttribute("class", "");
//                e.target.setAttribute("class", "txtItemOrBarcodeACI");
//                nextVN.setAttribute("class", "txtItemOrBarcodeACI active");
//                nextVN.parentNode.parentNode.setAttribute("class", "txtItemOrBarcodeACTR");
//                nextVN.focus();
//            }
//        } else if (e.key == "Enter") {
//            currentEl.value = e.target.value;
//            currentEl.focus();
//            dOM.querySelector("#txtItemOrBarcodeAC").innerHTML = "";
//            dOM.querySelector("#txtItemOrBarcodeAC").style.display = "None";
//            var inputs = $(currentEl).parents('tr').find('.dtInputs');            
//            Manager.GetProductDetailsForStockAdjustment(e.target.value, inputs);
//            $(currentEl).parents('tr').find('.dTableQty').focus();
//            currentEl = null;
//        } else if (e.key == "Escape") {
//            currentEl.value = "";
//            currentEl.focus();
//            dOM.querySelector("#txtItemOrBarcodeAC").innerHTML = "";
//            dOM.querySelector("#txtItemOrBarcodeAC").style.display = "None";
//            currentEl = null;
//        }
//    }

//});

//dOM.addEventListener('click', function (e) {
//    if (e.target && (e.target.getAttribute("class") && e.target.getAttribute("class").search('txtItemOrBarcodeACI') != -1)) {
//        e.preventDefault();
//        e.stopPropagation();

//        currentEl.value = e.target.value;
//        currentEl.focus();
//        dOM.querySelector("#txtItemOrBarcodeAC").innerHTML = "";
//        dOM.querySelector("#txtItemOrBarcodeAC").style.display = "None";
//        var inputs = $(currentEl).parents('tr').find('.dtInputs');        
//        Manager.GetProductDetailsForStockAdjustment(e.target.value, inputs);
//        $(currentEl).parents('tr').find('.dTableQty').focus();
//        currentEl = null;
//    }
//});
