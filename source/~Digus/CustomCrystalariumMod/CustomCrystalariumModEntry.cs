using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using SObject = StardewValley.Object;

namespace CustomCrystalariumMod
{
    /// <summary>The mod entry class loaded by SMAPI.</summary>
    public class CustomCrystalariumModEntry : Mod
    {
        public static IMonitor ModMonitor;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModMonitor = Monitor;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
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

            var harmony = HarmonyInstance.Create("Digus.CustomCrystalariumMod");

            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), "getMinutesForCrystalarium"),
                prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.GetMinutesForCrystalarium))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.performObjectDropInAction)),
                prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.PerformObjectDropInAction))
            );

            if (DataLoader.ModConfig.GetObjectBackOnChange && !DataLoader.ModConfig.GetObjectBackImmediately)
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(SObject), nameof(SObject.performRemoveAction)),
                    prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.PerformRemoveAction))
                );
            }
        }
    }
}
