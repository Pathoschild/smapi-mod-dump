using Harmony;
using StardewModdingAPI;

namespace BetterBombs
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            var config = Helper.ReadConfig<ModConfig>();
            Helper.WriteConfig(config);

            GameLocationPatches.Initialize(Monitor, helper, config);

            //I considered trying to do this without harmony patching, but this results in a significantly reduced code footprint
            //If anyone has an idea of how to do this without harmony, shoot me a pull request
			var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

			harmony.Patch(
				original: AccessTools.Method(typeof(StardewValley.GameLocation), nameof(StardewValley.GameLocation.explode)),
				prefix: new HarmonyMethod(typeof(GameLocationPatches), nameof(BetterBombs.GameLocationPatches.Explode_Prefix))
				);
        }

        
    }
}