using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using SObject = StardewValley.Object;

namespace ProducerFrameworkMod
{
    public class ProducerFrameworkModEntry : Mod
    {
        internal static IMonitor ModMonitor { get; set; }
        internal new static IModHelper Helper { get; set; }

        public override void Entry(IModHelper helper)
        {
            ModMonitor = Monitor;
            Helper = helper;
            
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += DataLoader.LoadContentPacks;
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

            var harmony = HarmonyInstance.Create("Digus.ProducerFrameworkMod");

            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.performObjectDropInAction)),
                prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.PerformObjectDropInAction))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), "loadDisplayName"),
                prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.LoadDisplayName))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.checkForAction)),
                postfix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.checkForAction))
            ); harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.minutesElapsed)),
                postfix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.minutesElapsed))
            );
        }
    }
}
