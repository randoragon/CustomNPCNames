using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using System.Linq;

namespace CustomNPCNames.Network
{
    class PacketSender : PacketType
    {
        /// <param name="type">The type of packet to send to the server (Network.PacketType)</param>
        /// <param name="id">Optional parameter for packet types that require an NPC to act upon.</param>
        /// <param name="id1">Optional parameter for name list operations.</param>
        /// <param name="name">Optional parameter for name list operations.</param>
        public static void SendPacketToServer(byte type, short id = NPCID.None, ulong id1 = 0, string name = "")
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                // In order to send full string dictionaries with packets, you need to pay attention to the packet size.
                // For speed reasons it's better to send more small packets then one big packet, so i'll stick to the max size of 256 bytes.
                // Each name string has a maximum length of 25 characters, each of which is 8 bits, so 1 byte long.
                // Add 4 bytes of data for an int representing the length of the string, you get a grand total of 25 + 4 = 29 bytes. Rounding up to 30 for safety reasons.
                // The max packet size being 256 bytes gives us floor(256/30)=8 strings per one sent packet (there's a few bytes of leeway left for minor additional information).
                ModPacket packet;
                switch (type) {
                    default:
                        packet = CustomNPCNames.instance.GetPacket();
                        packet.Write(type);
                        packet.Write(id);
                        packet.Send();
                        break;
                    case SEND_NAME:
                        packet = CustomNPCNames.instance.GetPacket();
                        packet.Write(type);
                        packet.Write(id);
                        packet.Write(name);
                        packet.Send();
                        break;
                    case SEND_CUSTOM_NAMES: // a maximum of 8 names will be sent per packet, see explanation starting at line 15
                        if (CustomWorld.CustomNames[id].Count == 0) {
                            packet = CustomNPCNames.instance.GetPacket();
                            packet.Write(SEND_CUSTOM_NAMES);
                            packet.Write(id);
                            packet.Write(-1);
                            packet.Send();
                        } else {
                            for (int i = 0; i < CustomWorld.CustomNames[id].Count; i += 8) {
                                packet = CustomNPCNames.instance.GetPacket();
                                packet.Write(SEND_CUSTOM_NAMES);
                                packet.Write(id);
                                packet.Write(i);
                                int count = System.Math.Min(8, CustomWorld.CustomNames[id].Count - i);
                                packet.Write((byte)count);
                                packet.Write(i + 8 >= CustomWorld.CustomNames[id].Count);
                                for (int j = i; j < i + count; j++) {
                                    packet.Write(CustomWorld.CustomNames[id][j].ToString());
                                    packet.Write(CustomWorld.CustomNames[id][j].ID);
                                }
                                packet.Send();
                            }
                        }
                        break;
                    case SEND_COPY_DATA: {
                            // mode and tryUnique
                            packet = CustomNPCNames.instance.GetPacket();
                            packet.Write(SEND_COPY_DATA);
                            packet.Write(UI.RenameUI.copyData.mode);
                            packet.Write(UI.RenameUI.copyData.tryUnique);
                            var bits = new BitsByte();
                            for (int i = 0; i < 3; i++) {
                                for (int j = 0; j < 8; j++) {
                                    bits[j] = UI.RenameUI.copyData.isMale[CustomNPCNames.TownNPCs[(short)((i * 8) + j)]];
                                }
                                Main.NewText(string.Format("Writing byte{0}={1}...", i + 1, bits.ToString()));
                                packet.Write(bits);
                            }
                            Main.NewText("Sending!");
                            packet.Send();

                            // CustomNames
                            foreach (short i in CustomNPCNames.TownNPCs) {
                                SendPacketToServer(SEND_COPY_NAMES, i);
                            }
                            SendPacketToServer(SEND_COPY_NAMES, 1000); // male
                            SendPacketToServer(SEND_COPY_NAMES, 1001); // female
                            SendPacketToServer(SEND_COPY_NAMES, 1002); // global
                        }
                        break;
                    case SEND_COPY_NAMES: // a maximum of 8 names will be sent per packet, see explanation starting at line 15
                        if (UI.RenameUI.copyData.customNames[id].Count == 0) {
                            packet = CustomNPCNames.instance.GetPacket();
                            packet.Write(SEND_COPY_NAMES);
                            packet.Write(id);
                            packet.Write(-1);
                            packet.Send();
                        } else {
                            for (int i = 0; i < UI.RenameUI.copyData.customNames[id].Count; i += 8) {
                                packet = CustomNPCNames.instance.GetPacket();
                                packet.Write(SEND_COPY_NAMES);
                                packet.Write(id);
                                packet.Write(i);
                                int count = System.Math.Min(8, UI.RenameUI.copyData.customNames[id].Count - i);
                                packet.Write((byte)count);
                                packet.Write(i + 8 >= UI.RenameUI.copyData.customNames[id].Count);
                                for (int j = i; j < i + count; j++) {
                                    packet.Write(UI.RenameUI.copyData.customNames[id][j].ToString());
                                    packet.Write(UI.RenameUI.copyData.customNames[id][j].ID);
                                }
                                packet.Send();
                            }
                        }
                        break;
                    case ADD_NAME:
                        packet = CustomNPCNames.instance.GetPacket();
                        packet.Write(ADD_NAME);
                        packet.Write(id);
                        packet.Write(name);
                        packet.Write(id1);
                        packet.Send();
                        break;
                    case EDIT_NAME:
                        packet = CustomNPCNames.instance.GetPacket();
                        packet.Write(EDIT_NAME);
                        packet.Write(id);
                        packet.Write(name);
                        packet.Write(id1);
                        packet.Send();
                        break;
                    case REMOVE_NAME:
                        packet = CustomNPCNames.instance.GetPacket();
                        packet.Write(REMOVE_NAME);
                        packet.Write(id);
                        packet.Write(id1);
                        packet.Send();
                        break;
                    case REQUEST_WORLD_SYNC:
                        packet = CustomNPCNames.instance.GetPacket();
                        packet.Write(REQUEST_WORLD_SYNC);
                        packet.Write(id);
                        packet.Write(id1);
                        packet.Send();
                        break;
                }
                Main.NewText(string.Format("Sending Packets({0})! ", type) + Main.time);
            } else {
                NetMessage.BroadcastChatMessage(Terraria.Localization.NetworkText.FromLiteral("Trying to send packages from server to server!"), Microsoft.Xna.Framework.Color.White);
            }
        }
    }

    // Each packet will be preceded with an ID number which represents what type of packet it is. Without an ID, the server wouldn't know what to do with the received packets, wouldn't it?
    class PacketType
    {
        public const byte NEXT_MODE          = 0;  // used when left-clicking the Mode button
        public const byte PREV_MODE          = 1;  // used when right-clicking the Mode button
        public const byte TOGGLE_TRY_UNIQUE  = 2;  // used when Toggle Unique Names button is pressed
        public const byte SEND_NAME          = 3;  // used when a client updates an NPC's name
        public const byte SWITCH_GENDER      = 4;  // used when Switch Gender button is pressed
        public const byte SEND_CUSTOM_NAMES  = 5;  // used in SEND_COPY_DATA
        public const byte SEND_COPY_DATA     = 6;  // used when pasting everything
        public const byte SEND_COPY_NAMES    = 7;  // used when pasting everything
        public const byte RANDOMIZE          = 8;  // used when clicking the Randomize button
        public const byte ADD_NAME           = 9;  // used when adding a name field
        public const byte REMOVE_NAME        = 10;  // used when removing a name field
        public const byte EDIT_NAME          = 11; // used when editing a name field
        public const byte REQUEST_WORLD_SYNC = 12; // used for syncing from the MultiplayerClient (see ModSync class)
    }
}
