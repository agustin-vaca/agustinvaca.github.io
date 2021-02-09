<%@ Page Language="VB" AutoEventWireup="false" CodeFile="index.aspx.vb" Inherits="index" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Impresion de Facturas</title>
        <link href="estilo.css" rel="stylesheet" />
        <link rel="preconnect" href="https://fonts.gstatic.com">
        <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@400;500;700&display=swap" rel="stylesheet">

        <script language="javascript" src="JScript.js" type="text/javascript"></script>


    </head>
    <body>
        <form class="form1" runat="server">
            <div class="navbar">
                <asp:Label ID="lbl_usuario" CssClass="usuario" Font-Names="Roboto Medium" runat="server" ForeColor="White" Font-Size="Large"></asp:Label>
                <asp:Label ID="lbl_idOperativo" CssClass="id" Font-Names="Roboto Medium" runat="server" ForeColor="Silver" Font-Size="Medium" Visible="false"></asp:Label>
                <div class="contenedor-salir">
                    <asp:HyperLink ID="HL_salir" Font-Names="Roboto Bold" runat="server" NavigateUrl="~/login.aspx" ForeColor="White">Salir</asp:HyperLink>
                </div>
            </div>
            <table class="tabla-entera">
                <tr>
                    <td align="center" colspan="5">
                        <asp:Label ID="Label3" runat="server" CssClass="text" BackColor="Transparent" BorderColor="Transparent"
                            Font-Names="Roboto Regular" Font-Bold="False" Font-Size="X-Large" Text="Facturas de Servicio"></asp:Label></td>
                </tr>
                <tr>
                    <td align="right" colspan="5" class="espacio-titulo">
                        <hr />
                    </td>
                </tr>
                <tr>
                    <td class="empresa">
                        <img src="Imagenes/1.PNG" class="numero"/>
                        <p>Seleccione la Empresa de Servicio</p>
                    </td>
                </tr>
                <tr>
                    <td class="opciones" colspan="4">
                        <asp:DropDownList ID="ddlModulo" CssClass="menu" runat="server" AutoPostBack="True">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td colspan="5" style="background-color: #dcdcdc">
                        &nbsp;
                    </td>
                </tr>
                <tr>
    <%--                <td class="telefono">
                        Telefono / Cuenta / Recaudacion (Opcional)
                    </td>--%>
                    <td colspan="4">
                        <asp:TextBox Visible="false" ID="txtCodigoRecaudacion" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <%--<td class="auto-style2">
                        Codigo Agencia (Opcional)
                    </td>--%>
                    <td colspan="4">
                        <asp:TextBox Visible="false" ID="txtCodigoAgencia" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
    <%--                <td class="auto-style2">
                        Codigo Cliente (Opcional)
                    </td>--%>
                    <td colspan="4">
                        <asp:TextBox Visible="false" ID="txtCodigoCliente" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td class="empresa" id="segundopaso">
                        <img src="Imagenes/2.PNG" class="numero2"/>
                        <p class="ingresefecha">Ingrese la Fecha del Débito</p>
                    </td>
                </tr>

                <tr>
    <%--                <td colspan="3">
                      <asp:CheckBox ID="chb_mostrar1" runat="server" Text="Mostrar listado Deb.Auto.CJN" />
                        &nbsp; &nbsp; &nbsp; Lineas Salto Pag<asp:TextBox ID="txt_lineas" runat="server"
                            Width="32px"></asp:TextBox><br />
                        <asp:CheckBox ID="chb_mostrar2" runat="server" Text="Mostrar listado BuscaCliente Sintesis" />
                        <br />
                        <asp:CheckBox ID="chb_mostrar3" runat="server" Text="Mostrar Listado Items de Cuenta" />
                        <!--Nro. Registros a pagar =<asp:TextBox ID="txt_NroRegistros" runat="server" Width="48px"></asp:TextBox><br />-->
                        <asp:CheckBox ID="chb_pagar_cjn" runat="server" Checked="True" Text="Pagar en CJN" /></td>--%>
                    <td colspan="2" class="empresa" id="margen-dos">
                        <asp:TextBox CssClass="menu" placeholder="AAAAMMDD" ID="txt_fecha" runat="server"></asp:TextBox>
                        <div class="fecha">
                            <p id="fecha">* Ejemplo: 1 de Febrero 2020:</p>
                            <p id="fecha2">20200201</p>
                        </div>

                    </td>

                </tr>
                <tr>
                    <td align="center" colspan="10" style="background-color: #f0fff0" class="auto-style3">
                        <div class="buscar">
                            <asp:Button ID="btn_conciliacion_det" runat="server" Text="Buscar"
                            Font-Bold="True" Height="32px" Width="192px" />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td align="center" colspan="3" rowspan="1" style="background-color: #f0fff0">
                        &nbsp;</td>
                    <td align="center" colspan="1" rowspan="1" style="background-color: #f0fff0" class="auto-style3">
                        &nbsp;</td>
                    <td align="center" colspan="1" rowspan="1" style="background-color: #f0fff0" class="auto-style3">
                        &nbsp;</td>
                </tr>
                <tr>
                    <td align="center" colspan="0" rowspan="1" style="background-color: #f0fff0">
                    </td>
                    <td align="center" colspan="0" rowspan="1" style="background-color: #f0fff0" class="auto-style3">
                        &nbsp;</td>
                    <td align="center" colspan="10" rowspan="1" style="background-color: #f0fff0" class="auto-style3">
                        <asp:Label Visible="false" ID="lbl_mensaje" runat="server"></asp:Label><br />
                        <asp:Label Visible="false" ID="lbl_duracion" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="5">
                        <p class="control-f">*use el comando CTRL F para buscar las facturas por numero telefonico</p>
                        <asp:Label ID="lbl_buscaDebitoAuto" runat="server" Font-Bold="True" Text="Listado de Debitos Automaticos - CJN"
                            Visible="False" BackColor="LightGreen" Height="30px"></asp:Label><br />
                        <asp:GridView ID="gvDebitosAutomaticos" runat="server" Height="16px">
                        </asp:GridView>
                        <asp:Label ID="lbl_buscaCliente" runat="server" Font-Bold="True" Text="Listado - Busca Cliente - SINTESIS"
                            Visible="False" BackColor="PaleTurquoise" Height="30px"></asp:Label><br />
                        <asp:GridView ID="gvBuscarCliente" runat="server">
                        </asp:GridView>
                        <asp:Label ID="lbl_buscarItemsDeCuenta" runat="server" Font-Bold="True" Text='Listado - Items pendientes de pagar segun "Busca Cliente" - SINTESIS'
                            Visible="False" BackColor="PaleTurquoise" Height="30px"></asp:Label><br />
                        <asp:GridView ID="gvBuscarItemsDeCuenta" runat="server">
                        </asp:GridView>
                        <asp:Label ID="lbl_RegistrarPago" runat="server" Font-Bold="True" Text="Listado - Registrar Pagos"
                            Visible="False" BackColor="PaleTurquoise" Height="30px"></asp:Label><br />
                        <asp:GridView ID="gvRegistrarPago" runat="server">
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="ckb" runat="server" Checked="true" Enabled="true" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:Label ID="lbl_Pagar" runat="server" BackColor="PaleTurquoise" Font-Bold="True"
                            Height="30px" Text="Listado - Pagar" Visible="False"></asp:Label>
                        <asp:GridView ID="gv_pagar" runat="server">
                        </asp:GridView>
                        <br />
                        <asp:Label ID="lbl_conciliaRes" runat="server" BackColor="PaleTurquoise" Font-Bold="True"
                            Height="30px" Font-Names="Roboto Medium" Text="Conciliacion Resumida" Visible="False"></asp:Label>&nbsp;
                        <asp:GridView ID="gv_concilia_res" runat="server">
                        </asp:GridView>
                        <asp:Label ID="lbl_conciliaDet" runat="server" Font-Names="Roboto Medium" Font-Bold="True"
                            Height="30px" Text="Facturas" Visible="False">
                        </asp:Label>
                        <asp:GridView CssClass="tabla-facturas" ID="gv_concilia_det" runat="server">
                            <Columns>
                                <asp:TemplateField ControlStyle-Font-Names="Roboto Regular" HeaderText="Reimprimir">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="ckb_imprimir" runat="server" Checked="false" Enabled="true" Font-Names="Roboto Light" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:Label ID="lbl_conciliaCRE" runat="server" Font-Names="Roboto Medium" Font-Bold="True"
                            Height="30px" Text="Facturas" Visible="False">
                        </asp:Label>
                        <asp:GridView CssClass="tabla-facturas" ID="gv_concilia_CRE" runat="server">
                            <Columns>
                                <asp:TemplateField ControlStyle-Font-Names="Roboto Regular" HeaderText="Reimprimir">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="cre_imprimir" runat="server" Checked="false" Enabled="true" Font-Names="Roboto Light" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:Label ID="lbl_conciliaSAGUAPAC" runat="server" Font-Names="Roboto Medium" Font-Bold="True"
                            Height="30px" Text="Facturas" Visible="False">
                        </asp:Label>
                        <asp:GridView CssClass="tabla-facturas" ID="gv_concilia_SAGUAPAC" runat="server">
                            <Columns>
                                <asp:TemplateField ControlStyle-Font-Names="Roboto Regular" HeaderText="Reimprimir">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="sag_imprimir" runat="server" Checked="false" Enabled="true" Font-Names="Roboto Light" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:Button ID="btn_reimprimir" runat="server" Text="Reimprimir" Visible="False" /><br />
                        <asp:Label ID="lbl_conciliaDetDeb" runat="server" BackColor="PaleTurquoise" Font-Bold="True"
                            Height="30px" Text="Conciliacion Detallada x Deb.Auto" Visible="False"></asp:Label>
                        <asp:GridView ID="gv_concilia_det_deb" runat="server">
                        </asp:GridView>
                        <asp:Button ID="btn_ascii_facturas" runat="server" Text="Crear ascii" Visible="False" />
                        <br />
                        <asp:Label ID="lbl_facturasPagadas" runat="server" BackColor="LightGreen" Font-Bold="True"
                            Height="30px" Text="Facturas Pagadas" Visible="False"></asp:Label>
                        <br />
                        <asp:GridView ID="gv_facturas" runat="server">
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                </tr>
            </table>
        </form>
    </body>
</html>
