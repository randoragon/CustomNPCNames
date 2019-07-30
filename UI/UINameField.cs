using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace CustomNPCNames.UI
{
    class UINameField : UIEntryPanel
    {
        private StringWrapper name;
        public string Name
        {
            get { return name.str; }
            set { name.str = value; }
        }
        public StringWrapper NameWrapper { get { return name; } }
        private string editName;            // this string is the one actually getting edited by keyboard (changes are applied to Name when the editing is finished)
        private bool hadFocus;              // used to determine whether or not the focus of the entry has just been lost - in Update()
        private string lastName;            // used to determine whether or not the text of the entry has just been changed - in Update()
        public uint nthElement;
        private bool isNew = false;
        public bool IsNew
        {
            get { return isNew; }
            set
            {
                if (value) {
                    SetFocusColor(new Color(70, 150, 40), new Color(50, 180, 20));
                } else {
                    SetFocusColor(new Color(170, 90, 80), new Color(30, 15, 10));
                }
                isNew = value;
            }
        }
        public float Presence { get; private set; } // represents how much the panel is on screen, e.g. if it goes out of bounds of the UINameList then this value is 0f, and if it's all on the screen it's 1f.

        public UINameField(StringWrapper name, uint nth) : base(name.str)
        {
            this.name = name;
            nthElement = nth;

            ContainCaption = false;
            SetScale(0.85f);
            SetWidth((0.85f * Main.fontMouseText.MeasureString("_________________________").X) + 18, 0);
            CaptionMaxLength = 25;

            focusVariant.Caption.VAlign = 0f;
            focusVariant.Caption.Top.Pixels = 9;
            idleVariant.Caption.VAlign = 0f;
            idleVariant.Caption.Top.Pixels = 9;
            SetText(name.str);
            base.SetIdleHoverText(System.Convert.ToString(name.ID));
        }

        public void TextCropTop(float padding)
        {
            idleVariant.Caption.Top.Pixels = 10 - padding;
            focusVariant.Caption.Top.Pixels = 10 - padding;
            idleVariant.PaddingTop = padding;
            focusVariant.PaddingTop = padding;
        }

        public void TextCropBottom(float padding)
        {
            idleVariant.PaddingBottom = padding;
            focusVariant.PaddingBottom = padding;
        }

        public override int CompareTo(object obj)
        {
            return (int)((obj as UINameField).nthElement - nthElement); // newest always comes first
        }

        public override void SetText(string text)
        {
            base.SetText(text);
            if (name != null)
            {
                editName = text;
            }
        }

        public override void Update(GameTime gameTime)
        {
            hadFocus = HasFocus;

            base.Update(gameTime);

            if (HasFocus && !RenameUI.panelListReady) {
                Deselect(false);
            }

            if (!hadFocus && HasFocus) {
                lastName = editName;
            }            

            if (!IsNew) {
                // Sync world data when necessary - only if the entry was exited from, and only if its contents have been altered. This is to minimize overhead
                if (hadFocus && !HasFocus && lastName != editName) {
                    Name = editName;
                    Network.PacketSender.SendPacketToServer(Network.PacketType.EDIT_NAME, RenameUI.SelectedNPC, name.ID, Name);
                } else if (RenameUI.removeMode && RenameUI.panelListReady && HasFocus && !hadFocus) {
                    CustomWorld.CustomNames[RenameUI.SelectedNPC].Remove(name);
                    RenameUI.panelList.RemoveName(this);
                    Deselect();
                    if (Main.netMode == NetmodeID.MultiplayerClient) {
                        Network.PacketSender.SendPacketToServer(Network.PacketType.REMOVE_NAME, RenameUI.SelectedNPC, name.ID);
                    }
                }
            } else {
                if (!HasFocus) {
                    IsNew = false;
                    RenameUI.panelList.newText = null;
                    CustomWorld.CustomNames[RenameUI.SelectedNPC].Add(name);
                    if (Main.netMode == NetmodeID.MultiplayerClient) {
                        Network.PacketSender.SendPacketToServer(Network.PacketType.ADD_NAME, RenameUI.SelectedNPC, name.ID, editName);
                    }
                } else {
                    RenameUI.panelList.newText = editName;
                }
            }

            focusVariant.BorderColor.A = 60;
            idleVariant.BorderColor.A = 60;
        }

        public override void Recalculate()
        {
            // update padding of list elements' text, because it is not affected by  UINameList's OverflowHidden, so it has to be cropped manually
            Rectangle dim = InterfaceHelper.GetFullRectangle(this);
            Rectangle listDim = InterfaceHelper.GetFullRectangle(RenameUI.panelList);
            if (dim.Y > listDim.Y) {
                Presence = 1f - (MathHelper.Clamp(dim.Y + dim.Height - listDim.Y - listDim.Height, 0, dim.Height) / dim.Height);
                TextCropTop(0f);
                TextCropBottom((1 - Presence) * dim.Height);
            } else {
                Presence = 1f - (MathHelper.Clamp(listDim.Y - dim.Y, 0, dim.Height) / dim.Height);
                TextCropTop((1 - Presence) * dim.Height);
                TextCropBottom(0f);
            }

            base.Recalculate();
        }
    }
}
