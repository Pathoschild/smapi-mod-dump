using System;
using System.Linq;
using System.Reflection;
using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
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
        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            new DataLoader(Helper);

            var harmony = HarmonyInstance.Create("Digus.CustomCaskMod");

            harmony.Patch(
                original: AccessTools.Method(typeof(Cask), nameof(Cask.performObjectDropInAction)),
                prefix: new HarmonyMethod(typeof(CaskOverrides), nameof(CaskOverrides.PerformObjectDropInAction))
            );

            if (!DataLoader.ModConfig.DisableAutomateCompatibility && Helper.ModRegistry.IsLoaded("Pathoschild.Automate"))
            {
                ModMonitor.Log("Automated detected, patching it to work with configured items and aging rates.",LogLevel.Info);
                try
                {
                    Assembly automateAssembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.FullName.StartsWith("Automate,"));
                    harmony.Patch(
                        original: AccessTools.Constructor(automateAssembly.GetType("Pathoschild.Stardew.Automate.Framework.Machines.Objects.CaskMachine"), new Type[] { typeof(Cask), typeof(GameLocation), typeof(Vector2) }),
                        postfix: new HarmonyMethod(typeof(CaskOverrides), nameof(CaskOverrides.CaskMachine))
                    );
                }
                catch (Exception ex)
                {
                    ModMonitor.Log("Error trying to patch Automate. Configured items and aging rates will not work with Automate.", LogLevel.Warn);
                    ModMonitor.Log(ex.Message, LogLevel.Trace);
                    ModMonitor.Log(ex.StackTrace, LogLevel.Trace);
                }
                
            }
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        public static void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            DataLoader.FillCaskDataIds();
        }
    }
}
