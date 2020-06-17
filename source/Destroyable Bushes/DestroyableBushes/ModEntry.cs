using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.TerrainFeatures;
using Harmony;

namespace DestroyableBushes
{
    public partial class ModEntry : Mod
    {
        public static Mod Instance { get; set; } = null;
        public static ModConfig Config { get; set; } = null;

        public override void Entry(IModHelper helper)
        {
            Instance = this; //provide a global reference to this mod's utilities

            Config = Helper.ReadConfig<ModConfig>(); //attempt to load (or create) config.json

            Helper.Events.GameLoop.GameLaunched += EnableGMCM; //try to enable Generic Mod Config Menu when the game has launched

            ApplyHarmonyPatches();
        }

        /// <summary>Applies any Harmony patches used by this mod.</summary>
        private void ApplyHarmonyPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create(ModManifest.UniqueID); //create this mod's Harmony instance

            //apply all patches
            HarmonyPatch_BushesAreDestroyable.ApplyPatch(harmony);
            HarmonyPatch_BushesDropWood.ApplyPatch(harmony);
        }
    }
}
