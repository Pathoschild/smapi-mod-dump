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

using MoreFertilizers.Framework;

using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace MoreFertilizers.HarmonyPatches.PrismaticFertilizer;

/// <summary>
/// Holds patches to draw the prismatic fertilizer in garden pots.
/// </summary>
[HarmonyPatch(typeof(Crop))]
internal static class CropInPotTranspiler
{
    [MethodImpl(TKConstants.Hot)]
    private static Color GetPrismaticColor(Color prevcolor, Vector2 tileLocation)
    {
        if (prevcolor != Color.White && Game1.currentLocation?.Objects?.TryGetValue(tileLocation, out SObject? obj) == true && obj is IndoorPot pot
            && pot.hoeDirt.Value is HoeDirt dirt && dirt.modData?.GetBool(CanPlaceHandler.PrismaticFertilizer) == true)
        {
            return Utility.GetPrismaticColor((int)(tileLocation.X + tileLocation.Y), 1);
        }
        return prevcolor;
    }

    [HarmonyPatch(nameof(Crop.drawWithOffset))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);

            helper.FindLast(new CodeInstructionWrapper[]
            {
                OpCodes.Ldarg_0,
                (OpCodes.Ldfld, typeof(Crop).GetCachedField(nameof(Crop.tintColor), ReflectionCache.FlagTypes.InstanceFlags)),
                OpCodes.Call, // this is an op_implicit
            })
            .Advance(3)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_2), // tile location
                new (OpCodes.Call, typeof(CropInPotTranspiler).GetCachedMethod(nameof(GetPrismaticColor), ReflectionCache.FlagTypes.StaticFlags)),
            });

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling {original.FullDescription()}:\n\n{ex}", LogLevel.Error);
            original.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
}
