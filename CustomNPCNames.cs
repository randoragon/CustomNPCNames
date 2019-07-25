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
        public static readonly Dictionary<short, Vector2> npcHeadOffset = new Dictionary<short, Vector2>() {
            { NPCID.Guide,              new Vector2(0, 0) },
            { NPCID.Merchant,           new Vector2(3, 3) },
            { NPCID.Nurse,              new Vector2(0, 0) },
            { NPCID.Demolitionist,      new Vector2(0, 0) },
            { NPCID.DyeTrader,          new Vector2(0, 0) },
            { NPCID.Dryad,              new Vector2(0, 0) },
            { NPCID.DD2Bartender,       new Vector2(-2, 0)},
            { NPCID.ArmsDealer,         new Vector2(0, 0) },
            { NPCID.Stylist,            new Vector2(0, -1)},
            { NPCID.Painter,            new Vector2(-2, 0)},
            { NPCID.Angler,             new Vector2(0, 0) },
            { NPCID.GoblinTinkerer,     new Vector2(-2, 0)},
            { NPCID.WitchDoctor,        new Vector2(0, 0) },
            { NPCID.Clothier,           new Vector2(-1, 0)},
            { NPCID.Mechanic,           new Vector2(0, 0) },
            { NPCID.PartyGirl,          new Vector2(-1, 0)},
            { NPCID.Wizard,             new Vector2(0, 0) },
            { NPCID.TaxCollector,       new Vector2(0, 0) },
            { NPCID.Truffle,            new Vector2(0, 0) },
            { NPCID.Pirate,             new Vector2(0, 0) },
            { NPCID.Steampunker,        new Vector2(0, 0) },
            { NPCID.Cyborg,             new Vector2(0, 0) },
            { NPCID.SantaClaus,         new Vector2(0, 0) },
            { NPCID.TravellingMerchant, new Vector2(2, -3)},
            { 1000,                     new Vector2(0, 0) }, // male
            { 1001,                     new Vector2(0, 0) }, // female
            { 1002,                     new Vector2(0, 0) }  // global
        };
        public static ModHotKey RenameMenuHotkey;
        public static RenameUI renameUI;
        private static UserInterface renameInterface;

        public override void Load()
        {
            // this makes sure that the UI doesn't get opened on the server console
            if (!Main.dedServ)
            {
                RenameMenuHotkey = RegisterHotKey("Toggle Menu", "K");
                renameUI = new RenameUI();
                renameUI.Initialize();
                renameInterface = new UserInterface();
                renameInterface.SetState(renameUI);
            }
        }

        public override void Unload()
        {
            renameUI = null;
            renameInterface = null;
            RenameMenuHotkey = null;
            CustomWorld.Unload();
            NPCs.CustomNPC.Unload();
            RenameUI.Unload();
            UINPCButton.Unload();

            base.Unload();
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
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer("CustomNPCMod: Menu UI", DrawRenameMenuUI, InterfaceScaleType.UI));
            }
        }

        public override void PreSaveAndQuit()
        {
            base.PreSaveAndQuit();
            CustomWorld.saveAndExit = true;
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

        public static string GetNPCName(short id)
        {
            switch (id)
            {
                case NPCID.Guide:
                    return "Guide";
                case NPCID.Merchant:
                    return "Merchant";
                case NPCID.Nurse:
                    return "Nurse";
                case NPCID.Demolitionist:
                    return "Demolitionist";
                case NPCID.Dryad:
                    return "Dryad";
                case NPCID.ArmsDealer:
                    return "Arms Dealer";
                case NPCID.Clothier:
                    return "Clothier";
                case NPCID.Mechanic:
                    return "Mechanic";
                case NPCID.GoblinTinkerer:
                    return "Goblin Tinkerer";
                case NPCID.Wizard:
                    return "Wizard";
                case NPCID.SantaClaus:
                    return "Santa Claus";
                case NPCID.Truffle:
                    return "Truffle";
                case NPCID.Steampunker:
                    return "Steampunker";
                case NPCID.DyeTrader:
                    return "Dye Trader";
                case NPCID.PartyGirl:
                    return "Party Girl";
                case NPCID.Cyborg:
                    return "Cyborg";
                case NPCID.Painter:
                    return "Painter";
                case NPCID.WitchDoctor:
                    return "Witch Doctor";
                case NPCID.Pirate:
                    return "Pirate";
                case NPCID.Stylist:
                    return "Stylist";
                case NPCID.TravellingMerchant:
                    return "Travelling Merchant";
                case NPCID.Angler:
                    return "Angler";
                case NPCID.TaxCollector:
                    return "Tax Collector";
                case NPCID.DD2Bartender:
                    return "Tavernkeep";
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// Makes it possible to store a string by mutable object reference
    /// </summary>
    public class StringWrapper
    {
        public string str;

        public StringWrapper(ref string str)
        { this.str = str; }

        public static explicit operator string(StringWrapper wr)
        { return wr.str; }

        public static implicit operator StringWrapper(string str)
        { return new StringWrapper(ref str); }

        public override string ToString()
        { return str; }

        public override bool Equals(object obj)
        {
            if (obj != null && GetType().Equals(obj.GetType())) {
                return str == ((StringWrapper)obj).str;
            } else {
                return false;
            }
        }

        public static List<StringWrapper> ConvertList(IList<string> list)
        {
            var ret = new List<StringWrapper>();
            foreach (string s in list)
            {
                ret.Add(s);
            }

            return ret;
        }

        public static bool ListContains(IList<StringWrapper> list, string value)
        {
            foreach (var i in list) {
                if ((string)i == value) {
                    return true;
                }
            }

            return false;
        }
    }
}
