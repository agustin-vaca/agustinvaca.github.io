
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.FileIO.FileSystem
Imports iTextSharp.text.pdf
Imports iTextSharp.text.pdf.parser

Partial Class cargar
    Inherits System.Web.UI.Page

    'Variables 
    Dim fecha As String
    Dim dia As String
    Dim mes As String
    Dim anho As String
    Dim nombreCompleto As String
    Dim filename As String
    Dim servicio As String

    Private Sub UploadButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles UploadButton.Click
        If FileUploadControl.HasFile Then
            Try
                If FileUploadControl.PostedFile.ContentType = "application/pdf" Then

                    filename = IO.Path.GetFileName(FileUploadControl.FileName)
                    FileUploadControl.SaveAs(HttpContext.Current.Server.MapPath("~/archivos/facturaPruebaTemp/") & filename)
                    StatusLabel.Text = "Estado: Archivo subido con exito"
                    renombrar(filename)
                Else
                    StatusLabel.Text = "Solo archivos PDF son aceptados"
                End If

            Catch ex As Exception
                StatusLabel.Text = "Upload status: The file could not be uploaded. The following error occured: " & ex.Message
            End Try
        End If

    End Sub

    Public Function ExtractTextFromPdf(ByVal path As String) As String
        Using reader As PdfReader = New PdfReader(path)
            Dim text As StringBuilder = New StringBuilder()

            For i As Integer = 1 To reader.NumberOfPages
                text.Append(PdfTextExtractor.GetTextFromPage(reader, i))
            Next

            Return text.ToString()
        End Using
    End Function

    Public Sub extraerNombre(ByVal path As String)


        'Extraer Nombre de Cliente
        Dim mystr As String = ExtractTextFromPdf(path)

        If mystr.Length = 0 Then
            nombreCompleto = ""
            Exit Sub
        End If

        Dim cut_at As String = "Nombre Consumidor :"
        Dim x As Integer = InStr(mystr, cut_at)

        Dim nombreCliente As String = mystr.Substring(0, x - 2)
        Dim string_after As String = mystr.Substring(x + cut_at.Length - 1)

        cut_at = "Cuenta"
        x = InStr(string_after, cut_at)

        nombreCliente = string_after.Substring(0, x - 8)


        'Extraer Codigo Fijo
        Dim cut_at1 As String = "Código Fijo: "
        Dim x1 As Integer = InStr(mystr, cut_at1)

        Dim codigoFijo As String = mystr.Substring(0, x1 - 2)
        Dim string_after1 As String = mystr.Substring(x1 + cut_at1.Length - 1)

        cut_at1 = "Código"
        x1 = InStr(string_after1, cut_at1)
        codigoFijo = string_after1.Substring(0, x1 - 2)
        codigoFijo = codigoFijo.Replace("/", "_")

        nombreCompleto = nombreCliente + "_" + codigoFijo + ".pdf"


        'Extraer fecha
        Dim cut_at2 As String = "Fecha Pago: "
        Dim x2 As Integer = InStr(mystr, cut_at2)

        fecha = mystr.Substring(0, x2 - 2)
        Dim string_after2 As String = mystr.Substring(x2 + cut_at2.Length - 1)

        cut_at2 = " "
        x2 = InStr(string_after2, cut_at2)
        fecha = string_after2.Substring(0, x2 - 1)

        Dim strarr As String() = fecha.Split("/"c)
        dia = strarr(0)
        mes = strarr(1)
        anho = strarr(2)

    End Sub

    Public Sub extraerNombreSAGUAPAC(ByVal path As String)


        'Extraer Nombre de Cliente
        Dim textoFactura As String = ExtractTextFromPdf(path)

        If textoFactura.Length = 0 Then
            nombreCompleto = ""
            Exit Sub
        End If

        Dim cut_at As String = "(es): "
        Dim x As Integer = InStr(textoFactura, cut_at)

        Dim nombreCliente As String = textoFactura.Substring(0, x - 2)
        Dim string_after As String = textoFactura.Substring(x + cut_at.Length - 1)

        cut_at = "Dir"
        x = InStr(string_after, cut_at)

        nombreCliente = string_after.Substring(0, x - 2)


        'Extraer numero factura
        Dim cut_at1 As String = "Factura: "
        Dim x1 As Integer = InStr(textoFactura, cut_at1)

        Dim codigoAsociado As String = textoFactura.Substring(0, x1 - 2)
        Dim string_after1 As String = textoFactura.Substring(x1 + cut_at1.Length - 1)

        cut_at1 = "N"
        x1 = InStr(string_after1, cut_at1)
        codigoAsociado = string_after1.Substring(0, x1 - 2)

        nombreCompleto = nombreCliente + "_" + codigoAsociado + ".pdf"


        'Extraer fecha
        Dim cut_at2 As String = "Fecha de Pago: "
        Dim x2 As Integer = InStr(textoFactura, cut_at2)

        fecha = textoFactura.Substring(0, x2 - 2)
        Dim string_after2 As String = textoFactura.Substring(x2 + cut_at2.Length - 1)

        cut_at2 = " "
        x2 = InStr(string_after2, cut_at2)
        fecha = string_after2.Substring(0, x2 - 1)

        Dim strarr As String() = fecha.Split("/"c)
        dia = strarr(0)
        mes = strarr(1)
        anho = strarr(2)

    End Sub

    Public Sub renombrar(ByVal file As String)
        partirPDF()

        Dim firstletter As Array = ExtractTextFromPdf(HttpContext.Current.Server.MapPath("~/archivos/facturaPruebaTemp/" & file)).ToCharArray()
        If firstletter(1) = "C" Then
            For Each foundFile As String In My.Computer.FileSystem.GetFiles(HttpContext.Current.Server.MapPath("~/archivos/facturaPruebaTemp/temp"))
                extraerNombre(foundFile)
                If nombreCompleto = "" Then
                    DeleteFile(foundFile)
                    Continue For
                End If
                My.Computer.FileSystem.MoveFile(foundFile, HttpContext.Current.Server.MapPath("~/archivos/facturasPrueba/CRE/") & anho & "\" & mes & "\" & dia & "\" & nombreCompleto, FileIO.UIOption.AllDialogs, FileIO.UICancelOption.ThrowException)
            Next
        Else
            For Each foundFile As String In My.Computer.FileSystem.GetFiles(HttpContext.Current.Server.MapPath("~/archivos/facturaPruebaTemp/temp"))
                extraerNombreSAGUAPAC(foundFile)
                If nombreCompleto = "" Then
                    DeleteFile(foundFile)
                    Continue For
                End If
                My.Computer.FileSystem.MoveFile(foundFile, HttpContext.Current.Server.MapPath("~/archivos/facturasPrueba/SAGUAPAC/") & anho & "\" & mes & "\" & dia & "\" & nombreCompleto, FileIO.UIOption.AllDialogs, FileIO.UICancelOption.ThrowException)
            Next
        End If
    End Sub

    Private Sub SplitAndSaveInterval(ByVal pdfFilePath As String, ByVal outputPath As String, ByVal startPage As Integer, ByVal interval As Integer, ByVal pdfFileName As String)
        Using reader As PdfReader = New PdfReader(pdfFilePath)
            Dim document As iTextSharp.text.Document = New iTextSharp.text.Document()
            Dim copy As PdfCopy = New PdfCopy(document, New FileStream(outputPath & "\" & pdfFileName & ".pdf", FileMode.Create))
            document.Open()

            For pagenumber As Integer = startPage To (startPage + interval) - 1

                If reader.NumberOfPages >= pagenumber Then
                    copy.AddPage(copy.GetImportedPage(reader, pagenumber))
                Else
                    Exit For
                End If
            Next

            document.Close()
        End Using
    End Sub

    Private Sub partirPDF()

        Dim pdfFilePath As String = HttpContext.Current.Server.MapPath("~/archivos/facturaPruebaTemp/") & filename
        Dim outputPath As String = HttpContext.Current.Server.MapPath("~/archivos/facturaPruebaTemp/temp")
        Dim interval As Integer = 1
        Dim pageNameSuffix As Integer = 0
        Dim reader As PdfReader = New PdfReader(pdfFilePath)
        Dim file As FileInfo = New FileInfo(pdfFilePath)
        Dim pdfFileName As String = file.Name.Substring(0, file.Name.LastIndexOf(".")) & "-"
        Dim pageNumber As Integer = 1

        While pageNumber <= reader.NumberOfPages
            pageNameSuffix += 1
            Dim newPdfFileName As String = String.Format(pdfFileName & "{0}", pageNameSuffix)
            SplitAndSaveInterval(pdfFilePath, outputPath, pageNumber, interval, newPdfFileName)
            pageNumber += interval
        End While
    End Sub

End Class
