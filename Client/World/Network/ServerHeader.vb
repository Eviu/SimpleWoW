Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Client.World

Namespace World.Network
    Public Class ServerHeader
        Implements Header
        Public Property Command() As WorldCommand Implements Header.Command
            Get
                Return m_Command
            End Get
            Private Set(value As WorldCommand)
                m_Command = value
            End Set
        End Property
        Private m_Command As WorldCommand
        Public Property Size() As Integer Implements Header.Size
            Get
                Return m_Size
            End Get
            Private Set(value As Integer)
                m_Size = value
            End Set
        End Property
        Private m_Size As Integer
        Public Property InputDataLength() As Integer
            Get
                Return m_InputDataLength
            End Get
            Private Set(value As Integer)
                m_InputDataLength = value
            End Set
        End Property
        Private m_InputDataLength As Integer

        Friend Sub New(data As Byte())
            InputDataLength = data.Length
            If InputDataLength = 4 Then
                Size = CInt(CUInt(data(0)) << 8 Or data(1))
                Command = CType(BitConverter.ToUInt16(data, 2), WorldCommand)
            ElseIf InputDataLength = 5 Then
                Size = CInt(((CUInt(data(0)) And &H7F) << 16) Or (CUInt(data(1)) << 8) Or data(2))
                Command = CType(BitConverter.ToUInt16(data, 3), WorldCommand)
            Else
                Return
            End If

            ' decrement since we already have command's two bytes
            Size -= 2
        End Sub

        Public Overrides Function ToString() As String
            Return [String].Format("Command {0} Header Size {1} Packet Size {2}", Command, InputDataLength, Size)
        End Function
    End Class
End Namespace
