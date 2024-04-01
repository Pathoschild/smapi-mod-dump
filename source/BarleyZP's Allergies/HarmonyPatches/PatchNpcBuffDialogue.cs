/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lisyce/SDV_Allergies_Mod
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace BZP_Allergies.HarmonyPatches
{
    [HarmonyPatch(typeof(NPC), nameof(NPC.checkAction))]
    internal class PatchNpcBuffDialogue : Initializable
    {
        [HarmonyPrefix]
        static bool CheckAction_Prefix(ref NPC __instance, ref Farmer who)
        {
            try
            {
                if (__instance.IsInvisible || __instance.isSleeping.Value || !who.CanMove)
                {
                    return true;  // run the original logic here
                }

                // is the farmer having a reaction?
                if (who.hasBuff(AllergenManager.ALLERIC_REACTION_DEBUFF))
                {
                    Dialogue? reactionDialogue = GetNpcAllergicReactionDialogue(__instance, who);
                    if (reactionDialogue != null && !ModEntry.NpcsThatReactedToday.Contains(__instance.Name))
                    {
                        __instance.CurrentDialogue.Push(reactionDialogue);
                        ModEntry.NpcsThatReactedToday.Add(__instance.Name);
                    }
                }

            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(CheckAction_Prefix)}:\n{ex}", LogLevel.Error);
            }

            return true;  // more dialogue can stack here; let the original method do its thing
        }

        private static Dialogue? GetNpcAllergicReactionDialogue(NPC npc, Farmer who)
        {
            if (who.isMarriedOrRoommates() && who.spouse == npc.Name)
            {
                Dialogue? marriageDialogue = npc.tryToGetMarriageSpecificDialogue(AllergenManager.REACTION_DIALOGUE_KEY);
                if (marriageDialogue != null)
                {
                    return marriageDialogue;
                }
            }
            
            return npc.TryGetDialogue(AllergenManager.REACTION_DIALOGUE_KEY);
        }
    }
}
