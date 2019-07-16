using Terraria.ModLoader;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;

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

        public UINameField(StringWrapper name) : base(name.str)
        {
            this.name = name;

            focusVariant.Caption.HAlign = 0f;
            focusVariant.Caption.Left.Set(6, 0);
            idleVariant.Caption.HAlign = 0f;
            idleVariant.Caption.Left.Set(6, 0);
            SetText(name.str);
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
            focusVariant.BorderColor.A = 60;
            idleVariant.BorderColor.A = 60;
        }
    }
}
