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
using AtraCore.Framework.ReflectionManager;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using StardewValley.Locations;

namespace StopRugRemoval.HarmonyPatches.Niceties;

/// <summary>
/// Transpilers to get the game to use the display name over the internal name.
/// </summary>
internal static class UseDisplayNameTranspilers
{
#pragma warning disable SA1116 // Split parameters should start on line after declaration. Reviewed.
    /// <summary>
    /// Multiplayer notification for someone getting caught checking trash cans.
    /// </summary>
    /// <param name="instructions">instructions.</param>
    /// <param name="gen">generator.</param>
    /// <param name="original">original method.</param>
    /// <returns>new instructions.</returns>
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(Town), nameof(Town.checkAction))]
    [SuppressMessage("SMAPI.CommonErrors", "AvoidNetField:Avoid Netcode types when possible", Justification = "Used for matching only.")]
    private static IEnumerable<CodeInstruction>? TranspileTownCheckAction(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldsfld, typeof(Game1).GetCachedField("multiplayer", ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Ldstr, "TrashCan"),
            })
            .FindNext(new CodeInstructionWrapper[]
            { // get the next reference to a character's name.
                new(OpCodes.Ldfld, typeof(Character).GetCachedField(nameof(Character.name), ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Call), // this is an op_implicit cast to string.
            })
            .GetLabels(out IList<Label>? labels)
            .Advance(2)
            .DefineAndAttachLabel(out Label nullcheck)
            .Advance(-2)
            .Insert(new CodeInstruction[]
            { // and replace it with the display name.
                new(OpCodes.Callvirt, typeof(Character).GetCachedProperty(nameof(Character.displayName), ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                new(OpCodes.Dup), // check if it was null
                new(OpCodes.Brtrue_S, nullcheck),
                new(OpCodes.Pop), // pop the displayName from the stack, revert to the name.
            }, withLabels: labels);

            helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into error transpiling {original.FullDescription()}.\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
#pragma warning restore SA1116 // Split parameters should start on line after declaration
}
