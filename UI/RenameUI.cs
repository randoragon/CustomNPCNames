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
        public static DragableUIPanel menuPanel;      // main window, parent for all the other UI objects
        public static List<UINPCButton> menuNPCList;  // the left stripe with NPC heads
        public static UIHoverImageButton closeButton;
        public static UIHoverImageButton helpButton;
        public static UIRenamePanel renamePanel;       // the name bar on top of the entire menu, next to the close button
        public static UINameList panelList;            // the big list of names for each category
        public static UIScrollbar panelListScrollbar;  // panelList's scrollbar
        public static UIPanelDragableChild namesPanel; // container within menuPanel for panelList and its buttons
        public static UIHoverImageButton addButton;
        public static UIHoverImage addButtonInactive;
        public static UIHoverImageButton removeButton;
        public static UIHoverImage removeButtonInactive;
        public static UIHoverImageButton clearButton;
        public static UIHoverImage clearButtonInactive;
        public static UIHoverImageButton switchGenderButton;
        public static UIHoverImage switchGenderButtonInactive;
        public static UIHoverImageButton randomizeButton;
        public static UIHoverImage randomizeButtonInactive;
        public static UINPCPreview npcPreview;
        public static UIModeCycleButton modeCycleButton;
        public static UIToggleUniqueButton uniqueNameButton;
        public static UIHoverImageButton copyButton;
        public static UIHoverImageButton pasteButton;
        public static UIHoverImage pasteButtonInactive;
        public static UIHoverImageButton carryButton;
        public static UIToggleText listMessage;
        public static UIToggleText listCount;
        public static bool removeMode = false;
        public static UIImage trashIcon;
        public static bool carry = true;
        public static ListData copyData;
        public static bool Visible = false;
        public static bool IsNPCSelected { get { return UINPCButton.Selection != null; } }
        public static short SelectedNPC { get { return UINPCButton.Selection.npcId; } }    // always check IsNPCSelected before calling this
        public static short savedSelectedNPC = -1;  // this variable gets assigned a value when loading a world file

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
            for (int i = 0; i < 24; i++) {
                short id = CustomNPCNames.TownNPCs[i];
                var newButton = new UINPCButton(GetNPCHeadTexture(id), CustomNPCNames.GetNPCName(id), id);
                newButton.Top.Set(60 + ((i % 12) * (34 + NPC_BUTTON_PADDING)), 0);
                newButton.Left.Set(8 + (i > 11 ? (34 + NPC_BUTTON_PADDING) : 0), 0);

                menuNPCList.Add(newButton);
                menuPanel.Append(menuNPCList[i]);
            }

            // Male-Female buttons
            menuNPCList.Add(new UINPCButton(ModContent.GetTexture("CustomNPCNames/UI/MaleIcon"), "Male", 1000)); // NPCID 1000 is conventionally assigned to male
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
            panelList.Width.Set(470, 0);
            panelList.Height.Set(449, 0);
            panelList.OverflowHidden = true;
            panelListScrollbar = new UIScrollbar();
            panelListScrollbar.Top.Set(42, 0);
            panelListScrollbar.Left.Set(449, 0);
            panelListScrollbar.Height.Set(433, 0);
            panelList.SetScrollbar(panelListScrollbar);
            namesPanel.Append(panelListScrollbar);
            namesPanel.Append(panelList);

            // List message
            listMessage = new UIToggleText("", 0.9f);
            listMessage.Top.Pixels = -15;
            listMessage.Left.Pixels = -10;
            listMessage.HAlign = 0.5f;
            listMessage.VAlign = 0.5f;
            listMessage.Deactivate();
            namesPanel.Append(listMessage);

            // List count
            listCount = new UIToggleText("00/00");
            listCount.TextColor = Color.LightGoldenrodYellow;
            listCount.Top.Pixels = 10;
            listCount.Left.Pixels = 188 + 30 + 6;
            namesPanel.Append(listCount);

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
            uniqueNameButton.Left.Set(86 + 206, 0);
            uniqueNameButton.SetScale(0.85f);
            menuPanel.Append(uniqueNameButton);

            // Copy, Paste, Carry buttons
            copyButton = new UIHoverImageButton(ModContent.GetTexture("CustomNPCNames/UI/copy_button"), "Copy Everything\n(hold Alt to Cut)");
            copyButton.Top.Set(596 - 41, 0);
            copyButton.Left.Set(86 + 206 + 182, 0);
            copyButton.OnClick += new MouseEvent(CopyButtonClicked);
            pasteButton = new UIHoverImageButton(ModContent.GetTexture("CustomNPCNames/UI/paste_button"), "Paste Everything");
            pasteButton.Top.Set(596 - 41, 0);
            pasteButton.Left.Set(86 + 206 + 182 + 30, 0);
            pasteButton.OnClick += new MouseEvent(PasteButtonClicked);
            pasteButtonInactive = new UIHoverImage(ModContent.GetTexture("CustomNPCNames/UI/paste_button_inactive"), "Mod Clipboard\nis empty");
            pasteButtonInactive.Top.Set(596 - 41, 0);
            pasteButtonInactive.Left.Set(86 + 206 + 182 + 30, 0);
            carryButton = new UIHoverImageButton(ModContent.GetTexture("CustomNPCNames/UI/carry_button_on"), "Toggle Data Carriage");
            carryButton.Top.Set(596 - 41, 0);
            carryButton.Left.Set(86 + 206 + 182 + 60, 0);
            carryButton.OnClick += new MouseEvent(CarryButtonClicked);
            menuPanel.Append(copyButton);
            menuPanel.Append(pasteButtonInactive);
            menuPanel.Append(carryButton);
            copyData = new ListData();

            Append(menuPanel);

            trashIcon = new UIImage(ModContent.GetTexture("CustomNPCNames/UI/trash_icon"));
        }

        public static void Unload()
        {
            menuPanel = null;
            menuNPCList = null;
            closeButton = null;
            helpButton = null;
            renamePanel = null;
            panelList = null;
            panelListScrollbar = null;
            namesPanel = null;
            addButton = null;
            addButtonInactive = null;
            removeButton = null;
            removeButtonInactive = null;
            clearButton = null;
            clearButtonInactive = null;
            switchGenderButton = null;
            switchGenderButtonInactive = null;
            randomizeButton = null;
            randomizeButtonInactive = null;
            npcPreview = null;
            modeCycleButton = null;
            uniqueNameButton = null;
            copyButton = null;
            pasteButton = null;
            pasteButtonInactive = null;
            carryButton = null;
            listMessage = null;
            listCount = null;
            trashIcon = null;
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
            Main.NewText("1. Changing an NPC's Name Manually", new Color(50, 125, 190));
            Main.NewText("  Click an NPC's button. If the NPC exists, their current name will appear");
            Main.NewText("  on the name plate in the top middle of the pop-up menu. To edit the name,");
            Main.NewText("  simply click on the plate and start typing. When you're finished, either");
            Main.NewText("  click Enter or anywhere out of the plate. To cancel your changes press Escape.");
            Main.NewText("2. Making Lists of Names", new Color(50, 125, 190));
            Main.NewText("  To add/remove entries use the Add and Remove buttons respectively.");
            Main.NewText("  You may edit already added entries the same way you would the name plate.");
            Main.NewText("  Each of the NPC, Male, Female and Global buttons holds a separate name list.");
            Main.NewText("3. NPC Name Randomization Methods", new Color(50, 125, 190));
            Main.NewText("  On the middle bottom of the menu you'll find a text box where you can pick");
            Main.NewText("  from 4 modes of randomizing names of newly spawned NPCs:");
            Main.NewText("  A) USING: VANILLA NAMES", new Color(255, 255, 120));
            Main.NewText("    This method will ignore the modded name lists and use default vanilla names.");
            Main.NewText("    You will only be able to change individual names manually.");
            Main.NewText("  B) USING: CUSTOM NAMES", new Color(255, 255, 120));
            Main.NewText("    This method will use each of the NPC lists' names adequately to the NPC");
            Main.NewText("    that's being spawned. Male, Female and Global lists will be ignored.");
            Main.NewText("  C) USING: GENDER NAMES", new Color(255, 255, 120));
            Main.NewText("    This method will use Male and Female lists in choosing the name of an NPC.");
            Main.NewText("    An NPC's gender can be changed by selecting them and pressing Switch Gender.");
            Main.NewText("  D) USING: GLOBAL NAMES", new Color(255, 255, 120));
            Main.NewText("    This method will use the Global list only. ");
            Main.NewText("4. The Unique Names Setting", new Color(50, 125, 190));
            Main.NewText("  For each method you can toggle the UNIQUE NAMES box. If enabled,");
            Main.NewText("  the mod will choose names not picked before first. However, names may repeat");
            Main.NewText("  if there's not enough unique entries. This has no effect in Vanilla Mode.");
            Main.NewText("5. Randomizing Entire Groups", new Color(50, 125, 190));
            Main.NewText("  Clicking the Randomize button inside Male, Female or Global tabs will cause");
            Main.NewText("  every NPC in that group to randomize its name. It's literally the same as");
            Main.NewText("  going through every such NPC and randomizing their name individually.");
            Main.NewText("6. Dealing with Empty Lists", new Color(50, 125, 190));
            Main.NewText("  If a target list is empty, e.g. Mode is set to CUSTOM NAMES but some NPCs'");
            Main.NewText("  lists are empty, vanilla names will be used for those NPCs and those only.");
            Main.NewText("  Such NPCs will be skipped altogether when randomizing names manually though.");
            Main.NewText("7. The Copy, Paste and Carry Buttons", new Color(50, 125, 190));
            Main.NewText("  The Copy and Paste buttons in the bottom right corner are a great tool");
            Main.NewText("  for transfering your data between different worlds. They save information");
            Main.NewText("  about all name lists, genders, the current Mode and the Unique setting.");
            Main.NewText("  With Data Carriage on, the same information will be automatically transferred");
            Main.NewText("  from the last opened world to a freshly generated one.");
            Main.NewText("  The Copy, Paste and Carry buttons won't retain data across separate tModLoader");
            Main.NewText("  sessions.");
            Main.NewText(" ");
            Main.NewText("(open the chat and use arrow keys to read)", new Color(0, 255, 0));
        }

        private void AddButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            SetRemoveMode(false);

            string newName = "";
            var newWrapper = new StringWrapper(ref newName);

            var field = new UINameField(newWrapper, (uint)panelList.Count);
            field.IsNew = true;
            panelList.Add(field);
            DeselectAllEntries();
            panelListScrollbar.ViewPosition = 0;
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
                    CustomWorld.CustomNames[SelectedNPC].Remove(i.NameWrapper);
                    panelList.RemoveName(i);
                }
                CustomWorld.SyncWorldData();
            }
        }

        private void SwitchGenderButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            NPCs.CustomNPC.isMale[SelectedNPC] = !NPCs.CustomNPC.isMale[SelectedNPC];
            npcPreview.UpdateNPC(SelectedNPC);
            CustomWorld.SyncWorldData();
        }

        private void RandomizeButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            if (SelectedNPC != 1000 && SelectedNPC != 1001 && SelectedNPC != 1002) {
                string oldName = NPCs.CustomNPC.currentNames[SelectedNPC];

                NPCs.CustomNPC.RandomizeName(SelectedNPC);

                if (oldName != NPCs.CustomNPC.currentNames[SelectedNPC]) {
                    CustomWorld.SyncWorldData();
                }
            } else if (SelectedNPC == 1000) {
                var old = new Dictionary<short, string>();
                foreach (short i in CustomNPCNames.TownNPCs) {
                    if (NPCs.CustomNPC.isMale[i]) { old.Add(i, NPCs.CustomNPC.currentNames[i]); }
                }
                NPCs.CustomNPC.RandomizeName(SelectedNPC);
                foreach (short i in CustomNPCNames.TownNPCs) {
                    if (NPCs.CustomNPC.isMale[i] && old[i] != NPCs.CustomNPC.currentNames[i]) { CustomWorld.SyncWorldData(); break; }
                }
            } else if (SelectedNPC == 1001) {
                var old = new Dictionary<short, string>();
                foreach (short i in CustomNPCNames.TownNPCs) {
                    if (!NPCs.CustomNPC.isMale[i]) { old.Add(i, NPCs.CustomNPC.currentNames[i]); }
                }
                NPCs.CustomNPC.RandomizeName(SelectedNPC);
                foreach (short i in CustomNPCNames.TownNPCs) {
                    if (!NPCs.CustomNPC.isMale[i] && old[i] != NPCs.CustomNPC.currentNames[i]) { CustomWorld.SyncWorldData(); break; }
                }
            } else if (SelectedNPC == 1002) {
                var old = new Dictionary<short, string>(NPCs.CustomNPC.currentNames);
                NPCs.CustomNPC.RandomizeName(SelectedNPC);
                foreach (short i in CustomNPCNames.TownNPCs) {
                    if (old[i] != NPCs.CustomNPC.currentNames[i]) { CustomWorld.SyncWorldData(); break; }
                }
            }
        }

        public void DeselectAllEntries()
        {
            panelList.DeselectAll();
            renamePanel.Deselect();
        }

        private void CopyButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            copyData.Copy();
            menuPanel.RemoveChild(pasteButtonInactive);
            menuPanel.Append(pasteButton);
            if (Keyboard.GetState().IsKeyDown(Keys.LeftAlt) || Keyboard.GetState().IsKeyDown(Keys.RightAlt)) {
                modeCycleButton.State = 0;
                uniqueNameButton.State = true;
                CustomWorld.ResetCustomNames();
                NPCs.CustomNPC.ResetCurrentGender();
                UINPCButton.Refresh();
            }
        }

        private void PasteButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            copyData.Paste();
            UINPCButton.Refresh();
        }

        private void CarryButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            if (carry) {
                carryButton.SetImage(ModContent.GetTexture("CustomNPCNames/UI/carry_button_off"));
                carry = false;
            } else {
                carryButton.SetImage(ModContent.GetTexture("CustomNPCNames/UI/carry_button_on"));
                carry = true;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Select the last world-saved NPC selection, if exists
            if (savedSelectedNPC != -1) {
                if (savedSelectedNPC == 0) {
                    UINPCButton.Deselect();
                    panelList.Clear();
                } else {
                    foreach (UINPCButton i in menuNPCList) {
                        if (i.npcId == savedSelectedNPC) {
                            var evt = new UIMouseEvent(i, new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
                            i.Click(evt);
                            savedSelectedNPC = 0;
                            break;
                        }
                    }
                }

                // Most of this is probably redundant, but it could help in case the game didn't close properly (save was skipped)
                removeMode = false;
                modeCycleButton.State = CustomWorld.mode;
                uniqueNameButton.State = CustomWorld.tryUnique;
                DeselectAllEntries();
                savedSelectedNPC = -1;
            }

            UpdateUIStates();

            if (removeMode) {
                var mouse = Mouse.GetState();
                trashIcon.Left.Set((mouse.X + 15) / Main.UIScale, 0);
                trashIcon.Top.Set((mouse.Y + 15) / Main.UIScale, 0);
            }
        }

        // These two Draw overrides help eliminate a one frame jump between unloaded and loaded save data when opening the menu for the first time in a world
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (savedSelectedNPC == -1) {
                base.Draw(spriteBatch);
            }
        }
        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            if (savedSelectedNPC == -1) {
                base.DrawChildren(spriteBatch);
            }
        }

        public void UpdateUIStates()
        {
            if (IsNPCSelected) {
                short id = SelectedNPC;
                bool noNames = (CustomWorld.mode == 1 && id != 1000 && id != 1001 && id != 1002 && CustomWorld.CustomNames[id].Count == 0)
                            || (CustomWorld.mode == 2 && (id != 1002 ? CustomWorld.CustomNames[(short)(id == 1000 || id == 1001 ? id : (NPCs.CustomNPC.isMale[id] ? 1000 : 1001))].Count == 0 : CustomWorld.CustomNames[1000].Count == 0 && CustomWorld.CustomNames[1001].Count == 0))
                            || (CustomWorld.mode == 3 && CustomWorld.CustomNames[1002].Count == 0);

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
                    if (id == 1000) {
                        bool noMaleNPCs = true;
                        foreach (short i in CustomNPCNames.TownNPCs) {
                            if (NPCs.CustomNPC.isMale[i] && NPC.CountNPCS(i) != 0) { noMaleNPCs = false; break; }
                        }

                        if (CustomWorld.mode == 1) {
                            noNames = true;
                            foreach (short i in CustomNPCNames.TownNPCs) {
                                if (NPCs.CustomNPC.isMale[i] && CustomWorld.CustomNames[i].Count != 0) { noNames = false; break; }
                            }
                        }

                        if (noMaleNPCs) {
                            namesPanel.RemoveChild(randomizeButton);
                            namesPanel.Append(randomizeButtonInactive);
                            randomizeButtonInactive.HoverText = "No male NPCs\nare alive right now!";
                        } else if (noNames) {
                            namesPanel.RemoveChild(randomizeButton);
                            namesPanel.Append(randomizeButtonInactive);
                            randomizeButtonInactive.HoverText = "There are no names\non the list to choose from!";
                        } else {
                            namesPanel.RemoveChild(randomizeButtonInactive);
                            namesPanel.Append(randomizeButton);
                            randomizeButton.HoverText = "Randomize Male Names";
                        }
                    } else if (id == 1001) {
                        bool noFemaleNPCs = true;
                        foreach (short i in CustomNPCNames.TownNPCs) {
                            if (!NPCs.CustomNPC.isMale[i] && NPC.CountNPCS(i) != 0) { noFemaleNPCs = false; break; }
                        }

                        if (CustomWorld.mode == 1) {
                            noNames = true;
                            foreach (short i in CustomNPCNames.TownNPCs) {
                                if (!NPCs.CustomNPC.isMale[i] && CustomWorld.CustomNames[i].Count != 0) { noNames = false; break; }
                            }
                        }

                        if (noFemaleNPCs) {
                            namesPanel.RemoveChild(randomizeButton);
                            namesPanel.Append(randomizeButtonInactive);
                            randomizeButtonInactive.HoverText = "No female NPCs\nare alive right now!";
                        } else if (noNames) {
                            namesPanel.RemoveChild(randomizeButton);
                            namesPanel.Append(randomizeButtonInactive);
                            randomizeButtonInactive.HoverText = "There are no names\non the list to choose from!";
                        } else {
                            namesPanel.RemoveChild(randomizeButtonInactive);
                            namesPanel.Append(randomizeButton);
                            randomizeButton.HoverText = "Randomize Female Names";
                        }
                    } else if (id == 1002) {
                        bool noNPCs = true;
                        foreach (short i in CustomNPCNames.TownNPCs) {
                            if (NPC.CountNPCS(i) != 0) { noNPCs = false; break; }
                        }

                        if (CustomWorld.mode == 1) {
                            noNames = true;
                            foreach (short i in CustomNPCNames.TownNPCs) {
                                if (CustomWorld.CustomNames[i].Count != 0) { noNames = false; break; }
                            }
                        }

                        if (noNPCs) {
                            namesPanel.RemoveChild(randomizeButton);
                            namesPanel.Append(randomizeButtonInactive);
                            randomizeButtonInactive.HoverText = "No NPCs are\nalive right now!";
                        } else if (noNames) {
                            namesPanel.RemoveChild(randomizeButton);
                            namesPanel.Append(randomizeButtonInactive);
                            randomizeButtonInactive.HoverText = "There are no names\non the list to choose from!";
                        } else {
                            namesPanel.RemoveChild(randomizeButtonInactive);
                            namesPanel.Append(randomizeButton);
                            randomizeButton.HoverText = "Randomize All";
                        }
                    }
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
                            randomizeButton.HoverText = "Randomize Name";
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

                namesPanel.RemoveChild(npcPreview);
                namesPanel.RemoveChild(switchGenderButton);
                namesPanel.Append(switchGenderButtonInactive);

                namesPanel.RemoveChild(randomizeButton);
                namesPanel.Append(randomizeButtonInactive);
                randomizeButtonInactive.HoverText = "No NPC Selected";
            }

            var k = Keyboard.GetState();
            if (!k.IsKeyDown(Keys.LeftAlt) && !k.IsKeyDown(Keys.RightAlt)) {
                copyButton.SetImage(ModContent.GetTexture("CustomNPCNames/UI/copy_button"));
                copyButton.HoverText = "Copy Everything\n(Hold Alt to Cut)";
            } else {
                copyButton.SetImage(ModContent.GetTexture("CustomNPCNames/UI/cut_button"));
                copyButton.HoverText = "Cut Everything";
            }
        }

        public static Texture2D GetNPCHeadTexture(short id)
        {
            int textureId = 0;
            switch (id) {
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

    /// <summary>
    /// This struct is a container for all lists data, it is used to make copying, pasting and carrying data between worlds easier.
    /// </summary>
    internal struct ListData
    {
        public byte mode;
        public bool tryUnique;
        public Dictionary<short, List<string>> customNames;
        public Dictionary<short, bool> isMale;

        public void Copy()
        {
            mode = CustomWorld.mode;
            tryUnique = CustomWorld.tryUnique;
            customNames = new Dictionary<short, List<string>>();
            foreach (KeyValuePair<short, List<StringWrapper>> i in CustomWorld.CustomNames) {
                var list = new List<string>();
                foreach (StringWrapper j in i.Value) {
                    list.Add(j.ToString());
                }
                customNames.Add(i.Key, list);
            }
            isMale = new Dictionary<short, bool>();
            foreach (KeyValuePair<short, bool> i in NPCs.CustomNPC.isMale) {
                isMale.Add(i.Key, i.Value);
            }
        }

        public void Paste()
        {
            RenameUI.modeCycleButton.State = mode;
            RenameUI.uniqueNameButton.State = tryUnique;
            if (customNames != null) {
                foreach (KeyValuePair<short, List<string>> i in customNames) {
                    CustomWorld.CustomNames[i.Key] = StringWrapper.ConvertList(i.Value);
                }
            }
            if (isMale != null) {
                foreach (KeyValuePair<short, bool> i in isMale) {
                    NPCs.CustomNPC.isMale[i.Key] = i.Value;
                }
            }
            CustomWorld.SyncWorldData();
        }
    }
}