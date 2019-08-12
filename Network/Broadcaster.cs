using Terraria;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace CustomNPCNames.Network
{
    class Broadcaster
    {
        public static bool SendVerbose { get; set; }
        public static bool SendNone { get; set; }

        static Broadcaster()
        {
            SendVerbose = true;
            SendNone = false;
        }

        public enum MessageType : byte
        {
            EDIT_NAME,
            ADD_CUSTOM_NAME,
            REMOVE_CUSTOM_NAME,
            CLEAR_NAME_LIST,
            EDIT_CUSTOM_NAME,
            RANDOMIZE_NPC,
            RANDOMIZE_GROUP,
            SWITCH_GENDER,
            CUT_EVERYTHING,
            PASTE_EVERYTHING,
            SWITCH_MODE,
            TOGGLE_UNIQUE
        }
        public static Dictionary<MessageType, bool> isVerbose = new Dictionary<MessageType, bool>() {
            { MessageType.EDIT_NAME,            true  },
            { MessageType.ADD_CUSTOM_NAME,      true  },
            { MessageType.REMOVE_CUSTOM_NAME,   true  },
            { MessageType.CLEAR_NAME_LIST,      false },
            { MessageType.EDIT_CUSTOM_NAME,     false },
            { MessageType.RANDOMIZE_NPC,        true  },
            { MessageType.RANDOMIZE_GROUP,      false },
            { MessageType.SWITCH_GENDER,        true  },
            { MessageType.CUT_EVERYTHING,       false },
            { MessageType.PASTE_EVERYTHING,     false },
            { MessageType.SWITCH_MODE,          false },
            { MessageType.TOGGLE_UNIQUE,        false }
        };

        public static void SendGlobalMessage(string message)
        {
            NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(message), new Color(255, 255, 100));
        }

        public static void SendGlobalMessage(string message, Color color)
        {
            NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(message), color);
        }

        public static void SendGlobalMessage(MessageType type, string message)
        {
            if (SendNone || (!SendVerbose && isVerbose[type])) { return; }
            NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("[c/FF0066:<Custom NPC Names>] " + message), new Color(255, 255, 100));
        }

        public static void SendGlobalMessage(MessageType type, string message, Color color)
        {
            if (SendNone || (!SendVerbose && isVerbose[type])) { return; }
            NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("[c/FF0066:<Custom NPC Names>] " + message), color);
        }
    }
}
