Imports System.IO

Namespace World.Network
    Public Class OutPacket
        Inherits BinaryWriter
        Implements Packet
        Public Property Header() As Header Implements Packet.Header
            Get
                Return m_Header
            End Get
            Private Set(value As Header)
                m_Header = Value
            End Set
        End Property
        Private m_Header As Header

        Private ReadOnly Buffer As MemoryStream
        Private FinalizedPacket As Byte()

        Public Sub New(command As WorldCommand, Optional emptyOffset As Integer = 0)
            MyBase.New()
            Me.Header = New ClientHeader(command, Me)

            Buffer = New MemoryStream()
            MyBase.OutStream = Buffer

            If emptyOffset > 0 Then
                Write(New Byte(emptyOffset - 1) {})
            End If
        End Sub

        Public Overloads Function Finalize() As Byte()
            If FinalizedPacket Is Nothing Then
                Dim data As Byte() = New Byte(Convert.ToInt32(6 + (Buffer.Length - 1))) {}
                Dim size As Byte() = DirectCast(Header, ClientHeader).EncryptedSize()
                Dim command As Byte() = DirectCast(Header, ClientHeader).EncryptedCommand()

                Array.Copy(size, 0, data, 0, 2)
                Array.Copy(command, 0, data, 2, 4)
                Array.Copy(Buffer.ToArray(), 0, data, 6, Buffer.Length)

                FinalizedPacket = data
            End If

            Return FinalizedPacket
        End Function
    End Class
End Namespace
