/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HaulinOats/StardewMods
**
*************************************************/

using System;
using System.Linq;
using GenericModConfigMenu;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using SObject = StardewValley.Object;

namespace DynamicCrops
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private ModConfig Config;
        string[] saplingNames = { "Cherry Sapling", "Apricot Sapling", "Orange Sapling", "Peach Sapling", "Pomegranate Sapling", "Apple Sapling", "Mango Sapling", "Banana Sapling" };

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            //get values from config
            this.Config = this.Helper.ReadConfig<ModConfig>();
            Monitor.Log($"harder seed maker: {this.Config.harderSeedMaker}", LogLevel.Debug);
            Monitor.Log($"inflate seed prices: {this.Config.inflateSeedPrices}", LogLevel.Debug);

            helper.Events.Content.AssetRequested += OnAssetRequested;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.Display.MenuChanged += OnMenuChanged;
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!this.Config.inflateSeedPrices) return;
            if (e.NewMenu is not ShopMenu shopMenu) return;

            //Raise seed prices except for fruit tree saplings
            foreach (var (item, value) in shopMenu.itemPriceAndStock)
            {
                if (item is SObject { Category: SObject.SeedsCategory } && !saplingNames.Contains(item.Name))
                {
                    //pseudo-randomize large seed values
                    shopMenu.itemPriceAndStock[item][0] = new Random().Next(20, 50) * 500;
                }
                    
            }
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            Helper.GameContent.InvalidateCache("Data/CraftingRecipes");
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (this.Config.harderSeedMaker)
            {
                //Make Seed Maker Harder to Make
                if (e.Name.IsEquivalentTo("Data/CraftingRecipes"))
                {
                    e.Edit(asset =>
                    {
                        var data = asset.AsDictionary<string, string>().Data;
                        var assetArray = data["Seed Maker"].Split('/');
                        //1 prismatic shard, 5 Iridium Bars, 3 Diamonds
                        assetArray[0] = "74 1 337 5 72 3";
                        var newValue = string.Join('/', assetArray);
                        data["Seed Maker"] = newValue;
                    });
                }
            }
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Inflate Shop Seed Prices",
                tooltip: () => "If checked, shop seed prices will become unprofitable",
                getValue: () => this.Config.inflateSeedPrices,
                setValue: value => this.Config.inflateSeedPrices = value
            );

            configMenu.AddBoolOption(
              mod: this.ModManifest,
              name: () => "Harder Seed Maker Recipe",
              tooltip: () => "If checked, seed maker will require rarer materials",
              getValue: () => this.Config.harderSeedMaker,
              setValue: value => this.Config.harderSeedMaker = value
          );
        }
    }
}
