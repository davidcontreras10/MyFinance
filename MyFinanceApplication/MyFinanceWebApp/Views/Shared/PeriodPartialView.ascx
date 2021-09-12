<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MyFinanceWebApp.Models.MainViewPageModel>" %>

<form action="AddPeriod" id="add-period-form" method="POST" >
    <div class="row">
        <div class="col-xs-5">
            <span>Initial Date</span><br />
            select-spend-type
        </div>
        <div class="col-xs-5">
            <span>End Date</span><br />
            <input type="datetime" id="datetimepicker-period-modal-end" class="datepicker-control" name="end"/>
        </div>
    </div>
    <br />
    <div class="row">
        <div class="col-md-12">
            <span>Budget</span><br />
            <input type="number" id="input-period-modal-budget" name="budget"/>
        </div>
    </div>
    <input id="hidden-accountId" type="hidden" name="accountId"/>
</form>







