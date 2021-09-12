///<reference path="../../jquery-2.1.0.js"/>
///<reference path="../../utils/utilities.js"/>

$(function () {

    var currentLoanRecordId = null;

    function loadAddLoanPaymentData(loanRecordId) {
        if (loanRecordId) {
            const urlParameters = {
	            loanRecordId: loanRecordId
            };

            setRegularPaymentControls();
            window.CallServiceGet("loan", "GetAddPaymentData", urlParameters, loadAddLoanPaymentDataSuccess);
        }
    };

    function loadAddLoanPaymentDataSuccess(data) {
        currentLoanRecordId = data.loanRecordId;
        window.loadDateField('#loan-payment-detail-datetime', data.accountInfo.minDate, data.suggestedDate, data.accountInfo.maxDate);
        window.loadHtmlSelect('#loan-payment-detail-currency-select', data.possibleCurrencyViewModels);
        $('#add-loan-payment-modal').modal('show');
    }

    function submitLoanPayment() {

	    const loanRecordId = currentLoanRecordId;
	    if (loanRecordId) {
            const model = getLoanPaymentData();
            const errors = getLoanPaymentValidation(model);
            if (errors && errors.length > 0) {
                handleErrors(errors);
            }
            else {
                window.callServicePost("loan", "SubmitPayment", null, model, submitLoanPaymentSuccess);
            }
        }
	}

	function submitLoanPaymentSuccess(data) {
        $('#loan-form').submit();
    }

    function handleErrors(errors) {
        if (errors && errors.length > 0) {
            let message = "";
            for (let i = 0; i < errors.length; i++) {
	            const error = errors[i];
	            message += " " + error.errorMessage;
	        }

	        alert(message);
        }
    }

    function getLoanPaymentData() {
        const amount = $('#loan-payment-detail-amount').val();
        const dateTime = $('#loan-payment-detail-datetime').val();
        const description = $('#loan-payment-detail-description').val();
        const isPending = $('#loan-payment-detail-is-pending').is(":checked");
        const currencyId = $('#loan-payment-detail-currency-select').find(":selected").val();
        const isFullPayment = isFullPaymentChecked();
        const model = {
	        Amount: isFullPayment ? 1 : amount,
	        CurrencyId: currencyId,
	        IsPending: isPending,
	        DateTime: dateTime,
	        Description: description,
	        LoanRecordId: currentLoanRecordId,
	        FullPayment: isFullPayment
        };

        return model;
    }

    function getLoanPaymentValidation(model) {
        const errors = new Array();
        if (!validLoanPaymentAmount(model.Amount)) {
            errors.push(
                {
                    field: "Amount",
                    errorMessage: "Invalid amount"
                }
            );
        }

        if (!isValidId(model.CurrencyId)) {
            errors.push(
                {
                    field: "CurrencyId",
                    errorMessage: "Invalid currency"
                });
        }

        if (!isValidId(model.LoanRecordId)) {
            errors.push(
                {
                    field: "LoanRecordId",
                    errorMessage: "Invalid Loan"
                });
        }

        return errors;
    }

    function validLoanPaymentAmount(value) {
        return isNumber(value);
    }

    function fullPaymentCheckChanged() {
	    const isChecked = isFullPaymentChecked();
	    if (isChecked) {
            loadFullPaymentData();
        } else {
            setRegularPaymentControls();
        }
	}

	function setRegularPaymentControls() {
        $('#loan-payment-detail-amount').prop('type', 'number');
        $('#loan-payment-detail-amount').prop('disabled', false);
        $('#loan-payment-detail-currency-select').prop('disabled', false);
        $('#loan-payment-detail-amount').val('');
        $('#loan-payment-detail-is-pending').prop('checked', false);
        $('#loan-payment-detail-is-pending').prop('disabled', false);
        $('#loan-payment-full-payment').prop('checked', false);
    }

    function isFullPaymentChecked() {
	    const isChecked = $('#loan-payment-full-payment').prop('checked');
	    return isChecked;
	}

	function loadFullPaymentData() {
		const parameters = {
			loanRecordId: currentLoanRecordId
		};

		window.CallServiceGet('loan', 'GetLoanDetailRecordsById', parameters, loadFullPaymentDataSuccess);
	}

	function loadFullPaymentDataSuccess(data) {
        const pendingPaymentFormatted = data.paymentPending.formatMoney('', 2, '.', ' ');
        $('#loan-payment-detail-amount').prop('type', 'text');
        $('#loan-payment-detail-amount').val(pendingPaymentFormatted);
        $('#loan-payment-detail-amount').prop('disabled', true);
        $('#loan-payment-detail-is-pending').prop('checked', false);
        $('#loan-payment-detail-is-pending').prop('disabled', true);
        $('#loan-payment-detail-currency-select').prop('disabled', true);
    }

    function startControls() {
        $('#loan-payment-full-payment').change(fullPaymentCheckChanged);
    }

    startControls();

    window.loadAddLoanPaymentData = loadAddLoanPaymentData;
    window.submitLoanPayment = submitLoanPayment;
});