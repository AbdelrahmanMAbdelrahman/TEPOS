 $(document).ready(function() {
        DbBackManager.LoadDB();
        DbBackManager.LoadDrive();
        $("#btnBackup").click(function() {
            DbBackManager.BackupDB();
        });

        $("input[name='isMdfFile']").change(function () {
            if ($(this).val() == 'true') {
                $("#cmbDatabase").attr('disabled', true);
            } else {
                $("#cmbDatabase").attr('disabled', false);
            }
        });
    });

    var DbBackManager = {
        LoadDrive: function() {
            var jsonParam = '';
            var serviceURL = "/Home/GetDrive/";
            JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

            function onSuccess(jsonData) {
                var objProgram = jsonData;
                JsManager.PopulateCombo('#cmbDrive', objProgram);
            }

            function onFailed(error) {
                window.alert(error.statusText);
            }
        },

        LoadDB: function() {
            var jsonParam = '';
            var serviceURL = "/Home/GetDatabase/";
            JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

            function onSuccess(jsonData) {
                var objProgram = jsonData;
                JsManager.PopulateCombo('#cmbDatabase', objProgram);
            }

            function onFailed(error) {
                window.alert(error.statusText);
            }
        },

        BackupDB: function () {
            JsManager.StartProcessBar("Please wait......");
            var jsonParam = {
                drive: $("#cmbDrive").val(),
                db: $("#cmbDatabase").val(),
                isMdfFile: $("input[name='isMdfFile']:checked").val()
            };
        var serviceURL = "/Home/BackupDB/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            if (jsonData == 0) {
                Message.Error('save');
            } else if (jsonData == 1) {
                Message.Success("Database successfully backup.");
            } else {
                Message.Warning(jsonData);
            }
            JsManager.EndProcessBar();
        }

        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
            JsManager.EndProcessBar();
        }
    }

    };