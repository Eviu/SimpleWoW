Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.IO
Imports Client.Authentication.Network
Imports System.Net.Sockets
Namespace Authentication
	Structure ClientAuthProof
		Implements ISendable
		Public A As Byte()
		Public M1 As Byte()
		Public crc As Byte()

		#Region "ISendable Members"

        Public Sub Send(writer As NetworkStream) Implements ISendable.Send
            Using stream = New MemoryStream(1 + A.Length + M1.Length + crc.Length + 2)
                Dim binaryStream = New BinaryWriter(stream)
                binaryStream.Write(CByte(AuthCommand.LOGON_PROOF))
                binaryStream.Write(A)
                binaryStream.Write(M1)
                binaryStream.Write(crc)
                binaryStream.Write(CByte(0))
                binaryStream.Write(CByte(0))
                stream.Seek(0, SeekOrigin.Begin)
                Dim buffer = stream.ToArray()
                writer.Write(buffer, 0, buffer.Length)
            End Using
        End Sub

		#End Region
	End Structure
End Namespace
