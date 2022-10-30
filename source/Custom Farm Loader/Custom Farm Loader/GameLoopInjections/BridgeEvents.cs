/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-custom-farm-loader
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
using System.Xml;
using StardewModdingAPI.Events;
using System.Reflection;
using Custom_Farm_Loader.Lib;
using Custom_Farm_Loader.Lib.Enums;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using Microsoft.Xna.Framework;
using xTile.Display;
using xTile.Tiles;
using xTile.Layers;

namespace Custom_Farm_Loader.GameLoopInjections
{
    public class BridgeEvents
    {
        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;
        public static void Initialize(Mod mod)
        {
            Mod = mod;
            Monitor = mod.Monitor;
            Helper = mod.Helper;

            var harmony = new Harmony(mod.ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method(typeof(Farm), nameof(Farm.isCollidingPosition), new[] { typeof(Rectangle), typeof(xTile.Dimensions.Rectangle), typeof(bool), typeof(int), typeof(bool), typeof(Character), typeof(bool), typeof(bool), typeof(bool) }),
               prefix: new HarmonyMethod(typeof(BridgeEvents), nameof(BridgeEvents.isCollidingPosition_Prefix))
            );

            Helper.Events.GameLoop.DayStarted += DayStarted;
        }

        public static void DrawLayer_Postfix(IDisplayDevice displayDevice, xTile.Dimensions.Rectangle mapViewport, xTile.Dimensions.Location displayOffset, bool wrapAround, int pixelZoom)
        {
            if (Game1.currentLocation?.Name != "Farm")
                return;

            var location = Game1.currentLocation;
            var cfl_layer = location.map.GetLayer("CFL_Buildings");
            cfl_layer.Draw(displayDevice, mapViewport, displayOffset, wrapAround, pixelZoom);
        }

        public static bool isCollidingPosition_Prefix(Farm __instance, ref bool __result, Rectangle position, xTile.Dimensions.Rectangle viewport, bool isFarmer, int damagesFarmer, bool glider, Character character, bool pathfinding, bool projectile = false, bool ignoreCharacterRequirement = false)
        {
            if( CustomFarm.IsCFLMapSelected()
                && Bridge.PassableBridgeAreas.Exists(el => characterIntersects(el, position, character.FacingDirection))) {
                __result = false;
                return false;
            }

            return true;
        }

        private static bool characterIntersects(Rectangle area, Rectangle character, int facingDirection)
        {
            //For those fatsos out there.
            int leeway = character.Width > 64 ? Math.Max(character.Width - 64, 16) : 0;

            return area.X - leeway <= character.X
                && area.X + area.Width - character.Width >= character.X
                && area.Y <= character.Y
                && area.Y + area.Height +  - character.Height >= character.Y;
        }

        public static void DayStarted(object sender, DayStartedEventArgs e)
        {
            if (!CustomFarm.IsCFLMapSelected())
                return;

            Farm farm = Game1.getFarm();
            Bridge.addBridgesTilesheet(farm);

            CustomFarm customFarm = CustomFarm.getCurrentCustomFarm();
            Bridge.PassableBridgeAreas = new List<Rectangle>();

            foreach (Bridge bridge in customFarm.Bridges)
                bridge.setTiles(farm);
        }

    }
}
