using Terraria.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;
using Terraria;

namespace CustomNPCNames.UI
{
    class UINPCButton : UIHoverImageButton
    {
        public UIImage NpcHead { get; private set; }
        public static UINPCButton Selection { get; private set; }
        public readonly short npcId;
        public readonly bool wide;

        public UINPCButton(Texture2D texture, string hoverText, short id, bool wide = false) : base(wide ? ModContent.GetTexture("CustomNPCNames/UI/UINPCButtonWide") : ModContent.GetTexture("CustomNPCNames/UI/UINPCButton"), hoverText)
        {
            npcId = id;
            this.wide = wide;

            OverflowHidden = true;
            SetPadding(1f);

            NpcHead = new UIImage(texture);
            NpcHead.Left.Set((34 - NpcHead.Width.GetValue(34) + (wide ? 34 : 0)) / 2, 0);
            NpcHead.Top.Set((34 - NpcHead.Height.GetValue(34)) / 2, 0);
            NpcHead.Width.Set(32, 0);
            NpcHead.Height.Set(32, 0);

            Append(NpcHead);
        }

        public override void Click(UIMouseEvent evt)
        {
            base.Click(evt);
            Selection = this;
            SetImage(wide ? ModContent.GetTexture("CustomNPCNames/UI/UINPCButtonWide_Selected") : ModContent.GetTexture("CustomNPCNames/UI/UINPCButton_Selected"));
            SetVisibility(1f, 1f);
            CustomNPCNames.renameUI.renamePanel.UpdateState();
            CustomNPCNames.renameUI.panelList.Clear();
            if (CustomWorld.CustomNames[Selection.npcId] != null && CustomWorld.CustomNames[Selection.npcId].Count > 0)
            {
                foreach (var i in CustomWorld.CustomNames[Selection.npcId])
                {
                    var entry = new UINameField(i);
                    entry.ContainCaption = false;
                    entry.SetScale(0.85f);
                    entry.SetWidth((0.85f * Main.fontMouseText.MeasureString("_________________________").X) + 18, 0);
                    entry.CaptionMaxLength = 25;
                    CustomNPCNames.renameUI.panelList.Add(entry);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!ReferenceEquals(this, Selection))
            {
                SetImage(wide ? ModContent.GetTexture("CustomNPCNames/UI/UINPCButtonWide") : ModContent.GetTexture("CustomNPCNames/UI/UINPCButton"));
                SetVisibility(1f, 0.5f);
            }
            base.Update(gameTime);
        }
    }
}
