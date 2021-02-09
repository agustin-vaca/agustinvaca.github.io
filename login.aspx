<%@ Page Language="VB" AutoEventWireup="false" CodeFile="login.aspx.vb" Inherits="login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Impresión de Facturas</title>
    <link href="estilo.css" rel="stylesheet" />
</head>

<body>
    <div class="navbar">
    </div>

<div id="content" >


    <form id="form1" runat="server">
        <div >
            
            <center>
                &nbsp;</center>
            <center>
                &nbsp;</center>
            <center>
                &nbsp;</center>
            <center>&nbsp;</center>
            <center>
                &nbsp;</center>
            <center>
                &nbsp;</center>
            <center>
               <div class="titulo-login">
                   <asp:Label ID="Label3" runat="server" CssClass="text"  BackColor="Transparent" BorderColor="Transparent"
                        Font-Names="Roboto Regular" Font-Bold="False" Font-Size="X-Large" Text="Facturas de Servicio"></asp:Label>
                    <hr />
               </div>
            <asp:Label id="mensaje" runat="server" ForeColor="Red" Font-Size="Medium" Font-Bold="True"></asp:Label>
            <table border="1" cellpadding ="0.5" cellspacing ="0.5" >
                <tr>
                    <td colspan = "2">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:Label ID="Label2" runat="server" Font-Bold="True" Text="Usuario Tritón"></asp:Label></td>
                    <td style="width: 154px">
                        <asp:TextBox ID="UsernameTextBox" runat="server" Width="150px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:Label ID="Label1" runat="server" Font-Bold="True" Text="Contraseña"></asp:Label></td>
                    <td style="width: 154px; height: 22px;">
                        <asp:TextBox ID="PasswordTextBox" runat="server" TextMode="Password" Width="150px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="right">
                        &nbsp;<asp:Button ID="ok" runat="server" Text="Aceptar" Width="70px" />&nbsp;
                        <asp:Button ID="Button1" runat="server" Text="Cancelar" Width="70px" /></td>
                </tr>
            </table>
            </center>
            <center>
                &nbsp;</center>
            <center>
                &nbsp;</center>
            <center>
                &nbsp;</center>
        </div>
    </form>
</div> 
</body>
</html>
