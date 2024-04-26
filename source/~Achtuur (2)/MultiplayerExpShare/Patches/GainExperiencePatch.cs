/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using System;
using System.Linq;

namespace MultiplayerExpShare.Patches;


public class GainExperiencePatch : BaseExpPatcher
{
    public override void Patch(Harmony harmony)
    {

        base.Patch(harmony);

        harmony.Patch(
            original: this.GetOriginalMethod<Farmer>(nameof(Farmer.gainExperience)),
            prefix: this.GetHarmonyMethod(nameof(Prefix_GainExperience))
        );

    }

    /// <summary>
    /// Calls <see cref="Farmer.gainExperience(int, int)"/> and sets <see cref="isProcessingSharedExp"/>. Use this method instead of <see cref="Farmer.gainExperience(int, int)"/> when working with shared exp
    /// </summary>
    /// <param name="farmer"></param>
    /// <param name="which"></param>
    /// <param name="howMuch"></param>
    /// <param name="isSharedExp"></param>
    public static void InvokeGainExperience(Farmer farmer, ExpGainData exp_data)
    {
        isProcessingSharedExp = true;

        int skill = AchtuurCore.Utility.Skills.GetSkillIdFromName(exp_data.skill_id);

        farmer.gainExperience(skill, exp_data.amount);
    }

    private static void Prefix_GainExperience(int which, ref int howMuch, Farmer __instance)
    {
        if (!CanExpBeShared())
            return;

        string skillName = AchtuurCore.Utility.Skills.GetSkillNameFromId(which);

        // Skip sharing if its disabled for that skill
        if (!ExpShareEnabledForSkill(which))
            return;

        // Get nearby farmer id's
        Farmer[] nearbyFarmers = ModEntry.GetNearbyPlayers()
            .Where(f => ModEntry.GetActorExpPercentage(f.GetSkillLevel(which), skillName) != 0f) // get all players that would actually receive exp
            .ToArray();

        // If no farmers nearby to share exp with, actor gets all
        if (nearbyFarmers.Length == 0)
            return;

        int level = __instance.GetSkillLevel(which);
        int actor_exp = GetActorExp(howMuch, level, skillName);
        // Calculate shared exp, with rounding
        int shared_exp = (int)Math.Round(howMuch * ModEntry.GetSharedExpPercentage(level, skillName) / nearbyFarmers.Length);

        // Send message of this instance of shared exp
        if (shared_exp > 0)
        {
            ModEntry.ShareExpWithFarmers(nearbyFarmers, skillName, shared_exp, "SharedExpGained");
        }

        AchtuurCore.Logger.DebugLog(ModEntry.Instance.Monitor, $"({Game1.player.Name}) is sharing exp with {nearbyFarmers.Length} farmer(s): {howMuch} -> {actor_exp} / {shared_exp}");

        howMuch = actor_exp;
    }

    /// <summary>
    /// Returns true if exp sharing is enabled for skill with skill_id <paramref name="skill_id"/>.
    /// 
    /// <para>Only works for vanilla Stardew skills, where id's are the same as in vanilla (0 = farming, 1 = fishing, 2 = foraging, 3 = mining, 4 = combat)</para>. Does not work with skill_id 5 (luck)
    /// </summary>
    /// <param name="skill_id"></param>
    /// <returns></returns>
    private static bool ExpShareEnabledForSkill(int skill_id)
    {
        if (skill_id < 0 || skill_id > 4)
            return false;

        return ModEntry.Instance.Config.VanillaSkillEnabled[skill_id];
    }
}
