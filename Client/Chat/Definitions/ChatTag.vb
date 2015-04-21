Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Chat.Definitions
	<Flags> _
	Public Enum ChatTag
		None = &H0
		Afk = &H1
		Dnd = &H2
		Gm = &H4
		Unknown = &H8
	End Enum
End Namespace
