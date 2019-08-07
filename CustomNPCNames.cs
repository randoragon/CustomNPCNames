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

        public override void Load()
        {
            instance = this;
            // this makes sure that the UI doesn't get opened on the server console
            if (!Main.dedServ) {
                RenameMenuHotkey = RegisterHotKey("Toggle Menu", "K");
                renameUI = new RenameUI();
                renameUI.Initialize();
                renameInterface = new UserInterface();
                renameInterface.SetState(renameUI);
            }
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            byte type = reader.ReadByte();
            switch (type) {
                case PacketType.NEXT_MODE:
                    CustomWorld.mode = (byte)(++CustomWorld.mode % 4);
                    ModSync.SyncWorldData(SyncType.MODE);
                    break;
                case PacketType.PREV_MODE:
                    CustomWorld.mode = (byte)(--CustomWorld.mode % 4);
                    ModSync.SyncWorldData(SyncType.MODE);
                    break;
                case PacketType.TOGGLE_TRY_UNIQUE:
                    CustomWorld.tryUnique = !CustomWorld.tryUnique;
                    ModSync.SyncWorldData(SyncType.TRY_UNIQUE);
                    break;
                case PacketType.SWITCH_GENDER: {
                        short id = reader.ReadInt16();
                        NPCs.CustomNPC.isMale[id] = !NPCs.CustomNPC.isMale[id];
                        ModSync.SyncWorldData(SyncType.GENDER, id);
                    }
                    break;
                case PacketType.SEND_NAME: {
                        short id = reader.ReadInt16();
                        string name = reader.ReadString();
                        NPC npc = NPCs.CustomNPC.FindFirstNPC(id);
                        if (npc != null) {
                            npc.GivenName = name;
                        }
                        ModSync.SyncWorldData(SyncType.NAME, id);
                    }
                    break;
                case PacketType.SEND_CUSTOM_NAMES: {
                        short id = reader.ReadInt16();
                        int index = reader.ReadInt32();
                        if (index != -1) {
                            byte offset = reader.ReadByte();
                            bool lastPacket = reader.ReadBoolean();
                            while (CustomWorld.CustomNames[id].Count < index + offset) {
                                CustomWorld.CustomNames[id].Add("");
                            }
                            if (lastPacket) {
                                int overshoot = CustomWorld.CustomNames[id].Count - (index + offset);
                                if (overshoot > 0) {
                                    CustomWorld.CustomNames[id].RemoveRange(index + offset, overshoot);
                                }
                            }
                            for (int i = index; i < index + offset; i++) {
                                CustomWorld.CustomNames[id][i] = reader.ReadString();
                            }

                            if (lastPacket) { ModSync.SyncWorldData(SyncType.CUSTOM_NAMES, id); }
                        } else {
                            CustomWorld.CustomNames[id].Clear();
                            ModSync.SyncWorldData(SyncType.CUSTOM_NAMES, id);
                        }
                    }
                    break;
                case PacketType.SEND_EVERYTHING: {
                        CustomWorld.mode = reader.ReadByte();
                        CustomWorld.tryUnique = reader.ReadBoolean();
                        for (int i = 0; i < 3; i++) {
                            BitsByte b = reader.ReadByte();
                            for (int j = 0; j < 8; j++) {
                                NPCs.CustomNPC.isMale[TownNPCs[(short)((8 * i) + j)]] = b[j];
                            }
                        }
                        ModSync.SyncWorldData(SyncType.EVERYTHING);
                    }
                    break;
                case PacketType.RANDOMIZE: {
                        short id = reader.ReadInt16();
                        NPCs.CustomNPC.RandomizeName(id);
                        // world sync is called from the RandomizeName method
                    }
                    break;
                case PacketType.ADD_NAME: {
                        short id = reader.ReadInt16();
                        string name = reader.ReadString();
                        ulong nameID = reader.ReadUInt64();
                        var newWrapper = new StringWrapper(ref name, nameID);
                        CustomWorld.CustomNames[id].Add(newWrapper);
                        ModSync.SyncWorldData(SyncType.CUSTOM_NAMES, id);
                    }
                    break;
                case PacketType.EDIT_NAME: {
                        short id = reader.ReadInt16();
                        string name = reader.ReadString();
                        ulong nameID = reader.ReadUInt64();
                        foreach (StringWrapper i in CustomWorld.CustomNames[id]) {
                            if (i.ID == nameID) {
                                i.str = name;
                                break;
                            }
                        }
                        ModSync.SyncWorldData(SyncType.CUSTOM_NAMES, id);
                    }
                    break;
                case PacketType.REMOVE_NAME: {
                        short id = reader.ReadInt16();
                        ulong nameID = reader.ReadUInt64();
                        for (int i = 0; i < CustomWorld.CustomNames[id].Count; i++) {
                            if (CustomWorld.CustomNames[id][i].ID == nameID) {
                                CustomWorld.CustomNames[id].RemoveAt(i);
                                break;
                            }
                        }
                        ModSync.SyncWorldData(SyncType.CUSTOM_NAMES, id);
                    }
                    break;
                case PacketType.REQUEST_WORLD_SYNC: {
                        byte syncType = (byte)reader.ReadInt16();
                        short syncId = (short)reader.ReadUInt64();
                        ModSync.SyncWorldData(syncType, syncId);
                    }
                    break;
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
                }
            } else {
                ID = id;
            }
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
