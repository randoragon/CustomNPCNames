using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Collections.Generic;

namespace CustomNPCNames.NPCs
{
    class ModdedNames : GlobalNPC
    {
        public override void SetDefaults(NPC npc)
        {
            foreach (var i in CustomNPCNames.CustomNames)
            {
                if (npc.type == i.Key)
                {
                    npc.GivenName = i.Value;
                }
            }
        }

        public override bool PreAI(NPC npc)
        {
            foreach (var i in CustomNPCNames.CustomNames)
            {
                if (npc.type == i.Key && npc.GivenName != i.Value)
                {
                    npc.GivenName = i.Value;
                }
            }

            return true;
        }
    }

}
