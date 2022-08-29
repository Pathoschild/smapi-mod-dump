/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/2Retr0/PlacementPlus
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using static PlacementPlus.ModState;

namespace PlacementPlus
{
    public class PlacementPlus : Mod
    {
        // Static instance of entry class to allow for static mod classes (i.e. patch classes) to interact with entry class data.
        internal static PlacementPlus Instance;

        /*********
        ** Public methods
        *********/
        public override void Entry(IModHelper helper)
        {
            Instance = this; // Initialize static instance first as Harmony patches rely on it.
            Initialize(ref helper); // Initialize ModState to begin tracking values.
            
            var harmony = new Harmony(ModManifest.UniqueID); harmony.PatchAll();
        }
    }
}