Imports Client.Chat.Definitions
Imports Client.World
Imports Client.World.Definitions
Imports Client.World.Network

Public Partial Class CommandLineUI
	<KeyBind(ConsoleKey.W)> _
	Public Sub DoWhisper()
		LogLine("Enter name of player to whisper, or enter 'Q' to go back.", LogLevel.Detail)
		Log("To: ")
        Dim target As String = Game.UI.ReadLine()
		If target = "Q" Then
			Return
		End If

		Log("Message: ")
        Dim message As String = Game.UI.ReadLine()

        Dim response As OutPacket = New OutPacket(WorldCommand.CMSG_MESSAGECHAT)

		response.Write(CUInt(ChatMessageType.Whisper))
        Dim race As Race = Game.World.SelectedCharacter.Race
        Dim language__1 As Language = If(race.IsHorde(), Language.Orcish, Language.Common)
		response.Write(CUInt(language__1))
		response.Write(target.ToCString())
		response.Write(message.ToCString())
		Game.SendPacket(response)

		'! Print on WhisperInform message
	End Sub

	<KeyBind(ConsoleKey.R)> _
	Public Sub DoWhisperReply()
		If Game.World.LastWhisperers.Count = 0 Then
			Return
		End If

        Dim target As String = Game.World.LastWhisperers.Peek()
		LogLine("Hit <TAB> to cycle trough last whisperers, hit <ENTER> to select current recipient, hit <ESCAPE> to return.", LogLevel.Detail)
		LogLine([String].Format("To: {0}", target))
		While True
			Dim key As ConsoleKeyInfo = Game.UI.ReadKey()
			Select Case key.Key
				Case ConsoleKey.Enter
					GoTo cont
				Case ConsoleKey.Tab
					'! To do: maybe a more efficient way for this:
                    Dim previous As String = Game.World.LastWhisperers.Dequeue()
					Game.World.LastWhisperers.Enqueue(previous)
					target = Game.World.LastWhisperers.Peek()
					If target <> previous Then
						LogLine([String].Format("To: {0}", target))
					End If
                    Continue While
				Case ConsoleKey.Escape
					Return
				Case Else
                    Continue While
			End Select
		End While
		cont:

		Log("Message: ")
        Dim message As String = Game.UI.ReadLine()

        Dim response As OutPacket = New OutPacket(WorldCommand.CMSG_MESSAGECHAT)

		response.Write(CUInt(ChatMessageType.Whisper))
        Dim race As Race = Game.World.SelectedCharacter.Race
        Dim language__1 As Language = If(race.IsHorde(), Language.Orcish, Language.Common)
		response.Write(CUInt(language__1))
		response.Write(target.ToCString())
		response.Write(message.ToCString())
		Game.SendPacket(response)

		'! Print on WhisperInform message
	End Sub
End Class
