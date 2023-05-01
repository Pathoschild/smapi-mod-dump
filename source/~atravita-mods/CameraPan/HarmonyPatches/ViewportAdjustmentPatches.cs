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

using CameraPan.Framework;

using HarmonyLib;

using Microsoft.Xna.Framework;

using StardewModdingAPI.Utilities;

using StardewValley.Locations;

namespace CameraPan.HarmonyPatches;

/// <summary>
/// Adjusts the viewport based on the offset vector.
/// </summary>
[HarmonyPatch(typeof(Game1))]
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Named for Harmony.")]
internal static class ViewportAdjustmentPatches
{

    private static readonly PerScreen<CameraBehavior> cameraBehavior = new(() => CameraBehavior.Both);

    /// <summary>
    /// Gets the current camera behavior.
    /// </summary>
    internal static CameraBehavior Behavior => cameraBehavior.Value;

    private static bool IsGamePanning => Game1.viewportTarget.X != -2.14748365E+09f;

    /// <summary>
    /// Adjusts the camera behavior for this location.
    /// </summary>
    /// <param name="config">Config instance.</param>
    /// <param name="location">Location to adjust for.</param>
    internal static void SetCameraBehaviorForConfig(ModConfig config, GameLocation? location)
    {
        if (location is null)
        {
            return;
        }

        CameraBehavior behavior;
        if (ModEntry.Config.PerMapCameraBehavior.TryGetValue(location.Name, out PerMapCameraBehavior perMapCameraBehavior)
            && perMapCameraBehavior != PerMapCameraBehavior.ByIndoorsOutdoors)
        {
            behavior = (CameraBehavior)perMapCameraBehavior;
        }
        else
        {
            behavior = location.IsOutdoors || location is BugLand ? config.OutdoorsCameraBehavior : config.IndoorsCameraBehavior;
        }

        if (location.forceViewportPlayerFollow)
        {
            behavior |= CameraBehavior.Locked;
        }
        if (behavior.HasFlagFast(CameraBehavior.Offset) && !string.IsNullOrWhiteSpace(location.getMapProperty("atravita.PanningForbidden")))
        {
            behavior &= ~CameraBehavior.Offset;
            Game1.addHUDMessage(new(I18n.Forbidden()));
            ModEntry.SetCameraHoverMessage(CameraBehaviorMessage.DisabledByMap);
        }
        else
        {
            ModEntry.SetCameraHoverMessage(behavior.HasFlagFast(CameraBehavior.Offset) ? CameraBehaviorMessage.Allowed : CameraBehaviorMessage.DisabledBySettings);
        }

        cameraBehavior.Value = behavior;

        ModEntry.Reset();
        ModEntry.ModMonitor.Log($"Setting camera behavior {behavior.ToStringFast()} for location {location.NameOrUniqueName}");
    }

    #region transpiler helpers

    [MethodImpl(TKConstants.Hot)]
    private static bool IsInEvent()
        => Game1.CurrentEvent is Event evt && (evt.farmer is not null && !evt.isFestival);

    [MethodImpl(TKConstants.Hot)]
    private static bool ShouldLock() => !IsGamePanning && !IsInEvent() && cameraBehavior.Value.HasFlagFast(CameraBehavior.Locked);

    /// <summary>
    /// Whether or not the map allows panning.
    /// </summary>
    /// <returns>True if panning should be allowed.</returns>
    [MethodImpl(TKConstants.Hot)]
    internal static bool ShouldOffset() => !IsGamePanning && !IsInEvent() && cameraBehavior.Value.HasFlagFast(CameraBehavior.Offset);

    [MethodImpl(TKConstants.Hot)]
    private static float GetXTarget(float prevVal) => ShouldOffset() ? ModEntry.Target.X : prevVal;

    [MethodImpl(TKConstants.Hot)]
    private static float GetYTarget(float prevVal) => ShouldOffset() ? ModEntry.Target.Y : prevVal;

    #endregion

    [MethodImpl(TKConstants.Hot)]
    [HarmonyPatch("getViewportCenter")]
    private static void Postfix(ref Point __result)
    {
        if (!IsGamePanning && !IsInEvent() && ShouldOffset()
            && (Math.Abs(Game1.viewportCenter.X - ModEntry.Target.X) >= 4 || Math.Abs(Game1.viewportCenter.Y - ModEntry.Target.Y) >= 4))
        {
            __result = Game1.viewportCenter = ModEntry.Target;
        }
    }

    [HarmonyPatch(nameof(Game1.UpdateViewPort))]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:Split parameters should start on line after declaration", Justification = "Reviewed.")]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            { // if (Game1.viewportFreeze || overrideFreeze)
                (OpCodes.Ldsfld, typeof(Game1).GetCachedField(nameof(Game1.viewportFreeze), ReflectionCache.FlagTypes.StaticFlags)),
                OpCodes.Ldc_I4_0,
                OpCodes.Ceq,
                OpCodes.Ldarg_0,
                OpCodes.Or,
                OpCodes.Brfalse,
            })
            .Push()
            .Advance(5)
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .DefineAndAttachLabel(out Label jumpPast)
            .Pop()
            .GetLabels(out IList<Label>? firstLabelsToMove)
            .Insert(new CodeInstruction[]
            { // insert if (!ShouldLock) around this block.
                new(OpCodes.Call, typeof(ViewportAdjustmentPatches).GetCachedMethod(nameof(ShouldLock), ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Brtrue, jumpPast),
            }, withLabels: firstLabelsToMove)
            .FindNext(new CodeInstructionWrapper[]
            { // if (Game1.currentLocation.forceViewportPlayerFollow)
                (OpCodes.Call, typeof(Game1).GetCachedProperty(nameof(Game1.currentLocation), ReflectionCache.FlagTypes.StaticFlags).GetGetMethod()),
                (OpCodes.Ldfld, typeof(GameLocation).GetCachedField(nameof(GameLocation.forceViewportPlayerFollow), ReflectionCache.FlagTypes.InstanceFlags)),
                OpCodes.Brfalse_S,
            })
            .Push()
            .Advance(3)
            .DefineAndAttachLabel(out Label jumpToLock)
            .Pop()
            .GetLabels(out IList<Label>? secondLabelsToMove)
            .Insert(new CodeInstruction[]
            { // insert if (ShouldLock() || Game1.currentLocation.forceViewportPlayerFollow)
                new(OpCodes.Call, typeof(ViewportAdjustmentPatches).GetCachedMethod(nameof(ShouldLock), ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Brtrue, jumpToLock),
            }, withLabels: secondLabelsToMove)
            .FindNext(new CodeInstructionWrapper[]
            { // fix up the X location
                (OpCodes.Callvirt, typeof(Character).GetCachedProperty(nameof(Character.Position), ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                (OpCodes.Ldfld, typeof(Vector2).GetCachedField(nameof(Vector2.X), ReflectionCache.FlagTypes.InstanceFlags)),
            })
            .Advance(2)
            .Insert(new CodeInstruction[]
            {
                new (OpCodes.Call, typeof(ViewportAdjustmentPatches).GetCachedMethod(nameof(GetXTarget), ReflectionCache.FlagTypes.StaticFlags)),
            })
            .FindNext(new CodeInstructionWrapper[]
            { // fix up the Y location
                (OpCodes.Callvirt, typeof(Character).GetCachedProperty(nameof(Character.Position), ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                (OpCodes.Ldfld, typeof(Vector2).GetCachedField(nameof(Vector2.Y), ReflectionCache.FlagTypes.InstanceFlags)),
            })
            .Advance(2)
            .Insert(new CodeInstruction[]
            {
                new (OpCodes.Call, typeof(ViewportAdjustmentPatches).GetCachedMethod(nameof(GetYTarget), ReflectionCache.FlagTypes.StaticFlags)),
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
