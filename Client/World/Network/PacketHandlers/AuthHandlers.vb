Imports System.Numerics
Imports System.Text
Imports Client.Crypto

Namespace World.Network
	Public Partial Class WorldSocket
		<PacketHandler(WorldCommand.ServerAuthChallenge)> _
		Private Sub HandleServerAuthChallenge(packet As InPacket)
			Dim one As UInteger = packet.ReadUInt32()
			Dim seed As UInteger = packet.ReadUInt32()

			Dim seed1 As BigInteger = packet.ReadBytes(16).ToBigInteger()
			Dim seed2 As BigInteger = packet.ReadBytes(16).ToBigInteger()

			Dim rand = System.Security.Cryptography.RandomNumberGenerator.Create()
			Dim bytes As Byte() = New Byte(3) {}
			rand.GetBytes(bytes)
			Dim ourSeed As BigInteger = bytes.ToBigInteger()

			Dim zero As UInteger = 0

			Dim authResponse As Byte() = HashAlgorithm.SHA1.Hash(Encoding.ASCII.GetBytes(Game.Username.ToUpper()), BitConverter.GetBytes(zero), BitConverter.GetBytes(CUInt(ourSeed)), BitConverter.GetBytes(seed), Game.Key.ToCleanByteArray())

			Dim response As New OutPacket(WorldCommand.ClientAuthSession)
			response.Write(CUInt(12340))
			' client build
			response.Write(zero)
			response.Write(Game.Username.ToUpper().ToCString())
			response.Write(zero)
			response.Write(CUInt(ourSeed))
			response.Write(zero)
			response.Write(zero)
			response.Write(zero)
			response.Write(CULng(zero))
			response.Write(authResponse)
			response.Write(zero)
			' length of addon data
			Send(response)

			' TODO: don't fully initialize here, auth may fail
			' instead, initialize in HandleServerAuthResponse when auth succeeds
			' will require special logic in network code to correctly decrypt/parse packet header
			AuthenticationCrypto.Initialize(Game.Key.ToCleanByteArray())
		End Sub

		<PacketHandler(WorldCommand.ServerAuthResponse)> _
		Private Sub HandleServerAuthResponse(packet As InPacket)
			Dim detail As CommandDetail = CType(packet.ReadByte(), CommandDetail)

			Dim billingTimeRemaining As UInteger = packet.ReadUInt32()
			Dim billingFlags As Byte = packet.ReadByte()
			Dim billingTimeRested As UInteger = packet.ReadUInt32()
			Dim expansion As Byte = packet.ReadByte()

			If detail = CommandDetail.AuthSuccess Then
				Dim request As New OutPacket(WorldCommand.ClientEnumerateCharacters)
				Send(request)
			Else
				Game.UI.LogLine(String.Format("Authentication succeeded, but received response {0}", detail))
				Game.UI.[Exit]()
			End If
		End Sub

		<PacketHandler(WorldCommand.ServerCharacterEnumeration)> _
		Private Sub HandleCharEnum(packet As InPacket)
			Dim count As Byte = packet.ReadByte()

			If count = 0 Then
				Game.UI.LogLine("No characters found!")
			Else
				Dim characters As Character() = New Character(count - 1) {}
                For i As Integer = 0 To count - 1
                    characters(i) = New Character(packet)
                Next

				Game.UI.PresentCharacterList(characters)
			End If
		End Sub
	End Class
End Namespace
