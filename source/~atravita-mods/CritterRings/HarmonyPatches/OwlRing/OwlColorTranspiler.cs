/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;

using System.Runtime.CompilerServices;

using AtraBase.Toolkit;

using AtraCore.Framework.ReflectionManager;

using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;

using HarmonyLib;

using Microsoft.Xna.Framework;

using StardewValley.BellsAndWhistles;

namespace CritterRings.HarmonyPatches.OwlRing;

/// <summary>
/// Patches owls to try to see their pretty sprite in the day.
/// </summary>
[HarmonyPatch(typeof(Owl))]
internal static class OwlColorTranspiler
{
    [MethodImpl(TKConstants.Hot)]
    private static Color GetColorForTime(Color prevcolor)
    {
        if (Game1.isDarkOut())
        {
            return prevcolor;
        }
        if (Game1.isStartingToGetDarkOut())
        {
            return Color.LightSteelBlue;
        }
        return Color.White;
    }

    [HarmonyPatch(nameof(Owl.drawAboveFrontLayer))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                (OpCodes.Call, typeof(Color).GetCachedProperty(nameof(Color.MediumBlue), ReflectionCache.FlagTypes.StaticFlags).GetGetMethod()),
            })
            .Advance(1)
            .Insert(new CodeInstruction[]
            {
                new (OpCodes.Call, typeof(OwlColorTranspiler).GetCachedMethod(nameof(GetColorForTime), ReflectionCache.FlagTypes.StaticFlags)),
            });

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into error transpiling {original.Name}\n\n{ex}", LogLevel.Error);
            original.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
}
