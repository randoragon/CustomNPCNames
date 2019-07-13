using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.ModLoader.IO;

namespace CustomNPCNames
{
    public class CustomWorld : ModWorld
    {
        public static Dictionary<short, List<string>> CustomNames;

        public override void Initialize()
        {
            ResetCustomNames();
        }

        public void ResetCustomNames()
        {
            CustomNames = new Dictionary<short, List<string>>();
            foreach (short i in CustomNPCNames.TownNPCs)
            {
                CustomNames.Add(i, new List<string>());
            }

            CustomNames.Add(1000, new List<string>()); // 
            CustomNames.Add(1001, new List<string>()); // for Male, Female and Global respectively
            CustomNames.Add(1002, new List<string>()); // 
        }

        public override TagCompound Save()
        {
            base.Save();

            var testl = new List<string>();
            testl.Add("Randoragon");
            testl.Add("Evilflame");
            testl.Add("Mahlarian");
            testl.Add("Deihmlehr");
            testl.Add("Luis");
            testl.Add("Mixen");
            testl.Add("Wolf");

            TagCompound tag = new TagCompound();
            tag.Add("guide",              testl);
            tag.Add("merchant",           CustomNames[NPCID.Merchant]);
            tag.Add("nurse",              CustomNames[NPCID.Nurse]);
            tag.Add("demolitionist",      CustomNames[NPCID.Demolitionist]);
            tag.Add("dyetrader",          CustomNames[NPCID.DyeTrader]);
            tag.Add("dryad",              CustomNames[NPCID.Dryad]);
            tag.Add("tavernkeep",         CustomNames[NPCID.DD2Bartender]);
            tag.Add("armsdealer",         CustomNames[NPCID.ArmsDealer]);
            tag.Add("stylist",            CustomNames[NPCID.Stylist]);
            tag.Add("painter",            CustomNames[NPCID.Painter]);
            tag.Add("angler",             CustomNames[NPCID.Angler]);
            tag.Add("goblintinkerer",     CustomNames[NPCID.GoblinTinkerer]);
            tag.Add("witchdoctor",        CustomNames[NPCID.WitchDoctor]);
            tag.Add("clothier",           CustomNames[NPCID.Clothier]);
            tag.Add("mechanic",           CustomNames[NPCID.Mechanic]);
            tag.Add("partygirl",          CustomNames[NPCID.PartyGirl]);
            tag.Add("wizard",             CustomNames[NPCID.Wizard]);
            tag.Add("taxcollector",       CustomNames[NPCID.TaxCollector]);
            tag.Add("truffle",            CustomNames[NPCID.Truffle]);
            tag.Add("pirate",             CustomNames[NPCID.Pirate]);
            tag.Add("steampunker",        CustomNames[NPCID.Steampunker]);
            tag.Add("cyborg",             CustomNames[NPCID.Cyborg]);
            tag.Add("santaclaus",         CustomNames[NPCID.SantaClaus]);
            tag.Add("travellingmerchant", CustomNames[NPCID.TravellingMerchant]);
            tag.Add("male",               CustomNames[1000]);
            tag.Add("female",             CustomNames[1001]);
            tag.Add("global",             CustomNames[1002]);

            return tag;
        }

        public override void Load(TagCompound tag)
        {
            if (tag.ContainsKey("guide"))
            {
                CustomNames[NPCID.Guide]              = (List<string>)tag.GetList<string>("guide");
                CustomNames[NPCID.Merchant]           = (List<string>)tag.GetList<string>("merchant");
                CustomNames[NPCID.Nurse]              = (List<string>)tag.GetList<string>("nurse");
                CustomNames[NPCID.Demolitionist]      = (List<string>)tag.GetList<string>("demolitionist");
                CustomNames[NPCID.DyeTrader]          = (List<string>)tag.GetList<string>("dyetrader");
                CustomNames[NPCID.Dryad]              = (List<string>)tag.GetList<string>("dryad");
                CustomNames[NPCID.DD2Bartender]       = (List<string>)tag.GetList<string>("tavernkeep");
                CustomNames[NPCID.ArmsDealer]         = (List<string>)tag.GetList<string>("armsdealer");
                CustomNames[NPCID.Stylist]            = (List<string>)tag.GetList<string>("stylist");
                CustomNames[NPCID.Painter]            = (List<string>)tag.GetList<string>("painter");
                CustomNames[NPCID.Angler]             = (List<string>)tag.GetList<string>("angler");
                CustomNames[NPCID.GoblinTinkerer]     = (List<string>)tag.GetList<string>("goblintinkerer");
                CustomNames[NPCID.WitchDoctor]        = (List<string>)tag.GetList<string>("witchdoctor");
                CustomNames[NPCID.Clothier]           = (List<string>)tag.GetList<string>("clothier");
                CustomNames[NPCID.Mechanic]           = (List<string>)tag.GetList<string>("mechanic");
                CustomNames[NPCID.PartyGirl]          = (List<string>)tag.GetList<string>("partygirl");
                CustomNames[NPCID.Wizard]             = (List<string>)tag.GetList<string>("wizard");
                CustomNames[NPCID.TaxCollector]       = (List<string>)tag.GetList<string>("taxcollector");
                CustomNames[NPCID.Truffle]            = (List<string>)tag.GetList<string>("truffle");
                CustomNames[NPCID.Pirate]             = (List<string>)tag.GetList<string>("pirate");
                CustomNames[NPCID.Steampunker]        = (List<string>)tag.GetList<string>("steampunker");
                CustomNames[NPCID.Cyborg]             = (List<string>)tag.GetList<string>("cyborg");
                CustomNames[NPCID.SantaClaus]         = (List<string>)tag.GetList<string>("santaclaus");
                CustomNames[NPCID.TravellingMerchant] = (List<string>)tag.GetList<string>("travellingmerchant");
                CustomNames[1000]                     = (List<string>)tag.GetList<string>("male");
                CustomNames[1001]                     = (List<string>)tag.GetList<string>("female");
                CustomNames[1002]                     = (List<string>)tag.GetList<string>("global");
            } else
            {
                ResetCustomNames();
            }
        }
    }
}
