Imports System.Collections.Generic
Imports System.IO
Imports System.Numerics
Imports System.Reflection
Imports System.Text

Public Module Extensions
    Sub New()
    End Sub
    <System.Runtime.CompilerServices.Extension> _
    Public Function ToHexString(array As Byte()) As String
        Dim builder As New StringBuilder()

        For i As Integer = array.Length - 1 To 0 Step -1
            builder.Append(array(i).ToString("X2"))
        Next

        Return builder.ToString()
    End Function

    ''' <summary>
    ''' places a non-negative value (0) at the MSB, then converts to a BigInteger.
    ''' This ensures a non-negative value without changing the binary representation.
    ''' </summary>
    <System.Runtime.CompilerServices.Extension> _
    Public Function ToBigInteger(array__1 As Byte()) As BigInteger
        Dim temp As Byte()
        If (array__1(array__1.Length - 1) And &H80) = &H80 Then
            temp = New Byte(array__1.Length) {}
            temp(array__1.Length) = 0
        Else
            temp = New Byte(array__1.Length - 1) {}
        End If

        Array.Copy(array__1, temp, array__1.Length)
        Return New BigInteger(temp)
    End Function

    ''' <summary>
    ''' Removes the MSB if it is 0, then converts to a byte array.
    ''' </summary>
    <System.Runtime.CompilerServices.Extension> _
    Public Function ToCleanByteArray(b As BigInteger) As Byte()
        Dim array__1 As Byte() = b.ToByteArray()
        If array__1(array__1.Length - 1) <> 0 Then
            Return array__1
        End If

        Dim temp As Byte() = New Byte(array__1.Length - 2) {}
        Array.Copy(array__1, temp, temp.Length)
        Return temp
    End Function

    <System.Runtime.CompilerServices.Extension> _
    Public Function ModPow(value As BigInteger, pow As BigInteger, [mod] As BigInteger) As BigInteger
        Return BigInteger.ModPow(value, pow, [mod])
    End Function

    <System.Runtime.CompilerServices.Extension> _
    Public Function ReadCString(reader As BinaryReader) As String
        Dim builder As New StringBuilder()

        While True
            Dim letter As Byte = reader.ReadByte()
            If letter = 0 Then
                Exit While
            End If

            builder.Append(ChrW(letter))
        End While

        Return builder.ToString()
    End Function

    <System.Runtime.CompilerServices.Extension> _
    Public Function SubArray(array__1 As Byte(), start As Integer, count As Integer) As Byte()
        Dim subArray__2 As Byte() = New Byte(count - 1) {}
        Array.Copy(array__1, start, subArray__2, 0, count)
        Return subArray__2
    End Function

    <System.Runtime.CompilerServices.Extension> _
    Public Function ToCString(str As String) As Byte()
        Dim data As Byte() = New Byte(str.Length) {}
        Array.Copy(Encoding.ASCII.GetBytes(str), data, str.Length)
        data(data.Length - 1) = 0
        Return data
    End Function

    <System.Runtime.CompilerServices.Extension> _
    Public Function GetAttributes(Of T As Attribute)(member As MemberInfo, inherit As Boolean) As IEnumerable(Of T)
        Return If(DirectCast(member.GetCustomAttributes(GetType(T), inherit), T()), New T() {})
    End Function

    <System.Runtime.CompilerServices.Extension> _
    Public Function TryGetAttributes(Of T As Attribute)(member As MemberInfo, inherit As Boolean, ByRef attributes As IEnumerable(Of T)) As Boolean
        Dim attrs = If(DirectCast(member.GetCustomAttributes(GetType(T), inherit), T()), New T() {})
        attributes = attrs
        Return attrs.Length > 0
    End Function
End Module
