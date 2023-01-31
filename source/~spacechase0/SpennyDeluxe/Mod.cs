/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using SpaceShared;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace SpennyDeluxe
{
    internal class Mod : StardewModdingAPI.Mod
    {
        public static Mod Instance;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Mod.Instance = this;
            Log.Monitor = this.Monitor;

            var harmony = new Harmony(ModManifest.UniqueID);
            new SpriteBatchPatcher().Apply(harmony, Monitor);
            harmony.PatchAll();
        }
    }
}
