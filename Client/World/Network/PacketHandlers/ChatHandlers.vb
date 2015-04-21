Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Client.Chat
Imports Client.Chat.Definitions

Namespace World.Network
	Public Partial Class WorldSocket
		<PacketHandler(WorldCommand.SMSG_MESSAGECHAT)> _
		Private Sub HandleMessageChat(packet As InPacket)
			Dim type = CType(packet.ReadByte(), ChatMessageType)
			Dim lang = CType(packet.ReadInt32(), Language)
			Dim guid = packet.ReadUInt64()
			Dim unkInt = packet.ReadInt32()

			Select Case type
				Case ChatMessageType.Say, ChatMessageType.Yell, ChatMessageType.Party, ChatMessageType.PartyLeader, ChatMessageType.Raid, ChatMessageType.RaidLeader, _
					ChatMessageType.RaidWarning, ChatMessageType.Guild, ChatMessageType.Officer, ChatMessageType.Emote, ChatMessageType.TextEmote, ChatMessageType.Whisper, _
					ChatMessageType.WhisperInform, ChatMessageType.System, ChatMessageType.Channel, ChatMessageType.Battleground, ChatMessageType.BattlegroundNeutral, ChatMessageType.BattlegroundAlliance, _
					ChatMessageType.BattlegroundHorde, ChatMessageType.BattlegroundLeader, ChatMessageType.Achievement, ChatMessageType.GuildAchievement
					If True Then
						Dim channel As New ChatChannel()
						channel.Type = type

						If type = ChatMessageType.Channel Then
							channel.ChannelName = packet.ReadCString()
						End If

						Dim sender = packet.ReadUInt64()

						Dim message As New ChatMessage()
						Dim textLen = packet.ReadInt32()
						message.Message = packet.ReadCString()
						message.Language = lang
						message.ChatTag = CType(packet.ReadByte(), ChatTag)
						message.Sender = channel

						'! If we know the name of the sender GUID, use it
						'! For system messages sender GUID is 0, don't need to do anything fancy
						Dim senderName As String = Nothing
						If type = ChatMessageType.System OrElse Game.World.PlayerNameLookup.TryGetValue(sender, senderName) Then
							message.Sender.Sender = senderName
							Game.UI.PresentChatMessage(message)
							Return
						End If

						'! If not we place the message in the queue,
						'! .. either existent
						Dim messageQueue As Queue(Of ChatMessage) = Nothing
						If Game.World.QueuedChatMessages.TryGetValue(sender, messageQueue) Then
							messageQueue.Enqueue(message)
						Else
							'! or non existent
							messageQueue = New Queue(Of ChatMessage)()
							messageQueue.Enqueue(message)
							Game.World.QueuedChatMessages.Add(sender, messageQueue)
						End If

						'! Furthermore we send CMSG_NAME_QUERY to the server to retrieve the name of the sender
						Dim response As New OutPacket(WorldCommand.CMSG_NAME_QUERY)
						response.Write(sender)
						Game.SendPacket(response)

						'! Enqueued chat will be printed when we receive SMSG_NAME_QUERY_RESPONSE

						Exit Select
					End If
				Case Else
					Return
			End Select
		End Sub

		<PacketHandler(WorldCommand.SMSG_CHAT_PLAYER_NOT_FOUND)> _
		Private Sub HandleChatPlayerNotFound(packet As InPacket)
			Game.UI.LogLine([String].Format("Player {0} not found!", packet.ReadCString()))
		End Sub
	End Class
End Namespace
