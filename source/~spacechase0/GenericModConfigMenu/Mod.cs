/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

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
        /*********
        ** Fields
        *********/
        private OwnModConfig Config;
        private RootElement Ui;
        private Button ConfigButton;

        /// <summary>Manages registered mod config menus.</summary>
        private readonly ModConfigManager ConfigManager = new();

        /// <summary>The mod API, if initialized.</summary>
        private Api Api;


        /*********
        ** Accessors
        *********/
        public static Mod Instance;

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
            I18n.Init(helper.Translation);
            Mod.Instance = this;
            Log.Monitor = this.Monitor;
            this.Config = helper.ReadConfig<OwnModConfig>();

            this.SetupTitleMenuButton();

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.UpdateTicking += this.OnUpdateTicking;
            helper.Events.Display.WindowResized += this.OnWindowResized;
            helper.Events.Display.Rendered += this.OnRendered;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            helper.Events.Input.MouseWheelScrolled += this.OnMouseWheelScrolled;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        /// <inheritdoc />
        public override object GetApi()
        {
            return this.Api ??= new Api(this.ConfigManager);
        }

        /// <summary>Open the menu which shows a list of configurable mods.</summary>
        public void OpenListMenu()
        {
            Mod.ActiveConfigMenu = new ModConfigMenu(this.Config.ScrollSpeed, openModMenu: mod => this.OpenModMenu(mod), this.ConfigManager);
        }

        /// <summary>Open the config UI for a specific mod.</summary>
        /// <param name="mod">The mod whose config menu to display.</param>
        /// <param name="page">The page to display within the mod's config menu.</param>
        public void OpenModMenu(IManifest mod, string page = null)
        {
            ModConfig config = this.ConfigManager.Get(mod, assert: true);

            Mod.ActiveConfigMenu = new SpecificModConfigMenu(
                config: config,
                scrollSpeed: this.Config.ScrollSpeed,
                page: page,
                openPage: newPage => this.OpenModMenu(mod, newPage),
                returnToList: this.OpenListMenu
            );
        }


        /*********
        ** Private methods
        *********/
        private void SetupTitleMenuButton()
        {
            this.Ui = new RootElement();

            Texture2D tex = this.Helper.Content.Load<Texture2D>("assets/config-button.png");
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
            Api configMenu = (Api)this.GetApi();

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
                max: 500
            );

            configMenu.AddKeybindList(
                mod: this.ModManifest,
                name: I18n.Options_OpenMenuKey_Name,
                tooltip: I18n.Options_OpenMenuKey_Desc,
                getValue: () => this.Config.OpenMenuKey,
                setValue: value => this.Config.OpenMenuKey = value
            );
        }

        /// <inheritdoc cref="IGameLoopEvents.UpdateTicking"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicking(object sender, UpdateTickingEventArgs e)
        {
            if (this.IsTitleMenuInteractable())
                this.Ui.Update();
        }

        /// <inheritdoc cref="IDisplayEvents.WindowResized"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnWindowResized(object sender, WindowResizedEventArgs e)
        {
            this.ConfigButton.LocalPosition = new Vector2(this.ConfigButton.Position.X, Game1.viewport.Height - 100);
        }

        /// <inheritdoc cref="IDisplayEvents.Rendered"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnRendered(object sender, RenderedEventArgs e)
        {
            if (this.IsTitleMenuInteractable())
                this.Ui.Draw(e.SpriteBatch);
        }

        /// <inheritdoc cref="IDisplayEvents.MenuChanged"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is GameMenu menu)
            {
                OptionsPage page = (OptionsPage)menu.pages[GameMenu.optionsTab];
                page.options.Add(new OptionsButton(I18n.Button_ModOptions(), this.OpenListMenu));
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

        /// <inheritdoc cref="IInputEvents.MouseWheelScrolled"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMouseWheelScrolled(object sender, MouseWheelScrolledEventArgs e)
        {
            Dropdown.ActiveDropdown?.ReceiveScrollWheelAction(e.Delta);
        }
    }
}
