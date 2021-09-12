$(function () {

	var initialPositions = null;
	var canUpdatePositions = false;

	const loadControls = function () {
// ReSharper disable once Html.EventNotResolved
		document.addEventListener('gridster-drag-stop-event', positionsChanged);
		$('#account-save-positions-link').addClass('disabled');
		$('#account-restore-positions-link').addClass('disabled');
		initialPositions = getGridsterAccountPositions();
		canUpdatePositions = false;
		$('.account-select')
			.multiselect({
				onChange: selectChanged
			});
	};

	var selectChanged = function () {
	    submitCurrentData();
	}

    var submitCurrentData = function() {
        const accountGroupId = geAccountGroupIdValue();
        const input = $("<input>")
	        .attr("type", "hidden")
	        .attr("name", "accountGroupId").val(accountGroupId);
        $('#account-form').append($(input));
        $('#account-form').submit();
    }

	var geAccountGroupIdValue = function() {
	    const selectedValues = getArraySelectedValues('#account-group-select');
	    if (selectedValues && selectedValues.length > 0) {
	        let value = selectedValues[0];
	        if (value == 0) {
	            value = null;
	        }

	        return value;
	    }

	    return null;
	};
    
	var accountWidgetClass = "account-widget";

	var getGridsterAccountPositions = function () {
		const result = new Array();
		const widgets = $('.' + accountWidgetClass);
		for (let i = 0; i < widgets.length; i++) {
			const widget = widgets[i];
			const dataRow = parseInt($(widget).attr('data-row'));
			const dataCol = parseInt($(widget).attr('data-col'));
			const widgetId = $(widget).attr('id');
			const accountId = parseInt(widgetId.replace((accountWidgetClass + '-'), ''));
			const gridPosition = {
				row: dataRow,
				col: dataCol,
				id: accountId
			};

			result.push(gridPosition);
		}
		return result;
	};

	var isGridsterPositionValid = function(positions) {
		if (positions) {
			if (positions.length < 2) {
				return true;
			}

			positions.sort(compareGridsterPosition);
			let rowValid = 1;
			const colValid1 = 1;
			const colValid2 = 3;
			let listPosition = 0;
			let item1 = positions[listPosition++];
			let item2 = positions[listPosition++];
			while (true) {
				if (!item1 || !item2 || item1.row != rowValid || item2.row != rowValid) {
					return false;
				}

				if (item1.col == item2.col ||
				(item1.col != colValid1 && item1.col != colValid2) ||
				(item2.col != colValid1 && item2.col != colValid2)) {
					return false;
				}

				rowValid++;
				if (positions.length <= listPosition) {
					break;
				}
				
				item1 = positions[listPosition++];
				if (positions.length <= listPosition) {
					return !(!item1 || item1.row != rowValid || (item1.col != colValid1));
				}

				item2 = positions[listPosition++];
			}
		}
		return true;
	};

	var getPositionsFromGridsterPositions= function(gridsterPositions) {
		if (!gridsterPositions) {
			return null;
		}

		gridsterPositions.sort(compareGridsterPosition);
		const positions = new Array();
		for (let i = 0; i < gridsterPositions.length; i++) {
			const item = gridsterPositions[i];
			const position = {
				AccountId: item.id,
				Position: i + 1
			};
			positions.push(position);
		}

		return positions;
	}

	var compareGridsterPosition= function(a,b) {
		if (a.row == b.row) {
			return a.col - b.col;
		}
		return  a.row - b.row;
	}
	
	const submitPositions = function () {
		if (!canUpdatePositions) {
			return;
		}
		const gridsterPositions = getGridsterAccountPositions();
		if (!isGridsterPositionValid(gridsterPositions)) {
			alert('Invalid positions');
			return;
		}

		const positions = getPositionsFromGridsterPositions(gridsterPositions);
		const parameters = {
			accountPositions:positions
		};
		CallService('Account', 'UpdateAccountPositions', parameters, submitAccountsSuccess);
	};

	var submitAccountsSuccess = function () {
	    submitCurrentData();
	}

	const restoreDefaultPositions = function() {
		submitCurrentData();
	};

	var positionsChanged = function() {
		const main = initialPositions;
		const compare = getGridsterAccountPositions();
		canUpdatePositions = arePositionDifferent(main, compare);
		if (canUpdatePositions) {
			$('#account-save-positions-link').removeClass('disabled');
			$('#account-restore-positions-link').removeClass('disabled');
		} else {
			$('#account-save-positions-link').addClass('disabled');
			$('#account-restore-positions-link').addClass('disabled');
		}
	}

	var arePositionDifferent = function(main, compare) {
		if (!main || !compare || main.length != compare.length) {
			return false;
		}

		const mainList = getPositionsFromGridsterPositions(main);
		const compareList = getPositionsFromGridsterPositions(compare);
		for (let i = 0; i < main.length; i++) {
			const mainItem = mainList[i];
			const compareItem = compareList[i];
			if (mainItem.AccountId != compareItem.AccountId) {
				return true;
			}
		}

		return false;
	}

	var validId = function (id, required) {
	    return (!required && (!id || id == 0)) || (id && isInt(id)) && id != 0;
	}

	const deleteAccount = function (accountId) {
		if (!confirm('Are you sure you want to delete this account and all its records?')) {
			return;
		}

		if (!validId(accountId, true)) {
			return;
		}

		const parameters = {
			accountId: accountId
		};

		callServicePost('Account', 'DeleteAccount', null, parameters, deleteAccountSuccess);
	};

    var deleteAccountSuccess = function() {
        submitCurrentData();
    }

    window.deleteAccount = deleteAccount;
	window.submitAccountPositions = submitPositions;
	window.restoreAccountPositions = restoreDefaultPositions;
    window.submitCurrentData = submitCurrentData;
	loadControls();
});