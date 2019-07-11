using Terraria;
using Terraria.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;

namespace CustomNPCNames.UI
{
    class UINPCButton : UIHoverImageButton
    {
        public UIImage npcHead { get; private set; }
        protected string hoverText;
        public static UINPCButton Selection { get; private set; }
        private bool amISelected;

        public UINPCButton(Texture2D texture, string hoverText) : base(ModContent.GetTexture("CustomNPCNames/UI/UINPCButton"), hoverText)
        {
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
        }

        public override void Update(GameTime gameTime)
        {
            if (this != Selection)
            {
                SetImage(ModContent.GetTexture("CustomNPCNames/UI/UINPCButton"));
                SetVisibility(1f, 0.5f);
            }
            base.Update(gameTime);
        }
    }
}
