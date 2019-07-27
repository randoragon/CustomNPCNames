using Terraria;
using Terraria.ModLoader;

namespace CustomNPCNames.Network
{
    class PacketSender
    {
        public static void SendPacketToServer(byte type)
        {
            // In order to send full string dictionaries with packets, you need to pay attention to the packet size.
            // For speed reasons it's better to send more small packets then one big packet, so i'll stick to the max size of 256 bytes.
            // Each name string has a maximum length of 25 characters, each of which is 8 bits, so 1 byte long.
            // Add 4 bytes of data for an int representing the length of the string, you get a grand total of 25 + 4 = 29 bytes. Rounding up to 30 for safety reasons.
            // The max packet size being 256 bytes gives us floor(256/30)=8 strings per one sent packet.
            ModPacket packet;
            switch (type) {
                case PacketType.MODE:
                    packet = CustomNPCNames.instance.GetPacket(2);
                    packet.Write(PacketType.MODE);
                    packet.Write(CustomWorld.mode);
                    packet.Send();
                    break;
                case PacketType.TRY_UNIQUE:
                    packet = CustomNPCNames.instance.GetPacket(2);
                    packet.Write(PacketType.TRY_UNIQUE);
                    packet.Write(CustomWorld.tryUnique ? (byte)1 : (byte)0);
                    packet.Send();
                    break;
                case PacketType.CURRENT_NAMES:


                    break;
                case PacketType.IS_MALE:
                    packet = CustomNPCNames.instance.GetPacket(4); // 24 boolean values = 3 bytes of data (+1 for packet type)
                    packet.Write(PacketType.IS_MALE);
                    var bits = new BitsByte();
                    for (int i = 0; i < 8; i++) {
                        bits[i] = NPCs.CustomNPC.isMale[(short)i];
                    }
                    packet.Write(bits);
                    for (int i = 0; i < 8; i++) {
                        bits[i] = NPCs.CustomNPC.isMale[(short)(8 + i)];
                    }
                    packet.Write(bits);
                    for (int i = 0; i < 8; i++) {
                        bits[i] = NPCs.CustomNPC.isMale[(short)(16 + i)];
                    }
                    packet.Write(bits);
                    packet.Send();
                    break;
            }

            Main.NewText(string.Format("Sending Packets({0})! ", type) + Main.time);
        }
    }

    // Each packet will be preceded with an ID number which represents what type of packet it is. Without an ID, the server wouldn't know what to do with the received packets, wouldn't it?
    class PacketType
    {
        public const byte MODE = 0;
        public const byte TRY_UNIQUE = 1;
        public const byte CURRENT_NAMES = 2;
        public const byte IS_MALE = 3;
        public const byte CUSTOM_NAMES = 4;
        public const byte EVERYTHING = 5;
    }
}
