using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.Exceptions;
using System.IO;

namespace CustomNPCNames
{
    class ModContent
    {
        public static void SplitName(string name, out string domain, out string subName)
        {
            int slash = name.IndexOf('/');
            if (slash < 0)
                throw new MissingResourceException("Missing mod qualifier: " + name);

            domain = name.Substring(0, slash);
            subName = name.Substring(slash + 1);
        }

        public static Texture2D GetTexture(string name)
        {
            if (Main.dedServ)
                return null;

            string modName, subName;
            SplitName(name, out modName, out subName);
            if (modName == "Terraria")
                return Main.instance.Content.Load<Texture2D>("Images" + Path.DirectorySeparatorChar + subName);

            Mod mod = ModLoader.GetMod(modName);
            if (mod == null)
                throw new MissingResourceException("Missing mod: " + name);

            return mod.GetTexture(subName);
        }

        public static void PrintTextureRectangle(ref Texture2D texture)
        {
            Main.NewText("recX: " + System.Convert.ToString(texture.Bounds.X) + "; recY: " + System.Convert.ToString(texture.Bounds.Y) + "; recW: " + System.Convert.ToString(texture.Bounds.Width) + "; recH: " + System.Convert.ToString(texture.Bounds.Height) + ';', new Color(0, 255, 0));
        }
    }
}