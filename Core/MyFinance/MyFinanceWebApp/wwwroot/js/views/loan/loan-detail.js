///<reference path="../../jquery-2.1.0.js"/>
///<reference path="../../utils/utilities.js"/>

$(function () {

    function showModal() {
        loadLoanAccounts();
    }

    function loadLoanAccounts() {
        makeControlsReady();
        window.CallServiceGet("Loan", "GetPossibleAccounts", null, loadLoanAccountsSuccess);
    }

    function loadLoanAccountsSuccess(data) {
        window.loadHtmlSelect('#loan-detail-source-account', data, true);
        window.loadDateField('#loan-detail-date', null, new Date(), null);
        $('#loan-detail-modal').modal('show');
        $('#loan-detail-source-account').focus();
        emptySourceAccount();
    }

    function sourceAccountUpdated(accountId) {
        if (accountId && accountId != "0") {
            const dateTime = getDateTime();
            const parameters = {
	            accountId: accountId,
	            dateTime: dateTime
            };

            window.CallServiceGet('loan', 'GetAddLoanData', parameters, sourceAccountUpdatedSuccess);
        } else {
            emptySourceAccount();
        }
    }

    function sourceAccountUpdatedSuccess(data) {
        loadCurrencies(data.possibleCurrencyViewModels);
        loadDateTime(data.accountInfo);
        updateDestinationAccount();
        enableControls();
    }

    function loadDateTime(accountInfo) {
        window.loadDateField('#loan-detail-date', accountInfo.minDate, new Date(), accountInfo.maxDate);
    }

    function loadCurrencies(currencies) {
        window.loadHtmlSelect("#loan-detail-currency", currencies);
    }

    function enableControls() {
        $('#loan-detail-amount').prop('disabled', false);
        $('#loan-detail-currency').prop('disabled', false);
        $('#loan-detail-name').prop('disabled', false);
        $('#loan-detail-date').prop('disabled', false);
        $('#loan-detail-description').prop('disabled', false);
    }

    function makeControlsReady() {
        cleanControls();
    }

    function cleanControls() {
        $('#loan-detail-destination-account').empty();
        $('#loan-detail-amount').val('');
        $('#loan-detail-currency').empty();
        $('#loan-detail-name').val('');
        $('#loan-detail-description').val('');
        $('#loan-detail-same-as-source').prop('checked', false);
    }

    function updateDestinationAccount() {
        $('#loan-detail-destination-account').prop('disabled', true);
        const currencyId = getCurrencyId();
        const accountId = getSourceAccountId();
        const dateTime = getDateTime();
        const sameAsSource = getSameAsSource();
        if (!sameAsSource && window.isValidId(currencyId) && window.isValidId(accountId)) {
	        const parameters = {
		        accountId: accountId,
		        dateTime: dateTime,
		        currencyId: currencyId
	        };

	        window.CallServiceGet('loan',
                'GetPossibleDestinationAccount',
                parameters,
                updateDestinationAccountSuccess);
        } else {
            if (sameAsSource) {
                $('#loan-detail-destination-account').empty();
            }
        }
    }

    function updateDestinationAccountSuccess(data) {
        $('#loan-detail-destination-account').empty();
        if (data && data.length > 0) {
            $('#loan-detail-destination-account').prop('disabled', false);
            window.loadHtmlSelect('#loan-detail-destination-account', data, true);
        }
    }

    function emptySourceAccount() {
        $('#loan-detail-destination-account').empty();
        $('#loan-detail-destination-account').prop('disabled', true);

        $('#loan-detail-amount').val('');
        $('#loan-detail-amount').prop('disabled', true);

        $('#loan-detail-currency').empty();
        $('#loan-detail-currency').prop('disabled', true);

        $('#loan-detail-name').val('');
        $('#loan-detail-name').prop('disabled', true);

        $('#loan-detail-date').prop('disabled', false);

        $('#loan-detail-description').val('');
        $('#loan-detail-description').prop('disabled', true);
    }

    function startControls() {
        $('#loan-detail-source-account').change(sourceAccountChanged);
        $('#loan-detail-currency').change(updateDestinationAccount);
        $('#loan-detail-same-as-source').change(updateDestinationAccount);
    }

    function sourceAccountChanged() {
	    const selectedId = getSourceAccountId();
	    if (selectedId) {
            sourceAccountUpdated(selectedId);
        } else {
            emptySourceAccount();
        }
	}

	function submitAddLoan() {
        const model = getAddLoanModel();
        const errors = isValidAddLoanModel(model);
        if (errors && errors.length > 0) {
            handleErrors(errors);
        }
        else {
            window.callServicePost('loan', 'SubmitLoan', null, model, submitAddLoanSuccess);
        }
    }

    function submitAddLoanSuccess(data) {
        location.reload();
    }

    function isValidAddLoanModel(model) {
        const errors = new Array();
        if (!isLoanNameValid(model.LoanName)) {
            errors.push({
                field: "LoanName",
                errorMessage: "Invalid loan name"
            });
        }

        if (!isAmountValid(model.Amount)) {
            errors.push({
                field: "Amount",
                errorMessage: "Invalid amount"
            });
        }

        if (!isSourceAccountIdValid(model.AccountId)) {
            errors.push({
                field: "AccountId",
                errorMessage: "Invalid account"
            });
        }

        if (!model.SameAsSource && !isDestinationAccountIdValid(model.DestinationAccountId)) {
            errors.push({
                field: "AccountId",
                errorMessage: "Invalid account"
            });
        }

        if (!window.isValidId(model.CurrencyId)) {
            errors.push({
                field: "CurrencyId",
                errorMessage: "Invalid currency"
            });
        }

        return errors;
    }

    function getAddLoanModel() {
        const accountId = getSourceAccountId();
        const destinationAccountId = getDestinationAccountId();
        const amount = getAmount();
        const loanName = getLoanName();
        const sameAsSource = getSameAsSource();
        const currencyId = getCurrencyId();
        const description = getDescription();
        const dateTime = getDateTime();
        const model = {
	        Amount: amount,
	        AccountId: accountId,
	        DestinationAccountId: destinationAccountId,
	        LoanName: loanName,
	        SameAsSource: sameAsSource,
	        CurrencyId: currencyId,
	        Description: description,
	        SpendDate: dateTime
        };

        return model;
    }

    function isAmountValid(value) {
	    const isValid = isNumber(value);
	    return isValid;
	}

	function isLoanNameValid(value) {
		const valid = value && value.length > 2;
		return valid;
	}

	function isSourceAccountIdValid(value) {
        return window.isValidId(value);
    }

    function isDestinationAccountIdValid(value) {
        return window.isValidId(value);
    }

    function getDescription() {
	    const value = $('#loan-detail-description').val();
	    return value;
	}

	function getAmount() {
		const value = $('#loan-detail-amount').val();
		return value;
	}

	function getLoanName() {
		const value = $('#loan-detail-name').val();
		return value;
	}

	function getSourceAccountId() {
		const value = $('#loan-detail-source-account').find(":selected").val();
		return value;
	}

	function getDestinationAccountId() {
		const value = $('#loan-detail-destination-account').find(":selected").val();
		return value;
	}

	function getCurrencyId() {
		const id = $('#loan-detail-currency').find(":selected").val();
		return id;
	}

	function getDateTime() {
		const date = $('#loan-detail-date').val();
		return date;
	}

	function getSameAsSource() {
		const checked = $('#loan-detail-same-as-source').prop('checked');
		return checked;
	}

	function handleErrors(errors) {
        alert('Input errors found');
    }

    startControls();
    window.showLoanDetailModal = showModal;
    window.submitAddLoan = submitAddLoan;
});