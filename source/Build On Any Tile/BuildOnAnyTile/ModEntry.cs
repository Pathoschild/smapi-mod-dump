/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/BuildOnAnyTile
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace BuildOnAnyTile
{
    public partial class ModEntry : Mod
    {
        public static Mod Instance { get; set; } = null;
        public static ModConfig Config { get; set; } = null;

        public override void Entry(IModHelper helper)
        {
            Instance = this; //provide a static reference to this mod's utilities

            try
            {
                Config = helper.ReadConfig<ModConfig>(); //try to load the config.json file
            }
            catch (Exception ex)
            {
                Monitor.Log($"Encountered an error while loading the config.json file. Default settings will be used instead. Full error message:\n-----\n{ex.ToString()}", LogLevel.Error);
                Config = new ModConfig(); //use the default settings
            }

            ApplyHarmonyPatches();
            
            Helper.Events.GameLoop.GameLaunched += EnableGMCM; //enable GMCM compatibility at launch
        }

        /// <summary>Applies any Harmony patches used by this mod.</summary>
        private void ApplyHarmonyPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create(ModManifest.UniqueID); //create this mod's Harmony instance

            HarmonyPatch_BuildOnAnyTile.ApplyPatch(harmony);
        }
    }
}
