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
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Patches
{
    class BillboardPatch : Patch<BillboardPatch>
    {
        public override string Name => nameof(BillboardPatch);

        public BillboardPatch()
        {
            Instance = this;
        }

        private static bool Before_receiveLeftClick(Billboard __instance, int x, int y)
        {
            try
            {
                if (!__instance.acceptQuestButton.visible || !__instance.acceptQuestButton.containsPoint(x, y))
                    return true;

                if (Game1.questOfTheDay == null || !Game1.questOfTheDay.IsManaged())
                    return true;

                if (Game1.player.hasQuest(Game1.questOfTheDay.id.Value))
                {
                    __instance.acceptQuestButton.visible = false;
                    return false;
                }

                var managedQuestOfTheDay = Game1.questOfTheDay.AsManagedQuest();

                Game1.playSound("newArtifact");
                Game1.questOfTheDay.accepted.Value = true;
                Game1.questOfTheDay.canBeCancelled.Value = managedQuestOfTheDay.Cancelable;
                Game1.questOfTheDay.dailyQuest.Value = managedQuestOfTheDay.IsDailyQuest();
                Game1.questOfTheDay.daysLeft.Value = managedQuestOfTheDay.DaysLeft;
                Game1.questOfTheDay.dayQuestAccepted.Value = Game1.Date.TotalDays;
                Game1.player.questLog.Add(Game1.questOfTheDay);
                Game1.player.acceptedDailyQuest.Set(managedQuestOfTheDay.IsDailyQuest());
                __instance.UpdateDailyQuestButton();

                return false;
            }
            catch (Exception e)
            {
                Instance.LogFailure(e, nameof(BillboardPatch.Before_receiveLeftClick));
                return true;
            }
        }

        protected override void Apply(HarmonyInstance harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Billboard), nameof(Billboard.receiveLeftClick)),
                prefix: new HarmonyMethod(typeof(BillboardPatch), nameof(BillboardPatch.Before_receiveLeftClick))
            );
        }
    }
}
