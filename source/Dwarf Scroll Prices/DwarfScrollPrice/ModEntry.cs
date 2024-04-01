/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/noriteway/StardewMods
**
*************************************************/

using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Objects;
using System.Linq;
using static StardewValley.Minigames.CraneGame;

namespace DwarfScrollPrice
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration from the player.</summary>
        private ModConfig? Config;
        private bool needRefresh = false;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            helper.Events.Content.AssetRequested += OnAssetRequested;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if(e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, ObjectData>().Data;
                    if(Config != null)
                    {
                        data["96"].Price = Config.price1;
                        data["97"].Price = Config.price2;
                        data["98"].Price = Config.price3;
                        data["99"].Price = Config.price4;
                    }
                });
            }
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // Get MCM API(if installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            
            // Exit if no config
            if(configMenu is null) return;
            if(Config == null) return;

            // Register mod for MCM
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => OnSave()
            );

            // Add UI for MCM
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.price1,
                setValue: value => Config.price1 = (int)value,
                name: () => "Dwarf Scroll I",
                tooltip: () => "Sets the sell price of dwarf scroll I.",
                min: 1,
                fieldId: "ds1"
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.price2,
                setValue: value => Config.price2 = (int)value,
                name: () => "Dwarf Scroll II",
                tooltip: () => "Sets the sell price of dwarf scroll II.",
                min: 1,
                fieldId: "ds2"
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.price3,
                setValue: value => Config.price3 = (int)value,
                name: () => "Dwarf Scroll III",
                tooltip: () => "Sets the sell price of dwarf scroll III.",
                min: 1,
                fieldId: "ds3"
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.price4,
                setValue: value => Config.price4 = (int)value,
                name: () => "Dwarf Scroll IV",
                tooltip: () => "Sets the sell price of dwarf scroll IV.",
                min: 1,
                fieldId: "ds4"
            );
        }

        // Called when the MCM for this mod saves
        void OnSave()
        {
            if(Config != null)
            {
                Helper.WriteConfig(Config);

                // Apply new prices to object data
                Game1.objectData["96"].Price = Config.price1;
                Game1.objectData["97"].Price = Config.price2;
                Game1.objectData["98"].Price = Config.price3;
                Game1.objectData["99"].Price = Config.price4;
            }

            // Apply new prices to existing scrolls
            RefreshScrolls();

            // Need to also refresh once a save is loaded if changes are made in the main menu
            needRefresh = true;
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            // Apply new prices to existing scrolls if needed
            if(needRefresh) RefreshScrolls();
        }

        private void RefreshScrolls()
        {
            Utility.ForEachItem((item, remove, replaceWith) =>
            {
                if(item.QualifiedItemId == "(O)96")
                {
                    Item newItem = ItemRegistry.Create("(O)96", item.Stack);
                    replaceWith(newItem);
                }

                if(item.QualifiedItemId == "(O)97")
                {
                    Item newItem = ItemRegistry.Create("(O)97", item.Stack);
                    replaceWith(newItem);
                }

                if(item.QualifiedItemId == "(O)98")
                {
                    Item newItem = ItemRegistry.Create("(O)98", item.Stack);
                    replaceWith(newItem);
                }

                if(item.QualifiedItemId == "(O)99")
                {
                    Item newItem = ItemRegistry.Create("(O)99", item.Stack);
                    replaceWith(newItem);
                }

                return true;
            });

            needRefresh = false;
        }
    }
}
