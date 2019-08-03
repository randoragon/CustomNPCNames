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
        public int SelectedIndex { get; protected set; }
        private KeyboardState oldKey;
        private KeyboardState curKey;
        protected bool lastKey = false; // false for Keys.Down, true for Keys.Up
        protected int keyClock = 0;
        //public string newText;        // holds the text of a "green" entry that's being added in case a PrintContents() gets called and interrupts editing.
        public StringWrapper curName; // holds the name of a currently selected entry in case a PrintContents() gets called and interrupts editing.

        private List<UIElement> _removeList;

        public UINameList() : base()
        {
            _removeList = new List<UIElement>();
        }

        public override void Update(GameTime gameTime)
        {
            // Remove items from removal list and update namefields' "nth" indexing
            if (_removeList.Count != 0) {
                foreach (UIElement i in _removeList) {
                    Remove(i);
                }
                _removeList.Clear();
                PrintContent();
            }
            

            // Calculate the currently selected Name Field of the list (or -1 for none)
            SelectedIndex = -1;
            for (int i = 0; i < Count; i++) {
                if ((_items[i] as UINameField).HasFocus) { SelectedIndex = i; break; }
            }

            // Up-Down arrow scrolling functionality
            oldKey = curKey;
            curKey = Keyboard.GetState();

            if (SelectedIndex != -1) {
                bool upPressed   = curKey.IsKeyDown(Keys.Up)   && !oldKey.IsKeyDown(Keys.Up);
                bool downPressed = curKey.IsKeyDown(Keys.Down) && !oldKey.IsKeyDown(Keys.Down);
                
                if ((downPressed || lastKey == false && keyClock == 30) && SelectedIndex < Count - 1) {
                    (_items[SelectedIndex] as UINameField).Deselect();
                    (_items[++SelectedIndex] as UINameField).Select();
                    _scrollbar.ViewPosition = System.Math.Max(_scrollbar.ViewPosition, ((SelectedIndex + 1) * 36f) - Height.Pixels);
                    keyClock = (keyClock == 30 && lastKey == false ? 28 : 0);
                    lastKey = false;
                } else if ((upPressed || lastKey == true && keyClock == 30) && SelectedIndex > 0) {
                    (_items[SelectedIndex] as UINameField).Deselect();
                    (_items[--SelectedIndex] as UINameField).Select();
                    _scrollbar.ViewPosition = System.Math.Min(_scrollbar.ViewPosition, SelectedIndex * 36f);
                    keyClock = (keyClock == 30 && lastKey == true ? 28 : 0);
                    lastKey = true;
                }

                if (SelectedIndex != 0 && SelectedIndex != Count - 1 && ((lastKey == true && curKey.IsKeyDown(Keys.Up)) || (lastKey == false && curKey.IsKeyDown(Keys.Down)))) {
                    keyClock += (keyClock < 30 ? 1 : 0);
                } else {
                    keyClock = 0;
                }
            }

            // Update List Message text and visibility
            if (RenameUI.IsNPCSelected && Count == 0) {
                RenameUI.listMessage.SetText("      The list is empty.\nClick the 'Add Name' button\n     to add a new entry.");
                RenameUI.listMessage.Activate();
            } else if (!RenameUI.IsNPCSelected) {
                RenameUI.listMessage.SetText("   Name list not available,\nbecause no NPC is selected.");
                RenameUI.listMessage.Activate();
            } else {
                RenameUI.listMessage.Deactivate();
            }

            if (CustomWorld.updateNameList) {
                PrintContent();
                CustomWorld.updateNameList = false;
            }

            // Update List Count
            RenameUI.listCount.SetText((SelectedIndex + 1) + "/" + Count);

            base.Update(gameTime);
        }

        public void AddNew(string str = "")
        {
            string newName = str;
            var newWrapper = new StringWrapper(ref newName);
            var field = new UINameField(newWrapper, (uint)Count);
            field.IsNew = true;
            //newText = str;
            curName = newWrapper;
            Terraria.Main.NewText("curName = newWrapper", Color.Orange);
            Add(field);
            DeselectAll();
            field.Select();
        }

        public void PrintContent()
        {
            if (RenameUI.IsNPCSelected) {
                bool reassignEditName = false;
                foreach (UINameField i in _items) {
                    if (i.HasFocus) {
                        // If an entry is being edited, its displayed value is not assigned to its StringWrapper yet, instead it's being held in editName. To reassign it later, it's necessary to take this editName and store it in another StringWrapper.
                        // StringWrapper.Equal() method compares only IDs, so if the new StringWrapper has the EditName as its str, and shares the same ID, the exact same entry will later be found based on the equality of IDs.
                        string name = i.EditName;
                        curName = new StringWrapper(ref name, i.NameWrapper.ID);
                        reassignEditName = true;
                        break;
                    }
                }
                RenameUI.panelList.Clear();
                short id = RenameUI.SelectedNPC;
                if (CustomWorld.CustomNames[id] != null && CustomWorld.CustomNames[id].Count > 0) {
                    for (uint i = (uint)CustomWorld.CustomNames[id].Count; i-- > 0;) {
                        RenameUI.panelList.Add(new UINameField(CustomWorld.CustomNames[id][(int)i], i));
                    }
                    RenameUI.panelListReady = false;
                }

                if (reassignEditName) {
                    Recalculate(); // this also calls RecalculateChildren(), where the actual reassignment will take place
                }
            }
        }

        public override void RecalculateChildren()
        {
            base.RecalculateChildren();
            // Reselect/Recreate an entry that was selected before a PrintContent(). This should only ever execute after a PrintContent()
            if (!RenameUI.panelListReady) {
                RenameUI.panelListReady = true;
                if (curName != null) {
                    bool foundExisting = false;
                    // Reselect entry if it exists...
                    foreach (UINameField i in _items) {
                        if (i.NameWrapper.Equals(curName)) {
                            i.Select();
                            i.HadFocus = true; // this is necessary to prevent the editName assignment in UINameField.Update(), i.e. make the nameField believe that it had been selected all along, not just now
                            i.SetText(curName.str);
                            Terraria.Main.NewText("ReSelect! (Found Existing) " + curName.str, Color.Orange);
                            foundExisting = true;
                            break;
                        }
                    }
                    // ...if it doesn't exist, assume it disappeared because it was a new one, and recreate it
                    if (!foundExisting) {
                        AddNew(curName.str);
                        Terraria.Main.NewText("ReCreate! (Existing Not Found)", Color.Orange);
                    }
                    curName = null;
                }
            }
            
        }

        public void RemoveName(UINameField field)
        {
            if (field.NameWrapper.Equals(curName)) {
                curName = null;
            }
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