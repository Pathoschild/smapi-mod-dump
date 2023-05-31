/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CropWalker
{
    internal class ModEntry : Mod
    {
        private Harmony harmony;
        private static Config IConfig;
        private static IMonitor IMonitor;

        public override void Entry(IModHelper helper)
        {
            harmony = new(Helper.ModRegistry.ModID);
            IMonitor = Monitor;
            IConfig = Helper.ReadConfig<Config>();

            harmony.Patch(
                original: AccessTools.Method(typeof(HoeDirt), nameof(HoeDirt.doCollisionAction)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.doCollisionActionHoeDirt_postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(HoeDirt), nameof(HoeDirt.isPassable)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.isPassable_prefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Grass), nameof(Grass.doCollisionAction)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.doCollisionActionGrass_postfix))
            );
        }

        private static void doCollisionActionHoeDirt_postfix(Rectangle positionOfCollider, int speedOfCollision, Vector2 tileLocation, Character who, GameLocation location, HoeDirt __instance)
        {
            try
            {
                if (!IConfig.FastCrops || (__instance.crop is null || __instance.crop.currentPhase.Value == 0 || who is not Farmer || !((who as Farmer)!.running)))
                    return;
                (who as Farmer)!.temporarySpeedBuff = 0f;
                return;
            }
            catch (Exception ex) { IMonitor.Log($"Failed patching {nameof(HoeDirt.doCollisionAction)}", LogLevel.Error); IMonitor.Log($"{ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}"); return; }
        }

        private static bool isPassable_prefix(Character c, HoeDirt __instance, ref bool __result)
        {
            try
            {
                if (!IConfig.PassableTrellis)
                    return true;
                __result = true;
                return false;
            }
            catch (Exception ex) { IMonitor.Log($"Failed patching {nameof(HoeDirt.isPassable)}", LogLevel.Error); IMonitor.Log($"{ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}"); return true; }
        }

        private static void doCollisionActionGrass_postfix(Rectangle positionOfCollider, int speedOfCollision, Vector2 tileLocation, Character who, GameLocation location, Grass __instance)
        {
            try
            {
                //                                          Not for cobwebs, suffer like the rest of us
                if (!IConfig.FastGrass || (__instance.grassType.Value == Grass.cobweb || who is not Farmer))
                    return;
                (who as Farmer)!.temporarySpeedBuff = 0f;
                return;
            }
            catch (Exception ex) { IMonitor.Log($"Failed patching {nameof(Grass.doCollisionAction)}", LogLevel.Error); IMonitor.Log($"{ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}"); return; }
        }
    }
}
