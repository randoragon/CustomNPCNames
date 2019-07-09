using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.ID;

namespace CustomNPCNames
{
    class CustomNPCNames : Mod
    {
        public static Dictionary<int, string> CustomNames = new Dictionary<int, string>();
        private static readonly int[] TownNPCs = {
            NPCID.Guide, NPCID.Merchant, NPCID.Nurse,
            NPCID.Demolitionist, NPCID.DyeTrader, NPCID.Dryad,
            NPCID.DD2Bartender, NPCID.ArmsDealer, NPCID.Stylist,
            NPCID.Painter, NPCID.Angler, NPCID.GoblinTinkerer,
            NPCID.WitchDoctor, NPCID.Clothier, NPCID.Mechanic,
            NPCID.PartyGirl, NPCID.Wizard, NPCID.TaxCollector,
            NPCID.Truffle, NPCID.Pirate, NPCID.Steampunker,
            NPCID.Cyborg, NPCID.SantaClaus, NPCID.TravellingMerchant,
            NPCID.OldMan, NPCID.SkeletonMerchant
        };
        public static ModHotKey RenameMenuHotkey;

        public CustomNPCNames()
        {
            //string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
            foreach (int i in TownNPCs) {
                CustomNames.Add(i, "CustomName");
            }
        }

        public override void Load()
        {
            RenameMenuHotkey = RegisterHotKey("Toggle Menu", "K");
        }

        public override void Unload()
        {
            RenameMenuHotkey = null;
        }
    }
}
