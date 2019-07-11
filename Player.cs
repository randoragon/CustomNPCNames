using Terraria.ModLoader;
using Terraria.GameInput;
using CustomNPCNames.UI;
using Terraria;

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
                    CustomNPCNames.renameUI.closeButton.HoverText = "Close (" + CustomNPCNames.RenameMenuHotkey.GetAssignedKeys()[0] + ')';
                }

                if (CustomNPCNames.RenameMenuHotkey.JustPressed)
                {
                    RenameUI.Visible = !RenameUI.Visible;
                    if (RenameUI.Visible)
                    {
                        CustomNPCNames.renameUI.renameBox.UpdateStatus();
                    }
                }
            }
        }
    }
}