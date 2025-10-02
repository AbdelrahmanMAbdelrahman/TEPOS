

$(document).ready(function () {
   
    $("#btnMakeBranchZeroStock").click(function () {
      
        Manager.MakeBranchZeroStock();
    });

});




var Manager = {
    MakeBranchZeroStock: function () {
        if (Message.Prompt()) {
            var jsonParam = {
            };
            var serviceURL = "/PosStock/SetBranchZeroStock/";
            JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

            function onSuccess(jsonData) {
                Message.Success(jsonData.MessageSave);
            }

            function onFailed(error) {
            }
        }
    },
}



