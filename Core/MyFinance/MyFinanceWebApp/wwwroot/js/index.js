var detailData;
var detailLabelEnable = false;

function SetDetailData(stringData) {
    detailData = jQuery.parseJSON(stringData);
    AddActionColumns(detailData);
}

function AddActionColumns(data) {
    //adds headers
    data.Header.Cells.push(createCellObject('Actions'));

    //adds action html
    for (var i = 0; i < data.Rows.length; i++) {
        var row = data.Rows[i];
        var object = createCellObject(ActionsHtml(row.RowId));
        row.Cells.push(object);
    }
}

function ActionsHtml(index) {
    return "<div> <span onclick='DeleteAction(" + index + ")' class='delete-action-text'>X</span> <span style='margin-left: 10px;' onclick='EditAction(" + index + ")' class='edit-action-text'>Edit</span></div>";
}

function DeleteAction(rowId) {
    if (confirm('Are you sure you want to delete this item?')) {
        var parameter =
        {
            spendId: rowId
        };
        CallService('Home', 'DeleteSpend', parameter, ReloadPageData);
    } else {
        // Do nothing!
    }
}

function EditAction(index) {
    alert('Ups! This function has not been yet implemented on this platform. Sorry for the inconvenience :(');
}

function ReloadPageData(data) {
    SetDetailData(data.detailTable);
    CreateDetailDataTable('#detailsDiv', detailData);
    $('#weekly-balance-label').text(data.weeklyAmount);

}


function SpendSubmitValidation() {
    return true;
}

function ValidateSubmit(event) {

}

function DetailLabelOnClick(event) {
    if (detailLabelEnable) {
        $('#detailsDiv').css('display', 'none');

    } else {
        $('#detailsDiv').css('display', 'block');
    }
    detailLabelEnable = !detailLabelEnable;
}

function CreateDetailDataTable(tableSelector, data) {
    var table = $(tableSelector);
    table[0].innerHTML = "";
    var mainTable = $("<div>", { id: "detail-contant-table", class: "r-table" });
    var headerRow = $("<div>", { class: "header-table-row table-row" });
    for (var i = 0; i < data.Header.Cells.length; i++) {
        var spanCell;
        if (i == 0) {
            spanCell = $("<span>", { class: "table-cell primary" });
        } else {
            spanCell = $("<span>", { class: "table-cell" });
        }
        spanCell[0].innerHTML = data.Header.Cells[i].Value;
        headerRow.append(spanCell);
    }
    mainTable.append(headerRow);
    for (var j = 0; j < data.Rows.length; j++) {
        var tableRow = $("<div>", { class: "table-row" });
        var dataRow = data.Rows[j];
        for (var k = 0; k < dataRow.Cells.length; k++) {
            var spanTableCell;
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