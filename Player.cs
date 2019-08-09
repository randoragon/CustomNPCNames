using Terraria.ModLoader;
using Terraria.GameInput;
using CustomNPCNames.UI;
using Terraria;
using Microsoft.Xna.Framework.Input;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace CustomNPCNames
{
    class Player : ModPlayer
    {
        public override void OnEnterWorld(Terraria.Player player)
        {
            Network.ModSync.SyncWorldData(Network.SyncType.EVERYTHING);
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (!Main.gameMenu) {

                if (CustomNPCNames.RenameMenuHotkey.JustPressed) {
                    
                    if (RenameUI.Visible) {
                        RenameUI.renamePanel.UpdateState();
                        Main.PlaySound(SoundID.MenuClose);
                        RenameUI.Visible = false;
                    } else {
                        Main.PlaySound(SoundID.MenuOpen);
                        RenameUI.Visible = true;
                    }
                }

                // FOR DEBUGGING
                if (Main.keyState.IsKeyDown(Keys.P) && !Main.oldKeyState.IsKeyDown(Keys.P)) {
                    Network.ModSync.SyncWorldData(Network.SyncType.EVERYTHING);
                }

                if (Main.keyState.IsKeyDown(Keys.I) && !Main.oldKeyState.IsKeyDown(Keys.I)) {
                    Network.PacketSender.SendPacketToServer(100, RenameUI.SelectedNPC);
                }

                if (Main.keyState.IsKeyDown(Keys.L) && !Main.oldKeyState.IsKeyDown(Keys.L)) {
                    CustomNPCNames.WaitForServerResponse = !CustomNPCNames.WaitForServerResponse;
                }

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