/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using HarmonyLib;
using StardewValley;
using System;
using System.Linq;
using System.Reflection;

namespace MultiplayerExpShare.Patches;

/// <summary>
/// Patch for <see cref="SpaceCore.Skills.AddExperience(Farmer, string, int)"/> to support exp sharing for SpaceCore based skills
/// </summary>
public class SpaceCoreExperiencePatch : BaseExpPatcher
{

    public override void Patch(Harmony harmony)
    {
        if (!ModEntry.Instance.Helper.ModRegistry.IsLoaded("spacechase0.SpaceCore"))
            return;

        base.Patch(harmony);

        Type spacecore_skills = AccessTools.TypeByName("SpaceCore.Skills");
        MethodInfo addExp_method = AccessTools.Method(spacecore_skills, "AddExperience");

        harmony.Patch(
            original: this.GetOriginalMethod(spacecore_skills, addExp_method.Name),
            prefix: this.GetHarmonyMethod(nameof(Prefix_AddExperienceSpaceCore))
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
        ModEntry.Instance.SpaceCoreAPI.AddExperienceForCustomSkill(farmer, exp_data.skill_id, exp_data.amount);
    }

    [HarmonyPriority(Priority.Last)]
    private static void Postfix_GainExperience()
    {
        if (isProcessingSharedExp)
        {
            isProcessingSharedExp = false;
        }
    }

    private static void Prefix_AddExperienceSpaceCore(Farmer farmer, string skillName, ref int amt)
    {
        if (!CanExpBeShared())
            return;

        // Skip sharing if its disabled for that skill
        if (!ExpShareEnabledForSkill(skillName))
            return;

        // Get nearby farmer id's
        Farmer[] nearbyFarmers = ModEntry.GetNearbyPlayers()
            .Where(f => ModEntry.GetActorExpPercentage(farmer, ModEntry.Instance.SpaceCoreAPI.GetLevelForCustomSkill(f, skillName), skillName) != 0f) // get all players that would actually receive exp
            .ToArray();

        // If no farmers nearby to share exp with, actor gets all
        if (nearbyFarmers.Length == 0)
            return;

        // calculate actor exp gain, with rounding
        int level = ModEntry.Instance.SpaceCoreAPI.GetLevelForCustomSkill(farmer, skillName);
        int actor_exp = GetActorExp(farmer, amt, level, skillName);

        // Calculate shared exp, with rounding
        int shared_exp = (int)Math.Round(amt * ModEntry.GetSharedExpPercentage(farmer, level, skillName) / nearbyFarmers.Length);

        // Send message of this instance of shared exp
        if (shared_exp > 0)
        {
            ModEntry.ShareExpWithFarmers(nearbyFarmers, skillName, shared_exp, "SharedExpGainedSpaceCore");
            //ExpGainDataSpaceCore expdata = new ExpGainDataSpaceCore(farmer.UniqueMultiplayerID, nearbyFarmers, skillName, shared_exp);
            //ModEntry.Instance.Helper.Multiplayer.SendMessage<ExpGainDataSpaceCore>(expdata, "SharedExpGainedSpaceCore", modIDs: new[] { ModEntry.Instance.ModManifest.UniqueID });
        }


        AchtuurCore.Logger.DebugLog(ModEntry.Instance.Monitor, $"({Game1.player.Name}) is sharing exp with {nearbyFarmers.Length} farmer(s) in {skillName}: {amt} -> {actor_exp} / {shared_exp}");

        // Set actor exp to howMuch, so rest of method functions as if it had gotten only actor_exp
        amt = actor_exp;
    }

    /// <summary>
    /// Returns true if sharing is enabled for this skill. Returns false if skill is not found
    /// </summary>
    /// <param name="skillName"></param>
    /// <returns></returns>
    private static bool ExpShareEnabledForSkill(string skillName)
    {
        var config = ModEntry.Instance.Config;
        return config.SpaceCoreSkillEnabled.ContainsKey(skillName) && config.SpaceCoreSkillEnabled[skillName];
    }
}
