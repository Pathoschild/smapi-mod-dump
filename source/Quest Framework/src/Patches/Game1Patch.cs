/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using Harmony;
using PurrplingCore.Patching;
using QuestFramework.Extensions;
using QuestFramework.Framework;
using StardewValley;
using System;

namespace QuestFramework.Patches
{
    class Game1Patch : Patch<Game1Patch>
    {
        public override string Name => nameof(Game1Patch);

        QuestManager QuestManager { get; }
        QuestOfferManager ScheduleManager { get; }

        public Game1Patch(QuestManager questManager, QuestOfferManager scheduleManager)
        {
            this.QuestManager = questManager;
            this.ScheduleManager = scheduleManager;
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

        protected override void Apply(HarmonyInstance harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.CanAcceptDailyQuest)),
                postfix: new HarmonyMethod(typeof(Game1Patch), nameof(Game1Patch.After_CanAcceptDailyQuest))
            );
        }
    }
}
