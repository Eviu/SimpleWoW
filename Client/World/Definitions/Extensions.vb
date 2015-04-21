
Namespace World.Definitions
    Public Module Extensions
        Sub New()
        End Sub
        <System.Runtime.CompilerServices.Extension> _
        Public Function IsHorde(race__1 As Race) As Boolean
            Return race__1 = Race.Bloodelf OrElse race__1 = Race.Orc OrElse race__1 = Race.Tauren OrElse race__1 = Race.Troll OrElse race__1 = Race.Undead
        End Function

        <System.Runtime.CompilerServices.Extension> _
        Public Function IsAlliance(race__1 As Race) As Boolean
            Return race__1 = Race.Draenei OrElse race__1 = Race.Dwarf OrElse race__1 = Race.Gnome OrElse race__1 = Race.Human OrElse race__1 = Race.Nightelf
        End Function
    End Module
End Namespace

