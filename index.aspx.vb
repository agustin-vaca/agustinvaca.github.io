Imports AjaxPro

Imports System.Data
Imports UTIL
Imports System.Drawing
Imports System.IO
Imports System.Diagnostics
Imports System.Threading.Thread
Imports System.Collections.Generic
Imports iTextSharp.text.pdf
Imports iTextSharp.text.pdf.parser

Partial Class index
    Inherits System.Web.UI.Page

    Dim ws1 As New wsSintesis.IntegradoWSService
    Dim wsModulos As New wsSintesis.wsModulos
    Dim wsCriterios As New wsSintesis.wsCriterios
    Dim wsCuentas As New wsSintesis.wsCuentas
    Dim wsItems As New wsSintesis.wsItems
    Dim wsSubItems As New wsSintesis.wsSubItems
    Dim wsItemPago As New wsSintesis.wsRespuesta
    Dim wsRespuesta As New wsSintesis.wsRespuesta
    Dim wsImpresion As New wsSintesis.wsImpresion
    Dim wsConciliaResumen As New wsSintesis.wsConciliaResumen
    Dim wsConciliaDetalle As New wsSintesis.wsConciliaDetalle
    Dim wsOperPagadas As New wsSintesis.wsOperPagadas
    Dim dia As String
    Dim mes As String
    Dim anho As String

    Private Shared _idOperativo As String
    Private Shared _tipo_acceso As String = "DESARROLLO" 'DESARROLLO / PRODUCCION
    Private Shared _ruta_archivo As String ' produccion = d:\DebitoSintesis\archivos\
    Private Shared _cliente_cjn As String = "134802" ''cuando este listo el algoritmo de lectura de facturas para la contabilidad y libro de compras se debe quitar las XXX a 134802XXX
    Public pagar_ws_cjn As Boolean = True

    Private Shared _ini_dt_buscarItem As Boolean = False

    Public size_font_gridview As Integer = 8


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            iniciarSesion()
            llenarModulo_y_Criterio()

            If ddlModulo.Text = "" Then
                Mensaje(Me, "NO SE HA ESTABLECIDO CONEXION CON EL WEBSERVICE DE SINTESIS...")
            End If

            'txt_NroRegistros.Text = 1
            'txt_lineas.Text = 6
        End If

        If IsNothing(Session("Usuario")) Then
            Response.Redirect("login.aspx")
        End If

        If _tipo_acceso = "DESARROLLO" Then
            _ruta_archivo = "C:\AGUSTIN\DebitoSintesis\archivos\"
        ElseIf _tipo_acceso = "PRODUCCION" Then
            _ruta_archivo = "d:\DebitoSintesis\archivos\"
        End If

        lbl_usuario.Text = "usuario: " & Session("Usuario")

    End Sub

    Public Function Obtener_RecaudadorSINTESIS(ByVal weCliente As Integer) As Integer
        If weCliente = 3 Then
            Return RecaudadoresServicios.Telecel
        End If
        If weCliente = 7 Then
            Return RecaudadoresServicios.Nuevatel
        End If
        If weCliente = 8 Then
            Return RecaudadoresServicios.Entel
        End If
        Return 0
    End Function

    Sub Mensaje(ByRef oPage As Page, ByVal sAviso As String)
        If sAviso.Trim.Length > 0 Then
            Dim s As String = "<script language =""jscript""> alert('" & sAviso & "') </script>"
            ClientScript.RegisterStartupScript(Me.GetType(), "alert", s)
        End If
    End Sub

    Public Sub MostrarMensaje(ByVal men As String)
        Mensaje(Me, men)
        lbl_mensaje.Text = men
    End Sub

    Protected Function getMSGError(ByVal msg As String) As String
        Dim c As Integer = InStr(msg, "ç")
        If c > 0 Then
            Dim r As String() = msg.Split("ç")
            If Not IsNothing(r) Then
                Return r(1)
            Else
                Return Nothing
            End If
        Else
            If Not IsNothing(msg) Then
                Return msg
            Else
                Return Nothing
            End If
        End If
    End Function

    Protected Sub iniciarSesion()
        Dim res As Boolean = True
        Try
            Dim wsInicio As New wsSintesis.wsInicio

            'If _tipo_acceso = "DESARROLLO" Then
            '    'desarrrollo()
            '    wsInicio = ws1.iniciarSesion("webJesusNazareno", "Service2015", "APWS")
            'ElseIf _tipo_acceso = "PRODUCCION" Then
            'produccion
            wsInicio = ws1.iniciarSesion("servicescjn", "Nuevo2018", "APWS")
            'End If

            If wsInicio.codError = 0 Then
                res = True
                _idOperativo = wsInicio.idOperativo
            Else
                If wsInicio.codError = 15 Or wsInicio.codError = 16 Then
                    res = False
                    MostrarMensaje("Cliente debe logearse nuevamente")
                Else
                    res = False
                    MostrarMensaje(wsInicio.mensaje)
                End If
            End If
        Catch ex As Exception
            res = False
            MostrarMensaje(ex.Message)
        End Try

    End Sub

    Protected Function servicioEstaActivo() As Boolean
        Dim res As Boolean = True
        Try
            Dim wsRespuesta As New wsSintesis.wsRespuesta
            wsRespuesta = ws1.servicioEstaActivo

            If wsRespuesta.codError = 0 Then
                res = True
            Else
                If wsRespuesta.codError = 15 Or wsRespuesta.codError = 16 Then
                    res = False
                    MostrarMensaje("Cliente debe logearse nuevamente")
                Else
                    res = False
                    MostrarMensaje(wsRespuesta.mensaje)
                End If
            End If
        Catch ex As Exception
            res = False
            MostrarMensaje(ex.Message)
        End Try
        Return res
    End Function

    Protected Function sesionEstaActiva(ByVal IdOperativo As String) As Boolean
        Dim res As Boolean = True
        Try
            Dim wsRespuesta As New wsSintesis.wsRespuesta
            wsRespuesta = ws1.sesionEstaActiva(IdOperativo)

            If wsRespuesta.codError = 0 Then
                res = True
            Else
                If wsRespuesta.codError = 15 Or wsRespuesta.codError = 16 Then
                    res = False
                    MostrarMensaje("Cliente debe logearse nuevamente")
                Else
                    res = False
                    MostrarMensaje(wsRespuesta.mensaje)
                End If
            End If
        Catch ex As Exception
            res = False
            MostrarMensaje(ex.Message)
        End Try
        Return res
    End Function

    Protected Sub llenarModulo_y_Criterio()
        lbl_idOperativo.Text = ""
        If servicioEstaActivo() Then

            If sesionEstaActiva(_idOperativo) = True Then
                Session("v_idOperativo") = _idOperativo

                lbl_idOperativo.Text = "Id:" & _idOperativo

                Dim aux As Data.DataTable = Obtener_Modulo(_idOperativo)

                ddlModulo.DataSource = aux
                ddlModulo.DataValueField = "codModulo"
                ddlModulo.DataTextField = "descripcion"
                ddlModulo.DataBind()
                ddlModulo.Items.Insert(0, New ListItem("Seleccione", "NA"))
                ddlModulo.Items.Insert(1, New ListItem("CRE", "1"))
                ddlModulo.Items.Insert(2, New ListItem("SAGUAPAC", "2"))

                'Dim aux2 As Data.DataTable = Obtener_Criterios(ddlModulo.SelectedValue)
                'ddlCriterios.DataSource = aux2
                'ddlCriterios.DataValueField = "CodCriterio"
                'ddlCriterios.DataTextField = "descripcion"
                'ddlCriterios.DataBind()

                'ddlCriterios.Enabled = False
            End If
        End If
    End Sub

    Protected Function Obtener_Modulo(ByVal v_idOperativo As String) As Data.DataTable
        Try
            Dim td As Data.DataTable = New Data.DataTable()
            Dim row As DataRow

            Dim colModulo As New DataColumn
            colModulo = New DataColumn
            colModulo.DataType = System.Type.GetType("System.String")
            colModulo.ColumnName = "codModulo"
            td.Columns.Add(colModulo)

            Dim coldescripcion As New DataColumn
            coldescripcion = New DataColumn
            coldescripcion.DataType = System.Type.GetType("System.String")
            coldescripcion.ColumnName = "descripcion"
            td.Columns.Add(coldescripcion)

            wsModulos = ws1.obtenerModulos(v_idOperativo)
            Dim wsModulo() As wsSintesis.wsModulo = wsModulos.modulo

            'Dim wsModulo_codModulo As Short = wsModulo(0).codModulo
            'Dim wsModulo_descripcion As String = wsModulo(0).descripcion
            'Dim wsModulo_tipoProceso As String = wsModulo(0).tipoProceso

            For v As Integer = 0 To (wsModulo.Length - 1)
                row = td.NewRow
                row("codModulo") = New ListItem(wsModulo(v).codModulo)
                row("descripcion") = New ListItem(wsModulo(v).descripcion)
                td.Rows.Add(row)
            Next
            Return td
        Catch ex As Exception
            Return Nothing
            ' mensaje(ex.Message)
        End Try
    End Function

    Protected Function Obtener_Criterios(ByVal _modulo As Integer) As Data.DataTable
        Try
            Dim td As Data.DataTable = New Data.DataTable()
            Dim row As DataRow

            _idOperativo = Session("v_idOperativo")

            Dim colCriterio As New DataColumn
            colCriterio = New DataColumn
            colCriterio.DataType = System.Type.GetType("System.String")
            colCriterio.ColumnName = "CodCriterio"
            td.Columns.Add(colCriterio)

            Dim coldescripcion As New DataColumn
            coldescripcion = New DataColumn
            coldescripcion.DataType = System.Type.GetType("System.String")
            coldescripcion.ColumnName = "descripcion"
            td.Columns.Add(coldescripcion)

            wsCriterios = ws1.obtenerCriteriosParaModulo(_idOperativo, _modulo, True)
            Dim wsCriterio() As wsSintesis.wsCriterio = wsCriterios.criterio

            For v As Integer = 0 To (wsCriterio.Length - 1)
                row = td.NewRow
                row("CodCriterio") = New ListItem(wsCriterio(v).codCriterio)
                row("descripcion") = New ListItem(wsCriterio(v).descripcion)
                '  td.Rows.Add()
                td.Rows.Add(row)
            Next

            Return td
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Protected Sub ddlModulo_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlModulo.SelectedIndexChanged

        limpiar_gridview()
        'Dim aux As Data.DataTable = Obtener_Criterios(ddlModulo.SelectedValue)
        'ddlCriterios.DataSource = aux
        'ddlCriterios.DataValueField = "CodCriterio"
        'ddlCriterios.DataTextField = "descripcion"
        'ddlCriterios.DataBind()

        'txtCodigoRecaudacion.Focus()
    End Sub

    Protected Sub limpiar_gridview()
        lbl_buscaCliente.Visible = False
        gvBuscarCliente.DataSource = Nothing
        gvBuscarCliente.DataBind()

        lbl_buscaDebitoAuto.Visible = False
        gvDebitosAutomaticos.DataSource = Nothing
        gvDebitosAutomaticos.DataBind()

        lbl_buscarItemsDeCuenta.Visible = False
        gvBuscarItemsDeCuenta.DataSource = Nothing
        gvBuscarItemsDeCuenta.DataBind()

        lbl_RegistrarPago.Visible = False
        gvRegistrarPago.DataSource = Nothing
        gvRegistrarPago.DataBind()

        lbl_Pagar.Visible = False
        gv_pagar.DataSource = Nothing
        gv_pagar.DataBind()

        lbl_mensaje.Text = Nothing
        Session("CDmensaje") = Nothing
        lbl_duracion.Text = Nothing

        lbl_conciliaRes.Visible = False
        gv_concilia_res.DataSource = Nothing
        gv_concilia_res.DataBind()

        lbl_conciliaDet.Visible = False
        btn_reimprimir.Visible = False
        gv_concilia_det.DataSource = Nothing
        gv_concilia_det.DataBind()

        lbl_conciliaCRE.Visible = False
        btn_reimprimir.Visible = False
        gv_concilia_CRE.DataSource = Nothing
        gv_concilia_CRE.DataBind()

        lbl_conciliaSAGUAPAC.Visible = False
        btn_reimprimir.Visible = False
        gv_concilia_SAGUAPAC.DataSource = Nothing
        gv_concilia_SAGUAPAC.DataBind()

        btn_ascii_facturas.Visible = False
        lbl_conciliaDetDeb.Visible = False
        gv_concilia_det_deb.DataSource = Nothing
        gv_concilia_det_deb.DataBind()

        lbl_facturasPagadas.Visible = False

        gv_facturas.DataSource = Nothing
        gv_facturas.DataBind()



    End Sub


    Protected Sub ini_dt_crearBuscarCliente(ByVal tabla As Data.DataTable)

        tabla.Columns.Add("CodRecaudacion", Type.GetType("System.String"))
        tabla.Columns.Add("NroOperacion", Type.GetType("System.String"))
        tabla.Columns.Add("FechaOpe", Type.GetType("System.String"))
        tabla.Columns.Add("BCmensaje", Type.GetType("System.String"))
        tabla.Columns.Add("CodCuenta", Type.GetType("System.String"))
        tabla.Columns.Add("Nombre", Type.GetType("System.String"))
        tabla.Columns.Add("Detalle", Type.GetType("System.String"))
        tabla.Columns.Add("CodServicio", Type.GetType("System.String"))
        tabla.Columns.Add("Servicio", Type.GetType("System.String"))

    End Sub

    Protected Sub ini_dt_buscarItem(ByVal tabla As Data.DataTable)

        If _ini_dt_buscarItem = False Then
            'tabla.Columns.Remove("CodRecaudacion")
            tabla.Columns.Add("CodRecaudacion", Type.GetType("System.String"))
            tabla.Columns.Add("NroCuenta", Type.GetType("System.String"))
            tabla.Columns.Add("NroOperacion", Type.GetType("System.String"))
            tabla.Columns.Add("FechaOperativa", Type.GetType("System.String"))
            tabla.Columns.Add("CodCuenta", Type.GetType("System.String"))
            tabla.Columns.Add("CodServicio", Type.GetType("System.String"))
            tabla.Columns.Add("Mensaje", Type.GetType("System.String"))
            tabla.Columns.Add("NitFac", Type.GetType("System.String"))
            tabla.Columns.Add("NombreFac", Type.GetType("System.String"))
            tabla.Columns.Add("CambiaNitYNombreFac", Type.GetType("System.String"))
            tabla.Columns.Add("nroItem", Type.GetType("System.String"))
            tabla.Columns.Add("descItem", Type.GetType("System.String"))
            tabla.Columns.Add("moneda", Type.GetType("System.String"))
            tabla.Columns.Add("dependeDeItem", Type.GetType("System.String"))
            tabla.Columns.Add("formaPago", Type.GetType("System.String"))
            tabla.Columns.Add("Monto", Type.GetType("System.String"))
            tabla.Columns.Add("Otro", Type.GetType("System.String"))

            _ini_dt_buscarItem = True
        End If

        tabla.Rows.Clear()

    End Sub

    Protected Sub ini_dt_RegistrarPago(ByVal tabla As Data.DataTable)

        tabla.Columns.Add("CodRecaudacion", Type.GetType("System.String"))
        tabla.Columns.Add("CodigoCliente", Type.GetType("System.String"))
        tabla.Columns.Add("CodAgencia", Type.GetType("System.String"))
        tabla.Columns.Add("IdOperativo", Type.GetType("System.String"))
        tabla.Columns.Add("CodModulo", Type.GetType("System.String"))
        tabla.Columns.Add("NroOperacion", Type.GetType("System.String"))
        tabla.Columns.Add("FechaOperativa", Type.GetType("System.String"))
        tabla.Columns.Add("Cuenta", Type.GetType("System.String"))
        tabla.Columns.Add("Servicio", Type.GetType("System.String"))

        tabla.Columns.Add("NombreFac", Type.GetType("System.String"))
        tabla.Columns.Add("NitFac", Type.GetType("System.String"))

        tabla.Columns.Add("DirecEnvio", Type.GetType("System.String"))

        tabla.Columns.Add("DescItem", Type.GetType("System.String"))

        tabla.Columns.Add("NroItem", Type.GetType("System.String"))
        tabla.Columns.Add("Monto", Type.GetType("System.String"))
        tabla.Columns.Add("NroCuenta", Type.GetType("System.String"))
        tabla.Columns.Add("SaldoCuenta", Type.GetType("System.String"))

    End Sub

    Protected Sub ini_dt_Pagar(ByVal tabla As Data.DataTable)

        tabla.Columns.Add("CodRecaudacion", Type.GetType("System.String"))
        tabla.Columns.Add("CodigoCliente", Type.GetType("System.String"))
        tabla.Columns.Add("CodAgencia", Type.GetType("System.String"))
        tabla.Columns.Add("IdOperativo", Type.GetType("System.String"))
        tabla.Columns.Add("CodModulo", Type.GetType("System.String"))
        tabla.Columns.Add("NroOperacion", Type.GetType("System.String"))
        tabla.Columns.Add("FechaOperativa", Type.GetType("System.String"))
        tabla.Columns.Add("Cuenta", Type.GetType("System.String"))
        tabla.Columns.Add("Servicio", Type.GetType("System.String"))

        tabla.Columns.Add("NombreFac", Type.GetType("System.String"))
        tabla.Columns.Add("NitFac", Type.GetType("System.String"))

        tabla.Columns.Add("DirecEnvio", Type.GetType("System.String"))

        tabla.Columns.Add("DescItem", Type.GetType("System.String"))

        tabla.Columns.Add("NroItem", Type.GetType("System.String"))
        tabla.Columns.Add("Monto", Type.GetType("System.String"))
        tabla.Columns.Add("NroCuenta", Type.GetType("System.String"))
        tabla.Columns.Add("SaldoCuenta", Type.GetType("System.String"))

    End Sub


    Protected Sub salir(ByVal mensaje As String)
        Session.Clear()
        Response.Redirect("login.aspx?m=" & mensaje)

        'destruir sesion
        'validar que no se pueda volver atras a la pagina index.aspx; en index se debe validar el valor de una variable session
        'Response.Redirect("login.aspx?mensaje=" & mensaje.ToString)
    End Sub

    Private Shared aux_deb_auto As Data.DataTable
    Private Shared dt_buscarItem As Data.DataTable = New Data.DataTable()


    Protected Function NVL(ByVal campo1 As Object, ByVal campo2 As Object) As Object
        If IsNothing(campo1) Or IsDBNull(campo1) Then
            Return campo2
        Else
            Return campo1
        End If
    End Function

    Private Shared dt_RegistrarPago As Data.DataTable
    Private Shared dt_Pagar As Data.DataTable


    Protected Sub LlenaDataTable_Pagar()

        Dim sigue As Boolean = True
        Dim contador As Integer = 0

        Try
            Dim ds_Pagar As DataSet = New DataSet("dataset")
            dt_Pagar = ds_Pagar.Tables.Add("tabla")

            ini_dt_Pagar(dt_Pagar)

            Dim Fila As DataRow = Nothing

            For i As Integer = 0 To gvRegistrarPago.Rows.Count - 1

                Dim vec_monto() As String = gvRegistrarPago.Rows(i).Cells.Item(15).Text.Split("|")
                Dim suma_monto As Double = 0.0
                For x As Integer = 0 To vec_monto.Length - 1
                    suma_monto += vec_monto(x)
                Next

                Dim saldo_cta As Double = gvRegistrarPago.Rows(i).Cells.Item(17).Text
                sigue = True
                If suma_monto > saldo_cta Then
                    sigue = False
                End If

                If gvRegistrarPago.Rows(i).Cells.Item(2).Text = _cliente_cjn Then
                    sigue = True
                End If

                If CType(gvRegistrarPago.Rows(i).FindControl("ckb"), CheckBox).Checked = True And sigue = True Then

                    Fila = ds_Pagar.Tables(0).NewRow()

                    Fila("CodRecaudacion") = gvRegistrarPago.Rows(i).Cells.Item(1).Text
                    Fila("CodigoCliente") = gvRegistrarPago.Rows(i).Cells.Item(2).Text
                    Fila("CodAgencia") = gvRegistrarPago.Rows(i).Cells.Item(3).Text
                    Fila("IdOperativo") = gvRegistrarPago.Rows(i).Cells.Item(4).Text
                    Fila("CodModulo") = gvRegistrarPago.Rows(i).Cells.Item(5).Text
                    Fila("NroOperacion") = gvRegistrarPago.Rows(i).Cells.Item(6).Text
                    Fila("FechaOperativa") = gvRegistrarPago.Rows(i).Cells.Item(7).Text
                    Fila("Cuenta") = gvRegistrarPago.Rows(i).Cells.Item(8).Text
                    Fila("Servicio") = gvRegistrarPago.Rows(i).Cells.Item(9).Text
                    Fila("NombreFac") = gvRegistrarPago.Rows(i).Cells.Item(10).Text
                    Fila("NitFac") = gvRegistrarPago.Rows(i).Cells.Item(11).Text
                    Fila("DirecEnvio") = ""

                    Fila("DescItem") = gvRegistrarPago.Rows(i).Cells.Item(13).Text
                    Fila("NroItem") = gvRegistrarPago.Rows(i).Cells.Item(14).Text
                    Fila("Monto") = gvRegistrarPago.Rows(i).Cells.Item(15).Text
                    Fila("NroCuenta") = gvRegistrarPago.Rows(i).Cells.Item(16).Text
                    Fila("SaldoCuenta") = gvRegistrarPago.Rows(i).Cells.Item(17).Text

                    ds_Pagar.Tables(0).Rows.Add(Fila)
                End If

            Next

            lbl_Pagar.Visible = True
            gv_pagar.Font.Size = size_font_gridview
            gv_pagar.DataSource = dt_Pagar
            gv_pagar.DataBind()
        Catch ex As Exception
            MostrarMensaje("LlenaDataTable_Pagar: " & ex.Message.ToString)
        End Try
    End Sub

    Protected Function formatea_DescItem(ByVal old_cadena) As String
        Dim new_cadena As String = old_cadena

        Dim a As Integer = InStr(new_cadena, "[")
        Dim b As Integer = InStr(new_cadena, "]")
        If a > 0 Then
            new_cadena = new_cadena.Substring(a, b - a - 1)
        End If

        new_cadena = new_cadena.Replace("-", "").Trim
        new_cadena = new_cadena.Replace("Deuda", "").Trim
        new_cadena = new_cadena.Replace("de", "").Trim
        new_cadena = new_cadena.Replace("periodo", "").Trim
        new_cadena = new_cadena.Replace("Periodo", "").Trim

        If ddlModulo.SelectedValue = 3 Then
            If new_cadena.Length > 6 Then
                new_cadena = new_cadena.Substring(0, 6)
            End If
        End If


        Return new_cadena
    End Function





    'NOTA: en Banca_PagoServicios el campo numero factura se envia "0" y en agencia se envia "129".
    'Para los servicios que tiene la CJN como cliente 134802, ¿seguiremos usando el debito a caja de ahorro?{contemplar que tiene que haber saldo en la cuenta}; para los servicios COTAS, CRE, SAGUAPAC, ya no se realiza movimiento de cta, se realiza la contabilidad segun datos cargados de las facturas pagadas.


    Protected Function Registrar_Pago(ByVal idOperativo As String, ByVal nroOperacion As Integer, ByVal fechaOperativa As Integer, ByVal codModulo As Short, ByVal cuenta As String, ByVal servicio As Short, ByVal nombreFac As String, ByVal nitFac As String, ByVal direcEnvio As String, ByVal items() As wsSintesis.wsItemPago) As Boolean
        'Return True
        Dim res As Boolean = False

        wsRespuesta = ws1.registrarPago(idOperativo, nroOperacion, True, fechaOperativa, True, codModulo, True, cuenta, servicio, True, nombreFac, nitFac, direcEnvio, items)
        If wsRespuesta.codError = 0 Then
            res = True
        Else
            If wsRespuesta.codError = 15 Or wsRespuesta.codError = 16 Then
                Session("CDmensaje") = "SINTESIS: Cliente debe logearse nuevamente..." & wsRespuesta.mensaje.ToString
                salir(Session("CDmensaje"))

            Else
                Session("CDmensaje") = cuenta & " " & wsRespuesta.mensaje.ToString

            End If
        End If
        Return res
    End Function
    Dim nombreArchivo As String
    Public Sub CrearArchivoFactura(ByVal CodAgencia As String, ByVal idOperativo As String, ByVal nroOperacion As Integer, ByVal fechaOperativa As Integer, ByVal codModulo As Short, ByVal servicio As Short, ByVal cuenta As String, ByVal monto As Integer, ByVal tipoFormulario As String)
        Try

            wsImpresion = ws1.obtenerImpresion(idOperativo, nroOperacion, True, fechaOperativa, True, codModulo, True, servicio, True, cuenta, tipoFormulario)
            If wsImpresion.codError = 0 Then
                Dim oSW As New StreamWriter(_ruta_archivo & ddlModulo.SelectedItem.Text & "_FACTURA_" & CodAgencia & "_" & fechaOperativa & "_" & idOperativo & "_" & nroOperacion & "_" & cuenta & ".txt")
                nombreArchivo = _ruta_archivo & ddlModulo.SelectedItem.Text & "_FACTURA_" & CodAgencia & "_" & fechaOperativa & "_" & idOperativo & "_" & nroOperacion & "_" & cuenta & ".txt"
                Dim Linea As String = ""

                For x As Integer = 0 To wsImpresion.lineaFactura.Length - 1

                    Dim a As Integer = InStr(wsImpresion.lineaFactura(x), "PB")

                    If a > 0 Then
                        'Linea &= wsImpresion.lineaFactura(x) & vbCrLf
                        'Linea &= vbCr
                        'For y As Integer = 1 To txt_lineas.Text
                        '    Linea &= vbNewLine
                        'Next
                    Else
                        Linea &= wsImpresion.lineaFactura(x) & vbNewLine
                    End If
                Next

                oSW.WriteLine(Linea)
                oSW.Flush()
                oSW.Close()
            ElseIf wsImpresion.codError = 15 Or wsImpresion.codError = 16 Then
                'Revertir_Pago(idOperativo, nroOperacion, fechaOperativa, codModulo, servicio, cuenta, monto)
                Session("CDmensaje") = "Cliente debe logearse nuevamente : " & wsImpresion.mensaje.ToString
                salir(Session("CDmensaje"))
            Else
                'Revertir_Pago(idOperativo, nroOperacion, fechaOperativa, codModulo, servicio, cuenta, monto)
                Session("CDmensaje") = cuenta & " " & wsImpresion.mensaje.ToString
            End If

        Catch ex As Exception
            Session("CDmensaje") = ex.Message
            Revertir_Pago(idOperativo, nroOperacion, fechaOperativa, codModulo, servicio, cuenta, monto)
        End Try


    End Sub

    Protected Sub Revertir_Pago(ByVal idOperativo As String, ByVal nroOperacion As Integer, ByVal fechaOperativa As Integer, ByVal codModulo As Short, ByVal servicio As Short, ByVal cuenta As String, ByVal montoPagado As Double)
        wsRespuesta = ws1.revertirPago(idOperativo, nroOperacion, True, fechaOperativa, True, codModulo, True, servicio, True, cuenta, montoPagado, True)
        If wsRespuesta.codError = 0 Then
            lbl_mensaje.Text = "PAGO REVERTIDO CON EXITO!!!"
        Else
            If wsRespuesta.codError = 15 Or wsRespuesta.codError = 16 Then
                Session("CDmensaje") = "Revertir_Pago: 'Cliente debe logearse nuevamente' : " & wsRespuesta.mensaje.ToString
                salir(Session("CDmensaje"))
            Else
                Session("CDmensaje") = wsRespuesta.mensaje.ToString
            End If
        End If
    End Sub

    Protected Sub gvBuscarItemsDeCuenta_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvBuscarItemsDeCuenta.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("onmouseover", "this.originalcolor=this.style.backgroundColor;" & " this.style.backgroundColor='PapayaWhip';")
            e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=this.originalcolor;")

            Dim s As String = Convert.ToString(System.Web.UI.DataBinder.Eval(e.Row.DataItem, "formapago"))
            If (s = "NSV") Then
                e.Row.BackColor = Color.Beige
                e.Row.Font.Name = "Roboto Light"

            End If
        End If

    End Sub

    Protected Function CrearBuscarCliente(ByVal _modulo As String, ByVal _criterio As String, ByVal cuenta As String, ByVal CI As String, ByVal codigo_cliente As String) As Data.DataTable
        Dim aux As Data.DataTable
        Try
            Dim cuentas() As String = {cuenta, CI}
            Dim cuentass() As String = {cuenta}

            If _modulo = 7 Then
                aux = Buscar_Cliente(_modulo, _criterio, cuentas, codigo_cliente)
            Else
                aux = Buscar_Cliente(_modulo, _criterio, cuentass, codigo_cliente)
            End If

            Return aux

        Catch ex As Exception
            MostrarMensaje(ex.Message)
            Return Nothing
        End Try
    End Function

    Protected Function Buscar_Cliente(ByVal _modulo As Integer, ByVal _criterio As Integer, ByVal _cuenta() As String, ByVal _codigo_cliente As String) As Data.DataTable
        Try

            Dim td As Data.DataTable = New Data.DataTable()
            Dim row As DataRow

            _idOperativo = Session("v_idOperativo")

            td.Columns.Add("NroOperacion", Type.GetType("System.String"))
            td.Columns.Add("FechaOpe", Type.GetType("System.String"))
            td.Columns.Add("BCmensaje", Type.GetType("System.String"))

            td.Columns.Add("CodCuenta", Type.GetType("System.String"))
            td.Columns.Add("Nombre", Type.GetType("System.String"))
            td.Columns.Add("Detalle", Type.GetType("System.String"))
            td.Columns.Add("CodServicio", Type.GetType("System.String"))
            td.Columns.Add("Servicio", Type.GetType("System.String"))

            wsCuentas = ws1.buscarCliente(_idOperativo, _modulo, True, _criterio, True, _cuenta)
            Dim wsCuenta() As wsSintesis.wsCuenta = wsCuentas.cuenta

            If wsCuentas.codError = 0 Then

                For v As Integer = 0 To (wsCuenta.Length - 1)

                    row = td.NewRow

                    row("NroOperacion") = New ListItem(wsCuentas.nroOperacion)
                    row("FechaOpe") = New ListItem(wsCuentas.fechaOperativa)
                    row("BCmensaje") = New ListItem(wsCuentas.mensaje)

                    row("CodCuenta") = New ListItem(wsCuenta(v).cuenta)
                    row("Nombre") = New ListItem(wsCuenta(v).nombre)
                    row("Detalle") = New ListItem(wsCuenta(v).detalle)
                    row("CodServicio") = New ListItem(wsCuenta(v).servicio)
                    row("Servicio") = New ListItem(wsCuenta(v).descServicio)

                    td.Rows.Add(row)
                Next
                Return td
            ElseIf wsCuentas.codError = 15 Or wsCuentas.codError = 16 Then
                Session("CDmensaje") = "Buscar_Cliente: Cliente debe logearse nuevamente: " & wsCuentas.mensaje
                MostrarMensaje(Session("CDmensaje"))
                Return Nothing
            Else
                Dim _cuenta_1 As String = Nothing
                Dim _cuenta_2 As String = Nothing
                If _cuenta.Length = 1 Then
                    _cuenta_1 = _cuenta(0)
                End If
                If _cuenta.Length > 1 Then
                    _cuenta_1 = _cuenta(0)
                    _cuenta_2 = _cuenta(1)
                End If
                lbl_mensaje.Text &= "<br/> codError=" & wsCuentas.codError & " | mensaje=" & wsCuentas.mensaje & " (" & _codigo_cliente & "|" & _cuenta_1 & "|" & _cuenta_2 & ")"
                'MostrarMensaje("Buscar_Cliente: " & _cuenta(0) & " | " & wsCuentas.mensaje)
                Return Nothing
            End If
        Catch ex As Exception
            MostrarMensaje(ex.Message.ToString)
            Return Nothing
        End Try

    End Function



    Public Sub CreaArchivoRegistrarPago(ByVal CodAgencia As String, ByVal idOperativo As String, ByVal nroOperacion As Integer, ByVal nroOperacionSpecified As Boolean, ByVal fechaOperativa As Integer, ByVal fechaOperativaSpecified As Boolean, ByVal codModulo As Short, ByVal codModuloSpecified As Boolean, ByVal cuenta As String, ByVal servicio As Short, ByVal servicioSpecified As Boolean, ByVal nombreFac As String, ByVal nitFac As String, ByVal direcEnvio As String, ByVal nroitem As String, ByVal monto As String, ByVal estado As Boolean)
        Dim fechaHora As String = Date.Now.Year.ToString & Now.Month.ToString & Now.Day.ToString & "_" & Date.Now.Hour.ToString & Date.Now.Minute.ToString & Date.Now.Second.ToString
        Dim oSW As New StreamWriter(_ruta_archivo & ddlModulo.SelectedItem.Text & "_" & CodAgencia & "_" & fechaHora & "_" & idOperativo & "_" & nroOperacion & "_" & cuenta & "_" & monto.Split("|")(0) & "_" & estado.ToString & ".txt")
        Dim Linea As String = "RealizarPago " & vbNewLine

        Linea &= "idOperativo= " & idOperativo & vbNewLine
        Linea &= "nroOperacion= " & nroOperacion & vbNewLine

        Linea &= "nroOperacionSpecified= " & nroOperacionSpecified & vbNewLine
        Linea &= "fechaOperativa= " & fechaOperativa & vbNewLine
        Linea &= "fechaOperativaSpecified= " & fechaOperativaSpecified & vbNewLine
        Linea &= "codModulo= " & codModulo & vbNewLine
        Linea &= "codModuloSpecified= " & codModuloSpecified & vbNewLine
        Linea &= "cuenta= " & cuenta & vbNewLine
        Linea &= "servicio= " & servicio & vbNewLine
        Linea &= "servicioSpecified= " & servicioSpecified & vbNewLine
        Linea &= "nombreFac= " & nombreFac & vbNewLine
        Linea &= "nitFac= " & nitFac & vbNewLine
        Linea &= "direcEnvio= " & direcEnvio & vbNewLine
        Linea &= "NroItem= " & nroitem & vbNewLine
        Linea &= "Monto= " & monto & vbNewLine
        Linea &= "ESTADO= " & estado.ToString & vbNewLine

        oSW.WriteLine(Linea)
        oSW.Flush()
        oSW.Close()


    End Sub

    'Protected Sub btn_conciliacion_res_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn_conciliacion_res.Click

    '    Dim aux_con_res As Data.DataTable
    '    If Not IsNothing(txt_fecha.Text) And txt_fecha.Text <> "" Then
    '        aux_con_res = conciliacionRes(_idOperativo, txt_fecha.Text, ddlModulo.SelectedValue)

    '        lbl_conciliaRes.Visible = True
    '        gv_concilia_res.Font.Size = size_font_gridview
    '        gv_concilia_res.DataSource = aux_con_res
    '        gv_concilia_res.DataBind()
    '    End If

    'End Sub

    Protected Function conciliacionRes(ByVal vidOperativo As String, ByVal vfechaOperativa As Integer, ByVal vmodulo As Integer) As Data.DataTable
        Try

            Dim td As Data.DataTable = New Data.DataTable()
            Dim row As DataRow

            td.Columns.Add("Servicio", Type.GetType("System.String"))
            td.Columns.Add("Descripcion", Type.GetType("System.String"))
            td.Columns.Add("CantPagos", Type.GetType("System.String"))
            td.Columns.Add("MontoPagos", Type.GetType("System.String"))
            td.Columns.Add("CantRever", Type.GetType("System.String"))
            td.Columns.Add("MontoRever", Type.GetType("System.String"))
            td.Columns.Add("CodMoneda", Type.GetType("System.String"))

            wsConciliaResumen = ws1.obtenerConciliacionResumida(vidOperativo, vfechaOperativa, True, vmodulo, True)
            Dim wsConResumen() As wsSintesis.wsResumen = wsConciliaResumen.resumen

            If wsConciliaResumen.codError = 0 Then

                For v As Integer = 0 To (wsConResumen.Length - 1)

                    row = td.NewRow

                    row("Servicio") = New ListItem(wsConResumen(v).servicio)
                    row("Descripcion") = New ListItem(wsConResumen(v).descripcion)
                    row("CantPagos") = New ListItem(wsConResumen(v).cantPagos)
                    row("MontoPagos") = New ListItem(wsConResumen(v).montoPagos)
                    row("CantRever") = New ListItem(wsConResumen(v).cantRever)
                    row("MontoRever") = New ListItem(wsConResumen(v).montoRever)
                    row("CodMoneda") = New ListItem(wsConResumen(v).moneda)


                    td.Rows.Add(row)
                Next
                Return td
            ElseIf wsConciliaResumen.codError = 15 Or wsConciliaResumen.codError = 16 Then
                MostrarMensaje("conciliacionRes: Cliente debe logearse nuevamente: " & wsConciliaResumen.mensaje)
                Return Nothing
            Else
                MostrarMensaje(wsConciliaResumen.mensaje)
                Return Nothing
            End If
        Catch ex As Exception
            MostrarMensaje(ex.Message.ToString)
            Return Nothing
        End Try

    End Function

    Protected Sub btn_conciliacion_det_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn_conciliacion_det.Click
        Dim aux_con_det As Data.DataTable
        If ddlModulo.SelectedValue = 1 Then

            If Not IsNothing(txt_fecha.Text) And txt_fecha.Text <> "" Then
                aux_con_det = conciliacion_CRE(txt_fecha.Text)

                lbl_conciliaCRE.Visible = True
                btn_reimprimir.Visible = True
                gv_concilia_CRE.Font.Size = size_font_gridview
                gv_concilia_CRE.DataSource = aux_con_det
                gv_concilia_CRE.DataBind()

            End If

        ElseIf ddlModulo.SelectedValue = 2 Then

            If Not IsNothing(txt_fecha.Text) And txt_fecha.Text <> "" Then
                aux_con_det = conciliacion_SAGUAPAC(txt_fecha.Text)

                lbl_conciliaSAGUAPAC.Visible = True
                btn_reimprimir.Visible = True
                gv_concilia_SAGUAPAC.Font.Size = size_font_gridview
                gv_concilia_SAGUAPAC.DataSource = aux_con_det
                gv_concilia_SAGUAPAC.DataBind()

            End If

        Else

            If Not IsNothing(txt_fecha.Text) And txt_fecha.Text <> "" Then
                aux_con_det = conciliacionDet(_idOperativo, txt_fecha.Text, ddlModulo.SelectedValue)

                lbl_conciliaDet.Visible = True
                btn_reimprimir.Visible = True
                gv_concilia_det.Font.Size = size_font_gridview
                gv_concilia_det.DataSource = aux_con_det
                gv_concilia_det.DataBind()

            End If
        End If
    End Sub

    Protected Function conciliacionDet(ByVal vidOperativo As String, ByVal vfechaOperativa As Integer, ByVal vmodulo As Integer) As Data.DataTable
        Try

            Dim td As Data.DataTable = New Data.DataTable()
            Dim row As DataRow

            td.Columns.Add("Servicio", Type.GetType("System.String"))
            td.Columns.Add("Descripcion", Type.GetType("System.String"))
            td.Columns.Add("CodMoneda", Type.GetType("System.String"))
            td.Columns.Add("Usuario", Type.GetType("System.String"))
            td.Columns.Add("CuentaCliente", Type.GetType("System.String"))
            td.Columns.Add("NroOperacion", Type.GetType("System.String"))
            td.Columns.Add("Monto", Type.GetType("System.String"))
            td.Columns.Add("Estado", Type.GetType("System.String"))

            wsConciliaDetalle = ws1.obtenerConciliacionDetallada(vidOperativo, vfechaOperativa, True, vmodulo, True)
            Dim wsConDetalle() As wsSintesis.wsServicioDetalle = wsConciliaDetalle.detalle

            If wsConciliaDetalle.codError = 0 Then

                For v As Integer = 0 To (wsConDetalle.Length - 1)

                    Dim wsOperacion() As wsSintesis.wsOperacion = wsConDetalle(v).operacionItem

                    For a As Integer = 0 To (wsOperacion.Length - 1)
                        row = td.NewRow
                        row("Servicio") = New ListItem(wsConDetalle(v).servicio)
                        row("Descripcion") = New ListItem(wsConDetalle(v).descServicio)
                        row("CodMoneda") = New ListItem(wsConDetalle(v).moneda)

                        row("Usuario") = New ListItem(wsOperacion(a).usuario)
                        row("CuentaCliente") = New ListItem(wsOperacion(a).cuentaCliente)
                        row("NroOperacion") = New ListItem(wsOperacion(a).nroOperacion)
                        row("Monto") = New ListItem(wsOperacion(a).monto)
                        row("Estado") = New ListItem(wsOperacion(a).estado)
                        td.Rows.Add(row)
                    Next
                Next
                Return td
            ElseIf wsConciliaDetalle.codError = 15 Or wsConciliaDetalle.codError = 16 Then
                MostrarMensaje("conciliacionDet: Cliente debe logearse nuevamente: " & wsConciliaDetalle.mensaje)
                Return Nothing
            Else
                MostrarMensaje(wsConciliaDetalle.mensaje)
                Return Nothing
            End If
        Catch ex As Exception
            MostrarMensaje(ex.Message.ToString)
            Return Nothing
        End Try

    End Function

    Protected Function conciliacion_CRE(ByVal vfechaOperativa As Integer) As DataTable
        'Descontruir fecha
        Dim fecha As String = vfechaOperativa.ToString
        anho = fecha.Substring(0, 4)
        mes = fecha.Substring(4, 2)
        dia = fecha.Substring(6, 2)

        'Crear lista para datos
        Dim lista As New List(Of Dictionary(Of String, String))()

        Try
            For Each foundFile As String In My.Computer.FileSystem.GetFiles(HttpContext.Current.Server.MapPath("~/archivos/facturasPrueba/CRE/" & anho & "/" & mes & "/" & dia & "/"))
                lista.Add(extraerDatosCRE(foundFile))
            Next

        Catch ex As Exception When TypeOf ex Is ArgumentException
            MostrarMensaje("conciliacionCREySAGUAPAC: " & ex.Message.ToString)
            Return Nothing

        Catch ex As Exception When TypeOf ex Is ArgumentNullException
            MostrarMensaje("conciliacionCREySAGUAPAC: " & ex.Message.ToString)
            Return Nothing

        Catch ex As Exception When TypeOf ex Is DirectoryNotFoundException
            MostrarMensaje("conciliacionCREySAGUAPAC: " & ex.Message.ToString)
            Return Nothing

        Catch ex As Exception When TypeOf ex Is System.Security.SecurityException
            MostrarMensaje("conciliacionCREySAGUAPAC: " & ex.Message.ToString)
            Return Nothing

        Catch ex As Exception When TypeOf ex Is UnauthorizedAccessException
            MostrarMensaje("conciliacionCREySAGUAPAC: " & ex.Message.ToString)
            Return Nothing

        End Try

        Dim td As Data.DataTable = New Data.DataTable()
        Dim row As DataRow

        td.Columns.Add("Nombre", Type.GetType("System.String"))
        td.Columns.Add("Codigo Fijo", Type.GetType("System.String"))
        td.Columns.Add("Fecha", Type.GetType("System.String"))
        td.Columns.Add("Monto", Type.GetType("System.String"))
        td.Columns.Add("Servicio", Type.GetType("System.String"))

        For Each item As Dictionary(Of String, String) In lista
            row = td.NewRow
            row("Nombre") = New ListItem(item.Item("Nombre"))
            row("Codigo Fijo") = New ListItem(item.Item("Codigo"))
            row("Fecha") = New ListItem(item.Item("Fecha"))
            row("Monto") = New ListItem(item.Item("Monto"))
            row("Servicio") = New ListItem(item.Item("Servicio"))
            td.Rows.Add(row)
        Next

        Return td

    End Function

    Public Function extraerDatosCRE(ByVal path As String) As Dictionary(Of String, String)

        Dim dic As New Dictionary(Of String, String)()

        'Extraer Nombre de Cliente
        Dim textoFactura As String = ExtractTextFromPdf(path)

        Dim cut_at As String = "Nombre Consumidor :"
        Dim x As Integer = InStr(textoFactura, cut_at)

        Dim nombreCliente As String = textoFactura.Substring(0, x - 2)
        Dim string_after As String = textoFactura.Substring(x + cut_at.Length - 1)

        cut_at = "Cuenta"
        x = InStr(string_after, cut_at)

        nombreCliente = string_after.Substring(0, x - 8)

        dic.Add("Nombre", nombreCliente)


        'Extraer Monto

        cut_at = "(A PAGAR):"
        x = InStr(textoFactura, cut_at)

        Dim montoTotal As String = textoFactura.Substring(0, x - 2)
        string_after = textoFactura.Substring(x + cut_at.Length - 1)

        cut_at = "Código"
        x = InStr(string_after, cut_at)

        montoTotal = string_after.Substring(0, x - 2)
        montoTotal = montoTotal.Replace(" ", "")

        dic.Add("Monto", montoTotal)

        'Extraer Codigo Fijo
        Dim cut_at1 As String = "Código Fijo: "
        Dim x1 As Integer = InStr(textoFactura, cut_at1)

        Dim codigoFijo As String = textoFactura.Substring(0, x1 - 2)
        Dim string_after1 As String = textoFactura.Substring(x1 + cut_at1.Length - 1)

        cut_at1 = "Código"
        x1 = InStr(string_after1, cut_at1)
        codigoFijo = string_after1.Substring(0, x1 - 2)

        dic.Add("Codigo", codigoFijo)

        'Extraer fecha
        Dim cut_at2 As String = "Fecha Pago: "
        Dim x2 As Integer = InStr(textoFactura, cut_at2)

        Dim fecha As String = textoFactura.Substring(0, x2 - 2)
        Dim string_after2 As String = textoFactura.Substring(x2 + cut_at2.Length - 1)

        cut_at2 = " "
        x2 = InStr(string_after2, cut_at2)
        fecha = string_after2.Substring(0, x2 - 1)

        dic.Add("Fecha", fecha)

        'Extraer Servicio
        Dim servicio As String = "CRE"
        dic.Add("Servicio", servicio)

        Return dic

    End Function

    Protected Function conciliacion_SAGUAPAC(ByVal vfechaOperativa As Integer) As DataTable
        'Descontruir fecha
        Dim fecha As String = vfechaOperativa.ToString
        anho = fecha.Substring(0, 4)
        mes = fecha.Substring(4, 2)
        dia = fecha.Substring(6, 2)

        'Crear lista para datos
        Dim lista As New List(Of Dictionary(Of String, String))()

        Try
            For Each foundFile As String In My.Computer.FileSystem.GetFiles(HttpContext.Current.Server.MapPath("~/archivos/facturasPrueba/SAGUAPAC/" & anho & "/" & mes & "/" & dia))
                lista.Add(extraerDatosSAGUAPAC(foundFile))
            Next


        Catch ex As Exception When TypeOf ex Is ArgumentException
            MostrarMensaje("conciliacionCREySAGUAPAC: " & ex.Message.ToString)
            Return Nothing

        Catch ex As Exception When TypeOf ex Is ArgumentNullException
            MostrarMensaje("conciliacionCREySAGUAPAC: " & ex.Message.ToString)
            Return Nothing

        Catch ex As Exception When TypeOf ex Is DirectoryNotFoundException
            MostrarMensaje("conciliacionCREySAGUAPAC: " & ex.Message.ToString)
            Return Nothing

        Catch ex As Exception When TypeOf ex Is System.Security.SecurityException
            MostrarMensaje("conciliacionCREySAGUAPAC: " & ex.Message.ToString)
            Return Nothing

        Catch ex As Exception When TypeOf ex Is UnauthorizedAccessException
            MostrarMensaje("conciliacionCREySAGUAPAC: " & ex.Message.ToString)
            Return Nothing

        End Try

        Dim td As Data.DataTable = New Data.DataTable()
        Dim row As DataRow

        td.Columns.Add("Nombre", Type.GetType("System.String"))
        td.Columns.Add("Monto", Type.GetType("System.String"))
        td.Columns.Add("Codigo Asociado", Type.GetType("System.String"))
        td.Columns.Add("Fecha", Type.GetType("System.String"))
        td.Columns.Add("Servicio", Type.GetType("System.String"))

        For Each item As Dictionary(Of String, String) In lista
            row = td.NewRow
            row("Nombre") = New ListItem(item.Item("Nombre"))
            row("Monto") = New ListItem(item.Item("Monto"))
            row("Codigo Asociado") = New ListItem(item.Item("Codigo"))
            row("Fecha") = New ListItem(item.Item("Fecha"))
            row("Servicio") = New ListItem(item.Item("Servicio"))
            td.Rows.Add(row)
        Next

        Return td

    End Function

    Public Function extraerDatosSAGUAPAC(ByVal path As String) As Dictionary(Of String, String)

        Dim dic As New Dictionary(Of String, String)()

        'Extraer Nombre de Cliente
        Dim textoFactura As String = ExtractTextFromPdf(path)

        Dim cut_at As String = "(es): "
        Dim x As Integer = InStr(textoFactura, cut_at)

        Dim nombreCliente As String = textoFactura.Substring(0, x - 2)
        Dim string_after As String = textoFactura.Substring(x + cut_at.Length - 1)

        cut_at = "Dir"
        x = InStr(string_after, cut_at)

        nombreCliente = string_after.Substring(0, x - 2)

        dic.Add("Nombre", nombreCliente)


        'Extraer Monto

        cut_at = "IMPORTE TOTAL:"
        x = InStr(textoFactura, cut_at)

        Dim montoTotal As String = textoFactura.Substring(0, x - 2)
        string_after = textoFactura.Substring(x + cut_at.Length - 1)

        cut_at = "S"
        x = InStr(string_after, cut_at)

        montoTotal = string_after.Substring(0, x - 2)
        montoTotal = montoTotal.Replace(" ", "")

        dic.Add("Monto", montoTotal)

        'Extraer Codigo Asociado
        Dim cut_at1 As String = "Asociado: "
        Dim x1 As Integer = InStr(textoFactura, cut_at1)

        Dim codigoFijo As String = textoFactura.Substring(0, x1 - 2)
        Dim string_after1 As String = textoFactura.Substring(x1 + cut_at1.Length - 1)

        cut_at1 = "M"
        x1 = InStr(string_after1, cut_at1)
        codigoFijo = string_after1.Substring(0, x1 - 2)

        dic.Add("Codigo", codigoFijo)

        'Extraer fecha
        Dim cut_at2 As String = "Fecha de Pago: "
        Dim x2 As Integer = InStr(textoFactura, cut_at2)

        Dim fecha As String = textoFactura.Substring(0, x2 - 1)
        Dim string_after2 As String = textoFactura.Substring(x2 + cut_at2.Length - 1)

        cut_at2 = " "
        x2 = InStr(string_after2, cut_at2)
        fecha = string_after2.Substring(0, x2 - 1)

        dic.Add("Fecha", fecha)

        'Extraer Servicio
        Dim servicio As String = "SAGUAPAC"
        dic.Add("Servicio", servicio)

        Return dic

    End Function
    Protected Function ExtractTextFromPdf(ByVal path As String) As String
        Using reader As PdfReader = New PdfReader(path)
            Dim text As StringBuilder = New StringBuilder()

            For i As Integer = 1 To reader.NumberOfPages
                text.Append(PdfTextExtractor.GetTextFromPage(reader, i))
            Next

            Return text.ToString()
        End Using
    End Function



    Protected Function CargarDatosFacturas_ConciliaDet(ByVal vidOperativo As String, ByVal vfechaOperativa As Integer, ByVal vmodulo As Integer) As Data.DataTable
        Try

            Dim td As Data.DataTable = New Data.DataTable()
            Dim row As DataRow

            td.Columns.Add("Servicio", Type.GetType("System.String"))
            td.Columns.Add("Descripcion", Type.GetType("System.String"))
            td.Columns.Add("CodMoneda", Type.GetType("System.String"))
            td.Columns.Add("Usuario", Type.GetType("System.String"))
            td.Columns.Add("CuentaCliente", Type.GetType("System.String"))
            td.Columns.Add("NroOperacion", Type.GetType("System.String"))
            'td.Columns.Add("Monto", Type.GetType("System.String"))
            'td.Columns.Add("Estado", Type.GetType("System.String"))

            wsConciliaDetalle = ws1.obtenerConciliacionDetallada(vidOperativo, vfechaOperativa, True, vmodulo, True)
            Dim wsConDetalle() As wsSintesis.wsServicioDetalle = wsConciliaDetalle.detalle

            If wsConciliaDetalle.codError = 0 Then

                For v As Integer = 0 To (wsConDetalle.Length - 1)

                    Dim wsOperacion() As wsSintesis.wsOperacion = wsConDetalle(v).operacionItem

                    For a As Integer = 0 To (wsOperacion.Length - 1)
                        If wsOperacion(a).estado = "P" Then
                            row = td.NewRow
                            row("Servicio") = New ListItem(wsConDetalle(v).servicio)
                            row("Descripcion") = New ListItem(wsConDetalle(v).descServicio)
                            row("CodMoneda") = New ListItem(wsConDetalle(v).moneda)

                            row("Usuario") = New ListItem(wsOperacion(a).usuario)
                            row("CuentaCliente") = New ListItem(wsOperacion(a).cuentaCliente)
                            row("NroOperacion") = New ListItem(wsOperacion(a).nroOperacion)
                            'row("Monto") = New ListItem(wsOperacion(a).monto)
                            'row("Estado") = New ListItem(wsOperacion(a).estado)
                            td.Rows.Add(row)
                        End If

                    Next
                Next
                Dim vec_col(5) As String
                vec_col(0) = "Servicio"
                vec_col(1) = "Descripcion"
                vec_col(2) = "CodMoneda"
                vec_col(3) = "Usuario"
                vec_col(4) = "CuentaCliente"
                vec_col(5) = "NroOperacion"


                Dim MyView As DataView = New DataView(td)
                Dim dtSinDuplicados As DataTable
                dtSinDuplicados = MyView.ToTable(True, vec_col)

                Dim td_fac As Data.DataTable = New Data.DataTable()
                Dim row_fac As DataRow

                td_fac.Columns.Add("Nit", Type.GetType("System.String"))
                td_fac.Columns.Add("NroFactura", Type.GetType("System.String"))
                td_fac.Columns.Add("NroAutorizacion", Type.GetType("System.String"))
                td_fac.Columns.Add("FecEmision", Type.GetType("System.String"))
                td_fac.Columns.Add("Monto", Type.GetType("System.String"))
                td_fac.Columns.Add("MontoFiscal", Type.GetType("System.String"))
                td_fac.Columns.Add("CodControl", Type.GetType("System.String"))
                td_fac.Columns.Add("NroCarnet", Type.GetType("System.String"))

                For Each dr As Data.DataRow In dtSinDuplicados.Rows
                    wsImpresion = ws1.obtenerImpresion(_idOperativo, dr("NroOperacion"), True, txt_fecha.Text, True, ddlModulo.SelectedValue, True, dr("Servicio"), True, dr("CuentaCliente"), "M")
                    If wsImpresion.codError = 0 Then
                        Dim Linea As String = ""
                        Dim datos() As String = Nothing
                        For x As Integer = 0 To wsImpresion.lineaFactura.Length - 1
                            Linea = wsImpresion.lineaFactura(x)

                            If Linea.StartsWith("<QR_ENT_G>") Then
                                row_fac = td_fac.NewRow

                                Linea = Linea.Replace("<QR_ENT_G>", "")
                                datos = Linea.Split("|")

                                row_fac("Nit") = New ListItem(datos(0))
                                row_fac("NroFactura") = New ListItem(datos(1))
                                row_fac("NroAutorizacion") = New ListItem(datos(2))
                                row_fac("FecEmision") = New ListItem(datos(3))
                                row_fac("Monto") = New ListItem(datos(4))
                                row_fac("MontoFiscal") = New ListItem(datos(5))
                                row_fac("CodControl") = New ListItem(datos(6))
                                row_fac("NroCarnet") = New ListItem(datos(7))

                                td_fac.Rows.Add(row_fac)
                            End If
                        Next
                    ElseIf wsImpresion.codError = 15 Or wsImpresion.codError = 16 Then
                        MostrarMensaje("CargarDatosFacturas_ConciliaDet: Cliente debe logearse nuevamente: " & wsImpresion.mensaje)
                        Return Nothing
                    Else
                        MostrarMensaje(wsImpresion.mensaje)
                        Return Nothing
                    End If
                Next

                'Return td
                'Return dtSinDuplicados
                Return td_fac
            ElseIf wsConciliaDetalle.codError = 15 Or wsConciliaDetalle.codError = 16 Then
                MostrarMensaje("CargarDatosFacturas_ConciliaDet: Cliente debe logearse nuevamente: " & wsConciliaDetalle.mensaje)
                Return Nothing
            Else
                MostrarMensaje(wsConciliaDetalle.mensaje)
                Return Nothing
            End If
        Catch ex As Exception
            MostrarMensaje(ex.Message.ToString)
            Return Nothing
        End Try

    End Function



    Protected Sub gv_concilia_det_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gv_concilia_det.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("onmouseover", "this.originalcolor=this.style.backgroundColor;" & " this.style.backgroundColor='PapayaWhip';")
            e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=this.originalcolor;")

            Dim s As String = Convert.ToString(System.Web.UI.DataBinder.Eval(e.Row.DataItem, "Estado"))
            If (s = "P") Then
                e.Row.BackColor = Color.Beige

            End If
        End If
    End Sub


    'Protected Sub btn_ObtOperacionesPag_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn_ObtOperacionesPag.Click
    '    Dim aux_con_det As Data.DataTable
    '    If Not IsNothing(txt_fecha.Text) And txt_fecha.Text <> "" Then
    '        aux_con_det = ObtOperacionesPagadas(_idOperativo, ddlModulo.SelectedValue, "101498894", "1")

    '        gv_facturas.Font.Size = size_font_gridview
    '        gv_facturas.DataSource = aux_con_det
    '        gv_facturas.DataBind()

    '    End If
    'End Sub

    Protected Function ObtOperacionesPagadas(ByVal vidOperativo As String, ByVal vmodulo As Integer, ByVal vCuenta As String, ByVal vServicio As String) As Data.DataTable
        Try

            Dim td As Data.DataTable = New Data.DataTable()
            Dim row As DataRow

            td.Columns.Add("Fecha", Type.GetType("System.String"))
            td.Columns.Add("NroOperacion", Type.GetType("System.String"))
            td.Columns.Add("Monto", Type.GetType("System.String"))
            td.Columns.Add("Moneda", Type.GetType("System.String"))


            wsOperPagadas = ws1.obtenerOperacionesPagadas(vidOperativo, vmodulo, True, vCuenta, vServicio, True)
            Dim wsOperPagada() As wsSintesis.wsOperPagada = wsOperPagadas.operPagada

            If wsOperPagadas.codError = 0 Then

                For v As Integer = 0 To (wsOperPagada.Length - 1)

                    row = td.NewRow
                    row("Fecha") = New ListItem(wsOperPagada(v).fecha)
                    row("NroOperacion") = New ListItem(wsOperPagada(v).nroOperacion)
                    row("Monto") = New ListItem(wsOperPagada(v).monto)
                    row("Moneda") = New ListItem(wsOperPagada(v).moneda)

                    td.Rows.Add(row)

                Next
                Return td
            ElseIf wsOperPagadas.codError = 15 Or wsOperPagadas.codError = 16 Then
                MostrarMensaje("conciliacionDet: Cliente debe logearse nuevamente: " & wsOperPagadas.mensaje)
                Return Nothing
            Else
                MostrarMensaje(wsOperPagadas.mensaje)
                Return Nothing
            End If
        Catch ex As Exception
            MostrarMensaje(ex.Message.ToString)
            Return Nothing
        End Try

    End Function
    Public Shared pd As New System.Drawing.Printing.PrintDocument
    Public Shared PrintFont As Font
    Public Shared FiletoPrint As System.IO.StreamReader

    Public Shared Sub printPage(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs)
        Dim linesPerPage As Single = 0
        Dim yPos As Single = 0
        Dim count As Integer = 0
        Dim leftMargin As Single = 0
        Dim topmargin As Single = 0
        Dim line As String = Nothing

        Try

            e.PageSettings.Landscape = False
            linesPerPage = e.MarginBounds.Height / PrintFont.GetHeight(e.Graphics)

            While count < linesPerPage
                line = FiletoPrint.ReadLine

                If line Is Nothing Then Exit While

                yPos = topmargin + count * PrintFont.GetHeight(e.Graphics)
                e.Graphics.DrawString(line, PrintFont, Brushes.Black, leftMargin, yPos, New StringFormat)
                count += 1
            End While

            If Not (line Is Nothing) Then e.HasMorePages = True

        Catch ex As Exception

        End Try
    End Sub




    'Public Function PrintPDF(inFile As String) As Boolean
    '    ' Declare Variables
    '    Dim PDFProcess As New Process()

    '    ' Print the passed PDF filename
    '    PDFProcess.StartInfo.FileName = "AcroRd32.exe"
    '    PDFProcess.StartInfo.Arguments = "/p /h " & inFile
    '    PDFProcess.StartInfo.UseShellExecute = True
    '    PDFProcess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized
    '    PDFProcess.Start()
    '    PDFProcess.Close()

    '    ' Wait 4 seconds and kill Adobe Reader
    '    ' This is needed to allow Adobe time to send output to the printer
    '    Sleep(4000)
    '    For Each item As Process In Process.GetProcesses
    '        If item.ProcessName = "AcroRd32" Then
    '            item.Kill()
    '        End If
    '    Next

    '    Return True

    'End Function

    Protected Sub btn_reimprimir_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn_reimprimir.Click
        Dim con As Integer = 0
        Try
            For i As Integer = 0 To gv_concilia_det.Rows.Count - 1
                If CType(gv_concilia_det.Rows(i).FindControl("ckb_imprimir"), CheckBox).Checked = True Then
                    CrearArchivoFactura(txtCodigoAgencia.Text, _idOperativo, gv_concilia_det.Rows(i).Cells.Item(6).Text, txt_fecha.Text, ddlModulo.SelectedValue, gv_concilia_det.Rows(i).Cells.Item(1).Text, gv_concilia_det.Rows(i).Cells.Item(5).Text, gv_concilia_det.Rows(i).Cells.Item(7).Text, "M")
                    con = con + 1
                    PrintFont = New Font("Arial", 10)
                    FiletoPrint = New System.IO.StreamReader(nombreArchivo)
                    AddHandler pd.PrintPage, New System.Drawing.Printing.PrintPageEventHandler(AddressOf printPage)
                    pd.Print()
                End If
            Next
            If con > 0 Then
                lbl_mensaje.Text = "ARCHIVO(S) CREADO(S) CON EXITO..."
            End If
        Catch ex As Exception
            MostrarMensaje("REIMPRIMIR: " & ex.Message)
        End Try


    End Sub

    Protected Sub crear_ascii_facturas(ByVal CodAgencia As String, ByVal fecha As String)
        Dim con As Integer = 0
        Try
            Dim fechaHora As String = Date.Now.Year.ToString & Now.Month.ToString & Now.Day.ToString & "_" & Date.Now.Hour.ToString & Date.Now.Minute.ToString & Date.Now.Second.ToString
            Dim oSW As New StreamWriter(_ruta_archivo & ddlModulo.SelectedItem.Text & "_" & CodAgencia & "_" & fecha & ".txt")
            Dim Linea As String = ""
            For i As Integer = 0 To gv_concilia_det_deb.Rows.Count - 1
                Linea &= gv_concilia_det_deb.Rows(i).Cells.Item(0).Text & "|"
                Linea &= gv_concilia_det_deb.Rows(i).Cells.Item(1).Text & "|"

                Dim a As Integer = InStr(gv_concilia_det_deb.Rows(i).Cells.Item(8).Text, "|")
                If a > 0 Then
                    Linea &= gv_concilia_det_deb.Rows(i).Cells.Item(8).Text
                End If

                Linea &= vbNewLine
                con = con + 1
            Next
            If con > 0 Then
                lbl_mensaje.Text = "ARCHIVO(S) CREADO(S) CON EXITO..."
            End If

            oSW.WriteLine(Linea)
            oSW.Flush()
            oSW.Close()
        Catch ex As Exception
            MostrarMensaje("crear_ascii_facturas: " & ex.Message)
        End Try

    End Sub

    Protected Sub btn_ascii_facturas_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn_ascii_facturas.Click
        crear_ascii_facturas(txtCodigoAgencia.Text, txt_fecha.Text)
    End Sub

    Protected Sub VerImpresion()
        Dim p As String
        p = ddlModulo.SelectedValue.ToString & "|" & txtCodigoRecaudacion.Text & "|" & txtCodigoAgencia.Text & "|" & txtCodigoCliente.Text & "|" & txt_fecha.Text & "|" & ddlModulo.SelectedItem.Text

        Dim popupScript As String = "<script language='JavaScript'>" & _
                                    "popUpReport('VerImpresion.aspx', '" & p & "');" & _
                                    "</script>"

        ClientScript.RegisterStartupScript(Me.GetType, "PopupScript", popupScript)
    End Sub


End Class
