using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        public readonly uint nthElement;
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

        public UINameField(StringWrapper name, uint nth) : base(name.str)
        {
            this.name = name;
            nthElement = nth;

            ContainCaption = false;
            SetScale(0.85f);
            SetWidth((0.85f * Main.fontMouseText.MeasureString("_________________________").X) + 18, 0);
            CaptionMaxLength = 25;

            focusVariant.Caption.HAlign = 0f;
            focusVariant.Caption.Left.Set(6, 0);
            idleVariant.Caption.HAlign = 0f;
            idleVariant.Caption.Left.Set(6, 0);
            SetText(name.str);
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
                Name = text;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!HasFocus && IsNew) {
                IsNew = false;
            }

            if (CustomNPCNames.renameUI.removeMode && HasFocus) {
                CustomWorld.CustomNames[UINPCButton.Selection.npcId].Remove(name);
                CustomNPCNames.renameUI.panelList.RemoveName(this);
                Deselect();
                return;
            }

            focusVariant.BorderColor.A = 60;
            idleVariant.BorderColor.A = 60;
        }

        public override void Recalculate()
        {
            base.Recalculate();
        }
    }
}
