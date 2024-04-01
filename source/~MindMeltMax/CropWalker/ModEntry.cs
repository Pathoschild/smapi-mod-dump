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
        private static Config IConfig;
        private static IMonitor IMonitor;

        public override void Entry(IModHelper helper)
        {
            Harmony harmony = new(Helper.ModRegistry.ModID);
            IMonitor = Monitor;
            IConfig = Helper.ReadConfig<Config>();

            harmony.Patch(
                original: AccessTools.Method(typeof(HoeDirt), nameof(HoeDirt.doCollisionAction)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(doCollisionActionHoeDirtPostfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(HoeDirt), nameof(HoeDirt.isPassable)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(isPassablePrefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Grass), nameof(Grass.doCollisionAction)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(doCollisionActionGrassPostfix))
            );
        }

        private static void doCollisionActionHoeDirtPostfix(Rectangle positionOfCollider, int speedOfCollision, Vector2 tileLocation, Character who, HoeDirt __instance)
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

        private static bool isPassablePrefix(Character c, ref bool __result)
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

        private static void doCollisionActionGrassPostfix(Rectangle positionOfCollider, int speedOfCollision, Vector2 tileLocation, Character who, Grass __instance)
        {
            try
            {
                if (!IConfig.FastGrass || who is not Farmer)
                    return;
                (who as Farmer)!.temporarySpeedBuff = 0f;
                return;
            }
            catch (Exception ex) { IMonitor.Log($"Failed patching {nameof(Grass.doCollisionAction)}", LogLevel.Error); IMonitor.Log($"{ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}"); return; }
        }
    }
}
