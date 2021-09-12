<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MyFinanceWebApp.Models.MainViewPageModel>" %>

<div class="spend-detail-popup">
    <div class="row">
        <div class="col-md-4">
            <label class="label-block">Amount</label>
            <input type="number" id="spend-detail-amount" />
        </div>
        <div class="col-md-4">
            <label class="label-block">Date</label>
            <input type="datetime" id="spend-detail-date" onchange="testFunction()"/>
        </div>
        <div class="col-md-4">
            <label class="label-block" style="padding-right: 30px;">Type</label>
            <select class="select-control" id="spend-detail-type">
            </select>
        </div>
    </div>
    <div class="row row-distance">
        <div class="col-md-12">
            <label class="label-block">Description</label>
            <input style="width: 100%; white-space: pre-wrap; display: inline-block;" type="text" id="spend-detail-description" value=""/>
        </div>
    </div>
    <div class="row row-distance">
        <div class="col-md-12">
            <label style="display: inline">Default accounts to include</label>
            <select id="spend-detail-accounts" multiple="multiple">   
            </select>
        </div>
    </div>
</div>