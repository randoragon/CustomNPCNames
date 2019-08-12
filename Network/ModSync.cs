using Terraria;
using Terraria.ID;

namespace CustomNPCNames.Network
{
    class ModSync : SyncType
    {
        /// <summary>
        /// ModWorld.NetSend() does not have formal parameters for extra things, so this static variable will be accessed by it to relay information about the NPCID to sync instead (optional only for some Sync Types).
        /// </summary>
        public static short ID { get; private set; }

        static ModSync()
        {
            ID = 0;
        }

        /// <summary>
        /// Prompts the server to synchronize its current information across all client machines. See Network.SyncType for different syncing options.
        /// </summary>
        /// <param name="type">Network.SyncType</param>
        /// <param name="id">Optional NPCID parameter for NPC-dependent Sync Types</param>
        public static void SyncWorldData(byte type, short id = 0)
        {
            if (Main.netMode == NetmodeID.Server) {
                Get = type;
                ID = id;
                NetMessage.SendData(MessageID.WorldData);
                Get = EVERYTHING;
            } else if (Main.netMode == NetmodeID.MultiplayerClient) {
                PacketSender.SendPacketToServer(PacketType.REQUEST_WORLD_SYNC, type, (ulong)id);
            }
        }
    }

    class SyncType
    {
        /// <summary>
        /// ModWorld.NetSend() does not have formal parameters for extra things, so this static variable will be accessed by it to relay information about the current Sync Type instead.
        /// </summary>
        public static byte Get { get; protected set; }

        static SyncType()
        {
            Get = EVERYTHING;
        }

        public const byte MODE          = 0;
        public const byte TRY_UNIQUE    = 1;
        public const byte NAME          = 2;
        public const byte GENDER        = 3;
        public const byte CUSTOM_NAMES  = 4;
        public const byte EVERYTHING    = 5;
        public const byte BUSY_FIELDS   = 6;
    }
}
