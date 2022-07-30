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
using AtraBase.Toolkit.Reflection;
using AtraCore.Framework.ReflectionManager;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using xTile.Dimensions;

namespace StopRugRemoval.HarmonyPatches;

#warning - remove in 1.6

/// <summary>
/// Patches to prevent gate removal.
/// </summary>
[HarmonyPatch(typeof(GameLocation))]
internal static class PreventGateRemoval
{
    private static readonly PerScreen<int> PerscreenedAttempts = new(createNewState: () => 0);
    private static readonly PerScreen<int> PerscreenedTicks = new(createNewState: () => 0);

    private static int Attempts
    {
        get => PerscreenedAttempts.Value;
        set => PerscreenedAttempts.Value = value;
    }

    private static int Ticks
    {
        get => PerscreenedTicks.Value;
        set => PerscreenedTicks.Value = value;
    }

    /// <summary>
    /// Checks to see if the FurniturePlacementKey is held.
    /// Also shows the message if needed.
    /// </summary>
    /// <param name="gameLocation">Game location.</param>
    /// <param name="tile">Tile.</param>
    /// <returns>true if held down, false otherwise.</returns>
    public static bool AreFurnitureKeysHeld(GameLocation gameLocation, Location tile)
    {
        if (Game1.ticks > Ticks + 120)
        {
            Attempts = 0;
            Ticks = Game1.ticks;
        }
        else
        {
            Attempts++;
        }
        if (ModEntry.Config.FurniturePlacementKey.IsDown() || !ModEntry.Config.Enabled)
        {
            return true;
        }
        else
        {
            Vector2 v = new(tile.X, tile.Y);
            if (Attempts > 12 && gameLocation.objects.TryGetValue(v, out SObject obj) && obj is Fence fence && fence.isGate.Value)
            {
                Attempts -= 5;
                Game1.showRedMessage(I18n.GateRemovalMessage(ModEntry.Config.FurniturePlacementKey));
            }
            return false;
        }
    }

    /**********************
    * if (vect.Equals(who.getTileLocation()) && !this.objects[vect].isPassable())
    *
    * to
    *
    * if (AreFurnitureKeysHeld() && vect.Equals(who.getTileLocation()) && !this.objects[vect].isPassable())
    ********************************************/
#pragma warning disable SA1116 // Split parameters should start on line after declaration. Reviewed.
    [HarmonyPatch(nameof(GameLocation.checkAction))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
                    {
                        new(OpCodes.Ldarg_0),
                        new(OpCodes.Ldfld),
                        new(SpecialCodeInstructionCases.LdLoc),
                        new(OpCodes.Callvirt),
                        new(OpCodes.Isinst),
                        new(OpCodes.Brtrue_S),
                    })
                .FindNext(new CodeInstructionWrapper[]
                    {
                        new(OpCodes.Ldloca_S),
                        new(SpecialCodeInstructionCases.LdArg),
                        new(OpCodes.Callvirt, typeof(Character).GetCachedMethod(nameof(Character.getTileLocation), ReflectionCache.FlagTypes.InstanceFlags)),
                        new(OpCodes.Call),
                    })
                .Push()
                .FindNext(new CodeInstructionWrapper[]
                    {
                        new(OpCodes.Brfalse),
                        new(SpecialCodeInstructionCases.LdArg),
                        new(OpCodes.Ldfld),
                        new(SpecialCodeInstructionCases.LdLoc),
                    })
                .StoreBranchDest()
                .AdvanceToStoredLabel()
                .DefineAndAttachLabel(out Label newLabel)
                .Pop()
                .GetLabels(out IList<Label> labels, clear: true)
                .Insert(new CodeInstruction[]
                    {
                        new(OpCodes.Ldarg_0),
                        new(OpCodes.Ldarg_1),
                        new(OpCodes.Call, typeof(PreventGateRemoval).StaticMethodNamed(nameof(AreFurnitureKeysHeld))),
                        new(OpCodes.Brfalse, newLabel),
                    },
                    withLabels: labels);
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Transpiler for GameLocation{nameof(GameLocation.checkAction)} failed with error {ex}", LogLevel.Error);
        }
        return null;
    }
#pragma warning restore SA1116 // Split parameters should start on line after declaration
}
