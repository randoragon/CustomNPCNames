using Terraria.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;

namespace CustomNPCNames.UI
{
    class UINPCButton : UIHoverImageButton
    {
        public UIImage npcHead { get; private set; }
        protected string hoverText;
        public static UINPCButton Selection { get; private set; }
        public readonly short npcId;

        public UINPCButton(Texture2D texture, string hoverText, short id) : base(ModContent.GetTexture("CustomNPCNames/UI/UINPCButton"), hoverText)
        {
            this.npcId = id;

            OverflowHidden = true;
            SetPadding(1f);

            npcHead = new UIImage(texture);
            npcHead.Left.Set((34 - npcHead.Width.GetValue(34)) / 2, 0);
            npcHead.Top.Set((34 - npcHead.Height.GetValue(34)) / 2, 0);
            npcHead.Width.Set(32, 0);
            npcHead.Height.Set(32, 0);

            Append(npcHead);
        }

        public override void Click(UIMouseEvent evt)
        {
            base.Click(evt);
            Selection = this;
            SetImage(ModContent.GetTexture("CustomNPCNames/UI/UINPCButton_Selected"));
            SetVisibility(1f, 1f);
            CustomNPCNames.renameUI.renameBox.UpdateStatus();
        }

        public override void Update(GameTime gameTime)
        {
            if (!ReferenceEquals(this, Selection))
            {
                SetImage(ModContent.GetTexture("CustomNPCNames/UI/UINPCButton"));
                SetVisibility(1f, 0.5f);
            }
            base.Update(gameTime);
        }
    }
}
