using Harmony;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN.Patches.GameLocationPatch
{
    /// <summary>
    /// SIMPLY ROUTING BULLSHIT IN OUR FAVOR.
    /// WE WANT CUSTOM SEASONAL TILESHEETS, WE GET SEASONAL TILESHEETS.
    /// </summary>
    //[HarmonyPatch(typeof(GameLocation))]
    //[HarmonyPatch("updateSeasonalTileSheets")]
    class updateSeasonalTileSheetsPatch
    {
        /// <summary>
        /// This will always return false, causing the original updateSeasonalTileSheets to be skipped. 
        /// </summary>
        /// <returns></returns>
        public static bool Prefix()
        {
            return false;
        }

        /// <summary>
        /// Pathoschild's Method of implementing seasonal custom tilesheets.
        /// https://stardewvalleywiki.com/User:Pathoschild/Modding_wishlist#Medium_changes
        /// </summary>
        /// <param name="__instance"></param>
        public static void Postfix(GameLocation __instance)
        {
            if (__instance.IsOutdoors && !__instance.Name.Equals("Desert"))
            {
                for (int i = 0; i < __instance.Map.TileSheets.Count; i++)
                {
                    string imageSource = __instance.Map.TileSheets[i].ImageSource;
                    string imageFile = Path.GetFileName(imageSource);
                    if (imageSource.Contains("spring_") || imageSource.Contains("summer_") || imageSource.Contains("fall_") || imageSource.Contains("winter_"))
                    {
                        string imageDir = Path.GetDirectoryName(imageSource);
                        if (string.IsNullOrWhiteSpace(imageDir))
                            imageDir = "Maps";
                        __instance.Map.TileSheets[i].ImageSource = Path.Combine(imageDir, Game1.currentSeason + "_" + imageFile.Split('_')[1]);
                        __instance.Map.DisposeTileSheets(Game1.mapDisplayDevice);
                        __instance.Map.LoadTileSheets(Game1.mapDisplayDevice);
                    }
                }
            }
        }
    }
}
