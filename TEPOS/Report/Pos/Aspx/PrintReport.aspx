<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintReport.aspx.cs" Inherits="ERP.Report.Pos.Aspx.PrintReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Print Report</title>
</head>
<body style="margin: 0;">
    <form id="form1" runat="server">
        <asp:Label ID="Label1" runat="server" Text="Label" style="margin-left: 5%; font-size: 20px; font-weight: bold;"></asp:Label>
      <embed runat="server" id="ifrmPrint"  style="width: 100%; height: 640px;"/>
    </form>
</body>
</html>

</body>
</html>
