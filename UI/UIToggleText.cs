using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework.Graphics;

namespace CustomNPCNames.UI
{
    /// <summary>
    /// Dead simple wrapper for UIText with added toggle functionality, e.g. turn visibility on and off.
    /// </summary>
    class UIToggleText : UIText
    {
        protected bool _visible = true;

        public UIToggleText(string text, float scale = 1f, bool large = false) : base(text, scale, large)
        { }

        public override void OnDeactivate()
        {
            base.OnDeactivate();
            _visible = false;
        }

        public override void OnActivate()
        {
            base.OnActivate();
            _visible = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_visible) {
                base.Draw(spriteBatch);
            }
        }
    }
}
