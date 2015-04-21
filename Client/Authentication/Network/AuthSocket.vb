Imports System.Collections.Generic
Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Numerics
Imports System.Text
Imports Client.Crypto
Imports Client.UI

Namespace Authentication.Network
    Class AuthSocket
        Inherits GameSocket
        Public Property Key() As BigInteger
            Get
                Return m_Key
            End Get
            Private Set(value As BigInteger)
                m_Key = Value
            End Set
        End Property
        Private m_Key As BigInteger
        Private m2 As Byte()

        Dim stream As NetworkStream

        Private Username As String
        Private PasswordHash As Byte()

        Private Hostname As String
        Private Port As Integer

        Private Handlers As Dictionary(Of AuthCommand, CommandHandler)

        Public Sub New(program As IGame, hostname As String, port As Integer, username As String, password As String)
            Me.Game = program

            Me.Username = username
            Me.Hostname = hostname
            Me.Port = port

            Dim authstring As String = String.Format("{0}:{1}", Me.Username, password)

            PasswordHash = HashAlgorithm.SHA1.Hash(Encoding.ASCII.GetBytes(authstring.ToUpper()))

            ReceiveData = New Byte(0) {}
        End Sub

        Protected Overrides Sub Finalize()
            Try
                Dispose()
            Finally
                MyBase.Finalize()
            End Try
        End Sub

        Private Sub SendLogonChallenge()
            Game.UI.LogLine("Sending logon challenge", LogLevel.Debug)

            Dim challenge As New ClientAuthChallenge() With { _
                 .username = Username, _
                 .IP = BitConverter.ToUInt32(TryCast(connection.Client.LocalEndPoint, IPEndPoint).Address.GetAddressBytes(), 0) _
            }

            challenge.Send(stream)
            ReadCommand()
        End Sub

#Region "Handlers"

        Public Overrides Sub InitHandlers()
            Handlers = New Dictionary(Of AuthCommand, CommandHandler)()

            Handlers(AuthCommand.LOGON_CHALLENGE) = AddressOf HandleRealmLogonChallenge
            Handlers(AuthCommand.LOGON_PROOF) = AddressOf HandleRealmLogonProof
            Handlers(AuthCommand.REALM_LIST) = AddressOf HandleRealmList
        End Sub

        Private Sub HandleRealmLogonChallenge()
            Dim challenge As New ServerAuthChallenge(New BinaryReader(connection.GetStream()))

            Select Case challenge.[error]
                Case AuthResult.SUCCESS
                    If True Then
                        Game.UI.LogLine("Received logon challenge", LogLevel.Debug)

                        Dim N As BigInteger, A__1 As BigInteger, B As BigInteger, a__2 As BigInteger, u As BigInteger, x As BigInteger, _
                            S As BigInteger, salt As BigInteger, unk1 As BigInteger, g As BigInteger, k As BigInteger
                        k = New BigInteger(3)

                        '#Region "Receive and initialize"

                        B = challenge.B.ToBigInteger()
                        ' server public key
                        g = challenge.g.ToBigInteger()
                        N = challenge.N.ToBigInteger()
                        ' modulus
                        salt = challenge.salt.ToBigInteger()
                        unk1 = challenge.unk3.ToBigInteger()

                        Game.UI.LogLine("---====== Received from server: ======---", LogLevel.Debug)
                        Game.UI.LogLine(String.Format("B={0}", B.ToCleanByteArray().ToHexString()), LogLevel.Debug)
                        Game.UI.LogLine(String.Format("N={0}", N.ToCleanByteArray().ToHexString()), LogLevel.Debug)
                        Game.UI.LogLine(String.Format("salt={0}", challenge.salt.ToHexString()), LogLevel.Debug)

                        '#End Region

                        '#Region "Hash password"

                        x = HashAlgorithm.SHA1.Hash(challenge.salt, PasswordHash).ToBigInteger()

                        Game.UI.LogLine("---====== shared password hash ======---", LogLevel.Debug)
                        Game.UI.LogLine(String.Format("g={0}", g.ToCleanByteArray().ToHexString()), LogLevel.Debug)
                        Game.UI.LogLine(String.Format("x={0}", x.ToCleanByteArray().ToHexString()), LogLevel.Debug)
                        Game.UI.LogLine(String.Format("N={0}", N.ToCleanByteArray().ToHexString()), LogLevel.Debug)

                        '#End Region

                        '#Region "Create random key pair"

                        Dim rand As System.Security.Cryptography.RandomNumberGenerator = System.Security.Cryptography.RandomNumberGenerator.Create()

                        Do
                            Dim randBytes As Byte() = New Byte(18) {}
                            rand.GetBytes(randBytes)
                            a__2 = randBytes.ToBigInteger()

                            A__1 = g.ModPow(a__2, N)
                        Loop While A__1.ModPow(1, N) = 0

                        Game.UI.LogLine("---====== Send data to server: ======---", LogLevel.Debug)
                        Game.UI.LogLine(String.Format("A={0}", A__1.ToCleanByteArray().ToHexString()), LogLevel.Debug)

                        '#End Region

                        '#Region "Compute session key"

                        u = HashAlgorithm.SHA1.Hash(A__1.ToCleanByteArray(), B.ToCleanByteArray()).ToBigInteger()

                        ' compute session key
                        ' TODO: session key computation fails for some reason
                        '     all variables match exactly to the server side, but
                        '     S is different
                        S = (B - k * g.ModPow(x, N)).ModPow(a__2 + u * x, N)

                        Dim keyHash As Byte()
                        Dim sData As Byte() = S.ToCleanByteArray()
                        Dim keyData As Byte() = New Byte(39) {}
                        Dim temp As Byte() = New Byte(15) {}

                        ' take every even indices byte, hash, store in even indices
                        For i As Integer = 0 To 15
                            temp(i) = sData(i * 2)
                        Next
                        keyHash = HashAlgorithm.SHA1.Hash(temp)
                        For i As Integer = 0 To 19
                            keyData(i * 2) = keyHash(i)
                        Next

                        ' do the same for odd indices
                        For i As Integer = 0 To 15
                            temp(i) = sData(i * 2 + 1)
                        Next
                        keyHash = HashAlgorithm.SHA1.Hash(temp)
                        For i As Integer = 0 To 19
                            keyData(i * 2 + 1) = keyHash(i)
                        Next

                        Key = keyData.ToBigInteger()

                        Game.UI.LogLine("---====== Compute session key ======---", LogLevel.Debug)
                        Game.UI.LogLine(String.Format("u={0}", u.ToCleanByteArray().ToHexString()), LogLevel.Debug)
                        Game.UI.LogLine(String.Format("S={0}", S.ToCleanByteArray().ToHexString()), LogLevel.Debug)
                        Game.UI.LogLine(String.Format("K={0}", Key.ToCleanByteArray().ToHexString()), LogLevel.Debug)

                        '#End Region

                        '#Region "Generate crypto proof"

                        ' XOR the hashes of N and g together
                        Dim gNHash As Byte() = New Byte(19) {}

                        Dim nHash As Byte() = HashAlgorithm.SHA1.Hash(N.ToCleanByteArray())
                        For i As Integer = 0 To 19
                            gNHash(i) = nHash(i)
                        Next
                        Game.UI.LogLine(String.Format("nHash={0}", nHash.ToHexString()), LogLevel.Debug)

                        Dim gHash As Byte() = HashAlgorithm.SHA1.Hash(g.ToCleanByteArray())
                        For i As Integer = 0 To 19
                            gNHash(i) = gNHash(i) Xor gHash(i)
                        Next
                        Game.UI.LogLine(String.Format("gHash={0}", gHash.ToHexString()), LogLevel.Debug)

                        ' hash username
                        Dim userHash As Byte() = HashAlgorithm.SHA1.Hash(Encoding.ASCII.GetBytes(Username))

                        ' our proof
                        Dim m1Hash As Byte() = HashAlgorithm.SHA1.Hash(gNHash, userHash, challenge.salt, A__1.ToCleanByteArray(), B.ToCleanByteArray(), Key.ToCleanByteArray())

                        Game.UI.LogLine("---====== Client proof: ======---", LogLevel.Debug)
                        Game.UI.LogLine(String.Format("gNHash={0}", gNHash.ToHexString()), LogLevel.Debug)
                        Game.UI.LogLine(String.Format("userHash={0}", userHash.ToHexString()), LogLevel.Debug)
                        Game.UI.LogLine(String.Format("salt={0}", challenge.salt.ToHexString()), LogLevel.Debug)
                        Game.UI.LogLine(String.Format("A={0}", A__1.ToCleanByteArray().ToHexString()), LogLevel.Debug)
                        Game.UI.LogLine(String.Format("B={0}", B.ToCleanByteArray().ToHexString()), LogLevel.Debug)
                        Game.UI.LogLine(String.Format("key={0}", Key.ToCleanByteArray().ToHexString()), LogLevel.Debug)

                        Game.UI.LogLine("---====== Send proof to server: ======---", LogLevel.Debug)
                        Game.UI.LogLine(String.Format("M={0}", m1Hash.ToHexString()), LogLevel.Debug)

                        ' expected proof for server
                        m2 = HashAlgorithm.SHA1.Hash(A__1.ToCleanByteArray(), m1Hash, keyData)

                        '#End Region

                        '#Region "Send proof"

                        Dim proof As New ClientAuthProof() With { _
                             .A = A__1.ToCleanByteArray(), _
                             .M1 = m1Hash, _
                             .crc = New Byte(19) {} _
                        }

                        Game.UI.LogLine("Sending logon proof", LogLevel.Debug)
                        proof.Send(stream)

                        '#End Region

                        Exit Select
                    End If
                Case AuthResult.NO_MATCH
                    Game.UI.LogLine("Unknown account name", LogLevel.[Error])
                    Exit Select
                Case AuthResult.ACCOUNT_IN_USE
                    Game.UI.LogLine("Account already logged in", LogLevel.[Error])
                    Exit Select
                Case AuthResult.WRONG_BUILD_NUMBER
                    Game.UI.LogLine("Wrong build number", LogLevel.[Error])
                    Exit Select
            End Select

            ' get next command
            ReadCommand()
        End Sub

        Private Sub HandleRealmLogonProof()
            Dim proof As New ServerAuthProof(New BinaryReader(connection.GetStream()))

            Select Case proof.[error]
                Case AuthResult.UPDATE_CLIENT
                    Game.UI.LogLine("Client update requested")
                    Exit Select
                Case AuthResult.NO_MATCH, AuthResult.UNKNOWN2
                    Game.UI.LogLine("Wrong password or invalid account or authentication error", LogLevel.[Error])
                    Exit Select
                Case AuthResult.WRONG_BUILD_NUMBER
                    Game.UI.LogLine("Wrong build number", LogLevel.[Error])
                    Exit Select
                Case Else
                    If proof.[error] <> AuthResult.SUCCESS Then
                        Game.UI.LogLine(String.Format("Unkown error {0}", proof.[error]), LogLevel.[Error])
                    End If
                    Exit Select
            End Select

            If proof.[error] <> AuthResult.SUCCESS Then
                SendLogonChallenge()
                Return
            End If

            Game.UI.LogLine("Received logon proof", LogLevel.Debug)

            Dim equal As Boolean = True
            equal = m2 IsNot Nothing AndAlso m2.Length = 20
            Dim i As Integer = 0
            While i < m2.Length AndAlso equal
                If Not (InlineAssignHelper(equal, m2(i) = proof.M2(i))) Then
                    Exit While
                End If
                i += 1
            End While

            If Not equal Then
                Game.UI.LogLine("Server auth failed!", LogLevel.[Error])
            Else
                Game.UI.LogLine("Authentication succeeded!")
                Game.UI.LogLine("Requesting realm list", LogLevel.Detail)
                Dim buffer = New Byte() {CByte(AuthCommand.REALM_LIST), &H0, &H0, &H0, &H0}
                stream.Write(buffer, 0, buffer.Length)
            End If

            ' get next command
            ReadCommand()
        End Sub

        Private Sub HandleRealmList()
            Dim reader As BinaryReader = New BinaryReader(connection.GetStream())

            Dim size As UInteger = reader.ReadUInt16()
            Dim realmList As New WorldServerList(reader)
            Game.UI.LogLine("Received realm list", LogLevel.Debug)

            Game.UI.PresentRealmList(realmList)
        End Sub

#End Region

#Region "GameSocket Members"

        Public Overrides Sub Start()
            ReadCommand()
        End Sub

        Private Sub ReadCommand()
            Try
                ' buffer and buffer bounds
                ' flags for the read
                ' callback to handle completion
                ' state object
                Me.connection.Client.BeginReceive(ReceiveData, 0, 1, SocketFlags.None, AddressOf Me.ReadCallback, Nothing)
            Catch
            End Try
        End Sub

        Protected Sub ReadCallback(result As IAsyncResult)
            Try
                Dim size As Integer = Me.connection.Client.EndReceive(result)

                If size = 0 Then
                    Game.UI.LogLine("Server has disconnected.", LogLevel.Info)
                    Game.[Exit]()
                End If

                Dim command As AuthCommand = DirectCast(ReceiveData(0), AuthCommand)

                Dim handler As CommandHandler = Nothing
                If Handlers.TryGetValue(command, handler) Then
                    handler()
                Else
                    Game.UI.LogLine(String.Format("Unkown or unhandled command '{0}'", command), LogLevel.Warning)
                End If
                ' these exceptions can happen as race condition on shutdown
            Catch ex As ObjectDisposedException
                Game.UI.LogLine(ex.Message.ToString(), LogLevel.Error)
            Catch ex As NullReferenceException
                Game.UI.LogLine(ex.Message.ToString(), LogLevel.Error)
            Catch ex As SocketException
                Game.UI.LogLine(ex.Message.ToString(), LogLevel.Error)
                Game.Reconnect()
            End Try
        End Sub

        Public Overrides Function Connect() As Boolean
            Game.UI.LogLine("   _____           _   _   _       ______          _           _   ")
            Game.UI.LogLine("  |  ___|         (_) | | ( )      | ___ \        (_)         | |  ")
            Game.UI.LogLine("  | |__   __   __  _  | | |/ ___   | |_/ / __ ___  _  ___  ___| |_ ")
            Game.UI.LogLine("  |  __|  \ \ / / | | | |   / __|  |  __/ '__/ _ \| |/ _ \/ __| __|")
            Game.UI.LogLine("  | |___   \ V /  | | | |   \__ \  | |  | | | (_) | |  __/ (__| |_ ")
            Game.UI.LogLine("  \____/    \_/   |_| |_|   |___/  \_|  |_|  \___/| |\___|\___|\__|")
            Game.UI.LogLine("                                                 _/ |              ")
            Game.UI.LogLine("                                                |__/               ")
            Game.UI.LogLine("  Copyright (C) Evil's Project 2015 - " & My.Computer.Clock.LocalTime.Year)
            Game.UI.LogLine("")
            Try
                Game.UI.Log("Connecting to realmlist... ")

                connection = New TcpClient(Me.Hostname, Me.Port)
                Stream = connection.GetStream()

                Game.UI.LogLine("done!")

                SendLogonChallenge()
            Catch ex As SocketException
                Game.UI.LogLine(String.Format("failed. ({0})", CType(ex.ErrorCode, SocketError)), LogLevel.[Error])
                Return False
            End Try

            Return True
        End Function
        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
            target = value
            Return value
        End Function

#End Region
    End Class
End Namespace
