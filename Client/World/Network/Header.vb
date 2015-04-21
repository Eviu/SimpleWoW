
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace World.Network
    Public Interface Header
        Property Command() As WorldCommand
        Property Size() As Integer
    End Interface
End Namespace