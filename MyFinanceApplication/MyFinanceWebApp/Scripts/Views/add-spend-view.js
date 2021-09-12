var addSpendMethodsSelectClass = "add-spend-accoount-include-method-selects";
var addSpendMethodsIdPrefix = "add-spend-accoount-include-method-select-";
var addSpendTbodyId = "add-spend-account-include-tbody";
var addSpendTableId = "add-spend-account-include-table";
var addIncomeCode = 2;
var addSpendCode = 1;
var editSpendCode = 3;
var currentAddAction = 0;
var currentEditSpendId = 0;
var addSpendDataViewModel;

function spendAmountClick(spendId, accountId) {
	loadAEditSpendData(accountId, spendId);
}

function loadAEditSpendData(accountId, spendId) {
	const accountPeriodId = window.GetAccountPeriodIdbyAccountId(accountId);
	const method = "GetEditSpendViewModel";
	const parameters = {};
	parameters["accountPeriodId"] = accountPeriodId;
	parameters["spendId"] = spendId;
	currentEditSpendId = spendId;
	window.window.CallServiceGet("home", method, parameters, loadEditSpendDataSuccess, loadAddSpendDataError);
}

function loadEditSpendDataSuccess(data) {
	currentAddAction = editSpendCode;
	addSpendDataViewModel = data;
	loadAddSpendInfo(data.SpendInfo);
	$("#add-spend-account-name").text(data.AccountName);
	$("#add-spend-is-pending").prop("checked", data.SpendInfo.IsPending);
	loadAddSpendCurrencies(data.SupportedCurrencies);
	updateAddSpendCurrencyMethods();
	loadAddSpendAccountInclude(data.SupportedAccountInclude);
	updateAccountIncludeMethods();
	loadAddSpendTypes();
	loadSpendPaymentDate(data.SpendInfo.SetPaymentDate);
	loadSpendDate(data.PossibleDateRange.StartDate, data.PossibleDateRange.ActualDate, data.PossibleDateRange.EndDate);
	setEditSpendControls();
	window.showAddSpendModal();
	if (data.SpendInfo.IsPending) {
		$("#add-spend-confirm-payment").show();
	}
	else {
		$("#add-spend-confirm-payment").hide();
	}
}

function loadAddIncomeData(accountId) {
	const accountPeriodId = window.GetAccountPeriodIdbyAccountId(accountId);
	const method = "GetAddSpendViewModel";
	const parameters = {};
	parameters["accountPeriodId"] = accountPeriodId;
	window.window.CallServiceGet("home", method, parameters, loadAddIncomeDataSuccess, loadAddSpendDataError);
}

function loadAddIncomeDataSuccess(data) {
	currentAddAction = addIncomeCode;
	addSpendDataViewModel = data;
	$("#add-spend-amount-value").val("");
	$("#add-spend-description").val("");
	$("#add-spend-account-name").text(data.AccountName);
	$("#add-spend-is-pending").prop("checked", false);
	loadAddSpendCurrencies(data.SupportedCurrencies);
	updateAddSpendCurrencyMethods();
	loadAddSpendAccountInclude(data.SupportedAccountInclude);
	updateAccountIncludeMethods();
	loadAddSpendTypes();
	loadAddSpendDate(data);
	loadSpendPaymentDate(null);
	setAddIncomeControls();
	window.showAddSpendModal();
}

function loadAddSpendData(accountId) {
	const accountPeriodId = window.GetAccountPeriodIdbyAccountId(accountId);
	const method = "GetAddSpendViewModel";
	const parameters = {};
	parameters["accountPeriodId"] = accountPeriodId;
	window.CallServiceGet("home", method, parameters, loadAddSpendDataSuccess, loadAddSpendDataError);
}

function loadAddSpendDataSuccess(data) {
	currentAddAction = addSpendCode;
	addSpendDataViewModel = data;
	$("#add-spend-amount-value").val("");
	$("#add-spend-description").val("");
	$("#add-spend-account-name").text(data.AccountName);
	$("#add-spend-is-pending").prop("checked", false);
	loadAddSpendCurrencies(data.SupportedCurrencies);
	updateAddSpendCurrencyMethods();
	loadAddSpendAccountInclude(data.SupportedAccountInclude);
	updateAccountIncludeMethods();
	loadAddSpendTypes();
	loadAddSpendDate(data);
	loadSpendPaymentDate(null);
	setAddSpendControls();
	window.showAddSpendModal();
}

function loadAddSpendDataError(error) {
	console.log(error);
}

function loadAddSpendInfo(data) {
	$("#add-spend-amount-value").val(data.ConvertedAmount);
	$("#add-spend-description").val(data.Description);
}

function setAddSpendControls() {
	setAddSpendModalHeader(addSpendCode);
	$("#spend-modal-button").show();
	$("#spend-modal-button").prop("disabled", false);
	disableControlsForEditSpend(false);
	$("#add-spend-set-payment-date").data("DateTimePicker").disable();
	$("#add-spend-confirm-payment").hide();
}

function setAddIncomeControls() {
	setAddSpendModalHeader(addIncomeCode);
	$("#spend-modal-button").show();
	$("#spend-modal-button").prop("disabled", false);
	disableControlsForEditSpend(false);
	$("#add-spend-set-payment-date").data("DateTimePicker").disable();
	$("#add-spend-confirm-payment").hide();
}

function setEditSpendControls() {
	setAddSpendModalHeader(editSpendCode);
	$("#spend-modal-button").prop("disabled", false);
	disableControlsForEditSpend(true);
}

function disableControlsForEditSpend(disable) {
	if (disable) {
		
		//$('#add-spend-date').data("DateTimePicker").disable();
		$("#add-spend-date").prop("disabled", true);
		$("#add-spend-set-payment-date").data("DateTimePicker").disable();
		$("#add-spend-amount-value").prop("disabled", true);
		$("#add-spend-currency-select").multiselect("disable");
		$("#add-spend-method-select").multiselect("disable");
		$("#add-spend-account-include-select").multiselect("disable");
		$("#add-spend-is-pending").attr("disabled", true);
		setAccountIncludeSpendTableDisable(true);
	} else {
		$("#add-spend-set-payment-date").data("DateTimePicker").enable();
		//$('#add-spend-date').data("DateTimePicker").enable();
		$("#add-spend-date").prop("disabled", false);
		$("#add-spend-amount-value").prop("disabled", false);
		$("#add-spend-currency-select").multiselect("enable");
		$("#add-spend-method-select").multiselect("enable");
		$("#add-spend-account-include-select").multiselect("enable");
		$("#add-spend-is-pending").removeAttr("disabled");
		setAccountIncludeSpendTableDisable(false);
	}
}

function setAccountIncludeSpendTableDisable(disable) {
	if (disable) {
		$("#add-spend-account-include-tbody").find("select").multiselect("disable");
	} else {
		$("#add-spend-account-include-tbody").find("select").multiselect("enable");
	}

}

function setAddSpendModalHeader(actionType) {
	const spendColor = "#c19393";
	const incomeColor = "#94c193";
	const editColor = "#f9d709";

	const spendMessage = "New Expense";
	const incomeMessage = "Add Income";
	const editMessage = "Spend Details";

	var color = "";
	var message = "";

	if (actionType == addSpendCode) {
		color = spendColor;
		message = spendMessage;
	}

	if (actionType == addIncomeCode) {
		color = incomeColor;
		message = incomeMessage;
	}

	if (actionType == editSpendCode) {
		color = editColor;
		message = editMessage;
	}

	$("#add-model-title-h4").text(message);
	$("#add-modal-header-id").css("background", color);
}

function loadAddSpendControls() {
	$("#add-spend-currency-select")
		.multiselect({
			onChange: function () {
				updateAddSpendCurrencyMethods();
				window.openMultiselect("#add-spend-method-select");
				updateAddSpendAccountIncludeData();
			},
			multiple: false
		});
	$("#add-spend-method-select").multiselect();
	$("#add-spend-account-include-select").multiselect({
		onChange: function () {
			updateAccountIncludeMethods();
			updateAddSpendAccountIncludeData();
		},
		buttonText: function () {
			return "Accounts";
		}
	});
	$("#add-spend-account-include-method-select").multiselect();
	$("#add-spend-type-select").multiselect();
}

function loadAddSpendTypes() {
	var selectedValues = null;
	const jquerySelector = "#add-spend-type-select";
	const spendTypes = addSpendDataViewModel.SpendTypeViewModels;
	$(jquerySelector).empty();
	if (spendTypes) {
		for (let i = 0; i < spendTypes.length; i++) {
			const item = spendTypes[i];
			if (item.IsDefault) {
				selectedValues = item.SpendTypeId;
			}
			$(jquerySelector).append($("<option>", {
				value: item.SpendTypeId,
				text: item.SpendTypeName
			}));
		}
		$(jquerySelector).multiselect("rebuild");
		$(jquerySelector).multiselect("select", -1);
		if (selectedValues) {
			$(jquerySelector).multiselect("select", selectedValues);
			$(jquerySelector).multiselect("refresh");
		}
	}
	$(jquerySelector).multiselect("rebuild");
}

function loadAddSpendCurrencies(currencyList) {
	var selectedValues = null;
	const jquerySelector = "#add-spend-currency-select";
	$(jquerySelector).empty();
	if (currencyList) {
		for (let i = 0; i < currencyList.length; i++) {
			const item = currencyList[i];
			if (item.Isdefault) {
				selectedValues = item.CurrencyId;
			}
			$(jquerySelector).append($("<option>", {
				value: item.CurrencyId,
				text: item.CurrencyName
			}));
		}
		$(jquerySelector).multiselect("rebuild");
		$(jquerySelector).multiselect("select", -1);
		if (selectedValues) {
			$(jquerySelector).multiselect("select", selectedValues);
			$(jquerySelector).multiselect("refresh");
		}
	}
}

function getAddSpendSelectedCurrency() {
	const selectedIds = window.getArraySelectedValues("#add-spend-currency-select");
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

function getAddSpendMethodsData(currencyId) {
	if (currencyId && currencyId > 0 && addSpendDataViewModel && addSpendDataViewModel.SupportedCurrencies) {
		const currencies = addSpendDataViewModel.SupportedCurrencies;
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

function getAddSpendAccountIncludeData(accountId) {
	if (accountId && accountId > 0 && addSpendDataViewModel && addSpendDataViewModel.SupportedAccountInclude) {
		const accounts = addSpendDataViewModel.SupportedAccountInclude;
		for (let i = 0; i < accounts.length; i++) {
			const item = accounts[i];
			if (item.AccountId == accountId)
				return item;
		}
	}
	return null;
}

function updateAddSpendCurrencyMethods() {
	const currentCurrencyId = getAddSpendSelectedCurrency();
	const methods = getAddSpendMethodsData(currentCurrencyId);
	var selectedValues = null;
	const jquerySelector = "#add-spend-method-select";
	$(jquerySelector).empty();
	if (methods) {
		for (let i = 0; i < methods.length; i++) {
			const item = methods[i];
			if (item.IsDefault) {
				selectedValues = item.Id;
			}
			$(jquerySelector).append($("<option>", {
				value: item.Id,
				text: item.Name
			}));
		}
		$(jquerySelector).multiselect("rebuild");
		$(jquerySelector).multiselect("select", -1);
		if (selectedValues) {
			$(jquerySelector).multiselect("select", selectedValues);
			$(jquerySelector).multiselect("refresh");
		}
			
	}
}

function loadAddSpendAccountInclude(accountIncludeList) {
	const selectedValues = new Array();
	const jquerySelector = "#add-spend-account-include-select";
	$(jquerySelector).empty();
	if (accountIncludeList) {
		for (let i = 0; i < accountIncludeList.length; i++) {
			const item = accountIncludeList[i];
			if (item.IsDefault) {
				selectedValues.push(item.AccountId);
			}
			$(jquerySelector).append($("<option>", {
				value: item.AccountId,
				text: item.AccountName
			}));
		}
		$(jquerySelector).multiselect("rebuild");
		if (selectedValues.length > 0)
			$(jquerySelector).multiselect("select", selectedValues);
	} else {
		$(jquerySelector).multiselect("rebuild");
	}
}

function clearAddSpendAccountIncludeMethodTable() {
	$("#" + addSpendTbodyId).remove();
	$("#" + addSpendTableId).append("<tbody id='" + addSpendTbodyId + "'>content</tbody>");

}

function updateAddSpendAccountIncludeData() {
	const data = getAddSpendUpdateAccountIncludeData();
	if (data.IncludedAccounts && data.IncludedAccounts.length > 0) {
		const method = "UpdateAddSpendAccountInclude";
		const controller = "home";
		window.CallService(controller,
			method,
			data,
			updateAddSpendAccountIncludeDataSuccess,
			updateAddSpendAccountIncludeDataError);
	}
}

function updateAddSpendAccountIncludeDataSuccess(data) {
	addSpendDataViewModel.SupportedAccountInclude = data;
	updateAccountIncludeMethods();
}

function updateAddSpendAccountIncludeDataError(error) {
	console.log(error);
}

function getAddSpendUpdateAccountIncludeData() {
	const accountId = getAddSpendAccountId();
	const currencyId = getAddSpendCurrency();
	const accountIncludeData = getAddSpendAccountInclude();
	return {
		AccountId: accountId,
		CurrencyId: currencyId,
		IncludedAccounts: accountIncludeData
	};
}

function getAddSpendAccountIncludeSelectedIds() {
	const ids = window.getArraySelectedValues("#add-spend-account-include-select");
	return ids;
}

function getAddSpendAccountIncludeDataList(ids) {
	const result = new Array();
	if (ids) {
		for (let i = 0; i < ids.length; i++) {
			const id = ids[i];
			const accountData = getAddSpendAccountIncludeData(id);
			if (accountData != null)
				result.push(accountData);
		}
	}
	return result;
}

function loadAddSpendAccountIncludeData(accountDataList) {
	clearAddSpendAccountIncludeMethodTable();
	if (accountDataList) {
		for (let i = 0; i < accountDataList.length; i++) {
			const accountData = accountDataList[i];
			const accountDataRow = createAccountIncludeDataRow(accountData);
			$("#" + addSpendTbodyId).append(accountDataRow);
		}
		$("." + addSpendMethodsSelectClass).multiselect();
	}
}

function createAccountIncludeDataRow(accountData) {
	if (accountData) {
		const accountColumn = '<td style="vertical-align: middle;">' + accountData.AccountName + "</td>";
		const methodsColumn = "<td>" + createMethodsSelect(accountData.MethodIds, accountData.AccountId) + "</td>";
		let amountInfo;
		if (accountData.Amount) {
			amountInfo = "<td><nobr><span>" + accountData.Amount.CurrencySymbol + accountData.Amount.Value + "</span></nobr></td>";
		} else {
			amountInfo = "<td></td>";
		}
		const row = "<tr>" + accountColumn + methodsColumn + amountInfo + "</tr>";
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
		let select = '<select id="' + addSpendMethodsIdPrefix + accountId + '" ' +
			'class="' + addSpendMethodsSelectClass + '">';
		select += options;
		select += "</select>";
		return select;
	} else {
		return "";
	}
}

function updateAccountIncludeMethods() {
	const selectedAccountIds = getAddSpendAccountIncludeSelectedIds();
	const accountIncludeList = getAddSpendAccountIncludeDataList(selectedAccountIds);
	loadAddSpendAccountIncludeData(accountIncludeList);
}

function loadAddSpendDate(accountData) {
	loadSpendDate(accountData.InitialDate, accountData.SuggestedDate, accountData.UserEndDate);
}

function loadSpendPaymentDate(suggestedDateJs) {
	const dateElement = $("#add-spend-set-payment-date");
	if (suggestedDateJs) {
		try {
			const suggestedDate = window.moment(suggestedDateJs).toDate();
			dateElement.data("DateTimePicker").date(suggestedDate);
		} catch (error) {
			console.log(error);
		}
	} else {
		dateElement.data("DateTimePicker").date(null);
	}
}

//function loadSpendDate(initialDateJs, suggestedDateJs, userEndDateJs) {
//	var dateElement = $('#add-spend-date');
//	if (suggestedDateJs) {
//		try {
//			var minTest = new Date(1, 1, 1, 1, 1, 1, 1);

//			var initialDate = window.moment(initialDateJs).toDate();
//			var suggestedDate = window.moment(suggestedDateJs).toDate();
//			var endDate = window.moment(userEndDateJs).toDate();
//			dateElement.data("DateTimePicker").minDate(minTest);

//			dateElement.data("DateTimePicker").maxDate(endDate);
//			dateElement.data("DateTimePicker").minDate(initialDate);
//			dateElement.data("DateTimePicker").date(suggestedDate);
//		} catch (error) {
//			console.log(error);
//		}
//	} else {
//		dateElement.data("DateTimePicker").maxDate(null);
//		dateElement.data("DateTimePicker").minDate(null);
//		dateElement.data("DateTimePicker").date(null);
//	}
//}

function loadSpendDate(initialDateJs, suggestedDateJs, userEndDateJs) {
	const dateElement = $("#add-spend-date");
	const isoFormat = "YYYY-MM-DDTHH:mm:ss";
	if (suggestedDateJs) {
		try {
			const initialDate = window.moment(initialDateJs).format(isoFormat);
			const suggestedDate = window.moment(suggestedDateJs).format(isoFormat);
			const endDate = window.moment(userEndDateJs).format(isoFormat);

			dateElement.attr("min", initialDate);
			dateElement.attr("max", endDate);
			dateElement.val(suggestedDate);
		} catch (error) {
			console.log(error);
		}
	} else {
		dateElement.attr("min", null);
		dateElement.attr("max", null);
	}
}

function getEditSpendFormData() {
	const modifyList = [4, 5];
	const result = {
		SpendTypeId: getAddSpendType(),
		Description: getAddSpendDescription(),
		ModifyList: modifyList
	};

	return result;
}

function getAddSpendFormData() {
	const result = {
		Amount: getAddSpendAmount(),
		SpendDate: getAddSpendDate(),
		SpendTypeId: getAddSpendType(),
		OriginalAccountData: getAddSpendOriginalAccountData(),
		IncludedAccounts: getAddSpendAccountInclude(),
		CurrencyId: getAddSpendCurrency(),
		Description: getAddSpendDescription(),
		IsPending: getIsPending()
	};
	return result;
}

function getIsPending() {
	return $("#add-spend-is-pending")[0].checked;
}

function getAddSpendDescription() {
	return $("#add-spend-description").val();
}

function getAddSpendAccountId() {
	return addSpendDataViewModel.AccountId;
}

function getAddSpendOriginalAccountData() {
	const accountId = getAddSpendAccountId();
	const methodId = getAddSpendMethodId();
	const resultItem = {
		AccountId: accountId,
		ConvertionMethodId: methodId
	};
	return resultItem;
}

function getAddSpendAmount() {
	const value = $("#add-spend-amount-value").val();
	if (window.isNumber(value)) {
		return parseFloat(value);
	}
	return 0;
}

function confirmSpend() {
	if (currentAddAction == editSpendCode) {
		if (confirm("Are you sure you want confirm payment now?")) {
			const parameter = {
				spendId: currentEditSpendId
			};
			CallService("Home", "ConfirmPendingSpend", parameter, confirmSpendSuccess);
		} else {
			// Do nothing!
		}   
	}
} 

function confirmSpendSuccess(data) {
	window.hideAddSpendModal();
	alert(data.length + " item(s) confirmed");
	window.refreshAccountData(data);
}

function submitAddSpend() {
	const controller = "home";
	var method = "";
	if (currentAddAction != editSpendCode) {
		const data = getAddSpendFormData();
		const validationResult = validateAddSpendData(data);
		if (validationResult.allValid) {
			data.SpendDate = data.SpendDate.parameter;
			
			if (currentAddAction == addSpendCode) {
				method = "AddSpendCurrency";
			}

			if (currentAddAction == addIncomeCode) {
				method = "AddIncome";
			}

			
			window.CallService(controller, method, data, submitAddSpendSuccess, null);
		} else {
			addSpendHandleErrors(validationResult);
		}
	} else {
		const model = getEditSpendFormData();
		model.SpendId = currentEditSpendId;
		const editValidationResult = validateEditSpendData(model);
		if (!editValidationResult.allValid) {
			addSpendHandleErrors(editValidationResult);
		} else {
			method = "EditSpend";
			window.CallService(controller, method, model, submitEditSpendSuccess, null);
		}
	}
}

function addSpendHandleErrors(errorResults) {
	const errorCount = errorResults.errorNumber.length;
	alert(errorCount + " error(s)");
}

function submitAddSpendSuccess(data) {
	window.hideAddSpendModal();
	alert(data.length + " account(s) modified");
	window.refreshAccountData(data);
}

function submitEditSpendSuccess(data) {
	window.hideAddSpendModal();
	alert(data.length + " account(s) modified");
	window.refreshAccountData(data);
}

function getAddSpendDate() {
	const value = new Date($("#add-spend-date").val());
	const parameterDate = window.createJsDateTimeObject(value);
	const result = {
		parameter: parameterDate,
		value: value
	};
	return result;
}

function getAddSpendCurrency() {
	const selected = window.getSelectedValueInt("#add-spend-currency-select");
	return selected;
}

function getAddSpendMethodId() {
	const selected = window.getSelectedValueInt("#add-spend-method-select");
	return selected;
}

function getAddSpendType() {
	const selected = window.getSelectedValueInt("#add-spend-type-select");
	return selected;
}

function getAddSpendAccountInclude() {
	const selects = $("." + addSpendMethodsSelectClass);
	const result = new Array();
	for (let i = 0; i < selects.length; i++) {
		const select = selects[i];
		const fullId = select.id;
		const accountId = window.getIdNumFromElement(fullId);
		const methodId = window.getSelectedValueInt("#" + fullId);
		const resultItem = {
			AccountId: accountId,
			ConvertionMethodId: methodId
		};
		result.push(resultItem);
	}
	return result;
}

function validateEditSpendData(editSpendData) {
	const errorMessages = new Array();
	const parameterError = new Array();
	var result = true;
	if (!validateAddSpendType(editSpendData.SpendTypeId)) {
		result = false;
		errorMessages.push("Invalid type");
		parameterError.push(5);
	}

	if (!validateAddSpendType(editSpendData.SpendId)) {
		result = false;
		errorMessages.push("Invalid spend id");
		parameterError.push(7);
	}

	return {
		allValid: result,
		errorMessages: errorMessages,
		errorNumber: parameterError
	};
}

function validateAddSpendData(addSpendData) {
	const errorMessages = new Array();
	const parameterError = new Array();
	var result = true;
	if (!validateAddSpendDate(addSpendData.SpendDate.value)) {
		result = false;
		errorMessages.push("Invalid date");
		parameterError.push(1);
	}
	if (!validateAddSpendCurrencyId(addSpendData.CurrencyId)) {
		result = false;
		errorMessages.push("Invalid currency");
		parameterError.push(2);
	}
	if (!validateAddSpendAmount(addSpendData.Amount)) {
		result = false;
		errorMessages.push("Invalid amount");
		parameterError.push(3);
	}
	if (!validateAddSpendOriginalAccountData(addSpendData.OriginalAccountData)) {
		result = false;
		errorMessages.push("Invalid account data");
		parameterError.push(4);
	}
	if (!validateAddSpendType(addSpendData.SpendTypeId)) {
		result = false;
		errorMessages.push("Invalid type");
		parameterError.push(5);
	}
	if (!validateAddSpendAccountInclude(addSpendData.IncludedAccounts)) {
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

function validateAddSpendDate(date) {
	if (date) {
		const accountData = addSpendDataViewModel;
		const initialDate = window.moment(accountData.InitialDate).toDate();
		const endDate = window.moment(accountData.UserEndDate).toDate();
		return date >= initialDate && date <= endDate;
	} else {
		return false;
	}
}

function validateAddSpendCurrencyId(currencyId) {
	if (currencyId) {
		return currencyId > 0;
	}
	return false;
}

function validateAddSpendAmount(amount) {
	if (amount) {
		return amount > 0;
	}
	return false;
}

function validateAddSpendOriginalAccountData(accountData) {
	if (accountData && accountData.AccountId && accountData.ConvertionMethodId) {
		return accountData.AccountId > 0 && accountData.ConvertionMethodId > 0;
	}
	return false;
}

function validateAddSpendType(spendTypeId) {
	if (spendTypeId) {
		return spendTypeId > 0;
	}
	return false;
}

function validateAddSpendAccountInclude(accountIncludeList) {
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