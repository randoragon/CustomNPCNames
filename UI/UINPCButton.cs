using Terraria.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;

namespace CustomNPCNames.UI
{
    class UINPCButton : UIHoverImageButton
    {
        public UIImage NpcHead { get; private set; }
        public static UINPCButton Selection { get; private set; }
        public readonly short npcId;

        public UINPCButton(Texture2D texture, string hoverText, short id) : base(ModContent.GetTexture("CustomNPCNames/UI/UINPCButton"), hoverText)
        {
            npcId = id;

            OverflowHidden = true;
            SetPadding(1f);

            NpcHead = new UIImage(texture);
            NpcHead.Left.Set((34 - NpcHead.Width.GetValue(34)) / 2, 0);
            NpcHead.Top.Set((34 - NpcHead.Height.GetValue(34)) / 2, 0);
            NpcHead.Width.Set(32, 0);
            NpcHead.Height.Set(32, 0);

            Append(NpcHead);
        }

        public override void Click(UIMouseEvent evt)
        {
            base.Click(evt);
            Selection = this;
            SetImage(ModContent.GetTexture("CustomNPCNames/UI/UINPCButton_Selected"));
            SetVisibility(1f, 1f);
            CustomNPCNames.renameUI.renameBox.UpdateState();
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
