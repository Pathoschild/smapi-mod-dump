/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/siweipancc/InstantAnimals
**
*************************************************/

using System.Collections.Generic;
using GenericModConfigMenu;
using InstantAnimals.wrapper;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.FarmAnimals;
using SObject = StardewValley.Object;

namespace InstantAnimals
{
    /// <summary>The mod entry point.</summary>
    // ReSharper disable once UnusedType.Global
    public class ModEntry : Mod
    {
        private ModConfig _config = new();
        private bool _adults = true;
        private SButton _button;

        private static InstantPurchaseAnimalsMenu? _purchaseMenu;
        private const string EventId = "3900074";
        private static bool _seen;


        public override void Entry(IModHelper helper)
        {
            Logger.Initialize(Monitor);
            _config = helper.ReadConfig<ModConfig>();
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.Display.MenuChanged += MenuChange;
            _button = _config.ToggleInstantBuyMenuButton;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu =
                this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                return;
            }

            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this._config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this._config)
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "BuyUsesResources",
                tooltip: () => "Indicates if animals should cost their usual resources.",
                getValue: () => this._config.BuyUsesResources,
                setValue: value => this._config.BuyUsesResources = value
            );
            configMenu.AddKeybind(
                mod: this.ModManifest,
                name: () => "ToggleInstantBuyMenuButton",
                tooltip: () => "Button to open and close the Instant Animals menu.",
                getValue: () => this._config.ToggleInstantBuyMenuButton,
                setValue: value =>
                {
                    this._config.ToggleInstantBuyMenuButton = value;
                    this._button = value;
                }
            );
        }

        /*********
         ** Private methods
         *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            // Open animal buy menu
            if (Game1.activeClickableMenu == null && e.Button == _config.ToggleInstantBuyMenuButton &&
                Game1.currentLocation is Farm)
            {
                this.Monitor.Log("Animal buy menu toggled", LogLevel.Debug);
                _seen = Game1.player.eventsSeen.Contains(EventId);
                if (!_seen)
                {
                    Game1.player.eventsSeen.Add(EventId);
                }

                _purchaseMenu = new InstantPurchaseAnimalsMenu(GetPurchaseAnimalStock(), Helper);
                Game1.activeClickableMenu = _purchaseMenu;
            }
            // or change to baby
            else if (Game1.activeClickableMenu is InstantPurchaseAnimalsMenu ipm &&
                     e.Button == _button)
            {
                _adults = !_adults;
                ipm.SetAdult(_adults);
            }
        }

        private static void MenuChange(object? sender, MenuChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            if (e.OldMenu is not InstantPurchaseAnimalsMenu || e.NewMenu is InstantPurchaseAnimalsMenu)
            {
                return;
            }

            if (!_seen)
            {
                Game1.player.eventsSeen.Remove(EventId);
            }
        }

        /// <summary>
        /// 从 FarmAnimals 派生所有可用的农场生物. 1.6 版本为 :
        /// ["WhiteChicken","BrownChicken","BlueChicken","VoidChicken","GoldenChicken","Duck","Rabbit","Dinosaur","WhiteCow","BrownCow","Goat","Sheep","Pig","Ostrich"]
        ///
        ///
        /// 可以加载 mod 生物.
        /// </summary>
        /// <returns></returns>
        private (List<SObject>, IDictionary<string, string>,
            IDictionary<string, FarmAnimalData>)
            GetPurchaseAnimalStock()
        {
            List<SObject> list = new List<SObject>();
            IDictionary<string, string> strings =
                this.Helper.GameContent.Load<IDictionary<string, string>>("Strings/StringsFromCSFiles");
            IDictionary<string, FarmAnimalData> dictionary =
                this.Helper.GameContent.Load<IDictionary<string, FarmAnimalData>>("Data/FarmAnimals");
            Logger.Info(
                $"load {dictionary.Count} animals from Data/FarmAnimals: [{string.Join(',', dictionary.Keys)}]");
            foreach (var (key, value) in dictionary)
            {
                var price = _config.BuyUsesResources ? value.SellPrice : 0;
                SObject o = new SObject("100", 1, isRecipe: false, price)
                {
                    Name = key,
                    displayName = value.DisplayName,
                    Type = null
                };
                list.Add(o);
            }


            return (list, strings, dictionary);
        }
    }
}