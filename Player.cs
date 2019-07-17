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
                    CustomNPCNames.renameUI.Visible = !CustomNPCNames.renameUI.Visible;
                    if (CustomNPCNames.renameUI.Visible)
                    {
                        CustomNPCNames.renameUI.renamePanel.UpdateState();
                    }
                }
            }
        }
    }
}