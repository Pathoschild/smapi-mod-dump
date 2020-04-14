using System.Reflection;

using Harmony;

using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;


namespace GoToBed.Framework {
    /// <summary>
    /// Provides StardewValley 1.3 spouse sleeping behavior
    /// (You can talk to your spouse after he/she went to bed).
    /// </summary>
    internal static class Stardew13SpouseSleepPatch {
        private static IMonitor monitor_;

        /// <summary>
        /// Creates Harmony patch.
        /// </summary>
        /// <param name="uniqueID">Unique identifier.</param>
        public static void Create(string uniqueID, IMonitor monitor) {
            monitor_ = monitor;
            monitor_.Log("Create Harmony patch");
            monitor_.Log("Disable Stardew13SpouseSleep in config.json if you experience problems");

            HarmonyInstance harmony  = HarmonyInstance.Create(uniqueID);
            MethodInfo      original = typeof(FarmHouse).GetMethod(nameof(FarmHouse.spouseSleepEndFunction));
            HarmonyMethod   prefix   = new HarmonyMethod(typeof(Stardew13SpouseSleepPatch),
                                                         nameof(Stardew13SpouseSleepPatch.Prefix_FarmHouse_spouseSleepEndFunction));
            HarmonyMethod   postfix  = new HarmonyMethod(typeof(Stardew13SpouseSleepPatch),
                                                         nameof(Stardew13SpouseSleepPatch.Postfix_FarmHouse_spouseSleepEndFunction));
            harmony.Patch(original, prefix, postfix);
        }

        /// <summary>
        /// Prefix that disables <c>FarmHouse.spouseSleepEndFunction()</c>.
        /// </summary>
        /// <returns><c>false</c></returns>
        private static bool Prefix_FarmHouse_spouseSleepEndFunction() {
            monitor_.Log("Disable spouseSleepEndFunction");

            return false;
        }

        /// <summary>
        /// Postfix that adds the marriage dialogue.
        /// </summary>
        private static void Postfix_FarmHouse_spouseSleepEndFunction(Character c, GameLocation location) {
            monitor_.Log("Add marriage dialogue");

            // PathFindController constructor removed marriage dialogue so we have to recreate it.
            // Fortunately we can just call checkForMarriageDialogue() with a time argument representing 6 o'clock PM.
            // (Patching FarmHouse.performTenMinuteUpdate() to change arguments of PathFindController() is not feasible).
            (c as NPC).checkForMarriageDialogue(1800, location);
        }
    }
}
