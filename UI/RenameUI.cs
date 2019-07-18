using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.UI;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework.Input;

namespace CustomNPCNames.UI
{
    internal class RenameUI : UIState
    {
        private DragableUIPanel menuPanel;      // main window, parent for all the other UI objects
        private List<UINPCButton> menuNPCList;  // the left stripe with NPC heads
        public UIHoverImageButton closeButton;
        public UIHoverImageButton helpButton;
        public UIRenamePanel renamePanel;       // the name bar on top of the entire menu, next to the close button
        public UINameList panelList;            // the big list of names for each category
        public UIScrollbar panelListScrollbar;  // panelList's scrollbar
        public UIPanelDragableChild namesPanel; // container within menuPanel for panelList and its buttons
        public UIHoverImageButton addButton;
        public UIHoverImage addButtonInactive;
        public UIHoverImageButton removeButton;
        public UIHoverImage removeButtonInactive;
        public UIHoverImageButton clearButton;
        public UIHoverImage clearButtonInactive;
        public UIHoverImageButton switchGenderButton;
        public UIHoverImage switchGenderButtonInactive;
        public UIHoverImageButton randomizeButton;
        public UIHoverImage randomizeButtonInactive;
        public UINPCPreview npcPreview;
        public UIModeCycleButton modeCycleButton;
        public UIToggleUniqueButton uniqueNameButton;
        public bool removeMode = false;
        public UIImage trashIcon;
        public bool Visible = false;
        public bool IsNPCSelected { get { return UINPCButton.Selection != null; } }
        public short SelectedNPC { get { return UINPCButton.Selection.npcId; } }    // always check IsNPCSelected before calling this
        

        public override void OnInitialize()
        {
            menuPanel = new DragableUIPanel();
            menuPanel.SetPadding(0);
            Rectangle menuCoords = new Rectangle(400, 100, 570, 596);
            menuPanel.Left.Set(menuCoords.X, 0f);
            menuPanel.Top.Set(menuCoords.Y, 0f);
            menuPanel.Width.Set(menuCoords.Width, 0f);
            menuPanel.Height.Set(menuCoords.Height, 0f);

            // Town NPC buttons
            const int NPC_BUTTON_PADDING = 4;
            menuNPCList = new List<UINPCButton>();
            for (int i = 0; i < 24; i++)
            {
                short id = CustomNPCNames.TownNPCs[i];
                var newButton = new UINPCButton(GetNPCHeadTexture(id), CustomNPCNames.GetNPCName(id), id);
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

            // Close button in the top right corner of the menu
            const int SMALL_BUTTON_PADDING = 8;
            closeButton = new UIHoverImageButton(ModContent.GetTexture("CustomNPCNames/UI/close_button"), "Close");
            closeButton.Left.Set(menuCoords.Width - 22 - SMALL_BUTTON_PADDING, 0f);
            closeButton.Top.Set(SMALL_BUTTON_PADDING, 0f);
            closeButton.Width.Set(22, 0f);
            closeButton.Height.Set(22, 0f);
            closeButton.OnClick += new MouseEvent(CloseButtonClicked);
            menuPanel.Append(closeButton);

            // Help button below the close button
            helpButton = new UIHoverImageButton(ModContent.GetTexture("CustomNPCNames/UI/help_button"), "Help");
            helpButton.Left.Set(menuCoords.Width - 22 - SMALL_BUTTON_PADDING, 0f);
            helpButton.Top.Set(SMALL_BUTTON_PADDING + 22 + 2, 0f);
            helpButton.Width.Set(22, 0f);
            helpButton.Height.Set(22, 0f);
            helpButton.OnClick += new MouseEvent(HelpButtonClicked);
            menuPanel.Append(helpButton);

            // Rename panel in the top middle part of the menu
            renamePanel = new UIRenamePanel();
            renamePanel.OverflowHidden = true;
            renamePanel.CaptionMaxLength = 25;
            renamePanel.Top.Set(8, 0);
            renamePanel.HAlign = 0.5f;
            renamePanel.Left.Set(-14, 0);
            renamePanel.Height.Set(40, 0);
            renamePanel.Width.Set(menuCoords.Width - 8 - 22 - 8, 0); // 4px padding from both sides, thus the additional -8
            menuPanel.Append(renamePanel);

            // Custom names panel
            namesPanel = new UIPanelDragableChild();
            namesPanel.OverflowHidden = true;
            namesPanel.SetPadding(3f);
            namesPanel.Left.Set(86, 0);
            namesPanel.Top.Set(60, 0);
            namesPanel.Width.Set(476, 0);
            namesPanel.Height.Set(490, 0);

            // Custom names list
            panelList = new UINameList();
            panelList.ListPadding = 2f;
            panelList.Top.Set(36, 0);
            panelList.Left.Set(4, 0);
            panelList.Height.Set(449, 0);
            panelList.Width.Set(460, 0);
            panelList.OverflowHidden = true;
            panelListScrollbar = new UIScrollbar();
            panelList.SetScrollbar(panelListScrollbar);
            panelList.Append(panelListScrollbar);
            namesPanel.Append(panelList);

            // Add, remove, clear, switch gender, randomize buttons
            addButton = new UIHoverImageButton(ModContent.GetTexture("CustomNPCNames/UI/add_button"), "Add Name");
            addButton.Top.Set(2, 0);
            addButton.Left.Set(4, 0);
            addButton.Width.Set(120, 0);
            addButton.Height.Set(30, 0);
            addButton.OnClick += new MouseEvent(AddButtonClicked);
            addButtonInactive = new UIHoverImage(ModContent.GetTexture("CustomNPCNames/UI/add_button_inactive"), "No NPC Selected");
            addButtonInactive.Top.Set(2, 0);
            addButtonInactive.Left.Set(4, 0);
            addButtonInactive.Width.Set(120, 0);
            addButtonInactive.Height.Set(30, 0);
            namesPanel.Append(addButtonInactive);
            removeButton = new UIHoverImageButton(ModContent.GetTexture("CustomNPCNames/UI/remove_button"), "Remove Name");
            removeButton.Top.Set(2, 0);
            removeButton.Left.Set(126, 0);
            removeButton.Width.Set(60, 0);
            removeButton.Height.Set(30, 0);
            removeButton.OnClick += new MouseEvent(RemoveButtonClicked);
            removeButtonInactive = new UIHoverImage(ModContent.GetTexture("CustomNPCNames/UI/remove_button_inactive"), "No NPC Selected");
            removeButtonInactive.Top.Set(2, 0);
            removeButtonInactive.Left.Set(126, 0);
            removeButtonInactive.Width.Set(60, 0);
            removeButtonInactive.Height.Set(30, 0);
            namesPanel.Append(removeButtonInactive);
            clearButton = new UIHoverImageButton(ModContent.GetTexture("CustomNPCNames/UI/clear_button"), "Clear All (hold Alt)");
            clearButton.Top.Set(2, 0);
            clearButton.Left.Set(188, 0);
            clearButton.Width.Set(30, 0);
            clearButton.Height.Set(30, 0);
            clearButton.OnClick += new MouseEvent(ClearButtonClicked);
            clearButtonInactive = new UIHoverImage(ModContent.GetTexture("CustomNPCNames/UI/clear_button_inactive"), "No NPC Selected");
            clearButtonInactive.Top.Set(2, 0);
            clearButtonInactive.Left.Set(188, 0);
            clearButtonInactive.Width.Set(30, 0);
            clearButtonInactive.Height.Set(30, 0);
            namesPanel.Append(clearButtonInactive);
            switchGenderButton = new UIHoverImageButton(ModContent.GetTexture("CustomNPCNames/UI/switch_gender_button"), "Switch Gender");
            switchGenderButton.Top.Set(2, 0);
            switchGenderButton.Left.Set(476 - 4 - 60 - 6 - 2 - 60, 0);
            switchGenderButton.Width.Set(60, 0);
            switchGenderButton.Height.Set(30, 0);
            switchGenderButton.OnClick += new MouseEvent(SwitchGenderButtonClicked);
            switchGenderButtonInactive = new UIHoverImage(ModContent.GetTexture("CustomNPCNames/UI/switch_gender_button_inactive"), "No NPC Selected");
            switchGenderButtonInactive.Top.Set(2, 0);
            switchGenderButtonInactive.Left.Set(476 - 4 - 60 - 6 - 2 - 60, 0);
            switchGenderButtonInactive.Width.Set(60, 0);
            switchGenderButtonInactive.Height.Set(30, 0);
            namesPanel.Append(switchGenderButtonInactive);
            randomizeButton = new UIHoverImageButton(ModContent.GetTexture("CustomNPCNames/UI/randomize_button"), "Randomize Name");
            randomizeButton.Top.Set(2, 0);
            randomizeButton.Left.Set(476 - 4 - 60 - 6, 0);
            randomizeButton.Width.Set(60, 0);
            randomizeButton.Height.Set(30, 0);
            randomizeButton.OnClick += new MouseEvent(RandomizeButtonClicked);
            randomizeButtonInactive = new UIHoverImage(ModContent.GetTexture("CustomNPCNames/UI/randomize_button_inactive"), "No NPC Selected");
            randomizeButtonInactive.Top.Set(2, 0);
            randomizeButtonInactive.Left.Set(476 - 4 - 60 - 6, 0);
            randomizeButtonInactive.Width.Set(60, 0);
            randomizeButtonInactive.Height.Set(30, 0);
            namesPanel.Append(randomizeButtonInactive);

            menuPanel.Append(namesPanel);

            // NPC Preview
            npcPreview = new UINPCPreview();
            npcPreview.Top.Set(8, 0);
            npcPreview.Left.Set(476 - 4 - 60 - 6 - 2 - 60 - 60 - 6, 0);
            npcPreview.Width.Set(70, 0);
            npcPreview.Height.Set(30, 0);

            // Mode Cycle Button
            modeCycleButton = new UIModeCycleButton();
            modeCycleButton.Top.Set(596 - 41, 0);
            modeCycleButton.Left.Set(86, 0);
            modeCycleButton.SetScale(0.85f);
            menuPanel.Append(modeCycleButton);

            // Toggle Unique Name Button
            uniqueNameButton = new UIToggleUniqueButton();
            uniqueNameButton.Top.Set(596 - 41, 0);
            uniqueNameButton.Left.Set(86 + 210, 0);
            uniqueNameButton.SetScale(0.85f);
            menuPanel.Append(uniqueNameButton);

            Append(menuPanel);

            trashIcon = new UIImage(ModContent.GetTexture("CustomNPCNames/UI/trash_icon"));
        }

        private void CloseButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.PlaySound(SoundID.MenuClose);
            Visible = false;
        }

        private void HelpButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.NewText(" ");
            Main.NewText("CUSTOM NPC NAMES HELP", new Color(255, 190, 40));
            Main.NewText("1. Changing an NPC's name manually", new Color(50, 125, 190));
            Main.NewText("  Click an NPC's button. If the NPC exists, their current name will appear");
            Main.NewText("  on the name plate in the top middle of the pop-up menu. To edit the name,");
            Main.NewText("  simply click on the plate and start typing. When you're finished, either");
            Main.NewText("  click Enter or anywhere out of the plate. To cancel your changes press Escape.");
            Main.NewText("2. Making lists of names", new Color(50, 125, 190));
            Main.NewText("  To add/remove entries use the Add and Remove buttons respectively.");
            Main.NewText("  You may edit already added entries the same way you do the name plate.");
            Main.NewText("  Each of the NPC, Male, Female and Global buttons has a separate list of names.");
            Main.NewText("3. NPC name randomization methods", new Color(50, 125, 190));
            Main.NewText("  On the middle bottom of the menu you'll find a text box where you can pick");
            Main.NewText("  from 4 methods of randomizing names of newly spawned NPCs:");
            Main.NewText("  A) USE VANILLA NAMES", new Color(255, 255, 120));
            Main.NewText("    This method will ignore the modded name lists and use default vanilla names.");
            Main.NewText("    You will still be able to change individual names manually.");
            Main.NewText("  B) USE CUSTOM NAMES", new Color(255, 255, 120));
            Main.NewText("    This method will use each of the NPC lists' names adequately to the NPC");
            Main.NewText("    that's being spawned. Male, Female and Global lists will be ignored.");
            Main.NewText("  C) USE GENDER NAMES", new Color(255, 255, 120));
            Main.NewText("    This method will use Male and Female lists in choosing the name of an NPC.");
            Main.NewText("    An NPC's gender can be changed by selecting them and pressing Switch Gender.");
            Main.NewText("  D) USE GLOBAL NAMES", new Color(255, 255, 120));
            Main.NewText("    This method will use the Global list only. ");
            Main.NewText(" ");
            Main.NewText("  For each method you can toggle the UNIQUE NAMES box. If enabled,");
            Main.NewText("  the mod will attempt to pick names not picked before, HOWEVER names may repeat");
            Main.NewText("  if there's not enough unique entries to pick from. The more, the better.");
            Main.NewText(" ");
            Main.NewText("(open the chat and use arrow keys to read)", new Color(0, 255, 0));
        }

        private void AddButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            SetRemoveMode(false);

            string newName = "";
            var newWrapper = new StringWrapper(ref newName);

            CustomWorld.CustomNames[UINPCButton.Selection.npcId].Add(newWrapper);
            var field = new UINameField(newWrapper, (uint)panelList.Count);
            field.IsNew = true;
            panelList.Add(field);
            DeselectAllEntries();
            field.HasFocus = true;
            field.Select();
        }

        private void RemoveButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            SetRemoveMode(!removeMode);
        }

        public void SetRemoveMode(bool active)
        {
            removeMode = active;

            if (!active) {
                DeselectAllEntries();
                RemoveChild(trashIcon);
                removeButton.SetVisibility(1f, 0.5f);
                removeButton.HoverText = "Remove Name";
            } else {
                Append(trashIcon);
                removeButton.SetVisibility(1f, 1f);
                removeButton.HoverText = "";
            }
        }

        private void ClearButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            KeyboardState key = Keyboard.GetState();
            if (key.IsKeyDown(Keys.LeftAlt) || key.IsKeyDown(Keys.RightAlt)) {
                foreach (UINameField i in panelList._items) {
                    CustomWorld.CustomNames[UINPCButton.Selection.npcId].Remove(i.NameWrapper);
                    panelList.RemoveName(i);
                }
            }
        }

        private void SwitchGenderButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            NPCs.CustomNPC.isMale[UINPCButton.Selection.npcId] = !NPCs.CustomNPC.isMale[UINPCButton.Selection.npcId];
            npcPreview.UpdateNPC(UINPCButton.Selection.npcId);
        }

        private void RandomizeButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            NPCs.CustomNPC.RandomizeName(UINPCButton.Selection.npcId);
        }

        public void DeselectAllEntries()
        {
            CustomNPCNames.renameUI.panelList.DeselectAll();
            renamePanel.Deselect();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            UpdateUIStates();

            if (removeMode) {
                var mouse = Mouse.GetState();
                trashIcon.Left.Set(mouse.X + 15, 0);
                trashIcon.Top.Set(mouse.Y + 15, 0);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public void UpdateUIStates()
        {
            if (IsNPCSelected) {
                short id = SelectedNPC;
                bool noNames = (id != 1000 && id != 1001 && id != 1002)
                           && ((CustomNPCNames.mode == 1 && CustomWorld.CustomNames[id].Count == 0)
                            || (CustomNPCNames.mode == 2 && CustomWorld.CustomNames[(short)(NPCs.CustomNPC.isMale[id] ? 1000 : 1001)].Count == 0)
                            || (CustomNPCNames.mode == 3 && CustomWorld.CustomNames[1002].Count == 0));

                namesPanel.RemoveChild(addButtonInactive);
                namesPanel.Append(addButton);
                namesPanel.RemoveChild(removeButtonInactive);
                namesPanel.Append(removeButton);
                namesPanel.RemoveChild(clearButtonInactive);
                namesPanel.Append(clearButton);

                if (id == 1000 || id == 1001 || id == 1002) {
                    namesPanel.RemoveChild(npcPreview);
                    namesPanel.RemoveChild(switchGenderButton);
                    namesPanel.Append(switchGenderButtonInactive);
                    namesPanel.RemoveChild(randomizeButton);
                    namesPanel.Append(randomizeButtonInactive);
                    randomizeButtonInactive.HoverText = "No NPC Selected";
                } else {
                    npcPreview.UpdateNPC(id);
                    namesPanel.Append(npcPreview);
                    namesPanel.RemoveChild(switchGenderButtonInactive);
                    namesPanel.Append(switchGenderButton);

                    if (NPC.CountNPCS(id) == 0) {
                        namesPanel.RemoveChild(randomizeButton);
                        namesPanel.Append(randomizeButtonInactive);
                        randomizeButtonInactive.HoverText = "This NPC is not alive\nand cannot be renamed!";
                    } else {
                        if (noNames) {
                            namesPanel.RemoveChild(randomizeButton);
                            namesPanel.Append(randomizeButtonInactive);
                            randomizeButtonInactive.HoverText = "There are no names\non the list to choose from!";
                        } else {
                            namesPanel.RemoveChild(randomizeButtonInactive);
                            namesPanel.Append(randomizeButton);
                        }
                    }
                }
            } else {
                namesPanel.RemoveChild(addButton);
                namesPanel.Append(addButtonInactive);
                namesPanel.RemoveChild(removeButton);
                namesPanel.Append(removeButtonInactive);
                namesPanel.RemoveChild(clearButton);
                namesPanel.Append(clearButtonInactive);

                namesPanel.RemoveChild(randomizeButton);
                namesPanel.Append(randomizeButtonInactive);
                randomizeButtonInactive.HoverText = "No NPC Selected";
            }
        }

        public static Texture2D GetNPCHeadTexture(short id)
        {
            int textureId = 0;
            switch(id)
            {
                case NPCID.Guide:
                    textureId = 1;
                    break;
                case NPCID.Merchant:
                    textureId = 2;
                    break;
                case NPCID.Nurse:
                    textureId = 3;
                    break;
                case NPCID.Demolitionist:
                    textureId = 4;
                    break;
                case NPCID.Dryad:
                    textureId = 5;
                    break;
                case NPCID.ArmsDealer:
                    textureId = 6;
                    break;
                case NPCID.Clothier:
                    textureId = 7;
                    break;
                case NPCID.Mechanic:
                    textureId = 8;
                    break;
                case NPCID.GoblinTinkerer:
                    textureId = 9;
                    break;
                case NPCID.Wizard:
                    textureId = 10;
                    break;
                case NPCID.SantaClaus:
                    textureId = 11;
                    break;
                case NPCID.Truffle:
                    textureId = 12;
                    break;
                case NPCID.Steampunker:
                    textureId = 13;
                    break;
                case NPCID.DyeTrader:
                    textureId = 14;
                    break;
                case NPCID.PartyGirl:
                    textureId = 15;
                    break;
                case NPCID.Cyborg:
                    textureId = 16;
                    break;
                case NPCID.Painter:
                    textureId = 17;
                    break;
                case NPCID.WitchDoctor:
                    textureId = 18;
                    break;
                case NPCID.Pirate:
                    textureId = 19;
                    break;
                case NPCID.Stylist:
                    textureId = 20;
                    break;
                case NPCID.TravellingMerchant:
                    textureId = 21;
                    break;
                case NPCID.Angler:
                    textureId = 22;
                    break;
                case NPCID.TaxCollector:
                    textureId = 23;
                    break;
                case NPCID.DD2Bartender:
                    textureId = 24;
                    break;
            }
            return ModContent.GetTexture("Terraria/NPC_Head_" + System.Convert.ToString(textureId));
        }
    }
}