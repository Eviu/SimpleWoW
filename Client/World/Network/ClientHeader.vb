Imports Client.World
Imports System.Linq
Namespace World.Network
    Class ClientHeader
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
        Private m_encryptedCommand As Byte()
        Public ReadOnly Property EncryptedCommand() As Byte()
            Get
                If m_encryptedCommand Is Nothing Then
                    m_encryptedCommand = BitConverter.GetBytes(CUInt(Me.Command))
                    AuthenticationCrypto.Encrypt(m_encryptedCommand, 0, m_encryptedCommand.Length)
                End If

                Return m_encryptedCommand
            End Get
        End Property

        Public Property Size() As Integer Implements Header.Size
            Get
                Return CInt(Packet.BaseStream.Length) + 4
            End Get
            Set(value As Integer)
                value = CInt(Packet.BaseStream.Length) + 4
            End Set
        End Property

        Private m_encryptedSize As Byte()
        Public ReadOnly Property EncryptedSize() As Byte()
            Get
                If m_encryptedSize Is Nothing Then
                    m_encryptedSize = BitConverter.GetBytes(Me.Size).SubArray(0, 2)
                    Array.Reverse(m_encryptedSize)
                    AuthenticationCrypto.Encrypt(m_encryptedSize, 0, 2)
                End If

                Return m_encryptedSize
            End Get
        End Property

        Private Packet As OutPacket

        Public Sub New(command As WorldCommand, packet__1 As OutPacket)
            Me.Command = command
            Packet = packet__1
        End Sub
    End Class
End Namespace
