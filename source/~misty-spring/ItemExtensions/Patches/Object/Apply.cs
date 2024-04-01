/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using HarmonyLib;
using ItemExtensions.Models;
using ItemExtensions.Models.Contained;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using Object = StardewValley.Object;

namespace ItemExtensions.Patches;

public partial class ObjectPatches
{
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);
    
    internal static void Apply(Harmony harmony)
    {
        Log($"Applying Harmony patch \"{nameof(ObjectPatches)}\": prefixing SDV method \"Object.IsHeldOverHead()\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(Object), nameof(Object.IsHeldOverHead)),
            prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(Pre_IsHeldOverHead))
        );
        
        Log($"Applying Harmony patch \"{nameof(ObjectPatches)}\": prefixing SDV method \"Object.maximumStackSize()\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(Object), nameof(Object.maximumStackSize)),
            prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(Pre_maximumStackSize))
        );
        
        Log($"Applying Harmony patch \"{nameof(ObjectPatches)}\": postfixing SDV method \"Object.actionWhenBeingHeld(Farmer)\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(Object), nameof(Object.actionWhenBeingHeld)),
            postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(Post_actionWhenBeingHeld))
        );
        
        Log($"Applying Harmony patch \"{nameof(ObjectPatches)}\": postfixing SDV method \"Object.actionWhenStopBeingHeld(Farmer)\".");
        harmony.Patch(
          original: AccessTools.Method(typeof(Object), nameof(Object.actionWhenStopBeingHeld)),
          postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(Post_actionWhenStopBeingHeld))
        );
        
        Log($"Applying Harmony patch \"{nameof(ObjectPatches)}\": postfixing SDV method \"Object.performRemoveAction()\".");
        harmony.Patch(
          original: AccessTools.Method(typeof(Object), nameof(Object.performRemoveAction)),
          postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(Post_performRemoveAction))
        );
        
        Log($"Applying Harmony patch \"{nameof(ObjectPatches)}\": postfixing SDV method \"Object.dropItem(GameLocation, Vector2, Vector2)\".");
        harmony.Patch(
          original: AccessTools.Method(typeof(Object), nameof(Object.dropItem)),
          postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(Post_dropItem))
        );
        
        Log($"Applying Harmony patch \"{nameof(ObjectPatches)}\": postfixing SDV method \"Object.performToolAction\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(Object), "performToolAction"),
            postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(Postfix_performToolAction))
        );

        Log($"Applying Harmony patch \"{nameof(ObjectPatches)}\": postfixing SDV method \"Object.initializeLightSource\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(Object), "initializeLightSource"),
            postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(Post_initializeLightSource))
        );

        Log($"Applying Harmony patch \"{nameof(ObjectPatches)}\": prefixing SDV method \"Object.IsHeldOverHead()\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(Object), nameof(Object.IsHeldOverHead)),
            prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(Pre_IsHeldOverHead))
        );
        
        Log($"Applying Harmony patch \"{nameof(ObjectPatches)}\": prefixing SDV method \"Object.onExplosion\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(Object), nameof(Object.onExplosion)),
            prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(Pre_onExplosion))
        );
    }
    
    private static void Post_initializeLightSource(Object __instance, Vector2 tileLocation, bool mineShaft = false)
    {
        try
        {
            if (__instance.QualifiedItemId is null)
                return;

            LightData data;
            if (!ModEntry.Data.TryGetValue(__instance.QualifiedItemId, out var mainData))
            {
                if (ModEntry.Ores.TryGetValue(__instance.ItemId, out var resData) == false)
                    return;
                else
                    data = resData.Light;
            }
            else
            {
                data = mainData.Light;
            }

            if (data is null)
                return;

            var color = data.GetColor();

            var rad = data.Size;
            var position = new Vector2(tileLocation.X * 64f + 16f, tileLocation.Y * 64f + 16f);

            //var identifier = (int)(tileLocation.X * 2000f + tileLocation.Y);
            __instance.lightSource = new LightSource(4, position, rad, color);
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
        }
    }
    
    internal static void Post_new(ref Object __instance, string itemId, int initialStack, bool isRecipe = false,
        int price = -1, int quality = 0)
    {
        try
        {
            if (!ModEntry.Ores.TryGetValue(__instance.ItemId, out var resource))
                return;

            if (resource == null || resource == new ResourceData())
                return;

            Log("Created item has resource data. Adding...");

            __instance.MinutesUntilReady = resource.Health; //mainData.Resource.MinToolLevel + 1;

            if (__instance.tempData is null)
            {
                __instance.tempData = new Dictionary<string, object>
                {
                    { "Health", resource.Health }
                };
            }
            else
            {
                __instance.tempData.Add("Health", resource.Health);
            }

            //__instance.Fragility = Object.fragility_Delicate;
            __instance.modData["Esca.FarmTypeManager/CanBePickedUp"] = "false";
            __instance.IsSpawnedObject = false;

            __instance.CanBeGrabbed = false;
            __instance.CanBeSetDown = true;
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
        }
    }

    /*
    private static void Post_canBeGivenAsGift(Object __instance, ref bool __result)
    {
        if (__instance is Furniture)
            __result = true;
    }*/
}