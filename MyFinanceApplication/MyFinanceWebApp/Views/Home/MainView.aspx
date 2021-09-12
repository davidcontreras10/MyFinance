<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<MyFinanceWebApp.Models.MainViewPageModel>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>My Finance</title>

	<meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0" />
	<link rel="icon" href="~/Content/images/FinanceIcon.ico"/>
	<link href="~/Content/bootstrap.css" rel="stylesheet" />
	<link href="~/Content/bootstrap-theme.min.css" rel="stylesheet" />
	<link href="~/Content/bootstrap-datetimepicker.min.css" rel="stylesheet" />
	<link href="~/Content/bootstrap-multiselect.css" rel="stylesheet" />
	<link href="~/Content/Views/main-view.css" rel="stylesheet" />
	<link href="~/Content/custom-bootstrap.css" rel="stylesheet" />

	<script type="text/javascript" src="<%= Page.ResolveUrl("~/Scripts/jquery-1.9.1.min.js") %>"></script>


	<script type="text/javascript" src="<%= Page.ResolveUrl("~/Scripts/utils/utilities.js") %>"></script>
	<script type="text/javascript" src="<%= Page.ResolveUrl("~/Scripts/moment.min.js") %>"></script>
	<script type="text/javascript" src="<%= Page.ResolveUrl("~/Scripts/bootstrap.js") %>"></script>
	<script type="text/javascript" src="<%= Page.ResolveUrl("~/Scripts/bootstrap-datetimepicker.js") %>"></script>

	<script type="text/javascript" src="<%= Page.ResolveUrl("~/Scripts/bootstrap-multiselect.js") %>"></script>
	<script type="text/javascript" src="<%= Page.ResolveUrl("~/Scripts/bootstrap-multiselect-collapsible-groups.js") %>"></script>

	<script type="text/javascript" src="<%= Page.ResolveUrl("~/Scripts/Views/main-view-utils.js") %>"></script>
	<script type="text/javascript" src="<%= Page.ResolveUrl("~/Scripts/Views/main-view.js") %>"></script>
	<script type="text/javascript" src="<%= Page.ResolveUrl("~/Scripts/Views/add-spend-view.js") %>"></script>
	<script type="text/javascript" src="<%= Page.ResolveUrl("~/Scripts/Views/transfer-modal.js") %>"></script>
	<script type="text/javascript" src="<%= Page.ResolveUrl("~/Scripts/Views/custom-period-partial-view.js") %>"></script>
	<script type="text/javascript" src="<%= Page.ResolveUrl("~/Scripts/Views/basic-trx-view.js") %>"></script>

	<script type="text/javascript">
		var siteRoot = '<%=Request.ApplicationPath%>';
	</script>
</head>
<body>

	<div>
		<% Html.RenderPartial("MainHeader", Model, ViewData); %>
	</div>

	<div id="custom-period-modal" class="modal fade" role="dialog" style="overflow-y: auto;">
		<div class="modal-dialog modal-lg">
			<!-- Modal content-->
			<div class="modal-content">
				<div id="custom-period-modal-header-id" class="modal-header">
					<button type="button" class="close" data-dismiss="modal">&times;</button>
					<h4 id="custom-period-model-title-h4" class="modal-title">Account Summary</h4>
					<h5 id="custom-period-sub-title"></h5>
				</div>
				<div class="modal-body">
					<% Html.RenderPartial("AccountFinancePartialView", Model); %>
				</div>
				<div class="modal-footer">
					<button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
				</div>
			</div>
		</div>
	</div>
	
	<div id="add-trx-modal" class="modal fade" role="dialog" style="overflow-y: auto;">
		<div class="modal-dialog">
			<!-- Modal content-->
			<div class="modal-content">
				<div id="add-trx-modal-header" class="modal-header">
					<button type="button" class="close" data-dismiss="modal">&times;</button>
					<h5 id="add-trx-account-name"></h5>
					<h4 id="add-trx-title" class="modal-title"></h4>
				</div>
				<div class="modal-body">
					<% Html.RenderPartial("BasicTrxPartialView", Model); %>
				</div>
				<div class="modal-footer">
					<button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
					<button type="button" class="btn btn-default" id="add-trx-modal-button" onclick="submitAddBasicTrx()">Save</button>
				</div>
			</div>
		</div>
	</div>

	<div id="add-spend-modal" class="modal fade" role="dialog" style="overflow-y: auto;">
		<div class="modal-dialog modal-lg">
			<!-- Modal content-->
			<div class="modal-content">
				<div id="add-modal-header-id" class="modal-header">
					<button type="button" class="close" data-dismiss="modal">&times;</button>
					<h4 id="add-model-title-h4" class="modal-title">New Expense</h4>
					<h5 id="add-spend-account-name"></h5>
				</div>
				<div class="modal-body">
					<% Html.RenderPartial("AddSpendPartialView", Model); %>
				</div>
				<div class="modal-footer">
					<button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
					<button type="button" class="btn btn-default" id="spend-modal-button" onclick="submitAddSpend()">Save</button>
				</div>
			</div>
		</div>
	</div>

	<div id="transfer-modal" class="modal fade" role="dialog" style="overflow-y: auto;">
		<div class="modal-dialog">
			<!-- Modal content-->
			<div class="modal-content">
				<div id="transfer-modal-header-id" class="modal-header">
					<button type="button" class="close" data-dismiss="modal">&times;</button>
					<h4 id="transfer-modal-title" class="modal-title">Transfers</h4>
				</div>
				<div class="modal-body">
					<% Html.RenderPartial("TransferPartialView", Model); %>
				</div>
				<div class="modal-footer">
					<button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
					<button type="button" class="btn btn-default" id="transfer-modal-submit" onclick="submitTransferModal()">Submit</button>
				</div>
			</div>
		</div>
	</div>

	<div id="ajax-loading-modal" class="modal fade" role="dialog">
		<div class="modal-dialog">
			<div class="modal-content">
				<div id="Div2" class="modal-header">
					<h4 id="H1" class="modal-title">Loading...</h4>
				</div>
				<div class="modal-body">
					<div class="loader center-content"></div>
				</div>
				<div class="modal-footer">
				</div>
			</div>
		</div>
	</div>

	<div id="myModal" class="modal fade" role="dialog">
		<div class="modal-dialog">
			<!-- Modal content-->
			<div class="modal-content">
				<div class="modal-header">
					<button type="button" class="close" data-dismiss="modal">&times;</button>
					<h4 class="modal-title">Create Period</h4>
				</div>
				<div class="modal-body">
					<%Html.RenderPartial("PeriodPartialView", Model); %>
				</div>
				<div class="modal-footer">
					<button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
					<button type="button" class="btn btn-default" id="add-period-submit">Save</button>
				</div>
			</div>

		</div>
	</div>

	<div id="spend-detail-modal" class="modal fade" role="dialog">
		<div class="modal-dialog">
			<!-- Modal content-->
			<div class="modal-content">
				<div class="modal-header">
					<button type="button" class="close" data-dismiss="modal">&times;</button>
					<h4 class="modal-title">Edit Spending</h4>
				</div>
				<div class="modal-body">
					<%Html.RenderPartial("SpendDetailPartialView", Model); %>
				</div>
				<div class="modal-footer">
					<button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
					<button type="button" class="btn btn-default" id="save-spend-detail-submit" onclick="spendDetailUpdate()">Save</button>
				</div>
			</div>
		</div>
	</div>
	
	<div id="accounts-summary" class="row" style="display: none; margin: 15px; padding: 5px;">
    </div>

	<div id="accordion">

		<%
			var count = 0;
			if (Model.Model.AccountGroupMainViewViewModels == null)
				return;
			foreach (var group in Model.Model.AccountGroupMainViewViewModels.OrderBy(k => k.AccountGroupPosition))
			{
				count++;
		%>
		<a data-toggle="collapse" href="#collapse<%=count.ToString() %>">
			<span class="accordion-title">
				<%=group.AccountGroupDisplayValue %>
			</span>
		</a>

		<div id="collapse<%=count.ToString() %>" class="collapse<%=(group.IsSelected ? " in":"") %>">
			<div style="margin-left: 25px; margin-right: 25px; margin-top: 15px;">

				<%
				for (int i = 0; i < group.Accounts.Count(); i++)
				{
					var account = group.Accounts.ElementAt(i);
					if (i % 2 == 0)
					{
				%>

				<div class="row">
					<% } %>
					<div class="col-md-6 outer-column" <%= account.BorderStyleValue %>>
						<div class="row" <%= account.HeaderStyleValue %>>
							<div class="col-md-4">
								<span class="main-box-title"><%= account.AccountName %></span>
							</div>
							<div class="col-md-7">
								<span class="main-box-title" id="period-title-<%= account.AccountId %>">No Period</span>
							</div>
						</div>
						<br />
						<br />
						<div class="row">
							<div class="col-md-3">
								<label class="box-title" onclick="showCustomPeriodModal(<%=account.AccountId %>)">Period Summary</label>
							</div>
							<div class="col-md-3">
							</div>
							<div class="col-md-6">
								<div style="float: right;">
									<select class="select-control-period" id="select-control-period-<%= account.AccountId %>">
										<option value="0">No Period</option>
										<% foreach (var accountPeriod in account.AccountPeriods.OrderByDescending(item => item.InitialDate))
										   { %>
										<option value="<%= accountPeriod.AccountPeriodId %>" <% if (account.CurrentPeriodId != 0 && accountPeriod.AccountPeriodId == account.CurrentPeriodId)
																								{ %>
											selected="selected" <% } %>><%= accountPeriod.GetDateInfo(false) %></option>
										<% } %>
									</select>
								</div>
							</div>
						</div>

						<br />
						<div class="row">

							<div class="col-md-12">
								<table class="table table-bordered table-low-margin <%= (account.GetTableTypeSyle()) %>"
									id="account-balance-table-<%= account.AccountId %>">
									<thead>
										<tr>
											<th data-field="Budget">Budget</th>
											<th data-field="Spent">Spending</th>
											<th data-field="Balance">Balance</th>
											<th data-field="OverallBalance">Overall Balance</th>
											<th data-field="OverallBalanceToday">Balance As Of Today</th>
										</tr>
									</thead>
									<tbody>
										<tr>
											<td id="period-budget-value-<%= account.AccountId %>"></td>
											<td id="period-spent-value-<%= account.AccountId %>"></td>
											<td id="period-balance-value-<%= account.AccountId %>"></td>
											<td id="account-general-balance-value-<%= account.AccountId %>">N/A</td>
											<td id="account-today-balance-value-<%= account.AccountId %>">N/A</td>
										</tr>
									</tbody>
								</table>
							</div>
						</div>
						<div class="row">
							<div class="col-md-8">
								<input type="button" value="New Expense" style="color: red;" onclick="openAddBasicTrxModal(<%= account.AccountId %>, 1);" />
								<input type="button" value="Add Income" style="color: green;" onclick="openAddBasicTrxModal(<%= account.AccountId %>, 2);" />
								<input type="button" value="Transfer" style="color: blue;" onclick="executeTransferAction(1,<%= account.AccountId %>);" />
							</div>
						</div>
						<br />
						<div>
							<input type="button" onclick="openCustomPeriodEvent('<%= account.AccountId %>')" id="account-summary-button-<%= account.AccountId %>" 
								value="Account Summary"/>
							<input type="button" onclick="showHideSpendList('<%= account.AccountId %>')" 
								id="details-button-<%= account.AccountId %>"
								value="Show Period Activity"/>
						</div>
						<div class="row">
							<div class="col-md-12">
								<div id="details-container-<%= account.AccountId %>" style="display: none;">
									<table class="table table-bordered table-low-margin" id="table-details-<%= account.AccountId %>">
									</table>
								</div>
							</div>
						</div>

					</div>

					<%
					if (i % 2 != 0 || (i == group.Accounts.Count() - 1))
					{
					%>
				</div>

				<%
					}
				%>

				<% } %>
			</div>
		</div>
		<br />
		<% 
			} 
		%>
	</div>
</body>
</html>
