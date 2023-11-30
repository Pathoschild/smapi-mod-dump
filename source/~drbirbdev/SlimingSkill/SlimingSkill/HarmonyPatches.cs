/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using BirbCore.Extensions;
using HarmonyLib;
using SpaceCore;
using StardewValley;
using StardewValley.Monsters;

namespace SlimingSkill;

/// <summary>
/// Gain XP from killing slimes
/// </summary>
[HarmonyPatch]
class Slime_TakeDamage_Transpiler
{
    static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(BigSlime), nameof(BigSlime.takeDamage), new System.Type[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(Farmer) });
        yield return AccessTools.Method(typeof(GreenSlime), nameof(GreenSlime.takeDamage), new System.Type[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(Farmer) });
    }

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (CodeInstruction instr in instructions)
        {
            if (instr.Is(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Stats), nameof(Stats.SlimesKilled))))
            {
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Slime_TakeDamage_Transpiler), nameof(DoDeathXp)));
            }
            yield return instr;
        }
    }

    public static void DoDeathXp(Monster monster)
    {
        int xp = ModEntry.Config.ExperienceFromSlimeKill;
        if (monster is BigSlime)
        {
            xp = ModEntry.Config.ExperienceFromToughSlimeKill;
        }
        else if (monster is GreenSlime slime)
        {
            if (slime.hasSpecialItem.Value || slime.prismatic.Value)
            {
                xp = ModEntry.Config.ExperienceFromRareSlimeKill;
            }
            else if (slime.Name == "Tiger Slime" || slime.Name == "Sludge")
            {
                xp = ModEntry.Config.ExperienceFromToughSlimeKill;
            }
        }
        // TODO: iron/lime/black slimes could be considered tough

        Skills.AddExperience(Game1.player, "drbirbdev.Sliming", xp);
    }
}

/// <summary>
/// Gain XP from incubating slime eggs
/// Rancher
///   - reduce incubation time
/// </summary>
[HarmonyPatch(typeof(Object), nameof(Object.performObjectDropInAction))]
class Object_PerformObjectDropInAction
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        int index = instructions.FindBCloseToA(new CodeInstruction(OpCodes.Ldstr, "coin"), new CodeInstruction(OpCodes.Ldc_I4, 4000));
        instructions = instructions.InsertAfterIndex(new CodeInstruction[]
        {
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Object_PerformObjectDropInAction), nameof(IncubateSlimeEgg))),
        }, index + 1);

        index = instructions.FindBCloseToA(new CodeInstruction(OpCodes.Ldstr, "slimeHit"), new CodeInstruction(OpCodes.Ldstr, "bubbles"));
        return instructions.InsertAfterIndex(new CodeInstruction[]
        {
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Object_PerformObjectDropInAction), nameof(PressSlimeEgg))),
        }, index);
    }

    public static void IncubateSlimeEgg(Object incubator)
    {
        Skills.AddExperience(Game1.player, "drbirbdev.Sliming", ModEntry.Config.ExperienceFromSlimeEgg);
        // TODO: Reduce incubation time;  Maybe undo vanilla (and MARGO?) professions decrease of incubation time;
    }

    public static void PressSlimeEgg(Object press)
    {
        Skills.AddExperience(Game1.player, "drbirbdev.Sliming", ModEntry.Config.ExperienceFromSlimeEgg);
    }
}

/// <summary>
/// Gain XP from opening a slime ball
/// Hatcher Profession
///   - gain extra drops
/// </summary>
[HarmonyPatch(typeof(Object), nameof(Object.checkForAction))]
class Object_CheckForAction
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (CodeInstruction instr in instructions)
        {
            if (instr.Is(OpCodes.Ldstr, "slimedead"))
            {

                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Object_CheckForAction), nameof(OpenSlimeball)));
            }
            yield return instr;
        }
    }

    public static void OpenSlimeball(Object slimeball)
    {
        Skills.AddExperience(Game1.player, "drbirbdev.Sliming", ModEntry.Config.ExperienceFromSlimeBall);
        // TODO: Hatcher profession bonuses
    }
}
