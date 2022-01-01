/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Enums;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches;

[UsedImplicitly]
internal class LevelUpMenuRevalidateHealthPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal LevelUpMenuRevalidateHealthPatch()
    {
        Original = RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.RevalidateHealth));
    }

    #region harmony patches

    /// <summary>
    ///     Patch revalidate player health after changes to the combat skill + revalidate fish pond capacity after changes
    ///     to the fishing skill.
    /// </summary>
    [HarmonyPrefix]
    private static bool LevelUpMenuRevalidateHealthPrefix(Farmer farmer)
    {
        var expectedMaxHealth = 100;
        if (farmer.mailReceived.Contains("qiCave")) expectedMaxHealth += 25;

        for (var i = 0; i < farmer.combatLevel.Value; ++i)
            if (!farmer.newLevels.Contains(new((int) SkillType.Combat, i)))
                expectedMaxHealth += 5;

        if (Game1.player.HasProfession("Fighter")) expectedMaxHealth += 15;
        if (Game1.player.HasProfession("Brute")) expectedMaxHealth += 25;

        if (farmer.maxHealth != expectedMaxHealth)
        {
            ModEntry.Log(
                $"Fixing max health of {farmer.Name}.\nCurrent: {farmer.maxHealth}\nExpected: {expectedMaxHealth}",
                LogLevel.Warn);
            farmer.maxHealth = expectedMaxHealth;
            farmer.health = Math.Min(farmer.maxHealth, farmer.health);
        }

        try
        {
            // revalidate fish pond capacity
            foreach (var b in Game1.getFarm().buildings.Where(b =>
                         (b.owner.Value == farmer.UniqueMultiplayerID || !Context.IsMultiplayer) && b is FishPond &&
                         !b.isUnderConstruction()))
            {
                var pond = (FishPond) b;
                pond.UpdateMaximumOccupancy();
                pond.currentOccupants.Value = Math.Min(pond.currentOccupants.Value, pond.maxOccupants.Value);
            }
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
            return false; // don't run original logic
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}