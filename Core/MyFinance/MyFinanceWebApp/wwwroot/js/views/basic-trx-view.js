///<reference path="../jquery-2.1.0.js"/>
///<reference path="./../moment.js"/>
///<reference path="../utils/utilities.js"/>
///<reference path="../Views/main-view.js"/>

$(function () {
	const addIncomeCode = 2;
	const addSpendCode = 1;
	const inputTrxAmount = $("#add-trx-amount");
	const selectTrxCurrency = $("#add-trx-currency");
	const selectTrxType = $("#add-trx-type");
	const inputTrxDescription = $("#add-trx-description");
	const inputTrxDateTime = $("#add-trx-date");
	const checkboxTrxPending = $("#add-trx-is-pending");
	let addTrxData;
	let currentAccountPeriodId;
	let currentActionTypeId;

	//Actions

	function submitAddTrx() {
		const model = getTrxFormData();
		const errors = validateAddTrxData(model);
		if (errors && errors.length > 0) {
			handleErrors(errors);
		} else {
			model.spendDate = model.spendDate.parameter;
			model.accountPeriodId = currentAccountPeriodId;
			const method = currentActionTypeId === addSpendCode ? "AddBasicSpend" : "AddBasicIncome";
			callServicePost("home", method, null, model, submitAddTrxFormSuccess);
		}
	}

	function submitAddTrxFormSuccess(data) {
		alert(data.length + " account(s) modified");
		hideModal();
		refreshAccountData(data);
	}

	//Data loading

	function loadAddTrxDataFromSource() {
		const method = "GetAddTrxViewModel";
		const parameters = {};
		parameters["accountPeriodId"] = currentAccountPeriodId;
		CallServiceGet("home", method, parameters, loadAddTrxDataFromSourceSuccess);
	}

	function loadAddTrxDataFromSourceSuccess(data) {
		addTrxData = data;
		setTrxDataForm(data);
		setTrxModalHeader(currentActionTypeId);
		showModal();
		inputTrxAmount.focus();
	}

	//Error handling
	function handleErrors(errorsArray) {
		let messages = "";
		for (let i = 0; i < errorsArray.length; i++) {
			const error = errorsArray[i];
			messages += `${error.message}\n`;
		}

		alert(messages);
	}

	//Data Validation

	function validateAddTrxData(model) {
		const errorResults = new Array();
		if (!isValidAmount(model.amount)) {
			errorResults.push({
				id: 1,
				message: "Invalid amount"
			});
		}

		if (!isValidTrxDate(model.spendDate, addTrxData.initialDate, addTrxData.userEndDate)) {
			errorResults.push({
				id: 2,
				message: "Invalid date"
			});
		}

		if (!isValidId(model.spendTypeId)) {
			errorResults.push({
				id: 3,
				message: "Invalid spend type"
			});
		}

		if (!isValidId(model.currencyId)) {
			errorResults.push({
				id: 4,
				message: "Invalid currency"
			});
		}

		return errorResults;
	}

	function isValidAmount(amount) {
		return window.isNumber(amount) && amount > 0;
	}

	function isValidTrxDate(date, serverInitial, serverEnd) {
		if (!date || !date.value) {
			return false;
		}

		if (serverInitial) {
			const initialDate = moment(serverInitial).toDate();
			if (date.value < initialDate) {
				return false;
			}
		}

		if (serverEnd) {
			const endDate = moment(serverEnd).toDate();
			if (date.value > endDate) {
				return false;
			}
		}

		return true;
	}

	//Form Read Data
	function getTrxFormData() {
		return {
			amount: getTrxAmount(),
			spendDate: getTrxDate(),
			spendTypeId: getTrxSpendType(),
			currencyId: getTrxCurrency(),
			description: getTrxDescription(),
			isPending: getTrxIsPending()
		};
	}

	function getTrxIsPending() {
		return checkboxTrxPending.prop("checked");
	}

	function getTrxDescription() {
		return inputTrxDescription.val();
	}

	function getTrxCurrency() {
		const selected = window.getSelectedValueInt("#add-trx-currency");
		return selected;
	}

	function getTrxSpendType() {
		const selected = window.getSelectedValueInt("#add-trx-type");
		return selected;
	}

	function getTrxDate() {
		return window.readDateField("#add-trx-date");
	}

	function getTrxAmount() {
		const value = inputTrxAmount.val();
		if (window.isNumber(value)) {
			return parseFloat(value);
		}
		return 0;
	}

	//Form Set Data
	function setTrxDataForm(data) {
		cleanTrxForm();
		if (!data) {
			return;
		}

		inputTrxAmount.val(data.convertedAmount);
		inputTrxDescription.val(data.description);
		setTrxTypes(data.spendTypeViewModels);
		setTrxCurrencies(data.supportedCurrencies);
		setTrxDateTime(data);
	}

	function setTrxTypes(trxTypes) {
		window.loadHtmlSelect("#add-trx-type", trxTypes, false);
	}

	function setTrxCurrencies(currencies) {
		window.loadHtmlSelect("#add-trx-currency", currencies, false);
	}

	function setTrxDateTime(dateRange) {
		window.setDateTimeInput("#add-trx-date", dateRange.initialDate, dateRange.suggestedDate, dateRange.userEndDate);
	}

	function cleanTrxForm() {
		inputTrxAmount.val("");
		selectTrxCurrency.empty();
		selectTrxType.empty();
		inputTrxDescription.val("");
		inputTrxDateTime.val(null);
		checkboxTrxPending.prop("checked", false);
	}

	function setTrxModalHeader(actionType) {
		const spendColor = "#c19393";
		const incomeColor = "#94c193";
		const editColor = "#f9d709";

		const spendMessage = "New Expense";
		const incomeMessage = "Add Income";
		const editMessage = "Spend Details";

		var color = "";
		var message = "";

		if (actionType === addSpendCode) {
			color = spendColor;
			message = spendMessage;
		}

		if (actionType === addIncomeCode) {
			color = incomeColor;
			message = incomeMessage;
		}

		if (actionType === editSpendCode) {
			color = editColor;
			message = editMessage;
		}

		$("#add-trx-title").text(message);
		$("#add-trx-modal-header").css("background", color);
	}

	//others
	function hideModal() {
		const modal = $("#add-trx-modal");
		modal.modal("hide");
	}

	function showModal() {
		const modal = $("#add-trx-modal");
		modal.modal("show");
	}

	//Initial
	function initiateWindow(accountId, actionTypeId) {
		if (!accountId || !actionTypeId) {
			return;
		}

		currentAccountPeriodId = window.GetAccountPeriodIdbyAccountId(accountId);
		currentActionTypeId = actionTypeId;
		cleanTrxForm();
		loadAddTrxDataFromSource();
	}

	function initialize() {
		$("#add-trx-is-pending-div").click(function (e) {
			e.stopPropagation();
			const isChecked = checkboxTrxPending.prop("checked");
			checkboxTrxPending.prop("checked", !isChecked);
		});

		checkboxTrxPending.click(function(e) {
			e.stopPropagation();
		});
	}

	initialize();
	window.openAddBasicTrxModal = initiateWindow;
	window.submitAddBasicTrx = submitAddTrx;
});