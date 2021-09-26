$(function () {

	var currentAccountGroupId;
	var updatePerformed;

	var reportUpdate = function() {
		updatePerformed = true;
	}

	var resetUpdateFlag = function() {
		if (!$('#account-group-modal').hasClass('in')) {
			updatePerformed = false;
		}
	}

	const loadControls = function() {
		$('.account-group-select')
			.multiselect({
				onChange: selectChanged
			});

		$('#account-group-modal')
			.on('hidden.bs.modal',
				function() {
					onHideModal();
				});
		$('#account-group-modal')
			.on('show.bs.modal',
				function() {
					resetUpdateFlag();
				});
	};

	var selectChanged = function(arg) {
		const id = arg[0].parentElement.id;
		if (id.includes('account-group-list-select')) {
	        updateDetails();
	    }
	}

	var loadAccountGroupList = function (id) {
		const parameters = {
			accountGroupId: id
		};

		CallServiceGet('Account', 'GetAccountGroupList', parameters, loadAccountGroupListSucces);
	}

	var loadAccountGroupListSucces = function(data) {
	    setDropDownSelectableObject('#account-group-list-select', data.AccountGroupViewModels, true, 'New...');
	    updateDetails();
	}

    var loadAccountGroupDetails = function(id) {
	    const parameters = {
		    accountGroupId: id
	    };

	    CallServiceGet('Account', 'GetAccountGroupDetailViewModel', parameters, loadAccountGroupDetailsSuccess);
	}

	var loadAccountGroupDetailsSuccess = function (data) {
        clearDetails();
        const details = data.Model;
        setDropDownSelectableObject('#account-group-position-select', data.AccountGroupPositions);
        if (details) {
            currentAccountGroupId = details.AccountGroupId;
            $('#account-group-account-name-text').val(details.AccountGroupName);
            $('#account-group-display-text').val(details.AccountGroupDisplayValue);
            $('#account-group-show-checkbox').prop('checked', details.DisplayDefault);
            setSelectedValues('#account-group-position-select', details.AccountGroupPosition.toString(), true);
            updateAddEditControls(details.AccountGroupId);
        } else {
            currentAccountGroupId = 0;
            updateAddEditControls(null);
        }
	    showModal();
    }

    var showModal = function () {
    	$('#account-group-modal').modal('show');
    }

    var onHideModal = function() {
		if (updatePerformed) {
			location.reload(true);
		}
    }

	var updateAddEditControls = function(accountGroupId) {
    	if (accountGroupId) {
		    $('#account-group-action-delete-button').prop('disabled', false);
            $('#account-group-modal-button').html('Update');
        } else {
    		$('#account-group-modal-button').html('Add');
    		$('#account-group-action-delete-button').prop('disabled', true);
	    }
    }

    var clearDetails = function() {
        $('#account-group-account-name-text').val('');
        $('#account-group-display-text').val('');
        $('#account-group-show-checkbox').prop('checked', '');
    }

    var updateDetails = function() {
	    const id = getSelectedValueInt('#account-group-list-select');
	    loadAccountGroupDetails(id);
	}

	var getAccountGroupModel = function() {
        if (!currentAccountGroupId) {
            currentAccountGroupId = 0;
        }

        const model = {
	        AccountGroupId: currentAccountGroupId,
	        AccountGroupName: $('#account-group-account-name-text').val(),
	        AccountGroupPosition: getAccountGroupIdValue(),
	        AccountGroupDisplayValue: $('#account-group-display-text').val(),
	        DisplayDefault: $("#account-group-show-checkbox:checkbox:checked").length > 0
        };

        return model;
    }

    var getAccountGroupIdValue = function() {
        const selectedValues = getArraySelectedValues('#account-group-position-select');
        if (selectedValues && selectedValues.length > 0) {
            let value = selectedValues[0];
            if (value == 0) {
                value = null;
            }

            return value;
        }

        return null;
    }

    var validateAccountGroupModel = function(model) {
        const errors = new Array();
        if (!model.AccountGroupName) {
            errors.push({
                fieldId: "Account group name",
                message: "Value cannot be empty"
            });
        }

        if (!model.AccountGroupDisplayValue) {
            errors.push({
                fieldId: "Account group display",
                message: "Value cannot be empty"
            });
        }

        if (!model.AccountGroupPosition || model.AccountGroupPosition < 0) {
            errors.push({
                fieldId: "Account group position",
                message: "Value cannot be empty or less than 0"
            });
        }

        return errors;
    }

	const actionDelete = function() {
		if (currentAccountGroupId && currentAccountGroupId > 0) {
			const parameters = {
				accountGroupId: currentAccountGroupId
			};

			callServicePost('Account', 'DeleteAccountGroup', null, parameters, actionDeleteSuccess);
		}
	};

	var actionDeleteSuccess = function (data) {
		reportUpdate();
		loadAccountGroupList(null);
	}

    const submit = function() {
	    if (currentAccountGroupId || currentAccountGroupId === 0) {
		    const model = getAccountGroupModel();
		    const errors = validateAccountGroupModel(model);
		    if (errors.length > 0) {
			    handleModelErrors(errors);
		    } else {
			    const modelParam = {
				    accountGroupClientViewModel: model
			    };

			    const method = currentAccountGroupId == 0 ? "AddAccountGroup" : "EditAccountGroup";
			    callServicePost('Account', method, null, modelParam, submitSuccess);
		    }
	    }
    };

    var submitSuccess = function (data) {
	    reportUpdate();
        const message = currentAccountGroupId == 0 ? 'Added successfully' : 'Updated successfully';
        alert(message);
        loadAccountGroupList(data);
    }

    var handleModelErrors = function(errors) {
	    const errorsCount = errors.length;
	    alert(errorsCount + ' error(s) found');
	}

	window.loadAccountGroupList = loadAccountGroupList;
    window.submitAccountGroupValues = submit;
    window.deleteAccountGroup = actionDelete;
	loadControls();
});