<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="_2C2PClientWebApp.Pages.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="height: 540px">
    <form id="form1" runat="server">
    <div>
    
        <asp:Label ID="lblTitle" runat="server" Text="2C2P Transaction Upload Panel" Font-Bold="true" Font-Size="Larger" style="margin-left:600px"></asp:Label>
    
    </div>
        
        <div>  
            <p>Browse to Upload File</p>  
            <asp:FileUpload ID="openFileChooser" runat="server" />  
        </div>  
        <p>  
            <asp:Button ID="btnChooseFile" runat="server" Text="Upload File" OnClick="btnChooseFile_Click" />  
        </p>  
    <p>  
        <asp:Label runat="server" ID="FileUploadStatus"></asp:Label>  
    </p> 

        <p>  
        <asp:Label runat="server" ID="lblFrom" style="margin-left:200px;text-align:center">Choose From & To Date</asp:Label>  
    </p>
        <asp:Calendar ID="fromDateCal" runat="server" OnSelectionChanged="FromDateChange"  style="float:left;padding-right:100px">
</asp:Calendar>
        <asp:Calendar ID="toDateCal" runat="server" OnSelectionChanged="ToDateChange" style="padding-left:100px">
</asp:Calendar>
<asp:TextBox ID="txtDateFrom" runat="server" style="padding-left:85px" Enabled="False"></asp:TextBox>
<asp:TextBox ID="txtDateTo" runat="server" style="padding-left:85px" Enabled="False"></asp:TextBox>
        <br/><br/>
        <b>Choose Currency&nbsp&nbsp</b><asp:DropDownList ID="dropListCurrency" runat="server" Width="123px">
            <asp:ListItem>Select</asp:ListItem>
            <asp:ListItem>USD</asp:ListItem>
            <asp:ListItem>EUR</asp:ListItem>
            <asp:ListItem>MMK</asp:ListItem>
            <asp:ListItem>SGD</asp:ListItem>
            <asp:ListItem>THB</asp:ListItem>
</asp:DropDownList>
        &nbsp&nbsp&nbsp&nbsp<b>Choose Status&nbsp&nbsp</b><asp:DropDownList ID="dropListStatus" runat="server" Width="123px">
            <asp:ListItem>Select</asp:ListItem>
            <asp:ListItem>A</asp:ListItem>
            <asp:ListItem>R</asp:ListItem>
            <asp:ListItem>D</asp:ListItem>
            
</asp:DropDownList>
         <br/><br/>
            <asp:Button ID="btnExecute" runat="server" Text="Execute Query" OnClick="btnExecuteQuery_Click" />  
        <p>
            <asp:TextBox ID="txtResult" runat="server" Height="170px" Width="1287px" Font-Overline="False" TextMode="MultiLine"></asp:TextBox>
        </p>
    </form>  
    </body>
</html>
