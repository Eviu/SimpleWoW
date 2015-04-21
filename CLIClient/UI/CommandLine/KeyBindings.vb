Imports System.Collections.Generic
Imports System.Reflection
Imports Client
Public Delegate Sub KeyBind()

<AttributeUsage(AttributeTargets.Method, AllowMultiple:=True, Inherited:=False)> _
Public NotInheritable Class KeyBindAttribute
    Inherits Attribute
    Public Key As ConsoleKey

    Public Sub New(key__1 As ConsoleKey)
        Key = key__1
    End Sub
End Class

Partial Public Class CommandLineUI
    Private _keyPressHandlers As Dictionary(Of ConsoleKey, KeyBind)

    Private Sub InitializeKeybinds()
        _keyPressHandlers = New Dictionary(Of ConsoleKey, KeyBind)()

        Const flags As BindingFlags = BindingFlags.[Public] Or BindingFlags.NonPublic Or BindingFlags.Instance
        Dim attributes As IEnumerable(Of KeyBindAttribute) = Nothing
        For Each method As MethodInfo In Me.[GetType]().GetMethods(flags)
            If Not method.TryGetAttributes(Of KeyBindAttribute)(False, attributes) Then
                Continue For
            End If

            Dim handler As KeyBind = DirectCast(KeyBind.CreateDelegate(GetType(KeyBind), Me, method), KeyBind)

            For Each Attribute As KeyBindAttribute In attributes
                _keyPressHandlers(Attribute.Key) = handler
            Next
        Next
    End Sub
End Class
