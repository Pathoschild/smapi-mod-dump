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
using PurrplingCore.Patching;
using QuestFramework.Extensions;
using QuestFramework.Framework;
using QuestFramework.Framework.Controllers;
using StardewValley;
using System;

namespace QuestFramework.Patches
{
    class Game1Patch : Patch<Game1Patch>
    {
        public override string Name => nameof(Game1Patch);

        private QuestManager QuestManager { get; }
        private QuestOfferManager ScheduleManager { get; }
        private ItemOfferController ItemOfferController { get; }

        public Game1Patch(QuestManager questManager, QuestOfferManager offerManager, ItemOfferController itemOfferController)
        {
            this.QuestManager = questManager;
            this.ScheduleManager = offerManager;
            this.ItemOfferController = itemOfferController;
            Instance = this;
        }

        private static void After_CanAcceptDailyQuest(ref bool __result)
        {
            try
            {
                if (Game1.questOfTheDay != null && Game1.questOfTheDay.IsManaged() && Game1.player.hasQuest(Game1.questOfTheDay.id.Value))
                    __result = false;
            }
            catch (Exception e)
            {
                Instance.LogFailure(e, nameof(Game1Patch.After_CanAcceptDailyQuest));
            }
        }

        private static void After_checkItemFirstInventoryAdd(Item item)
        {
            Instance.ItemOfferController.CheckItemOffersQuest(item);
        }

        protected override void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.CanAcceptDailyQuest)),
                postfix: new HarmonyMethod(typeof(Game1Patch), nameof(Game1Patch.After_CanAcceptDailyQuest))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.checkItemFirstInventoryAdd)),
                postfix: new HarmonyMethod(typeof(Game1Patch), nameof(Game1Patch.After_checkItemFirstInventoryAdd))
            );
        }
    }
}
