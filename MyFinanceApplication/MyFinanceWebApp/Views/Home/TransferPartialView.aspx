<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="MyFinanceModel.ClientViewModel" %>

<div class="row .row-margin">
    <div class="col-md-3 inner-column">
        <label id="transfer-from-account-name-label">From Account</label><br />
    </div>
    <div class="col-md-7 inner-column">
        <input type="text" id="transfer-from-account-name-value" readonly="readonly" />
    </div>
</div>

<div class="row" style="padding-top: 15px;">
    <div class="col-md-3">
        <label id="transfer-amount-title-id" class="transfer-section-title">Amount</label>
    </div>
</div>

<div class="row" style="padding-top: 10px;">
    <div class="col-md-3 inner-column transfer-amount-type-section" style="/*padding-left: 0; */">
        <input type="radio" name="transfer-amount-type-name" value="<%=BalanceTypes.Custom %>" id="default-transfer-amount-type-radio"/>
        <span id="transfer-amount-type-custom-label" >Custom</span>
    </div>
    <div class="col-md-7 inner-column transfer-amount-values" id="transfer-amount-values-<%=BalanceTypes.Custom%>">
        <span id="transfer-amount-type-custom-currency-symbol-text"></span>
        <input type="number" id="transfer-amount-type-custom-value" pattern="\d*"/>
        <select id="transfer-amount-currency-select"></select>
    </div>
</div>

<div class="row" style="padding-top: 10px;">
    <div class="col-md-3 inner-column transfer-amount-type-section">
        <input type="radio" name="transfer-amount-type-name" value="<%=BalanceTypes.AccountPeriodBalance%>">
        <span id="transfer-amount-type-period-balance-label">Period Balance</span>
    </div>
    <div class="col-md-7 inner-column transfer-amount-values" id="transfer-amount-values-<%=BalanceTypes.AccountPeriodBalance%>">
        <span id="transfer-amount-type-period-balance-currency-symbol-text"></span>
        <input type="text" id="transfer-amount-type-period-balance-text" readonly="readonly" />
    </div>
</div>

<div class="row" style="padding-top: 10px;">
    <div class="col-md-3 inner-column transfer-amount-type-section">
        <input type="radio" name="transfer-amount-type-name" value="<%=BalanceTypes.AccountOverallBalance%>">
        <span id="transfer-amount-type-overall-balance-label">Overall Balance</span>
    </div>
    <div class="col-md-7 inner-column transfer-amount-values" id="transfer-amount-values-<%=BalanceTypes.AccountOverallBalance%>">
        <span id="transfer-amount-type-period-overall-currency-symbol-text"></span>
        <input type="text" id="transfer-amount-type-overall-balance-text" readonly="readonly" />
    </div>
</div>

<div class="row" style="padding-top: 15px;">
    <div class="col-md-3 inner-column">
        <label>Destination Account</label>
    </div>
    <div class="col-md-7 inner-column">
        <select id="transfer-destination-account-select"></select>
    </div>
</div>

<div class="row" style="padding-top: 15px;">
    <div class="col-md-3 inner-column">
        <label>Select Type</label>
    </div>
    <div class="col-md-7">
        <select id="transfer-spend-type-select"></select>
    </div>
</div>

<div class="row" style="padding-top: 15px;">
    <div class="col-md-3 inner-column">
        <label>Transfer date</label>
    </div>
    <div class="col-md-7">
        <input type="datetime-local" class="form-control" id="transfer-datetime-value"/>
    </div>
</div>

<div class="row" style="padding-top: 15px;">
    <div class="col-md-3 inner-column">
        <label>Transfer Pending</label>
    </div>
    <div class="col-md-7">
        <input type="checkbox" id="transfer-is-pending-value"/>
    </div>
</div>

<div class="row" style="padding-top: 15px;">
    <div class="col-md-3 inner-column">
        <label>Description</label>
    </div>
    <div class="col-md-9">
        <input type="text" id="transfer-description-value" class="form-control"/>
    </div>
</div>
