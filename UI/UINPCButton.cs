using Terraria.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;

namespace CustomNPCNames.UI
{
    class UINPCButton : UIHoverImageButton, IDragableUIPanelChild
    {
        public UIImage NpcHead { get; private set; }
        public static UINPCButton Selection { get; protected set; }
        public readonly short npcId;
        public readonly bool wide;
        bool IDragableUIPanelChild.Hover
        {
            get
            {
                MouseState mouse = Mouse.GetState();
                Rectangle pos = InterfaceHelper.GetFullRectangle(this);
                return (mouse.X >= pos.X && mouse.X <= pos.X + pos.Width && mouse.Y >= pos.Y && mouse.Y <= pos.Y + pos.Height);
            }
        }

        public UINPCButton(Texture2D texture, string hoverText, short id, bool wide = false) : base(wide ? ModContent.GetTexture("CustomNPCNames/UI/UINPCButtonWide") : ModContent.GetTexture("CustomNPCNames/UI/UINPCButton"), hoverText)
        {
            npcId = id;
            this.wide = wide;

            OverflowHidden = true;
            SetPadding(1f);

            NpcHead = new UIImage(texture);
            NpcHead.Left.Set(((34 - NpcHead.Width.Pixels + (wide ? 34 : 0)) / 2) + CustomNPCNames.npcHeadOffset[id].X, 0);
            NpcHead.Top.Set(((34 - NpcHead.Height.Pixels) / 2) + CustomNPCNames.npcHeadOffset[id].Y, 0);
            NpcHead.Width.Set(32, 0);
            NpcHead.Height.Set(32, 0);

            Append(NpcHead);
        }

        public static void Refresh()
        {
            if (Selection != null) {
                var evt = new UIMouseEvent(Selection, Vector2.Zero);
                Selection.Click(evt);
            }
        }

        public static void Deselect()
        {
            Selection = null;
        }

        public override void Click(UIMouseEvent evt)
        {
            base.Click(evt);
            Selection = this;

            RenameUI.renamePanel.UpdateState();
            RenameUI.panelList.PrintContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!ReferenceEquals(this, Selection) || Selection == null)
            {
                SetImage(wide ? ModContent.GetTexture("CustomNPCNames/UI/UINPCButtonWide") : ModContent.GetTexture("CustomNPCNames/UI/UINPCButton"));
                SetVisibility(1f, 0.5f);
            } else {
                SetImage(wide ? ModContent.GetTexture("CustomNPCNames/UI/UINPCButtonWide_Selected") : ModContent.GetTexture("CustomNPCNames/UI/UINPCButton_Selected"));
                SetVisibility(1f, 1f);
            }
        }
    }
}
