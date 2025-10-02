var products = null;
var productOption = "";
var branch = "";
var cusClass = "";
var dTable = null;
$(document).ready(function () {
    $("#topTitle").html(CommonManager.TopTitle("Scheme"));

    $.ajax({
        url: '/PosDropDown/GetProducts',        
        dataType: 'JSON',
        async : false,
    }).done(function (jsonData) {
        products = jsonData;
        productOption = '<option value="">Select Product<option>';
        $.each(products, function (ind, val) {            
            productOption += '<option value="' + val.Id + '" data-code="'+val.Code+'">' + val.Name + '<option>';
        });

        $('.prdDD').html(productOption);        
    });

    $.ajax({
        url: '/PosDropDown/GetBranch',
        async: false,
        dataType: 'JSON'
    }).done(function (jsonData) {        
        branch = '';
        $.each(jsonData, function (ind, val) {
            branch += '<option value="' + val.Id + '">' + val.Name + '<option>';
        });

        $('#posBranch').html(branch);
    });

    $.ajax({
        url: '/PosDropDown/GetCustomerClass',
        async: false,
        dataType: 'JSON'
    }).done(function (jsonData) {        
        cusClass = '';
        $.each(jsonData, function (ind, val) {
            cusClass += '<option value="' + val.Id + '">' + val.Name + '<option>';
        });

        $('#cusClass').html(cusClass);
    });

    $('#schemeType').change(function () {
        var tType = $(this).find('option:selected').text();
        $('#schemeTypeL').html(tType);
        $('.schemeTypeI').attr('placeholder', tType);
    });

    $('.chosen').chosen();

    $('#btnClear').click(function () {
        Manager.FormReset();
    });

    Manager.GetDataForTable(0);

    dTable.on('order.dt search.dt', function () {
        dTable.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = dTableManager.indexColumn(i + 1);
        });
    }).draw();
});

$(document).on('change', '.prdDD', function () {
    var $item = $(this);
    $.ajax({
        url: '/PosDropDown/GetBatchByProductCode',
        data: { productCode: $item.find('option:selected').data('code') },
        async: false,
        dataType: 'JSON'
    }).done(function (jsonData) {
        products = jsonData;
        var productsBatchI = '<option value="">Batch/Size<option>';
        $.each(products, function (ind, val) {
            productsBatchI += '<option value="' + val.Id + '">' + val.Name + '<option>';
        });

        $item.parent().next().find('.prdDDB').html(productsBatchI);
        $item.parent().next().find('.prdDDB').chosen('destroy').chosen();
    });
});

$(document).on('click', '.productDivA', function () {
    $('#productDiv').append(
                            '<div class="row productDiv form-horizontal">' +
                                '<div class="col-md-10">'+                                      
                                    '<div class="w50 flL">'+
                                        '<select name="schemeType" class="form-control sPrd prdDD chosen">' +
                                            productOption +
                                        '</select>'+
                                    '</div>'+
                                    '<div class="w50 flL">'+
                                        '<select name="schemeType" class="form-control sPrdB prdDDB chosen">' +
                                            '<option value="">Batch/Size</option>'+                                            
                                        '</select>'+
                                    '</div>'+                                       
                                '</div>'+
                                '<div class="col-md-2" style="padding:0">'+
                                    '<span class="text-success far fa-plus-square cp productDivA"></span>'+
                                    '<span class="text-danger ml-3 far fa-minus-square cp productDivR"></span>'+
                                '</div>'+
                            '</div>'
                        );
    $('#productDiv .chosen').chosen();
});

$(document).on('click', '.productDivR', function () {
    if ($('#productDiv').children().length > 1)
        $(this).parents('.productDiv').remove();
});

$(document).on('click', '.slabQAPrd', function () {
    console.log($(this).parents('tr').index());
    $('#slabQAPrdModal').attr('data-row-index', $(this).parents('tr').index());
    $('#slabQAPrdModalBody').html(
                                '<div class="row form-horizontal">' +
                                    '<div class="col-md-12">' +
                                        '<div class="form-group">' +
                                            '<label class="col-md-4 control-label">' +
                                                'Availability' +
                                                '<span class="required">*</span>' +
                                            '</label>' +
                                            '<div class="col-md-4">' +
                                                '<select id="availability" class="form-control chosen">' +
                                                    '<option value="0">AND</option>' +
                                                    '<option value="1">OR</option>' +
                                                '</select>' +
                                            '</div>' +
                                        '</div>' +
                                    '</div>' +
                                '</div>' +
                                '<div class="row form-horizontal">'+
                                    '<div class="col-md-8">'+
                                        '<div class="form-group">'+
                                            '<label class="col-md-12 control-label itemLbl">'+
                                                'Product'+
                                                '<span class="required">*</span>'+
                                            '</label>'+                                            
                                        '</div>'+
                                    '</div>'+
                                    '<div class="col-md-4">'+
                                        '<div class="form-group">'+
                                            '<label class="col-md-12 control-label itemLbl">'+
                                               'Qty'+
                                                '<span class="required">*</span>'+
                                           '</label>'+                                            
                                        '</div>'+
                                    '</div>'+                                    
                                '</div>'+
                                '<div class="row form-horizontal">' +
                                        '<div class="col-md-8">' +
                                            '<div class="form-group">' +
                                                '<div class="col-md-12">' +
                                                    '<div class="w50 flL">' +
                                                       '<select name="schemeType" class="form-control fPrd prdDD chosen">' +
                                                            productOption +
                                                        '</select>' +
                                                    '</div>' +
                                                    '<div class="w50 flL">' +
                                                        '<select name="schemeType" class="form-control fPrdB prdDDB chosen">' +
                                                            '<option value="">Batch/Size</option>' +                                                            
                                                        '</select>' +
                                                    '</div>' +
                                                '</div>' +
                                            '</div>' +
                                        '</div>' +
                                        '<div class="col-md-4">' +
                                            '<div class="form-group">' +
                                                '<div class="col-md-9">' +
                                                    '<input type="number" class="form-control fPrdQ input-sm" placeholder="Qty" />' +
                                                '</div>' +
                                                '<div class="col-md-3" style="padding:0">' +
                                                    '<span class="text-success far fa-plus-square cp slabQAPrdModalPrdA"></span> ' +
                                                    '<span class="text-danger ml-3 far fa-minus-square cp slabQAPrdModalPrdR"></span>' +
                                                '</div>' +
                                            '</div>' +
                                        '</div>' +                                        
                                    '</div>'
                            );
    if ($(this).attr('data-item') != undefined && $(this).attr('data-item') != "") {
        var freeItem = JSON.parse($(this).attr('data-item'));
        var items = "";
        $.each(freeItem.items, function (ind, valV) {
            items += '<tr>'+
                        '<td>' + valV.product + '</td>' +
                        '<td>' + valV.productBatch + '</td>'+
                        '<td>' + valV.qty + '</td>' +
                    '</tr>'
        });
        $('#slabQAPrdModalBody').prepend(
                                        '<div class="row">' +
                                            '<div class="col-md-12">' +                                              
                                               '<p style="font-size:16px;"><b>Availability:</b> '+(freeItem.availability == 0 ? 'AND' : 'OR') +'</p> ' +
                                            '</div>' +
                                            '<div class="col-md-12">' +
                                                '<table class="table table-condensed">' +
                                                    '<thead>' +
                                                        '<tr>'+
                                                            '<th>Product</th>' +
                                                            '<th>Batch/Size</th>' +
                                                            '<th>QTY</th>' +
                                                        '</tr>' +                                                        
                                                    '</thead>' +
                                                    '<tbody>' +
                                                        items+
                                                    '</tbody>'+
                                                '</table>' +
                                                 '<hr/>' +
                                            '</div>'+
                                        '</div>'
                                    );

    }
    
    $('#slabQAPrdModal').modal('show');
    $('#slabQAPrdModalBody .chosen').chosen();
});

$(document).on('click', '.slabQAPrdA', function () {
    $('#slabQA').append(
                        '<tr>'+
                            '<td>'+
                                '<input type="number" class="form-control input-sm PurQtyOrAmt schemeTypeI" placeholder="Quantity" required value="0" />' +
                            '</td>'+
                            '<td>'+
                                '<input type="number" class="form-control input-sm Discount" placeholder="Discount" value="0"/>' +
                            '</td>'+
                            '<td>'+
                                '<input type="number" class="form-control input-sm FlatAmt" placeholder="Flat Amoumt" value="0"/>' +
                            '</td>'+
                            '<td>'+
                                '<span class="badge slabQAPrdCount" data-item="">0</span> <span class="label label-primary cp slabQAPrd">Product +-</span>'+
                            '</td>'+
                           ' <td>'+
                                '<span class="text-success far fa-plus-square cp slabQAPrdA"></span>'+
                                '<span class="text-danger ml-3 far fa-minus-square cp slabQAPrdR"></span>'+
                            '</td>'+
                        '</tr>'
                        );
});

$(document).on('click', '.slabQAPrdR', function () {
    if ($('#slabQA').children().length > 1)
        $(this).parents('tr').remove();
});

$(document).on('click', '.slabQAPrdModalPrdA', function () {
    $('#slabQAPrdModalBody').append(
                                    '<div class="row form-horizontal">' +
                                        '<div class="col-md-8">' +
                                            '<div class="form-group">' +
                                                '<div class="col-md-12">' +
                                                    '<div class="w50 flL">' +
                                                       '<select name="fPrdIP" class="form-control fPrd prdDD chosen">' +
                                                            productOption +
                                                        '</select>' +
                                                    '</div>' +
                                                    '<div class="w50 flL">' +
                                                        '<select name="fPrdIPB" class="form-control fPrdB prdDDB chosen">' +
                                                            '<option value="">Batch/Size</option>' +
                                                        '</select>' +
                                                    '</div>' +
                                                '</div>' +
                                            '</div>' +
                                        '</div>' +
                                        '<div class="col-md-4">' +
                                            '<div class="form-group">' +
                                                '<div class="col-md-9">' +
                                                    '<input type="number" class="fPrdQ form-control input-sm" placeholder="Qty" />' +
                                                '</div>' +
                                                '<div class="col-md-3" style="padding:0">' +
                                                    '<span class="text-success far fa-plus-square cp slabQAPrdModalPrdA"></span>' +
                                                    '<span class="text-danger ml-3 far fa-minus-square cp slabQAPrdModalPrdR"></span>' +
                                                '</div>' +
                                            '</div>' +
                                        '</div>' +
                                    '</div>'
                                    );
    $('#slabQAPrdModalBody .chosen').chosen();
});

$(document).on('click', '.slabQAPrdModalPrdR', function () {
    if ($('#slabQAPrdModalBody').children().length > 3)
        $(this).parents('.form-horizontal').remove();
});

$(document).on('click', '#btnSave', function () {
    var saveData = new Object();
    saveData.Description = $('#description').val();
    saveData.IsCombiScheme = $('#completaion').val();
    saveData.SchemeType = $('#schemeType').val();
    saveData.DateFrom = $('#validFrom').val();
    saveData.DateTo = $('#validTo').val();
    saveData.Status = $('#status').val();
    saveData.PosBranch = $('#posBranch').val();
    saveData.CusClass = $('#cusClass').val();
    saveData.PosProducts = [];
    saveData.PosSchemeSlab = [];

    $.each($('.sPrd'), function (indP, valP) {
        saveData.PosProducts.push({
            ProductId: $(valP).val(),
            ProductBatchId: $('.sPrdB').eq(indP).val(),
        });
    });

    $.each($('#slabQA tr'), function (indS, valS) {
        saveData.PosSchemeSlab.push({
            PurQtyOrAmt: $(valS).find('.PurQtyOrAmt').val(),
            DiscountPer: $(valS).find('.Discount').val(),
            FlatAmt: $(valS).find('.FlatAmt').val(),
            Products: $(valS).find('.slabQAPrdCount').data('item')
        });
    });

    Manager.Save(saveData);

});

function calFreeItem() {    
    var freeItem = [];
    var freeItemV = new Object();

    freeItemV.availability = parseInt($('#availability').val());
    freeItemV.items = [];

    $.each($('.fPrd'), function (ind, valF) {
        freeItem.push({
            PosProductId: $(valF).val(),
            PosProductBatchId: $('.fPrdB').eq(ind).val(),
            Qty: $('.fPrdQ').eq(ind).val(),
            PosUomMasterId: 1,
            Availability: parseInt($('#availability').val())
        });
        freeItemV.items.push({
            product: $(valF).find('option:selected').text(),
            productBatch: $('.fPrdB').eq(ind).find('option:selected').text(),
            qty: $('.fPrdQ').eq(ind).val()
        });
    });

    $('#slabQA tr').eq($('#slabQAPrdModal').attr('data-row-index')).find('.slabQAPrdCount').html(freeItem.length);
    $('#slabQA tr').eq($('#slabQAPrdModal').attr('data-row-index')).find('.slabQAPrdCount').attr('data-item', JSON.stringify(freeItem));
    $('#slabQA tr').eq($('#slabQAPrdModal').attr('data-row-index')).find('.slabQAPrd').attr('data-item', JSON.stringify(freeItemV));
}

$(document).on('click', '.dTableEdit', function () {
    var $item = $(this).parents('tr');
    var row = dTable.row($item).data();
    Manager.FormReset();
    //debugger;   
    
    $.get("/PosScheme/GetSchemeById?schemeId=" + row.Id)
        .done(function (jsonData) {
            if (jsonData == null) {
                Message.Error("notFound");
            } else {
                $('#btnEdit').attr("data-id", row.Id);
                $('#schemeCode').val(jsonData.SchemeCode);
                $('#description').val(jsonData.Description);
                $('#completaion').val(jsonData.IsCombiScheme ? "True" : "False");
                $('#schemeType').val(jsonData.SchemeType);
                $('#validFrom').val(JsManager.ChangeToSQLDateTimeFormatMMddyyyy(jsonData.DateFrom).substring(0, 10));
                $('#validTo').val(JsManager.ChangeToSQLDateTimeFormatMMddyyyy(jsonData.DateTo).substring(0, 10));
                $('#status').val(jsonData.Status ? "True" : "False");
                $('#posBranch').val(jsonData.PosBranch);
                $('#cusClass').val(jsonData.CusClass);                

                $.each(jsonData.PosProducts, function (indP, valP) {
                    if (indP > 0) {
                        $('#productDiv').append(
                            '<div class="row productDiv form-horizontal">' +
                                '<div class="col-md-10">' +
                                    '<div class="w50 flL">' +
                                        '<select name="schemeType" class="form-control sPrd prdDD chosen">' +
                                            productOption +
                                        '</select>' +
                                    '</div>' +
                                    '<div class="w50 flL">' +
                                        '<select name="schemeType" class="form-control sPrdB prdDDB chosen">' +
                                            '<option value="">Batch/Size</option>' +
                                        '</select>' +
                                    '</div>' +
                                '</div>' +
                                '<div class="col-md-2" style="padding:0">' +
                                    '<span class="text-success far fa-minus-square cp productDivA"></span>' +
                                    '<span class="text-danger ml-3 far fa-plus-square cp productDivR"></span>' +
                                '</div>' +
                            '</div>'
                        );
                        $('.sPrd').eq(indP).val(valP.ProductId).trigger("change");
                        $('.sPrdB').eq(indP).val(valP.ProductBatchId);
                    } else {
                        $('.sPrd').eq(0).val(valP.ProductId).trigger("change");
                        $('.sPrdB').eq(0).val(valP.ProductBatchId);
                    }                    
                });

                $.each(jsonData.PosSchemeSlab, function (indS, valS) {
                    if (indS > 0) {
                        $('#slabQA').append(
                       '<tr>' +
                           '<td>' +
                               '<input type="number" class="form-control input-sm PurQtyOrAmt schemeTypeI" placeholder="Quantity" required value="0"/>' +
                           '</td>' +
                           '<td>' +
                               '<input type="number" class="form-control input-sm Discount" placeholder="Discount" value="0"/>' +
                           '</td>' +
                           '<td>' +
                               '<input type="number" class="form-control input-sm FlatAmt" placeholder="Flat Amoumt" value="0"/>' +
                           '</td>' +
                           '<td>' +
                               '<span class="badge slabQAPrdCount" data-item="">0</span> <span class="label label-primary cp slabQAPrd">Product +-</span>' +
                           '</td>' +
                          ' <td>' +
                               '<span class="text-success far fa-plus-square cp slabQAPrdA"></span>' +
                               '<span class="text-danger ml-3 far fa-minus-square cp slabQAPrdR"></span>' +
                           '</td>' +
                       '</tr>'
                       );

                        $('.PurQtyOrAmt').eq(indS).val(valS.PurQtyOrAmt);
                        $('.Discount').eq(indS).val(valS.DiscountPer);
                        $('.FlatAmt').eq(indS).val(valS.FlatAmt);
                        $('.slabQAPrdCount').eq(indS).attr('data-item', JSON.stringify(valS.Products)).html(valS.Products.length);
                        var valSPJ = { "availability": "", "items": [] };
                        $.each(valS.Products, function (indSP, valSP) {
                            valSPJ.availability = valSP.Availability;
                            valSPJ.items.push({
                                "product": valSP.PosProductName,
                                "productBatch": valSP.PosProductBatchName,
                                "qty": valSP.Qty
                            });
                        });
                        if (valSPJ.items.length > 0)
                            $('.slabQAPrd').eq(indS).attr('data-item', JSON.stringify(valSPJ));
                        else
                            $('.slabQAPrd').eq(indS).removeAttr('data-item');
                    } else {
                        $('.PurQtyOrAmt').eq(0).val(valS.PurQtyOrAmt);
                        $('.Discount').eq(0).val(valS.DiscountPer);
                        $('.FlatAmt').eq(0).val(valS.FlatAmt);
                        $('.slabQAPrdCount').eq(0).attr('data-item', JSON.stringify(valS.Products)).html(valS.Products.length);
                        var valSPJ = {"availability" : "", "items" : []};
                        $.each(valS.Products, function (indSP, valSP) {
                            valSPJ.availability = valSP.Availability;
                            valSPJ.items.push({
                                "product": valSP.PosProductName,
                                "productBatch": valSP.PosProductBatchName,
                                "qty": valSP.Qty
                            });
                        });
                        if (valSPJ.items.length > 0)
                            $('.slabQAPrd').eq(0).attr('data-item', JSON.stringify(valSPJ));
                        else
                            $('.slabQAPrd').eq(0).removeAttr('data-item');
                    }                    
                });

                $('#completaion').trigger("change");
                $('.chosen').chosen('destroy').chosen();
                $('html, body').animate({scrollTop : 0}, 500);
            }
        })
        .fail(function (xhr, status, err) {
            Message.Error("notFound");
        });
    
});

$(document).on('click', '#btnEdit', function () {
    if ($(this).attr('data-id') == undefined || $(this).attr('data-id') < 1) {
        Message.Warning("Please select an item to edit first!!");
        return 0;
    }
    var saveData = new Object();
    saveData.Id = $(this).attr('data-id');
    saveData.SchemeCode = $('#schemeCode').val();    
    saveData.Description = $('#description').val();
    saveData.IsCombiScheme = $('#completaion').val();
    saveData.SchemeType = $('#schemeType').val();
    saveData.DateFrom = $('#validFrom').val();
    saveData.DateTo = $('#validTo').val();
    saveData.Status = $('#status').val();
    saveData.PosBranch = $('#posBranch').val();
    saveData.CusClass = $('#cusClass').val();
    saveData.PosProducts = [];
    saveData.PosSchemeSlab = [];

    $.each($('.sPrd'), function (indP, valP) {
        saveData.PosProducts.push({
            ProductId: $(valP).val(),
            ProductBatchId: $('.sPrdB').eq(indP).val(),
        });
    });

    $.each($('#slabQA tr'), function (indS, valS) {
        saveData.PosSchemeSlab.push({
            PurQtyOrAmt: $(valS).find('.PurQtyOrAmt').val(),
            DiscountPer: $(valS).find('.Discount').val(),
            FlatAmt: $(valS).find('.FlatAmt').val(),
            Products: $(valS).find('.slabQAPrdCount').data('item')
        });
    });
    
    Manager.Update(saveData);

});

$(document).on('click', '.dTableDelete', function () {
    var $item = $(this).parents('tr');
    var row = dTable.row($item).data();
    Manager.Delete(row.Id);
});


var Manager = {
    FormReset: function() {
        $('#editId').attr("data-id", 0);
        $('input[type=text]').val('');
        $('input[type=number]').val('');
        $('select').find('option:gt(0)').removeAttr('selected');
        $('select').find('option:eq(0)').attr('selected', 'selected');
        $('#productDiv > div:gt(0)').remove();
        $('#slabQA').html(
            '<tr>' +
            '<td>' +
            '<input type="number" class="form-control input-sm PurQtyOrAmt schemeTypeI" placeholder="Quantity" value="0" required />' +
            '</td>' +
            '<td>' +
            '<input type="number" class="form-control input-sm Discount" placeholder="Discount" value="0"/>' +
            '</td>' +
            '<td>' +
            '<input type="number" class="form-control input-sm FlatAmt" placeholder="Flat Amoumt" value="0"/>' +
            '</td>' +
            '<td>' +
            '<span class="badge slabQAPrdCount" data-item="">0</span> <span class="label label-primary cp slabQAPrd">Product +-</span>' +
            '</td>' +
            ' <td>' +
            '<span class="text-success far fa-plus-square cp slabQAPrdA"></span>' +
            '<span class="text-danger ml-3 far fa-minus-square cp slabQAPrdR"></span>' +
            '</td>' +
            '</tr>'
        );
        $('.chosen').chosen('destroy').chosen();
    },
    Validate: function(saveData) {
        var errorText = "";
        if (saveData.Description.length < 3)
            errorText += "Description is required<br>";
        if (saveData.DateFrom.length < 10)
            errorText += "Valid From is required<br>";
        if (saveData.DateTo.length < 10)
            errorText += "Valid To is required<br>";
        if (saveData.PosBranch == null || saveData.PosBranch.length < 1)
            errorText += "Branch is required<br>";
        if (saveData.CusClass == null || saveData.CusClass.length < 1)
            errorText += "Customer Class is required<br>";
        if (saveData.PosProducts.length < 1 ||
            saveData.PosProducts[0].ProductId == null ||
            saveData.PosProducts[0].ProductId.length < 1)
            errorText += "Product is required<br>";
        else if (saveData.PosProducts[0].ProductBatchId == null || saveData.PosProducts[0].ProductBatchId.length < 1)
            errorText += "Product Batch/Size is required<br>";

        if (errorText == "")
            return true;

        Message.Warning(errorText);
        return false;
    },
    Save: function(saveData) {

        if (Message.Prompt("Do you want to save this scheme?")) {
            var serviceURL = "/PosScheme/Save/";
            JsManager.SendJson(serviceURL, { 'saveData': JSON.stringify(saveData) }, onSuccess, onFailed);

            function onSuccess(jsonData) {
                if (jsonData == "0") {
                    Message.Error("save");
                } else {
                    Message.Success("save");
                    Manager.FormReset();
                    Manager.GetDataForTable(1);
                }
            }

            function onFailed(xhr, status, err) {
                Message.Exception(xhr);
            }
        }
    },
    Update: function(saveData) {

        if (Message.Prompt("Do you want to update this scheme?")) {

            var serviceURL = "/PosScheme/Update/";
            JsManager.SendJson(serviceURL, { 'saveData': JSON.stringify(saveData) }, onSuccess, onFailed);

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
    Delete: function(schemeId) {

        if (Message.Prompt("Do you want to delete this scheme")) {
            var jsonParam = { schemeId: schemeId };
            var serviceURL = "/PosScheme/DeleteScheme/";
            JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

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
        }

    },

    GetDataForTable: function(ref) {
        var jsonParam = '';
        var serviceURL = "/PosScheme/GetScheme/";
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
                            columns: [2, 3, 4, 5, 6, 7, 8]
                        },
                        title: 'Scheme List'
                    },
                    {
                        text: '<i class="fas fa-print"></i> Print',
                        className: 'btn btn-sm',
                        extend: 'print',
                        exportOptions: {
                            columns: [2, 3, 4, 5, 6, 7, 8]
                        },
                        title: 'Scheme List'
                    },
                    {
                        text: '<i class="far fa-file-excel"></i> Excel',
                        className: 'btn btn-sm',
                        extend: 'excelHtml5',
                        exportOptions: {
                            columns: [2, 3, 4, 5, 6, 7, 8]
                        },
                        title: 'Scheme List'
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
                        width: 15,
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
                    { data: 'SchemeCode', name: 'SchemeCode', title: 'Scheme Code', align: 'center' },
                    { data: 'Description', name: 'Description', title: 'Description' },
                    {
                        data: 'IsCombiScheme',
                        name: 'IsCombiScheme',
                        title: 'Combination',
                        render: function(data, row, ind) {
                            return (data) ? 'Yes' : 'No';
                        }
                    },
                    {
                        data: 'SchemeType',
                        name: 'SchemeType',
                        title: 'Scheme Type',
                        render: function(data, row, ind) {
                            return (data == 1) ? 'Quantity' : 'Amount';
                        }
                    },
                    {
                        data: 'DateFrom',
                        name: 'DateFrom',
                        title: 'Valid From',
                        width: 100,
                        render: function(data, row, ind) {
                            return JsManager.ChangeToSQLDateTimeFormatMMddyyyy(data).substring(0, 10);
                        }
                    },
                    {
                        data: 'DateTo',
                        name: 'DateTo',
                        title: 'Valid To',
                        width: 100,
                        render: function(data, row, ind) {
                            return JsManager.ChangeToSQLDateTimeFormatMMddyyyy(data).substring(0, 10);
                        }
                    },
                    {
                        data: 'Status',
                        name: 'Status',
                        title: 'Status',
                        render: function(data, row, ind) {
                            return (data) ? 'Enable' : 'Disable';
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

    }
}