var dtpDTFomat = "m/d/Y";
var dtpDTFomat1 = "d-m-Y";
var dtpDMYFomat = "d-m-Y";
var dtpTimeFomat = "h:m:t";
var dtFormat = "mm/dd/yyyy";
var timeFormat = "HH:mm tt";
var dttimeFormate = "m/d/y H:i:s";
var dtpTime = "H:i";

$(document).ready(function () {
    $(document).on('focus', ':input', function () {
        $(this).attr('autocomplete', 'off');
    });
});

function num_only(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        Message.Warning("Only numbers allowed.");
        return false;
    }
    return true;
};


var EventManager = {

    DataTableCommonButton: function () {
        return '<button class="btn btn-primary btn-datatable-padding float-left dTableEdit" title="Click to edit"><i class="fas fa-edit"></i></button>' +
            '<button class="btn btn-danger btn-datatable-padding float-left dTableDelete ml-2" title="Click to delete"><i class="far fa-trash-alt"></i></button>';
    }

};

var dTableManager = {
    dTableSerialNumber: function ($dataTable) {
        $dataTable.on('order.dt search.dt', function () {
            $dataTable.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                cell.innerHTML = dTableManager.IndexColumn(i + 1);
            });
        }).draw();
    },
    IndexColumn: function (ind) {
        return '<div class="font-weight" style="vertical-align: middle;" align="center">' + ind + '</div>';
    },
    Border: function (selector, tblHight) {
        $(selector).parent().css({
            //'background': 'rgb(255 252 252) !important',
            'minHeight': tblHight + 'px',
            'borderTop': '1px solid #dbdbdb !important',
            'borderLeft': '1px solid #dbdbdb',
            'borderRight': '1px solid #dbdbdb',
            'borderBottom': '1px solid #dbdbdb'
        });
    }
};

var CommonManager = {

    TopName: function (val) {
        return '<div class="row"> <div class="col-md-3 col-md-offset-5" style="text-align: center;padding: 0;background: #fdfdfd;border: none;border-bottom: 2px solid #0095d7;padding-bottom: 2px;margin-top: 2px;border-bottom-right-radius: 7px;border-bottom-left-radius: 7px;font-weight: 500;"><span>' + val + '</span></div></div>';
    },

    TopTitle: function (val) {
        return '<div class="col-md-2 col-md-offset-5 mb20" style="padding: 2px;text-align:center;background-color: #f5f5f5;z-index:15;">'+
            '<span class="glyphicon glyphicon-chevron-right"></span> <span>'+val+'</span>'+
            '</div>';
    },
    PrintHtmlPage: function (width, height, htmlText) {
        if ($(".printIframe").length)
            $(".printIframe").remove();
        var iframeEl = $('<iframe class="printIframe" style="display:none;"></iframe>');
        $('body').append(iframeEl);
        var iframeW = iframeEl.contents().find('body');
        var iframeWindow = (iframeEl[0].contentWindow || iframeEl[0].contentDocument);
        htmlText = '<div style="margin:0 auto;width:' + width + ';height:' + height + ';">' + htmlText + '</div>';
        iframeW.append(htmlText);
        iframeWindow.print();
      
    }

};