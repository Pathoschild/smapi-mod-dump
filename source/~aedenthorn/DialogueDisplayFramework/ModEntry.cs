/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Object = StardewValley.Object;

namespace DialogueDisplayFramework
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry : Mod, IAssetLoader
    {

        public static IMonitor SMonitor;
        public static IModHelper SHelper;
        public static ModConfig Config;
        public static ModEntry context;

        private static string dictPath = "aedenthorn.DialogueDisplayFramework/dictionary";
        private static string defaultKey = "default";
        private static Dictionary<string, DialogueDisplayData> dataDict = new Dictionary<string, DialogueDisplayData>();
        private static Dictionary<string, Texture2D> imageDict = new Dictionary<string, Texture2D>();
        private static List<string> loadedPacks = new List<string>();

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            context = this;

            SMonitor = Monitor;
            SHelper = helper;

            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.PatchAll();
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if(Config.EnableMod && e.Button == Config.ReloadButton)
            {
                foreach (var pack in loadedPacks)
                {
                    SHelper.ConsoleCommands.Trigger("patch", new string[] { "reload", pack });
                }
                LoadData();
            }
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            LoadData();
        }

        private static void LoadData()
        {
            //SMonitor.Log("Loading Data");

            loadedPacks.Clear();
            dataDict = SHelper.Content.Load<Dictionary<string, DialogueDisplayData>>(dictPath, ContentSource.GameContent);
            //SMonitor.Log($"Loaded {dataDict.Count} data entries");
            if (!dataDict.ContainsKey(defaultKey))
                dataDict[defaultKey] = new DialogueDisplayData() { disabled = true };

            imageDict.Clear();
            foreach(var key in dataDict.Keys)
            {
                if(dataDict[key].packName != null && !loadedPacks.Contains(dataDict[key].packName))
                {
                    loadedPacks.Add(dataDict[key].packName);
                }
                foreach (var image in dataDict[key].images)
                {
                    if(!imageDict.ContainsKey(image.texturePath))
                        imageDict[image.texturePath] = Game1.content.Load<Texture2D>(image.texturePath);
                }
                if (dataDict[key].portrait?.texturePath != null && !imageDict.ContainsKey(dataDict[key].portrait.texturePath))
                    imageDict[dataDict[key].portrait.texturePath] = Game1.content.Load<Texture2D>(dataDict[key].portrait.texturePath);
            }
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {

            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Mod Enabled",
                getValue: () => Config.EnableMod,
                setValue: value => Config.EnableMod = value
            );
        }
        public bool CanLoad<T>(IAssetInfo asset)
        {
            if (!Config.EnableMod)
                return false;

            return asset.AssetNameEquals(dictPath);
        }

        /// <summary>Load a matched asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public T Load<T>(IAssetInfo asset)
        {
            Monitor.Log("Loading dictionary");

            return (T)(object)new Dictionary<string, DialogueDisplayData>();
        }
    }
}