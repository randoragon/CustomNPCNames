using Terraria.ModLoader;
using Terraria.GameInput;
using CustomNPCNames.UI;
using Terraria;
using Microsoft.Xna.Framework.Input;

namespace CustomNPCNames
{
    class Player : ModPlayer
    {
        public override void OnEnterWorld(Terraria.Player player)
        {
            CustomWorld.SyncWorldData();
        }

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
                if (Main.keyState.IsKeyDown(Keys.P) && !Main.oldKeyState.IsKeyDown(Keys.P)) {
                    RenameUI.panelList.PrintContent();
                }

                //if (Keyboard.GetState().IsKeyDown(Keys.L)) {
                    
                //}

                if (Main.keyState.IsKeyDown(Keys.O) && !Main.oldKeyState.IsKeyDown(Keys.O)) {
                    foreach (var i in CustomWorld.CustomNames) {
                        string line = i.Key + "(" + i.Value.Count + "): {";
                        foreach (var j in i.Value) {
                            if (j != null) { line += "\"" + j.ToString() + "\", "; } else { line += "NULL, "; }
                        }
                        line += "}";
                        Main.NewText(line);
                    }
                }
            }
        }
    }
}