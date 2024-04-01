/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/custom-farm-loader
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using Custom_Farm_Loader.Lib;
using StardewValley.BellsAndWhistles;
using StardewValley.GameData;
using StardewModdingAPI;

namespace Custom_Farm_Loader.Menus
{
    public class CustomFarmSelection : IClickableMenu
    {
        private static Texture2D MissingMapIcon;
        private static Texture2D MissingMapPreview;
        private static Texture2D CurrentFarmPreview;

        private static List<ModFarmType> ModFarms = new List<ModFarmType>();

        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        private static bool IncludeVanilla = false;
        private static bool ChangeWhichFarm = true;

        public static void Initialize(Mod mod)
        {
            Mod = mod;
            Monitor = mod.Monitor;
            Helper = mod.Helper;

            MissingMapIcon = ModEntry._Helper.ModContent.Load<Texture2D>("assets/MissingMapIcon.png");
            MissingMapPreview = ModEntry._Helper.ModContent.Load<Texture2D>("assets/MissingMapPreview.png");
        }

        public const int Region_upArrow = 97865;
        public const int Region_downArrow = 97866;
        private Rectangle ScrollBarRunner;

        public ClickableTextureComponent UpArrow;
        public ClickableTextureComponent DownArrow;
        public ClickableTextureComponent ScrollBar;

        public const int ItemsPerPage = 4;
        public int DescriptionRowsPerPage = 3;
        private bool Scrolling;

        public List<ClickableComponent> CustomFarmButtons = new List<ClickableComponent>();
        private List<CustomFarm> CustomFarms = new List<CustomFarm>();
        public CustomFarm CurrentCustomFarm = null;

        public int currentItemIndex;

        protected int currentTabHover;


        protected int DescriptionScrollIndex = 0;
        protected string[] SplitPreviewDescription = new string[] { };
        public ClickableTextureComponent DescriptionUpArrow;
        public ClickableTextureComponent DescriptionDownArrow;
        public CustomFarmSelection(bool includeVanilla = false, bool changeWhichFarm = true) : base(Game1.uiViewport.Width / 2 - 320, Game1.uiViewport.Height - 64 - 192, 640, 192)
        {
            IncludeVanilla = includeVanilla || ModEntry.Config.IncludeVanilla;
            ChangeWhichFarm = changeWhichFarm;

            updatePosition();
            initializeUpperRightCloseButton();

            createClickTableTextures();
            applyTab();
            updateCustomFarmButtonNeighbors();

            populateClickableComponentList();

            if (CustomFarms.Count() > 0) {
                currentlySnappedComponent = CustomFarmButtons[0];
                if (Game1.options.SnappyMenus)
                    snapCursorToCurrentSnappedComponent();
            }

            calculateDescriptionRowsPerPage();
        }

        public void calculateDescriptionRowsPerPage()
        {
            DescriptionRowsPerPage = Helper.GameContent.CurrentLocaleConstant switch {
                LocalizedContentManager.LanguageCode.de => 5,
                LocalizedContentManager.LanguageCode.en => 4,
                LocalizedContentManager.LanguageCode.es => 3,
                LocalizedContentManager.LanguageCode.fr => 3,
                LocalizedContentManager.LanguageCode.hu => 3,
                LocalizedContentManager.LanguageCode.it => 4,
                LocalizedContentManager.LanguageCode.ja => 5,
                LocalizedContentManager.LanguageCode.ko => 3,
                LocalizedContentManager.LanguageCode.pt => 3,
                LocalizedContentManager.LanguageCode.ru => 5,
                LocalizedContentManager.LanguageCode.tr => 3,
                LocalizedContentManager.LanguageCode.zh => 4,
                _ => 3
            };
        }

        public void updateCustomFarmButtonNeighbors()
        {
            ClickableComponent last_valid_button = CustomFarmButtons[0];
            for (int i = 0; i < CustomFarmButtons.Count; i++) {
                ClickableComponent button = CustomFarmButtons[i];
                button.upNeighborImmutable = true;
                button.downNeighborImmutable = true;
                button.upNeighborID = ((i > 0) ? (i + 3546 - 1) : (-7777));
                button.downNeighborID = ((i < 3 && i < CustomFarms.Count - 1) ? (i + 3546 + 1) : (-7777));
                if (i >= CustomFarms.Count) {
                    if (button == currentlySnappedComponent) {
                        currentlySnappedComponent = last_valid_button;
                        if (Game1.options.SnappyMenus)
                            snapCursorToCurrentSnappedComponent();

                    }
                } else {
                    last_valid_button = button;
                }
            }
        }

        private void createClickTableTextures()
        {
            UpArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + 16, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f) {
                myID = 97865,
                downNeighborID = 106,
                leftNeighborID = 3546
            };
            DownArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + height - 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f) {
                myID = 106,
                upNeighborID = 97865,
                leftNeighborID = 3546,
            };
            ScrollBar = new ClickableTextureComponent(new Rectangle(UpArrow.bounds.X + 12, UpArrow.bounds.Y + UpArrow.bounds.Height + 4, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
            ScrollBarRunner = new Rectangle(ScrollBar.bounds.X, UpArrow.bounds.Y + UpArrow.bounds.Height + 4, ScrollBar.bounds.Width, height - 64 - UpArrow.bounds.Height - 28);

            for (int i = 0; i < ItemsPerPage; i++) {
                CustomFarmButtons.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + 16, yPositionOnScreen + 16 + i * ((height - 256) / 4), width - 32, (height - 256) / 4 + 4), string.Concat(i)) {
                    myID = i + 3546,
                    rightNeighborID = 97865,
                    fullyImmutable = true
                });
            }

            DescriptionUpArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 36, yPositionOnScreen + height - 200, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f) {
                myID = 801,
                downNeighborID = 802,
                upNeighborID = 3546,
                rightNeighborID = 106
            };
            DescriptionDownArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 36, yPositionOnScreen + height - 42, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f) {
                myID = 802,
                upNeighborID = 801,
                rightNeighborID = 106
            };
        }

        public void applyTab()
        {
            Game1.playSound("shwip");
            this.CustomFarms = CustomFarm.getAll();
            CustomFarm.getAll().ForEach(farm => farm.reloadTextures());
            loadVanillaFarms();
            loadModFarms();

            if (!_CharacterCustomizationMenu.isVanillaFarmSelected() || IncludeVanilla)
                CurrentCustomFarm = CustomFarms.Find(e => e.ID == Game1.GetFarmTypeID());
            DescriptionScrollIndex = 0;
            SplitPreviewDescription = new string[] { };
            assignCurrentFarmPreview();

            currentItemIndex = 0;
            setScrollBarToCurrentIndex();
            updateCustomFarmButtonNeighbors();
        }

        private void loadVanillaFarms()
        {
            CustomFarms.RemoveAll(el => new List<string>() { "0", "1", "2", "3", "4", "5", "6", "MeadowlandsFarm" }.Contains(el.ID));

            if (!IncludeVanilla)
                return;

            //v[0] = CustomFarm.VanillaTypes, v[1] = Label key
            var v = new string[][] {
                new string[] { "Standard", "FarmStandard" },
                new string[] { "Riverland", "FarmFishing" },
                new string[] { "Forest", "FarmForaging" },
                new string[] { "Hills", "FarmMining" },
                new string[] { "Wilderness", "FarmCombat" },
                new string[] { "Four Corners", "FarmFourCorners" },
                new string[] { "Beach", "FarmBeach" },
            };

            for (int i = 0; i < v.Length; i++) {
                var label = Game1.content.LoadString($"Strings\\UI:Character_{v[i][1]}");

                var f = new CustomFarm(true, v[i][0], v[i][0]) {
                    ID = i + "",
                    Name = label.Split("_").First(),
                    Author = "ConcernedApe",
                    Description = label.Substring(label.IndexOf('_') + 1),
                };

                if (i == 4)
                    f.Properties.SpawnMonstersAtNight = true;

                f.Icon = f.loadIconTexture();
                f.WorldMapOverlay = f.loadWorldMapTexture();

                CustomFarms.Add(f);
            }
        }

        private void loadModFarms()
        {
            ModFarms = new List<ModFarmType>();
            List<ModFarmType> modFarms = Game1.content.Load<List<ModFarmType>>("Data\\AdditionalFarms");

            foreach (ModFarmType farm in modFarms) {
                if (CustomFarms.Exists(el => el.ID == farm.Id))
                    continue;
                if (farm.Id == "MeadowlandsFarm" && !IncludeVanilla)
                    continue;

                ModFarms.Add(farm);
                CustomFarm newCustomFarm = new CustomFarm(farm);

                newCustomFarm.ID = farm.Id;
                newCustomFarm.Author = findMapAuthor(farm);
                if (farm.MapName != "")
                    newCustomFarm.Name = farm.MapName.Replace("_", " ");

                if (farm.TooltipStringPath != "")
                    try { newCustomFarm.Description = Game1.content.LoadString(farm.TooltipStringPath); } catch (Exception) {
                        Monitor.LogOnce($"Unable to load tooltip asset '{farm.TooltipStringPath}' for {farm.Id}; Resorting to default", LogLevel.Warn);
                    }
                if (farm.IconTexture != "") {
                    try {
                        newCustomFarm.Icon = loadCroppedIcon(farm);
                    } catch (Exception) {
                        Monitor.LogOnce($"Unable to load farm icon asset '{farm.IconTexture}' for {farm.Id}; Resorting to default", LogLevel.Warn);
                    }
                }

                if (farm.WorldMapTexture != null) {
                    try {
                        newCustomFarm.WorldMapOverlay = loadKnownWorldMapExceptions(farm);
                        if (newCustomFarm.WorldMapOverlay == null)
                            newCustomFarm.WorldMapOverlay = Helper.GameContent.Load<Texture2D>(farm.WorldMapTexture);
                    } catch (Exception) {
                        Monitor.LogOnce($"Unable to load world map asset '{farm.WorldMapTexture}' for {farm.Id}; Resorting to default", LogLevel.Warn);
                    }

                }

                if (farm.Id.Contains("."))
                    newCustomFarm.Author = farm.Id.Split(".").First();

                applyModdedVanillaFarm(farm, newCustomFarm);

                CustomFarms.Add(newCustomFarm);
            }

            CustomFarms = CustomFarms.OrderBy(o => o.Name).ToList();
        }

        private static void applyModdedVanillaFarm(ModFarmType farm, CustomFarm newCustomFarm)
        {
            if (farm.Id != "MeadowlandsFarm")
                return;
            var label = newCustomFarm.Description = Game1.content.LoadString(farm.TooltipStringPath);

            newCustomFarm.Name = label.Split("_").First();
            newCustomFarm.Author = "ConcernedApe";
        }

        private string findMapAuthor(ModFarmType modFarm)
        {
            List<Tuple<string, int>> relatedModRating = new List<Tuple<string, int>>();

            foreach (var mod in Helper.ModRegistry.GetAll().ToList()) {
                var manifest = mod.Manifest;
                if (manifest.UniqueID == modFarm.Id)
                    return manifest.Author;
                else if (manifest.UniqueID.Contains(modFarm.Id))
                    relatedModRating.Add(Tuple.Create(manifest.Author, 3));
                else if (manifest.Name == modFarm.Id)
                    relatedModRating.Add(Tuple.Create(manifest.Author, 2));
                else if (manifest.Name.Contains(modFarm.Id))
                    relatedModRating.Add(Tuple.Create(manifest.Author, 1));
            }

            if (relatedModRating.Count == 0)
                return "Unknown";

            var mostLikelyMod = relatedModRating.OrderBy(el => el.Item2).Last();
            return mostLikelyMod.Item1;
        }

        //Some custom farm maps have transparent pixels on the side to imitate vanilla behaviour
        //This trims them away
        public static Texture2D loadCroppedIcon(ModFarmType modFarm)
            => cropIcon(Helper.GameContent.Load<Texture2D>(modFarm.IconTexture));

        public static Texture2D cropIcon(Texture2D icon)
        {
            int count = icon.Bounds.Width * icon.Bounds.Height;
            Color[] data = new Color[count];
            icon.GetData(0, icon.Bounds, data, 0, count);

            //Getting the leftmost and rightmost column index of the first visible pixel
            int left = icon.Bounds.Width;
            int right = 0;
            for (int i = 0; i < count; i++) {
                if (data[i].A != 0) {
                    if (i % icon.Bounds.Width < left)
                        left = i % icon.Bounds.Width;

                    if (i % icon.Bounds.Width > right)
                        right = i % icon.Bounds.Width;
                }
            }

            if (left == 0 && right == icon.Bounds.Width - 1)
                return icon;
            else if (left == icon.Bounds.Width - 1)
                return icon;
            else
                return UtilityMisc.createSubTexture(icon, new Rectangle(left, 0, right - left + 1, icon.Bounds.Height));
        }


        //Some custom farm maps have complicated logic where they want to display the world map depending on season and whether SVE is installed
        //This is a hard coded way for CFL to still be able to display them properly during farm selection
        //despite not really having access to CP logic
        private Texture2D loadKnownWorldMapExceptions(ModFarmType modFarm)
        {
            Dictionary<string, string[]> knownWorldMapExceptions
             = new Dictionary<string, string[]> {
                 { "A_TK.FarmProjectForaging/WaFF", new[] { "A_TK.FarmProjectForaging", "../[CP] - Waterfall Forest - Xtra Content/assets/world_map/_default_SV/all_WaFF.png" } },
                 { "A_TK.FarmProjectForaging/WaFFLE", new[] { "A_TK.FarmProjectForaging", "../[CP] - Waterfall Forest - Xtra Content/assets/world_map/_default_SV/all_WaFFLE.png" } }
             };

            if (!knownWorldMapExceptions.ContainsKey(modFarm.Id))
                return null;

            var map = knownWorldMapExceptions[modFarm.Id];
            var path = UtilityMisc.getRelativeModDirectory(map[0]);

            Monitor.LogOnce($"Found '{modFarm.Id}' as part of known world map exceptions. Attempting hard coded load in '{path}\\{map[1]}'");

            try {
                return Helper.ModContent.Load<Texture2D>($"{path}\\{map[1]}");
            } catch (Exception ex) {
                Monitor.LogOnce($"Unable to load hard coded world map asset for '{modFarm.Id}'");
            }

            return null;
        }

        private void assignCurrentFarmPreview()
        {
            CurrentFarmPreview = null;

            if (CurrentCustomFarm == null || CurrentCustomFarm.Preview == "")
                return;

            try {
                CurrentFarmPreview = ModEntry._Helper.ModContent.Load<Texture2D>(CurrentCustomFarm.Preview);
            } catch (Exception ex) {
                ModEntry._Monitor.LogOnce($"Unable to load the map preview in:\n{CurrentCustomFarm.Preview}", LogLevel.Warn);
            }


        }

        public void updatePosition()
        {
            width = 1000 + IClickableMenu.borderWidth * 2;
            height = 600 + IClickableMenu.borderWidth * 2;
            xPositionOnScreen = Game1.uiViewport.Width / 2 - (1000 + IClickableMenu.borderWidth * 2) / 2;
            yPositionOnScreen = Game1.uiViewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2;
        }

        public override void snapToDefaultClickableComponent()
        {
            return;
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            updatePosition();
            initializeUpperRightCloseButton();

            CustomFarmButtons.Clear();
            createClickTableTextures();
            setScrollBarToCurrentIndex();
        }

        private void downArrowPressed()
        {
            DownArrow.scale = DownArrow.baseScale;
            currentItemIndex++;
            setScrollBarToCurrentIndex();
            updateCustomFarmButtonNeighbors();
        }

        private void upArrowPressed()
        {
            if (currentItemIndex == 0)
                return;

            UpArrow.scale = UpArrow.baseScale;
            currentItemIndex--;
            setScrollBarToCurrentIndex();
            updateCustomFarmButtonNeighbors();
        }

        private void setScrollBarToCurrentIndex()
        {
            if (CustomFarms.Count > 0) {
                ScrollBar.bounds.Y = (int)((float)(ScrollBarRunner.Height - ScrollBar.bounds.Height) / Math.Max(1, CustomFarms.Count - 4) * currentItemIndex + UpArrow.bounds.Bottom + 4);
                if (currentItemIndex == CustomFarms.Count - 4) {
                    ScrollBar.bounds.Y = DownArrow.bounds.Y - ScrollBar.bounds.Height - 4;
                }
            }
        }

        public void selectListElement(CustomFarm customFarm)
        {
            Game1.playSound("coin");
            CurrentCustomFarm = customFarm;
            DescriptionScrollIndex = 0;
            SplitPreviewDescription = new string[] { };

            if (ChangeWhichFarm)
                setWhichFarm(customFarm);

            Game1.spawnMonstersAtNight = customFarm.Properties.SpawnMonstersAtNight;
            assignCurrentFarmPreview();
        }

        public static void setWhichFarm(CustomFarm customFarm)
        {
            if (customFarm.IsVanillaMap) {
                Game1.whichFarm = int.Parse(customFarm.ID);
            } else {
                Game1.whichFarm = 7;
                if (ModFarms.Exists(e => e.Id == customFarm.ID))
                    Game1.whichModFarm = ModFarms.Find(e => e.Id == customFarm.ID);
                else
                    Game1.whichModFarm = customFarm.asModFarmType();
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            exitThisMenu();
            base.receiveRightClick(x, y, playSound);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y);

            if (DownArrow.containsPoint(x, y) && isScrollbarDrawn() && currentItemIndex < Math.Max(0, CustomFarms.Count - 4)) {
                downArrowPressed();
                Game1.playSound("shwip");
            } else if (UpArrow.containsPoint(x, y) && isScrollbarDrawn()) {
                upArrowPressed();
                Game1.playSound("shwip");
            } else if (ScrollBar.containsPoint(x, y) && isScrollbarDrawn()) {
                Scrolling = true;
            } else if (base.upperRightCloseButton.containsPoint(x, y)) {
                exitThisMenu();
                return;
            } else if (!DownArrow.containsPoint(x, y) && isScrollbarDrawn() && x > xPositionOnScreen + width && x < xPositionOnScreen + width + 128 && y > yPositionOnScreen && y < yPositionOnScreen + height) {
                Scrolling = true;
                leftClickHeld(x, y);
                releaseLeftClick(x, y);
            } else if (DescriptionUpArrow.containsPoint(x, y) && DescriptionScrollIndex != 0) {
                DescriptionScrollIndex--;
                Game1.playSound("shwip");
            } else if (DescriptionDownArrow.containsPoint(x, y) && SplitPreviewDescription.Length > DescriptionScrollIndex + DescriptionRowsPerPage) {
                DescriptionScrollIndex++;
                Game1.playSound("shwip");
            }


            for (int i = 0; i < CustomFarmButtons.Count; i++) {
                if (currentItemIndex + i >= CustomFarms.Count || !CustomFarmButtons[i].containsPoint(x, y))
                    continue;

                int index = currentItemIndex + i;

                selectListElement(CustomFarms[index]);

                updateCustomFarmButtonNeighbors();
                setScrollBarToCurrentIndex();
                return;
            }
        }

        public override void leftClickHeld(int x, int y)
        {
            if (Scrolling) {
                int y2 = ScrollBar.bounds.Y;
                ScrollBar.bounds.Y = Math.Min(yPositionOnScreen + height - 64 - 12 - ScrollBar.bounds.Height, Math.Max(y, yPositionOnScreen + UpArrow.bounds.Height + 20));
                float percentage = (float)(y - ScrollBarRunner.Y) / (float)ScrollBarRunner.Height;
                currentItemIndex = Math.Min(CustomFarms.Count - 4, Math.Max(0, (int)Math.Round((double)((CustomFarms.Count - 4) * percentage))));
                setScrollBarToCurrentIndex();
                updateCustomFarmButtonNeighbors();
                if (y2 != ScrollBar.bounds.Y) {
                    Game1.playSound("shiny4");
                }
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            base.releaseLeftClick(x, y);
            Scrolling = false;
        }

        public override void receiveScrollWheelAction(int direction)
        {
            if (!isScrollbarDrawn())
                return;

            if (direction > 0 && currentItemIndex > 0) {
                upArrowPressed();
                Game1.playSound("shiny4");
            } else if (direction < 0 && currentItemIndex < Math.Max(0, CustomFarms.Count - 4)) {
                downArrowPressed();
                Game1.playSound("shiny4");
            }
        }

        public override void receiveGamePadButton(Buttons b)
        {
            if ((b == Buttons.LeftShoulder || b == Buttons.LeftTrigger) && DescriptionScrollIndex != 0) {
                DescriptionScrollIndex--;
                Game1.playSound("shwip");
            } else if ((b == Buttons.RightShoulder || b == Buttons.RightTrigger) && SplitPreviewDescription.Length > DescriptionScrollIndex + DescriptionRowsPerPage) {
                DescriptionScrollIndex++;
                Game1.playSound("shwip");
            }

            base.receiveGamePadButton(b);
        }

        public override void performHoverAction(int x, int y)
        {
            this.UpArrow.tryHover(x, y, 0.5f);
            this.DownArrow.tryHover(x, y, 0.5f);
            this.DescriptionUpArrow.tryHover(x, y, 0.5f);
            this.DescriptionDownArrow.tryHover(x, y, 0.5f);
            this.upperRightCloseButton.tryHover(x, y, 0.5f);
        }

        protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
        {
            if (!isScrollbarDrawn())
                return;

            switch (direction) {
                case 2: {
                        if (currentItemIndex < Math.Max(0, CustomFarms.Count - 4)) {
                            downArrowPressed();

                            currentlySnappedComponent = getComponentWithID(0);
                            snapCursorToCurrentSnappedComponent();
                        }
                        break;
                    }
                case 0:
                    if (currentItemIndex > 0) {
                        upArrowPressed();

                        currentlySnappedComponent = getComponentWithID(3546);
                        snapCursorToCurrentSnappedComponent();
                    }
                    break;
            }
        }

        public override bool readyToClose()
        {
            GamePadState currentPadState = Game1.input.GetGamePadState();

            if (((currentPadState.IsButtonDown(Buttons.Start) && !Game1.oldPadState.IsButtonDown(Buttons.Start)) || (currentPadState.IsButtonDown(Buttons.B) && !Game1.oldPadState.IsButtonDown(Buttons.B))))
                exitThisMenu();

            return false;
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.staminaRect, new Rectangle(0, 0, Game1.graphics.GraphicsDevice.Viewport.Width, Game1.graphics.GraphicsDevice.Viewport.Height), Color.Black * 0.75f);

            //Top/Bottom Window w Borders
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), xPositionOnScreen, yPositionOnScreen, width, height - 256 + 32 + 4, Color.White, 4f);
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), xPositionOnScreen, yPositionOnScreen + height - 256 + 40, width, height - 448, Color.White, 4f);

            draw_custom_farm_buttons(b);
            draw_farm_preview(b);

            if (isScrollbarDrawn()) {
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), ScrollBarRunner.X, ScrollBarRunner.Y, ScrollBarRunner.Width, ScrollBarRunner.Height, Color.White, 4f);
                ScrollBar.draw(b);
                UpArrow.draw(b);
                DownArrow.draw(b);
            }

            base.draw(b);
            drawMouse(b);
        }

        private void draw_custom_farm_buttons(SpriteBatch b)
        {
            for (int k = 0; k < CustomFarmButtons.Count; k++) {
                if (currentItemIndex + k >= CustomFarms.Count)
                    continue;

                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), CustomFarmButtons[k].bounds.X, CustomFarmButtons[k].bounds.Y, CustomFarmButtons[k].bounds.Width, CustomFarmButtons[k].bounds.Height, (CustomFarmButtons[k].containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) && !Scrolling) ? Color.Wheat : Color.White, 4f, drawShadow: false);
                CustomFarm item = CustomFarms[currentItemIndex + k];

                //Icons
                Texture2D iconTexture = item.Icon != null ? item.Icon : MissingMapIcon;
                b.Draw(iconTexture, new Rectangle(CustomFarmButtons[k].bounds.X + 16, CustomFarmButtons[k].bounds.Y + 16, 72, 80), Color.White);

                //display Name
                string displayName = item.MaxPlayers > 0 ? $"{item.Name} ({item.MaxPlayers}P)" : item.Name;
                SpriteText.drawString(b, displayName, CustomFarmButtons[k].bounds.X + 96 + 8, CustomFarmButtons[k].bounds.Y + 28, 999999, -1, 999999, 1f, 0.88f, junimoText: false);

                if (item.ID == CurrentCustomFarm?.ID && item.Name == CurrentCustomFarm?.Name)
                    //Red farm selection rectangle
                    IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(375, 357, 3, 3), CustomFarmButtons[k].bounds.X + 4, CustomFarmButtons[k].bounds.Y + 4, CustomFarmButtons[k].bounds.Width - 8, CustomFarmButtons[k].bounds.Height - 8, Color.White, 4f, drawShadow: false);

            }

        }

        private void draw_farm_preview(SpriteBatch b)
        {
            if (CurrentCustomFarm == null)
                return;

            Texture2D previewTexture = CurrentFarmPreview == null ? CurrentCustomFarm.WorldMapOverlay : CurrentFarmPreview;
            if (previewTexture == null)
                previewTexture = MissingMapPreview;
            Rectangle previewRectangle = new Rectangle(xPositionOnScreen + 16, yPositionOnScreen + height - 216 + 18, (int)Math.Round(width * 0.4), height - 448 - 32);
            b.Draw(previewTexture, previewRectangle, Color.White);


            string displayName = CurrentCustomFarm.MaxPlayers > 0 ? $"{CurrentCustomFarm.Name} ({CurrentCustomFarm.MaxPlayers}P)" : CurrentCustomFarm.Name;
            string title = $"{displayName} by {CurrentCustomFarm.Author}";
            SpriteText.drawStringWithScrollBackground(b, title, previewRectangle.X + 16, yPositionOnScreen + height - 250);

            drawDescription(b, CurrentCustomFarm.getLocalizedDescription(),
                x: previewRectangle.X + previewRectangle.Width + 16,
                y: yPositionOnScreen + height - 216 + 30,
                width: width - previewRectangle.Width - 40);

        }

        public void drawDescription(SpriteBatch b, string description, int x, int y, int width)
        {
            if (SplitPreviewDescription.Length == 0 && description != "")
                SplitPreviewDescription = Game1.parseText(description, Game1.dialogueFont, width - 4).Split(Environment.NewLine);

            string descriptionString = "";
            for (int i = DescriptionScrollIndex; i < DescriptionScrollIndex + DescriptionRowsPerPage; i++) {
                if (SplitPreviewDescription.Length > i)
                    descriptionString += SplitPreviewDescription[i] + Environment.NewLine;
            }
            Utility.drawTextWithShadow(b, descriptionString, Game1.dialogueFont, new Vector2(x, y), Game1.textColor);

            if (DescriptionScrollIndex != 0)
                DescriptionUpArrow.draw(b);

            if (SplitPreviewDescription.Length > DescriptionScrollIndex + DescriptionRowsPerPage)
                DescriptionDownArrow.draw(b);
        }

        public bool isScrollbarDrawn() => CustomFarms.Count > 4;

    }
}

