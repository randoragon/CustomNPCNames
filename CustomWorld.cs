using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.ModLoader.IO;

namespace CustomNPCNames
{
    public class CustomWorld : ModWorld
    {
        public static Dictionary<short, List<StringWrapper>> CustomNames;

        public override void Initialize()
        {
            ResetCustomNames();
        }

        public void ResetCustomNames()
        {
            CustomNames = new Dictionary<short, List<StringWrapper>>();
            foreach (short i in CustomNPCNames.TownNPCs)
            {
                CustomNames.Add(i, new List<StringWrapper>());
            }

            CustomNames.Add(1000, new List<StringWrapper>()); // 
            CustomNames.Add(1001, new List<StringWrapper>()); // for Male, Female and Global respectively
            CustomNames.Add(1002, new List<StringWrapper>()); // 
        }

        

        public override TagCompound Save()
        {
            base.Save();

            Dictionary<short, List<string>> nameStrings = new Dictionary<short, List<string>>();
            foreach (KeyValuePair<short, List<StringWrapper>> i in CustomNames)
            {
                List<string> list = new List<string>();
                foreach (StringWrapper j in i.Value)
                {
                    list.Add((string)j);
                }
                nameStrings.Add(i.Key, list);
            }

            TagCompound tag = new TagCompound();
            tag.Add("guide",              nameStrings[NPCID.Guide]);
            tag.Add("merchant",           nameStrings[NPCID.Merchant]);
            tag.Add("nurse",              nameStrings[NPCID.Nurse]);
            tag.Add("demolitionist",      nameStrings[NPCID.Demolitionist]);
            tag.Add("dyetrader",          nameStrings[NPCID.DyeTrader]);
            tag.Add("dryad",              nameStrings[NPCID.Dryad]);
            tag.Add("tavernkeep",         nameStrings[NPCID.DD2Bartender]);
            tag.Add("armsdealer",         nameStrings[NPCID.ArmsDealer]);
            tag.Add("stylist",            nameStrings[NPCID.Stylist]);
            tag.Add("painter",            nameStrings[NPCID.Painter]);
            tag.Add("angler",             nameStrings[NPCID.Angler]);
            tag.Add("goblintinkerer",     nameStrings[NPCID.GoblinTinkerer]);
            tag.Add("witchdoctor",        nameStrings[NPCID.WitchDoctor]);
            tag.Add("clothier",           nameStrings[NPCID.Clothier]);
            tag.Add("mechanic",           nameStrings[NPCID.Mechanic]);
            tag.Add("partygirl",          nameStrings[NPCID.PartyGirl]);
            tag.Add("wizard",             nameStrings[NPCID.Wizard]);
            tag.Add("taxcollector",       nameStrings[NPCID.TaxCollector]);
            tag.Add("truffle",            nameStrings[NPCID.Truffle]);
            tag.Add("pirate",             nameStrings[NPCID.Pirate]);
            tag.Add("steampunker",        nameStrings[NPCID.Steampunker]);
            tag.Add("cyborg",             nameStrings[NPCID.Cyborg]);
            tag.Add("santaclaus",         nameStrings[NPCID.SantaClaus]);
            tag.Add("travellingmerchant", nameStrings[NPCID.TravellingMerchant]);
            tag.Add("male",               nameStrings[1000]);
            tag.Add("female",             nameStrings[1001]);
            tag.Add("global",             nameStrings[1002]);

            return tag;
        }

        public override void Load(TagCompound tag)
        {
            if (tag.ContainsKey("guide"))
            {
                CustomNames[NPCID.Guide]              = StringWrapper.ConvertList(tag.GetList<string>("guide"));
                CustomNames[NPCID.Merchant]           = StringWrapper.ConvertList(tag.GetList<string>("merchant"));
                CustomNames[NPCID.Nurse]              = StringWrapper.ConvertList(tag.GetList<string>("nurse"));
                CustomNames[NPCID.Demolitionist]      = StringWrapper.ConvertList(tag.GetList<string>("demolitionist"));
                CustomNames[NPCID.DyeTrader]          = StringWrapper.ConvertList(tag.GetList<string>("dyetrader"));
                CustomNames[NPCID.Dryad]              = StringWrapper.ConvertList(tag.GetList<string>("dryad"));
                CustomNames[NPCID.DD2Bartender]       = StringWrapper.ConvertList(tag.GetList<string>("tavernkeep"));
                CustomNames[NPCID.ArmsDealer]         = StringWrapper.ConvertList(tag.GetList<string>("armsdealer"));
                CustomNames[NPCID.Stylist]            = StringWrapper.ConvertList(tag.GetList<string>("stylist"));
                CustomNames[NPCID.Painter]            = StringWrapper.ConvertList(tag.GetList<string>("painter"));
                CustomNames[NPCID.Angler]             = StringWrapper.ConvertList(tag.GetList<string>("angler"));
                CustomNames[NPCID.GoblinTinkerer]     = StringWrapper.ConvertList(tag.GetList<string>("goblintinkerer"));
                CustomNames[NPCID.WitchDoctor]        = StringWrapper.ConvertList(tag.GetList<string>("witchdoctor"));
                CustomNames[NPCID.Clothier]           = StringWrapper.ConvertList(tag.GetList<string>("clothier"));
                CustomNames[NPCID.Mechanic]           = StringWrapper.ConvertList(tag.GetList<string>("mechanic"));
                CustomNames[NPCID.PartyGirl]          = StringWrapper.ConvertList(tag.GetList<string>("partygirl"));
                CustomNames[NPCID.Wizard]             = StringWrapper.ConvertList(tag.GetList<string>("wizard"));
                CustomNames[NPCID.TaxCollector]       = StringWrapper.ConvertList(tag.GetList<string>("taxcollector"));
                CustomNames[NPCID.Truffle]            = StringWrapper.ConvertList(tag.GetList<string>("truffle"));
                CustomNames[NPCID.Pirate]             = StringWrapper.ConvertList(tag.GetList<string>("pirate"));
                CustomNames[NPCID.Steampunker]        = StringWrapper.ConvertList(tag.GetList<string>("steampunker"));
                CustomNames[NPCID.Cyborg]             = StringWrapper.ConvertList(tag.GetList<string>("cyborg"));
                CustomNames[NPCID.SantaClaus]         = StringWrapper.ConvertList(tag.GetList<string>("santaclaus"));
                CustomNames[NPCID.TravellingMerchant] = StringWrapper.ConvertList(tag.GetList<string>("travellingmerchant"));
                CustomNames[1000]                     = StringWrapper.ConvertList(tag.GetList<string>("male"));
                CustomNames[1001]                     = StringWrapper.ConvertList(tag.GetList<string>("female"));
                CustomNames[1002]                     = StringWrapper.ConvertList(tag.GetList<string>("global"));
            } else
            {
                ResetCustomNames();
            }
        }

        public override void PreUpdate()
        {
            base.PostUpdate();
            NPCs.CustomNPC.UpdateNPCCount();
        }
    }
}
