using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using Terraria;
using System.IO;
using CustomNPCNames.Network;

namespace CustomNPCNames
{
    public class CustomWorld : ModWorld
    {
        public static Dictionary<short, List<StringWrapper>> CustomNames;
        public static byte mode;
        public static bool tryUnique;
        public static bool saveAndExit = false; // this is turned to true in CustomNPCNames.PreSaveAndExit() to distinguish autosave from save&exit
        public static bool updateNameList = false; // this is turned to true in NetReceive() to print renameUI's panelList's contents to the screen
        public static int receivedPackets = 0;  // these two are used exclusively with SEND_COPY_DATA (i.e. pasting everything) so that the world sync occurs only once
        public static int packetsTillSync = 0;  // after all the packets have arrived, as opposed to syncing a bunch of times every single packet. Just for optimization.
        public static List<BusyField> busyFields;       // this list contains references to fields which are being edited by other players, and therefore locked from access

        public CustomWorld()
        {
            ResetCustomNames();
            mode = 0;
            tryUnique = true;
        }

        public override void Initialize()
        {
            base.Initialize();
            busyFields = new List<BusyField>();
        }

        public static void Unload()
        {
            CustomNames = null;
            busyFields = null;
        }

        public static void ResetCustomNames()
        {
            CustomNames = new Dictionary<short, List<StringWrapper>>();
            foreach (short i in CustomNPCNames.TownNPCs) {
                CustomNames.Add(i, new List<StringWrapper>());
            }

            CustomNames.Add(1000, new List<StringWrapper>()); // 
            CustomNames.Add(1001, new List<StringWrapper>()); // for Male, Female and Global respectively
            CustomNames.Add(1002, new List<StringWrapper>()); // 
        }

        public override TagCompound Save()
        {
            base.Save();

            Dictionary<short, List<string>> nameStrings = new Dictionary<short, List<string>>();
            foreach (KeyValuePair<short, List<StringWrapper>> i in CustomNames) {
                List<string> list = new List<string>();
                foreach (StringWrapper j in i.Value) {
                    list.Add((string)j);
                    list.Add(System.Convert.ToString((long)j.ID));
                }
                nameStrings.Add(i.Key, list);
            }

            TagCompound tag = new TagCompound();
            tag.Add("guide",              nameStrings[NPCID.Guide]);
            tag.Add("merchant",           nameStrings[NPCID.Merchant]);
            tag.Add("nurse",              nameStrings[NPCID.Nurse]);
            tag.Add("demolitionist",      nameStrings[NPCID.Demolitionist]);
            tag.Add("dyetrader",          nameStrings[NPCID.DyeTrader]);
            tag.Add("dryad",              nameStrings[NPCID.Dryad]);
            tag.Add("tavernkeep",         nameStrings[NPCID.DD2Bartender]);
            tag.Add("armsdealer",         nameStrings[NPCID.ArmsDealer]);
            tag.Add("stylist",            nameStrings[NPCID.Stylist]);
            tag.Add("painter",            nameStrings[NPCID.Painter]);
            tag.Add("angler",             nameStrings[NPCID.Angler]);
            tag.Add("goblintinkerer",     nameStrings[NPCID.GoblinTinkerer]);
            tag.Add("witchdoctor",        nameStrings[NPCID.WitchDoctor]);
            tag.Add("clothier",           nameStrings[NPCID.Clothier]);
            tag.Add("mechanic",           nameStrings[NPCID.Mechanic]);
            tag.Add("partygirl",          nameStrings[NPCID.PartyGirl]);
            tag.Add("wizard",             nameStrings[NPCID.Wizard]);
            tag.Add("taxcollector",       nameStrings[NPCID.TaxCollector]);
            tag.Add("truffle",            nameStrings[NPCID.Truffle]);
            tag.Add("pirate",             nameStrings[NPCID.Pirate]);
            tag.Add("steampunker",        nameStrings[NPCID.Steampunker]);
            tag.Add("cyborg",             nameStrings[NPCID.Cyborg]);
            tag.Add("skeletonmerchant",   nameStrings[NPCID.SkeletonMerchant]);
            tag.Add("travellingmerchant", nameStrings[NPCID.TravellingMerchant]);
            tag.Add("male",   nameStrings[1000]);
            tag.Add("female", nameStrings[1001]);
            tag.Add("global", nameStrings[1002]);
            tag.Add("guide-gender",              NPCs.CustomNPC.isMale[NPCID.Guide]);
            tag.Add("merchant-gender",           NPCs.CustomNPC.isMale[NPCID.Merchant]);
            tag.Add("nurse-gender",              NPCs.CustomNPC.isMale[NPCID.Nurse]);
            tag.Add("demolitionist-gender",      NPCs.CustomNPC.isMale[NPCID.Demolitionist]);
            tag.Add("dyetrader-gender",          NPCs.CustomNPC.isMale[NPCID.DyeTrader]);
            tag.Add("dryad-gender",              NPCs.CustomNPC.isMale[NPCID.Dryad]);
            tag.Add("tavernkeep-gender",         NPCs.CustomNPC.isMale[NPCID.DD2Bartender]);
            tag.Add("armsdealer-gender",         NPCs.CustomNPC.isMale[NPCID.ArmsDealer]);
            tag.Add("stylist-gender",            NPCs.CustomNPC.isMale[NPCID.Stylist]);
            tag.Add("painter-gender",            NPCs.CustomNPC.isMale[NPCID.Painter]);
            tag.Add("angler-gender",             NPCs.CustomNPC.isMale[NPCID.Angler]);
            tag.Add("goblintinkerer-gender",     NPCs.CustomNPC.isMale[NPCID.GoblinTinkerer]);
            tag.Add("witchdoctor-gender",        NPCs.CustomNPC.isMale[NPCID.WitchDoctor]);
            tag.Add("clothier-gender",           NPCs.CustomNPC.isMale[NPCID.Clothier]);
            tag.Add("mechanic-gender",           NPCs.CustomNPC.isMale[NPCID.Mechanic]);
            tag.Add("partygirl-gender",          NPCs.CustomNPC.isMale[NPCID.PartyGirl]);
            tag.Add("wizard-gender",             NPCs.CustomNPC.isMale[NPCID.Wizard]);
            tag.Add("taxcollector-gender",       NPCs.CustomNPC.isMale[NPCID.TaxCollector]);
            tag.Add("truffle-gender",            NPCs.CustomNPC.isMale[NPCID.Truffle]);
            tag.Add("pirate-gender",             NPCs.CustomNPC.isMale[NPCID.Pirate]);
            tag.Add("steampunker-gender",        NPCs.CustomNPC.isMale[NPCID.Steampunker]);
            tag.Add("cyborg-gender",             NPCs.CustomNPC.isMale[NPCID.Cyborg]);
            tag.Add("skeletonmerchant-gender",   NPCs.CustomNPC.isMale[NPCID.SkeletonMerchant]);
            tag.Add("travellingmerchant-gender", NPCs.CustomNPC.isMale[NPCID.TravellingMerchant]);
            tag.Add("mode", mode);
            tag.Add("tryunique", tryUnique);
            tag.Add("selected-npc", (short)(UI.UINPCButton.Selection != null ? UI.UINPCButton.Selection.npcId : 0)); // 0 is conventional for none, -1 is conventional for reset

            // Reset everything if this is a save&exit moment (as opposed to autosave)
            if (saveAndExit) {
                UI.RenameUI.Visible = false;
                UI.UINPCButton.Deselect();
                UI.RenameUI.panelList.Clear();
                UI.RenameUI.removeMode = false;
                busyFields.Clear();

                if (!UI.RenameUI.carry) {
                    ResetCustomNames();
                    NPCs.CustomNPC.ResetCurrentGender();
                    UI.RenameUI.modeCycleButton.State = 0;
                    UI.RenameUI.uniqueNameButton.State = true;
                }

                saveAndExit = false;
            }

            return tag;
        }

        public override void Load(TagCompound tag)
        {
            if (tag.ContainsKey("guide")) {
                CustomNames[NPCID.Guide]              = StringWrapper.ConvertSaveList(tag.GetList<string>("guide"));
                CustomNames[NPCID.Merchant]           = StringWrapper.ConvertSaveList(tag.GetList<string>("merchant"));
                CustomNames[NPCID.Nurse]              = StringWrapper.ConvertSaveList(tag.GetList<string>("nurse"));
                CustomNames[NPCID.Demolitionist]      = StringWrapper.ConvertSaveList(tag.GetList<string>("demolitionist"));
                CustomNames[NPCID.DyeTrader]          = StringWrapper.ConvertSaveList(tag.GetList<string>("dyetrader"));
                CustomNames[NPCID.Dryad]              = StringWrapper.ConvertSaveList(tag.GetList<string>("dryad"));
                CustomNames[NPCID.DD2Bartender]       = StringWrapper.ConvertSaveList(tag.GetList<string>("tavernkeep"));
                CustomNames[NPCID.ArmsDealer]         = StringWrapper.ConvertSaveList(tag.GetList<string>("armsdealer"));
                CustomNames[NPCID.Stylist]            = StringWrapper.ConvertSaveList(tag.GetList<string>("stylist"));
                CustomNames[NPCID.Painter]            = StringWrapper.ConvertSaveList(tag.GetList<string>("painter"));
                CustomNames[NPCID.Angler]             = StringWrapper.ConvertSaveList(tag.GetList<string>("angler"));
                CustomNames[NPCID.GoblinTinkerer]     = StringWrapper.ConvertSaveList(tag.GetList<string>("goblintinkerer"));
                CustomNames[NPCID.WitchDoctor]        = StringWrapper.ConvertSaveList(tag.GetList<string>("witchdoctor"));
                CustomNames[NPCID.Clothier]           = StringWrapper.ConvertSaveList(tag.GetList<string>("clothier"));
                CustomNames[NPCID.Mechanic]           = StringWrapper.ConvertSaveList(tag.GetList<string>("mechanic"));
                CustomNames[NPCID.PartyGirl]          = StringWrapper.ConvertSaveList(tag.GetList<string>("partygirl"));
                CustomNames[NPCID.Wizard]             = StringWrapper.ConvertSaveList(tag.GetList<string>("wizard"));
                CustomNames[NPCID.TaxCollector]       = StringWrapper.ConvertSaveList(tag.GetList<string>("taxcollector"));
                CustomNames[NPCID.Truffle]            = StringWrapper.ConvertSaveList(tag.GetList<string>("truffle"));
                CustomNames[NPCID.Pirate]             = StringWrapper.ConvertSaveList(tag.GetList<string>("pirate"));
                CustomNames[NPCID.Steampunker]        = StringWrapper.ConvertSaveList(tag.GetList<string>("steampunker"));
                CustomNames[NPCID.Cyborg]             = StringWrapper.ConvertSaveList(tag.GetList<string>("cyborg"));
                CustomNames[NPCID.SkeletonMerchant]   = StringWrapper.ConvertSaveList(tag.GetList<string>("skeletonmerchant"));
                CustomNames[NPCID.TravellingMerchant] = StringWrapper.ConvertSaveList(tag.GetList<string>("travellingmerchant"));
                CustomNames[1000] = StringWrapper.ConvertSaveList(tag.GetList<string>("male"));
                CustomNames[1001] = StringWrapper.ConvertSaveList(tag.GetList<string>("female"));
                CustomNames[1002] = StringWrapper.ConvertSaveList(tag.GetList<string>("global"));
                NPCs.CustomNPC.isMale[NPCID.Guide] =              tag.GetBool("guide-gender");
                NPCs.CustomNPC.isMale[NPCID.Merchant] =           tag.GetBool("merchant-gender");
                NPCs.CustomNPC.isMale[NPCID.Nurse] =              tag.GetBool("nurse-gender");
                NPCs.CustomNPC.isMale[NPCID.Demolitionist] =      tag.GetBool("demolitionist-gender");
                NPCs.CustomNPC.isMale[NPCID.DyeTrader] =          tag.GetBool("dyetrader-gender");
                NPCs.CustomNPC.isMale[NPCID.Dryad] =              tag.GetBool("dryad-gender");
                NPCs.CustomNPC.isMale[NPCID.DD2Bartender] =       tag.GetBool("tavernkeep-gender");
                NPCs.CustomNPC.isMale[NPCID.ArmsDealer] =         tag.GetBool("armsdealer-gender");
                NPCs.CustomNPC.isMale[NPCID.Stylist] =            tag.GetBool("stylist-gender");
                NPCs.CustomNPC.isMale[NPCID.Painter] =            tag.GetBool("painter-gender");
                NPCs.CustomNPC.isMale[NPCID.Angler] =             tag.GetBool("angler-gender");
                NPCs.CustomNPC.isMale[NPCID.GoblinTinkerer] =     tag.GetBool("goblintinkerer-gender");
                NPCs.CustomNPC.isMale[NPCID.WitchDoctor] =        tag.GetBool("witchdoctor-gender");
                NPCs.CustomNPC.isMale[NPCID.Clothier] =           tag.GetBool("clothier-gender");
                NPCs.CustomNPC.isMale[NPCID.Mechanic] =           tag.GetBool("mechanic-gender");
                NPCs.CustomNPC.isMale[NPCID.PartyGirl] =          tag.GetBool("partygirl-gender");
                NPCs.CustomNPC.isMale[NPCID.Wizard] =             tag.GetBool("wizard-gender");
                NPCs.CustomNPC.isMale[NPCID.TaxCollector] =       tag.GetBool("taxcollector-gender");
                NPCs.CustomNPC.isMale[NPCID.Truffle] =            tag.GetBool("truffle-gender");
                NPCs.CustomNPC.isMale[NPCID.Pirate] =             tag.GetBool("pirate-gender");
                NPCs.CustomNPC.isMale[NPCID.Steampunker] =        tag.GetBool("steampunker-gender");
                NPCs.CustomNPC.isMale[NPCID.Cyborg] =             tag.GetBool("cyborg-gender");
                NPCs.CustomNPC.isMale[NPCID.SkeletonMerchant] =   tag.GetBool("skeletonmerchant-gender");
                NPCs.CustomNPC.isMale[NPCID.TravellingMerchant] = tag.GetBool("travellingmerchant-gender");
                mode =      tag.GetByte("mode");
                tryUnique = tag.GetBool("tryunique");
                UI.RenameUI.savedSelectedNPC = tag.ContainsKey("selected-npc") ? tag.GetShort("selected-npc") : (short)-1; // 0 is conventional for none, -1 is conventional for "Do Nothing"
            }
        }

        public override void NetSend(BinaryWriter packet)
        {
            packet.Write(SyncType.Get);

            switch (SyncType.Get) {
                case SyncType.MODE:
                    packet.Write(mode);
                    break;
                case SyncType.TRY_UNIQUE:
                    packet.Write(tryUnique);
                    break;
                case SyncType.NAME: {
                        short id = ModSync.ID;
                        packet.Write(id);
                        packet.Write(NPCs.CustomNPC.FindFirstNPC(id)?.GivenName);
                    }
                    break;
                case SyncType.GENDER: {
                        short id = ModSync.ID;
                        packet.Write(id);
                        packet.Write(NPCs.CustomNPC.isMale[id]);
                    }
                    break;
                case SyncType.CUSTOM_NAMES: {
                        short id = ModSync.ID;
                        packet.Write(id);
                        packet.Write(CustomNames[id].Count);
                        foreach (var i in CustomNames[id]) {
                            packet.Write(i.ToString());
                            packet.Write(i.ID);
                        }
                    }
                    break;
                case SyncType.EVERYTHING:
                    packet.Write(mode);
                    packet.Write(tryUnique);
                    foreach (KeyValuePair<short, List<StringWrapper>> i in CustomNames) {
                        packet.Write(i.Value.Count);
                        foreach (StringWrapper j in i.Value) {
                            packet.Write(j.ToString());
                            packet.Write(j.ID);
                        }
                    }
                    foreach (short i in CustomNPCNames.TownNPCs) {
                        packet.Write(NPCs.CustomNPC.isMale[i]);
                    }
                    break;
                case SyncType.BUSY_FIELDS:
                    packet.Write(busyFields.Count);
                    foreach(BusyField i in busyFields) {
                        packet.Write(i.ID);
                        packet.Write(i.player);
                    }
                    break;
            }
            
        }
        public override void NetReceive(BinaryReader reader)
        {
            byte syncType = reader.ReadByte();
            Main.NewText("Receiving (" + syncType + ")! " + Main.time);
            switch (syncType) {
                case SyncType.MODE: {
                        var readMode = reader.ReadByte();
                        if (CustomNPCNames.WaitForServerResponse) { return; }
                        mode = readMode;
                    }
                    break;
                case SyncType.TRY_UNIQUE: {
                        var readTryUnique = reader.ReadBoolean();
                        if (CustomNPCNames.WaitForServerResponse) { return; }
                        tryUnique = readTryUnique;
                    }
                    break;
                case SyncType.NAME: {
                        var readId = reader.ReadInt16();
                        var readName = reader.ReadString();
                        if (CustomNPCNames.WaitForServerResponse) { return; }
                        short id = readId;
                        string name = readName;
                        if (name != null) {
                            NPC npc = NPCs.CustomNPC.FindFirstNPC(id);
                            if (npc != null) {
                                npc.GivenName = name;
                            }
                        }
                    }
                    break;
                case SyncType.GENDER: {
                        var readId = reader.ReadInt16();
                        var readIsMale = reader.ReadBoolean();
                        if (CustomNPCNames.WaitForServerResponse) { return; }
                        short id = readId;
                        NPCs.CustomNPC.isMale[id] = readIsMale;
                    }
                    break;
                case SyncType.CUSTOM_NAMES: {
                        var readId = reader.ReadInt16();
                        var readCount = reader.ReadInt32();
                        string[] readNames = new string[readCount];
                        ulong[] readIDs = new ulong[readCount];
                        for (int i = 0; i < readCount; i++) {
                            readNames[i] = reader.ReadString();
                            readIDs[i] = reader.ReadUInt64();
                        }
                        if (CustomNPCNames.WaitForServerResponse) { return; }
                        short id = readId;
                        int count = readCount;
                        while (CustomNames[id].Count < count) { CustomNames[id].Add(""); }
                        while (CustomNames[id].Count > count) { CustomNames[id].RemoveAt(0); }
                        for (int i = 0; i < count; i++) {
                            CustomNames[id][i] = new StringWrapper(ref readNames[i], readIDs[i]);
                        }

                        if (Main.netMode == NetmodeID.MultiplayerClient) {
                            updateNameList = true;
                        }
                    }
                    break;
                case SyncType.EVERYTHING: {
                        var readMode = reader.ReadByte();
                        var readTryUnique = reader.ReadBoolean();
                        int[] readSize = new int[27];
                        StringWrapper[][] readWrappers = new StringWrapper[27][];
                        {
                            int j = 0;
                            foreach (KeyValuePair<short, List<StringWrapper>> i in CustomNames) {
                                readSize[j] = reader.ReadInt32();
                                readWrappers[j] = new StringWrapper[readSize[j]];
                                for (int k = 0; k < readSize[j]; k++) {
                                    string name = reader.ReadString();
                                    ulong id = reader.ReadUInt64();
                                    readWrappers[j][k] = new StringWrapper(ref name, id);
                                }
                                j++;
                            }
                        }
                        bool[] readIsMale = new bool[24];
                        for (int i = 0; i < 24; i++) {
                            readIsMale[i] = reader.ReadBoolean();
                        }
                        if (CustomNPCNames.WaitForServerResponse) { return; }
                        mode = readMode;
                        tryUnique = readTryUnique;
                        {
                            int j = 0;
                            foreach (KeyValuePair<short, List<StringWrapper>> i in CustomNames) {
                                while (i.Value.Count < readSize[j]) { i.Value.Add(""); }
                                while (i.Value.Count > readSize[j]) { i.Value.RemoveAt(0); }
                                for (int k = 0; k < readSize[j]; k++) {
                                    i.Value[k] = new StringWrapper(ref readWrappers[j][k].str, readWrappers[j][k].ID);
                                }
                                j++;
                            }

                            j = 0;
                            foreach (short i in CustomNPCNames.TownNPCs) {
                                NPCs.CustomNPC.isMale[i] = readIsMale[j++];
                            }
                        }
                        if (Main.netMode == NetmodeID.MultiplayerClient) {
                            updateNameList = true;
                        }
                    }
                    break;
                case SyncType.BUSY_FIELDS: {
                        var fieldCount = reader.ReadInt32();
                        BusyField[] busy = new BusyField[fieldCount];
                        for (int i = 0; i < fieldCount; i++) {
                            ulong id = reader.ReadUInt64();
                            byte player = reader.ReadByte();
                            busy[i] = new BusyField(id, player);
                        }
                        if (CustomNPCNames.WaitForServerResponse) { return; }
                        busyFields.Clear();
                        busyFields.AddRange(busy);
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Small container for ulong ID of a StringWrapper and player indice. Used to determine which entries are occupied and by whom.
    /// </summary>
    public struct BusyField
    {
        public readonly ulong ID;      // StringWrapper.ID for UINameFields, values from 1-24 for UIRenamePanel
        public readonly byte player;   // 255 for no one, 0-254 for player indices

        public BusyField(ulong ID, byte player)
        {
            this.ID = ID;
            this.player = player;
        }
    }
}
