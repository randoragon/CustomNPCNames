using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;
using Terraria;

namespace CustomNPCNames.UI
{
    /// <summary>
    /// This class is an extension of UIPanel with added caption and hover text functionality.
    /// </summary>
    class UITextPanel : UIPanel
    {
        public UIText Caption { get; protected set; }
        public string Text { get { return Caption.Text; } }
        public string HoverText { get; set; }
        public bool ContainCaption { get; set; }    // whether to adjust the panel's width to match Caption's
        public float Padding { get; set; }          // the horizontal padding used for containCaption

        public UITextPanel(string text = "", string hoverText = "")
        {
            Height.Set(40, 0);
            Caption = new UIText(text);
            Caption.HAlign = 0.5f;
            Caption.VAlign = 0.5f;
            Append(Caption);
            HoverText = hoverText;
            Padding = 12;
        }

        public virtual void SetText(string text, string hoverText = null)
        {
            Caption.SetText(text);
            HoverText = hoverText ?? HoverText;
            if (ContainCaption)
            {
                Width.Set(Main.fontMouseText.MeasureString(text).X + (2 * Padding), 0);
            }
        }

        public virtual void SetColor(Color bg, Color bd)
        {
            BackgroundColor = bg;
            BorderColor = bd;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            if (IsMouseHovering)
            {
                Main.hoverItemName = HoverText;
            }
        }
    }
}
