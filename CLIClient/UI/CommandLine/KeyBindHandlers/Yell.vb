Imports Client.Chat.Definitions
Imports Client.World
Imports Client.World.Definitions
Imports Client.World.Network
Public Partial Class CommandLineUI
	<KeyBind(ConsoleKey.Y)> _
	Public Sub DoYellChat()
		Log("Yell: ")
        Dim message As String = Game.UI.ReadLine()
        Dim response As OutPacket = New OutPacket(WorldCommand.CMSG_MESSAGECHAT)
		response.Write(CUInt(ChatMessageType.Yell))
        Dim race As Race = Game.World.SelectedCharacter.Race
        Dim language__1 As Language = If(race.IsHorde(), Language.Orcish, Language.Common)
		response.Write(CUInt(language__1))
        response.Write(message.ToCString())
		Game.SendPacket(response)
	End Sub
End Class
