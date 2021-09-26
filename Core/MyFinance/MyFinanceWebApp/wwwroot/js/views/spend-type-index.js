$(function() {

	var editableContent;

	const loadControls = function() {
		startEditableContent();
		loadSpendTypeData();
	};

	var startEditableContent = function () {
        editableContent = new $.EditableContentHandlerClass('.content-editable', editableContentChanged);
        editableContent.InitEvents();
    }
    
	var loadSpendTypeData = function() {
		CallServiceGet('SpendType', 'GetAllData', null, loadSpendTypeDataSuccess);
	}

	var loadSpendTypeDataSuccess = function(data) {
		createSpendTypeDataTable(data.AllSpendTypes, '#spend-type-list-table', true, false);
		createSpendTypeDataTable(data.UserSpendTypes, '#spend-type-my-list-table', false, true);
	}

	var createSpendTypeDataTable = function (data, jquerySelectorTable, editColumns, userTable) {
		const columns = ["Name", "Description", "Action"];
		$(jquerySelectorTable).empty();
		const tHeader = createTableHeader(columns);
		$(jquerySelectorTable).append(tHeader);
		const tbody = createSpendTypeTBody(data, editColumns, userTable);
		$(jquerySelectorTable).append(tbody);
		if (editColumns)
			editableContent.InitEvents();
	}

	var createTableHeader = function(columns) {
		var tbodyHeader = "<tbody>";
		tbodyHeader += "<tr>";
		for (let i = 0; i < columns.length; i++) {
			const column = columns[i];
			const th = "<th>" + column + "</th>";
			tbodyHeader += th;
		}

		tbodyHeader += "</tr>";
		tbodyHeader += "</tbody>";
		return tbodyHeader;
	}

	var createSpendTypeTBody = function(data, editColumns, userTable) {
		var tbody = "<tbody>";
		for (let i = 0; i < data.length; i++) {
			const spendType = data[i];
			let contentEditable = "";
			if (editColumns) {
				contentEditable += " ";
				contentEditable += " class='content-editable' contenteditable";
			}

			let tr = "<tr>";
			const tdName = "<td id='spend-type-" + spendType.SpendTypeId + "-field-SpendTypeName-id'" + contentEditable + ">" + spendType.SpendTypeName + "</td>";
			const tdDescription = "<td id='spend-type-" + spendType.SpendTypeId + "-field-SpendTypeDescription-id'" + contentEditable + ">" + spendType.Description + "</td>";
			let tdAction;
			if (userTable) {
				tdAction = createUserSpendTypeActionColumn(spendType.SpendTypeId);
			} else {
				tdAction = createAllSpendTypeActionColumn(spendType.SpendTypeId, spendType.IsSelected);
			}
			tr += tdName + tdDescription + tdAction;
			tr += "</tr>";
			tbody += tr;
		}

		return tbody;
	}

	var createAllSpendTypeActionColumn = function(spendTypeId, isSelected) {
		var td = "<td><div class='center-content'>";
		const deleteInput = "<input type='button' onclick='deleteSpendType(" + spendTypeId + ")' class='cell-button delete-button' value='X'/>";
		const moveInput = "<input type='button' onclick='addSpendTypeUser("+ spendTypeId +")' class='cell-button' value='&rarr;' />";
		td += deleteInput;
		if (!isSelected) {
			td += moveInput;
		}
		
		td += "</div></td>";
		return td;
	}

	var createUserSpendTypeActionColumn = function (spendTypeId) {
		var td = "<td><div class='center-content'>";
		const moveInput = "<input type='button' onclick='deleteSpendTypeUser(" + spendTypeId + ")' class='cell-button' value='&larr;' />";
		td += moveInput;
		td += "</div></td>";
		return td;
	}

	var editableContentChanged = function(data) {
		const id = data.id;
		const fieldName = getStringBetweenValues(id, 'field-', '-id');
		const spendTypeId = getStringBetweenValues(id, 'type-', '-field-');
		var editValue = $(data).text();
        if (editValue) {
            editValue = editValue.trim();
        }

		const jsonValue = createEditObject(spendTypeId, fieldName, editValue);
		console.log('id: ' + spendTypeId + ' field: ' + fieldName);
		editSpendType(jsonValue);
	}

	var editSpendType = function (model) {
		const modelValid = validateEditSpendTypeModel(model);
		if (modelValid) {
			callServicePost('spendType', 'EditSpendType', null, model, editSpendTypeSuccess);
		} else {
			loadSpendTypeData();
		}
	}

	var editSpendTypeSuccess = function(data) {
		if (data) {
			loadSpendTypeData();
		}
	}

	var createEditObject = function(spendTypeId, field, value) {
		const stringJson = '{"SpendTypeId": {0}, "{1}": "{2}"}'.format(spendTypeId, field, value);
		const jsonValue = JSON.parse(stringJson);
		return jsonValue;
	}

	const addSpendTypeUser = function(spendTypeId) {
		if (spendTypeId) {
			const model = {
				SpendTypeId: spendTypeId
			};

			callServicePost('spendType', 'AddSpendTypeUser', null, model, editSpendTypeUserSuccess);
		}
	};

	const deleteSpendTypeUser = function (spendTypeId) {
		if (spendTypeId) {
			const model = {
				SpendTypeId: spendTypeId
			};

			callServicePost('spendType', 'DeleteSpendTypeUser', null, model, editSpendTypeUserSuccess);
		}
	};

	const deleteSpendType = function (spendTypeId) {
		if (spendTypeId) {
			const model = {
				SpendTypeId: spendTypeId
			};

			callServicePost('spendType', 'DeleteSpendType', null, model, editSpendTypeUserSuccess);
		}
	};

	var editSpendTypeUserSuccess = function(data) {
		if (data && data.length > 0) {
			loadSpendTypeData();
		}
	}

	const addSpendType = function() {
		const addModel = getAddSpendTypeModel();
		const errors = validateAddSpendTypeModel(addModel);
		if (errors.length > 0) {
			handleAddSpendTypeErrors(errors);
		} else {
			callServicePost('SpendType', 'AddSpendType', null, addModel, addSpendTypeSuccess);
		}
	};

	var addSpendTypeSuccess = function(data) {
		if (data.length > 0) {
			clearAddSpendType();
			loadSpendTypeData();
		}
	}

	var clearAddSpendType = function() {
		$('#spend-type-name-text').val('');
		$('#spend-type-description-text').val('');
		$('#spend-type-is-selected-checkbox').prop('checked', false);
	}

	var handleAddSpendTypeErrors = function(errors) {
		alert('Data error');
	}

	var getAddSpendTypeModel = function() {
		var name = $('#spend-type-name-text').val();
		if (name) {
			name = name.trim();
		}

		var description = $('#spend-type-description-text').val();
		if (description) {
			description = description.trim();
		}

		const isSelected = $('input[id="spend-type-is-selected-checkbox"]:checked').length > 0;
		const model = {
			SpendTypeName: name,
			SpendTypeDescription: description,
			IsSelected: isSelected
		};

		return model;
	}

	var validateEditSpendTypeModel = function(model) {
		if (!model.hasOwnProperty('SpendTypeId') || model.SpendTypeId < 1) {
			return false;
		}

		if (model.hasOwnProperty('SpendTypeName')) {
			const value = model.SpendTypeName;
			if (!value || !value.trim()) {
				return false;
			}
		}

		return true;
	}

	var validateAddSpendTypeModel = function(model) {
		const errors = new Array();
		if (!model.SpendTypeName) {
			errors.push({
				field: "Name",
				message: "Name cannot be null or empty"
			});
		}

		//if (!model.SpendTypeDescription) {
		//	error.push({
		//		field: "Name",
		//		message: "Name cannot be null or empty"
		//	});
		//}
		return errors;
	}

	window.deleteSpendType = deleteSpendType;
	window.addSpendTypeUser = addSpendTypeUser;
	window.deleteSpendTypeUser = deleteSpendTypeUser;
	window.addSpendType = addSpendType;
	loadControls();
});