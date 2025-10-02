var dTableLocationTransfer = null;
var stockTypeDropdownValue = null;

$(document).ready(function () {
    Manager.LoadDataTableLocationTransfer();
    Manager.loadSupplier();
    Manager.loadBranch();
    Manager.loadStockType(0);
    Manager.addNewDTableRow();

    $("#FromLocation").change(function(){
        Manager.loadStockType(1);
    });

    $("#btnSave").prop("disabled", true);

    dTableLocationTransfer.on('order.dt search.dt', function () {
        dTableLocationTransfer.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
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
        Manager.SaveLocationTransfer();
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
    dTableLocationTransfer.row($(this).parents('tr')).remove().draw();
});


$(document).on('change', '.dtInputs', function (key) {
    var inputs = $(this).parents('tr').find('.dtInputs');
    Manager.calculateProductAmount(inputs);
});

$(document).on('change', '.dTableBatchName', function (key) {
    var inputs = $(this).parents('tr').find('.dtInputs');
    Manager.GetStockByLocationWise($(this).val(), inputs);

});

$(document).on('keydown', '.dtInputs', function(key) {
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

    GetStockByLocationWise: function (batchId, inputs) {
        var jsonParam = {
            batchId: batchId,
            officeId: $("#PosBranchId").val(),
            fromLocation: $("#FromLocation").val()
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
        dTableLocationTransfer.clear().row.add({
            'ProductCode': '',
            'ProductName': '',
            'BatchName': '',
            'StockOnHand': '',
            'Qty': '',
            'PurchaseRate': '0.00',
            'Amount': '0.00'
        }).draw();
        $("#CompanyInvNo").val('');
        $("#InvReferenceNo").val('');
        $("#NetPayable").val('');
        $("#Inv_TotalAmount").val(0);
    },
    SaveLocationTransfer: function () {
        var obj = [];
        obj.push({ 'Company Invoice No': $("#CompanyInvNo").val() });
        obj.push({ 'Invoice Date': $("#InvDate").val() });
        if (parseFloat($("#Inv_TotalAmount").val()) < 1) {
            Message.Warning("Select minimum one product to transfer stock location.");
            return;
        }
        if (JsManager.validate(obj)) {

            var locationTransferDetails = [];
            var objStock = new Object();
            objStock.CompanyInvNo = $("#CompanyInvNo").val();
            objStock.PosSupplierId = $("#PosSupplierId").val();
            objStock.FromPosBranchId = $("#PosBranchId").val();
            objStock.ToPosBranchId = $("#ToPosBranchId").val();
            objStock.FromLocation = $("#FromLocation").val();
            objStock.ToTocation = $("#ToLocation").val();
            objStock.InvDate = $("#InvDate").val();
            objStock.Remarks = $("#Inv_Remarks").val();
            var reqSts = 1;
            $.each($('#dTableLocationTransfer tbody tr'), function (rowIdx, val) {
                var objStockDetails = new Object();
                if ($(val).find(".dTableQty").val() && parseFloat($(val).find(".dTableQty").val()) < parseFloat("0.001")) {
                    Message.Warning($(val).find(".dTableProductName").val() + " Qty is can't be empty or zero.");
                    $(val).find(".dTableQty").focus();
                    $(val).find(".dTableQty").select();
                    reqSts = 0;
                    return;
                } else if ($(val).find(".dTableStockOnHand").val() && parseFloat($(val).find(".dTableStockOnHand").val()) < parseFloat("0.001")) {
                    Message.Warning("Zero stock can't be transfer of " + $(val).find(".dTableProductName").val());
                    $(val).find(".dTableStockOnHand").focus();
                    $(val).find(".dTableStockOnHand").select();
                    reqSts = 0;
                    return;
                } else {
                    if ($(val).find(".dTableQty").val() && $(val).find(".dTableQty").val()>0 && $(val).find(".dTableBatchName").val() && $(val).find(".dTableBatchName").val()!=null) {
                        objStockDetails.ProductCode = $(val).find(".dTableProductCode").val();
                        objStockDetails.PosProductBatchId = $(val).find(".dTableBatchName").val();
                        objStockDetails.Qty = $(val).find(".dTableQty").val();
                        locationTransferDetails.push(objStockDetails);
                    }
                }
            });
            objStock.LocationTransferDetails = locationTransferDetails;
            if (reqSts == 1) {
                if (Message.Prompt()) {
                    var jsonParam = { vmLocationTransfer: JSON.stringify(objStock) }
                    var serviceURL = "/PosStock/InsertLocationTransfer/";
                    JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
                }
            }


            function onSuccess(jsonData) {
                if (jsonData == "0") {
                    Message.Error("save");
                } else {
                    if (jsonData == "-1010") {
                        Message.Warning("Company Invoice Referance No can't be empty");
                    }else if (jsonData == "-1011") {
                        Message.Warning("Stock is not available to transfer please check and confirm");
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

    netAmtCalculation: function () {
        var productAmount = 0;
        $.each($('#dTableLocationTransfer tbody tr'), function (rowIdx, val) { 
            var tmpAmt = parseFloat($(val).find(".dTablePurchaseAmount").val());
            if (!isNaN(tmpAmt)) {
                productAmount += tmpAmt;
            } else {
                productAmount += 0;
            }
           
        });
        $("#Inv_TotalAmount").val(productAmount);
        
    },
    calculateProductAmount: function (inputs) {
        $("#btnSave").prop("disabled", true);
        $(inputs[5]).val($(inputs[2]).find(':selected').data('purchaserate'));
        var pRate = parseFloat($(inputs[5]).val());
        var pQty = parseFloat($(inputs[4]).val());
        var amt =pQty * pRate;
        
        $(inputs[6]).val(amt.toFixed(2));
        Manager.netAmtCalculation();
    },


    GetProductDetailsForLocationTransfer: function (pCode, inputs) {

        var jsonParam = {
            productCode: pCode,
            officeId: $("#PosBranchId").val(),
            fromLocation: $("#FromLocation").val()
        };
        var serviceURL = "/PosStock/GetProductDetailsForLocationTransfer/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            var foundSts = 0;
            var $tmpval;
            $.each($('#dTableLocationTransfer tbody tr'), function (rowIdx, val) {
            
                if ($(val).find(".dTableBatchName").val() == jsonData.PosProductBatchId) {
                    foundSts = 1;
                    $tmpval = val;
                }
                if ($(val).find(".dTableBatchName").val() == null && foundSts == 1) {
                    dTableLocationTransfer.row($(inputs[rowIdx]).parents('tr')).remove().draw();
                    $($tmpval).find(".dTableQty").focus();
                    $($tmpval).find(".dTableQty").select();
                    return;
                }


            });
           
            if (foundSts == 0){
                Manager.loadProductBatch(jsonData.Code, inputs);
                $(inputs[0]).val(jsonData.Code);
                $(inputs[1]).val(jsonData.Name);
                $(inputs[2]).val(jsonData.PosProductBatchId);
                $(inputs[3]).val(jsonData.StockOnHand);
                $(inputs[5]).val(jsonData.PurchaseRate);
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

    loadStockType: function(eventOnChange) {
        var jsonParam = '';
        var serviceURL = "/PosDropDown/GetStockType/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
        function onSuccess(jsonData) {
            var cbmOptions = "";
            if (eventOnChange == 0) {
                JsManager.PopulateCombo('#FromLocation', jsonData);
            }
            $.each(jsonData, function () {
                if ($("#FromLocation").val() != this.Id) {
                    cbmOptions += '<option value=\"' + this.Id + '\">' + this.Name + '</option>';
                }
            });
            $('#ToLocation').html(cbmOptions);
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
      dTableLocationTransfer.row.add({
            'ProductCode': '',
            'ProductName': '',
            'BatchName': '',
            'StockOnHand': '',
            'Qty': '',
            'PurchaseRate': '0.00',
            'Amount': '0.00'
        }).draw();
    },

 
    LoadDataTableLocationTransfer: function() {
        var objLocTrns = {
            'ProductCode': '',
            'ProductName': '',
            'BatchName': '',
            'StockOnHand': '',
            'Qty': '',
            'PurchaseRate': '0.00',
            'Amount': '0.00'
        };
        dTableLocationTransfer = $('#dTableLocationTransfer').DataTable({
                dom: 'B<"tableToolbar">rt',
                initComplete: function () {
                    dTableManager.Border("#dTableLocationTransfer", 300);
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
                        width:10,
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
                        title: 'Trns. Qty *',
                        width: 100,
                        orderable: false,
                        render: function(data, type, row, meta) {
                            return "<input type='number' id='row-" + meta.row + "-Qty' name='row-" + meta.row + "-Qty' value='" + data + "' class='minWidth100 form-control input-sm dtInputTextAlign dTableQty dtInputs' placeholder='Qty' min='0'/>";
                        }
                    },{
                        data: 'PurchaseRate',
                        name: 'PurchaseRate',
                        title: 'Rate',
                        width: 100,
                        orderable: false,
                        render: function(data, type, row, meta) {
                            return "<input type='number' id='row-" + meta.row + "-PurchaseRate' name='row-" + meta.row + "-PurchaseRate'  value='" + data + "' class='minWidth90 form-control input-sm dtInputTextAlign dtInputBackground dTablePurchaseRate dtInputs' placeholder='Rate' readonly='true'  min='0'/>";
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
                data: objLocTrns,
                rowsGroup: null,
                ordering:false
            });

        
    },
    dTableEventManager: function (key, inputs, idx, $item) {
        if (key.keyCode == 13) {
            if (idx == 0) {
                if ($(inputs[0]).val() != "" && $(inputs[0]).val() != "0") {
                    Manager.GetProductDetailsForLocationTransfer($(inputs[0]).val(), inputs);
                    inputs[idx + 4].focus();
                    inputs[idx + 4].select();
                } else {
                    inputs[0].focus();
                    inputs[0].select();
                }
            } else if (idx == 4) {
                if ($(inputs[4]).val() && $(inputs[0]).val() && $(inputs[4]).val() > 0 && $(inputs[2]).val() != null && $(inputs[3]).val() && $(inputs[3]).val() > 0) {
                    if ($(inputs[3]).val() && $(inputs[3]).val()>0) {
                        Manager.addNewDTableRow();
                        var nextInput = $item.parents('tr').next().find('.dtInputs');
                        nextInput[0].focus();
                    }
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
//        }

//        elWrap.innerHTML = '<table class="table table-sm table-bordered table-search" style="min-width:700px;background: #f7f7f7;margin-bottom:7px;">' +
//                            '<thead class="theadPrdSrcTbl">' +
//                                    '<tr style="margin:0;padding:0;">' +
//                                        '<th>Product</th>' +
//                                        '<th>Product Code</th>' +
//                                        '<th>Product Bar Code</th>' +
//                                        '<th>Batch/Size</th>' +
//                                        '<th>Rate</th>' +
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

