

$(document).ready(function () {
    RolePermission.ReadyFunction();
    RolePermission.RoleDropDown();

});

function EditSpan(inputId, btnSave, btnEdit) {
    $("#" + inputId).removeAttr('disabled').removeAttr('style');
    $("#" + inputId).css({ "border-radius": "4px", "border": "1px solid #DEB0A6", "padding-left": "2px" });
    $("#" + btnEdit).hide();
    $("#" + btnSave).show();
}

function SaveSpan(inputId, btnSave, btnEdit, resId) {
    var jsonParam = { id: resId, displayName: $("#" + inputId).val() };
    var serviceUrl = "/Security/UpdateResourcePermission/";
    JsManager.SendJson(serviceUrl, jsonParam, onSuccess, onFailed);
    function onSuccess(jsonData) {
        if (jsonData == "0") {
            Message.Error("save");
        }
        else {
            $("#" + inputId).css({ "border": "none", "background": "none" });
            $("#" + inputId).attr("disabled", "disabled");
            $("#" + btnEdit).show();
            $("#" + btnSave).hide();
        }
    }
    function onFailed(xhr, status, err) {
        Message.Exception(xhr);
    }

}


function UpdateStatus(chkId, inputType) {
    var objs = [];
    var dataTypeName;
    var rolePermissionId;
    var resourcePermissionId;

    //only for add edit and delete permission checkbox
    if (inputType == "inputprmi") {
        var obj = new Object();
        dataTypeName = $("#" + chkId).attr("data-type");

        if ($('#' + chkId).is(':checked') == true) {

            $("#" + chkId).prop('checked', true);
            if (dataTypeName == "add") {
                obj.Add = true;
            }
            else if (dataTypeName == "edit") {
                obj.Edit = true;
            }
            else if (dataTypeName == "delete") {
                obj.Delete = true;
            }
        }
        else {

            $("#" + chkId).prop('checked', false);

            if (dataTypeName == "add") {
                obj.Add = false;
            }
            else if (dataTypeName == "edit") {
                obj.Edit = false;
            }
            else if (dataTypeName == "delete") {
                obj.Delete = false;
            }
        }

        //2 for role permission,1 for resource permission
        obj.Flag = 2;
        obj.RolePermissionId = $("#" + chkId).attr("data-roleprmiid");
        obj.ResourcePermissionId = "";
        obj.ResourceStatus = false;
        objs.push(obj);
    }


    else if (inputType == "rolePrmiWithResourcePrmi") {

        $('#' + chkId).parent().find('input[type=checkbox]').each(function (i, v) {
            dataTypeName = $(v).attr("data-type");
            rolePermissionId = $(v).attr("data-roleprmiid");
            resourcePermissionId = $(v).attr("data-resprmiid");
            var obj = new Object();

            if ($('#' + chkId).is(':checked') == true) {

                $("#" + $(v).attr("Id")).prop('checked', true);

                if (dataTypeName == "add") {
                    obj.Add = true;
                    obj.RolePermissionId = rolePermissionId;
                    tmpFlag = 2;
                }
                else if (dataTypeName == "edit") {
                    obj.Edit = true;
                    obj.RolePermissionId = rolePermissionId;
                    tmpFlag = 2;
                }
                else if (dataTypeName == "delete") {
                    obj.Delete = true;
                    obj.RolePermissionId = rolePermissionId;
                    tmpFlag = 2;
                }
                else {
                    obj.ResourcePermissionId = resourcePermissionId;
                    obj.ResourceStatus = true;
                    tmpFlag = 1;
                }

            }
            else {

                $("#" + $(v).attr("Id")).prop('checked', false);

                if (dataTypeName == "add") {
                    obj.Add = false;
                    obj.RolePermissionId = rolePermissionId;
                    tmpFlag = 2;
                }
                else if (dataTypeName == "edit") {
                    obj.Edit = false;
                    obj.RolePermissionId = rolePermissionId;
                    tmpFlag = 2;
                }
                else if (dataTypeName == "delete") {
                    obj.Delete = false;
                    obj.RolePermissionId = rolePermissionId;
                    tmpFlag = 2;
                }
                else {
                    obj.ResourcePermissionId = resourcePermissionId;
                    obj.ResourceStatus = false;
                    tmpFlag = 1;
                }
            }
            obj.Flag = tmpFlag;
            objs.push(obj);
        });
    }

        //this is add,edit,delete with each other checkbox
    else {

        $("#" + chkId).parent().parent().find('input[type=checkbox]').each(function (i, v) {
            var obj = new Object();
            dataTypeName = $(v).attr("data-type");
            rolePermissionId = $(v).attr("data-roleprmiid");
            resourcePermissionId = $(v).attr("data-resprmiid");
            var tmpFlag;

            if ($('#' + chkId).is(':checked') == true) {

                $("#" + $(v).attr("Id")).prop('checked', true);

                if (dataTypeName == "add") {
                    obj.Add = true;
                    obj.RolePermissionId = rolePermissionId;
                    tmpFlag = 2;
                }
                else if (dataTypeName == "edit") {
                    obj.Edit = true;
                    obj.RolePermissionId = rolePermissionId;
                    tmpFlag = 2;
                }
                else if (dataTypeName == "delete") {
                    obj.Delete = true;
                    obj.RolePermissionId = rolePermissionId;
                    tmpFlag = 2;
                }
                else {
                    obj.ResourcePermissionId = resourcePermissionId;
                    obj.ResourceStatus = true;
                    tmpFlag = 1;
                }
            }
            else {

                $("#" + $(v).attr("Id")).prop('checked', false);

                if (dataTypeName == "add") {
                    obj.Add = false;
                    obj.RolePermissionId = rolePermissionId;
                    tmpFlag = 2;
                }
                else if (dataTypeName == "edit") {
                    obj.Edit = false;
                    obj.RolePermissionId = rolePermissionId;
                    tmpFlag = 2;
                }
                else if (dataTypeName == "delete") {
                    obj.Delete = false;
                    obj.RolePermissionId = rolePermissionId;
                    tmpFlag = 2;
                }
                else {
                    obj.ResourcePermissionId = resourcePermissionId;
                    obj.ResourceStatus = false;
                    tmpFlag = 1;
                }
            }
            obj.Flag = tmpFlag;
            if (dataTypeName != "NO") {
                objs.push(obj);
            }

        });
    }

    if (objs.length > 0) {
        var jsonParam = 'objRoleWithResourcePermission=' + JSON.stringify(objs);
        var serviceURL = "/Security/UpdateRoleWithResourcePermission/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
    }
    function onSuccess(jsonData) {
        if (jsonData == "0") {
            Message.Error("save");
        }
    }
    function onFailed(xhr, status, err) {
        Message.Exception(xhr);
    }

}


var RolePermission = {
    RoleDropDown: function () {
        var jsonParam = '';
        var serviceURL = "/PosDropDown/GetRole/";
        JsManager.SendJsonAsyncON(serviceURL, jsonParam, onSuccess, onFailed);
        function onSuccess(jsonData) {
            JsManager.PopulateCombo('#rollId', jsonData, 'Select Role','0');
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
    },

    ReadyFunction: function () {
        // first example
        $("#browser").treeview();

        // second example
        $("#navigation").treeview({
            persist: "location",
            collapsed: false,
            unique: true
        });

        // third example
        $("#red").treeview({
            animated: "fast",
            collapsed: true,
            unique: true,
            persist: "cookie",
            toggle: function () {
                //window.console && console.log("%o was toggled", this);
            }
        });

        // fourth example
        $("#black, #gray").treeview({
            control: "#treecontrol",
            persist: "cookie",
            cookieId: "treeview-black"
        });
        setTimeout(function () {
            $("#rollId").val(localStorage.getItem("RoleIdForResourcePermission") == null ? 0 : localStorage.getItem("RoleIdForResourcePermission"));
            localStorage.removeItem("RoleIdForResourcePermission");
            }, 1000);


        $('#rollId').change(function () {
            RolePermission.SetRoleIdForResourcePermission();
        });
    },

    SetRoleIdForResourcePermission: function () {
        var jsonParam = { roleId: $('#rollId').val() };
        var serviceURL = "/Security/SetRoleIdForResourcePermission/";
        JsManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
        function onSuccess(jsonData) {
            if (jsonData == 0) {
                Message.Warning("Reload and try again later");
            }
            else {
                localStorage.setItem("RoleIdForResourcePermission", $('#rollId').val());
                window.location = "/Security/RolePermission";
            }
        }
        function onFailed(xhr, status, err) {
            Message.Exception(xhr);
        }
    },

}









function AddNestedItem(parentRpermiId, parentResId) {
    // إزالة أي input سابق مفتوح
    $('.addChildInputContainer').remove();

    var parentLi = $('#chk_' + parentRpermiId).closest('li');

    var inputHtml =
        '<span class="addChildInputContainer">' +
        '  <input type="text" id="childName_' + parentRpermiId + '" placeholder="Enter child name" class="inptTxt childNameInput" />' +
        '  <span class="fas fa-check saveIcon inlineSave" title="Save" onclick="SaveNestedItem(' + parentRpermiId + ', ' + parentResId + ');"></span>' +
        '  <span class="fas fa-times cancelIcon inlineCancel" title="Cancel" onclick="$(this).closest(\'.addChildInputContainer\').remove();"></span>' +
        '</span>';

    if (parentLi.find('ul').length > 0) {
        parentLi.find('ul:first').prepend(inputHtml);
    } else {
        parentLi.append('<ul>' + inputHtml + '</ul>');
    }

    $('#childName_' + parentRpermiId).focus();
}

function SaveNestedItem(parentRpermiId, parentResId) {
    var childNameInput = $('#childName_' + parentRpermiId);
    var childName = childNameInput.val().trim();
    var parentLi = childNameInput.closest('li');

    if (childName === '') {
        alert('Please enter child name.');
        childNameInput.focus();
        return;
    }

    var secRoleId = $('#rollId').val();
    var secResourceId = $('#resourceId').val();

    if (!secRoleId || !secResourceId || !parentResId) {
        alert('Incorrect Role or Parent.');
        return;
    }

    $.ajax({
        url: addChildResourceUrl,
        type: 'POST',
        data: {
            parentResId: parentResId,
            childName: childName,
            secRoleId: secRoleId,
            secModuleId: secResourceId
        },
        success: function (response) {
            if (response.success) {
                $('.addChildInputContainer').remove();

                var newResource = response.resource;

                var newLiHtml =
                    '<li>' +
                    '  <input data-type="0" type="checkbox" ' + (newResource.Status ? "checked" : "") +
                    ' id="chk_' + newResource.RpermiId + '" data-resprmiid="' + newResource.ResId +
                    '" onchange="UpdateStatus(\'chk_' + newResource.RpermiId + '\', \'rolePrmiWithResourcePrmi\');" />' +
                    '  <div class="divPermissionBorderStyle">' +
                    '    <input id="inpt_' + newResource.RpermiId + '" type="text" disabled="disabled" class="inptTxt" value="' + newResource.DisplayName + '" />' +
                    '    <span id="edit_' + newResource.RpermiId + '" class="fas fa-edit editIcon" title="Edit" onclick="EditSpan(\'inpt_' + newResource.RpermiId + '\', \'save_' + newResource.RpermiId + '\', \'edit_' + newResource.RpermiId + '\');"></span>' +
                    '    <span id="save_' + newResource.RpermiId + '" class="fas fa-check saveIcon" style="display:none;" title="Save" onclick="SaveSpan(\'inpt_' + newResource.RpermiId + '\', \'save_' + newResource.RpermiId + '\', \'edit_' + newResource.RpermiId + '\', ' + newResource.RpermiId + ');"></span>' +
                    '    <span id="add_' + newResource.RpermiId + '" class="fas fa-plus addIcon" title="Add child item" onclick="AddNestedItem(' + newResource.RpermiId + ', ' + newResource.ResId + ');"></span>' +
                    '  </div>' +
                    '</li>';

                var ulElement = parentLi.find('ul:first');
                if (ulElement.length === 0) {
                    ulElement = $('<ul></ul>');
                    parentLi.append(ulElement);
                }
                ulElement.append(newLiHtml);

                alert('Element has been added successfully.');
            } else {
                alert('Fail: ' + response.message);
            }
        },
        error: function (xhr, status, error) {
            alert('error : ' + error);
        }
    });
}
