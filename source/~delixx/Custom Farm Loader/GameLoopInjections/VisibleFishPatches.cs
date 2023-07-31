/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-custom-farm-loader
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;
using HarmonyLib;
using Custom_Farm_Loader.Lib;

namespace Custom_Farm_Loader.GameLoopInjections
{
    //This is supposed to ensure that VisibleFish shows CFL fish as well
    //VF calls GameLocation.getFish which I prefix, but when CF calls it my prefix isn't being considered
    //This is a workaround for that
    public class VisibleFishPatches
    {
        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;
        public static void Initialize(Mod mod)
        {
            Mod = mod;
            Monitor = mod.Monitor;
            Helper = mod.Helper;

            Helper.Events.GameLoop.GameLaunched += gameLaunched;
        }

        private static void gameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            if (!Helper.ModRegistry.IsLoaded("shekurika.WaterFish"))
                return;

            var harmony = new Harmony(Mod.ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method("showFishInWater.FishManager:getFishAt"),
               prefix: new HarmonyMethod(typeof(VisibleFishPatches), nameof(VisibleFishPatches.getFish_Prefix))
            );
        }

        public static bool getFish_Prefix(int tileX, int tileY, ref StardewValley.Object __result) => _GameLocation.getFish_Prefix(Game1.currentLocation, ref __result, 0f, 0, 4, Game1.player, 0f, new Microsoft.Xna.Framework.Vector2(tileX, tileY));
        
    }
    
}
