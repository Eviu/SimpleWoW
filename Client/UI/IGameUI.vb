Imports Client.Authentication
Imports Client.World
Imports Client.Chat

Namespace UI
	Public Interface IGameUI
        Property Game() As IGame

        Property LogLevel() As LogLevel

        Sub Update()
        Sub [Exit]()

#Region "Packet handler presenters"

        Sub PresentRealmList(realmList As WorldServerList)
        Sub PresentCharacterList(characterList As Character())

        Sub PresentChatMessage(message As ChatMessage)

#End Region

#Region "UI Output"

        Sub Log(message As String, Optional level As LogLevel = LogLevel.Info)
        Sub LogLine(message As String, Optional level As LogLevel = LogLevel.Info)
        Sub LogException(message As String)

#End Region

		#Region "UI Input"

		Function ReadLine() As String
		Function Read() As Integer
		Function ReadKey() As ConsoleKeyInfo

		#End Region
	End Interface
End Namespace
