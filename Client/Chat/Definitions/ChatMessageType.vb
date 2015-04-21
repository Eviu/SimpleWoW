Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Chat.Definitions
	Public Enum ChatMessageType As UInteger
		System = 0
		Say = 1
		Party = 2
		Raid = 3
		Guild = 4
		Officer = 5
		Yell = 6
		Whisper = 7
		WhisperForeign = 8
		WhisperInform = 9
		Emote = 10
		TextEmote = 11
		MonsterSay = 12
		MonsterParty = 13
		MonsterYell = 14
		MonsterWhisper = 15
		MonsterEmote = 16
		Channel = 17
		ChannelJoin = 18
		ChannelLeave = 19
		ChannelList = 20
		ChannelNotice = 21
		ChannelNoticeUser = 22
		Afk = 23
		Dnd = 24
		Ignored = 25
		Skill = 26
		Loot = 27
		Money = 28
		Opening = 29
		Tradeskills = 30
		PetInfo = 31
		CombatMiscInfo = 32
		CombatXPGain = 33
		CombatHonorGain = 34
		CombatFactionChange = 35
		BattlegroundNeutral = 36
		BattlegroundAlliance = 37
		BattlegroundHorde = 38
		RaidLeader = 39
		RaidWarning = 40
		RaidBossEmote = 41
		RaidBossWhisper = 42
		Filtered = 43
		Battleground = 44
		BattlegroundLeader = 45
		Restricted = 46
		BattleNet = 47
		Achievement = 48
		GuildAchievement = 49
		ArenaPoints = 50
		PartyLeader = 51
		TargetIcons = 52
		'! Some random BN crap here
		Addon = UInteger.MaxValue
	End Enum
End Namespace