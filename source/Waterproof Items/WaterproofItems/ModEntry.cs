using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using Harmony;

namespace WaterproofItems
{
    public partial class ModEntry : Mod
    {
        public static Mod Instance { get; set; } = null;
        public static ModConfig Config { get; set; } = null;

        public override void Entry(IModHelper helper)
        {
            Instance = this; //provide a global reference to this mod's SMAPI utilities

            try
            {
                Config = Helper.ReadConfig<ModConfig>(); //attempt to load (or create) config.json
            }
            catch (Exception ex)
            {
                Monitor.Log($"Encountered an error while loading the config.json file. Default settings will be used instead. Full error message:\n-----\n{ex.ToString()}", LogLevel.Error);
                Config = new ModConfig(); //use the default settings
            }

            ApplyHarmonyPatches();

            Helper.Events.GameLoop.GameLaunched += EnableGMCM;
        }

        /// <summary>Applies any Harmony patches used by this mod.</summary>
        private void ApplyHarmonyPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create(ModManifest.UniqueID); //create this mod's Harmony instance

            HarmonyPatch_FloatingItemBehavior.ApplyPatch(harmony);

            HarmonyPatch_FloatingItemVisualEffect.Instance = harmony; //pass the harmony instance to this patch (handled differently to support reuse after launch)
            if (Config?.EnableCosmeticFloatingEffect == true) //if the cosmetic effect is enabled
                HarmonyPatch_FloatingItemVisualEffect.ApplyPatch();
        }
    }
}
