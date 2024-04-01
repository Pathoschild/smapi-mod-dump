/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Zamiell/stardew-valley-mods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace EatDrinkFromInventory
{
    public class ModEntry : Mod
    {
        // Variables
        bool isEating = false;
        int facingDirectionBeforeEating = 0;

        private ModConfig config = new();

        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                return;
            }

            configMenu.Register(
                mod: this.ModManifest,
                reset: () => config = new ModConfig(),
                save: () => this.Helper.WriteConfig(config)
            );

            configMenu.AddKeybindList(
                this.ModManifest,
                () => config.Hotkey,
                (KeybindList val) => config.Hotkey = val,
                () => "Hotkey",
                () => "The hotkey to consume the food/drink that the cursor is over."
            );
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            var oldIsEating = isEating;
            var newIsEating = Game1.player.isEating;
            isEating = newIsEating;

            if (oldIsEating && !newIsEating)
            {
                Game1.player.FacingDirection = facingDirectionBeforeEating;
                EmulatePause();
            }
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            if (config.Hotkey.IsDown())
            {
                ConsumerItemThatCursorIsOver();
            }
        }

        private void ConsumerItemThatCursorIsOver()
        {
            if (Game1.activeClickableMenu is not GameMenu gameMenu) {
                return;
            }

            if (gameMenu.pages.Count == 0)
            {
                return;
            }

            var firstPage = gameMenu.pages[0];
            if (firstPage is not InventoryPage inventoryPage)
            {
                return;
            }

            if (inventoryPage.hoveredItem is not StardewValley.Object obj)
            {
                return;
            }

            if (obj.Edibility <= 0)
            {
                return;
            }

            facingDirectionBeforeEating = Game1.player.FacingDirection;

            Game1.player.Items.ReduceId(obj.ItemId, 1);
            Game1.player.eatObject(obj);
            Game1.activeClickableMenu = null;
        }

        private void EmulatePause()
        {
            Game1.activeClickableMenu = new GameMenu();
        }

        private void Log(string msg)
        {
            this.Monitor.Log(msg, LogLevel.Debug);
        }
    }
}
