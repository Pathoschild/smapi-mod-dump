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
using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using Netcode;
using StardewModdingAPI.Events;
using StardewValley.Locations;

namespace NerfCavePuzzle.HarmonyPatches;

/// <summary>
/// Data model needed to save to save data.
/// </summary>
public class DataModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataModel"/> class.
    /// </summary>
    /// <remarks>This exists for NewtonSoft.</remarks>
    public DataModel()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataModel"/> class.
    /// </summary>
    /// <param name="times">Number of times failed.</param>
    public DataModel(int times)
        => this.Times = times;

    /// <summary>
    /// Gets or sets number of times failed.
    /// </summary>
    public int Times { get; set; } = 0;
}

/// <summary>
/// Transpilers! Yay.
/// </summary>
[HarmonyPatch(typeof(IslandWestCave1))]
internal static class IslandCaveWestDifficultyTranspiler
{
    private const string SAVEKEY = "CAVE_FAILED_TIMES";
    private const string REQUESTCAVEFAILED = "REQUEST_CAVE_FAILED";

    /// <summary>
    /// A new phase - this is the delay phase that waits for the player to interact again.
    /// </summary>
    private const int DELAYVALUE = 77;

    /// <summary>
    /// Handles saving from multiplayer.
    /// </summary>
    /// <param name="e">Event args.</param>
    internal static void HandleSaveFromMultiplayer(ModMessageReceivedEventArgs e)
    {
        if (Context.IsMainPlayer && e.FromModID == ModEntry.UniqueID && e.Type == SAVEKEY && int.TryParse(e.ReadAs<string>(), out int times))
        {
            ModEntry.DataHelper.WriteSaveData(SAVEKEY, new DataModel(times));
            ModEntry.ModMonitor.Log($"Recieved failure times {times}", LogLevel.Debug);
        }
        else if (Context.IsMainPlayer && e.FromModID == ModEntry.UniqueID && e.Type == REQUESTCAVEFAILED && e.ReadAs<string>() == "GET")
        {
            DataModel data = ModEntry.DataHelper.ReadSaveData<DataModel>(SAVEKEY) ?? new DataModel(0);
            ModEntry.MultiplayerHelper.SendMessage(data.Times.ToString(), REQUESTCAVEFAILED, new[] { ModEntry.UniqueID });
        }
        else if (e.FromModID == ModEntry.UniqueID && e.Type == REQUESTCAVEFAILED
            && int.TryParse(e.ReadAs<string>(), out int timesfailed) && Game1.getLocationFromName("IslandWestCave1") is IslandWestCave1 cave)
        {
            cave.timesFailed.Value = Math.Max(cave.timesFailed.Value, timesfailed);
        }
    }

    /// <summary>
    /// Writes the failure count to the save.
    /// </summary>
    /// <param name="times">Number of times failed.</param>
    private static void SaveFailureCount(int times)
    {
        try
        {
            if (Context.IsMainPlayer)
            {
                ModEntry.DataHelper.WriteSaveData(SAVEKEY, new DataModel(times));
            }
            else
            {
                ModEntry.MultiplayerHelper.SendMessage(times.ToString(), SAVEKEY, new[] { ModEntry.UniqueID }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
                ModEntry.ModMonitor.Log($"Attempt to send failures - {times}", LogLevel.Debug);
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed in trying to save the failure count.\n\n{ex}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Gets the number of times failed.
    /// </summary>
    /// <returns>Failure count.</returns>
    private static int GetFailureCount()
    {
        try
        {
            if (Context.IsMainPlayer)
            {
                DataModel data = ModEntry.DataHelper.ReadSaveData<DataModel>(SAVEKEY) ?? new DataModel(0);
                return data.Times;
            }
            else
            {
                ModEntry.MultiplayerHelper.SendMessage("GET", REQUESTCAVEFAILED, new[] { ModEntry.UniqueID }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
                return 0;
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed while trying to read failure count.\n\n{ex}", LogLevel.Error);
            return 0;
        }
    }

    /// <summary>
    /// Caps the difficulty at the maximum selected in the config.
    /// </summary>
    /// <param name="original">Original difficulty.</param>
    /// <returns>New difficulty.</returns>
    private static int GetAdjustedDifficulty(int original)
        => Math.Min(original, ModEntry.Config.MaxNotes);

    /// <summary>
    /// Gets a value that multiplies the gap between notes.
    /// </summary>
    /// <returns>The speed modifier.</returns>
    private static float GetSpeedModifier()
        => ModEntry.Config.SpeedModifer;

    private static int SetCorrectPhaseForPause(int localPhase)
    {
        try
        {
            if (ModEntry.Config.PauseBetweenRounds && localPhase != 0)
            {
                Game1.showGlobalMessage(I18n.GetByKey($"PauseBetweenRounds_{Game1.random.Next(6)}"));
                return DELAYVALUE;
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed while trying to set a different phase\n\n{ex}", LogLevel.Error);
        }
        return IslandWestCave1.PHASE_PLAY_SEQUENCE;
    }

#pragma warning disable SA1116 // Split parameters should start on line after declaration. Reviewed.
    [HarmonyPatch(nameof(IslandWestCave1.UpdateWhenCurrentLocation))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            { // Match switch (this.localPhase)
                new (OpCodes.Ldarg_0),
                new (OpCodes.Ldfld, typeof(IslandWestCave1).GetCachedField(nameof(IslandWestCave1.localPhase), ReflectionCache.FlagTypes.InstanceFlags)),
                new (SpecialCodeInstructionCases.StLoc),
            })
            .FindNext(new CodeInstructionWrapper[]
            { // Match and advance to case 4:
                new (SpecialCodeInstructionCases.LdLoc),
                new (OpCodes.Ldc_I4_4),
                new (OpCodes.Beq_S),
            })
            .Advance(2)
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .FindNext(new CodeInstructionWrapper[]
            { // for (int i = 0; i < this.currentDifficulty; i++) to for (int i = 0; i < Math.Min(this.currentDifficulty, Config.MaxNotes); i++)
                new (SpecialCodeInstructionCases.LdLoc),
                new (OpCodes.Ldarg_0),
                new (OpCodes.Ldfld, typeof(IslandWestCave1).GetCachedField(nameof(IslandWestCave1.currentDifficulty), ReflectionCache.FlagTypes.InstanceFlags)),
                new (OpCodes.Call),
                new (OpCodes.Blt_S),
            })
            .Advance(4)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Call, typeof(IslandCaveWestDifficultyTranspiler).GetCachedMethod(nameof(GetAdjustedDifficulty), ReflectionCache.FlagTypes.StaticFlags)),
            })
            .FindNext(new CodeInstructionWrapper[]
            { // this.netPhase.Value = 1;
                new (OpCodes.Ldarg_0),
                new (OpCodes.Ldfld, typeof(IslandWestCave1).GetCachedField(nameof(IslandWestCave1.netPhase), ReflectionCache.FlagTypes.InstanceFlags)),
                new (OpCodes.Ldc_I4_1),
                new (OpCodes.Callvirt, typeof(NetFieldBase<int, NetInt>).GetCachedProperty("Value", ReflectionCache.FlagTypes.InstanceFlags).GetSetMethod()),
            })
            .Advance(2)
            .GetLabels(out IList<Label> labelsToMove)
            .ReplaceInstruction(OpCodes.Call, typeof(IslandCaveWestDifficultyTranspiler).GetCachedMethod(nameof(SetCorrectPhaseForPause), ReflectionCache.FlagTypes.StaticFlags))
            .Insert(new CodeInstruction[]
            { // get the current phase, I need that to skip the pause on the first segment.
                new (OpCodes.Ldarg_0),
                new (OpCodes.Ldfld, typeof(IslandWestCave1).GetCachedField(nameof(IslandWestCave1.localPhase), ReflectionCache.FlagTypes.InstanceFlags)),
            }, withLabels: labelsToMove)
            .FindNext(new CodeInstructionWrapper[]
            { // this.betweenNotesTimer = 1500f/betweenNotesDivisor to this.betweenNotesTimer = 1500f/betweenNotesDivisor * DifficultyModifer
                new (OpCodes.Ldarg_0),
                new (OpCodes.Ldc_R4, 1500f),
                new (SpecialCodeInstructionCases.LdLoc),
                new (OpCodes.Conv_R4),
                new (OpCodes.Div),
                new (OpCodes.Stfld, typeof(IslandWestCave1).GetCachedField(nameof(IslandWestCave1.betweenNotesTimer), ReflectionCache.FlagTypes.InstanceFlags)),
            })
            .Advance(5)
            .Insert(new CodeInstruction[]
            {
                new (OpCodes.Call, typeof(IslandCaveWestDifficultyTranspiler).GetCachedMethod(nameof(GetSpeedModifier), ReflectionCache.FlagTypes.StaticFlags)),
                new (OpCodes.Mul),
            });
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into errors transpiling IslandWestCave1.UpdateWhenCurrentLocation.\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
#pragma warning restore SA1116 // Split parameters should start on line after declaration

    [HarmonyPostfix]
    [HarmonyPatch(nameof(IslandWestCave1.performAction))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention")]
    private static void PostFixPerformAction(IslandWestCave1 __instance, string action, Farmer who)
    {
        try
        {
            if (action is not null && who.IsLocalPlayer && !__instance.completed.Value)
            {
                string[] splits = action.Split(' ');
                if (!splits[0].Equals("CrystalCaveActivate", StringComparison.OrdinalIgnoreCase) || !__instance.isActivated.Value)
                {
                    return;
                }
                else if (__instance.netPhase.Value is DELAYVALUE)
                {
                    __instance.netPhase.Value = IslandWestCave1.PHASE_PLAY_SEQUENCE;
                    __instance.playSound("shwip");
                }
                else if (__instance.netPhase.Value == IslandWestCave1.PHASE_INTRO)
                {
                    __instance.timesFailed.Value = Math.Max(GetFailureCount(), __instance.timesFailed.Value);
                }
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed while postfixing PerformAction in the cave...{ex}", LogLevel.Error);
        }
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(IslandWestCave1.enterValue))]
    private static IEnumerable<CodeInstruction>? FailureTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            { // just after this.timesFailed++ insert a call that writes the value to the save.
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, typeof(IslandWestCave1).GetCachedField(nameof(IslandWestCave1.timesFailed), ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Dup),
                new(OpCodes.Callvirt),
                new(SpecialCodeInstructionCases.StLoc),
                new(SpecialCodeInstructionCases.LdLoc),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Add),
                new(OpCodes.Callvirt, typeof(NetFieldBase<int, NetInt>).GetCachedProperty("Value", ReflectionCache.FlagTypes.InstanceFlags).GetSetMethod()),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Add),
                new(OpCodes.Callvirt, typeof(NetFieldBase<int, NetInt>).GetCachedProperty("Value", ReflectionCache.FlagTypes.InstanceFlags).GetSetMethod()),
            })
            .Advance(1)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Dup),
                new(OpCodes.Call, typeof(IslandCaveWestDifficultyTranspiler).StaticMethodNamed(nameof(SaveFailureCount))),
            });
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into errors transpiling IslandWestCave1.enterValue.\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
}