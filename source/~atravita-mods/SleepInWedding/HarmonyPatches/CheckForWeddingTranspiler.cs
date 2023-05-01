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

using AtraCore.Framework.ReflectionManager;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using StardewValley.Locations;

namespace SleepInWedding.HarmonyPatches;

/// <summary>
/// Adds an additional check for the should-a-wedding-happen? check.
/// </summary>
[HarmonyPatch(typeof(GameLocation))]
internal static class CheckForWeddingTranspiler
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetWeddingTime() => ModEntry.Config.WeddingTime;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool AmInTownAndShouldStartWedding() => ModEntry.Config.WedWhenEnteringTown && Game1.currentLocation is Town;

    private static Event? PrepareSpouseIfNecessary(Event? @event)
    {
        if (@event is null)
        {
            return null;
        }
        else if (Game1.player.spouse is not null && Game1.player.isEngaged() && Game1.weddingsToday.Contains(Game1.player.UniqueMultiplayerID))
        {
            Friendship friendship = Game1.player.friendshipData[Game1.player.spouse];
            if (friendship.CountdownToWedding <= 1)
            {
                @event.onEventFinished = (Action)Delegate.Combine(@event.onEventFinished, () =>
                {
                    ModEntry.ModMonitor.Log($"Preparing spouse {Game1.player.spouse}!");
                    friendship.Status = FriendshipStatus.Married;
                    friendship.WeddingDate = new WorldDate(Game1.Date);
                    Game1.prepareSpouseForWedding(Game1.player);
                });
            }
        }
        return @event;
    }

    [HarmonyPatch(nameof(GameLocation.checkForEvents))]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:Split parameters should start on line after declaration", Justification = "Reviewed.")]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldsfld, typeof(Game1).GetCachedField(nameof(Game1.weddingsToday), ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Callvirt, typeof(List<long>).GetCachedProperty(nameof(List<long>.Count), ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                new(OpCodes.Ldc_I4_0),
                new(SpecialCodeInstructionCases.Wildcard, (instr) => instr.opcode == OpCodes.Ble || instr.opcode == OpCodes.Ble_S),
            })
            .GetLabels(out IList<Label>? labelsToMove, clear: true)
            .DefineAndAttachLabel(out Label skip)
            .Push()
            .Advance(3)
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .DefineAndAttachLabel(out Label bypassWedding)
            .Pop()
            .Insert(new CodeInstruction[]
            { // if (Config.WeddingTime > Game1.timeOfDay) && (!AmInTownAndShouldStartWedding()), skip wedding for now.
                new(OpCodes.Call, typeof(CheckForWeddingTranspiler).GetCachedMethod(nameof(GetWeddingTime), ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Ldsfld, typeof(Game1).GetCachedField(nameof(Game1.timeOfDay), ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Ble, skip),
                new(OpCodes.Call, typeof(CheckForWeddingTranspiler).GetCachedMethod(nameof(AmInTownAndShouldStartWedding), ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Brfalse, bypassWedding),
            }, withLabels: labelsToMove)
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(Game1).GetCachedMethod(nameof(Game1.getAvailableWeddingEvent), ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Stfld),
            })
            .Advance(2)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Call, typeof(CheckForWeddingTranspiler).GetCachedMethod(nameof(PrepareSpouseIfNecessary), ReflectionCache.FlagTypes.StaticFlags)),
            });

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into error transpiling {original.FullDescription()}.\n\n{ex}", LogLevel.Error);
            original.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
}
