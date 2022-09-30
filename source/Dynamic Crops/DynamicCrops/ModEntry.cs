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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace DynamicCrops
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private ModConfig Config;
        private bool flowersCanRegrow;
        private ModData cropsAndObjectData;
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            //get values from config
            this.Config = this.Helper.ReadConfig<ModConfig>();
            flowersCanRegrow = Config.flowersCanRegrow;
            Monitor.Log($"flowers can regrow: {flowersCanRegrow}", LogLevel.Debug);

            helper.Events.GameLoop.SaveCreated += OnSaveCreation;
            helper.Events.Content.AssetRequested += OnAssetRequested;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }

        private void OnSaveCreation(object sender, SaveCreatedEventArgs e)
        {
            Monitor.Log($"{Game1.player.Name}'s game has loaded...", LogLevel.Debug);
            Monitor.Log($"intiating dynamic crop pricing scripts...", LogLevel.Debug);
            cropsAndObjectData = ModData.initUtility(Config);
            Monitor.Log($"dynamic crops data created!");
            Helper.Data.WriteSaveData("crops-object-data", cropsAndObjectData);
            Monitor.Log("crop and object data saved to save file", LogLevel.Debug);
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            cropsAndObjectData = Helper.Data.ReadSaveData<ModData>("crops-object-data") ?? null;
            if(cropsAndObjectData != null){
                Monitor.Log("save file loaded. Invalidating cache...", LogLevel.Debug);
                Helper.GameContent.InvalidateCache("Data/Crops");
                Helper.GameContent.InvalidateCache("Data/ObjectInformation");
                Helper.GameContent.InvalidateCache("TileSheets/crops");
            }
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (Context.IsWorldReady && cropsAndObjectData != null)
            {
                if (e.Name.IsEquivalentTo("Data/Crops"))
                {
                    Monitor.Log("loading crop data...", LogLevel.Debug);
                    e.Edit(asset =>
                    {
                        var data = asset.AsDictionary<int, string>().Data;
                        foreach (var item in cropsAndObjectData.CropData)
                        {
                            data[int.Parse(item.Key)] = item.Value;
                        }
                    });
                    Monitor.Log("crop data loaded", LogLevel.Debug);
                }
                if (e.Name.IsEquivalentTo("Data/ObjectInformation"))
                {
                    Monitor.Log("loading object data...", LogLevel.Debug);
                    e.Edit(asset =>
                    {
                        var data = asset.AsDictionary<int, string>().Data;
                        foreach (var item in cropsAndObjectData.ObjectData)
                        {
                            data[int.Parse(item.Key)] = item.Value;
                        }
                    });
                    Monitor.Log("object data loaded", LogLevel.Debug);
                }
                if (e.Name.IsEquivalentTo("TileSheets/crops"))
                {
                    e.Edit(edit => {
                        Texture2D sourceTexture = Helper.ModContent.Load<Texture2D>("assets/Crops.png");
                        var targetTexture = edit.AsImage();
                        targetTexture.PatchImage(sourceTexture, null, new Rectangle(0,0, sourceTexture.Width, sourceTexture.Height));
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
                name: () => "Allow flowers to regrow",
                tooltip: () => "If checked, flowers can possibly be given the ability to regrow",
                getValue: () => this.Config.flowersCanRegrow,
                setValue: value => this.Config.flowersCanRegrow = value
            );

            //configMenu.AddTextOption(
            //    mod: this.ModManifest,
            //    name: () => "Example string",
            //    getValue: () => this.Config.ExampleString,
            //    setValue: value => this.Config.ExampleString = value
            //);
            //tooltip: () => "An optional description shown as a tooltip to the player.",

            //configMenu.AddTextOption(
            //    mod: this.ModManifest,
            //    name: () => "Balance Mode",
            //    getValue: () => this.Config.balanceMode,
            //    setValue: value => {
            //        this.Config.balanceMode = value;
            //        balanceMode = value;
            //    },
            //    allowedValues: new string[] { "Realistic", "More Realistic", "Lightweight", "Dynamic" }
            //);
        }
    }
}
