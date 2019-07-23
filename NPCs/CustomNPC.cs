using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.ID;
using System.Linq;
using Microsoft.Xna.Framework;

namespace CustomNPCNames.NPCs
{
    class CustomNPC : GlobalNPC
    {
        public  static Dictionary<short, string> currentNames;
        public  static Dictionary<short, bool>   isMale;
        public  static Dictionary<short, List<StringWrapper>> vanillaNames;
        private static Dictionary<short, ushort> npcCount;
        private static Dictionary<short, ushort> npcCountPrev;
        private static Dictionary<short, bool>   npcJustJoined;

        /// <summary>
        /// Determines whether or not an NPC of the given type has spawned since last time UpdateNPCCount() was called for that NPC.
        /// </summary>
        /// <remarks>
        /// It's neccessary because in some cases the SetDefaults() method is called even when an NPC is NOT spawning, and then the NPC name randomizes pointlessly.
        /// </remarks>
        public static bool HasNewSpawned(short type)
        {
            return npcCount[type] > npcCountPrev[type];
        }

        // this gets called in CustomWorld.PostUpdate()
        public static void UpdateNPCCount()
        {
            foreach (short id in CustomNPCNames.TownNPCs) {
                npcCountPrev[id] = npcCount[id];
                npcCount[id] = (ushort)NPC.CountNPCS(id);
            }
        }

        public CustomNPC()
        {
            currentNames  = new Dictionary<short, string>();
            isMale        = new Dictionary<short, bool>();
            npcJustJoined = new Dictionary<short, bool>();
            ResetCurrentNames();
            ResetCurrentGender();
            ResetJustJoined();

            npcCount = new Dictionary<short, ushort>();
            npcCountPrev = new Dictionary<short, ushort>();
            foreach (short i in CustomNPCNames.TownNPCs)
            {
                npcCount.Add(i, (ushort)NPC.CountNPCS(i));
                npcCountPrev.Add(i, npcCount[i]);
            }

            // vanilla names need to be added manually for randomization, because it's otherwise impossible to "force" a name change via Randomize Button
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
            vanillaNames.Add(NPCID.SantaClaus,         new List<StringWrapper>() { "Santa Claus" });
            vanillaNames.Add(NPCID.TravellingMerchant, new List<StringWrapper>() { "Abraham", "Aedan", "Aphraim", "Bohemas", "Eladon", "Gallius", "Llewellyn", "Mercer", "Rawleigh", "Riley", "Romeo", "Shipton", "Willy" });
        }

        public static void ResetCurrentNames()
        {
            currentNames.Clear();
            foreach (short i in CustomNPCNames.TownNPCs)
            {
                currentNames.Add(i, null);
            }
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
            isMale.Add(NPCID.SantaClaus,         true);
            isMale.Add(NPCID.TravellingMerchant, true);
        }

        public static void ResetJustJoined()
        {
            npcJustJoined.Clear();
            foreach (short i in CustomNPCNames.TownNPCs) {
                npcJustJoined.Add(i, false);
            }
            npcJustJoined[NPCID.Guide] = true; // because the Guide is alive by default
        }

        public static void RandomizeName(short type)
        {
            if (type == 1000) {
                foreach (short i in CustomNPCNames.TownNPCs) {
                    if (isMale[i] && NPC.CountNPCS(i) != 0) { currentNames[i] = null; }
                }
                foreach (int i in Enumerable.Range(0, CustomNPCNames.TownNPCs.Length).OrderBy(x => Main.rand.Next())) {
                    short id = CustomNPCNames.TownNPCs[i];
                    if (isMale[id] && NPC.CountNPCS(id) != 0) { RandomizeName(id); }
                }
                return;
            } else if (type == 1001) {
                foreach (short i in CustomNPCNames.TownNPCs) {
                    if (!isMale[i] && NPC.CountNPCS(i) != 0) { currentNames[i] = null; }
                }
                foreach (int i in Enumerable.Range(0, CustomNPCNames.TownNPCs.Length).OrderBy(x => Main.rand.Next())) {
                    short id = CustomNPCNames.TownNPCs[i];
                    if (!isMale[id] && NPC.CountNPCS(id) != 0) { RandomizeName(id); }
                }
                return;
            } else if (type == 1002) {
                foreach (short i in CustomNPCNames.TownNPCs) {
                    if (NPC.CountNPCS(i) != 0) { currentNames[i] = null; }
                }
                foreach (int i in Enumerable.Range(0, CustomNPCNames.TownNPCs.Length).OrderBy(x => Main.rand.Next())) {
                    short id = CustomNPCNames.TownNPCs[i];
                    if (NPC.CountNPCS(id) != 0) { RandomizeName(id); }
                }
                return;
            } else {
                var list = new List<StringWrapper>();
                currentNames[type] = null;

                switch (CustomWorld.mode) {
                    case 0: // Vanilla names mode
                        list = new List<StringWrapper>(vanillaNames[type]);
                        break;
                    case 1: // Custom Names mode
                        if (CustomWorld.CustomNames[type].Count != 0) {
                            list = new List<StringWrapper>(CustomWorld.CustomNames[type]);
                        } else {
                            list = new List<StringWrapper>() { NPC.GetFirstNPCNameOrNull(type) };
                        }
                        break;
                    case 2: // Gender Names mode
                        if (CustomWorld.CustomNames[(short)(isMale[type] ? 1000 : 1001)].Count != 0) {
                            list = new List<StringWrapper>(CustomWorld.CustomNames[(short)(isMale[type] ? 1000 : 1001)]);
                        } else {
                            list = new List<StringWrapper>() { NPC.GetFirstNPCNameOrNull(type) };
                        }
                        break;
                    case 3: // Global Names mode
                        if (CustomWorld.CustomNames[1002].Count != 0) {
                            list = new List<StringWrapper>(CustomWorld.CustomNames[1002]);
                        } else {
                            list = new List<StringWrapper>() { NPC.GetFirstNPCNameOrNull(type) };
                        }
                        break;
                }

                if (CustomWorld.tryUnique) {
                    var listsIntersection = new List<StringWrapper>();
                    var excludedNames = new List<StringWrapper>();
                    foreach (KeyValuePair<short, string> i in currentNames) {
                        if (NPC.CountNPCS(i.Key) != 0 && StringWrapper.ListContains(list, i.Value)) {
                            bool contains = false;
                            foreach (StringWrapper j in listsIntersection) {
                                if (j.str == i.Value) { contains = true; break; }
                            }
                            if (!contains) {
                                listsIntersection.Add(i.Value);
                            }
                        }
                    }

                    if (listsIntersection.Count < list.Count) {
                        foreach (StringWrapper i in listsIntersection) {
                            foreach (StringWrapper j in list) {
                                if (i.str == j.str) { excludedNames.Add(j); }
                            }
                        }

                        foreach (StringWrapper i in excludedNames) {
                            list.Remove(i);
                        }
                    }
                }

                currentNames[type] = (string)list[Main.rand.Next(list.Count)];
            }
        }
        
        public override void SetDefaults(NPC npc)
        {
            if (currentNames.ContainsKey((short)npc.type)) {
                npcJustJoined[(short)npc.type] = true;
                bool noNames = (CustomWorld.CustomNames != null
                && ((CustomWorld.mode == 1 && CustomWorld.CustomNames[(short)npc.type].Count == 0)
                 || (CustomWorld.mode == 2 && CustomWorld.CustomNames[(short)(isMale[(short)npc.type] ? 1000 : 1001)].Count == 0)
                 || (CustomWorld.mode == 3 && CustomWorld.CustomNames[1002].Count == 0)));

                if (!noNames) {
                    UpdateNPCCount();
                    foreach (short i in CustomNPCNames.TownNPCs) {
                        if (npc.type == i && HasNewSpawned(i)) {
                            RandomizeName(i);
                            npc.GivenName = currentNames[i];
                        }
                    }
                } else {
                    currentNames[(short)npc.type] = null;
                }
            }
        }

        public override bool PreAI(NPC npc)
        {
            if (currentNames.ContainsKey((short)npc.type) && currentNames[(short)npc.type] != null)
            {
                if (npc.GivenName != currentNames[(short)npc.type])
                {
                    if (npcJustJoined[(short)npc.type] && Main.chatLine[0].text.Length >= 18 && Main.chatLine[0].text.Substring(Main.chatLine[0].text.Length - 12, 12) == "has arrived!") { // the chatLine condition is here as a sanity check, because npcJustJoined by itself is not foolproof
                        // Replace the default chat message with one with the custom name
                        var line = new Terraria.UI.Chat.ChatLine();
                        line.text = string.Format("{0} the {1} has arrived!", currentNames[(short)npc.type], CustomNPCNames.GetNPCName((short)npc.type));
                        line.showTime = Main.chatLength;
                        line.color = new Color(50, 125, 255); // this is the vanilla color for Town NPC arrival messages
                        line.parsedText = Terraria.UI.Chat.ChatManager.ParseMessage(line.text, line.color).ToArray();
                        Main.chatLine[0] = line;
                        npcJustJoined[(short)npc.type] = false;
                    }

                    npc.GivenName = currentNames[(short)npc.type];
                }
            }
            return true;
        }
    }
}
