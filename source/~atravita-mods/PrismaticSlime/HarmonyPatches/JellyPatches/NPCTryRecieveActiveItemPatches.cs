/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraCore.Framework.DialogueManagement;

using AtraShared.ConstantsAndEnums;

using HarmonyLib;

namespace PrismaticSlime.HarmonyPatches.JellyPatches;

[HarmonyPatch(typeof(NPC))]
internal static class NPCTryRecieveActiveItemPatches
{
    [HarmonyPatch(nameof(NPC.tryToReceiveActiveObject))]
    private static bool Prefix(NPC __instance, Farmer who)
    {
        if (Utility.IsNormalObjectAtParentSheetIndex(who.ActiveObject, ModEntry.PrismaticJelly)
            && !who.team.specialOrders.Any(order => order.questKey.Value == "Wizard2"))
        {
            try
            {
                QueuedDialogueManager.PushCurrentDialogueToQueue(__instance);
                switch (__instance.Name)
                {
                    case "Wizard":
                    {
                        BuffEnum buffEnum = BuffEnumExtensions.GetRandomBuff();
                        Buff buff = buffEnum.GetBuffOf(1, 700, "The Wizard's Gift", "The Wizard's Gift"); // TODO: localization.

                        Game1.buffsDisplay.addOtherBuff(buff);

                        __instance.doEmote(Character.exclamationEmote);
                        Dialogue item = new(I18n.PrismaticJelly_Wizard(), __instance)
                        {
                            onFinish = () => who.Money += 2000,
                        };
                        __instance.CurrentDialogue.Push(item);
                        break;
                    }
                    case "Gus":
                    {
                        Dialogue item = new(I18n.PrismaticJelly_Gus(), __instance)
                        {
                            onFinish = () =>
                            {
                                DelayedAction.functionAfterDelay(
                                    () => who.addItemByMenuIfNecessaryElseHoldUp(new SObject(ModEntry.PrismaticJellyToast, 1)),
                                    200);
                            },
                        };
                        __instance.CurrentDialogue.Push(item);
                        break;
                    }
                    default:
                    {
                        __instance.doEmote(Character.happyEmote);
                        who.changeFriendship(100, __instance);
                        if (!__instance.Dialogue.TryGetValue("PrismaticSlimeJelly.Response", out string? response))
                        {
                            response = I18n.PrismaticJelly_Response(__instance.displayName);
                        }
                        __instance.CurrentDialogue.Push(new Dialogue(response, __instance));
                        break;
                    }
                }
                who.reduceActiveItemByOne();
                Game1.drawDialogue(__instance);
                return false;
            }
            catch (Exception ex)
            {
                ModEntry.ModMonitor.Log($"Failed while trying to override NPC.{nameof(NPC.tryToReceiveActiveObject)}\n\n{ex}", LogLevel.Error);
            }
        }

        return true;
    }
}
