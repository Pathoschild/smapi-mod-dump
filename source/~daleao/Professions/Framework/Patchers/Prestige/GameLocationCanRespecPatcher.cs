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
using Microsoft.Xna.Framework;

#endregion using directive

[UsedImplicitly]
internal sealed class GameLocationCanRespecPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationCanRespecPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal GameLocationCanRespecPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.canRespec));
    }

    #region harmony patches

    /// <summary>Patch to change Statue of Transcendance respec from (less than) 10 to (greater than) 10.</summary>
    [HarmonyPrefix]
    private static bool GameLocationCanRespecPrefix(ref bool __result, int skill_index)
    {
        if (!ShouldEnablePrestigeLevels)
        {
            if (!ShouldEnableSkillReset)
            {
                return true; // run original logic
            }

            __result = false;
            return false;
        }

        try
        {
            var player = Game1.player;
            var skill = Skill.FromValue(skill_index);
            if (!ShouldEnableSkillReset && !skill.CanGainPrestigeLevels())
            {
                return true; // run original logic
            }

            __result = player.GetUnmodifiedSkillLevel(skill_index) >= 15 &&
                       !player.newLevels.Contains(new Point(skill_index, 15)) &&
                       !player.newLevels.Contains(new Point(skill_index, 20)) &&
                       player.professions
                           .Intersect(((ISkill)skill).TierTwoProfessionIds)
                           .Count() > 1;
            return false; // don't run original logic;
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
