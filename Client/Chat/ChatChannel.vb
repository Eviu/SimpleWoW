Imports Client.Chat.Definitions

Namespace Chat
	''' <summary>
	''' A channel trough which a message is sent. This can also be whisper.
	''' </summary>
	Public Class ChatChannel
		Public Type As ChatMessageType
		Public Sender As String
		Public ChannelName As String = String.Empty
		' Optional
	End Class
End Namespace
