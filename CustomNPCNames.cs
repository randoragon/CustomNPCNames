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
    class CustomNPCNames : Mod
    {
        public static CustomNPCNames instance;
        public static readonly short[] TownNPCs = {
            NPCID.Guide,         NPCID.Merchant,        NPCID.Nurse,
            NPCID.Demolitionist, NPCID.DyeTrader,       NPCID.Dryad,
            NPCID.DD2Bartender,  NPCID.ArmsDealer,      NPCID.Stylist,
            NPCID.Painter,       NPCID.Angler,          NPCID.GoblinTinkerer,
            NPCID.WitchDoctor,   NPCID.Clothier,        NPCID.Mechanic,
            NPCID.PartyGirl,     NPCID.Wizard,          NPCID.TaxCollector,
            NPCID.Truffle,       NPCID.Pirate,          NPCID.Steampunker,
            NPCID.Cyborg,        NPCID.SantaClaus,      NPCID.TravellingMerchant
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
            { NPCID.SantaClaus,         new Vector2(0, 0) },
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
                    CustomWorld.SyncWorldData();
                    break;
                case PacketType.PREV_MODE:
                    CustomWorld.mode = (byte)(--CustomWorld.mode % 4);
                    CustomWorld.SyncWorldData();
                    break;
                case PacketType.TOGGLE_TRY_UNIQUE:
                    CustomWorld.tryUnique = !CustomWorld.tryUnique;
                    CustomWorld.SyncWorldData();
                    break;
                case PacketType.SEND_CURRENT_NAMES: {
                        short id = reader.ReadInt16();
                        if (id != 1000 && id != 1001 && id != 1002) {
                            NPCs.CustomNPC.currentNames[id] = reader.ReadString();
                            CustomWorld.SyncWorldData();
                        } else if (id == 1000) {
                            var ids = new List<short>();
                            foreach (short i in TownNPCs) {
                                if (NPCs.CustomNPC.isMale[i]) {
                                    ids.Add(i);
                                }
                            }
                            int index = reader.ReadInt32();
                            byte offset = reader.ReadByte();
                            bool lastPacket = reader.ReadBoolean();
                            for (int i = index; i < index + offset; i++) {
                                NPCs.CustomNPC.currentNames[ids[i]] = reader.ReadString();
                            }
                            CustomWorld.SyncWorldData();
                        }
                    }
                    break;
                case PacketType.SWITCH_GENDER: {
                        short id = reader.ReadInt16();
                        NPCs.CustomNPC.isMale[id] = !NPCs.CustomNPC.isMale[id];
                        CustomWorld.SyncWorldData();
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

                            if (lastPacket) { CustomWorld.SyncWorldData(); }
                        } else {
                            CustomWorld.CustomNames[id].Clear();
                            CustomWorld.SyncWorldData();
                        }
                    }
                    break;
                case PacketType.SEND_EVERYTHING: {
                        byte number = reader.ReadByte();
                        switch (number) {
                            case 1:
                                CustomWorld.mode = reader.ReadByte();
                                CustomWorld.tryUnique = reader.ReadBoolean();
                                break;
                            case 2:
                            case 3:
                            case 4:
                                for (int i = (number - 2) * 8; i < (number - 1) * 8; i++) {
                                    NPCs.CustomNPC.currentNames[TownNPCs[i]] = reader.ReadString();
                                }
                                break;
                            case 5: {
                                    // isMale
                                    for (int i = 0; i < 3; i++) {
                                        BitsByte b = reader.ReadByte();
                                        for (int j = 0; j < 8; j++) {
                                            NPCs.CustomNPC.isMale[TownNPCs[(short)((8 * i) + j)]] = b[j];
                                        }
                                    }
                                }
                                break;
                        }

                        CustomWorld.SyncWorldData();
                    }
                    break;
                case PacketType.RANDOMIZE: {
                        short id = reader.ReadInt16();
                        NPCs.CustomNPC.RandomizeName(id);
                        CustomWorld.SyncWorldData();
                    }
                    break;
                case PacketType.ADD_NAME: {
                        short id = reader.ReadInt16();
                        string name = reader.ReadString();
                        ulong nameID = reader.ReadUInt64();
                        var newWrapper = new StringWrapper(ref name, nameID);
                        CustomWorld.CustomNames[id].Add(newWrapper);
                        CustomWorld.SyncWorldData();
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
                        CustomWorld.SyncWorldData();
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
                        CustomWorld.SyncWorldData();
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
                case NPCID.SantaClaus:
                    return "Santa Claus";
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
                    ulong newId = 0x0000000000000000;
                    ushort userId = (ushort)Main.myPlayer; // ushort is sufficient, it supports up to (2^16)-1 unique players.
                    var now = DateTime.UtcNow;
                    ushort month = (ushort)now.Month;
                    ushort day = (ushort)now.Day;
                    ulong tickCount = (ulong)now.Ticks;         // converts binary format to unsigned for future parsing reasons
                    uint time = (uint)(tickCount * 0.0000065);  // convert to 60 ticks per second (the multiplayer is roughly equal to (24*3600*60)/(24*3600*10000000), rounding errors pretty much don't matter here)
                    newId |= (ulong)userId << 48; // 16 bits
                    newId |= (ulong)month  << 44; // 4 bits (<=15)
                    newId |= (ulong)day    << 39; // 5 bits (<=31)
                    newId |= (ulong)time   << 16; // 23 bits, because (24*3600*60) lies between 2^22 and 2^23.
                    newId |= (ulong)random.Next() >> 16; // rand.Next() generates an Int32, we have 16 bits left on the buffer, so we cut it in half by bit shifting 16 positions to the right.
                    // the random value exists because why not, there's 16 bits of data left on the buffer, so this adds an extra layer of security.
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
                return str == ((StringWrapper)obj).str;
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
