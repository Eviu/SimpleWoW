Imports System.Collections.Generic
Imports System.Net.Sockets
Imports System.Reflection
Imports System.Threading
Imports Client.Authentication
Imports Client.UI

Namespace World.Network
    Partial Public Class WorldSocket
        Inherits GameSocket
        Private ServerInfo As WorldServerInfo

        Private m_transferred As Long
        Public ReadOnly Property Transferred() As Long
            Get
                Return m_transferred
            End Get
        End Property

        Private m_sent As Long
        Public ReadOnly Property Sent() As Long
            Get
                Return m_sent
            End Get
        End Property

        Private m_received As Long
        Public ReadOnly Property Received() As Long
            Get
                Return m_received
            End Get
        End Property

        Public Sub New(program As IGame, serverInfo__1 As WorldServerInfo)
            Game = program
            ServerInfo = serverInfo__1

            SendLock = New Object()
        End Sub

#Region "Handler registration"

        Private PacketHandlers As Dictionary(Of WorldCommand, PacketHandler)

        Public Overrides Sub InitHandlers()
            PacketHandlers = New Dictionary(Of WorldCommand, PacketHandler)()

            RegisterHandlersFrom(Me)
            RegisterHandlersFrom(Game)
        End Sub

        Private Sub RegisterHandlersFrom(obj As Object)
            ' create binding flags to discover all non-static methods
            Dim flags As BindingFlags = BindingFlags.[Public] Or BindingFlags.NonPublic Or BindingFlags.Instance

            Dim attributes As IEnumerable(Of PacketHandlerAttribute) = Nothing
            For Each method As MethodInfo In obj.[GetType]().GetMethods(flags)
                If Not method.TryGetAttributes(False, attributes) Then
                    Continue For
                End If

                Dim handler As PacketHandler = DirectCast(PacketHandler.CreateDelegate(GetType(PacketHandler), obj, method), PacketHandler)

                For Each attribute As PacketHandlerAttribute In attributes
                    Game.UI.LogLine(String.Format("Registered '{0}.{1}' to '{2}'", obj.[GetType]().Name, method.Name, attribute.Command), LogLevel.Debug)
                    PacketHandlers(attribute.Command) = handler
                Next
            Next
        End Sub

#End Region

#Region "Asynchronous Reading"

        Private Index As Integer
        Private Remaining As Integer

        Private Sub BeginRead(callback As AsyncCallback, Optional state As Object = Nothing)
            Me.connection.Client.BeginReceive(ReceiveData, Index, Remaining, SocketFlags.None, callback, state)
        End Sub

        ''' <summary>
        ''' Determines how large the incoming header will be by
        ''' inspecting the first byte, then initiates reading the header.
        ''' </summary>
        Private Sub ReadSizeCallback(result As IAsyncResult)
            Dim bytesRead As Integer = Me.connection.Client.EndReceive(result)
            If bytesRead = 0 AndAlso result.IsCompleted Then
                ' TODO: world server disconnect
                Game.UI.LogLine("Server has closed the connection")
                Game.[Exit]()
                Return
            End If

            Interlocked.Increment(m_transferred)
            Interlocked.Increment(m_received)

            AuthenticationCrypto.Decrypt(ReceiveData, 0, 1)
            If (ReceiveData(0) And &H80) <> 0 Then
                ' need to resize the buffer
                Dim temp As Byte = ReceiveData(0)
                ReceiveData = New Byte(4) {}
                ReceiveData(0) = CByte((&H7F And temp))

                Remaining = 4
            Else
                Remaining = 3
            End If

            Index = 1
            BeginRead(New AsyncCallback(AddressOf ReadHeaderCallback))
        End Sub

        ''' <summary>
        ''' Reads the rest of the incoming header.
        ''' </summary>
        Private Sub ReadHeaderCallback(result As IAsyncResult)
            'if (ReceiveData.Length != 4 && ReceiveData.Length != 5)
            '  throw new Exception("ReceiveData.Length not in order");

            Dim bytesRead As Integer = Me.connection.Client.EndReceive(result)
            If bytesRead = 0 AndAlso result.IsCompleted Then
                ' TODO: world server disconnect
                Game.UI.LogLine("Server has closed the connection")
                Game.[Exit]()
                Return
            End If

            Interlocked.Add(m_transferred, bytesRead)
            Interlocked.Add(m_received, bytesRead)

            If bytesRead = Remaining Then
                ' finished reading header
                ' the first byte was decrypted already, so skip it
                AuthenticationCrypto.Decrypt(ReceiveData, 1, ReceiveData.Length - 1)
                Dim header As New ServerHeader(ReceiveData)

                Game.UI.LogLine(header.ToString(), LogLevel.Debug)
                If header.InputDataLength > 5 OrElse header.InputDataLength < 4 Then
                    Game.UI.LogException([String].Format("Header.InputataLength invalid: {0}", header.InputDataLength))
                End If

                If header.Size > 0 Then
                    ' read the packet payload
                    Index = 0
                    Remaining = header.Size
                    ReceiveData = New Byte(header.Size - 1) {}
                    BeginRead(New AsyncCallback(AddressOf ReadPayloadCallback), header)
                Else
                    ' the packet is just a header, start next packet
                    HandlePacket(New InPacket(header))
                    Start()
                End If
            Else
                ' more header to read
                Index += bytesRead
                Remaining -= bytesRead
                BeginRead(New AsyncCallback(AddressOf ReadHeaderCallback))
            End If
        End Sub

        ''' <summary>
        ''' Reads the payload data.
        ''' </summary>
        Private Sub ReadPayloadCallback(result As IAsyncResult)
            Dim bytesRead As Integer = Me.connection.Client.EndReceive(result)
            If bytesRead = 0 AndAlso result.IsCompleted Then
                ' TODO: world server disconnect
                Game.UI.LogLine("Server has closed the connection")
                Game.[Exit]()
                Return
            End If

            Interlocked.Add(m_transferred, bytesRead)
            Interlocked.Add(m_received, bytesRead)

            If bytesRead = Remaining Then
                ' get header and packet, handle it
                Dim header As ServerHeader = DirectCast(result.AsyncState, ServerHeader)
                Dim packet As New InPacket(header, ReceiveData)
                HandlePacket(packet)

                ' start new asynchronous read
                Start()
            Else
                ' more payload to read
                Index += bytesRead
                Remaining -= bytesRead
                BeginRead(New AsyncCallback(AddressOf ReadPayloadCallback), result.AsyncState)
            End If
        End Sub

#End Region

        Private Sub HandlePacket(packet As InPacket)
            Dim handler As PacketHandler = Nothing
            If PacketHandlers.TryGetValue(CType(packet.Header.Command, WorldCommand), handler) Then
                Game.UI.LogLine(String.Format("Received {0}", packet.Header.Command), LogLevel.Debug)

                If AuthenticationCrypto.Status = AuthStatus.Ready Then
                    ' AuthenticationCrypto is ready, handle the packet asynchronously
                    handler.BeginInvoke(packet, Sub(result) handler.EndInvoke(result), Nothing)
                Else
                    handler(packet)
                End If
            Else
                Game.UI.LogLine(String.Format("Unknown or unhandled command '{0}'", packet.Header.Command), LogLevel.Debug)
            End If
        End Sub

#Region "GameSocket Members"

        Public Overrides Sub Start()
            ReceiveData = New Byte(3) {}
            Index = 0
            Remaining = 1
            BeginRead(New AsyncCallback(AddressOf ReadSizeCallback))
        End Sub

        Public Overrides Function Connect() As Boolean
            Try
                Game.UI.Log(String.Format("Connecting to realm {0}... ", ServerInfo.Name))

                connection = New TcpClient(ServerInfo.Address, ServerInfo.Port)

                Game.UI.LogLine("done!")
            Catch ex As SocketException
                Game.UI.LogLine(String.Format("failed. ({0})", CType(ex.ErrorCode, SocketError)), LogLevel.[Error])
                Return False
            End Try

            Return True
        End Function

#End Region

        Private SendLock As Object

        Public Sub Send(packet As OutPacket)
            Dim data As Byte() = packet.Finalize()

            ' TODO: switch to asynchronous send
            SyncLock SendLock
                connection.Client.Send(data)
            End SyncLock

            Interlocked.Add(m_transferred, data.Length)
            Interlocked.Add(m_sent, data.Length)
        End Sub
    End Class
End Namespace
