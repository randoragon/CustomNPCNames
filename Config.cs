using Terraria.ModLoader.Config;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;
using Terraria;
using Terraria.IO;

// There's 2 config file systems in this mod. One is used for server configs, the other for client configs.
// The reason the client configs aren't based on the official ModConfig class is because I haven't found a way
// to write to the config files whenever I want using ModConfig. Goldenapple's method mentioned later however
// allows me to have such functionality, which I need. There's no interference between the two systems as far as I've tested.

namespace CustomNPCNames
{
    public class ServerConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [DefaultValue(false)]
        [Label("Enable Verbose Server Messages")]
        [Tooltip("Makes the server send a chat message every time\na user makes any change in the mod menu,\nas opposed to sending it only\nfor heavy operations (clearing, pasting, etc.).")]
        public bool VerboseServerMessages;

        [DefaultValue(false)]
        [Label("Disable Server Messages")]
        [Tooltip("Stops the server from sending any messages.\nThis overrides the Verbose setting.")]
        public bool DisableServerMessages;
    }

    public class ClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(false)]
        [Label("Carry")]
        [Tooltip("Preserves all name lists and settings when moving between worlds.\nOnly affects newly created worlds or worlds that\nhave not been saved with the mod enabled yet.")]
        public bool CarryProperty // the reason Carry and CarryProperty are oddly split like that is because I need Carry to be static, but config fields cannot be static.
        {
            get { return Carry; }
            set { Carry = value; }
        }
        public static bool Carry;
    }
}
