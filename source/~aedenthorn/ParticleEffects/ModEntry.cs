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
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace ParticleEffects
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry : Mod, IAssetLoader
    {

        public static IMonitor SMonitor;
        public static IModHelper SHelper;
        public static ModConfig Config;

        public static ModEntry context;
        public static readonly string dictPath = "Mods/aedenthorn.ParticleEffects/dict";
        public static Dictionary<string, ParticleEffectData> effectDict = new Dictionary<string, ParticleEffectData>();
        public static Dictionary<long, EntityParticleData> farmerEffectDict = new Dictionary<long, EntityParticleData>();
        public static Dictionary<string, EntityParticleData> npcEffectDict = new Dictionary<string, EntityParticleData>();
        public static Dictionary<string, EntityParticleData> locationEffectDict = new Dictionary<string, EntityParticleData>();
        public static Dictionary<string, EntityParticleData> objectEffectDict = new Dictionary<string, EntityParticleData>();

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            context = this;

            SMonitor = Monitor;
            SHelper = helper;
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.GameLoop.TimeChanged += GameLoop_TimeChanged;
            helper.Events.Display.RenderedWorld += Display_RenderedWorld;


            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method(typeof(Farmer), nameof(Farmer.draw), new System.Type[] { typeof(SpriteBatch) }),
               postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.Farmer_draw_postfix))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(Object), nameof(Object.draw), new System.Type[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }),
               postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.Object_draw_postfix))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(NPC), nameof(NPC.draw), new System.Type[] { typeof(SpriteBatch), typeof(float) }),
               postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.NPC_draw_postfix))
            );
        }

        private void GameLoop_TimeChanged(object sender, StardewModdingAPI.Events.TimeChangedEventArgs e)
        {
            LoadEffects();
        }

        private void GameLoop_DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            LoadEffects();
        }

        private void GameLoop_SaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            LoadEffects();
        }

        private void Display_RenderedWorld(object sender, StardewModdingAPI.Events.RenderedWorldEventArgs e)
        {
            if (!Config.EnableMod)
                return;
            foreach (var key in effectDict.Keys)
            {
                var ped = effectDict[key];
                switch (ped.type.ToLower())
                {
                    case "location":
                        if (Game1.currentLocation.Name == ped.name)
                            ShowLocationParticleEffect(e.SpriteBatch, Game1.currentLocation, key, ped);
                        break;
                    default:
                        if (locationEffectDict.ContainsKey(Game1.currentLocation.Name))
                            locationEffectDict[Game1.currentLocation.Name].particleDict.Remove(key);
                        break;
                }
            }
        }


        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is not null)
            {
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

        }

        /// <summary>Get whether this instance can load the initial version of the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
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

            return (T)(object)new Dictionary<string, ParticleEffectData>();
        }
    }
}