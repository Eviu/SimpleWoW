Imports System.Numerics
Imports System.Threading
Imports Client.Authentication
Imports Client.Authentication.Network
Imports Client.UI
Imports Client.World.Network
Imports Client.Chat
Imports System.Collections.Generic
Imports System.Threading.Tasks

Public Interface IGame
    Property Key() As BigInteger
    Property Username() As String

    Property UI() As IGameUI

    Property World() As GameWorld

    Sub ConnectTo(server As WorldServerInfo)
    Sub Reconnect()
    Sub Start()

    Function [Exit]() As Task

    Sub SendPacket(packet As OutPacket)
End Interface

Public Class Game(Of T As {IGameUI, New})
    Implements IGame
    Private Running As Boolean

    Private socket As GameSocket

    Public Property Key() As BigInteger Implements IGame.Key
        Get
            Return m_Key
        End Get
        Private Set(value As BigInteger)
            m_Key = value
        End Set
    End Property
    Private m_Key As BigInteger
    Public Property Username() As String Implements IGame.Username
        Get
            Return m_Username
        End Get
        Private Set(value As String)
            m_Username = value
        End Set
    End Property
    Private m_Username As String

    Public Property UI() As IGameUI Implements IGame.UI
        Get
            Return m_UI
        End Get
        Protected Set(value As IGameUI)
            m_UI = value
        End Set
    End Property
    Private m_UI As IGameUI

    Public Property World() As GameWorld Implements IGame.World
        Get
            Return _world
        End Get
        Private Set(value As GameWorld)
            _world = value
        End Set
    End Property

    Private _world As GameWorld

    Public Sub New(hostname As String, port As Integer, username As String, password As String, logLevel As LogLevel)
        UI = New T()
        UI.Game = Me
        UI.LogLevel = logLevel

        World = New GameWorld()

        Me.Username = username

        socket = New AuthSocket(Me, hostname, port, username, password)
        socket.InitHandlers()
    End Sub

    Public Sub ConnectTo(server As WorldServerInfo) Implements IGame.ConnectTo
        If TypeOf socket Is AuthSocket Then
            Key = DirectCast(socket, AuthSocket).Key
        End If

        socket.Dispose()

        socket = New WorldSocket(Me, server)
        socket.InitHandlers()

        If socket.Connect() Then
            socket.Start()
        Else
            [Exit]().Wait()
        End If
    End Sub
    Public Sub Reconnect() Implements IGame.Reconnect
        [Exit]().Wait()
    End Sub

    Public Sub Start() Implements IGame.Start
        ' the initial socket is an AuthSocket - it will initiate its own asynch read
        Running = socket.Connect()

        While Running
            ' main loop here
            UI.Update()
            Thread.Sleep(100)
        End While

        UI.[Exit]()
    End Sub

    Public Async Function [Exit]() As Task Implements IGame.[Exit]
        Running = False
        Await Task.FromResult(False)
    End Function

    Public Sub SendPacket(packet As OutPacket) Implements IGame.SendPacket
        If TypeOf socket Is WorldSocket Then
            DirectCast(socket, WorldSocket).Send(packet)
        End If
    End Sub
End Class
