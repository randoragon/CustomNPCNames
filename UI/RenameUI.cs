using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.UI;
using System.Collections.Generic;
using System.Linq;

namespace CustomNPCNames.UI
{
    internal class RenameUI : UIState
    {
        private DragableUIPanel menuPanel;      // main window, parent for all the other UI objects
        private List<UINPCButton> menuNPCList;  // the left scrollable panel with NPC heads
        public UIHoverImageButton closeButton;
        public UIRenamePanel renameBox;          // the name bar on top of the entire menu, next to the close button
        public static bool Visible = false;

        public override void OnInitialize()
        {
            menuPanel = new DragableUIPanel();
            menuPanel.SetPadding(0);
            Rectangle menuCoords = new Rectangle(400, 100, 560, 596);
            menuPanel.Left.Set(menuCoords.X, 0f);
            menuPanel.Top.Set(menuCoords.Y, 0f);
            menuPanel.Width.Set(menuCoords.Width, 0f);
            menuPanel.Height.Set(menuCoords.Height, 0f);

            // Add buttons on the left side of the menu
            const int NPC_BUTTON_PADDING = 4;
            menuNPCList = new List<UINPCButton>();

            // Town NPC buttons
            for (int i = 0; i < 24; i++)
            {
                var newButton = GetNPCBossHeadButton(CustomNPCNames.TownNPCs[i]);
                newButton.Top.Set(60 + ((i % 12) * (34 + NPC_BUTTON_PADDING)), 0);
                newButton.Left.Set(8 + (i > 11 ? (34 + NPC_BUTTON_PADDING) : 0), 0);

                menuNPCList.Add(newButton);
                menuPanel.Append(menuNPCList[i]);
            }

            // Male-Female buttons
            menuNPCList.Add(new UINPCButton(ModContent.GetTexture("CustomNPCNames/UI/MaleIcon"),   "Male",   1000)); // NPCID 1000 is conventionally assigned to male
            menuNPCList.Last().Top.Set(60 + (12 * (34 + NPC_BUTTON_PADDING)), 0);
            menuNPCList.Last().Left.Set(8, 0);
            menuPanel.Append(menuNPCList.Last());
            menuNPCList.Add(new UINPCButton(ModContent.GetTexture("CustomNPCNames/UI/FemaleIcon"), "Female", 1001)); // NPCID 1001 is conventionally assigned to female
            menuNPCList.Last().Top.Set(60 + (12 * (34 + NPC_BUTTON_PADDING)), 0);
            menuNPCList.Last().Left.Set(8 + (34 + NPC_BUTTON_PADDING), 0);
            menuPanel.Append(menuNPCList.Last());

            // Global button
            menuNPCList.Add(new UINPCButton(ModContent.GetTexture("CustomNPCNames/UI/GlobalIcon"), "Global", 1002, true)); // NPCID 1002 is conventionally assigned to global
            menuNPCList.Last().Top.Set(60 + (13 * (34 + NPC_BUTTON_PADDING)), 0);
            menuNPCList.Last().Left.Set(8, 0);
            menuPanel.Append(menuNPCList.Last());

            // Add close button in the top right corner of the menu
            const int CLOSE_BUTTON_PADDING = 8;
            Texture2D closeButtonTexture = ModContent.GetTexture("CustomNPCNames/UI/close_button");
            closeButton = new UIHoverImageButton(closeButtonTexture, "Close");
            closeButton.Left.Set(menuCoords.Width - 22 - CLOSE_BUTTON_PADDING, 0f);
            closeButton.Top.Set(CLOSE_BUTTON_PADDING, 0f);
            closeButton.Width.Set(22, 0f);
            closeButton.Height.Set(22, 0f);
            closeButton.OnClick += new MouseEvent(CloseButtonClicked);
            menuPanel.Append(closeButton);

            // Add rename panel in the top middle part of the menu
            renameBox = new UIRenamePanel();
            renameBox.OverflowHidden = true;
            renameBox.CaptionMaxLength = 25;
            renameBox.Top.Set(8, 0);
            renameBox.HAlign = 0.5f;
            renameBox.Left.Set(-14, 0);
            renameBox.Height.Set(40, 0);
            renameBox.Width.Set(menuCoords.Width - 8 - 22 - 8, 0); // 4px padding from both sides, thus the additional -8
            menuPanel.Append(renameBox);

            Append(menuPanel);
        }

        private void CloseButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.PlaySound(SoundID.MenuClose);
            Visible = false;
        }

        private UINPCButton GetNPCBossHeadButton(short id)
        {
            int textureId = 0;
            string npcName = "An Error Occurred";

            switch(id)
            {
                case NPCID.Guide:
                    textureId = 1; npcName = "Guide";
                    break;
                case NPCID.Merchant:
                    textureId = 2; npcName = "Merchant";
                    break;
                case NPCID.Nurse:
                    textureId = 3; npcName = "Nurse";
                    break;
                case NPCID.Demolitionist:
                    textureId = 4; npcName = "Demolitionist";
                    break;
                case NPCID.Dryad:
                    textureId = 5; npcName = "Dryad";
                    break;
                case NPCID.ArmsDealer:
                    textureId = 6; npcName = "Arms Dealer";
                    break;
                case NPCID.Clothier:
                    textureId = 7; npcName = "Clothier";
                    break;
                case NPCID.Mechanic:
                    textureId = 8; npcName = "Mechanic";
                    break;
                case NPCID.GoblinTinkerer:
                    textureId = 9; npcName = "Goblin Tinkerer";
                    break;
                case NPCID.Wizard:
                    textureId = 10; npcName = "Wizard";
                    break;
                case NPCID.SantaClaus:
                    textureId = 11; npcName = "Santa Claus";
                    break;
                case NPCID.Truffle:
                    textureId = 12; npcName = "Truffle";
                    break;
                case NPCID.Steampunker:
                    textureId = 13; npcName = "Steampunker";
                    break;
                case NPCID.DyeTrader:
                    textureId = 14; npcName = "Dye Trader";
                    break;
                case NPCID.PartyGirl:
                    textureId = 15; npcName = "Party Girl";
                    break;
                case NPCID.Cyborg:
                    textureId = 16; npcName = "Cyborg";
                    break;
                case NPCID.Painter:
                    textureId = 17; npcName = "Painter";
                    break;
                case NPCID.WitchDoctor:
                    textureId = 18; npcName = "Witch Doctor";
                    break;
                case NPCID.Pirate:
                    textureId = 19; npcName = "Pirate";
                    break;
                case NPCID.Stylist:
                    textureId = 20; npcName = "Stylist";
                    break;
                case NPCID.TravellingMerchant:
                    textureId = 21; npcName = "Travelling Merchant";
                    break;
                case NPCID.Angler:
                    textureId = 22; npcName = "Angler";
                    break;
                case NPCID.TaxCollector:
                    textureId = 23; npcName = "Tax Collector";
                    break;
                case NPCID.DD2Bartender:
                    textureId = 24; npcName = "Tavernkeep";
                    break;
            }
            return new UINPCButton(ModContent.GetTexture("Terraria/NPC_Head_" + System.Convert.ToString(textureId)), npcName, id);
        }
    }
}