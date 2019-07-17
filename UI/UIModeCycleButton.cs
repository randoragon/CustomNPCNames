using Terraria;
using Terraria.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CustomNPCNames.UI
{
    /// <summary>
    /// Customized class for the cycle text button at the bottom of the menu.
    /// </summary>
    class UIModeCycleButton : UITextPanel
    {
        protected byte State
        {
            set
            {
                CustomNPCNames.mode = value;
                switch (value)
                {
                    case 0: SetText("USING: VANILLA NAMES"); break;
                    case 1: SetText("USING: CUSTOM NAMES"); break;
                    case 2: SetText("USING: GENDER NAMES"); break;
                    case 3: SetText("USING: GLOBAL NAMES"); break;
                }
            }
        }

        protected MouseState curMouse;
        protected MouseState oldMouse;

        public UIModeCycleButton() : base("")
        {
            HoverText = "Cycle";
            State = 0;
            BorderColor = new Color(30, 10, 51);
            BackgroundColor = new Color(150, 50, 255);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            oldMouse = curMouse;
            curMouse = Mouse.GetState();

            Rectangle dim = InterfaceHelper.GetFullRectangle(this);
            bool hover = curMouse.X > dim.X && curMouse.X < dim.X + dim.Width && curMouse.Y > dim.Y && curMouse.Y < dim.Y + dim.Height;

            if (MouseButtonPressed(this) && hover)
            {
                State = (byte)(++CustomNPCNames.mode % 4);
            }
        }

        protected static bool MouseButtonPressed(UIModeCycleButton self)
        {
            return self.curMouse.LeftButton == ButtonState.Pressed && self.oldMouse.LeftButton == ButtonState.Released;
        }
    }
}
