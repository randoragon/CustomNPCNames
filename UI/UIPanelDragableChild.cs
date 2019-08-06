using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace CustomNPCNames.UI
{
    /// <summary>
    /// Simple wrapper for IDragableUIPanelChild implementation. Read more in DragableUIPanel.cs
    /// </summary>
    public class UIPanelDragableChild : UIPanel, IDragableUIPanelChild
    {
        bool IDragableUIPanelChild.Hover
        {
            get
            {
                MouseState mouse = Mouse.GetState();
                Rectangle pos = InterfaceHelper.GetFullRectangle(this);
                return (mouse.X >= pos.X && mouse.X <= pos.X + pos.Width && mouse.Y >= pos.Y && mouse.Y <= pos.Y + pos.Height);
            }
        }
    }
}
