
    $(document).ready(function() {
        Dashboard.Draw30DaysSalesWithDue();
        Dashboard.LastTwoMonthSalesBranchWise();
        Dashboard.BranchWiseLast30DaysSales();
    });


    var Dashboard = {
        BranchWiseLast30DaysSales: function() {
            var jsonParam = '';
            var serviceUrl = "/PosDashBoard/BranchWiseLast30DaysSales/";
            JsManager.SendJson(serviceUrl, jsonParam, onSuccess, onFailed);

            function onSuccess(jsonData) {
                var data = [];
                var keysAndLevel = [];
                $.each(jsonData.Data, function(i, v) {
                    var obj = new Object();
                    obj.Date = v.Date;
                    $.each(jsonData.BranchList, function(i1, v1) {
                        var salesOfBranch = v.BranchWiseSales.filter(w => w.Branch == v1.Branch)[0];
                        if (typeof salesOfBranch === "undefined") {
                            obj[v1.Branch] = 0.00;
                        } else {
                            obj[v1.Branch] = salesOfBranch.Sales;
                        }
                    });

                    data.push(obj);
                });

                $.each(jsonData.BranchList, function(i, v) {
                    keysAndLevel.push(v.Branch);
                });
                Morris.Area({
                    element: 'BranchWiseLast30DaysSales',
                    data: data,
                    xkey: 'Date',
                    ykeys: keysAndLevel,
                    labels: keysAndLevel,
                    xLabelAngle: 50,
                    hideHover: 'auto',
                    behaveLikeLine: true,
                    resize: true,
                    pointFillColors: ['red'],
                    pointStrokeColors: ['#fff'],
                    lineColors: ['#8cc449', 'red', '#2196f3', '#dab634', '#15d0bf', '#0c7d73', '#7d0c45', '#f94297', '#8842f9', '#441f80', '#77b9f1', '#627f98', '#73bf65', '#a9bf65'],
                    lineWidth: ['3px'],
                    fillOpacity: 0.4
                });
            }

            function onFailed(error) {
                console.log(error.statusText);
            }
        },

        LastTwoMonthSalesBranchWise: function () {
            var jsonParam = '';
            var serviceUrl = "/PosDashBoard/PosLastTwoMonthSalesBranchWise/";
            JsManager.SendJson(serviceUrl, jsonParam, onSuccess, onFailed);

            function onSuccess(jsonData) {
                Morris.Bar({
                    element: 'LastTwoMonthSalesBranchWise',
                    data: jsonData,
                    xkey: 'Branch',
                    ykeys: ['LastMonth', 'CurrentMonth'],
                    labels: ['Prev. Month', 'This Month'],
                    xLabelAngle: 50,
                    hideHover: 'auto',
                    behaveLikeLine: true,
                    resize: true,
                    barColors: ['#8cc2ec', '#2196f3']
                });
            }

            function onFailed(error) {
                console.log(error.statusText);
            }
        },


        Draw30DaysSalesWithDue: function() {
            var jsonParam = '';
            var serviceUrl = "/PosDashBoard/GetLast30DaysSales/";
            JsManager.SendJson(serviceUrl, jsonParam, onSuccess, onFailed);

            function onSuccess(jsonData) {
                Morris.Area({
                    element: 'Last30DaysSales',
                    data: jsonData,
                    xkey: 'Date',
                    ykeys: ['Sales', 'Due'],
                    labels: ['Sales Amtount', 'Due Amount'],
                    xLabelAngle: 50,
                    hideHover: 'auto',
                    behaveLikeLine: true,
                    resize: true,
                    pointFillColors: ['red'],
                    pointStrokeColors: ['#fff'],
                    lineColors: ['#2196f3', 'red'],
                    lineWidth: ['3px'],
                    fillOpacity: 0.4
                });
            }

            function onFailed(error) {
                console.log(error.statusText);
            }

        }

    };