var msgObj = {
    Title: "default title",
    Message: "Default Message",
    Type: "Success",
    Icon: "fas fa-exclamation-circle",
    Url: '',
    Target:''
};

var Message = {
    Prompt: function(customMsg) {
        if (typeof (customMsg) == "undefined")
            customMsg = "Do you want to proceed?.";

        var yesNo;
        if (confirm(customMsg)) {
            yesNo = true;
        } else {
            yesNo = false;
        }
        return yesNo;
    },

    Success: function(event, url, target) {
        var saveMsg = "Successfully saved.";
        var updateMsg = "Successfully updated.";
        var deleteMsg = "Successfully deleted.";
        var cancelMsg = "Successfully cancelled";
        var rejectMsg = "Successfully rejected";
        var addMsg = "Successfully Added";


        msgObj.Title = "Success";
        msgObj.Type = "success";
        msgObj.Icon = "far fa-check-circle";
        msgObj.Url = url;
        msgObj.Target = target;

        if (event == "save") {
            msgObj.Message = saveMsg;
            Message.MainMessage(msgObj);
        } else if (event == "update") {
            msgObj.Message = updateMsg;
            Message.MainMessage(msgObj);
        } else if (event == "delete") {
            msgObj.Message = deleteMsg;
            Message.MainMessage(msgObj);
        } else if (event == "cancel") {
            msgObj.Message = cancelMsg;
            Message.MainMessage(msgObj);
        } else if (event == "reject") {
            msgObj.Message = rejectMsg;
            Message.MainMessage(msgObj);
        } else if (event == "add") {
            msgObj.Message = addMsg;
            Message.MainMessage(msgObj);
        } else {
            msgObj.Message = event;
            Message.MainMessage(msgObj);
        }
    },

    Error: function(event, url, target) {
        var saveMsg = "Failed to save.";
        var updateMsg = "Failed to update.";
        var deleteMsg = "Failed to delete.";
        var printMsg = "Failed to print";
        var cancelMsg = "Failed to cancel";
        var rejectMsg = "Failed to reject";
        var unknownMsg = "Internal server error.";
        var addMsg = "Failed to add.";

        msgObj.Title = "Error !";
        msgObj.Type = "danger";
        msgObj.Icon = "fas fa-times";
        msgObj.Url = url;
        msgObj.Target = target;

        if (event == "save") {
            msgObj.Message = saveMsg;
            Message.MainMessage(msgObj);
        } else if (event == "update") {
            msgObj.Message = updateMsg;
            Message.MainMessage(msgObj);
        } else if (event == "delete") {
            msgObj.Message = deleteMsg;
            Message.MainMessage(msgObj);
        } else if (event == "cancel") {
            msgObj.Message = cancelMsg;
            Message.MainMessage(msgObj);
        } else if (event == "reject") {
            msgObj.Message = rejectMsg;
            Message.MainMessage(msgObj);
        } else if (event == "add") {
            msgObj.Message = addMsg;
            Message.MainMessage(msgObj);
        } else if (event == "unknown") {
            msgObj.Message = unknownMsg;
            Message.MainMessage(msgObj);
        } else if (event == "print") {
            msgObj.Message = printMsg;
            Message.MainMessage(msgObj);
        } else {
            msgObj.Message = event;
            Message.MainMessage(msgObj);
        }
    },

    Warning: function(message, url, target) {
        msgObj.Title = "Warning !";
        msgObj.Type = "warning";
        msgObj.Message = message;
        msgObj.Icon = "fas fa-exclamation-triangle";
        msgObj.Url = url;
        msgObj.Target = target;
        Message.MainMessage(msgObj);
    },

    Notification: function(message, url, target) {
        msgObj.Title = "Warning !";
        msgObj.Type = "info";
        msgObj.Message = message;
        msgObj.Icon = "far fa-bell";
        msgObj.Url = url;
        msgObj.Target = target;
        Message.MainMessage(msgObj);
    },

    SuccessMessage: function(message, url, target) {
        msgObj.Title = "Success";
        msgObj.Type = "success";
        msgObj.Message = message;
        msgObj.Icon = "far fa-check-circle";
        msgObj.Url = url;
        msgObj.Target = target;
        Message.MainMessage(msgObj);
    },

    ErrorMessage: function(message, url, target) {
        msgObj.Title = "Error !";
        msgObj.Type = "danger";
        msgObj.Message = message;
        msgObj.Icon = "fas fa-times";
        msgObj.Url = url;
        msgObj.Target = target;
        Message.MainMessage(msgObj);
    },

    Exception: function (xhr) {
        var msg = JSON.parse(xhr.responseText);
        var message = '';
        switch (msg) {
        case 547:
            message = 'This record is already in use !';
            break;
        case 50547:
            message = 'This record is already in use !';
            break;
        case 2601:
            message = 'This is already exist !';
            break;
        case 2627:
            message = 'This is already exist !';
            break;
        case 10054:
            message = 'Network problem. Please communicate with your network administrator !';
            break;
        case -1000:
            message = 'Session timeout reload and try again later.';
            break;
        case -1005:
            message = 'Invalid request!';
            break;
        case -1006:
            message = 'You have no Print/Preview permission!';
            break;
        case -1004:
            message = 'You have no delete/remove permission!';
            break;
        case -1003:
            message = 'You have no edit/update permission!';
            break;
        case -1002:
            message = 'You have no add/save permission!';
            break;
        case -1001:
            message = 'You have no resource permission!';
            break;
        case "ModelStateFalse":
            message = 'User input or Model state incorrect';
            break;
        default:
            message = "Internal error please contact with system administrator.";
        }
       
        message = message;
        msgObj.Type = "danger";
        msgObj.Message = message;
        msgObj.Icon = "fas fa-times";
        Message.MainMessage(msgObj);
    },

    MainMessage: function(obj) {
        $.notify({
            title: obj.Title,
            message: obj.Message,
            icon: obj.Icon,
            url: obj.Url,
            target: obj.Target
        }, {
            // settings
            type: obj.Type,
            element: 'body',
            position: null,
            allow_dismiss: true,
            newest_on_top: false,
            showProgressbar: false,
            placement: {
                from: "top",
                align: "center"
            },
            offset: 20,
            spacing: 10,
            z_index: 99999,
            delay: 5000,
            timer: 1000,
            url_target: '_blank',
            mouse_over: null,
            animate: {
                enter: 'animated fadeInDown',
                exit: 'animated fadeOutUp'
            },
            onShow: null,
            onShown: null,
            onClose: null,
            onClosed: null,
            icon_type: 'class'
        });
    }


};

