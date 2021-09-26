// Gets

function GetAccountPeriodIdbyAccountId(accountId) {
	if (accountId) {
		const selector = '#select-control-period-' + accountId;
		const selectedValues = window.getSelectedValues(selector);
		if (selectedValues) {
			const intValue = parseInt(selectedValues);
			if (intValue > 0) {
				return intValue;
			}
		}
	}
	return 0;
}