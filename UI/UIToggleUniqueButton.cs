using Terraria;
using Terraria.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CustomNPCNames.UI
{
    /// <summary>
    /// Customized class for the unique names checkbox at the bottom of the menu.
    /// </summary>
    class UIToggleUniqueButton : UITextPanel, IDragableUIPanelChild
    {
        public bool State
        {
            set
            {
                CustomWorld.tryUnique = value;
                UpdateState();
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

        public UIToggleUniqueButton() : base("")
        {
            HoverText = "Toggle";
            State = true;
        }

        public void UpdateState()
        {
            if (CustomWorld.tryUnique) {
                BorderColor = new Color(0, 40, 0);
                BackgroundColor = new Color(0, 150, 0);
                SetText("UNIQUE NAMES: ON");
            } else {
                BorderColor = new Color(40, 0, 0);
                BackgroundColor = new Color(150, 0, 0);
                SetText("UNIQUE NAMES: OFF");
            }
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
                State = !CustomWorld.tryUnique;
                Network.PacketSender.SendPacketToServer(Network.PacketType.TRY_UNIQUE);
            }
        }

        protected static bool MouseButtonPressed(UIToggleUniqueButton self)
        {
            return self.curMouse.LeftButton == ButtonState.Pressed && self.oldMouse.LeftButton == ButtonState.Released;
        }
    }
}
