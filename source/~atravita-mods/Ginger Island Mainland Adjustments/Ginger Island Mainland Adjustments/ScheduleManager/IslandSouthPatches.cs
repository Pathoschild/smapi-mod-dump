/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Toolkit;
using AtraShared;
using AtraShared.Utils.Extensions;
using GingerIslandMainlandAdjustments.AssetManagers;
using GingerIslandMainlandAdjustments.Configuration;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Locations;

namespace GingerIslandMainlandAdjustments.ScheduleManager;

/// <summary>
/// Patches for the IslandSouth class.
/// </summary>
[HarmonyPatch(typeof(IslandSouth))]
internal static class IslandSouthPatches
{
    /// <summary>
    /// Dictionary of NPCs and custom exclusions.
    /// </summary>
    /// <remarks>null is cache miss: reload if ever null.</remarks>
    private static Dictionary<NPC, string[]>? exclusions = null;

    /// <summary>
    /// Gets dictionary of NPCs and custom exclusions.
    /// </summary>
    /// <remarks>Cached, will reload automatically if not currently cached.</remarks>
    internal static Dictionary<NPC, string[]> Exclusions
    {
        get
        {
            exclusions ??= AssetLoader.GetExclusions();
            return exclusions;
        }
    }

    /// <summary>
    /// Clears/resets the Exclusions cache.
    /// </summary>
    internal static void ClearCache() => exclusions = null;

    /// <summary>
    /// Override the vanilla schedules if told to.
    /// </summary>
    /// <returns>False to skip vanilla function, true otherwise.</returns>
    /// <remarks>Setting my harmony priority low to try to be run **after** Custom NPC Exclusions.</remarks>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.LowerThanNormal)]
    [HarmonyPatch(nameof(IslandSouth.SetupIslandSchedules))]
    private static bool OverRideSetUpIslandSchedules()
    {
        if (Globals.Config.UseThisScheduler)
        {
            try
            {
                GIScheduler.GenerateAllSchedules();
                return false;
            }
            catch (Exception ex)
            {
                Globals.ModMonitor.Log($"Errors generating ginger island schedules, defaulting to vanilla code\n\n{ex}");
            }
        }
        return true;
    }

    /// <summary>
    /// Extends CanVisitIslandToday for custom exclusions as well.
    /// </summary>
    /// <param name="npc">the NPC to check.</param>
    /// <param name="__result">True if the NPC can go to the island, false otherwise.</param>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(IslandSouth.CanVisitIslandToday))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Convention used by Harmony")]
    private static void ExtendCanGoToIsland(NPC npc, ref bool __result)
    {
        try
        {
            if (!__result)
            {
                Farmer? spouse = npc.getSpouse();
                if ((Globals.Config.AllowSandy == VillagerExclusionOverride.Yes
                        || (Globals.Config.AllowSandy == VillagerExclusionOverride.IfMarried && spouse is not null))
                    && Globals.Config.UseThisScheduler
                    && npc.Name.Equals("Sandy", StringComparison.OrdinalIgnoreCase)
                    && !(Game1.dayOfMonth == 15)
                    && !Game1.IsFall)
                {
                    __result = true; // let Sandy come to the resort!
                }
                else if (Globals.Config.AllowGeorgeAndEvelyn
                    && Globals.Config.UseThisScheduler
                    && (npc.Name.Equals("George", StringComparison.OrdinalIgnoreCase) || npc.Name.Equals("Evelyn", StringComparison.OrdinalIgnoreCase)))
                {
                    __result = true; // let George & Evelyn come too!
                }
                else if (Globals.Config.UseThisScheduler
                    && (Globals.Config.AllowWilly == VillagerExclusionOverride.Yes
                        || (Globals.Config.AllowWilly == VillagerExclusionOverride.IfMarried && spouse is not null))
                    && npc.Name.Equals("Willy", StringComparison.OrdinalIgnoreCase))
                {
                    __result = true; // Allow Willy access to resort as well.
                }
                else if (Globals.Config.UseThisScheduler
                    && (Globals.Config.AllowWizard == VillagerExclusionOverride.Yes
                        || (Globals.Config.AllowWizard == VillagerExclusionOverride.IfMarried && spouse is not null))
                    && npc.Name.Equals("Wizard", StringComparison.OrdinalIgnoreCase))
                {
                    __result = true; // Allow the Wizard access to the result.
                }
                else
                {
                    // already false in code, ignore me for everyone else
                    return;
                }
            }
            if (!Exclusions.ContainsKey(npc))
            { // I don't have an entry for you.
                return;
            }
            string[] checkset = Exclusions[npc];
            foreach (string condition in checkset)
            {
                if (Game1.dayOfMonth.ToString().Equals(condition, StringComparison.OrdinalIgnoreCase)
                    || Game1.currentSeason.Equals(condition, StringComparison.OrdinalIgnoreCase)
                    || Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals(condition, StringComparison.OrdinalIgnoreCase)
                    || $"{Game1.currentSeason}_{Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)}".Equals(condition, StringComparison.OrdinalIgnoreCase)
                    || $"{Game1.currentSeason}_{Game1.dayOfMonth}".Equals(condition, StringComparison.OrdinalIgnoreCase))
                {
                    __result = false;
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            Globals.ModMonitor.Log($"Error in postfix for CanVisitIslandToday for {npc.Name}: \n\n{ex}", LogLevel.Warn);
        }
        return;
    }

    /// <summary>
    /// Prefixes HasIslandAttire to allow the player choice in whether the NPCs should wear their island attire.
    /// </summary>
    /// <param name="character">NPC in question.</param>
    /// <param name="__result">Result returned to original function.</param>
    /// <returns>True to continue to the vanilla function, false otherwise.</returns>
    /// <exception cref="UnexpectedEnumValueException{WearIslandClothing}">Unexpected enum value.</exception>
    [HarmonyPrefix]
    [HarmonyPatch(nameof(IslandSouth.HasIslandAttire))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Convention used by Harmony")]
    private static bool PrefixHasIslandAttire(NPC character, ref bool __result)
    {
        try
        {
            switch (Globals.Config.WearIslandClothing)
            {
                case WearIslandClothing.Default:
                    return true;
                case WearIslandClothing.All:
                    if (character.Name.Equals("Lewis", StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            Game1.temporaryContent.Load<Texture2D>($"Characters\\{NPC.getTextureNameForCharacter(character.Name)}_Beach");
                            __result = true;
                            return false;
                        }
                        catch (Exception)
                        {
                        }
                    }
                    return true;
                case WearIslandClothing.None:
                    __result = false;
                    return false;
                default:
                    TKThrowHelper.ThrowUnexpectedEnumValueException(Globals.Config.WearIslandClothing);
                    return true; // this will never get hit.
            }
        }
        catch (Exception ex)
        {
            Globals.ModMonitor.Log($"Error in prefix for HasIslandAttire for {character.Name}: \n\n{ex}", LogLevel.Warn);
        }
        return true;
    }
}
