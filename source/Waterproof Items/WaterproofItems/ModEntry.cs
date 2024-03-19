/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/WaterproofItems
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using System;

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

            Helper.Events.Display.RenderedActiveMenu += GMCM.Enable;
        }

        /// <summary>Applies any Harmony patches used by this mod.</summary>
        private void ApplyHarmonyPatches()
        {
            Harmony harmony = new Harmony(ModManifest.UniqueID); //create this mod's Harmony instance

            HarmonyPatch_FloatingItemBehavior.ApplyPatch(harmony);

            HarmonyPatch_FloatingItemVisualEffect.Instance = harmony; //pass the harmony instance to this patch (handled differently to support reuse after launch)
            if (Config?.FloatingAnimation == true) //if the floating animation is enabled
                HarmonyPatch_FloatingItemVisualEffect.ApplyPatch();
        }
    }
}
