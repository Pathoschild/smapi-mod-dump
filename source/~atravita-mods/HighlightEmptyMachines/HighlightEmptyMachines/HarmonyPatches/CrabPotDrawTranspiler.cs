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
using AtraShared.ConstantsAndEnums;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;

namespace HighlightEmptyMachines.HarmonyPatches;

/// <summary>
/// Hold patches against crab pots.
/// </summary>
[HarmonyPatch(typeof(CrabPot))]
internal class CrabPotDrawTranspiler
{
    [MethodImpl(TKConstants.Hot)]
    private static Color CrabPotNeedsInputColor(CrabPot obj)
        => ModEntry.Config.VanillaMachines[VanillaMachinesEnum.CrabPot]
            && obj.bait.Value is null && obj.heldObject.Value is null ? ModEntry.Config.EmptyColor : Color.White;

#pragma warning disable SA1116 // Split parameters should start on line after declaration. Reviewed
    [HarmonyPatch(nameof(CrabPot.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) })]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Call, typeof(Game1).GetCachedMethod<xTile.Dimensions.Rectangle, Vector2>(nameof(Game1.GlobalToLocal), ReflectionCache.FlagTypes.StaticFlags)),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new (OpCodes.Call, typeof(Color).GetCachedProperty(nameof(Color.White), ReflectionCache.FlagTypes.StaticFlags).GetGetMethod()),
            })
            .GetLabels(out IList<Label> colorLabels, clear: true)
            .ReplaceInstruction(OpCodes.Call, typeof(CrabPotDrawTranspiler).GetCachedMethod(nameof(CrabPotNeedsInputColor), ReflectionCache.FlagTypes.StaticFlags))
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
            }, withLabels: colorLabels);
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling {original.FullDescription()}:\n\n{ex}", LogLevel.Error);
            original.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
#pragma warning restore SA1116 // Split parameters should start on line after declaration
}