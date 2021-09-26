// ReSharper disable VariableUsedInInnerScopeBeforeDeclared
$(function () {

    var ADD_TRANSFER_ACTION_TYPE = 1;
    var TRANSFER_AMOUNT_RADIO_NAME = 'transfer-amount-type-name';
    const TRANSFER_AMOUNT_CLASS = 'transfer-amount-values';
    var TRANAFER_AMOUNT_SECTION_PREFIX = TRANSFER_AMOUNT_CLASS + '-';
    

    var transferCurrentAction = 0;
    var transferCurrentAccountPeriodId;

    const executeAction = function(actionType, accountId) {
	    if (actionType && accountId) {
		    if (actionType == ADD_TRANSFER_ACTION_TYPE) {
			    const accountPeriodId = GetAccountPeriodIdbyAccountId(accountId);
			    transferCurrentAccountPeriodId = accountPeriodId;
			    addTransferAction(accountPeriodId);
			    const modal = $('#transfer-modal');
			    modal.modal('show');
			    return;
		    }
	    }
    };

    const loadControls = function() {
	    $('#transfer-destination-account-select').multiselect();
	    $('#transfer-spend-type-select').multiselect();
	    $('#transfer-amount-currency-select')
		    .multiselect({
			    nonSelectedText: 'CCY',
			    onChange:function() {
				    updateTransferPossibleAccounts();
			    }
		    });
	    const radiosSelector = 'input[name=' + TRANSFER_AMOUNT_RADIO_NAME + ']';
	    $(radiosSelector)
		    .change(function() {
			    updateTransferAmountRadioControls();
			    updateTransferPossibleAccounts();
		    });
    };

    var addTransferAction = function(accountPeriodId) {
        transferCurrentAction = ADD_TRANSFER_ACTION_TYPE;
        loadAccountFinanceViewModel(accountPeriodId);
    }

    var loadAccountFinanceViewModel = function(accountPeriodId) {
        const parameters = {
	        accountPeriodId: accountPeriodId
        };
        const method = "GetTranseferAccountFinanceViewModel";
        CallServiceGet("home", method, parameters, loadAccountFinanceViewModelSuccess, loadAccountFinanceViewModelError);
    };

    var loadAccountFinanceViewModelError = function (error) {

    }

    var loadAccountFinanceViewModelSuccess = function(data) {
        loadTransferAccountFinanceViewModelInControls(data);
    }

    var loadTransferAccountFinanceViewModelInControls = function(data) {
        cleanTransferControls();
        if (data) {
            $('#transfer-from-account-name-value').val(data.AccountName);
            $('#transfer-amount-type-period-balance-text').val(data.numPeriodBalance);
            $('#transfer-amount-type-overall-balance-text').val(data.numGeneralBalance);
            loadTransferDate(data.InitialDate,data.SuggestedDate,data.EndDate);
            loadTransferSpendTypes(data.SpendTypeViewModels);
            $('#default-transfer-amount-type-radio').prop("checked", true);
            updateTransferAmountRadioControls();
            loadTransferAmountCurrency(data.SupportedCurrencies);
            loadTransferPossibleAccount(data.SupportedAccounts);
        }
    }

    var loadTransferSpendTypes = function(spendTypes) {
        var selectedValues = null;
        const jquerySelector = "#transfer-spend-type-select";
        $(jquerySelector).empty();
        if (spendTypes) {
            for (let i = 0; i < spendTypes.length; i++) {
                const item = spendTypes[i];
                if (item.Isdefault) {
                    selectedValues = item.CurrencyId;
                }
                $(jquerySelector).append($('<option>', {
                    value: item.SpendTypeId,
                    text: item.SpendTypeName
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

    var loadTransferAmountCurrency = function(currencyList) {
        var selectedValues = null;
        const jquerySelector = "#transfer-amount-currency-select";
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

    var updateTransferPossibleAccounts = function() {
        const selectedSelector = 'input[name=' + TRANSFER_AMOUNT_RADIO_NAME + ']:checked';
        const selectedValue = $(selectedSelector).val();
        if (selectedValue && transferCurrentAccountPeriodId) {
            const currencyId = getSelectedValueInt('#transfer-amount-currency-select');
            const parameters = {
	            accountPeriodId: transferCurrentAccountPeriodId,
	            currencyId: currencyId,
	            balanceType: selectedValue
            };
            CallServiceGet('home',
                'GetPossibleDestinationAccount',
                parameters,
                updateTransferPossibleAccountsSuccess,
                updateTransferPossibleAccountsError);
        }
    }

    var updateTransferPossibleAccountsError = function(error) {
        
    }
    
    var updateTransferPossibleAccountsSuccess = function(data) {
        loadTransferPossibleAccount(data);
    }

    var loadTransferPossibleAccount = function (accountList) {
        const jquerySelector = "#transfer-destination-account-select";
        $(jquerySelector).empty();
        if (accountList) {
            $(jquerySelector).append($('<option>', {
                value: 0,
                text: 'Select'
            }));
            for (let i = 0; i < accountList.length; i++) {
	            const item = accountList[i];
	            $(jquerySelector).append($('<option>', {
                    value: item.AccountId,
                    text: item.AccountName
                }));
	        }
	        $(jquerySelector).multiselect('rebuild');
            $(jquerySelector).multiselect('select', -1);
            $(jquerySelector).multiselect('refresh');
        }
    }

    var cleanTransferControls = function() {
        $('#transfer-from-account-name-value').val('');
        $('#transfer-amount-type-custom-value').val('');
        $('#transfer-amount-currency-select').empty();
        $('#transfer-amount-type-period-balance-text').val('');
        $('#transfer-amount-type-overall-balance-text').val('');
        $('#transfer-destination-account-select').empty('');
        $('#transfer-is-pending-value')[0].checked = false;
        $('#transfer-description-value').val('');
    };

    //var loadTransferDate = function (accountData) {
    //    var dateElement = $('#transfer-datetime-value');
    //    try {

    //        var minTest = new Date(1, 1, 1, 1, 1, 1, 1);

    //        var initialDate = moment(accountData.InitialDate).toDate();
    //        var suggestedDate = moment(accountData.SuggestedDate).toDate();
    //        var endDate = moment(accountData.UserEndDate).toDate();
    //        dateElement.data("DateTimePicker").minDate(minTest);

    //        dateElement.data("DateTimePicker").maxDate(endDate);
    //        dateElement.data("DateTimePicker").minDate(initialDate);
    //        dateElement.data("DateTimePicker").date(suggestedDate);
    //    } catch (error) {
    //        var e = error;
    //    }
    //}

    function loadTransferDate(initialDateJs, suggestedDateJs, userEndDateJs) {
        const dateElement = $('#transfer-datetime-value');
        const isoFormat = "YYYY-MM-DDTHH:mm:ss";
        if (suggestedDateJs) {
            try {
                const initialDate = window.moment(initialDateJs).format(isoFormat);
                const suggestedDate = window.moment(suggestedDateJs).format(isoFormat);
                const endDate = window.moment(userEndDateJs).format(isoFormat);

                dateElement.attr('min', initialDate);
                dateElement.attr('max', endDate);
                dateElement.val(suggestedDate);
            } catch (error) {
                console.log(error);
            }
        } else {
            dateElement.attr('min', null);
            dateElement.attr('max', null);
        }
    }

    var updateTransferAmountRadioControls = function (e) {
        const selectedSelector = 'input[name=' + TRANSFER_AMOUNT_RADIO_NAME + ']:checked';
        const selectedValue = $(selectedSelector).val();
        const selectedSectionId = TRANAFER_AMOUNT_SECTION_PREFIX + selectedValue;
        $('.transfer-amount-values').hide();
        $('#' + selectedSectionId).show();
    }

    var getTransferSelectedBalanceType = function() {
        const selectedSelector = 'input[name=' + TRANSFER_AMOUNT_RADIO_NAME + ']:checked';
        const selectedValue = $(selectedSelector).val();
        return selectedValue;
    }

    function getTransferIsPending() {
        return $('#transfer-is-pending-value')[0].checked;
    }

    var getTransferDateTime = function()
    {
        const value = new Date($('#transfer-datetime-value').val());
        //var value = $('#transfer-datetime-value').data("DateTimePicker").date().toDate();
        const parameterDate = createJsDateTimeObject(value);
        const result = {
	        parameter: parameterDate,
	        value: value
        };
        return result;
    }

    var getTransferModel = function() {
        const accountPeriodId = transferCurrentAccountPeriodId;
        const description = $('#transfer-description-value').val();
        const destinationAccountId = getSelectedValueInt('#transfer-destination-account-select');
        const spendTypeId = getSelectedValueInt('#transfer-spend-type-select');
        const amount = $('#transfer-amount-type-custom-value').val();
        const amountCurrencyId = getSelectedValueInt('#transfer-amount-currency-select');
        const transferDateTime = getTransferDateTime();
        const balanceType = getTransferSelectedBalanceType();
        const isPending = getTransferIsPending();
        const model = {
	        accountPeriodId: accountPeriodId,
	        destinationAccountId: destinationAccountId,
	        spendTypeId: spendTypeId,
	        amount: amount,
	        amountCurrencyId: amountCurrencyId,
	        transferDateTime: transferDateTime,
	        balanceType: balanceType,
	        isPending: isPending,
	        description: description
        };

        return model;
    };

    var validateTransferDate = function(dateValue) {
        return !!dateValue;
    };

    var validateTransferId = function(id) {
        return !!(id && id != 0);
    }

    var validateTransferModel = function(model) {
        const errorMessages = new Array();
        const parameterError = new Array();
        var result = true;

        if (!validateTransferDate(model.transferDateTime.value)) {
            result = false;
            errorMessages.push("Invalid date");
            parameterError.push(1);
        }

        if (!validateTransferId(model.accountPeriodId)) {
            result = false;
            errorMessages.push("Invalid account Period Id");
            parameterError.push(2);
        }

        if (!validateTransferId(model.destinationAccountId)) {
            result = false;
            errorMessages.push("Invalid destinationAccountId");
            parameterError.push(3);
        }

        if (!validateTransferId(model.spendTypeId)) {
            result = false;
            errorMessages.push("Invalid spendTypeId");
            parameterError.push(4);
        }

        if (!validateTransferId(model.amountCurrencyId)) {
            result = false;
            errorMessages.push("Invalid amountCurrencyId");
            parameterError.push(5);
        }

        if (!model.balanceType) {
            result = false;
            errorMessages.push("Invalid balanceType");
            parameterError.push(6);
        }

        return {
            allValid: result,
            errorMessages: errorMessages,
            errorNumber: parameterError
        };
    }

    var getSubmitTransferModel = function (data) {
	    const model = {
		    AccountPeriodId: data.accountPeriodId,
		    DestinationAccountId: data.destinationAccountId,
		    SpendTypeId: data.spendTypeId,
		    Amount: data.amount,
		    AmountCurrencyId: data.amountCurrencyId,
		    TransferDateTime: data.transferDateTime.parameter,
		    BalanceType: data.balanceType,
		    IsPending: data.isPending,
		    Description: data.description
	    };

	    return model;
    };

    function transferHandleErrors(errorResults) {
	    const errorCount = errorResults.errorNumber.length;
	    alert(errorCount + ' error(s)');
	}

	function hideTransferModal() {
		const modal = $('#transfer-modal');
		modal.modal('hide');
	}

	const transferSubmit = function() {
	    const model = getTransferModel();
	    const validationResult = validateTransferModel(model);
	    if (validationResult.allValid) {
		    const submitModel = getSubmitTransferModel(model);
		    CallService('home', 'SubmitTransfer', submitModel, transferSubmitSuccess);
	    } else {
		    transferHandleErrors(validationResult);
	    }
    };

    var transferSubmitSuccess = function (data) {
        hideTransferModal();
        alert(data.length + ' account(s) modified');
        refreshAccountData(data);
    };

    loadControls();
    window.executeTransferAction = executeAction;
    window.submitTransferModal = transferSubmit;
});
// ReSharper restore VariableUsedInInnerScopeBeforeDeclared