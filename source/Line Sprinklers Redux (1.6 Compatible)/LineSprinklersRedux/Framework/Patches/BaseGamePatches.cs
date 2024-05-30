/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rtrox/LineSprinklersRedux
**
*************************************************/


using HarmonyLib;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;

namespace LineSprinklersRedux.Framework.Patches
{
    internal static class BaseGamePatches
    {
        internal static void Apply(Harmony harmony)
        {
            try
            {
                harmony.Patch(
                    original: AccessTools.DeclaredMethod(typeof(SObject), nameof(SObject.GetSprinklerTiles)),
                    prefix: new HarmonyMethod(typeof(BaseGamePatches), nameof(Object_GetSprinklerTiles_Prefix)),
                    postfix: new HarmonyMethod(typeof(BaseGamePatches), nameof(Object_GetSprinklerTiles_Postfix))

                );
                harmony.Patch(
                    original: AccessTools.DeclaredMethod(typeof(SObject), nameof(SObject.IsSprinkler)),
                    postfix: new HarmonyMethod(typeof(BaseGamePatches), nameof(Object_IsSprinkler_Postfix))
                );
                harmony.Patch(
                    original: AccessTools.DeclaredMethod(typeof(SObject), nameof(SObject.IsInSprinklerRangeBroadphase)),
                    prefix: new HarmonyMethod(typeof(BaseGamePatches), nameof(Object_IsInSprinklerRangeBroadphase_Prefix))
                );
                harmony.Patch(
                    original: AccessTools.DeclaredMethod(typeof(SObject), nameof(SObject.GetModifiedRadiusForSprinkler)),
                    prefix: new HarmonyMethod(typeof(BaseGamePatches), nameof(Object_GetModifiedRadiusForSprinkler_Prefix))
                );
                harmony.Patch(
                    original: AccessTools.DeclaredMethod(typeof(SObject), nameof(SObject.ApplySprinklerAnimation)),
                    prefix: new HarmonyMethod(typeof(BaseGamePatches), nameof(Object_ApplySprinklerAnimation_Prefix))
                    );
                harmony.Patch(
                    original: AccessTools.Method(typeof(SObject), nameof(SObject.draw), [typeof(SpriteBatch), typeof(int), typeof(int), typeof(float)]),
                    prefix: new HarmonyMethod(typeof(BaseGamePatches), nameof(Object_draw_Prefix)),
                    postfix: new HarmonyMethod(typeof(BaseGamePatches), nameof(Object_draw_Postfix))
                );
                harmony.Patch(
                    original: AccessTools.Method(typeof(SObject), nameof(SObject.placementAction)),
                    prefix: new HarmonyMethod(typeof(BaseGamePatches), nameof(Object_placementAction_Prefix))
                );
            }
            catch (Exception e)
            {
                ModEntry.Mon!.Log($"Could not Patch LineSprinklers: \n{e}", LogLevel.Error);
            }
        }

        private static bool Object_GetSprinklerTiles_Prefix(SObject __instance, ref List<Vector2> __result)
        {
            if (!Sprinkler.IsLineSprinkler(__instance))
            {
                return true;
            }
            __result = Sprinkler.GetCoverage(__instance).ToList();
            return false;
        }

        private static void Object_GetSprinklerTiles_Postfix(SObject __instance, ref List<Vector2> __result)
        {
            if (!Sprinkler.IsLineSprinkler(__instance))
            {
                return;
            }
            __result = Sprinkler.GetCoverage(__instance).ToList();
        }

        private static void Object_IsSprinkler_Postfix(SObject __instance, ref bool __result)
        {
            __result = __result || Sprinkler.IsLineSprinkler(__instance);
        }

        private static bool Object_IsInSprinklerRangeBroadphase_Prefix(SObject __instance, Vector2 target, ref bool __result)
        {
            if (!Sprinkler.IsLineSprinkler(__instance))
            {
                return true;
            }
            __result = Sprinkler.GetCoverage(__instance).ToList().Contains(target);
            return false;
        }

        private static bool Object_GetModifiedRadiusForSprinkler_Prefix(SObject __instance, ref int __result)
        {
            if (!Sprinkler.IsLineSprinkler(__instance))
            {
                return true;
            }
            // In `DayUpdate(), the base game ensures that the modified radius for the object is at least 0
            // as -1 is returned for non-sprinkler objects. I'm not sure why, as it's also checking `IsSprinkler`.
            __result = 0;
            return false;
        }

        private static bool Object_ApplySprinklerAnimation_Prefix(SObject __instance)
        {
            if (Sprinkler.IsLineSprinkler(__instance))
            {
                Sprinkler.ApplySprinklerAnimation(__instance);
                return false;
            }
            return true;
        }

        private static bool Object_draw_Prefix(SObject __instance, SpriteBatch spriteBatch, int x, int y, float alpha)
        {
            if (Sprinkler.IsLineSprinkler(__instance))
            {
                Sprinkler.SetSpriteFromRotation(__instance);
            }
            return true;
        }

        private static void Object_draw_Postfix(SObject __instance, SpriteBatch spriteBatch, int x, int y, float alpha)
        {
            if (Sprinkler.IsLineSprinkler(__instance))
            {
                Sprinkler.DrawAttachments(__instance, spriteBatch, x, y, alpha);
            }
        }

        private static bool Object_placementAction_Prefix(SObject __instance, GameLocation location, int x, int y, Farmer who, ref bool __result)
        {
            if (Sprinkler.IsLineSprinkler(__instance) && location.doesTileHavePropertyNoNull(x / 64, y / 64, "NoSprinklers", "Back") == "T")
            {

                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:NoSprinklers"));
                __result = false;
                return false;
            }
            return true;
        }
    }
}
