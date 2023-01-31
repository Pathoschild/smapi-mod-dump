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
using Microsoft.Xna.Framework.Graphics;

using MoreFertilizers.Framework;

using StardewValley.TerrainFeatures;

namespace MoreFertilizers.HarmonyPatches.BushFertilizers;

/// <summary>
/// Holds transpiler against the bush draw function.
/// </summary>
[HarmonyPatch(typeof(Bush))]
internal static class BushDrawTranspiler
{
    [MethodImpl(TKConstants.Hot)]
    private static Color AdjustColorForBush(Color prevColor, Bush bush)
    {
        if (!ModEntry.Config.RecolorBushes)
        {
            return prevColor;
        }
        if (bush.size.Value == Bush.greenTeaBush)
        {
            if (bush.getAge() < Bush.daysToMatureGreenTeaBush && bush.modData?.GetBool(CanPlaceHandler.RapidBush) == true)
            {
                return Color.Orange;
            }
            else if (bush.modData?.GetBool(CanPlaceHandler.MiraculousBeverages) == true)
            {
                return Color.LightCoral;
            }
            if (bush.modData?.GetBool(CanPlaceHandler.BountifulBush) == true)
            {
                return Color.LawnGreen;
            }
        }
        else if (bush.size.Value == Bush.mediumBush)
        {
#warning - fix this to be less dumb in 1.6
            if (bush.modData?.GetBool(CanPlaceHandler.BountifulBush) == true)
            {
                string season = (bush.overrideSeason.Value == -1)
                    ? Game1.GetSeasonForLocation(bush.currentLocation)
                    : Utility.getSeasonNameFromNumber(bush.overrideSeason.Value);

                if (season.Equals("spring", StringComparison.OrdinalIgnoreCase))
                {
                    return Color.LawnGreen;
                }
                else if (season.Equals("fall", StringComparison.OrdinalIgnoreCase))
                {
                    return Color.Goldenrod;
                }
            }
        }
        return prevColor;
    }

    [HarmonyPatch(nameof(Bush.draw), new[] { typeof(SpriteBatch), typeof(Vector2) })]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);

            helper.FindNext(new CodeInstructionWrapper[]
            { // Find the reference to the bush's texture, not shadow textures.
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldsfld, typeof(Bush).GetCachedField(nameof(Bush.texture), ReflectionCache.FlagTypes.StaticFlags)),
            })
            .FindNext(new CodeInstructionWrapper[]
            { // Find where the color is loaded.
                new(OpCodes.Call, typeof(Color).GetCachedProperty(nameof(Color.White), ReflectionCache.FlagTypes.StaticFlags).GetGetMethod()),
            })
            .Advance(1)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(BushDrawTranspiler).GetCachedMethod(nameof(AdjustColorForBush), ReflectionCache.FlagTypes.StaticFlags)),
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