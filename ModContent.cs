using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;

namespace CustomNPCNames
{
    class ModContent
    {
        public static Texture2D GetTexture(string path)
        {
            return Main.instance.Content.Load<Texture2D>(path);
        }
    }
}
