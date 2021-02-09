<%@ Page Language="VB" AutoEventWireup="false" CodeFile="cargar.aspx.vb" Inherits="cargar" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Impresion de Facturas</title>
        <link href="cargar.css" rel="stylesheet" />
        <link rel="preconnect" href="https://fonts.gstatic.com">
        <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@400;500;700&display=swap" rel="stylesheet">

        <script language="javascript" src="JScript.js" type="text/javascript"></script>

    </head>
    <body>
        <form class="form1" id="Form1" method="post" runat="server" EncType="multipart/form-data" action="cargar.aspx">
            <header class="navbar">
                <asp:Label ID="lbl_usuario" CssClass="usuario" Font-Names="Roboto Medium" runat="server" ForeColor="White" Font-Size="Large"></asp:Label>
                <asp:Label ID="lbl_idOperativo" CssClass="id" Font-Names="Roboto Medium" runat="server" ForeColor="Silver" Font-Size="Medium" Visible="false"></asp:Label>
                <div class="contenedor-salir">
                    <asp:HyperLink ID="HL_salir" Font-Names="Roboto Bold" runat="server" NavigateUrl="~/login.aspx" ForeColor="White">Salir</asp:HyperLink>
                </div>
            </header>
            <main class="contenedor">
                <p class="titulo">Carga de Facturas</p>
                <hr class="line"/>
                <asp:FileUpload CssClass="seleccionar-archivo" id="FileUploadControl" runat="server" />
                <asp:Button CssClass="subir" runat="server" id="UploadButton" text="Subir"  />
                <br /><br />
                <asp:Label CssClass="status" runat="server" id="StatusLabel" text="Estado del documento: " />
            </main>
        </form>
    </body>
</html>
<hr class="line" />
