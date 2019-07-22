using Terraria.ModLoader;
using Terraria.GameInput;
using CustomNPCNames.UI;
using Terraria;
using Microsoft.Xna.Framework.Input;

namespace CustomNPCNames
{
    class Player : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (!Main.gameMenu)
            {
                if (CustomNPCNames.RenameMenuHotkey.ToString() != "")
                {
                    RenameUI.closeButton.HoverText = "Close (" + CustomNPCNames.RenameMenuHotkey.GetAssignedKeys()[0] + ')';
                }

                if (CustomNPCNames.RenameMenuHotkey.JustPressed)
                {
                    RenameUI.Visible = !RenameUI.Visible;
                    if (RenameUI.Visible)
                    {
                        RenameUI.renamePanel.UpdateState();
                    }
                }
            }
        }
    }
}