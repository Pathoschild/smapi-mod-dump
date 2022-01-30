/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/XxHarvzBackxX/recyclableCola
**
*************************************************/

using System;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using Object = StardewValley.Object;

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    internal Harmony Harmony;
    public static Random Random = new();
    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        Harmony = new Harmony(ModManifest.UniqueID);
        Harmony.Patch(AccessTools.Method(typeof(Object), nameof(Object.performObjectDropInAction)),
            postfix: new HarmonyMethod(AccessTools.Method(typeof(ModEntry), nameof(Postfix))));
    }

    public static void Postfix(ref bool __result, ref Object __instance, ref Item dropInItem, ref bool probe, ref Farmer who)
    {
        if (__instance?.Name is "Recycling Machine")
        {
            if (dropInItem?.Name is "Joja Cola")
            {
                __instance.heldObject.Value = new Object(380, Random.Next(1, 4));
                if (!probe)
                {
                    who.currentLocation.playSound("trashcan");
                    __instance.MinutesUntilReady = 60;
                    Game1.stats.PiecesOfTrashRecycled++;
                }
                __result = true;
            }
        }
    }
}