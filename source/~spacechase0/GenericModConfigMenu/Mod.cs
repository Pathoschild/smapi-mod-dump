/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using GenericModConfigMenu.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SpaceShared;
using SpaceShared.UI;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Menus;


namespace GenericModConfigMenu
{

    internal class Mod : StardewModdingAPI.Mod
    {
        public static Mod instance;

        /*********
        ** Fields
        *********/
        private OwnModConfig Config;
        private RootElement? Ui;
        private Button ConfigButton;

        private int countdown = 5;

        /// <summary>Manages registered mod config menus.</summary>
        internal readonly ModConfigManager ConfigManager = new();

        /*********
        ** Accessors
        *********/
        /// <summary>The current configuration menu.</summary>
        public static IClickableMenu ActiveConfigMenu
        {
            get
            {
                IClickableMenu menu = Game1.activeClickableMenu is TitleMenu ? TitleMenu.subMenu : Game1.activeClickableMenu;
                return menu is ModConfigMenu or SpecificModConfigMenu
                    ? menu
                    : null;
            }
            set
            {
                if (Game1.activeClickableMenu is TitleMenu)
                    TitleMenu.subMenu = value;
                else
                    Game1.activeClickableMenu = value;
            }
        }


        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Entry(IModHelper helper)
        {
            instance = this;
            I18n.Init(helper.Translation);
            Log.Monitor = this.Monitor;
            this.Config = helper.ReadConfig<OwnModConfig>();

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.UpdateTicking += this.OnUpdateTicking;
            helper.Events.Display.WindowResized += this.OnWindowResized;
            helper.Events.Display.Rendered += this.OnRendered;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            helper.Events.Input.MouseWheelScrolled += this.OnMouseWheelScrolled;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.Input.ButtonsChanged += this.OnButtonChanged;

            helper.Events.Content.AssetRequested += static (_, e) => AssetManager.Apply(e);
        }

        /// <inheritdoc />
        public override object GetApi(IModInfo mod)
        {
            return new Api(mod.Manifest, this.ConfigManager, mod => this.OpenModMenu(mod, page: null, listScrollRow: null), (s) => LogDeprecated( mod.Manifest.UniqueID, s));
        }


        /*********
        ** Private methods
        *********/
        private static HashSet<string> DidDeprecationWarningsFor = new();
        private void LogDeprecated(string modid, string str)
        {
            if (DidDeprecationWarningsFor.Contains(modid))
                return;
            DidDeprecationWarningsFor.Add(modid);
            Log.Info(str);
        }

        /// <summary>Open the menu which shows a list of configurable mods.</summary>
        /// <param name="scrollRow">The initial scroll position, represented by the row index at the top of the visible area.</param>
        private void OpenListMenu(int? scrollRow = null)
        {
            Mod.ActiveConfigMenu = new ModConfigMenu(this.Config.ScrollSpeed, openModMenu: (mod, curScrollRow) => this.OpenModMenu(mod, page: null, listScrollRow: curScrollRow), openKeybindingsMenu: currScrollRow => OpenKeybindingsMenu( currScrollRow ), this.ConfigManager, this.Helper.GameContent.Load<Texture2D>(AssetManager.KeyboardButton), scrollRow);
        }

        private void OpenKeybindingsMenu(int listScrollRow)
        {
            Mod.ActiveConfigMenu = new SpecificModConfigMenu(
                mods: this.ConfigManager,
                scrollSpeed: this.Config.ScrollSpeed,
                returnToList: () => this.OpenListMenu(listScrollRow)
            );
        }

        /// <summary>Open the config UI for a specific mod.</summary>
        /// <param name="mod">The mod whose config menu to display.</param>
        /// <param name="page">The page to display within the mod's config menu.</param>
        /// <param name="listScrollRow">The scroll position to set in the mod list when returning to it, represented by the row index at the top of the visible area.</param>
        private void OpenModMenu(IManifest mod, string page, int? listScrollRow)
        {
            ModConfig config = this.ConfigManager.Get(mod, assert: true);

            Mod.ActiveConfigMenu = new SpecificModConfigMenu(
                config: config,
                scrollSpeed: this.Config.ScrollSpeed,
                page: page,
                openPage: newPage => this.OpenModMenu(mod, newPage, listScrollRow),
                returnToList: () => this.OpenListMenu(listScrollRow)
            );
        }

        private void SetupTitleMenuButton()
        {
            if (this.Ui == null)
            {
                this.Ui = new RootElement();

                Texture2D tex = this.Helper.GameContent.Load<Texture2D>(AssetManager.ConfigButton);
                this.ConfigButton = new Button(tex)
                {
                    LocalPosition = new Vector2(36, Game1.viewport.Height - 100),
                    Callback = _ =>
                    {
                        Game1.playSound("newArtifact");
                        this.OpenListMenu();
                    }
                };

                this.Ui.AddChild(this.ConfigButton);
            }

            if (Game1.activeClickableMenu is TitleMenu tm && tm.allClickableComponents?.Find( (cc) => cc?.myID == 509800 ) == null )
            {
                // Gamepad support
                Texture2D tex = this.Helper.GameContent.Load<Texture2D>(AssetManager.ConfigButton);
                ClickableComponent button = new(new(0, Game1.viewport.Height - 100, tex.Width / 2, tex.Height / 2), "GMCM") // Why /2? Who knows
                {
                    myID = 509800,
                    rightNeighborID = tm.buttons[0].myID,
                };
                tm.allClickableComponents?.Add(button);
                tm.buttons[0].leftNeighborID = 509800;
            }
        }

        private bool IsTitleMenuInteractable()
        {
            if (Game1.activeClickableMenu is not TitleMenu titleMenu || TitleMenu.subMenu != null)
                return false;

            var method = this.Helper.Reflection.GetMethod(titleMenu, "ShouldAllowInteraction", false);
            if (method != null)
                return method.Invoke<bool>();
            else // method isn't available on Android
                return this.Helper.Reflection.GetField<bool>(titleMenu, "titleInPosition").GetValue();
        }

        /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // delay for long enough that CP can get a chance to edit
            // the texture.
            this.Helper.Events.GameLoop.UpdateTicking += this.FiveTicksAfterGameLaunched;

            Api configMenu = new Api(ModManifest, this.ConfigManager, mod => this.OpenModMenu(mod, page: null, listScrollRow: null), (s) => LogDeprecated( ModManifest.UniqueID, s));

            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new OwnModConfig(),
                save: () => this.Helper.WriteConfig(this.Config),
                titleScreenOnly: false
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: I18n.Options_ScrollSpeed_Name,
                tooltip: I18n.Options_ScrollSpeed_Desc,
                getValue: () => this.Config.ScrollSpeed,
                setValue: value => this.Config.ScrollSpeed = value,
                min: 1,
                max: 500,
                formatValue: null
            );

            configMenu.AddKeybindList(
                mod: this.ModManifest,
                name: I18n.Options_OpenMenuKey_Name,
                tooltip: I18n.Options_OpenMenuKey_Desc,
                getValue: () => this.Config.OpenMenuKey,
                setValue: value => this.Config.OpenMenuKey = value
            );
        }

        private void FiveTicksAfterGameLaunched(object sender, UpdateTickingEventArgs e)
        {
            if (this.countdown-- < 0)
            {
                this.SetupTitleMenuButton();
                this.Helper.Events.GameLoop.UpdateTicking -= this.FiveTicksAfterGameLaunched;
            }
        }

        private bool wasConfigMenu = false;

        /// <inheritdoc cref="IGameLoopEvents.UpdateTicking"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicking(object sender, UpdateTickingEventArgs e)
        {
            if (this.IsTitleMenuInteractable())
            {
                SetupTitleMenuButton();
                this.Ui?.Update();
            }

            if (wasConfigMenu && TitleMenu.subMenu == null)
            {
                var f = Helper.Reflection.GetField<bool>(Game1.activeClickableMenu, "titleInPosition");
                if (!f.GetValue())
                    f.SetValue(true);
            }
            wasConfigMenu = TitleMenu.subMenu is ModConfigMenu || TitleMenu.subMenu is SpecificModConfigMenu;
        }

        /// <inheritdoc cref="IDisplayEvents.WindowResized"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnWindowResized(object sender, WindowResizedEventArgs e)
        {
            if ( this.ConfigButton != null )
                this.ConfigButton.LocalPosition = new Vector2(this.ConfigButton.Position.X, Game1.viewport.Height - 100);
        }

        /// <inheritdoc cref="IDisplayEvents.Rendered"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnRendered(object sender, RenderedEventArgs e)
        {
            if (this.IsTitleMenuInteractable())
                this.Ui?.Draw(e.SpriteBatch);
        }

        /// <inheritdoc cref="IDisplayEvents.MenuChanged"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is GameMenu menu)
            {
                OptionsPage page = (OptionsPage)menu.pages[GameMenu.optionsTab];
                page.options.Add(new OptionsButton(I18n.Button_ModOptions(), () => this.OpenListMenu()));
            }
        }

        /// <inheritdoc cref="IInputEvents.ButtonPressed"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // open menu
            if (Context.IsPlayerFree && this.Config.OpenMenuKey.JustPressed())
                this.OpenListMenu();

            // pass input to menu
            else if (Mod.ActiveConfigMenu is SpecificModConfigMenu menu && e.Button.TryGetKeyboard(out Keys key))
                menu.receiveKeyPress(key);
        }

        /// <inheritdoc cref="IInputEvents.ButtonPressed"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonChanged(object sender, ButtonsChangedEventArgs e)
        {
            // pass to menu for keybinding
            if (Mod.ActiveConfigMenu is SpecificModConfigMenu menu)
                menu.OnButtonsChanged(e);
        }

        /// <inheritdoc cref="IInputEvents.MouseWheelScrolled"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMouseWheelScrolled(object sender, MouseWheelScrolledEventArgs e)
        {
            Dropdown.ActiveDropdown?.ReceiveScrollWheelAction(e.Delta);
        }
    }
}
