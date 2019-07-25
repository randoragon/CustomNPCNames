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

                if (CustomNPCNames.RenameMenuHotkey.JustPressed)
                {
                    RenameUI.Visible = !RenameUI.Visible;
                    if (RenameUI.Visible)
                    {
                        RenameUI.renamePanel.UpdateState();
                    }
                }

                // FOR DEBUGGING
                //if (Keyboard.GetState().IsKeyDown(Keys.P)) {
                //    NPCs.CustomNPC.ResetCurrentNames();
                //}

                //if (Keyboard.GetState().IsKeyDown(Keys.L)) {
                //    Main.NewText(NPCs.CustomNPC.currentNames.Count);
                //    foreach (var i in NPCs.CustomNPC.currentNames) {
                //        if (i.Value != null) { Main.NewText(string.Format("CurrentNames[{0}]: \"{1}\"", i.Key, i.Value)); } else { Main.NewText(string.Format("CurrentNames[{0}]: NULL", i.Key)); }
                //    }
                //}

                //if (Keyboard.GetState().IsKeyDown(Keys.O)) {
                //    foreach (var i in CustomWorld.CustomNames) {
                //        string line = i.Key + "(" + i.Value.Count + "): {";
                //        foreach (var j in i.Value) {
                //            if (j != null) { line += "\"" + j.ToString() + "\", "; } else { line += "NULL, "; }
                //        }
                //        line += "}";
                //        Main.NewText(line);
                //    }
                //}
            }
        }
    }
}