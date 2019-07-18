using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CustomNPCNames.UI
{
    /// <summary>
    /// This class is exclusively for displaying the custom names in rows on the menu (after selecting an NPC).
    /// </summary>
    class UINameList : UIList, IDragableUIPanelChild
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

        private List<UIElement> _removeList;

        public UINameList() : base()
        {
            _removeList = new List<UIElement>();
        }

        public override void Update(GameTime gameTime)
        {
            
            foreach (UIElement i in _removeList) {
                Remove(i);
            }

            base.Update(gameTime);
        }

        public void RemoveName(UINameField field)
        {
            _removeList.Add(field);
        }

        public virtual void DeselectAll(bool save = true)
        {
            foreach (object i in _items)
            {
                (i as UINameField).Deselect();
            }
        }
    }
}