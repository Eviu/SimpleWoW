Imports System.Collections.Generic
Imports Client.Chat
Imports Client.Chat.Definitions

Namespace World.Network
	Public Partial Class WorldSocket
		<PacketHandler(WorldCommand.SMSG_NAME_QUERY_RESPONSE)> _
		Private Sub HandleNameQueryResponse(packet As InPacket)
			Dim pguid = packet.ReadPackedGuid()
			Dim [end] = packet.ReadBoolean()
			If [end] Then
				'! True if not found, false if found
				Return
			End If

			Dim name = packet.ReadCString()

			If Not Game.World.PlayerNameLookup.ContainsKey(pguid) Then
				'! Add name definition per GUID
				Game.World.PlayerNameLookup.Add(pguid, name)
				'! See if any queued messages for this GUID are stored
				Dim messageQueue As Queue(Of ChatMessage) = Nothing
				If Game.World.QueuedChatMessages.TryGetValue(pguid, messageQueue) Then
					Dim m As ChatMessage
					While messageQueue.GetEnumerator().MoveNext()
						'! Print with proper name and remove from queue
						m = messageQueue.Dequeue()
						m.Sender.Sender = name
						Game.UI.PresentChatMessage(m)
					End While
				End If
			End If

			'
'            var realmName = packet.ReadCString();
'            var race = (Race)packet.ReadByte();
'            var gender = (Gender)packet.ReadByte();
'            var cClass = (Class)packet.ReadByte();
'            var decline = packet.ReadBoolean();
'
'            if (!decline)
'                return;
'
'            for (var i = 0; i < 5; i++)
'                var declinedName = packet.ReadCString();
'            

		End Sub
	End Class
End Namespace
