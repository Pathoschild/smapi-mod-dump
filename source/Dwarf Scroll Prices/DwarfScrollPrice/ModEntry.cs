/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/noriteway/StardewMods
**
*************************************************/

using System;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace DwarfScrollPrice
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration from the player.</summary>
        private ModConfig Config;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
            this.Config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/ObjectInformation"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<int, string>().Data;
                    int[] newPrice = { this.Config.price1, this.Config.price2, this.Config.price3, this.Config.price4 };

                    for (int i = 0; i < 4; i++)
                    {
                        string[] itemData = data[96 + i].Split('/');
                        itemData[1] = newPrice[i].ToString();
                        data[96 + i] = string.Join('/', itemData);
                    }
                });
            }
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                this.Monitor.Log($"Config Menu is null.", LogLevel.Debug);
                return;
            }

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                getValue: () => this.Config.price1,
                setValue: value => this.Config.price1 = (int)value,
                name: () => "Dwarf Scroll I",
                tooltip: () => "Sets the sell price of dwarf scroll I.",
                min: 1,
                fieldId: "ds1"
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                getValue: () => this.Config.price2,
                setValue: value => this.Config.price2 = (int)value,
                name: () => "Dwarf Scroll II",
                tooltip: () => "Sets the sell price of dwarf scroll II.",
                min: 1,
                fieldId: "ds2"
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                getValue: () => this.Config.price3,
                setValue: value => this.Config.price3 = (int)value,
                name: () => "Dwarf Scroll III",
                tooltip: () => "Sets the sell price of dwarf scroll III.",
                min: 1,
                fieldId: "ds3"
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                getValue: () => this.Config.price4,
                setValue: value => this.Config.price4 = (int)value,
                name: () => "Dwarf Scroll IV",
                tooltip: () => "Sets the sell price of dwarf scroll IV.",
                min: 1,
                fieldId: "ds4"
            );
        }
    }
}