<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExpiryDate.aspx.cs" Inherits="ERP.Report.Pos.Aspx.ExpiryDate.ExpiryDate" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../../../../Content/SiteForReport.css" rel="stylesheet" />
    <title>Expiry Date</title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="position: relative">
            <iframe id="frmPrint" name="IframeName" width="100%" runat="server" style="display: none"></iframe>
        <asp:ScriptManager runat="server" ID="ScriptManager1" ScriptMode="Release"></asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer2" runat="server" Width="100%" Height="1500"></rsweb:ReportViewer>
    </div>
    </form>
</body>
</html>
