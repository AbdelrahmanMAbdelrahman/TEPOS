
var dTable = null;
var _customerId = null;

$(document).ready(function () {
    Manager.GetCloseShiftData();
    Manager.Calculations();


    $("#btnOpenShift").click(function () {
        Manager.OpenShift();
        document.getElementById("ShiftForm").reset();
    });
    $("#btnCloseShift").click(function () {
        Manager.CloseShift();

    });
   

    //$(document).on('change', '#ClosingBallanceCash', function (key) {
    //    Manager.Calculations();
    //});
    //$(document).on('change', '#ClosingBallanceVisa', function (key) {
    //    Manager.Calculations();
    //});
    //$(document).on('change', '#ClosingBallanceBank', function (key) {
    //    Manager.Calculations();
    //});
    //$(document).on('change', '#RemainingCash', function (key) {
    //    Manager.Calculations();
    //});
});




var Manager = {
    GetCloseShiftData: function () {

        var serviceURL = "/PosSetting/GetCloseShiftData/";
        JsManager.SendJson(serviceURL, '', onSuccess, onFailed);

        function onSuccess(jsonData) {
            if (jsonData.error == "you not have shift To close") {
                Message.Error("you not have shift To close");
            }
            //else if (jsonData.PosShift.error == "You Can not Close Shift there are Hold Invoices") {
            //    Message.Error("You Can not Close Shift there are Hold Invoices");
            //}
            else if (jsonData.error == "No Invoices Is Created in Shift") {
                Message.Warning("No Invoices Is Created in Shift");
                $("#StartTime").val(JsManager.ChangeDateFormat(jsonData.PosShift.StartTime));
                $("#EndTime").val(JsManager.ChangeDateFormat(jsonData.PosShift.EndTime));
                //$("#OpeningBallance").val(jsonData.PosShift.OpeningBallance);
                // Manager.CloseShift();
            }
            else if (jsonData.error == "Must Enter Decimal Value") {
                Message.Error("Must Enter Decimal Value");

            }

            else {
                $("#txtShiftCode").text(jsonData.PosShift.Id);
                //$("#OpeningBallance").val(jsonData.PosShift.OpeningBallance);
                //$("#TotalSalesCash").val(jsonData.PosShift.TotalSalesCash);
                //$("#TotalSalesVisa").val(jsonData.PosShift.TotalSalesVisa);
                //$("#TotalSalesBank").val(jsonData.PosShift.TotalSalesBank);
                //$("#TotalSales").val(jsonData.PosShift.TotalSales);

                $("#StartTime").val(JsManager.ChangeDateFormat(jsonData.PosShift.StartTime));
                $("#EndTime").val(JsManager.ChangeDateFormat(jsonData.PosShift.EndTime));
               // $("#OpeningBallance").val(jsonData.PosShift.OpeningBallance);
                ////
                Manager.CalculatDate();
            }
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }


    },
    OpenShift: function () {

        var serviceURL = "/PosSetting/OpenShift/";
        JsManager.SendJson(serviceURL, '', onSuccess, onFailed);


        function onSuccess(jsonData) {
            if (jsonData.error == "you have shift not close") {
                Message.Error("you have shift not close");
            } else {
                $("#StartTime").val(jsonData.StartTime);
               // $("#OpeningBallance").val(jsonData.PosShift.OpeningBallance);
                Message.Success("save");

            }
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }

    },

    CloseShift: function () {
        debugger;
        var obj = [];
        obj.push({ 'ClosingBallanceCash': $("#ClosingBallanceCash").val() });
        obj.push({ 'ClosingBallanceVisa': $("#ClosingBallanceVisa").val() });
        obj.push({ 'ClosingBallanceBank': $("#ClosingBallanceBank").val() });
        obj.push({ 'RemainingCash': $("#RemainingCash").val() });
        if (Manager.validate(obj)) {
            if (Message.Prompt()) {
                var objData = new Object();
                var getCloseShiftDataUrl = "/PosSetting/GetCloseShiftData/";
                JsManager.SendJson(getCloseShiftDataUrl, '', onGetCloseDataSuccess, onGetCloseDataFailed);

                function onGetCloseDataSuccess(jsonData) {
                    debugger;
                    if (jsonData.error === "you not have shift To close") {
                        Message.Error("you not have shift To close");
                    } else if (jsonData.error === "No Invoices Is Created in Shift")    {
                        Message.Warning("No Invoices Is Created in Shift");
                        $("#StartTime").val(JsManager.ChangeDateFormat(jsonData.PosShift.StartTime));
                        $("#EndTime").val(JsManager.ChangeDateFormat(jsonData.PosShift.EndTime));
                      //  $("#OpeningBallance").val(jsonData.PosShift.OpeningBallance);
                    } else if (jsonData.error === "Must Enter Decimal Value") {
                        Message.Error("Must Enter Decimal Value");
                    } else {
                        $("#txtShiftCode").text(jsonData.PosShift.Id);
                        $("#StartTime").val(JsManager.ChangeDateFormat(jsonData.PosShift.StartTime));
                        $("#EndTime").val(JsManager.ChangeDateFormat(jsonData.PosShift.EndTime));
                        var ClosingBallanceCash = parseFloat($("#ClosingBallanceCash").val() || 0);
                        var ClosingBallanceVisa = parseFloat($("#ClosingBallanceVisa").val() || 0);
                        var ClosingBallanceBank = parseFloat($("#ClosingBallanceBank").val() || 0);
                        var RemainingCash = parseFloat($("#RemainingCash").val() || 0);
                        var CacheIn = parseFloat($("#CacheIn").val() || 0);
                         var CacheOut = parseFloat($("#CacheOut").val() || 0);
                        objData.RemainingCash = $("#RemainingCash").val();

                        if (typeof Manager.CalculatDate === "function") {
                            Manager.CalculatDate();
                        }
                        var OpeningBallance = parseFloat(jsonData.PosShift.OpeningBallance || 0);
                        var TotalSalesCash = parseFloat(jsonData.PosShift.TotalSalesCash || 0);
                        var TotalSalesVisa = parseFloat(jsonData.PosShift.TotalSalesVisa || 0);
                        var TotalSalesBank = parseFloat(jsonData.PosShift.TotalSalesBank || 0);
                        var TotalSales = parseFloat(jsonData.PosShift.TotalSales || 0);

                        var TotalSalesCashCancel = parseFloat(jsonData.PosShift.TotalSalesCashCancel || 0);
                        var TotalSalesVisaCancel = parseFloat(jsonData.PosShift.TotalSalesVisaCancel || 0);
                        var TotalSalesBankCancel = parseFloat(jsonData.PosShift.TotalSalesBankCancel || 0);
                        var TotalSalesCancel = parseFloat(jsonData.PosShift.TotalSalesCancel || 0);

                        var BranchName = jsonData.PosShift.PosBranch.Name;
                        var UserName = jsonData.UserName;
                        var diffCash = ClosingBallanceCash - TotalSalesCash;
                        var diffVisa = ClosingBallanceVisa-TotalSalesVisa  ;
                        var diffBank = ClosingBallanceBank- TotalSalesBank  ;
                        var diffBallance = OpeningBallance-RemainingCash  ;
                        var SumTotalSales = TotalSalesCash + TotalSalesVisa + TotalSalesBank;
                        var TotalClosingBallance = ClosingBallanceCash + ClosingBallanceVisa + ClosingBallanceBank;
                        var TotalDiff = diffCash + diffVisa + diffBank;
                       // var TotalShiftAll = diffBallance + TotalDiff + CacheIn - CacheOut;
                        var TotalShiftAll = ClosingBallanceCash + ClosingBallanceVisa + ClosingBallanceBank - TotalSalesCash - TotalSalesVisa - TotalSalesBank
                            + TotalSalesCashCancel + TotalSalesVisaCancel + TotalSalesBankCancel;

                        if (TotalShiftAll > 0) {
                            var shiftSatus = " يوجد زيادة ";
                        }
                        else if (TotalShiftAll < 0) {
                            var shiftSatus = "  يوجد عجز";
                        }
                        else if (TotalShiftAll == 0) {
                            var shiftSatus = "  لا يوجد عجز او زياده";
                        }

                        objData.TotalSalesCash = TotalSalesCash;
                        objData.TotalSalesVisa = TotalSalesVisa;
                        objData.TotalSalesBank = TotalSalesBank;
                        objData.OpeningBallance = OpeningBallance;
                        objData.TotalSales = TotalSales;

                        objData.TotalSalesCashCancel = TotalSalesCashCancel;
                        objData.TotalSalesVisaCancel = TotalSalesVisaCancel;
                        objData.TotalSalesBankCancel = TotalSalesBankCancel;
                        objData.TotalSalesCancel = TotalSalesCancel;
                        //objData.PosShift.PosBranch.BranchName = BranchName;
                        objData.UserName = UserName;
                        objData.BranchName = BranchName;
                        objData.shiftSatus = shiftSatus;

                        objData.diffCash = diffCash;
                        objData.diffVisa = diffVisa;
                        objData.diffBank = diffBank;
                        
                        objData.diffBallance = diffBallance;
                        objData.SumTotalSales = SumTotalSales;
                        objData.TotalClosingBallance = TotalClosingBallance;
                        objData.TotalDiff = TotalDiff;
                        objData.TotalShiftAll = TotalShiftAll;
                        objData.ClosingBallanceCash = $("#ClosingBallanceCash").val();
                        objData.ClosingBallanceVisa = $("#ClosingBallanceVisa").val();
                        objData.ClosingBallanceBank = $("#ClosingBallanceBank").val();
                        objData.RemainingCash = $("#RemainingCash").val();
                        objData.Notes = $("#Notes").val();
                        objData.closingComments = $("#closingComments").val();
                        objData.StartTime =$("#StartTime").val();
                        objData.EndTime = $("#EndTime").val();
                        objData.CacheIn = $("#CacheIn").val();
                        objData.CacheOut = $("#CacheOut").val();
                        objData.CacheOutDescription = $("#CacheOutDescription").val();
                        objData.CacheInDescription = $("#CacheInDescription").val();
                        objData.Id = $("#txtShiftCode").text();
                        

                        //objData.OpeningBallance = $("#OpeningBallance").val();
                        //console.log("objData:", objData);
                    //    jsonData.PosShift = objData;
                        }
                        //function onSuccess(objData) {
                        //    if (jsonData.PosShift.error == "You Can not Close Shift there are Hold Invoices") {
                        //        Message.Error("You Can not Close Shift there are Hold Invoices");
                        //    }
                        //    else if (jsonData.PosShift.error == "you not have shift To close") {
                        //        Message.Error("you not have shift To close");
                        //    } else
                        //        //  $("#StartTime").val(jsonData.PosShift.StartTime);
                        //        // $("#EndTime").val(jsonData.PosShift.EndTime);
                        //        console.log(objData);
                        //    Message.Success("save");

                        //    PosShiftPrintByPosPrinter(objData);
                        //}
                    var jsonParam = {
                        Shift: JSON.stringify(objData)
                    };
                    var serviceURL = "/PosSetting/CloseShift/";
                    JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

                    function onSuccess(jsonData2) {
                        if (jsonData2.error == "You Can not Close Shift there are Hold Invoices") {
                            Message.Error("You Can not Close Shift there are Hold Invoices");
                        }
                        else if (jsonData2.error == "you not have shift To close") {
                            Message.Error("you not have shift To close");
                        } else {
                            //  $("#StartTime").val(jsonData.PosShift.StartTime);
                            // $("#EndTime").val(jsonData.PosShift.EndTime);
                            // console.log(jsonData2);
                            Message.Success("save");

                            PosShiftPrintByPosPrinter(objData);
                            document.getElementById("ShiftForm").reset();
                            $("#txtShiftCode").text('');

                           // location.reload();

                        }
                    }
                    function onFailed(xhr, status, err) {
                        Message.Exception(xhr);
                    }
                   
                    
                }

                function onGetCloseDataFailed(xhr, status, err) {
                    Message.Exception(xhr);
                }
                

            }
        }
    },

    validate: function (obj) {
        if (obj.length > 0) {
            for (var object of obj) {
                for (var property in object) {
                    if (property.toString() != "Id") {
                        if (object[property] === "" || object[property] === null) {
                            Message.Warning(property.toString() + " is required.");
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        return false;

    },
    // sara whdan 8-12-2024
    Calculations: function () {       
        var serviceURL = "/PosSetting/GetCloseShiftData/";
        JsManager.SendJson(serviceURL, '', onSuccess, onFailed);

        function onSuccess(jsonData) {
            if (jsonData.error === "you not have shift To close") {
                Message.Error("you not have shift To close");
            } else if (jsonData.error === "No Invoices Is Created in Shift") {
                Message.Warning("No Invoices Is Created in Shift");
                $("#StartTime").val(JsManager.ChangeDateFormat(jsonData.PosShift.StartTime));
                $("#EndTime").val(JsManager.ChangeDateFormat(jsonData.PosShift.EndTime));
                $("#OpeningBallance").val(jsonData.PosShift.OpeningBallance);
            } else if (jsonData.PosShift.error === "Must Enter Decimal Value") {
                Message.Error("Must Enter Decimal Value");
            } else {
                $("#txtShiftCode").text(jsonData.PosShift.Id);
                $("#OpeningBallance").val(jsonData.PosShift.OpeningBallance);
                //$("#TotalSalesCash").val(jsonData.PosShift.TotalSalesCash);
                //$("#TotalSalesVisa").val(jsonData.PosShift.TotalSalesVisa);
                //$("#TotalSalesBank").val(jsonData.PosShift.TotalSalesBank);
                //$("#TotalSales").val(jsonData.PosShift.TotalSales);
                $("#StartTime").val(JsManager.ChangeDateFormat(jsonData.PosShift.StartTime));
                $("#EndTime").val(JsManager.ChangeDateFormat(jsonData.PosShift.EndTime));

                if (typeof Manager.CalculatDate === "function") {
                    Manager.CalculatDate();
                }

                //var TotalSalesCash = parseFloat(jsonData.PosShift.TotalSalesCash || 0);
                //var TotalSalesVisa = parseFloat(jsonData.PosShift.TotalSalesVisa || 0);
                //var TotalSalesBank = parseFloat(jsonData.PosShift.TotalSalesBank || 0);
                //var ClosingBallanceCash = parseFloat($("#ClosingBallanceCash").val() || 0);
                //var ClosingBallanceVisa = parseFloat($("#ClosingBallanceVisa").val() || 0);
                //var ClosingBallanceBank = parseFloat($("#ClosingBallanceBank").val() || 0);
                //var OpeningBallance = parseFloat(jsonData.PosShift.OpeningBallance || 0);
                //var RemainingCash = parseFloat($("#RemainingCash").val() || 0);

                //var diffCash = ClosingBallanceCash - TotalSalesCash;
                //var diffVisa = ClosingBallanceVisa-TotalSalesVisa  ;
                //var diffBank = ClosingBallanceBank-TotalSalesBank  ;
                //var diffBallance = RemainingCash- OpeningBallance ;

                //$("#diffBallance").val(diffBallance);
                //$("#diffSalesCash").val(diffCash);
                //$("#diffSalesVisa").val(diffVisa);
                //$("#diffSalesBank").val(diffBank);

                //var SumTotalSales = TotalSalesCash + TotalSalesVisa + TotalSalesBank;
                //var TotalClosingBallance = ClosingBallanceCash + ClosingBallanceVisa + ClosingBallanceBank;
                //var TotalDiff = diffCash + diffVisa + diffBank;

                //$("#SumTotalSales").val(SumTotalSales);
                //$("#TotalClosingBallance").val(TotalClosingBallance);
                //$("#TotalDifference").val(TotalDiff);

            }
}

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },






    CalculatDate: function () {
        var StartTime = $("#StartTime").val();
        var EndTime = $("#EndTime").val();

        let date1 = new Date(StartTime);
        let date2 = new Date(EndTime);

        // Calculating the time difference
        // of two dates
        let Difference_In_Time =
            date2.getTime() - date1.getTime();

        // Calculating the no. of days between
        // two dates
        let Difference_In_Days =
            Math.round
                (Difference_In_Time / (1000 * 3600));
        Difference_In_Days = Difference_In_Days + ' Hours';

        if (Difference_In_Days > 24) {
            Difference_In_Days = Difference_In_Days / 24;
            Difference_In_Days = Difference_In_Days + ' Days';
        }
        var ShiftDuration = Difference_In_Days;
        $("#ShiftDuration").val(ShiftDuration);
    },


}