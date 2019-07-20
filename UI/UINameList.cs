using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

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

            if (RenameUI.IsNPCSelected && Count == 0) {
                CustomNPCNames.renameUI.listMessage.SetText("      The list is empty.\nClick the 'Add Name' button\n     to add a new entry.");
                CustomNPCNames.renameUI.listMessage.Activate();
            } else if (!RenameUI.IsNPCSelected) {
                CustomNPCNames.renameUI.listMessage.SetText("   Name list not available,\nbecause no NPC is selected.");
                CustomNPCNames.renameUI.listMessage.Activate();
            } else {
                CustomNPCNames.renameUI.listMessage.Deactivate();
            }

            base.Update(gameTime);
        }

        public void PrintContent()
        {
            CustomNPCNames.renameUI.panelList.Clear();
            short id = RenameUI.SelectedNPC;
            if (CustomWorld.CustomNames[id] != null && CustomWorld.CustomNames[id].Count > 0) {
                for (uint i = (uint)CustomWorld.CustomNames[id].Count; i-- > 0;) {
                    CustomNPCNames.renameUI.panelList.Add(new UINameField(CustomWorld.CustomNames[id][(int)i], i));
                }
            }
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