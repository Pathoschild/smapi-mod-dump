/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jonqora/StardewMods
**
*************************************************/

using StardewModdingAPI;
using Harmony;

namespace BugFixAddItem
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        internal static ModEntry Instance { get; private set; }
        internal HarmonyInstance Harmony { get; private set; }

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Make resources available.
            Instance = this;

            // Apply Harmony patches.
            Harmony = HarmonyInstance.Create(ModManifest.UniqueID);
            UtilityPatches.Apply();
            ItemGrabMenuPatches.Apply();
        }
    }
}