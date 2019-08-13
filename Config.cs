using System.IO;
using Terraria;
using Terraria.IO;

namespace CustomNPCNames
{
    // Copied from https://forums.terraria.org/index.php?threads/modders-guide-to-config-files-and-optional-features.48581
    // thanks to goldenapple for his helpful guide on config files
    public static class Config
    {
        //The file will be stored in "Terraria/ModLoader/Mod Configs/CustomNPCNames.json"
        static string ConfigPath = Path.Combine(Main.SavePath, "Mod Configs", "CustomNPCNames.json");
        static Preferences Configuration = new Preferences(ConfigPath);

        public static void Load()
        {
            //Reading the config file
            bool success = ReadConfig();

            if (!success) {
                CustomNPCNames.instance.Logger.Error("Failed to read Example Mod's config file! Recreating config...");
                CreateConfig();
            }
        }

        //Returns "true" if the config file was found and successfully loaded.
        private static bool ReadConfig()
        {
            if (Configuration.Load()) {
                Configuration.Get("Carry", ref UI.RenameUI.carry);
                Configuration.Get("VerboseServerMessages", Network.Broadcaster.SendVerbose);
                Configuration.Get("DisableServerMessages", Network.Broadcaster.SendNone);
                return true;
            }
            return false;
        }

        //Creates a config file. This will only be called if the config file doesn't exist yet or it's invalid. 
        private static void CreateConfig()
        {
            Configuration.Clear();
            Configuration.Put("Carry", true);
            Configuration.Put("VerboseServerMessages", false);
            Configuration.Save();
        }

        public static void SetValue(string field, object value)
        {
            Configuration.Put(field, value);
            Configuration.Save();
        }
    }
}