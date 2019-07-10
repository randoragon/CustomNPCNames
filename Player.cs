using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameInput;
using Microsoft.Xna.Framework;

namespace CustomNPCNames
{
    class Player : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (CustomNPCNames.RenameMenuHotkey.ToString() != "")
            {
                CustomNPCNames.renameUI.closeButton.HoverText = "Close (" + CustomNPCNames.RenameMenuHotkey.GetAssignedKeys()[0] + ')';
            }

            if (CustomNPCNames.RenameMenuHotkey.JustPressed)
            {
                UI.RenameUI.Visible = !UI.RenameUI.Visible;
                Main.NewText(UI.RenameUI.Visible ? "Menu: ON" : "Menu: OFF", new Color(255, 255, 255));
            }
        }
    }
}