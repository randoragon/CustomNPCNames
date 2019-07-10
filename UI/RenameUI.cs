using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace CustomNPCNames.UI
{
    // ExampleUIs visibility is toggled by typing "/coin" in chat. (See CoinCommand.cs)
    // ExampleUI is a simple UI example showing how to use UIPanel, UIImageButton, and even a custom UIElement.
    internal class RenameUI : UIState
    {
        private DragableUIPanel menuPanel;
        private UIHoverImageButton closeButton;
        public static bool Visible = false;

        // In OnInitialize, we place various UIElements onto our UIState (this class).
        // UIState classes have width and height equal to the full screen, because of this, usually we first define a UIElement that will act as the container for our UI.
        // We then place various other UIElement onto that container UIElement positioned relative to the container UIElement.
        public override void OnInitialize()
        {
            // Here we define our container UIElement. In DragableUIPanel.cs, you can see that DragableUIPanel is a UIPanel with a couple added features.
            menuPanel = new DragableUIPanel();
            menuPanel.SetPadding(0);
            // We need to place this UIElement in relation to its Parent. Later we will be calling `base.Append(MenuPanel);`. 
            // This means that this class, ExampleUI, will be our Parent. Since ExampleUI is a UIState, the Left and Top are relative to the top left of the screen.
            menuPanel.Left.Set(400f, 0f);
            menuPanel.Top.Set(100f, 0f);
            menuPanel.Width.Set(320f, 0f);
            menuPanel.Height.Set(480f, 0f);
            

            Texture2D closeButtonTexture = ModContent.GetTexture("Terraria/UI/ButtonDelete");
            closeButton = new UIHoverImageButton(closeButtonTexture, "Close");
            closeButton.Left.Set(140, 0f);
            closeButton.Top.Set(10, 0f);
            closeButton.Width.Set(22, 0f);
            closeButton.Height.Set(22, 0f);
            closeButton.OnClick += new MouseEvent(CloseButtonClicked);
            menuPanel.Append(closeButton);
            
            Append(menuPanel);

            // As a recap, ExampleUI is a UIState, meaning it covers the whole screen. We attach MenuPanel to ExampleUI some distance from the top left corner.
            // We then place playButton, closeButton, and moneyDiplay onto MenuPanel so we can easily place these UIElements relative to MenuPanel.
            // Since MenuPanel will move, this proper organization will move playButton, closeButton, and moneyDiplay properly when MenuPanel moves.
        }

        private void CloseButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.PlaySound(SoundID.MenuClose);
            ModContent.PrintTextureInfo(ref closeButton.texture);
            Visible = false;
        }
    }
}