/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ferdaber/sdv-mods
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace ApiTest
{
    public class ModEntry : Mod
    {
        private static IMonitor ModMonitor;
        public override void Entry(IModHelper helper)
        {
            //Helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            //Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            ModMonitor = Monitor;
            AddPatches();
        }

        private void GameLoop_DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
        }

        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
        }

        private void AddPatches()
        {
            //var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            //harmony.Patch(
            //    original: AccessTools.Method(typeof(Tool), nameof(RenovateMenu.draw)),
            //    prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.patch_Tool_draw))
            //);
            //harmony.Patch(
            //    original: AccessTools.Method(typeof(Tool), nameof(CarpenterMenu.draw)),
            //    prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.patch_Tool_draw))
            //);
            //harmony.Patch(
            //    original: AccessTools.Method(typeof(Tool), nameof(Tool.draw)),
            //    prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.patch_Tool_draw))
            //);
            //harmony.Patch(
            //    original: AccessTools.Method(typeof(Game1), nameof(Game1.drawMouseCursor)),
            //    prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.patch_drawMouseCursor))
            //);
            //harmony.Patch(
            //    original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.drawPlacementBounds)),
            //    prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.patch_drawPlacementBounds))
            //);
        }

        public static bool patch_Tool_draw()
        {
            try
            {
                return false;
            }
            catch (Exception ex)
            {
                ModMonitor.Log($"Error in patch:\n{ex}", LogLevel.Error);
                return true;
            }
        }

        public static bool patch_drawMouseCursor(Game1 __instance)
        {
            try
            {
                var obj = Game1.player.ActiveObject;
                if (obj != null && obj.bigCraftable.Value && obj.ParentSheetIndex == 10)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                ModMonitor.Log($"Error in patch:\n{ex}", LogLevel.Error);
                return true;
            }
        }

        public static bool patch_drawPlacementBounds(StardewValley.Object __instance, SpriteBatch spriteBatch, GameLocation location)
        {
            try
            {
                if (__instance.bigCraftable.Value && __instance.ParentSheetIndex == 10)
                {
                    return false;
                } else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                ModMonitor.Log($"Error in patch:\n{ex}", LogLevel.Error);
                return true;
            }
        }
    }
}
