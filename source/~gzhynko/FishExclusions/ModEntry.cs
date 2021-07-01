/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/StardewMods
**
*************************************************/

using System;
using FishExclusions.Types;
using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

namespace FishExclusions
{
    /// <summary> The mod entry class loaded by SMAPI. </summary>
    public class ModEntry : Mod
    {
        #region Variables

        public static IMonitor ModMonitor;
        public static ModConfig Config;
        
        public static bool ExclusionsEnabled = true;

        public static IJsonAssetsApi JsonAssetsApi;
        
        #endregion
        #region Public methods
        
        /// <summary> The mod entry point, called after the mod is first loaded. </summary>
        /// <param name="helper"> Provides simplified APIs for writing mods. </param>
        public override void Entry(IModHelper helper)
        {
            ModMonitor = Monitor;

            try
            {
                Config = Helper.ReadConfig<ModConfig>();
            }
            catch (Exception exception)
            {
                // Notify user and exit.
                ModMonitor.Log($"Config file is formatted incorrectly, exiting. Details: {exception.Message}",
                    LogLevel.Warn);
            }
            
            CommandManager.RegisterCommands(helper);
            
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }
        
        public void SaveConfig(ModConfig newConfig)
        {
            Config = newConfig;
            Helper.WriteConfig(newConfig);
        }

        #endregion
        #region Private methods

        private void ApplyHarmonyPatches()
        {
            var harmony = HarmonyInstance.Create("GZhynko.FishExclusions");

            harmony.Patch(
                AccessTools.Method(typeof(GameLocation), nameof(GameLocation.getFish)),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.GetFish))
            );
            
            harmony.Patch(
                AccessTools.Method(typeof(MineShaft), nameof(MineShaft.getFish)),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.GetFish))
            );
        }

        /// <summary> Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations. </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            ApplyHarmonyPatches();
            ModConfig.SetUpModConfigMenu(Config, this);
            
            JsonAssetsApi = Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
        }
        
        #endregion
    }
}
