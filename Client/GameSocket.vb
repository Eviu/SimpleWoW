Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Net.Sockets

Public MustInherit Class GameSocket
	Implements IDisposable
	Public Property Game() As IGame
		Get
			Return m_Game
		End Get
		Protected Set
			m_Game = Value
		End Set
	End Property
	Private m_Game As IGame

	Protected Property connection() As TcpClient
		Get
			Return m_connection
		End Get
		Set
			m_connection = Value
		End Set
	End Property
	Private m_connection As TcpClient

	Public ReadOnly Property IsConnected() As Boolean
		Get
			Return connection.Connected
		End Get
	End Property

	#Region "Asynchronous Reading"

	Protected ReceiveData As Byte()

	Public MustOverride Sub Start()

	#End Region

	Public MustOverride Function Connect() As Boolean

	Public Sub Dispose() Implements IDisposable.Dispose
		Disconnect()
	End Sub

	Public Sub Disconnect()
		If connection IsNot Nothing Then
			connection.Close()
		End If
	End Sub

	Public MustOverride Sub InitHandlers()
End Class
