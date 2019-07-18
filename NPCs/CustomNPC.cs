using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.ID;

namespace CustomNPCNames.NPCs
{
    class CustomNPC : GlobalNPC
    {
        public  static Dictionary<short, string> currentNames;
        public  static Dictionary<short, bool>   isMale;
        private static Dictionary<short, ushort> npcCount;
        private static Dictionary<short, ushort> npcCountPrev;

        /// <summary>
        /// Determines whether or not an NPC of the given type has spawned since last time UpdateNPCCount() was called for that NPC.
        /// </summary>
        /// <remarks>
        /// It's neccessary because in some cases the SetDefaults() method is called even when an NPC is NOT spawning, and then the NPC name randomizes pointlessly.
        /// </remarks>
        public static bool HasNewSpawned(short type)
        {
            return npcCount[type] > npcCountPrev[type];
        }

        // this gets called in CustomWorld.PostUpdate()
        public static void UpdateNPCCount()
        {
            foreach (short id in CustomNPCNames.TownNPCs) {
                npcCountPrev[id] = npcCount[id];
                npcCount[id] = (ushort)NPC.CountNPCS(id);
            }
        }

        public CustomNPC()
        {
            currentNames = new Dictionary<short, string>();
            ResetCurrentNames();
            isMale = new Dictionary<short, bool>();
            ResetCurrentGender();

            npcCount = new Dictionary<short, ushort>();
            npcCountPrev = new Dictionary<short, ushort>();
            foreach (short i in CustomNPCNames.TownNPCs)
            {
                npcCount.Add(i, (ushort)NPC.CountNPCS(i));
                npcCountPrev.Add(i, npcCount[i]);
            }
        }

        public static void ResetCurrentNames()
        {
            currentNames.Clear();
            foreach (short i in CustomNPCNames.TownNPCs)
            {
                currentNames.Add(i, null);
            }
        }

        public static void ResetCurrentGender()
        {
            isMale.Clear();
            isMale.Add(NPCID.Guide,              true);
            isMale.Add(NPCID.Merchant,           true);
            isMale.Add(NPCID.Nurse,              false);
            isMale.Add(NPCID.Demolitionist,      true);
            isMale.Add(NPCID.DyeTrader,          true);
            isMale.Add(NPCID.Dryad,              false);
            isMale.Add(NPCID.DD2Bartender,       true);
            isMale.Add(NPCID.ArmsDealer,         true);
            isMale.Add(NPCID.Stylist,            false);
            isMale.Add(NPCID.Painter,            true);
            isMale.Add(NPCID.Angler,             true);
            isMale.Add(NPCID.GoblinTinkerer,     true);
            isMale.Add(NPCID.WitchDoctor,        true);
            isMale.Add(NPCID.Clothier,           true);
            isMale.Add(NPCID.Mechanic,           false);
            isMale.Add(NPCID.PartyGirl,          false);
            isMale.Add(NPCID.Wizard,             true);
            isMale.Add(NPCID.TaxCollector,       true);
            isMale.Add(NPCID.Truffle,            true);
            isMale.Add(NPCID.Pirate,             true);
            isMale.Add(NPCID.Steampunker,        false);
            isMale.Add(NPCID.Cyborg,             true);
            isMale.Add(NPCID.SantaClaus,         true);
            isMale.Add(NPCID.TravellingMerchant, true);
        }

        public static void RandomizeName(short type)
        {
            var list = new List<StringWrapper>();

            switch (CustomNPCNames.mode) {
                case 1: // Custom Names mode
                    if (CustomWorld.CustomNames[type].Count != 0) {

                        list = CustomWorld.CustomNames[type];
                    }
                    break;
                case 2: // Gender Names mode
                    if (CustomWorld.CustomNames[(short)(isMale[type] ? 1000 : 1001)].Count != 0) {
                        list = CustomWorld.CustomNames[(short)(isMale[type] ? 1000 : 1001)];
                    }
                    break;
                case 3: // Global Names mode
                    if (CustomWorld.CustomNames[1002].Count != 0) {
                        list = CustomWorld.CustomNames[1002];
                    }
                    break;
            }

            if (CustomNPCNames.mode != 0) {
                currentNames[type] = (string)list[Main.rand.Next(list.Count)];
            }
        }

        public override void SetDefaults(NPC npc)
        {
            if (CustomWorld.CustomNames != null) {
                UpdateNPCCount();
                foreach (short i in CustomNPCNames.TownNPCs) {
                    if (npc.type == i && HasNewSpawned(i)) {
                        RandomizeName(i);
                    }
                }
            }
        }

        public override bool PreAI(NPC npc)
        {
            if (currentNames.ContainsKey((short)npc.type))
            {
                if (currentNames[(short)npc.type] != null && npc.GivenName != currentNames[(short)npc.type])
                {
                    npc.GivenName = currentNames[(short)npc.type];
                }
            }
            return true;
        }
    }
}
