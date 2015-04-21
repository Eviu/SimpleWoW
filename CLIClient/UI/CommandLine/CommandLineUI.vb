Imports System.Diagnostics
Imports System.IO
Imports System.Text
Imports Client
Imports Client.Authentication
Imports Client.Chat
Imports Client.Chat.Definitions
Imports Client.World
Imports Client.World.Network

Partial Public Class CommandLineUI
    Implements IGameUI
#Region "Private Members"
    Private _logLevel As LogLevel
    Private _logFile As StreamWriter

#End Region

    Public Sub New()
        _logFile = New StreamWriter("Client.log".Replace(":"c, "_"c).Replace("/"c, "-"c))
        _logFile.AutoFlush = True

        InitializeKeybinds()
    End Sub

#Region "IGameUI Members"

    Public Property LogLevel() As LogLevel Implements IGameUI.LogLevel
        Get
            Return _logLevel
        End Get
        Set(value As LogLevel)
            _logLevel = value
        End Set
    End Property

    Public Property Game() As IGame Implements IGameUI.Game
        Get
            Return m_Game
        End Get
        Set(value As IGame)
            m_Game = Value
        End Set
    End Property
    Private m_Game As IGame

    Public Sub Update() Implements IGameUI.Update
        If Game.World.SelectedCharacter Is Nothing Then
            Return
        End If

        Dim keyPress As ConsoleKeyInfo = Console.ReadKey()
        Dim handler As KeyBind = Nothing
        If _keyPressHandlers.TryGetValue(keyPress.Key, handler) Then
            handler()
        End If
    End Sub

    Public Sub [Exit]() Implements IGameUI.[Exit]
        Console.Write("Press any key to continue...")
        Console.ReadKey(True)

        _logFile.Close()
    End Sub

    Public Sub PresentRealmList(worldServerList As WorldServerList) Implements IGameUI.PresentRealmList
        Dim selectedServer As WorldServerInfo = Nothing

        If worldServerList.Count = 1 Then
            selectedServer = worldServerList(0)
        Else
            LogLine(vbLf & vbTab & "Name" & vbTab & "Type" & vbTab & "Population")

            Dim index As Integer = 0
            For Each server As WorldServerInfo In worldServerList
                LogLine(String.Format("{0}" & vbTab & "{1}" & vbTab & "{2}" & vbTab & "{3}", System.Math.Max(System.Threading.Interlocked.Increment(index), index - 1), server.Name, server.Type, server.Population))
            Next

            ' select a realm - default to the first realm if there is only one
            index = If(worldServerList.Count = 1, 0, -1)
            While index > worldServerList.Count OrElse index < 0
                Log("Choose a realm:  ")
                If Not Integer.TryParse(Console.ReadLine(), index) Then
                    LogLine()
                End If
            End While
            selectedServer = worldServerList(index)
        End If

        Game.ConnectTo(selectedServer)
    End Sub

    Public Sub PresentCharacterList(characterList As Character()) Implements IGameUI.PresentCharacterList
        LogLine(vbLf & vbTab & "Name" & vbTab & "Level Class Race")

        Dim index As Integer = 0
        For Each character As Character In characterList
            LogLine(String.Format("{4}" & vbTab & "{0}" & vbTab & "{1} {2} {3}", character.Name, character.Level, character.Race, character.[Class], System.Math.Max(System.Threading.Interlocked.Increment(index), index - 1)))
        Next

        If characterList.Length < 10 Then
            LogLine(String.Format("{0}" & vbTab & "Create a new character. (NOT YET IMPLEMENTED)", index))
        End If

        Dim length As Integer = If(characterList.Length = 10, 10, (characterList.Length + 1))
        index = -1
        While index > length OrElse index < 0
            Log("Choose a character:  ")
            If Not Integer.TryParse(Console.ReadLine(), index) Then
                LogLine()
            End If
        End While

        If index < characterList.Length Then
            Game.World.SelectedCharacter = characterList(index)
            ' TODO: enter world

            LogLine(String.Format("Entering pseudo-world with character {0}", Game.World.SelectedCharacter.Name))

            Dim packet As New OutPacket(WorldCommand.CMSG_PLAYER_LOGIN)
            packet.Write(Game.World.SelectedCharacter.GUID)
            Game.SendPacket(packet)
            ' TODO: character creation
        Else
        End If
    End Sub

    Public Sub PresentChatMessage(message As ChatMessage) Implements IGameUI.PresentChatMessage
        Dim sb As New StringBuilder()
        sb.Append(If(message.Sender.Type = ChatMessageType.WhisperInform, "To: ", message.Sender.Type.ToString()))
        '! Color codes taken from default chat_cache in WTF folder
        '! TODO: RTF form?
        Select Case message.Sender.Type
            Case ChatMessageType.Channel
                If True Then
                    'sb.ForeColor(Color.FromArgb(255, 192, 192));
                    Console.ForegroundColor = ConsoleColor.DarkYellow
                    'Color.DarkSalmon;
                    sb.Append(" [")
                    sb.Append(message.Sender.ChannelName)
                    sb.Append("] ")
                    Exit Select
                End If
            Case ChatMessageType.Whisper
                Game.World.LastWhisperers.Enqueue(message.Sender.Sender)
                'goto case ChatMessageType.WhisperInform
                Console.ForegroundColor = ConsoleColor.Magenta
            Case ChatMessageType.WhisperInform, ChatMessageType.WhisperForeign
                'sb.ForeColor(Color.FromArgb(255, 128, 255));
                Console.ForegroundColor = ConsoleColor.Magenta
                'Color.DeepPink;
                Exit Select
            Case ChatMessageType.System, ChatMessageType.Money, ChatMessageType.TargetIcons, ChatMessageType.Achievement
                'sb.ForeColor(Color.FromArgb(255, 255, 0));
                Console.ForegroundColor = ConsoleColor.Yellow
                'Color.Gold;
                Exit Select
            Case ChatMessageType.Emote, ChatMessageType.TextEmote, ChatMessageType.MonsterEmote
                'sb.ForeColor(Color.FromArgb(255, 128, 64));
                Console.ForegroundColor = ConsoleColor.DarkRed
                'Color.Chocolate;
                Exit Select
            Case ChatMessageType.Party
                'sb.ForeColor(Color.FromArgb(170, 170, 255));
                Console.ForegroundColor = ConsoleColor.DarkCyan
                'Color.CornflowerBlue;
                Exit Select
            Case ChatMessageType.PartyLeader
                'sb.ForeColor(Color.FromArgb(118, 200, 255));
                Console.ForegroundColor = ConsoleColor.Cyan
                'Color.DodgerBlue;
                Exit Select
            Case ChatMessageType.Raid
                'sb.ForeColor(Color.FromArgb(255, 172, 0));
                Console.ForegroundColor = ConsoleColor.Red
                'Color.DarkOrange;
                Exit Select
            Case ChatMessageType.RaidLeader
                'sb.ForeColor(Color.FromArgb(255, 72, 9));
                Console.ForegroundColor = ConsoleColor.Red
                'Color.DarkOrange;
                Exit Select
            Case ChatMessageType.RaidWarning
                'sb.ForeColor(Color.FromArgb(255, 72, 0));
                Console.ForegroundColor = ConsoleColor.Red
                'Color.DarkOrange;
                Exit Select
            Case ChatMessageType.Guild, ChatMessageType.GuildAchievement
                'sb.ForeColor(Color.FromArgb(64, 255, 64));
                Console.ForegroundColor = ConsoleColor.Green
                'Color.LimeGreen;
                Exit Select
            Case ChatMessageType.Officer
                'sb.ForeColor(Color.FromArgb(64, 192, 64));
                Console.ForegroundColor = ConsoleColor.DarkGreen
                'Color.DarkSeaGreen;
                Exit Select
            Case ChatMessageType.Yell
                Console.ForegroundColor = ConsoleColor.DarkRed
                Exit Select
            Case ChatMessageType.Say
                'sb.ForeColor(Color.FromArgb(255, 255, 255));
                Console.ForegroundColor = ConsoleColor.White
                Exit Select
            Case Else
                Console.ForegroundColor = ConsoleColor.White
        End Select

        sb.Append("[")
        If message.ChatTag.HasFlag(ChatTag.Gm) Then
            sb.Append("<GM>")
        End If
        If message.ChatTag.HasFlag(ChatTag.Afk) Then
            sb.Append("<AFK>")
        End If
        If message.ChatTag.HasFlag(ChatTag.Dnd) Then
            sb.Append("<DND>")
        End If
        sb.Append(message.Sender.Sender)
        sb.Append("]: ")
        sb.Append(message.Message)

        LogLine(sb.ToString(), If(message.Language = Language.Addon, LogLevel.Debug, LogLevel.Info))
    End Sub

    Public Sub Log(message As String, Optional level As LogLevel = LogLevel.Info) Implements IGameUI.Log
        SyncLock Console.Out
            If level >= LogLevel Then
                Console.Write(message)
                _logFile.Write(message)
                Console.ResetColor()
            End If
        End SyncLock
    End Sub

    Public Sub LogLine(Optional level As LogLevel = LogLevel.Info)
        SyncLock Console.Out
            If level >= LogLevel Then
                Console.WriteLine()
                _logFile.WriteLine()
                Console.ResetColor()
            End If
        End SyncLock
    End Sub

    Public Sub LogLine(message As String, Optional level As LogLevel = LogLevel.Info) Implements IGameUI.LogLine
        SyncLock Console.Out
            If level >= LogLevel Then
                Console.WriteLine(message)
                _logFile.WriteLine(message)
                Console.ResetColor()
            End If
        End SyncLock
    End Sub

    Public Sub LogException(message As String) Implements IGameUI.LogException
        _logFile.WriteLine([String].Format("Exception: {0}", message))
        _logFile.WriteLine((New StackTrace(1, True)).ToString())
        Throw New Exception(message)
    End Sub

    Public Function ReadLine() As String Implements IGameUI.ReadLine
        '! We don't want to clutter the console, so wait for input before printing output
        Dim ret As String
        SyncLock Console.Out
            ret = Console.ReadLine()
        End SyncLock

        Return ret
    End Function

    Public Function Read() As Integer Implements IGameUI.Read
        '! We don't want to clutter the console, so wait for input before printing output
        Dim ret As Integer
        SyncLock Console.Out
            ret = Console.Read()
        End SyncLock

        Return ret
    End Function

    Public Function ReadKey() As ConsoleKeyInfo Implements IGameUI.ReadKey
        '! We don't want to clutter the console, so wait for input before printing output
        Dim ret As ConsoleKeyInfo
        SyncLock Console.Out
            ret = Console.ReadKey()
        End SyncLock

        Return ret
    End Function

#End Region
End Class
