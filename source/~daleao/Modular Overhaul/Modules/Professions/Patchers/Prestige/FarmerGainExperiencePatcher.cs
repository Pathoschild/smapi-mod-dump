/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Patchers.Prestige;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Professions.Integrations;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerGainExperiencePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerGainExperiencePatcher"/> class.</summary>
    internal FarmerGainExperiencePatcher()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.gainExperience));
        this.Prefix!.priority = Priority.LowerThanNormal;
    }

    #region harmony patches

    /// <summary>Patch to increase skill experience after each prestige + gate at level 10 until full prestige.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.LowerThanNormal)]
    private static bool FarmerGainExperiencePrefix(Farmer __instance, int which, ref int howMuch)
    {
        try
        {
            var skill = Skill.FromValue(which);
            if ((which == Farmer.luckSkill && LuckSkillIntegration.Instance?.IsRegistered != true) || howMuch <= 0)
            {
                return false; // don't run original logic
            }

            if (!__instance.IsLocalPlayer)
            {
                __instance.queueMessage(17, Game1.player, which, howMuch);
                return false; // don't run original logic
            }

            howMuch = Math.Max((int)(howMuch * skill.BaseExperienceMultiplier * ((ISkill)skill).PrestigeExperienceMultiplier), 1);
            var newLevel = Math.Min(
                Farmer.checkForLevelGain(skill.CurrentExp, skill.CurrentExp + howMuch),
                skill.MaxLevel);

            if (newLevel > skill.CurrentLevel)
            {
                for (var level = skill.CurrentLevel + 1; level <= newLevel; level++)
                {
                    var point = new Point(which, level);
                    if (!Game1.player.newLevels.Contains(point))
                    {
                        Game1.player.newLevels.Add(point);
                    }
                }

                skill.SetLevel(newLevel);
            }

            skill.AddExperience(howMuch);
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
