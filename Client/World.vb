Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Client.Chat
Imports Client.World

''' <summary>
''' Stores world variables
''' </summary>
Public Class GameWorld
	'! Player name lookup per GUID - trough CMSG/SMSG_NAME_QUERY(_response)
	Public PlayerNameLookup As New Dictionary(Of ULong, String)()

	'! Message queue for when sender's name hasn't been queried trough NAME_QUERY yet
	Public QueuedChatMessages As New Dictionary(Of ULong, Queue(Of ChatMessage))()

	'! Character currently logged into world
	Public SelectedCharacter As Character

	'! Persons who last whispered the client
	Public LastWhisperers As New Queue(Of String)()
End Class
