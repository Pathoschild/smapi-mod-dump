/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraCore.Framework.ReflectionManager;

using HarmonyLib;

using SpecialOrdersExtended.Managers;

namespace SpecialOrdersExtended.HarmonyPatches;

/// <summary>
/// Holds patches against Special Orders.
/// </summary>
[HarmonyPatch(typeof(SpecialOrder))]
internal static class SpecialOrderPatches
{
    /// <summary>
    /// Applies the patch that suppresses board updates.
    /// </summary>
    /// <param name="harmony">Harmony instance.</param>
    internal static void ApplyUpdatePatch(Harmony harmony)
    {
        try
        {
            HarmonyMethod? prefix = new(typeof(SpecialOrderPatches), nameof(PrefixUpdate))
            {
                priority = Priority.Last,
            };

            harmony.Patch(
                original: typeof(SpecialOrder).GetCachedMethod(nameof(SpecialOrder.UpdateAvailableSpecialOrders), ReflectionCache.FlagTypes.StaticFlags),
                prefix: prefix);
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed in apply board update patch:\n\n{ex}", LogLevel.Error);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(SpecialOrder.OnFail))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention.")]
    private static void PostfixOnFail(SpecialOrder __instance) => DialogueManager.ClearOnFail(__instance.questKey.Value);

    // Suppress the middle-of-night Special Order updates until
    // the board is open. There's no point.
    private static bool PrefixUpdate() => Game1.player.team.availableSpecialOrders.Count != 0
        || (ModEntry.Config.SurpressUnnecessaryBoardUpdates && SpecialOrder.IsSpecialOrdersBoardUnlocked());

    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    [HarmonyPatch(nameof(SpecialOrder.SetDuration))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
    private static bool PrefixSetDuration(SpecialOrder __instance)
    {
        try
        {
            Dictionary<string, int> overrides = AssetManager.GetDurationOverride().ToDictionary(kvp => kvp.Key, kvp => int.TryParse(kvp.Value, out int value) ? value : 0);
            if (overrides.TryGetValue(__instance.questKey.Value, out int duration))
            {
                WorldDate? date = new(Game1.year, Game1.currentSeason, Game1.dayOfMonth);
                __instance.dueDate.Value = date.TotalDays + (duration == -1 ? 99 : duration);
                return false;
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod failed while trying to override special order duration!\n\n{ex}");
        }
        return true;
    }

    [HarmonyPostfix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    [HarmonyPatch(nameof(SpecialOrder.IsTimedQuest))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
    private static void HandleUntimed(SpecialOrder __instance, ref bool __result)
    {
        if (__result && AssetManager.Untimed.Value.Contains(__instance.questKey.Value))
        {
            __result = false;
        }
    }
}