/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using StardewValley;
using StardewValley.Locations;

namespace LuckSkill.Overrides
{
    public static class ExperienceGainFix
    {
        public static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase original, IEnumerable<CodeInstruction> insns)
        {
            // TODO: Learn how to use ILGenerator
            
            int skipCounter = 3; // Skip the first three instructions, which just skip things if it is the luck skill
            var newInsns = new List<CodeInstruction>();
            foreach (var insn in insns)
            {
                if ( skipCounter > 0 )
                {
                    --skipCounter;
                    continue;
                }

                newInsns.Add(insn);
            }

            return newInsns;
        }
    }

    public static class OverpoweredGeodeFix
    {
        public static void Prefix(Farmer __instance, int which, ref int howMuch)
        {
            if ( which == Farmer.luckSkill && Game1.currentLocation is MineShaft ms)
            {
                bool foundGeode = false;
                var st = new StackTrace();
                foreach (var frame in st.GetFrames())
                {
                    if ( frame.GetMethod().Name.Contains("checkStoneForItems") )
                    {
                        foundGeode = true;
                        break;
                    }
                }

                if (foundGeode)
                {
                    int msa = ms.getMineArea(-1);
                    if (msa != 0)
                    {
                        howMuch /= msa;
                    }
                }
            }
        }
    }

    public static class FarmerGetProfessionHook
    {
        public static void Postfix(Farmer __instance, int skillType, int skillLevel, ref int __result)
        {
            if (skillType != Farmer.luckSkill)
                return;

            if (skillLevel == 5)
            {
                if (__instance.professions.Contains(Mod.PROFESSION_DAILY_LUCK))
                    __result = Mod.PROFESSION_DAILY_LUCK;
                else if (__instance.professions.Contains(Mod.PROFESSION_MORE_QUESTS))
                    __result = Mod.PROFESSION_MORE_QUESTS;
            }
            else if (skillLevel == 10)
            {
                if (__instance.professions.Contains(Mod.PROFESSION_DAILY_LUCK))
                {
                    if (__instance.professions.Contains(Mod.PROFESSION_CHANCE_MAX_LUCK))
                        __result = Mod.PROFESSION_CHANCE_MAX_LUCK;
                    else if (__instance.professions.Contains(Mod.PROFESSION_NO_BAD_LUCK))
                        __result = Mod.PROFESSION_NO_BAD_LUCK;
                }
                else if (__instance.professions.Contains(Mod.PROFESSION_MORE_QUESTS))
                {
                    if (__instance.professions.Contains(Mod.PROFESSION_NIGHTLY_EVENTS))
                        __result = Mod.PROFESSION_NIGHTLY_EVENTS;
                    else if (__instance.professions.Contains(Mod.PROFESSION_JUNIMO_HELP))
                        __result = Mod.PROFESSION_JUNIMO_HELP;
                }
            }
        }
    }
}
