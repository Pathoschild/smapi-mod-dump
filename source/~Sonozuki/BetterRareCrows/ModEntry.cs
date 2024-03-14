/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using BetterRarecrows.Config;
using BetterRarecrows.Patches;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace BetterRarecrows
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The mod configuration.</summary>
        public ModConfig Config { get; private set; }

        /// <summary>The singleton instance for <see cref="ModEntry"/>.</summary>
        public static ModEntry Instance { get; private set; }


        /*********
        ** Public Methods
        *********/
        /// <inheritdoc/>
        public override void Entry(IModHelper helper)
        {
            Instance = this;

            ApplyHarmonyPatches();

            Config = this.Helper.ReadConfig<ModConfig>();
        }


        /*********
        ** Private Methods
        *********/
        /// <summary>Applies the harmony patches for replacing game code.</summary>
        private void ApplyHarmonyPatches()
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(Farm), nameof(Farm.addCrows)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmPatch), nameof(FarmPatch.AddCrowsPrefix)))
            );
        }
    }
}
