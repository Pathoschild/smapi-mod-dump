/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rokugin/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

namespace ShopsAnywhere {
    internal class ModEntry : Mod {

        ModConfig config = new();
        ModConfigKeys Keys => config.Controls;

        public ClickableTextureComponent? advShopButton;

        public override void Entry(IModHelper helper) {
            config = helper.ReadConfig<ModConfig>();
            AssetManager.Initialize(helper.GameContent);

            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
            helper.Events.Content.AssetRequested += static (_, e) => AssetManager.OnAssetRequested(e);
            helper.Events.Content.AssetsInvalidated += static (_, e) => AssetManager.Reset(e.NamesWithoutLocale);

            helper.Events.Input.ButtonsChanged += OnButtonsChanged;
        }

        private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e) {
            if (!Context.IsWorldReady) return;
            
            if (Keys.ToggleShopTest.JustPressed()) {
                return;
            }
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e) {
            if (!Context.IsWorldReady) return;

            if (IsInventoryPage() && e.Button == SButton.MouseLeft &&
                    advShopButton!.containsPoint(Game1.getMouseX(Game1.uiMode), Game1.getMouseY(Game1.uiMode))) {
                Utility.TryOpenShopMenu(Game1.shop_adventurersGuild, (string?)null);
                Helper.Input.Suppress(e.Button);
            }
        }

        private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e) {
            if (IsInventoryPage()) {
                advShopButton = new ClickableTextureComponent(
                    bounds: new Rectangle(Game1.activeClickableMenu.xPositionOnScreen + config.AdvShopButtonOffsetX,
                    Game1.activeClickableMenu.yPositionOnScreen + config.AdvShopButtonOffsetY, AssetManager.advShopTexture.Width, AssetManager.advShopTexture.Height),
                    texture: AssetManager.advShopTexture,
                    sourceRect: Rectangle.Empty,
                    scale: 1f
                    );
                advShopButton.draw(e.SpriteBatch);
                
                if (advShopButton.containsPoint(Game1.getMousePosition().X, Game1.getMousePosition().Y)) {
                    if (Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.en) {
                        IClickableMenu.drawHoverText(e.SpriteBatch, Helper.Translation.Get("shops-button.name"), Game1.smallFont);
                    }
                }
            }
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
            SetupGMCM();
        }

        bool IsInventoryPage() {
            return Game1.activeClickableMenu is GameMenu menu && menu.GetCurrentPage() is InventoryPage;
        }

        private void SetupGMCM() {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (configMenu is null) return;

            configMenu.Register(mod: ModManifest, reset: () => config = new ModConfig(), save: () => Helper.WriteConfig(config));

            configMenu.AddKeybindList(
                mod: ModManifest,
                name: () => "Test Buttons",
                tooltip: () => "Buttons to trigger test functionality. Default: PgUp",
                getValue: () => config.Controls.ToggleShopTest,
                setValue: value => config.Controls.ToggleShopTest = value
                );
            configMenu.AddSectionTitle(ModManifest, () => Helper.Translation.Get("shops-button-config.name"));
            configMenu.AddNumberOption(
                ModManifest,
                () => (int)config.AdvShopButtonOffsetX,
                value => config.AdvShopButtonOffsetX = value,
                () => Helper.Translation.Get("x-offset.name"),
                () => Helper.Translation.Get("x-offset.tooltip")
                );
            configMenu.AddNumberOption(
                ModManifest,
                () => (int)config.AdvShopButtonOffsetY,
                value => config.AdvShopButtonOffsetY = value,
                () => Helper.Translation.Get("y-offset.name"),
                () => Helper.Translation.Get("y-offset.tooltip")
                );
        }

    }
}
