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
    public class ModEntry : Mod, IAssetLoader
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
            HarmonyPatch_ActionKitchen.ApplyPatch(harmony, Monitor);
            HarmonyPatch_AllowMiniFridges.ApplyPatch(harmony, Monitor);
        }

        /************************/
        /* IAssetLoader methods */
        /************************/

        public bool CanLoad<T>(IAssetInfo asset) => AssetHelper.CanLoad<T>(asset); //use AssetHelper
        public T Load<T>(IAssetInfo asset) => AssetHelper.Load<T>(asset); //use AssetHelper

        /**************/
        /* API method */
        /**************/

        /// <summary>Generates an API instance for another SMAPI mod.</summary>
        /// <remarks>See <see cref="IEmpApi"/> for documentation.</remarks>
        /// <returns>A new API instance.</returns>
        public override object GetApi() => new EmpApi();
    }
}
