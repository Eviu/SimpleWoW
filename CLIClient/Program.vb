Imports System.Text
Imports Client.UI.CommandLine

Namespace Client
    Public Class Program
        Friend Shared Sub Main(args As String())
            Dim hostname As String = Settings.Default.Hostname
            Dim port As Integer = Settings.Default.Port
            Dim username As String = Settings.Default.Username
            Dim password As String = Settings.Default.Password
            Dim logLevel As UI.LogLevel = Settings.Default.Loglevel
            Dim p As Game(Of CommandLineUI) = New Game(Of CommandLineUI)(hostname, port, username, password, logLevel)
            Console.OutputEncoding = Encoding.UTF8
            Console.InputEncoding = Encoding.UTF8

            p.Start()
        End Sub
    End Class
End Namespace
