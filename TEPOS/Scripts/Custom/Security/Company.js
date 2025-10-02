$(document).ready(function() {
    $("#btnSave").click(function() {
        Manager.Update();
    });

});

var Manager= {
    Update: function() {
        if (Message.Prompt()) {
            var jsonParam = new FormData($('#frmCPost').get(0));
            var serviceURL = "/Security/UpdateCompany/";
            JsManager.SendJsonWithFile(serviceURL, jsonParam, onSuccess, onFailed);
        }

        function onSuccess(jsonData) {
            if (jsonData == "0") {
                Message.Error("update");
            } else {
                Message.Success("update");
                window.location = "/Security/Company";
            }
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }

    }
}