/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using BetterRarecrows.Config;
using BetterRarecrows.Patches;
using Harmony;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;

namespace BetterRarecrows
{
    /// <summary>Mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The current rarecrows the player has placed on their farm.</summary>
        public static List<int> CurrentRarecrows { get; set; }

        /// <summary>This is the data the game was when crows last attempted the eat crops (This is a new CurrentRarecrows list can be created each day)</summary>
        public static int PreviousDate { get; set; } = 0;

        /// <summary>Provides methods for logging to the console.</summary>
        public static IMonitor ModMonitor { get; private set; }

        /// <summary>The mod configuration.</summary>
        public static ModConfig Config { get; private set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>The mod entry point.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory as well as the modding api.</param>
        public override void Entry(IModHelper helper)
        {
            ApplyHarmonyPatches();

            ModMonitor = this.Monitor;
            Config = this.Helper.ReadConfig<ModConfig>();
        }


        /*********
        ** Private Methods
        *********/
        /// <summary>The method that applies the harmony patches for replacing game code.</summary>
        private void ApplyHarmonyPatches()
        {
            // create a new Harmony instance for patching source code
            HarmonyInstance harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(Farm), nameof(Farm.addCrows)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmPatch), nameof(FarmPatch.AddCrowsPrefix)))
            );
        }
    }
}
