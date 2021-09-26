var editSpendMethodsSelectClass = "edit-spend-accoount-include-method-selects";
var editSpendMethodsIdPrefix = "edit-spend-accoount-include-method-select-";
var editSpendTbodyId = "edit-spend-account-include-tbody";
var editSpendTableId = "edit-spend-account-include-table";

var editSpendDataViewModel;



function loadEditSpendData(accountId) {
    const accountPeriodId = GetAccountPeriodIdbyAccountId(accountId);
    const method = "GetEditSpendViewModel";
    const parameters = {};
    parameters["accountPeriodId"] = accountPeriodId;
    CallServiceGet("home", method, parameters, loadEditSpendDataSuccess, loadEditSpendDataError);
}

function loadEditSpendDataSuccess(data) {
    editSpendDataViewModel = data;
    $('#edit-spend-amount-value').val('');
    $('#edit-spend-account-name').text(data.AccountName);
    loadEditSpendCurrencies(data.SupportedCurrencies);
    updateEditSpendCurrencyMethods();
    loadEditSpendAccountInclude(data.SupportedAccountInclude);
    updateEditAccountIncludeMethods();
    loadEditSpendTypes();
    loadEditSpendDate(data);

    showEditSpendModal();
}

function loadEditSpendDataError(error) {

}

function loadEditSpendControls() {
    $('#edit-spend-currency-select')
        .multiselect({
            onChange: function() {
                updateEditSpendCurrencyMethods();
                openMultiselect('#edit-spend-method-select');
                updateEditSpendAccountIncludeData();
            },
            multiple: false
        });
    $('#edit-spend-method-select').multiselect();
    $('#edit-spend-account-include-select').multiselect({
        onChange: function () {
            updateEditAccountIncludeMethods();
            updateEditSpendAccountIncludeData();
        },
        buttonText: function () {
            return 'Accounts';
        },
    });
    $('#edit-spend-account-include-method-select').multiselect();
    $('#edit-spend-type-select').multiselect();
}

function loadEditSpendTypes() {
    const jquerySelector = "#edit-spend-type-select";
    const spendTypes = editSpendDataViewModel.SpendTypeViewModels;
    $(jquerySelector).empty();
    if (spendTypes) {
        for (let i = 0; i < spendTypes.length; i++) {
	        const item = spendTypes[i];
	        $(jquerySelector).append($('<option>', {
                value: item.SpendTypeId,
                text: item.SpendTypeName
            }));
	    }
    }
    $(jquerySelector).multiselect('rebuild');
}

function loadEditSpendCurrencies(currencyList) {
    var selectedValues = null;
    const jquerySelector = "#edit-spend-currency-select";
    $(jquerySelector).empty();
    if (currencyList) {
        for (let i = 0; i < currencyList.length; i++) {
            const item = currencyList[i];
            if (item.Isdefault) {
                selectedValues = item.CurrencyId;
            }
            $(jquerySelector).append($('<option>', {
                value: item.CurrencyId,
                text: item.CurrencyName
            }));
        }
        $(jquerySelector).multiselect('rebuild');
        $(jquerySelector).multiselect('select', -1);
        if (selectedValues) {
            $(jquerySelector).multiselect('select', selectedValues);
            $(jquerySelector).multiselect('refresh');
        }
            
    }
}

function getEditSpendSelectedCurrency() {
    const selectedIds = getArraySelectedValues('#edit-spend-currency-select');
    if (selectedIds && selectedIds.length > 0) {
        const selectedId = selectedIds[0];
        if (isNaN(selectedId)) {
            return 0;
        }
        const id = parseInt(selectedId);
        return id;
    }
    return 0;
}

function getEditSpendMethodsData(currencyId) {
    if (currencyId && currencyId > 0 && editSpendDataViewModel && editSpendDataViewModel.SupportedCurrencies) {
        const currencies = editSpendDataViewModel.SupportedCurrencies;
        let currencyData = null;
        for (let i = 0; i < currencies.length; i++) {
	        const item = currencies[i];
	        if (item.CurrencyId == currencyId) {
                currencyData = item;
                break;
            }
	    }
	    if (currencyData != null && currencyData.MethodIds) {
            return currencyData.MethodIds;
        }
    }
    return new Array();
}

function getEditSpendAccountIncludeData(accountId) {
    if (accountId && accountId > 0 && editSpendDataViewModel && editSpendDataViewModel.SupportedAccountInclude) {
	    const accounts = editSpendDataViewModel.SupportedAccountInclude;
	    for (let i = 0; i < accounts.length; i++) {
		    const item = accounts[i];
		    if (item.AccountId == accountId)
                return item;
	    }
    }
	return null;
}

function updateEditSpendCurrencyMethods() {
    const currentCurrencyId = getEditSpendSelectedCurrency();
    const methods = getEditSpendMethodsData(currentCurrencyId);
    var selectedValues = null;
    const jquerySelector = "#edit-spend-method-select";
    $(jquerySelector).empty();
    if (methods) {
        for (let i = 0; i < methods.length; i++) {
            const item = methods[i];
            if (item.Isdefault) {
                selectedValues = item.CurrencyId;
            }
            $(jquerySelector).append($('<option>', {
                value: item.Id,
                text: item.Name
            }));
        }
        $(jquerySelector).multiselect('rebuild');
        if (selectedValues)
            $(jquerySelector).multiselect('select', selectedValues);
    }
}

function loadEditSpendAccountInclude(accountIncludeList) {
    const selectedValues = new Array();
    const jquerySelector = "#edit-spend-account-include-select";
    $(jquerySelector).empty();
    if (accountIncludeList) {
        for (let i = 0; i < accountIncludeList.length; i++) {
            const item = accountIncludeList[i];
            if (item.IsDefault) {
                selectedValues.push(item.AccountId);
            }
            $(jquerySelector).append($('<option>', {
                value: item.AccountId,
                text: item.AccountName
            }));
        }
        $(jquerySelector).multiselect('rebuild');
        if (selectedValues.length > 0)
            $(jquerySelector).multiselect('select', selectedValues);
    } else {
        $(jquerySelector).multiselect('rebuild');
    }

}

function clearEditSpendAccountIncludeMethodTable() {
    $("#" + editSpendTbodyId).remove();
    $("#" + editSpendTableId).append("<tbody id='" + editSpendTbodyId + "'>content</tbody>");

}

function updateEditSpendAccountIncludeData() {
	const data = getEditSpendUpdateAccountIncludeData();
	if (data.IncludedAccounts && data.IncludedAccounts.length > 0) {
        const method = "UpdateAddSpendAccountInclude";
        const controller = "home";
        CallService(controller,
            method,
            data,
            updateEditSpendAccountIncludeDataSuccess,
            updateEditSpendAccountIncludeDataError);
    }
}

function updateEditSpendAccountIncludeDataSuccess(data) {
    editSpendDataViewModel.SupportedAccountInclude = data;
    updateEditAccountIncludeMethods();
}

function updateEditSpendAccountIncludeDataError(error) {
    
}

function getEditSpendUpdateAccountIncludeData() {
    const accountId = getEditSpendAccountId();
    const currencyId = getEditSpendCurrency();
    const accountIncludeData = getAddSpendAccountInclude();
    return {
        AccountId: accountId,
        CurrencyId: currencyId,
        IncludedAccounts: accountIncludeData
    };
}

function getEditSpendAccountIncludeSelectedIds() {
	const ids = getArraySelectedValues('#edit-spend-account-include-select');
	return ids;
}

function getEditSpendAccountIncludeDataList(ids) {
    const result = new Array();
    if (ids) {
        for (let i = 0; i < ids.length; i++) {
            const id = ids[i];
            const accountData = getEditSpendAccountIncludeData(id);
            if (accountData != null)
                result.push(accountData);
        }
    }
    return result;
}

function loadEditSpendAccountIncludeData(accountDataList) {
    clearEditSpendAccountIncludeMethodTable();
    if (accountDataList) {
        for (let i = 0; i < accountDataList.length; i++) {
            const accountData = accountDataList[i];
            const accountDataRow = createAccountIncludeDataRow(accountData);
            $('#' + editSpendTbodyId).append(accountDataRow);
        }
        $('.' + editSpendMethodsSelectClass).multiselect();
    }
}

function createAccountIncludeDataRow(accountData) {
    if (accountData) {
        const accountColumn = '<td style="vertical-align: middle;">' + accountData.AccountName + '</td>';
        const methodsColumn = "<td>" + createMethodsSelect(accountData.MethodIds, accountData.AccountId) + "</td>";
        const row = "<tr>" + accountColumn + methodsColumn + "</tr>";
        return row;
    }
    return "";
}

function createMethodsSelect(methodList, accountId) {
    if (methodList && methodList.length > 0) {
        let options = "";
        for (let i = 0; i < methodList.length; i++) {
            const method = methodList[i];
            let option;
            if (method.IsDefault || methodList.length == 1) {
                option = '<option value="' + method.Id + '" selected>';
            } else {
                option = '<option value="' + method.Id + '">';
            }
            option += method.Name;
            option += "</option>";
            options += option;
        }
        let select = '<select id="' + editSpendMethodsIdPrefix + accountId + '" ' +
	        'class="' + editSpendMethodsSelectClass + '">';
        select += options;
        select += "</select>";
        return select;
    } else {
        return "";
    }
}

function updateEditAccountIncludeMethods() {
    const selectedAccountIds = getEditSpendAccountIncludeSelectedIds();
    const accountIncludeList = getEditSpendAccountIncludeDataList(selectedAccountIds);
    loadEditSpendAccountIncludeData(accountIncludeList);
}

function loadEditSpendDate(accountData) {
	const dateElement = $('#edit-spend-date');
	try {

        const minTest = new Date(1, 1, 1, 1, 1, 1, 1);

        const initialDate = moment(accountData.InitialDate).toDate();
        const suggestedDate = moment(accountData.SuggestedDate).toDate();
        const endDate = moment(accountData.UserEndDate).toDate();
        dateElement.data("DateTimePicker").minDate(minTest);

        dateElement.data("DateTimePicker").maxDate(endDate);
        dateElement.data("DateTimePicker").minDate(initialDate);
        dateElement.data("DateTimePicker").date(suggestedDate);
    } catch (error) {
        const e = error;
    }
}

function getEditSpendFormData() {
	const result = {
		Amount: getEditSpendAmount(),
		SpendDate: getEditSpendDate(),
		SpendTypeId: getAddSpendType(),
		OriginalAccountData: getEditSpendOriginalAccountData(),
		IncludedAccounts: getAddSpendAccountInclude(),
		CurrencyId: getEditSpendCurrency()
	};
	return result;
}

function getEditSpendAccountId() {
    return editSpendDataViewModel.AccountId;
}

function getEditSpendOriginalAccountData() {
    const accountId = getEditSpendAccountId();
    const methodId = getEditSpendMethodId();
    const resultItem = {
	    AccountId: accountId,
	    ConvertionMethodId: methodId
    };
    return resultItem;
}

function getEditSpendAmount() {
    const value = $('#edit-spend-amount-value').val();
    if (isNumber(value)) {
        return parseFloat(value);
    }
    return 0;
}


function submitEditSpend() {
    const data = getEditSpendFormData();
    const validationResult = validateEditSpendData(data);
    if (validationResult.allValid) {
        data.SpendDate = data.SpendDate.parameter;
        const method = "EditSpendCurrency";
        const controller = "home";
        CallService(controller, method, data, submitEditSpendSuccess, null);
    } else {
        editSpendHandleErrors(validationResult);
    }
}

function editSpendHandleErrors(errorResults) {
	const errorCount = errorResults.errorNumber.length;
	alert(errorCount + ' error(s)');
}

function submitEditSpendSuccess(data) {
    hideAddSpendModal();
    alert(data.length + ' account(s) modified');
    refreshAccountData(data);
}


function getEditSpendDate() {
    const value = $('#edit-spend-date').data("DateTimePicker").date().toDate();
    const parameterDate = createJsDateTimeObject(value);
    const result = {
	    parameter: parameterDate,
	    value: value
    };
    return result;
}

function getEditSpendCurrency() {
	const selected = getSelectedValueInt('#edit-spend-currency-select');
	return selected;
}

function getEditSpendMethodId() {
	const selected = getSelectedValueInt('#edit-spend-method-select');
	return selected;
}

function getAddSpendType() {
	const selected = getSelectedValueInt('#edit-spend-type-select');
	return selected;
}

function getAddSpendAccountInclude() {
    const selects = $('.' + editSpendMethodsSelectClass);
    const result = new Array();
    for (let i = 0; i < selects.length; i++) {
        const select = selects[i];
        const fullId = select.id;
        const accountId = getIdNumFromElement(fullId);
        const methodId = getSelectedValueInt('#' + fullId);
        const resultItem = {
	        AccountId: accountId,
	        ConvertionMethodId: methodId
        };
        result.push(resultItem);
    }
    return result;
}

function validateEditSpendData(spendData) {
    const errorMessages = new Array();
    const parameterError = new Array();
    var result = true;
    if (!validateEditSpendDate(spendData.SpendDate.value)) {
        result = false;
        errorMessages.push("Invalid date");
        parameterError.push(1);
    }
    if (!validateEditSpendCurrencyId(spendData.CurrencyId)) {
        result = false;
        errorMessages.push("Invalid currency");
        parameterError.push(2);
    }
    if (!validateEditSpendAmount(spendData.Amount)) {
        result = false;
        errorMessages.push("Invalid amount");
        parameterError.push(3);
    }
    if (!validateEditSpendOriginalAccountData(spendData.OriginalAccountData)) {
        result = false;
        errorMessages.push("Invalid account data");
        parameterError.push(4);
    }
    if (!validateEditSpendType(spendData.SpendTypeId)) {
        result = false;
        errorMessages.push("Invalid type");
        parameterError.push(5);
    }
    if (!validateEditSpendAccountInclude(spendData.IncludedAccounts)) {
        result = false;
        errorMessages.push("Invalid account include data");
        parameterError.push(6);
    }
    return {
        allValid: result,
        errorMessages: errorMessages,
        errorNumber: parameterError
    };
}

function validateEditSpendDate(date) {
    if (date) {
        const accountData = editSpendDataViewModel;
        const initialDate = moment(accountData.InitialDate).toDate();
        const endDate = moment(accountData.UserEndDate).toDate();
        return date >= initialDate && date <= endDate;
    } else {
        return false;
    }
}

function validateEditSpendCurrencyId(currencyId) {
    if (currencyId) {
        return currencyId > 0;
    }
    return false;
}

function validateEditSpendAmount(amount) {
    if (amount) {
        return amount > 0;
    }
    return false;
}

function validateEditSpendOriginalAccountData(accountData) {
    if (accountData && accountData.AccountId && accountData.ConvertionMethodId) {
        return accountData.AccountId > 0 && accountData.ConvertionMethodId > 0;
    }
    return false;
}

function validateEditSpendType(spendTypeId) {
    if (spendTypeId) {
        return spendTypeId > 0;
    }
    return false;
}

function validateEditSpendAccountInclude(accountIncludeList) {
    if (accountIncludeList) {
        for (let i = 0; i < accountIncludeList.length; i++) {
            const item = accountIncludeList[i];
            if (!item.AccountId || item.AccountId < 1) {
                return false;
            }
            if (!item.ConvertionMethodId || item.ConvertionMethodId < 1) {
                return false;
            }
        }
    }
    return true;
}