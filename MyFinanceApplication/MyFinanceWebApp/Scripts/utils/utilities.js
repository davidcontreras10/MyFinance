///<reference path="../jquery-1.9.1.intellisense.js"/>
///<reference path="../moment.js"/>

function CallService(controller, method, parameters, onSuccess, errorFunction) {
	const serviceUrl = CreateUrl(controller, method);
	//var serviceUrl = method;
	showLoadingModal(null);
	try {
		$.ajax({
			type: "POST",
			url: serviceUrl,
			data: parameters,
			dataType: "json",
			success: successFunc,
			error: errorFunc
		});
	} catch (e) {
		hideLoadingModal(null);
		throw e;
	}

	function successFunc(data, status) {
		hideLoadingModal(null);
		if (onSuccess) {
			onSuccess(data);
		}
	}

	function errorFunc(e, data) {
		hideLoadingModal(null);
		const jsonAuthError = IsAuthError(e);
		if (jsonAuthError) {
			handleAuthError(e);
		} else {
			alert("error");
			if (errorFunction) {
				errorFunction(data);
			}
		}
	}
}

function callServicePost(controller, method, urlParameters, model, onSuccess, errorFunction) {
	const serviceUrl = CreateUrl(controller, method, urlParameters);
	//var serviceUrl = method;
	showLoadingModal(null);
	try {
		$.ajax({
			type: "POST",
			url: serviceUrl,
			data: model,
			dataType: "json",
			success: successFunc,
			error: errorFunc
		});
	} catch (e) {
		hideLoadingModal(null);
		throw e;
	}

	function successFunc(data, status) {
		hideLoadingModal(null);
		if (onSuccess) {
			onSuccess(data);
		}
	}

	function errorFunc(e, data) {
		serviceErrorFunc(e, data, errorFunction);
	}
}

function CallServiceGet(controller, method, parameters, onSuccess, errorFunction) {
	const serviceUrl = CreateUrl(controller, method, parameters);
	showLoadingModal(null);
	try {
		$.ajax({
			type: "GET",
			url: serviceUrl,
			success: successFunc,
			error: errorFunc
		});
	} catch (e) {
		hideLoadingModal(null);
		throw e;
	}

	function successFunc(data, status) {
		hideLoadingModal(null);
		if (onSuccess) {
			onSuccess(data);
		}
	}

	function errorFunc(e, data) {
		serviceErrorFunc(e, data, errorFunction);
	}
}

function serviceErrorFunc(e, data, errorFunction) {
	hideLoadingModal(null);
	const jsonAuthError = IsAuthError(e);
	if (jsonAuthError) {
		handleAuthError(e);
	}
	else if (IsModelStateError(e)) {
		handleModelStateError(e);
	}
	else {
		alert("error");
		if (errorFunction) {
			errorFunction();
		}
	}
}

function CreateUrl(controller, method, urlParameters) {
	if (!controller) {
		controller = + "home/";
	}

	if (!window.siteRoot) {
		throw "siteRoot value not set";
	}

	var root = window.siteRoot;
	if (root === "/") {
		root = "";
	}

	const url = root + "/" + controller + "/" + method + CreateUrlParameter(urlParameters);
	return url;
}

function showLoadingModal(modalSelector) {
	if (!modalSelector) {
		modalSelector = "#ajax-loading-modal";
	}

	const modal = $(modalSelector);
	if (modal.modal) {
		modal.modal({
			backdrop: "static"
		});
	}
}

function hideLoadingModal(modalSelector) {
	if (!modalSelector) {
		modalSelector = "#ajax-loading-modal";
	}

	const modal = $(modalSelector);
	if (modal.modal) {
		modal.modal("hide");
		modal.hide();
	}
}

function CreateUrlParameter(parameters) {
	var result = "";
	if (parameters) {
		result += "?";
		let pars = "";
		for (key in parameters) {
            if (parameters[key] || parameters[key] === 0 || typeof (parameters[key]) == typeof (true)) {
				if (parameters[key].constructor === Array) {
					pars += "&" + CreateArrayParameter(key, parameters[key]);
				} else {
					pars += "&" + key + "=" + parameters[key];
				}
			}
		}

		if (pars && pars[0] === "&") {
			pars = pars.slice(1);
			result += pars;
			return result;
		}
	}
	return result;
}

function CreateArrayParameter(key, array) {
	if (!array || array.constructor != Array || array.length == 0) {
		return "";
	}

	var pars = "";
	for (let i = 0; i < array.length; i++) {
		const item = array[i];
		pars += "&" + key + "=" + item;
	}

	if (pars && pars[0] == "&") {
		pars = pars.slice(1);
		return pars;
	}

	return pars;
}

function handleModelStateError(error) {
	const jsonStr = error.responseText;
	if (!IsJsonString(jsonStr))
		return;
	const modelStateError = JSON.parse(jsonStr);
	const message = createModelStateMessage(modelStateError.modelState);
	alert(message);
}

function createModelStateMessage(modelState) {
	var message = "";
	for (let property in modelState) {
		if (modelState.hasOwnProperty(property)) {
			const errors = modelState[property];
			for (let i = 0; i < errors.length; i++) {
				const error = errors[i];
				message += error;
				message += " ";
			}
		}
	}

	return message;
}

function IsModelStateError(error) {
	if (!error)
		return false;
	const jsonStr = error.responseText;
	if (!IsJsonString(jsonStr))
		return false;
	const modelError = JSON.parse(jsonStr);
	return modelError.errorCode == 5000;
}

function handleAuthError(error) {
	const jsonStr = error.responseText;
	if (!IsJsonString(jsonStr))
		return false;
	const authError = JSON.parse(jsonStr);
	document.location = authError.actionUrl;
	window.location.replace(authError.actionUrl);
	return true;
}

function IsAuthError(error) {
	if (!error)
		return false;
	const jsonStr = error.responseText;
	if (!IsJsonString(jsonStr))
		return false;
	const authError = JSON.parse(jsonStr);
	return authError.errorCode == 2000;
}

function IsJsonString(str) {
	try {
		const jsonResult = JSON.parse(str);
		return jsonResult;
	} catch (e) {
		return false;
	}
}

function setDateValue(jquerySelector, value) {
	if (window.dateFieldLockFlag == null) {
		window.dateFieldLockFlag = new Array();
	}
	const array = window.dateFieldLockFlag;
	const dateElement = $(jquerySelector);
	array[jquerySelector] = true;
	dateElement.data("DateTimePicker").date(value);
	array[jquerySelector] = false;
}

function createCellObject(text) {
	return {
		Value: text
	};
}

function getStringBetweenValues(str, firstValue, secondValue) {
	const result = str.substring(str.lastIndexOf(firstValue) + firstValue.length, str.lastIndexOf(secondValue));
	return result;
}

//MM/DD/YYYY HH:mm tt
function getDateForParameter(date) {
	const dateObject = stringToDatetimeUs(date);
	return getDateForParameterDateObject(dateObject);
}

function getDateForParameterDateObject(date) {
	const dateObject = date;
	var dateString = createDateObject(dateObject);
	dateString = JSON.stringify(dateString);
	return dateString;
}

function createJsDateTimeObject(date) {
	const dd = date.getDate();//yields day 
	const MM = date.getMonth() + 1;//yields month 
	const yyyy = date.getFullYear(); //yields year 
	const HH = date.getHours();//yields hours  
	const mm = date.getMinutes();//yields minutes 
	const ss = date.getSeconds();//yields seconds After this construct a string with the above results as below
	const dateObject = {
		Day: dd,
		Month: MM,
		Year: yyyy,
		Hours: HH,
		Minutes: mm,
		Seconds: ss
	};
	return dateObject;
	//return dd + "/" + MM + "/" + yyyy + " " + HH + ':' + mm + ':' + ss;
}

function createDateObject(date) {
	const dd = date.getDate();//yields day 
	const MM = date.getMonth() + 1;//yields month 
	const yyyy = date.getFullYear(); //yields year 
	const HH = date.getHours();//yields hours  
	const mm = date.getMinutes();//yields minutes 
	const ss = date.getSeconds();//yields seconds After this construct a string with the above results as below
	const dateObject = {
		_day: dd,
		_month: MM,
		_year: yyyy,
		_hours: HH,
		_minutes: mm,
		_seconds: ss
	};
	return dateObject;
	//return dd + "/" + MM + "/" + yyyy + " " + HH + ':' + mm + ':' + ss;
}

//Format yyyy-MM-dd hh:mm:ss
function stringToDatetimeIso(dateString) {
	const reggie = /(\d{4})-(\d{2})-(\d{2}) (\d{2}):(\d{2}):(\d{2})/;
	const dateArray = reggie.exec(dateString);
	const dateObject = new Date(
		(+dateArray[1]),
		(+dateArray[2]) - 1, // Careful, month starts at 0!
		(+dateArray[3]),
		(+dateArray[4]),
		(+dateArray[5]),
		(+dateArray[6])
	);
	return dateObject;
}

function getIdNumFromElement(id) {
	if (id) {
		const elements = id.split("-");
		const num = elements[elements.length - 1];
		if (isInt(num)) {
			const result = parseInt(num);
			return result;
		}
	}
	return 0;
}

//Format MM/DD/YYYY HH:mm tt
function stringToDatetimeUs(dateString) {
	const splitDate = dateString.split(" ");
	const dateParts = splitDate[0].split("/");
	const timeParts = splitDate[1].split(":");
	const ampm = splitDate[2];
	var hours = parseInt(timeParts[0]);
	if (ampm.toUpperCase() == "PM" && hours != 12) {
		hours += 12;
	}
	if (ampm.toUpperCase() == "AM" && hours == 12) {
		hours = 0;
	}
	const month = parseInt(dateParts[0]) - 1;
	const dateObject = new Date(dateParts[2], month, dateParts[1], hours, timeParts[1]);
	return dateObject;
}

function stringToDateUs(dateString) {
	const splitDate = dateString.split(" ");
	const dateParts = splitDate[0].split("/");
	const timeParts = splitDate[1].split(":");
	const ampm = splitDate[2];
	var hours = parseInt(timeParts[0]);
	if (ampm.toUpperCase() == "PM" && hours != 12) {
		hours += 12;
	}
	if (ampm.toUpperCase() == "AM" && hours == 12) {
		hours = 0;
	}
	const month = parseInt(dateParts[0]) - 1;
	const dateObject = new Date(dateParts[2], month, dateParts[1]);
	return dateObject;
}

function setDropDownSelectableObject(jquerySelector, items, defaultOption, defaultOptionName) {
	$(jquerySelector).empty();
	var selectedValues = new Array();
	var isMultiple = false;
	if ($(jquerySelector).attr("multiple") == "multiple") {
		isMultiple = true;
	}
	if (items) {
		if (defaultOption) {
			if (!defaultOptionName) {
				defaultOptionName = "None selected";
			}

			const defaultOptionItem = {
				Id: 0,
				Name: defaultOptionName
			};

			items.unshift(defaultOptionItem);
		}
		$.each(items, function (i, item) {
			var selectedValue = "";
			if (item.IsSelected && !isMultiple) {
				selectedValue = ' selected="selected"';
			}

			const option = "<option value=" + item.Id + "" + selectedValue + ">" + item.Name + "</option>";
			if (isMultiple && item.IsSelected) {
				selectedValues.push(item.Id);
			}

			$(jquerySelector).append(option);
		});
	}

	$(jquerySelector).multiselect("rebuild");
	if (selectedValues.length > 0) {
		$(jquerySelector).multiselect("select", selectedValues);
	}
}

function setSimpleObjectSelectList(jquerySelector, items, selectedValues) {
	$(jquerySelector).empty();
	if (items) {
		$.each(items, function (i, item) {
			$(jquerySelector).append($("<option>", {
				value: item.Id,
				text: item.Name
			}));
		});
	}
	$(jquerySelector).multiselect("rebuild");
	if (selectedValues) {
		setSelectedValues(jquerySelector, selectedValues);
	}
}

function getArraySelectedValues(jquerySelector) {
	try {
		const jquerySelectorOptions = jquerySelector + " option:selected";
		const optionsSelected = $(jquerySelectorOptions);
		const values = new Array();
		for (let i = 0; i < optionsSelected.length; i++) {
			const option = optionsSelected[i];
			values.push(option.value);
		}
		return values;
	} catch (e) {
		return new Array();
	}
}

function getSelectedValues(jquerySelector) {
	try {
		const jquerySelectorOptions = jquerySelector + " option:selected";
		const optionsSelected = $(jquerySelectorOptions);
		let values = "";
		for (let i = 0; i < optionsSelected.length; i++) {
			const option = optionsSelected[i];
			values += option.value;
			if (i < optionsSelected.length - 1) {
				values += ",";
			}
		}
		return values;
	} catch (e) {
		return "";
	}
}

function getSelectedValueInt(jquerySelector) {
	const selectedIds = getSelectedValues(jquerySelector);
	if (isInt(selectedIds)) {
		return parseInt(selectedIds);
	}
	return 0;
}

function setSelectedValues(jquerySelector, values, rebuild) {
	if (values) {
		const valuesArray = values.split(",");
		$(jquerySelector).multiselect("select", valuesArray);
		if (rebuild) {
			$(jquerySelector).multiselect("rebuild");
		}
	}
}

function CreateBootstrapTable(tableSelector, data) {
	const mainTable = $(tableSelector);
	mainTable[0].innerHTML = "";
	const headerRow = $("<tr>");
	var tableCell;
	//fills header
	for (let i = 0; i < data.Header.Cells.length; i++) {
		tableCell = $("<th>");
		tableCell[0].innerHTML = data.Header.Cells[i].Value;
		headerRow.append(tableCell);
	}
	const headerTable = $("<thead>");
	headerTable.append(headerRow);
	mainTable.append(headerRow);

	const bodyTable = $("<tbody>");
	//fills data
    for (let j = 0; j < data.Rows.length; j++) {
        const dataRow = data.Rows[j];
        let tableRow;
        if (dataRow.InLineAttributes) {
            tableRow = $("<tr " + dataRow.InLineAttributes +">");
        }
        else {
            tableRow = $("<tr>");
        }
		
		for (let k = 0; k < dataRow.Cells.length; k++) {
			tableCell = $("<td>");
			tableCell[0].innerHTML = dataRow.Cells[k].Value;
			tableRow.append(tableCell);
		}
		bodyTable.append(tableRow);
	}
	mainTable.append(bodyTable);
}

function CreateResponseTable(tableSelector, data) {
	const table = $(tableSelector);
	table[0].innerHTML = "";
	const mainTable = $("<div>", { id: "detail-contant-table", class: "r-table" });
	const headerRow = $("<div>", { class: "header-table-row table-row" });
	for (let i = 0; i < data.Header.Cells.length; i++) {
		let spanCell;
		if (i == 0) {
			spanCell = $("<span>", { class: "table-cell primary" });
		} else {
			spanCell = $("<span>", { class: "table-cell" });
		}
		spanCell[0].innerHTML = data.Header.Cells[i].Value;
		headerRow.append(spanCell);
	}
	mainTable.append(headerRow);
	for (let j = 0; j < data.Rows.length; j++) {
		const tableRow = $("<div>", { class: "table-row" });
		const dataRow = data.Rows[j];
		for (let k = 0; k < dataRow.Cells.length; k++) {
			let spanTableCell;
			if (k == 0) {
				spanTableCell = $("<span>", { class: "table-cell primary" });
			} else {
				spanTableCell = $("<span>", { class: "table-cell" });
			}
			spanTableCell[0].innerHTML = dataRow.Cells[k].Value;
			tableRow.append(spanTableCell);
		}
		mainTable.append(tableRow);
	}
	table.append(mainTable);
}

function openMultiselect(jquerySelector) {
	$(jquerySelector).data().multiselect.$button.click();
}

function formatMoney(number) {
	if (typeof number != "number") {
		if (isNumber(number)) {
			number = parseFloat(number);
		} else {
			number = 0;
		}
	}
	return number.formatMoney("₡", "0", ".", " ");
}

function loadHtmlSelect(jquerySelector, itemList, addEmpty) {
    if (addEmpty) {
        itemList.unshift({
            id: 0,
            name: ""
        });
    }

    const element = $(jquerySelector);
    element.empty();
    if (itemList) {
        for (let i = 0; i < itemList.length; i++) {
            const item = itemList[i];
            const newOption = $("<option />").val(item.id).text(item.name);
            if (item.isSelected) {
                newOption.prop("selected", true);
            }

            element.append(newOption);
        }
    }
}

function isNumber(n) {
	return !isNaN(parseFloat(n)) && isFinite(n);
}

function isInt(n) {
	return !isNaN(parseInt(n)) && isFinite(n);
}

function isValidId(n) {
    return isInt(n) && n > 0;
}

function setDateTimeInput(jquerySelector, initialDateJs, suggestedDateJs, userEndDateJs) {
	const dateElement = $(jquerySelector);
	if (suggestedDateJs) {
		try {
			const isoFormat = "YYYY-MM-DDTHH:mm:ss";
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

String.prototype.insertAt = function (index, string) {
	return this.substr(0, index) + string + this.substr(index);
};

Number.prototype.formatMoney = function (symbol, c, d, t) {
	var n = this,
        c = isNaN(c = Math.abs(c)) ? 2 : c,
        d = d == undefined ? "." : d,
        t = t == undefined ? "," : t;
	const s = n < 0 ? "-" : "";
	const i = parseInt(n = Math.abs(+n || 0).toFixed(c)) + "";
	var j = (j = i.length) > 3 ? j % 3 : 0;
	return symbol + s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
};

String.prototype.format = function () {
	var args = arguments;
	return this.replace(/{(\d+)}/g, function (match, number) {
		return typeof args[number] != "undefined"
		  ? args[number]
		  : match
		;
	});
};

function readDateField(elementSelector) {
    const element = $(elementSelector);
    const value = new Date(element.val());
    const parameterDate = window.createJsDateTimeObject(value);
    const result = {
        parameter: parameterDate,
        value: value
    };
    return result;
}

function loadDateField(elementSelector, initialDateJs, suggestedDateJs, userEndDateJs) {
    const dateElement = $(elementSelector);
    const isoFormat = "YYYY-MM-DDTHH:mm:ss";
    if (suggestedDateJs) {
        try {

            dateElement.attr("min", null);
            dateElement.attr("max", null);
            if (initialDateJs) {
	            const initialDate = window.moment(initialDateJs).format(isoFormat);
	            dateElement.attr("min", initialDate);
	        }

	        const suggestedDate = window.moment(suggestedDateJs).format(isoFormat);
            dateElement.val(suggestedDate);

            if (userEndDateJs) {
	            const endDate = window.moment(userEndDateJs).format(isoFormat);
	            dateElement.attr("max", endDate);
	        }


        } catch (error) {
            console.log(error);
        }
    } else {
        dateElement.attr("min", null);
        dateElement.attr("max", null);
    }
}