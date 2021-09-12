$(function() {

    var ADD_ACTION = 1;
    var EDIT_ACTION = 2;

    var currentAccountId;
    var currentAction;
    var currentAccountIncludeData;

    var accountIncludeMethodsSelectClass = "account-detail-account-include-method-select";
    var accountIncludeMethodsIdPrefix = "account-detail-account-include-method-select-";
    var hexColorRegexPattern = /(^#[0-9A-F]{6}$)|(^#[0-9A-F]{3}$)/i;

    var accountIncludeTbodyId = "account-detail-account-include-tbody";
    var accountIncludeTableId = "account-detail-account-include-table";
    var currentChangesArray;

    const loadControls = function() {
	    $('#account-detail-header-color-input')
		    .change(
			    function() {
				    accountStyleChanged();
			    });

	    $('#account-detail-border-color-input')
		    .change(
			    function() {
				    accountStyleChanged();
			    });

	    $('#account-detail-base-budget-input')
		    .change(
			    function() {
				    baseBudgetChanged();
			    });

	    $('#account-detail-account-name-text')
		    .change(
			    function() {
				    accountNameChanged();
			    });

	    $('.account-detail-select')
		    .multiselect({
			    onChange: selectChanged
		    });
    };

    var selectChanged = function(arg) {
        const id = arg[0].parentElement.id;
        if (id.includes('account-include')) {
            accountIncludeChanged();
            return;
        }

        if (id.includes('account-type')) {
            accountTypeChanged();
        }

        if (id.includes('spend-type')) {
            spendTypeChanged();
        }

        if (id.includes('financial-entity')) {
            financialEntityChanged();
        }

        if (id.includes('-currency-')) {
            currencyChanged();
        }

        if (id.includes('-group-')) {
            groupChanged();
        }
    }

    var addChangeValue = function(changeValue) {
        if (!currentChangesArray) {
            currentChangesArray = new Array();
        }

        if (!currentChangesArray.includes(changeValue)) {
            currentChangesArray.push(changeValue);
        }
    }

    var currencyChanged = function() {
        if (currentAction == ADD_ACTION) {
            loadAccountIncludeData();
        }
    }

    var groupChanged = function() {
        addChangeValue('AccountGroupId');
    }

    var accountStyleChanged = function() {
        addChangeValue('HeaderColor');
    }

    var baseBudgetChanged = function() {
        addChangeValue('BaseBudget');
    }

    var accountNameChanged = function() {
        addChangeValue('AccountName');
    }

    var spendTypeChanged = function() {
        addChangeValue('SpendTypeId');
    }

    var financialEntityChanged = function() {
        addChangeValue('FinancialEntityId');
    }

    var accountTypeChanged = function() {
        addChangeValue('AccountTypeId');
    }

    var accountIncludeMethodChanged = function() {
        addChangeValue('AccountIncludes');
    }

    var accountIncludeChanged = function() {
        addChangeValue('AccountIncludes');
        updateCurrentAccountIncludeTable();
    }

    var submitAddAccount = function() {
        if (currentAction != ADD_ACTION) {
            return;
        }

        const model = getAddValues();
        const errors = validateAddModel(model);
        if (errors.length > 0) {
            handleEditModelValues(errors);
            return;
        }

        const parameters = {
	        clientAddAccount: model
        };

        callServicePost('Account', 'AddAccount', null, parameters, submitAddValuesSuccess);
    }

    var submitAddValuesSuccess = function() {
        alert('Added successfully');
        location.reload();
    };

    var submitEditValues = function() {
        if (currentAction != EDIT_ACTION) {
            return;
        }

        const model = getEditedValues();
        const errors = validateEditModel(model);
        if (errors.length > 0) {
            handleEditModelValues(errors);
            return;
        }

        const parameters = {
	        clientEditAccount: model
        };

        callServicePost('Account', 'UpdateAccount', null, parameters, submitEditValuesSuccess);
    }

    var submitEditValuesSuccess = function(data) {
        if (data == 0) {
            alert('Updated successfully');
            location.reload();
        } else {
            alert('error');
        }
    }

    var handleEditModelValues = function() {
        alert('Validation errors');
    }

    var validateAddModel = function(model) {
        const errors = new Array();
        if (!model) {
            errors.push({
                fieldId: -1,
                message: "Invalid model object"
            });
            return errors;
        }

        if (!validAccountNameValue(model.AccountName)) {
            errors.push({
                fieldId: "AccountName",
                message: "Invalid account name"
            });
        }

        if (!validFrontStyle(model.HeaderColor)) {
            errors.push({
                fieldId: "HeaderColor",
                message: "Invalid colors"
            });
        }

        if (!validBaseBudget(model.BaseBudget)) {
            errors.push({
                fieldId: "BaseBudget",
                message: "Invalid budget"
            });
        }

        if (!validId(model.AccountTypeId, true)) {
            errors.push({
                fieldId: "AccountTypeId",
                message: "Invalid Account Type"
            });
        }

        if (!validId(model.SpendTypeId)) {
            errors.push({
                fieldId: "SpendTypeId",
                message: "Invalid Spend Type"
            });
        }

        if (!validId(model.FinancialEntityId)) {
            errors.push({
                fieldId: "FinancialEntityId",
                message: "Invalid Financial EntityId"
            });
        }

        if (!validId(model.PeriodDefinitionId)) {
            errors.push({
                fieldId: "PeriodDefinitionId",
                message: "Invalid Period Type"
            });
        }

        if (!validId(model.CurrencyId)) {
            errors.push({
                fieldId: "CurrencyId",
                message: "Invalid Currency"
            });
        }

        if (!validId(model.AccountGroupId)) {
            errors.push({
                fieldId: "AccountGroupId",
                message: "Invalid Account group"
            });
        }

        if (!validAccountInclude(model.AccountIncludes)) {
            errors.push({
                fieldId: "AccountIncludes",
                message: "Invalid accounts to include"
            });
        }


        return errors;
    }

    var validateEditModel = function(model) {
        const errors = new Array();
        if (!model) {
            errors.push({
                fieldId: -1,
                message: "Invalid model object"
            });
            return errors;
        }

        if (model.EditAccountFields.includes('AccountName')) {
            if (!validAccountNameValue(model.AccountName)) {
                errors.push({
                    fieldId: "AccountName",
                    message: "Invalid account name"
                });
            }
        }

        if (model.EditAccountFields.includes('HeaderColor')) {
            if (!validFrontStyle(model.HeaderColor)) {
                errors.push({
                    fieldId: "HeaderColor",
                    message: "Invalid colors"
                });
            }
        }

        if (model.EditAccountFields.includes('BaseBudget')) {
            if (!validBaseBudget(model.BaseBudget)) {
                errors.push({
                    fieldId: "BaseBudget",
                    message: "Invalid budget"
                });
            }
        }

        if (model.EditAccountFields.includes('AccountTypeId')) {
            if (!validId(model.AccountTypeId, true)) {
                errors.push({
                    fieldId: "AccountTypeId",
                    message: "Invalid Account Type"
                });
            }
        }

        if (model.EditAccountFields.includes('SpendTypeId')) {
            if (!validId(model.SpendTypeId)) {
                errors.push({
                    fieldId: "SpendTypeId",
                    message: "Invalid Spend Type"
                });
            }
        }

        if (model.EditAccountFields.includes('FinancialEntityId')) {
            if (!validId(model.FinancialEntityId)) {
                errors.push({
                    fieldId: "FinancialEntityId",
                    message: "Invalid Financial EntityId"
                });
            }
        }

        if (model.EditAccountFields.includes('AccountGroupId')) {
            if (!validId(model.AccountGroupId)) {
                errors.push({
                    fieldId: "AccountGroupId",
                    message: "Invalid Account group"
                });
            }
        }

        if (model.EditAccountFields.includes('AccountIncludes')) {
            if (!validAccountInclude(model.AccountIncludes)) {
                errors.push({
                    fieldId: "AccountIncludes",
                    message: "Invalid accounts to include"
                });
            }
        }

        return errors;
    }

    var validAccountInclude = function(values) {
        if (!values) {
            return false;
        }

        for (let i = 0; i < values.length; i++) {
	        const item = values[i];
	        if (!validId(item.AccountId) || !validId(item.AccountIncludeId) || !validId(item.CurrencyConverterMethodId)) {
                return false;
            }
	    }

	    return true;
    }

    var validId = function(id, required) {
        return (!required && (!id || id == 0)) || (id && isInt(id)) && id != 0;
    }

    var validBaseBudget = function(value) {
	    return value && isNumber(value.Value);
    }

    var validFrontStyle = function(value) {
        if (!value) {
            return false;
        }

        if (value.HeaderColor) {
            return validColor(value.HeaderColor);
        }
        if (value.HeaderColor) {
            return validColor(value.BorderColor);
        }

        return true;
    }

    var validColor = function(value) {
        return hexColorRegexPattern.test(value);
    }

    var validAccountNameValue = function(value) {
        if (value) {
            return true;
        }
        return false;
    }

    var getAddValues = function() {
        const clientAddAccount = {
        
        };

        clientAddAccount.AccountName = getAccountNameValue();
        clientAddAccount.PeriodDefinitionId = getPeriodDefinitionIdValue();
        clientAddAccount.CurrencyId = getCurrencyIdValue();
        clientAddAccount.HeaderColor = getFrontStyleValue();
        clientAddAccount.BaseBudget = getBaseBudgetValue();
        clientAddAccount.AccountTypeId = getAccountTypeIdValue();
        clientAddAccount.SpendTypeId = getSpendTypeIdValue();
        clientAddAccount.FinancialEntityId = getFinancialEntityIdValue();
        clientAddAccount.AccountIncludes = getAccountIncludes();
        clientAddAccount.AccountGroupId = getAccountGroupIdValue();
        return clientAddAccount;
    }

    var getEditedValues = function() {
        if (!currentChangesArray) {
            return null;
        }

        const clientEditAccount = {
	        AccountId: currentAccountId
        };

        if (currentChangesArray.includes('AccountName')) {
            clientEditAccount.AccountName = getAccountNameValue();
        }

        if (currentChangesArray.includes('HeaderColor')) {
            clientEditAccount.HeaderColor = getFrontStyleValue();
        }

        if (currentChangesArray.includes('BaseBudget')) {
            clientEditAccount.BaseBudget = getBaseBudgetValue();
        }

        if (currentChangesArray.includes('AccountTypeId')) {
            clientEditAccount.AccountTypeId = getAccountTypeIdValue();
        }

        if (currentChangesArray.includes('SpendTypeId')) {
            clientEditAccount.SpendTypeId = getSpendTypeIdValue();
        }

        if (currentChangesArray.includes('FinancialEntityId')) {
            clientEditAccount.FinancialEntityId = getFinancialEntityIdValue();
        }

        if (currentChangesArray.includes('AccountIncludes')) {
            clientEditAccount.AccountIncludes = getAccountIncludes();
        }

        if (currentChangesArray.includes('AccountGroupId')) {
            clientEditAccount.AccountGroupId = getAccountGroupIdValue();
        }

        clientEditAccount.EditAccountFields = currentChangesArray;
        return clientEditAccount;
    }

    var getAccountGroupIdValue = function () {
        const selectedValues = getArraySelectedValues('#account-detail-group-select');
        if (selectedValues && selectedValues.length > 0) {
            let value = selectedValues[0];
            if (value == 0) {
                value = null;
            }

            return value;
        }

        return null;
    };

    var getAccountIncludes = function() {
        const selects = $('.' + accountIncludeMethodsSelectClass);
        const result = new Array();
        for (let i = 0; i < selects.length; i++) {
            const select = selects[i];
            const fullId = select.id;
            const accountId = getIdNumFromElement(fullId);
            const methodId = getSelectedValueInt('#' + fullId);
            const resultItem = {
	            AccountId: currentAccountId,
	            AccountIncludeId: accountId,
	            CurrencyConverterMethodId: methodId
            };
            result.push(resultItem);
        }
        return result;
    }

    var getFinancialEntityIdValue = function() {
        const selectedValues = getArraySelectedValues('#account-detail-financial-entity-select');
        if (selectedValues && selectedValues.length > 0) {
            let value = selectedValues[0];
            if (value == 0) {
                value = null;
            }

            return value;
        }

        return null;
    };

    var getPeriodDefinitionIdValue = function() {
        const selectedValues = getArraySelectedValues('#account-detail-period-type-select');
        if (selectedValues && selectedValues.length > 0) {
            let value = selectedValues[0];
            if (value == 0) {
                value = null;
            }

            return value;
        }

        return null;
    }

    var getCurrencyIdValue = function() {
        const selectedValues = getArraySelectedValues('#account-detail-currency-select');
        if (selectedValues && selectedValues.length > 0) {
            let value = selectedValues[0];
            if (value == 0) {
                value = null;
            }

            return value;
        }

        return null;
    };

    var getSpendTypeIdValue = function() {
        const selectedValues = getArraySelectedValues('#account-detail-spend-type-select');
        if (selectedValues && selectedValues.length > 0) {
            let value = selectedValues[0];
            if (value == 0) {
                value = null;
            }

            return value;
        }

        return null;
    };

    var getAccountTypeIdValue = function() {
        const selectedValues = getArraySelectedValues('#account-detail-account-type-select');
        if (selectedValues && selectedValues.length > 0) {
            let value = selectedValues[0];
            if (value == 0) {
                value = null;
            }

            return value;
        }

        return null;
    };

	var getBaseBudgetValue = function() {
		var value = $('#account-detail-base-budget-input').val();
		value = parseFloat(value);
		value = {
			Value: value
		}
		return value;
	};

    var getAccountNameValue = function() {
	    const value = $('#account-detail-account-name-text').val();
	    return value;
    };

    var getFrontStyleValue = function() {
	    const colors = {
		    HeaderColor: $('#account-detail-header-color-input').val(),
		    BorderColor: $('#account-detail-border-color-input').val()
	    };

	    return colors;
	}

	var showAccountDetailModal = function() {
        $('#account-detail-modal').modal('show');
    }

    var loadAccountIncludeData = function() {
        const currencyId = getCurrencyIdValue();
        const parameters = {
	        currencyId: currencyId
        };
        if (validId(currencyId, true)) {
            CallServiceGet('Account', 'GetAccountIncludes', parameters, loadAccountIncludeDataSuccess);
        }
    }

    var loadAccountIncludeDataSuccess = function(data) {
        setDropDownSelectableObject('#account-detail-account-include-select', data);
        currentAccountIncludeData = data;
        updateCurrentAccountIncludeTable();
    }

    const loadAddAccountViewModel = function() {
	    currentAction = ADD_ACTION;
	    CallServiceGet('Account', 'GetAddAccountViewModel', null, loadAddAccountViewModelSuccess);
    };

    var loadAddAccountViewModelSuccess = function(data) {
        clearControls();
        loadAccountDetailsInControls(data);
        showAccountDetailModal();
    }

    const loadAccountDetails = function(accountId) {
	    currentAction = EDIT_ACTION;
	    currentAccountId = accountId;
	    currentChangesArray = new Array();
	    if (accountId && accountId != 0) {
		    const parameter = {
			    accountId: accountId
		    };
		    CallServiceGet('Account', 'GetAccountDetailsInfoViewModel', parameter, loadAccountDetailsSuccess);
	    }
    };

    var loadAccountDetailsSuccess = function(data) {
        if (!data)
            return;
        clearControls();
        loadAccountDetailsInControls(data);
        setControlsForEdit();
        showAccountDetailModal();
    }

    var setControlsForEdit = function() {
        $('#account-detail-account-name-text').prop('readonly', false);
        $('#account-detail-spend-type-select').multiselect('enable');
        $('#account-detail-account-include-select').multiselect('enable');
        $('#account-detail-currency-select').multiselect('disable');
        $('#account-detail-period-type-select').multiselect('disable');
        $('#account-detail-account-type-select').multiselect('enable');
        $('#account-detail-financial-entity-select').multiselect('enable');
        $('#account-detail-group-select').multiselect('enable');
    }

    var loadAccountDetailsInControls = function(data) {
        $('#account-detail-account-name-text').val(data.AccountName);
        setDropDownSelectableObject('#account-detail-spend-type-select', data.SpendTypeViewModels);
        setDropDownSelectableObject('#account-detail-account-include-select', data.AccountIncludeViewModels);
        currentAccountIncludeData = data.AccountIncludeViewModels;
        updateCurrentAccountIncludeTable();
        setDropDownSelectableObject('#account-detail-currency-select', data.CurrencyViewModels);
        setDropDownSelectableObject('#account-detail-period-type-select', data.PeriodTypeViewModels);
        setDropDownSelectableObject('#account-detail-account-type-select', data.AccountTypeViewModels);
        setDropDownSelectableObject('#account-detail-spend-type-select', data.SpendTypeViewModels);
        setDropDownSelectableObject('#account-detail-financial-entity-select', data.FinancialEntityViewModels, true);
        setDropDownSelectableObject('#account-detail-group-select', data.AccountGroupViewModels);

        $('#account-detail-header-color-input').val(data.AccountStyle.HeaderColor);
        $('#account-detail-border-color-input').val(data.AccountStyle.BorderColor);
        $('#account-detail-base-budget-input').val(data.BaseBudget);
    }

    var updateCurrentAccountIncludeTable = function() {
        const selectedIds = getAccountIncludeSelectedIds();
        const accountIncludeData = getAccountIncludeDataList(selectedIds);
        updateAccountIncludeTable(accountIncludeData);
    }

    var getAccountIncludeDataList = function(ids) {
        const result = new Array();
        if (ids) {
            for (let i = 0; i < ids.length; i++) {
                const id = ids[i];
                const accountData = getAccountIncludeData(id);
                if (accountData != null)
                    result.push(accountData);
            }
        }
        return result;
    }

    var getAccountIncludeData = function(accountId) {
        if (accountId && accountId > 0 && currentAccountIncludeData) {
	        const accounts = currentAccountIncludeData;
	        for (let i = 0; i < accounts.length; i++) {
		        const item = accounts[i];
		        if (item.AccountId == accountId)
                    return item;
	        }
        }
	    return null;
    }

    var getAccountIncludeSelectedIds = function() {
	    const ids = getArraySelectedValues('#account-detail-account-include-select');
	    return ids;
	}

	var updateAccountIncludeTable = function(accountIncludes) {
        clearAccountIncludeMethodTable();
        if (accountIncludes) {
            for (let i = 0; i < accountIncludes.length; i++) {
                const accountData = accountIncludes[i];
                const accountDataRow = createAccountIncludeDataRow(accountData);
                $('#' + accountIncludeTableId).append(accountDataRow);
            }
            $('.' + accountIncludeMethodsSelectClass)
                .multiselect({
                    onChange: accountIncludeMethodChanged
                });
        }
    }

    var clearAccountIncludeMethodTable = function() {
        $("#" + accountIncludeTbodyId).remove();
        $("#" + accountIncludeTableId).append("<tbody id='" + accountIncludeTbodyId + "'>content</tbody>");
    }

    var createAccountIncludeDataRow = function(accountData) {
        if (accountData) {
            const accountColumn = '<td style="vertical-align: middle;">' + accountData.AccountName + '</td>';
            const methodsColumn = "<td>" + createMethodsSelect(accountData.MethodIds, accountData.AccountId) + "</td>";
            const row = "<tr>" + accountColumn + methodsColumn + "</tr>";
            return row;
        }
        return "";
    }

    var createMethodsSelect = function(methodList, accountId) {
        if (methodList && methodList.length > 0) {
            let options = "";
            for (let i = 0; i < methodList.length; i++) {
                const method = methodList[i];
                let option;
                if (method.IsSelected || methodList.length == 1) {
                    option = '<option value="' + method.Id + '" selected>';
                } else {
                    option = '<option value="' + method.Id + '">';
                }
                option += method.Name;
                option += "</option>";
                options += option;
            }
            let select = '<select id="' +
	            accountIncludeMethodsIdPrefix +
	            accountId +
	            '" ' +
	            'class="' +
	            accountIncludeMethodsSelectClass +
	            '">';
            select += options;
            select += "</select>";
            return select;
        } else {
            return "";
        }
    }

    var clearControls = function() {

    }

    const submitAction = function() {
	    if (currentAction == ADD_ACTION) {
		    submitAddAccount();
		    return;
	    }

	    if (currentAction == EDIT_ACTION) {
		    submitEditValues();
		    return;
	    }
    };

    window.submitAccountDetailWindow = submitAction;
    window.loadAccountDetails = loadAccountDetails;
    window.loadAddAccountViewModel = loadAddAccountViewModel;
    loadControls();
});