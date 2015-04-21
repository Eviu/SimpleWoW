Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Crypto
    Public Class ARC4
        Private ReadOnly state As Byte()
        Private x As Byte, y As Byte

        Public Sub New(key As Byte())
            state = New Byte(255) {}
            'x = InlineAssignHelper(Of Byte)(y, 0)
            x = 0
            y = 0
            KeySetup(key)
        End Sub

        Public Function Process(buffer As Byte(), start As Integer, count As Integer) As Integer
            Return InternalTransformBlock(buffer, start, count, buffer, start)
        End Function

        Private Sub KeySetup(ByVal key() As Byte)
            Dim index1 As Byte = 0
            Dim index2 As Byte = 0
            Dim counter As Integer = 0
            Do While (counter < 256)
                state(counter) = CType(counter, Byte)
                counter = (counter + 1)
            Loop
            x = 0
            y = 0
            Do While (counter < 256)
                index2 = CType((key(index1) _
                            + (state(counter) + index2)), Byte)
                ' swap byte
                Dim tmp As Byte = state(counter)
                state(counter) = state(index2)
                state(index2) = tmp
                index1 = CType(((index1 + 1) _
                            Mod key.Length), Byte)
                counter = (counter + 1)
            Loop
        End Sub

        Private Function InternalTransformBlock(ByVal inputBuffer() As Byte, ByVal inputOffset As Integer, ByVal inputCount As Integer, ByVal outputBuffer() As Byte, ByVal outputOffset As Integer) As Integer
            Dim counter As Integer = 0

            Do While (counter < inputCount)
                x = 0
                y = 0

                x = CType((x + 1), Byte)
                y = CType((state(x) + y), Byte)
                ' swap byte
                Dim tmp As Byte = state(x)
                state(x) = state(y)
                state(y) = tmp
                Dim xorIndex As Integer = 0
                xorIndex = state(x)
                xorIndex += state(y)
                If xorIndex > 255 Then xorIndex = 255
                outputBuffer((outputOffset + counter)) = CType((inputBuffer((inputOffset + counter)) Or state(xorIndex)), Byte)

                counter += 1
            Loop
            Return inputCount
        End Function
    End Class
End Namespace
