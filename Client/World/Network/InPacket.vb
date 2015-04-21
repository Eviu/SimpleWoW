Imports System.IO

Namespace World.Network
    Public Class InPacket
        Inherits BinaryReader
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

        Friend Sub New(header As ServerHeader)

            Me.New(header, New Byte() {})
        End Sub

        Friend Sub New(header__1 As ServerHeader, buffer As Byte())
            MyBase.New(New MemoryStream(buffer))
            Header = header__1
        End Sub

        Public Function ReadPackedGuid() As ULong
            Dim mask As Byte = ReadByte()

            If mask = 0 Then
                Return CULng(0)
            End If

            Dim res As ULong = 0

            Dim i As Integer = 0
            While i < 8
                If (mask And 1 << i) <> 0 Then
                    res += CULng(ReadByte()) << (i * 8)
                End If

                i += 1
            End While

            Return res
        End Function
    End Class
End Namespace
