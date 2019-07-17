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
            foreach (short i in CustomNPCNames.TownNPCs)
            {
                npcCountPrev[i] = npcCount[i];
                npcCount[i] = (ushort)NPC.CountNPCS(i);
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
            isMale.Add(NPCID.Guide, true);
            isMale.Add(NPCID.Merchant, true);
            isMale.Add(NPCID.Nurse, false);
            isMale.Add(NPCID.Demolitionist, true);
            isMale.Add(NPCID.DyeTrader, true);
            isMale.Add(NPCID.Dryad, false);
            isMale.Add(NPCID.DD2Bartender, true);
            isMale.Add(NPCID.ArmsDealer, true);
            isMale.Add(NPCID.Stylist, false);
            isMale.Add(NPCID.Painter, true);
            isMale.Add(NPCID.Angler, true);
            isMale.Add(NPCID.GoblinTinkerer, true);
            isMale.Add(NPCID.WitchDoctor, true);
            isMale.Add(NPCID.Clothier, true);
            isMale.Add(NPCID.Mechanic, false);
            isMale.Add(NPCID.PartyGirl, false);
            isMale.Add(NPCID.Wizard, true);
            isMale.Add(NPCID.TaxCollector, true);
            isMale.Add(NPCID.Truffle, true);
            isMale.Add(NPCID.Pirate, true);
            isMale.Add(NPCID.Steampunker, false);
            isMale.Add(NPCID.Cyborg, true);
            isMale.Add(NPCID.SantaClaus, true);
            isMale.Add(NPCID.TravellingMerchant, true);
        }

        public static void RandomizeName(short type)
        {
            var list = CustomWorld.CustomNames[type];
            currentNames[type] = (string)list[Main.rand.Next(list.Count)];
        }

        public override void SetDefaults(NPC npc)
        {
            if (CustomWorld.CustomNames != null)
            {
                foreach (short i in CustomNPCNames.TownNPCs)
                {
                    UpdateNPCCount();
                    if (npc.type == i && HasNewSpawned(i) && CustomWorld.CustomNames[i].Count != 0)
                    {
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
