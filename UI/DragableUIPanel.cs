using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace CustomNPCNames.UI
{
    // To prevent dragging when a DragableUIPanel's children are being interacted with, we do simple checks for each of the children,
    // e.g. we will enter drag state only when the dragable panel is clicked AND its children are not. For that reason every single child
    // must be derived from a custom "IDragableUIPanelChild" interface class which has a property for evaluating whether or not it's being hovered over.
    public class DragableUIPanel : UIPanel
    {
        private Vector2 offset;
        public bool dragging;

        public override void MouseDown(UIMouseEvent evt)
        {
            base.MouseDown(evt);

            foreach (UIElement i in Elements) {
                if ((i as IDragableUIPanelChild).Hover) { return; }
            }

            DragStart(evt);
        }

        public override void MouseUp(UIMouseEvent evt)
        {
            base.MouseUp(evt);
            if (dragging) {
                DragEnd(evt);
            }
        }

        private void DragStart(UIMouseEvent evt)
        {
            offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
            dragging = true;
        }

        private void DragEnd(UIMouseEvent evt)
        {
            Vector2 end = evt.MousePosition;
            dragging = false;

            Left.Set(end.X - offset.X, 0f);
            Top.Set(end.Y - offset.Y, 0f);

            Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            if (dragging)
            {
                Left.Set(Main.mouseX - offset.X, 0f);
                Top.Set(Main.mouseY - offset.Y, 0f);
                Recalculate();
            }

            var parentSpace = Parent.GetDimensions().ToRectangle();
            if (!GetDimensions().ToRectangle().Intersects(parentSpace))
            {
                Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
                Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
                // Recalculate forces the UI system to do the positioning math again.
                Recalculate();
            }
        }
    }

    interface IDragableUIPanelChild
    {
        bool Hover { get; }
    }
}