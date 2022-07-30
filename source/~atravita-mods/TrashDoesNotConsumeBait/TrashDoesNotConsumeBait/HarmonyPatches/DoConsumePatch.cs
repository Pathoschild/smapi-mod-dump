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
using AtraBase.Toolkit.Reflection;
using AtraCore.Framework.ReflectionManager;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using Microsoft.Toolkit.Diagnostics;
using Netcode;
using StardewValley.Tools;

namespace TrashDoesNotConsumeBait.HarmonyPatches;

/// <summary>
/// Applies the harmony patch against DoDoneFishing that consumes the bait/tackle.
/// </summary>
[HarmonyPatch(typeof(FishingRod))]
internal static class DoConsumePatch
{
    [MethodImpl(TKConstants.Hot)]
    private static float GetNormalChance() => ModEntry.Config.ConsumeChanceNormal;

    [MethodImpl(TKConstants.Hot)]
    private static float GetPreservingChance() => ModEntry.Config.ConsumeChancePreserving;

    /// <summary>
    /// Gets the replacement bait stack, or returns null for not found.
    /// Also handles messaging the player.
    /// </summary>
    /// <param name="original">Original bait.</param>
    /// <returns>Replacement bait, or null if not found.</returns>
    [MethodImpl(TKConstants.Hot)]
    private static SObject? GetReplacementBait(SObject original)
    {
        try
        {
            if (ModEntry.Config.AutomaticRefill && original is not null)
            {
                int? replacementIndex = null;
                for (int i = 0; i < Game1.player.Items.Count; i++)
                {
                    if (Game1.player.Items[i] is not SObject obj || obj.bigCraftable.Value || obj.Category != SObject.baitCategory)
                    {
                        continue;
                    }
                    else if (Utility.IsNormalObjectAtParentSheetIndex(obj, original.ParentSheetIndex))
                    {
                        replacementIndex = i;
                        break;
                    }
                    else if (!ModEntry.Config.SameBaitOnly)
                    {
                        replacementIndex ??= i;
                    }
                }
                if (replacementIndex is not null && Game1.player.Items[replacementIndex.Value] is SObject returnObj)
                {
                    Game1.player.Items[replacementIndex.Value] = null;
                    Game1.showGlobalMessage(original.ParentSheetIndex == returnObj.ParentSheetIndex
                        ? I18n.BaitReplacedSame(original.DisplayName)
                        : I18n.BaitReplaced(original.DisplayName, returnObj.DisplayName));
                    return returnObj;
                }
            }
            Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14085"));
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod failed while trying to replace bait.\n\n{ex}", LogLevel.Error);
        }
        return null;
    }

    /// <summary>
    /// Gets the replacement tackle, or returns null for not found.
    /// Also handles messaging the player.
    /// </summary>
    /// <param name="original">Original tackle.</param>
    /// <returns>Replacement tackle, or null if not found.</returns>
    [MethodImpl(TKConstants.Hot)]
    private static SObject? GetReplacementTackle(SObject original)
    {
        try
        {
            if (ModEntry.Config.AutomaticRefill && original is not null)
            {
                int? replacementIndex = null;
                for (int i = 0; i < Game1.player.Items.Count; i++)
                {
                    if (Game1.player.Items[i] is not SObject obj || obj.bigCraftable.Value || obj.Category != SObject.tackleCategory)
                    {
                        continue;
                    }
                    else if (Utility.IsNormalObjectAtParentSheetIndex(obj, original.ParentSheetIndex))
                    {
                        replacementIndex = i;
                        break;
                    }
                    else if (!ModEntry.Config.SameTackleOnly)
                    {
                        replacementIndex ??= i;
                    }
                }
                if (replacementIndex is not null && Game1.player.Items[replacementIndex.Value] is SObject returnObj)
                {
                    Game1.player.Items[replacementIndex.Value] = null;
                    Game1.showGlobalMessage(original.ParentSheetIndex == returnObj.ParentSheetIndex
                        ? I18n.TackleReplacedSame(original.DisplayName)
                        : I18n.TackleReplaced(original.DisplayName, returnObj.DisplayName));
                    return returnObj;
                }
            }
            Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14086"));
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod failed while trying to replace tackle.\n\n{ex}", LogLevel.Error);
        }
        return null;
    }

    /*****************************************************
     * Replaces the two constants at the start with calls to static functions. Also:
     *
     * if (base.attachments[0] != null && Game1.random.NextDouble() < (double)consumeChance)
     * to
     * if (!this.lastCatchWasJunk && base.attachments[0] != null && Game1.random.NextDouble() < (double)consumeChance)
     *
     * Also inserts calls to replace tackles/bait.
     * ***************************************************/
#pragma warning disable SA1116 // Split parameters should start on line after declaration
    [HarmonyPatch("doDoneFishing")]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);

            // We want to affect the chances of consuming bait, which is currently the first float local.
            int index = helper.GetIndexOfLocal(typeof(float));
            if (index == -1)
            {
                ThrowHelper.ThrowInvalidOperationException($"Tried to find first float local, failed.");
                return null;
            }
            CodeInstruction stloc = ILHelper.GetStLoc(index);
            helper.FindNext(new CodeInstructionWrapper[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld),
                    new(OpCodes.Callvirt, typeof(Farmer).GetCachedProperty(nameof(Farmer.IsLocalPlayer), ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                    new(OpCodes.Brfalse),
                })
            .FindNext(new CodeInstructionWrapper[]
                {
                    new(OpCodes.Ldc_R4, 1f),
                    new(stloc),
                })
            .ReplaceInstruction(new(OpCodes.Call, typeof(DoConsumePatch).GetCachedMethod(nameof(DoConsumePatch.GetNormalChance), ReflectionCache.FlagTypes.StaticFlags)), keepLabels: true)
            .FindNext(new CodeInstructionWrapper[]
                {
                    new(OpCodes.Ldc_R4, 0.5f),
                    new(stloc),
                })
            .ReplaceInstruction(new(OpCodes.Call, typeof(DoConsumePatch).GetCachedMethod(nameof(DoConsumePatch.GetPreservingChance), ReflectionCache.FlagTypes.StaticFlags)), keepLabels: true)
            .FindNext(new CodeInstructionWrapper[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, typeof(Tool).GetCachedField(nameof(Tool.attachments), ReflectionCache.FlagTypes.InstanceFlags)),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Callvirt),
                    new(OpCodes.Brfalse_S),
                })
            .Push()
            .Advance(4)
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .DefineAndAttachLabel(out Label label)
            .Pop()
            .GetLabels(out IList<Label> labelsToMove, clear: true)
            .Insert(new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, typeof(FishingRod).GetCachedField("lastCatchWasJunk", ReflectionCache.FlagTypes.InstanceFlags)),
                    new(OpCodes.Brtrue_S, label),
                }, withLabels: labelsToMove)
            .FindNext(new CodeInstructionWrapper[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, typeof(Tool).GetCachedField(nameof(Tool.attachments), ReflectionCache.FlagTypes.InstanceFlags)),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Ldnull),
                    new(OpCodes.Callvirt, typeof(NetArray<SObject, NetRef<SObject>>).GetCachedProperty("Item", ReflectionCache.FlagTypes.InstanceFlags).GetSetMethod()),
                })
            .Advance(3)
            .GetLabels(out IList<Label> labelsToMove2, clear: true)
            .ReplaceInstruction(OpCodes.Call, typeof(DoConsumePatch).GetCachedMethod(nameof(DoConsumePatch.GetReplacementBait), ReflectionCache.FlagTypes.StaticFlags))
            .Insert(new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, typeof(Tool).GetCachedField(nameof(Tool.attachments), ReflectionCache.FlagTypes.InstanceFlags)),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Call, typeof(NetArray<SObject, NetRef<SObject>>).GetCachedProperty("Item", ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                }, withLabels: labelsToMove2)
            .FindNext(new CodeInstructionWrapper[]
                {
                    new (OpCodes.Ldsfld),
                    new (OpCodes.Ldstr, "Strings\\StringsFromCSFiles:FishingRod.cs.14085"),
                    new (OpCodes.Callvirt),
                    new (OpCodes.Call, typeof(Game1).GetCachedMethod(nameof(Game1.showGlobalMessage), ReflectionCache.FlagTypes.StaticFlags)),
                })
            .Remove(4)
            .FindNext(new CodeInstructionWrapper[]
                {
                    new(OpCodes.Ldsfld, typeof(FishingRod).GetCachedField(nameof(FishingRod.maxTackleUses), ReflectionCache.FlagTypes.StaticFlags)),
                })
            .FindNext(new CodeInstructionWrapper[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, typeof(Tool).GetCachedField(nameof(Tool.attachments), ReflectionCache.FlagTypes.InstanceFlags)),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Ldnull),
                    new(OpCodes.Callvirt, typeof(NetArray<SObject, NetRef<SObject>>).GetCachedProperty("Item", ReflectionCache.FlagTypes.InstanceFlags).GetSetMethod()),
                })
            .Advance(3)
            .GetLabels(out IList<Label> labelsToMove3, clear: true)
            .ReplaceInstruction(OpCodes.Call, typeof(DoConsumePatch).GetCachedMethod(nameof(DoConsumePatch.GetReplacementTackle), ReflectionCache.FlagTypes.StaticFlags))
            .Insert(new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, typeof(Tool).GetCachedField(nameof(Tool.attachments), ReflectionCache.FlagTypes.InstanceFlags)),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Call, typeof(NetArray<SObject, NetRef<SObject>>).GetCachedProperty("Item", ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                }, withLabels: labelsToMove3)
            .FindNext(new CodeInstructionWrapper[]
                {
                    new (OpCodes.Ldsfld),
                    new (OpCodes.Ldstr, "Strings\\StringsFromCSFiles:FishingRod.cs.14086"),
                    new (OpCodes.Callvirt),
                    new (OpCodes.Call, typeof(Game1).GetCachedMethod(nameof(Game1.showGlobalMessage), ReflectionCache.FlagTypes.StaticFlags)),
                })
            .Remove(4);

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling doDoneFishing:\n\n{ex}", LogLevel.Error);
        }
        return null;
    }
#pragma warning restore SA1116 // Split parameters should start on line after declaration
}