<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<MyFinanceWebApp.Models.LoginModel>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0" />
    <title>Login</title>
    <link href="../../Content/GlobalStyles.css" rel="stylesheet" />
    <link href="../../Content/Login.css" rel="stylesheet" />
    
    <script type="text/javascript" src="<%= Page.ResolveUrl("~/Scripts/jquery-1.9.1.min.js") %>"></script>
    <script type="text/javascript" src="<%= Page.ResolveUrl("~/Scripts/utils/utilities.js") %>"></script>
</head>
<body>
    <div class="main-login-content">

        <ul class="form-list form-login" role="presentation">
            <% using (Html.BeginForm("Login", "User", new { ViewBag.ReturnUrl }))
               { %>
            <%: Html.AntiForgeryToken() %>
            
            <%--<%: Html.ValidationSummary() %>--%>
            
            <li>
                <%: Html.LabelFor(m=>m.Username) %>
                <%: Html.TextBoxFor(m=>m.Username) %>
                <%: Html.ValidationMessageFor(m=>m.Username) %>
            </li>


            <li>
                <%: Html.LabelFor(m => m.Password) %>
                <%: Html.PasswordFor(m => m.Password) %>
                <%: Html.ValidationMessageFor(m => m.Password) %>
            </li>
            <li>
                <%: Html.CheckBoxFor(m => m.RememberMe, new { @checked = true}) %>
                <%: Html.LabelFor(m => m.RememberMe, new { @class = "checkbox" }) %>
            </li>
            <li>
                <input type="submit" value="Go"/>
            </li>
                <li>
                    <%: Html.ActionLink("Forgot Password?","ForgottenPassword") %>
                </li>
        </ul>

        <% } %>
    </div>

</body>
</html>
