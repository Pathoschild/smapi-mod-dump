/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/CombatDummy
**
*************************************************/

using CombatDummy.Framework.Objects;
using CombatDummy.Framework.Utilities;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using Object = StardewValley.Object;

namespace CombatDummy.Framework.Patches.Entities
{
    internal class ObjectPatch : PatchTemplate
    {
        private readonly Type _object = typeof(Object);

        internal ObjectPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            if (Type.GetType("DynamicGameAssets.Game.CustomBigCraftable, DynamicGameAssets") is Type dgaCraftableType && dgaCraftableType != null)
            {
                harmony.Patch(AccessTools.Method(dgaCraftableType, nameof(Object.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
            }

            harmony.Patch(AccessTools.Method(_object, nameof(Object.updateWhenCurrentLocation), new[] { typeof(GameTime), typeof(GameLocation) }), postfix: new HarmonyMethod(GetType(), nameof(UpdateWhenCurrentLocationPostfix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Object.onExplosion), new[] { typeof(Farmer), typeof(GameLocation) }), prefix: new HarmonyMethod(GetType(), nameof(OnExplosionPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Object.isPassable), null), postfix: new HarmonyMethod(GetType(), nameof(IsPassablePostfix)));
        }

        private static void UpdateWhenCurrentLocationPostfix(Object __instance, GameTime time, GameLocation environment)
        {
            if (PracticeDummy.IsValid(__instance))
            {
                PracticeDummy.Update(__instance, time, environment);
            }
            else if (KnockbackDummy.IsValid(__instance))
            {
                KnockbackDummy.Update(__instance, time, environment);
            }
            else if (MaxHitDummy.IsValid(__instance))
            {
                MaxHitDummy.Update(__instance, time, environment);
            }
        }

        private static void IsPassablePostfix(Object __instance, ref bool __result)
        {
            if (KnockbackDummy.IsValid(__instance))
            {
                int knockbackCountdown = 0;
                if (__instance.modData.ContainsKey(ModDataKeys.DUMMY_KNOCKBACK_COUNTDOWN) is true)
                {
                    knockbackCountdown = Int32.Parse(__instance.modData[ModDataKeys.DUMMY_KNOCKBACK_COUNTDOWN]);
                }

                if (knockbackCountdown != int.MaxValue)
                {
                    __result = true;
                }
            }
        }

        private static bool OnExplosionPrefix(Object __instance, ref bool __result, Farmer who, GameLocation location)
        {
            if (PracticeDummy.IsValid(__instance) || KnockbackDummy.IsValid(__instance) || MaxHitDummy.IsValid(__instance))
            {
                __result = false;
                return false;
            }

            return true;
        }

        private static bool DrawPrefix(Object __instance, SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            if (PracticeDummy.IsValid(__instance))
            {
                PracticeDummy.Draw(__instance, spriteBatch, x, y, alpha);
                return false;
            }
            else if (KnockbackDummy.IsValid(__instance))
            {
                KnockbackDummy.Draw(__instance, spriteBatch, x, y, alpha);
                return false;
            }
            else if (MaxHitDummy.IsValid(__instance))
            {
                MaxHitDummy.Draw(__instance, spriteBatch, x, y, alpha);
                return false;
            }

            return true;
        }
    }
}
