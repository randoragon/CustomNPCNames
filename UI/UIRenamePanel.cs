using Microsoft.Xna.Framework;
using Terraria;
using Microsoft.Xna.Framework.Input;
using Terraria.GameInput;

namespace CustomNPCNames.UI
{
    /// <summary>
    /// This ugly class modifies and extends UIEntryPanel's functionality to specifically be the top rename panel on RenameUI menu.
    /// </summary>
    class UIRenamePanel : UIEntryPanel
    {
        protected State state { get; set; }
        protected enum State : byte
        {
            NO_SELECTION,       // when you first open the UI and no NPC is selected
            UNAVAILABLE,        // when you select an NPC that isn't present in the world
            ACTIVE              // when a valid, living NPC is selected. Unlocks renaming functionality.
        }

        public UIRenamePanel() : base()
        {
            state = State.NO_SELECTION;
        }

        public override void Update(GameTime gameTime)
        {
            //base.Update(gameTime);

            oldMouse = curMouse;
            curMouse = Mouse.GetState();

            Rectangle dim = InterfaceHelper.GetFullRectangle(idleVariant);
            bool hover = curMouse.X > dim.X && curMouse.X < dim.X + dim.Width && curMouse.Y > dim.Y && curMouse.Y < dim.Y + dim.Height;

            if (hover && MouseButtonPressed() && state == State.ACTIVE && !HasFocus)
            {
                HasFocus = true;
                RemoveChild(idleVariant);
                Append(focusVariant);
                cursorClock = 0;
            } else if (!hover && MouseButtonPressed() && HasFocus)
            {
                HasFocus = false;
                RemoveChild(focusVariant);
                Append(idleVariant);
            }

            if (HasFocus || (state == State.ACTIVE && !HasFocus))
            {
                Main.blockInput = false;

                // in case the NPC gets killed while we're viewing/editing its name
                if (NPC.GetFirstNPCNameOrNull(UINPCButton.Selection.npcId) == null)
                {
                    state = State.UNAVAILABLE;
                    RemoveChild(focusVariant);
                    idleVariant.SetText("NPC Unavailable", "This NPC is not alive\nand cannot be renamed!");
                    idleVariant.SetColor(new Color(80, 80, 80), new Color(20, 20, 20));
                    idleVariant.Width.Set(200, 0);
                    Append(idleVariant);
                }

                if (HasFocus)
                {
                    PlayerInput.WritingText = true;
                    Main.chatRelease = false;
                    string str = focusVariant.Text;
                    ProcessTypedKey(this, ref str, ref cursorPosition, CaptionMaxLength);

                    if (CaptionMaxLength != -1)
                    {
                        str = str.Substring(0, System.Math.Min(str.Length, CaptionMaxLength));
                    }

                    if (!str.Equals(focusVariant.Text))
                    {
                        CustomNPCNames.CustomNames[UINPCButton.Selection.npcId] = str;
                        focusVariant.SetText(str);
                        cursorPosition = str.Length;
                    }

                    if (KeyPressed(Keys.Enter) || KeyPressed(Keys.Escape))
                    {
                        if (KeyPressed(Keys.Escape))
                        {
                            CustomNPCNames.CustomNames[UINPCButton.Selection.npcId] = str;
                            focusVariant.SetText(idleVariant.Text);
                            cursorPosition = idleVariant.Text.Length;
                        }
                        HasFocus = false;
                        RemoveChild(focusVariant);
                        idleVariant.SetText(focusVariant.Text);
                        Append(idleVariant);
                    }
                }
            }
        }

        public void UpdateState()
        {
            if (UINPCButton.Selection != null)
            {
                string topNameBoxDisplay = NPC.GetFirstNPCNameOrNull(UINPCButton.Selection.npcId);

                if (topNameBoxDisplay != null)
                {
                    state = State.ACTIVE;
                    HasFocus = false;
                    RemoveChild(focusVariant);
                    SetText(topNameBoxDisplay);
                    SetIdleHoverText("Edit");
                    idleVariant.SetColor(new Color(80, 190, 150), new Color(20, 50, 40));
                    if (!HasChild(idleVariant)) { Append(idleVariant); }
                } else
                {
                    state = State.UNAVAILABLE;
                    HasFocus = false;
                    RemoveChild(focusVariant);
                    idleVariant.SetText("NPC Unavailable", "This NPC is not alive\nand cannot be renamed!");
                    idleVariant.Width.Set(200, 0);
                    idleVariant.SetColor(new Color(80, 80, 80), new Color(20, 20, 20));
                    if (!HasChild(idleVariant)) { Append(idleVariant); }
                }
            } else
            {
                state = State.NO_SELECTION;
                HasFocus = false;
                RemoveChild(focusVariant);
                idleVariant.SetText("Select NPC", "");
                idleVariant.SetColor(new Color(169, 169, 69), new Color(50, 50, 20));
                idleVariant.Width.Set(200, 0);
                if (!HasChild(idleVariant)) { Append(idleVariant); }
            }
        }

        public override void SetText(string text)
        {
            text = (CaptionMaxLength == -1) ? text : text.Substring(0, System.Math.Min(text.Length, CaptionMaxLength));
            if (!HasFocus) { idleVariant.SetText(text); }
            focusVariant.SetText(text);
            cursorPosition = text.Length;
        }
    }
}