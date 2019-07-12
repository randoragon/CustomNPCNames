using Terraria;
using Terraria.UI;
using Microsoft.Xna.Framework;

namespace CustomNPCNames.UI
{
    class InterfaceHelper
    {
        public static Rectangle GetFullRectangle(UIElement element)
        {
            Vector2 vector = new Vector2(element.GetDimensions().X, element.GetDimensions().Y);
            Vector2 position = new Vector2(element.GetDimensions().Width, element.GetDimensions().Height) + vector;
            vector = Vector2.Transform(vector, Main.UIScaleMatrix);
            position = Vector2.Transform(position, Main.UIScaleMatrix);
            Rectangle result = new Rectangle((int)vector.X, (int)vector.Y, (int)(position.X - vector.X), (int)(position.Y - vector.Y));
            int width = Main.spriteBatch.GraphicsDevice.Viewport.Width;
            int height = Main.spriteBatch.GraphicsDevice.Viewport.Height;
            result.X = Utils.Clamp<int>(result.X, 0, width);
            result.Y = Utils.Clamp<int>(result.Y, 0, height);
            result.Width = Utils.Clamp<int>(result.Width, 0, width - result.X);
            result.Height = Utils.Clamp<int>(result.Height, 0, height - result.Y);
            return result;
        }
    }
}
