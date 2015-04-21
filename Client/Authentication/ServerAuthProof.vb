Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.IO

Namespace Authentication
	Structure ServerAuthProof
		Public command As AuthCommand
		Public [error] As AuthResult
		Public ReadOnly M2 As Byte()
		Public unk1 As UInteger
		Public unk2 As UInteger
		Public unk3 As UShort

		Public Sub New(reader As BinaryReader)
            'Me.New()
			command = AuthCommand.LOGON_PROOF
			[error] = CType(reader.ReadByte(), AuthResult)
			If [error] <> AuthResult.SUCCESS Then
				reader.ReadUInt16()
				Return
			End If

			M2 = reader.ReadBytes(20)
			unk1 = reader.ReadUInt32()
			unk2 = reader.ReadUInt32()
			unk3 = reader.ReadUInt16()
		End Sub
	End Structure
End Namespace
