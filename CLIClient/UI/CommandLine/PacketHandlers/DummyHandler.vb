Imports Client.World
Imports Client.World.Network

Partial Class CommandLineUI
	'! Ignore the packet.
	<PacketHandler(WorldCommand.SMSG_POWER_UPDATE)> _
	<PacketHandler(WorldCommand.SMSG_SET_PROFICIENCY)> _
	<PacketHandler(WorldCommand.MSG_SET_DUNGEON_DIFFICULTY)> _
	<PacketHandler(WorldCommand.SMSG_LOGIN_VERIFY_WORLD)> _
	<PacketHandler(WorldCommand.SMSG_ACCOUNT_DATA_TIMES)> _
	<PacketHandler(WorldCommand.SMSG_FEATURE_SYSTEM_STATUS)> _
	<PacketHandler(WorldCommand.SMSG_LEARNED_DANCE_MOVES)> _
	<PacketHandler(WorldCommand.SMSG_BINDPOINTUPDATE)> _
	<PacketHandler(WorldCommand.SMSG_TALENTS_INFO)> _
	<PacketHandler(WorldCommand.SMSG_INSTANCE_DIFFICULTY)> _
	<PacketHandler(WorldCommand.SMSG_INITIAL_SPELLS)> _
	<PacketHandler(WorldCommand.SMSG_SEND_UNLEARN_SPELLS)> _
	<PacketHandler(WorldCommand.SMSG_ACTION_BUTTONS)> _
	<PacketHandler(WorldCommand.SMSG_INITIALIZE_FACTIONS)> _
	<PacketHandler(WorldCommand.SMSG_ALL_ACHIEVEMENT_DATA)> _
	<PacketHandler(WorldCommand.SMSG_EQUIPMENT_SET_LIST)> _
	<PacketHandler(WorldCommand.SMSG_LOGIN_SETTIMESPEED)> _
	<PacketHandler(WorldCommand.SMSG_SET_FORCED_REACTIONS)> _
	<PacketHandler(WorldCommand.SMSG_COMPRESSED_UPDATE_OBJECT)> _
	<PacketHandler(WorldCommand.SMSG_MONSTER_MOVE)> _
	Private Sub HandleDummyPacket(packet As InPacket)

	End Sub
End Class

