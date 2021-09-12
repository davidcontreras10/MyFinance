<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<div class="row .row-margin">
    <div class="col-md-3 inner-column">
        <label id="edit-spend-date-label">Date</label><br/>
        <input type="datetime" class="datepicker-control" id="edit-spend-date" name="date" />
    </div>
<%--    <div class="col-xs-1">
        <br/>
        <label id="edit-spend-currency-symbol" style="float: right; padding-top: 7px; padding-right: 5px;">C</label>
    </div>--%>
    <div class="col-md-3 inner-column" style="/*padding-left: 0;*/">
        <label id="edit-spend-amount-label">Amount</label><br/>
        <input type="text" id="edit-spend-amount-value" />
    </div>
    <div class="col-md-2 inner-column">
        <label>Currency</label><br/>
        <select id="edit-spend-currency-select"></select>
    </div>
    <div class="col-md-3 inner-column">
        <label>Exchange</label><br/>
        <select id="edit-spend-method-select"></select>
    </div>
</div>
<div class="row" style="padding-top: 10px;">
    <div class="col-md-3 inner-column">
        <label>Type</label><br/>
        <select id="edit-spend-type-select"></select>
    </div>
    <div class="col-md-3 inner-column">
        <label>Include in accounts...</label><br/>
        <select id="edit-spend-account-include-select" multiple="multiple"></select>
    </div>    
    <div class="col-md-3 inner-column">
        <div>
                                    <table class="table table-bordered" id="edit-spend-account-include-table">
                            <thead>
                                <tr>
                                    <th data-field="Budget">Account</th>
                                    <th data-field="Spent">Exchange</th>
                                </tr>
                            </thead>
                            <tbody id="edit-spend-account-include-tbody">
                                <tr>
                                    <td>
                                        Weekly
                                    </td>
                                    <td>
                                        <select id="edit-spend-test-select">
                                            <option>Bac Dol-Col</option>
                                            <option>Bac Dol-Col2</option>
                                            <option>Bac Dol-Col3</option>
                                        </select>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
        </div>      
    </div>
</div>
