/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Objects;

namespace CustomCaskMod
{
    /// <summary>The mod entry class loaded by SMAPI.</summary>
    public class CustomCaskModEntry : Mod
    {
        internal static IMonitor ModMonitor { get; set; }
        internal new static IModHelper Helper { get; set; }


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModMonitor = Monitor;
            Helper = helper;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;

            Helper.ConsoleCommands.Add("config_reload_contentpacks_customcaskmod", "Reload all content packs for custom cask mod.", DataLoader.LoadContentPacksCommand);
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialized at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            new DataLoader(Helper);

            var harmony = HarmonyInstance.Create("Digus.CustomCaskMod");

            harmony.Patch(
                original: AccessTools.Method(typeof(Cask), nameof(Cask.IsValidCaskLocation)),
                prefix: new HarmonyMethod(typeof(CaskOverrides), nameof(CaskOverrides.IsValidCaskLocation))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Cask), nameof(Cask.GetAgingMultiplierForItem)),
                prefix: new HarmonyMethod(typeof(CaskOverrides), nameof(CaskOverrides.GetAgingMultiplierForItem))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Cask), nameof(Cask.checkForMaturity)),
                prefix: new HarmonyMethod(typeof(CaskOverrides), nameof(CaskOverrides.checkForMaturity))
            );
        }

        /// <summary>Raised after the player loads a save slot and the world is initialized.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        public static void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            DataLoader.FillCaskDataIds();
        }
    }
}
