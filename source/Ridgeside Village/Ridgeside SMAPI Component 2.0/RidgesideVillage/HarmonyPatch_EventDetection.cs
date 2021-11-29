/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Rafseazz/Ridgeside-Village-Mod
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using StardewValley;
using StardewValley.Menus;
namespace RidgesideVillage
{
    //Corrects the location name in the "X has begun in Y" message
    //Obsolet in SDV 1.5.5, will be removed
    internal static class HarmonyPatch_EventDetection
    {
        private static IMonitor Monitor { get; set; }
        private static IModHelper Helper { get; set; }

        internal static void ApplyPatch(Harmony harmony, IModHelper helper)
        {
            Helper = helper;

            Log.Trace($"Applying Harmony Patch \"{nameof(GameMenu_ChangeTab_PostFix)}.");
            harmony.Patch(
                original: AccessTools.Method(typeof(GameMenu), nameof(GameMenu.changeTab)),
                prefix: new HarmonyMethod(typeof(HarmonyPatch_EventDetection), nameof(GameMenu_ChangeTab_PostFix))
            );
        }

        internal static void GameMenu_ChangeTab_PostFix(ref GameMenu __instance, int whichTab, bool playSound = true)
        {
            try
            {
              if(whichTab == GameMenu.mapTab)
                {
                    RSVWorldMap.Open(Game1.activeClickableMenu);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Harmony patch \"{nameof(GameMenu_ChangeTab_PostFix)}\" has encountered an error. \n{e.ToString()}");
            }
        }

    }
}
