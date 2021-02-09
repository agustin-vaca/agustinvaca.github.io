
Partial Class login
    Inherits System.Web.UI.Page

    Protected Sub ok_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ok.Click
        Dim ws As New wsOpenBank_ACCESSv2.ACCESS

        If ws.Banca_AutenticarUsuarioOB(UsernameTextBox.Text.ToUpper, PasswordTextBox.Text.ToUpper) Then
            Session("Usuario") = UsernameTextBox.Text.ToUpper.Trim
            Session("fecha") = DateTime.Now.ToString()

            Response.Redirect("index.aspx")
        Else

            mensaje.Visible = True
            mensaje.Text = "Acceso denegado"

            PasswordTextBox.Text = ""
            PasswordTextBox.Focus()
        End If

        'Response.Redirect("index.aspx")

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        UsernameTextBox.Focus()
        Session.Clear()
        If Request("m") <> "" Then
            mensaje.Text = "''" & Request("m") & "''"
        End If

    End Sub
End Class
