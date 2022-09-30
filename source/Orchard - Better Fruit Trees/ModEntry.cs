/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Saitoue/Orchard
**
*************************************************/

using System;
using System.Reflection;
using GenericModConfigMenu;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace Orchard
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        internal static ModConfig Config;

        


        
        

        

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Config = Helper.ReadConfig<ModConfig>();

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;

            helper.Events.Content.AssetRequested += this.OnAssetRequested;

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

            

        }

        /// <summary>
        /// changes description if tree fertilizer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/ObjectInformation"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<int, string>().Data;

                    string[] fields = data[805].Split('/');
                    fields[5] = "Sprinkle on a wild tree to ensure rapid growth, even in winter. Now also works on fruit trees. ";

                    data[805] = string.Join("/",fields);
                });
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
                reset: () =>Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            // add some config options
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Drop sapplings",
                tooltip: () => "Allows Fruit Trees to drop one Sapling per Season",
                getValue: () => Config.dropSappling,
                setValue: value =>Config.dropSappling = value
            );

            configMenu.AddBoolOption(
               mod: this.ModManifest,
               name: () => "Fruits give Foraging Exp",
               tooltip: () => "Shaking fruits of a tree will award Exp per fuit",
               getValue: () => Config.expFromTrees,
               setValue: value => Config.expFromTrees = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Out of season fruit trees",
                tooltip: () => "Fruit trees will grow out of season if fertilized.",
                getValue: () => Config.outOfSeasonTrees,
                setValue: value => Config.outOfSeasonTrees = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Fertilizer affects fruits",
                tooltip: () => "Fruit trees will grow more fruit if fertilized.",
                getValue: () => Config.extraFruitFertilizer,
                setValue: value => Config.extraFruitFertilizer = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Foraging affects fruits",
                tooltip: () => "Fruit Trees will grow additional fruit based on your foraging level",
                getValue: () => Config.extraFruitLevel,
                setValue: value => Config.extraFruitLevel = value
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                getValue: () => Config.fruitPerLevel,
                setValue: value => Config.fruitPerLevel = value,
                name: () => "Chance for extra fruits",
                tooltip: () => "Chance in percent to grow an extra Fruit per Foraging level",
                min:  1,
                max: 10,
                interval: 1,
                formatValue: null,
                fieldId: null
                );

        }

#if DEBUG
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {

            
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if ( e.Button.Equals(SButton.K))
            {
                this.Monitor.Log($"{Game1.player.Name} Started Tree count", LogLevel.Debug);

                foreach(TerrainFeature t in Game1.player.currentLocation.terrainFeatures.Values)
                {
                    if(t is FruitTree)
                    {
                        this.Monitor.Log($"counting Tree", LogLevel.Debug);
                        if (t.modData.ContainsKey("fertilizer"))
                        {
                            this.Monitor.Log($"Status :" + t.modData["fertilizer"] + "  X  " + t.currentTileLocation.X + "  Y  " + t.currentTileLocation.Y  + (t as FruitTree).indexOfFruit, LogLevel.Debug);
                        }
                        else
                        {
                            this.Monitor.Log($"coud not find key", LogLevel.Debug);
                        }
                        
                    }
                }
                int num = 1;
                foreach(Object o in Game1.player.currentLocation.Objects.Values)
                {

                    this.Monitor.Log($"object :" + num + "  " + o.Name, LogLevel.Debug);
                    num++;
                }
#endif
            }
        }
    }
}