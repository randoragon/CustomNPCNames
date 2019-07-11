using Terraria.ModLoader;
using Terraria.GameInput;

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
            }
        }
    }
}