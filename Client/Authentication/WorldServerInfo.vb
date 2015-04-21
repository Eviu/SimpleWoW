Imports System.IO

Namespace Authentication
	Public Class WorldServerInfo
		Public Property Type() As Byte
			Get
				Return m_Type
			End Get
			Private Set
				m_Type = Value
			End Set
		End Property
		Private m_Type As Byte
		Private locked As Byte
		Public Property Flags() As Byte
			Get
				Return m_Flags
			End Get
			Private Set
				m_Flags = Value
			End Set
		End Property
		Private m_Flags As Byte
		Public Property Name() As String
			Get
				Return m_Name
			End Get
			Private Set
				m_Name = Value
			End Set
		End Property
		Private m_Name As String
		Public Property Address() As String
			Get
				Return m_Address
			End Get
			Private Set
				m_Address = Value
			End Set
		End Property
		Private m_Address As String
		Public Property Port() As Integer
			Get
				Return m_Port
			End Get
			Private Set
				m_Port = Value
			End Set
		End Property
		Private m_Port As Integer
		Public Property Population() As Single
			Get
				Return m_Population
			End Get
			Private Set
				m_Population = Value
			End Set
		End Property
		Private m_Population As Single
		Private load As Byte
		Private timezone As Byte
		Private version_major As Byte
		Private version_minor As Byte
		Private version_bugfix As Byte
		Private build As UShort
		Private unk1 As Byte
		Private unk2 As UShort

		Public Sub New(reader As BinaryReader)
			Type = reader.ReadByte()
			locked = reader.ReadByte()
			Flags = reader.ReadByte()
			Name = reader.ReadCString()
			Dim address__1 As String = reader.ReadCString()
			Dim tokens As String() = address__1.Split(":"C)
			Address = tokens(0)
			Port = If(tokens.Length > 1, Integer.Parse(tokens(1)), 8085)
			Population = reader.ReadSingle()
			load = reader.ReadByte()
			timezone = reader.ReadByte()
			unk1 = reader.ReadByte()

			If (Flags And 4) <> 0 Then
				version_major = reader.ReadByte()
				version_minor = reader.ReadByte()
				version_bugfix = reader.ReadByte()
				build = reader.ReadUInt16()
			End If
		End Sub
	End Class
End Namespace
