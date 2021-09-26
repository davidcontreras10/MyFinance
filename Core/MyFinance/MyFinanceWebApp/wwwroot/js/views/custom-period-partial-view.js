///<reference path="../jquery-2.1.0.js"/>
///<reference path="../utils/utilities.js"/>
///<reference path="main-view-utils.js"/>

$(function () {
    var currentAccountPeriodId = null;
    var noUserOperation = false; 

    function openCustomPeriodEvent(accountId) {
        currentAccountPeriodId = null;
        const accountPeriodId = window.GetAccountPeriodIdbyAccountId(accountId);

        if (accountPeriodId == 0 || !accountPeriodId) {
            return;
        } else {
            currentAccountPeriodId = accountPeriodId;
            loadAccountPeriodData(accountPeriodId, true);
        }
    }

    function loadAccountPeriodData(accountPeriodId, defaultValues) {
        if (accountPeriodId == 0 || accountPeriodId == null) {
            return;
        }

        const parameters = {
	        accountPeriodId: accountPeriodId
        };

        if (defaultValues) {
            parameters["defaultValues"] = true;
            parameters["loanSpends"] = true;
            parameters["pendingSpends"] = false;
            parameters["amountTypeId"] = 0;
        } else {
            parameters["defaultValues"] = false;
            parameters["loanSpends"] = getLoanSpends();
            parameters["pendingSpends"] = getPendingSpends();
            parameters["amountTypeId"] = getAmountTypeId();
        }

        
        window.CallServiceGet('home', 'GetSimpleAccountFinanceViewModel', parameters, loadAccountPeriodDataSuccess);
    }

    function loadAccountPeriodDataSuccess(data) {
        clearFinanceSummary();
        if (data) {
            loadRequestParameters(data);
            if (data.accountFinanceViewModels && data.accountFinanceViewModels.length > 0) {
                loadFinanceSummary(data.accountFinanceViewModels[0]);
            }

            $('#custom-period-modal').modal('show');
        }    
    }

    function clearFinanceSummary() {
        $('#custom-period-budget-value').text('');
        $('#custom-period-spent-value').text('');
        $('#custom-period-balance-value').text('');
        $('#custom-account-general-balance-value').text('');
        $('#custom-account-today-balance-value').text('');
    }

    function loadFinanceSummary(accountData) {
        //$('#period-title-' + accountData.AccountId).text(accountData.AccountPeriodName);
        $('#custom-period-budget-value').text(accountData.numBudget);
        $('#custom-period-spent-value').text(accountData.numSpent);
        $('#custom-period-balance-value').text(accountData.numPeriodBalance);
        $('#custom-account-general-balance-value').text(accountData.numGeneralBalance);
        $('#custom-account-today-balance-value').text(accountData.numGeneralBalanceToday);
    }

    function loadRequestParameters(requestParameters) {
        if (requestParameters) {
            try {
                noUserOperation = true;
                $('#custom-period-loan-spends').prop('checked', requestParameters.loanSpends);
                $('#custom-period-pending-spends').prop('checked', requestParameters.pendingSpends);
                $('#custom-period-amount-type option:eq(' + requestParameters.amountTypeId + ')').prop('selected', true);
            }
            finally {
                noUserOperation = false;
            }

        }
    }

    function getLoanSpends() {
	    const checked = $('#custom-period-loan-spends').prop('checked');
	    return checked;
	}

	function getPendingSpends() {
		const checked = $('#custom-period-pending-spends').prop('checked');
		return checked;
	}

	function getAmountTypeId() {
		const selected = $('#custom-period-amount-type').find(":selected").val();
		return selected;
	}

	function requestParameterChanged() {
        if (noUserOperation) {
            return;
        }

        loadAccountPeriodData(currentAccountPeriodId, false);
    }

    function startControls() {
        $('#custom-period-loan-spends').change(requestParameterChanged);
        $('#custom-period-pending-spends').change(requestParameterChanged);
        $('#custom-period-amount-type').on('change', requestParameterChanged);
    }

    startControls();
    window.openCustomPeriodEvent = openCustomPeriodEvent;
});