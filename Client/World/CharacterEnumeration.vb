Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Client.World.Network
Imports Client.World.Definitions

Namespace World
	Public Class Character
		Public GUID As ULong
		Public Property Name() As String
			Get
				Return m_Name
			End Get
			Private Set
				m_Name = Value
			End Set
		End Property
		Private m_Name As String
		Public Property Race() As Race
			Get
				Return m_Race
			End Get
			Private Set
				m_Race = Value
			End Set
		End Property
		Private m_Race As Race
		Public Property [Class]() As [Class]
			Get
				Return m_Class
			End Get
			Private Set
				m_Class = Value
			End Set
		End Property
		Private m_Class As [Class]
		Private Gender As Gender
		Private Bytes As Byte()
		' 5
		Public Property Level() As Byte
			Get
				Return m_Level
			End Get
			Private Set
				m_Level = Value
			End Set
		End Property
		Private m_Level As Byte
		Private ZoneId As UInteger
		Private MapId As UInteger
		Private X As Single, Y As Single, Z As Single
		Private GuildId As UInteger
		Private Flags As UInteger
		Private PetInfoId As UInteger
		Private PetLevel As UInteger
		Private PetFamilyId As UInteger
		Private Items As Item() = New Item(19) {}

		Public Sub New(packet As InPacket)
			GUID = packet.ReadUInt64()
			Name = packet.ReadCString()
			Race = CType(packet.ReadByte(), Race)
			[Class] = CType(packet.ReadByte(), [Class])
			Gender = CType(packet.ReadByte(), Gender)
			Bytes = packet.ReadBytes(5)
			Level = packet.ReadByte()
			ZoneId = packet.ReadUInt32()
			MapId = packet.ReadUInt32()
			X = packet.ReadSingle()
			Y = packet.ReadSingle()
			Z = packet.ReadSingle()
			GuildId = packet.ReadUInt32()
			Flags = packet.ReadUInt32()
			packet.ReadUInt32()
			' customize (rename, etc)
			packet.ReadByte()
			' first login
			PetInfoId = packet.ReadUInt32()
			PetLevel = packet.ReadUInt32()
			PetFamilyId = packet.ReadUInt32()

			' read items
			For i As Integer = 0 To Items.Length - 2
				Items(i) = New Item(packet)
			Next

			' read bags
			For i As Integer = 0 To 3
				packet.ReadUInt32()
				packet.ReadByte()
				packet.ReadUInt32()
			Next
		End Sub
	End Class

	Class Item
		Private DisplayId As UInteger
		Private InventoryType As Byte

		Public Sub New(packet As InPacket)
			DisplayId = packet.ReadUInt32()
			InventoryType = packet.ReadByte()
				' ???
			packet.ReadUInt32()
		End Sub
	End Class
End Namespace
