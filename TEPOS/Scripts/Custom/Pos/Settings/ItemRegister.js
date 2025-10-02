var dTable=null;
var dTableItemBatch = null;
var dTableUom = null;
var _productId = 0;

$(document).ready(function () {
    Manager.GetItemList(0);
    Manager.GetItemBatch(0);
    Manager.addItemBatchRow();
    Manager.loadSupplier();
    Manager.loadUomGroup();
    Manager.LoadCategoryDropdown();
    Manager.LoadClassDropdown();
    Manager.LoadGroupDropdown();
    Manager.LoadSourceDropdown();
    Manager.LoadDescriptionDropdown();
    $("#Product_PosUomGroupId").change(function () {
        var refresh =dTableUom!=null?1: 0;
        Manager.GetUomDetails(refresh,$(this).val());
    });

    dTableItemBatch.on('order.dt search.dt', function () {
        dTableItemBatch.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = dTableManager.IndexColumn(i + 1);
        });
    }).draw();
    dTable.on('order.dt search.dt', function () {
        dTable.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = dTableManager.IndexColumn(i + 1);
        });
    }).draw();


});

$(document).on('click', '.spnOptBarcode', function () {
    JsManager.StartProcessBar();
    $("#svgBarcode").empty();
    $("#modalBarcode").modal('show');

    var $item = $(this).parents('tr');
    var row = dTable.row($item).data();
    Manager.GetItemBatch(row.Id);

    var jsonParam = { productId: row.Id };
    var serviceURL = "/PosSetting/GetBarcodeByItemWise/";
    JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);
    function onSuccess(jsonData) {
        $("#divBarcode").empty();
        $.each(jsonData, function(i, v) {
            var htm = '<div class="form-group">' +
                '<div class="input-group">' +
                '<span title="Click to unlock and change barcode" class="input-group-btn" style="background: #e6e6e6;padding-left: 3px;border-radius: 2px 0px 0px 2px;border: 1px solid #dad5d5;"><input type="checkbox" class="clsChkEnableEditBarcode" /></span>' +
                '<input type="text" value="'+v.BarCode+'" class="form-control input-sm clsInputShowBarcode" id="txtBarcode" aria-describedby="smbarcode" readonly=""/>' +
                '<span class="input-group-btn"><input type="button" class="btn btn-primary btn-sm clsBtnShowBarcode" value="Generate Barcode" style="height: 28px; line-height: 0px;" /></span>' +
                '</div>' +
                '</div>';
            $("#divBarcode").append(htm);
        });
    }
    function onFailed(xhr, status, err) {
    }
    JsManager.EndProcessBar();
});
$(document).on('click', '.spnOptBarcodeLabel', function () {
    JsManager.StartProcessBar();
    $("#BarcodeLabel").empty();
    $("#modalBarcodeLabel").modal('show');

    var $item = $(this).parents('tr');
    var row = dTable.row($item).data();
    Manager.GetItemBatch(row.Id);

    var jsonParam = { productId: row.Id };
    var serviceURL = "/PosSetting/GetBarcodeByItemWise/";
    JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);
    function onSuccess(jsonData) {
        $("#divBarcodeLabel").empty();
        $.each(jsonData, function (i, v) {
            var htm = '<div class="form-group">' +
                '<div class="input-group">' +
                '<input type="text" style="width: 17%;" value="' + v.BarCode + '" class="form-control input-sm clsInputShowBarCodeLabel" id="txtBarcodeLabel" aria-describedby="smbarcode" readonly=""/>' +
                '<input type="text" style="width: 36%;"  value="' + v.Name + '" class="form-control input-sm clsInputShowName" id="txtName" aria-describedby="smbarcode" readonly=""/>' +
                '<input type="text" style="width: 8%;" value="' + v.SellingRate + '" class="form-control input-sm clsInputShowSellingRate" id="txtSellingRate" aria-describedby="smbarcode" readonly=""/>' +
                '<input type="text" style="width: 15%;" value="' + JsManager.ChangeDateFormat(v.DateTo,0) + '" class="form-control input-sm clsInputShowDateTo" id="txtDateTo" aria-describedby="smbarcode" readonly=""/>' +
                '<span class="input-group-btn"><input type="button" class="btn btn-primary btn-sm clsBtnShowBarcodeLabel" value="Generate Barcode Label" style="height: 28px; line-height: 0px;" /></span>' +
                '</div>' +
                '</div>';
            $("#divBarcodeLabel").append(htm);
        });
    }
    function onFailed(xhr, status, err) {
    }
    JsManager.EndProcessBar();
});

$(document).on('click', '.clsBtnShowBarcode', function () {

    JsBarcode("#svgBarcode", 
        $(this).parent().parent().find('.clsInputShowBarcode').val(),
        {
            //format: "pharmacode",
            lineColor: "#000",
            height: 47,
            //width:3,
            displayValue: true
        }
        );
});

$(document).on('click', '.clsBtnShowBarcodeLabel', function () {

    var htmLabel = '<div class="form-group">' +
        '<div class="input-group">' +
        '<input type="text" style="width: 20%;" value="Code" class="form-control input-sm clsInputShowBarcodeLabelForPrint" id="LabelBarcodeLabelForPrint" aria-describedby="smBarcodeLabel" readonly=""/>' +
        '<input type="text" style="width: 50%;" value="Name" class="form-control input-sm clsInputShowName" id="LabelName" aria-describedby="smBarcodeLabel" readonly=""/>' +
        '<input type="text" style="width: 10%;" value="Price" class="form-control input-sm clsInputShowSellingRate" id="LabelSellingRate" aria-describedby="smBarcodeLabel" readonly=""/>' +
        '<input type="text" style="width: 20%;" value="Expiry Date" class="form-control input-sm clsInputShowDateTo" id="LabelDateTo" aria-describedby="smBarcodeLabel" readonly=""/>' +
        '</div>' +
        '<div class="input-group">' +
        '<input type="text" style="width: 20%;" value="' + $(this).parent().parent().find('#txtBarcodeLabel').val() + '" class="form-control input-sm clsInputShowBarcodeLabelForPrint" id="txtBarcodeLabelForPrint" aria-describedby="smBarcodeLabel" readonly=""/>' +
        '<input type="text" style="width: 50%;" value="' + $(this).parent().parent().find('#txtName').val() + '" class="form-control input-sm clsInputShowName" id="txtName" aria-describedby="smBarcodeLabel" readonly=""/>' +
        '<input type="text" style="width: 10%;" value="' + $(this).parent().parent().find('#txtSellingRate').val() + '" class="form-control input-sm clsInputShowSellingRate" id="txtSellingRate" aria-describedby="smBarcodeLabel" readonly=""/>' +
        '<input type="text" style="width: 20%;" value="' + $(this).parent().parent().find('#txtDateTo').val() + '" class="form-control input-sm clsInputShowDateTo" id="txtDateTo" aria-describedby="smBarcodeLabel" readonly=""/>' +
        '</div>' +
        '<div class="input-group"><br /><br /><br /></div>'
        '<div class="input-group"><br /></div>'
        '<div class="input-group"><br /></div>'
        '</div>';
    $("#BarcodeLabel").append(htmLabel);

       
});
$(document).on('click', '.clsChkEnableEditBarcode', function () {
    $(this).parent().parent().find('.clsInputShowBarcode').prop('readonly', false);
});

$(document).on('click', '#btnPrintBarcode', function () {
    
    if ($("#svgBarcode").clone().html().length > 1) {
        var barCode = '';
        for (var i = 0; i < $("#txtNumberOfBarcode").val(); i++) {
            barCode += $("#barcode").clone().html();
        }
        var printWindow ='<div id="mainDiv">' +barCode +'</div>';
        CommonManager.PrintHtmlPage("100%", "0", printWindow);
        } else {
        Message.Warning("At first generate barcode after then print.");
    }
});

$(document).on('click', '#btnPrintBarcodeLabel', function () {
    if ($("#BarcodeLabel").clone().html().length > 1) {
        var BarcodeLabel = '';
        for (var i = 0; i < $("#txtNumberOfBarcodeLabel").val(); i++) {
            BarcodeLabel += $("#BarcodeLabel").clone().html();
        }
        var printWindow = '<div id="mainDiv">' + BarcodeLabel + '<div id="Div" class="row"></div></div>';
        CommonManager.PrintHtmlPage("100%", "0", printWindow);
    } else {
        Message.Warning("At first generate BarcodeLabel after then print.");
    }
});

$(document).on('click', '.dTableDelete', function() {
    var $item = $(this).parents('tr');
    var row = dTable.row($item).data();
    Manager.Delete(row.Id);
});

$(document).on('click', '.dTableEdit', function () {
    JsManager.StartProcessBar();
    $('html,body').animate({ 'scrollTop': 0 }, 100);

    var $item = $(this).parents('tr');
    var row = dTable.row($item).data();
    _productId = row.PosProductId;
    $("#Product_Code").val(row.Code);
    $("#Product_CompanyCode").val(row.CompanyCode);
    $("#Product_Name").val(row.Name);
    $("#Product_NameInOtherLang").val(row.NameInOtherLang);
    $("#Product_ShortName").val(row.ShortName);
    $("#Product_PosProductScientificName").val(row.PosProductScientificName);//Adel
    $("#Product_IsHideToStock").prop("checked", row.IsHideToStock);
    $("#Product_IsPriceChangeable").prop("checked", row.IsPriceChangeable);
    $("#Product_PosProductHasExpire").prop("checked", row.PosProductHasExpire);//Adel
    $("#Product_PosProductIsShortage").prop("checked", row.PosProductIsShortage);//Adel
    $("#Product_PosProductNote").val(row.PosProductNote);//Adel

    var selectCategoryId = row.CategoryId ? row.CategoryId : 0;
    $("#Product_PosProductCategoryId").val(selectCategoryId);
    $("#Product_PosProductCategoryId").trigger('chosen:updated');

    var selectClassId = row.ClassId ? row.ClassId : 0; //Adel
    $("#Product_PosProductClassId").val(selectClassId); //Adel
    $("#Product_PosProductClassId").trigger('chosen:updated');//Adel

    var selectSourceId = row.SourceId ? row.SourceId : 0; //Adel
    $("#Product_PosProductSourceId").val(selectSourceId); //Adel
    $("#Product_PosProductSourceId").trigger('chosen:updated');//Adel

    var selectGroupId = row.GroupId ? row.GroupId : 0; //Adel
    $("#Product_PosProductGroupId").val(selectGroupId); //Adel
    $("#Product_PosProductGroupId").trigger('chosen:updated');//Adel

    var selectDescriptionId = row.DescriptionId ? row.DescriptionId : 0; //Adel
    $("#Product_PosProductDescriptionId").val(selectDescriptionId); //Adel
    $("#Product_PosProductDescriptionId").trigger('chosen:updated');//Adel

    $("#Product_PosUomGroupId").val(row.PosUomGroupId);
    $("#Product_PosUomGroupId").trigger('chosen:updated');
    $("#Product_PosUomGroupId").trigger('change');

    $("#Product_PosSupplierId").val(row.PosSupplierId);
    $("#Product_PosSupplierId").trigger('chosen:updated');
    $("#Product_Vat").val(row.Vat);
    $("#Product_Sd").val(row.Sd);

    Manager.GetItemBatch(row.Id);
    JsManager.EndProcessBar();
});


$(document).on("click", ".btnAddNewBatch", function () {
    Manager.addItemBatchRow();
});

$(document).on('click', '.btnRemoveNewBatch', function () {
    dTableItemBatch.row($(this).parents('tr')).remove().draw();
});

$(document).on('click', '#btnSave', function () {
    Manager.Save();
});

$(document).on('click', '#btnEdit', function () {
    Manager.Update();
});

$(document).on('click', '#btnClear', function () {
    Manager.formReset();
});

var Manager = {
    formReset: function () {
        _productId = 0;
        $("#frmItemRegister")[0].reset();
        $("#Product_PosUomGroupId").trigger("chosen:updated");
        $("#Product_PosUomGroupId").trigger('change');
        $("#Product_PosSupplierId").trigger("chosen:updated");
        $("#Product_PosProductCategoryId").trigger("chosen:updated");
        $("#Product_PosProductClassId").trigger("chosen:updated");//Adel
        $("#Product_PosProductSourceId").trigger("chosen:updated");//Adel
        $("#Product_PosProductGroupId").trigger("chosen:updated");//Adel
        $("#Product_PosProductDescriptionId").trigger("chosen:updated");//Adel

        $("#Product_Vat").val(0.00);
        $("#Product_Sd").val(0.00);
     
        Manager.GetItemBatch(1);
        Manager.addItemBatchRow();
    },

    Delete:function(productId) {
        if (Message.Prompt()) {
            var jsonParam = { productId:  productId};
            var serviceURL = "/PosSetting/DeleteProductById/";
            JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
        }


        function onSuccess(jsonData) {
            if (jsonData == "0") {
                Message.Error("delete");
            } else {
                Message.Success("delete");
               Manager.formReset();
                Manager.GetItemList(1);
            }
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

    Update: function () {
        var obj = [];
        obj.push({ 'Product Category': $("#Product_PosProductCategoryId").val() });
        obj.push({ 'Product Class': $("#Product_PosProductClassId").val() });//Adel
        obj.push({ 'Product Source': $("#Product_PosProductSourceId").val() });//Adel
        obj.push({ 'Product Group': $("#Product_PosProductGroupId").val() });//Adel
        obj.push({ 'Product Description': $("#Product_PosProductDescriptionId").val() });//Adel
        obj.push({ 'Product Code': $("#Product_Code").val() });
        obj.push({ 'Product Company Code': $("#Product_CompanyCode").val() });
        obj.push({ 'Product Name': $("#Product_Name").val() });
        obj.push({ 'Product ShortName': $("#Product_ShortName").val() });
        obj.push({ 'Units of measurement': $("#Product_PosUomGroupId").val() });
        $.each($('#dTableItemBatch tbody tr'), function(i, v) {
            obj.push({ 'Item Batch': $(v).find('.dTableBatch').val() });
            obj.push({ 'Item MRP': $(v).find('.dTableMrp').val() });
            obj.push({ 'Item Purchase Rate': $(v).find('.dTablePurchaseRate').val() });
            obj.push({ 'Item Selling Rate': $(v).find('.dTableSellingRate').val() });
        });
        if (JsManager.validate(obj)) {
            var itemBatch = [];
            $.each($('#dTableItemBatch tbody tr'), function(i, v) {
      
                var objBach = new Object();

                var idOfBatch = $(v).find('.dTableBatch').data('batchid');
                objBach.Id = idOfBatch == "undefined" ? 0 : idOfBatch;
                objBach.BatchName = $(v).find('.dTableBatch').val();
                objBach.Mrp = $(v).find('.dTableMrp').val();
                objBach.PurchaseRate = $(v).find('.dTablePurchaseRate').val();
                objBach.SellingRate = $(v).find('.dTableSellingRate').val();
                objBach.Weight = $(v).find('.dTableWeight').val();
                if (objBach.Weight == "") {
                    objBach.Weight = "0";
                }
                obj.push({ 'Item Weight': objBach.Weight });                
                objBach.BarCode = $(v).find('.dTableBarCode').val();
                itemBatch.push(objBach);
            });

            if (Message.Prompt("Do you want to change? carefully because of the change effect on all information. the best solution creates a new product or new batch after then transfer stock to a new product or batch and reduces stock from old product or batch.")) {
                var jsonParam = $("#frmItemRegister").serialize() + '&itemBatch=' + JSON.stringify(itemBatch)+"&productId="+_productId;
                var serviceURL = "/PosSetting/UpdateItemRegister/";
                JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
            }


            function onSuccess(jsonData) {
                if (jsonData == "0") {
                    Message.Error("update");
                } else {
                    Message.Success("update");
                    Manager.formReset();
                    Manager.GetItemList(1);
                }
            }

            function onFailed(xhr, status, err) {
                Message.Exception(xhr);
            }
        }
    },

    Save:function() {
        var obj = [];
        obj.push({ 'Product Category': $("#Product_PosProductCategoryId").val() });
        obj.push({ 'Product Class': $("#Product_PosProductClassId").val() });//Adel
        obj.push({ 'Product Source': $("#Product_PosProductSourceId").val() });//Adel
        obj.push({ 'Product Group': $("#Product_PosProductGroupId").val() });//Adel
        obj.push({ 'Product Description': $("#Product_PosProductDescriptionId").val() });//Adel
        obj.push({ 'Product Code': $("#Product_Code").val() });
        obj.push({ 'Product Company Code': $("#Product_CompanyCode").val() });
        obj.push({ 'Product Name': $("#Product_Name").val() });
        obj.push({ 'Product ShortName': $("#Product_ShortName").val() });
        obj.push({ 'Units of measurement': $("#Product_PosUomGroupId").val() });
        $.each($('#dTableItemBatch tbody tr'), function (i, v) {
            obj.push({ 'Item Batch': $(v).find('.dTableBatch').val() });
            obj.push({ 'Item MRP': $(v).find('.dTableMrp').val() });
            obj.push({ 'Item Purchase Rate': $(v).find('.dTablePurchaseRate').val() });
            obj.push({ 'Item Selling Rate': $(v).find('.dTableSellingRate').val() });
        });
        if (JsManager.validate(obj)) {
            var itemBatch = [];
            $.each($('#dTableItemBatch tbody tr'), function(i, v) {
                var objBach = new Object();
                objBach.BatchName = $(v).find('.dTableBatch').val();
                objBach.Mrp = $(v).find('.dTableMrp').val();
                objBach.PurchaseRate = $(v).find('.dTablePurchaseRate').val();
                objBach.SellingRate = $(v).find('.dTableSellingRate').val();
                objBach.Weight = $(v).find('.dTableWeight').val();
                if (objBach.Weight == "") {
                    objBach.Weight = "0";
                }
                obj.push({ 'Item Weight': objBach.Weight });                
                objBach.BarCode = $(v).find('.dTableBarCode').val();
                debugger;
                if (objBach.BarCode == '' || objBach.BarCode == null) {
                    objBach.BarCode = $("#Product_Code").val();
                    objBach.ProductCode = $("#Product_Code").val();
                }
                itemBatch.push(objBach);
            });

            if (Message.Prompt()) {
                var jsonParam = $("#frmItemRegister").serialize() + '&itemBatch=' + JSON.stringify(itemBatch);
                var serviceURL = "/PosSetting/InsertItemRegister/";
                JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
            }


            function onSuccess(jsonData) {
                if (jsonData == "0") {
                    Message.Error("save");
                } else {
                    Message.Success("save");
                    Manager.formReset();
                    Manager.GetItemList(1);
                }
            }

            function onFailed(xhr, status, err) {
                Message.Exception(xhr);
            }
        }
    },

    loadUomGroup:function() {
        var jsonParam = '';
        var serviceURL = "/PosDropDown/GetUomGroup/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
        function onSuccess(jsonData) {
            var objProgram = jsonData;
            JsManager.PopulateCombo('#Product_PosUomGroupId', objProgram, 'Unit Of measurement');
            $("#Product_PosUomGroupId").chosen();
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
    },

    LoadCategoryDropdown: function () {
        var jsonParam = '';
        var serviceURL = "/PosDropDown/GetCategories/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
        function onSuccess(jsonData) {
            var objProgram = jsonData;
            JsManager.PopulateCombo('#Product_PosProductCategoryId', objProgram, '  Category');
            $("#Product_PosProductCategoryId").chosen({width:'97%'});
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
    },
    //Adel
    LoadClassDropdown: function () {
        var jsonParam = '';
        var serviceURL = "/PosDropDown/GetProductClass/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
        function onSuccess(jsonData) {
            var objProgram = jsonData;
            JsManager.PopulateCombo('#Product_PosProductClassId', objProgram, '  Class');
            $("#Product_PosProductClassId").chosen({ width: '97%' });
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
    },
    //Adel
    LoadGroupDropdown: function () {
        var jsonParam = '';
        var serviceURL = "/PosDropDown/GetProductGroup/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
        function onSuccess(jsonData) {
            var objProgram = jsonData;
            JsManager.PopulateCombo('#Product_PosProductGroupId', objProgram, '  Group');
            $("#Product_PosProductGroupId").chosen({ width: '97%' });
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
    },
    //Adel
    LoadSourceDropdown: function () {
        var jsonParam = '';
        var serviceURL = "/PosDropDown/GetProductSource/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
        function onSuccess(jsonData) {
            var objProgram = jsonData;
            JsManager.PopulateCombo('#Product_PosProductSourceId', objProgram, '  Source');
            $("#Product_PosProductSourceId").chosen({ width: '97%' });
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
    },
    //Adel
    LoadDescriptionDropdown: function () {
        var jsonParam = '';
        var serviceURL = "/PosDropDown/GetProductDescription/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
        function onSuccess(jsonData) {
            var objProgram = jsonData;
            JsManager.PopulateCombo('#Product_PosProductDescriptionId', objProgram, '  Description');
            $("#Product_PosProductDescriptionId").chosen({ width: '97%' });
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
    },
    loadSupplier: function () {
        var jsonParam = '';
        var serviceURL = "/PosDropDown/GetSupplier/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
        function onSuccess(jsonData) {
            var objProgram = jsonData;
            JsManager.PopulateCombo('#Product_PosSupplierId', objProgram, 'Supplier');
            $("#Product_PosSupplierId").chosen();
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
    },

    addItemBatchRow:function() {
        dTableItemBatch.row.add({
            'BatchName': '',
            'Mrp': '',
            'PurchaseRate': '',
            'SellingRate': '',
            'Weight': '',
            'BarCode':''
        }).draw();
    },


    GetUomDetails: function (ref, uomGroupId) {
        var jsonParam = { uomGroupId:uomGroupId };
        var serviceURL = "/PosSetting/GetUomDetailsByUomGroupId/";
        JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);
        function onSuccess(jsonData) {
            Manager.LoadDataTableUomDetails(jsonData, ref);
           
        }
        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

    LoadDataTableUomDetails: function (userdata, isRef) {
        if (isRef == "0") {
            dTableUom = $('#dTableUom').DataTable({
                dom: 'B<"tableToolbar">rti',
                initComplete: function () {
                    dTableManager.Border("#dTableUom", 200);
                },
                buttons: [],
                scrollX: true,
                scrollCollapse: true,
                lengthMenu: [[-1], ["All"]],
                columnDefs: [
                    { visible: false, targets: [] },
                ],
                columns: [
                    { name: 'SL', title: '#', orderable: false, render: function (data, type, row, meta) {
                         return '<div class="font-weight" style="padding-top: 3px;" align="center">' + (meta.row + 1) + '</div>' } },
                    { data: 'UomCode', name: 'UomCode', title: 'Code', align: 'center', width: 120 },
                    { data: 'UomDescription', name: 'UomDescription', title: 'Description', width: 180 },
                    { data: 'ConversionFactor', name: 'ConversionFactor', title: 'Conversion Factor', width: 180 },
                    { data: 'IsBaseUom', name: 'IsBaseUom', title: 'Base Uom', width: 100 }
                ],
                fixedColumns: true,
                data: userdata,
                rowsGroup: null
            });

        } else {
            dTableUom.clear().rows.add(userdata).draw();

        }

    },

    GetItemBatch: function (ref) {
        var jsonParam = { productId: _productId };
        var serviceURL = "/PosSetting/GetItemBatch/";
        JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);
        function onSuccess(jsonData) {
            Manager.LoadDataTableItemBatch(jsonData, ref);
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

    LoadDataTableItemBatch: function (userdata, isRef) {
        if (isRef == "0") {
            dTableItemBatch = $('#dTableItemBatch').DataTable({
                dom: 'B<"tableToolbar">rti',
                initComplete: function() {
                    dTableManager.Border("#dTableItemBatch", 200);
                },
                buttons: [
                ],
                width: '200px',
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
                        return '';
                    }
                },
                {
                    data: 'BatchName',
                    name: 'BatchName',
                    orderable: false,
                    title: 'Batch/Size',
                    align: 'left',
                    width: 80,
                    render: function(data, type, row, meta) {
                        return "<input type='text' id='row-" + meta.row + "-BatchName' data-batchid=" + row["Id"] + " name='row-" + meta.row + "-BatchName' tabindex=" + (meta.row + 1 + 8) + "  value='" + data + "' class='form-control input-sm dTableBatch' placeholder='Batch/Size'/>";
                    }
                },
                {
                    data: 'Mrp',
                    name: 'Mrp',
                    title: 'MRP',
                    align: 'left',
                    width: 70,
                        orderable: false,
                        render: function(data, type, row, meta) {
                            return "<input type='number' id='row-" + meta.row + "-Mrp' name='row-" + meta.row + "-Mrp' tabindex=" + (meta.row + 1 + 8) + "  value='" + data + "' class='form-control input-sm dTableMrp' placeholder='MRP.'/>";
                        }
                    },
                    {
                        data: 'PurchaseRate',
                        name: 'PurchaseRate',
                        title: 'Purchase Rate',
                        orderable: false,
                        width: 85,
                        render: function(data, type, row, meta) {
                            return "<input type='number' id='row-" + meta.row + "-PurchaseRate' name='row-" + meta.row + "-PurchaseRate' tabindex=" + (meta.row + 1 + 8) + " value='" + data + "' class='form-control input-sm dTablePurchaseRate' placeholder='Purchase Rate' style='text-align:right;'/>";
                        }
                    },
                    {
                        data: 'SellingRate',
                        name: 'SellingRate',
                        title: 'Selling Rate',
                        width: 85,
                        orderable: false,
                        render: function(data, type, row, meta) {
                            return "<input type='number' id='row-" + meta.row + "-SellingRate' name='row-" + meta.row + "-SellingRate' tabindex=" + (meta.row + 1 + 8) + " value='" + data + "' class='form-control input-sm dTableSellingRate' placeholder='Selling Rate' style='text-align:right;'/>";
                        }
                    },
                    {
                        data: 'Weight',
                        name: 'Weight',
                        title: 'Weight(gm)',
                        width:70,
                        orderable: false,
                        render: function(data, type, row, meta) {
                            return "<input type='number' id='row-" + meta.row + "-Weight' name='row-" + meta.row + "-Weight' tabindex=" + (meta.row + 1 + 8) + " value='" + data + "' class='form-control input-sm dTableWeight' placeholder='Item Weight' style='text-align:right;'/>";
                        }
                    },
                    {
                        data: 'BarCode',
                        name: 'BarCode',
                        title: 'Bar Code',
                        width: 100,
                        orderable: false,
                        render: function(data, type, row, meta) {
                            return "<input type='number' id='row-" + meta.row + "-BarCode' name='row-" + meta.row + "-BarCode' tabindex=" + (meta.row + 1 + 8) + " value='" + data + "' class='form-control input-sm dTableBarCode' placeholder='Bar Code'/>";
                        }
                    },
                    {
                        name: 'Option',
                        title: 'Option',
                        width: 50,
                        orderable: false,
                        render: function(data, type, row) {
                            return "<div style='width:100%;'><input type='button' value='Ad.' class='btnAddNewBatch btn-blue' style='float:left;' /><input type='button' value='Rv.' class='btnRemoveNewBatch btn-danger' style='float:left;'/></div>";
                        }
                    }
                ],
                fixedColumns: true,
                data: userdata,
                rowsGroup: null,
                ordering: false
            });

        } else {
            dTableItemBatch.clear().rows.add(userdata).draw();

        }

    },

    GetItemList: function (ref) {
        var jsonParam ='';
        var serviceURL = "/PosSetting/GetItemList/";
        JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);
        function onSuccess(jsonData) {
            Manager.LoadDataTableItemList(jsonData, ref);
        }
        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

    LoadDataTableItemList: function (userdata, isRef) {
        if (isRef == "0") {
            dTable = $('#dTable').DataTable({
                dom: "<'row'<'col-md-6'B><'col-md-3'l><'col-md-3'f>>" + "<'row'<'col-md-12'tr>>" + "<'row'<'col-md-5'i><'col-md-7 mt-7'p>>",
                initComplete: function () {
                    dTableManager.Border("#dTable", 350);
                },
                buttons: [
                    {
                        text: '<i class="far fa-file-pdf"></i> PDF',
                        className: 'btn btn-sm',
                        extend: 'pdfHtml5',
                        exportOptions: {
                            columns: [2, 3, 4, 5, 6,7,8,9,10]
                        },
                        title: 'Items List'
                    },
                    {
                        text: '<i class="fas fa-print"></i> Print',
                        className: 'btn btn-sm',
                        extend: 'print',
                        exportOptions: {
                            columns: [2, 3, 4, 5, 6, 7, 8, 9, 10]
                        },
                        title: 'Items List'
                    },
                    {
                        text: '<i class="far fa-file-excel"></i> Excel',
                        className: 'btn btn-sm',
                        extend: 'excelHtml5',
                        exportOptions: {
                            columns: [2, 3, 4, 5, 6, 7, 8, 9, 10]
                        },
                        title: 'Items List'
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
                        title: '#',
                        render: function(data, type, row, meta) {
                            return '';
                        }
                    },
                    {
                        name: 'Option',
                        title: 'Option', width: 120,
                        render: function (data, type, row) {
                            return EventManager.DataTableCommonButton() + " <span class='spnOptBarcode' style='color: #000;font-weight: bold;margin-left:6px;cursor: pointer;font-size:18px;' title='Click to generate barcode'><i class='fas fa-barcode'></i></span>"
                                + " <span class='spnOptBarcodeLabel' style='color: #000;font-weight: bold;margin-left:6px;cursor: pointer;font-size:18px;' title='Click to generate barcode Label'><i class='fas fa-print'></i></span>";

                        }
                    },
                    { data: 'Code', name: 'Code', title: 'Code', align: 'center',width:100 },
                    { data: 'CompanyCode', name: 'CompanyCode', title: 'Company Code',width:100 },
                    { data: 'Name', name: 'Name', title: 'Product Name', width: 250 },
                    { data: 'ShortName', name: 'ShortName', title: 'Short Name', width: 200 },
                    { data: 'CategoryName', name: 'CategoryName', title: 'Category', width: 150 },
                    { data: 'ClassName', name: 'ClassName', title: 'Class', width: 150 },//Adel
                    { data: 'GroupName', name: 'GroupName', title: 'Group', width: 150 },//Adel
                    { data: 'SourceName', name: 'SourceName', title: 'Source', width: 150 },//Adel
                    { data: 'DescreptionName', name: 'DescreptionName', title: 'Descreption', width: 150 },//Adel
                    { data: 'NameInOtherLang', name: 'NameInOtherLang', title: 'Name In Arabic',width:230 },
                    { data: 'SupplierName', name: 'SupplierName', title: 'Supplier', width: 150 },
                    { data: 'NumberOfBatch', name: 'NumberOfBatch', title: 'Total Batch',width:80 },
                    { data: 'Vat', name: 'Vat', title: 'Vat', width: 60,align:'center' },
                    { data: 'Sd', name: 'Sd', title: 'SD', width: 60,align:'center' },
                    { data: 'UomGroup', name: 'UomGroup', title: 'Uom Group', width: 80 },
                    {
                        data: 'IsHideToStock', name: 'IsHideToStock', title: 'Hide to Stock', width: 110,
                        render: function(data, type, row) {
                            if (data == true)
                                return "Yes";
                            return "No";
                        }
                    },
                    {
                        data: 'IsPriceChangeable', name: 'IsPriceChangeable', title: 'Rate Cng.', width: 90,
                        render: function(data, type, row) {
                            if (data == true)
                                return "Yes";
                            return "No";
                        }
                    },
                    //adel
                    {
                        data: 'PosProductHasExpire', name: 'PosProductHasExpire', title: 'Has Expire', width: 90,
                        render: function (data, type, row) {
                            if (data == true)
                                return "Yes";
                            return "No";
                        }
                    },
                    {
                        data: 'PosProductIsShortage', name: 'PosProductIsShortage', title: 'Has Shortage', width: 90,
                        render: function (data, type, row) {
                            if (data == true)
                                return "Yes";
                            return "No";
                        }
                    }
                   
                ],
                fixedColumns: true,
                data: userdata,
                rowsGroup: null,
                ordering:false
            });

        } else {
            dTable.clear().rows.add(userdata).draw();

        }

    }
}