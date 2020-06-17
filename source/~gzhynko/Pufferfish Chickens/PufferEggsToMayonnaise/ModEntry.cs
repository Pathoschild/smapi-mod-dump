using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using SObject = StardewValley.Object;

namespace PufferEggsToMayonnaise
{
    /// <summary> The mod entry class loaded by SMAPI. </summary>
    public class ModEntry : Mod
    {
        public static IMonitor ModMonitor;
        public static ModEntry instance;
        public int mayoID = -2; // Pufferfish Mayonnaise ID
        public int eggID = -2; // Pufferfish Egg ID
        public int lEggID = -2; // Large Pufferfish Egg ID

        /*********
        ** Public methods
        *********/
        /// <summary> The mod entry point, called after the mod is first loaded. </summary>
        /// <param name="helper"> Provides simplified APIs for writing mods. </param>
        public override void Entry(IModHelper helper)
        {
            ModMonitor = Monitor;
            instance = this;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }

        /// <summary> Gets IDs of the new items assigned by Json Assets. </summary>
        public void GetItemIDs()
        {
            object api = Helper.ModRegistry.GetApi("spacechase0.JsonAssets");
            if (api != null)
            {
                mayoID = Helper.Reflection.GetMethod(api, "GetObjectId").Invoke<int>("Pufferfish Mayonnaise");
                eggID = Helper.Reflection.GetMethod(api, "GetObjectId").Invoke<int>("Pufferfish Egg");
                lEggID = Helper.Reflection.GetMethod(api, "GetObjectId").Invoke<int>("Large Pufferfish Egg");
            }
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var harmony = HarmonyInstance.Create("GZhynko.PufferEggsToMayonnaise");

            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.performObjectDropInAction)),
                prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.PerformObjectDropInAction))
            );
        }

        /// <summary>Raised after the save is loaded.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            GetItemIDs();

            /*
             * -2 = Unable to locate the Json Assets mod. The user has no Json Assets installed.
             * -1 = Json Assets can't provide IDs of the new objects. The user has not copied the [JA] folder to the Mods folder.
            */

            if (mayoID == -2)
            {
                Monitor.Log("Unable to get IDs of the new items. Make sure you have Json Assets installed.", LogLevel.Warn);
            }
            else if (mayoID == -1)
            {
                Monitor.Log("Unable to get IDs of the new items. Make sure you have extracted all the folders from the .zip file.", LogLevel.Warn);
            }
            else
            {
                Monitor.Log("Got IDs of the new items. Everything seems to be OK over here!", LogLevel.Trace);
            }
        }
    }
}
