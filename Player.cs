using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameInput;

namespace CustomNPCNames
{
    class Player : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (CustomNPCNames.RenameMenuHotkey.JustPressed)
            {
                UI.RenameUI.Visible = !UI.RenameUI.Visible;
                CustomNPCNames.CustomNames[NPCID.Guide] = (UI.RenameUI.Visible) ? "Poopy Face" : "Randoragon";
            }
        }
    }
}