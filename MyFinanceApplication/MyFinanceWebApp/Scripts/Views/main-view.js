/// <reference path="../jquery-1.9.1.intellisense.js"/>
/// <reference path="../bootstrap-datetimepicker.js" />
/// <reference path="../bootstrap-multiselect.js" />
/// <reference path="../utils/utilities.js" />
/// <reference path="main-view-utils.js" />
/// <reference path="add-spend-view.js" />

var showPendingData = true;

function loadSpendDetailControls() {
	$("#spend-detail-date").datetimepicker();
}


function loadControls() {
	$("#accordion").collapse();
	loadSpendDetailControls();
	loadAddSpendControls();
	$("#select-control-period").multiselect({
		onChange: function () {
			
		}
	});
	$(".datetimepicker-control").datetimepicker({
		defaultDate: new Date()
		//format: 'MM/DD/YYYY HH:mm:ss'
	});

	$(".datepicker-control").datetimepicker({
		//defaultDate: new Date()
		format: "MM/DD/YYYY"
	});

	$(".select-spend-type").multiselect();
	$(".select-control").multiselect();
	$(".select-control-period").multiselect({
		buttonText: function () {
			return "Change Period";
		},
		onChange: function (option) {
			const val = option.val();
			const valArray = new Array();
			valArray.push(val);
			loadAccountFinanceData(valArray);
		}
	});
	$("#add-period-submit").click(function () {

	});
}


// Initial

function loadInitialView() {
	const currentAccountPeriodIds = GetCurrentAccountPeriods();
    loadAccountFinanceData(currentAccountPeriodIds);
    loadSummaryFinanceData(null);
}


//Validation

function addSpendValidate(spendType, date, amount) {
	const errorMessages = [];
	if (!spendType || spendType <= 0) {
		errorMessages.push("Invalid spending type");
	}
	if (!date) {
		errorMessages.push("Invalid date");
	}
	if (!amount || amount <= 0) {
		errorMessages.push("Invalid amount");
	}
	return errorMessages;
}

function addActionColumns(data) {
	if (data && data.Header) {
		//adds headers
		data.Header.Cells.push(window.createCellObject("Actions"));

		//adds action html
		for (let i = 0; i < data.Rows.length; i++) {
			const row = data.Rows[i];
			const object = window.createCellObject(actionsHtml(row.RowId));
			row.Cells.push(object);
		}
	}
}

function actionsHtml(index) {
	return `<div style='text-align: center;'> <span onclick='deleteAction(${index})' class='delete-action-text'>X</span></div>`;
}

function validatePeriodForm() {
	return true;
}

function openPeriodModal(accountId) {
	const parameters = {
		accountId: accountId
	};
	window.CallService("home", "GetAddPeriodData", parameters, openPeriodModalSuccess);
}

function openPeriodModalSuccess(data) {
	if (data && data.AccountId !== 0 && data.IsValid) {
		const accountId = data.AccountId;
		const budget = data.Budget;
		let initialDate = data.NextInitialDate;
		initialDate = moment(initialDate).format("MM/DD/YYYY");
		$("#hidden-accountId").val(accountId);
		const dateControl = $("#datetimepicker-period-modal-initial");
		if (data.HasPeriods) {
			dateControl.val(initialDate);
			dateControl.prop("readonly", "readonly");
		} else {
			dateControl.val(initialDate);
			dateControl.removeAttr("readonly");
		}
		$("#input-period-modal-budget").val(budget);
		$("#myModal").modal("show");
	} else {
		if (data && !data.IsValid) {
			alert("Invalid account for custom period");
		}
	}
}

function showAddSpendModal() {
	const modal = $("#add-spend-modal");
	modal.modal("show");
	//$('#add-spend-amount-value').focus(10);
}

function showEditSpendModal() {
	const modal = $("#edit-spend-modal");
	modal.modal("show");
}

function hideAddSpendModal() {
	const modal = $("#add-spend-modal");
	modal.modal("hide");
}

//Actions

function refreshAccountData(modifiedAccounts) {
	const accountPeriodIds = new Array();
	for (let i = 0; i < modifiedAccounts.length; i++) {
		const accountId = modifiedAccounts[i].AccountId || modifiedAccounts[i].accountId;
		const accountPeriodId = window.GetAccountPeriodIdbyAccountId(accountId);
		accountPeriodIds.push(accountPeriodId);
	}

    loadAccountFinanceData(accountPeriodIds);
    loadSummaryFinanceData(modifiedAccounts);
}

function getModifiedPeriodIds(modifiedAccounts) {
	if (modifiedAccounts) {
		let accountPeriodIds = "";
		for (let i = 0; i < modifiedAccounts.length; i++) {
			const modifiedAccountItem = modifiedAccounts[i];
			const accountId = modifiedAccountItem.AccountId;
			const accountPeriodId = window.GetAccountPeriodIdbyAccountId(accountId);
			accountPeriodIds += accountPeriodId + "";
			if (i !== modifiedAccounts.length - 1) {
				accountPeriodIds += ",";
			}
		}
		return accountPeriodIds;
	}
	return "";
}


function switchSpendValues(id1, id2) {
	const visibleClass = "spend-amount-visible";
	const hiddenClass = "spend-amount-hidden";
	const class1 = $(`#${id1}`).attr("class");
	if (class1.indexOf(visibleClass) >= 0) {
		$(`#${id1}`).removeClass(visibleClass).addClass(hiddenClass);
		$(`#${id2}`).removeClass(hiddenClass).addClass(visibleClass);
	} else {
		$(`#${id2}`).removeClass(visibleClass).addClass(hiddenClass);
		$(`#${id1}`).removeClass(hiddenClass).addClass(visibleClass);
	}
	
}

function loadSummaryFinanceData(accountIds) {
    const method = "GetAccountsSummary";
    const parameters = {};
    parameters["accountIds"] = accountIds;
    $("#accounts-summary").addClass("loading-element");
    window.CallServiceGet("Home", method, parameters, loadSummaryFinanceDataSuccess, loadSummaryFinanceDataError);
}

function loadSummaryFinanceDataSuccess(responseData) {
    $("#accounts-summary").removeClass("loading-element");
    loadSummaryFinanceTable(responseData);
}

function loadSummaryFinanceDataError() {
    $("#accounts-summary").removeClass("loading-element");
}

function loadAccountFinanceData(accountPeriodIds) {
	const method = "GetAccountFinanceViewModel";
	const parameters = {};
	parameters["isPending"] = showPendingData;
	parameters["accountPeriodIds"] = accountPeriodIds;
	window.CallServiceGet("Home", method, parameters, loadAccountFinanceDataSuccess, loadAccountFinanceDataError);
}


function loadAccountFinanceDataSuccess(data) {
	if (data) {
		updateShowPendingHeader();
		for (let i = 0; i < data.length; i++) {
			const accountData = data[i];
			LoadFinanceBox(accountData);
		}
	}
}   

function loadAccountFinanceDataError() {
	
}

function LoadFinanceBox(accountData) {
	if (accountData) {
		$(`#period-title-${accountData.AccountId}`).text(accountData.AccountPeriodName);
		$(`#period-budget-value-${accountData.AccountId}`).text(accountData.numBudget);
		$(`#period-spent-value-${accountData.AccountId}`).text(accountData.numSpent);
		$(`#period-balance-value-${accountData.AccountId}`).text(accountData.numPeriodBalance);
		$(`#account-general-balance-value-${accountData.AccountId}`).text(accountData.numGeneralBalance);
		$(`#account-today-balance-value-${accountData.AccountId}`).text(accountData.numGeneralBalanceToday);
		loadFinanceSpendTable(accountData);
	}
}



function loadFinanceSpendTable(accountData) {
	if (accountData) {
		addActionColumns(accountData.SpendTable);
		CreateBootstrapTable(`#table-details-${accountData.AccountId}`, accountData.SpendTable);
	}
}

function addPeriodSubmit() {
	if (validatePeriodForm()) {
		$("#myModal").hide();
		$("#add-period-form").submit();
	}
}

function deleteAction(rowId) {
	if (confirm("Are you sure you want to delete this item?")) {
		const parameter = {
			spendId: rowId
		};
		CallService("Home", "DeleteSpend", parameter, deleteActionSuccess);
	}
}

function deleteActionSuccess(data) {
	refreshAccountData(data);
}

function editAction(index) {
	if (index !== 0) {
		alert("Ups! This function has not been yet implemented on this platform. Sorry for the inconvenience");
	}    
}


function createSpendListTable(accountId, data) {
	addActionColumns(data);
	const tableSelector = `#table-details-${accountId}`;
	$(tableSelector).empty();
	CreateBootstrapTable(tableSelector, data);
}

function cleanAccountBox(accountId) {
	$(`#period-title-${accountId}`).html(`<a class="action-link" onclick="openPeriodModal(${accountId})">No Period - Create</a>`);
	$(`#period-budget-value-${accountId}`).text("");
	$(`#period-spent-value-${accountId}`).text("");

	$(`#period-balance-value-${accountId}`).text("");
	$(`#account-general-balance-value-${accountId}`).text("N/A");
	$(`#table-details-${accountId}`).empty();
}

function updatedStatusAccountBoxAdd(accountId, disabled) {
	var selectStatus;
	if (disabled) {
		selectStatus = "disable";
	} else {
		selectStatus = "enable";
	}
	$(`#amount-field-${accountId}`).prop("disabled", disabled);

	$(`#datetimepicker-control-${accountId}`).prop("disabled", disabled);

	$(`#save-button-${accountId}`).prop("disabled", disabled);
	$(`#details-button-${accountId}`).prop("disabled", disabled);

	$(`#select-spend-type-${accountId}`).multiselect(selectStatus);
	$(`#select-other-periods-${accountId}`).multiselect(selectStatus);
}

function showHideSpendList(accountId) {
	const displayStatus = $(`#details-container-${accountId}`).css("display");
	if (displayStatus === "inline-block" || displayStatus === "block" || displayStatus === "table") {
		$(`#details-container-${accountId}`).css("display", "none");
		$(`#details-button-${accountId}`).prop("value", "Show spending list");
	} else {
		$(`#details-container-${accountId}`).css("display", "block");
		$(`#details-button-${accountId}`).prop("value", "Hide spending list");
	}
}

function showHideSummaryFinanceTable() {
    const isDisplaying = $("#accounts-summary").is(":visible");
    if (isDisplaying) {
        $("#accounts-summary").hide();
    } else {
        $("#accounts-summary").show().css("display", "inline-block");
    }
}

function updatePendingDataStatus() {
	showPendingData = !showPendingData;
	loadInitialView();
}

function updateShowPendingHeader() {
	const header = $("#show-pending-data-button");
	if (showPendingData) {
		header.text("Hide pending data");
	}
	else {
		header.text("Show pending data");
	}
}

function loadSummaryFinanceTable(summaryData) {
    recreateAccountSummaryElements(summaryData.summary);
}

function recreateAccountSummaryElements(accountsData) {
    recreateAccountSummaryTables(accountsData);
}

function recreateAccountSummaryTables(accountsData) {
    $("#accounts-summary").empty();
    if (accountsData && accountsData.length > 0) {
        let tables = "";
        for (let i = 0; i < accountsData.length; i++) {
            const accountData = accountsData[i];
            let div = `<div class='summary-account-div' id='account-summary-row-${accountData.accountId}'>`;
            div += `<span>${accountData.accountName}: </span>`;
            div += `<span>${accountData.balance.asString}</span>`;
            div += "</div>";
            tables += div;
        }

        tables += "</br>";
        $("#accounts-summary").append(tables);
    }
}

function recreateAccountSummaryTable(accountsData) {
    $("#accounts-summary").empty();
    if (accountsData && accountsData.length > 0) {
        let table = "<table class='table' style='max-width: 500px;'>";
        table += getAccountSummaryTableHeader();
        let tbody = "<body>";
        for (let i = 0; i < accountsData.length; i++) {
            const accountData = accountsData[i];
            tbody += getAccountSummaryRow(accountData);
        }

        tbody += "</body>";
        table += tbody;
        table += "</table>";
        $("#accounts-summary").append(table);
    }
}

function getAccountSummaryRow(accountData) {
    let row = `<tr id='account-summary-row-${accountData.accountId}'>`;
    row += `<td>${accountData.accountName}</td>`;
    row += `<td>${accountData.balance.asString}</td>`;
    row += "</tr>";
    return row;
}

function getAccountSummaryTableHeader() {
    const columns = ["Account Name", "Summary Balance"];
    let thead = "<thead>";
    for (let i = 0; i < columns.length; i++) {
        const column = columns[i];
        thead += "<th scope='col'>";
        thead += column;
        thead += "</th>";
    }

    thead += "</thead>";
    return thead;
}

function downloadPeriod(accountId) {
	const accountPeriodId = GetAccountPeriodIdbyAccountId(accountId);
	const urlParameters = {
		accountPeriodId: accountPeriodId,
		isPending: showPendingData
	}
	const url = window.CreateUrl('home', 'GetAccountFileAsync', urlParameters);
	$.ajax({
		type: "GET",
		url: url,
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: function (data) {
			//Convert Base64 string to Byte Array.
			const bytes = Base64ToBytes(data.bytes);
			const fileName = data.fileName;
			//Convert Byte Array to BLOB.
			const blob = new Blob([bytes], { type: "application/octetstream" });

			//Check the Browser type and download the File.
			const isIE = false || !!document.documentMode;
			if (isIE) {
				window.navigator.msSaveBlob(blob, fileName);
			} else {
				const url = window.URL || window.webkitURL;
				const link = url.createObjectURL(blob);
				const a = $("<a />");
				a.attr("download", fileName);
				a.attr("href", link);
				$("body").append(a);
				a[0].click();
				$("body").remove(a);
			}
		},
		error: function(error) {
			console.error(error);
		}
	});
}

//Internal

function Base64ToBytes(base64) {
	const s = window.atob(base64);
	const bytes = new Uint8Array(s.length);
	for (let i = 0; i < s.length; i++) {
		bytes[i] = s.charCodeAt(i);
	}
	return bytes;
}

function GetCurrentAccountPeriods() {
	return getArraySelectedValues(".select-control-period");
}

//LOAD
$(function () {
	window.downloadPeriod = downloadPeriod;
	loadControls();
	loadInitialView();
	//initialTest();
	//showAddSpendModal();
});

//TEST

function initialTest() {
	$("#spend-detail-modal").modal("show");
}