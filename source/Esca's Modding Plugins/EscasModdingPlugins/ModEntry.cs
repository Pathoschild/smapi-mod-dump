/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/EscasModdingPlugins
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;

namespace EscasModdingPlugins
{
    public class ModEntry : Mod
    {
        /// <summary>The beginning of each each map/tile property name implemented by this mod.</summary>
        public static readonly string PropertyPrefix = "Esca.EMP/";
        /// <summary>The beginning of each asset name implemented by this mod.</summary>
        public static readonly string AssetPrefix = "Mods/Esca.EMP/";

        /// <summary>Runs once after all mods are loaded by SMAPI. Initializes file data, events, and Harmony patches.</summary>
        public override void Entry(IModHelper helper)
        {
            //initialize utilities
            AssetHelper.Initialize(helper);
            TileData.Monitor = Monitor;

            //load config.json
            ModConfig.Initialize(helper, Monitor);

            //initialize mod interactions
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched_InitializeModInteractions;

            //initialize Harmony and mod features
            Harmony harmony = new Harmony(ModManifest.UniqueID);

            //fish locations
            HarmonyPatch_FishLocations.ApplyPatch(harmony, Monitor);

            //custom order boards
            HarmonyPatch_CustomOrderBoards.ApplyPatch(harmony, Monitor);
            DisplayNewOrderExclamationPoint.Enable(helper, Monitor);
            Command_CustomBoard.Enable(helper, Monitor);

            //destroyable bushes
            HarmonyPatch_DestroyableBushes.ApplyPatch(harmony, Monitor);

            //bed placement
            HarmonyPatch_BedPlacement.ApplyPatch(harmony, Monitor);
            HarmonyPatch_PassOutSafely.ApplyPatch(harmony, Monitor);

            //kitchen features
            ActionKitchen.Enable(Monitor);
            HarmonyPatch_AllowMiniFridges.ApplyPatch(harmony, Monitor);

            //water color
            WaterColor.Enable(helper, Monitor);
        }

        /// <summary>Initializes mod interactions when all mods have finished loading.</summary>
        private void GameLoop_GameLaunched_InitializeModInteractions(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            ModInteractions.GMCM.Initialize(Helper, Monitor, ModManifest);
        }

        /**************/
        /* API method */
        /**************/

        /// <summary>Generates an API instance for another SMAPI mod.</summary>
        /// <remarks>See <see cref="IEmpApi"/> for documentation.</remarks>
        /// <returns>A new API instance.</returns>
        public override object GetApi() => new EmpApi();
    }
}
