using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using System;

namespace CustomNPCNames.NPCs
{
    class ModdedNames : GlobalNPC
    {
        public static Dictionary<short, string> currentNames;

        public ModdedNames()
        {
            currentNames = new Dictionary<short, string>();
            ResetCurrentNames();
        }

        public static void ResetCurrentNames()
        {
            currentNames.Clear();
            foreach (short i in CustomNPCNames.TownNPCs)
            {
                currentNames.Add(i, null);
            }
        }

        public override void SetDefaults(NPC npc)
        {
            if (CustomWorld.CustomNames != null)
            {
                foreach (KeyValuePair<short, List<string>> i in CustomWorld.CustomNames)
                {
                    if (npc.type == i.Key && i.Value.Count != 0)
                    {
                        currentNames[(short)npc.type] = i.Value[Main.rand.Next(i.Value.Count)];
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
