
var JsManager = {
    SendJsonWithFile: function (serviceUrl, jsonParams, successCalback, errorCallback,type) {
        if (typeof type == "undefined") {
            type = 'POST';
        }
        $.ajax({
            type: type,
            url:serviceUrl,
            data: jsonParams,
            processData: false,
            contentType: false,
            success: successCalback,
            error: errorCallback
        });
    },

    SendJson: function (serviceUrl, jsonParams, successCalback, errorCallback, type) {
       if (typeof type == "undefined") {
           type = 'POST';
       }
        $.ajax({
            cache: false,
            async: true,
            type: type,
            url: serviceUrl,
            data: jsonParams,
            success: successCalback,
            error: errorCallback
        });

    },

    SendJsonTraditionlTrue: function (serviceUrl, jsonParams, successCalback, errorCallback, type) {
        if (typeof type == "undefined") {
            type = 'POST';
        }
        $.ajax({
            cache: false,
            async: true,
            type: type,
            url:  serviceUrl,
            data: jsonParams,
            traditional:true,
            success: successCalback,
            error: errorCallback
        });

    },

    SendJsonAsyncON: function (serviceUrl, jsonParams, successCalback, errorCallback,type) {
        if (typeof type == "undefined") {
            type = 'POST';
        }
        $.ajax({
            cache: false,
            async: false,
            type: type,
            url: serviceUrl,
            data: jsonParams,
            success: successCalback,
            error: errorCallback
        });

    },

    PopulateCombo: function (container, data, defaultText, defaultValue) {
        var cbmOptions = "";
        if (defaultText != null && defaultText!='') {
            if (typeof defaultValue == "undefined") {
                defaultValue = "";
            }
            cbmOptions = "<option selected value=" + defaultValue + ">" + defaultText + "</option>";
        }
        $.each(data, function () {
            cbmOptions += '<option value=\"' + this.Id + '\">' + this.Name + '</option>';
        });
        $(container).html(cbmOptions);
    },

   ChangeDateFormat: function (value, isTime) {
        var dateFormat = "";
        if (value != "" && value!=null) {
            var time = value.replace(/\/Date\(/g,"").replace(/\)\//g,"");
            var date = new Date();
            date.setTime(time);
            var dd = (date.getDate().toString().length == 2 ? date.getDate() : '0' + date.getDate()).toString();
            var mm = ((date.getMonth() + 1).toString().length == 2 ? (date.getMonth() + 1) : '0' + (date.getMonth() + 1)).toString();
            var yyyy = date.getFullYear().toString();
            var timeformat = "";
            if (isTime != 0) {
                timeformat = (date.getHours().toString().length == 2 ? date.getHours() : '0' + date.getHours()) + ':' + (date.getMinutes().toString().length == 2 ? date.getMinutes() : '0' + date.getMinutes()) + ':' + (date.getSeconds().toString().length == 2 ? date.getSeconds() : '0' + date.getSeconds());
                dateFormat = mm + '/' +dd + '/' + yyyy + ' ' + timeformat;
            }
            else {
                dateFormat = mm + '/' + dd + '/' + yyyy;
            }
       }
       if (dateFormat == "01/01/1900")
           return "";
        return dateFormat;
    },

    ChangeToSQLDateTimeFormatMMddyyyy: function(value, isTime) {

        if (value != "" && value != null) {
            var time = value.replace(/\/Date\(/g, "").replace(/\)\//g, "");
            var date = new Date();
            date.setTime(time);
            var dd = (date.getDate().toString().length == 2 ? date.getDate() : '0' + date.getDate()).toString();
            var mm =
                ((date.getMonth() + 1).toString().length == 2 ? (date.getMonth() + 1) : '0' + (date.getMonth() + 1))
                    .toString();
            var yyyy = date.getFullYear().toString();
            var timeformat = "";
            var sqlFormatedDate = "";
            if (isTime != 0) {
                timeformat = (date.getHours().toString().length == 2 ? date.getHours() : '0' + date.getHours()) +
                    ':' +
                    (date.getMinutes().toString().length == 2 ? date.getMinutes() : '0' + date.getMinutes()) +
                    ':' +
                    (date.getSeconds().toString().length == 2 ? date.getSeconds() : '0' + date.getSeconds());
                sqlFormatedDate = mm + '/' + dd + '/' + yyyy + ' ' + timeformat;
            } else {
                sqlFormatedDate = mm + '/' + dd + '/' + yyyy;
            }
            return sqlFormatedDate;
        } else {
            return "";
        }
    },

   

    DMYToMDY: function (value) {
        var datePart = value.match(/\d+/g);
        var day = datePart[0];
        var month = datePart[1];
        var year = datePart[2];
        return month + '/' + day + '/' + year;
    },
    MDYToDMY: function (value) {
        var datePart = value.match(/\d+/g);
        var month = datePart[0];
        var day = datePart[1];
        var year = datePart[2];
        return day + '/' + month + '/' + year;
    },

    DMYToYMD: function (value) {
        var datePart = value.match(/\d+/g);
        var day = datePart[0];
        var month = datePart[1];
        var year = datePart[2];
        return year + '/' + month + '/' + day;
    },

    MDYToDashDMY: function (value) {
        if (value != "") {
            var datePart = value.match(/\d+/g);
            var month = datePart[0];
            var day = datePart[1];
            var year = datePart[2];
            return day + '-' + month + '-' + year;
        }
    },

    
    validate: function(obj) {
        if (obj.length > 0) {
            for (var object of obj) {
                for (var property in object) {
                    if (property.toString() != "Id") {
                        if (object[property] === "" || object[property] === "0" || object[property] === null) {
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

    StartProcessBar: function (msg, width) {
        if (typeof (msg) === "undefined")
            msg = "Please Wait..........";
        if (typeof (width) === "undefined")
            width = "180px";

        var div = "<div id='ui_waitingbar' style='position: fixed;z-index: 99999;padding-top: 20%;top: 0;width: 100%;height: 100%;background: rgba(0, 0, 0, 0.18);left:0'><p style='width: " + width + ";-align: center;background: #f9f9f9;border-radius: 5px;padding: 10px 10px;margin: 0 auto;box-shadow: 2px 2px 15px #807b79;'><img height='30px' src='/Scripts/lib/assets/img/temp-img/WaitingProcessBar.gif' />&nbsp" + msg + "</p></div>";
        $("#process_notifi").append(div);
    },
    EndProcessBar: function () {
        $("#ui_waitingbar").remove();
    },

    JqBootstrapValidation: function (form, onValidate) {
        $(form).find('input,select,textarea').not('[type=submit]').jqBootstrapValidation(
            {
                preventSubmit: false,
                submitSuccess: onValidate,
                submitError: function ($form, event, errors) {
                    event.preventDefault();
                }
            });
    },

    BaseUrl: function() {
        return window.location.host;
    },

    UrlParams: function(k) {
        var p = {};
        location.search.replace(/[?&]+([^=&]+)=([^&]*)/gi, function(s, k, v) { p[k] = v });
        return k ? p[k] : p;
    }

}