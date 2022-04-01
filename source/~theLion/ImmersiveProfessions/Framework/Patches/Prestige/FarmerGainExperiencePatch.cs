/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Prestige;

#region using directives

using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Enums;
using StardewValley;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal class FarmerGainExperiencePatch : BasePatch
{
    private const int VANILLA_CAP_I = 15000;

    private static readonly int[] _vanillaExpPerLevel = {100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000};

    private static int _PrestigeCap => VANILLA_CAP_I + (int) ModEntry.Config.RequiredExpPerExtendedLevel * 10;

    /// <summary>Construct an instance.</summary>
    internal FarmerGainExperiencePatch()
    {
        Original = RequireMethod<Farmer>(nameof(Farmer.gainExperience));
        Prefix.priority = Priority.LowerThanNormal;
    }

    #region harmony patches

    /// <summary>Patch to increase skill experience after each prestige + gate at level 10 until full prestige.</summary>
    [HarmonyPrefix]
    private static bool FarmerGainExperiencePrefix(Farmer __instance, int which, ref int howMuch)
    {
        try
        {
            if (which == (int) SkillType.Luck && !ModEntry.ModHelper.ModRegistry.IsLoaded("spacechase0.LuckSkill") ||
                howMuch <= 0)
                return false; // don't run original logic

            if (!__instance.IsLocalPlayer)
            {
                __instance.queueMessage(17, Game1.player, which, howMuch);
                return false; // don't run original logic
            }

            var currentExp = Game1.player.experiencePoints[which];
            var currentLevel = Game1.player.GetUnmodifiedSkillLevel(which);
            var canGainPrestigeLevels = __instance.HasAllProfessionsInSkill(which);
            switch (currentLevel)
            {
                case >= 10 when !canGainPrestigeLevels:
                {
                    if (currentLevel > 10) __instance.SetSkillLevel((SkillType) which, 10);

                    if (currentExp > VANILLA_CAP_I) __instance.experiencePoints[which] = VANILLA_CAP_I;

                    return false; // don't run original logic
                }
                case >= 20:
                {
                    if (currentLevel > 20) __instance.SetSkillLevel((SkillType) which, 20);

                    var prestigeCap = _PrestigeCap;
                    if (currentExp > prestigeCap) __instance.experiencePoints[which] = prestigeCap;

                    return false; // don't run original logic
                }
            }

            if (!ModEntry.PlayerState.RevalidatedLevelThisSession)
            {
                var expectedLevel = 0;
                var i = 0;
                while (i < 10 && currentExp > _vanillaExpPerLevel[i++]) ++expectedLevel;

                var remainingExp = currentExp - VANILLA_CAP_I;
                if (ModEntry.Config.EnablePrestige && remainingExp > 0)
                {
                    i = 1;
                    while (i <= 10 && remainingExp > ModEntry.Config.RequiredExpPerExtendedLevel * i++) ++expectedLevel;
                }

                if (currentLevel < expectedLevel)
                {
                    for (var level = currentLevel + 1; level <= expectedLevel; ++level)
                    {
                        var newOldLevel = new Point(which, level);
                        if (!Game1.player.newLevels.Contains(newOldLevel))
                            Game1.player.newLevels.Add(newOldLevel);
                    }
                
                    Game1.player.SetSkillLevel((SkillType) which, expectedLevel);
                }

                ModEntry.PlayerState.RevalidatedLevelThisSession = true;
            }

            howMuch = (int) (howMuch * ModEntry.Config.BaseSkillExpMultiplierPerSkill[which]);
            if (ModEntry.Config.EnablePrestige)
            {
                howMuch = (int) (howMuch * Math.Pow(1f + ModEntry.Config.BonusSkillExpPerReset,
                    __instance.NumberOfProfessionsInSkill(which, true)));
            }

            var newLevel = Farmer.checkForLevelGain(currentExp, currentExp + howMuch);
            if (newLevel > currentLevel)
            {
                for (var level = currentLevel + 1; level <= newLevel; ++level)
                {
                    var newNewLevel = new Point(which, level);
                    if (!Game1.player.newLevels.Contains(newNewLevel))
                        Game1.player.newLevels.Add(newNewLevel);
                }

                Game1.player.SetSkillLevel((SkillType) which, newLevel);
            }

            Game1.player.experiencePoints[which] = Math.Min(currentExp + howMuch,
                canGainPrestigeLevels ? _PrestigeCap : VANILLA_CAP_I);

            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}