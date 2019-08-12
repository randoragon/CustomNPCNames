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
                    Network.PacketSender.SendPacketToServer(Network.PacketType.SEND_BUSY_FIELD, (short)Main.myPlayer, 22); //39950537469429693
                }

                if (Main.keyState.IsKeyDown(Keys.U) && !Main.oldKeyState.IsKeyDown(Keys.U)) {
                    Network.PacketSender.SendPacketToServer(Network.PacketType.SEND_BUSY_FIELD, 255, 22); //39950537469429693
                }

                if (Main.keyState.IsKeyDown(Keys.L) && !Main.oldKeyState.IsKeyDown(Keys.L)) {
                    Network.PacketSender.SendPacketToServer(Network.PacketType.RANDOMIZE, RenameUI.SelectedNPC);
                }

                if (Main.keyState.IsKeyDown(Keys.O) && !Main.oldKeyState.IsKeyDown(Keys.O)) {
                    PrintBusyFields();
                }
            }

            void PrintBusyFields()
            {
                Main.NewText("myPlayer: " + Main.myPlayer);
                foreach (var i in CustomWorld.busyFields) {
                    Main.NewText(string.Format("ID: {0}; player: {1};", i.ID, i.player));
                }
            }

            void PrintCustomNames()
            {
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