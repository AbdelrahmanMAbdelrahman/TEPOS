$(document).ready(function () {
    CustomerBilling.LoadCustomerNearestCityDDL();
   // CustomerBilling.LoadAllBranch();
    CustomerBilling.LoadAllBranchForCustomer();
    CustomerBilling.LoadCustomerClassDDL();
    CustomerBilling.LoadCustomerRegionDDL();

    $("#spanAddCustomer").click(function () {
        $("#modalCustomer").modal('show');
    });

    $("#btnSaveCustomer").click(function () {
        CustomerBilling.Save();
    });
});

var CustomerBilling = {
    Save: function () {
        var obj = [];
        obj.push({ 'Customer class': $("#PosCustomerClassId").val() });
        obj.push({ 'Phone no': $("#Phone").val() });
        obj.push({ 'Region': $("#PosRegionId").val() });
        obj.push({ 'Nearest city or zone': $("#PosCityOrNearestZoneId").val() });

            if (JsManager.validate(obj)) {
                if (Message.Prompt()) {
                    var jsonParam = $('#customerForm').serialize();
                    var serviceURL = "/PosSetting/SaveCustomer/";
                    JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
                }

                function onSuccess(jsonData) {
                    if (jsonData == "0") {
                        Message.Error("save");
                    }else if (jsonData == "PhoneNoExists") {
                        Message.Warning("Phone already exists.");
                    }
                    else {
                        Message.Success('Customer(' + jsonData.CustomerNo + ') saved successfully.');
                        $("#customerForm")[0].reset();
                        Manager.GetCustomer();
                        $("#txtCustomer").val(jsonData.Id);
                        $("#txtCustomer").trigger("chosen:updated");    
                        $("#modalCustomer").modal('hide');
                    }
                }

                function onFailed(xhr, status, err) {
                    Message.Exception(xhr);
                }
            }
        
    },

    LoadCustomerNearestCityDDL: function () {
        var jsonParam = "";
        var serviceURL = "/PosDropdownSetting/GetCustomerNearestCity/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            JsManager.PopulateCombo('#PosCityOrNearestZoneId', jsonData);
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },
    LoadCustomerClassDDL: function () {
        var jsonParam = "";
        var serviceURL = "/PosDropdownSetting/GetCustomerClasses/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            JsManager.PopulateCombo('#PosCustomerClassId', jsonData);
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

    LoadCustomerRegionDDL: function () {
        var jsonParam = "";
        var serviceURL = "/PosDropdownSetting/GetCustomerRegions/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            JsManager.PopulateCombo('#PosRegionId', jsonData);
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },
    LoadAllBranchForCustomer: function () {
        var jsonParam = "";
        var serviceURL = "/PosDropdownSetting/GetBranchList/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            JsManager.PopulateCombo('#PosBranchIdForCustomer', jsonData);
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },
    
};