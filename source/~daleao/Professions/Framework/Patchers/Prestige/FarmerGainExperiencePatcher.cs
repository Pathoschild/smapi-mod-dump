/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Patchers.Prestige;

#region using directives

using System.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerGainExperiencePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerGainExperiencePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FarmerGainExperiencePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.gainExperience));
    }

    #region harmony patches

    /// <summary>Patch to increase skill experience after each prestige + gate at level 10 until full prestige.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    private static bool FarmerGainExperiencePrefix(Farmer __instance, int which, ref int howMuch)
    {
        if (!ShouldEnableSkillReset && !ShouldEnablePrestigeLevels)
        {
            return true; // run original logic
        }

        if (which == Farmer.luckSkill || howMuch <= 0)
        {
            return false; // don't run original logic
        }

        if (!__instance.IsLocalPlayer && Game1.IsServer)
        {
            __instance.queueMessage(17, Game1.player, which, howMuch);
            return false; // don't run original logic
        }

        try
        {
            var skill = Skill.FromValue(which);
            howMuch = Math.Max((int)(howMuch * skill.BaseExperienceMultiplier * ((ISkill)skill).BonusExperienceMultiplier), 1);
            if (((skill.CurrentLevel == 10 && !skill.CanGainPrestigeLevels()) || skill.CurrentLevel == 20) &&
                Skill.List.All(s => __instance.professions.Intersect(((ISkill)s).TierTwoProfessionIds).Any()))
            {
                var old = MasteryTrackerMenu.getCurrentMasteryLevel();
                Game1.stats.Increment("MasteryExp", howMuch);
                if (MasteryTrackerMenu.getCurrentMasteryLevel() <= old)
                {
                    return false; // don't run original logic
                }

                Game1.showGlobalMessage(Game1.content.LoadString("Strings\\1_6_Strings:Mastery_newlevel"));
                Game1.playSound("newArtifact");
                return false; // don't run original logic
            }

            var newLevel = Math.Min(
                Farmer.checkForLevelGain(skill.CurrentExp, skill.CurrentExp + howMuch),
                skill.MaxLevel);
            if (newLevel <= skill.CurrentLevel)
            {
                skill.AddExperience(howMuch);
                return false; // don't run original logic
            }

            skill.AddExperience(howMuch);
            skill.SetLevel(newLevel);
            Game1.showGlobalMessage(Game1.content.LoadString("Strings\\1_6_Strings:NewIdeas"));
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
