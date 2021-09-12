///<reference path="../../jquery-2.1.0.js"/>
///<reference path="../../utils/utilities.js"/>


$(function () {
    const deleteLoan = function (loanRecordId) {
	    if (window.isValidId(loanRecordId)) {
		    if (confirm('This will delete all the transactions related to the selected loan. Do you want to proceed?')
		    ) {
			    const parameters = {
				    loanRecordId: loanRecordId
			    };

			    window.callServicePost('loan', 'deleteLoan', parameters, null, deleteLoanSuccess);
		    }
	    }
    };

    var deleteLoanSuccess = function (data) {
        console.log(data);
        location.reload();
    }

    const deleteLoanSpend = function(spendId) {
	    if (window.isValidId(spendId)) {
		    if (confirm('This will delete all the transactions related to this payment. Do you want to proceed?')
		    ) {
			    const parameters = {
				    spendId: spendId
			    };

			    window.callServicePost('loan', 'deleteSpend', parameters, null, deleteLoanSpendSuccess);
		    }
	    }
    };

    var deleteLoanSpendSuccess = function(data) {
        console.log(data);
        location.reload();
    }

    window.deleteLoan = deleteLoan;
    window.deleteLoanSpend = deleteLoanSpend;
});