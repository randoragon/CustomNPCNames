using Microsoft.Xna.Framework;
using Terraria;
using Microsoft.Xna.Framework.Input;
using Terraria.GameInput;

namespace CustomNPCNames.UI
{
    /// <summary>
    /// This class modifies and extends UIEntryPanel's functionality to specifically be the top rename panel on RenameUI menu.
    /// </summary>
    class UIRenamePanel : UIEntryPanel, IDragableUIPanelChild
    {
        protected State state { get; set; }
        protected enum State : byte
        {
            NO_SELECTION,       // when you first open the UI and no NPC is selected
            UNAVAILABLE,        // when you select an NPC that isn't present in the world
            ACTIVE,             // when a valid, living NPC is selected. Unlocks renaming functionality.
            NOT_NPC             // when a non-npc button is selected (male, female, global)
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
        private string lastName;

        public UIRenamePanel() : base()
        {
            state = State.NO_SELECTION;
            focusVariant.TextHAlign = 0.5f;
            idleVariant.TextHAlign = 0.5f;
        }

        public override void SetText(string text)
        {
            text = (CaptionMaxLength == -1) ? text : text.Substring(0, System.Math.Min(text.Length, CaptionMaxLength));
            if (!HasFocus) { idleVariant.SetText(text); }
            focusVariant.SetText(text);
            cursorPosition = text.Length;
            AdjustWidth();
        }

        public override void Update(GameTime gameTime)
        {
            oldMouse = curMouse;
            curMouse = Mouse.GetState();

            if (!HasFocus) {
                UpdateState();
            }

            Rectangle dim = InterfaceHelper.GetFullRectangle(idleVariant);
            bool hover = curMouse.X > dim.X && curMouse.X < dim.X + dim.Width && curMouse.Y > dim.Y && curMouse.Y < dim.Y + dim.Height;

            if (hover && MouseButtonPressed(this) && state == State.ACTIVE && !HasFocus) {
                HasFocus = true;
                RemoveChild(idleVariant);
                Append(focusVariant);
                cursorClock = 0;
                lastName = Text;
            } else if (!hover && MouseButtonPressed(this) && HasFocus) {
                Deselect();
            }

            if (HasFocus || (state == State.ACTIVE && !HasFocus)) {
                Main.blockInput = false;

                // in case the NPC gets killed while we're viewing/editing its name
                if (NPC.GetFirstNPCNameOrNull(UINPCButton.Selection.npcId) == null) {
                    UpdateState();
                }

                if (HasFocus) {
                    PlayerInput.WritingText = true;
                    Main.chatRelease = false;
                    string str = focusVariant.Text;
                    ProcessTypedKey(this, ref str, ref cursorPosition, CaptionMaxLength);

                    if (CaptionMaxLength != -1) {
                        str = str.Substring(0, System.Math.Min(str.Length, CaptionMaxLength));
                    }

                    if (!str.Equals(focusVariant.Text)) {
                        focusVariant.SetText(str);
                        if (ContainCaption) { AdjustWidth(); }
                        cursorPosition = str.Length;
                    }

                    if (KeyPressed(Keys.Enter) || KeyPressed(Keys.Escape)) {
                        if (KeyPressed(Keys.Escape)) {
                            SetText(idleVariant.Text);
                            HasFocus = false;
                            cursorPosition = idleVariant.Text.Length;
                        } else {
                            HasFocus = false;
                            SetText(focusVariant.Text);
                        }
                        RemoveChild(focusVariant);
                        NPCs.CustomNPC.currentNames[RenameUI.SelectedNPC] = idleVariant.Text;
                        Network.PacketSender.SendPacketToServer(Network.PacketType.SEND_CURRENT_NAMES, RenameUI.SelectedNPC);
                        Append(idleVariant);
                    }
                }
            }
        }

        public override void Deselect(bool save = true)
        {
            HasFocus = false;
            if (lastName != focusVariant.Text && save) {
                idleVariant.SetText(focusVariant.Text);
                if (RenameUI.IsNPCSelected && RenameUI.SelectedNPC != 1000 && RenameUI.SelectedNPC != 1001 && RenameUI.SelectedNPC != 1002) {
                    NPCs.CustomNPC.currentNames[RenameUI.SelectedNPC] = idleVariant.Text;
                    Network.PacketSender.SendPacketToServer(Network.PacketType.SEND_CURRENT_NAMES, RenameUI.SelectedNPC);
                }
                lastName = Text;
            }
            RemoveChild(focusVariant);
            Append(idleVariant);
        }

        public void UpdateState()
        {
            if (UINPCButton.Selection != null) {
                // Check for Male-Female conventional IDs
                if (UINPCButton.Selection.npcId == 1000) {
                    state = State.NOT_NPC;
                    HasFocus = false;
                    RemoveChild(focusVariant);
                    idleVariant.SetText("Masculine Names", "This tab contains names\nunique for male NPCs.");
                    SetWidth(Scale * 200, 0);
                    idleVariant.SetColor(new Color(0, 139, 255), new Color(13, 35, 61));
                    if (!HasChild(idleVariant)) { Append(idleVariant); }
                } else if (UINPCButton.Selection.npcId == 1001) {
                    state = State.NOT_NPC;
                    HasFocus = false;
                    RemoveChild(focusVariant);
                    idleVariant.SetText("Feminine Names", "This tab contains names\nunique for female NPCs.");
                    SetWidth(Scale * 200, 0);
                    idleVariant.SetColor(new Color(218, 0, 255), new Color(58, 13, 61));
                    if (!HasChild(idleVariant)) { Append(idleVariant); }
                } else if (UINPCButton.Selection.npcId == 1002) {
                    state = State.NOT_NPC;
                    HasFocus = false;
                    RemoveChild(focusVariant);
                    idleVariant.SetText("Global Names", "This tab contains\nnames for all NPCs.");
                    SetWidth(Scale * 200, 0);
                    idleVariant.SetColor(new Color(200, 80, 64), new Color(80, 25, 18));
                    if (!HasChild(idleVariant)) { Append(idleVariant); }
                } else {
                    string topNameBoxDisplay = NPC.GetFirstNPCNameOrNull(UINPCButton.Selection.npcId);

                    if (topNameBoxDisplay != null) {
                        state = State.ACTIVE;
                        HasFocus = false;
                        RemoveChild(focusVariant);
                        SetText(topNameBoxDisplay);
                        SetIdleHoverText("Edit");
                        idleVariant.SetColor(new Color(80, 190, 150), new Color(20, 50, 40));
                        if (!HasChild(idleVariant)) { Append(idleVariant); }
                    } else {
                        state = State.UNAVAILABLE;
                        HasFocus = false;
                        RemoveChild(focusVariant);
                        idleVariant.SetText("NPC Unavailable", "This NPC is not alive\nand cannot be renamed!");
                        SetWidth(Scale * 200, 0);
                        idleVariant.SetColor(new Color(80, 80, 80), new Color(20, 20, 20));
                        if (!HasChild(idleVariant)) { Append(idleVariant); }
                    }
                }
            } else {
                state = State.NO_SELECTION;
                HasFocus = false;
                RemoveChild(focusVariant);
                idleVariant.SetText("Select NPC", "");
                SetWidth(Scale * 200, 0);
                idleVariant.SetColor(new Color(169, 169, 69), new Color(50, 50, 20));
                if (!HasChild(idleVariant)) { Append(idleVariant); }
            }
        }
    }
}