/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PurrplingCore.Patching;
using QuestFramework.Framework;
using QuestFramework.Framework.Helpers;
using QuestFramework.Offers;
using QuestFramework.Framework.Messages;
using StardewValley;
using System;
using System.Linq;
using QuestFramework.Framework.Controllers;

namespace QuestFramework.Patches
{
    class NPCPatch : Patch<NPCPatch>
    {
        public override string Name => nameof(NPCPatch);

        QuestManager QuestManager { get; }
        NpcOfferController OfferController { get; }

        public NPCPatch(QuestManager questManager, NpcOfferController offerController)
        {
            this.QuestManager = questManager;
            this.OfferController = offerController;
            Instance = this;
        }

        private static bool Before_checkAction(NPC __instance, Farmer who, ref bool __result)
        {
            try
            {
                if (Game1.eventUp || __instance.IsInvisible || __instance.isSleeping.Value || !who.CanMove || who.isRidingHorse())
                    return true;

                if (who.ActiveObject != null && who.ActiveObject.canBeGivenAsGift() && !who.isRidingHorse())
                    return true;

                if (Game1.dialogueUp)
                    return true;

                Instance.QuestManager.AdjustQuest(new TalkMessage(who, __instance));

                if (Game1.dialogueUp && Game1.currentSpeaker == __instance)
                {
                    __result = true;
                    return false;
                }

                Instance.Monitor.VerboseLog($"Checking for new quest from NPC `{__instance.Name}`.");

                if (Instance.OfferController.TryOfferNpcQuest(__instance, out QuestOffer<NpcOfferAttributes> offer))
                {
                    __result = true;
                    Game1.drawDialogue(__instance, $"{offer.OfferDetails.DialogueText}[quest:{offer.QuestName.Replace('@', ' ')}]");
                    QuestFrameworkMod.Instance.Monitor.Log($"Getting new quest `{offer.QuestName}` to quest log from NPC `{__instance.Name}`.");

                    return false;
                }

                if (Instance.OfferController.TryOfferNpcSpecialOrder(__instance, out SpecialOrder specialOrder))
                {
                    if (!__instance.Dialogue.TryGetValue($"offerOrder_{specialOrder.questKey.Value}", out string dialogue))
                    {
                        dialogue = specialOrder.GetDescription();
                    }

                    __result = true;
                    Game1.player.team.specialOrders.Add(SpecialOrder.GetSpecialOrder(specialOrder.questKey.Value, specialOrder.generationSeed.Value));
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2011"), 2));
                    Game1.drawDialogue(__instance, dialogue);
                    QuestFrameworkMod.Multiplayer.globalChatInfoMessage("AcceptedSpecialOrder", Game1.player.Name, specialOrder.GetName());
                    Instance.OfferController.RefreshActiveIndicators();

                    return false;
                }
            }
            catch (Exception e)
            {
                Instance.LogFailure(e, nameof(Instance.Before_checkAction));
            }

            return true;
        }

        public static void After_hasTemporaryMessageAvailable(NPC __instance, ref bool __result)
        {
            if (Instance.OfferController.TryOfferNpcQuest(__instance, out var _) || Instance.OfferController.TryOfferNpcSpecialOrder(__instance, out var _))
            {
                __result = true;
            }
        }

        protected override void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.checkAction)),
                prefix: new HarmonyMethod(typeof(NPCPatch), nameof(NPCPatch.Before_checkAction))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.hasTemporaryMessageAvailable)),
                postfix: new HarmonyMethod(typeof(NPCPatch), nameof(NPCPatch.After_hasTemporaryMessageAvailable))
            );
        }
    }
}
