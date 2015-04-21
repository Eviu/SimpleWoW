Imports Client.World.Network

Namespace World
	Public Delegate Sub PacketHandler(packet As InPacket)

	<AttributeUsage(AttributeTargets.Method, AllowMultiple := True, Inherited := False)> _
	Public NotInheritable Class PacketHandlerAttribute
		Inherits Attribute
		Public Property Command() As WorldCommand
			Get
				Return m_Command
			End Get
			Private Set
				m_Command = Value
			End Set
		End Property
		Private m_Command As WorldCommand

		Public Sub New(command__1 As WorldCommand)
			Command = command__1
		End Sub
	End Class
End Namespace
