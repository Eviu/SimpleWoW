Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.IO

Namespace Authentication
	Structure ServerAuthChallenge
		Public ReadOnly command As AuthCommand
		Public ReadOnly unk2 As Byte
		Public ReadOnly [error] As AuthResult
		Public ReadOnly B As Byte()
		Public ReadOnly gLen As Byte
		Public ReadOnly g As Byte()
		Public ReadOnly nLen As Byte
		Public ReadOnly N As Byte()
		Public ReadOnly salt As Byte()
		Public ReadOnly unk3 As Byte()
		Public ReadOnly securityFlags As Byte

		Public Sub New(reader As BinaryReader)
			command = AuthCommand.LOGON_CHALLENGE
			unk2 = reader.ReadByte()
			[error] = CType(reader.ReadByte(), AuthResult)
			If [error] <> AuthResult.SUCCESS Then
				Return
			End If

			B = reader.ReadBytes(32)
			gLen = reader.ReadByte()
			g = reader.ReadBytes(1)
			nLen = reader.ReadByte()
			N = reader.ReadBytes(32)
			salt = reader.ReadBytes(32)
			unk3 = reader.ReadBytes(16)
			securityFlags = reader.ReadByte()
		End Sub
	End Structure
End Namespace
