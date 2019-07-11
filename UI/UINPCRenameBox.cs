using Terraria.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace CustomNPCNames.UI
{
    class UINPCRenameBox : UIElement
    {
        public readonly UITextBox activeVariant;
        public readonly UIIdleTextBox inactiveVariant;

        public UINPCRenameBox()
        {
            activeVariant = new UITextBox("");
            activeVariant.HAlign = 0.5f;
            activeVariant.Top.Set(8, 0);
            activeVariant.Left.Set(0, 0);
            activeVariant.SetTextMaxLength(25);
            activeVariant.BackgroundColor = new Color(170, 90, 80);
            activeVariant.BorderColor = new Color(30, 15, 10);

            inactiveVariant = new UIIdleTextBox("Select NPC:");
            inactiveVariant.HAlign = 0.5f;
            inactiveVariant.Top.Set(8, 0);
            inactiveVariant.Left.Set(0, 0);
            inactiveVariant.Width.Set(200, 0);
            inactiveVariant.Height.Set(40, 0);
            inactiveVariant.BackgroundColor = new Color(80, 80, 80);
            inactiveVariant.BorderColor = new Color(20, 20, 20);

            Append(inactiveVariant);
        }

        public void UpdateStatus()
        {
            if (UINPCButton.Selection != null)
            {
                string topNameBoxDisplay = NPC.GetFirstNPCNameOrNull(UINPCButton.Selection.npcId);

                if (topNameBoxDisplay != null)
                {
                    RemoveChild(inactiveVariant);
                    activeVariant.SetText(topNameBoxDisplay);
                    if (!HasChild(activeVariant)) { Append(activeVariant); }
                } else
                {
                    RemoveChild(activeVariant);
                    inactiveVariant.SetText("NPC Unavailable", "This NPC is not alive, \ntherefore cannot be renamed!");
                    if (!HasChild(inactiveVariant)) { Append(inactiveVariant); }
                }
            } else
            {
                RemoveChild(activeVariant);
                inactiveVariant.SetText("Select NPC:", "");
                if (!HasChild(inactiveVariant)) { Append(inactiveVariant); }
            }
        }
    }

    class UIIdleTextBox : UIPanel
    {
        public readonly UIText text;
        protected string hoverText;

        public UIIdleTextBox(string text, string hoverText = "")
        {
            this.text = new UIText(text);
            this.text.HAlign = 0.5f;
            this.text.VAlign = 0.5f;
            Append(this.text);
        }

        public void SetText(string text, string hoverText = "")
        {
            this.text.SetText(text);
            this.hoverText = hoverText;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            if (IsMouseHovering)
            {
                Main.hoverItemName = hoverText;
                //Main.showItemText = true;
            }
        }
    }
}
