using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.ID;
using System.Linq;

namespace CustomNPCNames.NPCs
{
    public class CustomNPC : GlobalNPC
    {
        public  static Dictionary<short, bool> isMale;
        public  static Dictionary<short, List<StringWrapper>> vanillaNames;
        

        public CustomNPC()
        {
            isMale = new Dictionary<short, bool>();
            ResetCurrentGender();

            // vanilla names need to be added manually to allow unique name randomization
            vanillaNames = new Dictionary<short, List<StringWrapper>>();
            vanillaNames.Add(NPCID.Guide,              new List<StringWrapper>() { "Andrew", "Asher", "Bradley", "Brandon", "Brett", "Brian", "Cody", "Cole", "Colin", "Connor", "Daniel", "Dylan", "Garrett", "Harley", "Jack", "Jacob", "Jake", "Jeff", "Jeffrey", "Joe", "Kevin", "Kyle", "Levi", "Logan", "Luke", "Marty", "Maxwell", "Ryan", "Scott", "Seth", "Steve", "Tanner", "Trent", "Wyatt", "Zach" });
            vanillaNames.Add(NPCID.Merchant,           new List<StringWrapper>() { "Alfred", "Barney", "Calvin", "Edmund", "Edwin", "Eugene", "Finn", "Frank", "Frederick", "Gilbert", "Gus", "Harold", "Howard", "Humphrey", "Isaac", "Joseph", "Louis", "Milton", "Mortimer", "Ralph", "Seymour" });
            vanillaNames.Add(NPCID.Nurse,              new List<StringWrapper>() { "Abigail", "Allison", "Amy", "Caitlin", "Carly", "Claire", "Emily", "Emma", "Hannah", "Heather", "Helen", "Holly", "Jenna", "Kaitlin", "Kaitlyn", "Katelyn", "Katherine", "Kathryn", "Katie", "Kayla", "Lisa", "Lorraine", "Madeline", "Molly" });
            vanillaNames.Add(NPCID.Demolitionist,      new List<StringWrapper>() { "Bazdin", "Beldin", "Boften", "Darur", "Dias", "Dolbere", "Dolgen", "Dolgrim", "Duerthen", "Durim", "Fikod", "Garval", "Gimli", "Gimut", "Jarut", "Morthal", "Norkas", "Norsun", "Oten", "Ovbere", "Tordak", "Urist" });
            vanillaNames.Add(NPCID.DyeTrader,          new List<StringWrapper>() { "Abdosir", "Ahinadab", "Ahirom", "Akbar", "Batnoam", "Bodashtart", "Danel", "Hannibal", "Hanno", "Hiram", "Kanmi", "Philosir", "Sikarbaal", "Tabnit", "Yehomilk", "Yutpan" });
            vanillaNames.Add(NPCID.Dryad,              new List<StringWrapper>() { "Alalia", "Alura", "Ariella", "Caelia", "Calista", "Celestia", "Chryseis", "Elysia", "Emerenta", "Evvie", "Faye", "Felicitae", "Isis", "Lunette", "Nata", "Nissa", "Rosalva", "Shea", "Tania", "Tatiana", "Xylia" });
            vanillaNames.Add(NPCID.DD2Bartender,       new List<StringWrapper>() { "Barkeep", "Bill", "Blacksmith", "Bruce", "Dale", "Dani Moo", "Driscan", "Elandrian", "Ernest", "Iamison", "Javahawk", "Jerry", "Moe", "Paddy", "Ted", "William" });
            vanillaNames.Add(NPCID.ArmsDealer,         new List<StringWrapper>() { "Andre", "Brimst", "Bronson", "Dante", "Darius", "Darnell", "Darryl", "DeAndre", "Demetrius", "DeShawn", "Dominique", "Jalen", "Jamal", "Malik", "Marquis", "Maurice", "Reginald", "Terrance", "Terrell", "Tony", "Trevon", "Tyrone", "Willie", "Xavier" });
            vanillaNames.Add(NPCID.Stylist,            new List<StringWrapper>() { "Annabel", "Biah", "Bri", "Brianne", "Esmeralda", "Flora", "Hazel", "Iris", "Kati", "Kylie", "Lola", "Meliyah", "Pearl", "Petra", "Rox", "Roxanne", "Ruby", "Scarlett", "Stella", "Tallulah" });
            vanillaNames.Add(NPCID.Painter,            new List<StringWrapper>() { "Bruno", "Carlo", "Darren", "Enzo", "Esreadel", "Guido", "Leonardo", "Lorenzo", "Luca", "Luciano", "Ludo", "Luigi", "Marco", "Mario", "Martino", "Mauro", "Raphael", "Stefano" });
            vanillaNames.Add(NPCID.Angler,             new List<StringWrapper>() { "Adam", "Bart", "Billy", "Bobby", "Bryce", "Charles", "Danny", "Grayson", "Ivan", "Izzy", "Jey", "Jimmy", "Johnny", "Matty", "Miles", "Nathan", "Phillip", "Sammy", "Simon", "Spencer", "Timmy", "Tyler" });
            vanillaNames.Add(NPCID.GoblinTinkerer,     new List<StringWrapper>() { "Arback", "Dalek", "Darz", "Durnok", "Fahd", "Fjell", "Gnudar", "Grodax", "Knogs", "Knub", "Mobart", "Mrunok", "Negurk", "Nort", "Nuxatk", "Ragz", "Sarx", "Smador", "Stazen", "Stezom", "Tgerd", "Tkanus", "Trogem", "Xanos", "Xon" });
            vanillaNames.Add(NPCID.WitchDoctor,        new List<StringWrapper>() { "Abibe", "Gboto", "Jamundi", "Kogi-ghi", "Konah", "Opuni", "Tairona", "U'wa", "Xirigua", "Zop'a" });
            vanillaNames.Add(NPCID.Clothier,           new List<StringWrapper>() { "Alfred", "Arthur", "Benjamin", "Cedric", "Clive", "Cyril", "Edgar", "Edmund", "Edward", "Eustace", "Fitz", "Graham", "Henry", "Herald", "James Desktop Version", "Lincoln", "Lloyd", "Mervyn", "Nigel", "Pip", "Rodney", "Rodrick", "Roland", "Rupert", "Sebastian" });
            vanillaNames.Add(NPCID.Mechanic,           new List<StringWrapper>() { "Amy", "Autumn", "Brooke", "Dawn", "Ella", "Ellen", "Ginger", "Jenny", "Kayla", "Korrie", "Lauren", "Marshanna", "Meredith", "Nancy", "Sally", "Selah", "Selene", "Shayna", "Sheena", "Shirlena", "Sophia", "Susana", "Terra", "Trisha" });
            vanillaNames.Add(NPCID.PartyGirl,          new List<StringWrapper>() { "Bailey", "Bambi", "Bunny", "Candy", "Cherry", "Dazzle", "Destiny", "Fantasia", "Fantasy", "Glitter", "Isis", "Lexus", "Paris", "Sparkle", "Star", "Sugar", "Trixy" });
            vanillaNames.Add(NPCID.Wizard,             new List<StringWrapper>() { "Abram", "Alasdair", "Arddun", "Arwyn", "Berwyn", "Dalamar", "Dulais", "Elric", "Fizban", "Gearroid", "Greum", "Gwentor", "Hirael", "Leomund", "Maelor", "Magius", "Merlyn", "Ningauble", "Sargon", "Seonag", "Tagar", "Xanadu" });
            vanillaNames.Add(NPCID.TaxCollector,       new List<StringWrapper>() { "McKinley", "Millard", "Fillmore", "Rutherford", "Chester", "Grover", "Cleveland", "Theodore", "Herbert", "Warren", "Lyndon", "Ronald", "Harrison", "Woodrow", "Tweed", "Blanton", "Dwyer", "Carroll", "Agnew" });
            vanillaNames.Add(NPCID.Truffle,            new List<StringWrapper>() { "Agaric", "Amanita", "Chanterelle", "Cremini", "Enoki", "Maitake", "Morel", "Muscaria", "Porcini", "Reishi", "Shiitake", "Shimeji" });
            vanillaNames.Add(NPCID.Pirate,             new List<StringWrapper>() { "Black Beard", "Captain Bullywort", "Captain Morgan", "Captain Stoney Dirt", "David", "Gunpowder Garry", "Jack", "Jake", "James T. Beard", "Red Beard", "Wet Beard" });
            vanillaNames.Add(NPCID.Steampunker,        new List<StringWrapper>() { "Ada", "Cornelia", "Cynthia", "Emeline", "Fidelia", "Hope", "Isabella", "Judith", "Leila", "Lilly", "Lydia", "Minerva", "Phoebe", "Savannah", "Selina", "Verity", "Vivian", "Whitney", "Zelda", "Zylphia" });
            vanillaNames.Add(NPCID.Cyborg,             new List<StringWrapper>() { "Alpha", "Beta", "Gamma", "Delta", "Zeta", "Theta", "Kappa", "Lambda", "Mu", "Nu", "Omicron", "Rho", "Sigma", "Upsilon", "Phi", "Ci", "Omega", "Fender", "T-3E0", "Niner-7", "A.N.D.Y", "Syn-X" });
            vanillaNames.Add(NPCID.SkeletonMerchant,   new List<StringWrapper>() { "Skellington", "Bones McGee", "Gloomy Mays", "Jack Sellington", "Billy Marrows", "Tom", "Rattles Magoo", "Mandible Calavera", "Mika", "No-Eyed Wiley" });
            vanillaNames.Add(NPCID.TravellingMerchant, new List<StringWrapper>() { "Abraham", "Aedan", "Aphraim", "Bohemas", "Eladon", "Gallius", "Llewellyn", "Mercer", "Rawleigh", "Riley", "Romeo", "Shipton", "Willy" });
        }

        public override bool Autoload(ref string name)
        {
            // Replace the way Vanilla assigns NPC names with the HookGetNewNPCName method
            On.Terraria.NPC.getNewNPCName += HookGetNewNPCName;
            return base.Autoload(ref name);
        }

        private string HookGetNewNPCName(On.Terraria.NPC.orig_getNewNPCName orig, int npcType)
        {

            foreach (short i in CustomNPCNames.TownNPCs) {
                if (i == npcType) {
                    return GetRandomName(i);
                }
            }

            return orig(npcType);
        }

        public static void Unload()
        {
            isMale = null;
            vanillaNames = null;
        }

        public static void ResetCurrentGender()
        {
            isMale.Clear();
            isMale.Add(NPCID.Guide,              true);
            isMale.Add(NPCID.Merchant,           true);
            isMale.Add(NPCID.Nurse,              false);
            isMale.Add(NPCID.Demolitionist,      true);
            isMale.Add(NPCID.DyeTrader,          true);
            isMale.Add(NPCID.Dryad,              false);
            isMale.Add(NPCID.DD2Bartender,       true);
            isMale.Add(NPCID.ArmsDealer,         true);
            isMale.Add(NPCID.Stylist,            false);
            isMale.Add(NPCID.Painter,            true);
            isMale.Add(NPCID.Angler,             true);
            isMale.Add(NPCID.GoblinTinkerer,     true);
            isMale.Add(NPCID.WitchDoctor,        true);
            isMale.Add(NPCID.Clothier,           true);
            isMale.Add(NPCID.Mechanic,           false);
            isMale.Add(NPCID.PartyGirl,          false);
            isMale.Add(NPCID.Wizard,             true);
            isMale.Add(NPCID.TaxCollector,       true);
            isMale.Add(NPCID.Truffle,            true);
            isMale.Add(NPCID.Pirate,             true);
            isMale.Add(NPCID.Steampunker,        false);
            isMale.Add(NPCID.Cyborg,             true);
            isMale.Add(NPCID.SkeletonMerchant,   true);
            isMale.Add(NPCID.TravellingMerchant, true);
        }

        public static string GetRandomName(short type)
        {
            List<StringWrapper> list;

            switch (CustomWorld.mode) {
                case 1: // Custom Names mode
                    list = (CustomWorld.CustomNames[type].Count != 0) ? new List<StringWrapper>(CustomWorld.CustomNames[type]) : new List<StringWrapper>(vanillaNames[type]);
                    break;
                case 2: // Gender Names mode
                    list = (CustomWorld.CustomNames[(short)(isMale[type] ? 1000 : 1001)].Count != 0) ? new List<StringWrapper>(CustomWorld.CustomNames[(short)(isMale[type] ? 1000 : 1001)]) : new List<StringWrapper>(vanillaNames[type]);
                    break;
                case 3: // Global Names mode
                    list = (CustomWorld.CustomNames[1002].Count != 0) ? new List<StringWrapper>(CustomWorld.CustomNames[1002]) : new List<StringWrapper>(vanillaNames[type]);
                    break;
                default: // Vanilla names mode
                    list = new List<StringWrapper>(vanillaNames[type]);
                    break;
            }
            if (CustomWorld.tryUnique) {
                var currentNames = new List<string>();
                foreach (NPC i in Main.npc) {
                    if (i.type != type && CustomNPCNames.TownNPCs.Contains((short)i.type)) {
                        currentNames.Add(i.GivenName);
                        if (currentNames.Count == 24) { break; } // 24 is how many elements CustomNPCNames.TownNPCs has
                    }
                }
                var listsIntersection = new List<StringWrapper>();
                foreach (StringWrapper i in list) {
                    if (currentNames.Contains(i.ToString())) {
                        listsIntersection.Add(i);
                    }
                }
                foreach (StringWrapper i in listsIntersection) {
                    list.Remove(i);
                }

                if (list.Count == 0) { list = listsIntersection; }
            }

            return list[WorldGen.genRand.Next(list.Count)].ToString();
        }

        public static void RandomizeName(short type)
        {
            if (Main.netMode == NetmodeID.SinglePlayer || Main.netMode == NetmodeID.Server) {
                if (type == 1000) {
                    foreach (NPC i in Main.npc) {
                        foreach (KeyValuePair<short, bool> j in isMale) {
                            if (j.Key == (short)i.type) {
                                if (j.Value && HasCustomNames(j.Key)) { i.GivenName = "\0"; }
                                break;
                            }
                        }
                    }
                    foreach (int i in Enumerable.Range(0, CustomNPCNames.TownNPCs.Length).OrderBy(x => Main.rand.Next())) {
                        short id = CustomNPCNames.TownNPCs[i];
                        if (isMale[id] && NPC.AnyNPCs(id) && HasCustomNames(id)) { RandomizeName(id); }
                    }
                    return;
                } else if (type == 1001) {
                    foreach (NPC i in Main.npc) {
                        foreach (KeyValuePair<short, bool> j in isMale) {
                            if (j.Key == (short)i.type) {
                                if (!j.Value && HasCustomNames(j.Key)) { i.GivenName = "\0"; }
                                break;
                            }
                        }
                    }
                    foreach (int i in Enumerable.Range(0, CustomNPCNames.TownNPCs.Length).OrderBy(x => Main.rand.Next())) {
                        short id = CustomNPCNames.TownNPCs[i];
                        if (!isMale[id] && NPC.AnyNPCs(id) && HasCustomNames(id)) { RandomizeName(id); }
                    }
                    return;
                } else if (type == 1002) {
                    foreach (NPC i in Main.npc) {
                        if (CustomNPCNames.TownNPCs.Contains((short)i.type) && HasCustomNames((short)i.type)) { i.GivenName = "\0"; }
                    }
                    foreach (int i in Enumerable.Range(0, CustomNPCNames.TownNPCs.Length).OrderBy(x => Main.rand.Next())) {
                        short id = CustomNPCNames.TownNPCs[i];
                        if (NPC.AnyNPCs(id) && HasCustomNames(id)) { RandomizeName(id); }
                    }
                    return;
                } else {
                    foreach (NPC i in Main.npc) {
                        if (i.type == type) {
                            string oldName = i.GivenName;
                            i.GivenName = GetRandomName(type);
                            if (oldName != i.GivenName) {
                                Network.ModSync.SyncWorldData(Network.SyncType.NAME, type);
                            }
                        }
                    }
                }
            } else if (Main.netMode == NetmodeID.MultiplayerClient) {
                Network.PacketSender.SendPacketToServer(Network.PacketType.RANDOMIZE, type);
            }
        }
        
        public static NPC FindFirstNPC(short type)
        {
            foreach (NPC i in Main.npc) {
                if (i.type == type) { return i; }
            }
            return null;
        }

        /// <summary>
        /// Checks whether or not an NPC has custom names to choose from prior to attempting randomization.
        /// </summary>
        /// <param name="type">NPCID of the Town NPC. Must be one of the values in CustomNPCNames.TownNPCs</param>
        /// <returns></returns>
        public static bool HasCustomNames(short type)
        {
            return (CustomWorld.mode == 0 
                || (CustomWorld.mode == 1 && CustomWorld.CustomNames[type].Count != 0)
                || (CustomWorld.mode == 2 && CustomWorld.CustomNames[(short)(isMale[type] ? 1000 : 1001)].Count != 0)
                || (CustomWorld.mode == 3 && CustomWorld.CustomNames[1002].Count != 0)
                );
        }
    }
}
