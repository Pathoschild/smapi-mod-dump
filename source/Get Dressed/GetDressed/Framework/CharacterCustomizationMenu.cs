using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace GetDressed.Framework
{
    /// <summary>The menu which lets the player customise their character's appearance.</summary>
    internal class CharacterCustomizationMenu : IClickableMenu
    {
        /*********
        ** Properties
        *********/
        /****
        ** Metadata
        ****/
        /// <summary>Provides simplified APIs for writing mods.</summary>
        private readonly IModHelper ModHelper;

        /// <summary>Encapsulates the underlying mod texture management.</summary>
        private readonly ContentHelper ContentHelper;

        /// <summary>The current per-save config settings.</summary>
        private readonly LocalConfig PlayerConfig;

        /// <summary>The global config settings.</summary>
        private readonly GlobalConfig GlobalConfig;

        /// <summary>The zoom level before the menu was opened.</summary>
        private readonly float PlayerZoomLevel;

        /// <summary>The current mod version.</summary>
        private readonly ISemanticVersion ModVersion;

        /****
        ** Components
        ****/
        /// <summary>The messages to display on the screen.</summary>
        private readonly List<Alert> Alerts = new List<Alert>();

        /// <summary>The color picker for the character's pants.</summary>
        private ColorPicker PantsColorPicker;

        /// <summary>The color picker for the character's hair.</summary>
        private ColorPicker HairColorPicker;

        /// <summary>The color picker for the character's eyes.</summary>
        private ColorPicker EyeColorPicker;

        /// <summary>The tabs used to switch submenu.</summary>
        private ClickableComponent[] MenuTabs = new ClickableComponent[0];

        /// <summary>The field labels.</summary>
        private readonly List<ClickableComponent> Labels = new List<ClickableComponent>();

        /// <summary>The labels for arrow selectors, which also show the currently selected value.</summary>
        private readonly List<ClickableComponent> SelectorLabels = new List<ClickableComponent>();

        /// <summary>The arrow buttons for selectors.</summary>
        private readonly List<ClickableTextureComponent> ArrowButtons = new List<ClickableTextureComponent>();

        /// <summary>The gender chooser buttons.</summary>
        private ClickableTextureComponent[] GenderButtons = new ClickableTextureComponent[0];

        /// <summary>The 'load favorite' buttons in the main submenu.</summary>
        private ClickableTextureComponent[] QuickLoadFavButtons = new ClickableTextureComponent[0];

        /// <summary>The favorite icons in the 'manage favourites' submenu.</summary>
        private ClickableTextureComponent[] ManageFavIcons = new ClickableTextureComponent[0];

        /// <summary>The additional favorite icons in the extended 'manage favourites' submenu.</summary>
        private readonly List<ClickableTextureComponent> ExtraFavButtons = new List<ClickableTextureComponent>();

        /// <summary>The 'set' buttons on the 'manage favorites' submenu.</summary>
        private ClickableTextureComponent[] SetFavButtons = new ClickableTextureComponent[0];

        /// <summary>The icon for the character customisation tab.</summary>
        private ClickableTextureComponent CharTabIcon;

        /// <summary>The icon for the 'manage favorites' tab.</summary>
        private ClickableTextureComponent FavIcon;

        /// <summary>The icon for the 'about' tab.</summary>
        private ClickableTextureComponent AboutIcon;

        /// <summary>The icon for the 'quick outfits' subtab on the 'manage favorites' submenu.</summary>
        private ClickableTextureComponent QuickFavsIcon;

        /// <summary>The icon for the 'extra outfits' subtab on the 'manage favorites' submenu.</summary>
        private ClickableTextureComponent ExtraFavsIcon;

        private ClickableTextureComponent CancelButton; // where is this used?

        /// <summary>The button on the main submenu which randomises the character.</summary>
        private ClickableTextureComponent RandomButton;

        /// <summary>The button on the main submenu which saves changes.</summary>
        private ClickableTextureComponent OkButton;

        /// <summary>The outline around the male option in <see cref="GenderButtons"/> when it's selected.</summary>
        private ClickableTextureComponent MaleOutlineButton;

        /// <summary>The outline around the female option in <see cref="GenderButtons"/> when it's selected.</summary>
        private ClickableTextureComponent FemaleOutlineButton;

        /// <summary>The 'load' button on the 'manage favorites' extended subtab when a favorite is selected.</summary>
        private ClickableTextureComponent LoadFavButton;

        /// <summary>The 'save' button on the 'manage favorites' extended subtab when a favorite is selected.</summary>
        private ClickableTextureComponent SaveFavButton;

        /// <summary>The 'set' button on the 'about' submenu to change the hotkey.</summary>
        private ClickableTextureComponent SetAccessKeyButton;

        /// <summary>The 'set' button on the 'about' submenu which toggles whether skirts are shown for male characters.</summary>
        private ClickableTextureComponent ToggleMaleSkirtsButton;

        /// <summary>The button on the 'about' submenu which zooms out the menu.</summary>
        private ClickableTextureComponent ZoomOutButton;

        /// <summary>The button on the 'about' submenu which zooms im the menu.</summary>
        private ClickableTextureComponent ZoomInButton;

        /// <summary>The button on the 'about' submenu which reset global settings to their default.</summary>
        private ClickableTextureComponent ResetConfigButton;

        /// <summary>A floating arrow which brings attention to the 'favorites' tab.</summary>
        private TemporaryAnimatedSprite FavTabArrow;

        /// <summary>A 'new' sprite that brings attention to the tabs.</summary>
        private TemporaryAnimatedSprite FloatingNew;

        /****
        ** State
        ****/
        /// <summary>The last color picker the player interacted with.</summary>
        private ColorPicker LastHeldColorPicker;

        /// <summary>How many times the player pressed the 'random' buttom since the menu was opened.</summary>
        private int TimesRandomised;

        /// <summary>The delay until the the character preview should be updated with the last colour picker change.</summary>
        private int ColorPickerTimer;

        /// <summary>The current selected favorite to load or save (or <c>-1</c> if none selected).</summary>
        private int CurrentFav = -1;

        /// <summary>The current tab being viewed.</summary>
        private MenuTab CurrentTab = MenuTab.Customise;

        /// <summary>The character's current face.</summary>
        private int FaceType;

        /// <summary>The character's current nose.</summary>
        private int NoseType;

        /// <summary>The character's current bottom.</summary>
        private int Bottoms;

        /// <summary>The character's current shoes.</summary>
        private int Shoes;

        /// <summary>The tooltip text to draw next to the cursor.</summary>
        private string HoverText;

        /// <summary>Whether the player is currently setting the menu key via <see cref="SetAccessKeyButton"/>.</summary>
        private bool IsSettingAccessMenuKey;

        /// <summary>Whether to show the <see cref="FavTabArrow"/>.</summary>
        private bool ShowFavTabArrow;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="contentHelper">Encapsulates the underlying mod texture management.</param>
        /// <param name="modHelper">Provides simplified APIs for writing mods.</param>
        /// <param name="modVersion">The current mod version.</param>
        /// <param name="globalConfig">The global config settings.</param>
        /// <param name="playerConfig">The current per-save config settings.</param>
        /// <param name="zoomLevel">The zoom level before the menu was opened.</param>
        public CharacterCustomizationMenu(ContentHelper contentHelper, IModHelper modHelper, ISemanticVersion modVersion, GlobalConfig globalConfig, LocalConfig playerConfig, float zoomLevel)
            : base(
                  x: Game1.viewport.Width / 2 - (680 + IClickableMenu.borderWidth * 2) / 2,
                  y: Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize,
                  width: 632 + IClickableMenu.borderWidth * 2,
                  height: 600 + IClickableMenu.borderWidth * 4 + Game1.tileSize
            )
        {
            // save metadata
            this.ContentHelper = contentHelper;
            this.ModHelper = modHelper;
            this.ModVersion = modVersion;
            this.GlobalConfig = globalConfig;
            this.PlayerConfig = playerConfig;
            this.PlayerZoomLevel = zoomLevel;

            // choose initial customisation values
            this.FaceType = playerConfig.ChosenFace.First();
            this.NoseType = playerConfig.ChosenNose.First();
            this.Bottoms = playerConfig.ChosenBottoms.First();
            this.Shoes = playerConfig.ChosenShoes.First();

            // override zoom level
            Game1.options.zoomLevel = this.GlobalConfig.MenuZoomOut ? 0.75f : 1f;
            Game1.overrideGameMenuReset = true;
            Game1.game1.refreshWindowSettings();
            this.UpdateLayout();
            Game1.player.faceDirection(2);
            Game1.player.FarmerSprite.StopAnimation();
        }

        /// <summary>The method called when the game window changes size.</summary>
        /// <param name="oldBounds">The former viewport.</param>
        /// <param name="newBounds">The new viewport.</param>
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            this.UpdateLayout();
        }

        /// <summary>The method invoked when the player presses the left mouse button.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        /// <param name="playSound">Whether to enable sound.</param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            Texture2D menuTextures = this.ContentHelper.MenuTextures;

            // tabs
            if (this.CharTabIcon.containsPoint(x, y))
                this.SetTab(MenuTab.Customise);
            else if (this.FavIcon.containsPoint(x, y) && this.CurrentTab != MenuTab.Favorites)
                this.SetTab(MenuTab.Favorites);
            else if (this.QuickFavsIcon.containsPoint(x, y) && this.CurrentTab == MenuTab.FavoritesExtras)
                this.SetTab(MenuTab.Favorites);
            else if (this.ExtraFavsIcon.containsPoint(x, y) && this.CurrentTab == MenuTab.Favorites)
                this.SetTab(MenuTab.FavoritesExtras);
            else if (this.AboutIcon.containsPoint(x, y))
                this.SetTab(MenuTab.About);

            // cancel button
            else if (this.CancelButton.containsPoint(x, y))
                this.ExitMenu();

            // tab contents
            switch (this.CurrentTab)
            {
                // main customisation tab
                case MenuTab.Customise:
                    // gender buttons
                    foreach (ClickableTextureComponent button in this.GenderButtons)
                    {
                        if (button.containsPoint(x, y))
                        {
                            this.HandleButtonAction(button.name);
                            button.scale -= 0.5f;
                            button.scale = Math.Max(3.5f, button.scale);
                            break;
                        }
                    }

                    // arrow buttons
                    foreach (ClickableTextureComponent current in this.ArrowButtons)
                    {
                        if (current.containsPoint(x, y))
                        {
                            this.HandleSelectorChange(current.name, Convert.ToInt32(current.hoverText));
                            current.scale -= 0.25f;
                            current.scale = Math.Max(0.75f, current.scale);
                            break;
                        }
                    }

                    // OK button
                    if (this.OkButton.containsPoint(x, y))
                    {
                        this.HandleButtonAction(this.OkButton.name);
                        this.OkButton.scale -= 0.25f;
                        this.OkButton.scale = Math.Max(0.75f, this.OkButton.scale);
                        break;
                    }

                    // color pickers
                    if (this.HairColorPicker.containsPoint(x, y))
                    {
                        Game1.player.changeHairColor(this.HairColorPicker.click(x, y));
                        this.LastHeldColorPicker = this.HairColorPicker;
                        break;
                    }
                    if (this.PantsColorPicker.containsPoint(x, y))
                    {
                        Game1.player.changePants(this.PantsColorPicker.click(x, y));
                        this.LastHeldColorPicker = this.PantsColorPicker;
                        break;
                    }
                    if (this.EyeColorPicker.containsPoint(x, y))
                    {
                        Game1.player.changeEyeColor(this.EyeColorPicker.click(x, y));
                        this.LastHeldColorPicker = this.EyeColorPicker;
                        break;
                    }

                    // quick favorites
                    for (int i = 0; i < this.QuickLoadFavButtons.Length; i++)
                    {
                        if (!this.QuickLoadFavButtons[i].containsPoint(x, y))
                            continue;

                        if (this.LoadFavorite(i + 1))
                            Game1.playSound("yoba");
                        else
                        {
                            this.Alerts.Add(new Alert(Game1.mouseCursors, new Rectangle(268, 470, 16, 16), Game1.viewport.Width / 2 - (700 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize, "Uh oh! No Favorite is Set!", 1000, false));
                            this.UpdateTabFloaters();
                            this.ShowFavTabArrow = true;
                        }
                        break;
                    }

                    // random button
                    if (this.RandomButton.containsPoint(x, y))
                    {
                        this.RandomiseCharacter();
                        this.RandomButton.scale = Game1.pixelZoom - 0.5f;
                        this.PantsColorPicker.setColor(Game1.player.pantsColor);
                        this.EyeColorPicker.setColor(Game1.player.newEyeColor);
                        this.HairColorPicker.setColor(Game1.player.hairstyleColor);
                    }
                    break;

                // 'manage favorites' tab
                case MenuTab.Favorites:
                    // quick favorite buttons
                    for (int i = 0; i < this.QuickLoadFavButtons.Length; i++)
                    {
                        if (this.SetFavButtons[i].containsPoint(x, y))
                        {
                            // set favorite
                            this.SetFavorite(i + 1);
                            this.ModHelper.WriteJsonFile(ModConstants.PerSaveConfigPath, this.PlayerConfig);

                            // hide 'new' button
                            if (this.GlobalConfig.ShowIntroBanner)
                            {
                                this.GlobalConfig.ShowIntroBanner = false;
                                this.ModHelper.WriteConfig(this.GlobalConfig);
                            }

                            // show 'favorite saved' alert
                            this.QuickLoadFavButtons[i].sourceRect.Y = 26;
                            this.ManageFavIcons[i].sourceRect.Y = 26;
                            this.Alerts.Add(new Alert(Game1.mouseCursors, new Rectangle(310, 392, 16, 16), Game1.viewport.Width / 2 - (700 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize, "New Favorite Saved.", 1200, false));
                            Game1.playSound("purchase");
                            break;
                        }
                    }
                    break;

                // favorites > 'extra outfits' subtab
                case MenuTab.FavoritesExtras:
                    // save favorite button
                    if (this.SaveFavButton.containsPoint(x, y) && this.CurrentFav > -1)
                    {
                        // set favorites
                        this.SetFavorite(this.CurrentFav + 1);
                        this.ModHelper.WriteJsonFile(ModConstants.PerSaveConfigPath, this.PlayerConfig);

                        //hide 'new' button
                        if (this.GlobalConfig.ShowIntroBanner)
                        {
                            this.GlobalConfig.ShowIntroBanner = false;
                            this.ModHelper.WriteConfig(this.GlobalConfig);
                        }

                        // show 'favorite saved' alert
                        this.ExtraFavButtons[this.CurrentFav - 6].sourceRect.Y = 26;
                        this.Alerts.Add(new Alert(Game1.mouseCursors, new Rectangle(310, 392, 16, 16), Game1.viewport.Width / 2 - (700 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize, "Favorite Saved To Slot " + (this.CurrentFav + 1) + " .", 1200, false));
                        Game1.playSound("purchase");
                        break;
                    }

                    // load favorite button
                    if (this.LoadFavButton.containsPoint(x, y) && this.CurrentFav > -1)
                    {
                        if (this.LoadFavorite(this.CurrentFav + 1))
                        {
                            this.PatchBaseTexture();
                            Game1.playSound("yoba");
                        }
                        else
                        {
                            this.Alerts.Add(new Alert(Game1.mouseCursors, new Rectangle(268, 470, 16, 16), Game1.viewport.Width / 2 - (700 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize, "Uh oh! No Favorite is Set!", 1000, false));
                            this.UpdateTabFloaters();
                        }
                        break;
                    }

                    // extra favorite icons
                    for (int i = 0; i < this.ExtraFavButtons.Count; i++)
                    {
                        if (this.ExtraFavButtons[i].containsPoint(x, y))
                        {
                            foreach (ClickableTextureComponent bigFavButton in this.ExtraFavButtons)
                                bigFavButton.drawShadow = false;
                            this.ExtraFavButtons[i].drawShadow = true;
                            this.CurrentFav = i + 6;
                            break;
                        }
                    }
                    break;

                // about tab
                case MenuTab.About:
                    // set access button
                    if (this.SetAccessKeyButton.containsPoint(x, y))
                    {
                        this.IsSettingAccessMenuKey = true;
                        Game1.playSound("breathin");
                        break;
                    }

                    // toggle male skirts button
                    if (this.ToggleMaleSkirtsButton.containsPoint(x, y))
                    {
                        this.GlobalConfig.HideMaleSkirts = !this.GlobalConfig.HideMaleSkirts;
                        this.ModHelper.WriteConfig(this.GlobalConfig);
                        this.Alerts.Add(new Alert(menuTextures, new Rectangle(this.GlobalConfig.HideMaleSkirts ? 48 : 80, 144, 16, 16), Game1.viewport.Width / 2 - (700 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize, "Skirts " + (this.GlobalConfig.HideMaleSkirts ? "Hidden" : "Unhidden") + " for Males.", 1200, false));
                        Game1.playSound("coin");
                        break;
                    }

                    // zoom in button
                    if (this.ZoomInButton.containsPoint(x, y) && this.GlobalConfig.MenuZoomOut)
                    {
                        Game1.options.zoomLevel = 1f;
                        Game1.overrideGameMenuReset = true;
                        Game1.game1.refreshWindowSettings();

                        this.UpdateLayout();

                        this.GlobalConfig.MenuZoomOut = false;
                        this.ModHelper.WriteConfig(this.GlobalConfig);

                        this.ZoomInButton.sourceRect.Y = 177;
                        this.ZoomOutButton.sourceRect.Y = 167;

                        Game1.playSound("drumkit6");
                        this.Alerts.Add(new Alert(menuTextures, new Rectangle(80, 144, 16, 16), Game1.viewport.Width / 2 - (700 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize, "Zoom Setting Changed.", 1200, false, 200));
                        break;
                    }

                    // zoom out button
                    if (this.ZoomOutButton.containsPoint(x, y) && !this.GlobalConfig.MenuZoomOut)
                    {
                        Game1.options.zoomLevel = 0.75f;
                        Game1.overrideGameMenuReset = true;
                        Game1.game1.refreshWindowSettings();

                        this.UpdateLayout();

                        this.GlobalConfig.MenuZoomOut = true;
                        this.ModHelper.WriteConfig(this.GlobalConfig);

                        this.ZoomInButton.sourceRect.Y = 167;
                        this.ZoomOutButton.sourceRect.Y = 177;

                        Game1.playSound("coin");
                        this.Alerts.Add(new Alert(menuTextures, new Rectangle(80, 144, 16, 16), Game1.viewport.Width / 2 - (700 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize, "Zoom Setting Changed.", 1200, false, 200));
                        break;
                    }

                    // reset config button
                    if (this.ResetConfigButton.containsPoint(x, y))
                    {
                        this.GlobalConfig.HideMaleSkirts = false;
                        this.GlobalConfig.MenuAccessKey = "C";
                        Game1.options.zoomLevel = 0.75f;
                        Game1.overrideGameMenuReset = true;
                        Game1.game1.refreshWindowSettings();
                        this.UpdateLayout();
                        this.GlobalConfig.MenuZoomOut = true;
                        this.ModHelper.WriteConfig(this.GlobalConfig);
                        this.Alerts.Add(new Alert(menuTextures, new Rectangle(160, 144, 16, 16), Game1.viewport.Width / 2 - (700 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize, "Options Reset to Default", 1200, false, 200));
                        Game1.playSound("coin");
                    }
                    break;
            }
        }

        /// <summary>The method invoked while the player is holding down the left mouse button.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        public override void leftClickHeld(int x, int y)
        {
            // update color pickers
            this.ColorPickerTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
            if (this.ColorPickerTimer <= 0)
            {
                if (this.LastHeldColorPicker != null)
                {
                    if (this.LastHeldColorPicker.Equals(this.HairColorPicker))
                        Game1.player.changeHairColor(this.HairColorPicker.clickHeld(x, y));
                    if (this.LastHeldColorPicker.Equals(this.PantsColorPicker))
                        Game1.player.changePants(this.PantsColorPicker.clickHeld(x, y));
                    if (this.LastHeldColorPicker.Equals(this.EyeColorPicker))
                        Game1.player.changeEyeColor(this.EyeColorPicker.clickHeld(x, y));
                }
                this.ColorPickerTimer = 100;
            }
        }

        /// <summary>The method invoked when the player releases the left mouse button.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        public override void releaseLeftClick(int x, int y)
        {
            // update color pickers
            this.HairColorPicker.releaseClick();
            this.PantsColorPicker.releaseClick();
            this.EyeColorPicker.releaseClick();
            this.LastHeldColorPicker = null;
        }

        /// <summary>The method invoked when the player presses the left mouse button.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        /// <param name="playSound">Whether to enable sound.</param>
        public override void receiveRightClick(int x, int y, bool playSound = true) { }

        /// <summary>The method invoked when the player presses a keyboard button.</summary>
        /// <param name="key">The key that was pressed.</param>
        public override void receiveKeyPress(Keys key)
        {
            // exit menu
            if (Game1.options.menuButton.Contains(new InputButton(key)) && this.readyToClose())
                this.ExitMenu();

            // set key
            else if (this.IsSettingAccessMenuKey)
            {
                this.GlobalConfig.MenuAccessKey = key.ToString();
                this.ModHelper.WriteConfig(this.GlobalConfig);
                this.IsSettingAccessMenuKey = false;
                this.Alerts.Add(new Alert(this.ContentHelper.MenuTextures, new Rectangle(96, 144, 16, 16), Game1.viewport.Width / 2 - (700 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize, "Menu Access Key Changed.", 1200, false));
                Game1.playSound("coin");
            }
        }

        /// <summary>Update the menu state.</summary>
        /// <param name="time">The elapsed game time.</param>
        public override void update(GameTime time)
        {
            base.update(time);

            // update alert messages
            for (int i = this.Alerts.Count - 1; i >= 0; i--)
            {
                if (this.Alerts.ElementAt(i).Update(time))
                    this.Alerts.RemoveAt(i);
            }

            // update tab arrows
            this.FavTabArrow?.update(time);
            this.FloatingNew?.update(time);
        }

        /// <summary>The method invoked when the cursor is over a given position.</summary>
        /// <param name="x">The X mouse position.</param>
        /// <param name="y">The Y mouse position.</param>
        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);

            // reset hover text
            this.HoverText = "";

            // check subtab hovers
            foreach (ClickableComponent current in this.MenuTabs)
            {
                if (current.containsPoint(x, y))
                {
                    if (current.name.Equals("Quick Outfits") || current.name.Equals("Extra Outfits"))
                    {
                        if (this.CurrentTab == MenuTab.Favorites || this.CurrentTab == MenuTab.FavoritesExtras)
                            this.HoverText = current.name;
                    }
                    return;
                }
            }

            // cancel buttons
            this.CancelButton.tryHover(x, y, 0.25f);
            this.CancelButton.tryHover(x, y, 0.25f);

            // tab contents
            switch (this.CurrentTab)
            {
                // main customise tab
                case MenuTab.Customise:
                    // arrow buttons
                    foreach (ClickableTextureComponent button in this.ArrowButtons)
                    {
                        button.scale = button.containsPoint(x, y)
                            ? Math.Min(button.scale + 0.02f, button.baseScale + 0.1f)
                            : Math.Max(button.scale - 0.02f, button.baseScale);
                    }
                    foreach (ClickableTextureComponent button in this.GenderButtons)
                    {
                        button.scale = button.containsPoint(x, y)
                            ? Math.Min(button.scale + 0.02f, button.baseScale + 0.1f)
                            : Math.Max(button.scale - 0.02f, button.baseScale);
                    }

                    // random button
                    this.RandomButton.tryHover(x, y, 0.25f);
                    this.RandomButton.tryHover(x, y, 0.25f);

                    // OK button
                    this.OkButton.scale = this.OkButton.containsPoint(x, y)
                        ? Math.Min(this.OkButton.scale + 0.02f, this.OkButton.baseScale + 0.1f)
                        : Math.Max(this.OkButton.scale - 0.02f, this.OkButton.baseScale);

                    // quick favorites
                    for (int i = 0; i < this.QuickLoadFavButtons.Length; i++)
                    {
                        this.QuickLoadFavButtons[i].tryHover(x, y, 0.25f);
                        this.QuickLoadFavButtons[i].tryHover(x, y, 0.25f);
                        if (this.QuickLoadFavButtons[i].containsPoint(x, y))
                            this.HoverText = this.HasFavSlot(i + 1) ? "" : "No Favorite Is Set";
                    }
                    break;

                // main favorites tab
                case MenuTab.Favorites:
                    // set buttons
                    foreach (ClickableTextureComponent button in this.SetFavButtons)
                    {
                        button.tryHover(x, y, 0.25f);
                        button.tryHover(x, y, 0.25f);
                    }
                    break;

                // favorite 'extra outfits' subtab
                case MenuTab.FavoritesExtras:
                    this.LoadFavButton.tryHover(x, y, 0.25f);
                    this.LoadFavButton.tryHover(x, y, 0.25f);

                    this.SaveFavButton.tryHover(x, y, 0.25f);
                    this.SaveFavButton.tryHover(x, y, 0.25f);
                    break;

                // about tab
                case MenuTab.About:
                    this.SetAccessKeyButton.tryHover(x, y, 0.25f);
                    this.SetAccessKeyButton.tryHover(x, y, 0.25f);

                    this.ToggleMaleSkirtsButton.tryHover(x, y, 0.25f);
                    this.ToggleMaleSkirtsButton.tryHover(x, y, 0.25f);

                    this.ResetConfigButton.tryHover(x, y, 0.25f);
                    this.ResetConfigButton.tryHover(x, y, 0.25f);

                    if (this.GlobalConfig.MenuZoomOut)
                    {
                        this.ZoomInButton.tryHover(x, y, 0.25f);
                        this.ZoomInButton.tryHover(x, y, 0.25f);
                    }
                    else
                    {
                        this.ZoomOutButton.tryHover(x, y, 0.25f);
                        this.ZoomOutButton.tryHover(x, y, 0.25f);
                    }
                    break;
            }
        }

        /// <summary>Draw the menu to the screen.</summary>
        /// <param name="spriteBatch">The sprite batch to which to draw.</param>
        public override void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

            // menu background
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width + 50, height, false, true);

            // tabs
            {
                // check selected tab
                bool isCustomiseTab = this.CurrentTab == MenuTab.Customise;
                bool isFavoriteTab = this.CurrentTab == MenuTab.Favorites || this.CurrentTab == MenuTab.FavoritesExtras;
                bool isAboutTab = this.CurrentTab == MenuTab.About;
                bool isMainOutfitsTab = this.CurrentTab == MenuTab.Favorites;
                bool isExtraOutfitsTab = this.CurrentTab == MenuTab.FavoritesExtras;

                // get tab positions
                Vector2 character = new Vector2(xPositionOnScreen + 45, yPositionOnScreen + (isCustomiseTab ? 25 : 16));
                Vector2 favorites = new Vector2(xPositionOnScreen + 110, yPositionOnScreen + (isFavoriteTab ? 25 : 16));
                Vector2 about = new Vector2(xPositionOnScreen + 175, yPositionOnScreen + (isAboutTab ? 25 : 16));
                Vector2 quickFavorites = new Vector2(xPositionOnScreen - (isMainOutfitsTab ? 40 : 47), yPositionOnScreen + 107);
                Vector2 extraFavorites = new Vector2(xPositionOnScreen - (isExtraOutfitsTab ? 40 : 47), yPositionOnScreen + 171);

                // customise tab
                spriteBatch.Draw(Game1.mouseCursors, character, new Rectangle(16, 368, 16, 16), Color.White, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.0001f);
                Game1.player.FarmerRenderer.drawMiniPortrat(spriteBatch, new Vector2(xPositionOnScreen + 53, yPositionOnScreen + (Game1.player.isMale ? (isCustomiseTab ? 35 : 26) : (isCustomiseTab ? 32 : 23))), 0.00011f, 3f, 2, Game1.player);

                // favorite tab
                spriteBatch.Draw(Game1.mouseCursors, favorites, new Rectangle(16, 368, 16, 16), Color.White, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.0001f);
                this.FavIcon.draw(spriteBatch);

                // about tab
                spriteBatch.Draw(Game1.mouseCursors, about, new Rectangle(16, 368, 16, 16), Color.White, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.0001f);
                this.AboutIcon.draw(spriteBatch);

                // favorite subtabs
                if (isFavoriteTab)
                {
                    spriteBatch.Draw(this.ContentHelper.MenuTextures, quickFavorites, new Rectangle(52, 202, 16, 16), Color.White, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.0001f);
                    spriteBatch.Draw(this.ContentHelper.MenuTextures, extraFavorites, new Rectangle(52, 202, 16, 16), Color.White, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.0001f);
                    this.QuickFavsIcon.draw(spriteBatch);
                    this.ExtraFavsIcon.draw(spriteBatch);
                }
            }

            // tab contents
            switch (this.CurrentTab)
            {
                // main customise tab
                case MenuTab.Customise:
                    // header
                    SpriteText.drawString(spriteBatch, "Customize Character:", xPositionOnScreen + 55, yPositionOnScreen + 115);

                    // portrait
                    spriteBatch.Draw(Game1.daybg, new Vector2(xPositionOnScreen + Game1.tileSize + Game1.tileSize * 2 / 3 - 2, (yPositionOnScreen + 75) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 4), Color.White);
                    Game1.player.FarmerRenderer.draw(spriteBatch, Game1.player.FarmerSprite.CurrentAnimationFrame, Game1.player.FarmerSprite.CurrentFrame, Game1.player.FarmerSprite.SourceRect, new Vector2(xPositionOnScreen - 2 + Game1.tileSize * 2 / 3 + Game1.tileSize * 2 - Game1.tileSize / 2, (yPositionOnScreen + 75) + IClickableMenu.borderWidth - Game1.tileSize / 4 + IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 2), Vector2.Zero, 0.8f, Color.White, 0f, 1f, Game1.player);

                    // gender buttons
                    foreach (ClickableTextureComponent textureComponent in this.GenderButtons)
                    {
                        textureComponent.draw(spriteBatch);
                        if (textureComponent.name.Equals("Male") && Game1.player.isMale)
                            this.MaleOutlineButton.draw(spriteBatch);
                        if (textureComponent.name.Equals("Female") && !Game1.player.isMale)
                            this.FemaleOutlineButton.draw(spriteBatch);
                    }

                    // favorite buttons
                    foreach (ClickableTextureComponent button in this.QuickLoadFavButtons)
                        button.draw(spriteBatch);

                    // arrow buttons
                    foreach (ClickableTextureComponent button in this.ArrowButtons)
                        button.draw(spriteBatch);

                    // labels
                    foreach (ClickableComponent label in this.Labels)
                    {
                        string text = "";
                        Color color = Game1.textColor;
                        int x = label.bounds.X;
                        switch (label.name.Substring(0, 4))
                        {
                            case "Shir":
                                text = string.Concat(Game1.player.shirt + 1);
                                x = x + 4;
                                break;

                            case "Skin":
                                text = string.Concat(Game1.player.skin + 1);
                                break;

                            case "Hair":
                                if (!label.name.Contains("Color"))
                                    text = string.Concat(Game1.player.hair + 1);
                                break;

                            case "Acc.":
                                if (this.FaceType == 0)
                                    text = string.Concat(Game1.player.accessory + 2);
                                else if (this.FaceType == 1 && Game1.player.accessory < 20)
                                    text = string.Concat(Game1.player.accessory + 2);
                                else if (this.FaceType == 1 && Game1.player.accessory > 20)
                                    text = string.Concat(Game1.player.accessory - 110);
                                break;

                            default:
                                color = Game1.textColor;
                                break;
                        }

                        Utility.drawTextWithShadow(spriteBatch, label.name, Game1.smallFont, new Vector2(label.bounds.X, label.bounds.Y), color);
                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            if (text.Length == 1) text = " " + text + " ";
                            Utility.drawTextWithShadow(spriteBatch, text, Game1.smallFont, new Vector2(x + Game1.smallFont.MeasureString("Shirt").X / 2f - Game1.smallFont.MeasureString("100").X / 2, label.bounds.Y + Game1.tileSize / 2), color);
                        }
                    }

                    // selector labels
                    foreach (ClickableComponent label in this.SelectorLabels)
                    {
                        string text = "";
                        Color color = Game1.textColor;
                        switch (label.name.Substring(0, 4))
                        {
                            case "Face":
                                text = string.Concat(this.FaceType + 1);
                                break;

                            case "Nose":
                                text = string.Concat(this.NoseType + 1);
                                break;

                            case " Bot":
                                text = string.Concat(this.Bottoms + 1);
                                break;

                            case "  Sh":
                                text = string.Concat(this.Shoes + 1);
                                break;

                            default:
                                color = Game1.textColor;
                                break;
                        }
                        Utility.drawTextWithShadow(spriteBatch, label.name, Game1.smallFont, new Vector2(label.bounds.X, label.bounds.Y), color);
                        if (!string.IsNullOrWhiteSpace(text))
                            Utility.drawTextWithShadow(spriteBatch, text, Game1.smallFont, new Vector2(label.bounds.X + Game1.smallFont.MeasureString("Face Type").X / 2f - Game1.smallFont.MeasureString("10").X / 2, label.bounds.Y + Game1.tileSize / 2), color);
                    }

                    // buttons
                    this.OkButton.draw(spriteBatch);
                    this.RandomButton.draw(spriteBatch);

                    // color pickers
                    this.HairColorPicker.draw(spriteBatch);
                    this.PantsColorPicker.draw(spriteBatch);
                    this.EyeColorPicker.draw(spriteBatch);

                    break;

                // main favorites tab
                case MenuTab.Favorites:
                    // header
                    SpriteText.drawString(spriteBatch, "Manage Favorites:", xPositionOnScreen + 55, yPositionOnScreen + 115);

                    // portrait
                    spriteBatch.Draw(Game1.daybg, new Vector2(xPositionOnScreen + 430 + Game1.tileSize + Game1.tileSize * 2 / 3 - 2, yPositionOnScreen + 105 + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 4), Color.White);
                    Game1.player.FarmerRenderer.draw(spriteBatch, Game1.player.FarmerSprite.CurrentAnimationFrame, Game1.player.FarmerSprite.CurrentFrame, Game1.player.FarmerSprite.SourceRect, new Vector2(xPositionOnScreen + 428 + Game1.tileSize * 2 / 3 + Game1.tileSize * 2 - Game1.tileSize / 2, (yPositionOnScreen + 105) + IClickableMenu.borderWidth - Game1.tileSize / 4 + IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 2), Vector2.Zero, 0.8f, Color.White, 0f, 1f, Game1.player);

                    // labels
                    Utility.drawTextWithShadow(spriteBatch, "You can set up to 6 quick favorite appearance", Game1.smallFont, new Vector2(xPositionOnScreen + 50, yPositionOnScreen + 175), Color.Black);
                    Utility.drawTextWithShadow(spriteBatch, "configurations for each character.", Game1.smallFont, new Vector2(xPositionOnScreen + 50, yPositionOnScreen + 200), Color.Black);
                    Utility.drawTextWithShadow(spriteBatch, "Your current appearance is shown on", Game1.smallFont, new Vector2(xPositionOnScreen + 50, yPositionOnScreen + 285), Color.Black);
                    Utility.drawTextWithShadow(spriteBatch, "the right, use one of the buttons below", Game1.smallFont, new Vector2(xPositionOnScreen + 50, yPositionOnScreen + 310), Color.Black);
                    Utility.drawTextWithShadow(spriteBatch, "to set it as a favorite :", Game1.smallFont, new Vector2(xPositionOnScreen + 50, yPositionOnScreen + 335), Color.Black);

                    // favorite icons
                    foreach (ClickableTextureComponent icon in this.ManageFavIcons)
                        icon.draw(spriteBatch);
                    Utility.drawTextWithShadow(spriteBatch, "1st Favorite", Game1.smallFont, new Vector2(xPositionOnScreen + 90, yPositionOnScreen + 475), Color.Black);
                    Utility.drawTextWithShadow(spriteBatch, "2nd Favorite", Game1.smallFont, new Vector2(xPositionOnScreen + 90, yPositionOnScreen + 550), Color.Black);
                    Utility.drawTextWithShadow(spriteBatch, "3rd Favorite", Game1.smallFont, new Vector2(xPositionOnScreen + 90, yPositionOnScreen + 625), Color.Black);
                    Utility.drawTextWithShadow(spriteBatch, "4th Favorite", Game1.smallFont, new Vector2(xPositionOnScreen + 467, yPositionOnScreen + 475), Color.Black);
                    Utility.drawTextWithShadow(spriteBatch, "5th Favorite", Game1.smallFont, new Vector2(xPositionOnScreen + 467, yPositionOnScreen + 550), Color.Black);
                    Utility.drawTextWithShadow(spriteBatch, "6th Favorite", Game1.smallFont, new Vector2(xPositionOnScreen + 467, yPositionOnScreen + 625), Color.Black);

                    Utility.drawTextWithShadow(spriteBatch, "Hint: Click the SET button lined up with each Favorite to", Game1.smallFont, new Vector2(xPositionOnScreen + 50, yPositionOnScreen + 700), Color.Black);
                    Utility.drawTextWithShadow(spriteBatch, "set your current appearance as that Favorite.", Game1.smallFont, new Vector2(xPositionOnScreen + 110, yPositionOnScreen + 725), Color.Black);

                    foreach (ClickableTextureComponent saveFavButton in this.SetFavButtons)
                        saveFavButton.draw(spriteBatch);

                    break;

                // 'extra outfits' subtab
                case MenuTab.FavoritesExtras:
                    // header
                    SpriteText.drawString(spriteBatch, "Manage Favorites:", xPositionOnScreen + 55, yPositionOnScreen + 115);

                    // portrait
                    spriteBatch.Draw(Game1.daybg, new Vector2(xPositionOnScreen + 430 + Game1.tileSize + Game1.tileSize * 2 / 3 - 2, (yPositionOnScreen + 105) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 4), Color.White);
                    Game1.player.FarmerRenderer.draw(spriteBatch, Game1.player.FarmerSprite.CurrentAnimationFrame, Game1.player.FarmerSprite.CurrentFrame, Game1.player.FarmerSprite.SourceRect, new Vector2(xPositionOnScreen + 428 + Game1.tileSize * 2 / 3 + Game1.tileSize * 2 - Game1.tileSize / 2, (yPositionOnScreen + 105) + IClickableMenu.borderWidth - Game1.tileSize / 4 + IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 2), Vector2.Zero, 0.8f, Color.White, 0f, 1f, Game1.player);

                    // labels
                    Utility.drawTextWithShadow(spriteBatch, "You can set up to 30 additional favorite appearance", Game1.smallFont, new Vector2(xPositionOnScreen + 50, yPositionOnScreen + 175), Color.Black);
                    Utility.drawTextWithShadow(spriteBatch, "configurations for each character.", Game1.smallFont, new Vector2(xPositionOnScreen + 50, yPositionOnScreen + 200), Color.Black);
                    Utility.drawTextWithShadow(spriteBatch, "Your current appearance is shown on", Game1.smallFont, new Vector2(xPositionOnScreen + 50, yPositionOnScreen + 285), Color.Black);
                    Utility.drawTextWithShadow(spriteBatch, "the right, select a favorite below to", Game1.smallFont, new Vector2(xPositionOnScreen + 50, yPositionOnScreen + 310), Color.Black);
                    Utility.drawTextWithShadow(spriteBatch, "save your appearance in it or load the", Game1.smallFont, new Vector2(xPositionOnScreen + 50, yPositionOnScreen + 335), Color.Black);
                    Utility.drawTextWithShadow(spriteBatch, "appearance saved in it :", Game1.smallFont, new Vector2(xPositionOnScreen + 50, yPositionOnScreen + 360), Color.Black);

                    // favorite icons
                    foreach (ClickableTextureComponent icon in this.ExtraFavButtons)
                        icon.draw(spriteBatch);

                    if (this.CurrentFav > -1)
                    {
                        string label = "Currently selected: " + (this.CurrentFav + 1) + "."; // <-- String for printing currently selected favorite
                        Utility.drawTextWithShadow(spriteBatch, label, Game1.smallFont, new Vector2(xPositionOnScreen + 140, yPositionOnScreen + 533), Color.Black);
                        this.SaveFavButton.draw(spriteBatch);

                        Utility.drawTextWithShadow(spriteBatch, "Overwrite Fav. Slot", Game1.smallFont, new Vector2(xPositionOnScreen + 140, yPositionOnScreen + 583), Color.Black);
                        this.LoadFavButton.draw(spriteBatch);
                    }
                    else
                    {
                        string whatever = "Please select a favorite...";
                        Utility.drawTextWithShadow(spriteBatch, whatever, Game1.smallFont, new Vector2(xPositionOnScreen + 140, yPositionOnScreen + 533), Color.Black);
                    }

                    break;

                // about tab
                case MenuTab.About:
                    // header
                    SpriteText.drawString(spriteBatch, "About This Mod:", xPositionOnScreen + 55, yPositionOnScreen + 115);

                    // info
                    Utility.drawTextWithShadow(spriteBatch, "Get Dressed created by JinxieWinxie and Advize", Game1.smallFont, new Vector2(xPositionOnScreen + 50, yPositionOnScreen + 175), Color.Black);
                    Utility.drawTextWithShadow(spriteBatch, $"You are using version:  {this.ModVersion}", Game1.smallFont, new Vector2(xPositionOnScreen + 50, yPositionOnScreen + 225), Color.Black);
                    SpriteText.drawString(spriteBatch, "Settings:", xPositionOnScreen + 55, yPositionOnScreen + 275);
                    Utility.drawTextWithShadow(spriteBatch, "Face Types (M-F): " + this.GlobalConfig.MaleFaceTypes + "-" + this.GlobalConfig.FemaleFaceTypes, Game1.smallFont, new Vector2(xPositionOnScreen + 50, yPositionOnScreen + 345), Color.Black);
                    Utility.drawTextWithShadow(spriteBatch, "Nose Types (M-F): " + this.GlobalConfig.MaleNoseTypes + "-" + this.GlobalConfig.FemaleNoseTypes, Game1.smallFont, new Vector2(xPositionOnScreen + 400, yPositionOnScreen + 345), Color.Black);
                    Utility.drawTextWithShadow(spriteBatch, "Bottoms Types (M-F): " + (this.GlobalConfig.HideMaleSkirts ? 2 : this.GlobalConfig.MaleBottomsTypes) + "-" + this.GlobalConfig.FemaleBottomsTypes, Game1.smallFont, new Vector2(xPositionOnScreen + 50, yPositionOnScreen + 395), Color.Black);
                    Utility.drawTextWithShadow(spriteBatch, "Shoes Types (M-F): " + this.GlobalConfig.MaleShoeTypes + "-" + this.GlobalConfig.FemaleShoeTypes, Game1.smallFont, new Vector2(xPositionOnScreen + 400, yPositionOnScreen + 395), Color.Black);
                    Utility.drawTextWithShadow(spriteBatch, "Show Dresser: " + this.GlobalConfig.ShowDresser, Game1.smallFont, new Vector2(xPositionOnScreen + 50, yPositionOnScreen + 445), Color.Black);
                    Utility.drawTextWithShadow(spriteBatch, "Stove in Corner: " + this.GlobalConfig.StoveInCorner, Game1.smallFont, new Vector2(xPositionOnScreen + 400, yPositionOnScreen + 445), Color.Black);

                    // set menu access key
                    Utility.drawTextWithShadow(spriteBatch, "Open Menu Key:  " + this.GlobalConfig.MenuAccessKey, Game1.smallFont, new Vector2(xPositionOnScreen + 50, yPositionOnScreen + 525), Color.Black);
                    this.SetAccessKeyButton.draw(spriteBatch);

                    // toggle skirs for male characters
                    Utility.drawTextWithShadow(spriteBatch, "Toggle Skirts for Male Characters  ", Game1.smallFont, new Vector2(xPositionOnScreen + 50, yPositionOnScreen + 600), Color.Black);
                    this.ToggleMaleSkirtsButton.draw(spriteBatch);

                    // set zoom level
                    Utility.drawTextWithShadow(spriteBatch, "Change Zoom Level  ", Game1.smallFont, new Vector2(xPositionOnScreen + 50, yPositionOnScreen + 675), Color.Black);
                    this.ZoomOutButton.draw(spriteBatch);
                    this.ZoomInButton.draw(spriteBatch);

                    // reset config options
                    Utility.drawTextWithShadow(spriteBatch, "Reset Options to Default  ", Game1.smallFont, new Vector2(xPositionOnScreen + 50, yPositionOnScreen + 750), Color.Black);
                    this.ResetConfigButton.draw(spriteBatch);

                    // set menu access key overlay
                    if (this.IsSettingAccessMenuKey)
                    {
                        spriteBatch.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.90f);
                        spriteBatch.DrawString(Game1.dialogueFont, "Press new key...", new Vector2(xPositionOnScreen + 225, yPositionOnScreen + 350), Color.White);
                    }
                    break;
            }

            // tab floaters
            if (this.GlobalConfig.ShowIntroBanner)
                FloatingNew?.draw(spriteBatch, true, 400, 950);
            if (this.ShowFavTabArrow)
                FavTabArrow?.draw(spriteBatch, true, 400, 950);

            // alerts
            foreach (Alert alert in this.Alerts)
                alert.Draw(spriteBatch, Game1.smallFont);

            // cancel button
            this.CancelButton.draw(spriteBatch);

            // cursor
            if (this.HoverText.Equals("No Favorite Is Set"))
            {
                spriteBatch.Draw(this.ContentHelper.MenuTextures, new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 6, 16, 16), Color.White, 0f, Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
                IClickableMenu.drawHoverText(spriteBatch, this.HoverText, Game1.smallFont, 20, 20);
            }
            else
            {
                spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16), Color.White, 0f, Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
                IClickableMenu.drawHoverText(spriteBatch, this.HoverText, Game1.smallFont);
            }
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Update the menu layout for a change in the zoom level or viewport size.</summary>
        private void UpdateLayout()
        {
            // reset window position
            this.xPositionOnScreen = Game1.viewport.Width / 2 - (680 + IClickableMenu.borderWidth * 2) / 2;
            this.yPositionOnScreen = Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize;

            // remove current components
            this.Labels.Clear();
            this.SelectorLabels.Clear();
            this.ArrowButtons.Clear();
            this.ExtraFavButtons.Clear();

            // initialise all components
            {
                Texture2D menuTextures = this.ContentHelper.MenuTextures;

                // tabs
                this.MenuTabs = new ClickableComponent[]
                {
                    this.CharTabIcon = new ClickableTextureComponent("Customize Character", new Rectangle(xPositionOnScreen + 62, yPositionOnScreen + 40, 50, 50), "", "", menuTextures, new Rectangle(9, 48, 8, 11), Game1.pixelZoom),
                    this.FavIcon = new ClickableTextureComponent("Manage Favorites", new Rectangle(xPositionOnScreen + 125, yPositionOnScreen + 40, 50, 50), "", "", menuTextures, new Rectangle(24, 26, 8, 8), Game1.pixelZoom),
                    this.AboutIcon = new ClickableTextureComponent("About", new Rectangle(xPositionOnScreen + 188, yPositionOnScreen + 33, 50, 50), "", "", menuTextures, new Rectangle(0, 48, 8, 11), Game1.pixelZoom),
                    this.QuickFavsIcon = new ClickableTextureComponent("Quick Outfits", new Rectangle(xPositionOnScreen - 23, yPositionOnScreen + 122, 50, 50), "", "", menuTextures, new Rectangle(8, 26, 8, 8), Game1.pixelZoom),
                    this.ExtraFavsIcon = new ClickableTextureComponent("Extra Outfits", new Rectangle(xPositionOnScreen - 23, yPositionOnScreen + 186, 50, 50), "", "", menuTextures, new Rectangle(0, 26, 8, 8), Game1.pixelZoom)
                };

                // cancel button
                this.CancelButton = new ClickableTextureComponent(new Rectangle((xPositionOnScreen + 675) + Game1.pixelZoom * 12, (yPositionOnScreen - 125) + Game1.tileSize + Game1.pixelZoom * 14, Game1.pixelZoom * 10, Game1.pixelZoom * 10), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), Game1.pixelZoom);

                // customize character submenu
                {
                    // buttons
                    this.RandomButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + Game1.pixelZoom * 12, (yPositionOnScreen + 75) + Game1.tileSize + Game1.pixelZoom * 14, Game1.pixelZoom * 10, Game1.pixelZoom * 10), Game1.mouseCursors, new Rectangle(381, 361, 10, 10), Game1.pixelZoom);
                    this.OkButton = new ClickableTextureComponent("OK", new Rectangle(xPositionOnScreen + 30 + width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize, yPositionOnScreen + height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 4, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);

                    // direction buttons
                    {
                        int yOffset = Game1.tileSize * 2;
                        this.ArrowButtons.Add(new ClickableTextureComponent("Direction", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize / 4, (yPositionOnScreen + 75) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, Game1.tileSize, Game1.tileSize), "", "-1", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f));
                        this.ArrowButtons.Add(new ClickableTextureComponent("Direction", new Rectangle(xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 2, (yPositionOnScreen + 75) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, Game1.tileSize, Game1.tileSize), "", "1", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f));
                    }

                    // gender buttons
                    {
                        int scale = Game1.pixelZoom / 2;
                        this.GenderButtons = new[]
                        {
                            new ClickableTextureComponent("Male", new Rectangle((xPositionOnScreen + 25) + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize / 2 + 8, (yPositionOnScreen + 70) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 3, Game1.tileSize, Game1.tileSize), null, "Male", Game1.mouseCursors, new Rectangle(128, 192, 16, 16), scale),
                            new ClickableTextureComponent("Female", new Rectangle((xPositionOnScreen + 25) + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize / 2 + Game1.tileSize + 8, (yPositionOnScreen + 70) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 3, Game1.tileSize, Game1.tileSize), null, "Female", Game1.mouseCursors, new Rectangle(144, 192, 16, 16), scale)
                        };
                        this.MaleOutlineButton = new ClickableTextureComponent("", new Rectangle((xPositionOnScreen + 24) + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize / 2 + 8, (yPositionOnScreen + 68) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 3, Game1.tileSize, Game1.tileSize), "", "", menuTextures, new Rectangle(19, 38, 19, 19), scale);
                        this.FemaleOutlineButton = new ClickableTextureComponent("", new Rectangle((xPositionOnScreen + 22) + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize / 2 + Game1.tileSize + 8, (yPositionOnScreen + 67) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 3, Game1.tileSize, Game1.tileSize), "", "", menuTextures, new Rectangle(19, 38, 19, 19), scale);
                    }

                    // color pickers
                    {
                        // eye color
                        int yOffset = 70;
                        this.Labels.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 4 + 8, (yPositionOnScreen + 0) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), "Eye Color:"));
                        this.EyeColorPicker = new ColorPicker(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + Game1.tileSize * 6 + Game1.tileSize * 3 / 4 + IClickableMenu.borderWidth, (yPositionOnScreen + 0) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset);
                        this.EyeColorPicker.setColor(Game1.player.newEyeColor);

                        // hair color
                        yOffset += Game1.tileSize + 8;
                        this.Labels.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 4 + 8, (yPositionOnScreen + 0) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), "Hair Color:"));
                        this.HairColorPicker = new ColorPicker(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + Game1.tileSize * 6 + Game1.tileSize * 3 / 4 + IClickableMenu.borderWidth, (yPositionOnScreen + 0) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset);
                        this.HairColorPicker.setColor(Game1.player.hairstyleColor);

                        // pants color
                        yOffset += Game1.tileSize + 8;
                        this.Labels.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 4 + 8, (yPositionOnScreen + 0) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), "Pants Color:"));
                        this.PantsColorPicker = new ColorPicker(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + Game1.tileSize * 6 + Game1.tileSize * 3 / 4 + IClickableMenu.borderWidth, (yPositionOnScreen + 0) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset);
                        this.PantsColorPicker.setColor(Game1.player.pantsColor);
                    }

                    // type selectors
                    {
                        // skin + face
                        int yOffset = Game1.tileSize * 4 + 8 + 50;
                        this.ArrowButtons.Add(new ClickableTextureComponent("Skin", new Rectangle((xPositionOnScreen + 16) + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize / 4, (yPositionOnScreen + 36) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, Game1.tileSize, Game1.tileSize), "", "-1", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 0.75f));
                        this.Labels.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize / 4 + Game1.tileSize + 8, (yPositionOnScreen + 20) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), "Skin"));
                        this.ArrowButtons.Add(new ClickableTextureComponent("Skin", new Rectangle(xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 2, (yPositionOnScreen + 36) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, Game1.tileSize, Game1.tileSize), "", "1", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 0.75f));

                        this.ArrowButtons.Add(new ClickableTextureComponent("Face Type", new Rectangle((xPositionOnScreen + 16) + Game1.tileSize * 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth, (yPositionOnScreen + 36) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, Game1.tileSize, Game1.tileSize), "", "-1", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 0.75f));
                        this.SelectorLabels.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + Game1.tileSize * 5 + 8 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth, (yPositionOnScreen + 20) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), "Face Type"));
                        this.ArrowButtons.Add(new ClickableTextureComponent("Face Type", new Rectangle(xPositionOnScreen + Game1.tileSize * 7 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth, (yPositionOnScreen + 36) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, Game1.tileSize, Game1.tileSize), "", "1", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 0.75f));

                        // hair + face
                        yOffset += Game1.tileSize + 8 + 30;
                        this.ArrowButtons.Add(new ClickableTextureComponent("Hair", new Rectangle((xPositionOnScreen + 16) + Game1.tileSize / 4 + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder, (yPositionOnScreen + 16) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, Game1.tileSize, Game1.tileSize), "", "-1", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 0.75f));
                        this.Labels.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize + 8, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), "Hair"));
                        this.ArrowButtons.Add(new ClickableTextureComponent("Hair", new Rectangle(xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + Game1.tileSize * 2 + IClickableMenu.borderWidth, (yPositionOnScreen + 16) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, Game1.tileSize, Game1.tileSize), "", "1", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 0.75f));

                        this.ArrowButtons.Add(new ClickableTextureComponent("Nose Type", new Rectangle((xPositionOnScreen + 16) + Game1.tileSize * 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth, (yPositionOnScreen + 16) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, Game1.tileSize, Game1.tileSize), "", "-1", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 0.75f));
                        this.SelectorLabels.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + Game1.tileSize * 5 + 8 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), "Nose Type"));
                        this.ArrowButtons.Add(new ClickableTextureComponent("Nose Type", new Rectangle(xPositionOnScreen + Game1.tileSize * 7 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth, (yPositionOnScreen + 16) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, Game1.tileSize, Game1.tileSize), "", "1", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 0.75f));

                        // shirt + bottoms
                        yOffset += Game1.tileSize + 8;
                        this.ArrowButtons.Add(new ClickableTextureComponent("Shirt", new Rectangle((xPositionOnScreen + 16) + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth, (yPositionOnScreen + 16) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, Game1.tileSize, Game1.tileSize), "", "-1", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 0.75f));
                        this.Labels.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize + 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), "Shirt"));
                        this.ArrowButtons.Add(new ClickableTextureComponent("Shirt", new Rectangle(xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + Game1.tileSize * 2 + IClickableMenu.borderWidth, (yPositionOnScreen + 16) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, Game1.tileSize, Game1.tileSize), "", "1", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 0.75f));

                        this.ArrowButtons.Add(new ClickableTextureComponent("Bottoms", new Rectangle((xPositionOnScreen + 16) + Game1.tileSize * 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth, (yPositionOnScreen + 16) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, Game1.tileSize, Game1.tileSize), "", "-1", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 0.75f));
                        this.SelectorLabels.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + Game1.tileSize * 5 + 8 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), " Bottoms"));
                        this.ArrowButtons.Add(new ClickableTextureComponent("Bottoms", new Rectangle(xPositionOnScreen + Game1.tileSize * 7 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth, (yPositionOnScreen + 16) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, Game1.tileSize, Game1.tileSize), "", "1", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 0.75f));

                        // accessory + shoes
                        yOffset += Game1.tileSize + 8;
                        this.ArrowButtons.Add(new ClickableTextureComponent("Acc", new Rectangle((xPositionOnScreen + 16) + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth, (yPositionOnScreen + 16) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, Game1.tileSize, Game1.tileSize), "", "-1", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 0.75f));
                        this.Labels.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize + 8, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), "Acc."));
                        this.ArrowButtons.Add(new ClickableTextureComponent("Acc", new Rectangle(xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + Game1.tileSize * 2 + IClickableMenu.borderWidth, (yPositionOnScreen + 16) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, Game1.tileSize, Game1.tileSize), "", "1", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 0.75f));

                        this.ArrowButtons.Add(new ClickableTextureComponent("Shoes", new Rectangle((xPositionOnScreen + 16) + Game1.tileSize * 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth, (yPositionOnScreen + 16) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, Game1.tileSize, Game1.tileSize), "", "-1", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 0.75f));
                        this.SelectorLabels.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + Game1.tileSize * 5 + 8 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), "  Shoes"));
                        this.ArrowButtons.Add(new ClickableTextureComponent("Shoes", new Rectangle(xPositionOnScreen + Game1.tileSize * 7 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth, (yPositionOnScreen + 16) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, Game1.tileSize, Game1.tileSize), "", "1", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 0.75f));
                    }

                    // text above quick favorite buttons
                    this.Labels.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + 288 + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 4 + 8, (yPositionOnScreen + 333) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 16, 1, 1), "Load"));
                    this.Labels.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + 271 + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 4 + 8, (yPositionOnScreen + 358) + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 16, 1, 1), "Favorite"));

                    // quick favorite star buttons
                    {
                        int xOffset = this.xPositionOnScreen + Game1.pixelZoom * 12;
                        int yOffset = this.yPositionOnScreen + Game1.tileSize + Game1.pixelZoom * 14;
                        int size = Game1.pixelZoom * 10;
                        int zoom = Game1.pixelZoom;
                        int y1 = this.HasFavSlot(1) ? 26 : 67;
                        int y2 = this.HasFavSlot(2) ? 26 : 67;
                        int y3 = this.HasFavSlot(3) ? 26 : 67;
                        int y4 = this.HasFavSlot(4) ? 26 : 67;
                        int y5 = this.HasFavSlot(5) ? 26 : 67;
                        int y6 = this.HasFavSlot(6) ? 26 : 67;
                        this.QuickLoadFavButtons = new[]
                        {
                            new ClickableTextureComponent(new Rectangle(xOffset + 565, yOffset + 425, size, size), menuTextures, new Rectangle(24, y1, 8, 8), zoom),
                            new ClickableTextureComponent(new Rectangle(xOffset + 565, yOffset + 475, size, size), menuTextures, new Rectangle(8, y2, 8, 8), zoom),
                            new ClickableTextureComponent(new Rectangle(xOffset + 565, yOffset + 525, size, size), menuTextures, new Rectangle(0, y3, 8, 8), zoom),
                            new ClickableTextureComponent(new Rectangle(xOffset + 610, yOffset + 425, size, size), menuTextures, new Rectangle(24, y4, 8, 8), zoom),
                            new ClickableTextureComponent(new Rectangle(xOffset + 610, yOffset + 475, size, size), menuTextures, new Rectangle(8, y5, 8, 8), zoom),
                            new ClickableTextureComponent(new Rectangle(xOffset + 610, yOffset + 525, size, size), menuTextures, new Rectangle(0, y6, 8, 8), zoom)
                        };
                    }
                }

                // 'manage favorites' star icons
                {
                    int xOffset = this.xPositionOnScreen + Game1.pixelZoom * 12;
                    int yOffset = this.yPositionOnScreen + Game1.tileSize + Game1.pixelZoom * 14;
                    int size = Game1.pixelZoom * 10;
                    const float zoom = 4.5f;
                    int y1 = this.HasFavSlot(1) ? 26 : 67;
                    int y2 = this.HasFavSlot(2) ? 26 : 67;
                    int y3 = this.HasFavSlot(3) ? 26 : 67;
                    int y4 = this.HasFavSlot(4) ? 26 : 67;
                    int y5 = this.HasFavSlot(5) ? 26 : 67;
                    int y6 = this.HasFavSlot(6) ? 26 : 67;
                    this.ManageFavIcons = new[]
                    {
                        new ClickableTextureComponent(new Rectangle(xOffset + 4, yOffset + 348, size, size), menuTextures, new Rectangle(24, y1, 8, 8), zoom),
                        new ClickableTextureComponent(new Rectangle(xOffset + 4, yOffset + 423, size, size), menuTextures, new Rectangle(8, y2, 8, 8), zoom),
                        new ClickableTextureComponent(new Rectangle(xOffset + 4, yOffset + 498, size, size), menuTextures, new Rectangle(0, y3, 8, 8), zoom),
                        new ClickableTextureComponent(new Rectangle(xOffset +380, yOffset + 348, size, size), menuTextures, new Rectangle(24, y4, 8, 8), zoom),
                        new ClickableTextureComponent(new Rectangle(xOffset +380, yOffset + 423, size, size), menuTextures, new Rectangle(8, y5, 8, 8), zoom),
                        new ClickableTextureComponent(new Rectangle(xOffset +380, yOffset + 498, size, size), menuTextures, new Rectangle(0, y6, 8, 8), zoom)
                    };
                }

                // 'manage favorites' set buttons
                {
                    int xOffset = this.xPositionOnScreen + Game1.pixelZoom * 12;
                    int yOffset = this.yPositionOnScreen + Game1.tileSize + Game1.pixelZoom * 14;
                    int xSize = Game1.pixelZoom * 15;
                    int ySize = Game1.pixelZoom * 10;
                    int zoom = 3;
                    this.SetFavButtons = new[]
                    {
                        new ClickableTextureComponent(new Rectangle(xOffset + 225, yOffset + 350, xSize, ySize), Game1.mouseCursors, new Rectangle(294, 428, 21, 11), zoom),
                        new ClickableTextureComponent(new Rectangle(xOffset + 225, yOffset + 425, xSize, ySize), Game1.mouseCursors, new Rectangle(294, 428, 21, 11), zoom),
                        new ClickableTextureComponent(new Rectangle(xOffset + 225, yOffset + 500, xSize, ySize), Game1.mouseCursors, new Rectangle(294, 428, 21, 11), zoom),

                        new ClickableTextureComponent(new Rectangle(xOffset + 595, yOffset + 350, xSize, ySize), Game1.mouseCursors, new Rectangle(294, 428, 21, 11), zoom),
                        new ClickableTextureComponent(new Rectangle(xOffset + 595, yOffset + 425, xSize, ySize), Game1.mouseCursors, new Rectangle(294, 428, 21, 11), zoom),
                        new ClickableTextureComponent(new Rectangle(xOffset + 595, yOffset + 500, xSize, ySize), Game1.mouseCursors, new Rectangle(294, 428, 21, 11), zoom)
                    };
                }

                // 'manage favorites' extra outfits buttons
                {
                    this.LoadFavButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 475 + Game1.pixelZoom * 12, this.yPositionOnScreen + 405 + Game1.tileSize + Game1.pixelZoom * 14, Game1.pixelZoom * 20, Game1.pixelZoom * 10), menuTextures, new Rectangle(0, 207, 26, 11), 3f);
                    this.SaveFavButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 475 + Game1.pixelZoom * 12, this.yPositionOnScreen + 455 + Game1.tileSize + Game1.pixelZoom * 14, Game1.pixelZoom * 20, Game1.pixelZoom * 10), menuTextures, new Rectangle(0, 193, 26, 11), 3f);

                    int xOffset = this.xPositionOnScreen + 80 + Game1.pixelZoom * 12;
                    int yOffset = this.yPositionOnScreen + Game1.tileSize + Game1.pixelZoom * 14;
                    int size = Game1.pixelZoom * 10;
                    const float zoom = 4.5f;
                    for (int i = 0; i < 10; i++)
                    {
                        int y = this.HasFavSlot(i + 7) ? 26 : 67;
                        this.ExtraFavButtons.Add(new ClickableTextureComponent(new Rectangle(xOffset + i * 50, yOffset + 520, size, size), menuTextures, new Rectangle(0, y, 8, 8), zoom));
                    }
                    for (int i = 0; i < 10; i++)
                    {
                        int y = this.HasFavSlot(i + 17) ? 26 : 67;
                        this.ExtraFavButtons.Add(new ClickableTextureComponent(new Rectangle(xOffset + i * 50, yOffset + 570, size, size), menuTextures, new Rectangle(0, y, 8, 8), zoom));
                    }
                    for (int i = 0; i < 10; i++)
                    {
                        int y = this.HasFavSlot(i + 27) ? 26 : 67;
                        this.ExtraFavButtons.Add(new ClickableTextureComponent(new Rectangle(xOffset + i * 50, yOffset + 620, size, size), menuTextures, new Rectangle(0, y, 8, 8), zoom));
                    }
                }

                // about menu
                this.SetAccessKeyButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 595 + Game1.pixelZoom * 12, this.yPositionOnScreen + 400 + Game1.tileSize + Game1.pixelZoom * 14, Game1.pixelZoom * 15, Game1.pixelZoom * 10), Game1.mouseCursors, new Rectangle(294, 428, 21, 11), 3f);
                this.ToggleMaleSkirtsButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 595 + Game1.pixelZoom * 12, this.yPositionOnScreen + 475 + Game1.tileSize + Game1.pixelZoom * 14, Game1.pixelZoom * 15, Game1.pixelZoom * 10), Game1.mouseCursors, new Rectangle(294, 428, 21, 11), 3f);
                this.ZoomOutButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 595 + Game1.pixelZoom * 12, this.yPositionOnScreen + 550 + Game1.tileSize + Game1.pixelZoom * 14, Game1.pixelZoom * 10, Game1.pixelZoom * 10), menuTextures, new Rectangle(0, this.GlobalConfig.MenuZoomOut ? 177 : 167, 7, 9), 3f);
                this.ZoomInButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 636 + Game1.pixelZoom * 12, this.yPositionOnScreen + 550 + Game1.tileSize + Game1.pixelZoom * 14, Game1.pixelZoom * 10, Game1.pixelZoom * 10), menuTextures, new Rectangle(10, this.GlobalConfig.MenuZoomOut ? 167 : 177, 7, 9), 3f);
                this.ResetConfigButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 595 + Game1.pixelZoom * 12, this.yPositionOnScreen + 625 + Game1.tileSize + Game1.pixelZoom * 14, Game1.pixelZoom * 15, Game1.pixelZoom * 10), Game1.mouseCursors, new Rectangle(294, 428, 21, 11), 3f);
            }

            // 
            this.UpdateTabFloaters();
            this.UpdateTabPositions();
        }

        /// <summary>Reinitialise components which bring attention to tabs.</summary>
        private void UpdateTabFloaters()
        {
            this.FloatingNew = new TemporaryAnimatedSprite(this.ContentHelper.MenuTextures, new Rectangle(0, 102, 23, 9), 115, 5, 1, new Vector2(xPositionOnScreen - 90, yPositionOnScreen + 35), false, false, 0.89f, 0f, Color.White, Game1.pixelZoom, 0f, 0f, 0f, true)
            {
                totalNumberOfLoops = 1,
                yPeriodic = true,
                yPeriodicLoopTime = 1500f,
                yPeriodicRange = Game1.tileSize / 8f
            };

            this.FavTabArrow = new TemporaryAnimatedSprite(this.ContentHelper.MenuTextures, new Rectangle(0, 120, 12, 14), 100f, 3, 5, new Vector2(xPositionOnScreen + 120, yPositionOnScreen - 38), false, false, 0.89f, 0f, Color.White, 3f, 0f, 0f, 0f, true)
            {
                yPeriodic = true,
                yPeriodicLoopTime = 1500f,
                yPeriodicRange = Game1.tileSize / 8f
            };
        }

        /// <summary>Recalculate the positions for all tabs and subtabs.</summary>
        private void UpdateTabPositions()
        {
            this.CharTabIcon.bounds.Y = this.yPositionOnScreen + (this.CurrentTab == MenuTab.Customise ? 50 : 40);
            this.FavIcon.bounds.Y = this.yPositionOnScreen + (this.CurrentTab == MenuTab.Favorites || this.CurrentTab == MenuTab.FavoritesExtras ? 50 : 40);
            this.AboutIcon.bounds.Y = this.yPositionOnScreen + (this.CurrentTab == MenuTab.About ? 43 : 33);
            this.QuickFavsIcon.bounds.X = this.xPositionOnScreen - (this.CurrentTab == MenuTab.Favorites ? 16 : 23);
            this.ExtraFavsIcon.bounds.X = this.xPositionOnScreen - (this.CurrentTab == MenuTab.FavoritesExtras ? 16 : 23);
        }

        /// <summary>Switch to the given tab.</summary>
        /// <param name="tab">The tab to display.</param>
        private void SetTab(MenuTab tab)
        {
            if (this.CurrentTab == tab)
                return;

            switch (tab)
            {
                // main customisation tab
                case MenuTab.Customise:
                    Game1.playSound("smallSelect");
                    this.CurrentTab = MenuTab.Customise;
                    this.UpdateTabPositions();
                    break;

                // main favorites tab
                case MenuTab.Favorites:
                    Game1.playSound("smallSelect");
                    this.CurrentTab = MenuTab.Favorites;
                    this.UpdateTabPositions();
                    this.ShowFavTabArrow = false;
                    break;

                case MenuTab.About:
                    Game1.playSound("smallSelect");
                    this.CurrentTab = MenuTab.About;
                    this.UpdateTabPositions();
                    break;

                case MenuTab.FavoritesExtras:
                    Game1.playSound("smallSelect");
                    this.CurrentTab = MenuTab.FavoritesExtras;
                    this.UpdateTabPositions();
                    this.CurrentFav = -1;
                    foreach (ClickableTextureComponent extraFavButton in this.ExtraFavButtons)
                        extraFavButton.drawShadow = false;
                    return;
            }
        }

        /// <summary>Exit the menu.</summary>
        /// <param name="playSound">Whether to play the default exit sound.</param>
        private void ExitMenu(bool playSound = true)
        {
            Game1.exitActiveMenu();

            if (playSound)
                Game1.playSound("bigDeSelect");

            Game1.options.zoomLevel = this.PlayerZoomLevel;
            Game1.overrideGameMenuReset = true;
            Game1.game1.refreshWindowSettings();
            Game1.player.canMove = true;
        }

        /// <summary>Randomise the character attributes.</summary>
        private void RandomiseCharacter()
        {
            // play sound
            string cueName = "drumkit6";
            if (this.TimesRandomised > 0)
            {
                switch (Game1.random.Next(15))
                {
                    case 0:
                        cueName = "drumkit1";
                        break;
                    case 1:
                        cueName = "dirtyHit";
                        break;
                    case 2:
                        cueName = "axchop";
                        break;
                    case 3:
                        cueName = "hoeHit";
                        break;
                    case 4:
                        cueName = "fishSlap";
                        break;
                    case 5:
                        cueName = "drumkit6";
                        break;
                    case 6:
                        cueName = "drumkit5";
                        break;
                    case 7:
                        cueName = "drumkit6";
                        break;
                    case 8:
                        cueName = "junimoMeep1";
                        break;
                    case 9:
                        cueName = "coin";
                        break;
                    case 10:
                        cueName = "axe";
                        break;
                    case 11:
                        cueName = "hammer";
                        break;
                    case 12:
                        cueName = "drumkit2";
                        break;
                    case 13:
                        cueName = "drumkit4";
                        break;
                    case 14:
                        cueName = "drumkit3";
                        break;
                }
            }
            Game1.playSound(cueName);
            this.TimesRandomised++;

            // randomise base
            if (Game1.player.isMale)
            {
                Game1.player.changeHairStyle(Game1.random.Next(16));
                this.ChangeBottoms(Game1.random.Next(this.GlobalConfig.MaleBottomsTypes), true);
                this.ChangeShoes(Game1.random.Next(this.GlobalConfig.MaleShoeTypes), true);
                this.ChangeFace(Game1.random.Next(this.GlobalConfig.MaleFaceTypes), true);
                this.ChangeNose(Game1.random.Next(this.GlobalConfig.MaleNoseTypes), true);
            }
            else
            {
                Game1.player.changeHairStyle(Game1.random.Next(16, 32));
                this.ChangeBottoms(Game1.random.Next(this.GlobalConfig.FemaleBottomsTypes), true);
                this.ChangeShoes(Game1.random.Next(this.GlobalConfig.FemaleShoeTypes), true);
                this.ChangeFace(Game1.random.Next(this.GlobalConfig.FemaleFaceTypes), true);
                this.ChangeNose(Game1.random.Next(this.GlobalConfig.FemaleNoseTypes), true);
            }
            this.PatchBaseTexture();

            // randomise base again?
            if (Game1.player.isMale)
            {
                Game1.player.changeHairStyle(Game1.random.Next(16));
                this.ChangeBottoms(Game1.random.Next(this.GlobalConfig.HideMaleSkirts ? 2 : this.GlobalConfig.MaleBottomsTypes));
                this.ChangeShoes(Game1.random.Next(this.GlobalConfig.MaleShoeTypes));
                this.ChangeFace(Game1.random.Next(this.GlobalConfig.MaleFaceTypes));
                this.ChangeNose(Game1.random.Next(this.GlobalConfig.MaleNoseTypes));
            }
            else
            {
                Game1.player.changeHairStyle(Game1.random.Next(16, 32));
                this.ChangeBottoms(Game1.random.Next(this.GlobalConfig.FemaleBottomsTypes));
                this.ChangeShoes(Game1.random.Next(this.GlobalConfig.FemaleShoeTypes));
                this.ChangeFace(Game1.random.Next(this.GlobalConfig.FemaleFaceTypes));
                this.ChangeNose(Game1.random.Next(this.GlobalConfig.FemaleNoseTypes));
            }

            // randomise accessories
            if (Game1.random.NextDouble() < 0.88)
            {
                int maxRange = (this.FaceType == 0) ? 127 : 131;
                int random = Game1.random.Next(maxRange);
                if (random > 19 && maxRange > 127) random += 108;

                if (Game1.player.isMale)
                    this.ChangeAccessory(random);
                else
                {
                    random = Game1.random.Next(6, maxRange);
                    if (random > 19 && maxRange > 127) random += 108;
                    this.ChangeAccessory(random);
                }
            }
            else
                Game1.player.changeAccessory(-1);

            // randomise hair style
            Game1.player.changeHairStyle(Game1.player.isMale ? Game1.random.Next(16) : Game1.random.Next(16, 32));

            // randomise hair color
            Color c = new Color(Game1.random.Next(25, 254), Game1.random.Next(25, 254), Game1.random.Next(25, 254));
            if (Game1.random.NextDouble() < 0.5)
            {
                c.R /= 2;
                c.G /= 2;
                c.B /= 2;
            }
            if (Game1.random.NextDouble() < 0.5)
                c.R = (byte)Game1.random.Next(15, 50);
            if (Game1.random.NextDouble() < 0.5)
                c.G = (byte)Game1.random.Next(15, 50);
            if (Game1.random.NextDouble() < 0.5)
                c.B = (byte)Game1.random.Next(15, 50);
            Game1.player.changeHairColor(c);

            // randomise shirt
            Game1.player.changeShirt(Game1.random.Next(112));

            // randomise skin color
            Game1.player.changeSkinColor(Game1.random.Next(6));
            if (Game1.random.NextDouble() < 0.25)
                Game1.player.changeSkinColor(Game1.random.Next(24));

            // randomise pants color
            Color color = new Color(Game1.random.Next(25, 254), Game1.random.Next(25, 254), Game1.random.Next(25, 254));
            if (Game1.random.NextDouble() < 0.5)
            {
                color.R /= 2;
                color.G /= 2;
                color.B /= 2;
            }
            if (Game1.random.NextDouble() < 0.5)
                color.R = (byte)Game1.random.Next(15, 50);
            if (Game1.random.NextDouble() < 0.5)
                color.G = (byte)Game1.random.Next(15, 50);
            if (Game1.random.NextDouble() < 0.5)
                color.B = (byte)Game1.random.Next(15, 50);
            Game1.player.changePants(color);

            // randomise eye color
            Color c2 = new Color(Game1.random.Next(25, 254), Game1.random.Next(25, 254), Game1.random.Next(25, 254));
            c2.R /= 2;
            c2.G /= 2;
            c2.B /= 2;
            if (Game1.random.NextDouble() < 0.5)
                c2.R = (byte)Game1.random.Next(15, 50);
            if (Game1.random.NextDouble() < 0.5)
                c2.G = (byte)Game1.random.Next(15, 50);
            if (Game1.random.NextDouble() < 0.5)
                c2.B = (byte)Game1.random.Next(15, 50);
            Game1.player.changeEyeColor(c2);
        }

        /// <summary>Load the specified favorite.</summary>
        /// <param name="index">The index of the favorite to load.</param>
        /// <returns>Returns whether the favorite exists and was loaded.</returns>
        private bool LoadFavorite(int index)
        {
            if (!this.HasFavSlot(index))
                return false;

            this.ChangeBottoms(this.PlayerConfig.ChosenBottoms[index]);
            this.ChangeAccessory(this.PlayerConfig.ChosenAccessory[index]);
            this.ChangeFace(this.PlayerConfig.ChosenFace[index]);
            this.ChangeNose(this.PlayerConfig.ChosenNose[index]);
            this.ChangeShoes(this.PlayerConfig.ChosenShoes[index]);
            Game1.player.changeSkinColor(this.PlayerConfig.ChosenSkin[index]);
            Game1.player.changeShirt(this.PlayerConfig.ChosenShirt[index]);
            Game1.player.changeHairStyle(this.PlayerConfig.ChosenHairstyle[index]);

            Color haircolorpackedvalue = new Color(0, 0, 0) { PackedValue = this.PlayerConfig.ChosenHairColor[index] };
            Game1.player.changeHairColor(haircolorpackedvalue);
            this.HairColorPicker.setColor(Game1.player.hairstyleColor);

            Color eyecolorpackedvalue = new Color(0, 0, 0) { PackedValue = this.PlayerConfig.ChosenEyeColor[index] };
            Game1.player.changeEyeColor(eyecolorpackedvalue);
            this.EyeColorPicker.setColor(Game1.player.newEyeColor);

            Color bottomscolorpackedvalue = new Color(0, 0, 0) { PackedValue = this.PlayerConfig.ChosenBottomsColor[index] };
            Game1.player.changePants(bottomscolorpackedvalue);
            this.PantsColorPicker.setColor(Game1.player.pantsColor);

            return true;
        }

        /// <summary>Set the current values to the given favorite.</summary>
        /// <param name="index">The favorite to set.</param>
        private void SetFavorite(int index)
        {
            this.PlayerConfig.ChosenBottoms[index] = this.Bottoms;
            this.PlayerConfig.ChosenAccessory[index] = Game1.player.accessory;
            this.PlayerConfig.ChosenFace[index] = this.FaceType;
            this.PlayerConfig.ChosenNose[index] = this.NoseType;
            this.PlayerConfig.ChosenShoes[index] = this.Shoes;
            this.PlayerConfig.ChosenSkin[index] = Game1.player.skin;
            this.PlayerConfig.ChosenShirt[index] = Game1.player.shirt;
            this.PlayerConfig.ChosenHairstyle[index] = Game1.player.hair;
            this.PlayerConfig.ChosenHairColor[index] = Game1.player.hairstyleColor.PackedValue;
            this.PlayerConfig.ChosenEyeColor[index] = Game1.player.newEyeColor.PackedValue;
            this.PlayerConfig.ChosenBottomsColor[index] = Game1.player.pantsColor.PackedValue;
        }

        /// <summary>Perform the action associated with a button.</summary>
        /// <param name="name">The button name.</param>
        private void HandleButtonAction(string name)
        {
            switch (name)
            {
                case "Male":
                    this.ChangeGender(true);
                    Game1.player.changeHairStyle(0);
                    break;

                case "Female":
                    this.ChangeGender(false);
                    Game1.player.changeHairStyle(16);
                    break;

                case "OK":
                    this.ExitMenu(playSound: false);

                    Game1.exitActiveMenu();
                    Game1.flashAlpha = 1f;

                    Game1.playSound("yoba");
                    this.PlayerConfig.ChosenAccessory[0] = Game1.player.accessory;
                    this.PlayerConfig.ChosenFace[0] = this.FaceType;
                    this.PlayerConfig.ChosenNose[0] = this.NoseType;
                    this.PlayerConfig.ChosenBottoms[0] = this.Bottoms;
                    this.PlayerConfig.ChosenShoes[0] = this.Shoes;
                    this.ModHelper.WriteJsonFile(ModConstants.PerSaveConfigPath, this.PlayerConfig);
                    break;
            }
            Game1.playSound("coin");
        }

        /// <summary>Perform the action associated with a selector.</summary>
        /// <param name="name">The selector name.</param>
        /// <param name="change">The change value.</param>
        private void HandleSelectorChange(string name, int change)
        {
            switch (name)
            {
                case "Skin":
                    Game1.player.changeSkinColor(Game1.player.skin + change);
                    Game1.playSound("skeletonStep");
                    return;

                case "Hair":
                    Game1.player.changeHairStyle(Game1.player.hair + change);
                    Game1.playSound("grassyStep");
                    return;

                case "Shirt":
                    Game1.player.changeShirt(Game1.player.shirt + change);
                    Game1.playSound("coin");
                    return;

                case "Acc":
                    this.ChangeAccessory(Game1.player.accessory + change);
                    Game1.playSound("purchase");
                    return;

                case "Face Type":
                    this.ChangeFace(this.FaceType + change);

                    if (this.FaceType == 0 && Game1.player.accessory > 127)
                        Game1.player.accessory = Game1.player.accessory - 112;
                    if (this.FaceType == 1 && Game1.player.accessory < 131 && Game1.player.accessory > 18)
                        Game1.player.accessory = Game1.player.accessory + 112;

                    Game1.playSound("skeletonStep");
                    return;

                case "Nose Type":
                    this.ChangeNose(this.NoseType + change);
                    Game1.playSound("grassyStep");
                    return;

                case "Bottoms":
                    this.ChangeBottoms(this.Bottoms + change);
                    Game1.playSound("coin");
                    return;

                case "Shoes":
                    this.ChangeShoes(this.Shoes + change);
                    Game1.playSound("purchase");
                    return;

                case "Direction":
                    Game1.player.faceDirection((Game1.player.facingDirection - change + 4) % 4);
                    Game1.player.FarmerSprite.StopAnimation();
                    Game1.player.completelyStopAnimatingOrDoingAction();
                    Game1.playSound("pickUpItem");
                    break;

                case "Direction2":
                    Game1.player.faceDirection((Game1.player.facingDirection - change + 4) % 4);
                    Game1.player.FarmerSprite.StopAnimation();
                    Game1.player.completelyStopAnimatingOrDoingAction();
                    Game1.playSound("pickUpItem");
                    break;
            }
        }

        /// <summary>Switch the player to the given accessory.</summary>
        /// <param name="index">The accessory index.</param>
        private void ChangeAccessory(int index)
        {
            if (index < -1)
                index = this.FaceType == 0 ? 127 : 239;

            if (this.FaceType != 0)
            {
                if (index == 19)
                    index = 131;
                if (index == 130)
                    index = 18;
            }
            if ((index >= 128 && this.FaceType == 0) || index >= 240)
                index = -1;

            Game1.player.accessory = index;
        }

        /// <summary>Switch the player to the given face.</summary>
        /// <param name="index">The face index.</param>
        /// <param name="noPatch">Whether to skip patching the farmer textures.</param>
        private void ChangeFace(int index, bool noPatch = false)
        {
            if (index < 0)
                index = (Game1.player.isMale ? this.GlobalConfig.MaleFaceTypes : this.GlobalConfig.FemaleFaceTypes) - 1;
            if (index >= (Game1.player.isMale ? this.GlobalConfig.MaleFaceTypes : this.GlobalConfig.FemaleFaceTypes))
                index = 0;

            this.FaceType = index;
            if (!noPatch)
                this.PatchBaseTexture();
        }

        /// <summary>Switch the player to the given nose.</summary>
        /// <param name="index">The nose index.</param>
        /// <param name="noPatch">Whether to skip patching the farmer textures.</param>
        private void ChangeNose(int index, bool noPatch = false)
        {
            if (index < 0)
                index = (Game1.player.isMale ? this.GlobalConfig.MaleNoseTypes : this.GlobalConfig.FemaleNoseTypes) - 1;
            if (index >= (Game1.player.isMale ? this.GlobalConfig.MaleNoseTypes : this.GlobalConfig.FemaleNoseTypes))
                index = 0;

            this.NoseType = index;
            if (!noPatch)
                this.PatchBaseTexture();
        }

        /// <summary>Switch the player to the given bottom.</summary>
        /// <param name="index">The bottom index.</param>
        /// <param name="noPatch">Whether to skip patching the farmer textures.</param>
        private void ChangeBottoms(int index, bool noPatch = false)
        {
            if (index < 0)
                index = (Game1.player.isMale ? (this.GlobalConfig.HideMaleSkirts ? 2 : this.GlobalConfig.MaleBottomsTypes) : this.GlobalConfig.FemaleBottomsTypes) - 1;
            if (index >= (Game1.player.isMale ? (this.GlobalConfig.HideMaleSkirts ? 2 : this.GlobalConfig.MaleBottomsTypes) : this.GlobalConfig.FemaleBottomsTypes))
                index = 0;

            this.Bottoms = index;
            if (!noPatch)
                this.PatchBaseTexture();
        }

        /// <summary>Switch the player to the given shoes.</summary>
        /// <param name="index">The shoes index.</param>
        /// <param name="noPatch">Whether to skip patching the farmer textures.</param>
        private void ChangeShoes(int index, bool noPatch = false)
        {
            if (index < 0)
                index = (Game1.player.isMale ? this.GlobalConfig.MaleShoeTypes : this.GlobalConfig.FemaleShoeTypes) - 1;
            if (index >= (Game1.player.isMale ? this.GlobalConfig.MaleShoeTypes : this.GlobalConfig.FemaleShoeTypes))
                index = 0;

            this.Shoes = index;
            if (!noPatch)
                this.PatchBaseTexture();
        }

        /// <summary>Switch the player to the given gender.</summary>
        /// <param name="male">Male if <c>true</c>, else female.</param>
        private void ChangeGender(bool male)
        {
            Game1.player.isMale = male;
            Game1.player.FarmerRenderer.baseTexture = this.ContentHelper.GetBaseFarmerTexture(male);
            Game1.player.FarmerRenderer.heightOffset = male ? 0 : 4;

            this.FaceType = this.NoseType = this.Bottoms = this.Shoes = 0;
            this.ContentHelper.FixFarmerEffects(Game1.player);
        }

        /// <summary>Overwrite the farmer textures to inject the current face and bottom type.</summary>
        private void PatchBaseTexture()
        {
            Game1.player.FarmerRenderer.baseTexture = this.ContentHelper.GetBaseFarmerTexture(Game1.player.isMale);
            string texturePath = Game1.player.isMale ? "male" : "female";

            this.ContentHelper.PatchTexture(ref Game1.player.FarmerRenderer.baseTexture, $"{texturePath}_face{this.FaceType}_nose{this.NoseType}.png", 0, 0);
            for (int i = 0; i < ModConstants.MaleShoeSpriteHeights.Length; i++)
                this.ContentHelper.PatchTexture(ref Game1.player.FarmerRenderer.baseTexture, $"{texturePath}_shoes{this.Shoes}.png", 1 * i, (1 * i) * 4, 96, 32, Game1.player.isMale ? ModConstants.MaleShoeSpriteHeights[i] : ModConstants.FemaleShoeSpriteHeights[i]);
            this.ContentHelper.PatchTexture(ref Game1.player.FarmerRenderer.baseTexture, texturePath + "_bottoms.png", this.Bottoms, 3);
            this.ContentHelper.FixFarmerEffects(Game1.player);
        }

        /// <summary>Get whether a favorite is defined.</summary>
        /// <param name="favoriteSlot">The favorite index to check.</param>
        private bool HasFavSlot(int favoriteSlot)
        {
            if (favoriteSlot < 0 || favoriteSlot > this.PlayerConfig.ChosenEyeColor.Length)
                return false;
            return this.PlayerConfig.ChosenEyeColor[favoriteSlot] != 0;
        }
    }
}
