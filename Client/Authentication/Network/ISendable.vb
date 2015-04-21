Imports System.IO
Imports System.Net.Sockets
Namespace Authentication.Network
	Interface ISendable
        Sub Send(writer As NetworkStream)
	End Interface
End Namespace
