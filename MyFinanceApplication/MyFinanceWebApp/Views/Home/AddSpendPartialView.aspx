<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<div class="row .row-margin">
    <div class="col-md-4 inner-column">
        <label id="add-spend-date-label">Date</label><br />
        <input type="datetime-local" id="add-spend-date" name="date" />
    </div>
    <%--    <div class="col-xs-1">
        <br/>
        <label id="add-spend-currency-symbol" style="float: right; padding-top: 7px; padding-right: 5px;">C</label>
    </div>--%>
    <div class="col-md-3 inner-column" style="/*padding-left: 0; */">
        <label id="add-spend-amount-label">Amount</label><br />
        <input type="number" id="add-spend-amount-value" pattern="\d*"/>
    </div>
    <div class="col-md-2 inner-column">
        <label>Currency</label><br />
        <select id="add-spend-currency-select"></select>
    </div>
    <div class="col-md-3 inner-column">
        <label>Exchange</label><br />
        <select id="add-spend-method-select"></select>
    </div>
</div>
<div class="row" style="padding-top: 10px;">
    <div class="col-md-8 inner-column">
        <label>Description</label><br />
        <input type="text" id="add-spend-description" style="width: 100%;"/>
    </div>
    <div class="col-md-3 inner-column">
        <label>Type</label><br />
        <select id="add-spend-type-select"></select>
    </div>
</div>
<div class="row" style="padding-top: 10px;">
    <div class="col-md-3 inner-column">
        <label>Include in accounts...</label><br />
        <select id="add-spend-account-include-select" multiple="multiple"></select>
    </div>
    <div class="col-md-5 inner-column">
        <div>
            <table class="table table-bordered" id="add-spend-account-include-table">
                <thead>
                    <tr>
                        <th data-field="Account">Account</th>
                        <th data-field="Exchange">Exchange</th>
                        <th data-field="Amount">Amount</th>
                    </tr>
                </thead>
                <tbody id="add-spend-account-include-tbody">
                    <tr>
                        <td></td>
                        <td>
                            <select id="add-spend-test-select"></select>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
    <div class="col-md-3 inner-column">
        <label id="add-spend-set-payment-date-label">Payment Date</label><br />
        <input type="datetime" class="datetimepicker-control" id="add-spend-set-payment-date" name="date" />
        <br />
        <input style="margin-top: 15px;" type="checkbox" id="add-spend-is-pending" name="isPending"/>Pending<br/>
        <a onclick="confirmSpend()" href="#" id="add-spend-confirm-payment">Confirm payment</a>
    </div>
</div>

<div id="spend-transfer-section">
    <div class="row" style="padding-top: 10px;">
        <div class="col-sm-3">
            <h5>Transfer information</h5>
        </div>
    </div>
</div>