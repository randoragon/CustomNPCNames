using Terraria;
using Terraria.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CustomNPCNames.UI
{
    /// <summary>
    /// Customized class for the cycle text button at the bottom of the menu.
    /// </summary>
    class UIModeCycleButton : UITextPanel, IDragableUIPanelChild
    {
        public byte State
        {
            set
            {
                CustomWorld.mode = value;
                switch (value)
                {
                    case 0: SetText("USING: VANILLA NAMES"); break;
                    case 1: SetText("USING: CUSTOM NAMES"); break;
                    case 2: SetText("USING: GENDER NAMES"); break;
                    case 3: SetText("USING: GLOBAL NAMES"); break;
                }
            }
        }
        bool IDragableUIPanelChild.Hover
        {
            get
            {
                MouseState mouse = Mouse.GetState();
                Rectangle pos = InterfaceHelper.GetFullRectangle(this);
                return (mouse.X >= pos.X && mouse.X <= pos.X + pos.Width && mouse.Y >= pos.Y && mouse.Y <= pos.Y + pos.Height);
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

            if (hover) {
                if (MouseButtonPressed(this)) {
                    State = (byte)(++CustomWorld.mode % 4);
                } else if (MouseRButtonPressed(this)) {
                    State = (byte)(--CustomWorld.mode % 4);
                }
            }
        }

        protected static bool MouseButtonPressed(UIModeCycleButton self)
        {
            return self.curMouse.LeftButton == ButtonState.Pressed && self.oldMouse.LeftButton == ButtonState.Released;
        }

        protected static bool MouseRButtonPressed(UIModeCycleButton self)
        {
            return self.curMouse.RightButton == ButtonState.Pressed && self.oldMouse.RightButton == ButtonState.Released;
        }
    }
}
