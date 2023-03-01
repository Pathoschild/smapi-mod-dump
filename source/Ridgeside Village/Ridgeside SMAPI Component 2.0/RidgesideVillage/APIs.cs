/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Rafseazz/Ridgeside-Village-Mod
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace RidgesideVillage
{
    internal class ExternalAPIs
    {
        public static IContentPatcherApi CP;
        public static IJsonAssetsApi JA;
        public static IWearMoreRingsApi MR;
        public static ISpaceCoreApi SC;
        public static IShopTileFrameworkAPI STFAPI;

        private static IMonitor Monitor { get; set; }
        private static IModHelper Helper { get; set; }

        internal static void Initialize(IModHelper helper)
        {
            Helper = helper;

            CP = Helper.ModRegistry.GetApi<IContentPatcherApi>("Pathoschild.ContentPatcher");
            if (CP is null)
            {
                Log.Alert("Content Patcher is not installed - RSV requires CP to run. Please install CP and restart your game.");
                return;
            }

            JA = Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
            if(JA == null)
            {
                Log.Warn("Json Assets API not found. This could lead to issues.");
            }

            MR = Helper.ModRegistry.GetApi<IWearMoreRingsApi>("bcmpinc.WearMoreRings");
            if (MR == null)
            {
                Log.Trace("Wear More Rings API not found. Using base game ring slots only.");
            }

            SC = Helper.ModRegistry.GetApi<ISpaceCoreApi>("spacechase0.SpaceCore");
            if (SC == null)
            {
                Log.Warn("SpaceCore API not found. This could lead to issues.");
            }

            STFAPI = Helper.ModRegistry.GetApi<IShopTileFrameworkAPI>("Cherry.ShopTileFramework");
            if (STFAPI == null)
            {
                Log.Warn("Shop Tile Framework API not found. This could lead to issues.");
            }
        }
    }
}
