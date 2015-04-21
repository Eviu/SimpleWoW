Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.IO
Imports System.Net.Sockets
Imports Client.Authentication.Network

Namespace Authentication
	Structure ClientAuthChallenge
		Implements ISendable
		Public username As String
		Public IP As UInteger

		Shared ReadOnly version As Byte() = New Byte() {3, 3, 5}
		Const build As UShort = 12340

		#Region "ISendable Members"

        Public Sub Send(writer As NetworkStream) Implements ISendable.Send
            Using Stream As New MemoryStream(128)
                Dim binaryStream As New BinaryWriter(Stream)
                binaryStream.Write(CByte(AuthCommand.LOGON_CHALLENGE))
                binaryStream.Write(CByte(6))
                binaryStream.Write(Convert.ToUInt16(username.Length + 30))
                binaryStream.Write("WoW".ToCString())
                binaryStream.Write(version)
                binaryStream.Write(build)
                binaryStream.Write("68x".ToCString())
                binaryStream.Write("niW".ToCString())
                binaryStream.Write(Encoding.ASCII.GetBytes("SUne"))
                binaryStream.Write(CUInt(&H3C))
                binaryStream.Write(IP)
                binaryStream.Write(CByte(username.Length))
                binaryStream.Write(Encoding.ASCII.GetBytes(username))
                Stream.Seek(0, SeekOrigin.Begin)
                Dim buffer As Byte() = Stream.ToArray()
                writer.Write(buffer, 0, buffer.Length)
            End Using
        End Sub

		#End Region
	End Structure
End Namespace
