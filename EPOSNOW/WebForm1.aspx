<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="EPOSNOW.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>

<asp:FileUpload ID="FileUpload1" runat="server" />
<asp:Button ID="btnImport" runat="server" Text="Import" OnClick="ImportCsv" />
<hr />
<asp:GridView ID="GridView1" runat="server">
</asp:GridView>

        </div>
    </form>
</body>
</html>