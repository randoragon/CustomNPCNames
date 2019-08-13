using System.Collections.Generic;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.UI;
using Microsoft.Xna.Framework;
using CustomNPCNames.UI;
using CustomNPCNames.Network;
using Terraria.Localization;


namespace CustomNPCNames
{
    public class CustomNPCNames : Mod
    {
        public static CustomNPCNames instance;
        public static readonly short[] TownNPCs = {
            NPCID.Guide,         NPCID.Merchant,         NPCID.Nurse,
            NPCID.Demolitionist, NPCID.DyeTrader,        NPCID.Dryad,
            NPCID.DD2Bartender,  NPCID.ArmsDealer,       NPCID.Stylist,
            NPCID.Painter,       NPCID.Angler,           NPCID.GoblinTinkerer,
            NPCID.WitchDoctor,   NPCID.Clothier,         NPCID.Mechanic,
            NPCID.PartyGirl,     NPCID.Wizard,           NPCID.TaxCollector,
            NPCID.Truffle,       NPCID.Pirate,           NPCID.Steampunker,
            NPCID.Cyborg,        NPCID.SkeletonMerchant, NPCID.TravellingMerchant
        };
        public static readonly Dictionary<short, Vector2> npcHeadOffset = new Dictionary<short, Vector2>() {
            { NPCID.Guide,              new Vector2(0, 0) },
            { NPCID.Merchant,           new Vector2(3, 3) },
            { NPCID.Nurse,              new Vector2(0, 0) },
            { NPCID.Demolitionist,      new Vector2(0, 0) },
            { NPCID.DyeTrader,          new Vector2(0, 0) },
            { NPCID.Dryad,              new Vector2(0, 0) },
            { NPCID.DD2Bartender,       new Vector2(-2, 0)},
            { NPCID.ArmsDealer,         new Vector2(0, 0) },
            { NPCID.Stylist,            new Vector2(0, -1)},
            { NPCID.Painter,            new Vector2(-2, 0)},
            { NPCID.Angler,             new Vector2(0, 0) },
            { NPCID.GoblinTinkerer,     new Vector2(-2, 0)},
            { NPCID.WitchDoctor,        new Vector2(0, 0) },
            { NPCID.Clothier,           new Vector2(-1, 0)},
            { NPCID.Mechanic,           new Vector2(0, 0) },
            { NPCID.PartyGirl,          new Vector2(-1, 0)},
            { NPCID.Wizard,             new Vector2(0, 0) },
            { NPCID.TaxCollector,       new Vector2(0, 0) },
            { NPCID.Truffle,            new Vector2(0, 0) },
            { NPCID.Pirate,             new Vector2(0, 0) },
            { NPCID.Steampunker,        new Vector2(0, 0) },
            { NPCID.Cyborg,             new Vector2(0, 0) },
            { NPCID.SkeletonMerchant,   new Vector2(0, 0) },
            { NPCID.TravellingMerchant, new Vector2(2, -3)},
            { 1000,                     new Vector2(0, 0) }, // male
            { 1001,                     new Vector2(0, 0) }, // female
            { 1002,                     new Vector2(0, 0) }  // global
        };
        public static ModHotKey RenameMenuHotkey;
        public static RenameUI renameUI;
        private static UserInterface renameInterface;
        private static bool _waitForServerResponse = false; // used in situations when it's critical not to change any mod data on the client until some server response comes
        private static int _serverResponseTimeout = -1;
        public static bool WaitForServerResponse
        {
            get { return Main.netMode == NetmodeID.MultiplayerClient ? _waitForServerResponse : false; }
            set { _waitForServerResponse = value; _serverResponseTimeout = value ? 600 : -1; } // if no response comes within 10 seconds, assume the packets are lost
        }

        public override void Load()
        {
            instance = this;
            Config.Load();
            // this makes sure that the UI doesn't get opened on the server console
            if (!Main.dedServ) {
                RenameMenuHotkey = RegisterHotKey("Toggle Menu", "K");
                renameUI = new RenameUI();
                renameUI.Initialize();
                renameInterface = new UserInterface();
                renameInterface.SetState(renameUI);
            }
        }

        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            if (_serverResponseTimeout != -1 && --_serverResponseTimeout == 0) {
                _waitForServerResponse = false;
                Main.NewText("Server Response Timeout. Either your internet connection is insufficient, or this is a bug.", Color.OrangeRed);
            }
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            byte type = reader.ReadByte();
            if (Main.myPlayer == 255) { // if the server is the recipient
                switch (type) {
                    case PacketType.NEXT_MODE: {
                            CustomWorld.mode = (byte)(++CustomWorld.mode % 4);
                            string curModeName;
                            switch (CustomWorld.mode) {
                                case 0: curModeName = "Vanilla"; break;
                                case 1: curModeName = "Custom"; break;
                                case 2: curModeName = "Gender"; break;
                                case 3: curModeName = "Global"; break;
                                default: curModeName = "ERROR! Please report this!"; break;
                            }
                            Broadcaster.SendGlobalMessage(Broadcaster.MessageType.SWITCH_MODE, string.Format("[c/9DFF66:{0}] changed Mode to {1}", Main.player[whoAmI].name, curModeName));
                            ModSync.SyncWorldData(SyncType.MODE);
                        }
                        break;
                    case PacketType.PREV_MODE: {
                            CustomWorld.mode = (byte)(--CustomWorld.mode % 4);
                            string curModeName;
                            switch (CustomWorld.mode) {
                                case 0: curModeName = "Vanilla"; break;
                                case 1: curModeName = "Custom"; break;
                                case 2: curModeName = "Gender"; break;
                                case 3: curModeName = "Global"; break;
                                default: curModeName = "ERROR! Please report this!"; break;
                            }
                            Broadcaster.SendGlobalMessage(Broadcaster.MessageType.SWITCH_MODE, string.Format("[c/9DFF66:{0}] changed Mode to {1}", Main.player[whoAmI].name, curModeName));
                            ModSync.SyncWorldData(SyncType.MODE);
                        }
                        break;
                    case PacketType.TOGGLE_TRY_UNIQUE:
                        CustomWorld.tryUnique = !CustomWorld.tryUnique;
                        Broadcaster.SendGlobalMessage(Broadcaster.MessageType.TOGGLE_UNIQUE, string.Format("[c/9DFF66:{0}] {1} Unique Names", Main.player[whoAmI].name, CustomWorld.tryUnique ? "enabled" : "disabled"));
                        ModSync.SyncWorldData(SyncType.TRY_UNIQUE);
                        break;
                    case PacketType.SWITCH_GENDER: {
                            short id = reader.ReadInt16();
                            NPCs.CustomNPC.isMale[id] = !NPCs.CustomNPC.isMale[id];
                            Broadcaster.SendGlobalMessage(Broadcaster.MessageType.SWITCH_GENDER, string.Format("[c/9DFF66:{0}] changed the {1}'s gender to {2}", Main.player[whoAmI].name, GetNPCName(id), NPCs.CustomNPC.isMale[id] ? "male" : "female"));
                            ModSync.SyncWorldData(SyncType.GENDER, id);
                        }
                        break;
                    case PacketType.SEND_NAME: {
                            short id = reader.ReadInt16();
                            string name = reader.ReadString();
                            NPC npc = NPCs.CustomNPC.FindFirstNPC(id);
                            if (npc != null) {
                                npc.GivenName = name;
                                Broadcaster.SendGlobalMessage(Broadcaster.MessageType.EDIT_NAME, string.Format("[c/9DFF66:{0}] changed the {1}'s name to [c/FFFF00:\"{2}\"]", Main.player[whoAmI].name, GetNPCName(id), name));
                                ModSync.SyncWorldData(SyncType.NAME, id);
                            }
                        }
                        break;
                    case PacketType.SEND_CUSTOM_NAMES: {
                            short id = reader.ReadInt16();
                            int index = reader.ReadInt32();
                            if (index != -1) {
                                byte offset = reader.ReadByte();
                                bool lastPacket = reader.ReadBoolean();
                                while (CustomWorld.CustomNames[id].Count < index + offset) {
                                    CustomWorld.CustomNames[id].Add(null);
                                }
                                if (lastPacket) {
                                    int overshoot = CustomWorld.CustomNames[id].Count - (index + offset);
                                    if (overshoot > 0) {
                                        CustomWorld.CustomNames[id].RemoveRange(index + offset, overshoot);
                                    }
                                }
                                for (int i = index; i < index + offset; i++) {
                                    string name = reader.ReadString();
                                    ulong nameId = reader.ReadUInt64();
                                    CustomWorld.CustomNames[id][i] = new StringWrapper(ref name, nameId);
                                }

                                if (lastPacket) { ModSync.SyncWorldData(SyncType.CUSTOM_NAMES, id); }
                            } else {
                                // only clear if there are no busy entries in the current tab
                                bool noBusyEntries = true;
                                foreach (StringWrapper i in CustomWorld.CustomNames[id]) {
                                    foreach (BusyField j in CustomWorld.busyFields) {
                                        if (i.ID == j.ID) { noBusyEntries = false; goto Break; }
                                    }
                                }

                                Break:
                                if (noBusyEntries) {
                                    CustomWorld.CustomNames[id].Clear();
                                    switch (id) {
                                        case 1000:
                                            Broadcaster.SendGlobalMessage(Broadcaster.MessageType.CLEAR_NAME_LIST, string.Format("[c/9DFF66:{0}] cleared the Male name list!", Main.player[whoAmI].name));
                                            break;
                                        case 1001:
                                            Broadcaster.SendGlobalMessage(Broadcaster.MessageType.CLEAR_NAME_LIST, string.Format("[c/9DFF66:{0}] cleared the Female name list!", Main.player[whoAmI].name));
                                            break;
                                        case 1002:
                                            Broadcaster.SendGlobalMessage(Broadcaster.MessageType.CLEAR_NAME_LIST, string.Format("[c/9DFF66:{0}] cleared the Global name list!", Main.player[whoAmI].name));
                                            break;
                                        default:
                                            Broadcaster.SendGlobalMessage(Broadcaster.MessageType.CLEAR_NAME_LIST, string.Format("[c/9DFF66:{0}] cleared the {1}'s name list!", Main.player[whoAmI].name, GetNPCName(id)));
                                            break;
                                    }
                                    ModSync.SyncWorldData(SyncType.CUSTOM_NAMES, id);
                                } else {
                                    var packet = instance.GetPacket();
                                    packet.Write(PacketType.SERVER_REJECT_CLEAR_ALL);
                                    packet.Send(whoAmI);
                                }
                            }
                        }
                        break;
                    case PacketType.SEND_COPY_DATA: {
                            // Only accept a paste if no custom names entries are busy and no other paste operations are happenings
                            bool anyBusyCustomNames = false;
                            foreach (BusyField i in CustomWorld.busyFields) {
                                foreach (short j in TownNPCs) {
                                    if (i.ID == (ulong)j) { anyBusyCustomNames = true; goto Break; }
                                }
                            }

                            Break:
                            if (CustomWorld.packetsTillSync == 0 && !anyBusyCustomNames) {
                                int packetCount = reader.ReadInt32();
                                CustomWorld.receivedPackets = 0;
                                CustomWorld.packetsTillSync = packetCount;
                                var packet = instance.GetPacket();
                                packet.Write(PacketType.SERVER_AWAITING_COPY_DATA);
                                packet.Send(whoAmI);
                            } else {
                                var packet = instance.GetPacket();
                                packet.Write(PacketType.SERVER_REJECT_COPY_DATA);
                                packet.Send(whoAmI);
                            }
                        }
                        break;
                    case PacketType.SEND_COPY_MODE_TRYUNIQUE_ISMALE: {
                            CustomWorld.mode = reader.ReadByte();
                            CustomWorld.tryUnique = reader.ReadBoolean();
                            for (int i = 0; i < 3; i++) {
                                BitsByte b = reader.ReadByte();
                                for (int j = 0; j < 8; j++) {
                                    NPCs.CustomNPC.isMale[TownNPCs[(short)((8 * i) + j)]] = b[j];
                                }
                            }
                            if (++CustomWorld.receivedPackets == CustomWorld.packetsTillSync) {
                                CustomWorld.packetsTillSync = 0;
                                Broadcaster.SendGlobalMessage(Broadcaster.MessageType.PASTE_EVERYTHING, string.Format("[c/9DFF66:{0}] pasted everything into the world!", Main.player[whoAmI].name));
                                ModSync.SyncWorldData(SyncType.EVERYTHING);
                            }
                        }
                        break;
                    case PacketType.SEND_COPY_NAMES: {
                            short id = reader.ReadInt16();
                            int index = reader.ReadInt32();
                            if (index != -1) {
                                byte offset = reader.ReadByte();
                                bool lastPacket = reader.ReadBoolean();
                                while (CustomWorld.CustomNames[id].Count < index + offset) {
                                    CustomWorld.CustomNames[id].Add(null);
                                }
                                if (lastPacket) {
                                    int overshoot = CustomWorld.CustomNames[id].Count - (index + offset);
                                    if (overshoot > 0) {
                                        CustomWorld.CustomNames[id].RemoveRange(index + offset, overshoot);
                                    }
                                }
                                for (int i = index; i < index + offset; i++) {
                                    string name = reader.ReadString();
                                    ulong nameId = reader.ReadUInt64();
                                    CustomWorld.CustomNames[id][i] = new StringWrapper(ref name, nameId);
                                }
                            } else {
                                CustomWorld.CustomNames[id].Clear();
                            }
                            if (++CustomWorld.receivedPackets == CustomWorld.packetsTillSync) {
                                CustomWorld.packetsTillSync = 0;
                                Broadcaster.SendGlobalMessage(Broadcaster.MessageType.PASTE_EVERYTHING, string.Format("[c/9DFF66:{0}] pasted everything into the world!", Main.player[whoAmI].name));
                                ModSync.SyncWorldData(SyncType.EVERYTHING);
                            }
                        }
                        break;
                    case PacketType.RANDOMIZE: {
                            short id = reader.ReadInt16();
                            // Only randomize if no one else is editing the name
                            bool isOnBusyList = false;
                            if (id != 1000 && id != 1001 && id != 1002) {
                                foreach (BusyField i in CustomWorld.busyFields) {
                                    if (i.ID == (ulong)id) { isOnBusyList = true; }
                                }
                            } else if (id == 1000) {
                                foreach (short i in TownNPCs) {
                                    if (NPCs.CustomNPC.isMale[i]) {
                                        foreach (BusyField j in CustomWorld.busyFields) {
                                            if (j.ID == (ulong)i) { isOnBusyList = true; goto Break; }
                                        }
                                    }
                                }
                            } else if (id == 1001) {
                                foreach (short i in TownNPCs) {
                                    if (!NPCs.CustomNPC.isMale[i]) {
                                        foreach (BusyField j in CustomWorld.busyFields) {
                                            if (j.ID == (ulong)i) { isOnBusyList = true; goto Break; }
                                        }
                                    }
                                }
                            } else if (id == 1000) {
                                foreach (short i in TownNPCs) {
                                    foreach (BusyField j in CustomWorld.busyFields) {
                                        if (j.ID == (ulong)i) { isOnBusyList = true; goto Break; }
                                    }
                                }
                            }

                            Break:
                            if (!isOnBusyList) {
                                NPCs.CustomNPC.RandomizeName(id);
                                // world sync is called from the RandomizeName method
                                switch (id) {
                                    case 1000:
                                        Broadcaster.SendGlobalMessage(Broadcaster.MessageType.RANDOMIZE_GROUP, string.Format("[c/9DFF66:{0}] randomized male NPCs' names!", Main.player[whoAmI].name));
                                        break;
                                    case 1001:
                                        Broadcaster.SendGlobalMessage(Broadcaster.MessageType.RANDOMIZE_GROUP, string.Format("[c/9DFF66:{0}] randomized female NPCs' names!", Main.player[whoAmI].name));
                                        break;
                                    case 1002:
                                        Broadcaster.SendGlobalMessage(Broadcaster.MessageType.RANDOMIZE_GROUP, string.Format("[c/9DFF66:{0}] randomized all NPCs' names!", Main.player[whoAmI].name));
                                        break;
                                    default:
                                        Broadcaster.SendGlobalMessage(Broadcaster.MessageType.RANDOMIZE_NPC, string.Format("[c/9DFF66:{0}] randomized the {1}'s name", Main.player[whoAmI].name, GetNPCName(id)));
                                        break;
                                }
                            } else {
                                var packet = instance.GetPacket();
                                packet.Write(PacketType.SERVER_REJECT_RANDOMIZE);
                                packet.Send(whoAmI);
                            }
                        }
                        break;
                    case PacketType.ADD_NAME: {
                            short id = reader.ReadInt16();
                            string name = reader.ReadString();
                            ulong nameID = reader.ReadUInt64();
                            var newWrapper = new StringWrapper(ref name, nameID);
                            CustomWorld.CustomNames[id].Add(newWrapper);
                            switch (id) {
                                case 1000:
                                    Broadcaster.SendGlobalMessage(Broadcaster.MessageType.ADD_CUSTOM_NAME, string.Format("[c/9DFF66:{0}] added [c/FFFF00:\"{1}\"] to the Masculine Names list.", Main.player[whoAmI], name));
                                    break;
                                case 1001:
                                    Broadcaster.SendGlobalMessage(Broadcaster.MessageType.ADD_CUSTOM_NAME, string.Format("[c/9DFF66:{0}] added [c/FFFF00:\"{1}\"] to the Feminine Names list.", Main.player[whoAmI], name));
                                    break;
                                case 1002:
                                    Broadcaster.SendGlobalMessage(Broadcaster.MessageType.ADD_CUSTOM_NAME, string.Format("[c/9DFF66:{0}] added [c/FFFF00:\"{1}\"] to the Global Names list.", Main.player[whoAmI], name));
                                    break;
                                default:
                                    Broadcaster.SendGlobalMessage(Broadcaster.MessageType.ADD_CUSTOM_NAME, string.Format("[c/9DFF66:{0}] added [c/FFFF00:\"{1}\"] to the {2}'s name list.", Main.player[whoAmI], name, GetNPCName(id)));
                                    break;
                            }
                            ModSync.SyncWorldData(SyncType.CUSTOM_NAMES, id);
                        }
                        break;
                    case PacketType.EDIT_NAME: {
                            short id = reader.ReadInt16();
                            string name = reader.ReadString();
                            ulong nameID = reader.ReadUInt64();
                            // Only edit if either the entry is not being edited by anyone, or if the request to edit it came from the player editing the entry
                            byte editingPlayer = 255;
                            foreach (BusyField j in CustomWorld.busyFields) {
                                if (j.ID == nameID) { editingPlayer = j.player; break; }
                            }
                            if (editingPlayer == 255 || whoAmI == editingPlayer) {
                                foreach (StringWrapper i in CustomWorld.CustomNames[id]) {
                                    if (i.ID == nameID) {
                                        switch (id) {
                                            case 1000:
                                                Broadcaster.SendGlobalMessage(Broadcaster.MessageType.EDIT_CUSTOM_NAME, string.Format("[c/9DFF66:{0}] edited [c/FFFF00:\"{1}\"] into [c/FFFF00:\"{2}\"] on the Masculine Names list.", Main.player[whoAmI], i.str, name));
                                                break;
                                            case 1001:
                                                Broadcaster.SendGlobalMessage(Broadcaster.MessageType.EDIT_CUSTOM_NAME, string.Format("[c/9DFF66:{0}] edited [c/FFFF00:\"{1}\"] into [c/FFFF00:\"{2}\"] on the Feminine Names list.", Main.player[whoAmI], i.str, name));
                                                break;
                                            case 1002:
                                                Broadcaster.SendGlobalMessage(Broadcaster.MessageType.EDIT_CUSTOM_NAME, string.Format("[c/9DFF66:{0}] edited [c/FFFF00:\"{1}\"] into [c/FFFF00:\"{2}\"] on the Global Names list.", Main.player[whoAmI].name, i.str, name));
                                                break;
                                            default:
                                                Broadcaster.SendGlobalMessage(Broadcaster.MessageType.EDIT_CUSTOM_NAME, string.Format("[c/9DFF66:{0}] edited [c/FFFF00:\"{1}\"] into [c/FFFF00:\"{2}\"] on the {3}'s name list.", Main.player[whoAmI].name, i.str, name, GetNPCName(id)));
                                                break;
                                        }
                                        i.str = name;
                                        ModSync.SyncWorldData(SyncType.CUSTOM_NAMES, id);
                                        break;
                                    }
                                }
                            } else {
                                var packet = instance.GetPacket();
                                packet.Write(PacketType.SERVER_REJECT_EDIT_NAME);
                                packet.Send(whoAmI);
                            }
                        }
                        break;
                    case PacketType.REMOVE_NAME: {
                            short id = reader.ReadInt16();
                            ulong nameID = reader.ReadUInt64();
                            for (int i = 0; i < CustomWorld.CustomNames[id].Count; i++) {
                                if (CustomWorld.CustomNames[id][i].ID == nameID) {
                                    // Only remove if either the entry is not being edited by anyone, or if the request to remove it came from the player editing the entry
                                    byte editingPlayer = 255;
                                    foreach (BusyField j in CustomWorld.busyFields) {
                                        if (j.ID == nameID) { editingPlayer = j.player; break; }
                                    }
                                    if (editingPlayer == 255 || whoAmI == editingPlayer) {
                                        switch (id) {
                                            case 1000:
                                                Broadcaster.SendGlobalMessage(Broadcaster.MessageType.REMOVE_CUSTOM_NAME, string.Format("[c/9DFF66:{0}] removed [c/FFFF00:\"{1}\"] from the Masculine Names list.", Main.player[whoAmI].name, CustomWorld.CustomNames[id][i].str));
                                                break;
                                            case 1001:
                                                Broadcaster.SendGlobalMessage(Broadcaster.MessageType.REMOVE_CUSTOM_NAME, string.Format("[c/9DFF66:{0}] removed [c/FFFF00:\"{1}\"] from the Feminine Names list.", Main.player[whoAmI].name, CustomWorld.CustomNames[id][i].str));
                                                break;
                                            case 1002:
                                                Broadcaster.SendGlobalMessage(Broadcaster.MessageType.REMOVE_CUSTOM_NAME, string.Format("[c/9DFF66:{0}] removed [c/FFFF00:\"{1}\"] from the Global Names list.", Main.player[whoAmI], CustomWorld.CustomNames[id][i].str));
                                                break;
                                            default:
                                                Broadcaster.SendGlobalMessage(Broadcaster.MessageType.REMOVE_CUSTOM_NAME, string.Format("[c/9DFF66:{0}] removed [c/FFFF00:\"{1}\"] from the {2}'s name list.", Main.player[whoAmI], CustomWorld.CustomNames[id][i].str, GetNPCName(id)));
                                                break;
                                        }
                                        CustomWorld.CustomNames[id].RemoveAt(i);
                                        ModSync.SyncWorldData(SyncType.CUSTOM_NAMES, id);
                                    } else {
                                        var packet = instance.GetPacket();
                                        packet.Write(PacketType.SERVER_REJECT_REMOVE_NAME);
                                        packet.Send(whoAmI);
                                    }
                                    break;
                                }
                            }
                        }
                        break;
                    case PacketType.REQUEST_WORLD_SYNC: {
                            byte syncType = (byte)reader.ReadInt16();
                            short syncId = (short)reader.ReadUInt64();
                            ModSync.SyncWorldData(syncType, syncId);
                        }
                        break;
                    case PacketType.SEND_BUSY_FIELD: {
                            ulong id = reader.ReadUInt64();
                            bool isBusy = reader.ReadBoolean();
                            BusyField busyField = new BusyField(id, (byte)whoAmI);
                            BusyField onList = new BusyField(0, 0); // ID = 0 represents an empty BusyField object, because it's impossible to achieve under normal circumstances
                            foreach (BusyField i in CustomWorld.busyFields) {
                                if (i.ID == id) { onList = i; break; }
                            }
                            if (isBusy) {
                                if (onList.ID == 0) {
                                    CustomWorld.busyFields.Add(busyField);
                                    ModSync.SyncWorldData(SyncType.BUSY_FIELDS);
                                }
                            } else {
                                CustomWorld.busyFields.Remove(onList);
                                ModSync.SyncWorldData(SyncType.BUSY_FIELDS);
                            }
                        }
                        break;
                    case PacketType.RESET_EVERYTHING: {
                            // Only do it if no custom name entries are being edited
                            bool anyBusyCustomNames = false;
                            foreach (BusyField i in CustomWorld.busyFields) {
                                foreach (short j in TownNPCs) {
                                    if (i.ID == (ulong)j) { anyBusyCustomNames = true; goto Break; }
                                }
                            }

                            Break:
                            if (!anyBusyCustomNames) {
                                CustomWorld.mode = 0;
                                CustomWorld.tryUnique = true;
                                CustomWorld.ResetCustomNames();
                                NPCs.CustomNPC.ResetCurrentGender();
                                Broadcaster.SendGlobalMessage(Broadcaster.MessageType.CUT_EVERYTHING, string.Format("[c/9DFF66:{0}] cut everything from the world!", Main.player[whoAmI].name));
                                ModSync.SyncWorldData(SyncType.EVERYTHING);
                            } else {
                                var packet = instance.GetPacket();
                                packet.Write(PacketType.SERVER_REJECT_CUT);
                                packet.Send(whoAmI);
                            }
                        }
                        break;
                    case PacketType.RESET_BUSY_PLAYER: {
                            int prevCount = CustomWorld.busyFields.Count;
                            for (int i = 0; i < CustomWorld.busyFields.Count; i++) {
                                if (CustomWorld.busyFields[i].player == whoAmI) { CustomWorld.busyFields.RemoveAt(i); }
                            }
                            if (CustomWorld.busyFields.Count < prevCount) {
                                ModSync.SyncWorldData(SyncType.BUSY_FIELDS);
                            }
                        }
                        break;
                }
            } else { // if a client is the recipient
                switch (type) {
                    case PacketType.SERVER_AWAITING_COPY_DATA: {
                            // Initiate sending copy data, because the server is ready and waiting
                            var packet = instance.GetPacket();
                            packet.Write(PacketType.SEND_COPY_MODE_TRYUNIQUE_ISMALE);
                            packet.Write(RenameUI.copyData.mode);
                            packet.Write(RenameUI.copyData.tryUnique);
                            var bits = new BitsByte();
                            for (int i = 0; i < 3; i++) {
                                for (int j = 0; j < 8; j++) {
                                    bits[j] = RenameUI.copyData.isMale[TownNPCs[(short)((i * 8) + j)]];
                                }
                                packet.Write(bits);
                            }
                            packet.Send();

                            // CustomNames
                            foreach (short i in TownNPCs) {
                                PacketSender.SendPacketToServer(PacketType.SEND_COPY_NAMES, i);
                            }
                            PacketSender.SendPacketToServer(PacketType.SEND_COPY_NAMES, 1000); // male
                            PacketSender.SendPacketToServer(PacketType.SEND_COPY_NAMES, 1001); // female
                            PacketSender.SendPacketToServer(PacketType.SEND_COPY_NAMES, 1002); // global

                            WaitForServerResponse = false;
                        }
                        break;
                    case PacketType.SERVER_REJECT_COPY_DATA:
                        Main.NewText("Server rejected your paste data. This can be because other users are editing entries, or because another user is pasting their data.", Color.OrangeRed);
                        break;
                    case PacketType.SERVER_REJECT_REMOVE_NAME:
                        Main.NewText("Server rejected your removal request, because the name you're trying to remove is being edited.", Color.OrangeRed);
                        break;
                    case PacketType.SERVER_REJECT_EDIT_NAME:
                        Main.NewText("Server rejected your edit request, because the name you're trying to change is already being edited.", Color.OrangeRed);
                        break;
                    case PacketType.SERVER_REJECT_RANDOMIZE:
                        Main.NewText("Server rejected your randomization request, because some of the names you're trying to randomize are being edited.", Color.OrangeRed);
                        break;
                    case PacketType.SERVER_REJECT_CLEAR_ALL:
                        Main.NewText("Server rejected your clear request, because some names in the current list are being edited.", Color.OrangeRed);
                        break;
                }
            }
        }

        public override void Unload()
        {
            renameUI = null;
            renameInterface = null;
            RenameMenuHotkey = null;
            CustomWorld.Unload();
            NPCs.CustomNPC.Unload();
            RenameUI.Unload();
            UINPCButton.Unload();
            instance = null;

            base.Unload();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            // it will only draw if the player is not on the main menu
            if (!Main.gameMenu && RenameUI.Visible) {
                renameInterface.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1) {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer("CustomNPCMod: Menu UI", DrawRenameMenuUI, InterfaceScaleType.UI));
            }
        }

        public override void PreSaveAndQuit()
        {
            base.PreSaveAndQuit();
            CustomWorld.saveAndExit = true;

            if (Main.netMode == NetmodeID.MultiplayerClient) {
                PacketSender.SendPacketToServer(PacketType.RESET_BUSY_PLAYER);
                RenameUI.Visible = false;
                UINPCButton.Deselect();
                RenameUI.panelList.Clear();
                RenameUI.removeMode = false;
                CustomWorld.busyFields.Clear();
                if (!RenameUI.carry) {
                    CustomWorld.ResetCustomNames();
                    NPCs.CustomNPC.ResetCurrentGender();
                    RenameUI.modeCycleButton.State = 0;
                    RenameUI.uniqueNameButton.State = true;
                }
            }
        }

        private bool DrawRenameMenuUI()
        {
            // it will only draw if the player is not on the main menu
            if (!Main.gameMenu && RenameUI.Visible) {
                renameInterface.Draw(Main.spriteBatch, new GameTime());
            }
            return true;
        }

        public static string GetNPCName(short id)
        {
            switch (id) {
                case NPCID.Guide:
                    return "Guide";
                case NPCID.Merchant:
                    return "Merchant";
                case NPCID.Nurse:
                    return "Nurse";
                case NPCID.Demolitionist:
                    return "Demolitionist";
                case NPCID.Dryad:
                    return "Dryad";
                case NPCID.ArmsDealer:
                    return "Arms Dealer";
                case NPCID.Clothier:
                    return "Clothier";
                case NPCID.Mechanic:
                    return "Mechanic";
                case NPCID.GoblinTinkerer:
                    return "Goblin Tinkerer";
                case NPCID.Wizard:
                    return "Wizard";
                case NPCID.SkeletonMerchant:
                    return "Skeleton Merchant";
                case NPCID.Truffle:
                    return "Truffle";
                case NPCID.Steampunker:
                    return "Steampunker";
                case NPCID.DyeTrader:
                    return "Dye Trader";
                case NPCID.PartyGirl:
                    return "Party Girl";
                case NPCID.Cyborg:
                    return "Cyborg";
                case NPCID.Painter:
                    return "Painter";
                case NPCID.WitchDoctor:
                    return "Witch Doctor";
                case NPCID.Pirate:
                    return "Pirate";
                case NPCID.Stylist:
                    return "Stylist";
                case NPCID.TravellingMerchant:
                    return "Travelling Merchant";
                case NPCID.Angler:
                    return "Angler";
                case NPCID.TaxCollector:
                    return "Tax Collector";
                case NPCID.DD2Bartender:
                    return "Tavernkeep";
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// Makes it possible to store a string by mutable object reference. Also has an advanced ID system for distinguishing every object on a multiplayer server.
    /// </summary>
    public class StringWrapper
    {
        public string str;
        public readonly ulong ID;
        private static Random random = new Random();

        public StringWrapper(ref string str, ulong id = 0)
        {
            this.str = str;
            if (id == 0) {
                if (Main.netMode == NetmodeID.SinglePlayer || Main.netMode == NetmodeID.MultiplayerClient) {
                    // ID must always be unique. To achieve that, it will combine datetime information with player information. That way, if two players send information at the same date and time, the IDs will be different because they're different players. If the same player were to send the information twice, it will be distinguishable because of the time difference.
                    ulong newId = 0x0000000000000000;      // this will be used as a buffer
                    byte userId = (byte)Main.myPlayer; // byte, because the max number of players is 255, which is (2^8)-1
                    var now = DateTime.UtcNow;
                    ushort month = (ushort)now.Month;
                    ushort day = (ushort)now.Day;
                    ulong tickCount = (ulong)now.Ticks;         // converts binary format to unsigned for future parsing reasons
                    uint time = (uint)(tickCount * 0.0000065);  // convert to 60 ticks per second (the multiplayer is roughly equal to (24*3600*60)/(24*3600*10000000), rounding errors pretty much don't matter here)
                    newId |= (ulong)userId << 56; // 8 bits
                    newId |= (ulong)month  << 52; // 4 bits (<=15)
                    newId |= (ulong)day    << 47; // 5 bits (<=31)
                    newId |= (ulong)time   << 24; // 23 bits, because (24*3600*60) lies between 2^22 and 2^23.
                    newId |= (ulong)random.Next() >> 8; // rand.Next() generates an Int32, we have 24 bits left on the buffer, so we fill the remaining space by shifting it 32-24=8 places to the right.
                    // the random value exists because why not, there's 24 bits of data left on the buffer, so this adds an extra layer of security.
                    ID = newId;
                } else if (Main.netMode == NetmodeID.Server) {
                    if (CustomNPCNames.instance != null) {
                        CustomNPCNames.instance.Logger.Error("Tried generating CustomNPCNames.StringWrapper.ID on the server.");
                        NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("CustomNPCNames Error: Tried generating StringWrapper.ID on the server. Please report this issue on the Mod's Homepage."), Color.Orange);
                    }
                    ID = 0;
                }
            } else {
                ID = id;
            }
        }

        /// <summary>Creates a deep copy of the calling StringWrapper object and returns it.</summary>
        public StringWrapper Clone()
        {
            string newName = str;
            return new StringWrapper(ref newName, ID);
        }

        public static explicit operator string(StringWrapper wr)
        { return wr.str; }

        public static implicit operator StringWrapper(string str)
        { return new StringWrapper(ref str); }

        public override string ToString()
        { return str; }

        public override bool Equals(object obj)
        {
            if (obj != null && GetType().Equals(obj.GetType())) {
                return ID == (obj as StringWrapper).ID;
            } else {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        
        public static List<StringWrapper> ConvertList(IList<string> list)
        {
            var ret = new List<StringWrapper>();
            foreach (string s in list) {
                ret.Add(s);
            }

            return ret;
        }

        /// <summary>
        /// This method exists exclusively for CustomWorld.Load() method.
        /// </summary>
        public static List<StringWrapper> ConvertSaveList(IList<string> list)
        {
            var ret = new List<StringWrapper>();
            for (int i = 0; i < list.Count; i += 2) {
                string str = list[i];
                ulong id = Convert.ToUInt64(list[i + 1]);
                ret.Add(new StringWrapper(ref str, id));
            }

            return ret;
        }

        public static bool ListContains(IList<StringWrapper> list, string value)
        {
            foreach (var i in list) {
                if ((string)i == value) {
                    return true;
                }
            }

            return false;
        }
    }
}
