/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/StardewMods
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using SObject = StardewValley.Object;

namespace IncubatePufferfishEggs
{
    /// <summary> The mod entry class loaded by SMAPI. </summary>
    public class ModEntry : Mod
    {
        #region Variables

        public static IMonitor ModMonitor;
        public static ModEntry Instance;
        public int EggId = -2; // Pufferfish Egg ID
        public int LEggId = -2; // Large Pufferfish Egg ID
        
        #endregion
        #region Public methods
        
        /// <summary> The mod entry point, called after the mod is first loaded. </summary>
        /// <param name="helper"> Provides simplified APIs for writing mods. </param>
        public override void Entry(IModHelper helper)
        {
            ModMonitor = Monitor;
            Instance = this;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }

        /// <summary> Gets IDs of the new items assigned by Json Assets. </summary>
        public void GetItemIDs()
        {
            object api = Helper.ModRegistry.GetApi("spacechase0.JsonAssets");
            if (api == null) return;
            
            EggId = Helper.Reflection.GetMethod(api, "GetObjectId").Invoke<int>("Pufferfish Egg");
            LEggId = Helper.Reflection.GetMethod(api, "GetObjectId").Invoke<int>("Large Pufferfish Egg");
        }

        #endregion
        #region Private methods

        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var harmony = HarmonyInstance.Create("GZhynko.IncubatePufferfishEggs");

            harmony.Patch(
                AccessTools.Method(typeof(SObject), nameof(SObject.performObjectDropInAction)),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.PerformObjectDropInAction))
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

            switch (EggId)
            {
                case -2:
                    Monitor.Log("Unable to get IDs of the new items. Make sure you have Json Assets installed.", LogLevel.Warn);
                    break;
                case -1:
                    Monitor.Log("Unable to get IDs of the new items. Make sure you have extracted all the folders from the .zip file.", LogLevel.Warn);
                    break;
                default:
                    Monitor.Log("Got IDs of the new items. Everything seems to be OK over here!");
                    break;
            }
        }
        
        #endregion
    }
}
