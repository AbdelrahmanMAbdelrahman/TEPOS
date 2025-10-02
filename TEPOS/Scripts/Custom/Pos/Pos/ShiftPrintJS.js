function PosShiftPrintByPosPrinter(jsonData) {
    debugger;
    var printWindow =
        '<div style="width:100%;float:left;">' +
        '<style type="text/css">' +
        '*{' +
        'margin: 0;' +
        'font-family: helvetica;' +
        '}' +
        '.fL{ font-size: 16px; }' +
        '.fM{ font-size: 12px; }' +
        '.fs{ font-size: 10px; }' +
        '.flL{ float:left; }' +
        '.flR{ float:right; }' +
        '.tl{ text-align: left; }' +
        '.tc{ text-align: center; }' +
        '.tr{ text-align: right; }' +
        '.fwb{ font-weight: bold; }' +
        '.mt5{ margin-top: 5px; }' +
        '.mtb10{ margin: 10px 0; }' +
        '.w50{ width: 50%; }' +
        '.w100{ width: 100%; }' +
        '#wrap{ width:285px; line-height: 1; padding: 20px 10px; margin: 0 auto; }' +
        '.bdrB{ border-bottom: 2px dashed #000; }' +
        '</style>' +
        '<div class="w100">' +
        '<div class="flL w100 fs"><b>Shift Code:</b> ' + jsonData.Id + ' </div>' +
        '<div class="flL w100 fs"><b>Start Time:</b> ' + jsonData.StartTime + ' </div>' +
        '<div class="flL w100 fs"><b>End Time:</b> ' + jsonData.EndTime + ' </div>' +
        '<div class="flL w100 fs"><b>Branch Name:</b> ' + jsonData.BranchName + ' </div>' +
        '<div class="flL w100 fs"><b>User Name:</b> ' + jsonData.UserName + ' </div>' +
        '<table class="w100 fs">' +
        '<tbody>' +
        '<div class="flL w100 bdrB mtb10"></div>' +
        '<tr><td colspan="2" class="fwb">Cash Sales:</td><td colspan="3">' + jsonData.TotalSalesCash + '</td></tr>' +
        '<tr><td colspan="2" class="fwb">Visa Sales:</td><td colspan="3">' + jsonData.TotalSalesVisa + '</td></tr>' +
        '<tr><td colspan="2" class="fwb">Bank Sales:</td><td colspan="3">' + jsonData.TotalSalesBank + '</td></tr>' +
        '<tr><td  class="flR w100 bdrB mtb10"></td></tr>' +
        '<tr><td colspan="2" class="fwb">Total Sales:</td><td colspan="3">' + jsonData.TotalSales + '</td></tr>' +
        '<tr><td  class="flR w100 bdrB mtb10"></td></tr>' +
        '<tr><td colspan="2" class="fwb">Cash Sales Cancelinv:</td><td colspan="3">' + jsonData.TotalSalesCashCancel + '</td></tr>' +
        '<tr><td colspan="2" class="fwb">Visa Sales Cancelinv:</td><td colspan="3">' + jsonData.TotalSalesVisaCancel + '</td></tr>' +
        '<tr><td colspan="2" class="fwb">Bank Sales Cancelinv:</td><td colspan="3">' + jsonData.TotalSalesBankCancel + '</td></tr>' +
        '<tr><td  class="flR w100 bdrB mtb10"></td></tr>' +
        '<tr><td colspan="2" class="fwb">Total Sales Cancelinv:</td><td colspan="3">' + jsonData.TotalSalesCancel + '</td></tr>' +
        '<tr><td  class="flR w100 bdrB mtb10"></td></tr>' +
        //'<tr><td colspan="2" class="fwb">Sum Total Sales:</td><td colspan="3">' + jsonData.SumTotalSales + '</td></tr>' +
        '<tr><td colspan="2" class="fwb">Cash Balance:</td><td colspan="3">' + jsonData.ClosingBallanceCash + '</td></tr>' +
        '<tr><td colspan="2" class="fwb">Visa Balance:</td><td colspan="3">' + jsonData.ClosingBallanceVisa + '</td></tr>' +
        '<tr><td colspan="2" class="fwb">Bank Balance:</td><td colspan="3">' + jsonData.ClosingBallanceBank + '</td></tr>' +
        '<tr><td  class="flR w100 bdrB mtb10"></td></tr>'+
        '<tr><td colspan="2" class="fwb">Total Closing Balance:</td><td colspan="3">' + jsonData.TotalClosingBallance + '</td></tr>' +
        '<tr><td  class="flR w100 bdrB mtb10"></td></tr>' +
        '<tr><td colspan="2" class="fwb">Difference Cash:</td><td colspan="3">' + jsonData.diffCash + '</td></tr>' +
        '<tr><td colspan="2" class="fwb">Difference Visa:</td><td colspan="3">' + jsonData.diffVisa + '</td></tr>' +
        '<tr><td colspan="2" class="fwb">Difference Bank:</td><td colspan="3">' + jsonData.diffBank + '</td></tr>' +
        '<tr><td  class="flR w100 bdrB mtb10"></td></tr>' +
        '<tr><td colspan="2" class="fwb">Total Difference:</td><td colspan="3">' + jsonData.TotalDiff + '</td></tr>' +
        '<tr><td  class="flR w100 bdrB mtb10"></td></tr>' +
        '<tr><td colspan="2" class="fwb">Remaining Cash:</td><td colspan="3">' + jsonData.RemainingCash + '</td></tr>' +
        '<tr><td colspan="2" class="fwb">Opening Balance:</td><td colspan="3">' + jsonData.OpeningBallance + '</td></tr>' +
        '<tr><td  class="flR w100 bdrB mtb10"></td></tr>' +
        '<tr><td colspan="2" class="fwb">Difference Balance:</td><td colspan="3">' + jsonData.diffBallance + '</td></tr>' +
        '<tr><td  class="flR w100 bdrB mtb10"></td></tr>' +
        '<tr><td colspan="2" class="fwb">Cache In</td><td colspan="3">' + jsonData.CacheIn + '</td></tr>' +
        '<tr><td colspan="2" class="fwb">Cache In Description</td><td colspan="3">' + jsonData.CacheInDescription + '</td></tr>' +
        '<tr><td colspan="2" class="fwb">Cache Out</td><td colspan="3">' + jsonData.CacheOut + '</td></tr>' +
        
        '<tr><td colspan="2" class="fwb">Cache Out Description</td><td colspan="3">' + jsonData.CacheOutDescription + '</td></tr>' +
        '<tr><td  class="flR w100 bdrB mtb10"></td></tr>' +
        '<tr><td colspan="2" class="fwb">Closing Comments:</td><td colspan="3">' + jsonData.closingComments + '</td></tr>' +
        '<tr><td colspan="2" class="fwb">Notes:</td><td colspan="3">' + jsonData.Notes + '</td></tr>' +
        '<tr><td  class="flR w100 bdrB mtb10"></td></tr>' +
        '<tr><td colspan="2" class="fwb">Total All Shift:</td><td colspan="3">' + jsonData.TotalShiftAll + '</td></tr>' +
        '<tr><td colspan="2" class="fwb">Shift Status:</td><td colspan="3">' + jsonData.shiftSatus + '</td></tr>' +
        
        '</tbody>' +
        '</table>' +
        '<div class="flL w100 bdrB mtb10"></div>' +
        '<div class="flL w100 tc fs fwb mt5">Printed Date & Time: ' + new Date().toLocaleString() + '</div>' +
        '</div>';
    CommonManager.PrintHtmlPage("285px", "0", printWindow);
}
