using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.ID;
using CustomNPCNames.UI;
using Terraria.UI;
using Terraria;
using Microsoft.Xna.Framework;
using System;

namespace CustomNPCNames
{
    class CustomNPCNames : Mod
    {
        
        public static readonly short[] TownNPCs = {
            NPCID.Guide,         NPCID.Merchant,        NPCID.Nurse,
            NPCID.Demolitionist, NPCID.DyeTrader,       NPCID.Dryad,
            NPCID.DD2Bartender,  NPCID.ArmsDealer,      NPCID.Stylist,
            NPCID.Painter,       NPCID.Angler,          NPCID.GoblinTinkerer,
            NPCID.WitchDoctor,   NPCID.Clothier,        NPCID.Mechanic,
            NPCID.PartyGirl,     NPCID.Wizard,          NPCID.TaxCollector,
            NPCID.Truffle,       NPCID.Pirate,          NPCID.Steampunker,
            NPCID.Cyborg,        NPCID.SantaClaus,      NPCID.TravellingMerchant
        };
        public static ModHotKey RenameMenuHotkey;
        public static RenameUI renameUI;
        private static UserInterface renameInterface;

        public override void Load()
        {
            RenameMenuHotkey = RegisterHotKey("Toggle Menu", "K");
            // this makes sure that the UI doesn't get opened on the server
            // the server can't see UI, can it? it's just a command prompt
            if (!Main.dedServ)
            {
                renameUI = new RenameUI();
                renameUI.Initialize();
                renameInterface = new UserInterface();
                renameInterface.SetState(renameUI);
            }
        }
        
        public override void UpdateUI(GameTime gameTime)
        {
            // it will only draw if the player is not on the main menu
            if (!Main.gameMenu && RenameUI.Visible)
            {
                renameInterface.Update(gameTime);
            }
        }
        
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "CustomNPCMod: Menu UI",
                    delegate {
                        if (RenameUI.Visible)
                        {
                            renameInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
        
        private bool DrawRenameMenuUI()
        {
            // it will only draw if the player is not on the main menu
            if (!Main.gameMenu && RenameUI.Visible)
            {
                renameInterface.Draw(Main.spriteBatch, new GameTime());
            }
            return true;
        }

        public override void Unload()
        {
            RenameMenuHotkey = null;
            renameUI = null;
            renameInterface = null;
        }
    }
}
